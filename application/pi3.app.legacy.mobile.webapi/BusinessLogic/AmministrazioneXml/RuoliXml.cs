// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
    public class RuoliXml
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(RuoliXml));

        public bool ExportStructure(XmlDocument doc, XmlNode amministrazione, string idAmm)
        {
            bool result = true; //presume successo
            try
            {
                System.Data.DataSet dataSetRuoli;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetRuoli(out dataSetRuoli, idAmm);
                if (!result)
                {
                    throw new Exception();
                }

                if (dataSetRuoli != null)
                {
                    XmlNode ruolo;
                    XmlNode ruoli = amministrazione.AppendChild(doc.CreateElement("RUOLI"));
                    foreach (System.Data.DataRow rowRuolo in dataSetRuoli.Tables["RUOLI"].Rows)
                    {
                        ruolo = ruoli.AppendChild(doc.CreateElement("RUOLO"));
                        ruolo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowRuolo["VAR_DESC_RUOLO"].ToString();
                        ruolo.AppendChild(doc.CreateElement("CODICE")).InnerText = rowRuolo["VAR_CODICE"].ToString().ToUpper();
                        ruolo.AppendChild(doc.CreateElement("LIVELLO")).InnerText = rowRuolo["NUM_LIVELLO"].ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'esportazione dei ruoli", exception);
                result = false;
            }
            return result;
        }
    }
}
