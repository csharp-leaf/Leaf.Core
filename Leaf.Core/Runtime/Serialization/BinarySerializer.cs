using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Runtime.Serialization
{
    /// <summary>
    /// Класс для сериализации и десериализации различных объектов. Хранилищем выступает бинарный файл.
    /// Поддержка асинхронных методов.
    /// </summary>
    public static class BinarySerializer
    {
        private static readonly BinaryFormatter Bf = new BinaryFormatter();

        /// <summary>
        /// Сохраняет объект в бинарный файл.
        /// </summary>
        /// <param name="source">объект сериализации</param>
        /// <param name="filePath">относительный путь до файла</param>
        /// <param name="overwrite">разрешить перезапись бинарного файла</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException"></exception>
        /// <exception cref="ArgumentException">Возникает если перезапись отключена и файл для сериализации уже существует.</exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        public static void Serialize(object source, string filePath, bool overwrite = true)
        {
            if (!overwrite && File.Exists(filePath))
                throw new ArgumentException($"Указанный файл сериализации '${filePath}' уже существует", nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                Bf.Serialize(fs, source);
        }

        /// <summary>
        /// Восстанавливает объект из бинарного файла.
        /// </summary>
        /// <param name="filePath">относительный путь до файла</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException"></exception>
        /// <exception cref="ArgumentException">Возникает если файл десериализации не существует.</exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        /// <returns>Десериализованный объект из файла</returns>
        public static T Deserialize<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException($"Файл для десериализации '${filePath}' не существует", nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open))
                return (T)Bf.Deserialize(fs);
        }

        /// <summary>
        /// Асинхронно сохраняет объект в бинарный файл.
        /// </summary>
        /// <param name="source">объект сериализации</param>
        /// <param name="filePath">относительный путь до файла</param>
        /// <param name="overwrite">разрешить перезапись бинарного файла</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException"></exception>
        /// <exception cref="ArgumentException">Возникает если перезапись отключена и файл для сериализации уже существует.</exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        public static async Task SerializeAsync(object source, string filePath, bool overwrite = true)
        {
            await Task.Run(() => Serialize(source, filePath, overwrite));
        }

        /// <summary>
        /// Асинхронно восстанавливает объект из бинарного файла.
        /// </summary>
        /// <param name="filePath">относительный путь до файла</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException"></exception>
        /// <exception cref="ArgumentException">Возникает если файл десериализации не существует.</exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        /// <returns>Десериализованный объект из файла</returns>
        public static async Task<T> DeserializeAsync<T>(string filePath)
        {
            return await Task.Run(() => Deserialize<T>(filePath));    
        }
    }
}
