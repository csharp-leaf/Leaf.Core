using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections.FileBased
{
    /// <summary>
    /// ���������������� ������ �����-����������, �������� �� �����.
    /// </summary>
    public class FileMaterialsList : FileMaterialsBase
    {
        private readonly MaterialsStorageList<string> _materialsStorage = new MaterialsStorageList<string>();

        /// <inheritdoc />
        /// <summary>
        /// ������ ���������������� ������ �����-���������� �� �����.
        /// </summary>
        public FileMaterialsList(string fileName, bool includeComments = false) : base(fileName, includeComments)
        {
            MaterialsStorage = _materialsStorage;
            BeforeReadFile += ResetPointer;
            ReadFromFile();
        }

        /// <summary>
        /// ���������� ��������� ��������� �������� �� ������.
        /// </summary>
        public string GetNextRandom()
        {
            if (MaterialsStorage == null)
                return null;

            lock (MaterialsStorage)
                return _materialsStorage.GetNextRandom();
        }

        /// <summary>
        /// ������������� ��� ������������ ��������� ������.
        /// </summary>
        public MaterialsListIteration Iteration
        {
            get {
                lock (MaterialsStorage)
                    return _materialsStorage.Iteration;
            }
            set {
                lock (MaterialsStorage)
                    _materialsStorage.Iteration = value;
            }            
        }

        /// <summary>
        /// ���������� ��������� �������� �������� ������ �� ������ (������� ������).
        /// </summary>
        public void ResetPointer()
        {
            if (MaterialsStorage == null)
                return;
            
            lock (MaterialsStorage)
                _materialsStorage.ResetPointer();
        }

        /// <summary>
        /// ��������� ������������� �������� � ������.
        /// </summary>
        /// <param name="item">�������</param>
        /// <returns>���������� ������, ���� ������� ��� ������ � ������.</returns>
        public bool Contains(string item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return _materialsStorage.Contains(item);
        }

        /// <summary>
        /// ������� ������� �� ������.
        /// </summary>
        /// <param name="item">�������</param>
        /// <returns>���������� ������, ���� ������� ��� ������ � �����.</returns>
        public bool Remove(string item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return _materialsStorage.Remove(item);            
        }
    }
}
