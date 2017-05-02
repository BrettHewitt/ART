using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class DictionaryXml<TKey, TValue>
    {
        [XmlArray(ElementName = "Keys")]
        [XmlArrayItem(ElementName = "Key")]
        public TKey[] Keys
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Values")]
        [XmlArrayItem(ElementName = "Value")]
        public TValue[] Values
        {
            get;
            set;
        }

        public DictionaryXml()
        {
            
        } 

        public DictionaryXml(TKey[] keys, TValue[] values)
        {
            Keys = keys;
            Values = values;
        } 

        public Dictionary<TKey, TValue> GetData()
        {
            int count1 = Keys.Length;
            int count2 = Values.Length;
            if (count1 != count2)
            {
                return null;
            }

            Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

            for (int i = 0; i < count1; i++)
            {
                data.Add(Keys[i], Values[i]);
            }

            return data;
        } 
    }
}
