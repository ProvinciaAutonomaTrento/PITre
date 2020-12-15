using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Import.Template.Builders
{
    public abstract class TemplateBuilder
    {
        private static TemplateBuilderInstances _instances;

        static TemplateBuilder(){
            _instances=new TemplateBuilderInstances();
        }

        public static TemplateBuilderInstances Instances
        {
            get
            {
                return _instances;
            }
        }

        public abstract void AddTextPlaceholder(string placeholder,string text);

        public abstract void AddLinkPlaceholder(string placeholder,string label, string href);

        public abstract void AddTextPlaceholder(string placeholder,string[] texts,TextPosition position);

        public abstract byte[] ReplacePlaceholders(byte[] input);

        public abstract string NewLine();
    }

    public class TemplateBuilderInstances{

        public TemplateBuilder this[TemplateType type]{
           get{
               if (type == TemplateType.RTF) return new RtfBuilder();
               return null;
            }
        }
    }

    public enum TemplateType
    {
        RTF
    }

    public enum TextPosition{
        VERTICAL,HORIZONTAL
    }
}
