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
using ConservazioneWA.Utils;
using System.Drawing;
using System.Net.Mail;
using Debugger = ConservazioneWA.Utils.Debugger;
using ConservazioneWA.DocsPaWR;



namespace ConservazioneWA.Esibizione
{
    public partial class HomePageEsibizione : System.Web.UI.Page
    {
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        //
        // Prova label della tabella
        Label lbl_IstanzeRichiestaCertificazione = new Label();
        Label lbl_IstanzeCertificate = new Label();

        Label lbl_IstanzeNuove = new Label();
        Label lbl_IstanzeChiuse = new Label();
        Label lbl_IstanzeRifiutate = new Label();
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(this.infoUtente.idAmministrazione);
            // Recupero user che si è loggato
            DocsPaWR.Utente user = (DocsPaWR.Utente)Session["userData"];
            
            InizializzaPagina(user);
            
            //menuTop.ProfiloUtente = CalcolaProfiloUtente();
            //menuTop.ProfiloUtente = ConservazioneManager.CalcolaProfiloUtente(this.infoUtente.idPeople, this.infoUtente.idAmministrazione);
            menuTop.ProfiloUtente = "ESIBIZIONE";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e) 
        {
            //
            // Recupero dalla sessione il ruolo selezionato
            //if (Session["RuoloSelezionato"] != null)
            //if (Request["RuoloSelezionato_idGruppo"] != null)
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
            {
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                string idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                DocsPaWR.Utente user = (DocsPaWR.Utente)Session["userData"];

                if (user != null)
                {
                    //Recupero il ruolo selezionato a partire dall'idGruppo
                    Ruolo ruoloSelected = user.ruoli.FirstOrDefault(item => item.idGruppo == idGruppo);
                    //Ruolo ruoloSelected = (Ruolo)Session["RuoloSelezionato"];

                    if (ruoloSelected != null)
                    {
                        lbl_selectedRole.Text = ruoloSelected.descrizione;
                        this.ddl_ruoli.SelectedValue = ruoloSelected.idGruppo;

                        // Rimetto in sessione il valore dell'idGruppo utilizzato dall'utente
                        HttpContext.Current.Session["RuoloSelezionato_idGruppo"] = ruoloSelected.idGruppo;
                        // Metto in sessione l'idcorrglobali
                        // Metto in sessione l'idcorrglobali
                        HttpContext.Current.Session["RuoloSelezionato_idCorrGlobali"] = Utils.ConservazioneManager.GetIdCorrGlobaliEsibizione(ruoloSelected.idGruppo);
                    }
                }
            }


            
        }

