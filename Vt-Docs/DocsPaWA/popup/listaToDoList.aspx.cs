using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Drawing;

namespace DocsPAWA.popup
{
    public partial class listaToDoList : System.Web.UI.Page
    {
        DocsPaWR.Utente utente;
        DocsPaWR.Ruolo ruoloCorrente;
        DocsPAWA.DocsPaWR.AllToDoList[] allTodoList;
        //protected System.Web.UI.WebControls.Button btn_ruolo;
        protected Table table;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Response.Expires = -1;

            ruoloCorrente = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            utente = UserManager.getUtente(this);
            if (!Page.IsPostBack)
            {
                
                //chiamata al webmethod che restituisce una lista con i dettagli per ogni todolist
                allTodoList = TrasmManager.getDettagliAllTodoList(utente);
                Session.Add("AllTodoList", allTodoList);
            }
            //costruzione grafica della lista
            if (allTodoList == null)
                allTodoList = (DocsPAWA.DocsPaWR.AllToDoList[])Session["AllTodoList"];
            inizializzaDati(allTodoList);
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
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        private void inizializzaDati(DocsPAWA.DocsPaWR.AllToDoList[] lista)
        {
            //proprietà tabella	
		    this.tbl_todolist.CssClass = "testo_grigio";
            this.tbl_todolist.CellPadding = 1;
            this.tbl_todolist.CellSpacing = 1;
            this.tbl_todolist.BorderWidth = 1;
            this.tbl_todolist.Attributes.Add("style", "'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 100%; BORDER-BOTTOM: 1px solid'");
            this.tbl_todolist.BackColor = Color.FromArgb(255, 255, 255);
            this.tbl_todolist.ID = "table_Todolist";

            TableRow triga; 
            TableCell tcell;
            int totNonLetti;
            int totNonAccettati;
            int totPredisposti;

            #region NOME COLONNE 
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);
            
            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(40);
            tcell.Text = "RUOLO";
            tcell.ColumnSpan = 2;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(20);
            tcell.Text = "TOTALI";
            tcell.HorizontalAlign = HorizontalAlign.Center;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(15);
            tcell.Text = "NON LETTI";
            tcell.HorizontalAlign = HorizontalAlign.Center;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(15);
            tcell.Text = "NON ACCETTATI";
            tcell.HorizontalAlign = HorizontalAlign.Center;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(15);
            tcell.Text = "PREDISPOSTI";
            tcell.HorizontalAlign = HorizontalAlign.Center;
            triga.Cells.Add(tcell);

            this.tbl_todolist.Rows.Add(triga);
            #endregion 

            System.Web.UI.WebControls.Button btn_ruolo;
            System.Web.UI.WebControls.Button btn_non_letti;
            System.Web.UI.WebControls.Button btn_non_accettati;
            System.Web.UI.WebControls.Button btn_doc_predisposti;
            int i = 0;

            //TableItemStyle tableStyle = new TableItemStyle();
            ////tableStyle.HorizontalAlign = HorizontalAlign.Center;
            ////tableStyle.VerticalAlign = VerticalAlign.Middle;
            //tableStyle.Font.Size = FontUnit.Small;
            //tableStyle.Font.Bold = true;
            //tableStyle.Font.Italic = true;
            //tableStyle.Wrap = true;

