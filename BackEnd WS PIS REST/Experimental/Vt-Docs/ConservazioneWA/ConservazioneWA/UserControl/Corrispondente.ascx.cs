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
using ConservazioneWA.Utils;

namespace ConservazioneWA.UserControl
{
    public partial class Corrispondente : System.Web.UI.UserControl
    {
        public string TIPO_CORRISPONDENTE = string.Empty;

        private string sCodice = string.Empty;
        private string sCssCodice = string.Empty;
        private string sDescrizione = string.Empty;
        private string sCssDescrizione = string.Empty;
        private int widthCodice = 0;
        private int widthDescrizione = 0;
        private bool sCodiceReadOnly = false;
        private bool sDescrizioneReadOnly = false;
        private bool disabledCorr = false;

        protected WSConservazioneLocale.InfoUtente infoUtente;

        public string FILTRO = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

            if (!IsPostBack)
            {
                this.txt_Codice.CssClass = CSS_CODICE;
                this.txt_Descrizione.CssClass = CSS_DESCRIZIONE;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Campo Codice
            if (sCodice != "")
                txt_Codice.Text = sCodice;
            if (sCssCodice != "")
                txt_Codice.CssClass = sCssCodice;
            if (sCodiceReadOnly)
            {
                txt_Codice.ReadOnly = sCodiceReadOnly;
                btn_Rubrica.Enabled = false;
            }
            if (widthCodice != 0)
                txt_Codice.Width = widthCodice;

            //Campo Descrizione
            if (sDescrizione != "")
                txt_Descrizione.Text = sDescrizione;
            if (sCssDescrizione != "")
                txt_Descrizione.CssClass = sCssDescrizione;

            txt_Descrizione.ReadOnly = sDescrizioneReadOnly;
            if (widthDescrizione != 0)
                txt_Descrizione.Width = widthDescrizione;

            if (disabledCorr)
            {
                this.txt_Codice.Text = string.Empty;
                this.txt_Descrizione.Text = string.Empty;
                this.txt_Codice.Enabled = false;
                this.txt_Descrizione.Enabled = false;
                this.btn_Rubrica.Enabled = false;
            }
            else
            {
                this.txt_Codice.Enabled = true;
                this.txt_Descrizione.Enabled = true;
                this.btn_Rubrica.Enabled = true;
            }

        }

        public bool DISABLED_CORR
        {
            get
            {
                return disabledCorr;
            }
            set
            {
                disabledCorr = value;
            }
        }


        public int WIDTH_CODICE
        {
            get
            {
                return widthCodice;
            }
            set
            {
                widthCodice = value;
            }
        }

        public int WIDTH_DESCRIZIONE
        {
            get
            {
                return widthDescrizione;
            }
            set
            {
                widthDescrizione = value;
            }
        }

        public string CSS_CODICE
        {
            get
            {
                return sCssCodice;
            }
            set
            {
                sCssCodice = value;
            }
        }

        public string CSS_DESCRIZIONE
        {
            get
            {
                return sCssDescrizione;
            }
            set
            {
                sCssDescrizione = value;
            }
        }

        public string TIPO
        {
            get
            {
                if (this.ViewState["TIPO"] != null)
                    return this.ViewState["TIPO"].ToString();
                else
                    return string.Empty;
            }
            protected set
            {
                this.ViewState["TIPO"] = value;
            }
        }

