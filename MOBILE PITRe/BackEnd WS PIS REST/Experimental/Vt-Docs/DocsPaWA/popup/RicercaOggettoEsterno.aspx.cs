using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPaIntegration;
using DocsPAWA.DocsPaWR;
using DocsPaIntegration.Config;
using DocsPaIntegration.Search;

namespace DocsPAWA.popup
{
    public partial class RicercaOggettoEsterno : System.Web.UI.Page
    {
        protected ConfigurationInfo ConfInfo
        {
            get
            {
                return (ConfigurationInfo)this.ViewState["confInfo"];
            }
            set
            {
                this.ViewState["confInfo"] = value;
            }
        }

        protected SearchInfo SearchInfo
        {
            get
            {
                return (SearchInfo)this.ViewState["searchInfo"];
            }
            set
            {
                this.ViewState["searchInfo"] = value;
            }
        }

        protected SearchOutput SearchOutput
        {
            get
            {
                return (SearchOutput)this.ViewState["searchOutput"];
            }
            set
            {
                this.ViewState["searchOutput"] = value;
            }
        }

        protected IIntegrationAdapter IntegrationAdapter
        {
            get
            {
                return IntegrationAdapterFactory.Instance.GetAdapterConfigured(ConfInfo);
            }
        }

        override protected void OnInit(EventArgs e)
        {

            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.btn_ric.Click += new EventHandler(this.btn_ric_Click);
            this.btn_ok.Click+=new EventHandler(this.btn_ok_Click);
            this.dg_OggEst.PageIndexChanged += new DataGridPageChangedEventHandler(this.dg_OggEst_PageIndexChanged);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string oggettoId = Request["oggettoId"];
                string context = Request["context"];
                OggettoCustom oggCust = null;
                if ("D".Equals(context))
                {
                    oggCust = ProfilazioneDocManager.getOggettoById(oggettoId, this);
                }
                else
                {
                    oggCust = ProfilazioneFascManager.getOggettoById(oggettoId, this);
                }
                ConfigurationInfo conf = new ConfigurationInfo();
                conf.Value = oggCust.CONFIG_OBJ_EST;
                ConfInfo = conf;
            }
            
            this.lbl_descr.Text = IntegrationAdapter.DescriptionLabel;
            this.lbl_codice.Text = IntegrationAdapter.IdLabel;
            this.dg_OggEst.Columns[1].HeaderText = IntegrationAdapter.IdLabel;
            this.dg_OggEst.Columns[2].HeaderText = IntegrationAdapter.DescriptionLabel;
        }

        private void btn_ric_Click(Object sender, EventArgs arg)
        {
            SearchInfo searchInfo = new SearchInfo();
            searchInfo.RequestedPage = 1;
            searchInfo.PageSize = 10;
            searchInfo.Descrizione = this.txt_descr.Text;
            searchInfo.Codice = this.txt_codice.Text;
            SearchInfo = searchInfo;
            SearchOutput res = null;
            try
            {
                res = IntegrationAdapter.Search(searchInfo);
                bindDataGrid(res, 0);
            }
            catch (SearchException e)
            {
                if (e.Code == SearchExceptionCode.SERVICE_UNAVAILABLE)
                {
                    handleDisservizio();
                }
                if (e.Code == SearchExceptionCode.SERVER_ERROR)
                {
                    Response.Write("<script language='javascript'>alert('"+e.Message+"')</script>");
                }
            }
            catch (Exception e)
            {
                handleDisservizio();
            }
        }

        private void dg_OggEst_PageIndexChanged(Object sender, EventArgs arg)
        {
            DataGridPageChangedEventArgs ev=(DataGridPageChangedEventArgs) arg;
            SearchInfo searchInfo = SearchInfo;
            searchInfo.RequestedPage = ev.NewPageIndex + 1;
            SearchInfo = searchInfo;
            SearchOutput res = null;
            try
            {
                res = IntegrationAdapter.Search(searchInfo);
                bindDataGrid(res,ev.NewPageIndex);
            }
            catch (Exception e)
            {
                handleDisservizio();
            }
        }

        private void btn_ok_Click(Object sender, EventArgs arg)
        {
            int selectedIndex = Int32.Parse(this.hf_checkedIndex.Value);
            SearchOutputRow selectedRow = SearchOutput.Rows[selectedIndex];
            ProfilazioneDocManager.setSearchOutputRowSelected(selectedRow);
            Response.Write("<script>window.returnValue = 'Y'; window.close();</script>");
        }

        private void handleDisservizio()
        {
            Response.Write("<script>window.returnValue = 'D'; window.close();</script>");
        }

        private void bindDataGrid(SearchOutput res,int numPage){
            dg_OggEst.VirtualItemCount = res.NumTotResults;
            dg_OggEst.CurrentPageIndex = numPage;
            SearchOutput = res;
            if (res.NumTotResults == 0)
            {
                this.lbl_noResult.Visible = true;
                this.dg_OggEst.Visible = false;
            }
            else
            {
                this.lbl_noResult.Visible = false;
                this.dg_OggEst.Visible = true;
                this.dg_OggEst.DataSource = res.Rows;
                this.dg_OggEst.DataBind();
            }
        }
    }
}
