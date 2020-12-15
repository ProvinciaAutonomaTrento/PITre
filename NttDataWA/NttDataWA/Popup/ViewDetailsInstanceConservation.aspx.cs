using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class ViewDetailsInstanceConservation : System.Web.UI.Page
    {
        #region Properties

        private List<TipoIstanzaConservazione> TypeInstanceConservation
        {
            get
            {
                if (HttpContext.Current.Session["TypeInstanceConservation"] != null)
                    return (List<TipoIstanzaConservazione>)HttpContext.Current.Session["TypeInstanceConservation"];
                else
                    return null;
            }
        }

        private InfoConservazione InstanceConservation
        {
            get
            {
                if (HttpContext.Current.Session["InstanceConservazione"] != null)
                    return (InfoConservazione)HttpContext.Current.Session["InstanceConservazione"];
                else
                    return null;
            }
        }

        private List<ItemsConservazione> ItemsConservation
        {
            get
            {
                if (HttpContext.Current.Session["ItemsConservation"] != null)
                    return (List<ItemsConservazione>)HttpContext.Current.Session["ItemsConservation"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ItemsConservation"] = value;
            }
        }

        private List<InfoConservazione> InstancesConservation
        {
            get
            {
                if (HttpContext.Current.Session["InstancesConservation"] != null)
                    return (List<InfoConservazione>)HttpContext.Current.Session["InstancesConservation"];
                else
                    return null;
            }
        }

        private bool IsZoom
        {
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private AreaConservazioneValidationResult ResultValidateInstance
        {
            set
            {
                HttpContext.Current.Session["ResultValidateInstance"] = value;
            }
        }

        private string IdInstance
        {
            set
            {
                HttpContext.Current.Session["IdInstance"] = value;
            }
        }

        private void RemovePropertiesPopupConservatioAreaValidation()
        {
            HttpContext.Current.Session.Remove("IdInstance");
            HttpContext.Current.Session.Remove("ResultValidateInstance");
        }

        #endregion

        #region Const

        private const string VIEW_DETAILS_ITEM = "ViewDetailsItem";
        private const string REMOVE_ITEM = "RemoveItem";
        private const string CLOSE_POPUP_CONSERVATION_AREA_VALIDATION = "closePopupConservationAreaValidation";
        private const string UP_PANEL_BUTTONS = "UpPnlButtons";

        #endregion

        #region Standar Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializeContent();
                InitializePage();
            }
            else
            {
                ReadRetValueFromPopup();
                RefreshScript();
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ViewInstanceConservationSendForConservation.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationSendForConservation", language);
            this.ViewInstanceConservationEnableInstanceReject.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationEnableInstanceReject", language);
            this.ViewInstanceConservationRemoveAll.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationRemoveAll", language);
            this.ViewInstanceConservationSelectedAsDefault.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationSelectedAsDefault", language);
            this.ViewInstanceConservationRemoveNonCompliant.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationRemoveNonCompliant", language);
            this.ViewInstanceConservationClose.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationClose", language);
            this.ViewInstanceConservationSendForConservation.ToolTip = Utils.Languages.GetLabelFromCode("ViewInstanceConservationSendForConservationToolTip", language);
            this.ViewInstanceConservationEnableInstanceReject.ToolTip = Utils.Languages.GetLabelFromCode("ViewInstanceConservationEnableInstanceRejectToolTip", language);
            this.ViewInstanceConservationRemoveAll.ToolTip = Utils.Languages.GetLabelFromCode("ViewInstanceConservationRemoveAllToolTip", language);
            this.ViewInstanceConservationSelectedAsDefault.ToolTip = Utils.Languages.GetLabelFromCode("ViewInstanceConservationSelectedAsDefaultToolTip", language);
            this.ViewInstanceConservationRemoveNonCompliant.ToolTip = Utils.Languages.GetLabelFromCode("ViewInstanceConservationRemoveNonCompliantToolTip", language);
            this.lblEnterDescription.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationLblEnterDescription", language);
            this.lblEnterNote.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationLblEnterNote", language);
            this.lblTypeConservation.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationLblTypeConservation", language);
            this.cbConsolidatesDocuments.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationCbConsolidatesDocuments", language);
            this.lblValidInstance.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationLblValidInstance", language);
            this.ddl_validInstance.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("ViewInstanceConservationDdl_validInstance", language));
            this.lblTotalSize.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationTotalSize", language);
            this.lblNumberDocument.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationNumberDocument", language);
            this.ConservationAreaValidation.Title = Utils.Languages.GetLabelFromCode("ConservationAreaValidationTitle", language);

            this.ViewInstanceConservationCheckFormat.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationCheckFormat", language);
            this.ViewInstanceConservationViewReport.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationViewReport", language);
            this.ViewInstanceConservationConvertAndSendForConservation.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationConvertAndSendForConservation", language);
            this.ReportFormatiConservazione.Title = Utils.Languages.GetLabelFromCode("ReportFormatiConservazioneTitle", language);
        }

        private void InitializeContent()
        {
            BindDdlTypeInstanceConservation();
            BindDdlListPolicy();
            LoadItemsConservation();
            SetSize();
            SetNumberDocument();
            GrdItemsConservation_Bind();
            this.cbConsolidatesDocuments.Checked = true;
            this.cbConsolidatesDocuments.Enabled = false;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_PANEL_BUTTONS))
            {

                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_CONSERVATION_AREA_VALIDATION)))
                {
                    if (!string.IsNullOrEmpty(this.ConservationAreaValidation.ReturnValue))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ConservationAreaValidation','');", true);
                        // Invio istanza di conservazione
                        if (SendInstance())
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessSendForConservationInstanceConservation', 'check', '','');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSendForConservationInstanceConservation', 'error', '','');", true);
                        }
                    }
                    RemovePropertiesPopupConservatioAreaValidation();
                    return;
                }
            }

        }

        /// <summary>
        /// Imposta la dimensione totale dei documenti nella label
        /// </summary>
        private void SetSize()
        {
            if (ItemsConservation != null)
            {
                int size = 0;
                foreach (ItemsConservazione item in ItemsConservation)
                {
                    size += Convert.ToInt32(item.SizeItem);
                }
                float sizeF = (float)size / 1048576;
                string size_appo = Convert.ToString(sizeF);
                if (size_appo.Contains(","))
                {
                    size_appo = size_appo.Substring(0, size_appo.IndexOf(",") + 2);
                }
                else
                {
                    if (size_appo.Contains("."))
                    {
                        size_appo = size_appo.Substring(0, size_appo.IndexOf(".") + 2);
                    }
                }

                this.lblTotalSize.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationTotalSize", UIManager.UserManager.GetUserLanguage()).Replace("@@", size_appo);
            }
        }
        /// <summary>
        /// Imposta il numero totale dei documenti nella label
        /// </summary>
        private void SetNumberDocument()
        {
            if (ItemsConservation != null)
            {
                this.lblNumberDocument.Text = Utils.Languages.GetLabelFromCode("ViewInstanceConservationNumberDocument", UIManager.UserManager.GetUserLanguage()) + " " + ItemsConservation.Count;
            }
        }

        private void InitializePage()
        {
            if (ItemsConservation != null)
            {
                #region OLDCODE
                //if (InstanceConservation.StatoConservazione != "Nuova" && InstanceConservation.StatoConservazione != "N")
                //{
                //    DisablesBasket();
                //    this.ViewInstanceConservationSendForConservation.Enabled = false;
                //    this.ViewInstanceConservationRemoveAll.Enabled = false;
                //    this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                //    this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                //    this.TxtEnterNote.Enabled = false;
                //    this.TxtEnterDescription.Enabled = false;
                //    this.ddl_typeConservation.Enabled = false;
                //    this.ddl_validInstance.Enabled = false;
                //    this.TxtEnterDescription.Text = InstanceConservation.Descrizione;
                //    this.TxtEnterNote.Text = InstanceConservation.Note;

                //    if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                //    {
                //        SetDdlTypeConversion();
                //    }

                //    if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                //    {
                //        if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                //        {
                //            this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                //        }
                //    }
                //    else
                //    {
                //        this.ddl_validInstance.SelectedValue = string.Empty;
                //    }
                //}
                //else
                //{
                //    if (ItemsConservation.Count > 0)
                //    {
                //        this.ViewInstanceConservationSendForConservation.Enabled = true;
                //        this.ViewInstanceConservationRemoveAll.Enabled = true;
                //        this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                //        this.TxtEnterNote.Enabled = true;
                //        this.TxtEnterDescription.Enabled = true;
                //        this.TxtEnterNote.Text = string.Empty;
                //        this.TxtEnterDescription.Text = string.Empty;
                //        this.ddl_typeConservation.Enabled = true;
                //        this.ddl_validInstance.Enabled = true;
                //        this.ddl_validInstance.SelectedValue = string.Empty;
                //        if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                //        {
                //            if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                //            {
                //                this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                //            }
                //            this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                //        }
                //        else
                //        {
                //            this.ddl_validInstance.SelectedValue = string.Empty;
                //            this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                //        }
                //        if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                //        {
                //            SetDdlTypeConversion();
                //        }

                //        if (!string.IsNullOrEmpty(InstanceConservation.predefinita.ToString()) && InstanceConservation.predefinita.ToString().Equals("True"))
                //        {
                //            this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                //        }
                //        else
                //        {
                //            this.ViewInstanceConservationSelectedAsDefault.Enabled = true;
                //        }

                //        //pulsante verifica 
                //        this.ViewInstanceConservationCheckFormat.Enabled = true;
                //        //pulsante report
                //        this.ViewInstanceConservationViewReport.Enabled = false;
                //        //pulsante converti
                //        this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;

                //        EnablesBasket();
                //    }
                //    else
                //    {
                //        this.ViewInstanceConservationSendForConservation.Enabled = false;
                //        this.ViewInstanceConservationRemoveAll.Enabled = false;
                //        this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                //        this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                //        this.TxtEnterNote.Enabled = true;
                //        this.TxtEnterDescription.Enabled = true;
                //        this.TxtEnterNote.Text = string.Empty;
                //        this.TxtEnterDescription.Text = string.Empty;
                //        this.ddl_typeConservation.Enabled = true;
                //        this.ddl_validInstance.Enabled = true;
                //        this.ddl_validInstance.SelectedValue = string.Empty;
                //        if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                //        {
                //            if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                //            {
                //                this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                //            }
                //            this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                //        }
                //        else
                //        {
                //            this.ddl_validInstance.SelectedValue = string.Empty;
                //            this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                //        }
                //        if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                //        {
                //            SetDdlTypeConversion();
                //        }

                //    }
                //}
                #endregion

                #region NEWCODE MEV 1.5 F02_01

                DisablesBasket();
                this.ViewInstanceConservationSendForConservation.Enabled = false;
                this.ViewInstanceConservationRemoveAll.Enabled = false;
                this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                this.TxtEnterNote.Enabled = false;
                this.TxtEnterDescription.Enabled = false;
                this.ddl_typeConservation.Enabled = false;
                this.ddl_validInstance.Enabled = false;
                this.TxtEnterDescription.Text = InstanceConservation.Descrizione;
                this.TxtEnterNote.Text = InstanceConservation.Note;
                //pulsante verifica 
                this.ViewInstanceConservationCheckFormat.Enabled = false;
                //pulsante report
                this.ViewInstanceConservationViewReport.Enabled = false;
                //pulsante converti
                this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                {
                    SetDdlTypeConversion();
                }

                if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                {
                    if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                    {
                        this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                    }
                }
                else
                {
                    this.ddl_validInstance.SelectedValue = string.Empty;
                }
                //NUOVA
                if (InstanceConservation.StatoConservazione == "N")
                {
                    if (ItemsConservation.Count > 0)
                    {
                        this.ViewInstanceConservationSendForConservation.Enabled = false;
                        this.ViewInstanceConservationRemoveAll.Enabled = true;
                        this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                        this.TxtEnterNote.Enabled = true;
                        this.TxtEnterDescription.Enabled = true;
                        this.TxtEnterNote.Text = string.Empty;
                        this.TxtEnterDescription.Text = string.Empty;
                        this.ddl_typeConservation.Enabled = true;
                        this.ddl_validInstance.Enabled = true;
                        this.ddl_validInstance.SelectedValue = string.Empty;
                        if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                        {
                            if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                            {
                                this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                            }
                            this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                        }
                        else
                        {
                            this.ddl_validInstance.SelectedValue = string.Empty;
                            this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                        }
                        if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                        {
                            SetDdlTypeConversion();
                        }

                        if (!string.IsNullOrEmpty(InstanceConservation.predefinita.ToString()) && InstanceConservation.predefinita.ToString().Equals("True"))
                        {
                            this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                        }
                        else
                        {
                            this.ViewInstanceConservationSelectedAsDefault.Enabled = true;
                        }

                        //pulsante verifica 
                        this.ViewInstanceConservationCheckFormat.Enabled = true;
                        //pulsante report
                        this.ViewInstanceConservationViewReport.Enabled = false;
                        //pulsante converti
                        this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;

                        EnablesBasket();
                    }
                    else
                    {
                        this.ViewInstanceConservationSendForConservation.Enabled = false;
                        this.ViewInstanceConservationRemoveAll.Enabled = false;
                        this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                        this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                        this.TxtEnterNote.Enabled = true;
                        this.TxtEnterDescription.Enabled = true;
                        this.TxtEnterNote.Text = string.Empty;
                        this.TxtEnterDescription.Text = string.Empty;
                        this.ddl_typeConservation.Enabled = true;
                        this.ddl_validInstance.Enabled = true;
                        this.ddl_validInstance.SelectedValue = string.Empty;
                        if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                        {
                            if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                            {
                                this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                            }
                            this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                        }
                        else
                        {
                            this.ddl_validInstance.SelectedValue = string.Empty;
                            this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                        }
                        if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                        {
                            SetDdlTypeConversion();
                        }
                        //pulsante verifica 
                        this.ViewInstanceConservationCheckFormat.Enabled = false;
                        //pulsante report
                        this.ViewInstanceConservationViewReport.Enabled = false;
                        //pulsante converti
                        this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                    }

                }

                //Veificata
                if (InstanceConservation.StatoConservazione == "B")
                {

                    this.ViewInstanceConservationRemoveAll.Enabled = true;
                    this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                    this.TxtEnterNote.Enabled = true;
                    this.TxtEnterDescription.Enabled = true;
                    this.TxtEnterNote.Text = string.Empty;
                    this.TxtEnterDescription.Text = string.Empty;
                    this.ddl_typeConservation.Enabled = true;
                    this.ddl_validInstance.Enabled = true;
                    this.ddl_validInstance.SelectedValue = string.Empty;
                    if (!string.IsNullOrEmpty(InstanceConservation.idPolicyValidata))
                    {
                        if (this.ddl_validInstance.Items.FindByValue(InstanceConservation.idPolicyValidata) != null)
                        {
                            this.ddl_validInstance.SelectedValue = InstanceConservation.idPolicyValidata;
                        }
                        this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                    }
                    else
                    {
                        this.ddl_validInstance.SelectedValue = string.Empty;
                        this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                    }
                    if (!string.IsNullOrEmpty(InstanceConservation.TipoConservazione))
                    {
                        SetDdlTypeConversion();
                    }

                    this.ViewInstanceConservationSelectedAsDefault.Enabled = false;


                    switch (InstanceConservation.esitoVerifica)
                    {
                        //InstanceConservation.esitoVerifica
                        //NonEffettuata = 0,
                        //Successo = 1,    
                        //DirettamenteConvertibili = 2, 
                        //IndirettamenteConvertibili = 3,
                        //Fallita = 4,
                        //Errore = 5

                        case 0:

                            //pulsante invia
                            this.ViewInstanceConservationSendForConservation.Enabled = false;
                            //pulsante verifica 
                            this.ViewInstanceConservationCheckFormat.Enabled = true;
                            //pulsante report
                            this.ViewInstanceConservationViewReport.Enabled = false;
                            //pulsante converti
                            this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                            break;
                        case 1:
                            //pulsante invia
                            this.ViewInstanceConservationSendForConservation.Enabled = true;
                            //pulsante verifica 
                            this.ViewInstanceConservationCheckFormat.Enabled = false;
                            //pulsante report
                            this.ViewInstanceConservationViewReport.Enabled = true;
                            //pulsante converti
                            this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                            break;
                        case 2:
                            //pulsante invia
                            this.ViewInstanceConservationSendForConservation.Enabled = false;
                            //pulsante verifica 
                            this.ViewInstanceConservationCheckFormat.Enabled = true;
                            //pulsante report
                            this.ViewInstanceConservationViewReport.Enabled = true;
                            //pulsante converti
                            this.ViewInstanceConservationConvertAndSendForConservation.Enabled = true;
                            break;
                        default:
                            //pulsante invia
                            this.ViewInstanceConservationSendForConservation.Enabled = false;
                            //pulsante verifica 
                            this.ViewInstanceConservationCheckFormat.Enabled = true;
                            //pulsante report
                            this.ViewInstanceConservationViewReport.Enabled = true;
                            //pulsante converti
                            this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                            break;

                    }

                    EnablesBasket();
                }

                //inviata
                if (InstanceConservation.StatoConservazione == "I")
                {
                    this.ViewInstanceConservationRemoveAll.Enabled = false;
                    //pulsante invia
                    this.ViewInstanceConservationSendForConservation.Enabled = false;
                    //pulsante verifica 
                    this.ViewInstanceConservationCheckFormat.Enabled = false;
                    //pulsante report
                    this.ViewInstanceConservationViewReport.Enabled = true;
                    //pulsante converti
                    this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                }

                //errore conversione/verifica
                if (InstanceConservation.StatoConservazione == "Z")
                {
                    EnablesBasket();
                    this.ViewInstanceConservationRemoveAll.Enabled = true;
                    //pulsante invia
                    this.ViewInstanceConservationSendForConservation.Enabled = false;
                    //pulsante verifica 
                    this.ViewInstanceConservationCheckFormat.Enabled = true;
                    //pulsante report
                    this.ViewInstanceConservationViewReport.Enabled = true;
                    //pulsante converti
                    this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                }

                #endregion

                if (InstanceConservation.StatoConservazione == "Respinta" || InstanceConservation.StatoConservazione == "R")
                {
                    bool istanzaNuova = false;
                    //controllo che nn ci sia un'altra istanza in stato nuovo, in questo nn abilito il pulsante per riabilitare una qualsiasi altra istanza
                    for (int i = 0; i < InstancesConservation.Count; i++)
                    {
                        if (InstancesConservation[i].StatoConservazione == "N" || InstancesConservation[i].StatoConservazione == "Nuova")
                        {
                            istanzaNuova = true;
                            break;
                        }
                    }
                    if (!istanzaNuova && InstancesConservation.Count > 0)
                    {
                        this.ViewInstanceConservationEnableInstanceReject.Enabled = true;
                    }
                    else
                    {
                        this.ViewInstanceConservationEnableInstanceReject.Enabled = false;
                    }
                    //pulsante verifica 
                    this.ViewInstanceConservationCheckFormat.Enabled = false;
                    //pulsante report
                    this.ViewInstanceConservationViewReport.Enabled = true;
                    //pulsante converti
                    this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
                }
                else
                {
                    this.ViewInstanceConservationEnableInstanceReject.Enabled = false;
                }
            }
        }

        private void LoadItemsConservation()
        {
            ItemsConservation = DocumentManager.getItemsConservazioneLite(InstanceConservation.SystemID, Page, UserManager.GetInfoUser()).ToList();
        }

        private void BindDdlTypeInstanceConservation()
        {
            ListItem item;
            foreach (TipoIstanzaConservazione typeInstance in TypeInstanceConservation)
            {
                item = new ListItem();
                item.Text = typeInstance.Descrizione;
                item.Value = typeInstance.Codice;
                this.ddl_typeConservation.Items.Add(item);
            }
        }

        private void DisablesBasket()
        {
            foreach (GridViewRow row in this.GrdItemsConservation.Rows)
            {
                (row.FindControl("ViewDetailsConservationRemoveItem") as CustomImageButton).Enabled = false;
            }
        }

        private void EnablesBasket()
        {
            foreach (GridViewRow row in this.GrdItemsConservation.Rows)
            {
                (row.FindControl("ViewDetailsConservationRemoveItem") as CustomImageButton).Enabled = true;
            }
        }

        private void SetDdlTypeConversion()
        {
            if (this.ddl_typeConservation.Items.FindByValue(InstanceConservation.TipoConservazione) != null)
            {
                this.ddl_typeConservation.SelectedValue = InstanceConservation.TipoConservazione;
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                {
                    this.cbConsolidatesDocuments.Checked = true;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "ESIBIZIONE")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_INTERNA")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = true;
                }
            }
        }

        private void BindDdlListPolicy()
        {
            Policy[] policyListaDocumenti = DocumentManager.GetListaPolicy(Int32.Parse(UserManager.GetInfoUser().idAmministrazione), "D");
            Policy[] policyListaFascicoli = DocumentManager.GetListaPolicy(Int32.Parse(UserManager.GetInfoUser().idAmministrazione), "F");
            Policy[] policyListaStampe = DocumentManager.GetListaPolicy(Int32.Parse(UserManager.GetInfoUser().idAmministrazione), "R");
            Policy[] policyListaRepertori = DocumentManager.GetListaPolicy(Int32.Parse(UserManager.GetInfoUser().idAmministrazione), "C");

            this.ddl_validInstance.Items.Clear();
            this.ddl_validInstance.Items.Add("");
            int y = 0;
            if (policyListaDocumenti != null)
            {
                for (int i = 0; i < policyListaDocumenti.Length; i++, y++)
                {
                    this.ddl_validInstance.Items.Add("[D] " + policyListaDocumenti[i].nome);
                    this.ddl_validInstance.Items[i + 1].Value = policyListaDocumenti[i].system_id;
                }
            }
            if (policyListaFascicoli != null)
            {
                for (int i = 0; i < policyListaFascicoli.Length; i++, y++)
                {
                    this.ddl_validInstance.Items.Add("[F] " + policyListaFascicoli[i].nome);
                    this.ddl_validInstance.Items[y + 1].Value = policyListaFascicoli[i].system_id;
                }
            }
            if (policyListaStampe != null)
            {
                for (int i = 0; i < policyListaStampe.Length; i++, y++)
                {
                    this.ddl_validInstance.Items.Add("[S] " + policyListaStampe[i].nome);
                    this.ddl_validInstance.Items[y + 1].Value = policyListaStampe[i].system_id;
                }
            }
            if (policyListaRepertori != null)
            {
                for (int i = 0; i < policyListaRepertori.Length; i++, y++)
                {
                    this.ddl_validInstance.Items.Add("[R] " + policyListaRepertori[i].nome);
                    this.ddl_validInstance.Items[y + 1].Value = policyListaRepertori[i].system_id;
                }
            }
        }
        #endregion

        #region Bind Grid ItemsConservation

        private void GrdItemsConservation_Bind()
        {
            this.GrdItemsConservation.DataSource = ItemsConservation;
            this.GrdItemsConservation.DataBind();
        }

        protected void GrdItemsConservation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    for (int i = 0; i < e.Row.Cells.Count - 2; i++)
                    {
                        e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('UpPanelGridItemsConservation');return false;";
                    }

                    ItemsConservazione item = e.Row.DataItem as ItemsConservazione;
                    if (!string.IsNullOrEmpty(item.policyValida) && item.policyValida.Equals("1"))
                    {
                        e.Row.CssClass += " invalid";
                    }
                    else
                    {
                        e.Row.CssClass = e.Row.CssClass.Replace(" invalid", "");
                    }
                    CustomImageButton buttonTypeDoc = e.Row.FindControl("ViewDetailsConservationDetailsItem") as CustomImageButton;
                    // gestione della icona dei dettagli
                    if (item.immagineAcquisita == "0" || string.IsNullOrEmpty(item.dirittiDocumento))
                    {
                        buttonTypeDoc.ImageUrl = "../Images/Icons/small_no_file.png";
                        buttonTypeDoc.OnMouseOverImage = "../Images/Icons/small_no_file.png";
                        buttonTypeDoc.OnMouseOutImage = "../Images/Icons/small_no_file.png";
                        buttonTypeDoc.ImageUrlDisabled = "../Images/Icons/small_no_file.png";
                        buttonTypeDoc.CssClass = "";
                        buttonTypeDoc.Enabled = false;
                    }
                    else
                    {
                        string url = ResolveUrl(FileManager.getFileIcon(this.Page, item.tipoFile.Replace(".", "")));
                        buttonTypeDoc.ImageUrl = url;
                        buttonTypeDoc.OnMouseOverImage = url;
                        buttonTypeDoc.OnMouseOutImage = url;
                        buttonTypeDoc.ImageUrlDisabled = url;
                    }
                    if (!string.IsNullOrEmpty(item.numProt))
                    {
                        (e.Row.FindControl("lbl_data_prot_or_crea") as Label).Attributes["style"] = "color:red";
                    }
                    else
                    {
                        (e.Row.FindControl("lbl_data_prot_or_crea") as Label).Attributes["style"] = "color:black";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdItemsConservation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.GrdItemsConservation.PageIndex = e.NewPageIndex;
                this.grid_rowindex.Value = "0";
                GrdItemsConservation_Bind();
                this.UpPanelGridItemsConservation.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GrdItemsConservation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            switch (e.CommandName)
            {
                case VIEW_DETAILS_ITEM:
                    string docNumber = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("lbl_docNumber") as Label).Text;
                    this.IsZoom = true;
                    UIManager.DocumentManager.setSelectedRecord(UIManager.DocumentManager.getDocumentDetails(this.Page, docNumber, docNumber));
                    FileManager.setSelectedFile(UIManager.DocumentManager.getSelectedRecord().documenti[0]);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupDocumentViewer", "parent.ajaxModalPopupDocumentViewer();", true);
                    NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
                    break;
                case REMOVE_ITEM:
                    string systemId = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("lbl_systemId") as Label).Text;
                    if (DocumentManager.eliminaDaAreaConservazione("", null, "", false, systemId))
                    {
                        ItemsConservation.Remove((from itemConservation in ItemsConservation
                                                  where itemConservation.SystemID.Equals(systemId)
                                                  select itemConservation).FirstOrDefault());
                        this.grid_rowindex.Value = "-1";
                        GrdItemsConservation_Bind();
                        SetSize();
                        SetNumberDocument();
                        this.UpPanelLabelInfoNumberSizeDocument.Update();
                        this.UpPanelGridItemsConservation.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorRemoveDocumentInstanceConservation', 'error', '','');", true);
                        return;
                    }
                    break;
            }
        }

        protected string GetLabelObject(ItemsConservazione item)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(item.dirittiDocumento))
            {
                result = Utils.Languages.GetLabelFromCode("ViewDetailsInstanceConservationNoRightsVisibility", UIManager.UserManager.GetUserLanguage());
            }
            else
            {
                result = item.desc_oggetto;
            }
            return result;
        }

        protected string GetLabeIdSignatureDate(ItemsConservazione item)
        {
            return item.numProt_or_id + "  " + item.data_prot_or_create;
        }

        protected string GetLabelTypeDoc(ItemsConservazione item)
        {
            if (item.TipoDoc.Equals("G"))
            {
                return "NP";
            }
            else
            {
                return item.TipoDoc;
            }
        }

        #endregion

        #region Event

        protected void ViewInstanceConservationSendForConservation_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (!string.IsNullOrEmpty(this.TxtEnterDescription.Text))
                {
                    // Validazione istanza di conservazione
                    AreaConservazioneValidationResult result = DocumentManager.validateIstanzaConservazione(InstanceConservation.SystemID);

                    if (result != null && result.IsValid)
                    {
                        // Invio istanza di conservazione
                        if (SendInstance())
                        {
                            this.UpPanelField.Update();
                            DisablesBasket();
                            this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                            this.UpPnlButtons.Update();
                            this.UpPanelGridItemsConservation.Update();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessSendForConservationInstanceConservation', 'check', '','');", true);
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ForcePostback", "__doPostBack();", true);
                            //this.UpPanelField.Update();
                            //DisablesBasket();
                            //this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                            //this.UpPnlButtons.Update();
                            //this.UpPanelGridItemsConservation.Update();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSendForConservationInstanceConservation', 'error', '','');", true);
                            return;
                        }
                    }
                    else
                    {
                        if (result != null)
                        {
                            IdInstance = InstanceConservation.SystemID;
                            ResultValidateInstance = result;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupConservationAreaValidation", "ajaxModalPopupConservationAreaValidation();", true);
                            return;
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSendForConservationDescriptionInstanceConservation', 'warning', '','');", true);
                    return;
                }
                //hdForceSend.Value = "false";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
        #region MEV 1.5 F02_01

        //Verifica e valida i documenti associati all'istanza
        protected void ViewInstanceConservationCheckFormat_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);

            //this.InstanceConservation.StatoConservazione.Equals(
            bool result = DocumentManager.checkAndValidateIstanzaConservazione(InstanceConservation.SystemID);

            if (result)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessCheckForConservationInstanceConservation', 'check', '','');", true);
                this.disableButton();
                this.DisablesBasket();
                this.UpPanelGridItemsConservation.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCheckForConservationInstanceConservation', 'warning', '','');", true);
            }
            return;

        }

        private void disableButton()
        {
            this.ViewInstanceConservationSendForConservation.Enabled = false;
            this.ViewInstanceConservationRemoveAll.Enabled = false;
            this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
            this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
            this.ViewInstanceConservationCheckFormat.Enabled = false;
            this.ViewInstanceConservationConvertAndSendForConservation.Enabled = false;
            this.ViewInstanceConservationViewReport.Enabled = false;
            this.UpPnlButtons.Update();
        }

        //mostra il popup del report sulla verifica formati
        protected void ViewInstanceConservationViewReport_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["ReportConservazioneListaDoc"] = null;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReportFormatiConservazione", "ajaxModalPopupReportFormatiConservazione();", true);

            return;
        }

        //converti e invia al cs
        protected void ViewInstanceConservationConvertAndSendForConservation_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (!string.IsNullOrEmpty(this.TxtEnterDescription.Text))
            {

                bool result = DocumentManager.convertAndSendForConservation(InstanceConservation.SystemID,
                                                                            this.TxtEnterNote.Text.Replace("'", "''"),
                                                                            this.TxtEnterDescription.Text.Replace("'", "''"),
                                                                            this.ddl_typeConservation.SelectedValue,
                                                                            string.Empty,
                                                                            this.cbConsolidatesDocuments.Checked);
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessConvertAndSendConservationInstanceConservation', 'check', '','');", true);
                    this.disableButton();
                    this.DisablesBasket();
                    this.UpPanelGridItemsConservation.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorConvertAndSendConservationInstanceConservation', 'warning', '','');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningSendForConservationDescriptionInstanceConservation', 'warning', '','');", true);
                return;
            }
            return;
        }

        protected void btnReportFormatiIstanzaConservazionePostback_Click(object sender, EventArgs e)
        {

        }

        #endregion

        protected void ViewInstanceConservationEnableInstanceReject_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                List<string> idProjectFascInseriti = new List<string>();
                int numFasc = 0;

                #region NewCode Mev CS 1.5 - F03_01
                // Indice dell'elemento lavorato
                int indexOfCurrentDoc = 0;

                // Dimensione complessiva raggiunta
                int currentDimOfDocs = 0;

                // Get valori limiti per le istanze di conservazione
                int DimMaxInIstanza = 0;
                int numMaxDocInIstanza = 0;
                int TolleranzaPercentuale = 0;
                try
                {
                    InfoUtente infoUt = UserManager.GetInfoUser();
                    DimMaxInIstanza = DocumentManager.getDimensioneMassimaIstanze(infoUt.idAmministrazione);
                    numMaxDocInIstanza = DocumentManager.getNumeroDocMassimoIstanze(infoUt.idAmministrazione);
                    TolleranzaPercentuale = DocumentManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
                }
                catch (Exception ex)
                {
                }
                #endregion

                for (int i = 0; i < ItemsConservation.Count; i++)
                {
                    // se il documento fa parte di un fascicolo
                    if (!string.IsNullOrEmpty(ItemsConservation[i].ID_Project))
                    {
                        if (ItemsConservation[i].tipo_oggetto == "F") //se è inserito tutto il fascicolo
                        {
                            if (!isInserito(ItemsConservation[i].ID_Project, idProjectFascInseriti))
                            {
                                // Mev CS 1.5 - F03_01
                                #region oldCode
                                //this.inserisciFascicoloInConservazione(ItemsConservation[i].ID_Project);
                                //idProjectFascInseriti.Add(ItemsConservation[i].ID_Project);
                                #endregion

                                #region NewCode
                                this.inserisciFascicoloInConservazione_WithConstraint(ItemsConservation[i].ID_Project,
                                DimMaxInIstanza,
                                numMaxDocInIstanza,
                                TolleranzaPercentuale);
                                idProjectFascInseriti.Add(ItemsConservation[i].ID_Project);
                                #endregion
                            }
                        }
                        else
                        {
                            // Mev CS 1.5 - F03_01
                            #region OldCode
                            //this.inserisciDocumentoInConservazione(ItemsConservation[i].ID_Profile, ItemsConservation[i].DocNumber, ItemsConservation[i].ID_Project);
                            #endregion

                            #region NewCode
                            // Inserimento in areaConservazione nel rispetto dei vincoli
                            this.inserisciDocumentoInConservazione_WithConstraint(ItemsConservation[i].ID_Profile,
                                ItemsConservation[i].DocNumber,
                                ItemsConservation[i].ID_Project,
                                ref indexOfCurrentDoc,
                                ref currentDimOfDocs,
                                DimMaxInIstanza,
                                numMaxDocInIstanza,
                                TolleranzaPercentuale);
                            #endregion
                        }
                    }
                    else
                    {
                        // Mev CS 1.5 - F03_01
                        #region OldCode
                        //this.inserisciDocumentoInConservazione(ItemsConservation[i].ID_Profile, ItemsConservation[i].DocNumber, ItemsConservation[i].ID_Project);
                        #endregion

                        #region NewCode
                        // Inserimento in areaConservazione nel rispetto dei vincoli
                        this.inserisciDocumentoInConservazione_WithConstraint(ItemsConservation[i].ID_Profile,
                            ItemsConservation[i].DocNumber,
                            ItemsConservation[i].ID_Project,
                            ref indexOfCurrentDoc,
                            ref currentDimOfDocs,
                            DimMaxInIstanza,
                            numMaxDocInIstanza,
                            TolleranzaPercentuale);
                        #endregion
                    }
                }

                this.ViewInstanceConservationEnableInstanceReject.Enabled = false;
                //this.panelDettaglioIstanza.Visible = false;
                //elimino l'istanza originale che è stata rifiutata
                DocumentManager.eliminaDaAreaConservazione(null, null, ItemsConservation[0].ID_Conservazione, true, "");
                SetSize();
                SetNumberDocument();
                this.UpPanelLabelInfoNumberSizeDocument.Update();
                this.UpPanelGridItemsConservation.Update();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ViewInstanceConservationRemoveAll_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                DocumentManager.eliminaDaAreaConservazione(null, null, InstanceConservation.SystemID, false, "");
                LoadItemsConservation();
                GrdItemsConservation_Bind();
                InitializePage();
                SetSize();
                SetNumberDocument();
                this.UpPanelLabelInfoNumberSizeDocument.Update();
                this.UpPanelGridItemsConservation.Update();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ViewInstanceConservationSelectedAsDefault_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                #region MEV CS 1.5 - F03_01 - NewCode
                bool dimViolata = false;
                bool numDocViolata = false;

                int dimMaxIstByAmm = 0;
                int numDocMaxIstByAmm = 0;
                dimMaxIstByAmm = DocumentManager.getDimensioneMassimaIstanze(UserManager.GetInfoUser().idAmministrazione);
                numDocMaxIstByAmm = DocumentManager.getNumeroDocMassimoIstanze(UserManager.GetInfoUser().idAmministrazione);

                int numDocCorrenteInIstanza = 0;
                int dimCorrenteInIstanza = 0;
                dimCorrenteInIstanza = DocumentManager.getDimensioneCorrenteIstanzaByte_byIdIstanza(InstanceConservation.SystemID);
                numDocCorrenteInIstanza = DocumentManager.getNumeroDocIstanza_byIDIstanza(InstanceConservation.SystemID);

                if (dimCorrenteInIstanza >= dimMaxIstByAmm)
                    dimViolata = true;

                if (numDocCorrenteInIstanza >= numDocMaxIstByAmm)
                    numDocViolata = true;

                if (dimViolata || numDocViolata)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ConstraintViolationSetInstanceDefaultInstanceConservation', 'error', '','');", true);
                }
                else
                {

                    if (DocumentManager.UpdatePreferredInstance(InstanceConservation.SystemID, UserManager.GetInfoUser(), RoleManager.GetRoleInSession()))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessSetInstanceDefaultInstanceConservation', 'check', '','');", true);
                        this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                        this.UpPnlButtons.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSetInstanceDefaultInstanceConservation', 'error', '','');", true);
                    }
                }
                #endregion

                #region oldCode
                //if (DocumentManager.UpdatePreferredInstance(InstanceConservation.SystemID, UserManager.GetInfoUser(), RoleManager.GetRoleInSession()))
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessSetInstanceDefaultInstanceConservation', 'check', '','');", true);
                //    this.ViewInstanceConservationSelectedAsDefault.Enabled = false;
                //    this.UpPnlButtons.Update();
                //}
                //else
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorSetInstanceDefaultInstanceConservation', 'error', '','');", true);
                //}
                #endregion
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ViewInstanceConservationRemoveNonCompliant_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (!string.IsNullOrEmpty(this.ddl_validInstance.SelectedValue))
                {
                    string policy_validazione = InstanceConservation.idPolicyValidata;
                    bool result = DocumentManager.ValidateIstanzaConservazioneConPolicy(this.ddl_validInstance.SelectedValue, InstanceConservation.SystemID, UserManager.GetInfoUser());

                    result = DocumentManager.EliminaDocumentiNonConformiPolicyDaIstanza(this.ddl_validInstance.SelectedValue, InstanceConservation.SystemID, UserManager.GetInfoUser());
                    if (result)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessRemoveNonCompliantPolicyInstanceConservation', 'check', '','');", true);
                        LoadItemsConservation();
                        GrdItemsConservation_Bind();
                        SetSize();
                        SetNumberDocument();
                        this.UpPanelGridItemsConservation.Update();
                        this.UpPanelLabelInfoNumberSizeDocument.Update();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningRemoveNonCompliantPolicyInstanceConservation', 'warning', '','');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ViewInstanceConservationClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('ViewDetailsInstanceConservation','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_validInstance_OnChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                string idPolicy = this.ddl_validInstance.SelectedValue;
                if (!string.IsNullOrEmpty(idPolicy))
                {
                    if (DocumentManager.ValidateIstanzaConservazioneConPolicy(idPolicy, InstanceConservation.SystemID, UserManager.GetInfoUser()))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessValidateVanlidateInstanceConservationPolicy', 'check', '','');", true);
                    }
                    this.ViewInstanceConservationRemoveNonCompliant.Enabled = true;
                }
                else
                {
                    if (DocumentManager.DeleteValidateIstanzaConservazioneConPolicy(idPolicy, InstanceConservation.SystemID, UserManager.GetInfoUser()))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('SuccessDeleteVanlidateInstanceConservationPolicy', 'check', '','');", true);
                    }
                    this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                }
                LoadItemsConservation();
                GrdItemsConservation_Bind();
                this.UpPanelGridItemsConservation.Update();
                this.UpPanelField.Update();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_typeConservation_OnChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (!string.IsNullOrEmpty(this.ddl_typeConservation.SelectedValue))
            {
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                {
                    this.cbConsolidatesDocuments.Checked = true;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "ESIBIZIONE")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = false;
                }
                if (this.ddl_typeConservation.SelectedValue == "CONSERVAZIONE_INTERNA")
                {
                    this.cbConsolidatesDocuments.Checked = false;
                    this.cbConsolidatesDocuments.Enabled = true;
                }
                this.UpPanelCbConsolidateDocuments.Update();
            }
        }

        #endregion

        #region Utils

        protected bool SendInstance()
        {

            if (DocumentManager.updateStatoAreaCons(InstanceConservation.SystemID,
                this.ddl_typeConservation.SelectedValue, this.TxtEnterNote.Text.Replace("'", "''"),
                this.TxtEnterDescription.Text.Replace("'", "''"), "", UserManager.GetInfoUser(), this.cbConsolidatesDocuments.Checked))
            {

                this.ViewInstanceConservationSendForConservation.Enabled = false;
                this.ViewInstanceConservationRemoveAll.Enabled = false;
                this.ViewInstanceConservationRemoveNonCompliant.Enabled = false;
                DisabledFields();
                DisablesBasket();
                this.UpPanelField.Update();
                return true;
                //this.hdForceSend.Value = "false";

            }
            else
            {
                return false;
            }
        }

        private void DisabledFields()
        {
            this.TxtEnterDescription.Enabled = false;
            this.TxtEnterNote.Enabled = false;
            this.ddl_typeConservation.Enabled = false;
            this.ddl_validInstance.Enabled = false;
        }

        protected bool isInserito(string idProject, List<string> idFascInseriti)
        {
            bool result = false;
            if (idFascInseriti.Count > 0)
            {
                for (int i = 0; i < idFascInseriti.Count; i++)
                {
                    if (idProject.Trim() == (idFascInseriti[i]).Trim())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        protected void inserisciDocumentoInConservazione(string idProfile, string docNumber, string idProject)
        {
            string errorMessage = string.Empty;
            SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(idProfile, docNumber, out errorMessage);// .getDettaglioDocumento(this, idProfile, docNumber);
            if (schedaDoc != null)
            {
                if (schedaDoc.inCestino == null)
                    schedaDoc.inCestino = string.Empty;
                if (schedaDoc.inCestino.Trim() != "1")
                {
                    string sysId = DocumentManager.addAreaConservazione(idProfile, idProject, docNumber, UserManager.GetInfoUser(), "D").ToString();

                    if (sysId != "-1")
                    {
                        int size_xml = DocumentManager.getItemSize(schedaDoc, sysId);
                        int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);

                        int numeroAllegati = schedaDoc.allegati.Length;
                        string fileName = schedaDoc.documenti[0].fileName;
                        string tipoFile = System.IO.Path.GetExtension(fileName);

                        int size_allegati = 0;
                        for (int i = 0; i < schedaDoc.allegati.Length; i++)
                        {
                            size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[i].fileSize);
                        }
                        int total_size = size_allegati + doc_size + size_xml;

                        DocumentManager.insertSizeInItemCons(sysId, total_size);

                        DocumentManager.updateItemsConservazione(tipoFile, Convert.ToString(numeroAllegati), sysId);
                    }
                }
            }
            if (errorMessage != string.Empty)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
            }

        }

        /// <summary>
        /// Nuovo metodo per l'inserimento di documenti in conservazione nel rispetto dei vincoli delle istanze
        /// </summary>
        protected void inserisciDocumentoInConservazione_WithConstraint(string idProfile,
            string docNumber,
            string idProject,
            ref int indexOfCurrentDoc,
            ref int currentDimOfDocs,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            string errorMessage = string.Empty;
            SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(idProfile, docNumber, out errorMessage);// .getDettaglioDocumento(this, idProfile, docNumber);
            if (schedaDoc != null)
            {
                if (schedaDoc.inCestino == null)
                    schedaDoc.inCestino = string.Empty;
                if (schedaDoc.inCestino.Trim() != "1")
                {
                    // Controllo Rispetto dei Vincoli dell'istanza
                    #region Vincoli Istanza di Conservazione
                    // Variabili di controllo per violazione dei vincoli sulle istanze
                    bool numDocIstanzaViolato = false;
                    bool dimIstanzaViolato = false;
                    int TotalSelectedDocumentSize = 0;

                    TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(schedaDoc);
                    // Dimensione documenti raggiunta
                    currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
                    // Numero di documenti raggiunti
                    indexOfCurrentDoc = indexOfCurrentDoc + 1;

                    numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
                    dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, percentualeTolleranza);

                    double DimensioneMassimaConsentitaPerIstanza = 0;
                    DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * percentualeTolleranza) / 100);

                    int DimMaxConsentita = 0;
                    DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

                    if (numDocIstanzaViolato || dimIstanzaViolato)
                    {
                        // Azzero le due variabili
                        currentDimOfDocs = 0;
                        indexOfCurrentDoc = 0;
                    }
                    #endregion

                    string sysId = DocumentManager.addAreaConservazione_WithConstraint(idProfile,
                        idProject,
                        docNumber,
                        UserManager.GetInfoUser(),
                        "D",
                        numDocIstanzaViolato,
                        dimIstanzaViolato,
                        DimMaxConsentita,
                        numMaxDocInIstanza,
                        TotalSelectedDocumentSize).ToString();

                    if (sysId != "-1")
                    {
                        int size_xml = DocumentManager.getItemSize(schedaDoc, sysId);
                        int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);

                        int numeroAllegati = schedaDoc.allegati.Length;
                        string fileName = schedaDoc.documenti[0].fileName;
                        string tipoFile = System.IO.Path.GetExtension(fileName);

                        int size_allegati = 0;
                        for (int i = 0; i < schedaDoc.allegati.Length; i++)
                        {
                            size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[i].fileSize);
                        }
                        int total_size = size_allegati + doc_size + size_xml;

                        DocumentManager.insertSizeInItemCons(sysId, total_size);

                        DocumentManager.updateItemsConservazione(tipoFile, Convert.ToString(numeroAllegati), sysId);
                    }
                }
            }
            if (errorMessage != string.Empty)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
            }

        }

        protected void inserisciFascicoloInConservazione(string idProject)
        {
            string[] listaDoc;
            listaDoc = ProjectManager.getIdDocumentiFromFascicolo(idProject);
            for (int i = 0; i < listaDoc.Length; i++)
            {
                string errorMessage = string.Empty;
                SchedaDocumento schedaDoc = new SchedaDocumento();
                schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(listaDoc[i].ToString(), "", out errorMessage);
                if (schedaDoc != null)
                {
                    if (schedaDoc.inCestino == null)
                        schedaDoc.inCestino = string.Empty;
                    if (schedaDoc.inCestino.Trim() != "1")
                    {
                        string sysId = DocumentManager.addAreaConservazione(schedaDoc.systemId, idProject, schedaDoc.docNumber, UserManager.GetInfoUser(), "F").ToString();
                        int size_xml = DocumentManager.getItemSize(schedaDoc, sysId);
                        int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);
                        int size_allegati = 0;
                        for (int j = 0; j < schedaDoc.allegati.Length; j++)
                        {
                            size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[j].fileSize);
                        }
                        int total_size = size_allegati + doc_size + size_xml;

                        int numeroAllegati = schedaDoc.allegati.Length;
                        string fileName = schedaDoc.documenti[0].fileName;
                        string tipoFile = System.IO.Path.GetExtension(fileName);

                        DocumentManager.insertSizeInItemCons(sysId, total_size);

                        DocumentManager.updateItemsConservazione(tipoFile, Convert.ToString(numeroAllegati), sysId);
                    }
                }
                if (errorMessage != string.Empty)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
                }
            }
        }

        /// <summary>
        /// Nuovo metodo per l'inserimento di documenti di un fascicolo in conservazione nel rispetto dei vincoli delle istanze
        /// </summary>
        protected void inserisciFascicoloInConservazione_WithConstraint(string idProject,
            int DimMaxInIstanza,
            int numMaxDocInIstanza,
            int percentualeTolleranza
            )
        {
            // Mev CS 1.5 - F03_01
            // New Code
            #region NewCode
            // Indice dell'elemento lavorato
            int indexOfCurrentDoc = 0;

            // Dimensione complessiva raggiunta
            int currentDimOfDocs = 0;

            #endregion
            //End New Code

            string[] listaDoc;
            listaDoc = ProjectManager.getIdDocumentiFromFascicolo(idProject);
            for (int i = 0; i < listaDoc.Length; i++)
            {
                string errorMessage = string.Empty;
                SchedaDocumento schedaDoc = new SchedaDocumento();
                schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(listaDoc[i].ToString(), "", out errorMessage);
                if (schedaDoc != null)
                {
                    if (schedaDoc.inCestino == null)
                        schedaDoc.inCestino = string.Empty;
                    if (schedaDoc.inCestino.Trim() != "1")
                    {
                        // Controllo Rispetto dei Vincoli dell'istanza
                        #region Vincoli Istanza di Conservazione
                        // Variabili di controllo per violazione dei vincoli sulle istanze
                        bool numDocIstanzaViolato = false;
                        bool dimIstanzaViolato = false;
                        int TotalSelectedDocumentSize = 0;

                        TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(schedaDoc);
                        // Dimensione documenti raggiunta
                        currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
                        // Numero di documenti raggiunti
                        indexOfCurrentDoc = indexOfCurrentDoc + 1;

                        numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
                        dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, percentualeTolleranza);

                        double DimensioneMassimaConsentitaPerIstanza = 0;
                        DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * percentualeTolleranza) / 100);

                        int DimMaxConsentita = 0;
                        DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

                        if (numDocIstanzaViolato || dimIstanzaViolato)
                        {
                            // Azzero le due variabili
                            currentDimOfDocs = 0;
                            indexOfCurrentDoc = 0;
                        }
                        #endregion

                        string sysId = DocumentManager.addAreaConservazione_WithConstraint(schedaDoc.systemId,
                            idProject,
                            schedaDoc.docNumber,
                            UserManager.GetInfoUser(),
                            "F",
                            numDocIstanzaViolato,
                            dimIstanzaViolato,
                            DimMaxConsentita,
                            numMaxDocInIstanza,
                            TotalSelectedDocumentSize).ToString();

                        if (sysId != "-1")
                        {
                            int size_xml = DocumentManager.getItemSize(schedaDoc, sysId);
                            int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);
                            int size_allegati = 0;
                            for (int j = 0; j < schedaDoc.allegati.Length; j++)
                            {
                                size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[j].fileSize);
                            }
                            int total_size = size_allegati + doc_size + size_xml;

                            int numeroAllegati = schedaDoc.allegati.Length;
                            string fileName = schedaDoc.documenti[0].fileName;
                            string tipoFile = System.IO.Path.GetExtension(fileName);

                            DocumentManager.insertSizeInItemCons(sysId, total_size);

                            DocumentManager.updateItemsConservazione(tipoFile, Convert.ToString(numeroAllegati), sysId);
                        }
                    }
                }
                if (errorMessage != string.Empty)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
                }
            }
        }

        #endregion
    }
}
