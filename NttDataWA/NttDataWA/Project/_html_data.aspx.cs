using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project
{
    public partial class _html_data : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Buffer = true;
            Response.ExpiresAbsolute = new DateTime();
            Response.Expires = 0;
            Response.CacheControl = "no-cache";

            //System.Threading.Thread.Sleep(500);

            switch (Request.QueryString["id"])
            {
                case "0":
                    step1.Visible = true;
                    break;
                case "rootnode_1":
                    step2.Visible = true;
                    break;
                case "rootnode_2":
                    step3.Visible = true;
                    break;
                case "childnode_2_1":
                    step4.Visible = true;
                    break;
                default:
                    if (Request.QueryString["q"] != null && Request.QueryString["q"].Length > 0)
                    {
                        switch (Request.QueryString["q"])
                        {
                            case "2012":
                                Response.Write(
                                      "[\n"
                                    + "	{ \"id\": \"rootnode_1\" , \"isResult\": \"true\" }\n"
                                    + "]\n"
                                );
                                break;

                            case "atti":
                                Response.Write(
                                      "[\n"
                                    + "	{ \"id\": \"rootnode_1\" , \"isResult\": \"false\" },\n"
                                    + "	{ \"id\": \"childnode_1_2\" , \"isResult\": \"true\" }\n"
                                    + "]\n"
                                );
                                break;
                        }
                    }
                    else
                    {
                        Response.Write("querystring: " + Request.QueryString.ToString() + "<br />form: " + Request.Form.ToString());
                    }
                    break;
            }
        }
    }
}