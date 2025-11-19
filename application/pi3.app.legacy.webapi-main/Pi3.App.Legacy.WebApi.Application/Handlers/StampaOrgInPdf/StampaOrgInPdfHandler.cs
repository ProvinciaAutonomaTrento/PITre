// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocumentFormat.OpenXml.Packaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StampaOrgInPdfRequest = Pi3.App.Legacy.WebApi.Application.Requests.StampaOrgInPdf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.StampaOrgInPdf
{
    public class StampaOrgInPdfHandler : IRequestHandler<StampaOrgInPdfRequest, StampaOrgInPdfResult>
    {
        protected readonly IPi3DbContext _dbContext;
        protected readonly ILogger<StampaOrgInPdfHandler> _logger;
        protected readonly IFileConverterService _fileConverterService;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IConfigurationService _configurationService;

        public class ORGANIGRAMMA
        {
            [XmlAttribute]
            public string title { get; set; }
            [XmlElement]
            public recordOrg[] RECORD;

            public class recordOrg
            {
                [XmlAttribute]
                public string tipo { get; set; }
                [XmlAttribute]
                public string desc { get; set; }
            }
        }



        public StampaOrgInPdfHandler(
            IPi3DbContext dbContext,
            ILogger<StampaOrgInPdfHandler> logger,
            IFileConverterService fileConverterService,
            IReportGeneratorService reportGeneratorService,
            IConfigurationService configurationService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._fileConverterService = fileConverterService;
            this._reportGeneratorService = reportGeneratorService;
            this._configurationService = configurationService;
        }


        public async Task<StampaOrgInPdfResult> Handle(StampaOrgInPdfRequest request,CancellationToken cancellationToken)
        {
            FileDocumento output = new DocsPaVO.documento.FileDocumento();

            try
            {
                string xml = request.xmlDoc.InnerXml;
                ORGANIGRAMMA org = null;
                using (TextReader tr = new StringReader(xml))
                {
                    XmlSerializer SerializerObj = new XmlSerializer(typeof(ORGANIGRAMMA));
                    org = (ORGANIGRAMMA)SerializerObj.Deserialize(tr);
                }


                var rm = new ReportModel()
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape,
                    OutputType = ReportOutputTypes.AsPdf
                };


                
                int lineCount = 0;
                int totPages = (org.RECORD.Length / 44) + 1;
                int currPage = 1;
                string titoloReport = org.title;
                string rows = "";
                foreach (ORGANIGRAMMA.recordOrg r in org.RECORD)
                {
                    if (lineCount == 0)
                    {
                        string header = String.Format("{0} - Pagina: {1} di {2}", titoloReport, currPage++, totPages);
                        rm.AddSection(new TextSectionModel()
                        {
                            Style = new TextSectionStyleModel()
                            {
                                Justification = Justifications.Left
                            },
                            Content = new TextContentModel()
                            {
                                Value = header,
                                Style = new TextStyleModel()
                                {
                                    FontName = "Courier",
                                    FontSize = 6,
                                    FontIsBold = false,
                                    FontColor = System.Drawing.Color.Black
                                }
                            }
                        });
                    }

                    string str = String.Format("{0}", r.desc);
                    
                    rm.AddSection(new TextSectionModel()
                    {
                        Style = new TextSectionStyleModel()
                        {
                            Justification = Justifications.Left
                        },
                        Content = new TextContentModel()
                        {
                            Value = str,
                            Style = new TextStyleModel()
                            {
                                FontName = "Courier",
                                FontSize = 6,
                                FontIsBold = true,
                                FontColor = System.Drawing.Color.Black
                            }
                        }
                    });
                    lineCount++;

                }

                using MemoryStream stream = new();
                var generatedReport = await this._reportGeneratorService.Generate(rm, stream);
                var content = stream.ToArray();

                output.estensioneFile = "pdf";
                output.name = "stampaOrganigramma";
                output.fullName = "stampaOrganigramma.pdf";
                output.length = content.Length;
                output.contentType = "application/pdf";
                output.content = content;
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
