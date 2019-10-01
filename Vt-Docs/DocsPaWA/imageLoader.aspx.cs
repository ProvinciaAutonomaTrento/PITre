using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPaIntegration;
using System.Drawing;
using DocsPAWA.DocsPaWR;
using DocsPaIntegration.Config;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA
{
    public partial class imageLoader : System.Web.UI.Page
    {
        private string ADAPTER_ICON_CONTEXT = "adapterIcon";

        protected void Page_Load(object sender, EventArgs e)
        {
            string context = Request["context"];
            if(ADAPTER_ICON_CONTEXT.Equals(context)){
                LoadAdapterIcon();
            }
        }

        private void LoadAdapterIcon()
        {
            string oggettoId = Request["oggettoId"];
            string type = Request["type"];
            string adapterId = Request["adapterId"];
            string adapterVersion = Request["adapterVersion"];
            string position = Request["position"];
            IIntegrationAdapter adapter = GetIntegrationAdapter(oggettoId, type, adapterId, adapterVersion,position);     
            Bitmap image = adapter.Icon;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
            stream.Position = 0;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            Response.ContentType = "image/gif";
            Response.BinaryWrite(data);
        }

        private IIntegrationAdapter GetIntegrationAdapter(string oggettoId, string type, string adapterId, string adapterVersion,string position)
        {
            if (!string.IsNullOrEmpty(position))
            {
                Templates template = (Templates) Session["template"];
                int pos=Int32.Parse(position);
                OggettoCustom temp=template.ELENCO_OGGETTI[pos];
                if(temp!=null && !string.IsNullOrEmpty(temp.CONFIG_OBJ_EST)){
                    ConfigurationInfo conf=new ConfigurationInfo();
                    conf.Value = temp.CONFIG_OBJ_EST;
                    return IntegrationAdapterFactory.Instance.GetAdapterConfigured(conf);
                }
            }
            if (!string.IsNullOrEmpty(oggettoId))
            {
                OggettoCustom oggCust = null;
                if ("D".Equals(type))
                {
                    oggCust = ProfilazioneDocManager.getOggettoById(oggettoId, this);
                }
                else
                {
                    oggCust = ProfilazioneFascManager.getOggettoById(oggettoId, this);
                }
                ConfigurationInfo conf = new ConfigurationInfo();
                conf.Value = oggCust.CONFIG_OBJ_EST;
                return IntegrationAdapterFactory.Instance.GetAdapterConfigured(conf);
            }
            else
            {
                if (!string.IsNullOrEmpty(adapterVersion))
                {
                    return IntegrationAdapterFactory.Instance.GetAdapter(adapterId, new Version(adapterVersion));
                }
                else
                {
                    return IntegrationAdapterFactory.Instance.GetAdapter(adapterId);
                }
            }
        }
    }
}
