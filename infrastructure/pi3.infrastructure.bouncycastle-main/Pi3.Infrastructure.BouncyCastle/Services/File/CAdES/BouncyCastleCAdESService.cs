// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Utilities;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.BouncyCastle.Services.File.TextExtractors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Pi3.Infrastructure.BouncyCastle.Services.File.CAdES
{
    public class BouncyCastleCAdESService : ICAdESService
    {
        #region Public Members

        public BouncyCastleCAdESService(ILogger<BouncyCastleCAdESService> logger)
        {
            this._logger = logger;
        }

        public async Task LoadOriginalFile(string inputFileFormat, Stream signedFile, Stream originalFile)
        {
            var ext = Path.GetExtension(inputFileFormat).Replace(".", "");
            switch(ext.ToUpper())
            {
                case "P7M":
                    await LoadOriginalFileP7M(signedFile, originalFile);
                    break;
                case "TSD":
                    await LoadOriginalFileTSD(signedFile, originalFile);
                    break;
            }
        }

        #endregion

        #region Private Members

        private readonly ILogger<BouncyCastleCAdESService> _logger;

        protected async Task LoadOriginalFileP7M(Stream signedFile, Stream originalFile)
        {
            bool originalContentFounded = false;
            
            Org.BouncyCastle.Cms.CmsSignedData cmsSignedData = null!;

            try
            {
                cmsSignedData = new Org.BouncyCastle.Cms.CmsSignedData(signedFile);
            }
            catch (CmsException)
            {
                originalContentFounded = true;

                signedFile.Position = 0;
                signedFile.CopyTo(originalFile);

                originalFile.Position = 0;
            }
            finally
            {
                if (!originalContentFounded)
                {
                    using var internalFile = new MemoryStream();

                    cmsSignedData.SignedContent.Write(internalFile);
                    internalFile.Position = 0;

                    await this.LoadOriginalFileP7M(internalFile, originalFile);
                }
            }
        }

        protected async Task LoadOriginalFileTSD(Stream signedFile, Stream originalFile)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                signedFile.CopyTo(memoryStream);
                Asn1Sequence sequenza = Asn1Sequence.GetInstance(memoryStream.ToArray());

                DerObjectIdentifier tsdOIDFile = sequenza[0] as DerObjectIdentifier;

                if (tsdOIDFile != null)
                {
                    if (tsdOIDFile.Id == CmsObjectIdentifiers.timestampedData.Id)
                    {
                        DerTaggedObject taggedObject = sequenza[1] as DerTaggedObject;
                        if (taggedObject != null)
                        {
                            Asn1Sequence asn1seq = Asn1Sequence.GetInstance(taggedObject, true);
                            TimeStampedData tsd = TimeStampedData.GetInstance(asn1seq);
                            var file = tsd.Content.GetOctets();

                            using var internalFile = new MemoryStream();
                            tsd.Content.GetOctetStream().CopyTo(internalFile);
                            internalFile.Position = 0;

                            if (internalFile.Length > 0)
                                await LoadOriginalFileP7M(internalFile, originalFile);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                
            }
        }

        #endregion
    }
}
