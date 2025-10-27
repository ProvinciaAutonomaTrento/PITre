// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;
using StackExchange.Redis;
using System.Globalization;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands
{
    public static class Helpers
    {
        public static async Task ImportaDocumento(
            string repositoryRootPath, 
            string tenantCode, 
            string idTenant, 
            Guid uploadId, 
            string descrizione,
            IDocumentBlobRepository documentBlobRepository, 
            Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo aggregate,
            IUploaderService uploaderService) 
        {
            if (!await uploaderService.UploadExists(uploadId))
                throw new UploadIdNotFoundPi3Exception(uploadId);

            var uploadMetadata = await uploaderService.GetUploadMetadata(uploadId);
            
            DocumentBlob documentBlobAggregate = null!;

            using (var uploadContentStream = await uploaderService.GetUploadedContent(uploadId))
            {
                var fileName = Path.GetFileName(uploadMetadata.FileName);

                documentBlobAggregate = new DocumentBlob(idTenant,
                    uploadMetadata.FinalizationDate, new TextValue(fileName));

                documentBlobAggregate.UploadStream(uploadContentStream, fileName);
                documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await documentBlobRepository.Add(documentBlobAggregate);
            }

            aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
            {
                FileName = documentBlobAggregate.FileName,
                FileSize = documentBlobAggregate.FileSize,
                CreationDate = documentBlobAggregate.CreationDate,
                ContentType = documentBlobAggregate.ContentType,
                Hash = documentBlobAggregate.Hash,
                HashName = Enum.Parse<HashNamesEnum>(documentBlobAggregate.HashName!.ToString()!, true),
                IdBlob = documentBlobAggregate.Id
            },
            new TargetVersionBehavior()
            {
                CreateNewVersion = true,
                Name = new TextValue(descrizione)
            });

            await uploaderService.RemoveUpload(uploadId);
        }

        public static async Task DecodificaEAggiungiClassificazioneNonProtocollato(
                string classificazione, 
                IClaimsPrincipalService claimsPrincipalService,
                IPi3DbContext context, 
                string? idTenant, 
                Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo aggregate)
        {
            var sessionGroupId = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            // -- step 1: REPERIMENTO REGISTRO DEL RUOLO CORRENTE
            var groupEntity = await context.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(cge => cge.ID_GRUPPO == sessionGroupId)
                    .Select(cge => new { cge.SYSTEM_ID })
                    .FirstOrDefaultAsync();

            if (groupEntity == null)
                throw new GroupNotFoundPi3Exception(sessionGroupId.ToString());
            
            var groupId = groupEntity.SYSTEM_ID;

            var systemIdRegistro = await (from der in context.RegistroEntities.AsNoTracking()
                                          join dlrr in context.RuoloRegistroEntities.AsNoTracking() on der.SYSTEM_ID equals dlrr.ID_REGISTRO
                                          where dlrr.ID_RUOLO_IN_UO == groupId && der.DTA_CLOSE == null
                                          select der.SYSTEM_ID)
                                          .FirstOrDefaultAsync();

            // -- step 2: REPERIEMNTO TITOLARIO ATTIVO
            var systemIdTitolario = await context.ProjectEntities
                .AsNoTracking()
                .Where(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
                        && prj.CHA_TIPO_PROJ == "T" 
                        && prj.ID_TITOLARIO == 0 
                        && prj.CHA_STATO == "A")
                .Select(prj => prj.SYSTEM_ID)
                .FirstOrDefaultAsync();

            // --step 3: REPERIMENTO DEL NODO TITOLARIO
            var titolario = await 
                (context.ProjectEntities
                    .AsNoTracking()
                    .Where(prj => prj.VAR_CODICE!.ToUpper() == classificazione.ToUpper()
                        && prj.CHA_TIPO_PROJ == "T" 
                        && prj.ID_REGISTRO == systemIdRegistro 
                        && prj.ID_TITOLARIO == systemIdTitolario)
                .Select(prj => new 
                { 
                    prj.SYSTEM_ID, 
                    prj.DESCRIPTION, 
                    prj.VAR_CODICE 
                }))
                .FirstOrDefaultAsync();

            if (titolario == null)
                throw new TitolarioNotFoundPi3Exception(systemIdTitolario);

            var idProjectRecordF = await context.ProjectEntities
                .AsNoTracking()
                .Where(p => p.ID_PARENT.HasValue && p.ID_PARENT == titolario.SYSTEM_ID
                        && p.CHA_TIPO_FASCICOLO == "G")
                .Select(p => p.SYSTEM_ID)
                .FirstAsync();

            var idProjectRecordC = await context.ProjectEntities
                .AsNoTracking()
                .Where(p => p.ID_PARENT.HasValue && p.ID_PARENT == idProjectRecordF
                        && p.CHA_TIPO_PROJ == "C")
                .Select(p => p.SYSTEM_ID)
                .FirstAsync();

            aggregate.AddClassification(idProjectRecordC!.ToString(),
                    (!string.IsNullOrWhiteSpace(titolario!.DESCRIPTION) ? new TextValue(titolario.DESCRIPTION) : null),
                    titolario.VAR_CODICE);
        }

        public static async Task<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo> GetFullDocument(IDocumentoAmministrativoRepository repository, string idTenant, string idDocumento) {
            var aggregate = await repository.Get(idTenant, idDocumento,
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true
                    }
                });
            return aggregate;
        }


        public static async Task RegistraProfilo(IPi3DbContext context, Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo aggregate, Profilo profilo)
        {
            var tipoAttoEntity = await context.TipoAttoEntities.FirstOrDefaultAsync(ta => ta.VAR_DESC_ATTO.ToUpper() == profilo.Nome.ToUpper());
            if (tipoAttoEntity == null)
                throw new TipoFascicoloNotFoundPi3Exception(profilo.Nome);

            if (aggregate.Profiles != null && aggregate.Profiles.Count > 0 && aggregate.Profiles.FirstOrDefault()?.Id != tipoAttoEntity.SYSTEM_ID.ToString())
            {
                aggregate.RemoveProfile(aggregate.Profiles.FirstOrDefault().Id);
                aggregate.AddProfile(
                    tipoAttoEntity.SYSTEM_ID.ToString(),
                    new TextValue(tipoAttoEntity.VAR_DESC_ATTO));
            }
            else if (aggregate.Profiles == null || aggregate.Profiles.Count == 0) {
                aggregate.AddProfile(
                    tipoAttoEntity.SYSTEM_ID.ToString(),
                    new TextValue(tipoAttoEntity.VAR_DESC_ATTO));
            }

            // Reperimento dei campi presenti nella tipologia
            var oggettiCustomEntities = await (from occ in context.OggettiCustomCompEntities.AsNoTracking()
                                               join oc in context.OggettiCustomEntities.AsNoTracking() on occ.ID_OGG_CUSTOM equals oc.SYSTEM_ID
                                               join to in context.TipoOggettoEntities.AsNoTracking() on oc.ID_TIPO_OGGETTO equals to.SYSTEM_ID
                                               where occ.ID_TEMPLATE == tipoAttoEntity.SYSTEM_ID
                                               select new
                                               {
                                                   SYSTEM_ID_OGG_CUSTOM = oc.SYSTEM_ID,
                                                   DESCRIZIONE_OGG_CUSTOM = oc.DESCRIZIONE,
                                                   SYSTEM_ID_TIPO_OGGETTO = to.SYSTEM_ID,
                                                   DESCRIZIONE_TIPO_OGGETTO = to.DESCRIZIONE
                                               }).ToListAsync();

            foreach (var campo in profilo.Campi)
            {
                ElementFieldValue fieldElement = campo.Tipo switch
                {
                    TipoCampoEnum.CampoDiTesto => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    //                    CasellaSelezione casellaSelezione => new ElementFieldMultiValue(casellaSelezione.Valori.Select(v => new TextValue(v)).ToArray()),
                    TipoCampoEnum.CasellaSelezione => new ElementFieldSingleValue(new TextValue(campo.Valori[0])),
                    TipoCampoEnum.CasellaSelezioneEsclusiva => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.MenuTendina => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.Corrispondente => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.Contatore => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.Data => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.Orario => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    TipoCampoEnum.OrarioSecondi => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                    _ => throw new TipoCampoNotFoundPi3Exception(campo.Tipo.ToString())
                }; //

                if (campo.Tipo == TipoCampoEnum.Corrispondente)
                {

                    var associazione = await context.CorrGlobaliEntities.FirstOrDefaultAsync
                        (itm => itm.VAR_CODICE.ToUpper() == campo.Valore.ToUpper());
                    fieldElement = new ElementFieldSingleValue(new TextValue(associazione.SYSTEM_ID.ToString()));
                }
                var campiData = new TipoCampoEnum[] { TipoCampoEnum.Data, TipoCampoEnum.Orario, TipoCampoEnum.OrarioSecondi };

                if (campiData.Contains(campo.Tipo))
                    _ = Convert.ToDateTime(campo.Valore, new CultureInfo("it-IT"));

                var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == campo.Nome);
                if (oggettoCustomEntity == null)
                    throw new OggettoCustomNotFoundPi3Exception(campo.Nome);

                var esistente = aggregate?.Profiles?.FirstOrDefault()?.Fields?.FirstOrDefault(fld => fld.Id == oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString());

                //if (aggregate.Profiles != null && aggregate.Profiles.Count > 0)
                //    esistente = aggregate.Profiles?.First().Fields.FirstOrDefault(fld => fld.Id == oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString();

                if (esistente == null)
                    aggregate.AddProfileField(tipoAttoEntity.SYSTEM_ID.ToString(),
                            oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                            new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
                            oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
                            fieldElement);
                else
                    aggregate.ChangeProfileFieldValue(tipoAttoEntity.SYSTEM_ID.ToString(),
                            oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                            fieldElement);

            }
        }
    }
}
