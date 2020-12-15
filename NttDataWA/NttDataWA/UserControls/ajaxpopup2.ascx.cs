using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AjaxPopupControl.UserControls
{
    public partial class ajaxpopup21 : System.Web.UI.UserControl
    {

        // fields

        private string id = "ctrl_"+Guid.NewGuid().ToString("N").Substring(0, 6);
        private string text = "Submit";
        private string title = "";
        private string url = "about:blank";
        private string posH = "center";
        private string posV = "center";
        private int width = 400;
        private int height = 300;
        private bool isFullScreen = false;
        private bool permitClose = false;
        private bool permitScroll = true;
        private string closeFunction = "";
        private bool showLoading = true;
        private bool forceDontClose = false;


        // properties

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                //id = value + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
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
                if (url.Contains("~"))
                    url = Page.ResolveClientUrl(url);
                return url;
            }
            set
            {
                url = value;
                if (url.IndexOf("?") > 0)
                {
                    url += "&popupid=" + Id;
                }
                else
                {
                    url += "?popupid=" + Id;
                }
            }
        }
        
        public string PosH
        {
            get
            {
                return posH;
            }
            set
            {
                posH = value;
            }
        }

        public string PosV
        {
            get
            {
                return posV;
            }
            set
            {
                posV = value;
            }
        }

        public string ReturnValue
        {
            get
            {
                return retval.Value;
            }
            set
            {
                retval.Value = value;
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

        public bool IsFullScreen
        {
            get
            {
                return isFullScreen;
            }
            set
            {
                isFullScreen = value;
            }
        }

        public bool PermitClose
        {
            get
            {
                return permitClose;
            }
            set
            {
                permitClose = value;
            }
        }

        public bool PermitScroll
        {
            get
            {
                return permitScroll;
            }
            set
            {
                permitScroll = value;
            }
        }

        public string CloseFunction
        {
            get
            {
                //return "close: {$('#frame').show(); " + closeFunction+"}";
                return closeFunction.Length > 0 ? "close: " + closeFunction.Replace("}", "$('#frame').css('visibility', 'visible');}") + "," : closeFunction;
            }
            set
            {
                closeFunction = value;
            }
        }

        public bool ShowLoading
        {
            get
            {
                return showLoading;
            }
            set
            {
                showLoading = value;
            }
        }

        public bool ForceDontClose
        {
            get
            {
                return forceDontClose;
            }
            set
            {
                forceDontClose = value;
            }
        }


        // events
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (!PermitClose)
                if (this.ForceDontClose)
                dontShowClose.Visible = true;

                //Button button = new Button();
                //button.ID = Id + "_button";
                //button.Text = text;
                //button.OnClientClick = "return ajaxModalPopup" + Id + "()";
                //plc.Controls.Add(button);

                pnl.ID = Id + "_panel";
                pnl.ClientIDMode = ClientIDMode.Static;
            }
        }

    }
}