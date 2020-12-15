using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class newParoleChiave : System.Web.UI.Page
    {
        protected DocsPAWA.DocsPaWR.Registro[] listaRF;
        string wnd;
        private System.Web.HttpContext ctx = System.Web.HttpContext.Current;

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_aggiungi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_Click);
          
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                wnd = Request.QueryString["wnd"];

                if (!Page.IsPostBack)
                {
                   
                    caricaRegistriDisponibili();
                    string dataUser = null;
                    DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                    
                    if (ctx.Session["userRuolo"] != null)
                    {
                        dataUser = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
                    }
                   
                    string idAmm = cr.idAmministrazione;
                    keyWord.ContextKey += "-"+dataUser + "-" + idAmm;
                    
                }
                disabilitaAddParoleChiave();
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "Ricerca/Inserimento in oggettario");
            }
        }

        /// <summary>
        /// Ritorna tutti i registri e gli rf associati a un ruolo, o solamente gli RF o solamente i registri
        /// in base al valore dello string all
        /// se all="" ritorna tutto (sia registri che rf)
        /// se all="0" ritorna solo i registri
        /// se all="1" ritorna solo gli RF
        /// </summary>
        /// <param name="all"></param>
        //private void caricaRegistriDisponibili(string all, string idAooColl)
        private void caricaRegistriDisponibili()
        {
            //il pannello è visibile solo se il ruolo vede almeno un RF

            DocsPaWR.Registro reg = null;

            //prendo tutti i registri

            DocsPaWR.Registro[] listaRegistri = UserManager.getListaRegistri(this);

            string labelCombo;
            if (listaRegistri != null && listaRegistri.Length > 0)
            {

                //importante. I value della combo dei registri sono formati da terne separate dal carattere "_":
                //nella prima posizione viene specificata la systemId del registro o dell'RF
                //nella seconda posizione viene specificato un valore popolato solo per gli RF:
                //  - "" per i registri
                //  - "0" per gli RF abilitati
                //  - "1" per gli RF non abilitati
                //nella terza posizione viene specificato l'IdAooCollegata (solo per gli RF)

                #region commento
                //for (int i = 0; i < listaRF.Length; i++)
                //{
                //    this.ddlRegRf.Items.Add((listaRF[i]).codRegistro);

                //    this.ddlRegRf.Items[i].Value = (listaRF[i]).systemId + "_" + listaRF[i].rfDisabled + "_" + listaRF[i].idAOOCollegata;         

                //}
                #endregion

                //this.pnlCombo.Visible = true;
                this.lblCombo.Visible = true;
                this.ddlRegRf.Visible = true;

                if (wnd == "proto" || wnd == "protoSempl") // caso di protocollo e protocollo semplificato
                {
                    if (wnd == "proto")
                        reg = UserManager.getRegistroSelezionato(this);
                    else
                    {
                        ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this);

                        reg = regMng.GetRegistroCorrente();
                    }


                    int l = -1;
                    for (int i = 0; i < listaRegistri.Length; i++)
                    {
                        if (listaRegistri[i].systemId == reg.systemId)//aggiungo il registro solo se coincide con quello di protocollo
                        {
                            l++;
                            this.ddlRegRf.Items.Add((listaRegistri[i]).codRegistro);

                            //this.ddlRegRf.Items[l].Value = (listaRegistri[i]).systemId + "_" + listaRegistri[i].rfDisabled + "_" + listaRegistri[i].idAOOCollegata;
                            this.ddlRegRf.Items[l].Value = "";
                            this.ddlRegRf.Items[l].Selected = true;

                            //prendo gli RF di ciascun registro
                            listaRF = UserManager.getListaRegistriWithRF(this, "1", (listaRegistri[i]).systemId);

                            if (listaRF != null && listaRF.Length > 0)
                            {
                                foreach (DocsPaWR.Registro currReg in listaRF)
                                {

                                    string strText = InserisciSpazi();
                                    this.ddlRegRf.Items.Add(strText + currReg.codRegistro);
                                    l++;
                                    //this.ddlRegRf.Items[l].Text = strText + currReg.codRegistro;
                                    this.ddlRegRf.Items[l].Value = currReg.systemId;
                                    //this.ddlRegRf.Items[l].Attributes.Add("style", " color:Gray");

                                }
                            }
                            else
                            {
                                //se non ci sono RF associati al registro di protocollo rendo invisibile il pannello
                                //this.pnlCombo.Visible = false;
                                this.lblCombo.Visible = false;
                                this.ddlRegRf.Visible = false;
                            }
                        }
                    }

                }
                else // caso di documenti grigi e ricerche
                {
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
                    
                    int l = -1;
                    int i = 0;
                    for (i = 0; i < listaRegistri.Length; i++)
                    {
                        l++;
                        //carico nella combo tutti i registri che il ruolo può vedere
                        if (!ddlRegRf.Items.Contains(ddlRegRf.Items.FindByValue((listaRegistri[i]).systemId)))
                        {
                            this.ddlRegRf.Items.Add((listaRegistri[i]).codRegistro);
                            this.ddlRegRf.Items[i].Value = (listaRegistri[i]).systemId;
                        }

                        ////prendo gli RF di ciascun registro
                        listaRF = UserManager.getListaRegistriWithRF(this, "1", (listaRegistri[i]).systemId);

                        if (listaRF != null && listaRF.Length > 0)
                        {
                            foreach (DocsPaWR.Registro currReg in listaRF)
                            {
                                string strText = InserisciSpazi();
                                if (!ddlRegRf.Items.Contains(ddlRegRf.Items.FindByValue(currReg.systemId)))
                                {
                                    this.ddlRegRf.Items.Add(strText + currReg.codRegistro);
                                    l++;
                                    this.ddlRegRf.Items[l].Value = currReg.systemId;
                                    //this.ddlRegRf.Items[l].Attributes.Add("style", " color:Gray");
                                }
                            }
                        }
                    }

                    ListItem item = new ListItem();
                    item.Text = "TUTTI";
                    item.Value = "";
                    if (!ddlRegRf.Items.Contains(ddlRegRf.Items.FindByText("TUTTI")))
                        this.ddlRegRf.Items.Add(item);

                    if (schedaDocumento != null)
                    {
                        if (schedaDocumento.registro == null)
                        {
                            if (ctx.Session["userRegistro"] != null)
                            {
                                ListItem founded =
                                    this.ddlRegRf.Items.FindByValue(((Registro) ctx.Session["userRegistro"]).systemId);
                                if (founded != null)
                                {
                                    founded.Selected = true;
                                    keyWord.ContextKey = founded.Value;
                                }
                            }
                            else
                            {
                                ListItem founded =
                                    this.ddlRegRf.Items.FindByValue(listaRegistri[0].systemId);
                                if (founded != null)
                                {
                                    founded.Selected = true;
                                    keyWord.ContextKey = founded.Value;
                                }
                            }
                        }
                        else
                        {
                            ListItem founded =
                                this.ddlRegRf.Items.FindByValue(((Registro) ctx.Session["userRegistro"]).systemId);
                            if (founded != null)
                            {
                                founded.Selected = true;
                                keyWord.ContextKey = founded.Value;

                                ////prendo gli RF di ciascun registro
                                listaRF = UserManager.getListaRegistriWithRF(this, "1", founded.Value);

                                string[] rf = new string[listaRF.Length];
                                int j = 0;
                                foreach (Registro regis in listaRF)
                                {
                                    rf[j] = regis.systemId;
                                    j++;
                                }

                                string[] index = new string[ddlRegRf.Items.Count];
                                j = 0;
                                for (int z = 0; z < ddlRegRf.Items.Count; z++)
                                {
                                    if (!string.IsNullOrEmpty(ddlRegRf.Items[z].Value) &&
                                        ddlRegRf.Items[z].Value != founded.Value &&
                                        !rf.Contains(ddlRegRf.Items[z].Value))
                                    {
                                        index[j] = ddlRegRf.Items[z].Value;
                                        j++;
                                    }

                                }

                                foreach (var val in index)
                                {
                                    ddlRegRf.Items.Remove(ddlRegRf.Items.FindByValue(val));
                                }
                            }

                        }
                    }
                    else
                    {
                        if (ctx.Session["userRegistro"] != null)
                        {
                            ListItem founded =
                                this.ddlRegRf.Items.FindByValue(((Registro)ctx.Session["userRegistro"]).systemId);
                            if (founded != null)
                            {
                                founded.Selected = true;
                                keyWord.ContextKey = founded.Value;
                            }
                        }
                        else
                        {
                            ListItem founded =
                                this.ddlRegRf.Items.FindByValue(listaRegistri[0].systemId);
                            if (founded != null)
                            {
                                founded.Selected = true;
                                keyWord.ContextKey = founded.Value;
                            }
                        }
                    }

                }

            }
            else
            {
                throw new Exception();
            }

        }

        private string InserisciSpazi()
        {
            string strText = "";
            for (short iff = 0; iff < 3; iff++)
            {
                strText += " ";
            }
            return strText;
        }

        protected void ddlRegRf_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dataUser = null;
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                    
            if (ctx.Session["userRuolo"] != null)
            {
                dataUser = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
            }
            
            string idAmm = cr.idAmministrazione;
            this.keyWord.ContextKey = this.ddlRegRf.SelectedValue + "-" + dataUser + "-" + idAmm;
        }

        protected void txtKeyWord_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hd_idKeyWord.Value))
            {
                ListItem item = new ListItem(txtKeyWord.Text, hd_idKeyWord.Value);
                if (!ListParoleChiave.Items.Contains(item))
                    ListParoleChiave.Items.Add(item);
                txtKeyWord.Text = "";
                hd_idKeyWord.Value = "";
            }
        }

        protected void btn_ok_Click(object sender, EventArgs e)
        {
            
            DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave = new DocsPAWA.DocsPaWR.DocumentoParolaChiave[0]; ;

            for (int i = 0; i < this.ListParoleChiave.Items.Count; i++)
            {
                
                DocsPaWR.DocumentoParolaChiave docParoleChiave = new DocsPAWA.DocsPaWR.DocumentoParolaChiave();
                docParoleChiave.systemId = this.ListParoleChiave.Items[i].Value;
                docParoleChiave.descrizione = this.ListParoleChiave.Items[i].Text;
                docParoleChiave.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
                docParoleChiave.idRegistro = ddlRegRf.SelectedValue;
                listaDocParoleChiave = Utils.addToArrayParoleChiave(listaDocParoleChiave, docParoleChiave);
                
            }

            if (wnd.Equals("docProf"))
            {

                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
                if (schedaDocumento != null)
                {
                    //					schedaDocumento.paroleChiave = listaDocParoleChiave;
                    schedaDocumento.paroleChiave = addParoleChiaveToDoc(schedaDocumento, listaDocParoleChiave);
                    schedaDocumento.daAggiornareParoleChiave = true;
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                }

                DocumentManager.setListaParoleChiaveSel(this, listaDocParoleChiave);

                //	Response.Write("<script>var k=window.open('../documento/docProfilo.aspx','IframeTabs'); window.close();</script>");		
                Response.Write("<script>window.opener.document.forms[0].submit(); window.close();</script>");

            }
            else
            {
                DocumentManager.setListaParoleChiaveSel(this, listaDocParoleChiave);

                if (wnd.Equals("RicC"))
                    Response.Write("<script>window.opener.f_Ricerca_C.submit(); window.close();</script>");
                else
                    if (wnd.Equals("RicG"))
                        Response.Write("<script>window.opener.ricDocGrigia.submit(); window.close();</script>");

            }
        }

        private void disabilitaAddParoleChiave()
        {
            if (UserManager.ruoloIsAutorized(this, this.btn_aggiungi.Tipologia.ToString()))
                this.btn_aggiungi.Visible = true;
            else
                this.btn_aggiungi.Visible = false;
        }

        private bool listaContains(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, DocsPAWA.DocsPaWR.DocumentoParolaChiave el)
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

        private DocsPAWA.DocsPaWR.DocumentoParolaChiave[] addParoleChiaveToDoc(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, DocsPAWA.DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave)
        {
            DocsPaWR.DocumentoParolaChiave[] listaPC;
            listaPC = schedaDocumento.paroleChiave;
            if (listaDocParoleChiave != null)
            {
                for (int i = 0; i < listaDocParoleChiave.Length; i++)
                {
                    if (!listaContains(schedaDocumento.paroleChiave, listaDocParoleChiave[i]))
                        listaPC = Utils.addToArrayParoleChiave(listaPC, listaDocParoleChiave[i]);
                }
            }
            return listaPC;

        }

       
        private void btn_aggiungi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if(string.IsNullOrEmpty(hd_idKeyWord.Value))
            {
                ////ToDo Insert della parola chiave
                DocsPaWR.DocumentoParolaChiave parolaC = new DocsPAWA.DocsPaWR.DocumentoParolaChiave();
                parolaC.descrizione = this.txtKeyWord.Text.ToUpper();
                parolaC.idRegistro = ddlRegRf.SelectedValue;

                parolaC = DocumentManager.addParolaChiave(this, parolaC);

                if (parolaC != null)
                {
                    ListItem item = new ListItem(parolaC.descrizione, parolaC.systemId);
                    if (!ListParoleChiave.Items.Contains(item))
                        ListParoleChiave.Items.Add(item);
                    txtKeyWord.Text = "";
                    hd_idKeyWord.Value = "";
                    Response.Write("<script>alert('Operazione effettuata con successo');</script>");
                    this.txtKeyWord.Text = "";
                }
                else /* modifica per gestione dato presente */
                {
                    Response.Write("<script>alert('Attenzione.Parola chiave già presente');</script>");
                }
                //Riempo la ListBox con la parola nuova
                //Response.Write("<script>alert('"+hd_newKey.Value+"')</script>");
                //ListItem item = new ListItem(txtKeyWord.Text, hd_idKeyWord.Value);
                //if (!ListParoleChiave.Items.Contains(item))
                //    ListParoleChiave.Items.Add(item);
                //txtKeyWord.Text = "";
            }

        }

        protected void btn_rimuovi_Click(object sender, ImageClickEventArgs e)
        {
            string[] index = new string[ListParoleChiave.Items.Count];
            int j = 0;
            for(int i =0;i<ListParoleChiave.Items.Count;i++)
            {
                if(ListParoleChiave.Items[i].Selected)
                {
                    index[j] = ListParoleChiave.Items[i].Value;
                    j++;
                }

            }

            foreach (var i in index)
            {
                ListParoleChiave.Items.Remove(ListParoleChiave.Items.FindByValue(i));
            }

        }
    }
}