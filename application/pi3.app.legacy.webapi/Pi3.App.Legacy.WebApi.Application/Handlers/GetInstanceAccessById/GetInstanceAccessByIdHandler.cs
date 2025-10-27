// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.filtri.LibroFirma;
using DocsPaVO.InstanceAccess;
using DocsPaVO.Modelli_Trasmissioni;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetInstanceAccessByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInstanceAccessById;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInstanceAccessById
{
    public class GetInstanceAccessByIdHandler : IRequestHandler<GetInstanceAccessByIdRequest, GetInstanceAccessByIdResult>
    {
        protected readonly ILogger<GetInstanceAccessByIdHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        private DateTime? Nvl(DateTime? e1, DateTime? e2)
        {
            return !e1.HasValue ? e2 : e1;
        }

        private async Task<string> GetTipologiaAllegato(long? versionId)
        {
            string? retValue = string.Empty;

            retValue = await this._dbContext.VersionEntities.AsNoTracking().Where( v => v.VERSION_ID == versionId).Select( v => v.CHA_ALLEGATI_ESTERNO).FirstOrDefaultAsync();

            return retValue ?? string.Empty;

        }

        private async Task<List<InstanceAccessAttachments>> GetInstanceAccessAttachments(string idInstanceAccessDocument)
        {
            List<InstanceAccessAttachments> listInstanceAccessAttachments = new();
            var rows = await this._dbContext.InstanceAccessAttEntities.AsNoTracking().Where(a => a.ID_INST_ACC_DOC == idInstanceAccessDocument.AsLong()).Select(a => new
            {
                ID_INSTANCE_ACCESS_DOCUMENT = a.ID_INST_ACC_DOC,
                SYSTEM_ID = a.SYSTEM_ID,
                ID_ATTACH = a.ID_ATTACH,
                FILE_NAME = IPi3DbContextMappedFunctions.GetNomeOriginale(a.ID_ATTACH),
                EXTENSION = IPi3DbContextMappedFunctions.GetChaImg(a.ID_ATTACH),
                a.ENABLE
            }).ToListAsync();

            foreach (var row in rows)
            {
                InstanceAccessAttachments instanceAccessAttachments = new InstanceAccessAttachments()
                {
                    SYSTEM_ID = row.SYSTEM_ID != null ? row.SYSTEM_ID.ToString() : string.Empty,
                    ID_INSTANCE_ACCESS_DOCUMENT = row.ID_INSTANCE_ACCESS_DOCUMENT != null ? row.ID_INSTANCE_ACCESS_DOCUMENT.ToString() : string.Empty,
                    ID_ATTACH = row.ID_ATTACH != null ? row.ID_ATTACH.ToString() : string.Empty,
                    FILE_NAME = !string.IsNullOrEmpty(row.FILE_NAME) ? row.FILE_NAME : string.Empty,
                    EXTENSION = !string.IsNullOrEmpty(row.EXTENSION) ? row.EXTENSION : string.Empty,
                    ENABLE = !string.IsNullOrEmpty(row.ENABLE) && row.ENABLE.Trim() == "1" ? true : false
                };
                listInstanceAccessAttachments.Add(instanceAccessAttachments);
            }

            return listInstanceAccessAttachments;
        }
        private async Task<int> GetTypeAttach(string docnumber)
        {
            int type = 0;
            var maxVersion = new List<int?>() { 
                await this._dbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == docnumber.AsLong()).MaxAsync(e => e.VERSION) 
            };
            var versionId = await this._dbContext.VersionEntities.AsNoTracking().Where(c => c.DOCNUMBER == docnumber.AsLong() && maxVersion.Contains(c.VERSION)).Select(v => v.VERSION_ID).FirstOrDefaultAsync();

            if(versionId != null)
            {
                switch (await this.GetTipologiaAllegato(versionId))
                {
                    case "P":
                        type = 2;
                        break;
                    case "I":
                        type = 3;
                        break;
                    case "1":
                        type = 4;
                        break;
                    default:
                        type = 1;
                        break;
                }
            }

            return type;
        }


        private async Task<List<InstanceAccessDocument>> GetInstanceAccessDocuments(string idInstanceAccess, string idAmm)
        {
            List<InstanceAccessDocument> listInstanceAccessDocuments = new List<InstanceAccessDocument>();

            try
            {
                var docs = await (from d in this._dbContext.InstanceAccessDocEntities.AsNoTracking()
                                  join p in this._dbContext.ProfileEntities.AsNoTracking() on d.DOCNUMBER equals p.DOCNUMBER
                                  join pr in this._dbContext.ProjectEntities.AsNoTracking() on d.ID_PROJECT equals pr.SYSTEM_ID into ac
                                  from prj in ac.DefaultIfEmpty()
                                  where d.ID_INST_ACC == idInstanceAccess.AsLong()
                                  orderby d.DOCNUMBER descending
                                  select new
                                  {
                                      ID_INSTANCE_ACCESS_DOCUMENT = d.SYSTEM_ID,
                                      ID_INSTANCE_ACCESS = d.ID_INST_ACC,
                                      DOCNUMBER = d.DOCNUMBER,
                                      ID_PROJECT = d.ID_PROJECT,
                                      TYPE_REQUEST = d.TIPO_RICHIESTA,
                                      OBJECT = p.VAR_PROF_OGGETTO,
                                      HASH = IPi3DbContextMappedFunctions.GetImpronta(d.DOCNUMBER.GetValueOrDefault()),
                                      FILE_NAME = IPi3DbContextMappedFunctions.GetNomeOriginale(d.DOCNUMBER.GetValueOrDefault()),
                                      TYPE_PROTO = p.CHA_TIPO_PROTO,
                                      NUMBER_PROTO = p.NUM_PROTO,
                                      MITT_DEST = IPi3DbContextMappedFunctions.CorrCat(d.DOCNUMBER.GetValueOrDefault(), p.CHA_TIPO_PROTO),
                                      REGISTER = IPi3DbContextMappedFunctions.GetRegDescr(p.ID_REGISTRO.GetValueOrDefault()),
                                      DESCRIPTION_TIPOLOGIA_ATTO = IPi3DbContextMappedFunctions.GetDescTipoDoc(p.ID_TIPO_ATTO.GetValueOrDefault()),
                                      COUNTER_REPERTORY = IPi3DbContextMappedFunctions.GetSegnaturaRepertorio(d.DOCNUMBER.GetValueOrDefault(), idAmm.AsLong()),
                                      EXTENSION = IPi3DbContextMappedFunctions.GetChaImg(d.DOCNUMBER.GetValueOrDefault()),
                                      SIGNED = IPi3DbContextMappedFunctions.GetChaFirmato(d.DOCNUMBER.GetValueOrDefault()),
                                      ID_DOCUMENTO_PRINCIPALE = p.ID_DOCUMENTO_PRINCIPALE,
                                      DATA = this._dbContext.GetSystemDateTime(), // this.Nvl(p.DTA_PROTO, p.CREATION_TIME)
                                      DATA_P = p.DTA_PROTO,
                                      DATA_CR = p.CREATION_TIME,
                                      DESCRIPTION_PROJECT = prj.DESCRIPTION,
                                      ID_FASCICOLO = prj.ID_FASCICOLO,
                                      ID_PARENT = prj.ID_PARENT,
                                      CODE_PROJECT = IPi3DbContextMappedFunctions.GetCodeProject(prj.SYSTEM_ID),
                                      CODE_CLASSIFICATION = IPi3DbContextMappedFunctions.GetCodTit2(prj.ID_PARENT.GetValueOrDefault()),
                                      d.ENABLE
                                  }).ToListAsync();


                foreach(var row in docs)
                {
                    InstanceAccessDocument instanceAccessDocument = new InstanceAccessDocument()
                    {
                        ID_INSTANCE_ACCESS_DOCUMENT = !string.IsNullOrEmpty(row.ID_INSTANCE_ACCESS_DOCUMENT.ToString()) ? row.ID_INSTANCE_ACCESS_DOCUMENT.ToString() : string.Empty,
                        ID_INSTANCE_ACCESS = !string.IsNullOrEmpty(row.ID_INSTANCE_ACCESS.ToString()) ? row.ID_INSTANCE_ACCESS.ToString() : string.Empty,
                        DOCNUMBER = row.DOCNUMBER != null ? row.DOCNUMBER.ToString() : string.Empty,
                        TYPE_REQUEST = row.TYPE_REQUEST ?? string.Empty,
                        INFO_DOCUMENT = new InfoDocument()
                        {
                            DOCNUMBER = row.DOCNUMBER != null ? row.DOCNUMBER.ToString() : string.Empty,
                            OBJECT = !string.IsNullOrEmpty(row.OBJECT) ? row.OBJECT : string.Empty,
                            HASH = !string.IsNullOrEmpty(row.HASH) ? row.HASH : string.Empty,
                            FILE_NAME = !string.IsNullOrEmpty(row.FILE_NAME) ? row.FILE_NAME : string.Empty,
                            TYPE_PROTO = !string.IsNullOrEmpty(row.TYPE_PROTO) ? row.TYPE_PROTO : string.Empty,
                            NUMBER_PROTO = row.NUMBER_PROTO != null ? row.NUMBER_PROTO.ToString() : string.Empty,
                            MITT_DEST = !string.IsNullOrEmpty(row.MITT_DEST) ? row.MITT_DEST : string.Empty,
                            REGISTER = !string.IsNullOrEmpty(row.REGISTER) ? row.REGISTER : string.Empty,
                            DESCRIPTION_TIPOLOGIA_ATTO = !string.IsNullOrEmpty(row.DESCRIPTION_TIPOLOGIA_ATTO) ? row.DESCRIPTION_TIPOLOGIA_ATTO : string.Empty,
                            COUNTER_REPERTORY = !string.IsNullOrEmpty(row.COUNTER_REPERTORY) ? row.COUNTER_REPERTORY.Substring(0, row.COUNTER_REPERTORY.Length - 2) : string.Empty,
                            ID_DOCUMENTO_PRINCIPALE = row.ID_DOCUMENTO_PRINCIPALE != null ? row.ID_DOCUMENTO_PRINCIPALE.ToString() : string.Empty,
                            TYPE_ATTACH = row.ID_DOCUMENTO_PRINCIPALE != null ? await this.GetTypeAttach(row.DOCNUMBER.ToString()) : 0,
                            EXTENSION =  row.EXTENSION ?? string.Empty,
                            IS_SIGNED = !string.IsNullOrEmpty(row.SIGNED) && row.SIGNED.Equals("1") ? true : false,
                            DATE_CREATION = (DateTime)this.Nvl(row.DATA_P, row.DATA_CR)
                        },
                        INFO_PROJECT = row.ID_PROJECT != null ? new InfoProject()
                        {
                            ID_PROJECT = row.ID_PROJECT != null ? row.ID_PROJECT.ToString() : string.Empty,
                            DESCRIPTION_PROJECT = row.DESCRIPTION_PROJECT ??  string.Empty,
                            ID_PARENT = row.ID_PARENT != null ? row.ID_PARENT.ToString() : string.Empty,
                            ID_FASCICOLO = row.ID_FASCICOLO != null ? row.ID_FASCICOLO.ToString() : string.Empty,
                            CODE_PROJECT =  row.CODE_PROJECT ?? string.Empty,
                            CODE_CLASSIFICATION = row.CODE_CLASSIFICATION ?? string.Empty
                        }: null,



                        ENABLE = (!string.IsNullOrEmpty(row.ENABLE.ToString()) && row.ENABLE.ToString().Trim() == "1") ? true : false,
                        ATTACHMENTS = await this.GetInstanceAccessAttachments(row.ID_INSTANCE_ACCESS_DOCUMENT.ToString())

                    };
                    listInstanceAccessDocuments.Add(instanceAccessDocument);
                }

            }
            catch(Exception ex)
            {
                return null;

            }
            return listInstanceAccessDocuments;


        }

        private async Task<DocsPaVO.InstanceAccess.InstanceAccess> GetInstanceAccess(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            InstanceAccess instanceAccess = null;

            var instanceEnt = await (from a in this._dbContext.InstanceAccessEntities.AsNoTracking()
                         join c in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.ID_RICHIEDENTE equals c.SYSTEM_ID into ac
                         from corr in ac.DefaultIfEmpty()
                         where (a.SYSTEM_ID == idInstanceAccess.AsLong())
                         orderby a.DTA_CREAZIONE descending
                         select new
                         {
                             ID_INSTANCE_ACCESS = a.SYSTEM_ID,
                             a.DESCRIPTION,
                             CREATION_DATE = a.DTA_CREAZIONE,
                             CLOSE_DATE = a.DTA_CHIUSURA,
                             ID_PEOPLE_OWNER = a.ID_PEOPLE_PROPRIETARIO,
                             ID_GROUPS_OWNER = a.ID_GRUPPO_PROPRIETARIO,
                             ID_RICHIEDENTE = a.ID_RICHIEDENTE,
                             REQUEST_DATE = a.DTA_RICHIESTA,
                             ID_DOCUMENT_REQUEST = a.ID_DOCUMENTO_RICHIESTO,
                             a.NOTE,
                             STATE_DOWNLOAD_FORWARD = a.CHA_STATO_DOWNLOAD_INOLTRO,
                             DESCRIPTION_RICHIEDENTE = IPi3DbContextMappedFunctions.GetDescCorr(a.ID_RICHIEDENTE.GetValueOrDefault()),
                             CODICE_RUBRICA = corr.VAR_COD_RUBRICA
                         }).FirstOrDefaultAsync();

            if(instanceEnt != null)
            {
                instanceAccess = new InstanceAccess()
                {
                    ID_INSTANCE_ACCESS = !string.IsNullOrEmpty(instanceEnt.ID_INSTANCE_ACCESS.ToString()) ? instanceEnt.ID_INSTANCE_ACCESS.ToString() : string.Empty,
                    DESCRIPTION = !string.IsNullOrEmpty(instanceEnt.DESCRIPTION) ? instanceEnt.DESCRIPTION : string.Empty,
                    CREATION_DATE = instanceEnt.CREATION_DATE,
                    ID_PEOPLE_OWNER = !string.IsNullOrEmpty(instanceEnt.ID_PEOPLE_OWNER.ToString()) ? instanceEnt.ID_PEOPLE_OWNER.ToString() : string.Empty,
                    ID_GROUPS_OWNER = !string.IsNullOrEmpty(instanceEnt.ID_GROUPS_OWNER.ToString()) ? instanceEnt.ID_GROUPS_OWNER.ToString() : string.Empty,
                    RICHIEDENTE = !string.IsNullOrEmpty(instanceEnt.ID_RICHIEDENTE.ToString()) ? new DocsPaVO.utente.Corrispondente()
                    {
                        systemId = instanceEnt.ID_RICHIEDENTE.ToString(),
                        codiceRubrica = instanceEnt.CODICE_RUBRICA,
                        descrizione = !string.IsNullOrEmpty(instanceEnt.DESCRIPTION_RICHIEDENTE) ? instanceEnt.DESCRIPTION_RICHIEDENTE : string.Empty
                    } : null,
                    ID_DOCUMENT_REQUEST = !string.IsNullOrEmpty(instanceEnt.ID_DOCUMENT_REQUEST.ToString()) ? instanceEnt.ID_DOCUMENT_REQUEST.ToString() : string.Empty,
                    NOTE = !string.IsNullOrEmpty(instanceEnt.NOTE) ? instanceEnt.NOTE : string.Empty,
                    STATE_DOWNLOAD_FORWARD = !string.IsNullOrEmpty(instanceEnt.STATE_DOWNLOAD_FORWARD) ?
                        Convert.ToChar(instanceEnt.STATE_DOWNLOAD_FORWARD) : '0'
                };

                if(instanceEnt.REQUEST_DATE != null)
                    instanceAccess.REQUEST_DATE = (DateTime)instanceEnt.REQUEST_DATE;
                if (instanceEnt.CLOSE_DATE != null)
                    instanceAccess.CLOSE_DATE = (DateTime)instanceEnt.CLOSE_DATE;

                if (instanceAccess != null)
                {
                    instanceAccess.DOCUMENTS = await this.GetInstanceAccessDocuments(idInstanceAccess, infoUtente.idAmministrazione);
                }
            }
            return instanceAccess;
        }



        public GetInstanceAccessByIdHandler(
            ILogger<GetInstanceAccessByIdHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext= dbContext;
        }

        public async Task<GetInstanceAccessByIdResult> Handle(GetInstanceAccessByIdRequest request, CancellationToken cancellationToken)
        {
            InstanceAccess output = null;
            try
            {
                output = await GetInstanceAccess(request.idInstanceAccess,request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }


    }
}
