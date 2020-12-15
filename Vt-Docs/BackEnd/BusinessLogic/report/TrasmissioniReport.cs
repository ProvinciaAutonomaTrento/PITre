using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using log4net;
//using iTextSharp;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using ICSharpCode.SharpZipLib; 

namespace BusinessLogic.Report
{
	public class TrasmissioniReport
	{
        private static ILog logger = LogManager.GetLogger(typeof(TrasmissioniReport));

		private static string getPath()
		{
			string path = "";
			path = System.AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["XSDFilePath"];
			
			if(!System.IO.Directory.Exists(path))
					throw new StampaPDF.ReportException(StampaPDF.ErrorCode.PathReportNotFound, "Path Report PDF non trovata");
			return path;
		}

		/// <summary>Il metodo restituisce un file PDF che rappresenta il report delle trasmissioni relative all'oggetto specificato (documento o fascicolo)</summary>
		/// <param name="obj">oggetto della trasmissione</param>
		/// <returns>FileDocumento</returns>
		public static int reportTrasmDocFasc(DocsPaVO.trasmissione.OggettoTrasm obj, out DocsPaVO.documento.FileDocumento fileDoc)
		{
			int result = 0;
			fileDoc = null;
			FileStream fs =null;
			StampaPDF.Report report = null;
			try 
			{
				string pathReport = getPath();
				string templateXML = pathReport +"XMLRepTrasmDocFasc.xml";
				//costruisce il dataTable con le informazioni per il report
				DataTable dt_U;
				DataTable dt_R;
				bool newPage = false;
				//trasmissioni a ruoli
				BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniDocFasc(out dt_R, obj, "R");
				//trasmissioni a utenti
				BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniDocFasc(out dt_U, obj, "U");

				if ( (dt_R==null || dt_R.Rows.Count<1)  && (dt_U==null || dt_U.Rows.Count<1) )
					return 0;
				
				//crea il report, inserendo i valori parametrici e riempiendo la tabella
				fs = new FileStream(templateXML,System.IO.FileMode.Open, System.IO.FileAccess.Read);

				report = new StampaPDF.Report(fs,pathReport);
                
				#region  Sostituzione valori parametrici in base al tipo di oggetto
				Hashtable ht = new Hashtable();
				string idOggetto;
				string descOggetto;
				if (obj.infoDocumento!=null)
				{
					if (obj.infoDocumento.segnatura!= null && !obj.infoDocumento.segnatura.Equals(""))
						idOggetto = "DOCUMENTO: " + obj.infoDocumento.segnatura;
					else
						idOggetto = "DOCUMENTO: " + obj.infoDocumento.docNumber;
					descOggetto= "OGGETTO: " + obj.infoDocumento.oggetto;
				} 
				else
				{
					idOggetto = "FASCICOLO: " + obj.infoFascicolo.codice;
					descOggetto = "DESCRIZIONE: " + obj.infoFascicolo.descrizione;
				}
				ht["@param1"] = idOggetto;
				ht["@param2"] = descOggetto;
				report.replace(null,ht);
				#endregion

				report.printData(null);

                //
              //  dt_R.Rows[0][3].ToString()==obj.infoDocumento.
                //
				if (dt_R!=null && dt_R.Rows.Count >0)
				{
					report.appendParagraph("R_P","",false); //intestazione trasm a Ruoli
					report.appendTable("R_T",dt_R,false); //tabella trasm a Ruoli
					newPage = true;
				}
				if (dt_U!=null && dt_U.Rows.Count >0)
				{
					report.appendParagraph("U_P","",newPage); //intestazione trasm a Utenti
					report.appendTable("U_T",dt_U,false); //tabella trasm a Utenti
				}
				MemoryStream ms = report.close();
				ms.Flush();
				fs.Close();

				fileDoc = new DocsPaVO.documento.FileDocumento();
				fileDoc.content = ms.GetBuffer();
				fileDoc.length = fileDoc.content.Length;
				fileDoc.contentType = "application/pdf";
				fileDoc.name = "";
			}
			catch(StampaPDF.ReportException re) 
			{
				logger.Debug("Errore nella creazione del Report (stampaPDF)",re);
				if(fs!= null)
					fs.Close();
				if (report != null)
					report.close();
				result = (int) re.code;
			}
			catch(Exception e) 
			{
				logger.Debug("Errore nella gestione di Report (stampa)",e);
				if(fs!= null)
					fs.Close();
				if (report != null)
					report.close();
				result = -1;
			}

			return result;
		}