        public string ID_CORRISPONDENTE
        {
            get
            {
                if (this.ViewState["ID_CORRISPONDENTE"] != null)
                    return this.ViewState["ID_CORRISPONDENTE"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["ID_CORRISPONDENTE"] = value;
            }
        }

        public string SYSTEM_ID_CORR
        {
            get
            {
                return txt_SystemId.Value;
            }
            set
            {
                txt_SystemId.Value = value;
            }
        }

        public string CODICE_TEXT
        {
            get
            {
                return txt_Codice.Text;
            }
            set
            {
                sCodice = value;
                txt_Codice.Text = value;
            }
        }

        public string DESCRIZIONE_TEXT
        {
            get
            {
                return txt_Descrizione.Text;
            }
            set
            {
                sDescrizione = value;
                txt_Descrizione.Text = value;
            }
        }

        public bool CODICE_READ_ONLY
        {
            get
            {
                return sCodiceReadOnly;
            }
            set
            {
                sCodiceReadOnly = value;
            }
        }

        public bool DESCRIZIONE_READ_ONLY
        {
            get
            {
                return sDescrizioneReadOnly;
            }
            set
            {
                sDescrizioneReadOnly = value;
            }
        }

        protected void btn_Rubrica_Click(object sender, ImageClickEventArgs e)
        {
            ////In questo caso il campo è readOnly.
            ////Controllo solo il codice perchè la descrizione è sempre readOnly,
            ////l'apertura della rubrica è quindi inutile in quanto non si può
            ////modificare il valore di questo userControl
            //if (sCodiceReadOnly)
            //    return;

            //DocsPAWA.DocsPaWR.Ruolo ruoloSelezionato = UserManager.getRuolo();
            ////Se in sessione c'è un ruolo, vuol dire che sono sul frontend e quindi la
            ////rubrica puo' funzionare, in quanto preleva da sessione il registro, il ruolo e
            ////l'utente che la vorrebbero utilizzando.
            ////In caso contrario, vuol dire che sto chiamando l'apertura della rubrica
            ////dall'amministrazione, in particolare da una delle due anteprime di profilazione.
            ////In questo caso non essendoci nessun registro, nè ruolo nè utente in sessione,
            ////l'apertura della rubrica viene inibita, ma non è un problema in quanto essendo
            ////un antemprima, serve solo per determinare una veste grafica gradevole per tipo
            ////di documento o di fascicolo che si sta creando.            
            //if (ruoloSelezionato != null)
            //{
            //    if (TIPO_CORRISPONDENTE != null)
            //    {
            //        switch (TIPO_CORRISPONDENTE)
            //        {
            //            case "INTERNI":
            //                if (!string.IsNullOrEmpty(FILTRO) && FILTRO.Equals("NO_UO"))
            //                {
            //                    Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT_NO_UO');", true);
            //                }
            //                else
            //                {
            //                    Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT');", true);
            //                }
            //                break;

            //            case "ESTERNI":
            //                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_EST');", true);
            //                break;

            //            case "INTERNI/ESTERNI":
            //                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_INT_EST');", true);
            //                break;
            //            default:
            //                Page.ClientScript.RegisterStartupScript(this.GetType(), "aperturaRubrica", "apriRubrica('CALLTYPE_CORR_NO_FILTRI');", true);
            //                break;
            //        }
            //    }
            //}

            ////Aggiungo in sessione l'id del campo, perchè poichè in una popup di profilazione
            ////questo tipo di campo puo' essere ripetuto, devo riuscire a distinguerlo
            //Session.Add("rubrica.idCampoCorrispondente", this.ID);
        }

        protected void txt_Codice_TextChanged(object sender, EventArgs e)
        {
            //In questo caso il campo è readOnly.
            //Controllo solo il codice perchè la descrizione è sempre readOnly,
            //l'apertura della rubrica è quindi inutile in quanto non si può
            //modificare il valore di questo userControl
            if (sCodiceReadOnly)
                return;

            string condRegistri = string.Empty;
            if (regAll != null && regAll.Length > 0)
            {
                condRegistri = " and (id_registro in (";
                foreach (ConservazioneWA.WSConservazioneLocale.Registro reg in regAll)
                    condRegistri += reg.systemId + ",";
                condRegistri = condRegistri.Substring(0, condRegistri.Length - 1);
                condRegistri += ") OR id_registro is null)";
            }

            //Se in sessione c'è un ruolo, vuol dire che sono sul frontend e quindi la
            //rubrica puo' funzionare, in quanto preleva da sessione il registro, il ruolo e
            //l'utente che la vorrebbero utilizzando.
            //In caso contrario, vuol dire che sto chiamando l'apertura della rubrica
            //dall'amministrazione, in particolare da una delle due anteprime di profilazione.
            //In questo caso non essendoci nessun registro, nè ruolo nè utente in sessione,
            //l'apertura della rubrica viene inibita, ma non è un problema in quanto essendo
            //un antemprima, serve solo per determinare una veste grafica gradevole per tipo
            //di documento o di fascicolo che si sta creando.            

            //if (txt_Codice.Text != null && txt_Codice.Text != "")
            if ((sCodice != null && sCodice != "") || (txt_Codice.Text != null && txt_Codice.Text != ""))
            {
                ConservazioneWA.WSConservazioneLocale.ElementoRubrica er = ConservazioneManager.getElementoRubrica(this.Page, txt_Codice.Text, condRegistri, infoUtente);
                //DocsPaWR.Corrispondente er = UserManager.getCorrispondenteRubrica(this.Page, this.txt_Codice.Text, DocsPaWR.RubricaCallType.CALLTYPE_PROTO_IN);
                if (er != null && er.descrizione != null && er.descrizione != "")
                {
                    this.TIPO = er.tipo;
                    this.ID_CORRISPONDENTE = er.systemId;
                    txt_Descrizione.Text = er.descrizione;
                    txt_Codice.Text = er.codice;
                }
                else
                {
                    this.TIPO = "";
                    this.ID_CORRISPONDENTE = "";
                    txt_Descrizione.Text = "";
                    txt_Codice.Text = "";
                }
            }
            else
            {
                this.TIPO = "";
                this.ID_CORRISPONDENTE = "";
                txt_Descrizione.Text = "";
                txt_Codice.Text = "";
            }
        }

        /// <summary>
        /// Template selezionato
        /// </summary>
        protected WSConservazioneLocale.Registro[] regAll
        {
            get
            {
                return HttpContext.Current.Session["registri"] as WSConservazioneLocale.Registro[];
            }
            set
            {
                HttpContext.Current.Session["registri"] = value;
            }
        }
    }
}