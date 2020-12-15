using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace DocsPaConservazione
{
    /// <summary>
    /// Questa classe si occupa di generare e salvare su filesystem il file XML relativo all'archiviazione
    /// espone inoltre i metodi di put e get del file per consentire la firma digitale dell'XML.
    /// </summary>
    public class InfoFolderXml
    {
        private ILog logger = LogManager.GetLogger(typeof(InfoFolderXml));
        /// <summary>
        /// Questo metodo costruisce il file XML generale che verrà sottoposto a firma del responsabile
        /// della conservazione
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="path_relativo"></param>
        /// <param name="root_conservazione"></param>
        /// <param name="schDoc"></param>
        /// <param name="infoCons"></param>
        /// <param name="isAllegato"></param>
        /// <param name="codFasc"></param>
        /// <returns></returns>
        public bool setInfoFolderXml(DocsPaVO.documento.FileRequest objFileRequest, string path_relativo, string root_conservazione, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.areaConservazione.InfoConservazione infoCons, bool isAllegato, string codFasc)
        {
            bool result = false;
            // leggo l'impronta del documento associato alla FileRequest passata come parametro
            string impronta = "";

            TextWriter textwriter = null;
            XmlTextWriter writer = null;
            TextWriter textwriter2 = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                doc.GetImpronta(out impronta, objFileRequest.versionId, objFileRequest.docNumber);

                //Verifica dati
                string segnatura = string.Empty;
                if (schDoc.protocollo != null)
                    segnatura = schDoc.protocollo.segnatura;
                string oggetto = string.Empty;
                if (schDoc.oggetto != null)
                    oggetto = schDoc.oggetto.descrizione;

                bool flag = File.Exists(root_conservazione);
                if (!flag)
                {
                    textwriter = new StreamWriter(root_conservazione, true, Encoding.UTF8);
                    writer = new XmlTextWriter(textwriter);
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("IDCONSERVAZIONE_" + infoCons.SystemID);
                    writer.WriteElementString("DATA_SOTTOSCRIZIONE", infoCons.Data_Invio);
                    writer.WriteElementString("NOTE", infoCons.Note);
                    writer.WriteElementString("DESCRIZIONE", infoCons.Descrizione);
                    writer.WriteElementString("TIPO_CONSERVAZIONE", infoCons.TipoConservazione);
                    //writer.WriteStartElement("DOCNUMBER_" + objFileRequest.docNumber);
                    writer.WriteStartElement("DOC");
                    //writer.WriteValue(objFileRequest.docNumber);
                    writer.WriteElementString("NUMBER", objFileRequest.docNumber);
                    writer.WriteElementString("Impronta", impronta);
                    writer.WriteElementString("File", path_relativo);
                    writer.WriteElementString("Segnatura", segnatura);
                    writer.WriteElementString("Oggetto", oggetto);
                    writer.WriteElementString("Codice_Fascicolo", codFasc);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    result = true;
                }
                else
                {
                    XmlDocument docXML = new XmlDocument();
                    docXML.Load(root_conservazione);
                    XmlNode root = docXML.DocumentElement;
                    if (!isAllegato)
                    {
                        //XmlElement newDoc = docXML.CreateElement("DOCNUMBER_" + objFileRequest.docNumber);
                        XmlElement newDoc = docXML.CreateElement("DOC");
                        XmlElement newData = docXML.CreateElement("NUMBER");
                        newData.InnerText = objFileRequest.docNumber;
                        newDoc.AppendChild(newData);
                        //XmlElement newData = docXML.CreateElement("Impronta");
                        newData = docXML.CreateElement("Impronta");
                        newData.InnerText = impronta;
                        newDoc.AppendChild(newData);
                        newData = docXML.CreateElement("File");
                        newData.InnerText = path_relativo;
                        newDoc.AppendChild(newData);
                        newData = docXML.CreateElement("Segnatura");
                        newData.InnerText = segnatura;
                        newDoc.AppendChild(newData);
                        newData = docXML.CreateElement("Oggetto");
                        newData.InnerText = oggetto;
                        newDoc.AppendChild(newData);
                        newData = docXML.CreateElement("Codice_Fascicolo");
                        newData.InnerText = codFasc;
                        newDoc.AppendChild(newData);
                        XmlNodeList rootList = docXML.SelectNodes("/IDCONSERVAZIONE_" + infoCons.SystemID);
                        if (rootList.Count > 0)
                        {
                            rootList[0].AppendChild(newDoc);
                        }
                    }
                    else
                    {
                        //Recupero l'oggetto dell'allegato
                        DocsPaConsManager cm = new DocsPaConsManager();
                        string descrizione = cm.getDescrizioneAllegato(objFileRequest.docNumber);

                        //Devo navigare fino ai nodi dove inserire gli allegati
                        //string str_cerca = "/IDCONSERVAZIONE_" + infoCons.SystemID + "/DOCNUMBER_" + schDoc.docNumber;
                        string str_cerca = "//IDCONSERVAZIONE_" + infoCons.SystemID + "/DOC[NUMBER = '" + schDoc.docNumber + "']";
                        XmlNodeList nodeList = docXML.SelectNodes(str_cerca);
                        XmlNode nodoDoc = null;
                        //Ciclo per inserire gli allegati nei relativi documenti
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            nodoDoc = nodeList[i];
                            //XmlElement newAllegato = docXML.CreateElement("AllegatoNum_" + objFileRequest.docNumber);
                            XmlElement newAllegato = docXML.CreateElement("Allegato");
                            XmlElement Dati = docXML.CreateElement("Numero");
                            Dati.InnerText = objFileRequest.docNumber;
                            newAllegato.AppendChild(Dati);
                            //XmlElement Dati = docXML.CreateElement("Impronta");
                            Dati = docXML.CreateElement("Impronta");
                            Dati.InnerText = impronta;
                            newAllegato.AppendChild(Dati);
                            Dati = docXML.CreateElement("Descrizione");
                            Dati.InnerText = descrizione;
                            newAllegato.AppendChild(Dati);
                            Dati = docXML.CreateElement("File");
                            Dati.InnerText = path_relativo;
                            newAllegato.AppendChild(Dati);
                            str_cerca = str_cerca + "/Allegato[Numero = '" + objFileRequest.docNumber + "']";
                            //Aggiungo l'allegato solo se non l'ho già fatto
                            if (nodoDoc.SelectNodes(str_cerca).Count < 1)
                            {
                                nodoDoc.AppendChild(newAllegato);
                            }
                        }
                    }
                    textwriter2 = new StreamWriter(root_conservazione, false, Encoding.UTF8);
                    docXML.Save(textwriter2);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                string err = "Errore nella scrittura del file XML dei metadati." + ex.Message;
                logger.Debug(err);
            }
            finally
            {
                if (textwriter != null)
                {
                    writer.Flush();
                    textwriter.Flush();
                    writer.Close();
                    textwriter.Close();
                }

                if (textwriter2 != null)
                {
                    textwriter2.Flush();
                    textwriter2.Close();
                }
            }
            return result;
        }

    }
}
