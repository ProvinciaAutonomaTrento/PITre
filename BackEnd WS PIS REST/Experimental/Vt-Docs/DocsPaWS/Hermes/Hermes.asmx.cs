using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;
using DocsPaVO.amministrazione;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.filtri;
using DocsPaVO.filtri.fascicolazione;
using System.Configuration;

namespace DocsPaWS.Hermes
{
    /// <summary>
    /// Summary description for Hermes
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Hermes : System.Web.Services.WebService
    {
       [WebMethod(Description="Creazione di un fascicolo contenente il bando di gara")]
       public virtual BandoSyncResponse InsertBando(BandoSyncRequest _bandoSyncRequest)
       {
           
           string user = string.Empty;
           string password = "0rcassasina";
           string codiceNodoTitolario = string.Empty;
           string NomeTemplate = string.Empty;
           
           if (ConfigurationManager.AppSettings["UtenteIntegrazione"] != null)
               user = ConfigurationManager.AppSettings["UtenteIntegrazione"];

           if (ConfigurationManager.AppSettings["CodiceNodoTitolario"] != null)
               codiceNodoTitolario = ConfigurationManager.AppSettings["CodiceNodoTitolario"];

           if (ConfigurationManager.AppSettings["NomeTemplate"] != null)
               NomeTemplate = ConfigurationManager.AppSettings["NomeTemplate"];

           BandoSyncResponse _BandoSyncResponse = new BandoSyncResponse();
           Utility _utility = new Utility();
           UserLogin login = new UserLogin();
           login.UserName = user;
           login.Password = password;
           Utente utente = _utility.Login(login);
           
           if (utente.ruoli.Count > 0)
           {
               Ruolo ruolo = utente.ruoli[0] as Ruolo;
               Registro[] registri = _utility.RegistroGetRegistriRuolo(ruolo.systemId);
               if (registri.Length > 0)
               {

                   OrgTitolario _classificazione = _utility.GetTitolarioAttivo(utente.idAmministrazione);
                   if (_classificazione != null)
                   {
                       Classificazione nodo = _utility.FascicolazioneGetClassificazione(ruolo.idAmministrazione, ruolo.idGruppo, utente.idPeople, registri[0], codiceNodoTitolario, _classificazione.ID);
                       if (nodo != null)
                       {
                           Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(NomeTemplate);
                           if (template.ELENCO_OGGETTI.Count > 0)
                           {


                               template.ELENCO_OGGETTI[0] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[0] as OggettoCustom, _bandoSyncRequest.Mandante);
                               template.ELENCO_OGGETTI[1] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[1] as OggettoCustom, _bandoSyncRequest.NumeroGaraAppalto.ToString());
                               template.ELENCO_OGGETTI[2] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[2] as OggettoCustom, _bandoSyncRequest.OrganizzazioneAcquisti);
                               template.ELENCO_OGGETTI[3] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[3] as OggettoCustom, _bandoSyncRequest.GruppoAcquisti);
                               template.ELENCO_OGGETTI[4] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[4] as OggettoCustom, _bandoSyncRequest.DescrizioneGara);
                               template.ELENCO_OGGETTI[5] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[5] as OggettoCustom, _bandoSyncRequest.DataScadenzaGara.ToString());
                               template.ELENCO_OGGETTI[6] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[6] as OggettoCustom, _bandoSyncRequest.DataEmissioneGara.ToString());
                               template.ELENCO_OGGETTI[7] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[7] as OggettoCustom, _bandoSyncRequest.DataAperturaBusteTecniche.ToString());
                               template.ELENCO_OGGETTI[8] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[8] as OggettoCustom, _bandoSyncRequest.DataRicezioneRelazioneTecnica.ToString());
                               template.ELENCO_OGGETTI[9] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[9] as OggettoCustom, _bandoSyncRequest.DataAperturaBusteEconomiche.ToString());
                               template.ELENCO_OGGETTI[10] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[10] as OggettoCustom, _bandoSyncRequest.DataPropostaAggiudicazioneTrattativa.ToString());
                               template.ELENCO_OGGETTI[11] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[11] as OggettoCustom, _bandoSyncRequest.DataAggiudicazione.ToString());
                               template.ELENCO_OGGETTI[12] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[12] as OggettoCustom, _bandoSyncRequest.DataAnnulamentoGara.ToString());
                               template.ELENCO_OGGETTI[13] = _utility.salvaValoreOggetto(template.ELENCO_OGGETTI[13] as OggettoCustom, _bandoSyncRequest.ValoreGara.ToString());

                               InfoUtente infoutente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utente, ruolo);
                               FiltroRicerca[][] listaFiltri = new FiltroRicerca[1][];
                               listaFiltri[0] = new FiltroRicerca[1];
                               List<FiltroRicerca> fVList = new List<FiltroRicerca>();
                               FiltroRicerca fV1 = new FiltroRicerca();
                               fV1.argomento = listaArgomenti.TIPO_FASCICOLO.ToString();
                               fV1.valore = "P";
                               fVList.Add(fV1);

                               fV1.argomento = listaArgomenti.PROFILAZIONE_DINAMICA.ToString();
                               fV1.template = template;
                               fV1.argomento = string.Empty;
                               fVList.Add(fV1);


                               listaFiltri[0] = fVList.ToArray();
                               //La classificazione è null perchè non specifico nessun nodo di titolario
                               //Ricerco i fascicoli
                               Fascicolo[] fascicoli = _utility.FascicolazioneGetListaFascicoli(null, listaFiltri[0], false, false, false, infoutente);
                               Fascicolo _fascicoloNew = null;
                               if (fascicoli.Length > 0)
                               {

                                   fascicoli[0].template = template;
                                   _fascicoloNew =  BusinessLogic.Fascicoli.FascicoloManager.updateFascicolo(fascicoli[0]);
                                   if (_fascicoloNew != null)
                                   {
                                       _BandoSyncResponse.CodiceEsito = 0;
                                       _BandoSyncResponse.DescrEsito = "Modifica effettuata correttamente";
                                   }
                               }
                               else
                               {
                                   Fascicolo _fascicolo = new Fascicolo();
                                   _fascicolo.descrizione = _bandoSyncRequest.DescrizioneGara;
                                   _fascicolo.idRegistro = registri[0].systemId;
                                   _fascicolo.apertura = System.DateTime.Now.ToShortDateString();
                                   _fascicolo.privato = "0";
                                   _fascicolo.codUltimo = _utility.FascicolazioneGetFascNumRif(nodo.systemID, registri[0].systemId);
                                   _fascicolo.template = template;
                                   //creazione di un nuovo fascicolo
                                   _fascicoloNew = _utility.FascicolazioneNewFascicolo(nodo, _fascicolo, false, infoutente, ruolo);

                                   if (_fascicoloNew != null)
                                   {
                                       _BandoSyncResponse.CodiceEsito = 0;
                                       _BandoSyncResponse.DescrEsito = "Inserimento effettuato correttamente";
                                   }

                               }
                           }
                           else
                           {
                               _BandoSyncResponse.CodiceEsito = 200;
                               _BandoSyncResponse.DescrEsito = "Il template " + NomeTemplate + " non è corretto o inesiste";
                           }
                       }
                       else
                       {
                           _BandoSyncResponse.CodiceEsito = 310;
                           _BandoSyncResponse.DescrEsito = "Il nodo" + codiceNodoTitolario + " non è corretto o inesiste";

                       }
                   }
                   else
                   {
                       _BandoSyncResponse.CodiceEsito = 300;
                       _BandoSyncResponse.DescrEsito = "Non è presente nessun titolario attivo";
                   }
               }
               else
               {
                   _BandoSyncResponse.CodiceEsito = 100;
                   _BandoSyncResponse.DescrEsito = "L'utente non ha associato nessun Registro";
               }
           }
           else
           {
               _BandoSyncResponse.CodiceEsito = 600;
               _BandoSyncResponse.DescrEsito = "Autenticazione utente fallita";
           }
    
           return _BandoSyncResponse;
       }
    }
}
