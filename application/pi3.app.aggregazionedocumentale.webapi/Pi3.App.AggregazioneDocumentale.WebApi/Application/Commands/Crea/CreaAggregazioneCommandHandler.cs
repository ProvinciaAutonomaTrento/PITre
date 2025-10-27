// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Crea
{
    public class CreaAggregazioneCommandHandler : IRequestHandler<CreaAggregazioneRequest, CreaAggregazioneCommandResponse>
    {
        private readonly IAggregazioneDocumentaleRepository _repository;
        private readonly IPi3DbContext _context;
        private readonly ILogger<CreaAggregazioneCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public CreaAggregazioneCommandHandler(IAggregazioneDocumentaleRepository repository,
            IPi3DbContext context, ILogger<CreaAggregazioneCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService) {
            this._repository = repository;
            this._context = context;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<CreaAggregazioneCommandResponse> Handle(CreaAggregazioneRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            if (request.CreaAggregazione == null)
            {
                throw new NotSupportedPi3Exception(ErrorDescriptions.NullRequest, ErrorDescriptions.ResourceManager);
            }
            if (string.IsNullOrWhiteSpace(request.CreaAggregazione.Descrizione))
            {
                throw new NotSupportedPi3Exception(ErrorDescriptions.InvalidAggregationDescription, ErrorDescriptions.ResourceManager);
            }
            if (string.IsNullOrWhiteSpace(request.CreaAggregazione.Classificazione.TipologiaFascicolo))
            {
                var message = string.Format(ErrorDescriptions.MissingTypeOfFile);
                throw new NotSupportedPi3Exception(message, ErrorDescriptions.ResourceManager);
            }

            if (string.IsNullOrWhiteSpace(request.CreaAggregazione.Classificazione.CodiceClassificazione))
            {
                var message = string.Format(ErrorDescriptions.MissingClassificationCode);
                throw new NotSupportedPi3Exception(ErrorDescriptions.MissingClassificationCode, ErrorDescriptions.ResourceManager);
            }

            // TODO: Aggiungere validazione campi
            var aggregate = new Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.AggregazioneDocumentale(
            idTenant!,
            DateTime.Now,
            new TextValue(request.CreaAggregazione.Descrizione),
            request.CreaAggregazione.TipoAggregazione,
            request.CreaAggregazione.TipologiaFascicolo,
            request.CreaAggregazione.TipologiaVisibilita);
            // tipologia fascicolo può essere null solo per i tipi aggregato SerieDocumentale, per


            // registrazione del registro
            var idRegistro = await DecodificaEAssegnaRegistro();


            await DecodificaEAggiungiClassificazione(request.CreaAggregazione.Classificazione);

            // registrazione documenti
            foreach (var doc in request.CreaAggregazione.Documenti ?? Enumerable.Empty<IdDoc>())
            {
                aggregate.AddIdDoc(new Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
                {
                    Identiticativo = doc.Identiticativo,
                });
            }

            aggregate.CreateFolderHierarchy(AggregateHelpers.FolderHierarcy2ValueObject(request.CreaAggregazione.Sottofascicoli));

            await DecodificaEAssegnaCollocazioneFisica();

            await _repository.Add(aggregate);

            // TODO: definire cosa deve rimandare
            var result = new CreaAggregazioneCommandResponse()
            {
                Id = aggregate.Id
            };
            return result;

            async Task DecodificaEAggiungiClassificazione(Classification classificazione) {
                var pianoConservazioneClassificazione = await this._context.PianoConservazioneEntities
                    .GetPianoConservazioneClassificazione(classificazione.TipologiaFascicolo,
                     classificazione.CodiceClassificazione, idTenant, idRegistro);
                if (pianoConservazioneClassificazione == null)
                {
                    var errorMessageClassificazione = string.Format(ErrorDescriptions.PianoConservazioneLookupFailed,
                        classificazione.CodiceClassificazione, classificazione.TipologiaFascicolo);
                    throw new NotSupportedPi3Exception(errorMessageClassificazione, ErrorDescriptions.ResourceManager);
                }
                else {
                    if (aggregate.TipoAggregazione == Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.TipiAggregazioneEnum.SerieDocumentale)
                    {
                        aggregate.AddClassification(pianoConservazioneClassificazione.ID_CLASSIFICAZIONE.ToString(),
                            new TextValue(pianoConservazioneClassificazione.CODICE_CLASSIFICAZIONE),
                            pianoConservazioneClassificazione.SYSTEM_ID.ToString(),
                            new TextValue(pianoConservazioneClassificazione.TIPOLOGIA_FASCICOLO));
                    }
                    else {
                        aggregate.AddClassification(pianoConservazioneClassificazione.ID_CLASSIFICAZIONE.ToString(),
                            new TextValue(pianoConservazioneClassificazione.CODICE_CLASSIFICAZIONE));
                    }

                }
            }

            async Task DecodificaEAssegnaCollocazioneFisica() {
                var pianoConservazioneCollocazione = await this._context.CorrGlobaliEntities
                    .PianoConservazioneCollocazioneDaCodice(request.CreaAggregazione.CollocazioneFisica.Codice);

                aggregate.AssignCollocazioneFisica(new Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.CollocazioneFisica()
                {
                    Id = pianoConservazioneCollocazione.SYSTEM_ID.ToString(),
                    Descrizione = new TextValue(pianoConservazioneCollocazione.VAR_DESC_CORR),
                    Cartaceo = request.CreaAggregazione.CollocazioneFisica.Cartaceo,
                    DataCollocazione = DateTime.Now,
                });
            }

            async Task<long> DecodificaEAssegnaRegistro() {
                // transcodifica
                //var lookupCodiceRegistro = await this._context.RegistroEntities
                //.AsNoTracking()
                //    .Where(entity => entity.VAR_CODICE == request.CreaAggregazione.CodiceRegistro)
                //    .Select(entity => entity.SYSTEM_ID).FirstOrDefaultAsync();

                var lookupCodiceRegistro = await this._context.RegistroEntities.SystemIdDaCodiceRegistro(request.CreaAggregazione.CodiceRegistro);
                if (lookupCodiceRegistro == 0)
                {
                    throw new NotSupportedPi3Exception(ErrorDescriptions.InvalidAggregationDescription, ErrorDescriptions.ResourceManager);
                }

                return lookupCodiceRegistro;
            }

            //Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy
            //    FolderHierarcy2ValueObject ( FolderHierarcy element)
            //{
            //    var docs = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc>();
            //    var folders = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy>();

            //    foreach (var doc in element.Documenti ?? Enumerable.Empty<IdDoc>())
            //    {
            //        docs.Add(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
            //        {
            //            Identiticativo = doc.Identiticativo
            //        });
            //    }

            //    foreach (var folder in element.Sottofascicoli ?? Enumerable.Empty<FolderHierarcy>())
            //    {
            //        folders.Add(FolderHierarcy2ValueObject(folder));
            //    }

            //    var result = new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy()
            //    {
            //        Name = new TextValue(element.Nome),
            //        IdDocs = docs,
            //        Folders = folders
            //    };
            //    return result;
            //}
        }

    }
}

