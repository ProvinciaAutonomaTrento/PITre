using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.ObjectTypes.Attributes
{

    public class IntegrationImageTypeAttribute : IntegrationObjectTypeAttribute
    {
        public IntegrationImageTypeAttribute(string name, bool mandatory) : base(name,mandatory)
        {

        }

        public override ObjectType Type
        {
            get {
                return ObjectType.IMAGE;
            }
        }

        public int MaxSize
        {
            get;
            set;
        }

        public override object GetValue(string value)
        {
            FileType res = FileType.Decode(value);
            if (this.MaxSize>0 && res.Content.Length > this.MaxSize*1000) throw new ObjectValidationException("La dimensione del file è maggiore di "+MaxSize+" kBytes");
            string[] split = res.Filename.Split(new char[] { '.' });
            string ext = split[split.Length - 1];
            if (!CheckExt(ext))
            {
                throw new ObjectValidationException("Il formato del file non è valido");
            }
            return res;
        }

        private bool CheckExt(string ext)
        {
            string[] exts = new string[] { "GIF", "BMP", "JPG" };
            foreach (string temp in exts)
            {
                if (temp.Equals(ext.ToUpper())) return true;
            }
            return false;
        }
    }
}
