using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;

namespace LiveCycle
{
    public class LCServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(LCServices));
        public static DocsPaVO.LiveCycle.ProcessFormOutput processFormPdf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput)
        {
            try
            {
                //Istanzio il servizio
                ProcessFormService.ProcessFormService processFormPdf =  LCServicesManager.getProcessFormService();
                DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = new DocsPaVO.LiveCycle.ProcessFormOutput();

                if (processFormPdf != null)
                {
                    //Popolo il binaryData del BLOB
                    ProcessFormService.BLOB inDoc = new ProcessFormService.BLOB();
                    inDoc.binaryData = processFormInput.fileDocumentoInput.content;

                    //Invoco il processo per il file pdf
                    ProcessFormService.XML xmlFile = null;
                    
                    //ProcessFormService.PDFSignatureVerificationResult resultSignature = null;
                    //resultSignature = processFormPdf.invoke(inDoc, out xmlFile);

                    bool signature = processFormPdf.invoke(inDoc, out xmlFile);

                    processFormPdf.Dispose();

                    //Processo l'xml
                    if (xmlFile != null && xmlFile.document != null)
                    {
                        //Leggo l'xml restituito
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlFile.document.ToString());

                        //Processo la tipologia di documento
                        DocumentiManager.compilaTipologiaDocumento(infoUtente, processFormInput, xmlDoc, ref processFormOutput);                                
                            
                        //Processo i campi standard del documento
                        DocumentiManager.compilaCampiDocumento(infoUtente, processFormInput, xmlDoc, ref processFormOutput);

                        /*
                        //Processo i campi nascosti della form                    
                        DocumentiManager.compilaCampiNascosti(infoUtente, xmlDoc, ref processFormOutput);
                        */
                    }

                    //Verifico se il documento è firmato o meno
                    //if (resultSignature != null && resultSignature.signatureStatus == ProcessFormService.PDFSignatureStatus.VALIDANDUNMODIFIED)
                    //    DocumentiManager.setFirmaDocumento(infoUtente, resultSignature, ref processFormOutput);                    
                    if(signature)
                        DocumentiManager.setFirmaDocumento(infoUtente, ref processFormOutput);                    
                    
                }
                else
                {
                    return null;
                }

                return processFormOutput;
            }
            catch (Exception e)
            {
                logger.Error("Errore in LiveCycle  - LCServices - metodo: processFormPdf()", e);
                return null;
            }
        }

        public static DocsPaVO.documento.FileDocumento generatePdfService(DocsPaVO.documento.FileDocumento fileDocumento)
        {
            try
            {
                //Istanzio il servizio
                GeneratePdfService.GeneratePDFServiceService generatePdf = LCServicesManager.getGeneratePdfService();

                if (generatePdf != null)
                {
                    //Popolo il binaryData del BLOB
                    GeneratePdfService.BLOB inDoc = new GeneratePdfService.BLOB();
                    inDoc.binaryData = fileDocumento.content;

                    //Sepcifico le opzioni PDF e Security
                    //String adobePDFSettings = "Standard";
                    //String securitySettings = "No Security";
                    //String fileTypeSettings = "Standard";
                    String adobePDFSettings = "PDFA1b 2005 CMYK";
                    String securitySettings = "No Security";
                    String fileTypeSettings = "Standard PITRE";
                    //String fileTypeSettings = "Standard OCR";
                    byte[] resultBinaryFile = null;

                    //Converto in pdf
                    GeneratePdfService.mapItem[] createPDFResults = generatePdf.CreatePDF(inDoc, fileDocumento.name, fileTypeSettings, adobePDFSettings, securitySettings, null, null);
                    generatePdf.Dispose();

                    //Recupero il file convertito in pdf
                    for (int count = 0; count < createPDFResults.Length; ++count)
                    {
                        //Recupero un elemento dalla mappa 				
                        GeneratePdfService.mapItem mapEntry = createPDFResults[count];
                        String mapKey = mapEntry.key as String;

                        //La chiave della mappa del documento convertito sarà "ConvertedDoc" 				
                        if (mapKey.Equals("ConvertedDoc"))
                        {
                            GeneratePdfService.BLOB resultBlob = mapEntry.value as GeneratePdfService.BLOB;
                            resultBinaryFile = resultBlob.binaryData;
                        }
                    }

                    fileDocumento.fullName = System.IO.Path.GetFileNameWithoutExtension(fileDocumento.fullName) + ".pdf";
                    fileDocumento.name = System.IO.Path.GetFileNameWithoutExtension(fileDocumento.name) + ".pdf";
                    fileDocumento.contentType = "application/pdf";
                    fileDocumento.content = resultBinaryFile;

                    //Faillace, segnalazione tiket 266571
                    //Inserita l'estensione e modifica nome file orginale.
                    fileDocumento.estensioneFile = "pdf";
                    if (!String.IsNullOrEmpty(fileDocumento.nomeOriginale))
                        fileDocumento.nomeOriginale= System.IO.Path.GetFileNameWithoutExtension(fileDocumento.nomeOriginale) + ".pdf";
                    
                    return fileDocumento;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in LiveCycle  - LCServices - metodo: generatePdfService()", e);
                return null;
            }
        }

        public static DocsPaVO.LiveCycle.ProcessFormOutput processBarcodeForm(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput)
        {
            try
            {
                //Istanzio il servizio

                ProcessBarcodeForm.ProcessBarcodeFormService processBarcodeForm = LCServicesManager.getProcessBarcodeFormService();
                DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput = new DocsPaVO.LiveCycle.ProcessFormOutput();

                if (processBarcodeForm != null)
                {
                    //Popolo il binaryData del BLOB
                    ProcessBarcodeForm.BLOB inDoc = new ProcessBarcodeForm.BLOB();
                    inDoc.binaryData = processFormInput.fileDocumentoInput.content;

                    //Invoco il processo per il file pdf
                    ProcessBarcodeForm.XML xmlFile = null;
                    xmlFile = processBarcodeForm.invoke(inDoc);
                    processBarcodeForm.Dispose();

                    //Processo l'xml
                    if (xmlFile != null && xmlFile.document != null)
                    {
                        //Leggo l'xml restituito
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlFile.document.ToString());

                        //Processo la tipologia di documento
                        DocumentiManager.compilaTipologiaDocumento(infoUtente, processFormInput, xmlDoc, ref processFormOutput);

                        //Processo i campi standard del documento
                        DocumentiManager.compilaCampiDocumento(infoUtente, processFormInput, xmlDoc, ref processFormOutput);

                        /*
                        //Processo i campi nascosti della form                    
                        DocumentiManager.compilaCampiNascosti(infoUtente, xmlDoc, ref processFormOutput);
                        */
                    }
                }
                else
                {
                    return null;
                }

                return processFormOutput;
            }
            catch (Exception e)
            {
               logger.Error("Errore in LiveCycle  - LCServices - metodo: processBarcodeForm()", e);
                return null;
            }
        }
    }
}
