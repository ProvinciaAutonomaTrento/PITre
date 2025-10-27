// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_OpenMultiSignSession
{

    // Richiede libreria MediatR
    public class HSM_OpenMultiSignSessionHandler : IRequestHandler<Requests.HSM_OpenMultiSignSession, HSM_OpenMultiSignSessionResult>
    {
        #region Public Members

        public HSM_OpenMultiSignSessionHandler(ILogger<HSM_OpenMultiSignSessionHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<HSM_OpenMultiSignSessionResult> Handle(Requests.HSM_OpenMultiSignSession request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("HSM_OpenMultiSignSession: INIZIO");

            string result = string.Empty;
            DocsPaVO.documento.FileRequest[] fileRequestList = request.fileRequestList;
            bool cofirma = request.cofirma;
            bool timestamp = request.timestamp;
            string tipoFirma = request.TipoFirma;
            try
            {

                int type = tipoFirma.Equals("CADES") ? 0 : 1;
                string sessionToken = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                var guid = new Guid(sessionToken);

                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                var directory = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads",
                            sessionToken)
                    .PathAsUnixPath();

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                Manifest.SignType st = (Manifest.SignType)type;
                Manifest.ManifestFile m = new Manifest.ManifestFile { SignatureType = st, timestamp = timestamp, Token = sessionToken, cosign = cofirma };

                List<string> tokenList = new List<string>();

                _logger.LogDebug($"HSM_OpenMultiSignSession: INIZIO creazione Manifest.xml {directory}");
                foreach (DocsPaVO.documento.FileRequest fr in fileRequestList)
                {
                    if (fr != null)
                    {
                        _logger.LogDebug($"HSM_OpenMultiSignSession: INIZIO recupero content per il file {fr.docNumber}");

                        DocsPaVO.documento.FileDocumento fd = (await this._mediator.Send(new Requests.DocumentoGetFileFirmato(fr, request.infoUtente))).output;

                        _logger.LogDebug($"HSM_OpenMultiSignSession: INIZIO recupero content per il file {fr.docNumber}");

                        //fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);

                        if (fd != null)
                        {
                            SHA256 mySHA256 = SHA256.Create();
                            string sha256Hash = BitConverter.ToString(fd.content.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                            if (!m.FileInformation.Any(x => x.hash.ToUpper() == sha256Hash.ToUpper()))
                            {
                                string fileName = fr.versionId;
                                string signFileName = Guid.NewGuid().ToString() + fileName + Path.GetExtension(fr.fileName);
                                File.WriteAllBytes(Path.Combine(directory, signFileName), fd.content);
                                m.FileInformation.Add(new Manifest.MainfestFileInformation { hash = sha256Hash.ToUpper(), OriginalFullName = signFileName });
                            }
                            //file esiste nel manifest uscire.

                            fileListToSign.Add(sha256Hash);
                            tokenList.Add(sha256Hash + "§" + fr.docNumber);
                        }
                    }
                }

                

                var filePath = Path.Combine(directory, "Manifest.xml");

                XmlDocument docSave = new XmlDocument();
                docSave.LoadXml(m.ToXmlString());
                docSave.Save(filePath);

                _logger.LogDebug($"HSM_OpenMultiSignSession: FINE creazione Manifest.xml");

                result = sessionToken;
                foreach (string toks in tokenList)
                    result += "|" + toks;

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            _logger.LogDebug("HSM_OpenMultiSignSession: FINE");

            return new HSM_OpenMultiSignSessionResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HSM_OpenMultiSignSessionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected List<String> fileListToSign = new List<string>();

        public class Manifest
        {
            public enum SignType
            {
                CADES,
                PADES
            }

            public class MainfestFileInformation
            {
                public string OriginalFullName;
                public string hash;
                public string SignedFullName;
            }

            public class ManifestFile
            {
                private static System.Xml.Serialization.XmlSerializer serializer;
                public string Token;
                public SignType SignatureType;
                public List<MainfestFileInformation> FileInformation = new List<MainfestFileInformation>();
                public bool cosign;
                public bool timestamp;

                private static System.Xml.Serialization.XmlSerializer Serializer
                {
                    get
                    {
                        if ((serializer == null))
                        {
                            serializer = new System.Xml.Serialization.XmlSerializer(typeof(ManifestFile));
                        }
                        return serializer;
                    }
                }

                public virtual string Serialize()
                {
                    System.IO.StreamReader streamReader = null;
                    System.IO.MemoryStream memoryStream = null;
                    try
                    {
                        memoryStream = new System.IO.MemoryStream();
                        Serializer.Serialize(memoryStream, this);
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        streamReader = new System.IO.StreamReader(memoryStream);
                        return streamReader.ReadToEnd();
                    }
                    finally
                    {
                        if ((streamReader != null))
                        {
                            streamReader.Dispose();
                        }
                        if ((memoryStream != null))
                        {
                            memoryStream.Dispose();
                        }
                    }
                }

                public static ManifestFile Deserialize(string xml)
                {
                    System.IO.StringReader stringReader = null;
                    try
                    {
                        stringReader = new System.IO.StringReader(xml);
                        return ((ManifestFile)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
                    }
                    finally
                    {
                        if ((stringReader != null))
                        {
                            stringReader.Dispose();
                        }
                    }
                }
            }
        }

        #endregion
    }

}
