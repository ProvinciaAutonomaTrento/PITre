using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO;
using System.Xml.Serialization;
using System.Xml;


namespace DocsPaWS.Pubblicazione
{
    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    [XmlInclude(typeof(DocsPaVO.documento.Protocollo))]
    [XmlInclude(typeof(DocsPaVO.documento.ProtocolloUscita))]
    [XmlInclude(typeof(DocsPaVO.documento.ProtocolloEntrata))]
    [XmlInclude(typeof(DocsPaVO.documento.ProtocolloInterno))]
    [XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]
    [XmlInclude(typeof(DocsPaVO.utente.UnitaOrganizzativa))]
    [XmlInclude(typeof(DocsPaVO.utente.RaggruppamentoFunzionale))]
    [XmlInclude(typeof(DocsPaVO.utente.Ruolo))]
    [XmlInclude(typeof(DocsPaVO.utente.Utente))]
    [WebService(Namespace = "http://valueteam.com/vtdocs/pubblicazionedocumenti")]
    public class PubblicazioneDocumentiServices : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.Pubblicazione.DocumentoDaPubblicare[] RicercaDocumentiDaPubblicare(DocsPaVO.Pubblicazione.FiltroDocumentiDaPubblicare filtro)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.RicercaDocumentiDaPubblicare(filtro);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumenti(DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtro)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.RicercaPubblicazioneDocumenti(filtro);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumentiBySystemId(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pub)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.RicercaPubblicazioneDocumentiBySystemId(pub);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public bool InserimentoPubblicazioneDocumento(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.InserimentoPubblicazioneDocumento(pubblicazione);
        }



        //[WebMethod()]
        //public DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(DocsPaVO.Pubblicazione.LoginInfo loginInfo,string tipoAtto)
        //{
        //    return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.getTemplatePerRicerca(tipoAtto );
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        [WebMethod()]                
        public bool UpdatePubblicazioneDocumenti(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.UpdatePubblicazioneDocumento(pubblicazione);
        }


        [WebMethod()]
        public bool UpdatePubblicazioneDocumentiGenerale(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.UpdatePubblicazioneDocumentoGenerale(pubblicazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaVO.documento.SchedaDocumento GetDocumento(DocsPaVO.Pubblicazione.DocumentoDaPubblicare documento)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.GetDocumento(documento);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual DocsPaVO.documento.FileDocumento GetFileDocumento(DocsPaVO.Pubblicazione.DocumentoDaPubblicare documento, DocsPaVO.documento.FileRequest fileRequest)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.GetFileDocumento(documento, fileRequest);
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtroPubblicazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public string MaxDataPubblicazioneDocumento(DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtroPubblicazione)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.MaxDataPubblicazioneDocumento(filtroPubblicazione);
        }

        [WebMethod()]
        public string ricercaCodice(string codice)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.ricercaCodice(codice);
        }

        [WebMethod()]
        public bool codicePerlaPubblicazione(string codice)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.codicePerlaPubblicazione(codice);
        }

        [WebMethod(Description = "Servizio per verificare se un documento è una ricevuta IS")]
        public string IsAllegatoIS(string versionId)
        {
            string retValue = string.Empty;

            try
            {
                retValue = BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(versionId);
            }
            catch (Exception e)
            {
                retValue = null;
            }
            return retValue;
        }

    }
}
