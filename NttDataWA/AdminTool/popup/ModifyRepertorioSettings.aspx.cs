using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.utils;
using System.Text;
using SAAdminTool.AdminTool.UserControl;

namespace SAAdminTool.popup
{
    public partial class ModifyRepertorioSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

        }

        /// <summary>
        /// Metodo per la costruzione dello script per l'apertura di questa pagina
        /// </summary>
        /// <param name="counterId">Id del contatore</param>
        /// <returns></returns>
        public static String GetOpenScript(String counterId)
        {
            //return String.Format(
            //   "window.open('{0}/popup/ModifyRepertorioSettings.aspx?counterId={1}');",
            //   Utils.getHttpFullPath(), counterId);
            return String.Format(
               "window.showModalDialog('{0}/popup/ModifyRepertorioSettings.aspx?counterId={1}', '', 'dialogWidth:588px;dialogHeight:450px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
               Utils.getHttpFullPath(), counterId);
        }

        /// <summary>
        /// Metodo per la formattazione di una data.
        /// </summary>
        /// <param name="dateTime">Data da formattare</param>
        /// <returns>Data nel formato dd/mm/yyyy</returns>
        protected String FormatDate(DateTime dateTime)
        {
            // Se la data è impostata al minimo possibile, molto probabilmente non è mai stata effettuata una
            // stampa del registro quindi viene restituito un trattino, altrimenti viene formattata
            if (dateTime == DateTime.MinValue)
                return "-";
            else
                return dateTime.ToString("dd/MM/yyyy");
        }

        protected String FormatDateTextBox(DateTime dateTime)
        {
                return dateTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Metodo utilizzato per decidere se abilitare le caselle di testo con le dati di inizio e fine validità
        /// </summary>
        /// <param name="settings">Impostazioni in base a cui decidere se abilitare o meno la casella di testo della data</param>
        /// <returns>True se bisogna visualizzarle attive</returns>
        protected bool EnableDateTextBox(RegistroRepertorioSingleSettings settings)
        {
            return settings.PrintFrequency != Frequency.N;
        }

        protected void dgRegistersRF_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    // Visualizzazione dettagli delle impostazioni per lo specifico registro / Rf
                    this.SettingsDataSource.SelectParameters["counterId"].DefaultValue = Request["counterId"];
                    this.SettingsDataSource.SelectParameters["registryId"].DefaultValue = ((HiddenField)(e.Item.FindControl("hfRegistryId"))).Value;
                    this.SettingsDataSource.SelectParameters["rfId"].DefaultValue = ((HiddenField)(e.Item.FindControl("hfRFId"))).Value;
                    this.SettingsDataSource.SelectParameters["settingsType"].DefaultValue = this.rblResponsableType.SelectedValue;

                    // Selezione dei dati
                    //this.SettingsDataSource.Select();
                    this.dvRegistryDetails.DataBind();

                    break;
                default:
                    break;
            }
        }

        protected void MinimalSettingsDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!String.IsNullOrEmpty("counterId"))
                e.InputParameters["counterId"] = Request["counterId"];
        }

        protected void MinimalSettingsDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            // Se ci sono risultati, viene impostato il livello di granularità delle impostazioni (solo se questo è il primo
            // caricamento della pagina)
            if (e.ReturnValue != null && ((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue).Length > 0 && !IsPostBack)
                this.rblResponsableType.SelectedValue = ((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue)[0].Settings.ToString();

            // Se non ci sono risultati o se ci sono ma hanno id registro ed id rf nulli, significa che il contatore
            // di repertorio è di tipologia o se ci sono risultati ma nono globali,  viene nascosto il pannello con la lista 
            // dei registri / rf
            if(e.ReturnValue == null || 
                (String.IsNullOrEmpty(((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue)[0].RfId) && 
                    String.IsNullOrEmpty(((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue)[0].RegistryId)) ||
                this.rblResponsableType.SelectedValue == SettingsType.G.ToString())
                this.pnlRegistersRF.Visible = false;
            else
                this.pnlRegistersRF.Visible = true;

            // Se ci sono risultati ma il contatore è un contatore di tipologie
            this.rblResponsableType.Items[1].Enabled = e.ReturnValue != null && !(String.IsNullOrEmpty(((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue)[0].RfId) && 
                    String.IsNullOrEmpty(((RegistroRepertorioSettingsMinimalInfo[])e.ReturnValue)[0].RegistryId));

            // Se la granularità selezionata è G, viene visualizzato il pannello dei dettagli
            //if (!this.rblResponsableType.Items[1].Enabled)
            if (this.rblResponsableType.SelectedValue == "G")
            {
                this.pnlSettingsDetails.Visible = true;
                this.SettingsDataSource.SelectParameters["counterId"].DefaultValue = Request["counterId"];
                this.SettingsDataSource.SelectParameters["settingsType"].DefaultValue = SettingsType.G.ToString();
                //this.SettingsDataSource.Select();
                this.dvRegistryDetails.DataBind();
            }
        }

        protected void rblResponsableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Se è stato selezionato G, viene nascosto il pannello delle impostazioni altrimenti viene visualizzato
            this.pnlSettingsDetails.Visible = this.rblResponsableType.SelectedValue == "G";
            this.pnlRegistersRF.Visible = this.rblResponsableType.SelectedValue == "S";


            // Se il pannello è visibile, viene caricato il dettaglio minimale delle impostazioni
            if (this.pnlSettingsDetails.Visible)
                this.dgRegistersRF.DataBind();

        }

        protected void ddlPrintFrequency_PreRender(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = RegistriRepertorioUtils.CounterSettings.PrintFrequency.ToString();
            
        }

        protected void ddlCounterState_PreRender(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = RegistriRepertorioUtils.CounterSettings.CounterState.ToString();
        }

        protected void ddlRights_PreRender(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = RegistriRepertorioUtils.CounterSettings.RoleRespRight.ToString();
 
        }

        protected void SettingsDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            // Se ci sono dati, viene visualizzato il pannello con le impostazioni per il particolare registro
            // altimenti viene nascosto
            this.pnlSettingsDetails.Visible = e.ReturnValue != null;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Initialize", "initialize();", true);

            if(e.Exception != null)
                this.AjaxMessageBox.ShowMessage("Si è verificato un errore durante il reperimento delle impostazioni.");

            // Salvataggio temporanero delle impostazioni
            if (e.ReturnValue != null && !this.UpdateFailed)
                RegistriRepertorioUtils.CounterSettings = e.ReturnValue as RegistroRepertorioSingleSettings;

                
        }

        protected void SettingsDataSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            // Salvataggio della frequenza di stampa automatica
            String frequency = (dvRegistryDetails.FindControl("ddlPrintFrequency") as DropDownList).SelectedValue;
            RegistriRepertorioUtils.CounterSettings.PrintFrequency = (Frequency)Enum.Parse(typeof(Frequency), frequency);
            
            DateTime date = DateTime.MinValue;
            
            // Salvataggio della data di inizio validità della stampa automatica
            DateTime.TryParse((dvRegistryDetails.FindControl("txtStart") as TextBox).Text, out date);
            RegistriRepertorioUtils.CounterSettings.DateAutomaticPrintStart = date;
            
            // Salvataggio della data di fine validità della stampa automatica
            DateTime.TryParse((dvRegistryDetails.FindControl("txtFinish") as TextBox).Text, out date);
            RegistriRepertorioUtils.CounterSettings.DateAutomaticPrintFinish = date;
            
            // Salvataggio dell'id del ruolo responsabile del registro di repertorio
            RuoloResponsabile roleResp = (dvRegistryDetails.FindControl("cRoleResp") as RuoloResponsabile);
            RegistriRepertorioUtils.CounterSettings.RoleRespId = roleResp.RoleSystemId;

            // Salvataggio della tipologia di diritti da associare al responsabile dei repertori
            String responsableRight = (dvRegistryDetails.FindControl("ddlRights") as DropDownList).SelectedValue;
            RegistriRepertorioUtils.CounterSettings.RoleRespRight = (ResponsableRight)Enum.Parse(typeof(ResponsableRight), responsableRight);
            

            // Salvataggio dell'id del ruolo e dell'utente responsabile delle stampe di repertorio
            RuoloResponsabile printerRole = (dvRegistryDetails.FindControl("cPrinterRole") as RuoloResponsabile);
            RegistriRepertorioUtils.CounterSettings.PrinterRoleRespId = printerRole.RoleSystemId;
            RegistriRepertorioUtils.CounterSettings.PrinterUserRespId = printerRole.UserSystemId;

            // Salvataggio dello stato del registro
            String counterState = (dvRegistryDetails.FindControl("ddlCounterState") as DropDownList).SelectedValue;
            RegistriRepertorioUtils.CounterSettings.CounterState = (RepertorioState)Enum.Parse(typeof(RepertorioState), counterState);
            
            e.InputParameters["counterId"] = Request["counterId"];
            e.InputParameters["settingsType"] = (SettingsType)Enum.Parse(typeof(SettingsType), this.rblResponsableType.SelectedValue);
            e.InputParameters["settings"] = RegistriRepertorioUtils.CounterSettings;

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
            e.InputParameters["idAmm"] = idAmministrazione;
        }

        protected void SettingsDataSource_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.OutputParameters != null &&
                e.OutputParameters["validationResult"] != null &&
                ((ValidationResultInfo)e.OutputParameters["validationResult"]).BrokenRules.Length > 0)
                this.FormatValidationError(e.OutputParameters["validationResult"] as ValidationResultInfo);
            else
            {
                this.AjaxMessageBox.ShowMessage("Salvataggio avvenuto correttamente.");

                // Aggiornamento dei dati sui registri / RF
                this.dgRegistersRF.DataBind();

                // Se si stanno gestendo impostazioni per il singolo RF / Registro di AOO, bisogna chiudere il dettaglio
                if (this.rblResponsableType.SelectedValue == SettingsType.S.ToString())
                    this.pnlSettingsDetails.Visible = false;
            }

            // Impostazione dell'esito dell'operazione di salvataggio dati
            this.UpdateFailed = !Convert.ToBoolean(e.ReturnValue);
             
        }

        /// <summary>
        /// Metodo per la formattazione degli errori di validazione
        /// </summary>
        /// <param name="validationResultInfo">Errori di validazione da visualizzare</param>
        private void FormatValidationError(ValidationResultInfo validationResultInfo)
        {
            StringBuilder plainMessages = new StringBuilder("Sono stati rilevati i seguenti errori di validazione:\\n");

            validationResultInfo.BrokenRules.Select(rule => { plainMessages.AppendFormat("{0}\\n\\n", rule.Description); return rule; }).ToList();

            this.AjaxMessageBox.ShowMessage(plainMessages.ToString());

        }

        /// <summary>
        /// Proprietà utilizzata per determinare se l'operazione di update dei dati è fallita.
        /// </summary>
        private bool UpdateFailed { get; set; }

    }
}