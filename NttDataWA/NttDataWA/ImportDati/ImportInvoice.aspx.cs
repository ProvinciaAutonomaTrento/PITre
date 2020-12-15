using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Collections;
using NttDatalLibrary;
using System.Text;
using System.Globalization;
using System.Data;
using NttDataWA.UserControls;
using System.Xml;

namespace NttDataWA.ImportDati
{
    public partial class ImportInvoice : System.Web.UI.Page
    {
        string language;

        protected void Page_Load(object sender, EventArgs e)
        {
            language = UIManager.UserManager.GetUserLanguage();

            if (!IsPostBack)
            {
                this.InitializePage();
            }

            this.RefreshScript();
        }

        private void InitializePage()
        {
            this.InitializeLanguage();

            this.BtnImportInvoiceUpdate.Enabled = false;
            this.BtnImportInvoiceConfirm.Enabled = false;
        }

        private void InitializeLanguage()
        {
            this.LitSearchProject.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTitle", language);
            this.LitImportInvoiceSearch.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceSearchLabel", language);
            this.LitTxtRifAmm.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtRifAmm", language);

            this.LitTxtIdDoc.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtIdDoc", language);
            this.LitTxtCIG.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtCIG", language);
            this.LitTxtPosFin.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtPosFin", language);

            this.LitTxtOptional1.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional1", language);
            this.LitTxtOptional2.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional2", language);
            this.LitTxtOptional3.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional3", language);
            this.LitTxtOptional4.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional4", language);
            this.LitTxtOptional5.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional5", language);
            this.LitTxtOptional6.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional6", language);
            this.LitTxtOptional7.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional7", language);
            this.LitTxtOptional8.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional8", language);
            this.LitTxtOptional9.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional9", language);
            this.LitTxtOptional10.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceTxtOptional10", language);

            this.litAddLine.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceAddLine", language);

            this.litDescrizione.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceDescrizione", language);
            this.litQuantita.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceQuantita", language);
            this.litPrezzoUnitario.Text = Utils.Languages.GetLabelFromCode("ImportInvoicePrezzoUnitario", language);
            this.litPrezzoTotale.Text = Utils.Languages.GetLabelFromCode("ImportInvoicePrezzoTotale", language);
            this.litAliquota.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceAliquota", language);

            this.BtnImportInvoiceSearch.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceBtnSearch", language);
            this.BtnImportInvoiceUpdate.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceBtnUpdate", language);
            this.BtnImportInvoiceConfirm.Text = Utils.Languages.GetLabelFromCode("ImportInvoiceBtnConfirm", language);
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OnlyNumbers", "OnlyNumbers();", true);
        }

        protected void BtnAddLine_Click(object sender, EventArgs e)
        {
            visualizzaNewLine();
            this.btnAddLine.Enabled = false;
        }

