using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DocsPAWA.UserControls
{
    public partial class RicercaNote : System.Web.UI.UserControl
    {
        protected  int caratteriDisponibili=2000;
        /// <summary>
        /// Proprietà testo. Imposta o restituisce il testo da ricercare
        /// all'interno delle note.
        /// </summary>
        public string Testo
        {
            get { return this.txt_note.Text; }
            set { this.txt_note.Text = value; }
        }

        public Unit TextBoxWidth
        {
            get { return this.txt_note.Width; }
            set { this.txt_note.Width = value; }
        }

        public Unit TextBoxHeight
        {
            get { return this.txt_note.Height; }
            set { this.txt_note.Height = value; }
        }

        public TextBoxMode TextMode
        {
            get { return this.txt_note.TextMode; }
            set { this.txt_note.TextMode = value; }
        }

        /// <summary>
        /// Proprietà tipo ricerca. Restituisce il carattere identificativo
        /// della tipologia di ricerca richiesta dall'utente.
        /// </summary>
        public char TipoRicerca
        {
            get 
            {
                // Il valore da restituire
                char toReturn = 'Q';

                // Se la lista di combo ha un elemento selezionato, ne viene restituito
                // il valore associato
                if(this.rl_visibilita.SelectedValue.Length > 0)
                    toReturn =  this.rl_visibilita.SelectedValue[0]; 

                // Restituzione del valore selezionato
                return toReturn;
            }
            set { this.rl_visibilita.SelectedValue = value.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!UserManager.isRFEnabled())
            {
                this.rl_visibilita.Items.Remove(this.rl_visibilita.Items.FindByValue("F"));
            }
            if (!IsPostBack)
            {
                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_NOTE");
                if (!string.IsNullOrEmpty(valoreChiave))
                    caratteriDisponibili = int.Parse(valoreChiave);


                txt_note.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                txt_note.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");
                txt_note.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Nota'," + clTesto.ClientID + ")");
            }
        }

        protected void DataChanged(object sender, EventArgs e)
        {
            ListItem item = this.rl_visibilita.Items.FindByValue("F");
            //Se è presente il bottone di selezione esclusiva "RF" si verifica quanti sono gli
            //RF associati al ruolo dell'utente
            if (item != null && this.rl_visibilita.Items.FindByValue("F").Selected)
            {
                DocsPaWR.Registro[] registriRf;
                if (ViewState["rf"] == null)
                {
                    DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo();
                    registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                }
                else
                    registriRf = (DocsPaWR.Registro[])ViewState["rf"];
                //Se un ruolo appartiene a più di un RF, allora selezionando dal menù il valore RF
                //l'utente deve selezionare su quale degli RF creare la nota
                if (registriRf != null && registriRf.Length > 0)
                {
                    Session.Add("RFNote", "OK^" + registriRf[0].systemId + "^" + registriRf[0].codRegistro);
                }
                else
                    Session.Remove("RFNote");
            }
        }

        public TextBox getTextBox()
        {
            return this.txt_note;
        }

    }
}