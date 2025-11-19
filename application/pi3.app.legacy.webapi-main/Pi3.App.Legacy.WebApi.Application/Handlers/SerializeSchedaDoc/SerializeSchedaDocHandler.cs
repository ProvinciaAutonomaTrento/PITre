// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializeSchedaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.SerializeSchedaDoc;
using DocsPaVO.areaConservazione;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using System.Xml.Serialization;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SerializeSchedaDoc
{
    public class SerializeSchedaDocHandler : IRequestHandler<SerializeSchedaDocRequest, SerializeSchedaDocResult>
    {
    #region Private Members
        protected readonly ILogger<SerializeSchedaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        

        private async Task<string> getFullName(string idPeople)
        {
            string? fullName = await this._dbContext.PeopleEntities.AsNoTracking().Where(
                p => p.SYSTEM_ID == idPeople.AsLong()
                ).Select(p => p.FULL_NAME).FirstAsync();

            return fullName == null ? "":fullName;

        }

        private async Task<string> serializeSchedaDoc(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string metadati = string.Empty;
            string result = "-1";
            MemoryStream memoryWriter = null;
            try
            {
                
                Metadati dati = new Metadati(schDoc);
                //Sostituisco al system_id il nome e cognome
                if (dati.protocollatore != null)
                {
                    if (!string.IsNullOrEmpty(dati.protocollatore.Nome_Cognome))
                    {
                        dati.protocollatore.Nome_Cognome = await getFullName(dati.protocollatore.Nome_Cognome);
                    }
                }
                if (dati.creatoreDocumento != null)
                {
                    if (!string.IsNullOrEmpty(dati.creatoreDocumento.Nome_Cognome))
                    {
                        dati.creatoreDocumento.Nome_Cognome = await getFullName(dati.creatoreDocumento.Nome_Cognome);
                    }
                }

                memoryWriter = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Metadati));
                serializer.Serialize(memoryWriter, dati);

                //Devo tornare all'inizio del MemoryStream per leggerne il contenuto!!!
                memoryWriter.Seek(0, SeekOrigin.Begin);
                result = memoryWriter.Length.ToString();
                metadati = new StreamReader(memoryWriter).ReadToEnd();
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            finally
            {
                if (memoryWriter != null)
                {
                    memoryWriter.Flush();
                    memoryWriter.Close();
                }
            }
            return result;
        }

        #endregion

        #region Public Members
        public SerializeSchedaDocHandler(ILogger<SerializeSchedaDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }
        public async Task<SerializeSchedaDocResult> Handle(SerializeSchedaDocRequest request, CancellationToken cancellationToken)
        {
            int size_xml = 0;
            try
            {
                size_xml = System.Convert.ToInt32(await serializeSchedaDoc(request.schDoc));
            }
            catch(Exception ex)
            {
                this._logger.LogDebug($"Errore in SerializeSchedaDoc - {ex.Message}");
            }
            return new SerializeSchedaDocResult(size_xml);
        }
        #endregion

    }
}
