// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
    public class RagioniTrasmissioneXml
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(RagioniTrasmissioneXml));

        public bool ExportStructure(XmlDocument doc, XmlNode amministrazione, string idAmm)
        {
            bool result = true; //presume successo
            try
            {
                System.Data.DataSet dataSetRagioni;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetRagioniTrasmissione(out dataSetRagioni, idAmm);
                if (!result)
                {
                    throw new Exception();
                }

                if (dataSetRagioni != null)
                {
                    XmlNode ragioni = amministrazione.AppendChild(doc.CreateElement("RAGIONITRASMISSIONE"));

                    foreach (System.Data.DataRow rowRagione in dataSetRagioni.Tables["RAGIONI"].Rows)
                    {
                        XmlNode ragione = ragioni.AppendChild(doc.CreateElement("RAGIONE"));
                        ragione.AppendChild(doc.CreateElement("CODICE")).InnerText = rowRagione["VAR_DESC_RAGIONE"].ToString().ToUpper();
                        ragione.AppendChild(doc.CreateElement("TIPO")).InnerText = rowRagione["CHA_TIPO_RAGIONE"].ToString();
                        ragione.AppendChild(doc.CreateElement("VISIBILITA")).InnerText = rowRagione["CHA_VIS"].ToString();
                        ragione.AppendChild(doc.CreateElement("DIRITTI")).InnerText = rowRagione["CHA_TIPO_DIRITTI"].ToString();
                        ragione.AppendChild(doc.CreateElement("DESTINATARIO")).InnerText = rowRagione["CHA_TIPO_DEST"].ToString();
                        ragione.AppendChild(doc.CreateElement("RISPOSTA")).InnerText = rowRagione["CHA_RISPOSTA"].ToString();
                        ragione.AppendChild(doc.CreateElement("TIPORISPOSTA")).InnerText = rowRagione["CHA_TIPO_RISPOSTA"].ToString();
                        ragione.AppendChild(doc.CreateElement("EREDITA")).InnerText = rowRagione["CHA_EREDITA"].ToString();
                        ragione.AppendChild(doc.CreateElement("NOTE")).InnerText = rowRagione["VAR_NOTE"].ToString();
                        ragione.AppendChild(doc.CreateElement("NOTIFICA")).InnerText = rowRagione["VAR_NOTIFICA_TRASM"].ToString();
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
