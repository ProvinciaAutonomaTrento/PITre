using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Documenti.DigitalSignature.PKCS_Utils
{
	public class CryptoFile
	{
        private Byte[] _content;
        public Byte[] Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public fileType MessageFileType;
        public String Name;

        public String Base64Content
        {
            get
            {
                return Convert.ToBase64String(this._content);
             }
            set
            {
                _content = Convert.FromBase64String(value);
            }
        }

        public static fileType mapType(string type)
        {
            if (type.ToLower().Contains("binary")) return fileType.Binary;
            if (type.ToLower().Contains("base64")) return fileType.Base64;
            if (type.ToLower().Contains("utf8")) return fileType.UTF8;
            if (type.ToLower().Contains("utf7")) return fileType.UTF8;
            return fileType.Unknown;
        }
        public static string splitInLines(string text, int chunkSize)
        {
            StringBuilder sb = new StringBuilder();
            int offset = 0;
            while (offset < text.Length)
            {
                int size = Math.Min(chunkSize, text.Length - offset);
                sb.AppendLine (text.Substring(offset, size));
                offset += size;
            }
            return sb.ToString();
        }
	}


    public enum fileType
    {
        Unknown,
        Binary,
        Base64,
        UTF8,
        UTF7
    }
}
