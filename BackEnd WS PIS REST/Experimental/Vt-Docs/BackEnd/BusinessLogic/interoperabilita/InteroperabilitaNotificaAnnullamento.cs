using System;
using System.Xml;
using System.Data;
using System.Globalization;
using log4net;

namespace BusinessLogic.Interoperabilità
{
	/// <summary>
	/// Summary description for InteroperabilitaNotificaAnnullamento.
	/// </summary>
	public class InteroperabilitaNotificaAnnullamento
	{
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaNotificaAnnullamento));
	
		public static bool processaNotificaAnnullamento(DocsPaVO.Interoperabilita.NotificaAnnullamento notifica, out string message)
		{
			message=string.Empty;
			try
			{
				CultureInfo ci = new CultureInfo("it-IT");
				string[] formati={"dd/MM/yyyy","yyyy-MM-dd","DD/MM/YYYY hh:mm:ss","DD/MM/YYYY hh.mm.ss","DD/MM/YYYY HH.mm.ss","DD/MM/YYYY HH:mm:ss"};
				
				string codiceAmministrazione = notifica.codAmm;
				string codiceAOO = notifica.codAOO;
				string numeroRegistrazione = notifica.numeroRegistrazione;
				DateTime dataRegistrazione = DateTime.ParseExact(notifica.dataRegistrazione,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				
				//info sul messaggio
				string codiceAmministrazioneMitt = notifica.codAmm_Mitt;
				string codiceAOOMitt = notifica.codAOO_Mitt;
				string numeroRegistrazioneMitt = notifica.numeroRegistr_Mitt;
				string motivoAnnulla = notifica.motivoAnnullamento;
				string provvedimento = notifica.provvedimento;
				DateTime dataRegistrazioneMitt = DateTime.ParseExact(notifica.dataRegistr_Mitt,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				
				//si trova il numero del documento
				logger.Debug("Ricerca id del profilo...");

                string idProf = Interoperabilità.InteroperabilitaUtils.findIdProfile(codiceAOOMitt, numeroRegistrazioneMitt, dataRegistrazioneMitt.Year);
				logger.Debug("idProfile="+idProf);

				if(idProf==null)
				{
					logger.Debug("Documento mittente non trovato"); 
					message="Documento mittente non trovato";
					return false;
				}

				//si esegue l'update della tabella stato invio
				if(codiceAOO!= null && !codiceAOO.Equals("") && codiceAmministrazione!=null && !codiceAmministrazione.Equals(""))
				{
					logger.Debug("Update della tabella stato invio: idProfile="+idProf+", CodiceAOO="+codiceAOO+", CodiceAmm="+codiceAmministrazione+", data="+dataRegistrazione.ToString("dd/MM/yyyy"));
					
					bool res_update=updateStatoInvioAnnulla(idProf,codiceAOO,codiceAmministrazione,dataRegistrazione.ToString("dd/MM/yyyy"),numeroRegistrazione,dataRegistrazione.Year,motivoAnnulla,provvedimento);
					if(!res_update)
					{ 
						logger.Debug("Errore: non e' stato eseguito l'update del profilo"); 
						message="Si è verificato un errore nell'aggiornamento della ricevuta di ritorno";
						return false;
					}
				}
				else
				{
					logger.Debug("L'update della tabella profile non può essere eseguito: codiceAOO o codiceAmministrazione nullo");
					message="Si è verificato un errore: dati mancanti ";
					return false;
				}

				return true;
			}
			catch(Exception e)
			{
                logger.Error("Si è verificato un problema nella ricevuta di ritorno. Eccezione: " + e.ToString()); 
				return false;	
			}
		}


		public static bool processaXmlAnnullamento(string path, string filename, DocsPaVO.utente.Registro reg, string mailId, string mailAddress)
		{

            bool isSignatureValid = interoperabilita.InteroperabilitaEccezioni.isSignatureValid(System.IO.Path.Combine(path, filename));
            if (!isSignatureValid)
                throw new System.Xml.Schema.XmlSchemaException();

			XmlDocument doc =new XmlDocument();
			InteropResolver my = new InteropResolver();
            XmlTextReader xtr = new XmlTextReader(System.IO.Path.Combine(path, filename)) { Namespaces = false };
			xtr.WhitespaceHandling = WhitespaceHandling.None;
			XmlValidatingReader xvr = new XmlValidatingReader(xtr);
			xvr.ValidationType = System.Xml.ValidationType.DTD;
			xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
			xvr.XmlResolver = my;

			try
			{ 
				doc.Load(xvr);
			}
			catch(System.Xml.Schema.XmlSchemaException e)
			{
                logger.Error("La mail viene sospesa perche' il  file Annullamento.xml non e' valido. Eccezione:" + e.Message);

                if (InteroperabilitaUtils.MailElaborata(mailId, "D"))
				{
					logger.Debug("Sospensione eseguita");
				}
				else
				{
					logger.Debug("Sospensione non eseguita");
				}

				return false;
			}
			catch(Exception e)
			{
                logger.Error("La mail viene sospesa. Eccezione:" + e.Message);

                if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
				{
					logger.Debug("Sospensione eseguita");
				}
				else
				{
					logger.Debug("Sospensione non eseguita");
				}
				
				return false;
			}
			finally
			{
				xvr.Close();
				xtr.Close();
			}

			try
			{
				CultureInfo ci = new CultureInfo("it-IT");
				string[] formati={"yyyy-MM-dd"};
				
				XmlElement elIdentificatore=(XmlElement) doc.DocumentElement.SelectSingleNode("Identificatore");
				string codiceAmministrazione = elIdentificatore.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
				string codiceAOO = elIdentificatore.SelectSingleNode("CodiceAOO").InnerText.Trim();
				string numeroRegistrazione = elIdentificatore.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
				DateTime dataRegistrazione = DateTime.ParseExact(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText.Trim(),formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				XmlElement elMotivo=(XmlElement) doc.DocumentElement.SelectSingleNode("Motivo");
				string motivo=elMotivo.InnerText.Trim();
				XmlElement elProvvedimento=(XmlElement) doc.DocumentElement.SelectSingleNode("Provvedimento");
				string provvedimento=elProvvedimento.InnerText.Trim();
				/*			
				//info sul messaggio
				XmlElement elIdentificatoreMitt=(XmlElement) doc.DocumentElement.SelectSingleNode("MessaggioRicevuto/Identificatore");
				string codiceAmministrazioneMitt = elIdentificatoreMitt.SelectSingleNode("CodiceAmministrazione").InnerText.Trim();
				string codiceAOOMitt = elIdentificatoreMitt.SelectSingleNode("CodiceAOO").InnerText.Trim();
				string numeroRegistrazioneMitt = elIdentificatoreMitt.SelectSingleNode("NumeroRegistrazione").InnerText.Trim();
				DateTime dataRegistrazioneMitt = DateTime.ParseExact(elIdentificatore.SelectSingleNode("DataRegistrazione").InnerText,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				
				//si trova il numero del documento
				logger.Debug("Ricerca id del profilo...");
				//può essere ricercato solo con i dati del protocollo in Arrivo
				string idProf = findIdProfRicevuta(codiceAmministrazione,codiceAOO,numeroRegistrazione,dataRegistrazione);
				logger.Debug("idProfile="+idProf);
				
				if(idProf==null)
				{
					logger.Debug("La mail viene sospesa: il documento indicato non è stato trovato"); 
					if(InteroperabilitaUtils.MailElaborata(mailId,"U"))
					{
						logger.Debug("Sospensione eseguita");
					}
					else
					{
						logger.Debug("Sospensione non eseguita");
					};
					return false;
				}
*/
				//si esegue l'update della tabella stato invio
                if(codiceAOO!= null && !codiceAOO.Equals("") && codiceAmministrazione!=null && !codiceAmministrazione.Equals(""))
				{
					logger.Debug("Update della tabella stato invio: CodiceAOO="+codiceAOO+", CodiceAmm="+codiceAmministrazione+", data="+dataRegistrazione.ToString("dd/MM/yyyy"));
					
					//si fa un aggiornamento inserendo anche l'annullamento ed il motivo
					//bool res_update=updateStatoInvio(idProf,codiceAOO,codiceAmministrazione,dataRegistrazione.ToString("dd/MM/yyyy"),numeroRegistrazione,dataRegistrazione.Year);
					bool res_update=updateStatoInvioAnnulla(null, codiceAOO,codiceAmministrazione,dataRegistrazione.ToString("dd/MM/yyyy"),numeroRegistrazione,dataRegistrazione.Year,motivo, provvedimento);

					if(!res_update)
					{ 
						logger.Debug("La mail viene sospesa: non e' stato eseguito l'update del profilo");
                        if (InteroperabilitaUtils.MailElaborata(mailId, "U"))
						{
							logger.Debug("Sospensione eseguita");
						}
						else
						{
							logger.Debug("Sospensione non eseguita");
						}
						return false;
					}
				}
				else
				{
					logger.Debug("L'update della tabella profile non può essere eseguito: codiceAOO o codiceAmministrazione nullo");
				}

				return true;
			}
			catch(Exception e)
			{
                logger.Error("La mail viene scartata. Eccezione: " + e.ToString()); 
				return false;	
			}
		}


		private static bool updateStatoInvioAnnulla(string idProfile,string codiceAOO,string codiceAmministrazione, string data, string numeroRegistrazione,int anno, string motivoAnnulla, string provvedimento)
		{
			bool result = false;

			try
			{				
				DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
				result=	obj.updStatoInvioAnnulla(idProfile,codiceAOO,codiceAmministrazione,data,numeroRegistrazione,anno,motivoAnnulla,provvedimento);
			}
			catch(Exception e)
			{
                logger.Error("Eccezione: " + e.Message);

				result = false;
			}

			return result;
		}
	

	}
}
