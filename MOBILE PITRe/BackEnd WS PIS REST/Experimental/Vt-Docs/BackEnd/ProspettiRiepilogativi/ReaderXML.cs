using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace ProspettiRiepilogativi
{
	/// <summary>
	/// ReaderXML: legge i dati dal file di configurazione
	/// dei Reports.
	/// </summary>
	public class ReaderXML
	{
		private string _xmlPath;

		#region Proprietà
		public string XML_Path
		{
			get
			{
				return _xmlPath;
			}
			set
			{
				_xmlPath = value;
			}
		}
		#endregion

		public ReaderXML(string xmlPath)
		{
			_xmlPath = xmlPath;
		}

		public ArrayList DO_ReadXML()
		{
			#region Dichiarazioni
			ArrayList reportList = new ArrayList();
			#endregion

			#region Caricamento del file

			//creo un mlDocument
			XmlDocument _xml = new XmlDocument();
			//creo un reader per il file specifico
			XmlTextReader _xtr = new XmlTextReader(_xmlPath);
			try
			{
				//carico il reader nell xmlDocument
				_xml.Load(_xtr);
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				_xtr.Close();	
			}
			#endregion

			#region Caricamento dei nodi

			try
			{
				//Recuperiamo tutti i nodi del file
				XmlNodeList xnl = _xml.SelectNodes("reports/report");

				foreach(XmlNode xn in xnl)
				{
					Report _rep = new Report();
					_rep.Descrizione = xn.SelectSingleNode("titolo").InnerText;
					_rep.Valore = xn.SelectSingleNode("codice").InnerText;

					//Verifichiamo che il report abbia dei sottoReport
					XmlNodeList xsnl = xn.SelectNodes("subReport");
					if(xsnl.Count > 0)
					{
						//Recuperiamo il sottoReport
						foreach(XmlNode xsn in xsnl)
						{
							Report _subRep = new Report(xsn.SelectSingleNode("titolo").InnerText,xsn.SelectSingleNode("codice").InnerText);
							_rep.DO_AddSubReport(_subRep);
						}
					}
					reportList.Add(_rep);
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
			#endregion    

			#region Return
			return reportList;
			#endregion
		}
	}
}
