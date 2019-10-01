using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;

namespace Amministrazione.Gestione_Organigramma
{
   /// <summary>
   /// Summary description for GestUtenti_inRuoloUO.
   /// </summary>
   public class GestUtenti_inRuoloUO : System.Web.UI.Page
   {
      #region WebControls e variabili

      protected System.Web.UI.WebControls.Button btn_find;
      protected System.Web.UI.WebControls.Label lbl_avviso;
      protected System.Web.UI.WebControls.DataGrid dg_utentiTrovati;
      protected System.Web.UI.WebControls.DataGrid dg_utenti;
      protected System.Web.UI.WebControls.TextBox txt_ricerca;
      protected System.Web.UI.WebControls.DropDownList ddl_ricerca;
      protected System.Web.UI.WebControls.Label lbl_risultatoUtentiRuolo;
      protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
      protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModalLF;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModalUtenteConTrasm;
        protected System.Web.UI.WebControls.Label lbl_percorso;

      private List<ProcessoFirma> processiCoinvolti_U = new List<ProcessoFirma>();
      private List<IstanzaProcessoDiFirma> istazaProcessiCoinvolti_U = new List<IstanzaProcessoDiFirma>();
      private List<string[]> AlertsLF = new List<string[]>();
      private InvalidaPassiCorrelatiDelegate invalidaPassiCorrelati;

