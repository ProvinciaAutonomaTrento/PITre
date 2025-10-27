// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
    public class AmministrazioniXml
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(AmministrazioniXml));

        public XmlDocument ExportStructureXmlDocument()
        {
            return ExportStructureCommon();
        }

        public XmlDocument ExportStructureCommon()
        {
            bool result = false;
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();

                //Create the root element and add it to the document.
                XmlNode root = doc.AppendChild(doc.CreateElement("DOCSPA"));

                //legge i server
                /*
				BusinessLogic.AmministrazioneXml.ServerXml serverXml=new BusinessLogic.AmministrazioneXml.ServerXml();
				result=serverXml.ExportStructure(doc,root);
				if(!result)
				{
					logger.Debug("Errore esportazione server");
					throw new Exception();
				}*/

                //esporta l'anagrafica delle funzioni elementari
                BusinessLogic.AmministrazioneXml.FunzioniXml funzioniXml = new BusinessLogic.AmministrazioneXml.FunzioniXml();
                result = funzioniXml.ExportAnagraficaFunzioni(doc, root);
                if (!result)
                {
                    logger.Debug("Errore esportazione funzioni elementari");
                    throw new Exception();
                }

                //esportazione tipi funzione
                result = funzioniXml.ExportStructure(doc, root);
                if (!result)
                {
                    logger.Debug("Errore esportazione funzioni");
                    throw new Exception();
                }

                //esportazione ragioni trasmissioni generali
                BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml ragioniXml = new BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml();
                result = ragioniXml.ExportStructure(doc, root, null);
                if (!result)
                {
                    logger.Debug("Errore esportazione ragioni trasmissioni generali");
                    throw new Exception();
                }

                //legge le amministrazioni
                System.Data.DataSet dataSet;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetAmministrazioni(out dataSet);
                if (!result)
                {
                    logger.Debug("Errore esportazione amministrazioni");
                    throw new Exception();
                }
                if (dataSet != null)
                {
                    foreach (System.Data.DataRow row in dataSet.Tables["AMMINISTRAZIONI"].Rows)
                    {
                        string idAmm = row["SYSTEM_ID"].ToString();

                        //esportazione dati della amministrazione
                        XmlNode amministrazione = root.AppendChild(doc.CreateElement("AMMINISTRAZIONE"));
                        XmlNode dati = amministrazione.AppendChild(doc.CreateElement("DATI"));

                        dati.AppendChild(doc.CreateElement("CODICE")).InnerText = row["VAR_CODICE_AMM"].ToString().ToUpper();
                        dati.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = row["VAR_DESC_AMM"].ToString();
                        dati.AppendChild(doc.CreateElement("LIBRERIA")).InnerText = row["VAR_LIBRERIA"].ToString();
                        dati.AppendChild(doc.CreateElement("SEGNATURA")).InnerText = row["VAR_FORMATO_SEGNATURA"].ToString();
                        dati.AppendChild(doc.CreateElement("FASCICOLATURA")).InnerText = row["VAR_FORMATO_FASCICOLATURA"].ToString();
                        //new gadamo
                        dati.AppendChild(doc.CreateElement("SERVERSMTP")).InnerText = row["VAR_SMTP"].ToString();
                        dati.AppendChild(doc.CreateElement("PORTASMTP")).InnerText = row["NUM_PORTA_SMTP"].ToString();
                        dati.AppendChild(doc.CreateElement("USERSMTP")).InnerText = row["VAR_USER_SMTP"].ToString();
                        dati.AppendChild(doc.CreateElement("PASSWORDSMTP")).InnerText = row["VAR_PWD_SMTP"].ToString();
                        dati.AppendChild(doc.CreateElement("PROTOCOLLO_INTERNO")).InnerText = row["CHA_PROTOINT"].ToString();
                        dati.AppendChild(doc.CreateElement("RAGIONE_TO")).InnerText = row["VAR_RAGIONE_TO"].ToString();
                        dati.AppendChild(doc.CreateElement("RAGIONE_CC")).InnerText = row["VAR_RAGIONE_CC"].ToString();
                        dati.AppendChild(doc.CreateElement("DOMINIO")).InnerText = row["VAR_DOMINIO"].ToString();
                        // old
                        #region codice commentato
                        //						if(row["VAR_FORMATO_FASCICOLATURA"]!=null)
                        //						{
                        //							dati.AppendChild(doc.CreateElement("DOMINIO")).InnerText = row["VAR_DOMINIO"].ToString();
                        //						}
                        //						if(row["VAR_SMTP"]!=null)
                        //						{
                        //							dati.AppendChild(doc.CreateElement("SERVERSMTP")).InnerText = row["VAR_SMTP"].ToString();
                        //						}
                        //						if(row["NUM_PORTA_SMTP"]!=null)
                        //						{
                        //							dati.AppendChild(doc.CreateElement("PORTASMTP")).InnerText = row["NUM_PORTA_SMTP"].ToString();
                        //						}
                        //						string temp="";
                        //						if(row["CHA_PROTOINT"]!=null)
                        //						{
                        //							if(row["CHA_PROTOINT"].ToString()!="") temp="1";
                        //						}
                        //						dati.AppendChild(doc.CreateElement("PROTOCOLLO_INTERNO")).InnerText = temp;
                        //
                        //						temp="";
                        //						if(row["VAR_RAGIONE_TO"]!=null)
                        //						{
                        //							temp=row["VAR_RAGIONE_TO"].ToString();
                        //						}
                        //						dati.AppendChild(doc.CreateElement("RAGIONE_TO")).InnerText = temp;
                        //
                        //						temp="";
                        //						if(row["VAR_RAGIONE_CC"]!=null)
                        //						{
                        //							temp=row["VAR_RAGIONE_CC"].ToString();
                        //						}
                        //						dati.AppendChild(doc.CreateElement("RAGIONE_CC")).InnerText = temp;
                        #endregion

                        //esportazione ragioni trasmissione
                        BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml ragioniGenXml = new BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml();
                        result = ragioniGenXml.ExportStructure(doc, amministrazione, idAmm);
                        if (!result)
                        {
                            logger.Debug("Errore esportazione ragioni trasmissione");
                            throw new Exception();
                        }

                        //esportazione registri
                        BusinessLogic.AmministrazioneXml.RegistriXml registriXml = new BusinessLogic.AmministrazioneXml.RegistriXml();
                        result = registriXml.ExportStructure(doc, amministrazione, idAmm);
                        if (!result)
                        {
                            logger.Debug("Errore esportazione registri");
                            throw new Exception();
                        }

                        //esportazione ruoli
                        BusinessLogic.AmministrazioneXml.RuoliXml ruoliXml = new BusinessLogic.AmministrazioneXml.RuoliXml();
                        result = ruoliXml.ExportStructure(doc, amministrazione, idAmm);
                        if (!result)
                        {
                            logger.Debug("Errore esportazione ruoli");
                            throw new Exception();
                        }

                        //esportazione utenti
                        //						BusinessLogic.AmministrazioneXml.UtentiXml utentiXml=new BusinessLogic.AmministrazioneXml.UtentiXml();
                        //						result=utentiXml.ExportStructure(doc,amministrazione,idAmm);
                        //						if(!result)
                        //						{
                        //							logger.Debug("Errore esportazione utenti");
                        //							throw new Exception();
                        //						}

                        //esportazione organigramma
                        //						BusinessLogic.AmministrazioneXml.OrganigrammaXml organigramma=new BusinessLogic.AmministrazioneXml.OrganigrammaXml();
                        //						result=organigramma.ExportStructure(doc,amministrazione,idAmm,"0");
                        //						if(!result)
                        //						{
                        //							logger.Debug("Errore esportazione organigramma");
                        //							throw new Exception();
                        //						}

                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la creazione dell'XML", exception);
                doc = null;
            }

            logger.Debug("Amministrazione: Export XML amministrazioni: OK!");
            return doc;
        }

    }
}
