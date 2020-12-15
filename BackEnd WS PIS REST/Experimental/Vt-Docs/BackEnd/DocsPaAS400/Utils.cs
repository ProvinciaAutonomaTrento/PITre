using System;
using log4net;

namespace DocsPaAS400
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
        private static ILog logger = LogManager.GetLogger(typeof(Utils));

		public static string sevenChars(string val)
		{
            string res="";
			for(int i=val.Length;i<7;i++)
			{
               res=res+"0";
			}
			res=res+val;
			return res;
		}

		public static string getDate(string date)
		{
		   string res="";
			if(date!=null && !date.Equals(""))
			{
				DateTime protDate=DateTime.Parse(date);
				res=res+"0"+protDate.ToString(Constants.INSERT_DATE_FORMAT);
			}
		   return res;
		}

		public static string getTimestamp()
		{
           string res=DateTime.Now.ToString(Constants.TIMESTAMP_FORMAT);
		   return res;
		}

		public static string getYear(string date)
		{
			string res="";
			if(date!=null)
			{
				DateTime protDate=DateTime.Parse(date);
				res=res+"0"+protDate.ToString(Constants.INSERT_YEAR_FORMAT);
			}
			return res;
		}
	    
		public static string getDettagliMittDest(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			string res="";
			DocsPaVO.utente.Corrispondente corr=null;
			if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
			{
				DocsPaVO.documento.ProtocolloEntrata pe=(DocsPaVO.documento.ProtocolloEntrata) schedaDoc.protocollo;
				corr=pe.mittente;
			}
			else
			{
				DocsPaVO.documento.ProtocolloUscita pu=(DocsPaVO.documento.ProtocolloUscita) schedaDoc.protocollo;
				if(pu.destinatari!=null && pu.destinatari.Count>0)
				corr=(DocsPaVO.utente.Corrispondente) pu.destinatari[0];
			}
			if(corr!=null)
			{
				res=getCorrectString(corr.descrizione);
			}
			return res;
		}

		public static string getNumProtMitt(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			string res="";
			if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
			{
				DocsPaVO.documento.ProtocolloEntrata pe=(DocsPaVO.documento.ProtocolloEntrata) schedaDoc.protocollo;
				res=getCorrectString(pe.descrizioneProtocolloMittente);
			}
			else
			{
				res="";
			}
			return res;

		}
		public static string getDataProtMitt(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			string res="";
			if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
			{
				DocsPaVO.documento.ProtocolloEntrata pe=(DocsPaVO.documento.ProtocolloEntrata) schedaDoc.protocollo;
				res=getDate(pe.dataProtocolloMittente);
			}
			else
			{
				res="";
			}
			return res;
		}

		public static string getProvenienzaMitt(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			string res="";
			DocsPaVO.utente.Corrispondente corr=null;
			if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
			{
				DocsPaVO.documento.ProtocolloEntrata pe=(DocsPaVO.documento.ProtocolloEntrata) schedaDoc.protocollo;
				corr=pe.mittente;
			}
			if(corr!=null)
			{   
				string systemId=null;
				if(corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)) || corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
				{
					systemId=corr.systemId;
					//res=getCorrectString(utente.);
				}
				if(corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
				{
					DocsPaVO.utente.Ruolo ruolo=(DocsPaVO.utente.Ruolo) corr;
					if(ruolo.uo!=null)
					{
						systemId=ruolo.uo.systemId;
					}
				}
				res=getCorrectString(getCitta(systemId));
			}
			return res;
		}

		public static string getClassificationCode(string idamm,string code,int position)
		{ 
			string res="";
			string sep=DocsPaDB.Utils.Personalization.getInstance(idamm).getSeparator();
			char[] separator=sep.ToCharArray();
            if(code==null || code.Equals("")) return res;
			string[] classifications=code.Split(separator);
			/*modifica del 04/02/2004 per il problema delle classifiche con solo il secondo livello (es. V.A.00006/6  e non XX.B.1.00010/33)
			/*if(position<classifications.Length)
			{
               res=classifications[position];
			}*/
			if(position==3) return classifications[classifications.Length-1];
			if(position<classifications.Length-1) return classifications[position];
			return "0";
		}

		public static string getCitta(string systemId)
		{
          // co.debug.add("getCitta");
			logger.Debug("getCitta");
			DocsPaDB.DBProvider db=null;
		   string res=null;
			bool rtn=false;
			try
			{
				if(systemId!=null && !systemId.Equals(""))
				{
					string queryString="SELECT VAR_CITTA FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI="+systemId;
					DocsPaUtils.Query q=new DocsPaUtils.Query(queryString);
					db=new DocsPaDB.DBProvider();

					rtn=db.ExecuteScalar(out res,queryString);
					if(!rtn)
						throw new Exception("AS400, reperimento valore città corrispondente "+systemId+" non riuscito");

				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				
				if(db!=null)
					db.Dispose();
			}
           return res;
		}

		public static string getCorrectString(string val)
		{
			string res="";
			if(val!=null)
			{
				res=val;
			}
			return res;
		}
	}
}
