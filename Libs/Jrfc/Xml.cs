using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrfc
{
    [Serializable]
    public class XmlKeyValuePair
    {
        public string Key;
        public string XmlString;

        public XmlKeyValuePair()
        {
            this.Key = string.Empty;
            this.XmlString = string.Empty;
        }

        public XmlKeyValuePair(string _Key, string _XmlString)
        {
            this.Key = _Key;
            this.XmlString = _XmlString;
        }
    }

    [Serializable]
    public class XmlDictionary : List<XmlKeyValuePair>
    {
        public XmlDictionary() { }

        public void Add(string _Key, string _XmlString)
        {
            XmlKeyValuePair kvp = new XmlKeyValuePair(_Key, _XmlString);
            base.Add(kvp);
        }
        public string this[string _Key]
        {
            get
            {
                string key_lc = _Key.ToLower();
                foreach (XmlKeyValuePair kvp in this)
                {
                    if (kvp.Key.ToLower() == key_lc)
                    {
                        return (kvp.XmlString);
                    }
                }
                return null;
            }
        }

        public string ToXmlString()
        {
            return Jrfc.Utility.ObjectSerializer<XmlDictionary>.ToXml(this);
        }

    }
}
