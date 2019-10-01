using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.ExportDati
{
    public partial class DownloadCorrispondenti : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string file = "exportCorrispondenti.xls";
            string fileName = "..\\ExportDati\\" + file;

            FileStream fs = new FileStream(MapPath(fileName), FileMode.Open);
            long cntBytes = new FileInfo(MapPath(fileName)).Length;
            byte[] byteArray = new byte[Convert.ToInt32(cntBytes)];
            fs.Read(byteArray, 0, Convert.ToInt32(cntBytes));
            fs.Close();

            if (byteArray != null)
            {
                this.Response.Clear();
                this.Response.ContentType = "application/octet-stream";
                this.Response.AddHeader("content-disposition", "attachment;filename=" + file);
                this.Response.BinaryWrite(byteArray);
                this.Response.End();
                this.Response.Flush();
                this.Response.Close();
            }
        }
    }
}