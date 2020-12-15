using System; 
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;

namespace DocsPAWA.fascicolo
{

	/// <summary>
	/// Summary description for p_File.
	/// </summary>
    public class tabPulsantiDoc : DocsPAWA.CssPage
    {
        protected DocsPaWebCtrlLibrary.ImageButton btn_visualizzaDoc;
        protected DocsPaWebCtrlLibrary.ImageButton btnFilterDocs;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowAllDocs;
        protected DocsPaWebCtrlLibrary.ImageButton btn_inserisciDoc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_importaDoc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_esportaDoc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtFilterDocumentsRetValue;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
       

        
        protected DocsPaWebCtrlLibrary.IFrameWebControl iFrameDoc;
      

        private void Page_Load(object sender, System.EventArgs e)
        {
            string newUrl = "";
           
            if (!IsPostBack)
            
            {
                // l'import dei documenti massivi è attivo solo se selzionato un fascicolo di tipo P
                if (! string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["IMPORT_MASSIVO"])  && System.Configuration.ConfigurationManager.AppSettings["IMPORT_MASSIVO"] == "1")
                {
                   
                        if (UserManager.ruoloIsAutorized(this, "IMP_DOC_MASSIVA"))
                            btn_importaDoc.Enabled= true;
                                
                        else
                            btn_importaDoc.Enabled = false;
                }
                else
                {
                    btn_importaDoc.Enabled = false;
                }
                if (UserManager.ruoloIsAutorized(this, "EXP_DOC_MASSIVA"))
                    btn_esportaDoc.Enabled = true;
                else
                    btn_esportaDoc.Enabled = false;
               
                
                this.btnFilterDocs.Attributes.Add("onClick", "ShowDialogSearchDocuments();");
                this.btnShowAllDocs.Attributes.Add("onClick", "ShowWaitCursor();");

                ViewState["idFolder"] = "";
                if (Request.QueryString["idFolder"] != null)
                {
                    ViewState["idFolder"] = Request.QueryString["idFolder"].ToString();
                }
            }

            if (Session["ListaDocs-CampiProf"] != null)
            {

                if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.DocumentInProject)
                {
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.DocumentInProject);
                }

