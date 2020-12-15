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
using RC = RubricaComune;
using RubricaComune.Proxy.Elementi;
using DocsPAWA.utils;
using System.Linq;
namespace DocsPAWA.AdminTool.Gestione_RubricaComune
{
    /// <summary>
    /// Pagina per l'invio di una UO come elemento in rubrica comune
    /// </summary>
    public partial class InvioUORubricaComune : System.Web.UI.Page
    {
        #region Handler eventi

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            try
            {
                if (!this.IsPostBack)
                {
                    // Le informazioni sull'RF devono essere visualizzate solo se 
                    // si sta inviando un RF a Rubrica Comune
                    this.RfInfo1.Visible = this.TipoElemento == Tipo.RF;
                    this.trRfInfo.Visible = this.TipoElemento == Tipo.RF;

                    // Caricamento dati
                    this.Fetch();
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInvia_Click(object sender, EventArgs e)
        {
            try
            {
                this.Save();

                this.Close(true);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnChiudi_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retValue"></param>
        protected virtual void Close(bool retValue)
        {
            this.txtReturnValue.Value = retValue.ToString().ToLower();

            this.RegisterClientScript("Close", "CloseDialog();");
        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        #endregion

        #region Gestione dati

        /// <summary>
        /// Caricamento dati
        /// </summary>
        protected virtual void Fetch()
        {
            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

            RC.Proxy.Elementi.ElementoRubrica elementoDaInviare = null;

            // Reperimento elemento rubrica relativo all'elemento da inviare
            switch (this.TipoElemento)
            {
                case Tipo.UO:
                    elementoDaInviare = RubricaComune.RubricaServices.NuovoElementoRubrica(sessionManager.getUserAmmSession(), this.IdElemento, this.TipoElemento);
                    break;
                case Tipo.RF:
                    elementoDaInviare = RubricaComune.RubricaServices.NuovoElementoRubrica(sessionManager.getUserAmmSession(), this.IdElemento, this.TipoElemento);
                    break;
            
            }

            if (elementoDaInviare != null)
            {
                this.txtCodice.Text = elementoDaInviare.Codice;

                // Se è valorizzata la proprietà Descrizione, viene visualizzata come descrizione del corrispondente
                // da inviare ad RC, altrimenti viene mostrata la descrizione del corrispondente pubblicato in RC
                if (String.IsNullOrEmpty(this.Descrizione))
                    this.txtDescrizione.Text = elementoDaInviare.Descrizione;
                else
                    this.txtDescrizione.Text = this.Descrizione;

                string idReg = (this.TipoElemento == Tipo.RF) ? this.IdElemento : (from r in
                                                                                       UserManager.getRegistriByCodAmm(elementoDaInviare.Amministrazione, "0")
                                                                                   where r.Codice.Equals(elementoDaInviare.AOO)
                                                                                   select r).FirstOrDefault() != null ?
                                                                                        (from r in
                                                                                             UserManager.getRegistriByCodAmm(elementoDaInviare.Amministrazione, "0")
                                                                                         where r.Codice.Equals(elementoDaInviare.AOO)
                                                                                         select r).First().IDRegistro : null;
                    

                // Se si stanno visualizzando i dati di un RF, vengono visualizzati anche i dettagli
                if (this.TipoElemento == Tipo.RF)
                    this.RfInfo1.ShowRfProperty(elementoDaInviare);
                if (!string.IsNullOrEmpty(idReg))
                {
                    DocsPaWR.CasellaRegistro[] caselle = DocsPAWA.utils.MultiCasellaManager.GetMailRegistro(idReg);
                    System.Collections.Generic.List<EmailInfo> listEmails = new System.Collections.Generic.List<EmailInfo>();
                    foreach (DocsPaWR.CasellaRegistro c in caselle)
                    {
                        listEmails.Add(new EmailInfo
                        {
                            Email = c.EmailRegistro,
                            Note = c.Note,
                            Preferita = (c.Principale.Equals("1")) ? true : false
                        });
                    }
                    if (listEmails != null && listEmails.Count > 0)
                        elementoDaInviare.Emails = listEmails.ToArray();

                }

                this.ElementoDaInviare = elementoDaInviare;
            }
            else
                throw new ApplicationException("Nessun elemento trovato");
        }

        /// <summary>
        /// Invio dei dati della UO a rubrica comune
        /// </summary>
        protected virtual void Save()
        {
            if (this.ElementoDaInviare != null)
            {
                RC.Proxy.Elementi.ElementoRubrica elemento;

                // Se è un RF, vengono salvati anche i dati aggiuntivi
                if (this.TipoElemento == Tipo.RF)
                {
                    this.RfInfo1.CompileRFProperty(this.ElementoDaInviare);
                    this.RfInfo1.SaveRfDetails(this.ElementoDaInviare, this.IdElemento);
                }
                
                // L'url per l'accesso all'interoperabilità semplificata
                // viene inviato solo se il registro con cui è interoperante la UO
                // è abilitato all'IS
                this.ElementoDaInviare.Urls = InteroperabilitaSemplificataManager.GetUrls(this.ElementoDaInviare.TipoCorrispondente, this.IdElemento);
                
                // Impostazione del tipo di corrispondente
                this.ElementoDaInviare.TipoCorrispondente = this.TipoElemento;
                
                if (this.ContainsElementoRubrica(out elemento))
                {
                    // L'elemento è già stato inviato in rubrica
                    this.ElementoDaInviare.Id = elemento.Id;
                    this.ElementoDaInviare.Codice = elemento.Codice;
                    this.ElementoDaInviare.Descrizione = this.txtDescrizione.Text;
                    this.ElementoDaInviare.DataCreazione = elemento.DataCreazione;
                    this.ElementoDaInviare.DataUltimaModifica = elemento.DataUltimaModifica;
                    this.ElementoDaInviare.UtenteCreatore = elemento.UtenteCreatore;
                    this.ElementoDaInviare.CHA_Pubblicato = "1";
                    
                    // Modifica dati in rubrica comune
                    this.ElementoDaInviare = this.RubricaServices.Update(this.ElementoDaInviare);
                }
                else
                {
                    // Impostazione della descrizione
                    this.ElementoDaInviare.Descrizione = this.txtDescrizione.Text;
                    // Imposto cha_pubblicato = 1 (necessario per rendere readonly il multicasella lato amministrazione --> gestione rubrica comune
                    this.ElementoDaInviare.CHA_Pubblicato = "1";
                    // Inserimento dati in rubrica comune
                    this.ElementoDaInviare = this.RubricaServices.Insert(this.ElementoDaInviare);
                }
            }
            else
                throw new ApplicationException("Nessun elemento trovato");
        }

        /// <summary>
        /// Verifica se esiste già un elemento rubrica con il codice visualizzato
        /// </summary>
        /// <param name="elemento">
        /// Elemento rubrica esistente
        /// </param>
        /// <returns></returns>
        protected bool ContainsElementoRubrica(out RC.Proxy.Elementi.ElementoRubrica elemento)
        {
            elemento = this.RubricaServices.SearchSingle(this.ElementoDaInviare.Codice);

            return (elemento != null);
        }

        /// <summary>
        /// Impostazione / reperimento dal viewstate dell'elemento relativo all'UO da inviare a rubrica
        /// </summary>
        protected RC.Proxy.Elementi.ElementoRubrica ElementoDaInviare
        {
            get
            {
                return this.ViewState["ElementoDaInviare"] as RC.Proxy.Elementi.ElementoRubrica;
            }
            set
            {
                this.ViewState["ElementoDaInviare"] = value;
            }
        }

        /// <summary>
        /// ID dell'elemento da inviare a rubrica comune
        /// </summary>
        protected string IdElemento
        {
            get
            {
                return this.Request.QueryString["IdElemento"];
            }
        }

        /// <summary>
        /// Recupero del tipo elemento 
        /// </summary>
        protected Tipo TipoElemento
        {
            get
            {
                Tipo tipo = Tipo.UO;
                try
                {
                    tipo = (Tipo)Enum.Parse(typeof(Tipo), this.Request.QueryString["TipoElemento"]);
                }
                catch (Exception ex)
                {
                    tipo = Tipo.UO;
                }

                return tipo;
            }
        }

        /// <summary>
        /// Recupero della descrizione del corrispondente locale. Viene presa in considerazione solo nel caso
        /// di pubblicazione di RF
        /// </summary>
        protected String Descrizione
        {
            get
            {
                return HttpUtility.UrlDecode(Request["Descrizione"]);

            }
        }

        /// <summary>
        /// Istanza servizi rubrica
        /// </summary>
        private RC.ElementiRubricaServices _rubricaServices = null;

        /// <summary>
        /// Reperimento istanza servizi rubrica
        /// </summary>
        protected RC.ElementiRubricaServices RubricaServices
        {
            get
            {
                if (this._rubricaServices == null)
                {
                    AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                    this._rubricaServices = RubricaComune.RubricaServices.GetElementiRubricaServiceInstance(sessionManager.getUserAmmSession());
                }

                return this._rubricaServices;
            }
        }

        #endregion

        #region Gestione JavaScript

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        protected void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        #endregion
    }
}
