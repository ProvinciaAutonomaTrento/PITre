// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Text;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Services.File.SigilloElettronico;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddSegnaturaXml
{
    public class AddSegnaturaXmlCommandHandler : IRequestHandler<AddSegnaturaXmlCommand, AddSegnaturaXmlCommandResponse>
    {
        #region Public Members
        public AddSegnaturaXmlCommandHandler(
            ILogger<AddSegnaturaXmlCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IConfigurationService configurationService,
            ISigilloElettronicoService sigilloElettronicoService,
            IDocumentBlobRepository documentBlobRepository
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._configurationService = configurationService;
            this._sigilloElettronicoService = sigilloElettronicoService;
            this._documentBlobRepository = documentBlobRepository;
        }

        public async Task<AddSegnaturaXmlCommandResponse> Handle(AddSegnaturaXmlCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isFirmato = false;
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);

                var aggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.DocNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = true,
                        LoadNote = true,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
                });

                var segnaturaXml = aggregate.GetSegnaturaXml();
                byte[] contentSegnaturaXML = Encoding.UTF8.GetBytes(segnaturaXml);

                var keyApplicaSigillo = await this._configurationService.GetValue<string>(idTenant, "BE_APPLICA_SIGILLO_SEGNATURA");
                if (keyApplicaSigillo == "1")
                {
                    var idRegistro = aggregate.DatiRegistrazione.IdRegistro.AsLong();
                    var codiceUnivocoAOOIpa = await this._dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == idRegistro).Select(r => r.VAR_CODICE_IPA).FirstAsync();

                    try
                    {
                        var response = await this._sigilloElettronicoService.SignXml(new SignXMLType()
                        {
                            FileDaFirmare = contentSegnaturaXML,
                            CodiceAOOIPA = codiceUnivocoAOOIpa,
                            CodiceEnteIPA = aggregate.Amministrazione.PAI.Amministrazione.CodiceIPA
                        });

                        if (response.FileFirmato != null && response.FileFirmato.Count() > 0)
                        {
                            isFirmato = true;
                            contentSegnaturaXML = response.FileFirmato;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception: ex, message: ex.Message, "Errore applicazione sigillo segnatura.xml");
                    }
                }

                var aggregateAllegatoSegnatura = new DocumentoAmministrativo(idTenant.ToString(),
                                DateTime.Now,
                                new OggettoDelDocumento()
                                {
                                    Descrizione = new TextValue(Resources.AllegatoSegnaturaXML)
                                },
                                null, null, aggregate.TipologiaVisibilita,
                                new IdDoc()
                                {
                                    Identiticativo = aggregate.Id
                                });

                aggregateAllegatoSegnatura.ChangeTipologiaAllegato(TipologieAllegatiEnum.Segnatura);

                var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue("segnatura.xml"));
                newDocumentBlobAggregate.UploadStream(new MemoryStream(contentSegnaturaXML), "segnatura.xml");
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await _documentBlobRepository.Add(newDocumentBlobAggregate);

                aggregateAllegatoSegnatura.AssignDocumentBlobRef(
                    new DocumentBlobRef()
                    {
                        IdBlob = newDocumentBlobAggregate.Id,
                        FileName = newDocumentBlobAggregate.FileName,
                        ContentType = newDocumentBlobAggregate.ContentType,
                        FileSize = newDocumentBlobAggregate.FileSize,
                        CreationDate = await _dbContext.GetSystemDateTime(),
                        Hash = newDocumentBlobAggregate.Hash,
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                        Cartaceo = false,
                        SegnaturaPermanente = false,
                        TipoFirma = isFirmato ? TipoFirmaEnum.Xades : TipoFirmaEnum.Nessuna
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true
                    });

                await _documentoAmministrativoRepository.Add(aggregateAllegatoSegnatura);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new AddSegnaturaXmlCommandResponse();
        }

        #endregion


        #region Private Members

        protected readonly ILogger<AddSegnaturaXmlCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISigilloElettronicoService _sigilloElettronicoService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        #endregion
    }
}
