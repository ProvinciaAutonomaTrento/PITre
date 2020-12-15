using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using log4net;

namespace BusinessLogic.Modelli.AsposeModelProcessor
{

    public class PdfModelProcessor : BaseDocModelProcessor
    {
        private static ILog logger = LogManager.GetLogger(typeof(PdfModelProcessor));

        Aspose.Pdf.License license;
        Aspose.Pdf.Facades.Form form;

        public PdfModelProcessor(byte[] pdfContent)
        {
            license = new License();
            DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor csmp = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

            byte[] licenseContent = csmp.GetLicense("ASPOSE");
            if (licenseContent != null)
            {
                System.IO.MemoryStream licenseStream = new System.IO.MemoryStream(licenseContent, 0, licenseContent.Length);
                license.SetLicense(licenseStream);
                licenseStream.Close();
            }

            MemoryStream ms = new MemoryStream(pdfContent);
            form = new Form();
            form.BindPdf(ms);
        }

        public override DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request)
        {
            throw new NotImplementedException();
        }

        protected override DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public DocsPaVO.ProfilazioneDinamica.Templates PopolaTemplateIstanzaProcedimenti(string nomeTipologia)
        {
            logger.Debug("BEGIN");
            logger.Debug("Ricerca tipologia fascicolo");
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(nomeTipologia);
            if (template != null && !string.IsNullOrEmpty(template.ID_TIPO_FASC))
            {
                // Caricamento licenza
                //DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor csmp = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

                //Aspose.Pdf.License lic = new License();
                //byte[] licenseContent = csmp.GetLicense("ASPOSE");
                //if (licenseContent != null)
                //{
                //    System.IO.MemoryStream licenseStream = new System.IO.MemoryStream(licenseContent, 0, licenseContent.Length);
                //    lic.SetLicense(licenseStream);
                //    licenseStream.Close();
                //}

                // Caricamento file pdf da byte array
                //MemoryStream ms = new MemoryStream(pdfContent);
                //Form form = new Form();
                //form.BindPdf(ms);

                // Ricerca e popolamento dei campi profilati
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                {
                    if (form.FieldNames.Contains(ogg.DESCRIZIONE))
                    {
                        string value = string.Empty;
                        switch (form.GetFieldType(ogg.DESCRIZIONE))
                        {
                            case FieldType.Text:
                                value = form.GetField(ogg.DESCRIZIONE);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    ogg.VALORE_DATABASE = value;
                                }
                                break;
                            case FieldType.ComboBox:
                                value = form.GetField(ogg.DESCRIZIONE);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    ogg.VALORE_DATABASE = value;
                                }
                                break;
                            case FieldType.CheckBox:
                                value = form.GetField(ogg.DESCRIZIONE);                               
                                break;
                            case FieldType.Radio:
                                value = form.GetButtonOptionCurrentValue(ogg.DESCRIZIONE);
                                var options = form.GetButtonOptionValues(ogg.DESCRIZIONE);
                                break;
                            default:
                                break;
                        }
                    }                    
                }                                
            }
            else
            {
                logger.Debug("Tipologia non trovata");
                template = null;
            }

            logger.Debug("END");
            return template;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates PopolaTemplateDocumento(string nomeTipologia, string idAmm)
        {
            logger.Debug("BEGIN");
            logger.Debug("Ricerca tipologia documento");
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(nomeTipologia, idAmm);
            if (template != null && !string.IsNullOrEmpty(template.ID_TIPO_ATTO))
            {
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                {
                    if (form.FieldNames.Contains(ogg.DESCRIZIONE))
                    {
                        string value = string.Empty;
                        switch (form.GetFieldType(ogg.DESCRIZIONE))
                        {
                            case FieldType.Text:
                                value = form.GetField(ogg.DESCRIZIONE);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    ogg.VALORE_DATABASE = value;
                                }
                                break;
                            case FieldType.ComboBox:
                                value = form.GetField(ogg.DESCRIZIONE);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    ogg.VALORE_DATABASE = value;
                                }
                                break;
                            case FieldType.CheckBox:
                                value = form.GetField(ogg.DESCRIZIONE);
                                break;
                            case FieldType.Radio:
                                value = form.GetButtonOptionCurrentValue(ogg.DESCRIZIONE);
                                var options = form.GetButtonOptionValues(ogg.DESCRIZIONE);
                                break;
                            default:
                                break;
                        }
                    }        
                }
            }
            else
            {
                logger.Debug("Tipologia documento non trovata");
                template = null;
            }

            logger.Debug("END");
            return template;
        }
    }
}
