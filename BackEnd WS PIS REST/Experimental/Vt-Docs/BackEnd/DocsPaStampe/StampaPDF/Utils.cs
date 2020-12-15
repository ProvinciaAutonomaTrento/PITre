using System;
using System.Xml;
using iTextSharp;
using iTextSharp.text;


namespace StampaPDF
{
	/// <summary>
	/// Summary description for UtilsXml.
	/// </summary>
	public class Utils
	{
	#region UTILS XML
		/// <summary>
		/// GetXmlField
		/// </summary>
		/// <param name="fieldName">Nome del nodo</param>
		/// <param name="node">Oggetto XmlNode</param>
		/// <returns>string</returns>
		public static string GetXmlField(string fieldName, XmlNode node)
		{
			string result = null;
			XmlNode child = node.SelectSingleNode(fieldName);
			if(child != null)
			{
				if(child.InnerText != "")
				{
					result = child.InnerText;
				}
			}
			return result;
		}

		/// <summary>
		/// SetXmlField
		/// </summary>
		/// <param name="fieldName">Nome del nodo</param>
		/// <param name="node">Oggetto XmlNode</param>
		public static void SetXmlField(string fieldName, XmlNode node, string strToWrite)
		{
			XmlNode child = node.SelectSingleNode(fieldName);
			child.InnerText = strToWrite;
		}

		public static XmlNode CercaCodice(string path, string percorsoXml, string nodoUnico, string tag, XmlDocument xmlDoc)
		{
			XmlNodeList lista = xmlDoc.SelectNodes(percorsoXml);
			foreach(XmlNode nodo in lista)
			{
				string t = nodo.ChildNodes[0].ChildNodes[0].Name;
				if (nodo.ChildNodes[0].ChildNodes[0].Name.Equals(tag))
				{
					t = nodo.ChildNodes[0].ChildNodes[0].InnerText;
					if (nodo.ChildNodes[0].ChildNodes[0].InnerText.Equals(nodoUnico))
						return nodo;
				}
			}
			return null;
		}


		public static string getAttribute(string name, XmlNode node)
		{
			return getAttribute(name, node, false);
		}

		public static float getAttributeF(string name, XmlNode node)
		{
			string val = getAttribute(name, node, false);
			if (val==null || val.Equals(""))
				return 0;
			return toFloat(val);
		}
		public static int getAttributeI(string name, XmlNode node)
		{
			string val = getAttribute(name, node, false);
			if (val==null || val.Equals(""))
				return 0;
			return toInt(val);
		}

		public static bool getAttributeB(string name, XmlNode node)
		{
			bool ret = false;
			string val = getAttribute(name, node, false);
			if (val != null && (val.Equals("true") || val.Equals("1")))
				ret = true;
			return ret;
		}
		public static string getAttribute(string name, XmlNode node, bool isNull)
		{
			string ret = "";

			if (node.Attributes[name] != null)
				ret = node.Attributes[name].Value;

			if (isNull && (ret!=null && ret.Equals("")))
				ret = null;

			return ret;	
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


	#endregion UTILS XML

	#region UTILS PDF
		public static iTextSharp.text.Color getColor(string color)
		{
			Color colore = new Color(System.Drawing.ColorTranslator.FromHtml(color));
			return colore;
		}

		public static int getFontStyle(string style)
		{
			if (style == null || style.Equals(""))
				return 0;
			
			switch(style) 
			{
				case "BOLD":
					return Font.BOLD;
				case "PLAIN":
					return Font.NORMAL;
				case "NORMAL":
					return Font.NORMAL;
				case "ITALIC":
					return Font.ITALIC;
				case "BOLDITALIC":
					return Font.BOLDITALIC;
				case "UNDERLINE":
					return Font.UNDERLINE;
				default:
					return Font.NORMAL;
			}
			
		}
		
		public static int getImageAlign(string align)
		{
			if (align == null || align.Equals(""))
				return Image.DEFAULT;
			
			switch(align.ToUpper()) 
			{
				case "LEFT":
					return Image.LEFT_ALIGN;
				case "RIGHT":
					return Image.RIGHT_ALIGN;
				case "MIDDLE":
                    return Image.MIDDLE_ALIGN;
				default:
					return Image.DEFAULT;
			}
		}

		public static int getAlign(string align)
		{
			if (align == null || align.Equals(""))
				return Element.ALIGN_JUSTIFIED;
			
			switch(align.ToUpper()) 
			{
				case "LEFT":
					return Element.ALIGN_LEFT;
				case "RIGHT":
					return Element.ALIGN_RIGHT;
				case "MIDDLE":
					return Element.ALIGN_MIDDLE;
				case "BOTTOM":
					return Element.ALIGN_BOTTOM;
				case "TOP":
					return Element.ALIGN_TOP;
				case "CENTER":
					return Element.ALIGN_CENTER;
				default:
					return Element.ALIGN_JUSTIFIED;
			}
		}

	#endregion

	}
}