        /// <summary>
        /// Metodo invocato al caricamento della pagina
        /// </summary>
        protected void InizializzaPagina(DocsPaWR.Utente user)
        {
            // Impostazione ruoli utente
            this.CaricaComboRuoli(user);

            // Sono presenti dei ruoli
            if (user != null && user.ruoli.Length > 0)
            {
                // Tabella di Riepilogo
                // inizializzo la tabella di riepilogo delle istanze di esibizione
                this.InizializzaTableRiepilogoIstanze(user);

                if (ddl_ruoli.Visible && ddl_ruoli.Items.Count > 0)
                {
                    // Label Ruolo selezionato
                    this.lbl_selectedRole.Text = this.ddl_ruoli.SelectedItem.Text;
                    if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] == null)
                        // Metto in sessione il ruolo selezionato
                        HttpContext.Current.Session["RuoloSelezionato_idGruppo"] = user.ruoli[this.ddl_ruoli.SelectedIndex].idGruppo;
                        // Metto in sessione l'idcorrglobali
                        HttpContext.Current.Session["RuoloSelezionato_idCorrGlobali"] = Utils.ConservazioneManager.GetIdCorrGlobaliEsibizione(user.ruoli[this.ddl_ruoli.SelectedIndex].idGruppo);
                }
            }

            // Label Amministrazione
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
        }

        /// <summary>
        /// Metodo invocato al caricamento della dropDownList dei ruoli
        /// </summary>
        protected void CaricaComboRuoli(DocsPaWR.Utente user) 
        {
            try
            {
                if (user != null)
                {
                    if (user.ruoli != null && user.ruoli.Length > 0)
                    {
                        // è presente almeno un ruolo
                        for (int i = 0; i < user.ruoli.Length; i++)
                        {
                            ListItem item = new ListItem(user.ruoli[i].descrizione, user.ruoli[i].idGruppo);
                            if(!this.ddl_ruoli.Items.Contains(item))
                                this.ddl_ruoli.Items.Add(item);
                        }
                        this.ddl_ruoli.Visible = true;
                    }
                    else
                    {
                        // Nessun ruolo presente
                        this.ddl_ruoli.Visible = false;
                        this.lbl_ruoli_info.Text = "Nessun ruolo presente per l'utente " + user.userId.ToString();
                        this.lbl_ruoli_info.ForeColor = Color.Red;
                        this.lbl_ruoli_info.Visible = true;
                    }
                }

            }
            catch (Exception e) 
            {
                this.ddl_ruoli.Visible = false;
                this.lbl_ruoli_info.Text = "Non è stato possibile recuperare le informazioni necessarie per l'utente";
                this.lbl_ruoli_info.ForeColor = Color.Red;
                this.lbl_ruoli_info.Visible = true;
            }
        }

        /// <summary>
        /// Metodo che inizializza la tabella di riepilogo delle istanze di esibizione
        /// </summary>
        protected void InizializzaTableRiepilogoIstanze(DocsPaWR.Utente user) 
        {
            // Per ogni roulo, gestisco la tabella
            for (int i = 0; i < user.ruoli.Length; i++) 
            {
                // Chiamata al Be per il conteggio delle istanze di esibizione
                WSConservazioneLocale.ContatoriEsibizione contatori = Utils.ConservazioneManager.GetContatoriEsibizione(this.infoUtente, user.ruoli[i].idGruppo);

                // Variabili per il conteggio istanze di esibizione
                int totIstanzeRichiestaCertificazione = 0;
                int totIstanzeCertificate = 0;

                int totIstanzeNuove = 0;
                int totIstanzeChiuse = 0;
                int totIstanzeRifiutate = 0;


                // Calcolo conteggio istanze di esibizione
                totIstanzeRichiestaCertificazione = contatori.InAttesaDiCertificazione + contatori.InAttesaDiCertificazione_Certificata;
                //totIstanzeCertificate = contatori.Chiuse + contatori.Chiuse_Certificata;
                totIstanzeCertificate = contatori.Chiuse_Certificata;

                //calcolo numero istanze nuove
                //somma campi nuove e nuove_certificata (deve essere sempre 0) del contatore
                //dovrebbe essere o 0 o 1
                totIstanzeNuove = contatori.Nuove + contatori.Nuove_Certificata;

                //calcolo numero istanze chiuse (senza certificazione)
                totIstanzeChiuse = contatori.Chiuse;

                //calcolo numero istanze rifiutate
                //somma campi rifiutate (deve essere sempre 0) e rifiutate_certificata del contatore
                totIstanzeRifiutate = contatori.Rifiutate + contatori.Rifiutate_Certificata;

                // Valori della tabella
                lbl_IstanzeRichiestaCertificazione.Text = totIstanzeRichiestaCertificazione.ToString();
                lbl_IstanzeCertificate.Text = totIstanzeCertificate.ToString();
                lbl_IstanzeNuove.Text = totIstanzeNuove.ToString();
                lbl_IstanzeChiuse.Text = totIstanzeChiuse.ToString();
                lbl_IstanzeRifiutate.Text = totIstanzeRifiutate.ToString();

                // Intestazione:
                // Riga di intestazione
                TableRow tblRowRuolo = new TableRow();
                
                // Cell Intestazione
                TableCell tblCellRuolo = new TableCell();
                tblCellRuolo.CssClass = "tab_riepiologo_sx";

                // Costruisco il corpo della Cell intestazione
                tblCellRuolo.Text = user.ruoli[i].descrizione;
                tblCellRuolo.Font.Bold = true;
                tblCellRuolo.ColumnSpan = 2;

                // Inserisco la cell nella row
                tblRowRuolo.Cells.Add(tblCellRuolo);

                // Aggiungo la riga Intestazione alla table
                this.table_riepilogo.Rows.Add(tblRowRuolo);

                #region NUOVE
                // Riga Istanze Nuove
                // Creo la riga Istanze Nuove
                TableRow tblRow0 = new TableRow();
                tblRow0.CssClass = "RowOverFirstEsib";
                //tblRow0.Attributes["onmouseover"] = "this.className='RowOverSelected';";
                //tblRow0.Attributes["onmouseout"] = "this.className='RowOverFirst';";

                // Creo le celle Istanze Nuove
                // Descrizione e Valore
                TableCell tableCellDescr0 = new TableCell();
                tableCellDescr0.CssClass = "tab_riepiologo_sx";
                TableCell tableCellValue0 = new TableCell();
                tableCellValue0.CssClass = "tab_riepiologo_dx";

                // Impostazione delle cell Descrizione e Valore
                tableCellDescr0.Text = "Nuove";
                tableCellValue0.Text = lbl_IstanzeNuove.Text;

                // Aggiungo le celle alla riga
                tblRow0.Cells.Add(tableCellDescr0);
                tblRow0.Cells.Add(tableCellValue0);

                // Aggiungo la riga Istanze Nuove alla table
                this.table_riepilogo.Rows.Add(tblRow0);
                #endregion

                #region RICHIESTA CERTIFICAZIONE
                // Riga Istanze Richiesta di Certificazione
                // Creo la riga Istanze Richiesta di Certificazione
                TableRow tblRow = new TableRow();
                tblRow.CssClass = "RowOverFirstEsib";
                //tblRow.Attributes["onmouseover"] = "this.className='RowOverSelected';";
                //tblRow.Attributes["onmouseout"] = "this.className='RowOverFirst';";

                // Creo le celle Istanze richiesta certificazione:
                // Descrizione e Valore
                TableCell tableCellDescr = new TableCell();
                tableCellDescr.CssClass = "tab_riepiologo_sx";
                TableCell tableCellValue = new TableCell();
                tableCellValue.CssClass = "tab_riepiologo_dx";

                // Impostazione delle cell Descrizione e Valore
                tableCellDescr.Text = "Richiesta Certificazione";
                tableCellValue.Text = lbl_IstanzeRichiestaCertificazione.Text;

                // Aggiungo le celle alla riga
                tblRow.Cells.Add(tableCellDescr);
                tblRow.Cells.Add(tableCellValue);

                // Aggiungo la riga Istanze Richiesta di Certificazione alla table
                this.table_riepilogo.Rows.Add(tblRow);
                #endregion

                #region CERTIFICATE
                // Riga Istanze Certificate
                // Creo la riga Istanze Certficate
                TableRow tblRow2 = new TableRow();
                tblRow2.CssClass = "RowOverFirstEsib";
                //tblRow2.Attributes["onmouseover"] = "this.className='RowOverSelected';";
                //tblRow2.Attributes["onmouseout"] = "this.className='RowOverFirst';";

                // Creo le celle Descrizione e valore per le Istanze Certificate
                TableCell tableCellDescr2 = new TableCell();
                tableCellDescr2.CssClass = "tab_riepiologo_sx";
                TableCell tableCellValue2 = new TableCell();
                tableCellValue2.CssClass = "tab_riepiologo_dx";

                // impostazione delle cell Descrizione e Valore
                tableCellDescr2.Text = "Certificate";
                tableCellValue2.Text = lbl_IstanzeCertificate.Text;

                // Aggiungo le celle alla riga Istanze Cerificate
                tblRow2.Cells.Add(tableCellDescr2);
                tblRow2.Cells.Add(tableCellValue2);

                // Aggiungo la riga alla table
                this.table_riepilogo.Rows.Add(tblRow2);
                #endregion

                #region CHIUSE
                // Riga Istanze Chiuse
                // Creo la riga Istanze Chiuse
                TableRow tblRow3 = new TableRow();
                tblRow3.CssClass = "RowOverFirstEsib";
                //tblRow3.Attributes["onmouseover"] = "this.className='RowOverSelected';";
                //tblRow3.Attributes["onmouseout"] = "this.className='RowOverFirst';";

                // Creo le celle Descrizione e valore per le Istanze Chiuse
                TableCell tableCellDescr3 = new TableCell();
                tableCellDescr3.CssClass = "tab_riepiologo_sx";
                TableCell tableCellValue3 = new TableCell();
                tableCellValue3.CssClass = "tab_riepiologo_dx";

                // impostazione delle celle Descrizione e Valore
                tableCellDescr3.Text = "Chiuse";
                tableCellValue3.Text = lbl_IstanzeChiuse.Text;

                // Aggiungo le celle alla riga Istanze Chiuse
                tblRow3.Cells.Add(tableCellDescr3);
                tblRow3.Cells.Add(tableCellValue3);

                // Aggiungo la riga alla table
                this.table_riepilogo.Rows.Add(tblRow3);
                #endregion

                #region RIFIUTATE
                // Riga Istanze Rifiutate
                // Creo la riga Istanze Rifiutate
                TableRow tblRow4 = new TableRow();
                tblRow4.CssClass = "RowOverFirstEsib";
                //tblRow4.Attributes["onmouseover"] = "this.className='RowOverSelected';";
                //tblRow4.Attributes["onmouseout"] = "this.className='RowOverFirst';";

                // Creo le celle Descrizione e valore per le Istanze Rifiutate
                TableCell tableCellDescr4 = new TableCell();
                tableCellDescr4.CssClass = "tab_riepiologo_sx";
                TableCell tableCellValue4 = new TableCell();
                tableCellValue4.CssClass = "tab_riepiologo_dx";

                // Impostazione delle celle Descrizione e Valore
                tableCellDescr4.Text = "Rifiutate";
                tableCellValue4.Text = lbl_IstanzeRifiutate.Text;

                // Aggiungo le celle alla riga Istanze Rifiutate
                tblRow4.Cells.Add(tableCellDescr4);
                tblRow4.Cells.Add(tableCellValue4);

                // Aggiungo la riga alla table
                this.table_riepilogo.Rows.Add(tblRow4);
                #endregion

            }
        }

        /// <summary>
        /// Metodo invocato al change dei ruoli nella ddl_ruoli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_ruoli_onSelectedIndexChanged(object sender, EventArgs e) 
        {
            // Recupero dalla sessione l'utente
            DocsPaWR.Utente user = (DocsPaWR.Utente)Session["userData"];

            if (user != null)
            {
                // Metto in sessione il ruolo selezionato
                //Session["RuoloSelezionato"] = user.ruoli[this.ddl_ruoli.SelectedIndex];
                HttpContext.Current.Session["RuoloSelezionato_idGruppo"] = user.ruoli[this.ddl_ruoli.SelectedIndex].idGruppo;
                
                // Metto in sessione l'idcorrglobali
                HttpContext.Current.Session["RuoloSelezionato_idCorrGlobali"] = Utils.ConservazioneManager.GetIdCorrGlobaliEsibizione(user.ruoli[this.ddl_ruoli.SelectedIndex].idGruppo);

                //Request.QueryString["RuoloSelezionato_idGruppo"] = user.ruoli[this.ddl_ruoli.SelectedIndex].idGruppo;
                this.lbl_selectedRole.Text = this.ddl_ruoli.SelectedItem.Text;
            }
        } 
    }
}