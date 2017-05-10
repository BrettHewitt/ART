/*Automated Rodent Tracker - A program to automatically track rodents
Copyright(C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

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
