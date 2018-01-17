using System;
using System.Collections.Generic;
using System.IO;

namespace Leaf.Core.Collections
{
    // TODO: Genetic
    /// <summary>
    /// Потокобезопасный список, читающий из файла
    /// </summary>
    public class ListFromFile // : IDisposable
    {
        private readonly List<string> _list = new List<string>(); // очередь с аккаунтами в формате string: login;password
        private readonly string _filename;
        private readonly Random _random = new Random();

        /// <summary>
        /// Создаёт потокобезопасный список из файла.
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public ListFromFile(string filename)
        {
            _filename = filename;
            ReloadElements();
        }

        /// <summary>
        /// Загрузка аккаунтов из файла, "//" и "#" служат коментариями
        /// </summary>
        /// <param name="includeComments">Если true, то коментарии тоже будут включены в выборку</param>
        /// <returns></returns>
        public bool ReloadElements(bool includeComments = false)
        {
            lock (_list)                
            {
                try
                {
                    if (!File.Exists(_filename))
                        return false;

                    if (_list.Count > 0)
                        _list.Clear();

                    using (StreamReader file = new StreamReader(_filename))
                    {
                        string line;
                        while (!string.IsNullOrWhiteSpace((line = file.ReadLine())))
                        {
                            if (!line.StartsWith("//") && !line.StartsWith("#") || includeComments)
                                _list.Add(line);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Возвращает следующий аккаунт из очереди, если она пуста будет возвращён null.
        /// </summary>
        public string NextRandom {
            get {
                lock (_list) {                    
                    if (_list.Count <= 0)
                        return null;

                    // иначе возвращаем случайный акк
                    int random = _random.Next(_list.Count - 1);
                    string account = _list[random];
                    _list.RemoveAt(random);
                    return account;                    
                }                                    
            }
        }

        /// <summary>
        /// Количество аккаунтов в очереди в данный момент.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Очиска содержимого списка, прочитанного из списка.
        /// </summary>
        public void Clear() {
            lock (_list)
            {
                _list.Clear();
            }            
        }
    }
}
