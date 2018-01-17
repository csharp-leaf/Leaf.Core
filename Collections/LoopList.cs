using System;
using System.Collections.Generic;

namespace Leaf.Core.Collections
{
    /// <summary>
    /// Потокобезопасный список, который способен поочередно выдавать элементы, зацикленно
    /// </summary>
    public class LoopList
    {
        private readonly List<string> _list;
        private readonly Random _random = new Random();
        private int _index;

        public LoopList(List<string> list)
        {
            _list = list;
        }

        public string GetNext()
        {
            lock (_list)
            {
                if (_list.Count <= 0)
                    return null;

                if (_index >= _list.Count)
                    _index = 0;

                return _list[_index++];
            }
        }

        public string GetNextRandom() {
            lock (_list)
            {
                if (_list.Count <= 0)
                    return null;

                // иначе возвращаем случайный акк
                int random = _random.Next(_list.Count - 1);
                string account = _list[random];
                //_list.RemoveAt(random);
                return account;
            }
        }

        public void Clear() {
            lock (_list)
            {
                _list.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_list)
                {
                    return _list.Count;
                }
            }
        }

        public void Remove(string proxy) {
            lock (_list)
            {
                _list.Remove(proxy);
            }
        }
    }
}
