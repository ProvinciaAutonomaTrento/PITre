using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DocsPaIntegration.ObjectTypes
{
    [Serializable]
    public class FileType
    {
        public string Filename
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public string Encode()
        {
            MemoryStream ws = new MemoryStream();
            BinaryFormatter sf = new BinaryFormatter();
            sf.Serialize(ws, this);
            byte[] bytes = ws.GetBuffer();
            string res=Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
            return res;
        }

        public static FileType Decode(string encoded)
        {
            byte[] memorydata = Convert.FromBase64String(encoded);
            MemoryStream rs = new MemoryStream(memorydata, 0, memorydata.Length);
            BinaryFormatter sf = new BinaryFormatter();
            FileType res=(FileType) sf.Deserialize(rs);
            return res;
        }
    }
}
