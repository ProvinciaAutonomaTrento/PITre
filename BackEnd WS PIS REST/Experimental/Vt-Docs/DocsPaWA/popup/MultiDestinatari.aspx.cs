using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using DocsPAWA.SiteNavigation;
using System.Collections.Generic;

namespace DocsPAWA.popup
{
    public partial class MultiDestinatari : CssPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {

            Utils.startUp(this);

            if (!IsPostBack)
            {
                this.tipo = string.Empty;
                this.ListCorr = null;
                GetTheme();
                GetInitData();
                this.tipo = Request.QueryString["tipo"];
            }

        }

        protected void grvListaCorr_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < grvListaCorr.Items.Count; i++)
            {
                if (grvListaCorr.Items[i].ItemIndex >= 0)
                {
                    Label storicizzato = (Label)grvListaCorr.Items[i].Cells[4].Controls[1];
                    if (storicizzato.Text != null && storicizzato.Text != String.Empty && storicizzato.Text.ToUpper().Equals("SI"))
                    {
                        grvListaCorr.Items[i].ForeColor = Color.Red;
                        grvListaCorr.Items[i].Font.Bold = true;
                        grvListaCorr.Items[i].Font.Strikeout = true;
                    }
                }
            }
        }

        protected void BtnSaveCorr_Click(object sender, EventArgs e)
        {
            string idCorr = Request.Form["rbl_pref"];
            string page = Request.QueryString["page"];
            string uc_corr_id = Request.QueryString["corrId"];
            bool standard = false;
            DocsPaWR.Corrispondente[] listaDest;

            //se provengo da ricerca o da corrispondente
            if (page != null && (page.Equals("ricerca") || page.Equals("corrispondente")))
            {
                if (!string.IsNullOrEmpty(idCorr))
                {
                    DocsPaWR.Corrispondente tempCorr = UserManager.getCorrispondenteBySystemID(this.Page, idCorr);

                    if (tempCorr == null || String.IsNullOrEmpty(tempCorr.systemId))
                        tempCorr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, idCorr);

                    //inserisco in sessione il corrispondente selezionato nella maschera
                    if (!string.IsNullOrEmpty(uc_corr_id))
                    {
                        Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente> dic_corr = new System.Collections.Generic.Dictionary<string, DocsPaWR.Corrispondente>();
                        if (Session["dictionaryCorrispondente"] != null)
                            dic_corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];

                        dic_corr[uc_corr_id] = tempCorr;
                        Session["dictionaryCorrispondente"] = dic_corr;
                    }

                    //elimino dalla sessione l'elenco dei corrispondenti multipli
                    //Session.Remove("multiCorr");
                    UserManager.SetIdCorrispondenteMultiDestinatario(this.Page, tempCorr.systemId.ToString());
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "window.returnValue='" + tempCorr.descrizione.Replace("'","^^^") + "@-@" + tempCorr.codiceRubrica + "';window.close();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Attenzione! Selezionare un corrispondente');", true);
                }
                return;
            }

            if (!string.IsNullOrEmpty(idCorr))
            {
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato();
                DocsPaWR.Corrispondente tempCorr = UserManager.getCorrispondenteBySystemID(this.Page, idCorr);
                if ((tempCorr == null) || (tempCorr.systemId == null))
                {
                    tempCorr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, idCorr);
                }

                if (schedaDoc != null && schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.tipoProto))
                {
                    if (schedaDoc.tipoProto.Equals("A"))
                    {
                        if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti != null && ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti.Length > 0 && UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenti, tempCorr))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "alert('Attenzione! Corrispondente già presente nei mittenti multipli');", true);
                        }
                        else
                        {
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = tempCorr;
                            ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("P"))
                    {
                        
                        if (tipo.Equals("M"))
                        {
                            //DeMarcoA-Bug UffRef
                            Session["tempCorrMultiSelected_P"] = tempCorr;

                            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente = tempCorr;
                            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari != null && ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari.Length > 0 && UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "alert('Attenzione! Corrispondente già presente nei destinatari');", true);
                            }
                            else
                            {
                                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Length > 0 && UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');", true);
                                }
                                else
                                {
                                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("I"))
                    {
                        
                        if (tipo.Equals("M"))
                        {
                            //DeMarcoA-Bug UffRef
                            Session["tempCorrMultiSelected_I"] = tempCorr;

                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente = tempCorr;
                            ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari != null && ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari.Length > 0 && UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "alert('Attenzione! Corrispondente già presente nei destinatari');", true);
                            }
                            else
                            {
                                if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Length > 0 && UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');", true);
                                }
                                else
                                {
                                    listaDest = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari;
                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }
                }


                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "window.close();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Attenzione! Selezionare un corrispondente');", true);
            }

        }

        protected void GetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                if (UserManager.getInfoUtente() != null)
                    idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);

            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);

            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }

        }

        protected void GetInitData()
        {
            this.ListCorr = Session["multiCorr"] as DocsPaWR.ElementoRubrica[];
            this.grvListaCorr.DataSource = this.ListCorr;
            if (!string.IsNullOrEmpty(Request.QueryString["page"]) && Request.QueryString["page"].Equals("ricerca"))
            {
                foreach (DataGridColumn col in grvListaCorr.Columns)
                {
                    if (col.HeaderText.ToUpper().Equals("STORICIZZATO"))
                    {
                        col.Visible = true;
                        break;
                    }
                }
            }
            this.grvListaCorr.DataBind();
        }

        protected String GetCorrID(DocsPaWR.ElementoRubrica elem)
        {
            if (!string.IsNullOrEmpty(elem.systemId))
            {
                return elem.systemId;
            }
            else
            {
                return elem.codice;
            }

        }

        protected String GetCorrName(DocsPaWR.ElementoRubrica elem)
        {
            return elem.descrizione;
        }

        protected String GetCorrCodice(DocsPaWR.ElementoRubrica elem)
        {
            return elem.codice;
        }

        protected String GetCorrTipo(DocsPaWR.ElementoRubrica elem)
        {
            string result = string.Empty;
            string codRegTemp = elem.codiceRegistro;
            if (elem.isRubricaComune == true)
            {
                codRegTemp = "RC";
            }
            else
            {
                if (codRegTemp == null || codRegTemp.Equals(""))
                {
                    if (elem.interno == true)
                    {
                        codRegTemp = string.Empty;
                    }
                    else
                    {
                        codRegTemp = "TUTTI";
                    }
                }
                else
                {
                    codRegTemp = elem.codiceRegistro;
                }
            }

            return codRegTemp;
        }

        private DocsPaWR.ElementoRubrica[] ListCorr
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["listCorr"] as DocsPaWR.ElementoRubrica[];
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["listCorr"] = value;
            }
        }

        private string tipo
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["tipo"] as string;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["tipo"] = value;
            }
        }

        protected string GetStoricizzato(DocsPaWR.ElementoRubrica elem)
        {
            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this.Page, elem.systemId);
            if (corr != null && (!string.IsNullOrEmpty(corr.dta_fine)))
                return "SI";
            else
                return "NO";
        }
    }
}
