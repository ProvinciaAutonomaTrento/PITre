using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.CheckInOut;

namespace DocsPAWA.popup
{
    public partial class annullaContatore : System.Web.UI.Page
    {
        public string idOggetto;
        public string docNumber;
        private bool isCheckedOut = false;
        private bool isFinalState = false;
        private bool isToAccept = false;
        private bool isNotCancel = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            idOggetto = Request["idOggetto"];
            docNumber = Request["docNumber"];
            // Verifica se il documento è in stato checkout
            isCheckedOut = CheckInOutServices.IsCheckedOutDocument();
            //Verifica se il documento si trova nello stato finale
            DocsPaWR.Stato statoDoc = DocsPAWA.DiagrammiManager.getStatoDoc(docNumber, this.Page);
            isFinalState = statoDoc != null ? statoDoc.STATO_FINALE : false;
            //verifica se il documento deve essere ancora accettato a seguito di una trasmissione
            isToAccept = UserManager.disabilitaButtHMDirittiTrasmInAccettazione(DocumentManager.getDocumentoSelezionato().accessRights);
            //verifico se il repertorio può essere eliminato
            isNotCancel = isCheckedOut || isFinalState || isToAccept;
            // preparo l'eventuale motivo del mancato annullamento
            if (isCheckedOut)
            {
                this.lbl_messageCheckOut_descrizione.Text = "Il documento è in checked out" + "<br/>";
            }
            if (isFinalState)
            {
                this.lbl_messageCheckOut_descrizione.Text = "Il documento si trova nello stato finale" + "<br/>";
            }
            if (isToAccept)
            {
                this.lbl_messageCheckOut_descrizione.Text = "Il documento deve essere ancora accettato" + "<br/>";
            }

            if (isNotCancel)
            {
                this.lblAnnullamento.Style.Add("display", "none");
                this.txt_note_annullamento.Style.Add("display", "none");
                this.btn_ok.Style.Add("display", "none");
            }
            else
            {
                this.lbl_messageCheckOut.Style.Add("display", "none");
                this.lbl_messageCheckOut_descrizione.Style.Add("display", "none");
            }
        }

        protected void btn_ok_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txt_note_annullamento.Text) && !String.IsNullOrEmpty(idOggetto))
            {
                //Annullamento
                ProfilazioneDocManager.AnnullaContatoreDiRepertorio(idOggetto, docNumber, this);

                DocsPaWR.SchedaDocumento documentoSelezionato = DocumentManager.getDocumentoSelezionato(this);
                documentoSelezionato.template = ProfilazioneDocManager.getTemplateDettagli(docNumber, this);
                DocumentManager.setDocumentoSelezionato(documentoSelezionato);

                //Storicizzazione
                DocsPaWR.OggettoCustom oggettoCustom = documentoSelezionato.template.ELENCO_OGGETTI.Where(oggetto => oggetto.SYSTEM_ID.ToString().Equals(idOggetto)).FirstOrDefault();
                DocsPaWR.Storicizzazione storico = new DocsPaWR.Storicizzazione();
                storico.ID_TEMPLATE = documentoSelezionato.template.SYSTEM_ID.ToString();
                storico.DATA_MODIFICA = oggettoCustom.DATA_ANNULLAMENTO;
                storico.ID_PROFILE = documentoSelezionato.docNumber;
                storico.ID_OGG_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                storico.ID_PEOPLE = UserManager.getInfoUtente(this).idPeople;
                storico.ID_RUOLO_IN_UO = UserManager.getInfoUtente(this).idCorrGlobali;
                storico.DESC_MODIFICA = txt_note_annullamento.Text.Replace("'", "''");

                ProfilazioneDocManager.Storicizza(storico, this);

                ClientScript.RegisterStartupScript(this.GetType(), "close", "window.close();", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('La motivazione è obbligatoria.');", true);
            }
        }
    }
}
