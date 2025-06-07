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

    // ʹ�ÿ����л����б�����ֵ䣬������Inspector�в鿴
    [SerializeField] private List<DataEntry> _entries = new List<DataEntry>();

    // ����ʱʹ�õ��ֵ�
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