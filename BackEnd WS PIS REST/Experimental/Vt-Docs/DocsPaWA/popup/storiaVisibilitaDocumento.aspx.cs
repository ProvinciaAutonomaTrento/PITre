using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
    public partial class storiaVisibilitaDocumento : DocsPAWA.CssPage
    {
        protected ArrayList Dt_elem;
        protected DocsPAWA.DocsPaWR.SchedaDocumento SchedaDoc;
        protected DocsPAWA.DocsPaWR.Fascicolo Fasc;
        protected DocsPAWA.DocsPaWR.StoriaDirittoDocumento[] ListaStoria;

        protected System.Web.UI.WebControls.Label LblDettagli;
        protected System.Web.UI.WebControls.Label LblTitolo;
        protected System.Web.UI.WebControls.DataGrid DGStoria;
        protected System.Web.UI.WebControls.Button Btn_ok;

        protected void Page_Load(object sender, EventArgs e)
        {
            string tipoObj = Request.QueryString["tipo"];
            BindGrid(tipoObj);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_ok.Click += new System.EventHandler(this.Btn_ok_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        private void Btn_ok_Click(object sender, System.EventArgs e)
        { 
            Response.Write("<script>window.close();</script>");
        }

        public void BindGrid(string tipoObj)
        {
            if (tipoObj.Equals("D"))
            {
                SchedaDoc = (DocsPAWA.DocsPaWR.SchedaDocumento)DocumentManager.getDocumentoSelezionato(this);
                if (SchedaDoc == null)
                {
                    this.LblDettagli.Text = "Errore nel reperimento dei dati del documento";
                    this.LblDettagli.Visible = true;
                    return;
                }
                ListaStoria = DocumentManager.getStoriaVisibilita(this, SchedaDoc.systemId, tipoObj, UserManager.getInfoUtente(this));
            }
            else
            {
                Fasc = (DocsPAWA.DocsPaWR.Fascicolo)FascicoliManager.getFascicoloSelezionato(this);
                if (Fasc == null)
                {
                    this.LblDettagli.Text = "Errore nel reperimento dei dati del fascicolo";
                    this.LblDettagli.Visible = true;
                    return;
                }
                ListaStoria = DocumentManager.getStoriaVisibilita(this, Fasc.systemID, tipoObj, UserManager.getInfoUtente(this));
            
            }

            if (ListaStoria == null || ListaStoria.Length==0)
            {
                this.LblDettagli.Text = "Nessun valore trovato";
                this.LblDettagli.Visible = true;
                this.DGStoria.Visible=false;
		        return;
            }
            else
            { 
                Dt_elem = new ArrayList();
				for(int i=0; i<ListaStoria.Length; i++)
                    Dt_elem.Add(new Cols(ListaStoria[i].utente, ListaStoria[i].ruolo, ListaStoria[i].data.Substring(0,10), ListaStoria[i].codOperazione, ListaStoria[i].descrizione));

				if (ListaStoria.Length > 0)
				{	
				    //DocumentManager.setDataGridAllegati(this,Dt_elem);					
					this.DGStoria.DataSource=Dt_elem;
					this.DGStoria.DataBind();
                    this.LblDettagli.Visible = false;
				}
				else
				{
                    this.LblDettagli.Visible = true;
				}
                this.DGStoria.Visible=true;
			}
		}

		public class Cols 
		{		
			private string utente;
			private string ruolo;
			private string data;
			private string codOperazione;
            private string descrizione;
           
			public Cols(string utente, string ruolo, string data, string codOperazione, string descrizione /*, int chiave*/)
			{
                this.utente = utente;
                this.ruolo = ruolo;
                this.data = data;
                this.codOperazione = codOperazione;
				this.descrizione = descrizione;
            }
					
			public string Utente{get{return utente;}}
			public string Ruolo{get{return ruolo;}}
			public string Data{get{return data;}}
			public string CodOperazione{get{return codOperazione;}}
            public string Descrizione{get{return descrizione;}}
          
    	}

            
    }

}

   
		
