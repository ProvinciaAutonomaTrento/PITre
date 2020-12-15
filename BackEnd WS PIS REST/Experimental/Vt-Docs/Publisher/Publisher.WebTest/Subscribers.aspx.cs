using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Publisher.WebTest
{
    public partial class Subscribers : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.FetchInstances();
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
        protected void btnInsertInstance_Click(object sender, EventArgs e)
        {
            this.grdInstances.EditItemIndex = 0;

            this.grdInstances.DataSource = new Subscriber.Proxy.ChannelInfo[1] { new Subscriber.Proxy.ChannelInfo() };
            this.grdInstances.DataBind();

            this.ClearRules();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInsertRule_Click(object sender, EventArgs e)
        {
            this.grdRules.EditItemIndex = 0;

            this.grdRules.DataSource = new Subscriber.Proxy.RuleInfo[1] { new Subscriber.Proxy.RuleInfo() };
            this.grdRules.DataBind();

            this.ClearSubRules();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInsertRuleOption_Click(object sender, EventArgs e)
        {
            this.grdRuleOptions.EditItemIndex = 0;

            this.grdRuleOptions.DataSource = new Subscriber.Proxy.NameValuePair[1] { new Subscriber.Proxy.NameValuePair() };
            this.grdRuleOptions.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInsertSubRule_Click(object sender, EventArgs e)
        {
            this.grdSubRules.EditItemIndex = 0;

            this.grdSubRules.DataSource = new Subscriber.Proxy.SubRuleInfo[1] { new Subscriber.Proxy.SubRuleInfo() };
            this.grdSubRules.DataBind();

            this.ClearRuleHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdInstances_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                this.grdInstances.EditItemIndex = e.Item.ItemIndex;

                this.FetchInstances();

                Label lblId = (Label)e.Item.FindControl("lblId");

                if (!string.IsNullOrEmpty(lblId.Text))
                    this.FetchRules(Convert.ToInt32(lblId.Text));
            }
            else if (e.CommandName == "Edit")
            {
            }
            else if (e.CommandName == "Update")
            {
                Label lblId = (Label)e.Item.FindControl("lblId");

                int id = Convert.ToInt32(lblId.Text);

                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    Subscriber.Proxy.ChannelInfo instance = new Subscriber.Proxy.ChannelInfo();
                    instance.Id = id;
                    instance.Name = ((TextBox)e.Item.FindControl("txtName")).Text;
                    instance.Description = ((TextBox)e.Item.FindControl("txtDescription")).Text;
                    instance.SmtpHost = ((TextBox)e.Item.FindControl("txtSmtpHost")).Text;
                    instance.SmtpPort = Convert.ToInt32(((TextBox)e.Item.FindControl("txtSmtpPort")).Text);
                    instance.SmtpSsl = ((CheckBox)e.Item.FindControl("txtSmtpSsl")).Checked;
                    instance.SmtpUserName = ((TextBox)e.Item.FindControl("txtSmtpUserName")).Text;
                    instance.SmtpPassword = ((TextBox)e.Item.FindControl("txtSmtpPassword")).Text;
                    instance.SmtpMail = ((TextBox)e.Item.FindControl("txtSmtpMail")).Text;

                    instance = da.SaveChannel(instance);
                }

                this.grdInstances.EditItemIndex = -1;

                this.FetchInstances();
            }
            else if (e.CommandName == "Delete")
            {
                Label lblId = (Label)e.Item.FindControl("lblId");

                int id = Convert.ToInt32(lblId.Text);

                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    da.DeleteChannel(id);
                }

                this.FetchInstances();
            }
            else if (e.CommandName == "Cancel")
            {
                this.grdInstances.EditItemIndex = -1;

                this.FetchInstances();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdRules_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Label lblId = (Label)e.Item.FindControl("lblId");
            int id;
            Int32.TryParse(lblId.Text, out id);

            Label lblIdInstance = (Label)e.Item.FindControl("lblIdInstance");
            int idInstance;
            Int32.TryParse(lblIdInstance.Text, out idInstance);

            if (e.CommandName == "Select")
            {
                this.grdRules.EditItemIndex = e.Item.ItemIndex;

                this.FetchRules(idInstance);

                this.FetchSubRules(id);

                this.FetchRuleOptions(id);

                this.FetchRuleHistory(id);
            }
            else if (e.CommandName == "Update")
            {
                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    Subscriber.Proxy.RuleInfo rule = null;

                    if (id > 0)
                    {
                        rule = da.GetRule(id);
                    }
                    else
                    {
                        lblIdInstance = (Label)this.grdInstances.SelectedItem.FindControl("lblId");
                        Int32.TryParse(lblIdInstance.Text, out idInstance);

                        rule = new Subscriber.Proxy.RuleInfo();
                        rule.Id = id;
                        rule.RuleName = ((TextBox)e.Item.FindControl("txtRuleName")).Text;
                        rule.IdInstance = idInstance;
                    }
                                        
                    rule.RuleDescription = ((TextBox)e.Item.FindControl("txtRuleDescription")).Text;
                    rule.Enabled = ((CheckBox)e.Item.FindControl("chkRuleEnabled")).Checked;
                    rule.RuleClassFullName = ((TextBox)e.Item.FindControl("txtRuleClassFullName")).Text;

                    rule = da.SaveRule(rule);
                }

                this.grdRules.EditItemIndex = -1;

                this.FetchRules(idInstance);
            }
            else if (e.CommandName == "Cancel")
            {
                this.grdRules.EditItemIndex = -1;

                this.FetchRules(idInstance);
            }
            else if (e.CommandName == "Delete")
            {
                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    da.DeleteRule(id);

                    this.grdRules.EditItemIndex = -1;

                    lblIdInstance = (Label)this.grdInstances.SelectedItem.FindControl("lblId");
                    Int32.TryParse(lblIdInstance.Text, out idInstance);
                    this.FetchRules(idInstance);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdSubRules_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Label lblId = (Label)e.Item.FindControl("lblId");
            int id;
            Int32.TryParse(lblId.Text, out id);

            if (e.CommandName == "Select")
            {
                Label lblIdParentRule = (Label)e.Item.FindControl("lblIdParentRule");

                this.grdSubRules.EditItemIndex = e.Item.ItemIndex;

                this.FetchSubRules(Convert.ToInt32(lblIdParentRule.Text));

                this.FetchSubRuleOptions(id);

                this.FetchRuleHistory(id);
            }
            else if (e.CommandName == "Update")
            {
                Label lblIdParentRule = (Label)this.grdRules.SelectedItem.FindControl("lblId");
                int idParentRule = Convert.ToInt32(lblIdParentRule.Text);

                Label lblIdInstance = (Label)this.grdInstances.SelectedItem.FindControl("lblId");
                int idInstance = Convert.ToInt32(lblIdInstance.Text);

                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    Subscriber.Proxy.SubRuleInfo rule = null;

                    if (id > 0)
                    {
                        rule = da.GetSubRule(id);
                    }
                    else
                    {
                        rule = new Subscriber.Proxy.SubRuleInfo();
                        rule.Id = id;

                        rule.IdInstance = idInstance;
                        rule.IdParentRule = idParentRule;
                        rule.SubRuleName = ((TextBox)e.Item.FindControl("txtSubRuleName")).Text;
                    }
                    
                    rule.RuleDescription = ((TextBox)e.Item.FindControl("txtRuleDescription")).Text;
                    rule.Enabled = ((CheckBox)e.Item.FindControl("chkRuleEnabled")).Checked;

                    rule = da.SaveSubRule(rule);
                }

                this.grdSubRules.EditItemIndex = -1;

                this.FetchSubRules(idParentRule);
            }
            else if (e.CommandName == "Cancel")
            {
                Label lblIdParentRule = (Label)this.grdRules.SelectedItem.FindControl("lblId");
                int idParentRule = Convert.ToInt32(lblIdParentRule.Text);

                this.grdSubRules.EditItemIndex = -1;

                this.FetchSubRules(idParentRule);
            }
            else if (e.CommandName == "Delete")
            {
                Label lblIdParentRule = (Label)e.Item.FindControl("lblIdParentRule");
                int idParentRule = Convert.ToInt32(lblIdParentRule.Text);

                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    da.DeleteRule(id);
                }

                this.grdSubRules.EditItemIndex = -1;

                this.FetchSubRules(idParentRule);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdRuleOptions_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int idRule;
            Int32.TryParse(this.hdParentRuleId.Value, out idRule);

            bool isSubRule;
            bool.TryParse(this.hdIsSubRule.Value, out isSubRule);

            if (e.CommandName == "Select")
            {
                //this.grdRuleOptions.EditItemIndex = e.Item.ItemIndex;

                //this.FetchRuleOptions(idRule);
            }
            else if (e.CommandName == "Update")
            {
                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    if (isSubRule)
                    {
                        Subscriber.Proxy.SubRuleInfo rule = da.GetSubRule(idRule);

                        string name = ((TextBox)e.Item.FindControl("txtName")).Text;
                        string value = ((TextBox)e.Item.FindControl("txtValue")).Text;

                        List<Subscriber.Proxy.NameValuePair> pairs = new List<Subscriber.Proxy.NameValuePair>(rule.Options);
                        Subscriber.Proxy.NameValuePair pair = pairs.Where(itm => itm.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault();

                        if (pair == null)
                        {
                            pair = new Subscriber.Proxy.NameValuePair();
                            pairs.Add(pair);
                        }

                        pair.Name = name;
                        pair.Value = value;

                        rule.Options = pairs.ToArray();

                        rule = da.SaveSubRule(rule);

                        this.grdRuleOptions.EditItemIndex = -1;
                        this.FetchSubRuleOptions(idRule);
                    }
                    else
                    {
                        Subscriber.Proxy.RuleInfo rule = da.GetRule(idRule);

                        string name = ((TextBox)e.Item.FindControl("txtName")).Text;
                        string value = ((TextBox)e.Item.FindControl("txtValue")).Text;

                        List<Subscriber.Proxy.NameValuePair> pairs = new List<Subscriber.Proxy.NameValuePair>(rule.Options);
                        Subscriber.Proxy.NameValuePair pair = pairs.Where(itm => itm.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault();

                        if (pair == null)
                        {
                            pair = new Subscriber.Proxy.NameValuePair();
                            pairs.Add(pair);
                        }

                        pair.Name = name;
                        pair.Value = value;

                        rule.Options = pairs.ToArray();

                        rule = da.SaveRule(rule);

                        this.grdRuleOptions.EditItemIndex = -1;
                        this.FetchRuleOptions(idRule);
                    }
                }
            }
            else if (e.CommandName == "Cancel")
            {
                this.grdRuleOptions.EditItemIndex = -1;

                this.FetchRuleOptions(idRule);
            }
            else if (e.CommandName == "Delete")
            {
                using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
                {
                    if (isSubRule)
                    {
                        Subscriber.Proxy.SubRuleInfo rule = da.GetSubRule(idRule);

                        List<Subscriber.Proxy.NameValuePair> pairs = new List<Subscriber.Proxy.NameValuePair>(rule.Options);
                        pairs.RemoveAt(e.Item.ItemIndex);
                        rule.Options = pairs.ToArray();

                        rule = da.SaveSubRule(rule);

                        this.grdRuleOptions.EditItemIndex = -1;
                        this.FetchSubRuleOptions(idRule);
                    }
                    else
                    {
                        Subscriber.Proxy.RuleInfo rule = da.GetRule(idRule);

                        List<Subscriber.Proxy.NameValuePair> pairs = new List<Subscriber.Proxy.NameValuePair>(rule.Options);
                        pairs.RemoveAt(e.Item.ItemIndex);
                        rule.Options = pairs.ToArray();

                        rule = da.SaveRule(rule);

                        this.grdRuleOptions.EditItemIndex = -1;
                        this.FetchRuleOptions(idRule);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdRulesHistory_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                this.FetchRuleHistoryItem(Convert.ToInt32(e.Item.Cells[0].Text));
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
                Int32.TryParse(this.Request.QueryString["idAdmin"], out id);
                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string SubscriberServiceUrl
        {
            get
            {
                return Properties.Settings.Default.SubscriberWebServices;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Subscriber.Proxy.SubscriberWebService CreateService()
        {
            return new Subscriber.Proxy.SubscriberWebService
            {
                Url = this.SubscriberServiceUrl
            };
        }

        /// <summary>
        /// 
        /// </summary>
        protected void FetchInstances()
        {
            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                this.grdInstances.SelectedIndex = -1;
                this.grdInstances.DataSource = da.GetChannelList();
                this.grdInstances.DataBind();

                this.ClearRules();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInstance"></param>
        protected void FetchRules(int idInstance)
        {
            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                this.grdRules.SelectedIndex = -1;
                this.grdRules.DataSource = da.GetRules(idInstance);
                this.grdRules.DataBind();

                this.ClearRuleOptions();

                this.ClearSubRules();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRule"></param>
        protected void FetchSubRules(int idRule)
        {
            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                Subscriber.Proxy.RuleInfo rule = da.GetRule(idRule);

                this.grdSubRules.SelectedIndex = -1;
                this.grdSubRules.DataSource = rule.SubRules;
                this.grdSubRules.DataBind();

                this.ClearRuleOptions();

                this.ClearRuleHistory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRule"></param>
        protected void FetchRuleOptions(int idRule)
        {
            this.hdParentRuleId.Value = idRule.ToString();
            this.hdIsSubRule.Value = "false";

            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                Subscriber.Proxy.RuleInfo rule = da.GetRule(idRule);
                
                this.grdRuleOptions.DataSource = rule.Options;
                this.grdRuleOptions.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRule"></param>
        protected void FetchSubRuleOptions(int idRule)
        {
            this.hdParentRuleId.Value = idRule.ToString();
            this.hdIsSubRule.Value = "true";

            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                Subscriber.Proxy.SubRuleInfo rule = da.GetSubRule(idRule);
                
                this.grdRuleOptions.DataSource = rule.Options;
                this.grdRuleOptions.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearRuleOptions()
        {
            this.hdParentRuleId.Value = string.Empty;
            this.hdIsSubRule.Value = string.Empty;

            this.grdRuleOptions.DataSource = null;
            this.grdRuleOptions.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRule"></param>
        protected void FetchRuleHistory(int idRule)
        {
            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                Subscriber.Proxy.GetRuleHistoryListRequest request = new Subscriber.Proxy.GetRuleHistoryListRequest
                {
                    IdRule = idRule,
                    PagingContext = new Subscriber.Proxy.PagingContextInfo
                    {
                         PageNumber = 1,
                         ObjectsPerPage = Int32.MaxValue
                    },
                    CustomFilters = null
                };

                Subscriber.Proxy.GetRuleHistoryListResponse response = da.GetRuleHistoryList(request); 

                this.grdRulesHistory.SelectedIndex = -1;
                this.grdRulesHistory.DataSource = response.Rules;
                this.grdRulesHistory.DataBind();

                this.ClearRuleHistoryDetail();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        protected void FetchRuleHistoryItem(int id)
        {
            using (Subscriber.Proxy.SubscriberWebService da = this.CreateService())
            {
                Subscriber.Proxy.RuleHistoryInfo historyInfo = da.GetHistoryItem(id);

                if (historyInfo != null)
                {
                    this.grdPublishedObject.SelectedIndex = -1;
                    this.grdPublishedObject.DataSource = historyInfo.ObjectSnapshot.Properties.Where(e => !e.Hidden);
                    this.grdPublishedObject.DataBind();

                    if (historyInfo.MailMessageSnapshot != null)
                    {
                        this.txtAppointmentAsText.Text = historyInfo.MailMessageSnapshot.AppointmentAsText;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearRules()
        {
            this.grdRules.SelectedIndex = -1;
            this.grdRules.EditItemIndex = -1;
            this.grdRules.DataSource = null;
            this.grdRules.DataBind();
            
            this.ClearRuleOptions();

            this.ClearSubRules();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearSubRules()
        {
            this.grdSubRules.SelectedIndex = -1;
            this.grdSubRules.EditItemIndex = -1;
            this.grdSubRules.DataSource = null;
            this.grdSubRules.DataBind();

            this.ClearRuleOptions();

            this.ClearRuleHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearRuleHistory()
        {
            this.grdRulesHistory.SelectedIndex = -1;
            this.grdRulesHistory.EditItemIndex = -1;
            this.grdRulesHistory.DataSource = null;
            this.grdRulesHistory.DataBind();

            this.ClearRuleHistoryDetail();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearRuleHistoryDetail()
        {
            this.grdPublishedObject.SelectedIndex = -1;
            this.grdPublishedObject.EditItemIndex = -1;
            this.grdPublishedObject.DataSource = null;
            this.grdPublishedObject.DataBind();
            
            this.txtAppointmentAsText.Text = string.Empty;
        }
    }

}