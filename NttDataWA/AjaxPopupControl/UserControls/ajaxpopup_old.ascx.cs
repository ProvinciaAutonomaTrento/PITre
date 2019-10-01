using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AjaxPopupControl
{
    public partial class WebUserControl1 : System.Web.UI.UserControl
    {
        // fields

        private string popupId = Guid.NewGuid().ToString();
        private string popupButtonId = "";
        private string popupLayerId = "";
        private string title = "";
        private string url = "about:blank";
        private int width = 400;
        private int height = 300;


        // properties

        public string PopupId
        {
            get
            {
                return popupId;
            }
            set
            {
                popupId = value;
            }
        }

        public string PopupButtonId
        {
            get
            {
                return popupButtonId;
            }
        }

        public string PopupLayerId
        {
            get
            {
                return popupLayerId;
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
                if (url.IndexOf("?") > 0)
                {
                    url += "&amp;popupid=" + popupId;
                }
                else
                {
                    url += "?popupid=" + popupId;
                }
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
            HtmlInputButton button = new HtmlInputButton();
            button.Value = Title;
            plc.Controls.Add(button);
            popupButtonId = button.ClientID;

            HtmlGenericControl layer = new HtmlGenericControl("div");
            layer.ID = "popup_"+popupId;
            layer.Attributes["class"] = "popup_" + popupId;
            plc.Controls.Add(layer);
            popupLayerId = layer.ClientID;

            HtmlInputHidden hidden = new HtmlInputHidden();
            //HtmlInputText hidden = new HtmlInputText();
            hidden.ID = "popup_" + popupId + "_value";
            hidden.Attributes["class"] = "popup_" + popupId + "_value";
            hidden.Value = popupId + "_value";
            plc.Controls.Add(hidden);
        }


    }
}