// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using iText.Kernel.Pdf;
using iText.Signatures;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Sign;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetFileFirmatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato;
using getSha256Request = Pi3.App.Legacy.WebApi.Application.Requests.getSha256;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getSha256
{
    public class getSha256Handler : IRequestHandler<getSha256Request, getSha256Result>
    {
        #region Public Members

        public getSha256Handler(ILogger<getSha256Handler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<getSha256Result> Handle(getSha256Request request, CancellationToken cancellationToken)
        {
            var massSignature = request.massSignature;

            var getFileFirmatoResult = await this._mediator.Send(new DocumentoGetFileFirmatoRequest(massSignature.fileRequest, request.infoUtente));

            DocsPaVO.documento.FileDocumento fd = getFileFirmatoResult.output;

            if (!massSignature.signPades)
            {
                if (massSignature.cosign)
                {
                    //logger.Debug("MassSignature 2");
                    byte[] signatureFile = Pkcs.EmbedFileToPkcs(fd.content, null);
                    massSignature.base64Signature = Convert.ToBase64String(signatureFile);
                    massSignature.base64Sha256 = Convert.ToBase64String(Pkcs.getHashFromSignature(fd.content));
                }
                else
                {
                    massSignature.base64Sha256 = fd.content.ComputeHashAsSha256String();
                }
            }
            else
            {
                // var hashToSign = await this.ComputeHashToSign(fd.content);

                massSignature.base64Sha256 = Convert.ToBase64String(fd.content);
            }

            return new getSha256Result(massSignature);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<getSha256Handler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        private async Task<byte[]> ComputeHashToSign(byte[] fileContent)
        {
            var signatureTime = new DateTime(2025, 04, 04, 12, 00, 00, DateTimeKind.Local);
            string fieldName = $"Signature_fixed";  // Usa un nome fisso per evitare variazioni

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

                byte[] hashToSign = digestContainer.GetHash();

                return hashToSign;
            }
        }

        #endregion
    }

    internal record SignaturaData(string signatureFieldName, DateTime signatureDateTime);

    /// <summary>
    /// DigestContainer to compute the hash that needs to be signed remotely
    /// </summary>
    internal class DigestContainer : IExternalSignatureContainer
    {
        private byte[] hash;
        private readonly SignaturaData signatureData;


        /// <summary>
        /// Costruttore senza parametri con valori predefiniti
        /// </summary>
        public DigestContainer()
        {
            // Usa valori predefiniti per i parametri di firma
            this.signatureData = new SignaturaData(
                $"Signature_{Guid.NewGuid():N}", // Nome campo univoco
                DateTime.Now); // Data e ora corrente
        }

        public DigestContainer(SignaturaData signatureData)
        {
            this.signatureData = signatureData;
        }

        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            //signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);

            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            //signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached); // Changed from Adbe_pkcs7_detached
            signDic.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
                                                                         //signDic.Put(PdfName.M, new PdfDate(DateTime.Now).GetPdfObject());
            signDic.Put(PdfName.M, new PdfDate(signatureData.signatureDateTime).GetPdfObject());
        }

        public byte[] Sign(Stream data)
        {
            using (var ms = new MemoryStream())
            {
                data.CopyTo(ms);
                // Store the raw data as the hash - this is what iText expects
                // Do NOT calculate SHA-256 here as the SignHashRemotely method expects raw data
                hash = ms.ToArray();
            }

            // Return a dummy signature - this won't be used since we're just calculating the hash
            return new byte[0];
        }

        public byte[] GetHash()
        {
            return hash;
        }
    }
}