using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace AspectOrientedProgramming
{
    public class DynamicProxy<T> : RealProxy, IRemotingTypeInfo
    {
        protected readonly T Decorated;
        private readonly bool _useLogging;

        // Standard Marshal method that returns a TransparentProxy instance 
        // that can act as ANY interface. 
        public static T Marshal(T target, bool useLogging)
        {
            var proxy = new DynamicProxy<T>(target, useLogging);
            return (T)proxy.GetTransparentProxy();
        }

        public static IDynamicProxy<T> MarshalProxy(T target, bool useLogging)
        {
            var proxy = new DynamicProxy<T>(target, useLogging);
            return (IDynamicProxy<T>)proxy.GetTransparentProxy();
        }

        // The basic constructor initializes the RealProxy with the 
        // IInterposed interface type and saves away the target instance. 

        protected DynamicProxy(T decorated, bool useLogging)
            : base(typeof(IDynamicProxy<T>))
        {
            Decorated = decorated;
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
// ReSharper disable PossibleNullReferenceException
                var result = methodInfo.Invoke(Decorated, methodCall.InArgs);
// ReSharper restore PossibleNullReferenceException
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
// ReSharper disable PossibleNullReferenceException
                methodCall.MethodName);
// ReSharper restore PossibleNullReferenceException
            try
            {
// ReSharper disable once PossibleNullReferenceException
                var result = methodInfo.Invoke(Decorated, methodCall.InArgs);
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