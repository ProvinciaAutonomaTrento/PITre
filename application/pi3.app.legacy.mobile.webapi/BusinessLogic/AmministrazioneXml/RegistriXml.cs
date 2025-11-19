// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
    public class RegistriXml
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(RegistriXml));

        public bool ExportStructure(XmlDocument doc, XmlNode amministrazione, string idAmm)
        {
            bool result = true; //presume successo
            try
            {
                System.Data.DataSet dataSetRegistri;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetRegistri(out dataSetRegistri, idAmm);
                if (!result)
                {
                    throw new Exception();
                }

                if (dataSetRegistri != null)
                {
                    if (dataSetRegistri.Tables["REGISTRI"].Rows.Count > 0)
                    {
                        XmlNode registri = amministrazione.AppendChild(doc.CreateElement("REGISTRI"));

                        foreach (System.Data.DataRow rowRegistro in dataSetRegistri.Tables["REGISTRI"].Rows)
                        {
                            XmlNode registro = registri.AppendChild(doc.CreateElement("REGISTRO"));
                            registro.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowRegistro["VAR_DESC_REGISTRO"].ToString();
                            registro.AppendChild(doc.CreateElement("CODICE")).InnerText = rowRegistro["VAR_CODICE"].ToString().ToUpper();
                            registro.AppendChild(doc.CreateElement("AUTOMATICO")).InnerText = rowRegistro["CHA_AUTOMATICO"].ToString().ToUpper();

                            XmlNode registroMail = registro.AppendChild(doc.CreateElement("EMAIL"));
                            registroMail.AppendChild(doc.CreateElement("INDIRIZZO")).InnerText = rowRegistro["VAR_EMAIL_REGISTRO"].ToString();
                            registroMail.AppendChild(doc.CreateElement("UTENTE")).InnerText = rowRegistro["VAR_USER_MAIL"].ToString();
                            registroMail.AppendChild(doc.CreateElement("PASSWORD")).InnerText = rowRegistro["VAR_PWD_MAIL"].ToString();
                            registroMail.AppendChild(doc.CreateElement("SMTP")).InnerText = rowRegistro["VAR_SERVER_SMTP"].ToString();
                            registroMail.AppendChild(doc.CreateElement("POP")).InnerText = rowRegistro["VAR_SERVER_POP"].ToString();
                            registroMail.AppendChild(doc.CreateElement("PORTASMTP")).InnerText = rowRegistro["NUM_PORTA_SMTP"].ToString();
                            registroMail.AppendChild(doc.CreateElement("PORTAPOP")).InnerText = rowRegistro["NUM_PORTA_POP"].ToString();
                            registroMail.AppendChild(doc.CreateElement("USERSMTP")).InnerText = rowRegistro["VAR_USER_SMTP"].ToString();
                            registroMail.AppendChild(doc.CreateElement("PASSWORDSMTP")).InnerText = rowRegistro["VAR_PWD_SMTP"].ToString();
                            //string ruoloRif=amministrazioneXml.GetRuoloRif(rowRegistro["SYSTEM_ID"].ToString (),idAmm);
                            string ruoloRif = "";
                            registro.AppendChild(doc.CreateElement("RUO_RIF")).InnerText = ruoloRif;
                            /*string utenteRif=amministrazioneXml.GetUtenteRif(ruoloRif,idAmm);
							if(utenteRif==null) 
							{
								utenteRif="";
							}
							else
							{
								utenteRif=utenteRif.ToUpper();
							}*/
                            string utenteRif = "";
                            registro.AppendChild(doc.CreateElement("UT_RIF")).InnerText = utenteRif;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'esportazione delle ragioni di trasmissione", exception);
                result = false;
            }
            return result;
        }

    }
}
