using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

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

            var retval = base.Invoke(msg);

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