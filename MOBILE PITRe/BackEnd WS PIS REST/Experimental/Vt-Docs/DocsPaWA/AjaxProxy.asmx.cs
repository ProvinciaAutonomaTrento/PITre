using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Text;
using System.Reflection;
using DocsPaUtils.Data;
using System.Collections.Generic;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for AjaxProxy
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AjaxProxy : System.Web.Services.WebService
    {
        protected static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();


        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaNote(string prefixText, int count, string contextKey)
        {
            string[] lista = null;
            lista = docsPaWS.GetElencoNote(contextKey, prefixText);

            //lista = new string[] { prefixText, count.ToString(), "Ciao", contextKey };

            return lista;

        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaCorrispondentiVeloce(string prefixText, int count, string contextKey)
        {
            //INIZIALIZZAZIONE
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string[] listaTemp = null;
            DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
            string idRuolo = null;
            string tipoRicerca = null;
            string idRegistro = null;
            bool doRuoli = false;
            bool doUo = false;
            bool doUtenti = false;
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            qco.caller = new DocsPAWA.DocsPaWR.RubricaCallerIdentity();
            qco.parent = "";
            char[] delimiterChars = { '-' };
            string[] splitData = contextKey.Split(delimiterChars);
            qco.caller.IdRuolo = splitData[0];
            qco.caller.IdRegistro = splitData[1];
            qco.descrizione = prefixText;
            string callType = splitData[3];
            infoUtente.idAmministrazione = splitData[2];
            bool abilitazioneRubricaComune = RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
            DocsPaWR.Registro[] regTemp = docsPaWS.UtenteGetRegistriWithRf(splitData[0], "", "");
            //Prendo soltanto i corrispondenti del mio registro e di tutti i miei rf se presenti
            DocsPaWR.Registro[] regOnliyTemp = null;
            if (regTemp != null && regTemp.Length > 0)
            {
                int countReg = 0;
                regOnliyTemp = new DocsPaWR.Registro[regTemp.Length];
                for(int y=0;y<regTemp.Length;y++)
                {
                    if ((!string.IsNullOrEmpty(regTemp[y].chaRF) && regTemp[y].chaRF.Equals("1")) || regTemp[y].systemId.Equals(qco.caller.IdRegistro))
                    {
                        regOnliyTemp[countReg] = regTemp[y];
                        countReg++;
                    }
                }
            }

            string retValue = "";

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
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_OUT;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.doListe = true;
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
                // Destinatario su protocollo interno
                case "CALLTYPE_PROTO_INT_DEST":
                    qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_DEST;
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.doListe = true;
                    qco.doRF = false;
                    qco.doRubricaComune = false;
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doUo = true;
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
            }

            listaTemp = docsPaWS.getElementiRubricaVeloce(infoUtente,qco);

            return listaTemp;

        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public virtual string[] GetListaParoleChiaveVeloce(string prefixText, int count, string contextKey)
        {
            string[] items = null;
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DataSet ris = new DataSet();
            string idRegistro = contextKey.Split('-')[0];
            ris = wws.FoundKeyWord(prefixText, idRegistro);
            int n = ris.Tables[0].Rows.Count;
            if(n>0)
            {
                items = new string[n];
            }
            int i = 0;
            foreach (DataRow row in ris.Tables[0].Rows)
            {
                items[i] = row["VAR_DESC_PAROLA"].ToString()+" "+"("+row["SYSTEM_ID"].ToString()+")";
                i++;
            }
            return items;
        }
    }

}
