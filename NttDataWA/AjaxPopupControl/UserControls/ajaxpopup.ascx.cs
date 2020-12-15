using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxPopupControl.UserControls
{
    public partial class ajaxpopup2 : System.Web.UI.UserControl
    {

        // fields

        private string id = Guid.NewGuid().ToString("N");
        private string text = "Submit";
        private string title = "";
        private string url = "about:blank";
        private int width = 400;
        private int height = 300;


        // properties

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                //if (url.IndexOf("?") > 0)
                //{
                //    url += "&amp;popupid=" + popupId;
                //}
                //else
                //{
                //    url += "?popupid=" + popupId;
                //}
            }
        }

        public string ReturnValue
        {
            get
            {
                return hidden.Value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }


        // events
        protected void Page_Load(object sender, EventArgs e)
        {
            button.Text = text;
            button.OnClientClick = "ajaxModalPopup"+Id+"()";
        }
    }
}