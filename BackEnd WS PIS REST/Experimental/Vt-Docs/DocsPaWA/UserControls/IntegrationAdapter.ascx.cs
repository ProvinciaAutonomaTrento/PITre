using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPaIntegration;
using System.Reflection;
using DocsPaIntegration.Config;
using DocsPaIntegration.ObjectTypes;
using DocsPAWA;
using DocsPaIntegration.Search;
using DocsPAWA.SiteNavigation;

namespace DocsPaWA.UserControls
{
    public partial class IntegrationAdapter : System.Web.UI.UserControl
    {
        private ViewStrategy _strategy = ViewStrategy.getInstance(IntegrationAdapterView.ANTEPRIMA);

        public string CssClass
        {
            get;
            set;
        }

        public string ManualInsertCssClass
        {
            get;
            set;
        }

        public bool IsManualInsert
        {
            get
            {
                if (string.IsNullOrEmpty((string)this.ViewState[ID + "_IsManualInsert"])) return false;
                return "1".Equals((string)this.ViewState[ID + "_IsManualInsert"]);
            }
            set
            {
                if (value)
                {
                    this.ViewState[ID + "_IsManualInsert"] = "1";
                }
                else
                {
                    this.ViewState[ID + "_IsManualInsert"] = "0";
                }
            }
        }

        public IntegrationAdapterView View
        {
            get
            {
                return _strategy.View;
            }
            set
            {
                _strategy = ViewStrategy.getInstance(value);
            }
        }

        public IntegrationAdapterValue Value
        {
            get
            {
                return _strategy.GetValue(this);
            }
            set
            {
                if (value != null) IsManualInsert = value.ManualInsert;
                _strategy.SetValue(value, this);
            }
        }

        public IIntegrationAdapter Adapter
        {
            get
            {
                return IntegrationAdapterFactory.Instance.GetAdapterConfigured(ConfigurationInfo);
            }
        }

        public bool Disservizio
        {
            get
            {
                if (string.IsNullOrEmpty((string)this.ViewState[ID + "_Disservizio"])) return false;
                return "1".Equals((string)this.ViewState[ID + "_Disservizio"]);
            }
            set
            {
                if (value)
                {
                    this.ViewState[ID + "_Disservizio"] = "1";
                }
                else
                {
                    this.ViewState[ID + "_Disservizio"] = "0";
                }
            }
        }

        public bool IsFasc
        {
            get;
            set;
        }

        public string ConfigurationValue
        {
            get
            {
                ConfigurationInfo confInfo = ConfigurationInfo;
                for (int i = 0; i < this.dg_Config.Items.Count; i++)
                {
                    string paramName = ((HiddenField)this.dg_Config.Items[i].Cells[0].Controls[0]).Value;
                    ConfigurationParam temp = confInfo[paramName];
                    string value = ObjectTypeControlStrategy.Instances[temp.Type].GetControlValue(this.dg_Config.Items[i].Cells[3].Controls);
                    temp.Value = value;
                }
                return confInfo.Value;
            }
            set
            {
                ConfigurationInfo configInfo = new ConfigurationInfo();
                configInfo.Value = value;
                ConfigurationInfo = configInfo;
                this.AdapterInfo = Adapter.AdapterInfo;
                this.lbl_codice.Text = Adapter.IdLabel;
                this.lbl_descrizione.Text = Adapter.DescriptionLabel;
                int maxLength = Math.Max(Adapter.IdLabel.Length, Adapter.DescriptionLabel.Length);
                _strategy.OnConfigValueChanged(this);
            }
        }

        protected string AdapterImageUrl
        {
            get
            {
                return _strategy.GetImageUrl(this);
            }
        }

        protected string RicercaLink
        {
            get
            {
                string pars = "oggettoId=" + ID;
                if (IsFasc)
                {
                    pars = pars + "&context=F";
                }
                else
                {
                    pars = pars + "&context=D";
                }
                return this.ResolveUrl("~/popup/RicercaOggettoEsterno.aspx?" + pars);
            }
        }

