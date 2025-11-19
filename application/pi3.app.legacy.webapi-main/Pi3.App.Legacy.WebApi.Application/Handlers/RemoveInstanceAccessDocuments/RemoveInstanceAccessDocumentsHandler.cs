// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.InstanceAccess;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RemoveInstanceAccessDocumentsRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveInstanceAccessDocuments;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveInstanceAccessDocuments
{
    public class RemoveInstanceAccessDocumentsHandler : IRequestHandler<RemoveInstanceAccessDocumentsRequest, RemoveInstanceAccessDocumentsResult>
    {
        protected readonly ILogger<RemoveInstanceAccessDocumentsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IMapper _mapper = null;

        public RemoveInstanceAccessDocumentsHandler(
            ILogger<RemoveInstanceAccessDocumentsHandler> logger,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this.InitializeMapper();

        }


        private async Task<bool> RemoveInstanceAccessAttachments(string idInstanceAccessDocument)
        {
            bool result = true;
            int rowAffected;


            try
            {
                InstanceAccessAttEntity? entToDelete = await this._dbContext.InstanceAccessAttEntities.AsNoTracking().
                    Where(att => att.SYSTEM_ID == idInstanceAccessDocument.AsLong()).FirstOrDefaultAsync();

                if (entToDelete != null)
                {
                    this._dbContext.InstanceAccessAttEntities.Remove(entToDelete);
                    rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (rowAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;

            }
            return result;

        }
        
        private async Task<bool> RemoveInstanceAccess(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            int rowAffected = 0;

            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            try
            {
                if (listInstanceAccessDocuments != null && listInstanceAccessDocuments.Count > 0)
                {
                    foreach (InstanceAccessDocument instanceAccessDocument in listInstanceAccessDocuments)
                    {
                        InstanceAccessDocEntity? entToDelete = await this._dbContext.InstanceAccessDocEntities.AsNoTracking().
                            Where(doc => doc.SYSTEM_ID == instanceAccessDocument.ID_INSTANCE_ACCESS_DOCUMENT.AsLong()).FirstOrDefaultAsync();
                        
                        if(entToDelete != null)
                        {
                            this._dbContext.InstanceAccessDocEntities.Remove(entToDelete);
                            rowAffected = await((DbContext)this._dbContext).SaveChangesAsync();

                            if (rowAffected > 0)
                            {
                                if (instanceAccessDocument.ATTACHMENTS != null && instanceAccessDocument.ATTACHMENTS.Count > 0)
                                {
                                    if (!await this.RemoveInstanceAccessAttachments(instanceAccessDocument.ID_INSTANCE_ACCESS_DOCUMENT))
                                    {
                                        result = false;
                                        break;
                                    }
                                }    
                            }
                            else
                            {
                                result = false;
                                break;
                            }
                        }

                    }
                }

                if (result)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            catch(Exception ex )
            {
                transaction.Rollback();
                result = false;

            }

            return result;
        }

        private async Task<DocsPaVO.documento.SchedaDocumento> GetDettaglio(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber)
        {
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true).AsLong();
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true).AsLong();
            var idAmministrazioneAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var idDocumentoPrincipale = await this._dbContext.ProfileEntities
            .AsNoTracking()
            .Where(p => p.SYSTEM_ID == idProfile.AsLong())
            .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
            .FirstOrDefaultAsync();

            if (idDocumentoPrincipale != null )
                idProfile = idDocumentoPrincipale?.ToString();

            await this._dbContext.AssertSecurityRights(idProfile.ToString(), idUser.ToString(), idGroup.ToString());

            var getDettaglioDocumento = await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoutente, idProfile, docNumber));

            getDettaglioDocumento.output.accessRights = (await this._dbContext.GetSecurity(idProfile.ToString(), idUser.ToString(), idGroup.ToString())).ACCESSRIGHTS.ToString();

            return getDettaglioDocumento.output;

        }

        private async Task<bool> RemoveInstanceAccessDoc(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;

            result = await this.RemoveInstanceAccess(listInstanceAccessDocuments, infoUtente);
            if (result)
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.GetTemplateInstanceAccess(infoUtente))).output;
                if (template != null)
                {
                    InstanceAccessDocument? doc = (from d in listInstanceAccessDocuments
                                                  where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE)
                                                  select d).FirstOrDefault();


                    if (doc != null)
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDoc = await this.GetDettaglio(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER, doc.INFO_DOCUMENT.DOCNUMBER);
                        if (schedaDoc != null)
                        {
                            string note = "Dichiarazione rimossa dall'utente " + infoUtente.userId;
                            string error = string.Empty;
                            result = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoExecCestina(infoUtente, schedaDoc, schedaDoc.tipoProto, note))).output ;
                        }
                    }

                }
            }


            return result;
        }

        public async Task<RemoveInstanceAccessDocumentsResult> Handle(RemoveInstanceAccessDocumentsRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.RemoveInstanceAccessDoc(request.listInstanceAccessDocuments.ToList(),request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }






        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoEntity, Templates>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => Convert.ToInt32(src.SYSTEM_ID)))
                     .ForMember(dest => dest.ID_TIPO_ATTO, opt => opt.MapFrom(src => src.SYSTEM_ID.ToString()))
                     .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESC_ATTO))
                     .ForMember(dest => dest.ABILITATO_SI_NO, opt => opt.MapFrom(src => src.ABILITATO_SI_NO.ToString()))
                     .ForMember(dest => dest.IN_ESERCIZIO, opt => opt.MapFrom(src => src.IN_ESERCIZIO))
                     .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1 != null ? src.EXT_MOD_1.Trim() : string.Empty))
                     .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2 != null ? src.EXT_MOD_2.Trim() : string.Empty))
                     .ForMember(dest => dest.PATH_MODELLO_STAMPA_UNIONE, opt => opt.MapFrom(src => src.PATH_MOD_SU != null ? src.PATH_MOD_SU.Trim() : string.Empty))
                     .ForMember(dest => dest.PATH_MODELLO_EXCEL, opt => opt.MapFrom(src => src.PATH_MOD_EXC))
                     .ForMember(dest => dest.PATH_XSD_ASSOCIATO, opt => opt.MapFrom(src => src.PATH_XSD_ASSOCIATO))
                     .ForMember(dest => dest.PATH_ALLEGATO_1, opt => opt.MapFrom(src => src.PATH_ALL_1))
                     .ForMember(dest => dest.SCADENZA, opt => opt.MapFrom(src => src.GG_SCADENZA.ToString()))
                     .ForMember(dest => dest.PRE_SCADENZA, opt => opt.MapFrom(src => src.GG_PRE_SCADENZA != null ? src.GG_PRE_SCADENZA.ToString() : string.Empty))
                     .ForMember(dest => dest.PRIVATO, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.CHA_PRIVATO) ? src.CHA_PRIVATO : "0"))
                     .ForMember(dest => dest.ID_AMMINISTRAZIONE, opt => opt.MapFrom(src => src.ID_AMM.ToString()))
                     .ForMember(dest => dest.CODICE_CLASSIFICA, opt => opt.MapFrom(src => src.COD_CLASS))
                     .ForMember(dest => dest.CODICE_MODELLO_TRASM, opt => opt.MapFrom(src => src.COD_MOD_TRASM))
                     .ForMember(dest => dest.IPER_FASC_DOC, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.IPERDOCUMENTO.ToString()) && src.IPERDOCUMENTO.ToString().Equals("1") ? "1" : "0"));
            });

            _mapper = configuration.CreateMapper();
        }
    }
}
