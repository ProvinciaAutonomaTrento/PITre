using System;
using System.Collections;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti
{
    /// <summary>
    /// Summary description for DettagliVisibilitaDocumentoHandler.
    /// </summary>
    public class DettagliVisibilitaDocumentoHandler
    {
        public DettagliVisibilitaDocumentoHandler()
        {
        }

        /// <summary>
        /// Reperimento dettagli di visibilità di un documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public DocumentoDiritto[] GetDettagliVisibilita(string idProfile)
        {
            if (idProfile == null || idProfile == string.Empty)
                throw new ApplicationException("Parametro idProfile mancante, impossibile determinare la visibilità del documento");

            DocsPaWebService ws = new DocsPaWebService();
            return ws.DocumentoGetVisibilita(UserManager.getInfoUtente(), idProfile, false);
        }

        /// <summary>
        /// Reperimento degli utenti
        /// </summary>
        /// <param name="codiceRubrica"></param>
        /// <returns></returns>
        public ArrayList GetUserList(string codiceRubrica)
        {
            ArrayList retValue = new ArrayList();

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
            qco.codiceRubrica = codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            DocsPaWebService ws = new DocsPaWebService();

            Corrispondente[] listaCorr = ws.AddressbookGetListaCorrispondenti(qco);

            foreach (Corrispondente corrispondente in listaCorr)
                retValue.Add(corrispondente.descrizione);

            return retValue;
        }

        /// <summary>
        /// Tipologia del diritto associato al documenot
        /// </summary>
        /// <param name="documentoDiritto"></param>
        /// <returns></returns>
        public static string GetTipoDiritto(DocsPAWA.DocsPaWR.DocumentoDiritto documentoDiritto)
        {
            string retValue = string.Empty;

            if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_ACQUISITO))
                retValue = "ACQUISITO";
            else
                if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_PROPRIETARIO))
                    retValue = "PROPRIETARIO";
                else
                    if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_TRASMISSIONE))
                        retValue = "TRASMISSIONE";
                    else
                        if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO))
                            retValue = "TRASMISSIONE IN FASC.";
                        else
                            if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_SOSPESO))
                                retValue = "SOSPESO";
                            else
                                if (documentoDiritto.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_DELEGATO))
                                    retValue = "DELEGATO";

            return retValue;
        }

        /// <summary>
        /// Tipologia del corrispondente
        /// </summary>
        /// <param name="documentoDiritto"></param>
        /// <returns></returns>
        public static string GetTipoCorrispondente(DocsPAWA.DocsPaWR.DocumentoDiritto documentoDiritto)
        {
            string retValue = string.Empty;

            if (documentoDiritto.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
                retValue = "UTENTE";

            else if (documentoDiritto.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                retValue = "RUOLO";

            else if (documentoDiritto.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                retValue = "U.O.";

            return retValue;
        }
    }
}
