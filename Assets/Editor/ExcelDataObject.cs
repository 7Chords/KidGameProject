using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ExcelData", menuName = "Excel/Excel Data")]
public class ExcelDataObject : ScriptableObject
{
    [System.Serializable]
    public class DataEntry
    {
        public string Key;
        public string Value;
    }

    // 使用可序列化的列表代替字典，便于在Inspector中查看
    [SerializeField] private List<DataEntry> _entries = new List<DataEntry>();

    // 运行时使用的字典
    [System.NonSerialized] private Dictionary<string, string> _values;

    public Dictionary<string, string> Values
    {
        get
        {
            if (_values == null)
            {
                _values = new Dictionary<string, string>();
                foreach (DataEntry entry in _entries)
                {
                    _values[entry.Key] = entry.Value;
                }
            }
            return _values;
        }
        set
        {
            _values = value;
            _entries.Clear();
            foreach (var pair in value)
            {
                _entries.Add(new DataEntry { Key = pair.Key, Value = pair.Value });
            }
        }
    }
}