        protected void BtnImportInvoiceSearch_Click(object sender, EventArgs e)
        {
            
            //verifica inserimento codice
            string codInvoice = this.TxtImportInvoiceSearch.Text;
            if (string.IsNullOrEmpty(codInvoice))
            {
                this.TxtRifAmm.Text = string.Empty;

                this.TxtIdDoc.Text = string.Empty;
                this.TxtCIG.Text = string.Empty;
                this.TxtPosFin.Text = string.Empty;

                this.TxtOptional1.Text = string.Empty;
                this.TxtOptional2.Text = string.Empty;
                this.TxtOptional3.Text = string.Empty;
                this.TxtOptional4.Text = string.Empty;
                this.TxtOptional5.Text = string.Empty;
                this.TxtOptional6.Text = string.Empty;
                this.TxtOptional7.Text = string.Empty;
                this.TxtOptional8.Text = string.Empty;
                this.TxtOptional9.Text = string.Empty;
                this.TxtOptional10.Text = string.Empty;

                string msgDesc = "msgImportInvoiceInvalidCode";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {

                string result = UIManager.ImportInvoiceManager.getFattura(codInvoice);
                if (string.IsNullOrEmpty(result))
                {






                    XmlDocument fatturaXML = Session["invoiceXML"] as XmlDocument;



                    XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(fatturaXML.NameTable);
                    //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                    //xmlnsManager.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                    string InvoiceNamespace = System.Configuration.ConfigurationManager.AppSettings["NAMESPACE_FATTURAPA"];
                    xmlnsManager.AddNamespace("p", InvoiceNamespace);
                    
                    if(fatturaXML != null)
                    {
                        // [1.2.6] // NON PRESENTE <FatturaElettronicaHeader><CedentePrestatore><RiferimentoAmministrazione>
                        this.TxtRifAmm.Text = string.Empty; //"0017-0000132831"; //
                        // [2.1.3.2] <FatturaElettronicaBody><DatiGenerali><DatiContratto><IdDocumento>
                        XmlNode nodo_2_1_3_2 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiContratto/IdDocumento", xmlnsManager);
                        this.TxtIdDoc.Text = nodo_2_1_3_2.InnerText.Trim(); // "214000655"; //
                        // [2.1.3.7] <FatturaElettronicaBody><DatiGenerali><DatiContratto><CodiceCIG>
                        XmlNode nodo_2_1_3_7 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiContratto/CodiceCIG", xmlnsManager);
                        this.TxtCIG.Text = nodo_2_1_3_7.InnerText.Trim();
                        // [2.2.1.16.2] *** NON PRESENTE <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><AltriDatiGestionali><RiferimentoTesto>
                        this.TxtPosFin.Text = string.Empty; //"5U211200900"; //
                        // [2.1.2.4] <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><NumItem>
                        XmlNode nodo_2_1_2_4 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiOrdineAcquisto/NumItem", xmlnsManager);
                        if (nodo_2_1_2_4 != null)
                        {
                            this.PnlTxtOptional1.Visible = true;
                            this.TxtOptional1.Text = nodo_2_1_2_4.InnerText.Trim();
                        }
                        else
                        {
                            this.PnlTxtOptional1.Visible = false;
                            this.TxtOptional1.Text = string.Empty;
                        }
                        // [2.1.2.5] <FatturaElettronicaBody><DatiGenerali><DatiOrdineAcquisto><CodiceCommessaConvenzione>
                        XmlNode nodo_2_1_2_5 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiOrdineAcquisto/CodiceCommessaConvenzione", xmlnsManager);
                        if (nodo_2_1_2_5 != null)
                        {
                            this.PnlTxtOptional2.Visible = true;
                            this.TxtOptional2.Text = nodo_2_1_2_5.InnerText.Trim();
                        }
                        else
                        {
                            this.PnlTxtOptional2.Visible = false;
                            this.TxtOptional2.Text = string.Empty;
                        }
                        // [2.2.1.7] <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><DataInizioPeriodo>
                        //XmlNode nodo_2_2_1_7 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiBeniServizi/DettaglioLinee/DataInizioPeriodo", xmlnsManager);
                        //this.TxtOptional3.Text = nodo_2_2_1_7.InnerText.Trim();
                        // [2.2.1.8] <FatturaElettronicaBody><DatiBeniServizi><DettaglioLinee><DataFinePeriodo>
                        //XmlNode nodo_2_2_1_8 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiBeniServizi/DettaglioLinee/DataFinePeriodo", xmlnsManager);
                        //this.TxtOptional4.Text = nodo_2_2_1_8.InnerText.Trim();
                        // [2.1.3.5]
                        XmlNode nodo_2_1_3_5 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiContratto/CodiceCommessaConvenzione", xmlnsManager);
                        this.TxtOptional5.Text = nodo_2_1_3_5.InnerText.Trim();
                        // [2.2.1.15]
                        XmlNode nodo_2_2_1_15 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiBeniServizi/DettaglioLinee/RiferimentoAmministrazione", xmlnsManager);
                        this.TxtOptional6.Text = nodo_2_2_1_15.InnerText;
                        // [2.1.4.2] <FatturaElettronicaBody><DatiGenerali><DatiConvenzione><IdDocumento>
                        XmlNode nodo_2_1_4_2 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiConvenzione/IdDocumento", xmlnsManager);
                        if(nodo_2_1_4_2 != null)
                            this.TxtOptional10.Text = nodo_2_1_4_2.InnerText;
                        // [2.1.5.1] <FatturaElettronicaBody><DatiGenerali><DatiRicezione><DatiRicezione>				
                        XmlNode nodo_2_1_5_1 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiRicezione/DatiRicezione", xmlnsManager);
                        if(nodo_2_1_5_1 != null)
                            this.TxtOptional7.Text = nodo_2_1_5_1.InnerText;
                        // [2.1.5.2] <FatturaElettronicaBody><DatiGenerali><DatiRicezione><IdDocumento>				
                        XmlNode nodo_2_1_5_2 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiRicezione/IdDocumento", xmlnsManager);
                        if(nodo_2_1_5_2 != null)
                            this.TxtOptional8.Text = nodo_2_1_5_2.InnerText;
                        // [2.1.5.4] <FatturaElettronicaBody><DatiGenerali><DatiRicezione><NumItem>				
                        XmlNode nodo_2_1_5_4 = fatturaXML.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiRicezione/NumItem", xmlnsManager);
                        if(nodo_2_1_5_4 != null)
                            this.TxtOptional9.Text = nodo_2_1_5_4.InnerText;
                    }
                    else
                    {
                        this.TxtRifAmm.Text = string.Empty; //"0017-0000132831"; //
                        this.TxtIdDoc.Text = string.Empty; // "214000655"; //
                        this.TxtCIG.Text = string.Empty; // "3737295166"; //
                        this.TxtPosFin.Text = string.Empty; //"5U211200900"; //
                        this.TxtOptional1.Text = string.Empty;
                        this.TxtOptional2.Text = string.Empty;
                        this.TxtOptional3.Text = string.Empty;
                        this.TxtOptional4.Text = string.Empty;
                    }
                    this.TxtOptional5.Text = string.Empty;
                    this.TxtOptional6.Text = string.Empty;
                    this.TxtOptional7.Text = string.Empty;
                    this.TxtOptional8.Text = string.Empty;
                    this.TxtOptional9.Text = string.Empty;
                    this.TxtOptional10.Text = string.Empty;

                    this.PnlOptionalFields.Visible = true;
                    this.UpPnlOptionalFields.Update();

                    this.PnlBtnAddLine.Visible = true;
                    this.UplAddLine.Update();

                    this.BtnImportInvoiceConfirm.Enabled = true;
                    //this.BtnImportInvoiceUpdate.Enabled = true;              
                    this.UpPnlDocumentData.Visible = true;
                    this.frame.Attributes["src"] = "PreviewInvoice.aspx";
                    this.UpPnlSearchInvoice.Update();
                    this.UpPnlDocumentData.Update();
                    this.UpPnlContentDxSx.Update();

                }
                else
                {
                    if (result.Equals("NotFound"))
                    {
                        string msgDesc = "msgImportInvoiceNotFound";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        if (result.Equals("NO_IPA"))
                        {
                            string msgDesc = "msgImportInvoiceNotIpa";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            string msgDesc = "msgImportInvoiceKO";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');}", true);
                        }
                    }
                }
            }
        }

        private void visualizzaNewLine()
        {
            this.txtDescrizione.Text = string.Empty;
            this.txtQuantita.Text = string.Empty;
            this.txtPrezzoUnitario.Text = string.Empty;
            this.txtPrezzoTotale.Text = string.Empty;
            this.txtAliquota.Text = string.Empty;

            this.PnlNewLine.Visible = true;
            this.UpnNewLine.Update();

        }

        protected void BtnImportInvoiceUpdate_Click(object sender, EventArgs e)
        {
            string rifAmm = this.TxtRifAmm.Text;
            string strIdDoc = this.TxtIdDoc.Text;
            string strCIG = this.TxtCIG.Text;
            string posFin = this.TxtPosFin.Text;

            string optional1 = this.TxtOptional1.Text;
            string optional2 = this.TxtOptional2.Text;
            string optional3 = this.TxtOptional3.Text;
            string optional4 = this.TxtOptional4.Text;
            string optional5 = this.TxtOptional5.Text;
            string optional6 = this.TxtOptional6.Text;
            string optional7 = this.TxtOptional7.Text;
            string optional8 = this.TxtOptional8.Text;
            string optional9 = this.TxtOptional9.Text;
            string optional10 = this.TxtOptional10.Text;

            string strDesc = this.txtDescrizione.Text;
            string strQuant = this.txtQuantita.Text;
            string strPrezUni = this.txtPrezzoUnitario.Text;
            string strPrezTot = this.txtPrezzoTotale.Text;
            string strAliquot = this.txtAliquota.Text;

            bool result = ImportInvoiceManager.updateParams(rifAmm, strIdDoc, strCIG, posFin, strDesc, strQuant, strPrezUni, strPrezTot, strAliquot, optional1, optional2, optional3, optional4, optional5, optional6, optional7, optional8, optional9, optional10);
            if (result)
            {
                this.frame.Attributes["src"] = "PreviewInvoice.aspx";
                this.UpPnlDocumentData.Update();
            }
            else
            {
                string msgDesc = "msgImportInvoiceUpdateParamKO";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');}", true);
            }
        }

        protected void BtnImportInvoiceConfirm_Click(object sender, EventArgs e)
        {

            string rifAmm = this.TxtRifAmm.Text;

            string strIdDoc = this.TxtIdDoc.Text;
            string strCIG = this.TxtCIG.Text;
            string posFin = this.TxtPosFin.Text;

            string optional1 = this.TxtOptional1.Text;
            string optional2 = this.TxtOptional2.Text;
            string optional3 = this.TxtOptional3.Text;
            string optional4 = this.TxtOptional4.Text;
            string optional5 = this.TxtOptional5.Text;
            string optional6 = this.TxtOptional6.Text;
            string optional7 = this.TxtOptional7.Text;
            string optional8 = this.TxtOptional8.Text;
            string optional9 = this.TxtOptional9.Text;
            string optional10 = this.TxtOptional10.Text;

            string strDesc = this.txtDescrizione.Text;
            string strQuant = this.txtQuantita.Text;
            string strPrezUni = this.txtPrezzoUnitario.Text;
            string strPrezTot = this.txtPrezzoTotale.Text;
            string strAliquot = this.txtAliquota.Text;

            string strErrore;
            if (VerificaCampiLinea(strDesc, strQuant, strPrezUni, strPrezTot, strAliquot, out strErrore))
            {

                bool result = ImportInvoiceManager.updateParams(rifAmm, strIdDoc, strCIG, posFin, strDesc, strQuant, strPrezUni, strPrezTot, strAliquot, optional1, optional2, optional3, optional4, optional5, optional6, optional7, optional8, optional9, optional10);

                if (result)
                    result = ImportInvoiceManager.uploadFattura();

                if (!result)
                {
                    string msgDesc = "msgImportInvoiceImportKO";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                else
                {
                    this.TxtRifAmm.Text = string.Empty;

                    this.TxtIdDoc.Text = string.Empty;
                    this.TxtCIG.Text = string.Empty;

                    this.TxtImportInvoiceSearch.Text = string.Empty;
                    this.PnlOptionalFields.Visible = false;
                    this.UpPnlOptionalFields.Update();
                    this.UpPnlSearchInvoice.Update();
                    this.BtnImportInvoiceConfirm.Enabled = false;
                    this.BtnImportInvoiceUpdate.Enabled = false;
                    this.UpPnlDocumentData.Visible = false;
                    this.frame.Attributes["src"] = string.Empty;
                    this.UpPnlSearchInvoice.Update();
                    this.UpPnlDocumentData.Update();
                    this.UpPnlContentDxSx.Update();

                    this.txtDescrizione.Text = string.Empty;
                    this.txtQuantita.Text = string.Empty;
                    this.txtPrezzoUnitario.Text = string.Empty;
                    this.txtPrezzoTotale.Text = string.Empty;
                    this.txtAliquota.Text = string.Empty;
                    this.PnlNewLine.Visible = false;
                    this.UpnNewLine.Update();
                    this.btnAddLine.Enabled = true;

                    string msgDesc = "msgImportInvoiceImportOK";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'check', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'check', '');}", true);

                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + strErrore.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + strErrore.Replace("'", @"\'") + "', 'error', '');}", true);
            }
        }

