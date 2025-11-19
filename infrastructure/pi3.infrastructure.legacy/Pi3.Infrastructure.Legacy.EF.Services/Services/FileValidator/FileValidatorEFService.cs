// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using FileTypeChecker.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using static System.Net.WebRequestMethods;
using iText.StyledXmlParser.Jsoup.Helper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator
{
    public class FileValidatorEFService : IFileValidatorService
    {       

        #region Public Members

        public FileValidatorEFService(
            ILogger<FileValidatorEFService> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            //ICAdESService cAdESService,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            //this._cAdESService = cAdESService;
            this._dbContext = dbContext;
        }

        public async Task<FileValidationResult> Validate(FileToValidate fileToValidate)
        {
            fileToValidate = fileToValidate ?? throw new ArgumentNullException(nameof(fileToValidate));
            Validator.ValidateObject(fileToValidate, new ValidationContext(fileToValidate));
            
            var filename = fileToValidate.Name;
            var internalFileName = string.Empty;
            var fileExtension = Path.GetExtension(filename).Replace(".", string.Empty).ToUpperInvariant();
            var internalFileExtension = string.Empty;

            var result = new FileValidationResult()
            {
                NameIsValid = true
            };

            result.FormatIsAdmitted = await this.IsFormatAdmitted(fileExtension);

            var isCAdES = this.IsCAdESExtension(fileToValidate.Name);

            if (isCAdES.Item1)
            {
                internalFileName = isCAdES.Item2;
                internalFileExtension = Path.GetExtension(internalFileName).Replace(".", string.Empty).ToUpperInvariant();

                result.FormatIsAdmitted = result.FormatIsAdmitted && await this.IsFormatAdmitted(internalFileExtension);
            }

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (fileToValidate.Name.Contains(c))
                {
                    result.NameIsValid = false;
                    break;
                }
            }

            if (fileToValidate.Stream != null)
            {
                var isCompliantToFormat = false;
                var hasForms = false;
                var lastPosition = fileToValidate.Stream.Position;
                fileToValidate.Stream.Position = 0;

                try
                {

                    if (!isCAdES.Item1 && !_formatsNotAdmittedByLibrary.Contains(fileExtension))
                    {
                        var matchResult = FileTypeChecker.FileTypeValidator.TryGetFileType(fileToValidate.Stream);

                        if (matchResult.HasMatch)
                        {
                            isCompliantToFormat = ((matchResult.Type.Extension ?? string.Empty)
                            .Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase));
                        }
                    }
                    else
                    {
                        // La libreria FileTypeChecker non interpreta file cades e formati EML, HTML ecc
                        switch(fileExtension)
                        {
                            case "XML":
                                isCompliantToFormat = IsXml(fileToValidate.Stream);
                                break;
                            default:
                                isCompliantToFormat = true;
                                break;
                        }
                    }
                    // Giorgio - cofntrollo campi form
                    if (isCompliantToFormat && fileExtension == "PDF")
                    {
                        try
                        {
                            fileToValidate.Stream.Position = 0;
                            using var pdfReader = new PdfReader(fileToValidate.Stream);
                            using var pdfDoc = new PdfDocument(pdfReader);
                            var acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);
                            var fields = acroForm?.GetAllFormFields();

                            if (fields != null && fields.Count > 0)
                            {
                                hasForms = fields.Any(f => f.Value.GetFormType() == PdfName.Btn
                                                        || f.Value.GetFormType() == PdfName.Tx
                                                        || f.Value.GetFormType() == PdfName.Ch);
                            }
                        }
                        catch(Exception ex)
                        {
                            this._logger.LogCritical(exception: ex, message: ex.Message);
                        }
                    }
                    // ─────────────────────────────────────────────────────────────  

                    //FileTypeChecker.MatchResult matchResult = null!;

                    //if (isCAdES.Item1)
                    //{
                    //    try
                    //    {
                    //        using var streamToValidate = new MemoryStream();
                    //        await this._cAdESService.LoadOriginalFile(internalFileExtension, fileToValidate.Stream, streamToValidate);

                    //        matchResult = FileTypeChecker.FileTypeValidator.TryGetFileType(streamToValidate);
                    //    }
                    //    catch
                    //    { }
                    //}
                    //else
                    //    matchResult = FileTypeChecker.FileTypeValidator.TryGetFileType(fileToValidate.Stream);

                    //if (matchResult != null && matchResult.HasMatch)
                    //{
                    //    isCompliantToFormat = ((matchResult.Type.Extension ?? string.Empty)
                    //        .Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase));
                    //}
                }
                catch
                { }
                finally
                {
                    fileToValidate.Stream.Position = lastPosition;
                }

                result.Compliance = new FileValidationCompliance()
                {
                    IsCompliantToFormat = isCompliantToFormat,
                    HasMacro = _formatsWithVbaMacros.Contains(fileExtension),
                    IsExecutable = _executableFormats.Contains(fileExtension),
                    HasForms = hasForms
                };
            }

            return result;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FileValidatorEFService> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        //protected readonly ICAdESService _cAdESService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly string[] _executableFormats =
        {
            "EXE", "BAT","CMD", "MSI", "COM", "VBS","P1", //Windows
            "APP", "COMMAND", "SH", "PKG", //macOS
            "OUT", "BIN", "SH, \"RUN", //Linux
            "ELF", "APK" //Altri sistemi operativi:
        };

        protected readonly string[] _formatsWithVbaMacros =
        {
            "DOCM", "PPTM", "XLSM"
        };

        //La libreria FileTypeChecker non interpreta correttamente queste estensioni
        protected readonly string[] _formatsNotAdmittedByLibrary =
        {
            "EML", "HTML", "XML"
        };
      

        protected async Task<bool> IsFormatAdmitted(string fileFormat)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            return await this._dbContext.FormatoDocumentoEntities.AsNoTracking()
                                       .Where(f => f.ID_AMMINISTRAZIONE == idTenant
                                           && f.FILE_EXTENSION.ToUpper() == fileFormat.ToUpper()
                                           && f.FILE_TYPE_USED == 1)
                                       .AnyAsync();
        }


        protected bool IsXml(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Legge i primi 512 byte
            byte[] buffer = new byte[512];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Prova a interpretare come testo UTF-8
            string header;
            try
            {
                header = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch
            {
                return false; // Non è testo valido
            }

            // Rimuove BOM e spazi iniziali
            header = header.TrimStart('\uFEFF', ' ', '\t', '\r', '\n');

            // Controlla se il primo carattere utile è '<'
            if (header.StartsWith("<"))
                return true;

            return false;
        }


        protected virtual (bool, string) IsCAdESExtension(string fileName)
        {
            var isSigned = false;
            var filename = fileName;

            if (filename.ToUpper().EndsWith("P7M") ||
                filename.ToUpper().EndsWith("TSD") ||
                filename.ToUpper().EndsWith("M7M"))
            {
                isSigned = true;
                filename = filename.Remove(filename.LastIndexOf("."));

                while (filename.LastIndexOf(".") > -1)
                {
                    if (!filename.ToUpper().EndsWith("P7M") &&
                        !filename.ToUpper().EndsWith("TSD") &&
                        !filename.ToUpper().EndsWith("M7M"))
                        break;

                    filename = filename.Remove(filename.LastIndexOf("."));
                }

                //Vado a rimuovere il (1) aggiunto dai browser
                if (filename.EndsWith(")") && filename.LastIndexOf("(") != -1)
                    filename = filename.Remove(filename.LastIndexOf("("));
            }

            return (isSigned, filename);
        }

        #endregion
    }
}
