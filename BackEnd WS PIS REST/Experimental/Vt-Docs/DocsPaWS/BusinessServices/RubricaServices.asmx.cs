using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using VtDocs.BusinessServices.Entities;
using VtDocs.BusinessServices.Entities.Rubrica;

namespace DocsPaWS.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VtDocs/Business/RubricaServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService()]
    public class RubricaServices : BusinessServices, VtDocs.BusinessServices.Services.Rubrica.IRubricaServices
    {
        /// <summary>
        /// Servizio per il reperimenti degli elementi in rubrica a partire dai dati di filtro impostati
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetElementiResponse GetElementi(GetElementiRequest request)
        {
            GetElementiResponse response = new GetElementiResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWebService())
                {
                    string filtroRegistroPerRicerca = string.Empty;

                    foreach (string id in request.IdRegistri)
                    {
                        if (!string.IsNullOrEmpty(filtroRegistroPerRicerca))
                            filtroRegistroPerRicerca += ",";

                        filtroRegistroPerRicerca += id;
                    }

                    var searchParameters = new DocsPaVO.rubrica.ParametriRicercaRubrica 
                    { 
                        caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity
                        {
                            IdRuolo = request.InfoUtente.idGruppo,
                            filtroRegistroPerRicerca = filtroRegistroPerRicerca
                        },
                        calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE,
                        tipoIE = request.TipoUtente,
                        codice = request.FiltroPerCodice,
                        descrizione = request.FiltroPerDescrizione,
                        citta = request.FiltroPerCitta,
                        localita = request.FiltroPerLocalita,
                        doUtenti = request.FiltraUtenti,
                        doRuoli = request.FiltraRuoli,
                        doUo = request.FiltraUO,
                        doListe = request.FiltraListe,
                        doRF = request.FiltraRF,
                        doRubricaComune = request.FiltraRubricaComune
                    };

                    var list = ws.rubricaGetElementiRubrica(searchParameters, request.InfoUtente, null);

                    if (list == null)
                        throw new ApplicationException("Errore nel reperimento degli elementi in rubrica");

                    response.Elementi = (DocsPaVO.rubrica.ElementoRubrica[])
                            list.ToArray(typeof(DocsPaVO.rubrica.ElementoRubrica));
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new GetElementiResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un corrispondente in rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetDettaglioElementoResponse GetDettaglioElemento(GetDettaglioElementoRequest request)
        {
            GetDettaglioElementoResponse response = new GetDettaglioElementoResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWebService())
                {
                    DocsPaVO.addressbook.QueryCorrispondente query = new DocsPaVO.addressbook.QueryCorrispondente
                    {
                        idAmministrazione = request.InfoUtente.idAmministrazione,
                        systemId = request.Id,
                        tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE
                    };

                    var corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(request.Id);

                    if (corrispondente == null)
                        throw new ApplicationException("Errore nel reperimento dei dati dell'elemento richiesto");

                    response.Elemento = new DettaglioElementoRubrica
                    {
                        Id = corrispondente.systemId,
                        Codice = corrispondente.codiceRubrica,
                        Descrizione = corrispondente.descrizione,
                        Email = corrispondente.email
                    };

                    DocsPaVO.addressbook.DettagliCorrispondente dataSet = ws.AddressbookGetDettagliCorrispondente(query) as DocsPaVO.addressbook.DettagliCorrispondente;

                    if (dataSet == null)
                        throw new ApplicationException("Errore nel reperimento dei dati di dettaglio dell'elemento richiesto");

                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        var row = dataSet.Corrispondente.Rows[0];

                        response.Elemento.Id = request.Id;
                        response.Elemento.Indirizzo = row["indirizzo"].ToString();
                        response.Elemento.Citta = row["citta"].ToString();
                        response.Elemento.CAP = row["cap"].ToString();
                        response.Elemento.Provincia = row["provincia"].ToString();
                        response.Elemento.Nazione = row["nazione"].ToString();
                        response.Elemento.Telefono = row["telefono"].ToString();
                        response.Elemento.Telefono2 = row["telefono2"].ToString();
                        response.Elemento.Fax = row["fax"].ToString();
                        response.Elemento.CodiceFiscale = row["codicefiscale"].ToString();
                        response.Elemento.Note = row["note"].ToString();
                        response.Elemento.Localita = row["localita"].ToString();
                        response.Elemento.LuogoNascita = row["luogonascita"].ToString();
                        response.Elemento.DataNascita = row["datanascita"].ToString();
                        response.Elemento.Titolo = row["titolo"].ToString();
                    }
                    else
                        throw new ApplicationException("Nessun elemento trovato in rubrica per l'identificativo richiesto");
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new GetDettaglioElementoResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il salvataggio dei dettagli di un corrispondente in rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public SaveElementiResponse SaveElementi(SaveElementiRequest request)
        {
            SaveElementiResponse response = new SaveElementiResponse();
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();

            try
            {
                using (DocsPaWebService ws = new DocsPaWebService())
                {
                    foreach (var elem in request.Elementi)
                    {
                        bool insertMode = (string.IsNullOrEmpty(elem.Id) || elem.Id == "0");

                        DocsPaVO.utente.Corrispondente corrispondente = null;

                        if (insertMode)
                        {
                            // Per ora, inserimento solo di unità organizzative
                            corrispondente = new DocsPaVO.utente.UnitaOrganizzativa();
                            corrispondente.tipoCorrispondente = "U";

                            corrispondente.idRegistro = elem.IdRegistro;
                            corrispondente.codiceCorrispondente = (elem.Codice ?? string.Empty).Replace("'", "''");
                            corrispondente.codiceRubrica = (elem.Codice ?? string.Empty).Replace("'", "''");

                            corrispondente.descrizione = (elem.Descrizione ?? string.Empty).Replace("'", "''");
                            corrispondente.indirizzo = (elem.Indirizzo ?? string.Empty).Replace("'", "''");
                            corrispondente.citta = (elem.Citta ?? string.Empty).Replace("'", "''");
                            corrispondente.cap = (elem.CAP ?? string.Empty).Replace("'", "''");
                            corrispondente.prov = (elem.Provincia ?? string.Empty).Replace("'", "''");
                            corrispondente.nazionalita = (elem.Nazione ?? string.Empty).Replace("'", "''");
                            corrispondente.telefono1 = (elem.Telefono ?? string.Empty).Replace("'", "''");
                            corrispondente.telefono2 = (elem.Telefono2 ?? string.Empty).Replace("'", "''");
                            corrispondente.fax = (elem.Fax ?? string.Empty).Replace("'", "''");
                            corrispondente.codfisc = (elem.CodiceFiscale ?? string.Empty).Replace("'", "''");
                            corrispondente.note = (elem.Note ?? string.Empty).Replace("'", "''");
                            corrispondente.localita = (elem.Localita ?? string.Empty).Replace("'", "''");
                            corrispondente.luogoDINascita = (elem.LuogoNascita ?? string.Empty).Replace("'", "''");
                            corrispondente.dataNascita = (elem.DataNascita ?? string.Empty).Replace("'", "''");
                            corrispondente.titolo = (elem.Titolo ?? string.Empty).Replace("'", "''");
                            corrispondente.email = (elem.Email ?? string.Empty).Replace("'", "''");
                            corrispondente.dettagli = true;
                            
                            DocsPaVO.addressbook.DettagliCorrispondente dataSet = new DocsPaVO.addressbook.DettagliCorrispondente();
                            var rowCorrispondente = dataSet.Corrispondente.NewCorrispondenteRow();

                            rowCorrispondente.indirizzo = (elem.Indirizzo ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.citta = (elem.Citta ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.cap = (elem.CAP ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.provincia = (elem.Provincia ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.nazione = (elem.Nazione ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.telefono = (elem.Telefono ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.telefono2 = (elem.Telefono2 ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.fax = (elem.Fax ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.codiceFiscale = (elem.CodiceFiscale ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.note = (elem.Note ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.localita = (elem.Localita ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.dataNascita = (elem.DataNascita ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.luogoNascita = (elem.LuogoNascita ?? string.Empty).Replace("'", "''");
                            rowCorrispondente.titolo = (elem.Titolo ?? string.Empty).Replace("'", "''");
                            
                            dataSet.Corrispondente.AddCorrispondenteRow(rowCorrispondente);

                            corrispondente.info = dataSet;

                            corrispondente = ws.AddressbookInsertCorrispondente(corrispondente, null, request.InfoUtente);

                            if (corrispondente == null)
                                throw new ApplicationException("Si è verificato un errore nell'inserimento dei dati dell'elemento");

                            elem.Id = corrispondente.systemId;
                            response.Elementi.Add(elem);
                        }
                        else
                        {
                            corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(elem.Id);

                            DocsPaVO.utente.DatiModificaCorr datiModifica = new DocsPaVO.utente.DatiModificaCorr
                            {
                                idCorrGlobali = corrispondente.systemId,
                                descCorr = (elem.Descrizione ?? string.Empty).Replace("'", "''"),
                                indirizzo = (elem.Indirizzo ?? string.Empty).Replace("'", "''"),
                                citta = (elem.Citta ?? string.Empty).Replace("'", "''"),
                                cap = (elem.CAP ?? string.Empty).Replace("'", "''"),
                                provincia = (elem.Provincia ?? string.Empty).Replace("'", "''"),
                                nazione = (elem.Nazione ?? string.Empty).Replace("'", "''"),
                                telefono = (elem.Telefono ?? string.Empty).Replace("'", "''"),
                                telefono2 = (elem.Telefono2 ?? string.Empty).Replace("'", "''"),
                                fax = (elem.Fax ?? string.Empty).Replace("'", "''"),
                                codFiscale = (elem.CodiceFiscale ?? string.Empty).Replace("'", "''"),
                                note = (elem.Note ?? string.Empty).Replace("'", "''"),
                                localita = (elem.Localita ?? string.Empty).Replace("'", "''"),
                                luogoNascita = (elem.LuogoNascita ?? string.Empty).Replace("'", "''"),
                                dataNascita = (elem.DataNascita ?? string.Empty).Replace("'", "''"),
                                titolo = (elem.Titolo ?? string.Empty).Replace("'", "''"),
                                email = (elem.Email ?? string.Empty).Replace("'", "''")
                            };

                            string newIdCorrispondente;
                            string errorMessage;

                            if (!BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(datiModifica, infoUtente, out newIdCorrispondente, out errorMessage))
                            {
                                throw new ApplicationException(errorMessage);
                            }

                            if (!string.IsNullOrEmpty(newIdCorrispondente))
                            {
                                int newIdCorrispondenteAsInt;
                                if (Int32.TryParse(newIdCorrispondente, out newIdCorrispondenteAsInt))
                                {
                                    if (newIdCorrispondenteAsInt > 0)
                                    {
                                        elem.Id = newIdCorrispondenteAsInt.ToString();
                                    }
                                }
                            }   

                            response.Elementi.Add(elem);
                        }
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new SaveElementiResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la cancellazione di un elemento da rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public DeleteElementoResponse DeleteElemento(DeleteElementoRequest request)
        {
            DeleteElementoResponse response = new DeleteElementoResponse();

            try
            {
                string errorMessage;
                if (!BusinessLogic.Utenti.UserManager.DeleteCorrispondenteEsterno(request.Id, 1, request.InfoUtente, out errorMessage))
                    throw new ApplicationException(errorMessage);
                
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new DeleteElementoResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }
    }
}