        public ValidationResult ValidateConfig()
        {
            ValidationResult res = new ValidationResult();
            try
            {
                if (string.IsNullOrEmpty(this.ddl_Adapter.SelectedValue))
                {
                    res.IsValid = false;
                    res.ErrorMessage = "Selezionare un adapter";
                }
                ConfigurationInfo configInfo = new ConfigurationInfo();
                configInfo.Value = ConfigurationValue;
                GetAdapterFromSelect().Init(configInfo);
                res.IsValid = true;
            }
            catch (ConfigurationException ce)
            {
                res.IsValid = false;
                if (ce.Errors.Count > 0)
                {
                    ConfigurationParam temp = ce.Errors[0].Param;
                    if (ce.Errors[0].Code == ValidationErrorCode.NOT_VALID_VALUE)
                    {
                        string message="Il parametro " + temp.Name + " ha valore non valido";
                        if(!string.IsNullOrEmpty(ce.Errors[0].Message)){
                            message = message + " : " + ce.Errors[0].Message;
                        }
                        res.ErrorMessage = message;
                    }
                    else
                    {
                        res.ErrorMessage = "Il parametro " + temp.Name + " è obbligatorio";
                    }
                }
                else
                {
                    res.ErrorMessage = ce.ErrorMessage;
                }
            }
            return res;
        }

        public void Clear()
        {
            this.ddl_Adapter.SelectedIndex = 0;
            this.dg_Config.Visible = false;
            this.tr_AdapterDescr.Visible = false;
            this.ConfigurationInfo = null;
            this.AdapterInfo = null;
        }


        protected ConfigurationInfo ConfigurationInfo
        {
            get
            {
                return (ConfigurationInfo)ViewState["ConfigurationInfo"];
            }
            set
            {
                ViewState["ConfigurationInfo"] = value;
            }
        }

        protected IntegrationAdapterInfo AdapterInfo
        {
            get
            {
                return (IntegrationAdapterInfo)ViewState["AdapterInfo"];
            }
            set
            {
                ViewState["AdapterInfo"] = value;
            }
        }

        public string Position
        {
            get
            {
                return (string)ViewState["Position"];
            }
            set
            {
                ViewState["Position"] = value;
            }
        }

        private void InitializeComponent()
        {
            this.ddl_Adapter.SelectedIndexChanged += new EventHandler(this.ddl_Adapter_SelectedIndexChanged);
            this.btn_cercaCodice.Click += new ImageClickEventHandler(this.btn_cercaCodice_Click);
        }

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _strategy.Page_Load(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            _strategy.OnPreRender(this);
        }

        protected void LoadAdaptersCombo()
        {
            List<IntegrationAdapterInfo> infos = IntegrationAdapterFactory.Instance.AdapterInfos;
            ddl_Adapter.Items.Clear();
            ddl_Adapter.Items.Add(new ListItem("", ""));
            foreach (IntegrationAdapterInfo info in infos)
            {
                string label = info.Name;
                if (info.Version != null)
                {
                    label = label + " (v. " + info.Version + ")";
                }
                string value = info.UniqueId + "||||" + info.Version;
                ddl_Adapter.Items.Add(new ListItem(label, value));
            }
        }

