using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

using Aspose.Slides;

namespace BusinessLogic.Modelli.AsposeModelProcessor
{
    public class PptModelProcessor : BaseDocModelProcessor
    {
        private static ILog logger = LogManager.GetLogger(typeof(PptModelProcessor));

        public override DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] GetProcessedTemplate(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento sd)
        {
            byte[] content = null;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            // Percorso modello
            string pathModel = sd.template.PATH_MODELLO_1;

            try
            {
                //System.IO.FileStream licenseStream = new System.IO.FileStream("C:\\Temp\\Aspose.total.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
               
                // Caricamento licenza
                DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor csmp = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

                Aspose.Slides.License lic = new License();
                byte[] licenseContent = csmp.GetLicense("ASPOSE");
                if (licenseContent != null)
                {
                    System.IO.MemoryStream licenseStream = new System.IO.MemoryStream(licenseContent, 0, licenseContent.Length);
                    lic.SetLicense(licenseStream);
                    licenseStream.Close();
                }

                DocsPaVO.Modelli.ModelKeyValuePair[] commonFields = this.GetModelKeyValuePairs(infoUtente, sd);

                // Lettura modello con Aspose
                using (Presentation ppt = new Presentation(pathModel))
                {

                    // 1 - Analisi master
                    foreach (LayoutSlide layoutSlide in ppt.LayoutSlides)
                    {
                        foreach (IShape shape in layoutSlide.Shapes)
                        {
                            if (shape is Aspose.Slides.AutoShape)
                            {
                                if (((IAutoShape)shape).TextFrame != null)
                                {
                                    string text = ((IAutoShape)shape).TextFrame.Text;
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        this.SearchAndReplaceFields(ref text, commonFields);
                                        ((IAutoShape)shape).TextFrame.Text = text;
                                    }
                                }
                            }
                        }
                    }

                    // Lettura caselle di testo
                    ITextFrame[] textFrames = Aspose.Slides.Util.SlideUtil.GetAllTextFrames(ppt, true);

                    foreach (ITextFrame tf in textFrames)
                    {
                        foreach (Paragraph par in tf.Paragraphs)
                        {
                            string text = par.Text;
                            if (!string.IsNullOrEmpty(text) && text.Contains("#"))
                            {
                                this.SearchAndReplaceFields(ref text, commonFields);
                                par.Text = text;
                            }
                            #region OLD CODE
                            //foreach (Portion port in par.Portions)
                            //{
                            //    string text = port.Text;
                            //    if (!string.IsNullOrEmpty(text) && text.Contains("#"))
                            //    {
                            //        this.SearchAndReplaceFields(ref text, commonFields);
                            //        port.Text = text;
                            //    }
                            //}
                            #endregion
                        }
                    }

                    #region OLD CODE
                    //Lettura slide
                    //foreach (ISlide slide in ppt.Slides)
                    //{
                    //    // Ciclo sulle forme della slide
                    //    foreach (IShape shape in slide.Shapes)
                    //    {
                    //        // Analisi testo                         
                    //        if (((IAutoShape)shape).TextFrame != null)
                    //        {
                    //            string text = ((IAutoShape)shape).TextFrame.Text;
                    //            if (!string.IsNullOrEmpty(text))
                    //            {
                    //                this.SearchAndReplaceFields(ref text, commonFields);
                    //                ((IAutoShape)shape).TextFrame.Text = text;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    // Salvataggio del documento
                    Aspose.Slides.Export.SaveFormat frm = new Aspose.Slides.Export.SaveFormat();
                    switch (sd.template.PATH_MODELLO_1_EXT.ToUpper())
                    {
                        case "PPT":
                        default:
                            frm = Aspose.Slides.Export.SaveFormat.Ppt;
                            break;
                        case "PPTX":
                            frm = Aspose.Slides.Export.SaveFormat.Pptx;
                            break;
                        case "PPSX":
                            frm = Aspose.Slides.Export.SaveFormat.Ppsx;
                            break;
                        case "ODP":
                            frm = Aspose.Slides.Export.SaveFormat.Odp;
                            break;
                    }

                    ppt.Save(stream, Aspose.Slides.Export.SaveFormat.Pptx);
                }

                content = stream.ToArray();
            }
            catch (Exception ex)
            {
                logger.DebugFormat("{0}\r\n{1}", ex.Message, ex.StackTrace);
                content = null;

            }
            return content;
        }

        private void SearchAndReplaceFields(ref string text, DocsPaVO.Modelli.ModelKeyValuePair[] fields)
        {
            if (text.Contains("#"))
            {
                foreach (DocsPaVO.Modelli.ModelKeyValuePair kvp in fields)
                {
                    text = text.Replace(this.FormatFieldName(kvp.Key), kvp.Value);
                }
            }
        }

        protected override DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            ArrayList listaCampi = this.GetOggettiProfilazione(schedaDocumento.docNumber, infoUtente.idAmministrazione, schedaDocumento.tipologiaAtto.descrizione);

            base.FetchCommonFields(listaCampi, infoUtente, schedaDocumento);

            List<DocsPaVO.Modelli.ModelKeyValuePair> list = new List<DocsPaVO.Modelli.ModelKeyValuePair>();

            foreach (string[] items in listaCampi)
            {
                DocsPaVO.Modelli.ModelKeyValuePair pair = new DocsPaVO.Modelli.ModelKeyValuePair();
                pair.Key = items[0];
                pair.Value = items[1];
                list.Add(pair);
            }

            return list.ToArray();
        }

        private string FormatFieldName(string name)
        {
            return "#" + name + "#";
        }

        private ArrayList GetOggettiProfilazione(string docNumber, string idAmministrazione, string tipoAtto)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(docNumber);
            ArrayList listaChiaviValori = new ArrayList();

            if (template != null && template.ELENCO_OGGETTI != null)
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                    if (oggettoCustom != null)
                    {
                        string[] itemToAdd = null;

                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Corrispondente":
                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(oggettoCustom.VALORE_DATABASE);


                                itemToAdd = new string[8] { "", "", "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                if (corr != null)
                                {
                                    itemToAdd[1] = corr.descrizione;
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(oggettoCustom.VALORE_DATABASE);
                                    if (corrIndirizzo != null)
                                    {
                                        //
                                        oggettoCustom.INDIRIZZO += corr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo + Environment.NewLine +
                                            corrIndirizzo.cap + '-' + corrIndirizzo.citta + '-' + corrIndirizzo.localita;
                                        itemToAdd[3] = oggettoCustom.INDIRIZZO;
                                        oggettoCustom.TELEFONO += corr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + '-' + corrIndirizzo.telefono2;
                                        itemToAdd[6] = oggettoCustom.TELEFONO;
                                        oggettoCustom.INDIRIZZO_TELEFONO += oggettoCustom.INDIRIZZO + Environment.NewLine + corrIndirizzo.telefono1 +
                                           '-' + corrIndirizzo.telefono2;
                                        itemToAdd[7] = oggettoCustom.INDIRIZZO_TELEFONO;

                                    }

                                }


                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                itemToAdd[5] = oggettoCustom.ID_AOO_RF;
                                listaChiaviValori.Add(itemToAdd);

                                break;
                            case "Contatore":
                                string formato = oggettoCustom.FORMATO_CONTATORE;
                                formato = formato.Replace("ANNO", oggettoCustom.ANNO);

                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null && !string.IsNullOrEmpty(reg.codRegistro))
                                    formato = formato.Replace("AOO", reg.codRegistro);

                                DocsPaVO.utente.Registro rf = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (rf != null && !string.IsNullOrEmpty(rf.codRegistro))
                                    formato = formato.Replace("RF", rf.codRegistro);

                                formato = formato.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = formato;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                itemToAdd[3] = oggettoCustom.FORMATO_CONTATORE;
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] = 
                                listaChiaviValori.Add(itemToAdd);

                                break;
                            case "CasellaDiSelezione":
                                string valore = string.Empty;
                                foreach (string val in oggettoCustom.VALORI_SELEZIONATI)
                                {
                                    if (val != null && val != "")
                                        valore += val + "-";
                                }
                                if (valore.Length > 1)
                                    valore = valore.Substring(0, valore.Length - 1);

                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = valore;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] =
                                listaChiaviValori.Add(itemToAdd);
                                break;
                            default:
                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = oggettoCustom.VALORE_DATABASE;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] =
                                listaChiaviValori.Add(itemToAdd);
                                break;
                        }
                    }
                }
            }

            return listaChiaviValori;
        }

    }
}
