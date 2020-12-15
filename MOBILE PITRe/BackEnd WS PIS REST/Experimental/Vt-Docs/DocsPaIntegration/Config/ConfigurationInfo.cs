using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace DocsPaIntegration.Config
{
    [Serializable]
    public class ConfigurationInfo
    {
        private List<ConfigurationParam> _configurationParams;
        private string _adapterUniqueId;
        private Version _adapterVersion;

        public ConfigurationInfo(string adapterUniqueId,Version adapterVersion)
        {
            this._adapterUniqueId = adapterUniqueId;
            this._adapterVersion = adapterVersion;
            this._configurationParams = new List<ConfigurationParam>();
        }

        public ConfigurationInfo()
        {
            this._configurationParams = new List<ConfigurationParam>();
        }


        public List<ConfigurationParam> ConfigurationParams
        {
            get
            {
                return _configurationParams;
            }
        }

        public string AdapterUniqueId
        {
            get
            {
                return _adapterUniqueId;
            }
            set
            {
                this._adapterUniqueId = value;
            }
        }

        [XmlElement("AdapterVersion")]
        public string AdapterVersionAsString{
            get
            {
                if (AdapterVersion != null) return AdapterVersion.ToString();
                return null;
            }
            set
            {
                if(!string.IsNullOrEmpty(value)){
                    AdapterVersion=new Version(value);
                }
            }
        }

        [XmlIgnore]
        public Version AdapterVersion
        {
            get
            {
                return _adapterVersion;
            }
            set
            {
                this._adapterVersion = value;
            }
        }

        public ConfigurationParam this[string name]
        {
            get{
                foreach (ConfigurationParam temp in _configurationParams)
                {
                    if (temp.Name.Equals(name)) return temp;
                }
                return null;
            }
        }

        [XmlIgnore]
        public string Value{
            get{
                string res = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(this.GetType());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, this);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                res = UTF8ByteArrayToString(memoryStream.ToArray());
                return res;
            }
            set{
                XmlSerializer xs = new XmlSerializer(this.GetType());
                MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(value));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                ConfigurationInfo temp=(ConfigurationInfo) xs.Deserialize(memoryStream);
                this._configurationParams = temp._configurationParams;
                this._adapterUniqueId = temp._adapterUniqueId;
                this._adapterVersion = temp._adapterVersion;
            }
        }

        private String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters).Substring(1);
            return constructedString;

        }

        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {

            UTF8Encoding encoding = new UTF8Encoding();

            Byte[] byteArray = encoding.GetBytes(pXmlString);

            return byteArray;

        } 
    }
}