        private bool VerificaCampiLinea(string strDesc, string strQuant, string strPrezUni, string strPrezTot, string strAliquot, out string strMessage)
        {
            bool retVal = false;
            strMessage = string.Empty;

            if (string.IsNullOrEmpty(strDesc) && string.IsNullOrEmpty(strQuant) && string.IsNullOrEmpty(strPrezUni) && string.IsNullOrEmpty(strPrezTot) && string.IsNullOrEmpty(strAliquot))
            {
                retVal = true;
            }
            else if (string.IsNullOrEmpty(strDesc) || string.IsNullOrEmpty(strQuant) || string.IsNullOrEmpty(strPrezUni) || string.IsNullOrEmpty(strPrezTot) || string.IsNullOrEmpty(strAliquot))
            {
                strMessage = Utils.Languages.GetMessageFromCode("msgImportInvoiceErrorCampi", language); 
            }
            else
            {
                decimal temDecimal;
                bool conversion = Decimal.TryParse(strPrezUni, out temDecimal);

                if (conversion)
                    retVal = true;
                else
                    strMessage = Utils.Languages.GetMessageFromCode("msgImportInvoiceErrorUnitario", language);

                decimal temDecimal2;
                Decimal.TryParse(strPrezTot, out temDecimal2);
                if (retVal && conversion)
                    retVal = true;
                else if (retVal)
                {
                    strMessage = Utils.Languages.GetMessageFromCode("msgImportInvoiceErrorTotali", language);
                    retVal = false;
                }
            }
            
            return retVal;
        }
    }
}