        protected void btn_cercaCodice_Click(object sender, EventArgs arg)
        {
            try
            {
                PuntualSearchInfo searchInfo = new PuntualSearchInfo(this.txt_codice.Text);
                SearchOutputRow output = Adapter.PuntualSearch(searchInfo);
                this.IsManualInsert = false;
                if (output == null)
                {
                    this.txt_codice.Text = "";
                    this.txt_descrizione.Text = "";
                    this.hf_codice.Value = "";
                    this.hf_descrizione.Value = "";
                }
                else
                {
                    this.txt_codice.Text = output.Codice;
                    this.txt_descrizione.Text = output.Descrizione;
                    this.hf_codice.Value = output.Codice;
                    this.hf_descrizione.Value = output.Descrizione;
                }
            }
            catch (SearchException e)
            {
                if (e.Code == SearchExceptionCode.SERVICE_UNAVAILABLE)
                {
                    handleDisservizio();
                }
                if (e.Code == SearchExceptionCode.SERVER_ERROR)
                {
                    this.txt_codice.Text = "";
                    this.txt_descrizione.Text = "";
                    this.hf_codice.Value = "";
                    this.hf_descrizione.Value = "";
                    Response.Write("<script language='javascript'>alert('" + e.Message + "')</script>");
                }
            }
            catch (Exception e)
            {
                handleDisservizio();
            }
        }

