using System;
using System.Collections.Generic;
using System.Web;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Spedizione
{
    /// <summary>
    /// Classe per la gestione della spedizione del documento
    /// </summary>
    public sealed class SpedizioneManager
    {
        /// <summary>
        /// Reperimento delle configurazioni impostate per la spedizione del protocollo in uscita ai destinatari
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.ConfigSpedizioneDocumento GetConfigSpedizioneDocumento()
        {
            return WsInstance.GetConfigSpedizioneDocumento(UserManager.getInfoUtente());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento GetSpedizioneDocumento(SchedaDocumento schedaDocumento)
        {
            return WsInstance.GetSpedizioneDocumento(UserManager.getInfoUtente(), schedaDocumento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento SpedisciDocumento(SchedaDocumento schedaDocumento)
        {
            return WsInstance.SpedisciDocumentoInAutomatico(UserManager.getInfoUtente(), schedaDocumento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoSpedizione"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento SpedisciDocumento(SchedaDocumento schedaDocumento, DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            return WsInstance.SpedisciDocumento(UserManager.getInfoUtente(), schedaDocumento, infoSpedizione);
        }

        /// <summary>
        /// Logica di business per determinare se è necessario avvisare
        /// l'utente prima della spedizione che non è stato acquisito alcun documento
        /// </summary>
        /// <param name="documento"></param>
        public static bool AvvisaSuSpedizioneDocumento(SchedaDocumento documento)
        {
            DocsPaWR.ConfigSpedizioneDocumento config = GetConfigSpedizioneDocumento();

            return (config.AvvisaSuSpedizioneDocumento && 
                        documento.documenti != null && documento.documenti[0].fileSize != "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool SpedizioneAutomaticaDocumentoAttiva()
        {
            DocsPaWR.ConfigSpedizioneDocumento config = GetConfigSpedizioneDocumento();

            return config.SpedizioneAutomaticaDocumento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getSetRicevutaPec(string idRegistro, string ricevutaPecDefault, string ricevutaPecOneTime, bool getData, string mailAddress)
        {
            return WsInstance.getSetRicevutaPec(idRegistro, ricevutaPecDefault, ricevutaPecOneTime, getData, mailAddress);
        }


        /// <summary>
        /// 
        /// </summary>
        private static DocsPaWR.DocsPaWebService _wsInstance = null;

        /// <summary>
        /// Istanza webservice
        /// </summary>
        private static DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (_wsInstance == null)
                {
                    _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    _wsInstance.Timeout = System.Threading.Timeout.Infinite;
                }
                return _wsInstance;
            }
        }
    }
}
