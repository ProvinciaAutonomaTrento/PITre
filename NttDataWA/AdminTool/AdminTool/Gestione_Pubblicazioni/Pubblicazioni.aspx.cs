using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Pubblicazioni
{
    public partial class Pubblicazioni : System.Web.UI.Page
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
                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
                Session["AdminBookmark"] = "Pubblicazioni";
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

                if (!this.IsPostBack)
                {
                    this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                    this.Fetch();
                }
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
        protected void btnNewChannel_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionNewChannel();
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
        protected void btnRefreshChannels_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionRefreshChannels();
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
        protected void btnSaveChannel_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionSaveChannelConfigurations();
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
        protected void btnCloseChannelConfigurations_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionCloseChannelConfigurations();
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
        protected void grdChannels_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            try
            {
                this.SelectedChannel = null;

                this.grdChannels.CurrentPageIndex = e.NewPageIndex;

                this.Fetch();

                this.SelectedDetail = SelectedDetailsEnum.None;
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
        protected void grdChannels_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    this.PerformActionSelectChannel(e);
                }
                else if (e.CommandName == "EditConfigurations")
                {
                    this.PerformActionEditChannelConfigurations(e);
                }
                else if (e.CommandName == "Start")
                {
                    this.PerformActionStartChannelService(e);
                }
                else if (e.CommandName == "Stop")
                {
                    this.PerformActionStopChannelService(e);
                }
                else if (e.CommandName == "Delete")
                {
                    this.PerformActionDeleteChannel(e);
                }
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
        protected void btnNewEvent_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionNewEvent();
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
        protected void btnCloseEvents_Click(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionCloseEvents();
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
        protected void grdEvents_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            try
            {
                this.grdEvents.CurrentPageIndex = e.NewPageIndex;
                this.grdEvents.EditItemIndex = -1;
                this.grdEvents.SelectedIndex = -1;

                int channelId = this.GetChannelId(this.grdChannels.SelectedItem);

                this.FetchEvents(channelId);
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
        protected void grdEvents_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Edit")
                {
                    this.PerformActionEditEvent(e);
                }
                else if (e.CommandName == "Cancel")
                {
                    this.PerformActionCancelEditEvent(e);
                }
                else if (e.CommandName == "Delete")
                {
                    this.PerformActionDeleteEvent(e);
                }
                else if (e.CommandName == "Update")
                {
                    this.PerformActionUpdateEvent(e);
                }
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
        protected void cboObjectTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.PerformActionSelectObjectType();
            }
            catch (Exception ex)
            {
                ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                this.ShowErrorMessage(originalEx.Message);
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Azione di inserimento dell'evento
        /// </summary>
        private void PerformActionNewEvent()
        {
            this.grdEvents.CurrentPageIndex = 0;
            this.grdEvents.SelectedIndex = -1;
            this.grdEvents.EditItemIndex = 0;

            //int channelId = this.GetChannelId(this.grdChannels.SelectedItem);

            this.grdEvents.DataSource = new Publisher.Proxy.EventInfo[1] 
                                        { 
                                            new Publisher.Proxy.EventInfo 
                                            { 
                                                IdChannel = this.SelectedChannel.Id
                                            } 
                                        };
            this.grdEvents.DataBind();

            this.grdEvents.SelectedIndex = 0;
            this.FetchEvent(this.grdEvents.SelectedItem);
        }

        /// <summary>
        /// Chiusura pannello eventi
        /// </summary>
        private void PerformActionCloseEvents()
        {
            this.SelectedChannel = null;
            this.SelectedDetail = SelectedDetailsEnum.None;
        }

        /// <summary>
        /// 
        /// </summary>
        private void PerformActionCloseChannelConfigurations()
        {
            this.SelectedChannel = null;
            this.SelectedDetail = SelectedDetailsEnum.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionCancelEditEvent(DataGridCommandEventArgs e)
        {
            // Reperimento id del canale di pubblicazione selezionato
            int idChannel = this.GetChannelId(this.grdChannels.SelectedItem);

            this.grdEvents.EditItemIndex = -1;
            this.grdEvents.SelectedIndex = -1;

            // Caricamento eventi
            this.FetchEvents(idChannel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionDeleteEvent(DataGridCommandEventArgs e)
        {
            Publisher.Proxy.EventInfo eventInfo = this.GetEvent(this.GetEventId(e.Item));

            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                eventInfo = ws.RemoveEvent(eventInfo);
            }

            this.SelectedChannel = this.GetChannel(this.SelectedChannel.Id);

            // Reperimento id del canale di pubblicazione selezionato
            //int idChannel = this.GetChannelId(this.grdChannels.SelectedItem);

            this.grdEvents.CurrentPageIndex = 0;
            this.grdEvents.EditItemIndex = -1;
            this.grdEvents.SelectedIndex = -1;

            this.FetchEvents(this.SelectedChannel.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionUpdateEvent(DataGridCommandEventArgs e)
        {
            int eventId = this.GetEventId(e.Item);

            Publisher.Proxy.EventInfo eventInfo = null;

            if (eventId > 0)
            {
                // Reperimento evento
                eventInfo = this.SelectedChannel.Events.Where(itm => itm.Id == eventId).First();
            }
            else
            {
                // Nuovo evento
                eventInfo = new Publisher.Proxy.EventInfo { IdChannel = this.SelectedChannel.Id };
            }

            eventInfo.ObjectType = this.GetObjectTypesDropDown(e.Item).SelectedValue;
            eventInfo.ObjectTemplateName = this.GetObjectTemplatesDropDown(e.Item).SelectedValue;
            eventInfo.EventName = this.GetObjectLogsDropDown(e.Item).SelectedValue;

            if (eventInfo.ObjectType == ObjectTypes.DOCUMENTO)
            {
                CheckBox chkLoadFile = (CheckBox)e.Item.FindControl("chkLoadFile");
                eventInfo.LoadFileIfDocumentType = chkLoadFile.Checked;
            }

            // Aggiornamento dati
            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
                eventInfo = ws.SaveEvent(eventInfo);

            this.SelectedChannel = this.GetChannel(this.SelectedChannel.Id);

            // Aggiornamento eventi

            this.grdEvents.EditItemIndex = -1;
            this.grdEvents.SelectedIndex = -1;

            // Caricamento eventi
            this.FetchEvents(this.SelectedChannel.Id);


            //int idChannel = this.GetChannelId(this.grdChannels.SelectedItem);

            //int eventId = this.GetEventId(e.Item);

            //Publisher.Proxy.EventInfo eventInfo = null;

            //if (eventId > 0)
            //{
            //    // Reperimento evento
            //    eventInfo = this.GetEvent(this.GetEventId(e.Item));
            //}
            //else
            //{
            //    // Nuovo evento
            //    eventInfo = new Publisher.Proxy.EventInfo { IdChannel = idChannel };
            //}

            //eventInfo.ObjectType = this.GetObjectTypesDropDown(e.Item).SelectedValue;
            //eventInfo.ObjectTemplateName = this.GetObjectTemplatesDropDown(e.Item).SelectedValue;
            //eventInfo.EventName = this.GetObjectLogsDropDown(e.Item).SelectedValue;

            //if (eventInfo.ObjectType == ObjectTypes.DOCUMENTO)
            //{
            //    CheckBox chkLoadFile = (CheckBox) e.Item.FindControl("chkLoadFile");
            //    eventInfo.LoadFileIfDocumentType = chkLoadFile.Checked;
            //}

            //// Aggiornamento dati
            //using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            //    eventInfo = ws.SaveEvent(eventInfo);

            //// Aggiornamento eventi
            


            //this.grdEvents.EditItemIndex = -1;
            //this.grdEvents.SelectedIndex = -1;

            //// Caricamento eventi
            //this.FetchEvents(idChannel);
        }

        /// <summary>
        /// Azione di modifica dell'evento
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionEditEvent(DataGridCommandEventArgs e)
        {
            this.grdEvents.EditItemIndex = e.Item.ItemIndex;

            // Reperimento id del canale di pubblicazione selezionato
            //int idChannel = this.GetChannelId(this.grdChannels.SelectedItem);

            // Caricamento eventi
            this.FetchEvents(this.SelectedChannel.Id);

            this.grdEvents.SelectedIndex = e.Item.ItemIndex;

            // Caricamento dati evento selezionato
            this.FetchEvent(this.grdEvents.SelectedItem);
        }

        /// <summary>
        /// Azione di selezione tipo oggetto per l'evento corrente
        /// </summary>
        private void PerformActionSelectObjectType()
        {
            // Caricamento profili oggetto
            this.FetchObjectTemplates(this.grdEvents.SelectedItem);

            // Caricamento logs configurabili per il tipo oggetto
            this.FetchObjectLogs(this.grdEvents.SelectedItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionDeleteChannel(DataGridCommandEventArgs e)
        {
            this.SelectedChannel = null;

            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(this.GetChannelId(e.Item));

                ws.RemoveChannel(instance);

                this.grdChannels.CurrentPageIndex = 0;
                this.grdChannels.EditItemIndex = -1;
                this.Fetch();

                this.PerformActionCloseEvents();
            }
        }

        /// <summary>
        /// Azione di avvio del servizio di pubblicazione
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionStartChannelService(DataGridCommandEventArgs e)
        {
            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(this.GetChannelId(e.Item));

                ws.StartChannel(instance);

                this.grdChannels.EditItemIndex = -1;
                this.Fetch();

                this.SelectedChannel = null;
                this.SelectedDetail = SelectedDetailsEnum.None;
            }
        }

        /// <summary>
        /// Azione di stop del servizio di pubblicazione
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionStopChannelService(DataGridCommandEventArgs e)
        {
            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(this.GetChannelId(e.Item));

                ws.StopChannel(instance);

                this.grdChannels.EditItemIndex = -1;
                this.Fetch();

                this.SelectedChannel = null;
                this.SelectedDetail = SelectedDetailsEnum.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionEditChannelConfigurations(DataGridCommandEventArgs e)
        {
            int channelId = this.GetChannelId(e.Item);

            // Reperimento canale di pubblicazione
            this.SelectedChannel = this.GetChannel(channelId);

            // Caricamento dati del canale di pubblicazione
            this.channelDetail.LoadData(this.IdAdmin, this.SelectedChannel);// this.GetChannelId(e.Item));

            this.grdChannels.SelectedIndex = e.Item.ItemIndex;

            this.SelectedDetail = SelectedDetailsEnum.ChannelConfigurationsDetails;
        }

        /// <summary>
        /// Azione di aggiornamento configurazioni canale
        /// </summary>
        private void PerformActionSaveChannelConfigurations()
        {
            this.SelectedChannel = this.channelDetail.SaveData();

            this.PerformActionRefreshChannels();
        }

        /// <summary>
        /// Azione di selezione del canale
        /// </summary>
        /// <param name="e"></param>
        private void PerformActionSelectChannel(DataGridCommandEventArgs e)
        {
            int idChannel = this.GetChannelId(e.Item);

            // Reperimento canale di pubblicazione
            this.SelectedChannel = this.GetChannel(idChannel);

            this.FetchEvents(idChannel);

            this.SelectedDetail = SelectedDetailsEnum.ChannelEventsDetail;
        }

        /// <summary>
        /// Azione di predisposizione all'inserimento di un nuovo canale
        /// </summary>
        private void PerformActionNewChannel()
        {
            this.SelectedChannel = new Publisher.Proxy.ChannelRefInfo
            {
                Admin = new Publisher.Proxy.AdminInfo
                {
                    Id = this.IdAdmin
                },
                State = Publisher.Proxy.ChannelStateEnum.Stopped,
                StartLogDate = DateTime.Now,
                ExecutionConfiguration = new Publisher.Proxy.JobExecutionConfigurations
                {
                    IntervalType = Publisher.Proxy.IntervalTypesEnum.ByMinute,
                    ExecutionTicks = TimeSpan.FromMinutes(1).Ticks.ToString()
                }
            };

            // Predisposizione all'inserimento del canale di pubblicazione
            this.channelDetail.LoadData(this.IdAdmin, this.SelectedChannel);

            // Dettaglio delle configurazioni visualizzato
            this.SelectedDetail = SelectedDetailsEnum.ChannelConfigurationsDetails;
        }

        /// <summary>
        /// Azione di aggiornamento canali
        /// </summary>
        private void PerformActionRefreshChannels()
        {
            this.SelectedChannel = null;
            this.SelectedDetail = SelectedDetailsEnum.None;

            this.Fetch();
        }

        #endregion

        #region Data Management

        /// <summary>
        /// Tipi oggetti disponibili
        /// </summary>
        [Serializable()]
        protected class ObjectTypes
        {
            public const string FASCICOLO = "Fascicolo";
            public const string DOCUMENTO = "Documento";
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        protected class LogItemInfo
        {
            public string Codice { get; set; }
            public string Descrizione { get; set; }
            public string Oggetto { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected class ChannelServiceStateDescriptions
        {
            public const string Started = "Avviato";
            public const string Stopped = "Fermato";
            public const string UnexpectedStopped = "Fermo non valido";
        }


        /// <summary>
        /// Id amministrazione correntemente selezionata
        /// </summary>
        private int IdAdmin
        {
            get
            {
                string val = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                int idAdmin;
                Int32.TryParse(val, out idAdmin);
                return idAdmin;
            }
        }

        /// <summary>
        /// Codice amministrazione correntemente selezionata
        /// </summary>
        private string AdminCode
        {
            get
            {
                return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            }
        }

        /// <summary>
        /// Reperimento Id del canale di pubblicazione presente nell'elemento della griglia 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int GetChannelId(DataGridItem item)
        {
            int id;
            Int32.TryParse(((Label)item.FindControl("lblId")).Text, out id);
            return id;
        }

        /// <summary>
        /// Reperimento dati canale di pubblicazione
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Publisher.Proxy.ChannelRefInfo GetChannel(int id)
        {
            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
                return ws.GetChannel(id);
        }

        /// <summary>
        /// Reperimento id dell'evento correntemente selezionato
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int GetEventId(DataGridItem item)
        {
            int id;
            Int32.TryParse(((Label)item.FindControl("lblId")).Text, out id);
            return id;
        }

        /// <summary>
        /// Canale correntemente selezionato
        /// </summary>
        private Publisher.Proxy.ChannelRefInfo SelectedChannel
        {
            get
            {
                return this.ViewState["SelectedChannel"] as Publisher.Proxy.ChannelRefInfo;
            }
            set
            {
                this.ViewState["SelectedChannel"] = value;
            }
        }

        /// <summary>
        /// Modello "Campi comuni"
        /// </summary>
        private const string TEMPLATE_CAMPI_COMUNI = "CAMPI COMUNI";

        /// <summary>
        /// Reperimento profili fascicolo
        /// </summary>
        /// <returns></returns>
        private string[] GetTemplatesFascicolo()
        {
            if (this.ViewState["GetTemplatesFascicolo"] == null)
            {
                // Caricamento profili fascicolo disponibili nel sistema
                System.Collections.ArrayList list = ProfilazioneFascManager.getTemplatesFasc(this.IdAdmin.ToString(), this);

                DocsPaWR.Templates[] templates = (DocsPaWR.Templates[])list.ToArray(typeof(DocsPaWR.Templates));

                // Dalla lista dei modelli viene scartato il modello "Campi comuni"
                templates = templates.Where(e => e.DESCRIZIONE.ToUpperInvariant() != TEMPLATE_CAMPI_COMUNI).ToArray();

                List<string> listTemplates = templates.Select(e => e.DESCRIZIONE).ToList();
                listTemplates.Insert(0, string.Empty);
                this.ViewState["GetTemplatesFascicolo"] = listTemplates.ToArray();
            }

            return (string[])this.ViewState["GetTemplatesFascicolo"];
        }

        /// <summary>
        /// Reperimento profili documento
        /// </summary>
        /// <returns></returns>
        private string[] GetTemplatesDocumento()
        {
            if (this.ViewState["GetTemplatesDocumento"] == null)
            {
                // Caricamento profili fascicolo disponibili nel sistema
                System.Collections.ArrayList list = ProfilazioneDocManager.getTemplates(this.IdAdmin.ToString(), this);

                DocsPaWR.Templates[] templates = (DocsPaWR.Templates[])list.ToArray(typeof(DocsPaWR.Templates));

                // Dalla lista dei modelli viene scartato il modello "Campi comuni"
                templates = templates.Where(e => e.DESCRIZIONE.ToUpperInvariant() != TEMPLATE_CAMPI_COMUNI).ToArray();

                List<string> listTemplates = templates.Select(e => e.DESCRIZIONE).ToList();
                listTemplates.Insert(0, string.Empty);
                this.ViewState["GetTemplatesDocumento"] = listTemplates.ToArray();
            }

            return (string[])this.ViewState["GetTemplatesDocumento"];
        }

        /// <summary>
        /// Reperimento log configurabili per il tipo oggetto
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private LogItemInfo[] GetLogItemsPerObjectType(string objectType)
        {
            List<LogItemInfo> logs = null;

            if (this.ViewState["LogItems"] == null)
            {
                logs = new List<LogItemInfo>();

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string xmlStream = ws.GetXmlLog(this.AdminCode);

                if (!string.IsNullOrEmpty(xmlStream))
                {
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(xmlStream);

                    foreach (System.Xml.XmlNode azione in xmlDoc.SelectSingleNode("NewDataSet").ChildNodes)
                    {
                        if (azione.ChildNodes[4].InnerText.Equals("1"))
                        {
                            //carica il dataset
                            logs.Add(
                                new LogItemInfo
                                {
                                    Codice = azione.ChildNodes[0].InnerText,
                                    Oggetto = azione.ChildNodes[2].InnerText,
                                    Descrizione = azione.ChildNodes[1].InnerText,
                                }

                            );
                        }
                    }

                    this.ViewState["LogItems"] = logs;
                }
            }

            logs = (List<LogItemInfo>)this.ViewState["LogItems"];

            LogItemInfo[] retValue = logs.Where(e => e.Oggetto.ToLowerInvariant().Trim() == objectType.ToLowerInvariant().Trim()).ToArray<LogItemInfo>();

            return retValue;
        }

        /// <summary>
        /// Caricamento canali
        /// </summary>
        private void Fetch()
        {
            using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            {
                this.grdChannels.DataSource = ws.GetAdminChannelList(this.IdAdmin);
                this.grdChannels.DataBind();
            }
        }

        /// <summary>
        /// Caricamento eventi del canale di pubblicazione
        /// </summary>
        /// <param name="idChannel"></param>
        private void FetchEvents(int idChannel)
        {
            this.grdEvents.DataSource = this.SelectedChannel.Events.OrderBy(e => e.ObjectTemplateName).ToArray();
            this.grdEvents.DataBind();

            //using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            //{
            //    this.grdEvents.DataSource = ws.GetEventList(idChannel);
            //    this.grdEvents.DataBind();
            //}
        }

        /// <summary>
        /// Caricamento tipi oggetti disponibili
        /// </summary>
        /// <param name="item"></param>
        private void FetchObjectTypes(DataGridItem item)
        {
            DropDownList cboObjectTypes = this.GetObjectTypesDropDown(item);

            cboObjectTypes.Items.Clear();
            cboObjectTypes.Items.Add(new ListItem(ObjectTypes.FASCICOLO, ObjectTypes.FASCICOLO));
            cboObjectTypes.Items.Add(new ListItem(ObjectTypes.DOCUMENTO, ObjectTypes.DOCUMENTO));
        }

        /// <summary>
        /// Caricamento template oggetto selezionato
        /// </summary>
        /// <param name="item"></param>
        private void FetchObjectTemplates(DataGridItem item)
        {
            DropDownList cboObjectTypes = this.GetObjectTypesDropDown(item);
            DropDownList cboObjectTemplates = this.GetObjectTemplatesDropDown(item);

            if (cboObjectTypes.SelectedValue == ObjectTypes.FASCICOLO)
            {
                cboObjectTemplates.DataSource = this.GetTemplatesFascicolo();
                cboObjectTemplates.DataBind();
            }
            else if (cboObjectTypes.SelectedValue == ObjectTypes.DOCUMENTO)
            {
                // Caricamento profili documento disponibili nel sistema
                cboObjectTemplates.DataSource = this.GetTemplatesDocumento();
                cboObjectTemplates.DataBind();
            }
        }

        /// <summary>
        /// Caricamento log configurabili per il tipo oggetto
        /// </summary>
        private void FetchObjectLogs(DataGridItem item)
        {
            DropDownList cboObjectTypes = this.GetObjectTypesDropDown(item);
            DropDownList cboLogEvents = this.GetObjectLogsDropDown(item);

            cboLogEvents.DataSource = this.GetLogItemsPerObjectType(cboObjectTypes.SelectedValue);
            cboLogEvents.DataBind();
        }

        /// <summary>
        /// Caricamento dati dell'evento
        /// </summary>
        /// <param name="e"></param>
        private void FetchEvent(DataGridItem item)
        {
            int eventId = this.GetEventId(item);

            Publisher.Proxy.EventInfo eventInfo = null;

            if (eventId > 0)
            {
                // Caricamento dati evento selezionato
                eventInfo = this.GetEvent(eventId);
            }

            // Caricamento tipi oggetto
            this.FetchObjectTypes(item);

            if (eventId > 0)
            {
                // Selezione tipo oggetto
                DropDownList cboObjectTypes = this.GetObjectTypesDropDown(item);
                cboObjectTypes.SelectedValue = eventInfo.ObjectType;
            }

            // Caricamento profili oggetto
            this.FetchObjectTemplates(item);

            if (eventId > 0)
            {
                // Selezione profilo oggetto
                DropDownList cboObjectTemplates = this.GetObjectTemplatesDropDown(item);
                cboObjectTemplates.SelectedValue = eventInfo.ObjectTemplateName;
            }


            // Caricamento log oggetto
            this.FetchObjectLogs(item);

            if (eventId > 0)
            {
                // Selezione log
                DropDownList cboLogEvents = this.GetObjectLogsDropDown(item);
                cboLogEvents.SelectedValue = eventInfo.EventName;
            }
        }

        /// <summary>
        /// Caricamento dati di un evento
        /// </summary>
        /// <param name="idEvent"></param>
        /// <returns></returns>
        private Publisher.Proxy.EventInfo GetEvent(int idEvent)
        {
            return this.SelectedChannel.Events.Where(e => e.Id == idEvent).FirstOrDefault();
            //using (Publisher.Proxy.PublisherWebService ws = PublisherServiceFactory.Create())
            //    return ws.GetEvent(idEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected string GetServiceState(Publisher.Proxy.ChannelRefInfo instance)
        {
            string serviceState = string.Empty;

            if (instance.State == Publisher.Proxy.ChannelStateEnum.Started)
            {
                serviceState = string.Format("Servizio avviato dal: {0}<BR />Server: {1}<BR />Ultima pubbl.: {2}",
                                        instance.StartExecutionDate.ToString(),
                                        instance.MachineName,
                                        instance.LastExecutionDate.ToString());
            }
            else if (instance.State == Publisher.Proxy.ChannelStateEnum.UnexpectedStopped)
            {
                serviceState = string.Format("Servizio in stato fermo non valido<BR />Server: {0}<BR />Ultima pubbl.: {1}",
                                        instance.StartExecutionDate.ToString(),
                                        instance.MachineName,
                                        instance.LastExecutionDate.ToString());
            }
            else
            {
                serviceState = string.Format("Servizio fermato dal: {0}<BR />Ultima pubbl.: {1}",
                                        instance.EndExecutionDate.ToString(),
                                        instance.LastExecutionDate.ToString());
            }

            return serviceState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected string GetServiceStartDate(Publisher.Proxy.ChannelRefInfo instance)
        {
            if (instance.StartExecutionDate == DateTime.MinValue)
                return string.Empty;
            else
                return instance.StartExecutionDate.ToString();
        }

        #endregion

        #region UI Management

        /// <summary>
        /// Dettagli grafici del canale
        /// </summary>
        protected enum SelectedDetailsEnum
        {
            None,
            ChannelEventsDetail,
            ChannelConfigurationsDetails,
        }

        /// <summary>
        /// Dettaglio correntemente selezionato
        /// </summary>
        private SelectedDetailsEnum SelectedDetail
        {
            get
            {
                if (this.ViewState["SelectedDetail"] == null)
                    return SelectedDetailsEnum.None;
                else
                    return (SelectedDetailsEnum)this.ViewState["SelectedDetail"];
            }
            set
            {
                this.ViewState["SelectedDetail"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        protected string GetServiceStateImageName(Publisher.Proxy.ChannelRefInfo channelRef)
        {
            if (channelRef.State == Publisher.Proxy.ChannelStateEnum.Started)
                return "~/AdminTool/Images/started.gif";
            else if (channelRef.State == Publisher.Proxy.ChannelStateEnum.UnexpectedStopped)
                return "~/AdminTool/Images/unexpectedStopped.gif";
            else
                return "~/AdminTool/Images/stopped.gif";
        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void ShowErrorMessage(string errorMessage)
        {
            if (!this.ClientScript.IsStartupScriptRegistered(this.GetType(), "ErrorMessage"))
                this.ClientScript.RegisterStartupScript(this.GetType(),
                        "ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "');", true);
        }

        /// <summary>
        /// Repeirmento della descrizione dell'evento
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        protected string GetEventDescription(Publisher.Proxy.EventInfo eventInfo)
        {
            string eventDescription = eventInfo.EventName;

            LogItemInfo[] events = this.GetLogItemsPerObjectType(eventInfo.ObjectType);

            LogItemInfo log = events.Where(e => e.Codice.ToLowerInvariant().Trim() == eventInfo.EventName.ToLowerInvariant().Trim()).FirstOrDefault();

            if (log != null)
            {
                eventDescription = log.Descrizione;
            }
            else
            {
                ShowErrorMessage(String.Format("Attenzione l'evento {0} non è piu configurato nel log manager, riabilitarlo o rimuovere l'evento monitorato", eventDescription));
            }

            return eventDescription;
        }

        /// <summary>
        /// Aggiornamento controlli UI
        /// </summary>
        protected void RefreshControlsEnabled()
        {
            if (this.SelectedChannel == null)
            {
                this.grdChannels.EditItemIndex = -1;
                this.grdChannels.SelectedIndex = -1;
            }

            this.tblChannelConfigurations.Visible = false;
            this.tblChannelEvents.Visible = false;

            // Rende visibile il pannello degli eventi solo se è stato selezionato almeno un canale di pubblicazione
            if (this.SelectedDetail == SelectedDetailsEnum.ChannelEventsDetail)
            {
                // Visibile il pannello della configurazione degli eventi
                this.tblChannelConfigurations.Visible = false;
                this.tblChannelEvents.Visible = true;

                if (this.SelectedChannel != null)
                {
                    // Disabilitazione del pulsante di inserimento se
                    // è selezionato un canale e se tale canale è fermo
                    this.btnNewEvent.Visible = (this.SelectedChannel.State == Publisher.Proxy.ChannelStateEnum.Stopped);
                }
            }
            else if (this.SelectedDetail == SelectedDetailsEnum.ChannelConfigurationsDetails)
            {
                // Visibile il pannello della configurazione del canale
                this.tblChannelConfigurations.Visible = true;
                this.tblChannelEvents.Visible = false;

                if (this.SelectedChannel != null)
                {
                    // Disabilitazione del pulsante di modifica se
                    // è selezionato un canale e se tale canale è fermo
                    this.btnSaveChannel.Visible = (this.SelectedChannel.State == Publisher.Proxy.ChannelStateEnum.Stopped);
                    this.channelDetail.Enabled = this.btnSaveChannel.Visible;
                }
            }

            // Aggiornamento abilitazione controlli
            this.RefreshEventsGridActionsControls();
        }

        ///// <summary>
        ///// Verifica se il canale correntemente selezionato è fermo o meno
        ///// </summary>
        ///// <returns></returns>
        //private bool ChannelIsStopped(DataGridItem item)
        //{
        //    //HiddenField hdChannelState = (HiddenField)item.FindControl("hdChannelState");
        //    //Publisher.Proxy.ChannelStateEnum state = (Publisher.Proxy.ChannelStateEnum)
        //    //    Enum.Parse(typeof(Publisher.Proxy.ChannelStateEnum), hdChannelState.Value, true);

            


        //    //return (state == Publisher.Proxy.ChannelStateEnum.Stopped);
        //}

        /// <summary>
        /// Aggiornamento abilitazione azioni della griglia
        /// </summary>
        /// <param name="item"></param>
        protected void RefreshEventsGridActionsControls()
        {
            if (this.SelectedChannel != null)
            {
                bool channelIsStopped = (this.SelectedChannel.State == Publisher.Proxy.ChannelStateEnum.Stopped); // this.ChannelIsStopped(this.grdChannels.SelectedItem);

                foreach (DataGridItem item in this.grdEvents.Items)
                {
                    bool editMode = (item.ItemIndex == this.grdEvents.EditItemIndex);

                    // Azioni degli eventi
                    //<!--
                    //<asp:ImageButton ID="btnEdit" runat="server" CommandName="Edit" ToolTip="Modifica" ImageUrl="~/AdminTool/Images/matita.gif" />
                    //<asp:ImageButton ID="btnCancel" runat="server" CommandName="Cancel" ToolTip="Annulla modifiche" ImageUrl="~/AdminTool/Images/chiudi.gif" />
                    //<asp:ImageButton ID="btnUpdate" runat="server" CommandName="Update" ToolTip="Salva modifiche" ImageUrl="~/AdminTool/Images/salva.gif" />
                    //<asp:ImageButton ID="btnDelete" runat="server" CommandName="Delete" ToolTip="Rimuovi evento" ImageUrl="~/AdminTool/Images/cestino.gif" />
                    //-->
                    //ImageButton btnEdit = (ImageButton)item.FindControl("btnEdit");
                    //ImageButton btnCancel = (ImageButton)item.FindControl("btnCancel");
                    //ImageButton btnUpdate = (ImageButton)item.FindControl("btnUpdate");
                    //ImageButton btnDelete = (ImageButton)item.FindControl("btnDelete");

                    Button btnEdit = (Button)item.FindControl("btnEdit");
                    Button btnCancel = (Button)item.FindControl("btnCancel");
                    Button btnUpdate = (Button)item.FindControl("btnUpdate");
                    Button btnDelete = (Button)item.FindControl("btnDelete");

                    btnEdit.Visible = channelIsStopped && !editMode;
                    btnCancel.Visible = channelIsStopped && editMode;
                    btnUpdate.Visible = channelIsStopped && editMode;
                    btnDelete.Visible = channelIsStopped && !editMode;

                    // Visibile solo se l'evento si riferisce ad un documento
                    if (channelIsStopped && editMode)
                    {
                        CheckBox chkLoadFile = (CheckBox)item.FindControl("chkLoadFile");
                        DropDownList cboObjectTypes = this.GetObjectTypesDropDown(item);
                        chkLoadFile.Visible = (cboObjectTypes.SelectedValue == ObjectTypes.DOCUMENTO);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected string GetTextEventsButton(Publisher.Proxy.ChannelRefInfo channel)
        {
            return string.Format("Eventi ({0})", channel.Events.Length.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DropDownList GetObjectTypesDropDown(DataGridItem item)
        {
            return (DropDownList)item.FindControl("cboObjectTypes");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DropDownList GetObjectTemplatesDropDown(DataGridItem item)
        {
            return (DropDownList)item.FindControl("cboObjectTemplates");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DropDownList GetObjectLogsDropDown(DataGridItem item)
        {
            return (DropDownList)item.FindControl("cboLogEvents");
        }

        /// <summary>
        /// Bug paginazione datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDataGridItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        #endregion
    }
}
