using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DocsPaDB;
using DocsPaVO.areaConservazione;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace DocsPaConservazione
{
    /// <summary>
    /// Questo oggetto si occupa di scrivere i metadati su un file XML e fornisce funzioni per eventuali
    /// trasformazioni/sostituzioni dei metadati letti da DB...
    /// </summary>
    public class InfoDocXml
    {
        private ILog logger = LogManager.GetLogger(typeof(InfoDocXml));
             /// <summary>
        /// Scrive su file XML i metadati congelati sul DB nella relativa istanza di conservazione nel campo CLOB
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public bool saveMetadati(string systemID, string fullName)
        {
            bool result = false;
            TextWriter textWr = null;
            XmlTextWriter XmlWr = null;
            try
            {
                DBProvider dbProvider = new DBProvider();
                string CLOB = dbProvider.GetLargeText("DPA_ITEMS_CONSERVAZIONE", systemID, "VAR_XML_METADATI");
                string pathXml = fullName + ".xml";
                textWr = new StreamWriter(pathXml, false, Encoding.UTF8);
                XmlWr = new XmlTextWriter(textWr);
                //XmlTextReader XmlRd = new XmlTextReader(CLOB, XmlNodeType.Document, null);
                if (!string.IsNullOrEmpty(CLOB))
                {
                    XmlWr.WriteRaw(CLOB);
                    result = true;
                }
                else
                    result = false;
            } catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (textWr != null)
                {
                    XmlWr.Flush();
                    textWr.Flush();
                    XmlWr.Close();
                    textWr.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Scrive su file XML i metadati da una stringa
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="fullName"></param>
        /// <param name="contenutoXML"></param>
        /// <returns></returns>
        public bool saveMetadatiString(string fullName,string contenutoXML)
        {
            bool result = false;
            TextWriter textWr = null;
            XmlTextWriter XmlWr = null;
            try
            {
                string pathXml = fullName + ".xml";
                textWr = new StreamWriter(pathXml, false, Encoding.UTF8);
                XmlWr = new XmlTextWriter(textWr);
            

                if (!string.IsNullOrEmpty(contenutoXML))
                {
                    XmlWr.WriteRaw(contenutoXML);
                    result = true;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (textWr != null)
                {
                    XmlWr.Flush();
                    textWr.Flush();
                    XmlWr.Close();
                    textWr.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Restituisce la dimensione dell'equivalente file XML che rappresenta la serializzazione dell'oggetto 
        /// passato come parametro
        /// </summary>
        /// <param name="schDoc">è l'oggetto scheda documento che dobbiamo serializzare</param>
        /// <param name="systemID">id di conservazione</param>
        /// <returns></returns>
        public string serializeScheda(DocsPaVO.documento.SchedaDocumento schDoc, string systemID)
        {
            string metadati = string.Empty;
            string result = "-1";
            MemoryStream memoryWriter = null;
            try
            {
                Metadati dati = new Metadati(schDoc);
                //Sostituisco al system_id il nome e cognome
                if (dati.protocollatore != null)
                {
                    if (!string.IsNullOrEmpty(dati.protocollatore.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.protocollatore.Nome_Cognome = cm.getFullName(dati.protocollatore.Nome_Cognome);
                    }
                }
                if (dati.creatoreDocumento != null)
                {
                    if (!string.IsNullOrEmpty(dati.creatoreDocumento.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.creatoreDocumento.Nome_Cognome = cm.getFullName(dati.creatoreDocumento.Nome_Cognome);
                    }
                }

                memoryWriter = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Metadati));
                serializer.Serialize(memoryWriter, dati);

                //Devo tornare all'inizio del MemoryStream per leggerne il contenuto!!!
                memoryWriter.Seek(0, SeekOrigin.Begin);
                result = memoryWriter.Length.ToString();
                metadati = new StreamReader(memoryWriter).ReadToEnd();

                //Inserisco i metadati XML nel DB nel campo CLOB
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DBProvider dbProvider = new DBProvider();
                    if (!dbProvider.SetLargeText("DPA_ITEMS_CONSERVAZIONE", systemID, "VAR_XML_METADATI", metadati))
                        throw new Exception("Errore nell'inserimento dei metadati nel DB");

                    transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (memoryWriter != null)
                {
                    memoryWriter.Flush();
                    memoryWriter.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Restituisce la dimensione dell'equivalente file XML che rappresenta la serializzazione dell'oggetto 
        /// passato come parametro
        /// </summary>
        /// <param name="schDoc">è l'oggetto scheda documento che dobbiamo serializzare</param>
        /// <param name="systemID">id di esibizione</param>
        /// <returns></returns>
        public string serializeSchedaEsib(DocsPaVO.documento.SchedaDocumento schDoc, string systemID)
        {
            string metadati = string.Empty;
            string result = "-1";
            MemoryStream memoryWriter = null;
            try
            {
                Metadati dati = new Metadati(schDoc);
                //Sostituisco al system_id il nome e cognome
                if (dati.protocollatore != null)
                {
                    if (!string.IsNullOrEmpty(dati.protocollatore.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.protocollatore.Nome_Cognome = cm.getFullName(dati.protocollatore.Nome_Cognome);
                    }
                }
                if (dati.creatoreDocumento != null)
                {
                    if (!string.IsNullOrEmpty(dati.creatoreDocumento.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.creatoreDocumento.Nome_Cognome = cm.getFullName(dati.creatoreDocumento.Nome_Cognome);
                    }
                }

                memoryWriter = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Metadati));
                serializer.Serialize(memoryWriter, dati);

                //Devo tornare all'inizio del MemoryStream per leggerne il contenuto!!!
                memoryWriter.Seek(0, SeekOrigin.Begin);
                result = memoryWriter.Length.ToString();
                metadati = new StreamReader(memoryWriter).ReadToEnd();

                //Inserisco i metadati XML nel DB nel campo CLOB
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DBProvider dbProvider = new DBProvider();
                    if (!dbProvider.SetLargeText("DPA_ITEMS_ESIBIZIONE", systemID, "VAR_XML_METADATI", metadati))
                        throw new Exception("Errore nell'inserimento dei metadati per esibizione nel DB");

                    transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (memoryWriter != null)
                {
                    memoryWriter.Flush();
                    memoryWriter.Close();
                }
            }
            return result;
        }

        #region CS 1.5 - F03_01
        /// <summary>
        /// Restituisce la dimensione dell'equivalente file XML che rappresenta la serializzazione dell'oggetto 
        /// passato come parametro
        /// </summary>
        /// <param name="schDoc">è l'oggetto scheda documento che dobbiamo serializzare</param>
        /// <returns></returns>
        public string serializeSchedaDoc(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string metadati = string.Empty;
            string result = "-1";
            MemoryStream memoryWriter = null;
            try
            {
                Metadati dati = new Metadati(schDoc);
                //Sostituisco al system_id il nome e cognome
                if (dati.protocollatore != null)
                {
                    if (!string.IsNullOrEmpty(dati.protocollatore.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.protocollatore.Nome_Cognome = cm.getFullName(dati.protocollatore.Nome_Cognome);
                    }
                }
                if (dati.creatoreDocumento != null)
                {
                    if (!string.IsNullOrEmpty(dati.creatoreDocumento.Nome_Cognome))
                    {
                        DocsPaConsManager cm = new DocsPaConsManager();
                        dati.creatoreDocumento.Nome_Cognome = cm.getFullName(dati.creatoreDocumento.Nome_Cognome);
                    }
                }

                memoryWriter = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Metadati));
                serializer.Serialize(memoryWriter, dati);

                //Devo tornare all'inizio del MemoryStream per leggerne il contenuto!!!
                memoryWriter.Seek(0, SeekOrigin.Begin);
                result = memoryWriter.Length.ToString();
                metadati = new StreamReader(memoryWriter).ReadToEnd();
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (memoryWriter != null)
                {
                    memoryWriter.Flush();
                    memoryWriter.Close();
                }
            }
            return result;
        }
        #endregion

    }
}
