using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using NttDataWA.Utils;
using System.ComponentModel;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA
{
    /// <summary>
    /// Summary description for AjaxProxyC:\Rilasci\3.21.x-Branch\NttDataWA\AjaxProxy.asmx
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AjaxProxy : System.Web.Services.WebService
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();


        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaNote(string prefixText, int count, string contextKey)
        {
            string[] lista = null;
            lista = docsPaWS.GetElencoNote(contextKey, prefixText);
            return lista;
        }



        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCorrispondentiVeloce_trasmD(string prefixText, int count, string contextKey)
        {
            return this.GetListaCorrispondentiVeloce2(prefixText, count, contextKey, string.Empty);
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCorrispondentiVeloce_trasmF(string prefixText, int count, string contextKey)
        {
            return this.GetListaCorrispondentiVeloce2(prefixText, count, contextKey, string.Empty);
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCorrispondentiVeloce(string prefixText, int count, string contextKey)
        {
            return this.GetListaCorrispondentiVeloce2(prefixText, count, contextKey, null);
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCorrispondentiVeloce2(string prefixText, int count, string contextKey, string objType)
        {
            //INIZIALIZZAZIONE
            DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
            string[] listaTemp = null;
            DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();
            qco.caller = new DocsPaWR.RubricaCallerIdentity();
            qco.parent = "";
            char[] delimiterChars = { '-' };
            string[] splitData = contextKey.Split(delimiterChars);
            qco.caller.IdRuolo = splitData[0];
            qco.caller.IdRegistro = splitData[1];
            qco.descrizione = prefixText;

            if (objType != null)
                qco.ObjectType = objType;
            string callType = splitData[3];
            infoUtente.idAmministrazione = splitData[2];
            bool abilitazioneRubricaComune = CommonAddressBook.Configurations.GetConfigurations(infoUtente).GestioneAbilitata;
            DocsPaWR.Registro[] regTemp = docsPaWS.UtenteGetRegistriWithRf(splitData[0], "", "");
            //Prendo soltanto i corrispondenti del mio registro e di tutti i miei rf se presenti
            DocsPaWR.Registro[] regOnliyTemp = null;
            if (regTemp != null && regTemp.Length > 0)
            {
                int countReg = 0;
                regOnliyTemp = new DocsPaWR.Registro[regTemp.Length];
                for (int y = 0; y < regTemp.Length; y++)
                {
                    if ((!string.IsNullOrEmpty(regTemp[y].chaRF) && regTemp[y].chaRF.Equals("1")) || regTemp[y].systemId.Equals(qco.caller.IdRegistro))
                    {
                        regOnliyTemp[countReg] = regTemp[y];
                        countReg++;
                    }
                }
            }

            string retValue = string.Empty;

            foreach (DocsPaWR.Registro item in regOnliyTemp)
            {
                if (item != null)
                {
                    retValue += " " + item.systemId + ",";
                }
            }
            if (retValue.EndsWith(","))
            {
                retValue = retValue.Remove(retValue.LastIndexOf(","));
            }

            qco.caller.filtroRegistroPerRicerca = retValue;

            switch (callType)
            {
                // Mittente su protocollo in ingresso
                case "CALLTYPE_PROTO_IN":
                case "CALLTYPE_CORR_INT_EST":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_IN;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                case "CALLTYPE_CORR_INT_EST_CON_DISABILITATI":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                case "CALLTYPE_CORR_EST_CON_DISABILITATI":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST_CON_DISABILITATI;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.ESTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                // Mittente su protocollo in uscita
                case "CALLTYPE_PROTO_OUT_MITT":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_OUT_MITT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doUo = true;
                    break;
                // Mittente su protocollo interno
                case "CALLTYPE_PROTO_INT_MITT":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    break;
                // Destinatari
                case "CALLTYPE_PROTO_OUT":
                    infoUtente.idGruppo = qco.caller.IdRuolo;
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_OUT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doListe = true;
                    qco.doRF = true;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    if (splitData.Length > 4)
                    {
                        qco.caller.IdPeople = splitData[4];
                        qco.caller.IdUtente = splitData[4];
                    }
                    break;
                // Destinatario su protocollo interno
                case "CALLTYPE_PROTO_INT_DEST":
                    infoUtente.idGruppo = qco.caller.IdRuolo;
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_DEST;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doUo = true;
                    if (splitData.Length > 4)
                    {
                        qco.caller.IdPeople = splitData[4];
                        qco.caller.IdUtente = splitData[4];
                    }
                    break;
                // Mittente protocollo in ingresso semplificato
                case "CALLTYPE_PROTO_INGRESSO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INGRESSO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    break;
                // Mittente protocollo in uscita semplificato
                case "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    break;
                //Destinatari protocollo in uscita semplificato
                case "CALLTYPE_PROTO_USCITA_SEMPLIFICATO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doListe = true;
                    qco.doRF = true;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    break;
                //Mittenti multipli ingresso
                case "CALLTYPE_MITT_MULTIPLI":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_MITT_MULTIPLI;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                // Mittente multiplo protocollo in ingresso semplificato
                case "CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    break;
                // Solo utenti interni
                case "CALLTYPE_IN_ONLY_USER":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    qco.doRuoli = false;
                    qco.doUo = false;
                    qco.doUtenti = true;
                    qco.doRubricaComune = false;
                    break;
                // Solo ruoli interni
                case "CALLTYPE_IN_ONLY_ROLE":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    qco.doRuoli = true;
                    qco.doUo = false;
                    qco.doUtenti = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_CORR_INT":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                case "CALLTYPE_CORR_INT_CON_DISABILITATI":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_CORR_EST":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.ESTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                case "CALLTYPE_CORR_INT_NO_UO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_NO_UO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = false;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_TRASM_ALL":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    if (splitData.Length > 4)
                        qco.ObjectType = splitData[4];
                    break;
                case "CALLTYPE_TRASM_INF":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    if (splitData.Length > 4)
                        qco.ObjectType = splitData[4];
                    break;
                case "CALLTYPE_TRASM_SUP":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    if (splitData.Length > 4)
                        qco.ObjectType = splitData[4];
                    break;
                case "CALLTYPE_TRASM_PARILIVELLO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    if (splitData.Length > 4)
                        qco.ObjectType = splitData[4];
                    break;
                case "CALLTYPE_OWNER_AUTHOR":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_GESTFASC_LOCFISICA":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_GESTFASC_LOCFISICA;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = false;
                    qco.doUo = true;
                    qco.doUtenti = false;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_RICERCA_TRASM_SOTTOPOSTO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = true;
                    qco.doUo = false;
                    qco.doUtenti = false;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
                case "CALLTYPE_LISTE_DISTRIBUZIONE":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doRuoli = true;
                    qco.doUo = true;
                    qco.doUtenti = true;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = true;
                    if (abilitazioneRubricaComune == false)
                    {
                        qco.doRubricaComune = false;
                    }
                    break;
                case "CALLTYPE_STAMPA_REG_UO":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_STAMPA_REG_UO;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doRuoli = false;
                    qco.doUo = true;
                    qco.doUtenti = false;
                    qco.doListe = false;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    break;
            }

            listaTemp = docsPaWS.getElementiRubricaVeloce(infoUtente, qco);

            return listaTemp;

        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListDescriptionProject(string prefixText, int count, string contextKey)
        {
            DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
            string[] listaTemp = null;

            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();

            char[] delimiterChars = { '-' };
            string[] splitData = contextKey.Split(delimiterChars);

            string IdRuolo = splitData[0];
            infoUtente.idGruppo = IdRuolo;
            string IdRegistro = splitData[1];
            infoUtente.idAmministrazione = splitData[2];
            string idTitolario = splitData[3];
            infoUtente.idPeople = splitData[5];
            infoUtente.idCorrGlobali = splitData[5];
            infoUtente.idGruppo = IdRuolo;
            string testoRicerca = prefixText;

            Registro reg = UIManager.RegistryManager.getRegistroBySistemId(IdRegistro);

            int pageSize = 10;
            int pageNumbers;
            int recordNumber;

            SearchResultInfo[] idProjects;

            DocsPaWR.FiltroRicerca[] fVList = null;
            DocsPaWR.FiltroRicerca fV1 = null;
            DocsPaWR.FiltroRicerca[][] qV = null;

            //array contenitore degli array filtro di ricerca
            qV = new DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPaWR.FiltroRicerca[1];
            fVList = new DocsPaWR.FiltroRicerca[0];

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "TITOLO";
            fV1.nomeCampo = null;
            fV1.valore = testoRicerca;
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "INCLUDI_FASCICOLI_FIGLI";
            fV1.nomeCampo = null;
            fV1.valore = "N";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "ID_TITOLARIO";
            fV1.nomeCampo = null;
            fV1.valore = idTitolario;
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "SOTTOFASCICOLO";
            fV1.nomeCampo = null;
            fV1.valore = "";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "EXTEND_TO_HISTORICIZED_OWNER";
            fV1.nomeCampo = null;
            fV1.valore = "False";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "CORR_TYPE_OWNER";
            fV1.nomeCampo = null;
            fV1.valore = "R";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "EXTEND_TO_HISTORICIZED_AUTHOR";
            fV1.nomeCampo = null;
            fV1.valore = "False";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "CORR_TYPE_AUTHOR";
            fV1.nomeCampo = null;
            fV1.valore = "R";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "ORACLE_FIELD_FOR_ORDER";
            fV1.nomeCampo = "P20";
            fV1.valore = "A.DTA_CREAZIONE";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "SQL_FIELD_FOR_ORDER";
            fV1.nomeCampo = "P20";
            fV1.valore = "A.DTA_CREAZIONE";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPaWR.FiltroRicerca();
            fV1.argomento = "ORDER_DIRECTION";
            fV1.nomeCampo = "P20";
            fV1.valore = "DESC";
            fVList = UIManager.ProjectManager.addToArrayFiltroRicerca(fVList, fV1);

            qV[0] = fVList;

            listaTemp = UIManager.ProjectManager.getListaFascicoliPagingCustomRicVelolce(
               infoUtente,         //0
               null,               //1
               reg,                //2
               qV[0],              //3
               false,              //4
               0,                  //5
               out pageNumbers,    //6
               out recordNumber,   //7
               pageSize,           //8
               true,               //9
               out idProjects,     //10
               null,               //11
               false,              //12
               true,               //13
               null,               //14
               null,               //15
               true);              //16

            // getListaFascicoliPagingCustomRicVelolce(
            //InfoUtente infoUtente,                            0           
            //FascicolazioneClassificazione classificazione,    1
            //DocsPaWR.Registro registro,                       2
            //FiltroRicerca[] filtriRicerca,                    3
            //bool childs,                                      4
            //int requestedPage,                                5
            //out int numTotPage,                               6
            //out int nRec,                                     7
            //int pageSize,                                     8
            //bool getSystemIdList,                             9
            //out SearchResultInfo[] idProjectList,             10
            //byte[] datiExcel,                                 11    
            //bool showGridPersonalization,                     12
            //bool export,                                      13
            //Field[] visibleFieldsTemplate,                    14 
            //String[] documentsSystemId,                       15
            //bool security)                                    16

            return listaTemp;
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCapComuni(string prefixText, string contextKey)
        {
            string[] listaTemp = null;

            listaTemp = AddressBookManager.GetListaCapComuni(prefixText, contextKey);

            return listaTemp;
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaComuni(string prefixText, string contextKey)
        {
            string[] listaTemp = null;

            listaTemp = AddressBookManager.GetListaComuni(prefixText);

            return listaTemp;
        }

    }
}
