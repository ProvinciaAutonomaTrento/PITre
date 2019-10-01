using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MText;
using System.Configuration;
using MText.DomainObjects;
using SAAdminTool.DocsPaWR;
using System.IO;
using System.Threading;
using System.Text;

namespace SAAdminTool.utils
{
    public class MTextUtils
    {
        // Reference al Web Service
        static DocsPaWebService ws;

        static MTextUtils()
        {
            ws = new DocsPaWebService();
            ws.Timeout = Timeout.Infinite;
        }

        #region Public Members

        /// <summary>
        /// Funzione per la verifica di attivazione di M/Text
        /// </summary>
        /// <returns>True se è attiva l'integrazione M/Text</returns>
        public static bool IsActiveMTextIntegration()
        {
            bool isActive = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["MTextModelProvider"]);

            return isActive;
        }

        /// <summary>
        /// Funzione per la conversione della lista di oggetti custom in dictionary
        /// </summary>
        /// <param name="template">Template da utilizzare per la creazione del documento</param>
        /// <returns>Dizionario Etichetta-Valore</returns>
        public static List<LabelValuePair> CustomObject2Dictionary(Templates template)
        {
            List<LabelValuePair> converted = new List<LabelValuePair>();

            // Conversione degli oggetti
            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
                converted.Add(new LabelValuePair()
                    {
                        Label = obj.DESCRIZIONE,
                        Value = GetValueForCustomObject(obj.SYSTEM_ID, template)
                    });

            return converted;

        }

        /// <summary>
        /// Funzione per la generazione di dizionario con dati di esempio relativi ad un template
        /// </summary>
        /// <param name="customObjects">Lista dei campi profilati da sfruttare per la generazione del dizionario di esempio</param>
        /// <returns>Dizionario con coppie EtichettaCampo - Valore</returns>
        public static List<LabelValuePair> GetSampleLabelValueCollection(List<OggettoCustom> customObjects)
        {
            // Creazione della lista
            List<LabelValuePair> values = new List<LabelValuePair>();

            foreach (OggettoCustom obj in customObjects)
                values.Add(new LabelValuePair()
                    {
                        Label = obj.DESCRIZIONE,
                        Value = GetSampleValueForField(obj)
                    });

            return values;

        }

        /// <summary>
        /// Funzione per la generazione del full qualified name per un documento a partire dal suo
        /// system id
        /// </summary>
        /// <param name="id">Id del documento da sfruttare per la costruzione dell'FQN</param>
        /// <returns></returns>
        public static String Id2FullQualifiedName(String id)
        {
            return Path.Combine(ConfigurationManager.AppSettings["M_TEXT_DOC_ROOT_PATH"], id + "_*" );
        }

        /// <summary>
        /// Funzione per il reperimento di un nome da un full qualified name
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static String GetNameFromQualifiedName(String qualifiedName)
        {
            return Path.GetFileNameWithoutExtension(qualifiedName);
        }

        /// <summary>
        /// Funzione per il salvataggio del path del documento M/Text associato ad una data versione di 
        /// un documento
        /// </summary>
        /// <param name="versionId">Id della versione</param>
        /// <param name="docNumber">Numero del documento</param>
        /// <param name="fullQualifiedName">Path del documento M/Text da associare al documento</param>
        public static void SetMTextFullQualifiedName(String versionId, String docNumber, String fullQualifiedName)
        {
            try
            {
                ws.SetMTextFullQualifiedName(new MTextDocumentInfo() {
                    DocumentVersionId = versionId,
                    DocumentDocNumber = docNumber,
                    FullQualifiedName = fullQualifiedName
                });
            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);
            }
        }

        /// <summary>
        /// Funzione per il reperimento del path del documento M/Text associato ad una data versione di un
        /// documento
        /// </summary>
        /// <param name="versionId">Id della versione</param>
        /// <param name="docNumber">Numero del documento</param>
        /// <returns>Path del modello M/Text associato</returns>
        public static String GetMTextFullQualifiedName(String versionId, String docNumber)
        {
            try
            {
                return ws.GetMTextFullQualifiedName(new MTextDocumentInfo() {
                    DocumentVersionId = versionId, 
                    DocumentDocNumber = docNumber
                }).FullQualifiedName;
            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Funzione per la restituzione di un valore di esempio per un campo
        /// </summary>
        /// <param name="customObject">Oggetto custom per cui generare un valore di esempio</param>
        /// <returns>Valore di esempio</returns>
        private static String GetSampleValueForField(OggettoCustom customObject)
        {
            String sampleValue = String.Empty;

            switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
            {
                case "CASELLADISELEZIONE":
                    sampleValue = "Item1; Item2";
                    break;

                case "CORRISPONDENTE":
                    sampleValue = "Name Surname";
                    break;

                case "LINK":
                    sampleValue = "http://fakesite.com";
                    break;

                default:
                    sampleValue = "Test Value";
                    break;
            }

            return sampleValue;

        }

        /// <summary>
        /// Funzione per la ricerca del valore da assegnare al campo di profilazione dinamica specificato
        /// </summary>
        /// <param name="customObjectId">Id del campo di cui recuperare il valore</param>
        /// <param name="template">Il template associato da cui prelevare il valore</param>
        /// <returns>Il valore da assegnare al campo</returns>
        public static String GetValueForCustomObject(int customObjectId, Templates template)
        {
            // L'oggetto custom da cui prelevare i risultati
            OggettoCustom customObject;

            // Il testo da restituire
            StringBuilder toReturn;

            // Inizializzazione del valore da restituire
            toReturn = new StringBuilder();

            // Prelevamento dell'oggetto custom con l'etichetta specificata
            customObject = template.ELENCO_OGGETTI.Where(e => e.SYSTEM_ID == customObjectId).FirstOrDefault();
            
            // Se l'oggetto custom è una casella di selezione, vengono mergiati i valori
            // selezionati altrimenti il valore da restituire è il valore associato al campo
            if (customObject != null)
            {
                switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
                {
                    case "CASELLADISELEZIONE":
                        foreach (String value in customObject.VALORI_SELEZIONATI)
                            if (!string.IsNullOrEmpty(value))
                                toReturn.Append(value + "; ");

                        if (toReturn.Length > 0)
                            toReturn.Remove(toReturn.Length - 2, 2);
                        break;

                    case "CORRISPONDENTE":
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemIDDisabled(null, customObject.VALORE_DATABASE);
                        if (corr != null)
                            toReturn.Append(corr.descrizione);
                        break;

                    case "LINK":
                        if (customObject.VALORE_DATABASE.Contains("||||"))
                        {
                            int stop = customObject.VALORE_DATABASE.IndexOf("||||");
                            toReturn.Append(customObject.VALORE_DATABASE.Substring(0, stop));
                        }
                        else
                            toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                    case "CONTATORESOTTOCONTATORE":
                        if (customObject.VALORE_DATABASE != null && !customObject.VALORE_DATABASE.Equals(""))
                            toReturn.Append(customObject.VALORE_DATABASE + "-" + customObject.VALORE_SOTTOCONTATORE);
                        break;

                    default:
                        toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                }
            }
     
            // Restituzione del valore
            return toReturn.ToString();
        }

        #endregion


    }
}