            foreach (DocsPAWA.DocsPaWR.AllToDoList todolist in lista)
            {
                #region RIGA DESCRIZIONE RUOLO
                triga = new TableRow();
                //triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);
                tcell = new TableCell();
                tcell.CssClass = "titolo_bianco";
                //tcell.ApplyStyle(tableStyle);
                tcell.ColumnSpan = 6;
                btn_ruolo = new System.Web.UI.WebControls.Button();
                btn_ruolo.ID = "btn_ruolo"+i;
                btn_ruolo.BorderWidth = 0;
                btn_ruolo.Font.Underline = true;
                btn_ruolo.CssClass = "pulsante_hand_3";
                btn_ruolo.BackColor = Color.FromArgb(242, 242, 242);
                btn_ruolo.ForeColor = Color.Black;
                //btn_ruolo.Height = Unit.Pixel(20);
                btn_ruolo.Click += new System.EventHandler(this.btn_ruolo_Click);
                int numCaratteri = 120;
                if (todolist.ruoloDesc.Length > numCaratteri)
                    btn_ruolo.Text = CalcolaTesto(numCaratteri, todolist.ruoloDesc);
                else
                    btn_ruolo.Text = todolist.ruoloDesc;
                tcell.Controls.Add(btn_ruolo);
                tcell.HorizontalAlign = HorizontalAlign.Left;
                triga.Cells.Add(tcell);
                this.tbl_todolist.Rows.Add(triga);
                #endregion

                #region RIGA TOTALI TRASMISSIONI
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);
                //CELLA VUOTA
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio_scuro";
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "&nbsp;";
                triga.Cells.Add(tcell);
                //CELLA TITOLO TRASMISSIONI
                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "TRASMISSIONI";
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);
                //CELLA TOTALE TRASMISSIONI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.Text = (todolist.trasmDocTot + todolist.trasmFascTot).ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA TOTALI NON LETTI TRASMISSIONI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.HorizontalAlign = HorizontalAlign.Center;
                totNonLetti = todolist.trasmDocNonLetti + todolist.trasmFascNonLetti;
                if (totNonLetti > 0)
                {
                    btn_non_letti = new System.Web.UI.WebControls.Button();
                    btn_non_letti.ID = "btn_Tra_NonLetti" + i;
                    btn_non_letti.BorderWidth = 0;
                    btn_non_letti.Font.Underline = true;
                    btn_non_letti.CssClass = "pulsante_hand_4";
                    btn_non_letti.Font.Bold = true;
                    btn_non_letti.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_letti.ForeColor = Color.Black;
                    btn_non_letti.Height = Unit.Pixel(20);
                    btn_non_letti.Click += new System.EventHandler(this.btn_non_letti_Click);
                    btn_non_letti.Text = totNonLetti.ToString();
                    tcell.Controls.Add(btn_non_letti);
                }
                else
                    tcell.Text = totNonLetti.ToString();
                triga.Cells.Add(tcell);
                //CELLA TOTALE NON ACCETTATI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                totNonAccettati = todolist.trasmDocNonAccettati + todolist.trasmFascNonAccettati;
                if (totNonAccettati > 0)
                {
                    btn_non_accettati = new System.Web.UI.WebControls.Button();
                    btn_non_accettati.ID = "btn_Tra_NonAccettati" + i;
                    btn_non_accettati.BorderWidth = 0;
                    btn_non_accettati.Font.Underline = true;
                    btn_non_accettati.CssClass = "pulsante_hand_4";
                    btn_non_accettati.Font.Bold = true;
                    btn_non_accettati.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_accettati.ForeColor = Color.Black;
                    btn_non_accettati.Height = Unit.Pixel(20);
                    btn_non_accettati.Click += new System.EventHandler(this.btn_non_accettati_Click);
                    btn_non_accettati.Text = totNonAccettati.ToString();
                    tcell.Controls.Add(btn_non_accettati);
                }
                else
                    tcell.Text = totNonAccettati.ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA DOCUMENTI PREDISPOSTI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                totPredisposti = todolist.docPredisposti;
                if (totPredisposti > 0)
                {
                    btn_doc_predisposti = new System.Web.UI.WebControls.Button();
                    btn_doc_predisposti.ID = "btn_Tra_DocPredisp" + i;
                    btn_doc_predisposti.BorderWidth = 0;
                    btn_doc_predisposti.Font.Underline = true;
                    btn_doc_predisposti.CssClass = "pulsante_hand_4";
                    btn_doc_predisposti.Font.Bold = true;
                    btn_doc_predisposti.BackColor = Color.FromArgb(242, 242, 242);
                    btn_doc_predisposti.ForeColor = Color.Black;
                    btn_doc_predisposti.Height = Unit.Pixel(20);
                    btn_doc_predisposti.Click += new System.EventHandler(this.btn_doc_predisposti_Click);
                    btn_doc_predisposti.Text = totPredisposti.ToString();
                    tcell.Controls.Add(btn_doc_predisposti);
                }
                else
                    tcell.Text = totPredisposti.ToString(); 
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                this.tbl_todolist.Rows.Add(triga);
                #endregion

