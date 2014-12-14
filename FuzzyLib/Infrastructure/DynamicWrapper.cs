using System.Collections.Generic;
using System.Dynamic;

namespace FuzzyLib.Infrastructure
{
    public class DynamicWrapper<TWrapped> : DynamicObject
    {
        private readonly Dictionary<string, TWrapped> _wrapped;

        public DynamicWrapper(Dictionary<string, TWrapped> wrapped)
        {
            _wrapped = wrapped;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;

            TWrapped wrapped;
            var retval = _wrapped.TryGetValue(name, out wrapped);

            result = wrapped;
            return retval;
        }
    }
}