        protected void ddl_Adapter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ddl_Adapter.SelectedValue))
            {
                this.dg_Config.Visible = false;
                this.tr_AdapterDescr.Visible = false;
            }
            else
            {
                this.tr_AdapterDescr.Visible = true;
                IIntegrationAdapter adapter = GetAdapterFromSelect();
                this.ConfigurationInfo = adapter.ConfigurationInfo;
                this.AdapterInfo = adapter.AdapterInfo;
                BindDgConfig();
            }
        }

        private void fillDescription()
        {
            this.tr_AdapterDescr.Visible = true;
            string descr = AdapterInfo.Description;
            if (string.IsNullOrEmpty(descr)) descr = "Nessuna descrizione presente";
            this.lbl_AdapterDescr.Text = descr;

            if (AdapterInfo.HasIcon)
            {
                this.img_Adapter.Visible = true;
                this.img_Adapter.ImageUrl = this.AdapterImageUrl;
            }
            else
            {
                this.img_Adapter.Visible = false;
            }
        }

        private IIntegrationAdapter GetAdapterFromSelect()
        {
            string[] split = this.ddl_Adapter.SelectedValue.Split(new string[] { "||||" }, StringSplitOptions.RemoveEmptyEntries);
            IIntegrationAdapter adapter = null;
            if (split.Length == 1)
            {
                adapter = IntegrationAdapterFactory.Instance.GetAdapter(split[0]);
            }
            else
            {
                adapter = IntegrationAdapterFactory.Instance.GetAdapter(split[0], new Version(split[1]));
            }
            return adapter;
        }

        private void BindDgConfig()
        {
            if (this.ConfigurationInfo != null)
            {
                this.dg_Config.Visible = true;
                this.dg_Config.DataSource = this.ConfigurationInfo.ConfigurationParams;
                this.dg_Config.DataBind();
                for (int i = 0; i < this.ConfigurationInfo.ConfigurationParams.Count; i++)
                {
                    ConfigurationParam temp = this.ConfigurationInfo.ConfigurationParams[i];
                    string name = temp.Name;
                    if (temp.Mandatory)
                    {
                        name = name + " *";
                    }
                    this.dg_Config.Items[i].Cells[1].Text = name;
                    this.dg_Config.Items[i].Cells[2].Text = temp.Type.ToString();
                    HiddenField hf = new HiddenField();
                    hf.Value = temp.Name;
                    this.dg_Config.Items[i].Cells[0].Controls.Add(hf);
                    ObjectTypeControlStrategy.Instances[temp.Type].AddControl(this.Page, this.dg_Config.Items[i].Cells[3].Controls, temp, CssClass);
                }
                this.fillDescription();
            }
        }

        protected void handleDisservizio()
        {
            this.Disservizio = true;
        }

        internal abstract class ViewStrategy
        {
            private Dictionary<IntegrationAdapterView, ViewStrategy> _strategies;

            public static ViewStrategy getInstance(IntegrationAdapterView view)
            {
                if (view == IntegrationAdapterView.ADMIN) return new AdminViewStrategy();
                if (view == IntegrationAdapterView.ANTEPRIMA) return new AnteprimaViewStrategy();
                if (view == IntegrationAdapterView.INSERT_MODIFY) return new InsertModifyViewStrategy();
                if (view == IntegrationAdapterView.READ_ONLY) return new ReadOnlyViewStrategy();
                if (view == IntegrationAdapterView.RICERCA) return new RicercaViewStrategy();
                return null;
            }

            public abstract void Page_Load(IntegrationAdapter control);

            public abstract void OnPreRender(IntegrationAdapter control);

            public abstract void OnConfigValueChanged(IntegrationAdapter control);

            public abstract void SetValue(IntegrationAdapterValue value, IntegrationAdapter control);

            public abstract IntegrationAdapterValue GetValue(IntegrationAdapter control);

            public abstract string GetImageUrl(IntegrationAdapter control);

            public abstract IntegrationAdapterView View
            {
                get;
            }
        }

        internal class AdminViewStrategy : ViewStrategy
        {

            public override void Page_Load(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.AdminView);
                if (!control.IsPostBack) control.LoadAdaptersCombo();
                control.BindDgConfig();
            }

            public override void OnPreRender(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.AdminView);
                control.BindDgConfig();
            }

            public override void OnConfigValueChanged(IntegrationAdapter control)
            {
                string selected = control.ConfigurationInfo.AdapterUniqueId + "||||" + control.ConfigurationInfo.AdapterVersion;
                control.ddl_Adapter.SelectedValue = selected;
            }

            public override IntegrationAdapterView View
            {
                get
                {
                    return IntegrationAdapterView.ADMIN;
                }
            }

            public override IntegrationAdapterValue GetValue(IntegrationAdapter control)
            {
                return null;
            }

            public override void SetValue(IntegrationAdapterValue value, IntegrationAdapter control)
            {

            }

            public override string GetImageUrl(IntegrationAdapter control)
            {
                string pars = pars = "adapterId=" + control.AdapterInfo.UniqueId;
                if (control.AdapterInfo.Version != null){
                    pars = pars + "&adapterVersion=" + control.AdapterInfo.Version;
                }
                return control.ResolveUrl("~/imageLoader.aspx?context=adapterIcon&" + pars);
            }
        }

        internal class AnteprimaViewStrategy : ViewStrategy
        {
            public override void Page_Load(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.InsertModifyView);
                control.txt_codice.CssClass = control.CssClass;
                control.txt_descrizione.CssClass = control.CssClass;
                control.txt_codice.ReadOnly = true;
                control.txt_descrizione.ReadOnly = true;
                control.btn_cerca.Enabled = false;
                control.btn_reset.Enabled = false;
                control.btn_cercaCodice.Enabled = false;
                if (control.AdapterInfo != null && control.AdapterInfo.HasIcon)
                {
                    control.btn_cerca.ImageUrl = control.AdapterImageUrl;
                }
            }

            public override void OnPreRender(IntegrationAdapter control)
            {
                Page_Load(control);
            }

            public override void OnConfigValueChanged(IntegrationAdapter control)
            {

            }

            public override void SetValue(IntegrationAdapterValue value, IntegrationAdapter control)
            {

            }

            public override IntegrationAdapterValue GetValue(IntegrationAdapter control)
            {
                return null;
            }

            public override string GetImageUrl(IntegrationAdapter control)
            {
                string pars = pars = "adapterId=" + control.AdapterInfo.UniqueId;
                if (control.AdapterInfo.Version != null)
                {
                    pars = pars + "&adapterVersion=" + control.AdapterInfo.Version;
                }
                if(!string.IsNullOrEmpty(control.Position)) {
                    pars = pars + "&position=" + control.Position;
                }
                return control.ResolveUrl("~/imageLoader.aspx?context=adapterIcon&" + pars);
            }

            public override IntegrationAdapterView View
            {
                get
                {
                    return IntegrationAdapterView.ANTEPRIMA;
                }
            }
        }

        internal class InsertModifyViewStrategy : ViewStrategy
        {
            public override void Page_Load(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.InsertModifyView);
                if (!control.IsManualInsert)
                {
                    control.txt_codice.CssClass = control.CssClass;
                    control.txt_descrizione.CssClass = control.CssClass;
                }
                else
                {
                    control.txt_codice.CssClass = control.ManualInsertCssClass;
                    control.txt_descrizione.CssClass = control.ManualInsertCssClass;
                }
                control.txt_codice.ReadOnly = false;
                control.txt_descrizione.ReadOnly = true;
                control.btn_cerca.Enabled = true;
                control.btn_cercaCodice.Enabled = true;
                control.btn_cerca.OnClientClick = "_" + control.ClientID + "_apriRicerca()";
                control.btn_reset.OnClientClick = "_" + control.ClientID + "_reset()";
                if (control.AdapterInfo != null && control.AdapterInfo.HasIcon)
                {
                    control.btn_cerca.ImageUrl = control.AdapterImageUrl;
                }
                if (control.Disservizio)
                {
                    control.txt_descrizione.ReadOnly = false;
                    control.img_disservizio.Visible = true;
                    control.btn_cerca.Visible = false;
                    control.btn_cercaCodice.Visible = false;
                }
            }

            public override void OnPreRender(IntegrationAdapter control)
            {
                Page_Load(control);
                if (control.IsPostBack)
                {
                    if ("1".Equals(control.hf_reset.Value))
                    {
                        control.hf_codice.Value = "";
                        control.hf_descrizione.Value = "";
                        control.txt_codice.Text = "";
                        control.txt_descrizione.Text = "";
                        control.hf_reset.Value = "0";
                    }
                    if ("1".Equals(control.hf_selectedObject.Value))
                    {
                        SearchOutputRow row = ProfilazioneDocManager.getSearchOutputRowSelected();
                        control.hf_codice.Value = row.Codice;
                        control.hf_descrizione.Value = row.Descrizione;
                        control.txt_codice.Text = row.Codice;
                        control.txt_descrizione.Text = row.Descrizione;
                        control.IsManualInsert = false;
                        control.hf_selectedObject.Value = "0";
                    }
                    if ("1".Equals(control.hf_disservizio.Value))
                    {
                        control.handleDisservizio();
                    }
                    if (control.Disservizio)
                    {
                        control.txt_descrizione.ReadOnly = false;
                        control.img_disservizio.Visible = true;
                        control.btn_cerca.Visible = false;
                        control.btn_cercaCodice.Visible = false;
                    }
                }
            }

            public override void OnConfigValueChanged(IntegrationAdapter control)
            {

            }

            public override void SetValue(IntegrationAdapterValue value, IntegrationAdapter control)
            {
                if (value != null)
                {
                    control.hf_codice.Value = value.Codice;
                    control.hf_descrizione.Value = value.Descrizione;
                    control.txt_codice.Text = value.Codice;
                    control.txt_descrizione.Text = value.Descrizione;
                }
            }

            public override IntegrationAdapterValue GetValue(IntegrationAdapter control)
            {
                if (!control.Disservizio)
                {
                    if (string.IsNullOrEmpty(control.hf_codice.Value)) return null;
                    return new IntegrationAdapterValue(control.hf_codice.Value, control.hf_descrizione.Value, control.IsManualInsert);
                }
                else
                {
                    if (string.IsNullOrEmpty(control.txt_codice.Text) || string.IsNullOrEmpty(control.txt_descrizione.Text)) return null;
                    return new IntegrationAdapterValue(control.txt_codice.Text, control.txt_descrizione.Text, true);
                }
            }

            public override string GetImageUrl(IntegrationAdapter control)
            {
                string pars = "oggettoId=" + control.ID;
                if (control.IsFasc)
                {
                    pars += "&type=F";
                }
                else
                {
                    pars += "&type=D";
                }
                return control.ResolveUrl("~/imageLoader.aspx?context=adapterIcon&" + pars);
            }

            public override IntegrationAdapterView View
            {
                get
                {
                    return IntegrationAdapterView.INSERT_MODIFY;
                }
            }
        }

        internal class ReadOnlyViewStrategy : ViewStrategy
        {
            public override void Page_Load(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.ReadOnlyView);
                if (!control.IsManualInsert)
                {
                    control.lbl_ro_codice_value.CssClass = control.CssClass;
                    control.lbl_ro_descrizione_value.CssClass = control.CssClass;
                }
                else
                {
                    control.lbl_ro_codice_value.CssClass = control.ManualInsertCssClass;
                    control.lbl_ro_descrizione_value.CssClass = control.ManualInsertCssClass;
                }
            }

            public override void OnPreRender(IntegrationAdapter control)
            {
                Page_Load(control);
            }

            public override void OnConfigValueChanged(IntegrationAdapter control)
            {
                control.lbl_ro_codice.Text = control.Adapter.IdLabel;
                control.lbl_ro_descrizione.Text = control.Adapter.DescriptionLabel;
            }

            public override IntegrationAdapterView View
            {
                get
                {
                    return IntegrationAdapterView.READ_ONLY;
                }
            }

            public override void SetValue(IntegrationAdapterValue value, IntegrationAdapter control)
            {
                if (value != null)
                {
                    control.lbl_ro_codice_value.Text = value.Codice;
                    control.lbl_ro_descrizione_value.Text = value.Descrizione;
                }
            }

            public override IntegrationAdapterValue GetValue(IntegrationAdapter control)
            {
                return new IntegrationAdapterValue(control.lbl_ro_codice_value.Text, control.lbl_ro_descrizione_value.Text, control.IsManualInsert);
            }

            public override string GetImageUrl(IntegrationAdapter control)
            {
                string pars = "oggettoId=" + control.ID;
                return control.ResolveUrl("~/imageLoader.aspx?context=adapterIcon&" + pars);
            }
        }

        internal class RicercaViewStrategy : ViewStrategy
        {
            public override void Page_Load(IntegrationAdapter control)
            {
                control.Views.SetActiveView(control.RicercaView);
                control.txt_ric_codice.CssClass = control.CssClass;
                control.txt_ric_descrizione.CssClass = control.CssClass;
            }

            public override void OnPreRender(IntegrationAdapter control)
            {
                Page_Load(control);
            }

            public override void OnConfigValueChanged(IntegrationAdapter control)
            {
                control.lbl_ric_codice.Text = control.Adapter.IdLabel;
                control.lbl_ric_descrizione.Text = control.Adapter.DescriptionLabel;
            }

            public override IntegrationAdapterView View
            {
                get
                {
                    return IntegrationAdapterView.RICERCA;
                }
            }

            public override void SetValue(IntegrationAdapterValue value, IntegrationAdapter control)
            {
                if (value != null)
                {
                    control.txt_ric_codice.Text = value.Codice;
                    control.txt_ric_descrizione.Text = value.Descrizione;
                }
            }

            public override IntegrationAdapterValue GetValue(IntegrationAdapter control)
            {
                return new IntegrationAdapterValue(control.txt_ric_codice.Text, control.txt_ric_descrizione.Text, false);
            }

            public override string GetImageUrl(IntegrationAdapter control)
            {
                string pars = "oggettoId=" + control.ID;
                return control.ResolveUrl("~/imageLoader.aspx?context=adapterIcon&" + pars);
            }
        }
    }

    public enum IntegrationAdapterView
    {
        ADMIN, ANTEPRIMA, READ_ONLY, INSERT_MODIFY, RICERCA
    }

    public class IntegrationAdapterValue
    {
        private string _codice;
        private string _descrizione;
        private bool _manualInsert;

        public IntegrationAdapterValue(string codice, string descrizione, bool manualInsert)
        {
            this._codice = codice;
            this._descrizione = descrizione;
            this._manualInsert = manualInsert;
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
        }

        public string Descrizione
        {
            get
            {
                if (!string.IsNullOrEmpty(_descrizione) && _descrizione.Length > 256) return _descrizione.Substring(0, 256);
                return _descrizione;
            }
        }

        public bool ManualInsert
        {
            get
            {
                return _manualInsert;
            }
        }
    }



    #region ObjectTypeControlStrategy
    internal abstract class ObjectTypeControlStrategy
    {
        private static Dictionary<ObjectType, ObjectTypeControlStrategy> _strategies;
        private static StrategyInstances _instances;

        static ObjectTypeControlStrategy()
        {
            _strategies = new Dictionary<ObjectType, ObjectTypeControlStrategy>();
            _strategies.Add(ObjectType.URL, new StringControlStrategy());
            _strategies.Add(ObjectType.STRING, new StringControlStrategy());
            _strategies.Add(ObjectType.NUMBER, new StringControlStrategy());
            _strategies.Add(ObjectType.BOOLEAN, new BooleanControlStrategy());
            _strategies.Add(ObjectType.IMAGE, new ImageControlStrategy());
            _instances = new StrategyInstances(_strategies);
        }

        public static StrategyInstances Instances
        {
            get
            {
                return _instances;
            }
        }

        public class StrategyInstances
        {
            private Dictionary<ObjectType, ObjectTypeControlStrategy> _strategies;

            public StrategyInstances(Dictionary<ObjectType, ObjectTypeControlStrategy> strategies)
            {
                this._strategies = strategies;
            }

            public ObjectTypeControlStrategy this[ObjectType type]
            {
                get
                {
                    return this._strategies[type];
                }
            }
        }

        public abstract void AddControl(Page page, ControlCollection controls, ConfigurationParam param, string cssClass);

        public abstract string GetControlValue(ControlCollection control);

    }

    internal class StringControlStrategy : ObjectTypeControlStrategy
    {
        public override void AddControl(Page page, ControlCollection controls, ConfigurationParam param, string cssClass)
        {
            TextBox res = new TextBox();
            res.CssClass = cssClass;
            res.Text = param.Value;
            controls.Add(res);
        }

        public override string GetControlValue(ControlCollection control)
        {
            return ((TextBox)control[0]).Text;
        }
    }

    internal class BooleanControlStrategy : ObjectTypeControlStrategy
    {
        public override void AddControl(Page page, ControlCollection controls, ConfigurationParam param, string cssClass)
        {
            CheckBox res = new CheckBox();
            if ("true".Equals(param.Value))
            {
                res.Checked = true;
            }
            controls.Add(res);
        }

        public override string GetControlValue(ControlCollection control)
        {
            if (((CheckBox)control[0]).Checked) return "true";
            return "false";
        }
    }

    internal class ImageControlStrategy : ObjectTypeControlStrategy
    {
        public override void AddControl(Page page, ControlCollection controls, ConfigurationParam param, string cssClass)
        {
            Label lbl = new Label();
            FileUpload res= new FileUpload();
            res.CssClass = cssClass;

            HiddenField hf=new HiddenField();
            if (!string.IsNullOrEmpty(param.Value))
            {
                hf.Value = param.Value;
                FileType temp=FileType.Decode(param.Value);
                lbl.Text = temp.Filename;
            }
            controls.Add(hf);
            controls.Add(lbl);
            controls.Add(res);
        }

        public override string GetControlValue(ControlCollection control)
        {
            FileUpload fu = (FileUpload)control[2];
            HiddenField hf = (HiddenField)control[0];
            if (fu.HasFile)
            {
                FileType temp = new FileType();
                temp.Filename = fu.FileName;
                temp.Content = fu.FileBytes;
                return temp.Encode();
            }
            else
            {
                return hf.Value;
            }
        }
    }
    #endregion ObjectTypeControlStrategy

    public class ValidationResult
    {
        public string ErrorMessage
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }
    }
}