                #region RIGA TOTALE DOCUMENTI
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);
                //CELLA VUOTA
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio_scuro";
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "&nbsp;";
                triga.Cells.Add(tcell);
                //CELLA TITOLO DOCUMENTI
                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "DOCUMENTI";
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);
                //CELLA TOTALI DOCUMENTI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.Text = todolist.trasmDocTot.ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA TOTALI DOCUMENTI NON LETTI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.HorizontalAlign = HorizontalAlign.Center;
                totNonLetti = todolist.trasmDocNonLetti;
                if (totNonLetti > 0)
                {
                    btn_non_letti = new System.Web.UI.WebControls.Button();
                    btn_non_letti.ID = "btn_Doc_NonLetti" + i;
                    btn_non_letti.BorderWidth = 0;
                    btn_non_letti.Font.Underline = true;
                    btn_non_letti.CssClass = "pulsante_hand_4";
                    btn_non_letti.Font.Bold = true;
                    btn_non_letti.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_letti.ForeColor = Color.Black;
                    btn_non_letti.Click += new System.EventHandler(this.btn_non_letti_Click);
                    btn_non_letti.Text = totNonLetti.ToString();
                    tcell.Controls.Add(btn_non_letti);
                }
                else
                    tcell.Text = totNonLetti.ToString();
                triga.Cells.Add(tcell);
                //CELLA DOCUMENTI NON ACCETTATI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                totNonAccettati = todolist.trasmDocNonAccettati;
                if (totNonAccettati > 0)
                {
                    btn_non_accettati = new System.Web.UI.WebControls.Button();
                    btn_non_accettati.ID = "btn_Doc_NonAccettati" + i;
                    btn_non_accettati.BorderWidth = 0;
                    btn_non_accettati.Font.Underline = true;
                    btn_non_accettati.CssClass = "pulsante_hand_4";
                    btn_non_accettati.Font.Bold = true;
                    btn_non_accettati.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_accettati.ForeColor = Color.Black;
                    btn_non_accettati.Click += new System.EventHandler(this.btn_non_accettati_Click);
                    btn_non_accettati.Text = totNonAccettati.ToString();
                    tcell.Controls.Add(btn_non_accettati);
                }
                else
                    tcell.Text = totNonAccettati.ToString();
                
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA TOTALE DOC PREDISPOSTI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                totPredisposti = todolist.docPredisposti;
                if (totPredisposti > 0)
                {
                    btn_doc_predisposti = new System.Web.UI.WebControls.Button();
                    btn_doc_predisposti.ID = "btn_DocPredisposti" + i;
                    btn_doc_predisposti.BorderWidth = 0;
                    btn_doc_predisposti.Font.Underline = true;
                    btn_doc_predisposti.CssClass = "pulsante_hand_4";
                    btn_doc_predisposti.Font.Bold = true;
                    btn_doc_predisposti.BackColor = Color.FromArgb(242, 242, 242);
                    btn_doc_predisposti.ForeColor = Color.Black;
                    btn_doc_predisposti.Click += new System.EventHandler(this.btn_doc_predisposti_Click);
                    btn_doc_predisposti.Text = totPredisposti.ToString();
                    tcell.Controls.Add(btn_doc_predisposti);
                }
                else
                    tcell.Text = totPredisposti.ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                this.tbl_todolist.Rows.Add(triga);
                #endregion

                #region RIGA TOTALE FASCICOLI
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);
                //CELLA VUOTA
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio_scuro";
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "&nbsp;";
                triga.Cells.Add(tcell);
                //CELLA TITOLO FASCICOLI
                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(20);
                tcell.Text = "FASCICOLI";
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);
                //CELLA TOTALI FASCICOLI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Width = Unit.Percentage(15);
                tcell.Font.Bold = true;
                tcell.Text = todolist.trasmFascTot.ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA TOTALI NON LETTI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.HorizontalAlign = HorizontalAlign.Center;
                totNonLetti = todolist.trasmFascNonLetti;
                if (totNonLetti > 0)
                {
                    btn_non_letti = new System.Web.UI.WebControls.Button();
                    btn_non_letti.ID = "btn_Fas_NonLetti" + i;
                    btn_non_letti.BorderWidth = 0;
                    btn_non_letti.Font.Underline = true;
                    btn_non_letti.CssClass = "pulsante_hand_4";
                    btn_non_letti.Font.Bold = true;
                    btn_non_letti.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_letti.ForeColor = Color.Black;
                    btn_non_letti.Click += new System.EventHandler(this.btn_non_letti_Click);
                    btn_non_letti.Text = totNonLetti.ToString();
                    tcell.Controls.Add(btn_non_letti);
                }
                else
                    tcell.Text = todolist.trasmFascNonLetti.ToString();
                triga.Cells.Add(tcell);
                //CELLA FASCICOLI NON ACCETTATI
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                totNonAccettati = todolist.trasmFascNonAccettati;
                if (totNonAccettati > 0)
                {
                    btn_non_accettati = new System.Web.UI.WebControls.Button();
                    btn_non_accettati.ID = "btn_Fas_NonAccettati" + i;
                    btn_non_accettati.BorderWidth = 0;
                    btn_non_accettati.Font.Underline = true;
                    btn_non_accettati.CssClass = "pulsante_hand_4";
                    btn_non_accettati.Font.Bold = true;
                    btn_non_accettati.BackColor = Color.FromArgb(242, 242, 242);
                    btn_non_accettati.ForeColor = Color.Black;
                    btn_non_accettati.Click += new System.EventHandler(this.btn_non_accettati_Click);
                    btn_non_accettati.Text = totNonAccettati.ToString();
                    tcell.Controls.Add(btn_non_accettati);
                }
                else
                    tcell.Text = totNonAccettati.ToString();
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                //CELLA VUOTA
                tcell = new TableCell();
                tcell.CssClass = "testo_grigio";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(15);
                tcell.Text = "---";
                tcell.HorizontalAlign = HorizontalAlign.Center;
                triga.Cells.Add(tcell);
                this.tbl_todolist.Rows.Add(triga);
                #endregion

                //RIGA VUOTA
                tcell = new TableCell();
                tcell.ColumnSpan = 5;
                tcell.CssClass = "testo_grigio_scuro";
                triga = new TableRow();
                tcell.Text = "&nbsp;";
                triga.Cells.Add(tcell);
                this.tbl_todolist.Rows.Add(triga);

                i++;
            }
        }

        protected void btn_non_letti_Click(object sender, EventArgs e)
        {
            string descrBtn = ((System.Web.UI.WebControls.Button)sender).ClientID;
            string nomePulsante = descrBtn.Substring(0, 16);
            int indice = Convert.ToInt32(descrBtn.Substring(16, descrBtn.Length - 16));
            switch (nomePulsante)
            { 
                case "btn_Tra_NonLetti":
                    gestione_pulsanti_NonLetti(indice, "T");
                    break;
                case "btn_Doc_NonLetti":
                    gestione_pulsanti_NonLetti(indice, "D");
                    break;
                case "btn_Fas_NonLetti":
                    gestione_pulsanti_NonLetti(indice, "F");
                    break;
            }
            
        }

        private void gestione_pulsanti_NonLetti(int posizione, string oggetto)
        {
            DocsPAWA.DocsPaWR.AllToDoList todolist = allTodoList[posizione];
            utente = UserManager.getUtente(this);
            DocsPAWA.DocsPaWR.Ruolo[] ruoli = utente.ruoli;
            foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
            {
                if (ruolo.descrizione == todolist.ruoloDesc)
                {
                    Session.Add("TrasmNonViste", oggetto);
                    Session.Remove("TrasmNonAccettate");
                    Session.Remove("TrasmDocPredisposti");
                    returnToSceltaRuolo(ruolo);
                    return;
                }
            }
        }

        protected void btn_non_accettati_Click(object sender, EventArgs e)
        {
            string descrBtn = ((System.Web.UI.WebControls.Button)sender).ClientID;
            string nomePulsante = descrBtn.Substring(0, 20);
            int indice = Convert.ToInt32(descrBtn.Substring(20, descrBtn.Length - 20));
            switch (nomePulsante)
            {
                case "btn_Tra_NonAccettati":
                    gestione_pulsanti_NonAccettati(indice, "T");
                    break;
                case "btn_Doc_NonAccettati":
                    gestione_pulsanti_NonAccettati(indice, "D");
                    break;
                case "btn_Fas_NonAccettati":
                    gestione_pulsanti_NonAccettati(indice, "F");
                    break;
            }

        }

        private void gestione_pulsanti_NonAccettati(int posizione, string oggetto)
        {
            DocsPAWA.DocsPaWR.AllToDoList todolist = allTodoList[posizione];
            utente = UserManager.getUtente(this);
            DocsPAWA.DocsPaWR.Ruolo[] ruoli = utente.ruoli;
            foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
            {
                if (ruolo.descrizione == todolist.ruoloDesc)
                {
                    Session.Add("TrasmNonAccettate", oggetto);
                    Session.Remove("TrasmNonViste");
                    Session.Remove("TrasmDocPredisposti");
                    returnToSceltaRuolo(ruolo);
                    return;
                }
            }
        }

        protected void btn_doc_predisposti_Click(object sender, EventArgs e)
        {
            string descrBtn = ((System.Web.UI.WebControls.Button)sender).ClientID;
            string nomePulsante = descrBtn.Substring(0, 18);
            int indice = Convert.ToInt32(descrBtn.Substring(18, descrBtn.Length - 18));
            switch (nomePulsante)
            {
                case "btn_Tra_DocPredisp":
                    gestione_pulsanti_DocPredisposti(indice);
                    break;
                case "btn_DocPredisposti":
                    gestione_pulsanti_DocPredisposti(indice);
                    break;
            }
        }

        private void gestione_pulsanti_DocPredisposti(int posizione)
        {
            DocsPAWA.DocsPaWR.AllToDoList todolist = allTodoList[posizione];
            utente = UserManager.getUtente(this);
            DocsPAWA.DocsPaWR.Ruolo[] ruoli = utente.ruoli;
            foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
            {
                if (ruolo.descrizione == todolist.ruoloDesc)
                {
                    Session.Add("TrasmDocPredisposti", true);
                    Session.Remove("TrasmNonViste");
                    Session.Remove("TrasmNonAccettate");
                    returnToSceltaRuolo(ruolo);
                    return;
                }
            }
        }

        private void returnToSceltaRuolo(DocsPAWA.DocsPaWR.Ruolo ruolo)
        {
            Session.Add("newRuolo", ruolo);
            string script = "<script>window.returnValue = true; window.close();</script>";
            this.Page.RegisterStartupScript("closelista", script);
            return;                                   
        }

        protected void btn_ruolo_Click(object sender, EventArgs e)
        {
            string descrRuolo = ((System.Web.UI.WebControls.Button)sender).Text;
            if (descrRuolo.Contains("\n"))
                descrRuolo = descrRuolo.Replace("\n", " ");
            utente = UserManager.getUtente(this);
            DocsPAWA.DocsPaWR.Ruolo[] ruoli = utente.ruoli;
            Session.Remove("TrasmNonViste");
            Session.Remove("TrasmNonAccettate");
            Session.Remove("TrasmDocPredisposti");
            
            // Rimozione filtri su ricerca trasmissioni in todolist
            ricercaTrasm.DialogFiltriRicercaTrasmissioni.RemoveCurrentFilters();

            SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

            if (context != null)
            {
                if (context.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE)
                {
                    context.PageNumber = 1;
                }
            }

            foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
            {
                if (ruolo.descrizione == descrRuolo)
                {
                    returnToSceltaRuolo(ruolo);
                    return;
                }
            }

            
        }

       

        private string CalcolaTesto(int numCaratteri, string descRuolo)
        {
            string testoFinale = string.Empty;
            string testoTemp = string.Empty;
            string[] testo = descRuolo.Split(' ');
            int i = 0;
            while (i < testo.Length)
            {
                testoTemp += testo[i] + " ";
                i++;
                while (i < testo.Length && ((testoTemp.Length + testo[i].Length + 1) <= numCaratteri))
                {
                    testoTemp += testo[i] + " ";
                    i++;
                }
                testoTemp = testoTemp.Substring(0, testoTemp.Length - 1);
                testoFinale += testoTemp + "\n";
                testoTemp = string.Empty;
            }
            testoFinale = testoFinale.Substring(0, testoFinale.Length - 1);
            return testoFinale;
        }
    }
}
