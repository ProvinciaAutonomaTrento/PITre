using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class ProjectTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                    this.InitializeKey();
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

        public void InitializePage()
        {
            if (!string.IsNullOrEmpty(this.PageCaller))
            {
                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();

                switch (this.PageCaller.ToUpper())
                {
                    case "PROJECT":
                        this.LinkProfile.Enabled = true;
                        this.LiProfile.Attributes.Remove("class");
                        this.LiProfile.Attributes.Add("class", "prjIAmProfile");

                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            this.LinkTransmissions.Enabled = true;
                            this.LinkStructure.Enabled = true;

                            this.LinkEvents.Enabled = true;

                            this.LiStructure.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiStructure.Attributes.Add("class", "prjOther");
                            this.LiTransmissions.Attributes.Add("class", "prjOther");

                            this.LiEvents.Attributes.Add("class", "prjOther");

                            if (fascicolo.tipo.Equals("G"))
                            {
                                this.LinkVisibility.Enabled = false;
                            }
                            else
                            {
                                this.LinkVisibility.Enabled = true;
                                this.LiVisibility.Attributes.Remove("class");
                                this.LiVisibility.Attributes.Add("class", "prjOther");
                            }

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        else
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = false;
                            this.LinkStructure.Enabled = false;
                            this.LinkVisibility.Enabled = false;
                            this.LinkEvents.Enabled = false;
                        }

                        break;

                    case "VISIBILITY":
                        this.LinkVisibility.Enabled = true;
                        this.LinkVisibility.Attributes.Remove("class");
                        this.LiVisibility.Attributes.Add("class", "prjIAmProfile");

                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkStructure.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkEvents.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiStructure.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "prjOther");
                            this.LiStructure.Attributes.Add("class", "prjOther");
                            this.LiTransmissions.Attributes.Add("class", "prjOther");
                            this.LiEvents.Attributes.Add("class", "prjOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "TRANSMISSIONS":
                        this.LinkTransmissions.Enabled = true;
                        this.LiTransmissions.Attributes.Remove("class");
                        this.LiTransmissions.Attributes.Add("class", "prjIAmProfile");
                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkStructure.Enabled = true;
                            this.LinkVisibility.Enabled = true;
                            this.LinkEvents.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiStructure.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "prjOther");
                            this.LiStructure.Attributes.Add("class", "prjOther");
                            this.LiVisibility.Attributes.Add("class", "prjOther");
                            this.LiEvents.Attributes.Add("class", "prjOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "STRUCTURE":
                        this.LinkStructure.Enabled = true;
                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "prjIAmProfile");
                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;
                            this.LinkEvents.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiEvents.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "prjOther");
                            this.LiVisibility.Attributes.Add("class", "prjOther");
                            this.LiTransmissions.Attributes.Add("class", "prjOther");
                            this.LiEvents.Attributes.Add("class", "prjOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;

                    case "EVENTS":
                        this.LinkEvents.Enabled = true;
                        this.LinkEvents.Attributes.Remove("class");
                        this.LiEvents.Attributes.Add("class", "prjIAmProfile");
                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            this.LinkProfile.Enabled = true;
                            this.LinkTransmissions.Enabled = true;
                            this.LinkVisibility.Enabled = true;
                            this.LinkStructure.Enabled = true;

                            this.LiProfile.Attributes.Remove("class");
                            this.LiVisibility.Attributes.Remove("class");
                            this.LiTransmissions.Attributes.Remove("class");
                            this.LiStructure.Attributes.Remove("class");

                            this.LiProfile.Attributes.Add("class", "prjOther");
                            this.LiVisibility.Attributes.Add("class", "prjOther");
                            this.LiTransmissions.Attributes.Add("class", "prjOther");
                            this.LiStructure.Attributes.Add("class", "prjOther");

                            this.LinkProfile.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiStructure.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiTransmissions.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                            this.LiEvents.Attributes["onclick"] = "disallowOp('IdMasterBody')";
                        }
                        break;
                }

                this.UnderlineTab(this.PageCaller.ToUpper());
                this.UpProjectTabs.Update();
            }
        }

        private void UnderlineTab(string pageCaller)
        {
            Fascicolo prj = UIManager.ProjectManager.getProjectInSession();

            string language = UIManager.UserManager.GetUserLanguage();
            string attachment = Utils.Languages.GetLabelFromCode("DocumentTabAttachment", language);
            string classificationScheme = Utils.Languages.GetLabelFromCode("DocumentTabClassification", language);
            string transmissions = Utils.Languages.GetLabelFromCode("DocumentTabTransmission", language);
            string deletedVisibility = Utils.Languages.GetLabelFromCode("DeletedVisibilityTooltip", language);

            DocsPaWR.Tab tab = null;

            if (prj != null && !string.IsNullOrEmpty(prj.systemID))
            {
                tab = UIManager.ProjectManager.GetProjectTab(prj.systemID, UIManager.UserManager.GetInfoUser());

                if (tab != null && !tab.TransmissionsNumber.Equals("0"))
                {
                    this.LinkTransmissions.ToolTip = tab.TransmissionsNumber + " " + transmissions;
                    this.LiTransmissions.Attributes.Remove("class");
                    if (pageCaller.Equals("TRANSMISSIONS"))
                    {
                        this.LiTransmissions.Attributes.Add("class", "prjIAmProfileUnderline");
                    }
                    else
                    {
                        this.LiTransmissions.Attributes.Add("class", "prjOtherUnderline");
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



        private void InitializeKey()
        {
      
        }


        public void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LinkProfile.Text = Utils.Languages.GetLabelFromCode("LinkContent", language);
            this.LinkTransmissions.Text = Utils.Languages.GetLabelFromCode("LinkTransmissions", language);
            this.LinkStructure.Text = Utils.Languages.GetLabelFromCode("LinkStructure", language);
            this.LinkVisibility.Text = Utils.Languages.GetLabelFromCode("LinkVisibility", language);
            this.LinkEvents.Text = Utils.Languages.GetLabelFromCode("LinkEvents", language);
        }

        private void VisibiltyRoleFunctions()
        {

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_TAB_TRASMISSIONI"))
            {
                this.LiTransmissions.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_STRUCTURE"))
            {
                this.LiStructure.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PRO_VISIBILITA"))
            {
                this.LiVisibility.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_PROT_OG_STORIA"))
            {
                this.LiEvents.Visible = false;
            }
        }


        public void RefreshProjectabs()
        {
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
            {
                this.LinkTransmissions.Enabled = true;
                this.LinkStructure.Enabled = true;

                this.LinkEvents.Enabled = true;

                this.LiStructure.Attributes.Remove("class");
                this.LiTransmissions.Attributes.Remove("class");
                this.LiEvents.Attributes.Remove("class");

                this.LiStructure.Attributes.Add("class", "prjOther");
                this.LiTransmissions.Attributes.Add("class", "prjOther");

                this.LiEvents.Attributes.Add("class", "prjOther");

                if (fascicolo.tipo.Equals("G"))
                {
                    this.LinkVisibility.Enabled = false;
                }
                else
                {
                    this.LinkVisibility.Enabled = true;
                    this.LiVisibility.Attributes.Remove("class");
                    this.LiVisibility.Attributes.Add("class", "prjOther");
                }
            }

            this.UnderlineTab(this.PageCaller.ToUpper());

            this.UpProjectTabs.Update();
        }

        /// <summary>
        /// Rimuove i valori in sessione quando si seleziona un nuovo tab
        /// </summary>
        private void RemoveProperty()
        {
            HttpContext.Current.Session.Remove("searchCorrespondentIntExtWithDisabled");
        }

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
    }
}