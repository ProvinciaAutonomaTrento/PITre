using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class AttachmentTimestamp : System.Web.UI.Page
    {

        #region Properties

        private TimestampDoc TimestampDoc
        {
            get
            {
                if (HttpContext.Current.Session["TimestampDoc"] != null)
                    return (TimestampDoc)HttpContext.Current.Session["TimestampDoc"];
                else return null;
            }
        }

        private void RemoveTimestampDoc()
        {
            HttpContext.Current.Session.Remove("TimestampDoc");
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (this.TimestampDoc != null)
            {
                TimestampDoc t = this.TimestampDoc;
                RemoveTimestampDoc();
                string fileName = t.DOC_NUMBER + "-" + t.NUM_SERIE + ".tsr";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(Convert.FromBase64String(t.TSR_FILE));
                Response.End();
            }

        }
    }
}