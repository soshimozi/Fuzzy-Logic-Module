using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using TestFuzzyLib.Annotations;

namespace TestFuzzyLib
{
    public class ObservableDynamicProxy<T> : DynamicProxy<T> where T : ObservableObject<T>
    {
        protected ObservableDynamicProxy(T decorated, bool useLogging) : base(decorated, useLogging)
        {
        }

        // Standard Marshal method that returns a TransparentProxy instance 
        // that can act as ANY interface. 
        public new static T Marshal(T target, bool useLogging)
        {
            var proxy = new ObservableDynamicProxy<T>(target, useLogging);
            return (T)proxy.GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall != null ? methodCall.MethodBase as MethodInfo : null;

            var isSetAccessor = methodInfo != null &&
                                (methodInfo.DeclaringType != null && (methodInfo.DeclaringType.GetProperties()
                                    .Any(prop => prop.GetSetMethod() == methodInfo)));

            if (!isSetAccessor) return base.Invoke(msg);

            var objectType = typeof(T);
            var propertyName = methodInfo.Name.Substring(4);

            var propInfo = objectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

            if (propInfo == null) return base.Invoke(msg);
            if (propInfo.GetCustomAttributes(false).All(c => c.GetType() != typeof(ObservableAttribute)))
                return base.Invoke(msg);

            // notify this guy
            var notifier = objectType.GetMethod("OnPropertyChanged",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            if (notifier != null)
            {
                notifier.Invoke(_decorated, new object[] { propertyName });
            }

            return base.Invoke(msg);
        }

    }
    
    public class DynamicProxy<T> : RealProxy, IRemotingTypeInfo
    {
        protected readonly T _decorated;
        private bool _useLogging;

        // Standard Marshal method that returns a TransparentProxy instance 
        // that can act as ANY interface. 
        public static T Marshal(T target, bool useLogging)
        {
            var proxy = new DynamicProxy<T>(target, useLogging);
            return (T)proxy.GetTransparentProxy();
        }

        // The basic constructor initializes the RealProxy with the 
        // IInterposed interface type and saves away the target instance. 

        protected DynamicProxy(T decorated, bool useLogging)
            : base(typeof(IDynamicProxy<T>))
        {
            _decorated = decorated;
            _useLogging = useLogging;
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

        private void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }

        public override IMessage Invoke(IMessage msg)
        {
            return _useLogging ? InvokeWithLogging(msg) : InvokeWithoutLogging(msg);
        }

        private IMessage InvokeWithoutLogging(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall != null ? methodCall.MethodBase as MethodInfo : null;

            try
            {
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
                return new ReturnMessage(result, null, 0,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                return new ReturnMessage(e, methodCall);
            }
        }

        private IMessage InvokeWithLogging(IMessage msg)
        {

            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall != null ? methodCall.MethodBase as MethodInfo : null;

            Log("In Dynamic Proxy - Before executing '{0}'",
                methodCall.MethodName);
            try
            {
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
                Log("In Dynamic Proxy - After executing '{0}' ",
                    methodCall.MethodName);
                return new ReturnMessage(result, null, 0,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                Log(string.Format(
                    "In Dynamic Proxy- Exception {0} executing '{1}'", e,
                    methodCall.MethodName));
                return new ReturnMessage(e, methodCall);
            }
        }
    }
}