        /// <summary>Il metodo restituisce un file PDF che rappresenta il report delle trasmissioni relative all'oggetto specificato (documento o fascicolo)</summary>
        /// <param name="obj">oggetto della trasmissione</param>
        /// <returns>FileDocumento</returns>
        /// overloading per passare nome e cognome dell'utente loggato,in modo da evidenziare le trasmissioni di cui l'utente è destinatario
        /// Dimitri
        public static int reportTrasmDocFasc(DocsPaVO.trasmissione.OggettoTrasm obj, out DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente infoUt)
        {
            int result = 0;
            fileDoc = null;
            FileStream fs = null;
            StampaPDF.Report report = null;
            try
            {
                string pathReport = getPath();
                string templateXML = pathReport + "XMLRepTrasmDocFasc.xml";
                //costruisce il dataTable con le informazioni per il report
                DataTable dt_U;
                DataTable dt_R;
                bool newPage = false;
                //trasmissioni a ruoli
                BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniDocFasc(out dt_R, obj, "R");
                //trasmissioni a utenti
                BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniDocFasc(out dt_U, obj, "U");

                if ((dt_R == null || dt_R.Rows.Count < 1) && (dt_U == null || dt_U.Rows.Count < 1))
                    return 0;

                //crea il report, inserendo i valori parametrici e riempiendo la tabella
                fs = new FileStream(templateXML, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                report = new StampaPDF.Report(fs, pathReport);

                #region  Sostituzione valori parametrici in base al tipo di oggetto
                Hashtable ht = new Hashtable();
                string idOggetto;
                string descOggetto;
                if (obj.infoDocumento != null)
                {
                    if (obj.infoDocumento.segnatura != null && !obj.infoDocumento.segnatura.Equals(""))
                        idOggetto = "DOCUMENTO: " + obj.infoDocumento.segnatura;
                    else
                        idOggetto = "DOCUMENTO: " + obj.infoDocumento.docNumber;
                    descOggetto = "OGGETTO: " + obj.infoDocumento.oggetto;
                }
                else
                {
                    idOggetto = "FASCICOLO: " + obj.infoFascicolo.codice;
                    descOggetto = "DESCRIZIONE: " + obj.infoFascicolo.descrizione;
                }
                ht["@param1"] = idOggetto;
                ht["@param2"] = descOggetto;
                report.replace(null, ht);
                #endregion

                report.printData(null);

                //
                //  dt_R.Rows[0][3].ToString()==obj.infoDocumento.
                //
                if (dt_R != null && dt_R.Rows.Count > 0)
                {
                    report.appendParagraph("R_P", "", false); //intestazione trasm a Ruoli
                    report.appendTable("R_T", dt_R, false,infoUt); //tabella trasm a Ruoli
                    newPage = true;
                }
                if (dt_U != null && dt_U.Rows.Count > 0)
                {
                    report.appendParagraph("U_P", "", newPage); //intestazione trasm a Utenti
                    report.appendTable("U_T", dt_U, false,infoUt); //tabella trasm a Utenti
                }
                MemoryStream ms = report.close();
                ms.Flush();
                fs.Close();

                fileDoc = new DocsPaVO.documento.FileDocumento();
                fileDoc.content = ms.GetBuffer();
                fileDoc.length = fileDoc.content.Length;
                fileDoc.contentType = "application/pdf";
                fileDoc.name = "";
            }
            catch (StampaPDF.ReportException re)
            {
                logger.Debug("Errore nella creazione del Report (stampaPDF)", re);
                if (fs != null)
                    fs.Close();
                if (report != null)
                    report.close();
                result = (int)re.code;
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione di Report (stampa)", e);
                if (fs != null)
                    fs.Close();
                if (report != null)
                    report.close();
                result = -1;
            }

            return result;
        }


		/// <summary>Il metodo restituisce un file PDF che rappresenta il report delle trasmissioni relative alla UO specificata</summary>
		/// <param name="filtriTrasm">filtri della trasmissione</param>
		/// <param name="UO">UO che ha ricevuto o trasmesso</param>
		/// <returns>FileDocumento</returns>
		public static int reportTrasmUO(DocsPaVO.filtri.FiltroRicerca[] filtriTrasm, out DocsPaVO.documento.FileDocumento fileDoc, string UO)
		{
			int result = 0;
			fileDoc = null;
			FileStream fs =null;
			bool newPage = false;
			string tipoOggetto = "D";
			StampaPDF.Report report = null;
			string templateXML; 
			string paramDate;
			try 
			{
				string pathReport = getPath();
				//costruisce il dataTable con le informazioni per il report
				DataTable dt_Eff;
				DataTable dt_Ric;
				//effettuate
				BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniUO(out dt_Eff, filtriTrasm, "E");
				//ricevute
				BusinessLogic.Trasmissioni.QueryTrasmManager.getTrasmissioniUO(out dt_Ric, filtriTrasm, "R");
				
				if ( (dt_Eff==null || dt_Eff.Rows.Count<1)  && (dt_Ric==null || dt_Ric.Rows.Count<1) )
					return 0;
				//crea il report, inserendo i valori parametrici e riempiendo la tabella
				paramDate = setParam(filtriTrasm, ref tipoOggetto);

				templateXML = pathReport;
				if (tipoOggetto.Equals("D"))
					templateXML += "XMLRepTrasmUO_DOC.xml";
				else 
					templateXML += "XMLRepTrasmUO_FASC.xml";
				fs = new FileStream(templateXML,System.IO.FileMode.Open, System.IO.FileAccess.Read);
				report = new StampaPDF.Report(fs, pathReport);

				#region  Sostituzione valori parametrici in base al tipo di oggetto
				Hashtable ht = new Hashtable();
				ht["@param1"] = UO;
				ht["@param2"] = paramDate;
				
				report.replace(null,ht);
				#endregion

				report.printData(null);
				if (dt_Eff!=null && dt_Eff.Rows.Count >0)
				{
					report.appendParagraph("E_P","",false); //intestazione trasm Effettuate
					report.appendTable("E_T",dt_Eff,false); //tabella trasm Effettuate
					newPage = true;
				}
				if (dt_Ric!=null && dt_Ric.Rows.Count >0)
				{
					report.appendParagraph("R_P","",newPage); //intestazione trasm Ricevute
					report.appendTable("R_T",dt_Ric,false); //tabella trasm Ricevute
				}
				fs.Close();
				MemoryStream ms = report.close();
				ms.Flush();
				
				fileDoc = new DocsPaVO.documento.FileDocumento();
				fileDoc.content = ms.GetBuffer();
				fileDoc.length = fileDoc.content.Length;
				fileDoc.contentType = "application/pdf";
				fileDoc.name = "";
			}
			catch(StampaPDF.ReportException re) 
			{
				logger.Debug("Errore nella creazione del Report (stampaPDF)",re);
				if(fs!= null)
					fs.Close();
				if (report != null)
					report.close();
				result = (int) re.code;
			}
			catch(Exception e) 
			{
				logger.Debug("Errore nella gestione di Report (stampa)",e);
				if(fs!= null)
					fs.Close();
				if (report != null)
					report.close();
				result = -1;
			}
			return result;
		}

		private static string setParam(DocsPaVO.filtri.FiltroRicerca[] filtriTrasm, ref string tipoOggetto)
		{
			DocsPaVO.filtri.FiltroRicerca f;
			string val = "";
			for (int i = 0; i < filtriTrasm.Length; i++) 
			{
				f = filtriTrasm[i];
				if (f.valore != null && !f.valore.Equals("")) 
				{
					switch(f.argomento) 
					{
						case "TRASMISSIONE_IL":
							val += "del " + f.valore;	
							break;
						case "TRASMISSIONE_SUCCESSIVA_AL":
							val += "dal " + f.valore;
							break;
						case "TRASMISSIONE_PRECEDENTE_IL":
							val += " fino al " + f.valore;
							break;
						case "TIPO_OGGETTO":
							tipoOggetto = f.valore;
							break;
					}
				}
			}
			return val;
		}

	}
}
