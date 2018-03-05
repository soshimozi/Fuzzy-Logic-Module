using FuzzyLib.AOP;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace FuzzyLib.Observables
{
    public class ObservableDynamicProxy<T> : RealProxy, IRemotingTypeInfo where T : MarshalByRefObject
    {
        private readonly T Decorated;

        protected ObservableDynamicProxy(T decorated) : base(typeof(IDynamicProxy<T>)) => Decorated = decorated;

        // We don't support marshaling objects wrapped in a 
        // Interposer, mainly because objects wrapped 
        // by this proxy may not be marshalable. That is they 
        // may not extend System.MarshalByRefObject. 
        public override ObjRef CreateObjRef(Type requestedType) =>
            throw new NotSupportedException("Remoting of an ObservableDynamicProxy object is not supported.");


        // Allow a cast to IDynamicProxy<T> or to any type supported by the 
        // underlying target object. 
        public bool CanCastTo(Type type, object o) => type == typeof(IDynamicProxy<T>) || type.IsAssignableFrom(typeof(T));

        public string TypeName
        {
            get => throw new NotSupportedException(); set => throw new NotSupportedException();
        }

        // Standard Marshal method that returns a TransparentProxy instance 
        // that can act as ANY interface. 
        public static T Marshal(T target)
        {
            var proxy = new ObservableDynamicProxy<T>(target);
            return (T)proxy.GetTransparentProxy();
        }

        public static T Create(params object[] args)
        {
            var wrapped = (T)Activator.CreateInstance(typeof(T), args);
            return Marshal(wrapped);
            //var interceptor = new ObservableDynamicProxy<T>(wrapped);
            //return (T)interceptor.GetTransparentProxy();
        }


        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall != null ? methodCall.MethodBase as MethodInfo : null;

            Exception ex = null;
            if (!TryGetResult(methodCall, methodInfo, ref ex, out object result)) return new ReturnMessage(ex, methodCall);

            var  retval = new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);

            if (IsSet(methodInfo) && IsObservable(methodInfo))
            {
                // notify this guy
                var notifier = typeof(T).GetMethod("OnPropertyChanged",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                if (notifier != null)
                {
                    notifier.Invoke(Decorated, new object[] { methodInfo.Name.Substring(4) });
                }
            }

            return retval;
        }

        private bool TryGetResult(IMethodCallMessage methodCall, MethodInfo methodInfo, ref Exception ex, out object result)
        {
            try
            {
                result = methodInfo.Invoke(Decorated, methodCall.InArgs);
                return true;
            }
            catch(Exception err)
            {
                ex = err;
                result = null;

                return false;
            }
        }

        private bool IsObservable(MethodInfo methodInfo)
        {
            var objectType = typeof(T);
            var propertyName = methodInfo.Name.Substring(4);

            var propInfo = objectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

            if (propInfo == null) return false;
            if (propInfo.GetCustomAttributes(false).All(c => c.GetType() != typeof(ObservableAttribute)))
                return false;

            return true;
        }

        private bool IsSet(MethodInfo methodInfo)
        {
            return methodInfo != null &&
                                (methodInfo.DeclaringType != null && (methodInfo.DeclaringType.GetProperties()
                                    .Any(prop => prop.GetSetMethod() == methodInfo)));
        }
    }
}