using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Pubblicazioni
{
    /// <summary>
    /// Dettaglio del canale di pubblicazione
    /// </summary>
    public partial class ChannelDetail : System.Web.UI.UserControl
    {
        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                this.ShowErrorMessage(originalEx.Message);
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
                this.RefreshControlsEnabled();
            }
            catch (Exception ex)
            {
                ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                this.ShowErrorMessage(originalEx.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFetchSubscribers_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionFetchSubscribers();
            }
            catch (Exception ex)
            {
                ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                this.ShowErrorMessage(originalEx.Message);
            }
        }

        #endregion

        #region Data Management

        /// <summary>
        /// Azione di modifica dei dati
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="channel"></param>
        public void LoadData(int idAdmin, Publisher.Proxy.ChannelRefInfo channel)
        {
            this.Clear();

            this.Channel = channel;

            if (this.Channel.Id > 0)
            {
                // Modalità di modifica dati
                this.FetchSubscribers(this.Channel.SubscriberServiceUrl);
            }
            else
            {
                // Modalità di inserimento
            }

            // Caricamento dati del canale
            this.Fetch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Publisher.Proxy.JobExecutionConfigurations GetExecutionConfigurations()
        {
            Publisher.Proxy.JobExecutionConfigurations retValue = null;

            if (this.group1.Checked)
            {
                // Configurazioni Secondi / Minuti / Ore

                int interval;
                Int32.TryParse(this.txtGroup1Number.Text, out interval);
                if (interval == 0)
                    interval = 1;

                Publisher.Proxy.IntervalTypesEnum intervalType =
                    (Publisher.Proxy.IntervalTypesEnum) Enum.Parse(typeof(Publisher.Proxy.IntervalTypesEnum), this.cboGroup1ExecutionMode.SelectedValue, true);

                string ticks = string.Empty;

                if (intervalType == Publisher.Proxy.IntervalTypesEnum.BySecond)
                    ticks = TimeSpan.FromSeconds(interval).Ticks.ToString();
                else if (intervalType == Publisher.Proxy.IntervalTypesEnum.ByMinute)
                    ticks = TimeSpan.FromMinutes(interval).Ticks.ToString();
                else if (intervalType == Publisher.Proxy.IntervalTypesEnum.Hourly)
                    ticks = TimeSpan.FromHours(interval).Ticks.ToString();

                retValue = new Publisher.Proxy.JobExecutionConfigurations
                {
                    IntervalType = intervalType,
                    ExecutionTicks = ticks
                };
            }
            else if (this.group2.Checked)
            {
                // Configurazioni Giornaliere
                retValue = new Publisher.Proxy.JobExecutionConfigurations
                {
                    IntervalType = Publisher.Proxy.IntervalTypesEnum.Daily,
                    ExecutionTicks = this.timeGroup2.GetTimeSpan().Ticks.ToString()
                };
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

                retValue = new Publisher.Proxy.JobExecutionConfigurations
                {
                    IntervalType = Publisher.Proxy.IntervalTypesEnum.Weekly,
                    ExecutionTicks = ts.Ticks.ToString()
                };
            }
            else if (this.group4.Checked)
            {
                // Configurazioni a fasce basate su secondi
                TimeSpan tsStart = this.timeGroup4Start .GetTimeSpan();
                TimeSpan tsStop = this.timeGroup4Stop.GetTimeSpan();
                int interval;
                Int32.TryParse(this.txtGroup4Number.Text, out interval);
                string ticks= TimeSpan.FromMinutes(interval).Ticks.ToString();

                String TickRange =String.Format ("{0}|{1}|{2}",ticks, tsStart.Ticks.ToString(),tsStop.Ticks.ToString());
                retValue = new Publisher.Proxy.JobExecutionConfigurations
                {
                    IntervalType = Publisher.Proxy.IntervalTypesEnum.Block,
                    ExecutionTicks = TickRange
                };
            }
            return retValue;
        }

        /// <summary>
        /// Azione di salvataggio dei dati
        /// </summary>
        public Publisher.Proxy.ChannelRefInfo SaveData()
        {
            this.Channel.SubscriberServiceUrl = this.txtSubscriberUrl.Text;
            this.Channel.ChannelName = this.cboSubscribers.SelectedItem.Text;

            DateTime date;
            if (DateTime.TryParse(this.txtComputeFrom.Text, out date))
            {
                date = date.Add(this.txtComputeFromTime.GetTimeSpan());

                this.Channel.StartLogDate = date;
            }

            this.Channel.ExecutionConfiguration = this.GetExecutionConfigurations();

            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                this.Channel = ws.SaveChannel(this.Channel);
            }

            return this.Channel;
        }

        /// <summary>
        /// Reperimento dei sottoscrittori disponibili nel sistema
        /// </summary>
        /// <returns></returns>
        private Subscriber.Proxy.ChannelInfo[] GetSubscribers(string serviceUrl)
        {
            //if (!this.PingSubscriberServices(serviceUrl))
            //    throw new ApplicationException("Servizio sottoscrittore non raggiungibile");

            try
            {
                // Caricamento dei sottoscrittori configurati e raggiungibili tramite l'url indicato
                using (Subscriber.Proxy.SubscriberWebService ws = SubscriberServiceFactory.Create(serviceUrl))
                    return ws.GetChannelList();
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                ApplicationException ex = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(soapEx);

                throw ex;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Servizio sottoscrittore non raggiungibile");
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="serviceUrl"></param>
        ///// <returns></returns>
        //private bool PingSubscriberServices(string serviceUrl)
        //{
        //    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                
        //    System.Net.NetworkInformation.PingReply result = ping.Send(serviceUrl, Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds));

        //    return (result.Status == System.Net.NetworkInformation.IPStatus.Success);
        //}

        /// <summary>
        /// Reperimento canale di pubblicazione
        /// </summary>
        /// <returns></returns>
        private Publisher.Proxy.ChannelRefInfo Channel
        {
            get
            {
                return this.ViewState["Channel"] as Publisher.Proxy.ChannelRefInfo;
            }
            set
            {
                this.ViewState["Channel"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceUrl"></param>
        private void FetchSubscribers(string serviceUrl)
        {
            cboSubscribers.DataValueField = "Name";
            cboSubscribers.DataTextField = "Name";

            cboSubscribers.DataSource = this.GetSubscribers(serviceUrl);
            cboSubscribers.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Clear()
        {
            this.txtSubscriberUrl.Text = string.Empty;
            this.cboSubscribers.Items.Clear();
            this.txtComputeFrom.Text = string.Empty;
            this.txtComputeFromTime.SetTimeSpan(new TimeSpan(0, 0, 0));

            this.txtGroup1Number.Text = string.Empty;
            this.cboGroup1ExecutionMode.SelectedValue = Publisher.Proxy.IntervalTypesEnum.BySecond.ToString();

            this.timeGroup2.SetTimeSpan(new TimeSpan(0, 0, 0));

            this.timeGroup3.SetTimeSpan(new TimeSpan(0, 0, 0));
        }

        /// <summary>
        /// Caricamento dati dettaglio
        /// </summary>
        private void Fetch()
        {
            this.txtSubscriberUrl.Text = this.Channel.SubscriberServiceUrl;
            this.cboSubscribers.SelectedValue = this.Channel.ChannelName;
            this.txtComputeFrom.Text = this.Channel.StartLogDate.ToString("dd/MM/yyyy");
            this.txtComputeFromTime.SetTimeSpan(new TimeSpan(this.Channel.StartLogDate.Hour, this.Channel.StartLogDate.Minute, this.Channel.StartLogDate.Second));

            if (this.Channel.ExecutionConfiguration == null)
                this.Channel.ExecutionConfiguration = new Publisher.Proxy.JobExecutionConfigurations();

            //Per il blockType è richiesto un elebarozaione in piu
            if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.Block)
            {
                //TimeSpan ts = new TimeSpan(Convert.ToInt64(this.Channel.ExecutionConfiguration.ExecutionTicks));
                this.group4.Checked = true;
                string[] spans = this.Channel.ExecutionConfiguration.ExecutionTicks.Split('|');
                string interval=string.Empty;
                string startTime=string.Empty;
                string stopTime=string.Empty;
                if (spans.Length >0)
                    interval = spans[0];
                if (spans.Length > 1)
                    startTime = spans[1];
                if (spans.Length > 2)
                    stopTime = spans[2];

                TimeSpan tsInterval = new TimeSpan(Convert.ToInt64(interval));
                this.txtGroup4Number.Text = tsInterval.Minutes.ToString();

                TimeSpan tsStart = new TimeSpan(Convert.ToInt64(startTime ));
                TimeSpan tsStop = new TimeSpan(Convert.ToInt64(stopTime));
                this.txtGroup4Number.Text = tsInterval.Minutes.ToString();
                this.timeGroup4Start.SetTimeSpan(tsStart);
                this.timeGroup4Stop.SetTimeSpan(tsStop);
            }
            else
            {
                // Creazione TimeSpan
                TimeSpan ts = new TimeSpan(Convert.ToInt64(this.Channel.ExecutionConfiguration.ExecutionTicks));

                if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.BySecond)
                {
                    this.group1.Checked = true;
                    this.cboGroup1ExecutionMode.SelectedValue = this.Channel.ExecutionConfiguration.IntervalType.ToString();
                    this.txtGroup1Number.Text = ts.Seconds.ToString();
                }
                else if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.ByMinute)
                {
                    this.group1.Checked = true;
                    this.cboGroup1ExecutionMode.SelectedValue = this.Channel.ExecutionConfiguration.IntervalType.ToString();
                    this.txtGroup1Number.Text = ts.Minutes.ToString();
                }
                else if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.Hourly)
                {
                    this.group1.Checked = true;
                    this.cboGroup1ExecutionMode.SelectedValue = this.Channel.ExecutionConfiguration.IntervalType.ToString();
                    this.txtGroup1Number.Text = ts.Hours.ToString();
                }
                else if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.Daily)
                {
                    this.group2.Checked = true;
                    this.timeGroup2.SetTimeSpan(ts);
                }
                else if (this.Channel.ExecutionConfiguration.IntervalType == Publisher.Proxy.IntervalTypesEnum.Weekly)
                {
                    this.group3.Checked = true;
                    this.cboGroup3Days.SelectedValue = ts.Days.ToString();
                    this.timeGroup3.SetTimeSpan(ts);
                }
            }
        }

        #endregion

        #region UI Management

        /// <summary>
        /// Abilitazione / disabilitazione controllo
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool Enabled
        {
            get
            {
                if (this.ViewState["Enabled"] == null)
                    return true;
                else
                    return Convert.ToBoolean(this.ViewState["Enabled"]);
            }
            set
            {
                this.ViewState["Enabled"] = value;
            }
        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void ShowErrorMessage(string errorMessage)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "ErrorMessage"))
                this.Page.ClientScript.RegisterStartupScript(this.GetType(),
                        "ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "');", true);
        }

        /// <summary>
        /// Aggiornamento controlli UI
        /// </summary>
        private void RefreshControlsEnabled()
        {
            this.txtSubscriberUrl.Enabled = this.Enabled;
            this.cboSubscribers.Enabled = this.Enabled;
            this.btnFetchSubscribers.Enabled = this.Enabled;
            this.txtComputeFrom.Enabled = this.Enabled;
            this.txtComputeFromTime.Enabled = this.Enabled;

            this.group1.Enabled = this.Enabled;
            this.txtGroup1Number.Enabled = false;
            this.cboGroup1ExecutionMode.Enabled = false;

            this.group2.Enabled = this.Enabled;
            this.timeGroup2.Enabled = false;

            this.group3.Enabled = this.Enabled;
            this.cboGroup3Days.Enabled = false;
            this.timeGroup3.Enabled = false;


            this.group4.Enabled = this.Enabled;
            this.txtGroup4Number.Enabled = false;
            this.timeGroup4Start.Enabled = false;
            this.timeGroup4Stop.Enabled = false;

            if (this.Enabled)
            {
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
                else if (this.group4.Checked)
                {
                    this.group4.Enabled = true;
                    this.txtGroup4Number.Enabled = true;
                    this.timeGroup4Start.Enabled = true;
                    this.timeGroup4Stop.Enabled = true;
                }
            }
        }

        // <summary>
         
        // </summary>
        //private void DisableAllIntervalControls()
        //{
        //    this.txtGroup1Number.Enabled = false;
        //    this.cboGroup1ExecutionMode.Enabled = false;

        //    this.timeGroup2.Enabled = false;

        //    this.cboGroup3Days.Enabled = false;
        //    this.timeGroup3.Enabled = false;
        //}

        #endregion

        #region Actions

        /// <summary>
        /// Azione di caricamento dei sottoscrittori
        /// </summary>
        private void PerformActionFetchSubscribers()
        {
            this.FetchSubscribers(this.txtSubscriberUrl.Text);
        }

        #endregion
    }
}
