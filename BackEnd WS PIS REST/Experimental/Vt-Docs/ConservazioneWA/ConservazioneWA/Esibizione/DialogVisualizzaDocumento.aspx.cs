using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.Esibizione
{
    public partial class DialogVisualizzaDocumento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            // Parametri passati dal chiamante:
            string idIstanza = Request.QueryString["idIstanza"];
            this.hd_idIstanza.Value = idIstanza;
            string idDoc = string.Empty;
            idDoc = this.hd_idDocCorrente.Value;
            //string idDoc = Request.QueryString["idDoc"];
            //this.hd_idDoc.Value = idDoc;

            if (!IsPostBack)
            {
                this.hd_idDocPrincipale.Value = Request.QueryString["idDoc"];
                this.hd_idDocCorrente.Value = this.hd_idDocPrincipale.Value;
                this.btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            this.ImgBtn_Principale.Attributes.Add("onClick", string.Format("CaricaDoc('{0}');", this.hd_idDocPrincipale.Value));
            this.btnChiudi.Attributes.Add("onClick", "CloseWindow('Chiudi');");
            this.caricaAllegati(idIstanza, this.hd_idDocPrincipale.Value);
            

        }

        private void caricaAllegati(string idIstanza, string idDocPrincipale) 
        {

            //valore cablato
            bool localStore = true;
            //
            // Get isLocalStore
            localStore = ConservazioneWA.Utils.ConservazioneManager.isLocalStore();

            WSConservazioneLocale.InfoUtente infoUt = (WSConservazioneLocale.InfoUtente)Session["infoutCons"];

            //recupero la lista degli allegati dal file di chiusura
            Dictionary<string, string> lista = Utils.ConservazioneManager.getFilesFromUniSincro(infoUt, idIstanza, localStore);

            int i = 1;

            foreach (KeyValuePair<string, string> all in lista)
            {
                //se è un allegato contiene nel path l'iddoc del doc principale, e il suo id è diverso da quello del doc principale
                if (!(all.Key == idDocPrincipale) && ((all.Value.Split('§')[2]).Contains(idDocPrincipale)))
                {
                    string idAllegato = all.Value.Split('§')[1];

                    ImageButton btn = new ImageButton();
                    btn.ID = "btn_all" + i;
                    btn.ImageUrl = "../Img/ico_allegato.gif";
                    btn.Attributes.Add("onClick", string.Format("CaricaDoc('{0}');", idAllegato));
                    btn.ToolTip = "Allegato " + i;
                    this.div_allegati.Controls.Add(btn);
                    this.div_allegati.Controls.Add(new LiteralControl("<br />"));
                    //this.cell_btn.Controls.Add(btn);
                    //this.cell_btn.Controls.Add(new LiteralControl("<br />"));
                    i = i + 1;
                }
            }


        
        }

    }
}