using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.UserControls
{
    public partial class LinkDocFasc : System.Web.UI.UserControl
    {
        public delegate void HandleInternalLink(string idObj);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (hf_Reset.Value.Equals("1"))
                //{
                //    this.hf_Id.Value = string.Empty;
                //    this.txt_NomeObj.Text = string.Empty;
                //    hf_Reset.Value = "0";
                //}

                //if (hf_SelectedObject.Value.Equals("1"))
                //{
                //    if (this.IsFascicolo)
                //    {
                //        //Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoRicerca();
                //        //if (fasc != null)
                //        //{
                //        //    this.hf_Id.Value = fasc.systemID;
                //        //    this.txt_NomeObj.Text = fasc.descrizione;
                //        //    this.txt_Maschera.Text = fasc.codice + " " + CutValue(fasc.descrizione);
                //        //}
                //    }
                //    else
                //    {
                //        InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());
                //        if (infoDoc != null)
                //        {
                //            this.hf_Id.Value = infoDoc.idProfile;
                //            this.txt_NomeObj.Text = infoDoc.oggetto;
                //            if (!string.IsNullOrEmpty(infoDoc.segnatura))
                //            {
                //                this.txt_Maschera.Text = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                //            }
                //            else
                //            {
                //                this.txt_Maschera.Text = infoDoc.idProfile + " " + CutValue(infoDoc.oggetto);
                //            }
                //        }
                //    }
                //    this.hf_SelectedObject.Value = "0";
                //}
                this.hpl_Link.Text = this.LinkText;
      
                ////this.LinkDocFascBtn_Reset.OnClientClick = "_" + ClientID + "_reset();return false;";
                ////if (IsFascicolo)
                ////{
                ////    this.lbl_oggetto.Text = "Fascicolo: ";
                ////    this.LinkDocFascBtn_Cerca.OnClientClick = "_" + ClientID + "_apriRicercaFascicoli()";
                ////}
                ////else
                ////{
                ////    this.lbl_oggetto.Text = "Documento: ";
                ////    this.LinkDocFascBtn_Cerca.OnClientClick = "_" + ClientID + "_apriRicercaDocumenti()";
                ////}
                ////if (IsEsterno)
                ////{
                ////    this.hpl_Link.OnClientClick = "_" + ClientID + "_apriLinkEsterno()";
                ////}
                this.pnlLink_Link.Visible = HideLink;
                if (IsInsertModify)
                {
                    this.pnlLink_InsertModify.Visible = true;
                    if (IsAnteprima)
                    {
                        this.LinkDocFascBtn_Cerca.Enabled = false;
                        this.LinkDocFascBtn_Reset.Enabled = false;
                        this.hpl_Link.Enabled = false;
                    }
                    if (IsEsterno)
                    {
                        this.tr_interno.Visible = false;
                        this.tr_esterno.Visible = true;
                    }
                    else
                    {
                        this.tr_interno.Visible = true;
                        this.tr_esterno.Visible = false;
                    }
                }
                else
                {
                    this.pnlLink_InsertModify.Visible = false;
                }


                this.InitializePage();
                this.InitializeLanguage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        public bool IsInsertModify
        {
            get;
            set;
        }

        public bool HideLink
        {
            get;
            set;
        }

        public bool IsAnteprima
        {
            get;
            set;
        }

        public bool IsEsterno
        {
            get;
            set;
        }

        public bool IsFascicolo
        {
            get;
            set;
        }

        public string Value
        {
            get
            {
                if (IsEsterno)
                {
                    if (string.IsNullOrEmpty(txt_MascheraValue.Text) || string.IsNullOrEmpty(this.txt_Link.Text)) return string.Empty;
                    return this.txt_MascheraValue.Text + "||||" + this.txt_Link.Text;
                }
                else
                {
                    if (string.IsNullOrEmpty(txt_MascheraValue.Text) || string.IsNullOrEmpty(this.hf_IdValue.Value)) return string.Empty;
                    return this.txt_MascheraValue.Text + "||||" + this.hf_IdValue.Value;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] values = value.Split(new string[] { "||||" }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 2)
                    {
                        this.txt_MascheraValue.Text = values[0];
                        if (IsEsterno)
                        {
                            this.txt_Link.Text = values[1];
                        }
                        else
                        {
                            string id = values[1];
                            if (IsFascicolo)
                            {
                                Fascicolo fasc = ProjectManager.getFascicoloById(id);
                                this.hf_IdValue.Value = id;
                                if (fasc != null)
                                {
                                    this.txt_NomeObjValue.Text = fasc.descrizione;
                                }
                                else
                                {
                                    this.txt_NomeObjValue.Text = "Fascicolo non visibile";
                                }
                            }
                            else
                            {
                                InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(id, id, this.Page);
                                this.hf_IdValue.Value = id;
                                if (infoDoc != null)
                                {
                                    this.txt_NomeObjValue.Text = infoDoc.oggetto;
                                }
                                else
                                {
                                    this.txt_NomeObjValue.Text = "Documento non visibile";
                                }
                            }
                        }
                    }
                }
            }
        }

        private string LinkText
        {
            get
            {
                if (string.IsNullOrEmpty(this.txt_MascheraValue.Text))
                {
                    return "Link non inserito";
                }
                else
                {
                    return this.txt_MascheraValue.Text;
                }
            }
        }

        protected void hpl_Link_Click(Object sender, EventArgs e)
        {

            string idDocFasc = this.hf_IdValue.Value;
            if (IsEsterno)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redExternal", "window.open('" + this.txt_Link.Text + "','');", true);
            }
            else
            {
                if (!string.IsNullOrEmpty(idDocFasc))
                {
                    List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                    Navigation.NavigationObject actualPage = new Navigation.NavigationObject();

                    if (IsFascicolo)
                    {
                        Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(idDocFasc);
                        fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);

                         if(!string.IsNullOrEmpty(this.From) && this.From.Equals("D"))
                         {
                            actualPage.IdObject = UIManager.DocumentManager.getSelectedRecord().systemId;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT.ToString();
                            actualPage.Page = "DOCUMENT.ASPX";
                         }
                         else
                         {
                            actualPage.IdObject = UIManager.ProjectManager.getProjectInSession().systemID;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                            actualPage.Page = "PROJECT.ASPX";
                         }


                        UIManager.ProjectManager.setProjectInSession(fascicolo);
                        Response.Redirect("~/Project/project.aspx");
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(this.From) && this.From.Equals("D"))
                        {
                            actualPage.IdObject = UIManager.DocumentManager.getSelectedRecord().systemId;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT.ToString(), true,this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT.ToString();
                            actualPage.Page = "DOCUMENT.ASPX";
                           
                        }
                        else
                        {
                             actualPage.IdObject = UIManager.ProjectManager.getProjectInSession().systemID;
                            actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), string.Empty);
                            actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.PROJECT.ToString(), true, this.Page);
                            actualPage.CodePage = Navigation.NavigationUtils.NamePage.PROJECT.ToString();
                            actualPage.Page = "PROJECT.ASPX";
                        }

                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);

                        SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this.Page, idDocFasc, idDocFasc);
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        Response.Redirect("~/Document/Document.aspx");
                    }
                }
            }
        }

        private string CutValue(string value)
        {
            if (value.Length < 20) return value;
            int firstSpacePos = value.IndexOf(' ', 20);
            if (firstSpacePos == -1) firstSpacePos = 20;
            return value.Substring(0, firstSpacePos) + "...";
        }

        protected void InitializePage()
        {

        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.TxtLinkDocFasc.Text = Utils.Languages.GetLabelFromCode("TxtLinkDocFasc", language);
            this.TxtLinkDocFasc2.Text = Utils.Languages.GetLabelFromCode("TxtLinkDocFasc2", language);
            this.LinkDocFascBtn_Cerca.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Cerca", language);
            this.LinkDocFascBtn_Cerca.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Cerca", language);
            this.LinkDocFascBtn_Reset.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            this.LinkDocFascBtn_Reset.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            this.lbl_oggetto_link_Interno.Text = Utils.Languages.GetLabelFromCode("lbl_oggetto_link_Interno", language);
        }


        [Browsable(true)]
        public string TxtEtiLinkDocOrFasc
        {
            get
            {
                return this.TxtEtiLinkDocOrFascValue.Text;
            }
            set
            {
                this.TxtEtiLinkDocOrFascValue.Text = value;
            }
        }

        [Browsable(true)]
        public string txt_Maschera
        {
            get
            {
                return this.txt_MascheraValue.Text;
            }
            set
            {
                this.txt_MascheraValue.Text = value;
            }
        }


        [Browsable(true)]
        public string txt_NomeObj
        {
            get
            {
                return this.txt_NomeObjValue.Text;
            }
            set
            {
                this.txt_NomeObjValue.Text = value;
            }
        }

        [Browsable(true)]
        public string hf_Id
        {
            get
            {
                return this.hf_IdValue.Value;
            }
            set
            {
                this.hf_IdValue.Value = value;
            }
        }

        [Browsable(true)]
        public string From
        {
            get;
            set;
        }

        protected void LinkDocFascBtn_Cerca_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["LinkCustom.type"] = this.ID;

            if (this.IsFascicolo)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlTypeDocument", "ajaxModalPopupSearchProjectCustom();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlTypeDocument", "ajaxModalPopupOpenAddDocCustom();", true);
            }
        }

        protected void LinkDocFascBtn_Reset_Click(object sender, EventArgs e)
        {
            this.txt_NomeObjValue.Text = string.Empty;
            this.hf_SelectedObject.Value = string.Empty;
            this.UpPnlLinkCustom.Update();
        }

    }
}