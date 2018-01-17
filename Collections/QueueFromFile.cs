using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Leaf.Core.Collections
{
    // TODO: Genetic

    /// <summary>
    /// Потокобезопасная очередь, читаемая из файла.
    /// </summary>
    public class QueueFromFile
    {
        private readonly Queue<string> _queue = new Queue<string>();
        private readonly string _filename;

        /// <summary>
        /// Общее число элементов в очереди из указанного файла.
        /// </summary>
        public int TotalCount { get; private set; } // всего записей при старте

        /// <summary>
        /// Создаёт очередь сторок из файла.
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public QueueFromFile(string filename)
        {
            _filename = filename;
            ReloadElements();
        }

        /// <summary>
        /// Загрузка аккаунтов из файла.
        /// </summary>
        public bool ReloadElements()
        {
            lock (_queue)                
            {
                try
                {
                    if (!File.Exists(_filename))
                        return false;

                    if (_queue.Count > 0)
                        _queue.Clear();

                    using (var file = new StreamReader(_filename))
                    {
                        string line;
                        int counter = 0;
                        while (!string.IsNullOrWhiteSpace((line = file.ReadLine())))
                        {
                            if (line.StartsWith("//") || line.StartsWith("#")) 
                                continue;

                            ++counter;
                            _queue.Enqueue(line.Trim());
                        }
                        TotalCount = counter;
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
        public string GetNext() {
            lock (_queue) {
                return _queue.Count > 0 ? _queue.Dequeue() : null;
            }
        }

        /// <summary>
        /// Возвращает следующий элемент из очереди и записывает переменную.
        /// </summary>
        public bool NextInQueue(out string outStr)
        {
            outStr = GetNext();
            return outStr != null;
        }


        public void Enqeue(string arg) {
            if (string.IsNullOrWhiteSpace(arg))
                return;

            lock (_queue) {
                _queue.Enqueue(arg.Trim());
            }
        }

        /// <summary>
        /// Количество аккаунтов в очереди в данный момент.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_queue)
                    return _queue.Count;
            }
        }

        public void Clear()
        {
            lock (_queue)
                _queue.Clear();
        }

        public string DeqeueAll()
        {
            if (Count <= 0)
                return string.Empty;

            var sb = new StringBuilder();

            while (NextInQueue(out string item))
            {
                if (!string.IsNullOrEmpty(item))
                    sb.AppendLine(item);
            }

            return sb.ToString();
        }
    }
}
