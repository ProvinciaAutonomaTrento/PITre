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
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using DocsPAWA.DocsPaWR;
using log4net;
namespace DocsPAWA
{
    /// <summary>
    /// Summary description for TabTrasmissioniRic.
    /// </summary>
    public class tabTrasmissioniRicFasc : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(tabTrasmissioniRicFasc));
        protected System.Web.UI.WebControls.Table tbl_trasmRic;
        protected System.Web.UI.WebControls.Button btn_accetta;
        protected System.Web.UI.WebControls.Button btn_rifiuta;
        protected System.Web.UI.WebControls.Button btn_AdL;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Button btn_visto;
        protected System.Web.UI.WebControls.Button btn_vistoADL;
        protected System.Web.UI.WebControls.TextBox txt_noteAccRif;
        protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_enableAccRif;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr2_enableAccRif;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr2_vistoeADL;
        //my_var 
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected int DimTable;
        protected DocsPAWA.DocsPaWR.Trasmissione trasmRic;
        //protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmRic;
        protected Utilities.MessageBox MessageBox1;
        protected System.Web.UI.HtmlControls.HtmlGenericControl tagBase;

        protected System.Web.UI.WebControls.Table tblTx;

        protected System.Web.UI.WebControls.Panel pnl_destinatari;

        protected int red = 0;
        protected int green = 0;
        protected int blu = 0;

        protected bool toDoList;

        /// <summary>
        /// Caricamento dati trasmissione
        /// </summary>
        /// <param name="trasmissione"></param>
        protected void FetchData(Trasmissione trasmissione)
        {
            Boolean verificaRuolo = false;
            //controllo per trasmissioni del ruolo
            //controllo se devo abilitare i tasti 
            if (trasmissione.trasmissioniSingole.Length > 0)
            {

                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmRic.trasmissioniSingole);

                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();

                DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);

                DocsPaWR.TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

                if (trasmSing == null)
                {
                    // Se non è stata trovata la trasmissione come destinatario ad utente, 
                    // cerca quella con destinatario ruolo corrente dell'utente
                    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();

                    trasmSing = trasmSingoleUtente.Where(e => e.corrispondenteInterno.systemId == infoUtente.idCorrGlobali).FirstOrDefault();

                    //Modifica per ricerche trasmissioni a ruolo per problema trasmissione utente
                    if (trasmSing == null || (trasmSing.trasmissioneUtente.Length == 0))
                    {
                        DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        DocsPaWR.TrasmissioneUtente[] trasmUtenteNew = wws.getTrasmissioneUtenteInRuolo(infoUtente, trasmissione.trasmissioniSingole[0].systemId);
                        if (trasmUtenteNew.Length == 0)
                        {
                            verificaRuolo = true;
                        }
                        else
                        {
                            if (trasmSing != null)
                            {
                                for (int i = 0; i < trasmUtenteNew.Length; i++)
                                {
                                    if (trasmSing.trasmissioneUtente.Length == 0)
                                    {
                                        trasmSing.trasmissioneUtente = new TrasmissioneUtente[trasmUtenteNew.Length];
                                        trasmSing.trasmissioneUtente[i] = trasmUtenteNew[i];
                                    }
                                    else
                                    {
                                        trasmSing.trasmissioneUtente[i] = trasmUtenteNew[i];
                                    }
                                }
                                if (trasmissione.trasmissioniSingole.Length <= 1)
                                {
                                    trasmissione.trasmissioniSingole = new TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
                                    trasmissione.trasmissioniSingole[0] = trasmSing;
                                }
                                else
                                {
                                    trasmissione.trasmissioniSingole[1] = trasmSing;
                                }
                            }
                            verificaRuolo = false;
                        }
                    }
                }

                if (verificaRuolo == false)
                {
                   
                    
                    DocsPaWR.TrasmissioneUtente trasmUtente = null;

                    DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
                    DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

                    listaTrasmSing = trasmissione.trasmissioniSingole;
                    if (listaTrasmSing.Length > 0)
                    {
                        trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[0];
                        listaTrasmUtente = trasmSing.trasmissioneUtente;
                        if (listaTrasmUtente.Length > 0)
                            trasmUtente = (DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[0];
                    }
                    
                    //FINE MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE

                    if (trasmSing.ragione.tipo == "W" && trasmUtente.dataRifiutata.Equals("") && trasmUtente.dataAccettata.Equals(""))
                    {
                       // drawBorderRow();

                        this.isEnabledAccRif(this.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing));
                    }

                    if (trasmSing.ragione.tipo != "W")
                    {
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"]) &&
                            ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"] == "2")
                        {
                            this.tr2_vistoeADL.Visible = true;
                        

                        DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                        if (wws.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                            this.tr2_vistoeADL.Visible = true;
                        else
                            this.tr2_vistoeADL.Visible = false;
                        }
                    }

                    drawTableDettagli(trasmissione);

                    //DETTAGI DESTINATARI

                    string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_DETTAGLIO_TRASM_TODOLIST");
                    if (this.toDoList == true && !string.IsNullOrEmpty(valoreChiave) || this.toDoList == false)
                    {
                        this.pnl_destinatari.Visible = true;
                        drawTableDettagliDestinatari(trasmissione);
                    }
                }
                else
                {
                    drawTableNoNotificaRuolo(trasmissione);
                    //DETTAGI DESTINATARI
                    string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_DETTAGLIO_TRASM_TODOLIST");
                    if (this.toDoList == true && !string.IsNullOrEmpty(valoreChiave) || this.toDoList == false)
                    {
                        this.pnl_destinatari.Visible = true;
                        drawTableDettagliDestinatari(trasmissione);
                    }
                }

            }
        }

        private bool checkTrasm_UNO_TUTTI_AccettataRifiutata(DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSingola)
        {
            DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return wws.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSingola);
        }
        private void Page_Load(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            Response.Expires = -1;
            Utils.startUp(this);

            this.tblTx.Style.Remove("border-collapse");

            // prelevo l'informazione del colore del tema dal DB
            string Tema = GetCssAmministrazione();
            string color = string.Empty;
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                color = realTema[2];
            }
            else
                color = "810d06";

            red = Convert.ToInt32(color.Substring(0, 2), 16);
            green = Convert.ToInt32(color.Substring(2, 2), 16);
            blu = Convert.ToInt32(color.Substring(4), 16);

            if (!string.IsNullOrEmpty(this.Request.QueryString["nomeForm"]))
            {
                // from : TODOLIST
                this.tagBase.Visible = true;
                this.btn_chiudi.Visible = true;
                if ((this.Request.QueryString["nomeForm"].ToUpper()).Equals("TODOLIST"))
                {
                    this.toDoList = true;
                }
                else
                {
                    this.toDoList = false;
                }
            }
            else
            {
                // default
                this.tagBase.Visible = false;
                this.btn_chiudi.Visible = false;
            }

            try
            {
                if (!this.IsPostBack)
                {
                    trasmRic = TrasmManager.getDocTrasmSel(this);

                    if (trasmRic != null)
                        this.FetchData(trasmRic);
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                System.Web.Services.Protocols.SoapException et = es;
                logger.Error("Errore" + es.Message.ToString());
                Response.Redirect("ErrorPage.aspx?error=" + es.Message.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Errore" + ex.Message.ToString());
                Response.Redirect("ErrorPage.aspx?error=" + ex.Message.ToString());
            }
            logger.Info("END");
        }

        protected void drawTableNoNotificaRuolo(DocsPAWA.DocsPaWR.Trasmissione trasmNoNotifica)
        {

            //proprietà tabella			
            this.tbl_trasmRic.CssClass = "testo_grigio";
            this.tbl_trasmRic.CellPadding = 1;
            this.tbl_trasmRic.CellSpacing = 1;
            this.tbl_trasmRic.BorderWidth = 1;
            this.tbl_trasmRic.Attributes.Add("style", "'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 100%; BORDER-BOTTOM: 1px solid'");
            this.tbl_trasmRic.BackColor = Color.FromArgb(255, 255, 255);

            this.titolo.Text = "Dettaglio trasmissione senza notifica";

            //crea tr e td
            TableRow triga = new TableRow();
            TableCell tcell = new TableCell();
            string txt_val;

            // SEGNATURA
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(75, 75, 75);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Segnatura";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmNoNotifica.infoFascicolo.codice;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // Oggetto
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(75, 75, 75);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Oggetto";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;

            string errorMessage;
            int result = DocumentManager.verificaACL("F", trasmNoNotifica.infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out errorMessage);
            if (result == 0 || result == 1)
            {
                tcell.Text = "Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo trasmesso";
            }
            else
            {
                tcell.Text = trasmNoNotifica.infoFascicolo.descrizione;
            }

            
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // NOTE GENERALI
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(75, 75, 75);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Note generali";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmNoNotifica.noteGenerali;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

        }

        protected void drawTableDettagli(DocsPAWA.DocsPaWR.Trasmissione trasmRic)
        {
            //proprietà tabella			
            this.tbl_trasmRic.CssClass = "testo_grigio";
            this.tbl_trasmRic.CellPadding = 1;
            this.tbl_trasmRic.CellSpacing = 1;
            this.tbl_trasmRic.BorderWidth = 1;
            this.tbl_trasmRic.BackColor = Color.FromArgb(255, 255, 255);

            //Instanzio l'oggetto trasmissione singola e trasmissione utente
            DocsPaWR.TrasmissioneSingola trasmSing = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            DocsPaWR.TrasmissioneUtente trasmUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
            DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
            DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

            listaTrasmSing = trasmRic.trasmissioniSingole;
            if (listaTrasmSing.Length > 0)
            {
                trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[0];
                listaTrasmUtente = trasmSing.trasmissioneUtente;
                if (listaTrasmUtente.Length > 0)
                    trasmUtente = (DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[0];
            }

            if (trasmRic != null && trasmRic.infoFascicolo != null && trasmRic.infoFascicolo.codice == null)
            {
                DocsPAWA.DocsPaWR.Fascicolo tempFasc = new DocsPAWA.DocsPaWR.Fascicolo();
                DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                tempFasc = wws.FascicolazioneGetDettaglioFascicolo(infoUtente, trasmRic.infoFascicolo, false);
                trasmRic.infoFascicolo.codice = tempFasc.codice;
                trasmRic.infoFascicolo.descrizione = tempFasc.descrizione;
            }
            //crea tr e td
            TableRow triga = new TableRow();
            TableCell tcell = new TableCell();
            string txt_val;

            // CODICE FASCICOLO
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(75, 75, 75);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Codice Fascicolo";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmRic.infoFascicolo.codice;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // DESCRIZIONE FASCICOLO
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Descrizione";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;

            string errorMessage;

            int result = DocumentManager.verificaACL("F", trasmRic.infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out errorMessage);
            if (result == 0 || result == 1)
            {
                tcell.Text = "Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo trasmesso";
            }
            else
            {
                tcell.Text = trasmRic.infoFascicolo.descrizione;
            }
            
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // NOTE GENERALI
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(75, 75, 75);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Note generali";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmRic.noteGenerali;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // NOTE INDIVIDUALI
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Note individuali";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmSing.noteSingole;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // DESTINATARIO			
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Destinatario";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            txt_val = trasmSing.corrispondenteInterno.descrizione;
            if (!(txt_val != null && !txt_val.Equals(""))) txt_val = "";
            tcell.Text = txt_val;
            triga.Cells.Add(tcell);

            // VISTA IL 
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Vista il";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(20);
            txt_val = trasmUtente.dataVista;
            tcell.Text = txt_val;
            triga.Cells.Add(tcell);

            // RISPOSTO IL
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Risposto il";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(20);
            txt_val = trasmUtente.dataRisposta;
            tcell.Text = txt_val;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // ACCETTATA IL
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Accettata il";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(20);
            txt_val = trasmUtente.dataAccettata;
            tcell.Text = txt_val;
            triga.Cells.Add(tcell);

            // RIFIUTATA IL
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Rifiutata il";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(20);
            txt_val = trasmUtente.dataRifiutata;
            tcell.Text = txt_val;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // NOTE ACC/RIF
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Note Acc/Rif";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            if (trasmUtente.dataRifiutata != null && !trasmUtente.dataRifiutata.Equals(""))
                tcell.Text = trasmUtente.noteRifiuto;
            else
            {
                if (trasmUtente.dataAccettata != null && !trasmUtente.dataAccettata.Equals(""))
                    tcell.Text = trasmUtente.noteAccettazione;
            }
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);

            // RIMOSSA DA COSE DA FARE
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(30);
            tcell.Text = "Rimossa da cose da fare";
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "testo_grigio";
            tcell.BackColor = Color.FromArgb(242, 242, 242);
            tcell.Width = Unit.Percentage(70);
            tcell.ColumnSpan = 3;
            tcell.Text = trasmUtente.dataRimossaTDL;
            triga.Cells.Add(tcell);
            this.tbl_trasmRic.Rows.Add(triga);
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
            this.tblTx.PreRender += new System.EventHandler(this.TblTx_PreRender);
            this.btn_accetta.Click += new System.EventHandler(this.btn_accetta_Click);
            this.btn_rifiuta.Click += new System.EventHandler(this.btn_rifiuta_Click);
            this.btn_AdL.Click += new EventHandler(this.btn_AdL_Click);
            this.MessageBox1.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.MessageBox1_GetMessageBoxResponse);
            this.Load += new System.EventHandler(this.Page_Load);
            this.btn_visto.Click += new EventHandler(btn_visto_Click);
            this.btn_vistoADL.Click += new EventHandler(btn_vistoADL_Click);
        }

        void btn_vistoADL_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;
            string idTrasm = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).systemId;
            string docNumber = infoFascicolo.idFascicolo;
            FileManager.setdatavistaSP_TV(UserManager.getInfoUtente(), docNumber, "F", String.Empty, idTrasm);
            DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, UserManager.getInfoUtente().idGruppo, UserManager.getInfoUtente().idPeople, ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo);
            FascicoliManager.addFascicoloInAreaDiLavoro(this, fascicolo);
            //Response.Write("<script language='javascript'>self.close();</script>");
            if (!string.IsNullOrEmpty(this.Request.QueryString["nomeForm"]))
                Response.Write("<script language='javascript'>self.close();</script>");
            else
                Response.Write("<script language='javascript'>try { top.principale.document.location = '../fascicolo/gestioneFasc.aspx?tab=Trasmissioni'; } catch (e) {}</script>");
        }

        void btn_visto_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;
            string idTrasm = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).systemId;
            string docNumber = infoFascicolo.idFascicolo;
            FileManager.setdatavistaSP_TV(UserManager.getInfoUtente(), docNumber, "F", String.Empty, idTrasm);
            DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, UserManager.getInfoUtente().idGruppo, UserManager.getInfoUtente().idPeople, ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo);
            //Response.Write("<script language='javascript'>self.close();</script>");
            if (!string.IsNullOrEmpty(this.Request.QueryString["nomeForm"]))
                Response.Write("<script language='javascript'>self.close();</script>");
            else
                Response.Write("<script language='javascript'>try { top.principale.document.location = '../fascicolo/gestioneFasc.aspx?tab=Trasmissioni'; } catch (e) {}</script>");

        }
        #endregion

        protected string getDataCorrente()
        {
            DateTime dt_cor = DateTime.Now;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
            System.ComponentModel.DateTimeConverter dCon = new System.ComponentModel.DateTimeConverter();
            return dt_cor.ToString("d", ci);
        }

        private bool accettaRifiuta(DocsPAWA.DocsPaWR.TrasmissioneTipoRisposta tipoRisp)
        {
            logger.Info("BEGIN");
            DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
            this.trasmRic = trasmissione;
            bool rtn = true;
            string errore = string.Empty;
            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
            {
                if (this.txt_noteAccRif.Text.Equals(""))
                {
                    Response.Write("<script>alert('Inserire le note del Rifiuto');</script>");
                    rtn = false;

                }
            }
            //Chiamo il metodo
            try
            {
                if (rtn)
                {
                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                    System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(trasmRic.trasmissioniSingole);

                    List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();

                    DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);

                    DocsPaWR.TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

                    if (trasmSing == null)
                    {
                        // Se non è stata trovata la trasmissione come destinatario ad utente, 
                        // cerca quella con destinatario ruolo corrente dell'utente
                        trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();

                        trasmSing = trasmSingoleUtente.Where(e => e.corrispondenteInterno.systemId == infoUtente.idCorrGlobali).FirstOrDefault();
                    }

                    //aggiorno la trasmissione singola con i dati 
                    //DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)this.trasmRic.trasmissioniSingole[0];
                    DocsPaWR.TrasmissioneUtente trasmUtente = null;

                    //MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE

                    /*  for (int i = 0; i < trasmSing.trasmissioneUtente.Length; i++)
                      {
                          if (trasmSing.trasmissioneUtente[i].utente.idPeople == UserManager.getUtente().idPeople)
                          {
                              trasmUtente = trasmSing.trasmissioneUtente[i];
                              break;
                          }

                      }*/

                    DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
                    DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

                    listaTrasmSing = trasmissione.trasmissioniSingole;
                    if (listaTrasmSing.Length > 0)
                    {
                        trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[0];
                        listaTrasmUtente = trasmSing.trasmissioneUtente;
                        if (listaTrasmUtente.Length > 0)
                            trasmUtente = (DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[0];
                    }


                    //note acc/rif
                    if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                        trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                    else
                        trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                    //data Accettazione /Rifiuto
                    if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                        trasmUtente.dataRifiutata = Utils.getDataOdiernaDocspa(); //getDataCorrente();
                    else
                        trasmUtente.dataAccettata = Utils.getDataOdiernaDocspa(); //getDataCorrente();
                    //tipoRisposta
                    trasmUtente.tipoRisposta = tipoRisp;

                    if (TrasmManager.executeAccRif(this, trasmUtente, trasmRic.systemId, out errore))
                    {
                        TrasmManager.setDocTrasmSel(this, trasmRic);
                    }
                    else
                    {
                        rtn = false;
                        trasmUtente.dataRifiutata = null;
                        trasmUtente.dataAccettata = null;
                        //Response.Write("<script>alert('In questo momento non è stato possibile accettare/rifiutare la trasmissione. Riprovare più tardi!');</script>");
                        Response.Write("<script>alert('" + errore + "');</script>");
                    }

                    //NEL CASO DI TRASMISSIONE A RUOLO ACCETTO TUTTE LE TRASMISSIONE AD UTENTE
                    if (trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO && listaTrasmSing.Length > 1)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSingTemp;
                        for (int i = 1; i < listaTrasmSing.Length; i++)
                        {
                            trasmSingTemp = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[i];
                            listaTrasmUtente = trasmSingTemp.trasmissioneUtente;
                            if (listaTrasmUtente.Length > 0 && trasmSingTemp.tipoDest == TrasmissioneTipoDestinatario.UTENTE)
                            {
                                trasmUtente = (DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSingTemp.trasmissioneUtente[0];
                                //note acc/rif
                                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                                    trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                                else
                                    trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                                //data Accettazione /Rifiuto
                                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                                    trasmUtente.dataRifiutata = Utils.getDataOdiernaDocspa(); //getDataCorrente();
                                else
                                    trasmUtente.dataAccettata = Utils.getDataOdiernaDocspa(); //getDataCorrente();
                                //tipoRisposta
                                trasmUtente.tipoRisposta = tipoRisp;

                                TrasmManager.executeAccRif(this, trasmUtente, trasmRic.systemId, out errore);
                            }
                        }
                    }

                }
                logger.Info("END");
                return rtn;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                rtn = false;
                System.Web.Services.Protocols.SoapException et = es;
                Response.Redirect("ErrorPage.aspx?error=" + es.Message.ToString());
                return rtn;
            }
        }

        private void btn_rifiuta_Click(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            try
            {
                bool result = accettaRifiuta(DocsPAWA.DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO);
                if (result)
                {
                    this.isEnabledAccRif(false);

                    //Cancello i riferimenti alle tramissioni da controllare per quanto riguarda
                    //il passaggio di stato automatico
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;
                        string idProject = infoFascicolo.idFascicolo;
                        DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoFasc(idProject, this);
                        if (stato != null)
                        {
                            string idStato = Convert.ToString(stato.SYSTEM_ID);
                            DocsPAWA.DiagrammiManager.deleteStoricoTrasmDiagrammiFasc(idProject, idStato, this);
                        }
                    }
                }
                //rimuovo dalla sessione l'ogggetto data che contiene le info_todolist in quanto è da aggiornare!
                Session.Remove("data");
                //ClientScript.RegisterStartupScript(this.GetType(), "chiude", "<script>self.close();</script>");
                // Ai DS da fastidio, verificare !!
                //Aggiornamento dati dettaglio trasmissione
                DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);

                this.FetchData(trasmissione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AdL_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.btn_accetta_Click(sender, e);

                // inserisco il documento nell'area di lavoro dell'utente
                trasmRic = TrasmManager.getDocTrasmSel(this);
                //DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;
                //trasmRic = TrasmManager.trasmGetDettaglioTrasmissione (UserManager.getUtente(this), UserManager.getRuolo(this), this.CurrentPageList[e.Item.ItemIndex]);
                if (trasmRic != null)
                {

                    if (trasmRic.infoFascicolo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

                        DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, infoUtente.idGruppo, infoUtente.idPeople, trasmRic.infoFascicolo);

                        FascicoliManager.addFascicoloInAreaDiLavoro(this, fascicolo);
                    }
                }

                //luluciani  ClientScript.RegisterStartupScript(this.GetType(), "chiude", "<script>self.close();</script>");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void btn_accetta_Click(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
            this.trasmRic = trasmissione;

            bool rtn = accettaRifiuta(DocsPAWA.DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);
            if (rtn)
            {
                this.isEnabledAccRif(false);
            }
            Session.Remove("data");

            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                // DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);
                if (trasmissione.tipoOggetto.ToString() == "FASCICOLO")
                {
                    DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;

                    //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                    if (DocsPAWA.DiagrammiManager.isUltimaDaAccettare(trasmissione.systemId, this))
                    {

                        DocsPaWR.Stato statoSucc = DocsPAWA.DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo, this);
                        DocsPaWR.Stato statoCorr = DocsPAWA.DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo, this);

                        //Se il fascicolo è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                        Templates tipoFasc = ProfilazioneFascManager.getTemplateFascDettagli(infoFascicolo.idFascicolo, this);
                        if (tipoFasc != null && tipoFasc.IN_ESERCIZIO.ToUpper().Equals("NO"))
                        {
                            RegisterStartupScript("docInCheckOut", "<script>alert('Attenzione il fascicolo appartiene ad una tipologia sospesa. \\nNon è stato considerato nessun eventuale passaggio di stato automatico.');</script>");
                            return;
                        }
                        
                        if (statoSucc != null)
                        {
                            if (statoSucc.STATO_FINALE)
                            {
                                //Scatta l'alert
                                MessageBox1.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl documento diventerà di sola lettura.\\nConfermi ?");
                                return;
                            }
                            else
                            {
                                //Cambio stato
                                DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA), this);
                                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(infoFascicolo.idFascicolo, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                                //Cancellazione storico trasmissioni
                                DocsPAWA.DiagrammiManager.deleteStoricoTrasmDiagrammiFasc(infoFascicolo.idFascicolo, Convert.ToString(statoCorr.SYSTEM_ID), this);
                                //Verifico se il nuovo stato ha delle trasmissioni automatiche
                                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo, this);
                                string idTipoFasc = ProfilazioneFascManager.getIdTemplateFasc(infoFascicolo.idFascicolo, this);
                                if (idTipoFasc != "")
                                {
                                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc, this));
                                    for (int i = 0; i < modelli.Count; i++)
                                    {
                                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                        if (mod.SINGLE == "1")
                                        {
                                            TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo, this);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < mod.MITTENTE.Length; j++)
                                            {
                                                if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                                {
                                                    TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo, this);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            FascicoliManager.removeFascicoloSelezionato(this);
            Fascicolo fasc = FascicoliManager.getFascicolo(this, ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo.idFascicolo);
            FascicoliManager.setFascicoloSelezionato(fasc);
            this.FetchData(trasmissione);
            Response.Write("<script language='javascript'>try { top.principale.document.location = '../fascicolo/gestioneFasc.aspx?tab=documenti'; } catch (e) {}</script>");
            logger.Info("END");
        }

        private void isEnabledAccRif(bool isEnabled)
        {
            this.tr_enableAccRif.Visible = isEnabled;
            this.tr2_enableAccRif.Visible = isEnabled;
        }

        private void MessageBox1_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPaWR.InfoFascicolo infoFascicolo = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo;
                DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                DocsPaWR.Stato statoSucc = DocsPAWA.DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo, this);
                DocsPaWR.Stato statoCorr = DocsPAWA.DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo, this);

                //Cambio stato
                DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA), this);
                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(infoFascicolo.idFascicolo, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                //Cancellazione storico trasmissioni
                DocsPAWA.DiagrammiManager.deleteStoricoTrasmDiagrammiFasc(infoFascicolo.idFascicolo, Convert.ToString(statoCorr.SYSTEM_ID), this);

                //Verifico se il nuovo stato ha delle trasmissioni automatiche
                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo, this);
                string idTipoFasc = ProfilazioneFascManager.getIdTemplateFasc(infoFascicolo.idFascicolo, this);
                if (idTipoFasc != "")
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc, this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        if (mod.SINGLE == "1")
                        {
                            TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo, this);
                        }
                        else
                        {
                            for (int j = 0; j < mod.MITTENTE.Length; j++)
                            {
                                if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoFascicolo, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                //Metto il fascicolo in sola lettura
                DocsPAWA.DocsPaWR.Fascicolo fasc = (DocsPAWA.DocsPaWR.Fascicolo)wws.FascicolazioneGetDettaglioFascicolo(UserManager.getInfoUtente(this), infoFascicolo, false);
                fasc.stato = "C";
                FascicoliManager.setFascicolo(this, ref fasc);
                FascicoliManager.setFascicoloSelezionato(this, fasc);

                bool result = accettaRifiuta(DocsPAWA.DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);
                //this.btn_accetta.Visible	= false;
                //this.btn_rifiuta.Visible	= false;
                //this.txt_noteAccRif.Visible = false;
                if (result)
                {
                    this.isEnabledAccRif(false);

                }
            }
        }

        private void drawTableDettagliDestinatari(DocsPAWA.DocsPaWR.Trasmissione trasmDett)
        {

            //prendo le trasm singole dalla Trasm
            DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            System.Collections.Generic.List<FiltroRicerca> filters = new System.Collections.Generic.List<FiltroRicerca>();

            FiltroRicerca item = new FiltroRicerca();
            item.argomento = "ID_TRASMISSIONE";
            item.valore = trasmDett.systemId;
            filters.Add(item);


            DocsPaWR.Trasmissione[] trasmissioni = null;
            int totalPageNumber;
            int recordCount;
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
            DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
            DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
            DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento = null;

            trasmissioni = TrasmManager.getQueryEffettuateDocumentoPaging(this, oggettoTrasm, utenteCorrente, mioRuolo, filters.ToArray(), 1, out totalPageNumber, out recordCount); ;

            if (trasmissioni.Length > 0)
            {
                DocsPAWA.DocsPaWR.TrasmissioneSingola[] listTrasmSing = trasmissioni[0].trasmissioniSingole;

                if (listTrasmSing != null)
                {
                    for (int g = 0; g < listTrasmSing.Length; g++)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listTrasmSing[g];
                        DrawTable(tblTx, trasmSing, g);
                    }
                }
            }
        }

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }



        protected void DrawTable(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing, int h)
        {

            //sto solo visualizzando le trasmissioni singole e utente

            this.DrawHeaderTrasmSing(tbl);
            DrawRowTxVisual(tbl, trasmSing, h);
        }

        protected void DrawHeaderTrasmSing(System.Web.UI.WebControls.Table tbl)
        {
            // Put user code to initialize the page here
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            //tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Destinatario";
            tc.Width = Unit.Percentage(17);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            //tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Ragione";
            tc.Width = Unit.Percentage(24);
            tc.ColumnSpan = 2;
            tr.Cells.Add(tc);


            //Tipo c'è solo per determinate ragioni
            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            //tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Tipo";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            //tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Note Individuali";
            tc.Height = Unit.Pixel(15);

            tc.Width = Unit.Percentage(25);

            tr.Cells.Add(tc);


            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            //tc.BackColor=Color.FromArgb(149,149,149);
            tc.BackColor = Color.FromArgb(red, green, blu);
            tc.Text = "Scade il";

            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);



            tbl.Rows.Add(tr);

        }

        protected void DrawRowTxVisual(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing, int f)
        {

            try
            {

                TableRow tr = new TableRow();
                tr.CssClass = "bg_grigioN";
                TableCell tc = new TableCell();

                //Destinatario		
                tc = new TableCell();
                tc.Text = formatBlankValue(trasmSing.corrispondenteInterno.descrizione);
                tc.Width = Unit.Percentage(17);
                tr.Cells.Add(tc);

                // Ragione
                tc = new TableCell();
                tc.Text = formatBlankValue(trasmSing.ragione.descrizione);
                tc.Height = Unit.Pixel(15);
                tc.ColumnSpan = 2;
                tc.Width = Unit.Percentage(24);
                tr.Cells.Add(tc);

                //Tipo    c'è solo per trasmSing indirizzate a Ruoli
                tc = new TableCell();
                tc.Height = Unit.Pixel(15);
                tc.Width = Unit.Percentage(12);

                //if (trasmSing.ragione.tipo.Equals("W"))
                //{
                if (trasmSing.tipoTrasm.Equals("T"))
                {
                    tc.Text = "TUTTI";
                }
                else
                {
                    if (trasmSing.tipoTrasm.Equals("S"))
                    {
                        tc.Text = "UNO";
                    }
                    else
                    {
                        tc.Text = formatBlankValue(null);
                    }
                }
                //}
                tr.Cells.Add(tc);

                //Note Individuali
                tc = new TableCell();



                bool isOwnerTrasmissione = this.CheckTrasmRicevuteDaUtenteCorrente(trasmSing);
                bool canViewDettTrasmSingola = false;

                // visualizzazione del testo relativo alla nota singola
                // solamente se l'utente che ha effettuato la trasmissione
                // sia lo stesso di quello correntemente connesso
                if (isOwnerTrasmissione)
                    tc.Text = formatBlankValue(trasmSing.noteSingole);
                else
                    tc.Text = new string('-', 15);

                tc.Height = Unit.Pixel(15);
                tc.Width = Unit.Percentage(25);
                tr.Cells.Add(tc);

                //DATA SCAD.		
                tc = new TableCell();
                tc.Text = formatBlankValue(trasmSing.dataScadenza);
                tc.Height = Unit.Pixel(15);
                tc.Width = Unit.Percentage(12);
                tr.Cells.Add(tc);

                tbl.Rows.Add(tr);


                //************************************************************************
                //************ROW TRASM UTENTE********************************************
                //************************************************************************				
                if (trasmSing.trasmissioneUtente != null)
                {
                    DrawHeaderTrasmUt(tbl);
                    for (int i = 0; i < trasmSing.trasmissioneUtente.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente TrasmUt = (DocsPAWA.DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[i];

                        //Cella Bianca 	
                        tr = new TableRow();
                        tc = new TableCell();

                        //Utente					
                        tc = new TableCell();
                        if (TrasmUt.valida != null && TrasmUt.valida.Equals("1"))
                            tr.CssClass = "testo_grigio";
                        else
                            tr.CssClass = "testo_grigio_light";

                        tc.BackColor = Color.FromArgb(242, 242, 242);

                        string descrizioneUtente = TrasmUt.utente.descrizione;

                        tc.Text = formatBlankValue(descrizioneUtente);
                        tc.Width = Unit.Percentage(17);
                        tr.Cells.Add(tc);

                        //Data Vista
                        tc = new TableCell();
                        tc.HorizontalAlign = HorizontalAlign.Center;
                        tc.CssClass = "testo_grigio";
                        tc.BackColor = Color.FromArgb(242, 242, 242);
                        if (!string.IsNullOrEmpty(TrasmUt.dataVista) && (string.IsNullOrEmpty(TrasmUt.cha_vista_delegato) || TrasmUt.cha_vista_delegato.Equals("0")))
                            tc.Text = formatBlankValue(TrasmUt.dataVista);
                        else
                            tc.Text = formatBlankValue(null);
                        tc.Width = Unit.Percentage(12);
                        tr.Cells.Add(tc);

                        //Data Acc.		
                        //Tipo c'è solo per determinate ragioni
                        tc = new TableCell();
                        tc.HorizontalAlign = HorizontalAlign.Center;
                        tc.CssClass = "testo_grigio";
                        tc.BackColor = Color.FromArgb(242, 242, 242);
                        string textDataAcc;

                        if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                        {
                            textDataAcc = formatBlankValue(TrasmUt.dataAccettata);
                            //if (TrasmUt.cha_accettata_delegato.Equals("1"))
                            //    textDataAcc += "<br>(" + TrasmUt.idPeopleDelegato + ")";
                        }
                        else
                        {
                            textDataAcc = formatBlankValue(null);
                        }
                        tc.Text = textDataAcc;
                        tc.Height = Unit.Pixel(15);
                        tc.Width = Unit.Percentage(12);
                        tr.Cells.Add(tc);

                        //Data Rif.		
                        tc = new TableCell();
                        tc.HorizontalAlign = HorizontalAlign.Center;
                        tc.CssClass = "testo_grigio";
                        tc.BackColor = Color.FromArgb(242, 242, 242);

                        string textDataRif = ""; ;
                        if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                        {
                            textDataRif = formatBlankValue(TrasmUt.dataRifiutata);
                            //if (TrasmUt.cha_rifiutata_delegato.Equals("1"))
                            //    textDataRif += "<br>(" + TrasmUt.idPeopleDelegato + ")";
                        }
                        else
                        {
                            textDataRif = formatBlankValue(null);
                        }
                        tc.Text = textDataRif;
                        tc.Height = Unit.Pixel(15);
                        tc.Width = Unit.Percentage(12);
                        tr.Cells.Add(tc);

                        //Info 	Acc. / Info Rif.	
                        tc = new TableCell();
                        tc.CssClass = "testo_grigio";
                        tc.BackColor = Color.FromArgb(242, 242, 242);

                        // viene verificato se l'utente corrente
                        // può visualizzare i dettagli (note rifiuto e note accettazione)
                        // della trasmissione singola:
                        // - ha pieni diritti di visualizzazione
                        //   se è l'utente che ha creato la trasmissione;
                        // - altrimenti viene verificato se l'utente corrente è lo stesso
                        //   che ha ricevuto la trasmissione (e quindi l'ha accettata)
                        canViewDettTrasmSingola = (isOwnerTrasmissione);
                        if (!canViewDettTrasmSingola)
                            canViewDettTrasmSingola = this.CheckTrasmUtenteCorrente(TrasmUt);

                        if (!canViewDettTrasmSingola)
                        {
                            tc.Text = new string('-', 15);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                            {
                                tc.Text = formatBlankValue(TrasmUt.noteAccettazione);
                            }
                            else
                                if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                                    tc.Text = formatBlankValue(TrasmUt.noteRifiuto);
                                else
                                    tc.Text = formatBlankValue(null);
                        }

                        tc.Height = Unit.Pixel(15);
                        tc.Width = Unit.Percentage(25);
                        tr.Cells.Add(tc);

                        //Data Risp. è un link che porta al documento 
                        tc = new TableCell();
                        tc.HorizontalAlign = HorizontalAlign.Center;
                        tc.CssClass = "testo_grigio";
                        tc.BackColor = Color.FromArgb(242, 242, 242);
                        //creo obj-aspx Hyperlink 

                        if (trasmSing.ragione.descrizione.Equals("RISPOSTA"))
                        {
                            tc.Text = formatBlankValue(null);
                        }
                        else
                        {
                            tc.Text = formatBlankValue(null);
                        }
                        tc.Height = Unit.Pixel(15);
                        tc.Width = Unit.Percentage(12);
                        tr.Cells.Add(tc);
                        tbl.Rows.Add(tr);

                        //RIGA IN CASO DI DELEGA
                        if (!string.IsNullOrEmpty(TrasmUt.idPeopleDelegato) && (TrasmUt.cha_accettata_delegato == "1" || TrasmUt.cha_rifiutata_delegato == "1" || TrasmUt.cha_vista_delegato == "1") && (!string.IsNullOrEmpty(TrasmUt.dataVista) || !string.IsNullOrEmpty(TrasmUt.dataAccettata) || !string.IsNullOrEmpty(TrasmUt.dataRifiutata)))
                        {
                            //Cella Bianca 	
                            tr = new TableRow();

                            //Utente					
                            tc = new TableCell();
                            tr.CssClass = "testo_grigio_light";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            descrizioneUtente = TrasmUt.idPeopleDelegato + "<br>(Delegato da " + TrasmUt.utente.descrizione + ")";
                            tc.Text = formatBlankValue(descrizioneUtente);
                            tc.Width = Unit.Percentage(17);
                            tr.Cells.Add(tc);

                            //Data Vista
                            tc = new TableCell();
                            tc.HorizontalAlign = HorizontalAlign.Center;
                            tc.CssClass = "testo_grigio";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            if (!string.IsNullOrEmpty(TrasmUt.dataVista) && TrasmUt.cha_vista_delegato.Equals("1"))
                                tc.Text = formatBlankValue(TrasmUt.dataVista);
                            else
                                tc.Text = formatBlankValue(null);
                            tc.Width = Unit.Percentage(12);
                            tr.Cells.Add(tc);

                            //Data Acc.		
                            tc = new TableCell();
                            tc.HorizontalAlign = HorizontalAlign.Center;
                            tc.CssClass = "testo_grigio";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            if (TrasmUt.cha_accettata_delegato.Equals("1"))
                                textDataAcc = formatBlankValue(TrasmUt.dataAccettata);
                            else
                                textDataAcc = formatBlankValue(null);
                            tc.Text = textDataAcc;
                            tc.Height = Unit.Pixel(15);
                            tc.Width = Unit.Percentage(12);
                            tr.Cells.Add(tc);

                            //Data Rif.		
                            tc = new TableCell();
                            tc.HorizontalAlign = HorizontalAlign.Center;
                            tc.CssClass = "testo_grigio";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            textDataRif = formatBlankValue(TrasmUt.dataRifiutata);
                            if (TrasmUt.cha_rifiutata_delegato.Equals("1"))
                                textDataRif = formatBlankValue(TrasmUt.dataRifiutata);
                            else
                                textDataRif = formatBlankValue(null);
                            tc.Text = textDataRif;
                            tc.Height = Unit.Pixel(15);
                            tc.Width = Unit.Percentage(12);
                            tr.Cells.Add(tc);

                            //Info 	Acc. / Info Rif.	
                            tc = new TableCell();
                            tc.CssClass = "testo_grigio";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && TrasmUt.cha_accettata_delegato.Equals("1"))
                            {
                                tc.Text = formatBlankValue(TrasmUt.noteAccettazione);
                            }
                            else
                                if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && TrasmUt.cha_rifiutata_delegato.Equals("1"))
                                    tc.Text = formatBlankValue(TrasmUt.noteRifiuto);
                                else
                                    tc.Text = formatBlankValue(null);
                            //tc.Text = formatBlankValue(null);
                            tc.Height = Unit.Pixel(15);
                            tc.Width = Unit.Percentage(25);
                            tr.Cells.Add(tc);

                            //Data Risp. è un link che porta al documento 
                            tc = new TableCell();
                            tc.HorizontalAlign = HorizontalAlign.Center;
                            tc.CssClass = "testo_grigio";
                            tc.BackColor = Color.FromArgb(242, 242, 242);
                            tc.Text = formatBlankValue(null);
                            tc.Height = Unit.Pixel(15);
                            tc.Width = Unit.Percentage(12);
                            tr.Cells.Add(tc);
                            tbl.Rows.Add(tr);
                        }

                    }

                    //fine row utente
                    //riga rossa
                    tr = new TableRow();
                    tc = new TableCell();
                    tc.ColumnSpan = 6;
                    tr.CssClass = "bg_grigioS";
                    tc.Width = Unit.Percentage(100);
                    tc.Height = Unit.Pixel(10);
                    tr.Cells.Add(tc);

                    tbl.Rows.Add(tr);
                }
                else
                {
                    //riga rossa
                    tr = new TableRow();
                    tc = new TableCell();
                    tc.ColumnSpan = 6;
                    tr.CssClass = "bg_grigioS";
                    tc.Width = Unit.Percentage(100);
                    tc.Height = Unit.Pixel(10);

                    tr.Cells.Add(tc);

                    tr.Cells.Add(tc);

                    tbl.Rows.Add(tr);
                }

            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private string formatBlankValue(string valore)
        {
            string retValue = "&nbsp;";

            if (valore != null && valore != "")
            {
                retValue = valore;
            }

            return retValue;
        }

        /// <summary>
        /// verifica se l'utente relativo ad una trasmissione utente sia lo
        /// stesso soggetto correntemente connesso all'applicazione
        /// </summary>
        /// <param name="trasmUtente"></param>
        private bool CheckTrasmUtenteCorrente(DocsPAWA.DocsPaWR.TrasmissioneUtente trasmUtente)
        {
            bool retValue = false;

            if (trasmUtente.utente != null)
            {
                retValue = (trasmUtente.utente.idPeople.Equals(UserManager.getInfoUtente(this).idPeople));
            }

            return retValue;
        }

        protected void DrawHeaderTrasmUt(System.Web.UI.WebControls.Table tbl)
        {
            TableRow tr = new TableRow();

            TableCell tc = new TableCell();

            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Utente";
            tc.Width = Unit.Percentage(17);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Vista il";
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);


            //Tipo c'è solo per determinate ragioni
            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Acc. il";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Rif. il";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);


            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Info Acc. / Info Rif.";
            tc.Height = Unit.Pixel(15);

            tc.Width = Unit.Percentage(25);

            tr.Cells.Add(tc);



            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Risp. il";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(12);

            tr.Cells.Add(tc);




            tbl.Rows.Add(tr);



        }

        /// <summary>
        /// verifica se la trasmissione è stata ricevuta 
        /// dall'utente correntemente connesso
        /// </summary>
        /// <returns></returns>
        private bool CheckTrasmRicevuteDaUtenteCorrente(DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing)
        {
            bool retValue = false;
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
            DocsPaWR.Ruolo mioRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);

            foreach (DocsPAWA.DocsPaWR.TrasmissioneUtente trasmUt in trasmSing.trasmissioneUtente)
            {
                if (trasmUt.utente.idPeople.Equals(utenteCorrente.idPeople) || trasmSing.corrispondenteInterno.systemId.Equals(mioRuolo.systemId))
                {
                    retValue = true;
                }
            }


            return retValue;
        }

        private void TblTx_PreRender(object sender, System.EventArgs e)
        {

            this.tblTx.Attributes.Remove("cellspacing");
            this.tblTx.Attributes.Remove("cellpadding");
            this.tblTx.Attributes.Add("cellpadding", "0");
            this.tblTx.Attributes.Add("cellspacing", "0");
            //			this.tblTx.Attributes.Add("style","'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 100%; BORDER-BOTTOM: 1px solid'");
            this.tblTx.Attributes.Add("borderColorDark", "#ffffff");

        }
    }
}
