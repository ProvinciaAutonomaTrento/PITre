using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Aspose.Pdf.Text;
using log4net;

namespace BusinessLogic.Modelli.AsposeModelProcessor
{

    public class PdfModelProcessor : BaseDocModelProcessor
    {
        private static ILog logger = LogManager.GetLogger(typeof(PdfModelProcessor));

        Aspose.Pdf.License license;
        Aspose.Pdf.Facades.Form form;

        public PdfModelProcessor()
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
        }

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

        public DocsPaVO.ProfilazioneDinamica.Templates PopolaTemplateIstanzaProcedimenti(string idTemplate)
        {
            logger.Debug("BEGIN");
            logger.Debug("Ricerca tipologia fascicolo");
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);
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

        public DocsPaVO.ProfilazioneDinamica.Templates PopolaTemplateDocumento(string idTemplate)
        {
            logger.Debug("BEGIN");
            logger.Debug("Ricerca tipologia documento");
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
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

        public DocsPaVO.documento.FileDocumento CreaRicevuta(DocsPaVO.utente.InfoUtente userInfo, string idDocument, string text)
        {
            byte[] content = null;
            DocsPaVO.documento.FileDocumento doc = new DocsPaVO.documento.FileDocumento();

            Document pdf = new Document();
            pdf.Pages.Add();

            Aspose.Pdf.Text.TextFragment tf = new Aspose.Pdf.Text.TextFragment(text);

            pdf.Pages[1].Paragraphs.Add(tf);

            using (MemoryStream stream = new MemoryStream())
            {
                pdf.Save(stream);

                if (stream != null)
                {
                    doc.content = stream.ToArray();
                }

                stream.Close();
            }

            return doc;
        }

        #region REPORTISTICA
        public DocsPaVO.documento.FileDocumento CreaReportProcedimentoSingolo(DocsPaVO.Procedimento.Report.ReportProcedimentoRequest request, List<DocsPaVO.Procedimento.DettaglioProcedimento> list)
        {
            DocsPaVO.documento.FileDocumento doc = new DocsPaVO.documento.FileDocumento();

            Document pdf = new Document();
            Page page = pdf.Pages.Add();

            page.SetPageSize(PageSize.A4.Width, PageSize.A4.Height);
            page.PageInfo.Margin.Left = 10;
            page.PageInfo.Margin.Right = 10;
            page.PageInfo.Margin.Top = 90;
            page.PageInfo.Margin.Bottom = 10;
            page.PageInfo.IsLandscape = true;

            // estrazione parametri
            string nomeProcedimento = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(request.IdProcedimento).DESCRIZIONE;
            double durataMedia = BusinessLogic.Procedimenti.AnalisiManager.DurataMediaProcedimento(list);
            int procedimentiChiusi = BusinessLogic.Procedimenti.AnalisiManager.ProcedimentiChiusi(list);

            // Fasi
            Dictionary<string, double> durataFasi = BusinessLogic.Procedimenti.AnalisiManager.DurataMediaFasiProcedimento(list, request.IdProcedimento, request.IdAmm);

            page.Paragraphs.Add(this.AddTitolo("Dettaglio del procedimento"));
            page.Paragraphs.Add(this.AddSottotitolo(nomeProcedimento));
            if (!string.IsNullOrEmpty(request.Anno))
            {
                page.Paragraphs.Add(this.AddSottotitolo("Anno di riferimento: " + request.Anno));
            }

            Table t1 = this.AddTable();
            t1.Rows.Add(this.AddRow(new string[] { "Procedimenti attivati nel periodo selezionato", list.Count.ToString() }));
            t1.Rows.Add(this.AddRow(new string[] { "Procedimenti conclusi", procedimentiChiusi.ToString() }));

            Table t2 = this.AddTable();
            t2.Rows.Add(this.AddRow(new string[] { "Durata media Procedimento (giorni)", durataMedia < 0 ? "non disponibile" : durataMedia.ToString() }));

            if (durataFasi != null)
            {
                foreach (KeyValuePair<string, double> kvp in durataFasi)
                {
                    t2.Rows.Add(this.AddRow(new string[] { string.Format("Durata media {0} (giorni)", kvp.Key), kvp.Value < 0 ? "non disponibile" : kvp.Value.ToString() }));
                }
            }


            page.Paragraphs.Add(t1);
            page.Paragraphs.Add(t2);
            
            using (MemoryStream stream = new MemoryStream())
            {
                pdf.Save(stream);

                if (stream != null)
                {
                    doc.content = stream.ToArray();
                }

                stream.Close();
            }

            return doc;
        }

        private TextFragment AddTitolo(string text)
        {
            TextFragment tf = new TextFragment(text);
            tf.TextState.Font = FontRepository.FindFont("Helvetica");
            tf.TextState.FontSize = 20;
            tf.Margin.Bottom = 10;
            return tf;
        }

        private TextFragment AddSottotitolo(string text)
        {
            TextFragment tf = new TextFragment(text);
            tf.TextState.Font = FontRepository.FindFont("Helvetica");
            tf.TextState.FontSize = 16;
            tf.Margin.Bottom = 5;
            return tf;
        }

        private TextFragment AddTesto(string text)
        {
            TextFragment tf = new TextFragment(text);
            tf.TextState.Font = FontRepository.FindFont("Helvetica");
            tf.TextState.FontSize = 12;
            return tf;
        }

        private Table AddTable()
        {
            Table table = new Table();
            table.Margin = new MarginInfo(0, 5, 20, 10);
            table.DefaultCellPadding = new MarginInfo(5, 10, 0, 10);
            table.Border = new BorderInfo(BorderSide.All, .5f, Color.Black);
            table.DefaultCellBorder = new BorderInfo(BorderSide.All, .1f, Color.Black);
            table.ColumnAdjustment = ColumnAdjustment.AutoFitToWindow;
            
            return table;
        }

        private Row AddRow(string[] text)
        {
            Row row = new Row();
            row.DefaultCellTextState.Font = FontRepository.FindFont("Helvetica");
            row.DefaultCellTextState.FontSize = 12;
            for (int i = 0; i < text.Count(); i++)
            {
                row.Cells.Add(text[i]);
            }
            row.Cells[0].DefaultCellTextState.FontSize = 16;
            return row;
        }
        #endregion
    }
}
