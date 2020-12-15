using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Conservazione
{
    public partial class ControlliAutomatici : System.Web.UI.Page
    {

        SAAdminTool.DocsPaWR.ChiaveConfigurazione configString;
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }


        /// <summary>
        /// Aggiornamento di una chiave
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.ValidationResultInfo UpdateChiaveConfig(ref SAAdminTool.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            //Undo Modifiche Lorusso 22-10-2012
            ////DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            AmmUtils.WebServiceLink wsl = new AmmUtils.WebServiceLink();
            return wsl.updateChiaveConfig(chiaveConfig);
            //End Undo
            //DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            //return ws.updateChiaveConfig(chiaveConfig);
        }


        private SAAdminTool.DocsPaWR.ChiaveConfigurazione GetChiaveConfigurazione(string idChiaveConfig, string idAmministrazione)
        {
            utils.ConfigRepository cR = utils.InitConfigurationKeys.getInstance(idAmministrazione);
            return (SAAdminTool.DocsPaWR.ChiaveConfigurazione)cR[idChiaveConfig];
        }


        private void Clear(string idAmm)
        {
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            ws.clearHashTableChiaviConfig(idAmm);
            utils.InitConfigurationKeys.remove(idAmm);
        }


        //CHIAVE BE_CONSERVAZIONE_AUTOTEST_JOB 
        // Vuota / nulla <- non abilitata
        // Tipo|tiks


        //Tipi di Intervallo
        public enum IntervalTypesEnum
        {
            BySecond,
            ByMinute,
            Hourly,
            Daily,
            Weekly,
            Block,
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                configString = GetChiaveConfigurazione("BE_CONSERVAZIONE_AUTOTEST_JOB", IdAmministrazione.ToString());
                setEnabled();
            }

        }


        private void setEnabled()
        {
            if (configString == null)
            {
                Enabled.Checked = false;
                return;
            }
            if (String.IsNullOrEmpty(configString.Valore) || (configString.Valore == "0"))
            {
                Enabled.Checked = false;
            }
            Enabled.Checked = true;
        }

        private void SetControls(string configString)
        {

            if (String.IsNullOrEmpty ( configString ))
            {
                Enabled.Checked = false;
                return;

            }
            if (configString =="0")
            {
                Enabled.Checked = false;
                return;

            }

            string[] configVals = configString.Split('|');
            
            string tick="0";
            string tipo = configVals[0];
            if (configVals.Length > 1)
                tick = configVals[1];
            else
            {
                group1.Checked = false;
                group2.Checked = false;
                group3.Checked = false;
                Enabled.Checked = false;
                return;
            }

            switch (tipo)
            {
                case "BySecond":
                    group1.Checked = true;
                    cboGroup1ExecutionMode.SelectedIndex = 0;
                    txtGroup1Number.Text = TimeSpan.FromTicks(long.Parse(tick)).Seconds.ToString();
                    break;
                case "ByMinute":
                    group1.Checked = true;
                    cboGroup1ExecutionMode.SelectedIndex = 0;
                    txtGroup1Number.Text = TimeSpan.FromTicks(long.Parse(tick)).Minutes.ToString();
                    break;
                case "Hourly":
                    group1.Checked = true;
                    cboGroup1ExecutionMode.SelectedIndex = 1;
                    txtGroup1Number.Text = TimeSpan.FromTicks(long.Parse(tick)).Hours.ToString();
                    break;
                case "Daily":
                    group2.Checked = true;
                    timeGroup2.SetTimeSpan(TimeSpan.FromTicks(long.Parse(tick)));
                    break;
                case "Weekly":
                    group3.Checked = true;
                    TimeSpan t3 = new TimeSpan(long.Parse(tick));
                    cboGroup3Days.SelectedIndex = t3.Days - 1; //fare attenzione
                    timeGroup3.SetTimeSpan(t3);
                    break;
                default:
                    group1.Checked = false;
                    group2.Checked = false;
                    group3.Checked = false;
                    Enabled.Checked = false;
                    break;

            }
        }

        protected void GestioneGrafica()
        {
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {

                if (configString != null)
                {
                    SetControls(configString.Valore);
                }


                if (this.Enabled.Checked == true)
                {
                    RefreshControls(true);
                }
                else
                {
                    RefreshControls(false);
                }


            }
            catch (Exception ex)
            {
                ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetExecutionConfigurations()
        {
            string retValue = string.Empty;

            if (this.group1.Checked)
            {
                // Configurazioni Secondi / Minuti / Ore
                int interval;
                Int32.TryParse(this.txtGroup1Number.Text, out interval);
                if (interval == 0)
                    interval = 1;

                IntervalTypesEnum intervalType =
                    (IntervalTypesEnum)Enum.Parse(typeof(IntervalTypesEnum), this.cboGroup1ExecutionMode.SelectedValue, true);

                string ticks = string.Empty;

                if (intervalType == IntervalTypesEnum.BySecond)
                    ticks = TimeSpan.FromSeconds(interval).Ticks.ToString();
                else if (intervalType == IntervalTypesEnum.ByMinute)
                    ticks = TimeSpan.FromMinutes(interval).Ticks.ToString();
                else if (intervalType == IntervalTypesEnum.Hourly)
                    ticks = TimeSpan.FromHours(interval).Ticks.ToString();

                return String.Format("{0}|{1}", intervalType, ticks);
            }
            else if (this.group2.Checked)
            {
                // Configurazioni Giornaliere
                retValue = String.Format("{0}|{1}", IntervalTypesEnum.Daily, this.timeGroup2.GetTimeSpan().Ticks.ToString());
                
            }
            else if (this.group3.Checked)
            {
                // Configurazioni Settimanali

                TimeSpan ts = this.timeGroup3.GetTimeSpan();
                ts = new TimeSpan(int.Parse(this.cboGroup3Days.SelectedValue),
                                        ts.Hours,
                                        ts.Minutes,
                                        ts.Seconds,
                                        ts.Milliseconds);


                retValue= String.Format("{0}|{1}", IntervalTypesEnum.Weekly, ts.Ticks);
            }
            return retValue;
        }

        /// <summary>
        /// Aggiornamento controlli UI
        /// </summary>
        private void RefreshControls(bool enabled)
        {
            if (enabled)
            {

                this.group1.Enabled = true;
                this.txtGroup1Number.Enabled = false;
                this.cboGroup1ExecutionMode.Enabled = false;

                this.group2.Enabled = true;
                this.timeGroup2.Enabled = false;

                this.group3.Enabled = true;
                this.cboGroup3Days.Enabled = false;
                this.timeGroup3.Enabled = false;


                if (this.group1.Checked)
                {
                    this.group1.Enabled = true;
                    this.txtGroup1Number.Enabled = true;
                    this.cboGroup1ExecutionMode.Enabled = true;
                }
                else if (this.group2.Checked)
                {
                    this.group2.Enabled = true;
                    this.timeGroup2.Enabled = true;
                }
                else if (this.group3.Checked)
                {
                    this.group3.Enabled = true;
                    this.cboGroup3Days.Enabled = true;
                    this.timeGroup3.Enabled = true;
                }

            }
            else
            {
                this.group1.Enabled = false ;
                this.group2.Enabled = false;
                this.group3.Enabled = false;
            }
        }

        protected void btn_salva_scheduler_Click(object sender, EventArgs e)
        {
            if (configString == null)
            {
                configString = new DocsPaWR.ChiaveConfigurazione
                {
                    Codice = "BE_CONSERVAZIONE_AUTOTEST_JOB",
                    TipoChiave = "B",
                    IDAmministrazione = IdAmministrazione.ToString(),
                    Modificabile = "0",
                    Visibile = "0",
                    IsGlobale = "0",
                    Descrizione = "Chiave di configurazione per gestire i controlli automatici della conservazione",
                    Valore ="0"
                };

                DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                ws.addChiaveConfigurazione(configString);
                this.Clear(IdAmministrazione.ToString());
                configString = GetChiaveConfigurazione("BE_CONSERVAZIONE_AUTOTEST_JOB", IdAmministrazione.ToString());
            }


            if (this.Enabled.Checked == false)
            {
                configString.Valore = "0";
            }
            else
            {
                configString.Valore = GetExecutionConfigurations();
            }
            DocsPaWR.ValidationResultInfo result = null;

            result = UpdateChiaveConfig(ref configString);

            if (!result.Value)
            {
               // this.ShowValidationMessage(result);

                //
                // MEV CS 1.3 - LOG controlli automatici
                try
                {
                    SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                    DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                    ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_CONTROLLI_AUTOMATICI", string.Empty, "Attivazione controlli automatici per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), false);
                }
                catch (Exception ex) 
                {
                }
                // End MEV CS 1.3 - LOG controlli automatici
                //
            }
            else
            {


                // Pulizia della cache
                //pulizia della cache sul backend e sul frontend
                this.Clear(IdAmministrazione.ToString());
                //ricalcolo delle chiavi
                //this.FillListChiaviConfig();

                //
                // MEV CS 1.3 - LOG controlli automatici
                try
                {
                    SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                    DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                    ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_CONTROLLI_AUTOMATICI", string.Empty, "Attivazione controlli automatici per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), true);
                }
                catch (Exception ex) 
                {
                }
                // End MEV CS 1.3 - LOG controlli automatici
                //
            }
        }

        protected void Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Enabled.Checked == true)
            {
                RefreshControls(true);
            } else {
                RefreshControls(false);
            }
        }
    }
}
