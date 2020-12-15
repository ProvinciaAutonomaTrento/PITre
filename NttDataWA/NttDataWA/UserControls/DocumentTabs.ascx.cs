using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UserControls
{
    public partial class DocumentTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                    this.LoadKeys();
                    this.VisibiltyRoleFunctions();
                    this.RemoveProperty();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadKeys()
        {

        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PRO_VISIBILITA"))
            {
                this.LiVisibility.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_TRASMISSIONI"))
            {
                this.LiTransmissions.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ALLEGATI"))
            {
                this.LiAttachedFiles.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_OG_STORIA"))
            {
                this.LiEvents.Visible = false;
            }
        }
      
        public void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkProfile.Text = Utils.Languages.GetLabelFromCode("LinkProfile", language);
            this.LinkClassificationSchemes.Text = Utils.Languages.GetLabelFromCode("LinkClassificationSchemes", language);
            this.LinkAttachedFiles.Text = Utils.Languages.GetLabelFromCode("LinkAttachedFiles", language);
            this.LinkTransmissions.Text = Utils.Languages.GetLabelFromCode("LinkTransmissions", language);
            this.LinkVisibility.Text = Utils.Languages.GetLabelFromCode("LinkVisibility", language);
            this.LinkEvents.Text = Utils.Languages.GetLabelFromCode("LinkEvents", language);
        }

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
             
                SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();

                switch (this.PageCaller.ToUpper())
                {
                    case "DOCUMENT":
                        this.LinkProfile.Enabled = true;
                        this.LiProfile.Attributes.Remove("class");
                        this.LiProfile.Attributes.Add("class", "docIAmProfile");
                        if (doc!=null && doc.documentoPrincipale != null)
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkEvents.Enabled = true; //Emanuela 23-01-2015: richiesta da zanotti la possibilità di visuallizare gli eventi per gli allegati

                            this.LiEvents.Attributes.Remove("class");

                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                            LiClassificationSchemes.Visible = false;
                            LiAttachedFiles.Visible = false;
                            LiTransmissions.Visible = false;
                            LiVisibility.Visible = false;
                            //LiEvents.Visible = false;
                        }
                        else if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;
                            this.LinkEvents.Enabled = true;

                            this.LiClassificationSchemes.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            this.LiTransmissions.Attributes.Add("class", "docOther");
                            this.LiVisibility.Attributes.Add("class", "docOther");
                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        else if (this.IsForwarded || (UIManager.DocumentManager.IsNewDocument() && doc.repositoryContext != null))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            string language = UIManager.UserManager.GetUserLanguage();
                            string attachment = Utils.Languages.GetLabelFromCode("DocumentTabAttachment", language);
                            if (doc.allegati != null && doc.allegati.Length > 0)
                            {
                                this.LinkAttachedFiles.ToolTip = doc.allegati.Length.ToString() + " " + attachment;
                                this.LiAttachedFiles.Attributes.Add("class", "docOtherUnderline");
                            }
                            else
                            {
                                this.LinkAttachedFiles.ToolTip = "0" + " " + attachment;
                            }
                            if(!this.IsForwarded)
                                this.LiAttachedFiles.Attributes["onclick"] = "$('#btnChangeTabNewDocument').click();return false;";
                            
                                //this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody');__doPostBack('UpDocumentTabs', 'EditDocumentInWorking');return false;";
                        }
                        break;

                    case "CLASSIFICATIONS":
                        this.LinkClassificationSchemes.Enabled = true;
                        this.LiClassificationSchemes.Attributes.Remove("class");
                        this.LiClassificationSchemes.Attributes.Add("class", "docIAmProfile");
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkEvents.Enabled = true;
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            this.LiTransmissions.Attributes.Add("class", "docOther");
                            this.LiVisibility.Attributes.Add("class", "docOther");
                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "ATTACHMENTS":
                        this.LinkAttachedFiles.Enabled = true;
                        this.LiAttachedFiles.Attributes.Remove("class");
                        this.LiAttachedFiles.Attributes.Add("class", "docIAmProfile");
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkEvents.Enabled = true;
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiClassificationSchemes.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                            this.LiTransmissions.Attributes.Add("class", "docOther");
                            this.LiVisibility.Attributes.Add("class", "docOther");
                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                        }
                        else if (this.IsForwarded || (UIManager.DocumentManager.IsNewDocument() && doc.repositoryContext != null))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LiProfile.Attributes.Remove("class");
                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "TRANSMISSIONS":
                        this.LinkTransmissions.Enabled = true;
                        this.LinkTransmissions.Attributes.Remove("class");
                        this.LiTransmissions.Attributes.Add("class", "docIAmProfile");
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkEvents.Enabled = true;
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiClassificationSchemes.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            this.LiVisibility.Attributes.Add("class", "docOther");
                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "VISIBILITY":
                        this.LinkVisibility.Enabled = true;
                        this.LinkVisibility.Attributes.Remove("class");
                        this.LiVisibility.Attributes.Add("class", "docIAmProfile");
                        if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkEvents.Enabled = true;
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiClassificationSchemes.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            this.LiTransmissions.Attributes.Add("class", "docOther");
                            this.LiEvents.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "EVENTS":
                        this.LinkEvents.Enabled = true;
                        this.LinkEvents.Attributes.Remove("class");
                        this.LiEvents.Attributes.Add("class", "docIAmProfile");
                        //Emanuela 23-01-2015: richiesta da zanotti la possibilità di visuallizare gli eventi per gli allegati
                        if (doc != null && doc.documentoPrincipale != null)
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkEvents.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiProfile.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                            LiClassificationSchemes.Visible = false;
                            LiAttachedFiles.Visible = false;
                            LiTransmissions.Visible = false;
                            LiVisibility.Visible = false;

                        }
                        else if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                        {
                            this.LinkAttachedFiles.Enabled = true;
                            this.LinkClassificationSchemes.Enabled = true;
                            this.LinkEvents.Enabled = true;
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiClassificationSchemes.Attributes.Remove("class");
                            this.LiAttachedFiles.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "docOther");
                            this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                            this.LiAttachedFiles.Attributes.Add("class", "docOther");
                            this.LiTransmissions.Attributes.Add("class", "docOther");
                            this.LiVisibility.Attributes.Add("class", "docOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;
                }

                this.UnderlineTab(this.PageCaller.ToUpper());
            }

        }

        private void UnderlineTab(string pageCaller)
        {
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();

            string language = UIManager.UserManager.GetUserLanguage();
            string attachment = Utils.Languages.GetLabelFromCode("DocumentTabAttachment", language);
            string classificationScheme = Utils.Languages.GetLabelFromCode("DocumentTabClassification2", language);
            string transmissions = Utils.Languages.GetLabelFromCode("DocumentTabTransmission", language);
            string deletedVisibility = Utils.Languages.GetLabelFromCode("DeletedVisibilityTooltip", language);

            DocsPaWR.Tab tab = null;

            if (doc != null && !string.IsNullOrEmpty(doc.systemId))
            {
                tab = UIManager.DocumentManager.GetDocumentTab(doc.systemId, UIManager.UserManager.GetInfoUser());

                if (tab != null && tab.ClassificationsNumber != "0")
                {
                    this.LinkClassificationSchemes.ToolTip = tab.ClassificationsNumber + " " + classificationScheme;
                    this.LiClassificationSchemes.Attributes.Remove("class");

                    if (pageCaller.Equals("CLASSIFICATIONS"))
                    {
                        this.LiClassificationSchemes.Attributes.Add("class", "docIAmProfileUnderline");
                    }
                    else
                    {
                        this.LiClassificationSchemes.Attributes.Add("class", "docOtherUnderline");
                    }
                }
                else
                {
                    this.LinkClassificationSchemes.ToolTip = "0" + " " + classificationScheme;
                }


                if (doc.allegati != null && doc.allegati.Length > 0)
                {
                    this.LinkAttachedFiles.ToolTip = doc.allegati.Length.ToString() + " " + attachment;
                    this.LiAttachedFiles.Attributes.Remove("class");
                    if (pageCaller.Equals("ATTACHMENTS"))
                    {
                         this.LiAttachedFiles.Attributes.Add("class", "docIAmProfileUnderline");
                    }
                    else
                    {
                        this.LiAttachedFiles.Attributes.Add("class", "docOtherUnderline");
                    }

                }
                else
                {
                    this.LinkAttachedFiles.ToolTip = "0" + " " + attachment;
                }

                if (tab != null && !tab.TransmissionsNumber.Equals("0"))
                {
                    this.LinkTransmissions.ToolTip = tab.TransmissionsNumber + " " + transmissions;
                    this.LiTransmissions.Attributes.Remove("class");
                    if (pageCaller.Equals("TRANSMISSIONS"))
                    {
                        this.LiTransmissions.Attributes.Add("class", "docIAmProfileUnderline");
                    }
                    else
                    {
                        this.LiTransmissions.Attributes.Add("class", "docOtherUnderline");
                    }

                }
                else
                {
                    this.LinkTransmissions.ToolTip = "0" + " " + transmissions;
                }

                if (tab != null && tab.DeletedSecurity)
                {
                    this.LinkVisibility.ToolTip = deletedVisibility;
                    this.LiVisibility.Attributes.Remove("class");
                    if (pageCaller.Equals("VISIBILITY"))
                    {
                        this.LiVisibility.Attributes.Add("class", "docIAmProfileUnderline");
                    }
                    else
                    {
                        this.LiVisibility.Attributes.Add("class", "docOtherUnderline");
                    }

                }

            }
        }

        public void ResetLayoutTab(bool repeatAdvanced)
        {
            this.LinkProfile.Enabled = true;
            this.LinkClassificationSchemes.Enabled = false;
            this.LinkAttachedFiles.Enabled = false;
            this.LinkTransmissions.Enabled = false;
            this.LinkEvents.Enabled = false;
            this.LinkVisibility.Enabled = false;

            //this.LinkAttachedFiles.ToolTip = "0 " + Utils.Languages.GetLabelFromCode("DocumentTabAttachment", UIManager.UserManager.GetUserLanguage());
            if(!repeatAdvanced)
            this.LinkAttachedFiles.ToolTip = null;

            this.LinkTransmissions.ToolTip = null;
            this.LinkClassificationSchemes.ToolTip = null;
            this.LinkEvents.ToolTip = null;
            this.LinkVisibility.ToolTip = null;

            this.LiProfile.Attributes.Remove("class");
            this.LiClassificationSchemes.Attributes.Remove("class");
            this.LiAttachedFiles.Attributes.Remove("class");
            this.LiTransmissions.Attributes.Remove("class");
            this.LiVisibility.Attributes.Remove("class");
            this.LiEvents.Attributes.Remove("class");

            this.LiProfile.Attributes.Add("class", "docIAmProfile");
            this.LiClassificationSchemes.Attributes.Add("class", "docOtherD");
            this.LiAttachedFiles.Attributes.Add("class", "docOtherD");
            this.LiTransmissions.Attributes.Add("class", "docOtherD");
            this.LiVisibility.Attributes.Add("class", "docOtherD");
            this.LiEvents.Attributes.Add("class", "docOtherD");

            this.LiProfile.Attributes.Remove("onclick");
            this.LiClassificationSchemes.Attributes.Remove("onclick");
            this.LiAttachedFiles.Attributes.Remove("onclick");
            this.LiTransmissions.Attributes.Remove("onclick");
            this.LiVisibility.Attributes.Remove("onclick");
            this.LiEvents.Attributes.Remove("onclick");

            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
            if (doc.repositoryContext != null)
            {
                this.LinkAttachedFiles.Enabled = true;
                this.LiAttachedFiles.Attributes.Add("class", "docOther");
                string language = UIManager.UserManager.GetUserLanguage();
                string attachment = Utils.Languages.GetLabelFromCode("DocumentTabAttachment", language);
                this.LinkAttachedFiles.ToolTip = "0" + " " + attachment;
            }


            this.UpDocumentTabs.Update();
        }

        public virtual void RefreshLayoutTab()
        {
            switch (this.PageCaller.ToUpper())
            {
                case "DOCUMENT":
                    this.LinkProfile.Enabled = true;
                    this.LinkClassificationSchemes.Enabled = true;
                    this.LinkAttachedFiles.Enabled = true;
                    this.LinkTransmissions.Enabled = true;
                    this.LinkVisibility.Enabled = true;
                    this.LinkEvents.Enabled = true;

                    this.LiClassificationSchemes.Attributes.Remove("class");
                    this.LiAttachedFiles.Attributes.Remove("class");
                    this.LiTransmissions.Attributes.Remove("class");
                    this.LiVisibility.Attributes.Remove("class");
                    this.LiEvents.Attributes.Remove("class");

                    this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                    this.LiAttachedFiles.Attributes.Add("class", "docOther");
                    this.LiTransmissions.Attributes.Add("class", "docOther");
                    this.LiVisibility.Attributes.Add("class", "docOther");
                    this.LiEvents.Attributes.Add("class", "docOther");

                    this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                    break;

                case "CLASSIFICATIONS":
                    this.LinkProfile.Enabled = true;
                    this.LinkClassificationSchemes.Enabled = true;
                    this.LinkAttachedFiles.Enabled = true;
                    this.LinkTransmissions.Enabled = true;
                    this.LinkVisibility.Enabled = true;
                    this.LinkEvents.Enabled = true;

                    this.LiProfile.Attributes.Remove("class");
                    this.LiAttachedFiles.Attributes.Remove("class");
                    this.LiTransmissions.Attributes.Remove("class");
                    this.LiVisibility.Attributes.Remove("class");
                    this.LiEvents.Attributes.Remove("class");

                    this.LiProfile.Attributes.Add("class", "docOther");
                    this.LiAttachedFiles.Attributes.Add("class", "docOther");
                    this.LiTransmissions.Attributes.Add("class", "docOther");
                    this.LiVisibility.Attributes.Add("class", "docOther");
                    this.LiEvents.Attributes.Add("class", "docOther");

                    this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                    break;

                case "ATTACHMENTS":
                    SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                    if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                    {
                        this.LinkProfile.Enabled = true;
                        this.LinkClassificationSchemes.Enabled = true;
                        this.LinkAttachedFiles.Enabled = true;
                        this.LinkTransmissions.Enabled = true;
                        this.LinkVisibility.Enabled = true;
                        this.LinkEvents.Enabled = true;

                        this.LiProfile.Attributes.Remove("class");
                        this.LiClassificationSchemes.Attributes.Remove("class");
                        this.LiTransmissions.Attributes.Remove("class");
                        this.LiVisibility.Attributes.Remove("class");
                        this.LiEvents.Attributes.Remove("class");

                        this.LiProfile.Attributes.Add("class", "docOther");
                        this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                        this.LiTransmissions.Attributes.Add("class", "docOther");
                        this.LiVisibility.Attributes.Add("class", "docOther");
                        this.LiEvents.Attributes.Add("class", "docOther");

                        this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    }
                    else if (this.IsForwarded || (UIManager.DocumentManager.IsNewDocument() && doc.repositoryContext != null))
                    {
                        this.LinkProfile.Enabled = true;
                        this.LiProfile.Attributes.Remove("class");
                        this.LiProfile.Attributes.Add("class", "docOther");
                    }

                    break;

                case "TRANSMISSIONS":
                    this.LinkProfile.Enabled = true;
                    this.LinkClassificationSchemes.Enabled = true;
                    this.LinkAttachedFiles.Enabled = true;
                    this.LinkTransmissions.Enabled = true;
                    this.LinkVisibility.Enabled = true;
                    this.LinkEvents.Enabled = true;

                    this.LiProfile.Attributes.Remove("class");
                    this.LiClassificationSchemes.Attributes.Remove("class");
                    this.LiAttachedFiles.Attributes.Remove("class");
                    this.LiVisibility.Attributes.Remove("class");
                    this.LiEvents.Attributes.Remove("class");

                    this.LiProfile.Attributes.Add("class", "docOther");
                    this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                    this.LiAttachedFiles.Attributes.Add("class", "docOther");
                    this.LiVisibility.Attributes.Add("class", "docOther");
                    this.LiEvents.Attributes.Add("class", "docOther");

                    this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                    break;

                case "VISIBILITY":
                    this.LinkProfile.Enabled = true;
                    this.LinkClassificationSchemes.Enabled = true;
                    this.LinkAttachedFiles.Enabled = true;
                    this.LinkTransmissions.Enabled = true;
                    this.LinkVisibility.Enabled = true;
                    this.LinkEvents.Enabled = true;

                    this.LiProfile.Attributes.Remove("class");
                    this.LiClassificationSchemes.Attributes.Remove("class");
                    this.LiAttachedFiles.Attributes.Remove("class");
                    this.LiTransmissions.Attributes.Remove("class");
                    this.LiEvents.Attributes.Remove("class");

                    this.LiProfile.Attributes.Add("class", "docOther");
                    this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                    this.LiAttachedFiles.Attributes.Add("class", "docOther");
                    this.LiTransmissions.Attributes.Add("class", "docOther");
                    this.LiEvents.Attributes.Add("class", "docOther");

                    this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                    break;

                case "EVENTS":
                    this.LinkProfile.Enabled = true;
                    this.LinkClassificationSchemes.Enabled = true;
                    this.LinkAttachedFiles.Enabled = true;
                    this.LinkTransmissions.Enabled = true;
                    this.LinkVisibility.Enabled = true;
                    this.LinkEvents.Enabled = true;

                    this.LiProfile.Attributes.Remove("class");
                    this.LiClassificationSchemes.Attributes.Remove("class");
                    this.LiAttachedFiles.Attributes.Remove("class");
                    this.LiTransmissions.Attributes.Remove("class");
                    this.LiVisibility.Attributes.Remove("class");

                    this.LiProfile.Attributes.Add("class", "docOther");
                    this.LiClassificationSchemes.Attributes.Add("class", "docOther");
                    this.LiAttachedFiles.Attributes.Add("class", "docOther");
                    this.LiTransmissions.Attributes.Add("class", "docOther");
                    this.LiVisibility.Attributes.Add("class", "docOther");

                    this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiClassificationSchemes.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiAttachedFiles.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                    this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";

                    break;
            }

            this.UnderlineTab(this.PageCaller.ToUpper());

            this.UpDocumentTabs.Update();
        }

        /// <summary>
        /// Rimuove i valori in sessione quando si seleziona un nuovo tab
        /// </summary>
        private void RemoveProperty()
        {
            UIManager.DocumentManager.RemoveSelectedAttachId();
            HttpContext.Current.Session.Remove("isZoom");
            HttpContext.Current.Session.Remove("IsInvokingFromProjectStructure");
            HttpContext.Current.Session.Remove("selectedNumberVersion");
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }

        #region Session Utility
        [Browsable(true)]
        public string PageCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["pageCaller"] != null)
                {
                    result = HttpContext.Current.Session["pageCaller"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageCaller"] = value;
            }
        }

        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsForwarded"] = value;
            }
        }

        #endregion
    }
}