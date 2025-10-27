// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.DomainEventHandlers;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SerializeSchedaRequest = Pi3.App.Legacy.WebApi.Application.Requests.SerializeScheda;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SerializeScheda
{
    public class SerializeSchedaHandler : IRequestHandler<SerializeSchedaRequest, SerializeSchedaResult>
    {
        #region Public Members

        public SerializeSchedaHandler(IPi3DbContext dbContext, ILogger<SerializeSchedaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<SerializeSchedaResult> Handle(SerializeSchedaRequest request, CancellationToken cancellationToken)
        {
            int output = 0;
            try
            {
                output = System.Convert.ToInt32(await this.SerializeScheda(request.schDoc,request.systemID));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SerializeSchedaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        private async Task<string> SerializeScheda(DocsPaVO.documento.SchedaDocumento schDoc, string systemID)
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
                        dati.protocollatore.Nome_Cognome = await this.GetFullName(dati.protocollatore.Nome_Cognome);
                    }
                }
                if (dati.creatoreDocumento != null)
                {
                    if (!string.IsNullOrEmpty(dati.creatoreDocumento.Nome_Cognome))
                    {
                        dati.creatoreDocumento.Nome_Cognome = await this.GetFullName(dati.creatoreDocumento.Nome_Cognome);
                    }
                }

                memoryWriter = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Metadati));
                serializer.Serialize(memoryWriter, dati);

                //Devo tornare all'inizio del MemoryStream per leggerne il contenuto!!!
                memoryWriter.Seek(0, SeekOrigin.Begin);
                result = memoryWriter.Length.ToString();
                metadati = new StreamReader(memoryWriter).ReadToEnd();

                //Inserisco i metadati XML nel DB nel campo CLOB
                var itemToUpdate = await this._dbContext.ItemConservazioneEntities.Where(i => i.SYSTEM_ID == systemID.AsLong()).FirstOrDefaultAsync();

                if(itemToUpdate != null)
                {
                    itemToUpdate.VAR_XML_METADATI = metadati;
                    try
                    {
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Resource.AddingMetadataKO);
                    }
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(message: ex.Message, exception:ex);
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


        private async Task<string> GetFullName(string idPeople)
        {
            string nomeCognome = await this._dbContext.PeopleEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idPeople.AsLong()).Select(c => c.FULL_NAME).FirstOrDefaultAsync();
            return nomeCognome;
        }


        #endregion
    }
}