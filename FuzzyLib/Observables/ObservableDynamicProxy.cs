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

        protected ObservableDynamicProxy(T decorated) : base(typeof(IDynamicProxy<T>)) // : base(decorated, useLogging)
        {
            Decorated = decorated;
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            // We don't support marshaling objects wrapped in a 
            // Interposer, mainly because objects wrapped 
            // by this proxy may not be marshalable. That is they 
            // may not extend System.MarshalByRefObject. 
            throw new NotSupportedException("Remoting of an ObservableDynamicProxy object is not supported.");
        }

        public bool CanCastTo(Type type, object o)
        {
            // Allow a cast to IDynamicProxy<T> or to any type supported by the 
            // underlying target object. 
            return type == typeof(IDynamicProxy<T>) || type.IsAssignableFrom(typeof(T));
        }

        public string TypeName
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        //// Standard Marshal method that returns a TransparentProxy instance 
        //// that can act as ANY interface. 
        //public new static T Marshal(T target, bool useLogging)
        //{
        //    var proxy = new ObservableDynamicProxy<T>(target, useLogging);
        //    return (T)proxy.GetTransparentProxy();
        //}

        public static T Create(params object[] args)
        {
            var wrapped = (T)Activator.CreateInstance(typeof(T), args);
            var interceptor = new ObservableDynamicProxy<T>(wrapped);
            return (T)interceptor.GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall != null ? methodCall.MethodBase as MethodInfo : null;

            IMessage retval;
            try
            {
                // ReSharper disable PossibleNullReferenceException
                var result = methodInfo.Invoke(Decorated, methodCall.InArgs);
                // ReSharper restore PossibleNullReferenceException
                retval = new ReturnMessage(result, null, 0,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                return new ReturnMessage(e, methodCall);
            }

            var isSetAccessor = methodInfo != null &&
                                (methodInfo.DeclaringType != null && (methodInfo.DeclaringType.GetProperties()
                                    .Any(prop => prop.GetSetMethod() == methodInfo)));

            if (!isSetAccessor) return retval;

            var objectType = typeof(T);
            var propertyName = methodInfo.Name.Substring(4);

            var propInfo = objectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

            if (propInfo == null) return retval;
            if (propInfo.GetCustomAttributes(false).All(c => c.GetType() != typeof(ObservableAttribute)))
                return retval;

            // notify this guy
            var notifier = objectType.GetMethod("OnPropertyChanged",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            if (notifier != null)
            {
                notifier.Invoke(Decorated, new object[] { propertyName });
            }

            return retval;
        }
    }
}