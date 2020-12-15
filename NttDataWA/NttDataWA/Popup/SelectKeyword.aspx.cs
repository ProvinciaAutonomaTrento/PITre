using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class SelectKeyword : System.Web.UI.Page
    {

        private string keyWordAdd
        {
            get
            {
                if (HttpContext.Current.Session["keyWordAdd"] != null)
                {
                    return HttpContext.Current.Session["keyWordAdd"] as String;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["keyWordAdd"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                    //Session.Abandon(); //DA CANCELLARE!
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            this.setListaParoleChiave();
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SelectKeywordBtnOk.Text = Utils.Languages.GetLabelFromCode("SelectKeywordBtnOk", language);
            this.SelectKeywordBtnAdd.Text = Utils.Languages.GetLabelFromCode("SelectKeywordBtnAdd", language);
            this.SelectKeywordBtnChiudi.Text = Utils.Languages.GetLabelFromCode("SelectKeywordBtnChiudi", language);
            this.SelectKeywordLbl.Text = Utils.Languages.GetLabelFromCode("SelectKeywordLbl", language);
            this.AddKeyword.Title = Utils.Languages.GetLabelFromCode("AddKeywordLblTitle", language);
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.AddKeyword.ReturnValue))
            {
                this.setListaParoleChiave();
                this.UpdPnlListKeywords.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddKeyword','')", true);
            }
        }

        private void setListaParoleChiave()
        {
            DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiave(this);
            this.ListParoleChiave.Items.Clear();
            if (listaParoleChiave.Length > 0)
            {
                for (int i = 0; i < listaParoleChiave.Length; i++)
                {
                    this.ListParoleChiave.Items.Add(((DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).descrizione);
                    this.ListParoleChiave.Items[i].Value = ((DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).systemId;
                    if (!string.IsNullOrEmpty(keyWordAdd))
                    {
                        if (this.ListParoleChiave.Items[i].Value.Equals(keyWordAdd))
                        {
                            this.ListParoleChiave.Items[i].Selected = true;
                            HttpContext.Current.Session.Remove("keyWordAdd");
                        }
                    }
                }
            }
        }




        protected void SelectKeywordBtnOk_Click(object sender, EventArgs e)
        {           
            try {
                DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave = new DocsPaWR.DocumentoParolaChiave[0]; ;

                for (int i = 0; i < this.ListParoleChiave.Items.Count; i++)
                {
                    if (this.ListParoleChiave.Items[i].Selected)
                    {
                        DocsPaWR.DocumentoParolaChiave docParoleChiave = new DocsPaWR.DocumentoParolaChiave();
                        docParoleChiave.systemId = this.ListParoleChiave.Items[i].Value;
                        docParoleChiave.descrizione = this.ListParoleChiave.Items[i].Text;
                        docParoleChiave.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                        listaDocParoleChiave = utils.addToArrayParoleChiave(listaDocParoleChiave, docParoleChiave);
                    }
                }

                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();
                if (schedaDocumento != null)
                {
                    schedaDocumento.paroleChiave = addParoleChiaveToDoc(schedaDocumento, listaDocParoleChiave);
                    schedaDocumento.daAggiornareParoleChiave = true;
                }

                Session["ReturnValuePopup"] = listaDocParoleChiave;

                DocumentManager.setSelectedRecord(schedaDocumento);                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectKeyword", "parent.closeAjaxModal('SelectKeyword', 'up', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private DocsPaWR.DocumentoParolaChiave[] addParoleChiaveToDoc(DocsPaWR.SchedaDocumento schedaDocumento, DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave)
        {
            DocsPaWR.DocumentoParolaChiave[] listaPC;
            listaPC = schedaDocumento.paroleChiave;
            if (listaDocParoleChiave != null)
            {
                for (int i = 0; i < listaDocParoleChiave.Length; i++)
                {
                    if (!listaContains(schedaDocumento.paroleChiave, listaDocParoleChiave[i]))
                        listaPC = utils.addToArrayParoleChiave(listaPC, listaDocParoleChiave[i]);
                }
            }
            return listaPC;
        }

        private bool listaContains(DocsPaWR.DocumentoParolaChiave[] lista, DocsPaWR.DocumentoParolaChiave el)
        {
            bool trovato = false;
            if (lista != null)
            {
                for (int i = 0; i < lista.Length; i++)
                {
                    if (lista[i].systemId.Equals(el.systemId))
                    {
                        trovato = true;
                        break;
                    }
                }
            }
            return trovato;
        }

        protected void SelectKeywordBtnAdd_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddKeyword", "ajaxModalPopupAddKeyword();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SelectKeywordBtnChiudi_Click(object sender, EventArgs e)
        {           
            try {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectKeyword", "parent.closeAjaxModal('SelectKeyword', '', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}