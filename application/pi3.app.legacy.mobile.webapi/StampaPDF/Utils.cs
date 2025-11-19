// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StampaPDF
{
    public class Utils
    {
        public static string getAttribute(string name, XmlNode node)
        {
            return getAttribute(name, node, false);
        }

        public static string getAttribute(string name, XmlNode node, bool isNull)
        {
            string ret = "";

            if (node.Attributes[name] != null)
                ret = node.Attributes[name].Value;

            if (isNull && (ret != null && ret.Equals("")))
                ret = null;

            return ret;
        }

        public static float getAttributeF(string name, XmlNode node)
        {
            string val = getAttribute(name, node, false);
            if (val == null || val.Equals(""))
                return 0;
            return toFloat(val);
        }
        public static int getAttributeI(string name, XmlNode node)
        {
            string val = getAttribute(name, node, false);
            if (val == null || val.Equals(""))
                return 0;
            return toInt(val);
        }

        public static int toInt(string val)
        {
            if (val != null && !val.Equals(""))
                return Int32.Parse(val);
            return 0;
        }

        public static float toFloat(string val)
        {
            if (val != null && !val.Equals(""))
                return float.Parse(val);
            return 0;
        }

        public static bool getAttributeB(string name, XmlNode node)
        {
            bool ret = false;
            string val = getAttribute(name, node, false);
            if (val != null && (val.Equals("true") || val.Equals("1")))
                ret = true;
            return ret;
        }

    }
}
