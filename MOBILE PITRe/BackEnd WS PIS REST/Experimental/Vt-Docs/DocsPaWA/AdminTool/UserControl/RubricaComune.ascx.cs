using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RubricaComune.Proxy.Elementi;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.utils;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class RubricaComune : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private String _idElemento = String.Empty;
        public String IdElemento
        {
            get
            {
                return this._idElemento;
            }

            set
            {
                this._idElemento = value;

                // Visualizzazione delle informazioni sull'eventuale ultimo invio dell'RF
                // a Rubrica Comune
                if(this.GestioneRubricaComuneAbilitata)
                    ShowData(this._idElemento);

            }
        }

        /// <summary>
        /// Metodo per la pulizia dei dati visualizzati
        /// </summary>
        private void ClearData()
        {
            this.lblCodiceRC.Text = String.Empty;
            this.lblDataCreazioneRC.Text = String.Empty;
            this.lblDataUltimaModificaRC.Text = String.Empty;
            this.lblDescrizioneRC.Text = String.Empty;
        }

        /// <summary>
        /// Metodo per il caricamento e la visualizzazione delle informazioni di aggiornamento
        /// relative ad un RF
        /// </summary>
        /// <param name="idRf">Id dell'RF</param>
        private void ShowData(String idRf)
        {
            SessionManager sessionMng = new SessionManager();

            ElementoRubrica elemento = DocsPAWA.RubricaComune.RubricaServices.GetElementoRubricaRF(sessionMng.getUserAmmSession(), idRf);

            this.ClearData();

            if (elemento != null)
            {
                const string format = "dd/MM/yyyy HH:mm:ss";

                // L'uo è già presente in rubrica comune come elemento di interesse generale;
                // viene mostrata la data di ultima modifica e la data di creazione
                this.lblCodiceRC.Text = elemento.Codice;
                this.lblDescrizioneRC.Text = elemento.Descrizione;
                this.lblDataCreazioneRC.Text = elemento.DataCreazione.ToString(format);
                this.lblDataUltimaModificaRC.Text = elemento.DataUltimaModifica.ToString(format);
            }
        }

        public Tipo Tipo { get; set; }

        /// <summary>
        /// Descrizione del corrispondente da inviare a rubrica comune (corrispondente locale).
        /// Viene utilizzato per proporla in caso di pubblicazione di un RF in RC
        /// </summary>
        public String Descrizione { get; set; }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }

            set
            {
                // Nel caso di invio di un RF, il controllo è visualizzabile
                // se è attiva la relativa funzionalità
                base.Visible = value && this.GestioneRubricaComuneAbilitata;

                if (base.Visible)
                    this.UpdateControlData();
            }

        }

        private void UpdateControlData()
        {
            var urlSend = String.Format("../Gestione_Organigramma/InvioUORubricaComune.aspx?IdElemento={0}&TipoElemento={1}&Descrizione={2}", this.IdElemento, this.Tipo, HttpUtility.UrlEncode(this.Descrizione).Replace("'","\\'"));

            this.btnPubblicaInRC.OnClientClick = String.Format("return window.showModalDialog('{0}', '', 'dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;');", urlSend);
        }

        private bool GestioneRubricaComuneAbilitata
        {
            get
            {
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.DocsPaWR.ConfigurazioniRubricaComune config = DocsPAWA.RubricaComune.Configurazioni.GetConfigurazioni(sessionManager.getUserAmmSession());
                return config.GestioneAbilitata;
            }
        }

    }

}
