using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace AmmUtils
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class UtilsXml
	{
		public UtilsXml()
		{}

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

		/// <summary>
		/// Metodo per estrapolare i dati (codice, descrizione e dominio di default) dell'amm.ne corrente contenuti dalla Session["AMMDATASET"]
		/// Il formato della Session["AMMDATASET"] è: <codice>@<descrizione>
		/// </summary>
		/// <param name="sessione">Oggetto sessione</param>
		/// <param name="posizione">0 = codice, 1 = descrizione, 2 = dominio di default</param>
		/// <returns></returns>
		public static string GetAmmDataSession(string sessione, string posizione)
		{
			string[] dati;
			string val = "";
            if (sessione != null)
            {
                dati = sessione.Split('@');
                switch (posizione)
                {
                    case "0":			// codice
                        val = dati[0];
                        break;
                    case "1":			// descrizione
                        val = dati[1];
                        break;
                    case "2":			// dominio
                        val = dati[2];
                        break;
                    case "3":			// system_id (dpa_amministra)
                        val = dati[3];
                        break;
                }
            }
			return val;
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
	
		public static bool isNumeric(string strToCheck) 
		{		
			Regex regExp=new Regex("\\D");			
			return !regExp.IsMatch(strToCheck);
		}

		public static bool IsAlphaNumeric(string strToCheck)
		{
			// solo num (da 0 a 9)...
			// alfa (dalla a alla z / dalla A alla Z) ...
			// underscore "_"
			// punto "."
			Regex objAlphaNumericPattern=new Regex("[^a-zA-Z0-9_.-]+");
			return !objAlphaNumericPattern.IsMatch(strToCheck);    
		}

        public static bool IsAlphaNumericDominio(string strToCheck, char sep)
        {
            // solo num (da 0 a 9)...
            // alfa (dalla a alla z / dalla A alla Z) ...
            // underscore "_"
            // punto "."
            // chiocciola "@"
            // backslash "\"
            Regex dominioPattern = new Regex("[a-zA-Z0-9_.]+" + sep + "[a-zA-Z0-9_.-]");
            //Regex prova = new Regex ("[a-zA-Z0-9_\\.]+@[a-zA-Z0-9-]+\\.[a-zA-Z]{0,4}");
            return !dominioPattern.IsMatch(strToCheck);    
        }

		public static bool IsValidEmail(string strToCheck)
		{			
			return Regex.IsMatch(strToCheck, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 
		}

   }
}
