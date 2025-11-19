// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.ExportFascicolo;
using DocsPaVO.fascicolazione;
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GetInfoFascicoloAsXmlRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInfoFascicoloAsXml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInfoFascicoloAsXml
{
    public class GetInfoFascicoloAsXmlHandler : IRequestHandler<GetInfoFascicoloAsXmlRequest, GetInfoFascicoloAsXmlResult>
    {
        #region Public Members

        public GetInfoFascicoloAsXmlHandler(ILogger<GetInfoFascicoloAsXmlHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetInfoFascicoloAsXmlResult> Handle(GetInfoFascicoloAsXmlRequest request, CancellationToken cancellationToken)
        {
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            var metaInfoFascicolo = new MetaInfoFascicolo();

            ExportFascicoloRequest requestInfoFascicolo = ExportFascicoloRequest.Parse(request.requestAsXml);

            Fascicolo fascicolo = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloById(requestInfoFascicolo.IdFascicolo, requestInfoFascicolo.UserInfo))).output;
            Folder folder = (await this._mediator.Send(new Requests.FascicolazioneGetFolder(idUser.ToString(), idGroup.ToString(), fascicolo))).output;

            List<MetaInfoFascicolo> folders = new List<MetaInfoFascicolo>();
            foreach (var child in folder.childs)
            {
                folders.Add(await CreateInfoFascicoloForFolder(child, idUser, idGroup));
            }

            metaInfoFascicolo.Id = fascicolo.systemID;
            metaInfoFascicolo.Nome = GetValidName(fascicolo.codice);
            metaInfoFascicolo.Documenti = await GetInfoDocumenti(folder, idUser, idGroup);
            metaInfoFascicolo.Fascicoli = folders.ToArray();

            var output = metaInfoFascicolo.ToXmlString(true);

            return new GetInfoFascicoloAsXmlResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInfoFascicoloAsXmlHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        protected IMapper _mapper = null;

        protected async Task<MetaInfoFascicolo> CreateInfoFascicoloForFolder(Folder folder, long idUser, long idGroup)
        {
            MetaInfoFascicolo infoFascicolo = new MetaInfoFascicolo
            {
                Id = folder.systemID,
                Nome = GetValidName(folder.descrizione)
            };

            infoFascicolo.Documenti = await GetInfoDocumenti(folder, idUser, idGroup);

            List<MetaInfoFascicolo> folders = new List<MetaInfoFascicolo>();
            foreach (var child in folder.childs)
            {
                folders.Add(await CreateInfoFascicoloForFolder(child, idUser, idGroup));
            }

            infoFascicolo.Fascicoli = folders.ToArray();

            return infoFascicolo;
        }

        protected async Task<MetaInfoDocumento[]> GetInfoDocumenti(Folder folder, long idUser, long idGroup)
        {
            List<MetaInfoDocumento> documenti = new List<MetaInfoDocumento>();

            var profileEntity = await _dbContext.ProfileEntities
                .Join(_dbContext.SecurityEntities,
                    p => p.SYSTEM_ID,
                    s => s.THING,
                    (p, s) => new { p, s })
                .Join(_dbContext.ProjectComponentEntities,
                    j => j.p.SYSTEM_ID,
                    pc => pc.LINK,
                    (j, pc) => new { j.p, j.s, pc })
                .Where(j => j.pc.TYPE == "D"
                    && j.pc.PROJECT_ID == folder.systemID.AsLong()
                    && (j.s.PERSONORGROUP == idUser || j.s.PERSONORGROUP == idGroup) && j.s.ACCESSRIGHTS >= 0
                    && (j.p.CHA_IN_CESTINO ?? "0") == "0")
                .Select(j => new
                {
                    j.p.SYSTEM_ID,
                    j.p.VAR_SEGNATURA,
                    j.p.CHA_TIPO_PROTO
                })
                .Distinct()
                .ToListAsync();

            profileEntity.ForEach(p =>
            {
                var idLastVersion = _dbContext.VersionEntities.AsNoTracking()
                .Where(v => v.DOCNUMBER == p.SYSTEM_ID)
                .Max(v => v.VERSION_ID);

                var componentsEntity = _dbContext.ComponentEntities.AsNoTracking()
                .Where(c => c.VERSION_ID == idLastVersion && c.DOCNUMBER == p.SYSTEM_ID)
                .Select(c => new
                {
                    c.VAR_NOMEORIGINALE,
                    c.PATH
                })
                .FirstOrDefault();

                var timestampEntity = _dbContext.TimestampDocEntities.AsNoTracking()
                    .Where(t => t.VERSION_ID == idLastVersion && t.DOC_NUMBER == p.SYSTEM_ID)
                    .OrderByDescending(t => t.DTA_CREAZIONE)
                    .FirstOrDefault();

                if (timestampEntity != null && !string.IsNullOrEmpty(timestampEntity.TSR_FILE))
                {
                    documenti.Add(new MetaInfoDocumento()
                    {
                        Id = "TSR_" + p.SYSTEM_ID.ToString(),
                        Nome = CleanOriginalFileNameDocumento(!string.IsNullOrEmpty(p.VAR_SEGNATURA) ? p.VAR_SEGNATURA : p.SYSTEM_ID.ToString(), componentsEntity.VAR_NOMEORIGINALE ?? string.Empty) + ".TSR",
                        FullName = componentsEntity.PATH + ".TSR",
                        IsAllegato = false,
                        IsProtocollo = false
                    });
                }
                else
                {
                    documenti.Add(new MetaInfoDocumento()
                    {
                        Id = p.SYSTEM_ID.ToString(),
                        Nome = CleanOriginalFileNameDocumento(!string.IsNullOrEmpty(p.VAR_SEGNATURA) ? p.VAR_SEGNATURA : p.SYSTEM_ID.ToString(), componentsEntity.VAR_NOMEORIGINALE ?? string.Empty),
                        FullName = componentsEntity.PATH ?? string.Empty,
                        IsAllegato = false,
                        IsProtocollo = p.CHA_TIPO_PROTO != "G"
                    });
                }

                var allegatiEntities = _dbContext.ProfileEntities.AsNoTracking()
                .Where(a => a.ID_DOCUMENTO_PRINCIPALE == p.SYSTEM_ID)
                .Select(a => new
                {
                    a.SYSTEM_ID
                })
                .ToList();

                allegatiEntities.ForEach(a =>
                {
                    var idLastVersionAllegato = _dbContext.VersionEntities.AsNoTracking()
                       .Where(v => v.DOCNUMBER == a.SYSTEM_ID)
                       .Max(v => v.VERSION_ID);

                    var componentsAllegatoEntity = _dbContext.ComponentEntities.AsNoTracking()
                    .Where(c => c.VERSION_ID == idLastVersionAllegato && c.DOCNUMBER == a.SYSTEM_ID)
                    .Select(c => new
                    {
                        c.VAR_NOMEORIGINALE,
                        c.PATH
                    })
                    .FirstOrDefault();

                    var timestampAllegatoEntity = _dbContext.TimestampDocEntities.AsNoTracking()
                        .Where(t => t.VERSION_ID == idLastVersionAllegato && t.DOC_NUMBER == a.SYSTEM_ID)
                        .OrderByDescending(t => t.DTA_CREAZIONE)
                        .FirstOrDefault();

                    if (timestampAllegatoEntity != null && !string.IsNullOrEmpty(timestampAllegatoEntity.TSR_FILE))
                    {
                        documenti.Add(new MetaInfoDocumento()
                        {
                            Id = "TSR_" + a.SYSTEM_ID.ToString(),
                            Nome = CleanOriginalFileNameDocumento(a.SYSTEM_ID.ToString(), componentsAllegatoEntity.VAR_NOMEORIGINALE ?? string.Empty) + ".TSR",
                            FullName = componentsAllegatoEntity.PATH + ".TSR",
                            IsAllegato = false,
                            IsProtocollo = false
                        });
                    }
                    else
                    {
                        documenti.Add(
                        new DocsPaVO.ExportFascicolo.MetaInfoDocumento
                        {
                            Id = a.SYSTEM_ID.ToString(),
                            IsAllegato = true,
                            IsProtocollo = false,
                            FullName = componentsAllegatoEntity.PATH ?? string.Empty,
                            Nome = CleanOriginalFileNameDocumento(a.SYSTEM_ID.ToString(), componentsAllegatoEntity.VAR_NOMEORIGINALE ?? string.Empty)
                        });
                    }
                });
            });

            return documenti.ToArray();
        }

        /// <summary>
        /// Questa funzione si occupa di sostituire i caratteri non validi
        /// per il salvataggio di un file su file system con un carattere -
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetValidName(string name)
        {
            string retVal = name;

            retVal = retVal.Replace("\\", "_");
            retVal = retVal.Replace("/", "_");
            retVal = retVal.Replace(":", "_");
            retVal = retVal.Replace("*", "_");
            retVal = retVal.Replace("?", "_");
            retVal = retVal.Replace("\"", "_");
            retVal = retVal.Replace("<", "_");
            retVal = retVal.Replace(">", "_");
            retVal = retVal.Replace("|", "_");

            return retVal;
        }

        protected string CleanOriginalFileNameDocumento(string nomeDocumento, string originalFileName)
        {
            nomeDocumento = GetValidName(nomeDocumento);
            originalFileName = Path.GetFileNameWithoutExtension(originalFileName);
             string nameSepa = "_";
            if (originalFileName.Contains(nomeDocumento))
                originalFileName = originalFileName.Replace(nomeDocumento, String.Empty);

            if (originalFileName.EndsWith("_"))
                originalFileName = originalFileName.Remove(originalFileName.Length - 1);

            string nome = String.Format("{0}{1}{2}", nomeDocumento, nameSepa, originalFileName);
            return nome;
        }


        #endregion
    }
}