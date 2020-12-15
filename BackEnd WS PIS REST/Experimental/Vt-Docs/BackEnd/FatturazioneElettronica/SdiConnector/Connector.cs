
using System;
namespace SdiConnector
{
    public class Connector
    {

        public static string SendFileToSdi(DocsPaVO.documento.FileDocumento fileDoc)
        {
            string idSdi = string.Empty;

            RicezioneFattureServices.DPAConnectorClient sdi = new RicezioneFattureServices.DPAConnectorClient();
            RicezioneFattureServices.InviaFatturaToSdiRequest sdiReq = new RicezioneFattureServices.InviaFatturaToSdiRequest();

            sdiReq.File = fileDoc.content;
            sdiReq.FileName = fileDoc.name;

            RicezioneFattureServices.InviaFatturaToSdiResponse sdiRes = sdi.InviaFatturaToSdi(sdiReq);

            if (sdiRes.Success)
                idSdi = sdiRes.SdI_Identifier;

            return idSdi;
        }

        public static string SendFileToSdiViaPec(DocsPaVO.documento.FileDocumento fileDoc, string docnumber)
        {
            try
            {
                RicezioneFattureServices.DPAConnectorClient sdi = new RicezioneFattureServices.DPAConnectorClient();
                RicezioneFattureServices.InviaFatturaToSdiViaPecRequest sdiReq = new RicezioneFattureServices.InviaFatturaToSdiViaPecRequest();

                sdiReq.File = fileDoc.content;
                sdiReq.FileName = fileDoc.name;
                sdiReq.EmailSubject = string.Format("Invio Fattura Elettronica a SDI via PEC #{0}#", docnumber);
                sdiReq.EmailBody = "Si invia via PEC la fattura elettronica";
                RicezioneFattureServices.InviaFatturaToSdiViaPecResponse sdiRes = sdi.InviaFatturaToSdiViaPec(sdiReq);

                return string.Format("{0}:{1}", sdiRes.Success, sdiRes.ErrorMessage);
            }
            catch (Exception  ex)
            {
                return string.Format("{0}:{1}", "False", ex.Message);
            }
        }

    }
}
