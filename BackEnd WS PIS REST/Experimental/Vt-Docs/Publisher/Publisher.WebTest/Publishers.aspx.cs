using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Publisher.WebTest
{
    public partial class Publishers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.FetchAdmin();

                this.Fetch();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboAdminList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.Fetch();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void FetchAdmin()
        {
            using (PITRE.DocsPaWebService ws = new PITRE.DocsPaWebService())
            {
                string msg;
                this.cboAdminList.DataValueField = "systemId";
                this.cboAdminList.DataTextField = "codice";
                this.cboAdminList.DataSource = ws.amministrazioneGetAmministrazioni(out msg);
                this.cboAdminList.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtSubscriberServiceUrl_TextChanged(object sender, EventArgs e)
        {
            DataGridItem item = this.grdInstances.Items[this.grdInstances.EditItemIndex];

            this.FetchSubscribers(item);
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdInstances_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int id;
            Int32.TryParse(((Label)e.Item.FindControl("lblId")).Text, out id);

            if (e.CommandName == "Select")
            {   
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    this.grdInstances.EditItemIndex = e.Item.ItemIndex;
                    this.Fetch();

                    this.FetchSubscribers(this.grdInstances.Items[this.grdInstances.EditItemIndex]);

                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(id);
                }
            }
            else if (e.CommandName == "FetchSubscribers")
            {
                this.FetchSubscribers(e.Item);
            }
            else if (e.CommandName == "Start")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(id);

                    ws.StartChannel(instance);

                    this.grdInstances.EditItemIndex = -1;
                    this.Fetch();

                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(id);
                }
            }
            else if (e.CommandName == "Stop")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(id);

                    ws.StopChannel(instance);

                    this.grdInstances.EditItemIndex = -1;
                    this.Fetch();

                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(id);
                }
            }
            else if (e.CommandName == "Update")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    Publisher.Proxy.ChannelRefInfo instance = null;

                    if (id != 0)
                    {
                        instance = ws.GetChannel(id);
                    }
                    else
                    {
                        instance = new Proxy.ChannelRefInfo
                        {
                            Admin = new Proxy.AdminInfo
                            {
                                Id = this.IdAdmin
                            }
                        };
                    }

                    instance.SubscriberServiceUrl = Properties.Settings.Default.SubscriberWebServices;
                    instance.ChannelName = ((DropDownList) e.Item.FindControl("cboSubscribers")).SelectedItem.Text;

                    TextBox txtExecutionInterval = (TextBox)e.Item.FindControl("txtExecutionInterval");
                    if (txtExecutionInterval != null)
                    {
                        int interval;
                        Int32.TryParse(txtExecutionInterval.Text, out interval);

                        instance.ExecutionConfiguration = new Proxy.JobExecutionConfigurations
                        {
                             IntervalType = Proxy.IntervalTypesEnum.BySecond,
                             ExecutionTicks = TimeSpan.FromSeconds(interval).Ticks.ToString()
                        };
                    }
                    
                    TextBox txtStartLogDate = (TextBox)e.Item.FindControl("txtStartLogDate");
                    if (txtStartLogDate != null)
                    {
                        DateTime startLogDate;
                        DateTime.TryParse(txtStartLogDate.Text, out startLogDate);
                        instance.StartLogDate = startLogDate;
                    }

                    instance = ws.SaveChannel(instance);

                    this.grdInstances.EditItemIndex = -1;
                    this.Fetch();

                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(id);
                }
            }
            else if (e.CommandName == "Delete")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    if (id != 0)
                    {
                        Publisher.Proxy.ChannelRefInfo instance = ws.GetChannel(id);

                        ws.RemoveChannel(instance);

                        this.grdInstances.EditItemIndex = -1;
                        this.Fetch();

                        this.grdEvents.EditItemIndex = -1;
                        this.FetchEvents(id);
                    }
                }
            }
            else if (e.CommandName == "Cancel")
            {
                this.grdInstances.EditItemIndex = -1;
                this.Fetch();

                this.grdEvents.EditItemIndex = -1;
                this.FetchEvents(id);
            }
            else if (e.CommandName == "GoToSubscriber")
            {
                Label lblSubscriberServiceUrl = (Label) e.Item.FindControl("lblSubscriberServiceUrl");

                if (lblSubscriberServiceUrl != null)
                {
                    Response.Redirect(string.Format("Subscribers.aspx?subscriberUrl={0}&idAdmin={1}&caller=Publishers.aspx", 
                                lblSubscriberServiceUrl.Text, this.IdAdmin), false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected string GetExecutionInterval(Publisher.Proxy.ChannelRefInfo instance)
        {
            if (instance.Id != 0)
            {
                long ticks;
                long.TryParse(instance.ExecutionConfiguration.ExecutionTicks, out ticks);
                return TimeSpan.FromTicks(ticks).TotalSeconds.ToString();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdEvents_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int id;
            Int32.TryParse(((Label)e.Item.FindControl("lblId")).Text, out id);

            int idInstance;
            Int32.TryParse(((Label)e.Item.FindControl("lblIdInstance")).Text, out idInstance);

            if (e.CommandName == "Select")
            {
                this.grdEvents.EditItemIndex = e.Item.ItemIndex;

                this.FetchEvents(idInstance);
            }
            else if (e.CommandName == "Delete")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    Publisher.Proxy.EventInfo ev = ws.GetEvent(id);
                    ev = ws.RemoveEvent(ev);
                    
                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(idInstance);
                }

            }
            else if (e.CommandName == "Update")
            {
                using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
                {
                    ws.Url = Properties.Settings.Default.PublisherWebServices;

                    Publisher.Proxy.EventInfo ev = null;

                    if (id != 0)
                    {
                        ev = ws.GetEvent(id);
                    }
                    else
                    {
                        Int32.TryParse(((Label)this.grdInstances.SelectedItem.FindControl("lblId")).Text, out idInstance);

                        ev = new Proxy.EventInfo
                        {
                            IdChannel = idInstance
                        };
                    }

                    ev.ObjectType = ((TextBox)e.Item.FindControl("txtObjectType")).Text;
                    ev.ObjectTemplateName = ((TextBox)e.Item.FindControl("txtObjectTemplateName")).Text;
                    ev.EventName = ((TextBox)e.Item.FindControl("txtEventName")).Text;
                    ev.DataMapperFullClass = ((TextBox)e.Item.FindControl("txtDataMapper")).Text;
                    ev.LoadFileIfDocumentType = ((CheckBox)e.Item.FindControl("chkLoadFile")).Checked;

                    ev = ws.SaveEvent(ev);

                    this.grdEvents.EditItemIndex = -1;
                    this.FetchEvents(idInstance);
                }
            }
            else if (e.CommandName == "Cancel")
            {
                this.grdEvents.EditItemIndex = -1;
                this.FetchEvents(idInstance);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.Response.Redirect(this.Request.QueryString["caller"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNewInstance_Click(object sender, EventArgs e)
        {
            this.grdInstances.EditItemIndex = 0;
            this.grdInstances.DataSource = new Publisher.Proxy.ChannelRefInfo[1] { new Publisher.Proxy.ChannelRefInfo() };
            this.grdInstances.DataBind();

            this.FetchSubscribers(this.grdInstances.Items[0]);

            this.ClearEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.grdInstances.EditItemIndex = -1;

            this.Fetch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNewEvent_Click(object sender, EventArgs e)
        {
            int idInstance;
            Int32.TryParse(((Label)this.grdInstances.SelectedItem.FindControl("lblId")).Text, out idInstance);

            this.grdEvents.EditItemIndex = 0;
            this.grdEvents.DataSource = new Publisher.Proxy.EventInfo[1] { new Publisher.Proxy.EventInfo { IdChannel = idInstance } };
            this.grdEvents.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRefreshEvents_Click(object sender, EventArgs e)
        {
            int idInstance;
            Int32.TryParse(((Label)this.grdInstances.SelectedItem.FindControl("lblId")).Text, out idInstance);

            if (idInstance > 0)
            {
                this.grdEvents.EditItemIndex = -1;

                this.FetchEvents(idInstance);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int IdAdmin
        {
            get
            {
                int id;
                Int32.TryParse(this.cboAdminList.SelectedValue, out id);
                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Fetch()
        {
            using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
            {
                ws.Url = Properties.Settings.Default.PublisherWebServices;

                this.grdInstances.DataSource = ws.GetAdminChannelList(this.IdAdmin);
                this.grdInstances.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearEvents()
        {
            this.grdEvents.EditItemIndex = -1;
            this.grdEvents.DataSource = null;
            this.grdEvents.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        protected void FetchSubscribers(DataGridItem item)
        {
            DropDownList cboSubscribers = (DropDownList)item.FindControl("cboSubscribers");

            if (cboSubscribers != null)
            {
                // Caricamento dei sottoscrittori configurati e raggiungibili tramite l'url indicato
                using (Subscriber.Proxy.SubscriberWebService ws = new Subscriber.Proxy.SubscriberWebService())
                {
                    ws.Url = Properties.Settings.Default.SubscriberWebServices;

                    cboSubscribers.DataValueField = "Name";
                    cboSubscribers.DataTextField = "Name";

                    cboSubscribers.DataSource = ws.GetChannelList();
                    cboSubscribers.DataBind();

                    HiddenField hdSubscriber = (HiddenField)item.FindControl("hdSubscriber");

                    if (hdSubscriber != null && !string.IsNullOrEmpty(hdSubscriber.Value))
                    {
                        cboSubscribers.SelectedValue = hdSubscriber.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInstance"></param>
        private void FetchEvents(int idInstance)
        {
            using (Publisher.Proxy.PublisherWebService ws = new Publisher.Proxy.PublisherWebService())
            {
                ws.Url = Properties.Settings.Default.PublisherWebServices;

                this.grdEvents.DataSource = ws.GetEventList(idInstance);
                this.grdEvents.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected string GetServiceState(Publisher.Proxy.ChannelRefInfo instance)
        {
            
            if (instance.State == Proxy.ChannelStateEnum.Started)
                return "Avviato";
            else
                return "Fermato";
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
    }
}