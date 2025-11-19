// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Modelli
{
    public class StampaRicevutaProtocolloPdf
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento Create(DocsPaVO.utente.InfoUtente userInfo, string idDocument)
        {
#if false  //qui dentro si usa itextsharp, quindi inutile continuare nella conversione
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

#endif        
            return null;
        }
    }
}
