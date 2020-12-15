using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLogic.Import.Template.Builders
{
    public class RtfBuilder : TemplateBuilder
    {
        private Dictionary<string, string> _placeholders;
        private int MAX_CHECK_LENGTH = 1000;

        public RtfBuilder()
        {
            _placeholders = new Dictionary<string, string>();
        }

        public override void AddTextPlaceholder(string placeholder, string text)
        {
            if (!_placeholders.ContainsKey(formatPlaceholder(placeholder).ToUpper())) _placeholders.Add(formatPlaceholder(placeholder).ToUpper(), formatValue(text));
        }

        public override void AddLinkPlaceholder(string placeholder, string label, string href)
        {
            string placeholderName=formatPlaceholder(placeholder).ToUpper();
            if(!_placeholders.ContainsKey(placeholderName)) _placeholders.Add(placeholderName,"{\\field{\\*\\fldinst{HYPERLINK \""+href+"\"}}{\\fldrslt {"+formatValue(label)+"}}}");
        }

        public override string NewLine()
        {
            return " \\par ";
        }

        public override void AddTextPlaceholder(string placeholder, string[] texts, TextPosition position)
        {
            string separator = " ";
            if (position == TextPosition.VERTICAL) separator = NewLine();
            string res = formatValue(texts[0]);
            for (int i = 1; i < texts.Length; i++)
            {
                res += separator + formatValue(texts[i]);
            }
            string placeholderName = formatPlaceholder(placeholder).ToUpper();
            if (!_placeholders.ContainsKey(placeholderName)) _placeholders.Add(placeholderName, res);
        }

        private string formatPlaceholder(string placeholder)
        {
            return "#" + placeholder + "#";
        }

        private string formatValue(string value)
        {
            return value.Replace("\\", "\\'5C").Replace("\\'5Cpar","\\par");
        }

        private int MaxPlaceholderLength
        {
            get
            {
                return _placeholders.Keys.OrderBy(temp => temp.Length).Last().Length;
            }
        }

        public override byte[] ReplacePlaceholders(byte[] input)
        {
            List<byte> res = new List<byte>();
            for (int i = 0; i < input.Length; i++)
            {
                char temp = (char)input[i];
                if (temp == '#')
                {
                    i = HandlePlaceholder(i, input, res);
                }
                else
                {
                    res.Add(input[i]);
                }
            }
            return res.ToArray();
        }

        private int HandlePlaceholder(int index,byte[] input, List<byte> output){
            string placeholder = "#";
            int j = 0;
            bool endFound = false;
            while(j<MAX_CHECK_LENGTH && !endFound){
                j++;
                placeholder = placeholder + (char)input[index+j];
                if (input[index + j] == '#') endFound = true;
            }
            string value = placeholder;
            string key = GetTextFromRtf(placeholder).ToUpper();
            if (_placeholders.ContainsKey(key))
            {
                value = _placeholders[key];
            }
            foreach (char temp in value.ToCharArray())
            {
                output.Add((byte)temp);
            }
            return index+j;
        }

        private string GetTextFromRtf(string rtf)
        {
            Regex rex = new Regex("(\\\\\\n?[A-Za-z0-9]+[ ]?)|(})|({)|(\\r)(\\n)");
            string temp1 = rex.Replace(rtf, "");
            Regex rex2 = new Regex("([ ]+)");
            string temp2 = rex2.Replace(temp1, " ");
            return temp2;
        }

    }
}
