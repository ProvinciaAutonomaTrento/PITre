using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public abstract class MassivePage : CssPage
    {

        private void InitializeComponent()
        {
            this.MassiveMasterPage.confermaDelegate = btnConferma_Click;
            this.MassiveMasterPage.pageName = PageName;
            this.MassiveMasterPage.idLabel = (IsFasc) ? "Codice" : "Doc.";
        }

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private MassiveMasterPage MassiveMasterPage
        {
            get
            {
                return (MassiveMasterPage)this.Master;
            }
        }

        protected void showMessage(string message)
        {
            MassiveMasterPage.showMessage(message);
        }

        protected void generateReport(MassiveOperationReport report,string titolo)
        {
            MassiveMasterPage.generateReport(report,titolo,IsFasc);
        }

        protected void addJSOnConfermaButton(string js)
        {
            MassiveMasterPage.addJSOnConfermaButton(js);
        }

        protected void addJSOnChiudiButton(string js)
        {
            MassiveMasterPage.addJSOnChiudiButton(js);
        }

        protected abstract bool btnConferma_Click(object sender, EventArgs e);

        protected abstract string PageName
        {
            get;
        }

        protected abstract bool IsFasc
        {
            get;
        }
    }
}