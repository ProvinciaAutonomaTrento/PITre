using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.Utils;


namespace ConservazioneWA.Esibizione
{
    public partial class SalvaCertificazione : System.Web.UI.Page
    {

        protected string tipofirma;
        protected bool firmabool;
        protected string idDocumento;
        protected string idEsibizione;
        protected DocsPaWR.DocsPaWebService WS;
        protected WSConservazioneLocale.InfoUtente infoUtente;


        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Expires = -1;

            try
            {
                idEsibizione = Request.QueryString["idEsibizione"];
                idDocumento = Request.QueryString["idDocumento"];
                tipofirma = Request.QueryString["tipofirma"];
                this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

                if (string.IsNullOrEmpty(tipofirma))
                {
                    tipofirma = string.Empty;
                }
                if (tipofirma.Equals("cosign"))
                {
                    firmabool = true;
                }
                else
                {
                    firmabool = false;
                }

                byte[] ba = Request.BinaryRead(Request.ContentLength);
                DocsPaWR.DocsPaWebService WS = new ProxyManager().getProxyDocsPa();

                DocsPaWR.FileDocumento fd = new ConservazioneWA.DocsPaWR.FileDocumento();

                ASCIIEncoding ae = new ASCIIEncoding();
                string base64content = ae.GetString(ba);
                byte[] content = Convert.FromBase64String(base64content);

                //timestamp
                bool result = Utils.ConservazioneManager.MarcaCertificazioneIstanzaEsibizione(idEsibizione, this.infoUtente, content);
                if (!result)
                {
                    this.RegisterClientScript("errMarca", "alert('Apposizione marca temporale fallita.');");

                }


                DocsPaWR.InfoUtente infoutWS = WS.getInfoUtente(infoUtente.idPeople, infoUtente.idGruppo);

                if (!IsPostBack)
                {
                    DocsPaWR.FileRequest fr = WS.GetVersionsMainDocument(infoutWS, idDocumento)[0];
                    //il file è in formato pdf
                    //fr.fileName += ".pdf";

                    fr.dataInserimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    bool retValue = WS.AppendDocumentoFirmato(base64content, firmabool, ref fr, infoutWS);

                    if (!retValue)
                    {
                        Debugger.Write("Errore nel Page_Load (docs = NULL)");
                        throw new Exception();
                    }
                    else
                    {
                        //aggiornamento tabella DPA_AREA_ESIBIZIONE
                        if (!ConservazioneManager.UpdateCertificazioneIstanzaEsibizione(this.infoUtente, this.idEsibizione))
                        {
                            Debugger.Write("Errore nel Page_Load (aggiornamento DPA_AREA_ESIBIZIONE)");
                            throw new Exception("Errore nell'aggioramento dei dati");
                        }

                    }

                }

                string confirmMsg = "Firma avvenuta con successo.";
                this.RegisterClientScript("SignCertOK", "alert('" + confirmMsg + "');");

            }

            catch (Exception ex)
            {
                string errorMsg = "Errore nella firma del documento di certificazione: " + ex.Message ;
                this.RegisterClientScript("SignCertKO", "alert('" + errorMsg + "');");
            }

        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

    }
}