      //---------------------------------------------------------------------------------
      protected DataSet dsUtenti;
        #endregion

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
         this.ddl_ricerca.SelectedIndexChanged += new System.EventHandler(this.ddl_ricerca_SelectedIndexChanged);
         this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
         this.dg_utentiTrovati.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_utentiTrovati_ItemCommand);
         this.dg_utentiTrovati.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_utentiTrovati_PageIndexChanged);
         this.dg_utentiTrovati.PreRender += new System.EventHandler(this.dg_utentiTrovati_PreRender);
         //this.dg_utenti.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_utenti_ItemCreated);
         this.dg_utenti.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_utenti_ItemCommand);
         this.Load += new System.EventHandler(this.Page_Load);
         this.PreRender += new System.EventHandler(this.GestUtenti_inRuoloUO_PreRender);
          
      }
      #endregion

      #region Page Load
      private void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            Response.Expires = -1;

            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
               Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
               Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

            this.SetDefaultBtn_Ricerca();

            if (!IsPostBack)
            {
               this.lbl_percorso.Text = "&nbsp;Gestione utenti: " + Server.UrlDecode(Request.QueryString["percorso"].ToString().Replace("|@ap@|", "'"));
               this.ddl_ricerca.SelectedIndex = 2;
               this.Inizialize();
            }
            else
            {
               // gestione del valore di ritorno della modal Dialog 
               if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
               {
                  this.GestRitornoAvviso(this.hd_returnValueModal.Value);
               }

                if (this.hd_returnValueModalLF.Value != null && this.hd_returnValueModalLF.Value != string.Empty && this.hd_returnValueModalLF.Value != "undefined")
                {
                    this.GestRitornoAvvisoLF(this.hd_returnValueModalLF.Value);
                }
                if (this.hd_returnValueModalUtenteConTrasm.Value != null && this.hd_returnValueModalUtenteConTrasm.Value != string.Empty && this.hd_returnValueModalUtenteConTrasm.Value != "undefined")
                {
                    this.GestRitornoAvvisoUtenteConTrasm(this.hd_returnValueModalUtenteConTrasm.Value);
                }
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

      private void SetCoinvoltoInLibroFirma(string idPeople, string idRuolo, string nomeUtente)
      {
        string retVal = string.Empty;

        string passivi = string.Empty;
        string attivi = string.Empty;
        
        DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        int countProcessiCoinvolti_U = wws.GetCountProcessiDiFirmaByUtenteTitolare(idPeople, idRuolo);
        if (countProcessiCoinvolti_U > 0)
        {
            passivi = (
                (countProcessiCoinvolti_U > 1) ?
                "Il soggetto in modifica è coinvolto in " + countProcessiCoinvolti_U + " processi di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tali processi." :
                "Il soggetto in modifica è coinvolto in un processo di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.");
       
            int countIstazaProcessiCoinvolti_U = wws.GetCountIstanzeProcessiDiFirmaByUtenteCoinvolto(idPeople, idRuolo);
            if (countIstazaProcessiCoinvolti_U > 0)
            {
                attivi = (
                  (countIstazaProcessiCoinvolti_U > 1) ?
                  "l soggetto in modifica è coinvolto in " + countIstazaProcessiCoinvolti_U + " processi di firma avviati e non ancora conclusi. Proseguendo nella modifica, tutti i processi coivolti saranno interrotti." :
                  "l soggetto in modifica è coinvolto in un processo di firma avviato e non ancora concluso. Proseguendo nella modifica, il processo coinvolto sarà interrotto.");
            }
        }

        retVal = (!string.IsNullOrEmpty(passivi) && !string.IsNullOrEmpty(attivi) ? passivi + "<br /> Inoltre i" + attivi : (
            !string.IsNullOrEmpty(passivi) ? passivi : (!string.IsNullOrEmpty(attivi) ? "I" + attivi : "")));
        if (!string.IsNullOrEmpty(retVal))
            retVal = retVal + "<br />Importante avvisare il disegnatore dei processi coinvolti e il proponente.<br />Sei sicuro di voler rimuovere " + nomeUtente ;
        string[] itemToAdd = new string[] { (string.IsNullOrEmpty(retVal)?"":idPeople), retVal };
        AlertsLF.Add(itemToAdd);
      }

      /// <summary>
      /// 
      /// </summary>
      private void Inizialize()
      {
            this.RemoveSessionSostUtente();
            this.RemoveSessionSostUtenteLF();
            this.RemoveSessionInterrompiProcessi();
            this.LoadUtentiAttuali();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="ctrl"></param>
      private void SetFocus(System.Web.UI.Control ctrl)
      {
         string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
         RegisterStartupScript("focus", s);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void GestUtenti_inRuoloUO_PreRender(object sender, System.EventArgs e)
      {
         try
         {
            if (this.dg_utentiTrovati.Items.Count > 0)
            {
               foreach (DataGridItem item in this.dg_utentiTrovati.Items)
               {
                  item.Cells[5].Attributes.Add("onclick", "window.document.body.style.cursor='wait'; void(0);");
               }
            }
         }
         catch
         {
            this.gestErrori();
         }
      }
      #endregion

      #region Utenti attualmente presenti nel ruolo

      private void LoadUtentiAttuali()
      {
         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

            theManager.ListaUtenti(Request.QueryString["idGruppo"]);

            if (theManager.getListaUtenti() != null && theManager.getListaUtenti().Count > 0)
            {

               this.dg_utenti.Visible = true;
                if (this.GetSessionSostUtenteLF() == null || this.GetSessionSostUtenteLF() == string.Empty)
                {
                    this.lbl_risultatoUtentiRuolo.Visible = false;
                }

               this.InitializeDataSetUtenti();

               DataRow row;
               int intCounter = 1;
               Session["ListaMessaggiLibroFirma"] = null;
               AlertsLF.Clear();
               List<DocsPAWA.DocsPaWR.OrgUtente> listaUtenti = theManager.getListaUtenti().Cast<DocsPAWA.DocsPaWR.OrgUtente>().ToList();
               if (listaUtenti != null && listaUtenti.Count > 0)
               {
                   listaUtenti = (from u in listaUtenti orderby (u.Cognome + " " + u.Nome) ascending select u).ToList();
               }
               foreach (DocsPAWA.DocsPaWR.OrgUtente utente in listaUtenti)
               {
                  row = dsUtenti.Tables[0].NewRow();
                  row["IDUtente"] = utente.IDCorrGlobale;
                  row["codice"] = utente.CodiceRubrica;
                  row["descrizione"] = utente.Cognome + " " + utente.Nome;
                  row["IDAmministrazione"] = utente.IDAmministrazione;
                  row["IDPeople"] = utente.IDPeople;

                  dsUtenti.Tables["UTENTI"].Rows.Add(row);

                  //SetCoinvoltoInLibroFirma(utente.IDPeople, Request.QueryString["idGruppo"], utente.Cognome + " " + utente.Nome);
                  //if (intCounter == theManager.getListaUtenti().Count)
                  //      Session["ListaMessaggiLibroFirma"] = AlertsLF;

                  intCounter += 1;
               }

               DataView dv = dsUtenti.Tables["UTENTI"].DefaultView;
               dv.Sort = "descrizione ASC";
               dg_utenti.DataSource = dv;
               dg_utenti.DataBind();
            }
            else
            {
               this.dg_utenti.Visible = false;
               this.lbl_risultatoUtentiRuolo.Visible = true;
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

      private void InitializeDataSetUtenti()
      {
         dsUtenti = new DataSet();
         DataColumn dc;
         dsUtenti.Tables.Add("UTENTI");
         dc = new DataColumn("IDUtente");
         dsUtenti.Tables["UTENTI"].Columns.Add(dc);
         dc = new DataColumn("codice");
         dsUtenti.Tables["UTENTI"].Columns.Add(dc);
         dc = new DataColumn("descrizione");
         dsUtenti.Tables["UTENTI"].Columns.Add(dc);
         dc = new DataColumn("IDAmministrazione");
         dsUtenti.Tables["UTENTI"].Columns.Add(dc);
         dc = new DataColumn("IDPeople");
         dsUtenti.Tables["UTENTI"].Columns.Add(dc);
      }

        protected void dg_utenti_ItemDataBaund(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType.Equals(ListItemType.Item) || e.Item.ItemType.Equals(ListItemType.AlternatingItem))
            {
                /*
                string idUtente = e.Item.Cells[4].Text;
                string idRuolo = Request.QueryString["idGruppo"].ToString();
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                int countProcessiCoinvolti_U = wws.GetCountProcessiDiFirmaByUtenteTitolare(idUtente, idRuolo);
                int countIstazaProcessiCoinvolti_U = wws.GetCountIstanzeProcessiDiFirmaByUtenteCoinvolto(idUtente, idRuolo);
                if (countProcessiCoinvolti_U > 0 || countIstazaProcessiCoinvolti_U > 0)
                {
                    e.Item.Cells[5].Attributes.Add("onclick", "AvvisoRuoloConLF('" + idRuolo + "','" + idUtente + "','" + countProcessiCoinvolti_U + "','" + countIstazaProcessiCoinvolti_U + "');");
                }
                else
                {
                    string utente = e.Item.Cells[2].Text;
                    string alert = string.Empty;
                    alert = "if (!window.confirm('Sei sicuro di voler rimuovere " + utente + " ?')) {return false};";
                    e.Item.Cells[5].Attributes.Add("onclick", alert);
                }
                */
                string utente = e.Item.Cells[2].Text;
                string alert = string.Empty;
                alert = "if (!window.confirm('Sei sicuro di voler rimuovere " + utente + " ?')) {return false};";
                e.Item.Cells[5].Attributes.Add("onclick", alert);
                e.Item.Cells[6].Visible = false;
                if (this.GetSessionSostUtenteLF() != null && this.GetSessionSostUtenteLF() != string.Empty)
                {
                    e.Item.Cells[5].Visible = false;
                    e.Item.Cells[6].Visible = true;
                    string idPeopleEliminato = this.GetSessionSostUtenteLF();
                    if (e.Item.Cells[4].Text.Equals(idPeopleEliminato))
                        e.Item.Visible = false;
                }
            }
        }

        #endregion

        #region Ricerca utenti

        private void ddl_ricerca_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (this.ddl_ricerca.SelectedValue.ToString().Equals("*"))
         {
            this.txt_ricerca.Enabled = false;
            this.txt_ricerca.Text = "";
            this.txt_ricerca.BackColor = System.Drawing.Color.LightGray;
         }
         else
         {
            this.txt_ricerca.Enabled = true;
            this.txt_ricerca.BackColor = System.Drawing.Color.White;
         }
      }

      private void btn_find_Click(object sender, System.EventArgs e)
      {
         this.RicercaUtenti();
      }

      private void RicercaUtenti()
      {
         try
         {
            string ricerca = this.txt_ricerca.Text.Trim();

            if ((ricerca != null && ricerca != string.Empty && ricerca != "") || (this.ddl_ricerca.SelectedValue.ToString().Equals("*")))
            {
               string listaDaEscludere = string.Empty;

               // prende le IDUtente da escludere nella ricerca
               if (this.dg_utenti.Items.Count > 0)
               {
                  foreach (DataGridItem item in this.dg_utenti.Items)
                  {
                     listaDaEscludere += "," + item.Cells[0].Text;
                  }
                  listaDaEscludere = "(" + listaDaEscludere.Substring(1, listaDaEscludere.Length - 1) + ")";
               }

               Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
               theManager.ListaUtenti(Request.QueryString["idAmm"], this.ddl_ricerca.SelectedValue.ToString(), ricerca, listaDaEscludere);

               if (theManager.getListaUtenti() != null && theManager.getListaUtenti().Count > 0)
               {
                  this.dg_utentiTrovati.Visible = true;
                  this.lbl_avviso.Text = "";

                  this.InitializeDataSetUtenti();

                  DataRow row;
                  foreach (DocsPAWA.DocsPaWR.OrgUtente utente in theManager.getListaUtenti())
                  {
                     row = dsUtenti.Tables[0].NewRow();
                     row["IDUtente"] = utente.IDCorrGlobale;
                     row["codice"] = utente.CodiceRubrica;
                     row["descrizione"] = utente.Cognome + " " + utente.Nome;
                     row["IDAmministrazione"] = utente.IDAmministrazione;
                     row["IDPeople"] = utente.IDPeople;

                     dsUtenti.Tables["UTENTI"].Rows.Add(row);
                  }

                  // Impostazione dataset in sessione
                  this.SetSessionDataSetUtenti(dsUtenti);
                  dg_utentiTrovati.CurrentPageIndex = 0;
                  DataView dv = dsUtenti.Tables["UTENTI"].DefaultView;
                  dv.Sort = "descrizione ASC";
                  this.dg_utentiTrovati.DataSource = dv;
                  this.dg_utentiTrovati.DataBind();
               }
               else
               {
                  this.dg_utentiTrovati.Visible = false;
                  this.lbl_avviso.Text = "Nessun dato trovato.";
               }
            }
         }
         catch
         {

            this.gestErrori();
         }
      }

      private void dg_utentiTrovati_PreRender(object sender, System.EventArgs e)
      {
         for (int i = 0; i < this.dg_utentiTrovati.Items.Count; i++)
         {

            if (this.dg_utentiTrovati.Items[i].ItemIndex >= 0)
            {
               switch (this.dg_utentiTrovati.Items[i].ItemType.ToString().Trim())
               {
                  case "Item":
                     this.dg_utentiTrovati.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                     this.dg_utentiTrovati.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                     break;

                  case "AlternatingItem":
                     this.dg_utentiTrovati.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                     this.dg_utentiTrovati.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                     break;
               }
            }
         }
      }

      /// <summary>
      /// Imposta il tasto di default che permette la Post al server in merito alla sezione RICERCA
      /// </summary>
      private void SetDefaultBtn_Ricerca()
      {
         DocsPAWA.Utils.DefaultButton(this, ref this.txt_ricerca, ref this.btn_find);
      }
      #endregion

      #region Inserimento nuovo utente

      private void dg_utentiTrovati_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
      {
         try
         {
            this.dg_utentiTrovati.CurrentPageIndex = e.NewPageIndex;

            DataSet ds = this.GetSessionDataSetUtenti();
            DataView dv = ds.Tables["UTENTI"].DefaultView;
            dv.Sort = "descrizione ASC";
            this.dg_utentiTrovati.DataSource = dv;
            this.dg_utentiTrovati.DataBind();
         }
         catch
         {
            this.gestErrori();
         }
      }

      private void dg_utentiTrovati_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {
         try
         {
            if (e.CommandName.Equals("Inserimento"))
            {
               // gestione del inserimento obbligatorio di un nuovo utente dopo l'eliminazione dell'ultimo utente del ruolo
               if (this.GetSessionSostUtente() != null && this.GetSessionSostUtente() != string.Empty)
               {
                  if (this.SostituzioneUtente(e.Item.Cells[4].Text))
                  {
                    string idRuolo = Request.QueryString["idGruppo"];
                    SostituisciUtentePassiCorrelati(idRuolo, this.dg_utenti.Items[0].Cells[4].Text, e.Item.Cells[4].Text);
                     // elimina l'unico utente presente nella datagrid per questo ruolo
                     if (this.EliminaUtenteInRuolo(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idGruppo"]))
                     {
                        // ripulisce l'AREA DI LAVORO
                        if (this.EliminaADLUtente(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                        {
                           this.RemoveSessionSostUtente();
                        }
                     }
                     this.RemoveSessionInterrompiProcessi();
                  }
               }

               if (this.InserimentoUtente(e.Item.Cells[4].Text, Request.QueryString["idGruppo"]))
               {
                  if (this.InsTrasmUtente(e.Item.Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                  {
                     this.InitializeDataSetUtenti();

                     dsUtenti = this.GetSessionDataSetUtenti();

                     this.LoadRicercaDopoIns(dsUtenti, e.Item.Cells[0].Text);

                     this.LoadUtentiAttuali();
                  }
               }
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

      private void LoadRicercaDopoIns(DataSet dsUtenti, string IDeliminato)
      {
         try
         {
            foreach (DataRow row in dsUtenti.Tables["UTENTI"].Rows)
            {
               if (row["IDUtente"].Equals(IDeliminato))
               {
                  row.Delete();
                  break;
               }
            }

            if (dsUtenti.Tables["UTENTI"].Rows.Count > 0)
            {
               if (this.dg_utentiTrovati.Items.Count == 1)
                  this.dg_utentiTrovati.CurrentPageIndex -= 1;

               DataView dv = dsUtenti.Tables["UTENTI"].DefaultView;
               dv.Sort = "descrizione ASC";
               this.dg_utentiTrovati.DataSource = dv;
               this.dg_utentiTrovati.DataBind();

               this.SetSessionDataSetUtenti(dsUtenti);
            }
            else
            {
               this.dg_utentiTrovati.Visible = false;
               //this.txt_ricerca.Text = "";
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

      private bool InserimentoUtente(string idPeople, string idGruppo)
      {
         bool result = false;

         try
         {

            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            theManager.InsUtenteInRuolo(idPeople, idGruppo, idAmm, "newUser");

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  this.ClientScript.RegisterStartupScript(this.GetType(), "alertJavaScript", scriptString);
                  //this.Page.RegisterStartupScript("alertJavaScript", scriptString);
               }
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }

         return result;
      }

      private bool InsTrasmUtente(string idPeople, string idCorrGlobRuolo)
      {
         bool result = false;

         try
         {

            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.InsTrasmUtente(idPeople, idCorrGlobRuolo);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  //this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                  this.ClientScript.RegisterStartupScript(this.GetType(), "alertJavaScript", scriptString);
               }
            }

            esito = null;

         }
         catch
         {
            this.gestErrori();
         }
         return result;
      }

      private bool SostituzioneUtente(string idPeopleNewUT)
      {
         bool result = false;

         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.SostituzioneUtente(idPeopleNewUT, Request.QueryString["idCorrGlobRuolo"]);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  this.Page.RegisterStartupScript("alertJavaScript", scriptString);
               }
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }

         return result;
      }

      #endregion

      #region Gestione del risultato della ricerca utenti in sessione

      private void SetSessionDataSetUtenti(DataSet dsUtenti)
      {
         Session["UTENTIDATASET"] = dsUtenti;
      }

      private DataSet GetSessionDataSetUtenti()
      {
         return (DataSet)Session["UTENTIDATASET"];
      }

      private void RemoveSessionDataSetUtenti()
      {
         this.GetSessionDataSetUtenti().Dispose();
         Session.Remove("UTENTIDATASET");
      }

      #endregion

      #region Gestione sessione inserimento obbligatorio di un nuovo utente per sostituire l'ultimo utente eliminato

      private void SetSessionSostUtente(string idPeople)
      {
         Session["UTENTEELIMINATO"] = idPeople;
      }

      private string GetSessionSostUtente()
      {
         return (string)Session["UTENTEELIMINATO"];
      }

      private void RemoveSessionSostUtente()
      {
         Session.Remove("UTENTEELIMINATO");
      }

        #endregion

        #region Gestione sessione sostituzione obbligatoria di un utente per libro firma

        private void SetSessionSostUtenteLF(string idPeople)
        {
            Session["UTENTEELIMINATOLF"] = idPeople;
        }

        private string GetSessionSostUtenteLF()
        {
            return (string)Session["UTENTEELIMINATOLF"];
        }

        private void RemoveSessionSostUtenteLF()
        {
            Session.Remove("UTENTEELIMINATOLF");
        }

        private void SetSessionInterrompiProcessi(bool interrompi)
        {
            Session["INTERROMPIPROCESSILFU"] = interrompi;
        }

        private bool GetSessionInterrompiProcessi()
        {
            if (Session["INTERROMPIPROCESSILFU"] == null)
                return false;
            else
                return (bool)Session["INTERROMPIPROCESSILFU"];
        }

        private void RemoveSessionInterrompiProcessi()
        {
            Session.Remove("INTERROMPIPROCESSILFU");
        }
        #endregion
        #region Elimina utente

        private void dg_utenti_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {
            try
            {
                this.RemoveSessionInterrompiProcessi();
                if (e.CommandName.Equals("Eliminazione"))
                {
                    // verifica che l'utente non sia connesso a docspa
                    if (this.VerificaUtenteLoggato(e.Item.Cells[1].Text, e.Item.Cells[3].Text))
                    {
                        // INC000000577348
                        // verifica che l'utente non sia responsabile di una stampa di repertorio
                        if (this.VerificaUtenteRespStampeRep(e.Item.Cells[4].Text, Request.QueryString["idGruppo"], e.Item.Cells[3].Text))
                        {
                            // verifica che l'utente non sia configurato come responsabile della conservazione
                            if (this.VerificaUtenteRespCons(e.Item.Cells[4].Text, Request.QueryString["idGruppo"], e.Item.Cells[3].Text))
                            {
                                if (VerificaPresenzaProcessiFirma(e.Item.Cells[4].Text, Request.QueryString["idGruppo"]))
                                {
                                    Session["indexUser"] = e.Item.ItemIndex;
                                    return;
                                }

                                // verifica che non sia l'unico del ruolo
                                if (this.dg_utenti.Items.Count > 1)
                                {
                                    if(VerificaPresenzaTrasmissioniPendentiUtente(e.Item.Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                    {
                                        Session["indexUser"] = e.Item.ItemIndex;
                                        return;
                                    }
                                    // NON è l'unico utente nel ruolo quindi lo elimina dal ruolo
                                    if (this.EliminaUtenteInRuolo(e.Item.Cells[4].Text, Request.QueryString["idGruppo"]))
                                    {
                                        // ripulisce l'AREA DI LAVORO
                                        if (this.EliminaADLUtente(e.Item.Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                        {
                                            this.LoadUtentiAttuali();
                                        }

                                        // ricarica la ricerca
                                        this.RicercaUtenti();
                                    }
                                }
                                else
                                {
                                    // è l'unico utente del ruolo...
                                    // verifica che il ruolo non abbia trasmissioni con work-flow
                                    if (this.RuoloConTrasmissioni(Request.QueryString["idCorrGlobRuolo"]))
                                    {
                                        // ruolo con trasmissioni... avvisa l'amministratore e apre una modal Dialog
                                        if (!this.Page.IsStartupScriptRegistered("openModalRuoloConTX"))
                                        {
                                            string scriptString = "<SCRIPT>AvvisoRuoloConTX('" + e.Item.Cells[2].Text.Replace("'", "\\'") + "','" + e.Item.Cells[4].Text + "','" + Request.QueryString["idCorrGlobRuolo"] + "');</SCRIPT>";
                                            this.Page.RegisterStartupScript("openModalRuoloConTX", scriptString);
                                        }
                                    }
                                    else
                                    {
                                        // elimina utente dal ruolo
                                        if (this.EliminaUtenteInRuolo(e.Item.Cells[4].Text, Request.QueryString["idGruppo"]))
                                        {
                                            // ripulisce l'AREA DI LAVORO
                                            if (this.EliminaADLUtente(e.Item.Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                            {

                                                // ripulisce il datagrid
                                                this.InitializeDataSetUtenti();

                                                DataView dv = dsUtenti.Tables["UTENTI"].DefaultView;
                                                dv.Sort = "descrizione ASC";
                                                dg_utenti.DataSource = dv;
                                                dg_utenti.DataBind();

                                                this.dg_utenti.Visible = false;
                                                this.lbl_risultatoUtentiRuolo.Visible = true;

                                                // ricarica la ricerca
                                                this.RicercaUtenti();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (e.CommandName.Equals("Sostituzione"))
                {
                    string idOldPeople = this.GetSessionSostUtenteLF();
                    string idNewPeople = e.Item.Cells[4].Text;
                    string idRuolo = Request.QueryString["idGruppo"];

                    if (SostituisciUtentePassiCorrelati(idRuolo, idOldPeople, idNewPeople))
                    {
                        if (this.EliminaUtenteInRuolo(idOldPeople, Request.QueryString["idGruppo"]))
                        {
                           this.EliminaADLUtente(idOldPeople, Request.QueryString["idCorrGlobRuolo"]);
                            this.RemoveSessionSostUtenteLF();
                            this.LoadUtentiAttuali();
                        }
                    }
                }
            }
            catch
            {
                this.gestErrori();
            }
      }

        private bool SostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;

            try
            {
                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                result = theManager.SostituisciUtentePassiCorrelati(idRuolo, idOldPeople, idNewPeople);

                if (!result)
                {
                    string scriptString = "<SCRIPT>alert('Attenzione, errore durante la sostituzione dell'utente');</SCRIPT>";
                    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                }
            }
            catch
            {
                this.gestErrori();
            }

            return result;
        }
        
      private bool VerificaUtenteLoggato(string userId, string idAmm)
      {
         bool result = false;

         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.VerificaUtenteLoggato(userId, idAmm);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  this.Page.RegisterStartupScript("alertJavaScript", scriptString);
               }
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }

         return result;
      }

      private bool VerificaUtenteRespStampeRep(string userId, string roleId, string idAmm)
      {
          bool result = false;

          try
          {
              Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
              theManager.VerificaUtenteRespStampeRep(userId, roleId, idAmm);

              DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
              esito = theManager.getEsitoOperazione();

              if (esito.Codice.Equals(0))
              {
                  result = true;
              }
              else
              {
                  if (!this.Page.IsStartupScriptRegistered("alertStampaRep"))
                  {
                      string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                      this.Page.RegisterStartupScript("alertStampaRep", scriptString);
                  }
              }
          }
          catch
          {
              this.gestErrori();
          }

          return result;
      }

      private bool VerificaUtenteRespCons(string userId, string roleId, string idAmm)
      {
          bool result = false;

          try
          {
              DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
              string userIdRespCons = wws.GetIdUtenteRespConservazione(idAmm);
              string roleIdRespCons = wws.GetIdRuoloRespConservazione(idAmm);
              if (userId.Equals(userIdRespCons) && roleId.Equals(roleIdRespCons))
              {
                  if (!this.Page.IsStartupScriptRegistered("alertRespCons")) ;
                  string scriptString = "<SCRIPT>alert('Attenzione, non è possibile rimuovere l\\'utente dal ruolo in quanto configurato come responsabile della conservazione');</SCRIPT>";
                  this.Page.RegisterStartupScript("alertRespCons", scriptString);
              }
              else
              {
                  result = true;
              }
          }
          catch
          {
              this.gestErrori();
          }
          return result;
      }

        private bool VerificaPresenzaTrasmissioniPendentiUtente(string idPeople, string idCorrGlobali)
        {
            bool result = false;
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            try
            {
                if(ws.VerificaPresenzaTrasmissioniPendentiUtente(idPeople, idCorrGlobali))
                {
                    result = true;
                    string scriptString = "<SCRIPT>AvvisoUtenteConTrasm('" + idPeople + "','" + idCorrGlobali + "');</SCRIPT>";
                    this.Page.RegisterStartupScript("AvvisoUtenteConTrasm", scriptString);
                }
            }
            catch(Exception e)
            {
                this.gestErrori();
            }
            return result;
        }

        private bool VerificaPresenzaProcessiFirma(string idPeople, string idGruppo)
        {
            bool result = false;
            //Verifico che non ci siano processi attivi
            string idUtente;
            string idRuolo = Request.QueryString["idGruppo"].ToString();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            int countProcessiCoinvolti = 0;
            int countIstazaProcessiCoinvolti = 0;
            string tipoTitolare = string.Empty;

            idUtente = idPeople;
            countProcessiCoinvolti = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, idUtente);
            countIstazaProcessiCoinvolti = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, idUtente);
            if (countProcessiCoinvolti > 0 || countIstazaProcessiCoinvolti > 0)
                tipoTitolare = Manager.OrganigrammaManager.SoggettoInModifica.UTENTE;

            if (this.dg_utenti.Items.Count == 1)
            {
                int countProcessiUtente = countProcessiCoinvolti;
                int countIstanzeUtente = countIstazaProcessiCoinvolti;
                countProcessiCoinvolti = ws.GetCountProcessiDiFirmaByTitolare(idRuolo, "");
                countIstazaProcessiCoinvolti = ws.GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, "");
                
                //Se i processi a ruolo sono maggiori rispetto a quelli utente allora ci sono processi in cui è coinvolto il ruolo senza alcun utente
                if (countProcessiCoinvolti > 0 || countIstazaProcessiCoinvolti > 0)
                {
                    if(countProcessiCoinvolti > countProcessiUtente || countIstazaProcessiCoinvolti > countIstanzeUtente)
                        tipoTitolare = !string.IsNullOrEmpty(tipoTitolare) ? Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_E_RUOLO : 
                            Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_RUOLO;
                }
            }
            if (countProcessiCoinvolti > 0 || countIstazaProcessiCoinvolti > 0)
            {
                string scriptString = "<SCRIPT>AvvisoRuoloConLF('" + tipoTitolare + "','" + idRuolo + "','" + idUtente + "','" + countProcessiCoinvolti + "','" + countIstazaProcessiCoinvolti + "');</SCRIPT>";
                this.Page.RegisterStartupScript("AvvisoRuoloConLF", scriptString);
                return true;
            }
            return result;
        }

      private bool EliminaUtenteInRuolo(string idPeople, string idGruppo)
      {
         bool result = false;

         try
         {
            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            ArrayList listaAOO = new ArrayList();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            if (this.IsResponsabileAOO(idPeople, idGruppo))
            {
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                string[] lista = theManager.getAmmRespAOO(idPeople);
                string l = string.Empty;
                for (int i = 0; i < lista.Length; i++)
                {
                    l +="\\n" + lista[i];
                }
                l += "\\n";
                string scriptString = "<SCRIPT>alert('Attenzione, utente responsabile della Interoperabilità tra AOO per i seguenti registri: "+ l +"Per rimuoverlo cambiare il responsabile dei registri riportati.');</SCRIPT>";
               this.Page.RegisterStartupScript("alertJavaScript", scriptString);
            }
            else
            {

               Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

               theManager.EliminaUtenteInRuolo(idPeople, idGruppo, idAmm);

               esito = theManager.getEsitoOperazione();

               if (esito.Codice.Equals(0))
               {
                  result = true;
                  bool interrompi = this.GetSessionInterrompiProcessi();
                    if (interrompi)
                    {
                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                        //InvalidaPassiCorrelati(idPeople);
                        AsyncCallback callback = new AsyncCallback(CallBack);
                        invalidaPassiCorrelati = new InvalidaPassiCorrelatiDelegate(InvalidaPassiCorrelati);
                        invalidaPassiCorrelati.BeginInvoke(idPeople, idGruppo, sessionManager.getUserAmmSession(), callback, null);
                    }
               }
               else
               {
                   string scriptString = string.Empty;

                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        switch (esito.Codice)
                        {
                            case 1: // errore generico
                                scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                break;
                            case 9: // nota
                                scriptString = "<SCRIPT>alert('NOTA, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                                result = true;
                                break;
                        }   
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }
               }
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }
         return result;
      }

      private void InvalidaPassiCorrelati(string idPeople, string idRuolo, InfoUtenteAmministratore infoAmm)
      {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            string tipoTick = "U";
            if (this.dg_utenti.Items.Count == 1)
            {
                idPeople = string.Empty;
                tipoTick = "R";
            }
            wws.InvalidaPassiCorrelatiTitolare(idRuolo, idPeople, tipoTick, infoAmm);
      }

      public delegate void InvalidaPassiCorrelatiDelegate(string idPeople, string idGruppo, InfoUtenteAmministratore infoAmm);

      private void CallBack(IAsyncResult result)
      {
          invalidaPassiCorrelati.EndInvoke(result);
      }

      /// <summary>
      /// Verifica se l'utente è responsabile di una AOO
      /// </summary>
      /// <param name="idPeople"></param>
      /// <returns></returns>
      private bool IsResponsabileAOO(string idPeople, string idGruppo)
      {
         Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
         return theManager.VerificaSeRespAOO(idPeople, idGruppo);
      }
      private bool EliminaADLUtente(string idPeople, string idCorrGlobGruppo)
      {
         bool result = false;

         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.EliminaADLUtente(idPeople, idCorrGlobGruppo);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  this.Page.RegisterStartupScript("alertJavaScript", scriptString);
               }
            }
            esito = null;
         }
         catch
         {
            this.gestErrori();
         }

         return result;
      }

      private bool RuoloConTrasmissioni(string idCorrGlobRuolo)
      {
         bool result = true;

         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.VerificaTrasmRuolo(idCorrGlobRuolo);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = false;
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }
         return result;
      }

      private bool RifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
      {
         bool result = false;

         try
         {

            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.RifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);

            DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            esito = theManager.getEsitoOperazione();

            if (esito.Codice.Equals(0))
            {
               result = true;
            }
            else
            {
               if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
               {
                  string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                  this.Page.RegisterStartupScript("alertJavaScript", scriptString);
               }
            }

            esito = null;
         }
         catch
         {
            this.gestErrori();
         }

         return result;
      }

        private bool AccettaTrasmConWF(string idCorrGlobRuolo)
        {
            bool result = false;

            try
            {

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.AccettaTrasmConWF(idCorrGlobRuolo);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(0))
                {
                    result = true;
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }
                }

                esito = null;
            }
            catch
            {
                this.gestErrori();
            }

            return result;
        }

        private bool AccettaTrasmConWFUtente(string idPeople, string idCorrGlobaliRuolo)
        {
            bool result = false;

            try
            {

                Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.AccettaTrasmConWFUtente(idPeople, idCorrGlobaliRuolo);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(0))
                {
                    result = true;
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "\\'") + "');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }
                }

                esito = null;
            }
            catch
            {
                this.gestErrori();
            }

            return result;
        }

        private void GestRitornoAvviso(string valore)
      {
         try
         {
            switch (valore)
            {
               case "Y":
                  this.dg_utenti.Visible = false;
                  this.lbl_risultatoUtentiRuolo.Visible = true;

                  this.SetSessionSostUtente(this.dg_utenti.Items[0].Cells[4].Text);

                  string vecchioUtente = this.dg_utenti.Items[0].Cells[2].Text;

                  this.dg_utentiTrovati.Visible = false;
                  this.txt_ricerca.Text = "";

                  this.lbl_risultatoUtentiRuolo.Visible = true;
                  this.lbl_risultatoUtentiRuolo.Text = "<font color='#ff0000'><br>...attesa inserimento obbligatorio<br>di un nuovo utente al posto di:<br>" + vecchioUtente + "</font>";

                  this.hd_returnValueModal.Value = "";
                  break;

               case "N":
                  // aggiorna tutte le trasmissioni con Work-flow come Rifiutate e con Nota: "Trasmissione rifiutata per mancanza di utenti nel ruolo"
                  if (this.RifiutaTrasmConWF(Request.QueryString["idCorrGlobRuolo"], Request.QueryString["idGruppo"]))
                  {
                     // elimina l'unico utente presente nella datagrid per questo ruolo
                     if (this.EliminaUtenteInRuolo(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idGruppo"]))
                     {
                        // ripulisce l'AREA DI LAVORO
                        if (this.EliminaADLUtente(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                        {
                           this.dg_utenti.Visible = false;
                           this.lbl_risultatoUtentiRuolo.Visible = true;

                           this.hd_returnValueModal.Value = "";
                        }
                     }
                  }
                  break;
                case "NA":
                    if (this.AccettaTrasmConWF(Request.QueryString["idCorrGlobRuolo"]))
                    {
                        // elimina l'unico utente presente nella datagrid per questo ruolo
                        if (this.EliminaUtenteInRuolo(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idGruppo"]))
                        {
                            // ripulisce l'AREA DI LAVORO
                            if (this.EliminaADLUtente(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                            {
                                this.dg_utenti.Visible = false;
                                this.lbl_risultatoUtentiRuolo.Visible = true;

                                this.hd_returnValueModal.Value = "";
                            }
                        }
                    }
                    break;
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

        private void GestRitornoAvvisoLF(string valore)
        {
            try
            {
                int index = Convert.ToInt32(Session["indexUser"]);
                string vecchioUtente;
                switch (valore)
                {
                    case "ADD_USER":
                        this.dg_utenti.Visible = false;
                        this.lbl_risultatoUtentiRuolo.Visible = true;
                        this.SetSessionInterrompiProcessi(false);
                        this.SetSessionSostUtente(this.dg_utenti.Items[0].Cells[4].Text);
                        vecchioUtente = this.dg_utenti.Items[0].Cells[2].Text;

                        this.dg_utentiTrovati.Visible = false;
                        this.txt_ricerca.Text = "";

                        this.lbl_risultatoUtentiRuolo.Visible = true;
                        this.lbl_risultatoUtentiRuolo.Text = "<font color='#ff0000'><br>...attesa inserimento obbligatorio<br>di un nuovo utente al posto di:<br>" + vecchioUtente + "</font>";

                        this.hd_returnValueModal.Value = "";
                        break;
                    case "REPLACE_USER":
                        this.SetSessionInterrompiProcessi(false);
                        this.SetSessionSostUtenteLF(this.dg_utenti.Items[index].Cells[4].Text);
                        this.LoadUtentiAttuali();
                        this.lbl_risultatoUtentiRuolo.Visible = true;
                        vecchioUtente = this.dg_utenti.Items[index].Cells[2].Text;

                        this.dg_utentiTrovati.Visible = false;
                        this.txt_ricerca.Text = "";

                        this.lbl_risultatoUtentiRuolo.Visible = true;
                        this.lbl_risultatoUtentiRuolo.Text = "<font color='#ff0000'><br>...attesa sostituzione obbligatoria<br>di un utente al posto di:<br>" + vecchioUtente + "</font>";

                        this.hd_returnValueModal.Value = "";
                        break;
                    case "Y":
                        // verifica che non sia l'unico del ruolo
                        this.SetSessionInterrompiProcessi(true);
                        if (this.dg_utenti.Items.Count > 1)
                        {
                            // NON è l'unico utente nel ruolo quindi lo elimina dal ruolo
                            if (this.EliminaUtenteInRuolo(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idGruppo"]))
                            {
                                // ripulisce l'AREA DI LAVORO
                                if (this.EliminaADLUtente(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                {
                                    this.LoadUtentiAttuali();
                                }

                                // ricarica la ricerca
                                this.RicercaUtenti();
                            }
                        }
                        else
                        {
                            // è l'unico utente del ruolo...
                            // verifica che il ruolo non abbia trasmissioni con work-flow
                            if (this.RuoloConTrasmissioni(Request.QueryString["idCorrGlobRuolo"]))
                            {
                                // ruolo con trasmissioni... avvisa l'amministratore e apre una modal Dialog
                                if (!this.Page.IsStartupScriptRegistered("openModalRuoloConTX"))
                                {
                                    string scriptString = "<SCRIPT>AvvisoRuoloConTX('" + dg_utenti.Items[index].Cells[2].Text.Replace("'", "\\'") + "','" + dg_utenti.Items[index].Cells[4].Text + "','" + Request.QueryString["idCorrGlobRuolo"] + "');</SCRIPT>";
                                    this.Page.RegisterStartupScript("openModalRuoloConTX", scriptString);
                                }
                            }
                            else
                            {
                                // elimina utente dal ruolo
                                if (this.EliminaUtenteInRuolo(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idGruppo"]))
                                {
                                    // ripulisce l'AREA DI LAVORO
                                    if (this.EliminaADLUtente(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                    {

                                        // ripulisce il datagrid
                                        this.InitializeDataSetUtenti();

                                        DataView dv = dsUtenti.Tables["UTENTI"].DefaultView;
                                        dv.Sort = "descrizione ASC";
                                        dg_utenti.DataSource = dv;
                                        dg_utenti.DataBind();

                                        this.dg_utenti.Visible = false;
                                        this.lbl_risultatoUtentiRuolo.Visible = true;

                                        // ricarica la ricerca
                                        this.RicercaUtenti();
                                    }
                                }
                            }
                        }
                        break;

                    case "N":
                        break;
                }
                this.hd_returnValueModalLF.Value = "";
                Session["indexUser"] = null;
            }
            catch
            {
                this.gestErrori();
            }
        }

        private void GestRitornoAvvisoUtenteConTrasm(string valore)
        {
            try
            {
                int index = Convert.ToInt32(Session["indexUser"]);
                switch (valore)
                {
                    case "Y":
                        if (this.AccettaTrasmConWFUtente(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                        {
                            if (this.EliminaUtenteInRuolo(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idGruppo"]))
                            {
                                // ripulisce l'AREA DI LAVORO
                                if (this.EliminaADLUtente(dg_utenti.Items[index].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                                {
                                    this.LoadUtentiAttuali();
                                }

                                // ricarica la ricerca
                                this.RicercaUtenti();
                            }
                        }

                        break;

                    case "N":
                        break;
                }
                this.hd_returnValueModalUtenteConTrasm.Value = "";
                Session["indexUser"] = null;
            }
            catch
            {
                this.gestErrori();
            }
        }

        #endregion

        #region Gestione errori
        /// <summary>
        /// gestore degli errori
        /// </summary>
        private void gestErrori()
      {
         this.dg_utentiTrovati.Visible = false;
         this.lbl_avviso.Text = "Errore di sistema!";
      }
      #endregion
   }
}
