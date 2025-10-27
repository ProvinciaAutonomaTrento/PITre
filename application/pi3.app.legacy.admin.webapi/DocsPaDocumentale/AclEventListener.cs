// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDocumentale.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale.Documentale
{
    public class AclEventListener : IAclEventListener
    {
        private static Type _type = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static AclEventListener()
        {
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            //{
            //    string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

            //    if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
            _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.AclEventListener);

            //    else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.AclEventListener);

            //    else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_FILENET.Documentale.AclEventListener);

            //    else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_PITRE.Documentale.AclEventListener);

            //    else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC.Documentale.AclEventListener);

            //    else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_GFD.Documentale.AclEventListener);



            //    //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
            //    else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.AclEventListener);
            //    //Fine

            //}
        }

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IAclEventListener _instance = null;

        protected IAclEventListener Instance
        {
            get
            {
                return this._instance;
            }
        }

        public AclEventListener(DocsPaVO.utente.InfoUtente infoUtente)
        {
            _instance = (IAclEventListener)Activator.CreateInstance(_type, infoUtente);
        }

        public void DocumentoCreatoEventHandler(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            this.Instance.DocumentoCreatoEventHandler(schedaDocumento, ruolo, ruoliSuperiori);
        }

        public void FascicoloCreatoEventHandler(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            this.Instance.FascicoloCreatoEventHandler(classificazione, fascicolo, ruolo, ruoliSuperiori);
        }

        public void SottofascicoloCreatoEventHandler(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            this.Instance.SottofascicoloCreatoEventHandler(folder, ruolo, ruoliSuperiori);
        }

        public void TrasmissioneCompletataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.infoSecurity[] infoSecurityList)
        {
            this.Instance.TrasmissioneCompletataEventHandler(trasmissione, infoSecurityList);
        }

        public void TrasmissioneAccettataRifiutataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.trasmissione.TipoRisposta tipoRisposta)
        {
            this.Instance.TrasmissioneAccettataRifiutataEventHandler(trasmissione, ruolo, tipoRisposta);
        }

        public void SmistamentoDocumentoCompletatoEventHandler(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.Smistamento.DocumentoSmistamento documento, DocsPaVO.Smistamento.RuoloSmistamento ruolo, string accessRights)
        {
            this.Instance.SmistamentoDocumentoCompletatoEventHandler(mittente, documento, ruolo, accessRights);
        }
    }
}
