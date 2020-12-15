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

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for TabTrasmissioniEff.
    /// </summary>
    public class tabTrasmissioniEffFasc : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Table tblTx;

        //my var
        public int i;
        protected DocsPAWA.DocsPaWR.TrasmissioneSingola[] listTrasmSing;
        protected DocsPAWA.DocsPaWR.Trasmissione trasmSel;
        protected System.Web.UI.WebControls.Label titolo;
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione listRag;
        protected System.Web.UI.WebControls.Button btn_chiudi;

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Utils.startUp(this);
                Page.Response.Expires = 0;
                this.tblTx.Style.Remove("border-collapse");
                if (Session["fascicolo"] != null)
                {
                    string fasc = (string)Session["fascicolo"];
                    if (fasc.ToUpper().Equals("TODOLIST") || fasc.ToUpper().Equals("RICERCA"))
                        this.btn_chiudi.Visible = false;
                    else
                        this.btn_chiudi.Visible = true;
                }
                else
                    this.btn_chiudi.Visible = true;

                string chiudi = Request.QueryString["chiudi"];
                if (!string.IsNullOrEmpty(chiudi))
                {
                    if (chiudi.ToUpper().Equals("SI"))
                        this.btn_chiudi.Visible = true;
                    else
                        this.btn_chiudi.Visible = false;
                }

                //Prendo la trasmissione selezionata dall'utente e ne visualizzo i dettagli

                if (TrasmManager.getDocTrasmSel(this) != null)
                {
                    trasmSel = TrasmManager.getDocTrasmSel(this);

                    //creazione delle righe su Informazioni Generali
                    DrawInfoFasc(tblTx, trasmSel);
                    DrawOggFasc(tblTx, trasmSel);

                    //vanno fatti una sola volta comunque fuori dal ciclo
                    DrawNoteGen(tblTx, trasmSel);

                    //prendo le trasm singole dalla Trasm
                    listTrasmSing = trasmSel.trasmissioniSingole;
                    if (listTrasmSing != null)
                    {
                        for (int g = 0; g < listTrasmSing.Length; g++)
                        {
                            DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listTrasmSing[g];
                            DrawTable(tblTx, trasmSing, g);
                        }
                    }
                }
                else
                {
                    //solo per fargli vis la note gen
                    //				DocsPaWR.Trasmissione trasmVuota= new DocsPAWA.DocsPaWR.Trasmissione();
                    //				trasmVuota.noteGenerali="&nbps;";
                    //				DrawNoteGen(tblTx,trasmVuota);
                }

                TrasmManager.removeDocTrasmSel(this);
                //			TrasmManager.removeDocTrasmQueryEff(this);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void DrawInfoFasc(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();

            tr.BackColor = Color.FromArgb(75, 75, 75);
            tc.CssClass = "titolo_bianco";
            tc.ColumnSpan = 6;
            tc.Text = "Fascicolo";
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(20);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);

            tr = new TableRow();
            tc = new TableCell();
            tc.ColumnSpan = 6;
            tc.CssClass = "testo_grigio";
            tc.BackColor = Color.FromArgb(242, 242, 242);
            tc.Text = trasm.infoFascicolo.codice;

            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(40);

            tr.Cells.Add(tc);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);
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

        protected void DrawOggFasc(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();

            tr.BackColor = Color.FromArgb(75, 75, 75);
            tc.CssClass = "titolo_bianco";
            tc.ColumnSpan = 6;
            tc.Text = "Oggetto";
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(20);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);

            tr = new TableRow();
            tc = new TableCell();
            tc.ColumnSpan = 6;
            tc.CssClass = "testo_grigio";
            tc.BackColor = Color.FromArgb(242, 242, 242);

            string errorMessage;
            
            int result = DocumentManager.verificaACL("F", trasm.infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out errorMessage);
            if (result == 0 || result == 1)
            {
                tc.Text = "Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo trasmesso";
            }
            else
            {
                tc.Text = trasm.infoFascicolo.descrizione;
            }

            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(40);

            tr.Cells.Add(tc);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);
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

        protected void DrawNoteGen(System.Web.UI.WebControls.Table tbl, DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();

            tr.BackColor = Color.FromArgb(75, 75, 75);
            tc.CssClass = "titolo_bianco";
            tc.ColumnSpan = 6;
            tc.Text = "Note Generali";
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(20);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);

            tr = new TableRow();
            tc = new TableCell();
            tc.ColumnSpan = 6;
            tc.CssClass = "testo_grigio";
            tc.BackColor = Color.FromArgb(242, 242, 242);
            tc.Text = trasm.noteGenerali;

            tc.Width = Unit.Percentage(100);
            tc.Height = Unit.Pixel(40);

            tr.Cells.Add(tc);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);
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
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Destinatario";
            tc.Width = Unit.Percentage(20);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Ragione";
            tc.Width = Unit.Percentage(20);
            tc.ColumnSpan = 2;
            tr.Cells.Add(tc);

            //Tipo c'è solo per determinate ragioni
            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Tipo";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Note Individuali";
            tc.Height = Unit.Pixel(15);

            tc.Width = Unit.Percentage(30);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Data scad.";

            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);
        }
        //fine Trasm Singola 
        protected void DrawHeaderTrasmUt(System.Web.UI.WebControls.Table tbl)
        {
            TableRow tr = new TableRow();

            TableCell tc = new TableCell();

            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Utente";
            tc.Width = Unit.Percentage(15);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "data vista";
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            //Tipo c'è solo per determinate ragioni
            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Data acc.";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Data Rif.";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Info Acc. / Info Rif.";
            tc.Height = Unit.Pixel(15);

            tc.Width = Unit.Percentage(30);

            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.CssClass = "titolo_bianco";
            tc.BackColor = Color.FromArgb(149, 149, 149);
            tc.Text = "Data risp.";
            tc.Height = Unit.Pixel(15);
            tc.Width = Unit.Percentage(10);

            tr.Cells.Add(tc);

            tbl.Rows.Add(tr);
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

                bool isOwnerTrasmissione = this.CheckTrasmEffettuataDaUtenteCorrente();
                bool canViewDettTrasmSingola = false;

                //Note Individuali
                tc = new TableCell();
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
                        {
                            tr.CssClass = "testo_grigio";
                        }
                        else
                        {
                            tr.CssClass = "testo_grigio_light";
                        }

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
                        if (!string.IsNullOrEmpty(TrasmUt.dataVista) && TrasmUt.cha_vista_delegato.Equals("0"))
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
                        if (TrasmUt.valida != null && TrasmUt.valida.Equals("0"))
                        {
                            textDataAcc = formatBlankValue(TrasmUt.dataAccettata);
                        }
                        else
                        {
                            textDataAcc = "---";
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

                        string textDataRif;
                        //if (TrasmUt.valida!=null && TrasmUt.valida.Equals("0"))
                        if (TrasmUt.dataRifiutata != null && TrasmUt.dataRifiutata != "")
                        {
                            textDataRif = formatBlankValue(TrasmUt.dataRifiutata);
                        }
                        else
                        {
                            textDataRif = "---";
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
                            //							HyperLink hLink = new HyperLink();
                            //							string nomeHlink="hl_"+i.ToString();
                            //							hLink.CssClass="link";
                            //								
                            ////							DocsPaWR.Trasmissione trasmRisp=TrasmManager.getTrasmRispostadaUt(this,TrasmUt);
                            //								DocsPaWR.Trasmissione trasmRisp=TrasmManager.getTrasmRispostadaSing(this,trasmSing);
                            //							hLink.NavigateUrl="../trasmissione/gestioneTrasm.aspx?"+trasmRisp.systemId+"&tipoTrasm=E";
                            //							//bisogna metterla in session:	
                            //							TrasmManager.setTrasmRispSel(this,trasmRisp);
                            //							//	hLink.NavigateUrl="../trasmissione/gestioneTrasm.aspx?"+""+"&tipoTrasm=E";
                            //							hLink.Text=trasmSing.ragione.descrizione;
                            //							hLink.Target="principale";
                            //							hLink.BackColor=Color.FromArgb(242,242,242);
                            //							hLink.Height=Unit.Pixel(15);
                            //							tc.Controls.Add(hLink);
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
                        if (!string.IsNullOrEmpty(TrasmUt.idPeopleDelegato) && (TrasmUt.cha_accettata_delegato == "1" || TrasmUt.cha_rifiutata_delegato == "1" || TrasmUt.cha_vista_delegato == "1"))
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
                            tc.Text = formatBlankValue(null);
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
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void TblTx_PreRender(object sender, System.EventArgs e)
        {

            this.tblTx.Attributes.Remove("cellspacing");
            this.tblTx.Attributes.Remove("cellpadding");
            this.tblTx.Attributes.Add("cellpadding", "0");
            this.tblTx.Attributes.Add("cellspacing", "0");
            //			this.tblTx.Attributes.Add("style","'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 100%; BORDER-BOTTOM: 1px solid'");
            this.tblTx.Attributes.Add("borderColorDark", "#ffffff");

        }

        /// <summary>
        /// verifica se la trasmissione è stata effettuata 
        /// dall'utente correntemente connesso
        /// </summary>
        /// <returns></returns>
        private bool CheckTrasmEffettuataDaUtenteCorrente()
        {
            bool retValue = false;

            DocsPaWR.Trasmissione trasm = TrasmManager.getDocTrasmSel(this);

            if (trasm != null && trasm.utente != null)
                retValue = (trasm.utente.idPeople.Equals(UserManager.getInfoUtente(this).idPeople));

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
    }
}
