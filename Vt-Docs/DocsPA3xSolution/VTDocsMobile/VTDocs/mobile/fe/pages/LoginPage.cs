using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace VTDocs.mobile.fe.pages
{
    public class LoginPage : GeneralPage<VTDocs.mobile.fe.model.LoginModel>
    {
        protected MultiView MultiView;
        protected View View1;
        protected View View2;

        protected void Page_Load(object sender, EventArgs e)
        {
            MultiView.SetActiveView(View2);
        }

    }
}