using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using log4net;
using VtDocs.BusinessServices.Entities;
using VtDocs.BusinessServices.Entities.Administration;
using VtDocs.BusinessServices.Entities.Administration.Organigramma;

namespace DocsPaWS.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VtDocs/Business/OrganigrammaServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService()]
    public class OrganigrammaServices : BusinessServices, VtDocs.BusinessServices.Administration.IOrganigrammaServices
    {
        /// <summary>
        /// 
        /// </summary>
        private static ILog _logger = LogManager.GetLogger(typeof(OrganigrammaServices));

        /// <summary>
        /// Servizio per il reperimento dei dati di un ruolo in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloResponse GetRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloResponse();

            try
            {
                response.Ruolo = BusinessLogic.Amministrazione.OrganigrammaManager.GetRole(request.Id);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaResponse GetOrganigramma(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaResponse();

            try
            {
                response.UnitaOrganizzativa = this.BuildOrganigramma(request.IdUnitaOrganizzativa,
                                                        request.IdAmministrazione,
                                                        request.Recursive,
                                                        request.FetchRuoliIfNotRecursive,
                                                        request.GetQualificheUtentiInRuolo);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio di ricerca di elementi in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaResponse Ricerca(VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaResponse();

            try
            {
                ArrayList result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmRicercaInOrg(
                                                                request.TipoRicercaDescrizione,
                                                                request.Codice,
                                                                request.Descrizione,
                                                                request.IdAmministrazione,
                                                                request.RicercaStoricizzati,
                                                                request.RicercaPerCodiceEsatto);

                if (result == null)
                    throw new ApplicationException("Errore nella ricerca in organigramma");

                response.List = new List<DocsPaVO.amministrazione.OrgRisultatoRicerca>();

                foreach (DocsPaVO.amministrazione.OrgRisultatoRicerca item in result)
                    response.List.Add(item);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected string GetIdPeople(DocsPaVO.utente.InfoUtente infoUtente, string userId)
        {
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(userId);

            if (utente == null)
                utente = BusinessLogic.Utenti.UserManager.getUtente(userId, infoUtente.idAmministrazione);

            if (utente == null)
                utente = BusinessLogic.Utenti.UserManager.getUtenteByMatricola(userId, infoUtente.idAmministrazione);

            if (utente != null)
                return utente.idPeople;
            else
                return null;
        }

        ///// <summary>
        ///// Servizio per il reperimento dei ruoli superiori in organigramma
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[WebMethod()]
        //public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoliSuperioriResponse GetRuoliSuperiori(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoliSuperioriRequest request)
        //{
        //    VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoliSuperioriResponse response = new GetRuoliSuperioriResponse();

        //    try
        //    {
        //        // Reperimento del ruolo corrente dell'utente
        //        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.InfoUtente.idGruppo);

        //        response.Superiori = this.GetRuoliSuperiori(ruolo.uo.systemId, ruolo.livello);
        //        response.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoliSuperioriResponse();
        //        response.Success = false;

        //        if (request.TrowOnError)
        //            throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
        //        else
        //            response.Exception = ex.Message;
        //    }

        //    return response;
        //}

        /// <summary>
        /// Servizio per il reperimento degli utenti superiori in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse GetUtentiSuperiori(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse();
            
            try
            {
                DocsPaVO.utente.Ruolo[] ruoli = null;

                if (string.IsNullOrEmpty(request.IdUtente))
                {
                    // Reperimento dei superiori per l'utente corrente
                    
                    
                    if (request.GetSuperioriRuoloCorrente)
                    {
                        // Calcolo solo del ruolo corrente
                        List<DocsPaVO.utente.Ruolo> list = new List<DocsPaVO.utente.Ruolo>();
                        list.Add(BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.InfoUtente.idGruppo));
                        ruoli = list.ToArray();
                    }
                    else
                    {
                        // Reperimento di tutti i ruoli dell'utente corrente 
                        ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(request.InfoUtente.idPeople.ToString()).ToArray(typeof(DocsPaVO.utente.Ruolo));
                    }
                }
                else
                {
                    // Reperimento dei superiori per un particolare utente
                    string idPeople = this.GetIdPeople(request.InfoUtente, request.IdUtente);

                    // Reperimento di tutti i ruoli per l'utente richiesto
                    ruoli = (DocsPaVO.utente.Ruolo[]) BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople.ToString()).ToArray(typeof(DocsPaVO.utente.Ruolo));

                    if (!string.IsNullOrEmpty(request.IdUO))
                    {
                        ruoli = ruoli.Where(e => e.uo.systemId == request.IdUO).ToArray();                        
                    }
                }

                const string LIVELLO_SUPERIORI = "1000";

                foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                {
                    string livelloSuperiori = (request.IgnoraLivelloSuperiori ? LIVELLO_SUPERIORI : ruolo.livello);

                    var internalResponse = this.GetUtentiSuperiori(ruolo.uo.systemId, ruolo.livello, ruolo.systemId, request.CodiceQualifica, request.GetPrimoSuperiore, livelloSuperiori);

                    response.Superiori.AddRange(internalResponse.Superiori);
                    response.RuoliSuperiori.AddRange(internalResponse.RuoliSuperiori);
                }

                // Ordinamento per gerarchia
                response.Superiori = response.Superiori.Distinct().OrderBy(e => e.Ruolo.Gerarchia).ToList();
                response.RuoliSuperiori = response.RuoliSuperiori.Distinct().OrderBy(e => e.Gerarchia).ToList();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUO"></param>
        /// <param name="livello"></param>
        /// <returns></returns>
        private List<RuoloInUO> GetRuoliSuperiori(string idUO, string livello)
        {
            List<RuoloInUO> superiori = new List<RuoloInUO>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_RUOLI_SUPERIORI");

                queryDef.setParam("livello", livello);
                queryDef.setParam("idUO", idUO);

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_RUOLI_SUPERIORI: {0}", commandText);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        // Inserimento degli utenti superiori
                        superiori.Add(new RuoloInUO
                        {
                            Id = this.GetReaderIntValue(reader, "IdRuolo"),
                            Codice = this.GetReaderStringValue(reader, "CodiceRuolo"),
                            Descrizione = this.GetReaderStringValue(reader, "DescrizioneRuolo"),
                            Livello = this.GetReaderIntValue(reader, "LivelloTipoRuolo"),
                            Responsabile = (this.GetReaderStringValue(reader, "RuoloResponsabile") == "1"),
                            Segretario = (this.GetReaderStringValue(reader, "RuoloSegretario") == "1"),
                            DiRiferimento = (this.GetReaderStringValue(reader, "RuoloRiferimento") == "1"),
                            IdUO = this.GetReaderIntValue(reader, "IdUO"),
                            CodiceUO = this.GetReaderStringValue(reader, "CodiceUO"),
                            DescrizioneUO = this.GetReaderStringValue(reader, "DescrizioneUO"),
                            ClassificaUO = this.GetReaderStringValue(reader, "ClassificaUO"),
                            IdParentUO = this.GetReaderIntValue(reader, "IdParentUO")
                        });
                    }
                }
            }

            return superiori;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUO"></param>
        /// <param name="livello"></param>
        /// <param name="idRuolo"></param>
        /// <param name="codiceQualifica"></param>
        /// <param name="getPrimoSuperiore">
        /// Indica se restituire soltanto il primo superiore trovato
        /// </param>
        /// <param name="livelloSuperiori"></param>
        /// <returns></returns>
        private VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse GetUtentiSuperiori(
                        string idUO, 
                        string livello,
                        string idRuolo, 
                        string codiceQualifica,
                        bool getPrimoSuperiore,
                        string livelloSuperiori)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse();
            
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_SUPERIORI");

                queryDef.setParam("myLiv", livello);
                queryDef.setParam("myUo", idUO);
                queryDef.setParam("myRole", idRuolo);
                queryDef.setParam("livSuperiori", (string.IsNullOrEmpty(livelloSuperiori) ? livello : livelloSuperiori));

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UTENTI_SUPERIORI: {0}", commandText);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        // Reperimento utente superiore (per qualifica se richiesto)
                        var utenteSuperiore = this.CreateUtenteSuperiore(reader);
                        
                        bool addUtente = true;

                        if (getPrimoSuperiore && response.Superiori.Count > 0)
                        {
                            // Se richiesto il reperimento solo del primo utente superiore,
                            // viene inibito l'inserimento dell'utente
                            addUtente = false;
                        }

                        if (addUtente)
                        {
                            if (!string.IsNullOrEmpty(codiceQualifica))
                            {
                                var qualificheUtenteInRuolo = BusinessLogic.utenti.QualificheManager.GetPeopleGroupsQualifiche
                                                            (utenteSuperiore.IdAmministrazione.ToString(),
                                                            utenteSuperiore.Ruolo.IdUO.ToString(),
                                                            utenteSuperiore.Ruolo.Id.ToString(),
                                                            utenteSuperiore.Id.ToString());

                                addUtente = (qualificheUtenteInRuolo.Count(e => e.CODICE.Equals(codiceQualifica, StringComparison.InvariantCultureIgnoreCase)) > 0);
                            }

                            if (addUtente)
                            {
                                // Inserimento degli utenti superiori
                                response.Superiori.Add(utenteSuperiore);
                            }
                        }

                        // Reperimento ruolo superiore
                        var ruoloSuperiore = this.CreateRuoloSuperiore(reader);

                        if (response.RuoliSuperiori.Count(e => e.Id == ruoloSuperiore.Id) == 0)
                        {
                            response.RuoliSuperiori.Add(ruoloSuperiore);
                        }

                    }
                }
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private UtenteInRuolo CreateUtenteSuperiore(System.Data.IDataReader reader)
        {
            return new UtenteInRuolo
            {
                Id = this.GetReaderIntValue(reader, "Id"),
                UserId = this.GetReaderStringValue(reader, "UserId"),
                Nome = this.GetReaderStringValue(reader, "Nome"),
                Cognome = this.GetReaderStringValue(reader, "Cognome"),
                NomeCompleto = this.GetReaderStringValue(reader, "NomeCompleto"),
                Matricola = this.GetReaderStringValue(reader, "Matricola"),
                IdAmministrazione = this.GetReaderIntValue(reader, "IdAmministrazione"),
                EMail = this.GetReaderStringValue(reader, "EMail"),
                Ruolo = this.CreateRuoloSuperiore(reader)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private RuoloInUO CreateRuoloSuperiore(System.Data.IDataReader reader)
        {
            return new RuoloInUO
            {
                Id = this.GetReaderIntValue(reader, "IdRuolo"),
                Codice = this.GetReaderStringValue(reader, "CodiceRuolo"),
                Descrizione = this.GetReaderStringValue(reader, "DescrizioneRuolo"),
                CodiceTipoRuolo = this.GetReaderStringValue(reader, "CodiceTipoRuolo"),
                DescrizioneTipoRuolo = this.GetReaderStringValue(reader, "DescrizioneTipoRuolo"),
                Livello = this.GetReaderIntValue(reader, "LivelloTipoRuolo"),
                Responsabile = (this.GetReaderStringValue(reader, "RuoloResponsabile") == "1"),
                Segretario = (this.GetReaderStringValue(reader, "RuoloSegretario") == "1"),
                DiRiferimento = (this.GetReaderStringValue(reader, "RuoloRiferimento") == "1"),
                Gerarchia = this.GetReaderIntValue(reader, "Gerarchia"),
                IdUO = this.GetReaderIntValue(reader, "IdUO"),
                CodiceUO = this.GetReaderStringValue(reader, "CodiceUO"),
                DescrizioneUO = this.GetReaderStringValue(reader, "DescrizioneUO"),
                ClassificaUO = this.GetReaderStringValue(reader, "ClassificaUO"),
                IdParentUO = this.GetReaderIntValue(reader, "IdParentUO")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private string GetReaderStringValue(System.Data.IDataReader reader, string fieldName)
        {
            return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? string.Empty : reader.GetString(reader.GetOrdinal(fieldName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private int GetReaderIntValue(System.Data.IDataReader reader, string fieldName)
        {
            return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? 
                        0 :
                        Convert.ToInt32(reader.GetValue(reader.GetOrdinal(fieldName)));
        }

        /// <summary>
        /// Servizio per il reperimento dei responsabili per un utente in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse GetResponsabili(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse();

            try
            {
                List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile> utenti = new List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile>();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_IN_ORGANIGRAMMA");
                queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                queryDef.setParam("idUo", request.IdUO);
                queryDef.setParam("codiceQualifica", request.QualificaDiscriminanteUO);
                //queryDef.setParam("codiceQualificaUtente", request.Qualifica);
                queryDef.setParam("customFilters", string.Format("WHERE DOCSADM.utenteHasQualifica('{0}', ID_UTENTE) = 1 AND RESPONSABILE = '1'", request.Qualifica));

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UTENTI_IN_ORGANIGRAMMA_CON_QUALIFICA: {0}", commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            utenti.Add(new UtenteResponsabile
                            {
                                Id = reader.GetValue(reader.GetOrdinal("ID_UTENTE")).ToString(),
                                Codice = reader.GetString(reader.GetOrdinal("USER_ID")),
                                Cognome = reader.GetString(reader.GetOrdinal("COGNOME")),
                                Nome = reader.GetString(reader.GetOrdinal("NOME")),
                                Matricola = reader.GetString(reader.GetOrdinal("MATRICOLA"))
                            });
                        }
                    }
                }

                response.Utenti = utenti.ToArray();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;


            //VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse();

            //try
            //{
            //    List<UOByQualifica> listUoByQualificaDiscriminante = new List<UOByQualifica>();

            //    if (!string.IsNullOrEmpty(request.QualificaDiscriminanteUO))
            //    {
            //        var responseUOs = this.GetListUOByQualificheUtente(
            //                                    new GetListUOByQualificheUtenteRequest
            //                                    {
            //                                        InfoUtente = request.InfoUtente,
            //                                        QualificheUtente = new List<string>() 
            //                                        { 
            //                                            request.QualificaDiscriminanteUO 
            //                                        },
            //                                        TrowOnError = true
            //                                    });

            //        if (responseUOs.Success)
            //        {
            //            listUoByQualificaDiscriminante = responseUOs.UOs;
            //        }
            //    }

            //    List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile> utenti = new List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile>();

            //    string idPeople = string.Empty;

            //    if (!string.IsNullOrEmpty(request.IdUtente))
            //        idPeople = this.GetIdPeople(request.InfoUtente, request.IdUtente);
            //    else
            //        idPeople = request.InfoUtente.idPeople;

            //    foreach (var idUo in this.GetListIdUoUtente(idPeople))
            //    {
            //        this.FetchUtentiResponsabili(request.InfoUtente, listUoByQualificaDiscriminante, request.Qualifica, idUo, utenti);
            //    }

            //    response.Utenti = utenti.ToArray();
            //    response.Success = true;
            //}
            //catch (Exception ex)
            //{
            //    response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse();
            //    response.Success = false;

            //    if (request.TrowOnError)
            //        throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            //    else
            //        response.Exception = ex.Message;
            //}

            //return response;
        }

        /// <summary>
        /// Reperimento degli id uo per l'utente richiesto
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private List<int> GetListIdUoUtente(string idPeople)
        {
            List<int> retValue = new List<int>();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LIST_UO_UTENTE");
            queryDef.setParam("idPeople", idPeople);

            string commandText = queryDef.getSQL();
            _logger.InfoFormat("S_GET_LIST_UO_UTENTE: {0}", commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        retValue.Add(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_UO")).ToString()));
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Servizio per il reperimento delle UO contenenti almeno un utente con la qualifica specificata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetListUOByQualificheUtenteResponse GetListUOByQualificheUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetListUOByQualificheUtenteRequest request)
        {
            GetListUOByQualificheUtenteResponse response = new GetListUOByQualificheUtenteResponse();

            try
            {
                if (request.QualificheUtente == null || 
                    (request.QualificheUtente != null && request.QualificheUtente.Count == 0))
                {
                    throw new ApplicationException("Nessuna qualifica specificata");
                }

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LIST_UO_BY_QUALIFICHE_UTENTE");

                string qualifiche = string.Empty;

                foreach (var q in request.QualificheUtente)
                {
                    if (!string.IsNullOrEmpty(qualifiche))
                        qualifiche += ", ";
                    qualifiche += string.Format("'{0}'", q);
                }

                queryDef.setParam("qualifiche", qualifiche);

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_LIST_UO_BY_QUALIFICHE_UTENTE: {0}", commandText);

                List<UOByQualifica> uos = new List<UOByQualifica>();
                
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            uos.Add(new UOByQualifica
                            {
                                Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")).ToString()),
                                Codice = reader.GetString(reader.GetOrdinal("CODICE")),
                                Descrizione = reader.GetString(reader.GetOrdinal("DESCRIZIONE")),
                                Classifica = (reader.IsDBNull(reader.GetOrdinal("CLASSIFICA_UO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CLASSIFICA_UO"))),
                                Livello = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LIVELLO")).ToString())
                            });
                        }
                    }

                    response.UOs = uos;
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new GetListUOByQualificheUtenteResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// Servizio per il reperimento dei dati relativi posizione di un utente in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteResponse GetSchedaUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteResponse();

            try
            {
                string idPeople = request.InfoUtente.idPeople;

                if (!string.IsNullOrEmpty(request.IdUtente))
                {
                    idPeople = this.GetIdPeople(request.InfoUtente, request.IdUtente);
                }

                List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UO> list = new List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UO>();

                const string LIVELLO_SUPERIORI = "1000";

                // Reperimento ruoli utente
                DocsPaVO.utente.Ruolo[] ruoliUtente = (DocsPaVO.utente.Ruolo[]) 
                    BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

                foreach (DocsPaVO.utente.Ruolo ruolo in ruoliUtente)
                {
                    List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile> utentiResponsabili = new List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile>();

                    string livelloSuperiori = (request.IgnoraLivelloSuperiori ? LIVELLO_SUPERIORI : ruolo.livello);

                    foreach (var superiore in this.GetUtentiSuperiori(ruolo.uo.systemId, ruolo.livello, ruolo.systemId, string.Empty, request.GetPrimoSuperiorePerRuolo, livelloSuperiori).Superiori.Where(e => e.Ruolo.Responsabile))
                    {
                        utentiResponsabili.Add(
                            new UtenteResponsabile
                            {
                                Id = superiore.Id.ToString(),
                                Codice = superiore.UserId,
                                Matricola = superiore.Matricola,
                                Cognome = superiore.Cognome,
                                Nome = superiore.Nome,
                                UO = superiore.Ruolo.DescrizioneUO
                            });
                    }

                    list.Add(
                        new VtDocs.BusinessServices.Entities.Administration.Organigramma.UO
                        {
                            DatiUO = new DocsPaVO.amministrazione.OrgUO
                            {
                                Codice = ruolo.uo.codice,
                                Descrizione = ruolo.uo.descrizione,
                                IDCorrGlobale = ruolo.uo.systemId,
                                CodiceRubrica = ruolo.uo.codiceRubrica,
                                IDAmministrazione = ruolo.uo.idAmministrazione,
                                IDParent = (ruolo.uo.parent != null ? ruolo.uo.parent.systemId : string.Empty)
                            },
                            PathIdUO = ruolo.uo.GetPath(),
                            Responsabili = utentiResponsabili
                        }
                    );
                }

                response.Utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                response.Utente.ruoli = new ArrayList(ruoliUtente);

                if (request.GetQualifiche)
                {
                    // Reperimento, se richiesto, delle qualifiche assegnate all'utente 
                    response.Qualifiche = BusinessLogic.utenti.QualificheManager.GetPeopleGroupsQualificheByIdPeople(response.Utente.idPeople);
                }

                response.UnitaOrganizzative = list.ToArray();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// Servizio per il reperimento dei dati utente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteResponse GetUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteResponse response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteResponse();
            response.Utente = new DocsPaVO.utente.Utente();

            try
            {
                //Reperimento dati utente a partire dallo user_id
                if (!String.IsNullOrEmpty(request.UserId))
                {
                    response.Utente = BusinessLogic.Utenti.UserManager.getUtente(request.UserId, request.InfoUtente.idAmministrazione);
                    response.Success = true;
                }
                //Reperimento dati utente a partire dalla matricola
                else if (!String.IsNullOrEmpty(request.Matricola))
                {
                    response.Utente = BusinessLogic.Utenti.UserManager.getUtenteByMatricola(request.Matricola, request.InfoUtente.idAmministrazione);
                    response.Success = true;
                }
                //Reperimento dati utente a partire dalla system_id
                else if (request.IdPeople != 0)
                {
                    response.Utente = BusinessLogic.Utenti.UserManager.getUtente(request.IdPeople.ToString());
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }

                if (response.Success)
                {
                    response.Utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(response.Utente.idPeople.ToString());
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti in ogranigramma a partire da una UO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaResponse GetUtentiInOrganigramma(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaResponse response = new GetUtentiInOrganigrammaResponse();

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_IN_ORGANIGRAMMA");

                queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                string idUo = string.Empty;

                if (string.IsNullOrEmpty(request.IdUo))
                {
                    // Determinazione dell'id dell'uo a partire dall'info utente
                    var ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.InfoUtente.idGruppo);

                    idUo = ruolo.uo.systemId;
                }
                else
                    idUo = request.IdUo;
                
                queryDef.setParam("idUo", idUo);
                queryDef.setParam("codiceQualifica", request.CodiceQualificaDiscriminanteIfRecursive);

                string customFilters = string.Empty;

                if (!string.IsNullOrEmpty(request.FiltroPerDenominazione))
                {
                    customFilters = string.Format("NOME_COMPLETO LIKE '{0}%'", request.FiltroPerDenominazione.Replace("'", "''"));
                }

                if (!string.IsNullOrEmpty(request.FiltroPerCodiceQualifica))
                {
                    // Reperimento dei soli utenti che hanno la qualifica richiesta
                    if (!string.IsNullOrEmpty(customFilters))
                        customFilters += " AND ";

                    customFilters = customFilters + string.Format("{0}.UtenteHasQualifica('{1}', ID_UTENTE) > 0", 
                                                DocsPaDbManagement.Functions.Functions.GetDbUserSession(),  
                                                request.FiltroPerCodiceQualifica);
                }

                if (request.FiltroPerUtentiResponsabili)
                {
                    // Reperimento dei soli utenti che fanno parte di un ruolo responsabile
                    if (!string.IsNullOrEmpty(customFilters))
                        customFilters += " AND ";

                    customFilters += "RESPONSABILE = '1'";
                }

                if (!string.IsNullOrEmpty(customFilters))
                {
                    customFilters = string.Format(" WHERE {0}", customFilters);
                }

                queryDef.setParam("customFilters", customFilters);

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UTENTI_IN_ORGANIGRAMMA: {0}", commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            if (response.Utenti == null)
                                response.Utenti = new List<GetUtentiInOrganigrammaResponse.UtenteInOrganigramma>();

                            response.Utenti.Add(
                                new GetUtentiInOrganigrammaResponse.UtenteInOrganigramma
                                {
                                    Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_UTENTE"))),
                                    UserName = reader.GetString(reader.GetOrdinal("USER_ID")),
                                    Matricola = (reader.IsDBNull(reader.GetOrdinal("MATRICOLA")) ? string.Empty : reader.GetString(reader.GetOrdinal("MATRICOLA"))),
                                    Denominazione = reader.GetString(reader.GetOrdinal("NOME_COMPLETO")),
                                    Responsabile = (reader.IsDBNull(reader.GetOrdinal("RESPONSABILE")) ? false : reader.GetString(reader.GetOrdinal("RESPONSABILE")) == "1"),
                                    MailAddress = (reader.IsDBNull(reader.GetOrdinal("EMAIL_ADDRESS")) ? string.Empty : reader.GetString(reader.GetOrdinal("EMAIL_ADDRESS")))
                                });
                        }
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti in organigramma che hanno impostata una determinata qualifica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaResponse GetUtentiConQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaResponse response = new GetUtentiConQualificaResponse();

            try
            {
                if (request.Recursive)
                {
                    // Reperimento ricorsivo degli utenti con qualifica
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_IN_UO_CON_QUALIFICA");
                    queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    queryDef.setParam("idUO", request.IdUO);
                    queryDef.setParam("codiceQualificaDiscriminante", request.CodiceQualificaUODiscriminante);
                    queryDef.setParam("codiceQualifica", request.CodiceQualifica);

                    string commandText = queryDef.getSQL();
                    _logger.InfoFormat("S_GET_UTENTI_IN_UO_CON_QUALIFICA: {0}", commandText);

                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            response.Utenti = new List<DocsPaVO.Qualifica.PeopleQualifica>();

                            while (reader.Read())
                            {
                                response.Utenti.Add(new DocsPaVO.Qualifica.PeopleQualifica
                                {
                                    Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_UTENTE"))),
                                    IdPeopleGroups = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_ASSOCIAZIONE_QUALIFICA"))),
                                    UserId = reader.GetString(reader.GetOrdinal("USER_ID")),
                                    Cognome = reader.GetString(reader.GetOrdinal("COGNOME")),
                                    Nome = reader.GetString(reader.GetOrdinal("NOME")),
                                    NomeCompleto = reader.GetString(reader.GetOrdinal("NOME_COMPLETO")),
                                    Matricola = (reader.IsDBNull(reader.GetOrdinal("MATRICOLA")) ? string.Empty : reader.GetString(reader.GetOrdinal("MATRICOLA")))
                                });
                            }
                        }
                    }
                }
                else
                {
                    // Reperimento degli utenti con qualifica di una determinata UO
                    response.Utenti = BusinessLogic.utenti.QualificheManager.GetPeopleConQualifica(
                            request.CodiceQualifica,
                            request.IdAmministrazione,
                            request.IdUO,
                            request.IdRuolo);
                }

                response.Utenti = response.Utenti.OrderBy(e => e.NomeCompleto).ToList();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per l'inserimento di una qualifica ad un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneQualificaResponse AddAssociazioneQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneQualificaRequest request)
        {
            VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneQualificaResponse response = new AddAssociazioneQualificaResponse();

            try
            {
                using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
                {
                    int idAmministrazione = (request.IdAmministrazione == 0 ? Convert.ToInt32(request.InfoUtente.idAmministrazione) : request.IdAmministrazione);

                    var qualificheInAmministrazione = BusinessLogic.utenti.QualificheManager.GetQualifiche(idAmministrazione);

                    foreach (var q in request.CodiciQualifiche)
                    {
                        var qualifica = qualificheInAmministrazione.FirstOrDefault(e => e.CODICE == q);

                        if (qualifica != null)
                        {
                            DocsPaVO.Validations.ValidationResultInfo validationResult = null;

                            if (request.IdGruppo == 0)
                            {
                                // Nella request non è stato passato alcun ruolo,
                                // pertanto la qualifica viene associata all'utente in tutti i suoi ruoli
                                var ruoliUtente = (DocsPaVO.utente.Ruolo[])
                                    BusinessLogic.Utenti.UserManager.getRuoliUtente(request.IdUtente.ToString()).ToArray(typeof(DocsPaVO.utente.Ruolo));

                                foreach (DocsPaVO.utente.Ruolo ruoloUtente in ruoliUtente)
                                {
                                    validationResult = BusinessLogic.utenti.QualificheManager.InsertPeopleGroupsQual(
                                        new DocsPaVO.Qualifica.PeopleGroupsQualifiche
                                        {
                                            ID_AMM = idAmministrazione,
                                            ID_QUALIFICA = qualifica.SYSTEM_ID,
                                            CODICE = qualifica.CODICE,
                                            DESCRIZIONE = qualifica.DESCRIZIONE,
                                            ID_UO = Convert.ToInt32(ruoloUtente.uo.systemId),
                                            ID_GRUPPO = Convert.ToInt32(ruoloUtente.idGruppo),
                                            ID_PEOPLE = request.IdUtente
                                        });
                                }
                            }
                            else
                            {
                                validationResult = BusinessLogic.utenti.QualificheManager.InsertPeopleGroupsQual(
                                    new DocsPaVO.Qualifica.PeopleGroupsQualifiche
                                    {
                                        ID_AMM = idAmministrazione,
                                        ID_QUALIFICA = qualifica.SYSTEM_ID,
                                        CODICE = qualifica.CODICE,
                                        DESCRIZIONE = qualifica.DESCRIZIONE,
                                        ID_UO = request.IdUO,
                                        ID_GRUPPO = request.IdGruppo,
                                        ID_PEOPLE = request.IdUtente
                                    });
                            }

                            if (validationResult.BrokenRules.Count > 0 && request.ThrowIfAssociazioneExist)
                            {
                                var brokenRules = (DocsPaVO.Validations.BrokenRule[])validationResult.BrokenRules.ToArray(typeof(DocsPaVO.Validations.BrokenRule));

                                throw new ApplicationException(brokenRules[0].Description);
                            }
                        }
                        else
                            throw new ApplicationException(string.Format("Qualifica con codice'{0}' non censita per l'amministrazione richiesta.", q));
                    }

                    transactionalContext.Complete();
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new AddAssociazioneQualificaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la rimozione dell'associazione di una qualifica ad un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneQualificaResponse RemoveAssociazioneQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneQualificaRequest request)
        {
            RemoveAssociazioneQualificaResponse response = new RemoveAssociazioneQualificaResponse();

            try
            {
                if (request.IdPeopleGroups > 0)
                {
                    var validationResult = BusinessLogic.utenti.QualificheManager.DeletePeopleGroups(request.IdPeopleGroups.ToString());

                    if (validationResult.BrokenRules.Count > 0)
                    {
                        var brokenRules = (DocsPaVO.Validations.BrokenRule[])validationResult.BrokenRules.ToArray(typeof(DocsPaVO.Validations.BrokenRule));

                        throw new ApplicationException(brokenRules[0].Description);
                    }
                }
                else
                {
                    var getUtentiConQualificaResponse = this.GetUtentiConQualifica(new GetUtentiConQualificaRequest
                                                                {
                                                                    InfoUtente = request.InfoUtente,
                                                                    TrowOnError = true,
                                                                    IdUO = request.IdUO,
                                                                    CodiceQualifica = request.CodiceQualifica,
                                                                    CodiceQualificaUODiscriminante = request.CodiceQualificaUODiscriminante,
                                                                    Recursive = true
                                                                });

                    if (getUtentiConQualificaResponse.Success)
                    {
                        // Rimozione di tutte le associazioni dell'utente richiesto
                        foreach (var ass in getUtentiConQualificaResponse.Utenti.Where(e => e.Id.ToString().Equals(request.IdUtente, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var validationResult = BusinessLogic.utenti.QualificheManager.DeletePeopleGroups(ass.IdPeopleGroups.ToString());

                            if (validationResult.BrokenRules.Count > 0)
                            {
                                var brokenRules = (DocsPaVO.Validations.BrokenRule[])validationResult.BrokenRules.ToArray(typeof(DocsPaVO.Validations.BrokenRule));

                                throw new ApplicationException(brokenRules[0].Description);
                            }
                        }
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new RemoveAssociazioneQualificaResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruoloInUo"></param>
        /// <param name="tipoRuoloInUo"></param>
        /// <param name="idUo"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        protected DocsPaVO.utente.Ruolo FindRuoloInUo(string ruoloInUo, string tipoRuolo, string idUo, string idAmministrazione)
        {
            DocsPaVO.utente.Ruolo retValue = null;

            if (!string.IsNullOrEmpty(ruoloInUo))
            {
                // Ricerca del ruolo in base all'Id o al Codice

                int idRuoloAsInt;
                if (Int32.TryParse(ruoloInUo, out idRuoloAsInt))
                {
                    // L'identificativo fornito è l'id del ruolo direttamente
                    retValue = BusinessLogic.Utenti.UserManager.getRuoloById(ruoloInUo);

                    if (retValue == null)
                        retValue = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(ruoloInUo);
                }
                else
                {
                    // L'identificativo fornito è il codice del ruolo
                    retValue = BusinessLogic.Utenti.UserManager.getRuoloByCodice(ruoloInUo);

                    if (retValue != null)
                        retValue = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(retValue.idGruppo);
                }

                if (retValue == null)
                    throw new ApplicationException(string.Format("Nessun ruolo trovato con l'identificativo '{0}'", ruoloInUo));
            }
            else if (!string.IsNullOrEmpty(tipoRuolo) && !string.IsNullOrEmpty(idUo))
            {
                // Reperimento della lista dei tipi ruolo censiti nell'amministrazione richiesta
                DocsPaVO.amministrazione.OrgTipoRuolo[] tipiRuolo = (DocsPaVO.amministrazione.OrgTipoRuolo[])
                    BusinessLogic.Amministrazione.OrganigrammaManager.GetListTipiRuolo(idAmministrazione).ToArray(typeof(DocsPaVO.amministrazione.OrgTipoRuolo));

                var tipoRuoloInUO = tipiRuolo.FirstOrDefault(e => e.IDTipoRuolo.Equals(tipoRuolo, StringComparison.InvariantCultureIgnoreCase) ||
                                                            e.Codice.Equals(tipoRuolo, StringComparison.InvariantCultureIgnoreCase));

                if (tipoRuoloInUO == null)
                    throw new ApplicationException(string.Format("Tipo Ruolo con identificativo '{0}' non trovato nell'UO '{1}' in amministrazione '{2}'", tipoRuolo, idUo, idAmministrazione));

                // Reperimento dei ruoli censiti in una UO
                DocsPaVO.amministrazione.OrgRuolo[] ruoliInUo = (DocsPaVO.amministrazione.OrgRuolo[])
                        BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUo(idUo, false).ToArray(typeof(DocsPaVO.amministrazione.OrgRuolo));

                var orgRuolo = ruoliInUo.FirstOrDefault(e => e.IDTipoRuolo == tipoRuoloInUO.IDTipoRuolo);

                if (orgRuolo == null)
                    throw new ApplicationException(string.Format("Ruolo di tipo '{0}' non trovato nell'UO '{1}' in amministrazione '{2}'", tipoRuolo, idUo, idAmministrazione));

                retValue = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(orgRuolo.IDGruppo);
            }
            else
            {
                // Parametri insufficienti
                throw new ApplicationException("Parametri insufficienti per identificare il ruolo in cui inserire l'utente");
            }

            return retValue;
        }

        /// <summary>
        /// Servizio per l'inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneUtenteRuoloResponse AddAssociazioneUtenteRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneUtenteRuoloRequest request)
        {
            AddAssociazioneUtenteRuoloResponse response = new AddAssociazioneUtenteRuoloResponse();

            try
            {
                string idRuolo = string.Empty;

                using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
                {
                    // Reperimento dell'id ruolo in uo
                    var ruolo = this.FindRuoloInUo(request.RuoloInUO, request.TipoRuoloInUO, request.IdUO, (string.IsNullOrEmpty(request.IdAmministrazione) ? request.InfoUtente.idAmministrazione : request.IdAmministrazione));

                    // Inserimento dell'utente nel ruolo
                    var esitoOperazione = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsUtenteInRuolo(request.InfoUtente, request.IdUtente, ruolo.idGruppo);

                    if (esitoOperazione.Codice != 0)
                        throw new ApplicationException(esitoOperazione.Descrizione);

                    if (request.AssociazioneQualifica != null)
                    {
                        request.AssociazioneQualifica.TrowOnError = request.TrowOnError;
                        request.AssociazioneQualifica.IdGruppo = Convert.ToInt32(ruolo.idGruppo);
                        request.AssociazioneQualifica.IdAmministrazione = Convert.ToInt32(ruolo.idAmministrazione);
                        request.AssociazioneQualifica.IdUO = Convert.ToInt32(ruolo.uo.systemId);
                        
                        var responseAssociazioneQualifica = this.AddAssociazioneQualifica(request.AssociazioneQualifica);

                        if (!responseAssociazioneQualifica.Success)
                            throw new ApplicationException(responseAssociazioneQualifica.Exception);
                    }

                    transactionalContext.Complete();
                }

                response.Success = true;
                response.IdRuolo = idRuolo;
            }
            catch (Exception ex)
            {
                response = new AddAssociazioneUtenteRuoloResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la rimozione di un utente da un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneUtenteRuoloResponse RemoveAssociazioneUtenteRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneUtenteRuoloRequest request)
        {
            RemoveAssociazioneUtenteRuoloResponse response = new RemoveAssociazioneUtenteRuoloResponse();

            try
            {
                // Reperimento dell'id ruolo in uo
                var ruolo = this.FindRuoloInUo(request.RuoloInUO, request.TipoRuoloInUO, request.IdUO, (string.IsNullOrEmpty(request.IdAmministrazione) ? request.InfoUtente.idAmministrazione : request.IdAmministrazione));

                // Inserimento dell'utente nel ruolo
                var esitoOperazione = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUtenteInRuolo(request.InfoUtente, request.IdUtente, ruolo.idGruppo);

                if (esitoOperazione.Codice != 0)
                    throw new ApplicationException(esitoOperazione.Descrizione);

                response.Success = true;
                response.IdRuolo = ruolo.systemId;
            }
            catch (Exception ex)
            {
                response = new RemoveAssociazioneUtenteRuoloResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei registri associati ad un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRegistriRuoloResponse GetRegistriRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRegistriRuoloRequest request)
        {
            GetRegistriRuoloResponse response = new GetRegistriRuoloResponse();

            try
            {
                if (!string.IsNullOrEmpty(request.IdRuolo))
                {
                    response.Registri = (DocsPaVO.utente.Registro[])
                        BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(request.IdRuolo).ToArray(typeof(DocsPaVO.utente.Registro));
                }
                else if (!string.IsNullOrEmpty(request.IdUO))
                {
                    // E' stato fornito solo l'identificativo di UO, pertanto vengono reperiti i registri del ruolo definito come responsabile
                    var orgRuolo = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetRuoloResponsabileUO(request.IdUO);

                    if (orgRuolo != null)
                    {
                        response.Registri = (DocsPaVO.utente.Registro[])
                            BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(orgRuolo.IDCorrGlobale).ToArray(typeof(DocsPaVO.utente.Registro));
                    }
                    else
                    {
                        throw new ApplicationException("Nessun ruolo responsabile definito per l'UO");
                    }
                }
                else
                {
                    throw new ApplicationException("Nessun parametro fornito per il reperimento dei registri");
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new GetRegistriRuoloResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento delle UO superiori
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUOSuperioriResponse GetUOSuperiori(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUOSuperioriRequest request)
        {
            GetUOSuperioriResponse response = new GetUOSuperioriResponse();

            try
            {
                string user = (string.IsNullOrEmpty(request.User) ? request.InfoUtente.idPeople : request.User);
                
                string customFilters = string.Empty;

                if (!string.IsNullOrEmpty(request.CodiceQualifica))
                {
                    customFilters =String.Format(" WHERE {0}.HasQualifica('{1}', ID) = 1", DocsPaDbManagement.Functions.Functions.GetDbUserSession(), request.CodiceQualifica);
                }

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_SUPERIORI_BY_UTENTE");
                queryDef.setParam("user", user);
                queryDef.setParam("customFilters", customFilters);

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UO_SUPERIORI: {0}", commandText);

                List<UOByQualifica> uos = new List<UOByQualifica>();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            uos.Add(new UOByQualifica
                            {
                                Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")).ToString()),
                                Codice = reader.GetString(reader.GetOrdinal("CODICE")),
                                Descrizione = reader.GetString(reader.GetOrdinal("DESCRIZIONE")),
                                Classifica = (reader.IsDBNull(reader.GetOrdinal("CLASSIFICA_UO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CLASSIFICA_UO"))),
                                Livello = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("LIVELLO")).ToString())
                            });
                        }
                    }

                    response.UOs = uos;
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new GetUOSuperioriResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="listUoByQualificaDiscriminante"></param>
        /// <param name="qualifica"></param>
        /// <param name="uo"></param>
        /// <param name="utenti"></param>
        protected void FetchUtentiResponsabili(
                            DocsPaVO.utente.InfoUtente infoUtente,
                            List<UOByQualifica> listUoByQualificaDiscriminante,
                            string qualifica,
                            int idUnitaOrganizzativa,
                            List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile> utenti)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_SUPERIORI");
                queryDef.setParam("idUo", idUnitaOrganizzativa.ToString());

                string commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UO_SUPERIORI: {0}", commandText);

                List<int> listIdUoSuperiori = new List<int>();

                int idUoRoot = 0;

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        int idUO = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")).ToString());

                        if (listUoByQualificaDiscriminante.Count(e => e.Id == idUO) > 0)
                        {
                            // Individuazione della radice richiesta
                            idUoRoot = idUO;
                            break;
                        }
                    }
                }
                
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_INFERIORI");
                queryDef.setParam("idUo", idUoRoot.ToString());

                commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UO_INFERIORI: {0}", commandText);

                List<int> listIdUoInferiori = new List<int>();

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        int idUO = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")).ToString());

                        if (idUO == idUoRoot)
                        {
                            // L'UO radice è considerata nal calcolo
                            listIdUoInferiori.Add(idUO);
                        }
                        else if (listUoByQualificaDiscriminante.Count(e => e.Id == idUO) == 0)
                        {
                            listIdUoInferiori.Add(idUO);
                        }
                        else
                        {
                            // Scarta dal calcolo le UO presenti nella lista discriminanti
                        }
                    }
                }

                // Per ciascuna UO, reperimento degli utenti responsabili
                Dictionary<int, int> utenteUoValuePairs = new Dictionary<int, int>();

                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_RESPONSABILI_AMMINISTRAZIONE");
                queryDef.setParam("idAmministrazione", infoUtente.idAmministrazione);

                commandText = queryDef.getSQL();
                _logger.InfoFormat("S_GET_UTENTI_RESPONSABILI_AMMINISTRAZIONE: {0}", commandText);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        int idUO = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_UO")).ToString());

                        if (listIdUoInferiori.Count(e => e == idUO) > 0)
                        {
                            string idUtente = reader.GetValue(reader.GetOrdinal("ID")).ToString();

                            // Se l'utente non è già stato inserito
                            bool mustAdd = true;

                            if (!string.IsNullOrEmpty(qualifica))
                            {
                                // Verifica se l'utente dispone della qualifica richiesta
                                var qualificheAssegnate = BusinessLogic.utenti.QualificheManager.GetPeopleGroupsQualificheByIdPeople(idUtente);

                                mustAdd = qualificheAssegnate.Count(e => e.CODICE.Equals(qualifica, StringComparison.InvariantCultureIgnoreCase)) > 0;
                            }

                            if (mustAdd)
                            {
                                utenti.Add
                                    (
                                        new VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile
                                        {
                                            Id = idUtente,
                                            Codice = reader.GetString(reader.GetOrdinal("USER_ID")),
                                            Matricola = reader.GetString(reader.GetOrdinal("MATRICOLA")),
                                            Nome = reader.GetString(reader.GetOrdinal("NOME")),
                                            Cognome = reader.GetString(reader.GetOrdinal("COGNOME")),
                                            UO = reader.GetString(reader.GetOrdinal("DESCRIZIONE_UO"))
                                        }
                                    );
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUnitaOrganizzativa"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="recursive"></param>
        /// <param name="fetchRuoliIfNotRecursive"></param>
        /// <param name="getQualificheUtentiInRuolo"></param>
        /// <returns></returns>
        protected VtDocs.BusinessServices.Entities.Administration.Organigramma.UO BuildOrganigramma(
                                                                                            int idUnitaOrganizzativa,
                                                                                            string idAmministrazione,
                                                                                            bool recursive,
                                                                                            bool fetchRuoliIfNotRecursive,
                                                                                            bool getQualificheUtentiInRuolo)
        {
            DocsPaVO.amministrazione.OrgUO orgUO = null;

            if (idUnitaOrganizzativa > 0)
            {
                orgUO = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetDatiUOCorrente(idUnitaOrganizzativa.ToString());
            }
            else
            {
                orgUO = (DocsPaVO.amministrazione.OrgUO)BusinessLogic.Amministrazione.OrganigrammaManager.GetListUo("0", "0", idAmministrazione)[0];
            }

            VtDocs.BusinessServices.Entities.Administration.Organigramma.UO retValue = new VtDocs.BusinessServices.Entities.Administration.Organigramma.UO
            {
                DatiUO = orgUO,
                Ruoli = new List<DocsPaVO.amministrazione.OrgRuolo>((DocsPaVO.amministrazione.OrgRuolo[])BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUo(orgUO.IDCorrGlobale, getQualificheUtentiInRuolo).ToArray(typeof(DocsPaVO.amministrazione.OrgRuolo)))
            };

            // Reperimento UO sottoposte
            ArrayList genericListUOSottoposte = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUo(
                                                                orgUO.IDCorrGlobale,
                                                                (Convert.ToInt32(orgUO.Livello) + 1).ToString(),
                                                                idAmministrazione);
            if (genericListUOSottoposte != null)
            {
                retValue.UOSottoposte = new List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UO>();

                foreach (DocsPaVO.amministrazione.OrgUO orgUOSottoposta in genericListUOSottoposte)
                {
                    VtDocs.BusinessServices.Entities.Administration.Organigramma.UO uoSottoposta = new VtDocs.BusinessServices.Entities.Administration.Organigramma.UO
                    {
                        DatiUO = orgUOSottoposta
                    };

                    retValue.UOSottoposte.Add(uoSottoposta);

                    if (recursive)
                    {
                        uoSottoposta.UOSottoposte.Add(this.BuildOrganigramma(Convert.ToInt32(orgUOSottoposta.IDCorrGlobale),
                                                                orgUOSottoposta.IDAmministrazione,
                                                                recursive,
                                                                fetchRuoliIfNotRecursive,
                                                                getQualificheUtentiInRuolo));
                    }
                    else
                    {
                        if (fetchRuoliIfNotRecursive)
                        {
                            // Caricamento ruoli e utenti (se richiesto e se non ricorsivo)
                            uoSottoposta.Ruoli = new List<DocsPaVO.amministrazione.OrgRuolo>
                                ((DocsPaVO.amministrazione.OrgRuolo[])
                                    BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUo(uoSottoposta.DatiUO.IDCorrGlobale, getQualificheUtentiInRuolo).ToArray(typeof(DocsPaVO.amministrazione.OrgRuolo)));
                        }
                    }
                }
            }

            return retValue;
        }
    }
}
