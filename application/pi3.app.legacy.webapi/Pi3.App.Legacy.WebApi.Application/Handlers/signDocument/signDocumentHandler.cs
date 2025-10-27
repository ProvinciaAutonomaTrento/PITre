// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
extern alias BCrypto1;

using BCrypto1::Org.BouncyCastle.Cms;
using DocsPaVO.documento;
using iText.Kernel.Pdf;
using iText.Signatures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Security.Labels.RetentionLabels.Item.RetentionEventType;
using Pi3.App.Legacy.WebApi.Application.Handlers.getSha256;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Sign;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using static iText.IO.Codec.TiffWriter;
using signDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.signDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.signDocument
{
    public class signDocumentHandler : IRequestHandler<signDocumentRequest, signDocumentResult>
    {
        #region Public Members

        public signDocumentHandler(ILogger<signDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<signDocumentResult> Handle(signDocumentRequest request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

            FileDocumento fd = (await _mediator.Send(new Requests.DocumentoGetFileFirmato(request.massSignature.fileRequest, request.infoUtente))).output;

            byte[] content = fd.content;
            byte[] signedContent = null;
            var isPades = request.massSignature.signPades;

            if (!isPades)
            {
                if (request.massSignature.cosign)
                    content = Pkcs.extractSignedContent(content);

                signedContent = Pkcs.EmbedFileToPkcs(Convert.FromBase64String(request.massSignature.base64Signature), content);
            }
            else
            {
                // signedContent = await this.SignPadesFile(content, Convert.FromBase64String(request.massSignature.base64Signature));
                signedContent = Convert.FromBase64String(request.massSignature.base64Signature);
            }

            request.massSignature.result = (await _mediator.Send(new Requests.AppendDocumentoFirmatoManager(signedContent,
                    request.massSignature.cosign,
                    request.massSignature.fileRequest,
                    request.infoUtente,
                    isPades))).output;

            var isDocInLibroFirma = (await _mediator.Send(new Requests.IsDocInLibroFirma(request.massSignature.fileRequest.docNumber))).output;

            if (request.massSignature.result)
            {
                string method = request.massSignature.signPades ? "DOC_SIGNATURE_P" : "DOC_SIGNATURE";
                string description = request.massSignature.signPades ? Resources.SignDescriptionPADES : Resources.SignDescriptionCADES;

                if (isDocInLibroFirma)
                {
                    await this.AggiornaDataEsecuzioneElemento(request.massSignature.fileRequest.docNumber.AsLong(), DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                    await this.SalvaStoricoIstanzaProcessoFirmaByDocnumber(request.massSignature.fileRequest.docNumber.AsLong(), description, idPeople.AsLong(), userId, groupId, idPeopleDelegato);
                   
                    //Inserisco nella coda del motore di Libro firma
                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = request.massSignature.fileRequest.docNumber,
                            Evento = method,
                        }));
                }

                await this._webMethodLoggerService.LogOK(method, request.massSignature.fileRequest.docNumber, description, null, "PITRE");
            }
            else if(isDocInLibroFirma)
            {
                await this.AggiornaErroreEsitoFirma(request.massSignature.fileRequest.docNumber.AsLong(), "Errore durante la procedura di firma");
            }

            return new signDocumentResult(request.massSignature);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<signDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        private async Task<byte[]> SignPadesFile(byte[] fileContent, byte[] signedHash)
        {
            //CmsSignedData cmsSignedData = new(signedHash);

            //SignerInformationStore signers = cmsSignedData.GetSignerInfos();
            //foreach (SignerInformation signer in signers.GetSigners())
            //{
            //    signedHash = signer.GetSignature();
            //}

            var signatureTime = new DateTime(2025, 04, 04, 12, 00, 00, DateTimeKind.Local);
            string fieldName = $"Signature_fixed";  // Usa un nome fisso per evitare variazioni

            // Apri il documento
            using (var inputStream = new MemoryStream(fileContent))
            using (var tempStream = new MemoryStream()) // Aggiungi una dichiarazione per tempStream
            {
                PdfReader reader = new PdfReader(inputStream);
                PdfSigner signer = new PdfSigner(reader, tempStream, new StampingProperties());

                // Configura il campo firma
                signer.SetFieldName(fieldName);

                // Configura l'aspetto della firma
                // Configura l'aspetto della firma
                var appearance = signer.GetSignatureAppearance()
                    .SetReason("Test Signature")
                    .SetLocation("Test Environment");

                // Calcola l'hash usando un container speciale
                DigestContainer digestContainer = new DigestContainer(new SignaturaData(fieldName, signatureTime));
                signer.SignExternalContainer(digestContainer, 8192);
                byte[] hash = digestContainer.GetHash();
                byte[] preparedPdf = tempStream.ToArray();

                IExternalSignatureContainer container = new HashOnlySignatureContainer(signedHash);

                // Crea uno stream di output per il PDF firmato
                using (var finalStream = new MemoryStream())
                {
                    // IMPORTANTE: Crea un nuovo PdfDocument dal PDF preparato
                    using (PdfReader preparedReader = new PdfReader(new MemoryStream(preparedPdf)))
                    using (PdfDocument pdfDoc = new PdfDocument(preparedReader))
                    {
                        // Applica la firma al documento preparato
                        PdfSigner.SignDeferred(pdfDoc, fieldName, finalStream, container);

                        return finalStream.ToArray();
                    }
                }
            }
        }

        private async Task AggiornaDataEsecuzioneElemento(long docNumber, string stato)
        {
            var elInLfEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(x => x.DOC_NUMBER == docNumber && x.DTA_ESECUZIONE == null)
                .FirstOrDefaultAsync();

            if (elInLfEntity != null)
            {
                elInLfEntity.DOC_NUMBER = docNumber;
                elInLfEntity.STATO_FIRMA = stato;
                elInLfEntity.DTA_ESECUZIONE = DateTime.Now;
            }

            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
        }

        private async Task SalvaStoricoIstanzaProcessoFirmaByDocnumber(long docNumber, string description, long idPeople, string userId, long groupId, long idPeopleDelegato)
        {
            var istanza = await this._dbContext.IstanzaProcessoFirmaEntities
                .Where(x => x.ID_DOCUMENTO == docNumber && x.STATO.Equals("IN_EXEC"))
                .Select(x => new { ID_ISTANZA = x.ID_ISTANZA, CHA_CAMBIO_STATO_DIAG = x.CHA_CAMBIO_STATO_DIAG })
                .FirstOrDefaultAsync();

            if (istanza != null)
            {
                long corrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == groupId).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                var istanzaProcessoFirmaStoEntity = new IstanzaProcFirmaStoEntity()
                {
                    ID_USER = userId,
                    DOC_NUMBER = docNumber,
                    ID_ISTANZA_PROCESSO = istanza.ID_ISTANZA,
                    DTA_DATE = DateTime.Now,
                    VAR_DESC_AZIONE = description.Replace("'", "''"),
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = groupId == 0 ? 0 : corrGlobaliGroup,
                    ID_PEOPLE_DELEGATO = idPeopleDelegato,
                    CHA_CAMBIO_STATO_DIAG = istanza.CHA_CAMBIO_STATO_DIAG
                };

                await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcessoFirmaStoEntity);
                int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
            }
        }

        private async Task AggiornaErroreEsitoFirma(long docNumber, string msgError)
        {
            var elInLfEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(x => x.DOC_NUMBER == docNumber && x.DTA_ESECUZIONE == null)
                .FirstOrDefaultAsync();

            if (elInLfEntity != null)
                elInLfEntity.ERRORE_FIRMA = msgError;

            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
        }

        #endregion
    }

    internal class HashOnlySignatureContainer : IExternalSignatureContainer
    {
        private readonly byte[] _remoteSignature;

        public HashOnlySignatureContainer(byte[] remoteSignature)
        {
            _remoteSignature = remoteSignature;
        }

        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            //signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);
            signDic.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
        }

        public byte[] Sign(Stream data)
        {
            // The hash data can be read here if needed, then return the already-computed signature.
            return _remoteSignature;
        }
    }
}