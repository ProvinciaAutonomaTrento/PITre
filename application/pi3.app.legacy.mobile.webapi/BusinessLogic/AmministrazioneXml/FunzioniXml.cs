// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
    public class FunzioniXml
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(FunzioniXml));

        public bool ExportAnagraficaFunzioni(XmlDocument doc, XmlNode root)
        {
            bool result = true; //presume successo
            try
            {
                System.Data.DataSet dataSetFunzioniElementari;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetFunzioniElementari(out dataSetFunzioniElementari);
                if (!result)
                {
                    logger.Debug("Errore lettura anagrafica funzioni elementari");
                    throw new Exception();
                }

                if (dataSetFunzioniElementari != null)
                {
                    XmlNode funzioniElementari = root.AppendChild(doc.CreateElement("FUNZIONIELEMENTARI"));
                    foreach (System.Data.DataRow rowFunzioni in dataSetFunzioniElementari.Tables["FUNZIONIELEMENTARI"].Rows)
                    {
                        XmlNode funzioneElementare = funzioniElementari.AppendChild(doc.CreateElement("FUNZIONEELEMENTARE"));
                        funzioneElementare.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowFunzioni["VAR_DESC_FUNZIONE"].ToString();
                        funzioneElementare.AppendChild(doc.CreateElement("CODICE")).InnerText = rowFunzioni["COD_FUNZIONE"].ToString().ToUpper();
                        string tipoFunzFull = "Full";
                        string tipoFunz = rowFunzioni["CHA_TIPO_FUNZ"].ToString();
                        if (tipoFunz == "R")
                        {
                            tipoFunzFull = "Read";
                        }
                        if (tipoFunz == "W")
                        {
                            tipoFunzFull = "Write";
                        }
                        funzioneElementare.AppendChild(doc.CreateElement("ACCESSO")).InnerText = tipoFunzFull;
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'esportazione della anagrafica funzioni elementari", exception);
                result = false;
            }
            return result;
        }

        public bool ExportStructure(XmlDocument doc, XmlNode root)
        {
            bool result = true; //presume successo
            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                System.Data.DataSet dataSetTipi;
                //amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
                result = amministrazioneXml.Exp_GetTipiFunzione(out dataSetTipi);
                if (!result)
                {
                    logger.Debug("Errore lettura tipi funzioni");
                    throw new Exception();
                }

                if (dataSetTipi != null)
                {
                    XmlNode tipiFunzione = root.AppendChild(doc.CreateElement("TIPIFUNZIONE"));

                    foreach (System.Data.DataRow rowTipo in dataSetTipi.Tables["TIPIFUNZIONE"].Rows)
                    {

                        string idTipoFunzione = rowTipo["SYSTEM_ID"].ToString();

                        XmlNode tipo = tipiFunzione.AppendChild(doc.CreateElement("TIPO"));
                        XmlNode datiTipo = tipo.AppendChild(doc.CreateElement("DATI"));
                        datiTipo.AppendChild(doc.CreateElement("CODICE")).InnerText = rowTipo["VAR_COD_TIPO"].ToString().ToUpper();
                        datiTipo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowTipo["VAR_DESC_TIPO_FUN"].ToString();

                        //esportazione funzioni
                        System.Data.DataSet dataSetFunzioni;
                        result = amministrazioneXml.Exp_GetFunzioni(out dataSetFunzioni, idTipoFunzione);
                        if (!result)
                        {
                            logger.Debug("Errore lettura funzioni");
                            throw new Exception();
                        }

                        if (dataSetFunzioni != null)
                        {
                            XmlNode funzioni = tipo.AppendChild(doc.CreateElement("FUNZIONI"));
                            foreach (System.Data.DataRow rowFunzione in dataSetFunzioni.Tables["FUNZIONI"].Rows)
                            {
                                XmlNode funzione = funzioni.AppendChild(doc.CreateElement("FUNZIONE"));
                                funzione.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowFunzione["VAR_DESC_FUNZIONE"].ToString();
                                funzione.AppendChild(doc.CreateElement("CODICE")).InnerText = rowFunzione["COD_FUNZIONE"].ToString().ToUpper();
                                string tipoFunz = rowFunzione["ID_TIPO_FUNZIONE"].ToString();
                                string tipoFunzFull = "Full";
                                if (tipoFunz == "R")
                                {
                                    tipoFunzFull = "Read";
                                }
                                if (tipoFunz == "W")
                                {
                                    tipoFunzFull = "Write";
                                }
                                funzione.AppendChild(doc.CreateElement("ACCESSO")).InnerText = tipoFunzFull;
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'esportazione delle funzioni", exception);
                result = false;
            }
            return result;
        }

    }
}
