using System;
using System.Collections.Generic;
using System.IO;
using MText.Exceptions;
using MText.Helper;
using MText.DomainObjects;
using System.Threading;
using System.Configuration;
using log4net;

namespace MText
{
    /// <summary>
    /// Questa classe si occupa di interfacciarsi con i servizi web esposti da M/TEXT CS
    /// fornendo l'accezzo alle funzionalità CRUD di M/TEXT CS
    /// </summary>
    public class MTextModelProvider
    {
        // Instanza del logger
        private ILog logger = LogManager.GetLogger(typeof(MTextModelProvider));

        /// <summary>
        /// Istanza del proxy dei servizi web offerti da MText / CS
        /// </summary>
        private IntegrationAdapterService integrationAdapterService;

        /// <summary>
        /// Url del server in cui è installato il server M/Text
        /// </summary>
        private String mTextAppletUrl;

        /// <summary>
        /// Funzione per l'inizializzazione dell'interface verso MText
        /// </summary>
        /// <param name="mtextServiceUrl">Url dei servizi web di MText</param>
        public MTextModelProvider(String mTextServiceUrl)
        {
            // Instanziazione del proxy, impostazione dell'url del servizio e
            // dell'url del server M/Text
            this.integrationAdapterService = new IntegrationAdapterService();
            this.integrationAdapterService.Url = mTextServiceUrl;
            this.mTextAppletUrl = new Uri(mTextServiceUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
            
        }


        /// <summary>
        /// Funzione per il reperimento della lista di modelli definiti tramite lo strumento
        /// di amministrazione di M/Text CS
        /// </summary>
        /// <returns>Lista dei databindings definiti dal workbench M/Text CS</returns>
        public List<ModelInfo> GetModels()
        {
            List<ModelInfo> models = new List<ModelInfo>();

            // Definizione request
            getDocumentTypes request = new getDocumentTypes();
            request.arg0 = new configurationProperty[0];

            // Invio richiesta e analisi risultati
            String[] documentTypes;

            try
            {
                documentTypes = this.integrationAdapterService.getDocumentTypes(request);

                // Conversione oggetti
                foreach (String model in documentTypes)
                    models.Add(MTextHelper.DecodeBindingInformation(model));
                
            }
            catch (Exception e)
            {
                throw new DataBindingsListRetriveException(e);
            }

            return models;
        }

        /// <summary>
        /// Funzione per la creazione di un XML che può essere utilizzato durante la definizione
        /// del template in M/Text
        /// </summary>
        /// <param name="templateName">Nome del template per cui generare un esempio di data source</param>
        /// <param name="mTextFields">Lista di coppie Etichetta - Valore con le informazioni su nome e valore assunto dai campi profilati M/Text</param>
        /// <returns></returns>
        public Byte[] GetXmlExample(String templateName, List<LabelValuePair> mTextFields)
        {
            Byte[] dataSourceExample = null;

            try
            {
                dataSourceExample = MTextHelper.GetDataSource(templateName, mTextFields);
            }
            catch (Exception e)
            {
                this.logger.Debug(
                    String.Format(
                        "Errore durante la generazione del data source di esempio per la tipologia {0}",
                        templateName),
                    e);
            }

            return dataSourceExample;
        }

        /// <summary>
        /// Funzione per la creazione di un documento
        /// </summary>
        /// <param name="fullQualifiedName">Nome da assegnare al documento</param>
        /// <param name="mTextFieldsInfo">Lista di coppie Etichetta - Valore da utilizzare per la creazione del documento M/Text</param>
        /// <param name="databinding">Modello da utilizzare</param>
        /// <returns>Id del documento creato</returns>
        public String CreateDocument(String fullQualifiedName, List<LabelValuePair> mTextFieldsInfo, String databinding)
        {
            // Id del documento da restituire
            String documentID = String.Empty;

            String bindingName = Path.GetFileNameWithoutExtension(databinding);

            // Generazione del data source a partire dalla lista di campi profilati
            Byte[] dataSource = MTextHelper.GetDataSource(bindingName, mTextFieldsInfo);

            // Creazione argomento con le informazioni sul datasource
            namedData[] namedData = new namedData[]
            {
                new namedData()
                    {
                        data = dataSource,
                        name = bindingName
                    }
            };

            // Invocazione del servizio per la creazione del documento e
            // restituzione dell'id del documento creato
            try
            {
                createDocumentFromBindingResponse response = this.integrationAdapterService.createDocumentFromBinding(
                    new createDocumentFromBinding()
                    {
                        arg0 = fullQualifiedName,
                        arg1 = databinding,
                        arg2 = namedData,
                        arg3 = new configurationProperty[0]
                    });

                documentID = response.@return;
            }
            catch (Exception e)
            {
                throw new DocumentCreationException(e);
            }

            return documentID;
        }

        /// <summary>
        /// Funzione per l'export di un documento MText.
        /// </summary>
        /// <param name="fullQualifiedName">Nome del documento da esportare</param>
        /// <param name="mimeType">Mime type del documento da esportare</param>
        /// <returns>Documento esportato</returns>
        public Byte[] ExportDocument(String fullQualifiedName, String mimeType)
        {
            Byte[] exportedDocument;

            try
            {
                // Esportazione del documento nel formato richiesto
                exportDocumentResponse response = this.integrationAdapterService.exportDocument(
                    new exportDocument()
                    {
                        arg0 = fullQualifiedName,
                        arg1 = mimeType,
                        arg2 = new configurationProperty[0]
                    });

                exportedDocument = response.@return;
            }
            catch (Exception e)
            {
                throw new ExportException(e);
 
            }

            return exportedDocument;
            
        }

        /// <summary>
        /// Funzione per il reperimento dell'url di un documento
        /// </summary>
        /// <param name="fullQualifiedName">Nome del documento da reperire</param>
        /// <returns>Url da aprire per visualizzare l'applet per la modifica del documento</returns>
        public String GetDocumentEditUrl(String fullQualifiedName)
        {
            String documentUrl;

            try
            {

                editDocumentResponse response = this.integrationAdapterService.editDocument(
                    new editDocument()
                        {
                            arg0 = fullQualifiedName,
                            arg1 = this.mTextAppletUrl,
                            arg2 = new configurationProperty[0]
                        });

                documentUrl = response.@return;
            }
            catch (Exception e)
            {
                throw new DocumentRetrivingException(e);
            }

            return documentUrl;

        }

        /// <summary>
        /// Funzione per la rimozione di un documento
        /// </summary>
        /// <param name="fullQualifiedName">Nome del documento da cancellare</param>
        public void DeleteDocument(String fullQualifiedName)
        {
            // Creazione parametro per l'eliminazione del documento
            deleteDocument deleteDocument = new deleteDocument()
                {
                    arg0 = fullQualifiedName,
                    arg1 = new configurationProperty[0]
                };

            try
            {
                deleteDocumentResponse response = this.integrationAdapterService.deleteDocument(deleteDocument);
            }
            catch (Exception e)
            {
                throw new DocumentDeleteException(e);
            }

        }
    }
}
