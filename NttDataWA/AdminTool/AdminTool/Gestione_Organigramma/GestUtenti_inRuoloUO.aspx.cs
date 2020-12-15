using System;
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
      protected System.Web.UI.WebControls.Label lbl_percorso;

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
         this.dg_utenti.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_utenti_ItemCreated);
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
            }
         }
         catch
         {
            this.gestErrori();
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void Inizialize()
      {
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
               this.lbl_risultatoUtentiRuolo.Visible = false;

               this.InitializeDataSetUtenti();

               DataRow row;
               foreach (SAAdminTool.DocsPaWR.OrgUtente utente in theManager.getListaUtenti())
               {
                  row = dsUtenti.Tables[0].NewRow();
                  row["IDUtente"] = utente.IDCorrGlobale;
                  row["codice"] = utente.CodiceRubrica;
                  row["descrizione"] = utente.Cognome + " " + utente.Nome;
                  row["IDAmministrazione"] = utente.IDAmministrazione;
                  row["IDPeople"] = utente.IDPeople;

                  dsUtenti.Tables["UTENTI"].Rows.Add(row);
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

      private void dg_utenti_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
      {
         e.Item.Cells[5].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questo utente?')) {return false};");
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
                  foreach (SAAdminTool.DocsPaWR.OrgUtente utente in theManager.getListaUtenti())
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
         SAAdminTool.Utils.DefaultButton(this, ref this.txt_ricerca, ref this.btn_find);
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
                     // elimina l'unico utente presente nella datagrid per questo ruolo
                     if (this.EliminaUtenteInRuolo(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idGruppo"]))
                     {
                        // ripulisce l'AREA DI LAVORO
                        if (this.EliminaADLUtente(this.dg_utenti.Items[0].Cells[4].Text, Request.QueryString["idCorrGlobRuolo"]))
                        {
                           this.RemoveSessionSostUtente();
                        }
                     }
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

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

      #region Elimina utente

      private void dg_utenti_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {
          try
          {
              if (e.CommandName.Equals("Eliminazione"))
              {
                  // verifica che l'utente non sia connesso a docspa
                  if (this.VerificaUtenteLoggato(e.Item.Cells[1].Text, e.Item.Cells[3].Text))
                  {
                      // INC000000577348
                      // verifica che l'utente non sia responsabile di una stampa di repertorio
                      if (this.VerificaUtenteRespStampeRep(e.Item.Cells[4].Text, Request.QueryString["idGruppo"], e.Item.Cells[3].Text))
                      {
                          // verifica che non sia l'unico del ruolo
                          if (this.dg_utenti.Items.Count > 1)
                          {
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
                                      string scriptString = "<SCRIPT>AvvisoRuoloConTX('" + e.Item.Cells[2].Text.Replace("'", "\\'") + "');</SCRIPT>";
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
          catch
          {
              this.gestErrori();
          }
      }

      private bool VerificaUtenteLoggato(string userId, string idAmm)
      {
         bool result = false;

         try
         {
            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.VerificaUtenteLoggato(userId, idAmm);

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

              SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

      private bool EliminaUtenteInRuolo(string idPeople, string idGruppo)
      {
         bool result = false;

         try
         {
            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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

      private bool RifiutaTrasmConWF(string idCorrGlobRuolo)
      {
         bool result = false;

         try
         {

            Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.RifiutaTrasmConWF(idCorrGlobRuolo);

            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
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
                  if (this.RifiutaTrasmConWF(Request.QueryString["idCorrGlobRuolo"]))
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
