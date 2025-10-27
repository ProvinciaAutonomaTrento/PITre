// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.Converters;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.File.Converters
{
    public class MockFileConverterService : IFileConverterService
    {
        protected readonly List<string> _supportedFormats = new List<string>()
        {
            ".pdf", // Solo per mock
            ".bmp",".gif",".jpeg",".jpg",".tif",".tiff",".png",".jpf",".jpx",".jp2",".j2k",".j2c",".jpc", //Image
            ".dwg",".dxf",".dwf", //AutoCAD
            ".swf",".flv", //AdobeFlash
            ".xls",".xlsx", //MSExcel
            ".ppt",".pptx", //MSPowerpoint
            ".mpp", //MSProject
            ".pub", //MSPublisher
            ".vsd", //MSVisio
            ".doc",".docx",".rtf",".txt", //MSWord
            ".odt",".odp",".ods",".odg",".odf",".sxw",".sxi",".sxc",".sxd", //OpenOffice
            ".wpd", //WordPerfect
            ".pmd",".pm6",".p65",".pm",//PageMaker
            ".fm", //FrameMaker
            ".psd", //Photoshop
            ".xml", //Notepad
            ".htm", //Htm
            ".html" //Html
        };

        public async Task<FileConvertedContent> Convert(string inputFileFormat, Stream inputFileStream, FileConverterOutputFormatsEnum outputFormat)
        {
            return await this.Convert(inputFileFormat, Resources.Files.Test_documento, outputFormat);
        }

        public async Task<FileConvertedContent> Convert(string inputFileFormat, byte[] inputFileContent, FileConverterOutputFormatsEnum outputFormat)
        {
            return new FileConvertedContent()
            {
                Content = Resources.Files.Test_documento,
                ContentType = "application/pdf",
                FileName = nameof(Resources.Files.Test_documento)
            };
        }

        public async Task<IReadOnlyList<string>> GetSupportedInputFileFormats()
        {
            return _supportedFormats.AsReadOnly();
        }

        public async Task<bool> IsSupportedInputFileFormat(string inputFileFormat)
        {
            return (await GetSupportedInputFileFormats()).Any(f => f.Contains(Path.GetExtension(inputFileFormat)));
        }
    }
}
