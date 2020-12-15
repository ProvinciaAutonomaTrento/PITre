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
    public partial class listaSpedizioni : DocsPAWA.CssPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                getDatiSpedizioni();
            }
        }
        protected DocsPaWR.SchedaDocumento schedaDocumento;
        protected ArrayList Dg_elem;


        private void getDatiSpedizioni()
        {
            //carica_Info
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);

            if (schedaDocumento == null || schedaDocumento.systemId.Equals(""))
            {
                this.lbl_message.Text = "Problemi nel reperimento delle spedizioni";
                this.lbl_message.Visible = true;
                return;
            }

            //prende tutti le spedizioni
            DocsPaWR.StatoInvio[] listaSpedizioni;
            listaSpedizioni = DocumentManager.getListaSpedizioni(this, schedaDocumento.systemId);
            Dg_elem = new ArrayList();
            if (listaSpedizioni != null && listaSpedizioni.Length > 0)
            {
                for (int i = 0; i < listaSpedizioni.Length; i++)
                {

                    BindGrid(listaSpedizioni[i]);

                }
                if (Dg_elem.Count > 0)
                {
                    this.DataGrid1.DataSource = Dg_elem;
                    this.DataGrid1.DataBind();
                }
            }

        }

        public void BindGrid(DocsPaWR.StatoInvio st)
        {
            //Costruisco il datagrid
            string data_spedizione;
            string cod_aoo;
            string cod_amm;
            string destinatario;
            string indirizzo;
            string dettaglio ="";
            data_spedizione = st.dataSpedizione;
            destinatario = st.destinatario;
            cod_aoo = st.codiceAOO;
            cod_amm = st.codiceAmm;
            indirizzo = st.indirizzo;
            if (!cod_amm.Equals(""))
                dettaglio = "COD AMM: " + cod_amm;
            if (!cod_aoo.Equals(""))
            {
                if (!dettaglio.Equals(""))
                    dettaglio = dettaglio + " - ";
                dettaglio = dettaglio + "COD AOO: " + cod_aoo;
            }
           
            Dg_elem.Add(new Cols(data_spedizione,destinatario, indirizzo, dettaglio, 0));

        }



        public class Cols
        {
            private string data;
            private string destinatario;
            private string indirizzo;
            private string dettaglio;
            private int chiave;

            public Cols(string data, string destinatario, string indirizzo, string dettaglio, int chiave)
            {
                this.data = data;
                this.destinatario = destinatario;
                this.indirizzo = indirizzo;
                this.dettaglio = dettaglio;
                this.chiave = chiave;
            }

            public string Data { get { return data; } }
            public string Destinatario { get { return destinatario; } }
            public string Indirizzo { get { return indirizzo; } }
            public string Dettaglio { get { return dettaglio; } }
            public int Chiave { get { return chiave; } }


        }

    }
 
    }

