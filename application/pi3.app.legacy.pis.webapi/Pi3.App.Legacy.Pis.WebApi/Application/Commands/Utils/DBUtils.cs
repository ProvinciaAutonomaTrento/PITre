// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper.Execution;
using DocsPaVO;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X9;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocuments;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale2;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocsPaVO.Import.Pregressi;
using System.DirectoryServices.ActiveDirectory;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorio;
using DocsPaVO.ricerche;
using DocsPaVO.filtri;
using DocumentFormat.OpenXml.EMMA;
using System.Reflection;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public class DBUtils
    {
        #region Utenti e ruoli
        public static Utente getUtenteByCodice(string username, string codeAdm, IPi3DbContext dbContext)
        {
            var pE = (from a in dbContext.PeopleEntities
                      join b in dbContext.AmministraEntities
                      on new { IdAmm = a.ID_AMM ?? 0, VarCodice = codeAdm.ToLower() } equals new { IdAmm = b.SYSTEM_ID, VarCodice = b.VAR_CODICE_AMM.ToLower() }
                      where a.USER_ID.ToLower() == username.ToLower()
                      select a).FirstOrDefault();

            Utente utente = null;
            if (pE != null && pE.SYSTEM_ID > 0)
            {
                utente = new Utente()
                {
                    idAmministrazione = pE.ID_AMM.ToString(),
                    userId = pE.USER_ID,
                    systemId = pE.SYSTEM_ID.ToString(),
                    descrizione = pE.FULL_NAME,
                    telefono = pE.VAR_TELEFONO,
                    email = pE.EMAIL_ADDRESS,
                    notifica = pE.CHA_NOTIFICA,
                    cognome = pE.VAR_COGNOME,
                    nome = pE.VAR_NOME,
                    matricola = pE.MATRICOLA,
                    idPeople = pE.SYSTEM_ID.ToString()

                };
            }
            return utente;
        }

        public static Utente getUtente(string username, string idAmm, IPi3DbContext dbContext)
        {
            Utente utente = null;
            var ute = (from a in dbContext.PeopleEntities
                       where a.USER_ID.ToUpper() == username.ToUpper() && a.ID_AMM == idAmm.AsLong()
                       select a).FirstOrDefault();

            if (ute != null && ute.SYSTEM_ID > 0)
            {
                utente = new Utente()
                {
                    idAmministrazione = ute.ID_AMM.ToString(),
                    userId = ute.USER_ID,
                    systemId = ute.SYSTEM_ID.ToString(),
                    descrizione = ute.FULL_NAME,
                    telefono = ute.VAR_TELEFONO,
                    email = ute.EMAIL_ADDRESS,
                    notifica = ute.CHA_NOTIFICA,
                    cognome = ute.VAR_COGNOME,
                    nome = ute.VAR_NOME,
                    matricola = ute.MATRICOLA,
                    idPeople = ute.SYSTEM_ID.ToString()
                };
            }
            return utente;
        }

        public static Utente getUtenteById(string idPeople, IPi3DbContext dbContext)
        {
            Utente utente = null;
            var ute = (from a in dbContext.PeopleEntities
                       where a.SYSTEM_ID == idPeople.AsLong()
                       select a).FirstOrDefault();

            if (ute != null && ute.SYSTEM_ID > 0)
            {
                utente = new Utente()
                {
                    idAmministrazione = ute.ID_AMM.ToString(),
                    userId = ute.USER_ID,
                    systemId = ute.SYSTEM_ID.ToString(),
                    descrizione = ute.FULL_NAME,
                    telefono = ute.VAR_TELEFONO,
                    email = ute.EMAIL_ADDRESS,
                    notifica = ute.CHA_NOTIFICA,
                    cognome = ute.VAR_COGNOME,
                    nome = ute.VAR_NOME,
                    matricola = ute.MATRICOLA,
                    idPeople = ute.SYSTEM_ID.ToString()
                };
            }
            return utente;
        }

        public static Ruolo getRuoloByCodice(string codRuolo, IPi3DbContext dbContext)
        {
            Ruolo ruolo = null;

            var re = (from a in dbContext.CorrGlobaliEntities
                      where a.VAR_COD_RUBRICA.ToUpper() == codRuolo.ToUpper() && a.DTA_FINE == null && a.CHA_TIPO_IE == "I" && a.CHA_TIPO_URP == "R"
                      select a).FirstOrDefault();

            if (re != null)
            {
                ruolo = new Ruolo()
                {
                    systemId = re.SYSTEM_ID.ToString(),
                    idRegistro = re.ID_REGISTRO.ToString(),
                    idAmministrazione = re.ID_AMM.ToString(),
                    codiceRubrica = re.VAR_COD_RUBRICA,
                    descrizione = re.VAR_DESC_CORR,
                    idOld = re.ID_OLD.ToString(),
                    dta_fine = re.DTA_FINE.ToString(),
                    livello = re.NUM_LIVELLO.ToString(),
                    codice = re.VAR_CODICE,
                    idGruppo = re.ID_GRUPPO.ToString(),
                    //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                    tipoIE = re.CHA_TIPO_IE,
                    tipoCorrispondente = re.CHA_TIPO_URP,
                    codiceAOO = re.VAR_CODICE_AOO,
                    codiceAmm = re.VAR_CODICE_AMM,
                    codiceIstat = re.VAR_CODICE_ISTAT,
                    email = re.VAR_EMAIL,
                    uo = new UnitaOrganizzativa() { systemId = re.ID_UO.ToString() }

                };
            }

            return ruolo;
        }

        public static Ruolo getRuoloById(string idCorrGlobali, IPi3DbContext dbContext)
        {
            Ruolo ruolo = null;

            var re = (from a in dbContext.CorrGlobaliEntities
                      where a.SYSTEM_ID == idCorrGlobali.AsLong() &&
                        a.DTA_FINE == null &&
                        a.CHA_TIPO_IE == "I" &&
                        a.CHA_TIPO_URP == "R"
                      select a)
                      .FirstOrDefault();

            if (re != null)
            {
                ruolo = new Ruolo()
                {
                    systemId = re.SYSTEM_ID.ToString(),
                    idRegistro = re.ID_REGISTRO.ToString(),
                    idAmministrazione = re.ID_AMM.ToString(),
                    codiceRubrica = re.VAR_COD_RUBRICA,
                    descrizione = re.VAR_DESC_CORR,
                    idOld = re.ID_OLD.ToString(),
                    dta_fine = re.DTA_FINE.ToString(),
                    livello = re.NUM_LIVELLO.ToString(),
                    codice = re.VAR_CODICE,
                    idGruppo = re.ID_GRUPPO.ToString(),
                    //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                    tipoIE = re.CHA_TIPO_IE,
                    tipoCorrispondente = re.CHA_TIPO_URP,
                    codiceAOO = re.VAR_CODICE_AOO,
                    codiceAmm = re.VAR_CODICE_AMM,
                    codiceIstat = re.VAR_CODICE_ISTAT,
                    email = re.VAR_EMAIL,
                    uo = new UnitaOrganizzativa() { systemId = re.ID_UO.ToString() }

                };

                List<Funzione> funzioniList = new List<Funzione>();
                var funzioniEntity = dbContext.FunzioneEntities.AsNoTracking()
                    .Join(dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                    .Join(dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                    .Where(j => j.r.ID_RUOLO_IN_UO == re.SYSTEM_ID)
                    .Select(j => new
                    {
                        j.f.SYSTEM_ID,
                        j.f.COD_FUNZIONE,
                        j.f.VAR_DESC_FUNZIONE,
                        j.f.ID_TIPO_FUNZIONE,
                        j.t.VAR_COD_TIPO,
                        j.t.VAR_DESC_TIPO_FUN
                    })
                    .ToList();
                foreach (var f in funzioniEntity)
                {
                    funzioniList.Add(new Funzione()
                    {
                        systemId = f.SYSTEM_ID.ToString(),
                        descrizione = f.VAR_DESC_FUNZIONE,
                        codice = f.COD_FUNZIONE,
                        idTipoFunzione = f.ID_TIPO_FUNZIONE.ToString(),
                        codTipoFunzione = f.VAR_COD_TIPO,
                        descTipoFunzione = f.VAR_DESC_TIPO_FUN
                    });
                }
                ruolo.funzioni = funzioniList.ToArray();
            }


            return ruolo;
        }

        public static Ruolo getRuoloByIdGruppo(string idGruppo, IPi3DbContext dbContext)
        {
            Ruolo ruolo = null;

            var re = (from a in dbContext.CorrGlobaliEntities
                      where a.ID_GRUPPO == idGruppo.AsLong() &&
                            a.DTA_FINE == null &&
                            a.CHA_TIPO_IE == "I" &&
                            a.CHA_TIPO_URP == "R"
                      select a).FirstOrDefault();

            if (re != null)
            {
                ruolo = new Ruolo()
                {
                    systemId = re.SYSTEM_ID.ToString(),
                    idRegistro = re.ID_REGISTRO.ToString() is not null ? re.ID_REGISTRO.ToString() : "",
                    idAmministrazione = re.ID_AMM.ToString(),
                    codiceRubrica = re.VAR_COD_RUBRICA,
                    descrizione = re.VAR_DESC_CORR,
                    idOld = re.ID_OLD.ToString(),
                    dta_fine = re.DTA_FINE.ToString(),
                    livello = re.NUM_LIVELLO.ToString(),
                    codice = re.VAR_CODICE,
                    idGruppo = re.ID_GRUPPO.ToString(),
                    //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                    tipoIE = re.CHA_TIPO_IE,
                    tipoCorrispondente = re.CHA_TIPO_URP,
                    codiceAOO = re.VAR_CODICE_AOO,
                    codiceAmm = re.VAR_CODICE_AMM,
                    codiceIstat = re.VAR_CODICE_ISTAT,
                    email = re.VAR_EMAIL

                };
            }

            return ruolo;
        }

        public static List<UserMinimalInfo> GetUsersInRoleMinimalInfo(string idGruppo, IPi3DbContext dbContext)
        {
            List<UserMinimalInfo> retval = null;

            var query = from p in dbContext.PeopleEntities
                        where dbContext.PeopleGroupEntities
                            .Where(pg => pg.GROUPS_SYSTEM_ID == idGruppo.AsLong() && pg.DTA_FINE == null)
                            .Select(pg => pg.PEOPLE_SYSTEM_ID)
                            .Contains(p.SYSTEM_ID)
                        select p;

            if (query.Any())
            {
                retval = new List<UserMinimalInfo>();
                foreach (var p in query)
                {
                    retval.Add(new UserMinimalInfo() { Description = p.FULL_NAME, SystemId = p.SYSTEM_ID.ToString() });
                }
            }

            return retval;
        }

        public static Ruolo getRuoloPreferito(string idPeople, IPi3DbContext dbContext)
        {
            Ruolo ruolo = null;

            var res = (from a in dbContext.CorrGlobaliEntities
                       join b in dbContext.PeopleGroupEntities
                       on new { IdGruppo = a.ID_GRUPPO, PeopleSystemId = idPeople.AsLong() } equals new { IdGruppo = b.GROUPS_SYSTEM_ID, PeopleSystemId = b.PEOPLE_SYSTEM_ID ?? 0 }
                       where b.DTA_FINE == null
                       orderby b.CHA_PREFERITO descending
                       select new { a, b });

            // il nulls last non funziona
            CorrGlobaliEntity re = new CorrGlobaliEntity();
            if (res != null && res.Any())
            {
                re = res.Where(x => x.b.CHA_PREFERITO == "1").Select(x => x.a).FirstOrDefault();
                if (re == null || re.SYSTEM_ID < 1) re = res.Select(x => x.a).FirstOrDefault();
            }

            if (re != null)
            {
                ruolo = new Ruolo()
                {
                    systemId = re.SYSTEM_ID.ToString(),
                    idRegistro = re.ID_REGISTRO.ToString(),
                    idAmministrazione = re.ID_AMM.ToString(),
                    codiceRubrica = re.VAR_COD_RUBRICA,
                    descrizione = re.VAR_DESC_CORR,
                    idOld = re.ID_OLD.ToString(),
                    dta_fine = re.DTA_FINE.ToString(),
                    livello = re.NUM_LIVELLO.ToString(),
                    codice = re.VAR_CODICE,
                    idGruppo = re.ID_GRUPPO.ToString(),
                    //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                    tipoIE = re.CHA_TIPO_IE,
                    tipoCorrispondente = re.CHA_TIPO_URP,
                    codiceAOO = re.VAR_CODICE_AOO,
                    codiceAmm = re.VAR_CODICE_AMM,
                    codiceIstat = re.VAR_CODICE_ISTAT,
                    email = re.VAR_EMAIL,
                    uo = new UnitaOrganizzativa() { systemId = re.ID_UO.ToString() }
                };
            }

            return ruolo;
        }

        public static ArrayList getRuoliUtente(string idPeople, IPi3DbContext dbContext)
        {
            ArrayList retval = new ArrayList();

            var res = (from a in dbContext.CorrGlobaliEntities
                       join b in dbContext.PeopleGroupEntities
                       on new { IdGruppo = a.ID_GRUPPO, PeopleSystemId = idPeople.AsLong() } equals new { IdGruppo = b.GROUPS_SYSTEM_ID, PeopleSystemId = b.PEOPLE_SYSTEM_ID ?? 0 }
                       where b.DTA_FINE == null
                       orderby b.CHA_PREFERITO descending
                       select new { a, b });

            // il nulls last non funziona
            CorrGlobaliEntity re = new CorrGlobaliEntity();
            Ruolo ruolo = null;
            if (res != null && res.Any())
            {
                foreach (var c in res)
                {
                    ruolo = new Ruolo()
                    {
                        systemId = c.a.SYSTEM_ID.ToString(),
                        idRegistro = c.a.ID_REGISTRO.ToString(),
                        idAmministrazione = c.a.ID_AMM.ToString(),
                        codiceRubrica = c.a.VAR_COD_RUBRICA,
                        descrizione = c.a.VAR_DESC_CORR,
                        idOld = c.a.ID_OLD.ToString(),
                        dta_fine = c.a.DTA_FINE.ToString(),
                        livello = c.a.NUM_LIVELLO.ToString(),
                        codice = c.a.VAR_CODICE,
                        idGruppo = c.a.ID_GRUPPO.ToString(),
                        //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                        tipoIE = c.a.CHA_TIPO_IE,
                        tipoCorrispondente = c.a.CHA_TIPO_URP,
                        codiceAOO = c.a.VAR_CODICE_AOO,
                        codiceAmm = c.a.VAR_CODICE_AMM,
                        codiceIstat = c.a.VAR_CODICE_ISTAT,
                        email = c.a.VAR_EMAIL
                    };
                    if (!string.IsNullOrWhiteSpace(c.b.CHA_PREFERITO) && c.b.CHA_PREFERITO == "1") retval.Insert(0, ruolo);
                    else retval.Add(ruolo);

                }


            }

            return retval;
        }

        public static ArrayList getRuoliUtenteForEnabledActions(string idPeople, string tipofunzione, string idAmministrazione, IPi3DbContext dbContext)
        {
            ArrayList retval = new ArrayList();
            var query = from a in dbContext.PeopleGroupEntities
                        join b in dbContext.CorrGlobaliEntities on a.GROUPS_SYSTEM_ID equals b.ID_GRUPPO
                        join c in dbContext.TipoRuoloEntities on b.ID_TIPO_RUOLO equals c.SYSTEM_ID
                        join fr in dbContext.TipoFRuoloEntities on b.SYSTEM_ID equals fr.ID_RUOLO_IN_UO
                        join d in dbContext.TipoFunzioneEntities on fr.ID_TIPO_FUNZ equals d.SYSTEM_ID
                        join e in dbContext.PeopleEntities on a.PEOPLE_SYSTEM_ID equals e.SYSTEM_ID
                        where b.CHA_TIPO_IE == "I" && b.CHA_TIPO_URP == "R" &&
                              c.SYSTEM_ID == b.ID_TIPO_RUOLO && d.VAR_COD_TIPO.ToUpper() == tipofunzione.ToUpper() &&
                              a.PEOPLE_SYSTEM_ID == idPeople.AsLong() &&
                              d.ID_AMM == idAmministrazione.AsLong()
                        select b;
            if (query != null && query.Any())
            {
                foreach (var role in query)
                {
                    retval.Add(new Ruolo()
                    {
                        systemId = role.SYSTEM_ID.ToString(),
                        idRegistro = role.ID_REGISTRO.ToString(),
                        idAmministrazione = role.ID_AMM.ToString(),
                        codiceRubrica = role.VAR_COD_RUBRICA,
                        descrizione = role.VAR_DESC_CORR,
                        idOld = role.ID_OLD.ToString(),
                        dta_fine = role.DTA_FINE.ToString(),
                        livello = role.NUM_LIVELLO.ToString(),
                        codice = role.VAR_CODICE,
                        idGruppo = role.ID_GRUPPO.ToString(),
                        //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                        tipoIE = role.CHA_TIPO_IE,
                        tipoCorrispondente = role.CHA_TIPO_URP,
                        codiceAOO = role.VAR_CODICE_AOO,
                        codiceAmm = role.VAR_CODICE_AMM,
                        codiceIstat = role.VAR_CODICE_ISTAT,
                        email = role.VAR_EMAIL,
                        uo = new UnitaOrganizzativa() { systemId = role.ID_UO.ToString() }
                    });
                }
            }

            return retval;
        }

        public static TipoRuolo getTipoRuoloByCodice(string codice, string idAmministra, IPi3DbContext dbContext)
        {
            TipoRuolo retval = null;

            var res = (from a in dbContext.TipoRuoloEntities
                       where a.ID_AMM == idAmministra.AsLong() && a.VAR_CODICE.ToUpper() == codice.ToUpper()
                       select a).FirstOrDefault();
            if (res != null)
            {
                retval = new TipoRuolo()
                {
                    systemId = res.SYSTEM_ID.ToString(),
                    codice = res.VAR_CODICE,
                    descrizione = res.VAR_DESC_RUOLO,
                    id_Amm = res.ID_AMM.ToString(),
                    livello = res.NUM_LIVELLO.ToString()
                };
            }

            return retval;

        }
        #endregion

        #region Documenti

        public static string getDocIdFromProtocolSignature(string signature, IPi3DbContext dbContext)
        {
            string idDoc = "";
            var query = (from a in dbContext.ProfileEntities
                         where a.VAR_SEGNATURA.ToUpper() == signature.ToUpper()
                         select a.SYSTEM_ID).FirstOrDefault().ToString();

            if (!string.IsNullOrWhiteSpace(query)) idDoc = query;


            return idDoc;
        }

        public static StateOfDiagram getStatoDiagrammaDoc(string idDocument, IPi3DbContext dbContext)
        {
            StateOfDiagram retval = null;
            var query = (from adiag in dbContext.DiagrammiEntities
                         join bstat in dbContext.StatoEntities on adiag.ID_STATO equals bstat.SYSTEM_ID
                         where adiag.DOC_NUMBER == idDocument.AsLong()
                         select bstat).FirstOrDefault();

            if (query != null && query.SYSTEM_ID > 0)
            {
                retval = new StateOfDiagram()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.VAR_DESCRIZIONE,
                    DiagramId = query.ID_DIAGRAMMA.ToString(),
                    InitialState = query.STATO_INIZIALE == 1,
                    FinaleState = query.STATO_FINALE == 1
                };
            }
            return retval;
        }

        public static long maxFileSizePermitted(IPi3DbContext dbContext)
        {
            long chiaveMaxFileSizePermitted = 0;
            Int64.TryParse(
                dbContext.ChiaviConfigurazioneEntities.Where(c => c.VAR_CODICE == "BE_PIS_MAXFILESIZE_MB" && c.ID_AMM == 0).Select(c => c.VAR_VALORE).FirstOrDefault(), out chiaveMaxFileSizePermitted);
            return chiaveMaxFileSizePermitted;
        }

        public static List<Documents.File> GetAllegatiDocumento(string idDocument, string codeApplication, IPi3DbContext dbContext)
        {
            List<Documents.File> retval = new List<Documents.File>();
            try
            {
                var allegati = from a in dbContext.ProfileEntities
                               join b in dbContext.VersionEntities on a.SYSTEM_ID equals b.DOCNUMBER
                               join d in dbContext.ComponentEntities on b.VERSION_ID equals d.VERSION_ID
                               from c in dbContext.AlboDocPubbEntities.Where(x => x.DOCNUMBER == a.SYSTEM_ID && x.ID_DOC_PRINCIPALE == idDocument.AsLong()).DefaultIfEmpty()
                               where a.ID_DOCUMENTO_PRINCIPALE == idDocument.AsLong() && a.CHA_IN_CESTINO == null
                               && b.VERSION_ID == (from v in dbContext.VersionEntities where v.DOCNUMBER == a.SYSTEM_ID select v.VERSION_ID).Max()
                               select new
                               {
                                   idDoc = a.SYSTEM_ID,
                                   idVersione = b.VERSION_ID,
                                   tipoAllegato = b.CHA_ALLEGATI_ESTERNO,
                                   pubbAlbo = c.DA_PUBB,
                                   filename = d.VAR_NOMEORIGINALE,
                                   nomeAllegato = a.VAR_PROF_OGGETTO
                               };
                bool soloAllegatiUtente = false;
                if (!string.IsNullOrEmpty(codeApplication))
                {
                    var integrazioneAlbo = from a in dbContext.AlboPubbVerticaliEntities
                                           where a.CODEAPP_VERTICALE.ToUpper() == codeApplication.ToUpper()
                                           select a;
                    if (codeApplication.ToUpper() == "ALBO_TELEMATICO" || codeApplication.ToUpper() == "GDOC") soloAllegatiUtente = true;
                    if (!soloAllegatiUtente && integrazioneAlbo != null && integrazioneAlbo.Any()) soloAllegatiUtente = true;
                }
                foreach (var allegato in allegati)
                {
                    if (soloAllegatiUtente && (allegato.tipoAllegato != "0" || allegato.pubbAlbo != "S")) continue;

                    retval.Add(new Documents.File()
                    {
                        Description = allegato.nomeAllegato,
                        Name = allegato.filename,
                        Id = allegato.idDoc.ToString()

                    });

                }

            }
            catch (Exception ex)
            {
                retval = null;
            }

            return retval;
        }

        public static async Task<long?> GetIdDocPrincipale(string idDoc, IPi3DbContext dbContext)
        {
            long? retval = null;
            try
            {
                retval = await dbContext.ProfileEntities
                    .Where(p => p.SYSTEM_ID == idDoc.AsLong())
                        .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
            return retval;

        }

        public static Template getTemplateFromDocumentId(string idDocument, IPi3DbContext dbContext)
        {
            Template retval = null;

            var tipologia = (from a in dbContext.TipoAttoEntities
                             join b in dbContext.ProfileEntities on a.SYSTEM_ID equals b.ID_TIPO_ATTO
                             where b.SYSTEM_ID.Equals(idDocument.AsLong())
                             select new { idTipoAtto = a.SYSTEM_ID, descTipoAtto = a.VAR_DESC_ATTO }).FirstOrDefault();
            if (tipologia != null && tipologia.idTipoAtto > 0)
            {
                retval = new Template()
                {
                    Id = tipologia.idTipoAtto.ToString(),
                    Name = tipologia.descTipoAtto,
                    Type = "D"
                };


                var query = from a in dbContext.AssociazioneTemplatesEntities
                            join c in dbContext.OggettiCustomEntities on a.ID_OGGETTO equals c.SYSTEM_ID
                            join e in dbContext.TipoOggettoEntities on c.ID_TIPO_OGGETTO equals e.SYSTEM_ID
                            //from d in dbContext.AROggCustomFascEntityEntities.Where(x => x.ID_OGGETTO_CUSTOM == c.SYSTEM_ID && x.ID_TEMPLATE == a.ID_TEMPLATE && x.ID_RUOLO == Int64.Parse(idGruppo)).DefaultIfEmpty()
                            where a.DOC_NUMBER.ToString() == idDocument
                            select new
                            {
                                idOggetto = c.SYSTEM_ID,
                                descOggetto = c.DESCRIZIONE,
                                obbligatorio = c.CAMPO_OBBLIGATORIO,
                                valore = a.VALORE_OGGETTO_DB,
                                tipooggetto = e.DESCRIZIONE,
                                tipoContatore = c.CHA_TIPO_TAR,
                                idRegistro = a.ID_AOO_RF,
                                repertorio = c.REPERTORIO,
                                dataInserimento = a.DTA_INS
                                // ,scrittura = d.INS_MOD,
                                // lettura = d.VIS
                            };

                if (query != null && query.Any())
                {
                    List<Field> campi = new List<Field>();
                    Field campo = null;
                    Dictionary<string, List<string>> selezioneMultipla = new Dictionary<string, List<string>>();

                    foreach (var ogg in query)
                    {
                        campo = new Field();
                        campo.Id = ogg.idOggetto.ToString();
                        campo.Name = ogg.descOggetto;
                        campo.Required = ogg.obbligatorio == "SI";
                        //if (ogg.scrittura == 1) campo.Rights = "INSERT_AND_MODIFY";
                        //else if (ogg.lettura == 1) campo.Rights = "VIEW";
                        //else campo.Rights = "NONE";
                        switch (ogg.tipooggetto)
                        {
                            case "Corrispondente":
                                campo.Value = ogg.valore;
                                campo.Type = "Correspondent";
                                break;
                            case "CampoDiTesto":
                                campo.Value = ogg.valore;
                                campo.Type = "TextField";
                                break;
                            case "SelezioneEsclusiva":
                                campo.Value = ogg.valore;
                                campo.Type = "ExclusiveSelection";
                                break;
                            case "MenuATendina":
                                campo.Value = ogg.valore;
                                campo.Type = "DropDown";
                                break;
                            case "Data":
                                campo.Value = ogg.valore;
                                campo.Type = "Date";
                                break;
                            case "Contatore":
                                campo.Value = ogg.valore;
                                campo.Type = "Counter";
                                if (ogg.tipoContatore.Equals("A") || ogg.tipoContatore.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = getRegistro(ogg.idRegistro.ToString(), dbContext);
                                    if (reg != null)
                                    {
                                        campo.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                //fieldTemp.CounterToTrigger ???
                                //if (ogg.idRegistro != null && ogg.idRegistro > 0) campo.CodeRegisterOrRF = (from a in dbContext.RegistroEntities where a.SYSTEM_ID == ogg.idRegistro select a.VAR_CODICE).FirstOrDefault();
                                if (ogg.repertorio == 1)
                                {
                                    var otherInfo = new List<string>();
                                    var segnaturaRepertorio = (from a in dbContext.AssociazioneTemplatesEntities
                                                               where a.DOC_NUMBER == idDocument
                                                               && a.VAR_SEGNATURA != null
                                                               select a.VAR_SEGNATURA
                                                                ).FirstOrDefault();
                                    otherInfo.Add(string.Format("{0}: {1}",
                                    "Segnatura di repertorio",
                                    segnaturaRepertorio
                                    ));
                                    otherInfo.Add(
                                        string.Format("{0}: {1}",
                                        "Data di repertorio",
                                        ogg.dataInserimento
                                    ));

                                    selezioneMultipla.Add(ogg.idOggetto.ToString(), otherInfo);
                                }
                                break;
                            case "ContatoreSottocontatore":
                                campo.Value = ogg.valore;
                                campo.Type = "SubCounter";
                                if (ogg.idRegistro != null && ogg.idRegistro > 0) campo.CodeRegisterOrRF = (from a in dbContext.RegistroEntities where a.SYSTEM_ID == ogg.idRegistro select a.VAR_CODICE).FirstOrDefault();
                                break;
                            case "CasellaDiSelezione":
                                campo.Type = "MultipleChoise";
                                // I valori multipli vengono associati come righe della associazione templates
                                if (selezioneMultipla.ContainsKey(ogg.idOggetto.ToString()))
                                {
                                    selezioneMultipla[ogg.idOggetto.ToString()].Add(ogg.valore);
                                }
                                else
                                {
                                    selezioneMultipla.Add(ogg.idOggetto.ToString(), new List<string>());
                                    selezioneMultipla[ogg.idOggetto.ToString()].Add(ogg.valore);
                                }

                                break;
                            case "Link":
                                campo.Value = ogg.valore;
                                campo.Type = "Link";
                                break;
                            case "Separatore":
                                campo.Type = "Divider";
                                break;
                        }
                        if (!((campi.Where(x => x.Id == campo.Id).Select(x => x)).Any()))
                            campi.Add(campo);
                    }

                    if (selezioneMultipla.Any())
                    {
                        foreach (var c in campi)
                        {
                            if (selezioneMultipla.ContainsKey(c.Id))
                            {
                                c.MultipleChoice = selezioneMultipla[c.Id].ToArray();
                            }
                        }
                    }

                    retval.Fields = campi.ToArray();
                }

                retval.StateDiagram = getStateDiagramFromDocumentId(idDocument, dbContext);
            }

            return retval;
        }

        public static StateDiagram getStateDiagramFromDocumentId(string idDocument, IPi3DbContext dbContext)
        {
            StateDiagram retval = null;

            var query = (from a in dbContext.DiagrammiEntities
                         join b in dbContext.DiagrammiStatoEntities on a.ID_DIAGRAMMA equals b.SYSTEM_ID
                         join c in dbContext.StatoEntities on a.ID_STATO equals c.SYSTEM_ID
                         where a.DOC_NUMBER.ToString() == idDocument
                         select new
                         {
                             idDiagramma = b.SYSTEM_ID,
                             descDiagramma = b.VAR_DESCRIZIONE,
                             idStato = c.SYSTEM_ID,
                             descStato = c.VAR_DESCRIZIONE,
                             statoIniziale = c.STATO_INIZIALE == 1,
                             statoFinale = c.STATO_FINALE == 1
                         }).FirstOrDefault();

            if (query != null && query.idDiagramma > 0)
            {
                retval = new StateDiagram()
                {
                    Id = query.idDiagramma.ToString(),
                    Description = query.descDiagramma,
                    StateOfDiagram = new StateOfDiagram[1]
                };
                retval.StateOfDiagram[0] = new StateOfDiagram()
                {
                    Id = query.idStato.ToString(),
                    Description = query.descStato,
                    DiagramId = query.idDiagramma.ToString(),
                    InitialState = query.statoIniziale,
                    FinaleState = query.statoFinale
                };
            }

            return retval;

        }

        public static async Task<LinkedDocument> getParentDocInfoFromDocId(string idDocument, IPi3DbContext dbContext)
        {
            LinkedDocument retval = null;
            try
            {
                var query = (from a in dbContext.ProfileEntities
                             join b in dbContext.ProfileEntities on a.SYSTEM_ID equals b.ID_PARENT
                             where b.SYSTEM_ID == idDocument.AsLong()
                             select new
                             {
                                 ParentId = a.SYSTEM_ID,
                                 ParentObject = a.VAR_PROF_OGGETTO,
                                 ParentType = a.CHA_TIPO_PROTO,
                                 ParentSignature = a.VAR_SEGNATURA
                             }).FirstOrDefault();
                if (query != null && query.ParentId > 0)
                {
                    retval = new LinkedDocument()
                    {
                        DocumentType = query.ParentType,
                        Id = query.ParentId.ToString(),
                        Object = query.ParentObject,
                        Signature = query.ParentSignature,
                        LinkType = "PARENT"
                    };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return retval;
        }

        public static async Task<List<LinkedDocument>> getChildDocsInfoFromDocId(string idDocument, IPi3DbContext dbContext)
        {
            List<LinkedDocument> retval = null;
            try
            {
                var query = (from a in dbContext.ProfileEntities
                             where a.ID_PARENT == idDocument.AsLong()
                             select new
                             {
                                 LinkedId = a.SYSTEM_ID,
                                 LinkedObject = a.VAR_PROF_OGGETTO,
                                 LinkedType = a.CHA_TIPO_PROTO,
                                 LinkedSignature = a.VAR_SEGNATURA
                             });
                if (query != null && query.Any())
                {
                    retval = new List<LinkedDocument>();
                    foreach (var linked in query)
                    {
                        retval.Add(new LinkedDocument()
                        {
                            DocumentType = linked.LinkedType,
                            Id = linked.LinkedId.ToString(),
                            Object = linked.LinkedObject,
                            Signature = linked.LinkedSignature,
                            LinkType = "LINKED"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return retval;
        }

        public static async Task<List<DocEventInfo>> SearchDocEvents(string fromDate, string ToDate, string eventi, string idTipologie, string idList, InfoUtente infoUtente, IPi3DbContext dbContext)
        {
            List<DocEventInfo> retval = new List<DocEventInfo>();
            #region creazione query
            string query = "SELECT * FROM ( " +
                            "(SELECT P.SYSTEM_ID AS IDLOG, " +
                            "P.USERID_OPERATORE AS USER_ID_OP, " +
                            "P.ID_PEOPLE_OPERATORE AS ID_PEOPLE_OP, " +
                            "P.ID_GRUPPO_OPERATORE AS ID_RUOLO_OP, " +
                            "P.ID_AMM AS ID_AMM, " +
                            "TO_CHAR(DTA_AZIONE, 'dd/mm/yyyy hh24:mi:ss') AS DATAAZIONE, " +
                            "P.ID_OGGETTO AS ID_DOCUMENTO, " +
                            "P.VAR_DESC_OGGETTO AS DESC_OGGETTO, " +
                            "P.VAR_COD_AZIONE AS COD_AZIONE, " +
                            "P.CHA_ESITO AS ESITO, " +
                            "P.DESC_PRODUCER AS DESC_OP, " +
                            "P3.ID_TIPO_ATTO AS ID_TEMPLATE, " +
                            "(SELECT VAR_DESC_ATTO FROM DPA_TIPO_ATTO dta WHERE dta.SYSTEM_ID = P3.ID_TIPO_ATTO) as DESC_TEMPLATE, " +
                            "P3.VAR_SEGNATURA AS SEGNATURAPROTO, " +
                            "'' AS MOTIVO_ANNULLAMENTO_DET " +
                            "FROM DPA_LOG P, PROFILE P3 " +
                            "WHERE P.ID_OGGETTO = P3.SYSTEM_ID " +
                            "@fromDate@" + //"p.DTA_AZIONE >= to_date('@fromDate@ 00:00:00','dd/mm/yyyy HH24:mi:ss') " +
                            " @toDate@ " +
                            "AND  P.VAR_COD_AZIONE IN (@eventi@) " +
                            "AND EXISTS (SELECT 'X' FROM SECURITY E WHERE E.THING=P.ID_OGGETTO AND E.ACCESSRIGHTS > 20 AND E.PERSONORGROUP IN (@idRuolo@, @idUtente@)) " +
                            "@tipi@ @idList@ " +
                            "@union@ " +
                            ")) ORDER BY IDLOG";

            if (!string.IsNullOrEmpty(fromDate))
            {
                string fromDate2 = string.Format(" AND P.DTA_AZIONE >= TO_DATE ('{0}00:00:00','dd/mm/yyyy hh24:mi:ss') ", fromDate.Replace("'", "''"));
                query = query.Replace("@fromDate@", fromDate2);
            }
            else
                query = query.Replace("@fromDate@", "");

            if (!string.IsNullOrEmpty(ToDate))
            {
                string toDate2 = string.Format(" AND P.DTA_AZIONE < TO_DATE ('{0} 23:59:59','dd/mm/yyyy hh24:mi:ss') ", ToDate.Replace("'", "''"));

                query = query.Replace("@toDate@", toDate2);
            }
            else
            {
                query = query.Replace("@toDate@", "");
            }

            query = query.Replace("@eventi@", eventi);
            if (!string.IsNullOrWhiteSpace(idTipologie))
            {
                query = query.Replace("@tipi@", string.Format("AND P3.ID_TIPO_ATTO in ({0})", idTipologie));
            }
            else
            {
                query = query.Replace("@tipi@", "");
            }
            if (!String.IsNullOrWhiteSpace(idList))
            {
                query = query.Replace("@idList@", string.Format("AND P.ID_OGGETTO in ({0})", idList));
            }
            else
            {
                query = query.Replace("@idList@", "");
            }
            string union = "";
            if (eventi.ToUpper().Contains("ANNULLATA_LIQ"))
            {
                string fromDateLiq = "";
                if (!string.IsNullOrEmpty(fromDate))
                {
                    fromDateLiq = string.Format(" AND dat.DTA_INS >= TO_DATE ('{0} 00:00:00','dd/mm/yyyy hh24:mi:ss') ", fromDate.Replace("'", "''"));
                }

                string toDateLiq = "";
                if (!string.IsNullOrEmpty(ToDate))
                {
                    toDateLiq = string.Format(" AND dat.DTA_INS < TO_DATE ('{0} 23:59:59','dd/mm/yyyy hh24:mi:ss') ", ToDate.Replace("'", "''"));
                }

                string idListLiq = string.Empty;
                if (!String.IsNullOrWhiteSpace(idList))
                {
                    idListLiq = string.Format("AND P.ID_OGGETTO in ({0})", idList);
                }

                union += " UNION ";
                union += string.Format(" ( SELECT P.SYSTEM_ID AS IDLOG, P.USERID_OPERATORE AS USER_ID_OP, P.ID_PEOPLE_OPERATORE AS ID_PEOPLE_OP, P.ID_GRUPPO_OPERATORE AS ID_RUOLO_OP, " +
                    " P.ID_AMM AS ID_AMM, TO_CHAR(dat.DTA_INS, 'dd/mm/yyyy hh24:mi:ss') AS DATAAZIONE, P.ID_OGGETTO AS ID_DOCUMENTO,  " +
                    " '{0}' AS DESC_OGGETTO,'{1}' AS COD_AZIONE, " +
                    " P.CHA_ESITO AS ESITO, P.DESC_PRODUCER AS DESC_OP, dat.ID_template AS ID_TEMPLATE, " +
                    " (SELECT VAR_DESC_ATTO FROM DPA_TIPO_ATTO dta WHERE dta.SYSTEM_ID = dat.ID_TEMPLATE) as DESC_TEMPLATE, " +
                    " '' AS SEGNATURAPROTO, " +
                    " '' AS MOTIVO_ANNULLAMENTO_DET " +
                    " FROM DPA_ASSOCIAZIONE_TEMPLATES dat, DPA_LOG P       " +
                    " WHERE dat.ID_TEMPLATE = (SELECT SYSTEM_ID FROM DPA_TIPO_ATTO where upper(VAR_DESC_ATTO) = '{2}') " +
                    " AND dat.DOC_NUMBER = P.ID_OGGETTO " +
                    " {3}  " +
                    " {4}  " +
                    " AND P.System_id = (select system_id from dpa_log pp where pp.id_oggetto = dat.doc_number and " +
                    " pp.DTA_AZIONE > (dat.DTA_INS - INTERVAL '1' Minute) and pp.DTA_AZIONE < (dat.DTA_INS + INTERVAL '1' minute) and rownum =1)  " +
                    " AND dat.ID_OGGETTO IN (SELECT SYSTEM_ID FROM DPA_OGGETTI_CUSTOM WHERE SYSTEM_ID = DAT.ID_OGGETTO AND  UPPER(DESCRIZIONE) = UPPER('{5}')) " +
                    " AND dat.VALORE_OGGETTO_DB = '{6}' {7} AND EXISTS (SELECT 'X' FROM SECURITY E WHERE E.THING = P.ID_OGGETTO AND E.ACCESSRIGHTS > 20 AND E.PERSONORGROUP IN ({8}, {9}))) ",
                    "Annullata liquidazione", "ANNULLATA_LIQ", "LIQUIDAZIONE", fromDateLiq, toDateLiq, "Rifiutato Firma (annullo definitivo in contabilità)", "SI", idListLiq, infoUtente.idGruppo, infoUtente.idPeople);
            }
            if (eventi.ToUpper().Contains("ANNULLATA_DET"))
            {
                string fromDateDet = "";
                if (!string.IsNullOrEmpty(fromDate))
                {
                    fromDateDet = string.Format(" AND dat.DTA_INS >= TO_DATE ('{0} 00:00:00','dd/mm/yyyy hh24:mi:ss') ", fromDate.Replace("'", "''"));
                }

                string toDateDet = "";
                if (!string.IsNullOrEmpty(ToDate))
                {
                    toDateDet = string.Format(" AND dat.DTA_INS < TO_DATE ('{0} 23:59:59','dd/mm/yyyy hh24:mi:ss') ", ToDate.Replace("'", "''"));
                }

                string idListDet = string.Empty;
                if (!String.IsNullOrWhiteSpace(idList))
                {
                    idListDet = string.Format("AND P.ID_OGGETTO in ({0})", idList);
                }

                union += " UNION ";
                union += string.Format(" ( SELECT P.SYSTEM_ID AS IDLOG, " +
                    " P.USERID_OPERATORE AS USER_ID_OP, " +
                    " P.ID_PEOPLE_OPERATORE AS ID_PEOPLE_OP, " +
                    " P.ID_GRUPPO_OPERATORE AS ID_RUOLO_OP, " +
                    " P.ID_AMM AS ID_AMM, " +
                    " TO_CHAR(dat.DTA_INS, 'dd/mm/yyyy hh24:mi:ss') AS DATAAZIONE, " +
                    " P.ID_OGGETTO AS ID_DOCUMENTO,  " +
                    " '{0}' AS DESC_OGGETTO,'{1}' AS COD_AZIONE, " +
                    " P.CHA_ESITO AS ESITO, " +
                    " P.DESC_PRODUCER AS DESC_OP, " +
                    " dat.ID_template AS ID_TEMPLATE, " +
                    " (SELECT VAR_DESC_ATTO FROM DPA_TIPO_ATTO dta WHERE dta.SYSTEM_ID = dat.ID_TEMPLATE) as DESC_TEMPLATE, " +
                    " '' AS SEGNATURAPROTO, " +
                    "(select valore_oggetto_db from DPA_OGGETTI_CUSTOM a, dpa_associazione_templates b " +
                    "where A.SYSTEM_ID = B.ID_OGGETTO and upper(a.descrizione) = upper('{2}') " +
                    "and b.doc_number = p.id_oggetto) AS MOTIVO_ANNULLAMENTO_DET " +
                    " FROM DPA_ASSOCIAZIONE_TEMPLATES dat, DPA_LOG P " +
                    " WHERE dat.ID_TEMPLATE = (SELECT SYSTEM_ID FROM DPA_TIPO_ATTO where upper(VAR_DESC_ATTO) = '{3}') " +
                    " AND dat.DOC_NUMBER = P.ID_OGGETTO " +
                    " {4}  " +
                    " {5}  " +
                    " AND P.System_id = (select system_id from dpa_log pp where pp.id_oggetto = dat.doc_number and " +
                    " pp.DTA_AZIONE > (dat.DTA_INS - INTERVAL '1' Minute) and pp.DTA_AZIONE < (dat.DTA_INS + INTERVAL '1' minute) and rownum =1)  " +
                    " AND dat.ID_OGGETTO IN (SELECT SYSTEM_ID FROM DPA_OGGETTI_CUSTOM WHERE SYSTEM_ID = DAT.ID_OGGETTO AND  UPPER(DESCRIZIONE) = UPPER('{6}')) " +
                    " AND dat.VALORE_OGGETTO_DB = '{7}' {8} AND EXISTS(SELECT 'X' FROM SECURITY E WHERE E.THING = P.ID_OGGETTO AND E.ACCESSRIGHTS > 20 AND E.PERSONORGROUP IN ({9}, {10})) ) ",
                    "Rifiutato Firma (annullo definitivo in DDG)", "ANNULLATA_DET", "Motivo del rifiuto Firma", "DETERMINAZIONE", fromDateDet, toDateDet, "Rifiutato Firma (annullo definitivo in DDG)", "SI", idListDet, infoUtente.idGruppo, infoUtente.idPeople);
            }
            if (eventi.ToUpper().Contains("ANNULLATA_DEL"))
            {
                string fromDateDel = "";
                if (!string.IsNullOrEmpty(ToDate))
                {
                    fromDateDel = string.Format(" AND dat.DTA_INS >= TO_DATE ('{0} 00:00:00','dd/mm/yyyy hh24:mi:ss') ", fromDate.Replace("'", "''"));
                }

                string toDateDel = "";
                if (!string.IsNullOrEmpty(ToDate))
                {
                    toDateDel = string.Format(" AND dat.DTA_INS < TO_DATE ('{0} 23:59:59','dd/mm/yyyy hh24:mi:ss') ", ToDate.Replace("'", "''"));
                }

                string idListDel = string.Empty;
                if (!String.IsNullOrWhiteSpace(idList))
                {
                    idListDel = string.Format("AND P.ID_OGGETTO in ({0})", idList);
                }

                union += " UNION ";
                union += string.Format(" ( SELECT P.SYSTEM_ID AS IDLOG, P.USERID_OPERATORE AS USER_ID_OP, P.ID_PEOPLE_OPERATORE AS ID_PEOPLE_OP, P.ID_GRUPPO_OPERATORE AS ID_RUOLO_OP, " +
                    " P.ID_AMM AS ID_AMM, TO_CHAR(dat.DTA_INS, 'dd/mm/yyyy hh24:mi:ss') AS DATAAZIONE, P.ID_OGGETTO AS ID_DOCUMENTO,  " +
                    " '{0}' AS DESC_OGGETTO,'{1}' AS COD_AZIONE, " +
                    " P.CHA_ESITO AS ESITO, P.DESC_PRODUCER AS DESC_OP, dat.ID_template AS ID_TEMPLATE, " +
                    " (SELECT VAR_DESC_ATTO FROM DPA_TIPO_ATTO dta WHERE dta.SYSTEM_ID = dat.ID_TEMPLATE) as DESC_TEMPLATE, " +
                    " '' AS SEGNATURAPROTO, " +
                    "(select valore_oggetto_db from DPA_OGGETTI_CUSTOM a, dpa_associazione_templates b " +
                    "where A.SYSTEM_ID = B.ID_OGGETTO and upper(a.descrizione) = upper('{2}') " +
                    "and b.doc_number = p.id_oggetto) AS MOTIVO_ANNULLAMENTO_DET " +
                    " FROM DPA_ASSOCIAZIONE_TEMPLATES dat, DPA_LOG P " +
                    " WHERE dat.ID_TEMPLATE = (SELECT SYSTEM_ID FROM DPA_TIPO_ATTO where upper(VAR_DESC_ATTO) = '{3}') " +
                    " AND dat.DOC_NUMBER = P.ID_OGGETTO " +
                    " {4}  " +
                    " {5}  " +
                    " AND P.System_id = (select system_id from dpa_log pp where pp.id_oggetto = dat.doc_number and " +
                    " pp.DTA_AZIONE > (dat.DTA_INS - INTERVAL '1' Minute) and pp.DTA_AZIONE < (dat.DTA_INS + INTERVAL '1' minute) and rownum =1)  " +
                    " AND dat.ID_OGGETTO IN (SELECT SYSTEM_ID FROM DPA_OGGETTI_CUSTOM WHERE SYSTEM_ID = DAT.ID_OGGETTO AND  UPPER(DESCRIZIONE) = UPPER('{6}')) " +
                    " AND dat.VALORE_OGGETTO_DB = '{7}' {8} AND EXISTS(SELECT 'X' FROM SECURITY E WHERE E.THING = P.ID_OGGETTO AND E.ACCESSRIGHTS > 20 AND E.PERSONORGROUP IN ({9}, {10})) ) ",
                    "Rifiutato Firma (annullo definitivo in DDG)", "ANNULLATA_DEL", "Motivo del rifiuto Firma", "PARERE DELIBERAZIONE", fromDateDel, toDateDel, "Rifiutato Firma (annullo definitivo in DDG)", "SI", idListDel, infoUtente.idGruppo, infoUtente.idPeople);
            }
            if (eventi.ToUpper().Contains("ANNULLATA_DEC"))
            {
                string fromDateDec = "";
                if (!string.IsNullOrEmpty(fromDate))
                {
                    fromDateDec = string.Format(" AND dat.DTA_INS >= TO_DATE ('{0} 00:00:00','dd/mm/yyyy hh24:mi:ss') ", fromDate.Replace("'", "''"));
                }

                string toDateDec = "";
                if (!string.IsNullOrEmpty(ToDate))
                {
                    toDateDec = string.Format(" AND dat.DTA_INS < TO_DATE ('{0} 23:59:59','dd/mm/yyyy hh24:mi:ss') ", ToDate.Replace("'", "''"));
                }

                string idListDec = string.Empty;
                if (!String.IsNullOrWhiteSpace(idList))
                {
                    idListDec = string.Format("AND P.ID_OGGETTO in ({0})", idList);
                }

                union += " UNION ";
                union += string.Format(" ( SELECT P.SYSTEM_ID AS IDLOG, P.USERID_OPERATORE AS USER_ID_OP, P.ID_PEOPLE_OPERATORE AS ID_PEOPLE_OP, P.ID_GRUPPO_OPERATORE AS ID_RUOLO_OP, " +
                    " P.ID_AMM AS ID_AMM, TO_CHAR(dat.DTA_INS, 'dd/mm/yyyy hh24:mi:ss') AS DATAAZIONE, P.ID_OGGETTO AS ID_DOCUMENTO,  " +
                    " '{0}' AS DESC_OGGETTO,'{1}' AS COD_AZIONE, " +
                    " P.CHA_ESITO AS ESITO, P.DESC_PRODUCER AS DESC_OP, dat.ID_template AS ID_TEMPLATE, " +
                    " (SELECT VAR_DESC_ATTO FROM DPA_TIPO_ATTO dta WHERE dta.SYSTEM_ID = dat.ID_TEMPLATE) as DESC_TEMPLATE, " +
                    " '' AS SEGNATURAPROTO, " +
                    "(select valore_oggetto_db from DPA_OGGETTI_CUSTOM a, dpa_associazione_templates b " +
                    "where A.SYSTEM_ID = B.ID_OGGETTO and upper(a.descrizione) = upper('{2}') " +
                    "and b.doc_number = p.id_oggetto) AS MOTIVO_ANNULLAMENTO_DET " +
                    " FROM DPA_ASSOCIAZIONE_TEMPLATES dat, DPA_LOG P " +
                    " WHERE dat.ID_TEMPLATE = (SELECT SYSTEM_ID FROM DPA_TIPO_ATTO where upper(VAR_DESC_ATTO) = '{3}') " +
                    " AND dat.DOC_NUMBER = P.ID_OGGETTO " +
                    " {4}  " + //"AND dat.DTA_INS >= TO_DATE('{4} 00:00:00', 'dd/mm/yyyy HH24:mi:ss') " +
                    " {5}  " +
                    " AND P.System_id = (select system_id from dpa_log pp where pp.id_oggetto = dat.doc_number and " +
                    " pp.DTA_AZIONE > (dat.DTA_INS - INTERVAL '1' Minute) and pp.DTA_AZIONE < (dat.DTA_INS + INTERVAL '1' minute) and rownum =1)  " +
                    " AND dat.ID_OGGETTO IN (SELECT SYSTEM_ID FROM DPA_OGGETTI_CUSTOM WHERE SYSTEM_ID = DAT.ID_OGGETTO AND  UPPER(DESCRIZIONE) = UPPER('{6}')) " +
                    " AND dat.VALORE_OGGETTO_DB = '{7}' {8} AND EXISTS(SELECT 'X' FROM SECURITY E WHERE E.THING = P.ID_OGGETTO AND E.ACCESSRIGHTS > 20 AND E.PERSONORGROUP IN ({9}, {10})) ) ",
                    "Rifiutato Firma", "ANNULLATA_DEC", "Motivo del rifiuto Firma", "DECRETO DEL PRESIDENTE", fromDateDec, toDateDec, "Rifiutato Firma", "SI", idListDec, infoUtente.idGruppo, infoUtente.idPeople);
            }
            query = query.Replace("@union@", union);
            query = query.Replace("@idRuolo@", infoUtente.idGruppo);
            query = query.Replace("@idUtente@", infoUtente.idPeople);
            #endregion

            DataSet dsRes = new DataSet();
            using (var connection = ((DbContext)dbContext).Database.GetDbConnection())
            {
                using (var command = ((DbContext)dbContext).Database.GetDbConnection().CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    try
                    {
                        connection.Open();
                        var reader = command.ExecuteReader();
                        do
                        {
                            // loads the DataTable (schema will be fetch automatically)
                            var tb = new DataTable();
                            tb.Load(reader);
                            dsRes.Tables.Add(tb);

                        } while (!reader.IsClosed);


                        if (dsRes != null && dsRes.Tables[0] != null && dsRes.Tables[0].Rows != null && dsRes.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow r in dsRes.Tables[0].Rows)
                            {
                                var eventInfo = new DocEventInfo()
                                {
                                    ActionCode = r["COD_AZIONE"].ToString(),
                                    ActionDate = r["DATAAZIONE"].ToString(),
                                    AdministrationId = r["ID_AMM"].ToString(),
                                    IdDocument = r["ID_DOCUMENTO"].ToString(),
                                    IdLog = r["IDLOG"].ToString(),
                                    ObjectDescription = r["DESC_OGGETTO"].ToString(),
                                    OperationExecuted = r["ESITO"].ToString(),
                                    OperatorDescription = r["DESC_OP"].ToString(),
                                    OperatorPeopleID = r["ID_PEOPLE_OP"].ToString(),
                                    OperatorRoleID = r["ID_RUOLO_OP"].ToString(),
                                    OperatorUsername = r["USER_ID_OP"].ToString(),

                                };
                                string idTemplate = r["ID_TEMPLATE"].ToString();
                                if (!string.IsNullOrWhiteSpace(idTemplate))
                                {
                                    eventInfo.Template = new Template()
                                    {
                                        Id = idTemplate,
                                        Name = r["DESC_TEMPLATE"].ToString()
                                    };
                                }

                                #region gestione Other info
                                List<FieldLite> otherinfos = new List<FieldLite>();
                                switch (eventInfo.ActionCode)
                                {
                                    case "DOCUMENTO_REPERTORIATO":
                                        // Il prelievo della segnatura di repertorio ci mette troppo tempo.
                                        string segRep = "";
                                        if (!string.IsNullOrWhiteSpace(eventInfo.ObjectDescription) && eventInfo.ObjectDescription.ToLower().Contains("repertoriato documento"))
                                        {
                                            segRep = eventInfo.ObjectDescription.ToUpper().Replace("REPERTORIATO DOCUMENTO:", "").Trim();
                                        }
                                        if (string.IsNullOrWhiteSpace(segRep))
                                        {
                                            // sfrutto il nuovo campo VAR_SEGNATURA
                                            segRep = (from a in dbContext.AssociazioneTemplatesEntities
                                                      where a.DOC_NUMBER == eventInfo.IdDocument
                                                      && a.VAR_SEGNATURA != null
                                                      select a.VAR_SEGNATURA
                                                     ).FirstOrDefault();
                                        }
                                        otherinfos.Add(new FieldLite() { Name = "Segnatura di Repertorio", Value = segRep });
                                        break;

                                    case "AGG_PROT":
                                    case "RECORD_PREDISPOSED":
                                        otherinfos.Add(new FieldLite() { Name = "Segnatura di Protocollo", Value = r["SEGNATURAPROTO"].ToString() });
                                        break;
                                    case "DOC_SIGNATURE":
                                    case "DOC_SIGNATURE_P":
                                        var versionId = (from a in dbContext.VersionEntities
                                                         where a.DOCNUMBER == eventInfo.IdDocument.AsLong() && (a.CHA_SEGNATURA == null || a.CHA_SEGNATURA == "0")
                                                         select a.VERSION_ID).Max();
                                        List<string> firmatariList = (from a in dbContext.FirmatarioDocEntities
                                                                      where a.ID_PROFILE == eventInfo.IdDocument.AsLong() && a.ID_VERSION == versionId
                                                                      select a.DESCRIZIONE_FIRMATARIO).ToList();
                                        string firmatari = string.Empty;
                                        if (firmatariList != null && firmatariList.Count > 0)
                                        {
                                            firmatariList.ForEach(f => firmatari += f + " ");
                                        }
                                        //Si aggiunge nel campo OperatorUsername 
                                        eventInfo.OperatorUsername = firmatari;
                                        eventInfo.OperatorDescription = firmatari;
                                        eventInfo.OperatorPeopleID = string.Empty;
                                        eventInfo.OperatorRoleID = string.Empty;
                                        break;
                                    case "ANNULLATA_DET":
                                    case "ANNULLATA_DEC":
                                    case "ANNULLATA_DEL":
                                        otherinfos.Add(new FieldLite() { Name = "Motivo del rifiuto Firma", Value = r["MOTIVO_ANNULLAMENTO_DET"].ToString() });

                                        break;
                                }
                                if (otherinfos != null && otherinfos.Count > 0) eventInfo.OtherInfo = otherinfos.ToArray();

                                retval.Add(eventInfo);
                                #endregion

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("APPLICATION_ERROR");
                    }

                }
            }
            return retval;
        }

        public static async Task<SearchDocumentsCommandResponse> SearchDocuments(Filter[] filtri, InfoUtente infoUt, int numPage, int pageSize, bool allDocuments, IPi3DbContext dbContext)
        {
            SearchDocumentsCommandResponse response = new SearchDocumentsCommandResponse();
            List<Document> retval = new List<Document>();
            #region creazione query
            string queryCount = "SELECT COUNT(*) FROM PROFILE A ";
            string query = "select A.system_id, A.DOCNUMBER, TO_CHAR(A.CREATION_DATE,'dd/mm/yyyy hh24:mi:ss') as dta_creazione," +
                " TO_CHAR(A.DTA_PROTO_IN,'dd/mm/yyyy hh24:mi:ss') as dta_arrivo, " +
                " TO_CHAR(A.DTA_ANNULLA,'dd/mm/yyyy hh24:mi:ss') as dta_annullamento, a.cha_tipo_proto, a.id_documento_principale, a.var_prof_oggetto, a.var_segnatura," +
                " A.CHA_PRIVATO, A.CHA_PERSONALE, A.ID_REGISTRO, (select var_codice from dpa_el_registri der where der.system_id = a.id_registro) as codregistro, " +
                " A.NUM_PROTO, A.NUM_ANNO_PROTO, TO_CHAR(A.DTA_PROTO,'dd/mm/yyyy') as protocoldate, " +
                " A.ID_TIPO_ATTO, (select var_desc_atto from dpa_tipo_atto dta where dta.system_id = a.id_tipo_atto) as var_desc_atto from profile A ";
            string condition = $"WHERE (A.CHA_IN_CESTINO IS NULL OR A.CHA_IN_CESTINO = '0' ) AND EXISTS (select 'x' from security where thing = A.system_id and personorgroup in ({infoUt.idGruppo},{infoUt.idPeople})) ";
            string pagination = "";
            List<string> tipo_proto = new List<string>();
            bool predisposti = false, allegati = false, estrazioneTemplate = false;
            #endregion
            // gestione filtri
            if (filtri != null && filtri.Length > 0)
            {
                foreach (Filter filtro in filtri)
                {
                    filtro.Value = filtro.Value.Replace("'", "''");
                    switch (filtro.Name.ToUpper())
                    {
                        case "YEAR":
                            condition += $" AND (A.CREATION_DATE BETWEEN TO_DATE('01/01/{filtro.Value} 00:00:00', 'dd/mm/yyyy hh24:mi:ss') and to_date('31/12/{filtro.Value} 23:59:59', 'dd/mm/yyyy hh24:mi:ss') or A.NUM_ANNO_PROTO = {filtro.Value}) ";
                            break;
                        case "IN_PROTOCOL":
                            if (filtro.Value.ToLower() == "true")
                            {
                                tipo_proto.Add("A");
                            }
                            break;
                        case "OUT_PROTOCOL":
                            if (filtro.Value.ToLower() == "true")
                            {
                                tipo_proto.Add("P");
                            }
                            break;
                        case "INTERNAL_PROTOCOL":
                            if (filtro.Value.ToLower() == "true")
                            {
                                tipo_proto.Add("I");
                            }
                            break;
                        case "NOT_PROTOCOL":
                            if (filtro.Value.ToLower() == "true")
                            {
                                tipo_proto.Add("G");
                            }
                            break;
                        case "PREDISPOSED":
                            if (filtro.Value.ToLower() == "true")
                            {
                                predisposti = true;
                            }
                            break;
                        case "ATTACHMENTS":
                            if (filtro.Value.ToLower() == "true")
                            {
                                allegati = true;
                            }
                            break;
                        case "PRINTS":
                            if (filtro.Value.ToLower() == "true")
                            {
                                tipo_proto.Add("C");
                                tipo_proto.Add("R");
                            }
                            break;
                        case "NUM_PROTOCOL_FROM":
                            condition += $" AND A.NUM_PROTO >= {filtro.Value}";
                            break;
                        case "NUM_PROTOCOL_TO":
                            condition += $" AND A.NUM_PROTO <= {filtro.Value}";
                            break;
                        case "CREATION_DATE_FROM":
                            condition += $" AND A.CREATION_DATE >= TO_DATE('{filtro.Value} 00:00:00', 'dd/mm/yyyy hh24:mi:ss')";

                            break;
                        case "CREATION_DATE_TO":
                            condition += $" AND A.CREATION_DATE <= TO_DATE('{filtro.Value} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')";
                            break;
                        case "PROTOCOL_DATE_FROM":
                            condition += $" AND A.DTA_PROTO >= TO_DATE('{filtro.Value} 00:00:00', 'dd/mm/yyyy hh24:mi:ss')";
                            break;
                        case "PROTOCOL_DATE_TO":
                            condition += $" AND A.DTA_PROTO <= TO_DATE('{filtro.Value} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')";
                            break;
                        case "SENDER_RECIPIENT":
                            condition += $" AND exists (select 'x' from dpa_doc_arrivo_par dap where dap.id_profile = a.system_id and dap.id_mitt_dest in ({filtro.Value}))";
                            break;
                        case "TEMPLATE":
                            condition += $" AND A.ID_TIPO_ATTO = {filtro.Value}";
                            if (filtro.Template != null && filtro.Template.Fields != null && filtro.Template.Fields.Length > 0)
                            {
                                foreach (var campo in filtro.Template.Fields)
                                {
                                    if (!string.IsNullOrWhiteSpace(campo.Name) && !string.IsNullOrEmpty(campo.Value))
                                    {
                                        condition += $" AND EXISTS (select 'x' from dpa_associazione_templates dat join dpa_oggetti_custom doc on dat.ID_OGGETTO = doc.system_id where upper(doc.descrizione) ='{campo.Name.Replace("'", "''").ToUpper()}' and upper(dat.valore_oggetto_db) like '%{campo.Value.Replace("'", "''").ToUpper()}%' )";
                                    }
                                }
                            }
                            break;
                        case "DOCNUMBER_FROM":
                            condition += $" AND A.SYSTEM_ID >= {filtro.Value}";
                            break;
                        case "DOCNUMBER_TO":
                            condition += $" AND A.SYSTEM_ID <= {filtro.Value}";
                            break;
                        case "REGISTER":
                            var registro = getRegistroByCodAOO(filtro.Value, infoUt.idAmministrazione, dbContext);
                            if (registro != null)
                            {
                                condition += $" AND A.id_registro = {registro.systemId}";
                            }
                            else
                            {
                                throw new RestException("REGISTER_NOT_FOUND");
                            }
                            break;
                        case "OBJECT":
                            condition += $" AND UPPER(a.VAR_PROF_OGGETTO) LIKE UPPER('%{filtro.Value}%')";
                            break;
                        case "FULL_TEXT_SEARCH":
                            break;
                        case "TEMPLATE_EXTRACTION":
                            if (filtro.Value.ToLower() == "true")
                            { estrazioneTemplate = true; }
                            break;
                        case "STATEDIAGRAM":
                            int iddiagramma = 0;
                            if (Int32.TryParse(filtro.Value, out iddiagramma))
                                condition += string.Format(" AND EXISTS (select 'x' from dpa_diagrammi where dpa_diagrammi.doc_number = a.system_id and dpa_diagrammi.id_diagramma = {0})", iddiagramma.ToString());
                            else
                                condition += string.Format(" AND EXISTS (SELECT 'X' FROM DPA_DIAGRAMMI, dpa_diagrammi_stato where dpa_diagrammi.doc_number = a.system_id and dpa_diagrammi.id_diagramma = dpa_diagrammi_stato.system_id and upper(dpa_diagrammi_Stato.var_Descrizione) = '{0}')", filtro.Value);
                            break;
                        case "DOCUMENT_STATES":
                            string stati = "";
                            if (filtro.Value.Contains(";"))
                            {
                                stati = filtro.Value.ToUpper().Replace(";", "','");
                            }
                            else stati = filtro.Value.ToUpper();

                            condition += string.Format(" and exists(select 'x' from dpa_diagrammi, dpa_stati where dpa_diagrammi.doc_number = a.system_id and dpa_diagrammi.id_stato = dpa_stati.system_id and upper(dpa_stati.var_descrizione) in ('{0}'))", stati);

                            break;
                        case "NOT_IN_PROJECT":
                            if (filtro.Value.ToLower() == "true")
                            {
                                condition += $" AND not exists (select 'x' from project_components where link = a.system_id)";
                            }
                            break;
                        default:
                            //throw new RestException("FILTER_NOT_FOUND");
                            break;
                    }
                }
            }
            string tipoProtoCondition = "", predispostiCondition = "";
            if (!allegati)
            {
                condition += " AND A.ID_DOCUMENTO_PRINCIPALE IS NULL";
            }
            else
            {
                if (!tipo_proto.Contains("G")) tipo_proto.Add("G");
            }

            if (predisposti)
            {
                predispostiCondition = " CHA_TIPO_PROTO IN ('A','P','I') and VAR_SEGNATURA IS NULL ";

            }
            if (tipo_proto.Count > 0)
            {
                if (tipo_proto.Count > 1)
                    tipoProtoCondition += $" AND ((A.CHA_TIPO_PROTO in ('{string.Join("','", tipo_proto)}') AND (A.CHA_DA_PROTO IS NULL OR A.CHA_DA_PROTO != '1')  ) " + (predisposti ? $"OR ({predispostiCondition}) " : "") + " )";
                else
                    tipoProtoCondition += $" AND (A.CHA_TIPO_PROTO = '{tipo_proto.First()}' " + (predisposti ? $"OR ({predispostiCondition}) " : " AND (A.CHA_DA_PROTO IS NULL OR A.CHA_DA_PROTO != '1') ") + ")";
            }
            else if (predisposti)
            {
                tipoProtoCondition += $" AND {predispostiCondition} ";
            }
            condition += tipoProtoCondition;



            // Effettuare prima la count
            int numRisultati = 0;
            DataSet dsRes = new DataSet();
            using (var connection = ((DbContext)dbContext).Database.GetDbConnection())
            {
                connection.Open(); try
                {
                    using (var command = ((DbContext)dbContext).Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryCount + condition;


                        string results = command.ExecuteScalar().ToString();
                        numRisultati = Int32.Parse(results);


                    }

                    if (numRisultati == 0)
                    {
                        return new SearchDocumentsCommandResponse()
                        {
                            Code = SearchDocumentsResponseCode.OK,
                            Documents = new Document[0],
                            TotalDocumentsNumber = 0
                        };
                    }
                    else if (numRisultati > 2000)
                    {
                        throw new RestException("TOO_MANY_RESULTS");

                    }
                    response.TotalDocumentsNumber = numRisultati;

                    // quindi prelevare i dati
                    if (!allDocuments)
                    {
                        pagination = $" OFFSET {((numPage - 1) * pageSize)} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                    }



                    using (var command2 = ((DbContext)dbContext).Database.GetDbConnection().CreateCommand())
                    {
                        command2.CommandType = CommandType.Text;
                        command2.CommandText = string.Concat(query, condition, pagination);

                        var reader = command2.ExecuteReader();
                        do
                        {
                            // loads the DataTable (schema will be fetch automatically)
                            var tb = new DataTable();
                            tb.Load(reader);
                            dsRes.Tables.Add(tb);

                        } while (!reader.IsClosed);


                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("APPLICATION_ERROR");
                }
            }

            if (dsRes != null && dsRes.Tables[0] != null && dsRes.Tables[0].Rows != null && dsRes.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in dsRes.Tables[0].Rows)
                {
                    string value;

                    Document doc = new Document()
                    {
                        Id = r["SYSTEM_ID"].ToString(),
                        Object = r["VAR_PROF_OGGETTO"].ToString()
                    };

                    doc.DocNumber = r["DOCNUMBER"].ToString();
                    value = r["DTA_ANNULLAMENTO"].ToString();
                    if (!string.IsNullOrEmpty(value)) doc.Annulled = true;
                    doc.ArrivalDate = r["dta_arrivo"].ToString();
                    doc.CreationDate = r["dta_creazione"].ToString();
                    doc.DocumentType = r["cha_tipo_proto"].ToString();
                    doc.IdParent = r["id_documento_principale"].ToString();
                    if (!string.IsNullOrWhiteSpace(doc.IdParent) && doc.DocumentType == "G") doc.IsAttachments = true;
                    doc.Signature = r["var_segnatura"].ToString();
                    value = r["cha_privato"].ToString();
                    if (!string.IsNullOrWhiteSpace(value) && value == "1") doc.PrivateDocument = true;
                    value = r["cha_personale"].ToString();
                    if (!string.IsNullOrWhiteSpace(value) && value == "1") doc.PersonalDocument = true;

                    if (string.IsNullOrWhiteSpace(doc.Signature) && doc.DocumentType != "G") doc.Predisposed = true;

                    doc.ProtocolNumber = r["num_proto"].ToString();
                    doc.ProtocolYear = r["num_anno_proto"].ToString();
                    doc.ProtocolDate = r["protocoldate"].ToString();

                    value = r["id_registro"].ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        doc.Register =
                            new Registers.Register()
                            {
                                Id = value,
                                Code = r["codregistro"].ToString()
                            };
                    }

                    value = r["id_tipo_atto"].ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        doc.Template =
                            new Template()
                            {
                                Id = value,
                                Name = r["var_desc_atto"].ToString()
                            };

                        if (estrazioneTemplate)
                        {
                            doc.Template = getTemplateFromDocumentId(doc.Id, dbContext);
                        }

                    }

                    retval.Add(doc);

                }
            }
            response.Documents = retval.ToArray();

            return response;
        }


        public static async Task<List<Documento>> GetVersionsMainDocument(InfoUtente infoUtente, long idPeople, long idGruppo, long docnumber, IPi3DbContext dbContext)
        {
            long idCorrGlobaliPeople = await dbContext.CorrGlobaliEntities.Where(c => c.ID_PEOPLE == idPeople).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            long idCorrGlobaliGruppo = Convert.ToInt32(infoUtente.idCorrGlobali);

            var profile = await dbContext.ProfileEntities
                .Join(
                    dbContext.DocumentTypesEntities,
                    profile => profile.DOCUMENTTYPE,
                    documentTypes => documentTypes.SYSTEM_ID,
                    (profile, documentType) => new
                    {
                        profile.SYSTEM_ID,
                        profile.DOCNUMBER,
                        profile.DOCSERVER_LOC,
                        profile.PATH,
                        profile.IN_LIBROFIRMA,
                        documentType.TYPE_ID,
                        documentType.DESCRIPTION
                    }
                ).FirstOrDefaultAsync(p => p.DOCNUMBER == docnumber);

            var versions = await dbContext.ComponentEntities
                .Join(
                    dbContext.VersionEntities,
                    components => components.VERSION_ID,
                    versions => versions.VERSION_ID,
                    (components, versions) => new
                    {
                        versions.VERSION_ID,
                        versions.DOCNUMBER,
                        versions.VERSION,
                        versions.SUBVERSION,
                        versions.VERSION_LABEL,
                        versions.AUTHOR,
                        versions.COMMENTS,
                        versions.NUM_PAG_ALLEGATI,
                        versions.CHA_DA_INVIARE,
                        versions.CHA_SEGNATURA,
                        versions.DTA_CREAZIONE,
                        versions.DTA_ARRIVO,
                        versions.CARTACEO,
                        versions.ID_PEOPLE_DELEGATO,
                        components.ID_PEOPLE_PUTFILE,
                        components.DTA_FILE_ACQUIRED,
                        components.ID_PEOPLE_DELEGATO_PUTFILE,
                        components.PATH,
                        components.VAR_IMPRONTA,
                        components.FILE_SIZE,
                        components.CHA_FIRMATO,
                        components.CHA_TIPO_FIRMA,
                        components.VAR_NOMEORIGINALE
                    }
                 )
                .Join(
                    dbContext.PeopleEntities,
                    versions => versions.AUTHOR,
                    people => people.SYSTEM_ID,
                    (versions, people) => new { people.FULL_NAME, versions }
                )
                .Where(v => v.versions.DOCNUMBER == docnumber && v.versions.VERSION > 0)
                .ToListAsync();

            var hide_doc_versions = await dbContext.TrasmissioneEntities
                .Join(
                    dbContext.TrasmSingolaEntities,
                    trasmissione => trasmissione.SYSTEM_ID,
                    trasmSingola => trasmSingola.ID_TRASMISSIONE,
                    (trasmissione, trasmSingola) => new { trasmissione.ID_PROFILE, trasmSingola.HIDE_DOC_VERSIONS, trasmSingola.ID_CORR_GLOBALE }
                )
                .FirstOrDefaultAsync(t => t.ID_PROFILE == docnumber && (t.ID_CORR_GLOBALE == idCorrGlobaliGruppo || t.ID_CORR_GLOBALE == idCorrGlobaliPeople));

            if (hide_doc_versions != null && hide_doc_versions.HIDE_DOC_VERSIONS == "1")
            {
                var security = await dbContext.SecurityEntities
                    .FirstOrDefaultAsync(s => s.THING == docnumber && (s.PERSONORGROUP == idGruppo || s.PERSONORGROUP == idPeople) && (s.CHA_TIPO_DIRITTO == "P") || s.CHA_TIPO_DIRITTO == "A");

                if (security == null)
                {
                    var max_version = versions.Max(v => v.versions.VERSION_ID);
                    versions = versions.Where(v => v.versions.VERSION_ID == max_version).ToList();
                }
            }

            var output = new List<Documento>();
            foreach (var version in versions.OrderByDescending(v => v.versions.VERSION_ID))
            {
                var documento = new DocsPaVO.documento.Documento();

                documento.docNumber = version.versions.DOCNUMBER.ToString();
                documento.versionId = version.versions.VERSION_ID.ToString();
                documento.version = version.versions.VERSION.ToString();
                documento.subVersion = version.versions.SUBVERSION;
                documento.versionLabel = version.versions.VERSION_LABEL;
                documento.idPeople = version.versions.AUTHOR.ToString();
                documento.descrizione = version.versions.COMMENTS;
                documento.fileName = version.versions.VAR_NOMEORIGINALE != null ? version.versions.VAR_NOMEORIGINALE : string.Empty;
                documento.fileSize = version.versions.FILE_SIZE.ToString();
                documento.autore = version.FULL_NAME;
                documento.idPeopleDelegato = "0";
                if (version.versions.ID_PEOPLE_DELEGATO != null && version.versions.ID_PEOPLE_DELEGATO != 0)
                {
                    documento.idPeopleDelegato = version.versions.ID_PEOPLE_DELEGATO.ToString();
                    documento.autore = dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_DELEGATO).First().FULL_NAME + " SOSTITUTO DI " + version.FULL_NAME;
                }

                if (version.versions.ID_PEOPLE_PUTFILE != null && version.versions.ID_PEOPLE_PUTFILE > 0)
                    documento.autoreFile = dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_PUTFILE).First().FULL_NAME;

                if (version.versions.ID_PEOPLE_DELEGATO_PUTFILE != null && version.versions.ID_PEOPLE_DELEGATO_PUTFILE > 0)
                    documento.autoreFile = dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_DELEGATO_PUTFILE).First().FULL_NAME + " SOSTITUTO DI " + documento.autoreFile;

                documento.dataAcquisizione = version.versions.DTA_FILE_ACQUIRED != null ? version.versions.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null;
                documento.docServerLoc = profile.DOCSERVER_LOC;
                documento.path = version.versions.PATH;
                documento.dataInserimento = version.versions.DTA_CREAZIONE.AsDateTimeFormat();
                documento.dataArrivo = version.versions.DTA_ARRIVO.AsDateTimeFormat();
                documento.daInviare = version.versions.CHA_DA_INVIARE;
                documento.firmato = version.versions.CHA_FIRMATO;
                documento.tipoFirma = version.versions.CHA_TIPO_FIRMA;
                documento.tipologia = new TipologiaCanale { codice = profile.TYPE_ID, descrizione = profile.DESCRIPTION };
                documento.cartaceo = version.versions.CARTACEO > 0;
                documento.impronta = version.versions.VAR_IMPRONTA != null ? version.versions.VAR_IMPRONTA : string.Empty;
                documento.inLibroFirma = profile.IN_LIBROFIRMA == "1";
                documento.conSegnaturaPermanente = version.versions.CHA_SEGNATURA == "1";

                output.Add(documento);
            }

            return output;
        }


        #endregion

        #region Fascicoli

        public static Project getProjectFromDB(string idProject, IPi3DbContext dbContext)
        {
            var idProjectAsLong = idProject.AsLong();

            Project retval = null;

            var query = (from a in dbContext.ProjectEntities
                         where a.SYSTEM_ID == idProjectAsLong
                         select a)
                         .FirstOrDefault();

            if (query != null && query.SYSTEM_ID != 0)
            {
                retval = new Project()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.DESCRIPTION,
                    Code = query.VAR_CODICE,
                    ClosureDate = query.DTA_CHIUSURA == null ? null : query.DTA_CHIUSURA.Value.ToString("dd/MM/yyyy"),
                    Open = query.CHA_STATO == "A",
                    OpeningDate = query.DTA_APERTURA.Value.ToString("dd/MM/yyyy"),
                    //CreationDate = query.DTA_CREAZIONE.Value.ToString("dd/MM/yyyy"),
                    CreationDate = query.DTA_CREAZIONE == null ? null : query.DTA_CREAZIONE.Value.ToString("dd/MM/yyyy"),
                    CollocationDate = query.DTA_UO_LF == null ? null : query.DTA_UO_LF.Value.ToString("dd/MM/yyyy"),
                    Type = query.CHA_TIPO_FASCICOLO,
                    Number = query.NUM_FASCICOLO > 0 ? query.NUM_FASCICOLO.ToString() : null,
                    Controlled = query.CHA_CONTROLLATO == "1",
                    IdParent = query.ID_PARENT.ToString(),
                    Paper = query.CARTACEO == "1",
                    Private = query.CHA_PRIVATO == "1"

                };

                if (query.ID_TITOLARIO != null)
                {
                    var titolario = getTitolarioById(query.ID_TITOLARIO.ToString(), dbContext);
                    retval.ClassificationScheme = new ClassificationSchemes.ClassificationScheme()
                    {
                        Id = titolario.ID,
                        Description = titolario.Descrizione,
                        Active = titolario.Stato == OrgStatiTitolarioEnum.Attivo
                    };
                }

                if (query.ID_REGISTRO != null)
                {
                    var registro = getRegistro(query.ID_REGISTRO.ToString(), dbContext);
                    retval.Register = RestUtils.GetRegister(registro);
                }

                if (query.DTA_UO_LF != null)
                {
                    retval.PhysicsCollocation = (from a in dbContext.CorrGlobaliEntities where a.SYSTEM_ID == query.ID_UO_LF select a.VAR_DESC_CORR).FirstOrDefault();
                }

                if (query.ID_PARENT > 0 && retval.Type == "P")
                {
                    retval.CodeNodeClassification = (from a in dbContext.ProjectEntities where a.SYSTEM_ID == query.ID_PARENT select a.VAR_CODICE).FirstOrDefault();
                }
                var listnote = getNoteOggetto(idProject, dbContext);
                if (listnote != null && listnote.Any())
                    retval.Note = listnote.ToArray();

                retval.Template = getTemplateFromProjectId(idProject, dbContext);

            }
            return retval;
        }

        public static Project getProjectLiteById(string idProject, IPi3DbContext dbContext)
        {
            Project retval = null;

            var query = (from a in dbContext.ProjectEntities
                         where a.SYSTEM_ID.ToString() == idProject
                         select a).FirstOrDefault();
            if (query != null && query.SYSTEM_ID > 0)
            {
                retval = new Project()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.DESCRIPTION,
                    Code = query.VAR_CODICE,
                    ClosureDate = query.DTA_CHIUSURA == null ? null : query.DTA_CHIUSURA.Value.ToString("dd/MM/yyyy"),
                    Open = query.CHA_STATO == "A",
                    OpeningDate = query.DTA_APERTURA.Value.ToString("dd/MM/yyyy"),
                    CreationDate = query.DTA_CREAZIONE.Value.ToString("dd/MM/yyyy"),
                    CollocationDate = query.DTA_UO_LF == null ? null : query.DTA_UO_LF.Value.ToString("dd/MM/yyyy"),
                    Type = query.CHA_TIPO_FASCICOLO,
                    Number = query.NUM_FASCICOLO > 0 ? query.NUM_FASCICOLO.ToString() : null,
                    Controlled = query.CHA_CONTROLLATO == "1",
                    IdParent = query.ID_PARENT.ToString(),
                    Paper = query.CARTACEO == "1",
                    Private = query.CHA_PRIVATO == "1"
                };
            }


            return retval;

        }

        public static Project getProjectLiteByCodeAndClassId(string codeProject, string idClassificationScheme, IPi3DbContext dbContext)
        {
            Project retval = null;

            var query = (from a in dbContext.ProjectEntities
                         where a.VAR_CODICE == codeProject && a.ID_TITOLARIO == idClassificationScheme.AsLong()
                         select a).FirstOrDefault();
            if (query != null && query.SYSTEM_ID > 0)
            {
                retval = new Project()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.DESCRIPTION,
                    Code = query.VAR_CODICE,
                    ClosureDate = query.DTA_CHIUSURA == null ? null : query.DTA_CHIUSURA.Value.ToString("dd/MM/yyyy"),
                    Open = query.CHA_STATO == "A",
                    OpeningDate = query.DTA_APERTURA.Value.ToString("dd/MM/yyyy"),
                    CreationDate = query.DTA_CREAZIONE.Value.ToString("dd/MM/yyyy"),
                    CollocationDate = query.DTA_UO_LF == null ? null : query.DTA_UO_LF.Value.ToString("dd/MM/yyyy"),
                    Type = query.CHA_TIPO_FASCICOLO,
                    Number = query.NUM_FASCICOLO > 0 ? query.NUM_FASCICOLO.ToString() : null,
                    Controlled = query.CHA_CONTROLLATO == "1",
                    IdParent = query.ID_PARENT.ToString(),
                    Paper = query.CARTACEO == "1",
                    Private = query.CHA_PRIVATO == "1"
                };
            }


            return retval;

        }

        public static Template getTemplateFromProjectId(string projectId, IPi3DbContext dbContext)
        {
            var projectIdAsLong = projectId.AsLong();

            Template retval = null;

            var tipologia = (from a in dbContext.TipoFascEntities
                             join b in dbContext.ProjectEntities on a.SYSTEM_ID equals b.ID_TIPO_FASC
                             where b.SYSTEM_ID == projectIdAsLong
                             select new { idTipoFasc = a.SYSTEM_ID, descTipoFasc = a.VAR_DESC_FASC }).FirstOrDefault();
            if (tipologia != null && tipologia.idTipoFasc > 0)
            {
                retval = new Template()
                {
                    Id = tipologia.idTipoFasc.ToString(),
                    Name = tipologia.descTipoFasc,
                    Type = "F"
                };


                var query = from a in dbContext.AssTemplatesFascEntities
                            join c in dbContext.OggettiCustomFascEntities on a.ID_OGGETTO equals c.SYSTEM_ID
                            join e in dbContext.TipoOggettoFascEntities on c.ID_TIPO_OGGETTO equals e.SYSTEM_ID
                            where a.ID_PROJECT == projectId
                            select new
                            {
                                idOggetto = c.SYSTEM_ID,
                                descOggetto = c.DESCRIZIONE,
                                obbligatorio = c.CAMPO_OBBLIGATORIO,
                                valore = a.VALORE_OGGETTO_DB,
                                tipooggetto = e.DESCRIZIONE,
                                tipoContatore = c.CHA_TIPO_TAR,
                                idRegistro = a.ID_AOO_RF
                                // ,scrittura = d.INS_MOD,
                                // lettura = d.VIS
                            };

                if (query != null && query.Any())
                {
                    List<Field> campi = new List<Field>();
                    Field campo = null;
                    Dictionary<string, List<string>> selezioneMultipla = new Dictionary<string, List<string>>();

                    foreach (var ogg in query)
                    {
                        campo = new Field();
                        campo.Id = ogg.idOggetto.ToString();
                        campo.Name = ogg.descOggetto;
                        campo.Required = ogg.obbligatorio == "SI";

                        //if (ogg.scrittura == 1) campo.Rights = "INSERT_AND_MODIFY";
                        //else if (ogg.lettura == 1) campo.Rights = "VIEW";
                        //else campo.Rights = "NONE";
                        switch (ogg.tipooggetto)
                        {
                            case "Corrispondente":
                                campo.Value = ogg.valore;
                                campo.Type = "Correspondent";
                                break;
                            case "CampoDiTesto":
                                campo.Value = ogg.valore;
                                campo.Type = "TextField";
                                break;
                            case "SelezioneEsclusiva":
                                campo.Value = ogg.valore;
                                campo.Type = "ExclusiveSelection";
                                break;
                            case "MenuATendina":
                                campo.Value = ogg.valore;
                                campo.Type = "DropDown";
                                break;
                            case "Data":
                                campo.Value = ogg.valore;
                                campo.Type = "Date";
                                break;
                            case "Contatore":
                                campo.Value = ogg.valore;
                                campo.Type = "Counter";
                                if (ogg.idRegistro != null && ogg.idRegistro > 0) campo.CodeRegisterOrRF = (from a in dbContext.RegistroEntities where a.SYSTEM_ID == ogg.idRegistro select a.VAR_CODICE).FirstOrDefault();
                                break;
                            case "ContatoreSottocontatore":
                                campo.Value = ogg.valore;
                                campo.Type = "SubCounter";
                                if (ogg.idRegistro != null && ogg.idRegistro > 0) campo.CodeRegisterOrRF = (from a in dbContext.RegistroEntities where a.SYSTEM_ID == ogg.idRegistro select a.VAR_CODICE).FirstOrDefault();
                                break;
                            case "CasellaDiSelezione":
                                campo.Type = "MultipleChoise";
                                // I valori multipli vengono associati come righe della associazione templates
                                if (selezioneMultipla.ContainsKey(ogg.idOggetto.ToString()))
                                {
                                    selezioneMultipla[ogg.idOggetto.ToString()].Add(ogg.valore);
                                }
                                else
                                {
                                    selezioneMultipla.Add(ogg.idOggetto.ToString(), new List<string>());
                                    selezioneMultipla[ogg.idOggetto.ToString()].Add(ogg.valore);
                                }

                                break;
                            case "Link":
                                campo.Value = ogg.valore;
                                campo.Type = "Link";
                                break;
                            case "Separatore":
                                campo.Type = "Divider";
                                break;
                        }
                        campo.Value = campo.Value ?? string.Empty;
                        if (!((campi.Where(x => x.Id == campo.Id).Select(x => x)).Any()))
                            campi.Add(campo);
                    }

                    if (selezioneMultipla.Any())
                    {
                        foreach (var c in campi)
                        {
                            if (selezioneMultipla.ContainsKey(c.Id))
                            {
                                c.MultipleChoice = selezioneMultipla[c.Id].ToArray();
                            }
                        }
                    }

                    retval.Fields = campi.ToArray();
                }

                retval.StateDiagram = getStateDiagramFromProjectId(projectId, dbContext);
            }

            return retval;
        }

        public static StateDiagram getStateDiagramFromProjectId(string projectId, IPi3DbContext dbContext)
        {
            var projectIdAsLong = projectId.AsLong();
            StateDiagram retval = null;

            var query = (from a in dbContext.DiagrammiEntities
                         join b in dbContext.DiagrammiStatoEntities on a.ID_DIAGRAMMA equals b.SYSTEM_ID
                         join c in dbContext.StatoEntities on a.ID_STATO equals c.SYSTEM_ID
                         where a.ID_PROJECT == projectIdAsLong
                         select new
                         {
                             idDiagramma = b.SYSTEM_ID,
                             descDiagramma = b.VAR_DESCRIZIONE,
                             idStato = c.SYSTEM_ID,
                             descStato = c.VAR_DESCRIZIONE,
                             statoIniziale = c.STATO_INIZIALE == 1,
                             statoFinale = c.STATO_FINALE == 1
                         }).FirstOrDefault();

            if (query != null && query.idDiagramma > 0)
            {
                retval = new StateDiagram()
                {
                    Id = query.idDiagramma.ToString(),
                    Description = query.descDiagramma,
                    StateOfDiagram = new StateOfDiagram[1]
                };
                retval.StateOfDiagram[0] = new StateOfDiagram()
                {
                    Id = query.idStato.ToString(),
                    Description = query.descStato,
                    DiagramId = query.idDiagramma.ToString(),
                    InitialState = query.statoIniziale,
                    FinaleState = query.statoFinale
                };
            }

            return retval;

        }
        public static List<ArchivePlanREST> SearchArchivePlansRest(Dictionary<string, string> filters, string pagenum, string pagesize, IPi3DbContext dbContext)
        {
            var output = new List<ArchivePlanREST>();
            var pageNum = Convert.ToInt32(pagenum.AsLong());
            var pageSize = Convert.ToInt32(pagesize.AsLong());
            var baseQuery = (from a in dbContext.PianoConservazioneEntities.AsNoTracking()
                             join b in dbContext.RegistroEntities.AsNoTracking() on a.ID_REGISTRO equals b.SYSTEM_ID
                             join c in dbContext.ProjectEntities.AsNoTracking() on a.ID_TITOLARIO equals c.SYSTEM_ID
                             join d in dbContext.PianoConsAssTempoConsEntities.AsNoTracking() on a.TEMPO_CONSERVAZIONE equals d.TEMPO_CONSERVAZIONE
                             where !a.DTA_FINE.HasValue
                             orderby a.TIPOLOGIA_FASCICOLO
                             select new
                             {
                                 a.SYSTEM_ID,
                                 a.ID_AMM,
                                 a.CODICE_CLASSIFICAZIONE,
                                 a.ID_CLASSIFICAZIONE,
                                 a.VOCE_PROCEDIMENTO,
                                 a.NUMERO_PROCEDIMENTO,
                                 a.TIPOLOGIA_FASCICOLO,
                                 a.TEMPO_CONSERVAZIONE,
                                 a.NOTE_CHIUSURA_FASCICOLO,
                                 a.NOTE_SCARTABILITA_DOC,
                                 a.NOTE_DOCUMENTI,
                                 a.ID_REGISTRO,
                                 CODICE_REGISTRO = b.VAR_CODICE,
                                 DESC_REGISTRO = b.VAR_DESC_REGISTRO,
                                 STATO_REGISTRO = b.CHA_STATO,
                                 REGISTROISRF = b.CHA_RF,
                                 a.ID_TITOLARIO,
                                 DESC_TITOLARIO = c.DESCRIPTION,
                                 STATO_TITOLARIO = c.CHA_STATO,
                                 d.TEMPO_CONSERVAZIONE_IN_ANNI
                             });

            // Aggiunta condizioni
            if (filters != null && filters.Count > 0)
            {
                foreach (var f in filters)
                {
                    switch (f.Key)
                    {
                        case "CLASSIFICATION_NODE_CODE":
                            baseQuery = baseQuery.Where(o => o.CODICE_CLASSIFICAZIONE == f.Value);
                            break;
                        case "CLASSIFICATION_NODE_ID":
                            baseQuery = baseQuery.Where(o => o.ID_CLASSIFICAZIONE == f.Value.AsLong());
                            break;
                        case "ADMINISTRATION_ID":
                            baseQuery = baseQuery.Where(o => o.ID_AMM == f.Value.AsLong());
                            break;
                        case "DESCRIPTION":
                            baseQuery = baseQuery.Where(o => EF.Functions.Like(o.TIPOLOGIA_FASCICOLO.ToLower(), $"%{f.Value.ToLowerInvariant()}%"));
                            break;
                        case "CLASSIFICATION_SCHEME_ID":
                            baseQuery = baseQuery.Where(o => o.ID_TITOLARIO == f.Value.AsLong());
                            break;
                        case "REGISTER_ID":
                            baseQuery = baseQuery.Where(o => o.ID_REGISTRO == f.Value.AsLong());
                            break;
                        case "REGISTER_CODE":
                            baseQuery = baseQuery.Where(o => o.CODICE_REGISTRO == f.Value);
                            break;
                        case "ID":
                            baseQuery = baseQuery.Where(o => o.SYSTEM_ID == f.Value.AsLong());
                            break;
                        case "DOCUMENT_TEMPLATE_ID":
                            baseQuery = baseQuery.Where(o => dbContext.PianoConsTipoAttoEntities.AsNoTracking()
                            .Where(p => p.ID_TIPO_ATTO == f.Value.AsLong() && o.SYSTEM_ID == p.ID_PIANO_CONSERVAZIONE).Any());
                            break;
                        case "PROJECT_TEMPLATE_ID":
                            baseQuery = baseQuery.Where(o => dbContext.PianoConsTipoFascEntities.AsNoTracking()
                            .Where(p => p.ID_TIPO_FASC == f.Value.AsLong() && o.SYSTEM_ID == p.ID_PIANO_CONSERVAZIONE).Any());
                            break;
                        default:
                            break;
                    }
                }
            }

            // Paginazione
            baseQuery = baseQuery.Skip(pageNum * pageSize - pageSize).Take(pageSize);

            if (baseQuery.Any())
            {
                foreach (var row in baseQuery)
                {
                    output.Add(new()
                    {
                        IdAmm = row.ID_AMM != null ? row.ID_AMM.ToString() : null,
                        IdClassificazione = row.ID_CLASSIFICAZIONE != null ? row.ID_CLASSIFICAZIONE.ToString() : null,
                        CodiceClassificazione = row.CODICE_CLASSIFICAZIONE,
                        CodiceRegistro = row.CODICE_REGISTRO,
                        DescrizioneRegistro = row.DESC_REGISTRO,
                        DescTitolario = row.DESC_TITOLARIO,
                        IdRegistro = row.ID_REGISTRO != null ? row.ID_REGISTRO.ToString() : null,
                        IdTitolario = row.ID_TITOLARIO != null ? row.ID_TITOLARIO.ToString() : null,
                        NoteChiusuraFascicolo = row.NOTE_CHIUSURA_FASCICOLO,
                        NoteDocumenti = row.NOTE_DOCUMENTI,
                        NoteScartabilitaDocumenti = row.NOTE_SCARTABILITA_DOC,
                        NumeroProcedimento = row.NUMERO_PROCEDIMENTO,
                        RegistroIsRF = row.REGISTROISRF,
                        StatoRegistro = row.STATO_REGISTRO,
                        SystemId = row.SYSTEM_ID.ToString(),
                        TempoConservazione = row.TEMPO_CONSERVAZIONE,
                        TipologiaFascicolo = row.TIPOLOGIA_FASCICOLO,
                        TitolarioAttivo = row.STATO_TITOLARIO,
                        VoceProcedimento = row.VOCE_PROCEDIMENTO,
                        TempoConservazioneAnni = row.TEMPO_CONSERVAZIONE_IN_ANNI != null ? row.TEMPO_CONSERVAZIONE_IN_ANNI.ToString() : null
                    });
                }
            }
            return output;
        }
        public static ProjectWithArchivePlan GetProjectWithArchivePlanFromDB(string idProject, IPi3DbContext dbContext)
        {
            ProjectWithArchivePlan retval = null;

            var query = (from a in dbContext.ProjectEntities where a.SYSTEM_ID.ToString() == idProject select a).FirstOrDefault();

            if (query != null && query.SYSTEM_ID != 0)
            {
                retval = new ProjectWithArchivePlan()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.DESCRIPTION,
                    Code = query.VAR_CODICE,
                    ClosureDate = query.DTA_CHIUSURA == null ? null : query.DTA_CHIUSURA.Value.ToString("dd/MM/yyyy"),
                    Open = query.CHA_STATO == "A",
                    OpeningDate = query.DTA_APERTURA.Value.ToString("dd/MM/yyyy"),
                    CreationDate = query.DTA_CREAZIONE.Value.ToString("dd/MM/yyyy"),
                    CollocationDate = query.DTA_UO_LF == null ? null : query.DTA_UO_LF.Value.ToString("dd/MM/yyyy"),
                    Type = query.CHA_TIPO_FASCICOLO,
                    Number = query.NUM_FASCICOLO > 0 ? query.NUM_FASCICOLO.ToString() : null,
                    Controlled = query.CHA_CONTROLLATO == "1",
                    IdParent = query.ID_PARENT.ToString(),
                    Paper = query.CARTACEO == "1",
                    Private = query.CHA_PRIVATO == "1"

                };

                var titolario = getTitolarioById(query.ID_TITOLARIO.ToString(), dbContext);
                retval.ClassificationScheme = new ClassificationSchemes.ClassificationScheme()
                {
                    Id = titolario.ID,
                    Description = titolario.Descrizione,
                    Active = titolario.Stato == OrgStatiTitolarioEnum.Attivo
                };

                var registro = getRegistro(query.ID_REGISTRO.ToString(), dbContext);
                retval.Register = RestUtils.GetRegister(registro);

                if (query.DTA_UO_LF != null)
                {
                    retval.PhysicsCollocation = (from a in dbContext.CorrGlobaliEntities where a.SYSTEM_ID == query.ID_UO_LF select a.VAR_DESC_CORR).FirstOrDefault();
                }

                if (query.ID_PARENT > 0 && retval.Type == "P")
                {
                    retval.CodeNodeClassification = (from a in dbContext.ProjectEntities where a.SYSTEM_ID == query.ID_PARENT select a.VAR_CODICE).FirstOrDefault();
                }
                var listnote = getNoteOggetto(idProject, dbContext);
                if (listnote != null && listnote.Any())
                    retval.Note = listnote.ToArray();

                retval.Template = getTemplateFromProjectId(idProject, dbContext);

                if (query.ID_PIANO_CONSERVAZIONE != null && query.ID_PIANO_CONSERVAZIONE > 0)
                    retval.ArchivePlan = GetArchivePlanFromDB(query.ID_PIANO_CONSERVAZIONE.ToString(), dbContext);
            }
            return retval;
        }

        public static ArchivePlan GetArchivePlanFromDB(string idArchivePlan, IPi3DbContext dbContext)
        {
            ArchivePlan retval = null;

            var query = (from a in dbContext.PianoConservazioneEntities
                         join b in dbContext.RegistroEntities on a.ID_REGISTRO equals b.SYSTEM_ID
                         join c in dbContext.ProjectEntities on a.ID_TITOLARIO equals c.SYSTEM_ID
                         join d in dbContext.PianoConsAssTempoConsEntities on a.TEMPO_CONSERVAZIONE equals d.TEMPO_CONSERVAZIONE
                         where a.SYSTEM_ID == idArchivePlan.AsLong()
                         select new
                         {
                             a,
                             idregistro = b.SYSTEM_ID,
                             coderegitro = b.VAR_CODICE,
                             descRegistro = b.VAR_DESC_REGISTRO,
                             statoRegistro = b.CHA_STATO,
                             isRf = b.CHA_RF,
                             idTitolario = c.SYSTEM_ID,
                             descTitolario = c.DESCRIPTION,
                             titoAttivo = c.CHA_STATO,
                             d.TEMPO_CONSERVAZIONE_IN_ANNI
                         }).FirstOrDefault();

            if (query != null && query.a.SYSTEM_ID > 0)
            {
                retval = new ArchivePlan
                {
                    Id = query.a.SYSTEM_ID.ToString(),
                    ArchivePeriod = query.a.TEMPO_CONSERVAZIONE,
                    ArchivePeriodYears = query.TEMPO_CONSERVAZIONE_IN_ANNI.ToString(),
                    ClassificationNodeCode = query.a.CODICE_CLASSIFICAZIONE,
                    ClassificationNodeId = query.a.ID_CLASSIFICAZIONE.ToString(),
                    ClassificationScheme = new ClassificationSchemes.ClassificationScheme() { Id = query.idTitolario.ToString(), Description = query.descTitolario, Active = query.titoAttivo == "A" },
                    Description = query.a.TIPOLOGIA_FASCICOLO,
                    DocumentDiscardabilityNotes = query.a.NOTE_SCARTABILITA_DOC,
                    DocumentNotes = query.a.NOTE_DOCUMENTI,
                    ProceedingNumber = query.a.NUMERO_PROCEDIMENTO,
                    ProceedingVoice = query.a.VOCE_PROCEDIMENTO,
                    ProjectClosureNotes = query.a.NOTE_CHIUSURA_FASCICOLO,
                    Register = new Registers.Register()
                    {
                        Code = query.coderegitro,
                        Id = query.idregistro.ToString(),
                        Description = query.descRegistro,
                        State = query.statoRegistro,
                        IsRF = query.isRf == "1"
                    }
                };
            }

            return retval;
        }

        public static List<Projects.Folder> getProjectFolders(string idproject, IPi3DbContext dbContext)
        {
            List<Projects.Folder> retval = null;

            var query = from a in dbContext.ProjectEntities
                        where a.ID_FASCICOLO == idproject.AsLong()
                        orderby a.SYSTEM_ID
                        select a;
            if (query != null && query.Any())
            {
                retval = new List<Projects.Folder>();
                foreach (var f in query)
                {
                    retval.Add(new Projects.Folder()
                    {
                        Id = f.SYSTEM_ID.ToString(),
                        Description = f.DESCRIPTION,
                        IdProject = f.ID_FASCICOLO.ToString(),
                        IdParent = f.ID_PARENT.ToString(),
                        CreationDate = f.DTA_APERTURA != null ? f.DTA_APERTURA.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
                    });
                }
            }

            return retval;

        }

        public static StateOfDiagram getStatoDiagrammaFascicolo(string idFascicolo, IPi3DbContext dbContext)
        {
            StateOfDiagram retval = null;
            var query = (from adiag in dbContext.DiagrammiEntities
                         join bstat in dbContext.StatoEntities on adiag.ID_STATO equals bstat.SYSTEM_ID
                         where adiag.ID_PROJECT == idFascicolo.AsLong()
                         select bstat).FirstOrDefault();

            if (query != null && query.SYSTEM_ID > 0)
            {
                retval = new StateOfDiagram()
                {
                    Id = query.SYSTEM_ID.ToString(),
                    Description = query.VAR_DESCRIZIONE,
                    DiagramId = query.ID_DIAGRAMMA.ToString(),
                    InitialState = query.STATO_INIZIALE == 1,
                    FinaleState = query.STATO_FINALE == 1
                };
            }
            return retval;
        }

        public static bool OpenProject(string idProject, IPi3DbContext dbContext)
        {
            bool retval = false;
            try
            {
                var fascicolo = (from a in dbContext.ProjectEntities where a.SYSTEM_ID == idProject.AsLong() select a).FirstOrDefault();
                if (fascicolo != null)
                {
                    fascicolo.CHA_STATO = "A";
                    fascicolo.DTA_APERTURA = DateTime.Now;
                    fascicolo.DTA_CHIUSURA = null;
                    fascicolo.ID_AUTHOR_CHIUSURA = null;
                    fascicolo.ID_UO_CHIUSURA = null;
                    fascicolo.ID_RUOLO_CHIUSURA = null;
                    ((DbContext)dbContext).SaveChanges();
                    retval = true;
                }
                else retval = false;
            }
            catch (Exception ex) { retval = false; }
            return retval;
        }

        public static bool CloseProject(string idProject, InfoUtente infoUt, IPi3DbContext dbContext)
        {
            bool retval = false;
            try
            {
                var fascicolo = (from a in dbContext.ProjectEntities where a.SYSTEM_ID == idProject.AsLong() select a).FirstOrDefault();
                if (fascicolo != null)
                {
                    fascicolo.CHA_STATO = "C";
                    fascicolo.DTA_CHIUSURA = DateTime.Now;
                    fascicolo.ID_AUTHOR_CHIUSURA = infoUt.idPeople.AsLong();
                    var ruolo = getRuoloById(infoUt.idCorrGlobali, dbContext);
                    fascicolo.ID_UO_CHIUSURA = ruolo.uo.systemId.AsLong();
                    fascicolo.ID_RUOLO_CHIUSURA = ruolo.systemId.AsLong();
                    ((DbContext)dbContext).SaveChanges();
                    retval = true;
                }
                else retval = false;

            }
            catch (Exception ex) { retval = false; }
            return retval;
        }

        public static List<Project> GetFascicoloDaCodiceConSecurity(string codeProject, long idAmministrazione, long classificationSchemeId, InfoUtente infoUtente, IPi3DbContext dbContext)
        {

            List<Projects.Project> fascicoli = null;
            List<long> ids = new List<long>() { infoUtente.idGruppo.AsLong(), infoUtente.idPeople.AsLong() };
            // 1. Base Query (Filtraggio)
            var filteredProjects = dbContext.ProjectEntities.Where(x =>
                x.VAR_CODICE.Equals(codeProject) &&
                x.ID_AMM == idAmministrazione &&
                x.ID_TITOLARIO == classificationSchemeId
            );

            // 2. Filtro di Sicurezza
            filteredProjects = filteredProjects.Where(x =>
                dbContext.SecurityEntities.Any(e =>
                    e.THING == x.SYSTEM_ID &&
                    ids.Contains((long)e.PERSONORGROUP) &&
                    e.ACCESSRIGHTS > 0
                )
            );

            // 3. Ordinamento
            filteredProjects = filteredProjects.OrderBy(x => x.VAR_COD_LIV1);

            // 4. Selezione (Proiezione)
            var result = filteredProjects.Select(x => new Projects.Project
            {
                Id = x.SYSTEM_ID.ToString(),
                Code = x.VAR_CODICE,
                Description = x.DESCRIPTION,
                ClassificationScheme = new ClassificationSchemes.ClassificationScheme()
                {
                    Id = x.ID_TITOLARIO.ToString()
                }
            });

            // 5. Conversione in Lista
            fascicoli = result.ToList();


            return fascicoli;
        }

        public static async Task<List<Documents.Document>> SearchDocumentsInFolder(Projects.Folder objFolder, InfoUtente infoUt, int numPage, int pageSize, IPi3DbContext dbContext)
        {
            List<Documents.Document> listaDocumenti = null;
            if (string.IsNullOrEmpty(objFolder.Id))
                throw new Exception("ID folder in GetDocumentiPagingCustom fascicolo è vuoto.");

            /*
            var docs = null; //TODO query 

            foreach (var d in docs)
            {
                var doc = new Documents.Document()
                {

                };

                listaDocumenti.Add(doc);
            }
            */
            return listaDocumenti;
        }
        #endregion

        #region Titolari
        public static ArrayList getTitolariUtilizzabili(string idAmm, IPi3DbContext dbContext)
        {
            ArrayList retval = null;

            var projs = (from prj in dbContext.ProjectEntities
                         where prj.ID_AMM == idAmm.AsLong() &&
                                prj.ID_TITOLARIO == 0 &&
                                prj.ID_PARENT == 0 &&
                                prj.CHA_STATO != "D"
                         orderby prj.CHA_STATO, prj.DTA_CESSAZIONE descending
                         select prj);

            if (projs != null && projs.Any())
            {
                retval = new ArrayList();
                foreach (var prj in projs)
                {
                    retval.Add(RestUtils.CreateTitolario(prj));
                }
            }
            return retval;
        }

        public static OrgTitolario getTitolarioById(string idTitolario, IPi3DbContext dbContext)
        {
            OrgTitolario retval = null;

            var proj = (from prj in dbContext.ProjectEntities
                        where prj.SYSTEM_ID == idTitolario.AsLong() &&
                            prj.ID_TITOLARIO == 0 && prj.ID_PARENT == 0
                        select prj).FirstOrDefault();
            if (proj != null && proj.SYSTEM_ID > 0)
            {
                retval = RestUtils.CreateTitolario(proj);
            }

            return retval;
        }
        #endregion

        #region Registri
        public static DocsPaVO.utente.Registro getRegistroByCodAOO(string codiceRegistro, string idAmm, IPi3DbContext dbContext)
        {
            DocsPaVO.utente.Registro retval = null;
            var regdb = (from reg in dbContext.RegistroEntities
                         where reg.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper() && reg.ID_AMM == idAmm.AsLong()
                         select reg
                          ).FirstOrDefault();
            if (regdb != null && regdb.SYSTEM_ID > 0)
            {
                retval = RestUtils.CreateRegistro(regdb);
            }

            return retval;
        }

        public static DocsPaVO.utente.Registro getRegistro(string idRegistro, IPi3DbContext dbContext)
        {
            var idRegistroAsLong = idRegistro.AsLong();

            DocsPaVO.utente.Registro retval = null;
            var regdb = (from reg in dbContext.RegistroEntities
                         where reg.SYSTEM_ID == idRegistroAsLong
                         select reg
                          ).FirstOrDefault();
            if (regdb != null && regdb.SYSTEM_ID > 0)
            {
                retval = RestUtils.CreateRegistro(regdb);
            }
            return retval;
        }

        public static ArrayList getListaRegistriRfRuolo(string idRuolo, string isRF, IPi3DbContext dbContext)
        {
            ArrayList retval = new ArrayList();
            IQueryable<RegistroEntity> regdb = null;

            var idRuoloAsLong = idRuolo.AsLong();

            if (!string.IsNullOrEmpty(isRF))
            {
                regdb = (from a in dbContext.RegistroEntities
                         join b in dbContext.RuoloRegistroEntities on a.SYSTEM_ID equals b.ID_REGISTRO
                         where b.ID_RUOLO_IN_UO == idRuoloAsLong && a.CHA_RF == isRF
                         select a);
            }
            else
            {
                regdb = (from a in dbContext.RegistroEntities
                         join b in dbContext.RuoloRegistroEntities on a.SYSTEM_ID equals b.ID_REGISTRO
                         where b.ID_RUOLO_IN_UO == idRuoloAsLong
                         select a);
            }
            if (regdb != null && regdb.Any())
            {
                foreach (RegistroEntity reg in regdb)
                {
                    retval.Add(RestUtils.CreateRegistro(reg));
                }
            }

            return retval;
        }

        public static string getFormatoSegnatura(string idAmm, IPi3DbContext dbContext)
        {
            string formatoSegnatura = "";

            var query = (from a in dbContext.AmministraEntities
                         where a.SYSTEM_ID == idAmm.AsLong()
                         select a.VAR_FORMATO_SEGNATURA).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(query)) { formatoSegnatura = query; }

            return formatoSegnatura;
        }

        public static string GetSeparatore(string idAmm, IPi3DbContext dbContext)
        {
            string separatore = "";

            var query = (from a in dbContext.AmministraEntities
                         where a.SYSTEM_ID == idAmm.AsLong()
                         select a.CHA_SEPARATORE).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(query)) { separatore = query; }

            return separatore;
        }

        #endregion

        #region Tipologie e campi profilati
        public static StateOfDiagram GetStateOfDiagram(DocsPaVO.DiagrammaStato.Stato stato, string idDiagramma)
        {
            StateOfDiagram result = new StateOfDiagram();
            if (stato != null)
            {
                result.Description = stato.DESCRIZIONE;
                result.DiagramId = idDiagramma;
                result.FinaleState = stato.STATO_FINALE;
                result.Id = stato.SYSTEM_ID.ToString();
                result.InitialState = stato.STATO_INIZIALE;
            }
            else
            {
                result = null;
            }
            return result;
        }
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateFromPis(Template templatePis, DocsPaVO.ProfilazioneDinamica.Templates template, bool search, InfoUtente infoUtente, IPi3DbContext dbContext)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = new DocsPaVO.ProfilazioneDinamica.Templates();

            if (templatePis == null || template == null)
            {
                result = null;
            }
            else
            {
                result.DESCRIZIONE = templatePis.Name;
                result.SYSTEM_ID = Int32.Parse(templatePis.Id);

                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                       template.ELENCO_OGGETTI;


                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                {
                    Field campo = templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                    // Impostazione del valore
                    if (campo != null && (string.IsNullOrEmpty(campo.Value) || ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione") && (campo.MultipleChoice == null || campo.MultipleChoice.Length == 0))))
                    {

                        if (campo.Required && !search)
                        {
                            //Campo obbligatorio
                            throw new RestException("FIELD_REQUIRED");
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }

                        //Nel caso di contatorei che non sono di registor o rf posso far scattare il contatore senza valore
                        if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")) && (!oggettoCustom.TIPO_CONTATORE.Equals("A") && !oggettoCustom.TIPO_CONTATORE.Equals("R")))
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                    }
                    else
                    {
                        if (campo != null)
                        {
                            oggettoCustom.VALORE_DATABASE = campo.Value;
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                        //Passo come valore il codice del Registro rf del contatore

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                        {
                            //Aggiunta per contatore
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                if (campo != null && !string.IsNullOrEmpty(campo.CodeRegisterOrRF))
                                {
                                    DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(campo.CodeRegisterOrRF, infoUtente.idAmministrazione, dbContext);

                                    if (reg != null)
                                    {
                                        oggettoCustom.ID_AOO_RF = reg.systemId;
                                    }
                                    else
                                    {
                                        //Registro|RF mancante
                                        throw new RestException("REGISTER_NOT_FOUND");
                                    }
                                }
                                else
                                {
                                    if (!search)
                                        //Id registro o rf mancante
                                        throw new RestException("REQUIRED_ID_REGISTER");
                                }

                            }

                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }


                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                        {
                            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count(); i++)
                            {
                                if (campo != null)
                                {
                                    foreach (string word in campo.MultipleChoice)
                                    {
                                        if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                        {
                                            oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result = template;
            }

            return template;
        }

        public static List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> GetDirittiCampiTipologiaDoc(string idRuolo, string idTemplate, IPi3DbContext pi3DbContext)
        {
            var query = pi3DbContext.AROggCustomDocEntities.AsNoTracking().Where(a => a.ID_RUOLO == idRuolo.AsLong() && a.ID_TEMPLATE == idTemplate.AsLong());
            List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> output = new();
            if (query.Any())
            {
                foreach (var diritti in query)
                {
                    var assDocFascRuoli = RestUtils.CreateAssDocFascRuoli(diritti);
                    if (assDocFascRuoli != null)
                        output.Add(assDocFascRuoli);
                }
            }
            else
            {
                var template = DBUtils.GetTemplateById(idTemplate, pi3DbContext);

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    assDocFascRuoli.ID_GRUPPO = idRuolo;
                    assDocFascRuoli.ID_TIPO_DOC_FASC = idTemplate;
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                    if (!String.IsNullOrEmpty(template.IPER_FASC_DOC) && template.IPER_FASC_DOC.Equals("1"))
                        assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                    else
                        assDocFascRuoli.VIS_OGG_CUSTOM = "0";
                    assDocFascRuoli.ANNULLA_REPERTORIO = "0";
                    output.Add(assDocFascRuoli);
                }
            }
            return output;
        }

        public static List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> GetDirittiCampiTipologiaFasc(string idRuolo, string idTemplate, IPi3DbContext pi3DbContext)
        {
            var query = pi3DbContext.AROggCustomFascEntityEntities.AsNoTracking().Where(
                e => e.ID_TEMPLATE == idTemplate.AsLong() && e.ID_RUOLO == idRuolo.AsLong()
                );
            List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> output = new();

            if (query.Any())
            {
                foreach (var diritti in query)
                {
                    var assDocFascRuoli = RestUtils.CreateAssDocFascRuoliFasc(diritti);
                    if (assDocFascRuoli != null)
                    {
                        output.Add(assDocFascRuoli);
                    }
                }
            }
            else
            {
                var template = DBUtils.GetTemplateById(idTemplate, pi3DbContext);
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    assDocFascRuoli.ID_GRUPPO = idRuolo;
                    assDocFascRuoli.ID_TIPO_DOC_FASC = idTemplate;
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                    if (!String.IsNullOrEmpty(template.IPER_FASC_DOC) && template.IPER_FASC_DOC.Equals("1"))
                        assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                    else
                        assDocFascRuoli.VIS_OGG_CUSTOM = "0";
                    output.Add(assDocFascRuoli);
                }
            }

            return output;
        }
        public static List<TemplateLite> getListTemplatesLiteByRole(string idAmm, string idGruppo, IPi3DbContext dbContext)
        {
            List<TemplateLite> retval = null;

            var query = (from vis in dbContext.VisTipoDocEntities
                         join d in dbContext.TipoAttoEntities on vis.ID_TIPO_DOC equals d.SYSTEM_ID
                         where d.ID_AMM == idAmm.AsLong() && d.ABILITATO_SI_NO == 1 && vis.ID_RUOLO == idGruppo.AsLong() && vis.DIRITTI != 0
                         orderby d.VAR_DESC_ATTO
                         select d).Distinct();

            if (query != null && query.Any())
            {
                retval = new List<TemplateLite>();
                foreach (var d in query)
                {
                    retval.Add(new TemplateLite() { system_id = d.SYSTEM_ID.ToString(), name = d.VAR_DESC_ATTO });
                }
            }

            return retval;
        }

        public static List<TemplateLite> GetListTemplatesLiteByRoleWithReadWriteRights(string idAmm, string idGruppo, IPi3DbContext dbContext)
        {
            List<TemplateLite> retval = null;

            var query = (from vis in dbContext.VisTipoDocEntities.AsNoTracking()
                         join d in dbContext.TipoAttoEntities.AsNoTracking() on vis.ID_TIPO_DOC equals d.SYSTEM_ID
                         where d.ID_AMM == idAmm.AsLong() && d.ABILITATO_SI_NO == 1 && vis.ID_RUOLO == idGruppo.AsLong() && vis.DIRITTI == 2
                         orderby d.VAR_DESC_ATTO
                         select d).Distinct();

            if (query != null && query.Any())
            {
                retval = new List<TemplateLite>();
                foreach (var d in query)
                {
                    retval.Add(new TemplateLite() { system_id = d.SYSTEM_ID.ToString(), name = d.VAR_DESC_ATTO });
                }
            }

            return retval;
        }

        //PD_GET_RUOLI_TIPO_FASC
        public static List<TemplateLite> GetListTemplatesLiteByRoleWithReadWriteRightsFasc(string idAmm, string idGruppo, IPi3DbContext dbContext)
        {
            List<TemplateLite> retval = null;

            var query = (from vis in dbContext.VisTipoFascEntities.AsNoTracking()
                         join d in dbContext.TipoFascEntities.AsNoTracking() on vis.ID_TIPO_FASC equals d.SYSTEM_ID
                         where d.ID_AMM == idAmm.AsLong() && d.ABILITATO_SI_NO == 1 && vis.ID_RUOLO == idGruppo.AsLong() && vis.DIRITTI == 2
                         orderby d.VAR_DESC_FASC
                         select d).Distinct();

            if (query != null && query.Any())
            {
                retval = new List<TemplateLite>();
                foreach (var d in query)
                {
                    retval.Add(new TemplateLite() { system_id = d.SYSTEM_ID.ToString(), name = d.VAR_DESC_FASC });
                }
            }

            return retval;
        }

        public static string GetRightsCustomObjectDoc(IPi3DbContext dbContext, long idTemplate, string idGruppo, long customObjectSid)
        {
            var idGruppoAsLong = idGruppo.AsLong();

            string rights = string.Empty;
            var ogg = dbContext.AROggCustomDocEntities.AsNoTracking()
                .Where(x => x.ID_OGGETTO_CUSTOM == customObjectSid &&
                x.ID_TEMPLATE == idTemplate &&
                x.ID_RUOLO == idGruppoAsLong)
                .FirstOrDefault();

            if (ogg != null)
            {
                if (ogg.INS_MOD == 1) rights = "INSERT_AND_MODIFY";
                else if (ogg.VIS == 1) rights = "VIEW";
                else rights = "NONE";
            }

            return rights;
        }
        public static string GetFieldTypeFromCustomObjDoc(IPi3DbContext dbContext, OggettoCustom obj)
        {
            string type = string.Empty;

            var tipoOgg = dbContext.TipoOggettoEntities.AsNoTracking().Where(t => t.SYSTEM_ID == obj.TIPO.SYSTEM_ID).Select(t => t.DESCRIZIONE).FirstOrDefault() ?? string.Empty;
            switch (tipoOgg)
            {
                case "Corrispondente":
                    type = "Correspondent";
                    break;
                case "CampoDiTesto":
                    type = "TextField";
                    break;
                case "SelezioneEsclusiva":
                    type = "ExclusiveSelection";
                    break;
                case "MenuATendina":
                    type = "DropDown";
                    break;
                case "Data":
                    type = "Date";
                    break;
                case "Contatore":
                    type = "Counter";
                    break;
                case "ContatoreSottocontatore":
                    type = "SubCounter";
                    break;
                case "CasellaDiSelezione":
                    type = "MultipleChoise";

                    break;
                case "Link":
                    type = "Link";
                    break;
                case "Separatore":
                    type = "Divider";
                    break;
            }
            return type;
        }
        public static List<TemplateLite> getListTemplatesLiteFascByRole(string idAmm, string idGruppo, IPi3DbContext dbContext)
        {
            List<TemplateLite> retval = null;

            var query = (from vis in dbContext.VisTipoFascEntities
                         join d in dbContext.TipoFascEntities on vis.ID_TIPO_FASC equals d.SYSTEM_ID
                         where d.ID_AMM == idAmm.AsLong() && d.ABILITATO_SI_NO == 1 && vis.ID_RUOLO == idGruppo.AsLong() && vis.DIRITTI != 0
                         orderby d.VAR_DESC_FASC
                         select d).Distinct();

            if (query != null && query.Any())
            {
                retval = new List<TemplateLite>();
                foreach (var d in query)
                {
                    retval.Add(new TemplateLite() { system_id = d.SYSTEM_ID.ToString(), name = d.VAR_DESC_FASC });
                }
            }

            return retval;
        }

        public static Template GetProjectTemplateByIdTemplate(string idTemplate, string idGruppo, IPi3DbContext dbContext)
        {
            var idTemplateAsLong = idTemplate.AsLong();

            Template retval = null;
            var tipologia = (from a in dbContext.TipoFascEntities
                             from b in dbContext.VisTipoFascEntities.Where(x => x.ID_TIPO_FASC == a.SYSTEM_ID && x.ID_RUOLO == idGruppo.AsLong()).DefaultIfEmpty()
                             where a.SYSTEM_ID == idTemplateAsLong
                             select new
                             {
                                 idTipofasc = a.SYSTEM_ID,
                                 descTipoFasc = a.VAR_DESC_FASC,
                                 diritti = b.DIRITTI
                             }).FirstOrDefault();

            if (tipologia == null || tipologia.diritti == null || tipologia.diritti < 1) return null;

            if (tipologia != null && tipologia.idTipofasc > 0)
            {
                retval = new Template()
                {
                    Id = tipologia.idTipofasc.ToString(),
                    Name = tipologia.descTipoFasc,
                    Type = "F"
                };

                var query = from a in dbContext.AssTemplatesFascEntities
                            join b in dbContext.OggettiCustomFascEntities on a.ID_OGGETTO equals b.SYSTEM_ID
                            join c in dbContext.TipoOggettoFascEntities on b.ID_TIPO_OGGETTO equals c.SYSTEM_ID
                            from d in dbContext.AROggCustomFascEntityEntities
                                        .Where(x => x.ID_OGGETTO_CUSTOM == b.SYSTEM_ID &&
                                                x.ID_TEMPLATE == a.ID_TEMPLATE &&
                                                x.ID_RUOLO == idGruppo.AsLong())
                                        .DefaultIfEmpty()
                            where a.ID_TEMPLATE == idTemplate.AsLong() && a.ID_PROJECT == null
                            select new
                            {
                                idOggetto = b.SYSTEM_ID,
                                descOggetto = b.DESCRIZIONE,
                                obbligatorio = b.CAMPO_OBBLIGATORIO,
                                valore = "",
                                tipooggetto = c.DESCRIZIONE,
                                tipoContatore = b.CHA_TIPO_TAR
                                ,
                                scrittura = d.INS_MOD,
                                lettura = d.VIS
                            };
                if (query != null && query.Any())
                {
                    List<Field> campi = new List<Field>();
                    Field campo = null;
                    Dictionary<string, List<string>> selezioneMultipla = new Dictionary<string, List<string>>();
                    List<string> valori = null;
                    foreach (var ogg in query)
                    {
                        valori = new List<string>();
                        campo = new Field();
                        campo.Id = ogg.idOggetto.ToString();
                        campo.Name = ogg.descOggetto;
                        campo.Required = ogg.obbligatorio == "SI";
                        if (ogg.scrittura == 1) campo.Rights = "INSERT_AND_MODIFY";
                        else if (ogg.lettura == 1) campo.Rights = "VIEW";
                        else campo.Rights = "NONE";
                        switch (ogg.tipooggetto)
                        {
                            case "Corrispondente":
                                campo.Value = ogg.valore;
                                campo.Type = "Correspondent";
                                break;
                            case "CampoDiTesto":
                                campo.Value = ogg.valore;
                                campo.Type = "TextField";
                                break;
                            case "SelezioneEsclusiva":
                                campo.Value = ogg.valore;
                                campo.Type = "ExclusiveSelection";
                                valori = (from a in dbContext.AssValoriFascEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "MenuATendina":
                                campo.Value = ogg.valore;
                                campo.Type = "DropDown";
                                valori = (from a in dbContext.AssValoriFascEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "Data":
                                campo.Value = ogg.valore;
                                campo.Type = "Date";
                                break;
                            case "Contatore":
                                campo.Value = ogg.valore;
                                campo.Type = "Counter";
                                break;
                            case "ContatoreSottocontatore":
                                campo.Value = ogg.valore;
                                campo.Type = "SubCounter";
                                break;
                            case "CasellaDiSelezione":
                                campo.Type = "MultipleChoise";
                                valori = (from a in dbContext.AssValoriFascEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();

                                break;
                            case "Link":
                                campo.Value = ogg.valore;
                                campo.Type = "Link";
                                break;
                            case "Separatore":
                                campo.Type = "Divider";
                                break;
                        }
                        if (!((campi.Where(x => x.Id == campo.Id).Select(x => x)).Any()))
                            campi.Add(campo);
                    }
                    retval.Fields = campi.ToArray();
                }

                retval.StateDiagram = GetProjectDiagramByIdTemplate(idTemplate, dbContext);

            }
            return retval;
        }
        public static long GetDiagrammaAssociato(string idTipoDoc, IPi3DbContext dbContext)
        {
            var output = 0;

            try
            {
                var idTipoDocAsLong = idTipoDoc.AsLong();

                var idDiagramma = dbContext.AssDiagrammiEntities.AsNoTracking()
                    .Where(d => d.ID_TIPO_DOC == idTipoDocAsLong && d.ID_DIAGRAMMA != null)
                    .Select(d => d.ID_DIAGRAMMA)
                    .FirstOrDefault();

                if (idDiagramma != null)
                    output = Convert.ToInt32(idDiagramma);
            }
            catch (Exception ex)
            {
                output = 0;
            }

            return output;
        }

        public static StateDiagram GetProjectDiagramByIdTemplate(string idTemplate, IPi3DbContext dbContext)
        {
            StateDiagram retval = null;
            var query = (from a in dbContext.AssDiagrammiEntities
                         join b in dbContext.DiagrammiStatoEntities on a.ID_DIAGRAMMA equals b.SYSTEM_ID
                         join c in dbContext.StatoEntities on b.SYSTEM_ID equals c.ID_DIAGRAMMA
                         where a.ID_TIPO_FASC == idTemplate.AsLong()
                         select new
                         {
                             idDiagramma = b.SYSTEM_ID,
                             descDiagramma = b.VAR_DESCRIZIONE,
                             idStato = c.SYSTEM_ID,
                             descStato = c.VAR_DESCRIZIONE,
                             statoIniziale = c.STATO_INIZIALE == 1,
                             statoFinale = c.STATO_FINALE == 1
                         });
            if (query != null && query.Any())
            {
                retval = new StateDiagram();
                var diagramma = query.FirstOrDefault();
                if (diagramma != null)
                {

                    retval.Description = diagramma.descDiagramma;
                    retval.Id = diagramma.idDiagramma.ToString();

                }
                List<StateOfDiagram> stati = new List<StateOfDiagram>();
                foreach (var stato in query)
                {
                    stati.Add(new StateOfDiagram()
                    {
                        Id = stato.idStato.ToString(),
                        Description = stato.descStato,
                        DiagramId = stato.idDiagramma.ToString(),
                        InitialState = stato.statoIniziale,
                        FinaleState = stato.statoFinale
                    });
                }
                retval.StateOfDiagram = stati.ToArray();
            }

            return retval;
        }

        public static Template GetDocumentTemplateByIdTemplate(string idTemplate, string idGruppo, IPi3DbContext dbContext, bool checkVis = true)
        {
            Template retval = null;
            var tipologia = (from a in dbContext.TipoAttoEntities
                             from b in dbContext.VisTipoDocEntities.Where(x => x.ID_TIPO_DOC == a.SYSTEM_ID && x.ID_RUOLO == idGruppo.AsLong()).DefaultIfEmpty()
                             where a.SYSTEM_ID == idTemplate.AsLong()
                             select new
                             {
                                 idTipoDoc = a.SYSTEM_ID,
                                 descTipoDoc = a.VAR_DESC_ATTO,
                                 diritti = b.DIRITTI
                             }).FirstOrDefault();

            if (tipologia == null || (checkVis ? (tipologia.diritti == null || tipologia.diritti < 1) : false)) return null;

            if (tipologia != null && tipologia.idTipoDoc > 0)
            {
                retval = new Template()
                {
                    Id = tipologia.idTipoDoc.ToString(),
                    Name = tipologia.descTipoDoc,
                    Type = "D"
                };

                var query = from a in dbContext.AssociazioneTemplatesEntities
                            join b in dbContext.OggettiCustomEntities on a.ID_OGGETTO equals b.SYSTEM_ID
                            join c in dbContext.TipoOggettoEntities on b.ID_TIPO_OGGETTO equals c.SYSTEM_ID
                            from d in dbContext.AROggCustomDocEntities
                                .Where(x => x.ID_OGGETTO_CUSTOM == b.SYSTEM_ID &&
                                    x.ID_TEMPLATE == a.ID_TEMPLATE &&
                                    x.ID_RUOLO == idGruppo.AsLong()).DefaultIfEmpty()
                            where a.ID_TEMPLATE == idTemplate.AsLong() && a.DOC_NUMBER == null
                            select new
                            {
                                idOggetto = b.SYSTEM_ID,
                                descOggetto = b.DESCRIZIONE,
                                obbligatorio = b.CAMPO_OBBLIGATORIO,
                                valore = "",
                                tipooggetto = c.DESCRIZIONE,
                                tipoContatore = b.CHA_TIPO_TAR
                                ,
                                scrittura = d.INS_MOD,
                                lettura = d.VIS
                            };
                if (query != null && query.Any())
                {
                    List<Field> campi = new List<Field>();
                    Field campo = null;
                    Dictionary<string, List<string>> selezioneMultipla = new Dictionary<string, List<string>>();
                    List<string> valori = null;
                    foreach (var ogg in query)
                    {
                        valori = new List<string>();
                        campo = new Field();
                        campo.Id = ogg.idOggetto.ToString();
                        campo.Name = ogg.descOggetto;
                        campo.Required = ogg.obbligatorio == "SI";
                        if (ogg.scrittura == 1) campo.Rights = "INSERT_AND_MODIFY";
                        else if (ogg.lettura == 1) campo.Rights = "VIEW";
                        else campo.Rights = "NONE";
                        switch (ogg.tipooggetto)
                        {
                            case "Corrispondente":
                                campo.Value = ogg.valore;
                                campo.Type = "Correspondent";
                                break;
                            case "CampoDiTesto":
                                campo.Value = ogg.valore;
                                campo.Type = "TextField";
                                break;
                            case "SelezioneEsclusiva":
                                campo.Value = ogg.valore;
                                campo.Type = "ExclusiveSelection";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "MenuATendina":
                                campo.Value = ogg.valore;
                                campo.Type = "DropDown";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "Data":
                                campo.Value = ogg.valore;
                                campo.Type = "Date";
                                break;
                            case "Contatore":
                                campo.Value = ogg.valore;
                                campo.Type = "Counter";
                                break;
                            case "ContatoreSottocontatore":
                                campo.Value = ogg.valore;
                                campo.Type = "SubCounter";
                                break;
                            case "CasellaDiSelezione":
                                campo.Type = "MultipleChoise";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();

                                break;
                            case "Link":
                                campo.Value = ogg.valore;
                                campo.Type = "Link";
                                break;
                            case "Separatore":
                                campo.Type = "Divider";
                                break;
                        }
                        if (!((campi.Where(x => x.Id == campo.Id).Select(x => x)).Any()))
                            campi.Add(campo);
                    }
                    retval.Fields = campi.ToArray();
                }

                retval.StateDiagram = GetDocumentDiagramByIdTemplate(idTemplate, dbContext);

            }
            return retval;
        }
        public static Template GetDocumentTemplateByDescrizione(string descAtto, string idGruppo, IPi3DbContext dbContext, bool checkVis = true)
        {
            Template retval = null;
            var tipologia = (from a in dbContext.TipoAttoEntities
                             from b in dbContext.VisTipoDocEntities.Where(x => x.ID_TIPO_DOC == a.SYSTEM_ID && x.ID_RUOLO == idGruppo.AsLong()).DefaultIfEmpty()
                             where a.VAR_DESC_ATTO.ToUpper() == descAtto.ToUpper()
                             select new
                             {
                                 idTipoDoc = a.SYSTEM_ID,
                                 descTipoDoc = a.VAR_DESC_ATTO,
                                 diritti = b.DIRITTI
                             }).FirstOrDefault();

            if (tipologia == null || (checkVis ? (tipologia.diritti == null || tipologia.diritti < 1) : false)) return null;

            if (tipologia != null && tipologia.idTipoDoc > 0)
            {
                retval = new Template()
                {
                    Id = tipologia.idTipoDoc.ToString(),
                    Name = tipologia.descTipoDoc,
                    Type = "D"
                };

                var query = from a in dbContext.AssociazioneTemplatesEntities
                            join b in dbContext.OggettiCustomEntities on a.ID_OGGETTO equals b.SYSTEM_ID
                            join c in dbContext.TipoOggettoEntities on b.ID_TIPO_OGGETTO equals c.SYSTEM_ID
                            from d in dbContext.AROggCustomDocEntities
                                .Where(x => x.ID_OGGETTO_CUSTOM == b.SYSTEM_ID &&
                                    x.ID_TEMPLATE == a.ID_TEMPLATE &&
                                    x.ID_RUOLO == idGruppo.AsLong()).DefaultIfEmpty()
                            where a.ID_TEMPLATE == tipologia.idTipoDoc && a.DOC_NUMBER == null
                            select new
                            {
                                idOggetto = b.SYSTEM_ID,
                                descOggetto = b.DESCRIZIONE,
                                obbligatorio = b.CAMPO_OBBLIGATORIO,
                                valore = "",
                                tipooggetto = c.DESCRIZIONE,
                                tipoContatore = b.CHA_TIPO_TAR
                                ,
                                scrittura = d.INS_MOD,
                                lettura = d.VIS
                            };
                if (query != null && query.Any())
                {
                    List<Field> campi = new List<Field>();
                    Field campo = null;
                    Dictionary<string, List<string>> selezioneMultipla = new Dictionary<string, List<string>>();
                    List<string> valori = null;
                    foreach (var ogg in query)
                    {
                        valori = new List<string>();
                        campo = new Field();
                        campo.Id = ogg.idOggetto.ToString();
                        campo.Name = ogg.descOggetto;
                        campo.Required = ogg.obbligatorio == "SI";
                        if (ogg.scrittura == 1) campo.Rights = "INSERT_AND_MODIFY";
                        else if (ogg.lettura == 1) campo.Rights = "VIEW";
                        else campo.Rights = "NONE";
                        switch (ogg.tipooggetto)
                        {
                            case "Corrispondente":
                                campo.Value = ogg.valore;
                                campo.Type = "Correspondent";
                                break;
                            case "CampoDiTesto":
                                campo.Value = ogg.valore;
                                campo.Type = "TextField";
                                break;
                            case "SelezioneEsclusiva":
                                campo.Value = ogg.valore;
                                campo.Type = "ExclusiveSelection";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "MenuATendina":
                                campo.Value = ogg.valore;
                                campo.Type = "DropDown";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();
                                break;
                            case "Data":
                                campo.Value = ogg.valore;
                                campo.Type = "Date";
                                break;
                            case "Contatore":
                                campo.Value = ogg.valore;
                                campo.Type = "Counter";
                                break;
                            case "ContatoreSottocontatore":
                                campo.Value = ogg.valore;
                                campo.Type = "SubCounter";
                                break;
                            case "CasellaDiSelezione":
                                campo.Type = "MultipleChoise";
                                valori = (from a in dbContext.AssociazioneValoriEntities
                                          where a.ID_OGGETTO_CUSTOM == ogg.idOggetto
                                          orderby a.SYSTEM_ID
                                          select a.VALORE).ToList();
                                if (valori != null) campo.MultipleChoice = valori.ToArray();

                                break;
                            case "Link":
                                campo.Value = ogg.valore;
                                campo.Type = "Link";
                                break;
                            case "Separatore":
                                campo.Type = "Divider";
                                break;
                        }
                        if (!((campi.Where(x => x.Id == campo.Id).Select(x => x)).Any()))
                            campi.Add(campo);
                    }
                    retval.Fields = campi.ToArray();
                }

                retval.StateDiagram = GetDocumentDiagramByIdTemplate(tipologia.idTipoDoc.ToString(), dbContext);

            }
            return retval;
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateById(string idTemp, IPi3DbContext pi3DbContext)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            long idTemplate = idTemp.AsLong();

            #region Fetching Template
            var q1 = pi3DbContext.TipoAttoEntities
                //.Where(x => x.SYSTEM_ID == idTemplate)
                .Join(pi3DbContext.AssociazioneTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at });

            var q2 = q1
                //.Where(x => string.IsNullOrEmpty(x.at.DOC_NUMBER))
                .Join(pi3DbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc });

            var q3 = q2
                .Join(pi3DbContext.OggettiCustomCompEntities, q2 => q2.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q2, occ) => new { q2.ta, q2.at, q2.oc, occ });

            var q4 = q3
                .Join(pi3DbContext.TipoOggettoEntities, q3 => q3.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q3, to) => new { q3.ta, q3.at, q3.oc, q3.occ, to });

            var values = q4.Where(x => x.ta.SYSTEM_ID == idTemplate && string.IsNullOrEmpty(x.at.DOC_NUMBER) && x.occ.ID_TEMPLATE == idTemplate).ToList();

            if (values.Count() == 0)
            {
                var t = pi3DbContext.TipoAttoEntities
                .FirstOrDefault(x => x.SYSTEM_ID == idTemplate);

                template = new DocsPaVO.ProfilazioneDinamica.Templates
                {
                    SYSTEM_ID = Convert.ToInt32(t.SYSTEM_ID),
                    ID_TIPO_ATTO = t.SYSTEM_ID.ToString(),
                    DESCRIZIONE = t.VAR_DESC_ATTO,
                    ABILITATO_SI_NO = t.ABILITATO_SI_NO.ToString(),
                    IN_ESERCIZIO = t.IN_ESERCIZIO,
                    PATH_MODELLO_1 = t.PATH_MOD_1,
                    PATH_MODELLO_2 = t.PATH_MOD_2,
                    PATH_MODELLO_1_EXT = t.EXT_MOD_1,
                    PATH_MODELLO_2_EXT = t.EXT_MOD_2,
                    PATH_MODELLO_STAMPA_UNIONE = t.PATH_MOD_SU,
                    PATH_MODELLO_EXCEL = t.PATH_MOD_EXC,
                    PATH_XSD_ASSOCIATO = t.PATH_XSD_ASSOCIATO,
                    PATH_ALLEGATO_1 = t.PATH_ALL_1,
                    SCADENZA = t.GG_SCADENZA.ToString(),
                    PRE_SCADENZA = t.GG_PRE_SCADENZA.ToString(),
                    PRIVATO = t.CHA_PRIVATO ?? "0",
                    ID_AMMINISTRAZIONE = t.ID_AMM.ToString(),
                    CODICE_CLASSIFICA = t.COD_CLASS,
                    CODICE_MODELLO_TRASM = t.COD_MOD_TRASM,
                    IPER_FASC_DOC = (t.IPERDOCUMENTO != null && t.IPERDOCUMENTO == 1) ? "1" : "0",
                    NUM_MESI_CONSERVAZIONE = t.NUM_MESI_CONSERVAZIONE.ToString(),
                    IS_TYPE_INSTANCE = Convert.ToChar(t.IS_TYPE_INSTANCE),
                    INVIO_CONSERVAZIONE = t.CHA_INVIO_CONSERVAZIONE ?? "0",
                    CHA_ASSOC_MANUALE = t.CHA_ASSOC_MANUALE ?? "0",
                    ID_CONTESTO_PROCEDURALE = t.ID_CONTESTO_PROCEDURALE.ToString()
                };
            }
            else
            {
                //Set template
                var v = values[0];
                template = new DocsPaVO.ProfilazioneDinamica.Templates
                {
                    SYSTEM_ID = Convert.ToInt32(v.ta.SYSTEM_ID),
                    ID_TIPO_ATTO = v.ta.SYSTEM_ID.ToString(),
                    DESCRIZIONE = v.ta.VAR_DESC_ATTO,
                    DOC_NUMBER = v.at.DOC_NUMBER,
                    ABILITATO_SI_NO = v.ta.ABILITATO_SI_NO.ToString(),
                    IN_ESERCIZIO = v.ta.IN_ESERCIZIO,
                    PATH_MODELLO_1 = v.ta.PATH_MOD_1,
                    PATH_MODELLO_2 = v.ta.PATH_MOD_2,
                    PATH_MODELLO_1_EXT = v.ta.EXT_MOD_1,
                    PATH_MODELLO_2_EXT = v.ta.EXT_MOD_2,
                    PATH_MODELLO_STAMPA_UNIONE = v.ta.PATH_MOD_SU,
                    PATH_MODELLO_EXCEL = v.ta.PATH_MOD_EXC,
                    PATH_XSD_ASSOCIATO = v.ta.PATH_XSD_ASSOCIATO,
                    PATH_ALLEGATO_1 = v.ta.PATH_ALL_1,
                    SCADENZA = v.ta.GG_SCADENZA.ToString(),
                    PRE_SCADENZA = v.ta.GG_PRE_SCADENZA.ToString(),
                    PRIVATO = v.ta.CHA_PRIVATO ?? "0",
                    ID_AMMINISTRAZIONE = v.ta.ID_AMM.ToString(),
                    CODICE_CLASSIFICA = v.ta.COD_CLASS,
                    CODICE_MODELLO_TRASM = v.ta.COD_MOD_TRASM,
                    IPER_FASC_DOC = (v.ta.IPERDOCUMENTO != null && v.ta.IPERDOCUMENTO == 1) ? "1" : "0",
                    NUM_MESI_CONSERVAZIONE = v.ta.NUM_MESI_CONSERVAZIONE.ToString(),
                    IS_TYPE_INSTANCE = Convert.ToChar(v.ta.IS_TYPE_INSTANCE),
                    INVIO_CONSERVAZIONE = v.ta.CHA_INVIO_CONSERVAZIONE ?? "0",
                    CHA_ASSOC_MANUALE = v.ta.CHA_ASSOC_MANUALE ?? "0",
                    ID_CONTESTO_PROCEDURALE = v.ta.ID_CONTESTO_PROCEDURALE.ToString()
                };

                //Cerco gli oggetti custom associati al template 
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
                values.ForEach(v =>
                {
                    //Imposto i valori degli oggetti custom dei tipi oggetto
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom
                    {
                        SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                        CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                        CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                        ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                        DESCRIZIONE = v.oc.DESCRIZIONE,
                        MULTILINEA = v.oc.MULTILINEA,
                        NUMERO_DI_LINEE = v.oc.NUMERO_DI_LINEE,
                        ORIZZONTALE_VERTICALE = v.oc.ORIZZONTALE_VERTICALE,
                        POSIZIONE = v.occ.POSIZIONE.ToString(),
                        CAMPO_XML_ASSOC = v.occ.CAMPO_XML_ASSOC,
                        OPZIONI_XML_ASSOC = v.occ.OPZIONI_XML_ASSOC,
                        RESETTA_CONTATORE_INIZIO_ANNO = v.oc.RESET_ANNO,
                        FORMATO_CONTATORE = v.oc.FORMATO_CONTATORE,
                        TIPO_RICERCA_CORR = v.oc.RICERCA_CORR,
                        ID_RUOLO_DEFAULT = v.oc.ID_R_DEFAULT,
                        CAMPO_COMUNE = v.oc.CAMPO_COMUNE.ToString(),
                        TIPO_CONTATORE = v.oc.CHA_TIPO_TAR,
                        CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                        REPERTORIO = v.oc.REPERTORIO.ToString(),
                        CONS_REPERTORIO = v.oc.CHA_CONS_REPERTORIO,
                        DA_VISUALIZZARE_RICERCA = v.oc.DA_VISUALIZZARE_RICERCA.ToString(),
                        ANNO = v.at.ANNO.ToString(),
                        VALORE_DATABASE = v.at.VALORE_OGGETTO_DB,
                        ID_AOO_RF = v.at.ID_AOO_RF != null ? v.at.ID_AOO_RF.ToString() : "0",
                        FORMATO_ORA = v.oc.FORMATO_ORA,
                        TIPO_LINK = v.oc.TIPO_LINK,
                        TIPO_OBJ_LINK = v.oc.TIPO_OBJ_LINK,
                        MODULO_SOTTOCONTATORE = v.oc.MODULO_SOTTOCONTATORE.ToString(),
                        VALORE_SOTTOCONTATORE = v.at.VALORE_SC.ToString(),
                        DATA_INSERIMENTO = v.at.DTA_INS.ToString(),
                        DATA_ANNULLAMENTO = v.at.DTA_ANNULLAMENTO.ToString(),
                        CODICE_DB = v.at.CODICE_DB,
                        MANUAL_INSERT = v.at.MANUAL_INSERT != null && v.at.MANUAL_INSERT == 1,
                        ANNO_ACC = v.at.ANNO_ACC,
                        CONSOLIDAMENTO = v.oc.CHA_CONSOLIDAMENTO ?? "0",
                        CONSERVAZIONE = v.oc.CHA_CONSERVAZIONE ?? "0",
                    };

                    //Controllo contatore
                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    {
                        var contCustomDoc = pi3DbContext.ContCustomDocEntities
                            .Where(x => x.ID_OGG == oggettoCustom.SYSTEM_ID)
                            .ToList()
                            .FirstOrDefault();

                        if (contCustomDoc != null)
                        {
                            oggettoCustom.DATA_INIZIO = contCustomDoc.DATA_INIZIO.ToString();
                            oggettoCustom.DATA_FINE = contCustomDoc.DATA_FINE.ToString();
                        }
                    }

                    //set tipo oggetto
                    oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                    {
                        SYSTEM_ID = Convert.ToInt32(v.to.SYSTEM_ID),
                        DESCRIZIONE_TIPO = v.to.DESCRIZIONE
                    };

                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = pi3DbContext.OggettiCustomEntities
                            .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                            .Select(x => x.CONFIG_OBJ_EST)
                            .FirstOrDefault()
                            .ToString();
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Seleziono i valori per l'oggettoCustom
                    var oggCustomValues = pi3DbContext.AssociazioneValoriEntities
                        .Where(x => x.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID)
                        .ToList();
                    List<string> valSel = new();
                    List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                    oggCustomValues.ForEach(v =>
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                        {
                            SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                            DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                            VALORE = v.VALORE,
                            VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT,
                            COLOR_BG = v.COLOR_BG,
                            ABILITATO = Convert.ToInt32(v.ABILITATO)
                        };
                        elencoValoriList.Add(valoreOggetto);
                        valSel.Add(string.Empty);
                    });

                    oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                    oggettoCustom.VALORI_SELEZIONATI = valSel.ToArray();
                    elencoOggettiList.Add(oggettoCustom);
                });
                template.ELENCO_OGGETTI = elencoOggettiList.ToArray();
            }
            #endregion

            return template;
        }
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateByDescrizione(string desc, string id_amm, IPi3DbContext pi3DbContext)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;

            #region Fetching Template
            var q1 = pi3DbContext.TipoAttoEntities
                //.Where(x => x.SYSTEM_ID == idTemplate)
                .Join(pi3DbContext.AssociazioneTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at });

            var q2 = q1
                //.Where(x => string.IsNullOrEmpty(x.at.DOC_NUMBER))
                .Join(pi3DbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc });

            var q3 = q2
                .Join(pi3DbContext.OggettiCustomCompEntities, q2 => q2.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q2, occ) => new { q2.ta, q2.at, q2.oc, occ });

            var q4 = q3
                .Join(pi3DbContext.TipoOggettoEntities, q3 => q3.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q3, to) => new { q3.ta, q3.at, q3.oc, q3.occ, to });

            var values = q4.Where(x => x.ta.VAR_DESC_ATTO.ToUpper().Equals(desc.ToUpper()) && x.ta.ID_AMM == id_amm.AsLong() && x.ta.ABILITATO_SI_NO == 1 && string.IsNullOrEmpty(x.at.DOC_NUMBER) && x.occ.ID_TEMPLATE == x.ta.SYSTEM_ID).ToList();

            if (values.Count() == 0)
            {
                var t = pi3DbContext.TipoAttoEntities
                .FirstOrDefault(x => x.VAR_DESC_ATTO.ToUpper().Equals(desc.ToUpper()));

                template = new DocsPaVO.ProfilazioneDinamica.Templates
                {
                    SYSTEM_ID = Convert.ToInt32(t.SYSTEM_ID),
                    ID_TIPO_ATTO = t.SYSTEM_ID.ToString(),
                    DESCRIZIONE = t.VAR_DESC_ATTO,
                    ABILITATO_SI_NO = t.ABILITATO_SI_NO.ToString(),
                    IN_ESERCIZIO = t.IN_ESERCIZIO,
                    PATH_MODELLO_1 = t.PATH_MOD_1,
                    PATH_MODELLO_2 = t.PATH_MOD_2,
                    PATH_MODELLO_1_EXT = t.EXT_MOD_1,
                    PATH_MODELLO_2_EXT = t.EXT_MOD_2,
                    PATH_MODELLO_STAMPA_UNIONE = t.PATH_MOD_SU,
                    PATH_MODELLO_EXCEL = t.PATH_MOD_EXC,
                    PATH_XSD_ASSOCIATO = t.PATH_XSD_ASSOCIATO,
                    PATH_ALLEGATO_1 = t.PATH_ALL_1,
                    SCADENZA = t.GG_SCADENZA.ToString(),
                    PRE_SCADENZA = t.GG_PRE_SCADENZA.ToString(),
                    PRIVATO = t.CHA_PRIVATO ?? "0",
                    ID_AMMINISTRAZIONE = t.ID_AMM.ToString(),
                    CODICE_CLASSIFICA = t.COD_CLASS,
                    CODICE_MODELLO_TRASM = t.COD_MOD_TRASM,
                    IPER_FASC_DOC = (t.IPERDOCUMENTO != null && t.IPERDOCUMENTO == 1) ? "1" : "0",
                    NUM_MESI_CONSERVAZIONE = t.NUM_MESI_CONSERVAZIONE.ToString(),
                    IS_TYPE_INSTANCE = Convert.ToChar(t.IS_TYPE_INSTANCE),
                    INVIO_CONSERVAZIONE = t.CHA_INVIO_CONSERVAZIONE ?? "0",
                    CHA_ASSOC_MANUALE = t.CHA_ASSOC_MANUALE ?? "0",
                    ID_CONTESTO_PROCEDURALE = t.ID_CONTESTO_PROCEDURALE.ToString()
                };
            }
            else
            {
                //Set template
                var v = values[0];
                template = new DocsPaVO.ProfilazioneDinamica.Templates
                {
                    SYSTEM_ID = Convert.ToInt32(v.ta.SYSTEM_ID),
                    ID_TIPO_ATTO = v.ta.SYSTEM_ID.ToString(),
                    DESCRIZIONE = v.ta.VAR_DESC_ATTO,
                    DOC_NUMBER = v.at.DOC_NUMBER,
                    ABILITATO_SI_NO = v.ta.ABILITATO_SI_NO.ToString(),
                    IN_ESERCIZIO = v.ta.IN_ESERCIZIO,
                    PATH_MODELLO_1 = v.ta.PATH_MOD_1,
                    PATH_MODELLO_2 = v.ta.PATH_MOD_2,
                    PATH_MODELLO_1_EXT = v.ta.EXT_MOD_1,
                    PATH_MODELLO_2_EXT = v.ta.EXT_MOD_2,
                    PATH_MODELLO_STAMPA_UNIONE = v.ta.PATH_MOD_SU,
                    PATH_MODELLO_EXCEL = v.ta.PATH_MOD_EXC,
                    PATH_XSD_ASSOCIATO = v.ta.PATH_XSD_ASSOCIATO,
                    PATH_ALLEGATO_1 = v.ta.PATH_ALL_1,
                    SCADENZA = v.ta.GG_SCADENZA.ToString(),
                    PRE_SCADENZA = v.ta.GG_PRE_SCADENZA.ToString(),
                    PRIVATO = v.ta.CHA_PRIVATO ?? "0",
                    ID_AMMINISTRAZIONE = v.ta.ID_AMM.ToString(),
                    CODICE_CLASSIFICA = v.ta.COD_CLASS,
                    CODICE_MODELLO_TRASM = v.ta.COD_MOD_TRASM,
                    IPER_FASC_DOC = (v.ta.IPERDOCUMENTO != null && v.ta.IPERDOCUMENTO == 1) ? "1" : "0",
                    NUM_MESI_CONSERVAZIONE = v.ta.NUM_MESI_CONSERVAZIONE.ToString(),
                    IS_TYPE_INSTANCE = Convert.ToChar(v.ta.IS_TYPE_INSTANCE),
                    INVIO_CONSERVAZIONE = v.ta.CHA_INVIO_CONSERVAZIONE ?? "0",
                    CHA_ASSOC_MANUALE = v.ta.CHA_ASSOC_MANUALE ?? "0",
                    ID_CONTESTO_PROCEDURALE = v.ta.ID_CONTESTO_PROCEDURALE.ToString()
                };

                //Cerco gli oggetti custom associati al template 
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
                values.ForEach(v =>
                {
                    //Imposto i valori degli oggetti custom dei tipi oggetto
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom
                    {
                        SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                        CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                        CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                        ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                        DESCRIZIONE = v.oc.DESCRIZIONE,
                        MULTILINEA = v.oc.MULTILINEA,
                        NUMERO_DI_LINEE = v.oc.NUMERO_DI_LINEE,
                        ORIZZONTALE_VERTICALE = v.oc.ORIZZONTALE_VERTICALE,
                        POSIZIONE = v.occ.POSIZIONE.ToString(),
                        CAMPO_XML_ASSOC = v.occ.CAMPO_XML_ASSOC,
                        OPZIONI_XML_ASSOC = v.occ.OPZIONI_XML_ASSOC,
                        RESETTA_CONTATORE_INIZIO_ANNO = v.oc.RESET_ANNO,
                        FORMATO_CONTATORE = v.oc.FORMATO_CONTATORE,
                        TIPO_RICERCA_CORR = v.oc.RICERCA_CORR,
                        ID_RUOLO_DEFAULT = v.oc.ID_R_DEFAULT,
                        CAMPO_COMUNE = v.oc.CAMPO_COMUNE.ToString(),
                        TIPO_CONTATORE = v.oc.CHA_TIPO_TAR,
                        CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                        REPERTORIO = v.oc.REPERTORIO.ToString(),
                        CONS_REPERTORIO = v.oc.CHA_CONS_REPERTORIO,
                        DA_VISUALIZZARE_RICERCA = v.oc.DA_VISUALIZZARE_RICERCA.ToString(),
                        ANNO = v.at.ANNO.ToString(),
                        VALORE_DATABASE = v.at.VALORE_OGGETTO_DB,
                        ID_AOO_RF = v.at.ID_AOO_RF != null ? v.at.ID_AOO_RF.ToString() : "0",
                        FORMATO_ORA = v.oc.FORMATO_ORA,
                        TIPO_LINK = v.oc.TIPO_LINK,
                        TIPO_OBJ_LINK = v.oc.TIPO_OBJ_LINK,
                        MODULO_SOTTOCONTATORE = v.oc.MODULO_SOTTOCONTATORE.ToString(),
                        VALORE_SOTTOCONTATORE = v.at.VALORE_SC.ToString(),
                        DATA_INSERIMENTO = v.at.DTA_INS.ToString(),
                        DATA_ANNULLAMENTO = v.at.DTA_ANNULLAMENTO.ToString(),
                        CODICE_DB = v.at.CODICE_DB,
                        MANUAL_INSERT = v.at.MANUAL_INSERT != null && v.at.MANUAL_INSERT == 1,
                        ANNO_ACC = v.at.ANNO_ACC,
                        CONSOLIDAMENTO = v.oc.CHA_CONSOLIDAMENTO ?? "0",
                        CONSERVAZIONE = v.oc.CHA_CONSERVAZIONE ?? "0",
                    };

                    //Controllo contatore
                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    {
                        var contCustomDoc = pi3DbContext.ContCustomDocEntities
                            .Where(x => x.ID_OGG == oggettoCustom.SYSTEM_ID)
                            .ToList()
                            .FirstOrDefault();

                        if (contCustomDoc != null)
                        {
                            oggettoCustom.DATA_INIZIO = contCustomDoc.DATA_INIZIO.ToString();
                            oggettoCustom.DATA_FINE = contCustomDoc.DATA_FINE.ToString();
                        }
                    }

                    //set tipo oggetto
                    oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                    {
                        SYSTEM_ID = Convert.ToInt32(v.to.SYSTEM_ID),
                        DESCRIZIONE_TIPO = v.to.DESCRIZIONE
                    };

                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = pi3DbContext.OggettiCustomEntities
                            .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                            .Select(x => x.CONFIG_OBJ_EST)
                            .FirstOrDefault()
                            .ToString();
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Seleziono i valori per l'oggettoCustom
                    var oggCustomValues = pi3DbContext.AssociazioneValoriEntities
                        .Where(x => x.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID)
                        .ToList();
                    List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                    List<string> valoriSel = new();
                    oggCustomValues.ForEach(v =>
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                        {
                            SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                            DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                            VALORE = v.VALORE,
                            VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT,
                            COLOR_BG = v.COLOR_BG,
                            ABILITATO = Convert.ToInt32(v.ABILITATO)
                        };
                        elencoValoriList.Add(valoreOggetto);
                        valoriSel.Add(string.Empty);
                    });

                    oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                    oggettoCustom.VALORI_SELEZIONATI = valoriSel.ToArray();
                    elencoOggettiList.Add(oggettoCustom);
                });
                template.ELENCO_OGGETTI = elencoOggettiList.ToArray();
            }
            #endregion
            return template;
        }
        public static StateDiagram GetDocumentDiagramByIdTemplate(string idTemplate, IPi3DbContext dbContext)
        {
            StateDiagram retval = null;
            var query = (from a in dbContext.AssDiagrammiEntities
                         join b in dbContext.DiagrammiStatoEntities on a.ID_DIAGRAMMA equals b.SYSTEM_ID
                         join c in dbContext.StatoEntities on b.SYSTEM_ID equals c.ID_DIAGRAMMA
                         where a.ID_TIPO_DOC == idTemplate.AsLong()
                         select new
                         {
                             idDiagramma = b.SYSTEM_ID,
                             descDiagramma = b.VAR_DESCRIZIONE,
                             idStato = c.SYSTEM_ID,
                             descStato = c.VAR_DESCRIZIONE,
                             statoIniziale = c.STATO_INIZIALE == 1,
                             statoFinale = c.STATO_FINALE == 1
                         });
            if (query != null && query.Any())
            {
                retval = new StateDiagram();
                var diagramma = query.FirstOrDefault();
                if (diagramma != null)
                {

                    retval.Description = diagramma.descDiagramma;
                    retval.Id = diagramma.idDiagramma.ToString();

                }
                List<StateOfDiagram> stati = new List<StateOfDiagram>();
                foreach (var stato in query)
                {
                    stati.Add(new StateOfDiagram()
                    {
                        Id = stato.idStato.ToString(),
                        Description = stato.descStato,
                        DiagramId = stato.idDiagramma.ToString(),
                        InitialState = stato.statoIniziale,
                        FinaleState = stato.statoFinale
                    });
                }
                retval.StateOfDiagram = stati.ToArray();
            }

            return retval;
        }

        public static List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> GetRuoliTipoFasc(InfoUtente infoUtente, string idTipoFasc, IPi3DbContext dbContext)
        {
            var visTipoFasc = dbContext.VisTipoFascEntities.AsNoTracking()
                .Where(p => p.ID_TIPO_FASC == idTipoFasc.AsLong() &&
                p.ID_RUOLO == infoUtente.idGruppo.AsLong() &&
                p.DIRITTI == 2
            );
            List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> visOutput = new();

            if (visTipoFasc.Any())
            {
                foreach (var tipo in visTipoFasc)
                {
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli obj = new()
                    {
                        ID_GRUPPO = tipo.ID_RUOLO != null ? tipo.ID_RUOLO.ToString() : null,
                        ID_TIPO_DOC_FASC = tipo.ID_TIPO_FASC != null ? tipo.ID_TIPO_FASC.ToString() : null,
                        DIRITTI_TIPOLOGIA = tipo.DIRITTI != null ? tipo.DIRITTI.ToString() : null
                    };

                    visOutput.Add(obj);
                }
            }

            return visOutput;
        }

        #endregion

        #region Rubrica
        public static DocsPaVO.utente.Corrispondente GetDettaglioCorrispondente(string idCorrispondente, IPi3DbContext dbContext)
        {
            DocsPaVO.utente.Corrispondente corrispondente = null;

            if (!string.IsNullOrEmpty(idCorrispondente))
            {
                var dett = dbContext.DettGlobaliEntities.AsNoTracking().Where(d => d.ID_CORR_GLOBALI == idCorrispondente.AsLong()).FirstOrDefault();
                if (dett != null)
                {
                    corrispondente = new()
                    {
                        indirizzo = dett.VAR_INDIRIZZO ?? string.Empty,
                        cap = dett.VAR_CAP ?? string.Empty,
                        prov = dett.VAR_PROVINCIA ?? string.Empty,
                        nazionalita = dett.VAR_NAZIONE ?? string.Empty,
                        codfisc = dett.VAR_COD_FISC ?? string.Empty,
                        telefono1 = dett.VAR_TELEFONO ?? string.Empty,
                        telefono2 = dett.VAR_TELEFONO2 ?? string.Empty,
                        fax = dett.VAR_FAX ?? string.Empty,
                        note = dett.VAR_NOTE ?? string.Empty,
                        citta = dett.VAR_CITTA ?? string.Empty,
                        partitaiva = dett.VAR_COD_PI ?? string.Empty,
                        localita = dett.VAR_LOCALITA ?? string.Empty,
                    };
                }
                else
                {
                    corrispondente = new()
                    {
                        indirizzo = string.Empty,
                        cap = string.Empty,
                        prov = string.Empty,
                        nazionalita = string.Empty,
                        codfisc = string.Empty,
                        telefono1 = string.Empty,
                        telefono2 = string.Empty,
                        fax = string.Empty,
                        note = string.Empty,
                        citta = string.Empty,
                        partitaiva = string.Empty,
                        localita = string.Empty,
                    };
                }
            }

            return corrispondente;
        }
        public static DocsPaVO.rubrica.ParametriRicercaRubrica GetParametriRicercaRubricaFromPis(Filter[] filters, DocsPaVO.utente.InfoUtente infoUtente, IPi3DbContext dbContext)
        {
            DocsPaVO.rubrica.ParametriRicercaRubrica result = new DocsPaVO.rubrica.ParametriRicercaRubrica();

            result.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
            result.parent = "";
            result.caller.IdRuolo = infoUtente.idGruppo;
            result.caller.filtroRegistroPerRicerca = string.Empty;
            bool filterFound = false;

            foreach (Filter fil in filters)
            {
                if (fil != null && !string.IsNullOrEmpty(fil.Value))
                {
                    if (fil.Name.ToUpper().Equals("OFFICES"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doUo = true;
                        }
                        else
                        {
                            result.doUo = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("USERS"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doUtenti = true;
                        }
                        else
                        {
                            result.doUtenti = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("ROLES"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRuoli = true;
                        }
                        else
                        {
                            result.doRuoli = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("TYPE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            if (fil.Value.ToUpper().Equals("EXTERNAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                            }
                            if (fil.Value.ToUpper().Equals("INTERNAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
                            }
                            if (fil.Value.ToUpper().Equals("GLOBAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                            }
                        }
                    }

                    if (fil.Name.ToUpper().Equals("COMMON_ADDRESSBOOK"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRubricaComune = true;
                        }
                        else
                        {
                            result.doRubricaComune = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("RF"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRF = true;
                        }
                        else
                        {
                            result.doRF = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("CODE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codice = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("EXACT_CODE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codice = fil.Value;
                            result.queryCodiceEsatta = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("DESCRIPTION"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.descrizione = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("CITY"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.citta = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("LOCALITY"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.localita = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("MAIL"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.emailsPISREST = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("NATIONAL_IDENTIFICATION_NUMBER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codiceFiscale = fil.Value;
                        }
                    }
                    if (fil.Name.ToUpper().Equals("VAT_NUMBER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.partitaIva = fil.Value;
                        }
                    }
                    if (fil.Name.ToUpper().Equals("REGISTRY_OR_RF"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(fil.Value, infoUtente.idAmministrazione, dbContext);
                            if (reg == null)
                            {
                                throw new RestException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                result.caller.IdRegistro = reg.systemId;
                                result.caller.filtroRegistroPerRicerca = reg.systemId;
                            }
                        }
                    }
                    if (fil.Name.ToUpper().Equals("EXTRACT_ID_COMMONADDRESSBOOK"))
                    {
                        filterFound = true;
                    }
                    if (fil.Name.ToUpper().Equals("EXTRACT_DETAILS"))
                    {
                        filterFound = true;
                    }
                    if (fil.Name.ToUpper().Equals("EXTENDED_SEARCH_NO_REG"))
                    {
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            filterFound = true;
                            result.caller.filtroRegistroPerRicerca = "";
                            ArrayList registriLista = DBUtils.getListaRegistriRfRuolo(infoUtente.idCorrGlobali, string.Empty, dbContext);
                            foreach (DocsPaVO.utente.Registro reg in registriLista)
                            {
                                if (!string.IsNullOrEmpty(result.caller.filtroRegistroPerRicerca))
                                    result.caller.filtroRegistroPerRicerca += ("," + reg.systemId);
                                else result.caller.filtroRegistroPerRicerca += reg.systemId;
                            }
                        }
                    }
                    if (fil.Name.ToUpper().Equals("MAIL_NOTES"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.noteEmailPISREST = fil.Value;
                        }
                    }

                    if (!filterFound)
                        throw new RestException("FILTER_NOT_FOUND");

                }
            }

            return result;
        }

        public static DocsPaVO.utente.Corrispondente GetCorrespondentFromPisNewInsertOccasionale(
            Correspondent corrispondente,
            InfoUtente infoUtente,
            IPi3DbContext pi3DbContext)
        {
            DocsPaVO.utente.Corrispondente output = new DocsPaVO.utente.Corrispondente();

            if (corrispondente != null)
            {
                output.descrizione = corrispondente.Description;
                output.tipoCorrispondente = corrispondente.Type;
                output.email = corrispondente.Email;
                output.idAmministrazione = infoUtente.idAmministrazione;
                if (!string.IsNullOrEmpty(corrispondente.CodeRegisterOrRF))
                {
                    var reg = DBUtils.getRegistroByCodAOO(corrispondente.CodeRegisterOrRF, infoUtente.idAmministrazione, pi3DbContext);
                    if (reg == null)
                    {
                        throw new RestException("REGISTER_NOT_FOUND");
                    }
                    else
                    {
                        output.idRegistro = reg.systemId;
                    }
                }

                if (corrispondente.PreferredChannel != null)
                {
                    var mezziSped = DBUtils.ListaMezziSpedizione(infoUtente.idAmministrazione, true, pi3DbContext);
                    foreach (DocsPaVO.amministrazione.MezzoSpedizione mSpediz in mezziSped)
                    {
                        if (mSpediz.Descrizione.ToUpper().Equals(corrispondente.PreferredChannel.ToUpper()))
                        {
                            output.canalePref = new DocsPaVO.utente.Canale();
                            output.canalePref.descrizione = mSpediz.Descrizione;
                            output.canalePref.systemId = mSpediz.IDSystem;
                            output.canalePref.tipoCanale = mSpediz.chaTipoCanale;
                            break;
                        }
                    }
                }

                #region Dettagli corrispondente
                string indirizzo = "";
                string citta = "";
                string cap = "";
                string provincia = "";
                string nazione = "";
                string telefono = "";
                string fax = "";
                string cf = "";
                string note = "";
                string locazione = "";
                string partitaIva = "";
                string codiceIpa = "";

                if (!string.IsNullOrEmpty(corrispondente.Address))
                {
                    indirizzo = corrispondente.Address.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.City))
                {
                    citta = corrispondente.City.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Cap))
                {
                    cap = corrispondente.Cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Province))
                {
                    provincia = corrispondente.Province.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Nation))
                {
                    nazione = corrispondente.Nation.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.PhoneNumber))
                {
                    telefono = corrispondente.PhoneNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Fax))
                {
                    fax = corrispondente.Fax.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.NationalIdentificationNumber))
                {
                    cf = corrispondente.NationalIdentificationNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Note))
                {
                    note = corrispondente.Note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.Location))
                {
                    locazione = corrispondente.Location.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corrispondente.VatNumber))
                {
                    partitaIva = corrispondente.VatNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                dettagli.Corrispondente.AddCorrispondenteRow(
                      indirizzo,
                       citta,
                        cap,
                        provincia,
                        nazione,
                        telefono,
                        string.Empty,
                        fax,
                        cf,
                        note,
                        locazione,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        partitaIva
                        );

                output.info = dettagli;
                output.dettagli = false;
                #endregion
            }
            else
            {
                output = null;
            }
            return output;
        }

        public static List<CorrespondentEmail> getEmailsCorrEsterno(string idCorr, IPi3DbContext dbContext)
        {
            List<CorrespondentEmail> retval = null;

            try
            {
                var query = from a in dbContext.MailCorrEsterniEntities
                            where a.ID_CORR == idCorr.AsLong()
                            select a;
                if (query != null && query.Any())
                {
                    retval = new List<CorrespondentEmail>();
                    foreach (var m in query)
                    {
                        retval.Add(new CorrespondentEmail()
                        {
                            Email = m.VAR_EMAIL,
                            Note = m.VAR_NOTE,
                            Main = m.VAR_PRINCIPALE
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                retval = null;
            }

            return retval;
        }

        public static Correspondent GetCorrespondentFromDB(string idCorr, IPi3DbContext dbContext)
        {
            Correspondent retval = null;
            try
            {
                var query = (from a in dbContext.CorrGlobaliEntities
                             where a.SYSTEM_ID == idCorr.AsLong()
                             select a).FirstOrDefault();

                if (query != null && query.SYSTEM_ID > 0)
                {
                    retval = new Correspondent()
                    {
                        Id = query.SYSTEM_ID.ToString(),
                        Code = query.VAR_COD_RUBRICA,
                        CorrespondentType = query.CHA_TIPO_URP ?? query.CHA_TIPO_CORR,
                        Type = query.CHA_TIPO_IE,
                        Description = query.VAR_DESC_CORR,
                        AOOCode = query.VAR_CODICE_AOO,
                        AdmCode = query.VAR_CODICE_AMM,
                        Email = query.VAR_EMAIL,
                        IsCommonAddress = !string.IsNullOrEmpty(query.RUBRICA_ESTERNA),
                        Name = !string.IsNullOrWhiteSpace(query.VAR_NOME) ? query.VAR_NOME : null,
                        Surname = !string.IsNullOrWhiteSpace(query.VAR_COGNOME) ? query.VAR_COGNOME : null,

                    };

                    if (query.ID_REGISTRO != null && query.ID_REGISTRO > 0)
                    {
                        var reg = getRegistro(query.ID_REGISTRO.ToString(), dbContext);
                        if (reg != null) retval.CodeRegisterOrRF = reg.codRegistro;
                    }

                    var dettagli = (from b in dbContext.DettGlobaliEntities
                                    where b.ID_CORR_GLOBALI == idCorr.AsLong()
                                    select b).FirstOrDefault();

                    if (dettagli != null && dettagli.ID_CORR_GLOBALI > 0)
                    {
                        retval.Address = dettagli.VAR_INDIRIZZO;
                        retval.Cap = dettagli.VAR_CAP;
                        retval.Note = dettagli.VAR_NOTE;
                        retval.City = dettagli.VAR_CITTA;
                        retval.Fax = dettagli.VAR_FAX;
                        retval.Location = dettagli.VAR_LOCALITA;
                        retval.Nation = dettagli.VAR_NAZIONE;
                        retval.NationalIdentificationNumber = dettagli.VAR_COD_FISC;
                        retval.PhoneNumber = dettagli.VAR_TELEFONO;
                        retval.PhoneNumber2 = dettagli.VAR_TELEFONO2;
                        retval.Province = dettagli.VAR_PROVINCIA;
                        retval.VatNumber = dettagli.VAR_COD_PI;
                    }
                    string canalePreferenziale = (from a in dbContext.DocumentTypesEntities
                                                  join b in dbContext.CanaleCorrEntities on a.SYSTEM_ID equals b.ID_DOCUMENTTYPE
                                                  where b.ID_CORR_GLOBALE == idCorr.AsLong() && b.CHA_PREFERITO == "1"
                                                  select a.DESCRIPTION).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(canalePreferenziale)) retval.PreferredChannel = canalePreferenziale;
                }
            }
            catch
            {
                retval = null;
            }



            return retval;
        }

        public static async Task<string> AddCorrispondenteOccasionale(Correspondent correspondent, InfoUtente infoUt, IPi3DbContext dbContext)
        {
            string idCorrCreato = null;
            string codTemp = "OCC_OCC_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            CorrGlobaliEntity corr = new CorrGlobaliEntity()
            {
                VAR_COD_RUBRICA = codTemp,
                VAR_CODICE = codTemp,
                VAR_DESC_CORR = correspondent.Description,
                VAR_DESC_CORR_OLD = correspondent.Description,
                VAR_EMAIL = correspondent.Email,
                ID_AMM = infoUt.idAmministrazione.AsLong(),
                DTA_INIZIO = DateTime.Now,
                CHA_DETTAGLI = "0",
                CHA_TIPO_CORR = "O",
                ID_PARENT = 0,
                ID_OLD = 0,
                VAR_CHIAVE_AE = "0",
                CHA_SYSTEM_ROLE = "0"
            };
            dbContext.CorrGlobaliEntities.Add(corr);
            ((DbContext)dbContext).SaveChanges();
            idCorrCreato = corr.SYSTEM_ID.ToString();

            //TODO: update del codice con OCC_+system_id
            corr.VAR_COD_RUBRICA = "OCC_" + corr.SYSTEM_ID;
            corr.VAR_CODICE = "OCC_" + corr.SYSTEM_ID;
            ((DbContext)dbContext).SaveChanges();

            if (!string.IsNullOrWhiteSpace(correspondent.Email))
            {
                MailCorrEsterniEntity mail = new MailCorrEsterniEntity()
                {
                    ID_CORR = corr.SYSTEM_ID,
                    VAR_EMAIL = correspondent.Email,
                    VAR_PRINCIPALE = "1"
                };
                await dbContext.MailCorrEsterniEntities.AddAsync(mail);
                await ((DbContext)dbContext).SaveChangesAsync();
            }

            return idCorrCreato;
        }

        #endregion

        #region Note
        public static List<Note> getNoteOggetto(string idOggetto, IPi3DbContext dbContext)
        {
            List<Note> retval = null;
            var idOggettoAsLong = idOggetto.AsLong();

            var query = (from note in dbContext.NoteEntities
                         join people in dbContext.PeopleEntities on note.IDUTENTECREATORE equals people.SYSTEM_ID
                         where note.IDOGGETTOASSOCIATO == idOggettoAsLong
                         select new { idnote = note.SYSTEM_ID, note.TESTO, idpeople = people.SYSTEM_ID, people.FULL_NAME, people.USER_ID, people.VAR_COGNOME, people.VAR_NOME });

            if (query != null && query.Any())
            {
                retval = new List<Note>();
                foreach (var n in query)
                {
                    retval.Add(new Note()
                    {
                        Id = n.idnote.ToString(),
                        Description = n.TESTO,
                        User = new Roles.User()
                        {
                            Id = n.idpeople.ToString(),
                            Description = n.FULL_NAME,
                            UserId = n.USER_ID,
                            Name = n.VAR_NOME,
                            Surname = n.VAR_COGNOME
                        }
                    });
                }
                ;
            }

            return retval;
        }

        #endregion

        #region Logs

        public static List<DocsPaVO.documento.LogDocumento> getListaLogEventiDoc(string idDocument, IPi3DbContext dbContext)
        {
            List<DocsPaVO.documento.LogDocumento> retval = null;

            var query = from a in dbContext.LogEntities
                        where a.VAR_OGGETTO == "DOCUMENTO" && a.ID_OGGETTO == idDocument.AsLong()
                        select a;
            if (query != null && query.Any())
            {
                retval = new List<DocsPaVO.documento.LogDocumento>();
                foreach (var l in query)
                {
                    retval.Add(new DocsPaVO.documento.LogDocumento()
                    {

                        codAzione = l.VAR_COD_AZIONE,
                        dataAzione = l.DTA_AZIONE?.ToString("dd/MM/yyyy HH:mm:ss"),
                        idAmm = l.ID_AMM.ToString(),
                        systemId = l.SYSTEM_ID.ToString(),
                        descrOggetto = l.VAR_DESC_OGGETTO,
                        chaEsito = l.CHA_ESITO,
                        descProduttore = l.DESC_PRODUCER,
                        idGruppoOperatore = l.ID_GRUPPO_OPERATORE.ToString(),
                        idPeopleOPeratore = l.ID_PEOPLE_OPERATORE.ToString(),
                        userIdOperatore = l.USERID_OPERATORE
                    });
                }
            }

            return retval;
        }
        #endregion

        #region Sistemi esterni
        public static bool ApplicationIgnoresRights(string codeApplication, IPi3DbContext dbContext)
        {
            bool retval = false;
            if (!String.IsNullOrEmpty(codeApplication) && (codeApplication == "ALBO_TELEMATICO" || IsVerticalePubbAlbo(codeApplication, dbContext)))
                retval = true;
            return retval;
        }

        public static bool IsVerticalePubbAlbo(string codeAppVerticale, IPi3DbContext dbContext)
        {
            bool retval = false;
            var query = from a in dbContext.AlboPubbVerticaliEntities
                        where a.CODEAPP_VERTICALE == codeAppVerticale
                        select a;
            if (query != null && query.Any()) { retval = true; }
            return retval;
        }




        #endregion

        #region Spedizione

        public static List<DocsPaVO.amministrazione.MezzoSpedizione> ListaMezziSpedizione(
            string idAmm,
            bool vediTutti,
            IPi3DbContext pi3DbContext)
        {
            List<DocsPaVO.amministrazione.MezzoSpedizione> mezziSpedizione = new();

            var spedQuery = (from a in pi3DbContext.DocumentTypesEntities.AsNoTracking()
                             where a.CHA_TIPO_CANALE != null
                             select new
                             {
                                 IDSYSTEM_MS = a.SYSTEM_ID,
                                 DESCRIPTION = a.DESCRIPTION,
                                 CHA_TIPO_CANALE = a.CHA_TIPO_CANALE,
                                 DISABLED = a.DISABLED
                             }).Distinct().OrderBy(a => a.DESCRIPTION).ToList();

            spedQuery.ForEach(a =>
            {
                var mezzo = new DocsPaVO.amministrazione.MezzoSpedizione
                {
                    Descrizione = a.DESCRIPTION,
                    IDAmministrazione = idAmm,
                    IDSystem = a.IDSYSTEM_MS.ToString(),
                    chaTipoCanale = a.CHA_TIPO_CANALE,
                    Disabled = a.DISABLED

                };

                if (vediTutti)
                {
                    mezziSpedizione.Add(mezzo);
                }
                else if (string.IsNullOrEmpty(mezzo.Disabled))
                {
                    mezziSpedizione.Add(mezzo);
                }
                mezzo = null;
            });

            return mezziSpedizione;
        }

        #endregion

        #region Trasmissioni
        public static async Task<long> GetTrasmPendenteConWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople, IPi3DbContext dbContext)
        {
            long output = 0;

            var idCorrGlobaliAsLong = idRuoloInUO.AsLong();
            var idProjectAsLong = idProject.AsLong();
            var idPeopleAsLong = idPeople.AsLong();

            output = await dbContext.TrasmissioneEntities.AsNoTracking()
                .Join(dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, s => s.ID_TRASMISSIONE, (t, s) => new { t, s })
                .Join(dbContext.TrasmUtenteEntities.AsNoTracking(), j => j.s.SYSTEM_ID, u => u.ID_TRASM_SINGOLA, (j, u) => new { j.t, j.s, u })
                .Join(dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.s.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new { j.t, j.s, j.u, r })
                .Where(j => j.t.ID_PROJECT == idProjectAsLong && j.t.DTA_INVIO != null && j.t.CHA_TIPO_OGGETTO == "F"
                    && j.s.ID_CORR_GLOBALE == idCorrGlobaliAsLong && j.r.CHA_TIPO_RAGIONE == "W"
                    && j.u.ID_PEOPLE == idPeopleAsLong && j.u.CHA_ACCETTATA == "0" && j.u.CHA_RIFIUTATA == "0" && j.u.CHA_VALIDA == "1")
                .Select(x => x.t.SYSTEM_ID)
                .FirstOrDefaultAsync();

            return output;
        }
        #endregion

    }
}
