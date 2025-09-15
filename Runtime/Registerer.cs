using System.Collections.Generic;

namespace com.fscigliano.CommonExtensions
{
    public abstract class Registerer<TId, TObj>
    {
        private Dictionary<TId, TObj> _registered = new();
        private List<TObj> _registeredList = new();

        public void Register(TId id, TObj o)
        {
            _registered.Add(id, o);
            _registeredList.Add(o);
        }

        public bool Contains(TId id)
        {
            return _registered.ContainsKey(id);
        }

        public TObj Unregister(TId id)
        {
            var o = _registered[id];
            _registered.Remove(id);
            _registeredList.Remove(o);
            return o;
        }

        public TObj Get(TId id)
        {
            return _registered[id];
        }

        public void Clear(List<TObj> content = null)
        {
            if (content != null)
            {
                for (int i = 0; i < _registeredList.Count; i++)
                {
                    content.Add(_registeredList[i]);
                }
            }

            _registered.Clear();
            _registeredList.Clear();
        }
    }
}