                // Creazione oggetti filtro
                DocsPaWR.FiltroRicerca[][] qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                FiltroRicerca[] fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
               
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocumentInProject();

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null){
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                    qV[0] = fVList;

                    DocsPAWA.ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.RemoveCurrentFilter();
                    DocsPAWA.ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.SetCurrentFilter(qV);
                }
                //SETTA ORDINAMENTO

                AbilitaDisabilitaPulsanti();
                if (Session["ListaDocs-CampiProf"].ToString() == "ListaDocs" )
                {
                    newUrl = "NewDocListInProject.aspx";
                   

                   
                        newUrl += "?FilterDocumentsRetValue=" + (this.txtFilterDocumentsRetValue.Value!="").ToString();


                    if(ViewState["idFolder"]!=null)
                        newUrl += "&idFolder=" + ViewState["idFolder"].ToString();
                        
                }
                else
                {
                    string tipo = "";
                    string codTipologiaFasc = "";
                    string editMode = "";

                    if (Request.QueryString["tipoFascicolo"] != null)
                    {
                        tipo = Request.QueryString["tipoFascicolo"].ToString();
                    }

                    if (Request.QueryString["codTipologiaFasc"] != null)
                    {
                        codTipologiaFasc = Request.QueryString["codTipologiaFasc"].ToString();
                    }

                    if (Request.QueryString["editMode"] != null)
                    {
                        editMode = Request.QueryString["editMode"].ToString();
                    }
                    Session["ListaDocs-CampiProf"] = "CampiProf";
                     newUrl = "tabFascCampiProf.aspx?tipoFascicolo=" + tipo + "&codTipologiaFasc=" + codTipologiaFasc + "&editMode=" + editMode;
                }
                
            }


            
            
           this.iFrameDoc.NavigateTo = newUrl;           
        }

        private void tabPulsantiDoc_PreRender(object sender, System.EventArgs e)
        {
            this.verificaHMdiritti();
        }
        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (FascicoliManager.getFascicoloSelezionato() != null)
            {
                DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato();
                if (Fasc != null && (Fasc.accessRights != null && Fasc.accessRights != ""))
                {
                    //if( UserManager.disabilitaButtHMDiritti(Fasc.accessRights) || (Fasc.inArchivio!= null && Fasc.inArchivio=="1") )
                    if (UserManager.disabilitaButtHMDiritti(Fasc.accessRights))
                    {
                        this.btn_inserisciDoc.Enabled = false;
                    }
                }
            }
        }
        private void AbilitaDisabilitaPulsanti()
        {
            if ( Session["ListaDocs-CampiProf"].ToString() == "ListaDocs")
            {
                bool filtroattivo  = (ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter() != null); 

                btn_inserisciDoc.Enabled = true;
                btn_visualizzaDoc.Enabled = false;
                btn_importaDoc.Enabled = true;
                btn_esportaDoc.Enabled = true;
                btnFilterDocs.Enabled = true;
                btnShowAllDocs.Enabled = filtroattivo;
               
            }
            else
            {


                btn_inserisciDoc.Enabled = true;
                btn_visualizzaDoc.Enabled = true;
                btnFilterDocs.Enabled = false;
                btnShowAllDocs.Enabled = false;
                btn_importaDoc.Enabled = true;
                btn_esportaDoc.Enabled = true;
            }

            if (FascicoliManager.getFascicoloSelezionato() != null)
            {
                if (FascicoliManager.getFascicoloSelezionato().stato.Equals("C"))
                {
                    btn_inserisciDoc.Enabled = false;
                    btn_importaDoc.Enabled = false;
                    btn_esportaDoc.Enabled = false;
                }
             }
        
        }

        protected void btn_visualizzaDoc_Click(object sender, ImageClickEventArgs e)
        {
            AbilitaDisabilitaPulsanti();
            VisualizzalistaDocs();
        }

        private void VisualizzalistaDocs()
        {
            if (Session["IdFolderselezionato"] != null)
            {
                Session["ListaDocs-CampiProf"] = "ListaDocs";
                string newUrl = "NewDocListInProject.aspx?idFolder=" + Session["IdFolderselezionato"].ToString();
                newUrl += "&FilterDocumentsRetValue=" + (this.txtFilterDocumentsRetValue.Value != "").ToString();
                this.iFrameDoc.NavigateTo = newUrl;



            }
        }

        protected void btnFilterDocs_Click(object sender, ImageClickEventArgs e)
        {
           
            AbilitaDisabilitaPulsanti();
            if (Session["IdFolderselezionato"] != null)
            {
                // Session["ListaDocs-CampiProf"] = "ListaDocs";
                string newUrl = "NewDocListInProject.aspx?idFolder=" + Session["IdFolderselezionato"].ToString();
                newUrl += "&FilterDocumentsRetValue=true" ;
                this.iFrameDoc.NavigateTo = newUrl;

                //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");

            }
        }

        protected void btnShowAllDocs_Click(object sender, ImageClickEventArgs e)
        {
          
            AbilitaDisabilitaPulsanti();
            if (Session["IdFolderselezionato"] != null)
            {


                this.txtFilterDocumentsRetValue.Value = "";
                // Session["ListaDocs-CampiProf"] = "ListaDocs";
                string newUrl = "NewDocListInProject.aspx?idFolder=" + Session["IdFolderselezionato"].ToString();
                newUrl += "&FilterDocumentsRetValue=False" ;
                ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();
                this.iFrameDoc.NavigateTo = newUrl;

                Session.Remove("templateRicerca");
                //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");

            }
        }
        protected void btn_esportaDoc_Click(object sender, ImageClickEventArgs e)
        {
            string idFolder = "";
            string idFascicolo = "";
            if (Session["fascDocumenti.FolderSel"] != null)
            {
                DocsPaWR.Folder folder = (DocsPaWR.Folder)Session["fascDocumenti.FolderSel"];
                idFolder = folder.systemID;
                DocsPAWA.DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato(this);
                // Ricavo il system id del fascicolo correntemente aperto
                //string idFascicolo = ViewState["idFolder"].ToString();
                // Richiamo la funzione javascript per l'apertura del popup modale per
                // l'esportazione del fascicolo
                RegisterStartupScript("openModale", "<script>OpenPopUpExportFasc('" + Fasc.systemID + "'" + ")</script>");
            }
        }
        protected void btn_importaDoc_Click(object sender, ImageClickEventArgs e)
        {
            // Ricavo il codice del fascicolo correntemente aperto
           // string codFascicolo = Request.QueryString["codFasc"].ToString();
            // Richiamo la funzione javascript per l'apertura del popup modale per
            // l'esportazione del fascicolo
            string idFolder = "";
            string idFascicolo = "";
            if(Session["fascDocumenti.FolderSel"] != null)
            {
                DocsPaWR.Folder folder = (DocsPaWR.Folder)Session["fascDocumenti.FolderSel"];
                idFolder = folder.systemID;
                DocsPAWA.DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato(this);
                //RegisterStartupScript("openModale", "<script>ApriPopUpImportDoc('" + Fasc.codice + "'" + ")</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "openModale",
                    String.Format("ApriPopUpImportDoc('{0}');", Fasc.idTitolario), true);
            }
        }
        protected void btn_inserisciDoc_Click(object sender, ImageClickEventArgs e)
        {
            bool Aclrevocata = bool.Parse(Request.QueryString["AclRevocata"].ToString());
            if (!Aclrevocata)
            {

                //DocsPaWR.Folder selectedFolder=getSelectedFolder();
                bool rootFolder = false;
                // DocsPaWR.Folder selectedFolder = getSelectedFolder(out rootFolder);
                string idFolder = "";
                string scriptName = "";
                string script = "";

                DocsPAWA.DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato(this);
                string tipoDoc = "tipoDoc=T";
                string action = "action=addDocToFolder";
                string parameter = "";

                if (Session["fascDocumenti.FolderSel"] != null)
                {
                    DocsPaWR.Folder folder = (DocsPaWR.Folder)Session["fascDocumenti.FolderSel"];
                    idFolder = folder.systemID;
                    parameter = "folderId=" + idFolder;

                    /* massimo digregorio 
                    descrizione: visualizzazione dei DOCUMENTI in ADL filtratri per Registro del FASCICOLO. 
                    new:*/
                    string paramIdReg = String.Empty;
                    if (Fasc.idRegistroNodoTit != null && !Fasc.idRegistroNodoTit.Equals(String.Empty))
                        paramIdReg = "&idReg=" + Fasc.idRegistroNodoTit;

                    string queryString = tipoDoc + "&" + action + "&" + parameter + paramIdReg;


                    script = "<script>ApriFinestraRicercaDocPerClassifica('" + queryString + "');</script>";
                    //script="<script>ApriFinestraADL('../popup/areaDiLavoro.aspx?"+queryString+"');</script>";
                    // scriptName="addFromADL";
                    scriptName = "addRicPerClass";
                }
                else
                {
                    if (Fasc != null)
                    {
                        DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();

                        info = UserManager.getInfoUtente(this.Page);

                        DocsPAWA.DocsPaWR.Folder tempFolder = FascicoliManager.getFolderByIdFasc(info, Fasc);

                        parameter = "folderId=" + tempFolder.systemID;

                        string paramIdReg = String.Empty;
                        if (Fasc.idRegistroNodoTit != null && !Fasc.idRegistroNodoTit.Equals(String.Empty))
                            paramIdReg = "&idReg=" + Fasc.idRegistroNodoTit;

                        string queryString = tipoDoc + "&" + action + "&" + parameter + paramIdReg;

                        script = "<script>ApriFinestraRicercaDocPerClassifica('" + queryString + "');</script>";
                        //script="<script>ApriFinestraADL('../popup/areaDiLavoro.aspx?"+queryString+"');</script>";
                        //scriptName="addFromADL";
                        scriptName = "addRicPerClass";
                    }
                    else
                    {
                        script = "<script>alert('Selezionare un Folder');</script>";
                        scriptName = "SelectFolderAlert";
                    }
                }

                this.RegisterStartupScript(scriptName, script);

                VisualizzalistaDocs();
            }
        }

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
            this.btn_visualizzaDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_visualizzaDoc_Click);
            this.btnFilterDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnFilterDocs_Click);
            this.btnShowAllDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowAllDocs_Click);
            this.btn_inserisciDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_inserisciDoc_Click);
            this.btn_importaDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_importaDoc_Click);
            this.btn_esportaDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_esportaDoc_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender+=new EventHandler(tabPulsantiDoc_PreRender);
         


        }

        #endregion
    }
}
