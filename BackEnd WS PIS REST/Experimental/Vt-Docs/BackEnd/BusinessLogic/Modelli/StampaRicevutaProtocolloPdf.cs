using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp;
using iTextSharp.text.pdf;
using System.Web;
using System.Web.UI;
using System.IO;
using DocsPaVO;


namespace BusinessLogic.Modelli
{
    /// <summary>
    /// 
    /// </summary>
    public class StampaRicevutaProtocolloPdf
    {
        private StampaRicevutaProtocolloPdf()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento Create(DocsPaVO.utente.InfoUtente userInfo, string idDocument)
        {
            byte[] content = null;
            DocsPaVO.documento.FileDocumento fdoc = new DocsPaVO.documento.FileDocumento();
            StampaRicevuteDocModelProcessor processor = new StampaRicevuteDocModelProcessor();
            DocsPaVO.documento.SchedaDocumento document = GetDocument(userInfo, idDocument);
            DocsPaVO.Modelli.ModelResponse response =
                        processor.ProcessModel(
                            new DocsPaVO.Modelli.ModelRequest 
                            { 
                                DocumentId = idDocument, 
                                UserInfo = userInfo,
                                ModelType = BaseDocModelProcessor.MODEL_STAMPA_RICEVUTA
                            });

        
            if (string.IsNullOrEmpty(response.Exception))
            {
                
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    PdfStamper ps = null;

                    // read existing PDF document
                    PdfReader r = new PdfReader(
                        // optimize memory usage
                           // Reperimento del path del modello per la stampa della ricevuta di protocollo

                      new RandomAccessFileOrArray(GetModelPath(document)), null);
                    

                    ps = new PdfStamper(r, stream);

                    // retrieve properties of PDF form w/AcroFields object
                    AcroFields af = ps.AcroFields;
                    // fill in PDF fields by parameter:
                    // 1. field name
                    // 2. text to insert
                    af.SetField("AMMINISTRAZIONE", FindDocumentValue(BaseDocModelProcessor.DocumentCommonFields.AMMINISTRAZIONE, response.DocumentModel.KeyValuePairs));
                    af.SetField("DATA_ORA_PROTOCOLLO", FindDocumentValue(BaseDocModelProcessor.DocumentCommonFields.DATA_ORA_PROTOCOLLO, response.DocumentModel.KeyValuePairs));
                    af.SetField("NUMERO_PROTOCOLLO", FindDocumentValue(BaseDocModelProcessor.DocumentCommonFields.NUM_PROTOCOLLO, response.DocumentModel.KeyValuePairs));
                    af.SetField("SEGNATURA", FindDocumentValue(BaseDocModelProcessor.DocumentCommonFields.SEGNATURA, response.DocumentModel.KeyValuePairs));
                    af.SetField("OGGETTO", FindDocumentValue(BaseDocModelProcessor.DocumentCommonFields.OGGETTO, response.DocumentModel.KeyValuePairs));
                    // make resultant PDF read-only for end-user
                    ps.FormFlattening = true;
                   
                   
                    
                    //stream.Position = 0;
                    //content = new byte[stream.Length];
                    //stream.Read(content, 0, content.Length);
                    
                     ps.Close();
                     fdoc.content = stream.ToArray();
                     stream.Close();
                    
                    // ps.Close();
                    //stream.Close();
                    //stream.Flush();
                    
                    

                   
                   
                }

                return fdoc;
            }
            else
                throw new ApplicationException(response.Exception);
        }

        /// <summary>
        /// Ricerca valore del documento nelle chiavi predefinite
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private static string FindDocumentValue(string key, DocsPaVO.Modelli.ModelKeyValuePair[] pairs)
        {
             DocsPaVO.Modelli.ModelKeyValuePair pair = pairs.Where(e => e.Key == key).FirstOrDefault();

             if (pair != null)
                 return pair.Value;
             else
                 return string.Empty;
        }

        /// <summary>
        /// Reperimento del path del modello
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        private static  string GetModelPath(DocsPaVO.documento.SchedaDocumento document)
        {
            if (document.registro == null)
                throw new ApplicationException("Nessun registro impostato per il documento");

            DocsPaVO.amministrazione.OrgRegistro registro = new DocsPaVO.amministrazione.OrgRegistro();
            registro.Codice = document.registro.codRegistro;
            registro.CodiceAmministrazione = document.registro.codAmministrazione;

            // Reperimento del path del modello per la stampa ricevuta del registro
            string modelPath = Amministrazione.RegistroManager.GetPathModelloStampaRicevuta(registro, "pdf");
           // modelPath = modelPath.Replace(".rtf", ".pdf");
            if (!File.Exists(modelPath))
                // Se non è stato impostato alcun modello per la stampa per il registro di protocollo,
                // viene reperito il modello predefinito
                modelPath = string.Format(@"{0}\report\StampaRic\Ricevuta.pdf", AppDomain.CurrentDomain.BaseDirectory);

            return modelPath;
        }


        /// <summary>
        /// Reperimento della scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.SchedaDocumento GetDocument(DocsPaVO.utente.InfoUtente infoUtente, string id)
        {
            // Reperimento scheda documento
            DocsPaVO.documento.SchedaDocumento document = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, id, id);

            if (document == null)
                throw new ApplicationException(string.Format("Nessun documento trovato con id {0}", id));

            return document;
        }
    }
}
