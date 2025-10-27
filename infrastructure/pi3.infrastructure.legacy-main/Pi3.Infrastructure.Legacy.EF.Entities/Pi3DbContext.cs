// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public abstract class Pi3DbContext : DbContext, IPi3DbContext
    {
        #region Public Members

        public DbSet<ChiaveConfigurazioneEntity> ChiaviConfigurazioneEntities { get; set; }

        public DbSet<AmministrazioneEntity> AmministraEntities { get; set; }

        public DbSet<PeopleEntity> PeopleEntities { get; set; }

        public DbSet<GroupEntity> GroupEntities { get; set; }

        public DbSet<PeopleGroupEntity> PeopleGroupEntities { get; set; }

        public DbSet<ProfileEntity> ProfileEntities { get; set; }

        public DbSet<VersionEntity> VersionEntities { get; set; }

        public DbSet<ComponentEntity> ComponentEntities { get; set; }

        public DbSet<SecurityEntity> SecurityEntities { get; set; }

        public DbSet<RegistroEntity> RegistroEntities { get; set; }

        public DbSet<ProjectComponentEntity> ProjectComponentEntities { get; set; }

        public DbSet<ProjectEntity> ProjectEntities { get; set; }

        public DbSet<CorrGlobaliEntity> CorrGlobaliEntities { get; set; }

        public DbSet<TipoOggettoEntity> TipoOggettoEntities { get; set; }
        public DbSet<TipoOggettoFascEntity> TipoOggettoFascEntities { get; set; }

        public DbSet<OggettiCustomEntity> OggettiCustomEntities { get; set; }

        public DbSet<OggettiCustomCompEntity> OggettiCustomCompEntities { get; set; }

        public DbSet<AssociazioneTemplatesEntity> AssociazioneTemplatesEntities { get; set; }

        public DbSet<TipoAttoEntity> TipoAttoEntities { get; set; }
        public DbSet<TipoFascEntity> TipoFascEntities { get; set; }

        public DbSet<DocArrivoParEntity> DocArrivoParEntities { get; set; }

        public DbSet<IndexerHandledRequestEntity> IndexerHandledRequests { get; set; }

        public DbSet<IndexerRequestEntity> IndexerRequests { get; set; }

        public DbSet<RagioneTrasmissioneEntity> RagioneTrasmissioneEntities { get; set; }

        public DbSet<DocumentTypesEntity> DocumentTypesEntities { get; set; }

        public DbSet<RuoloRegistroEntity> RuoloRegistroEntities { get; set; }

        public DbSet<LogEntity> LogEntities { get; set; }

        public DbSet<LogStoricoEntity> LogStoricoEntities { get; set; }

        public DbSet<AreaLavoroEntity> AreaLavoroEntities { get; set; }

        public DbSet<OggettarioEntity> OggettarioEntities { get; set; }

        public DbSet<MailCorrEsterniEntity> MailCorrEsterniEntities { get; set; }

        public DbSet<AROggCustomDocEntity> AROggCustomDocEntities { get; set; }

        public DbSet<TipoRuoloEntity> TipoRuoloEntities { get; set; }

        public DbSet<OggettiStoEntity> OggettiStoEntities { get; set; }


        public DbSet<ADLFlashbackEntity> ADLFlashbackEntities { get; set; }

        public DbSet<AROggCustomFascEntity> AROggCustomFascEntityEntities { get; set; }

        public DbSet<AlboDocPubbEntity> AlboDocPubbEntities { get; set; }

        public DbSet<AlboPubbVerticaliEntity> AlboPubbVerticaliEntities { get; set; }

        public DbSet<AlertConservazioneEntity> AlertConservazioneEntities { get; set; }

        public DbSet<AnagraficaEventDocumentEntity> AnagraficaEventDocumentEntities { get; set; }

        public DbSet<AnagraficaEventiEntity> AnagraficaEventiEntities { get; set; }

        public DbSet<AnagraficaFunzioniEntity> AnagraficaFunzioniEntities { get; set; }

        public DbSet<AnagraficaLogEntity> AnagraficaLogEntities { get; set; }

        public DbSet<AreaConservazioneEntity> AreaConservazioneEntities { get; set; }

        public DbSet<AssAllegatoEntity> AssAllegatoEntities { get; set; }

        public DbSet<AssDiagrammiEntity> AssDiagrammiEntities { get; set; }

        public DbSet<AssDocMailInteropEntity> AssDocMailInteropEntities { get; set; }

        public DbSet<AssGridsEntity> AssGridsEntities { get; set; }

        public DbSet<AssIndxSisEntity> AssIndxSisEntities { get; set; }

        public DbSet<AssLettereDocumentiEntity> AssLettereDocumentiEntities { get; set; }

        public DbSet<AssPolicyProfilazioneEntity> AssPolicyProfilazioneEntities { get; set; }

        public DbSet<AssPolicyTypeEntity> AssPolicyTypeEntities { get; set; }

        public DbSet<AssPregressiEntity> AssPregressiEntities { get; set; }

        public DbSet<AssRuoloOggCustomEntity> AssRuoloOggCustomEntities { get; set; }

        public DbSet<AssRuoloStatiDiagrammaEntity> AssRuoloStatiDiagrammaEntities { get; set; }

        public DbSet<AssTemplatesFascEntity> AssTemplatesFascEntities { get; set; }

        public DbSet<VisTipoDocEntity> VisTipoDocEntities { get; set; }
        public DbSet<VisTipoFascEntity> VisTipoFascEntities { get; set; }

        public DbSet<AssValoriFascEntity> AssValoriFascEntities { get; set; }

        public DbSet<AssociazioneStatiPhasesEntity> AssociazioneStatiPhasesEntities { get; set; }

        public DbSet<AssociazioneTemplatesTmpEntity> AssociazioneTemplatesTmpEntities { get; set; }

        public DbSet<AssociazioneValoriEntity> AssociazioneValoriEntities { get; set; }

        public DbSet<TrasmSingolaEntity> TrasmSingolaEntities { get; set; }

        public DbSet<TrasmUtenteEntity> TrasmUtenteEntities { get; set; }

        public DbSet<TrasmissioneEntity> TrasmissioneEntities { get; set; }

        public DbSet<AutorizzRuoliProcessiEntity> AutorizzRuoliProcessiEntities { get; set; }

        public DbSet<BigFilesEntity> BigFilesEntities { get; set; }

        public DbSet<CacheEntity> CacheEntities { get; set; }

        public DbSet<CanaliEntity> CanaliEntities { get; set; }

        public DbSet<CanaliRegEntity> CanaliRegEntities { get; set; }

        public DbSet<CaratTimbroEntity> CaratTimbroEntities { get; set; }

        public DbSet<CheckMailboxEntity> CheckMailboxEntities { get; set; }

        public DbSet<CheckinCheckoutEntity> CheckinCheckoutEntities { get; set; }

        public DbSet<ChiaviConfigTemplateEntity> ChiaviConfigTemplateEntities { get; set; }

        public DbSet<ClassificazioneTipiDocEntity> ClassificazioneTipiDocEntities { get; set; }

        public DbSet<ClientModelProcessorsEntity> ClientModelProcessorsEntities { get; set; }

        public DbSet<CollMSpedizDocumentoEntity> CollMSpedizDocumentoEntities { get; set; }

        public DbSet<ColoreTimbroEntity> ColoreTimbroEntities { get; set; }

        public DbSet<ConfigAlertConsEntity> ConfigAlertConsEntities { get; set; }

        public DbSet<ConfigAnagraficaEsternaEntity> ConfigAnagraficaEsternaEntities { get; set; }

        public DbSet<ConfigCacheEntity> ConfigCacheEntities { get; set; }

        public DbSet<ConfigStampaConsEntity> ConfigStampaConsEntities { get; set; }

        public DbSet<ConfigVersamentoEntity> ConfigVersamentoEntities { get; set; }
        public DbSet<ConsVerificaEntity> ConsVerificaEntities { get; set; }

        public DbSet<ConsolidatedDocsEntity> ConsolidatedDocsEntities { get; set; }

        public DbSet<ContCustomDocEntity> ContCustomDocEntities { get; set; }

        public DbSet<ContCustomFascEntity> ContCustomFascEntities { get; set; }

        public DbSet<ContatoriDocEntity> ContatoriDocEntities { get; set; }

        public DbSet<ContatoriFascEntity> ContatoriFascEntities { get; set; }

        public DbSet<ContestoProceduraleEntity> ContestoProceduraleEntities { get; set; }

        public DbSet<ConvPdfServerEntity> ConvPdfServerEntities { get; set; }

        public DbSet<CorrAbilitatiEntity> CorrAbilitatiEntities { get; set; }

        public DbSet<CorrGruppoEntity> CorrGruppoEntities { get; set; }

        public DbSet<CorrInteropEntity> CorrInteropEntities { get; set; }

        public DbSet<CorrStoEntity> CorrStoEntities { get; set; }

        public DbSet<CountToDoListEntity> CountToDoListEntities { get; set; }

        public DbSet<DataArrivoStoEntity> DataArrivoStoEntities { get; set; }

        public DbSet<MailRegistriEntity> MailRegistriEntities { get; set; }

        public DbSet<DiagrammiStoEntity> DiagrammiStoEntities { get; set; }
        
        public DbSet<PosizTimbroEntity> PosizTimbroEntities { get; set; }

        public DbSet<RegProtoEntity> RegProtoEntities { get; set; }

        public DbSet<RegFascEntity> RegFascEntities { get; set; }

        public DbSet<IstanzaProcessoFirmaEntity> IstanzaProcessoFirmaEntities { get; set; }

        public DbSet<IstanzaPassoFirmaEntity> IstanzaPassoFirmaEntities { get; set; }

        public DbSet<DatiFatturazioneEntity> DatiFatturazioneEntities { get; set; }

        public DbSet<DatiScaricatiEntity> DatiScaricatiEntities { get; set; }

        public DbSet<DelegaEntity> DelegheEntities { get; set; }

        public DbSet<DescrizioneFascEntity> DescrizioneFascEntities { get; set; }

        public DbSet<DesktopAppEntity> DesktopAppEntities { get; set; }

        public DbSet<DettGlobaliEntity> DettGlobaliEntities { get; set; }

        public DbSet<DettPolicyEsecuzioneEntity> DettPolicyEsecuzioneEntities { get; set; }

        public DbSet<DiagrammiEntity> DiagrammiEntities { get; set; }

        public DbSet<DiagrammiStatoEntity> DiagrammiStatoEntities { get; set; }

        public DbSet<DispositivoStampaEntity> DispositivoStampaEntities { get; set; }

        public DbSet<DisservizioEntity> DisservizioEntities { get; set; }

        public DbSet<DocsPaEntity> DocsPaEntities { get; set; }

        public DbSet<DocCollegamentoEntity> DocCollegamentoEntities { get; set; }

        public DbSet<DocPubEntity> DocPubEntities { get; set; }

        public DbSet<ElementoInLibroFirmaStorEntity> ElementoInLibroFirmaStorEntities { get; set; }

        public DbSet<ElementoInLibroFirmaEntity> ElementoInLibroFirmaEntities { get; set; }

        public DbSet<CanaleCorrEntity> CanaleCorrEntities { get; set; }

        public DbSet<ElencoNoteEntity> ElencoNoteEntities { get; set; }

        public DbSet<EsecuzionePolicyParerEntity> EsecuzionePolicyParerEntities { get; set; }

        public DbSet<EsecuzionePolicyFascParerEntity> ExecuzionePolicyFascParerEntities { get; set; }

        public DbSet<EsitoVerificaConsEntity> EsitoVerificaConsEntities { get; set; }

        public DbSet<EventDocumentEntity> EventDocumentEntities { get; set; }

        public DbSet<EventMonitorEntity> EventMonitorEntities { get; set; }

        public DbSet<EventTypeAssertionEntity> EventTypeAssertionEntities { get; set; }

        public DbSet<ExternalSystemEntity> ExternalSystemEntities { get; set; }

        public DbSet<ExtAppEntity> ExtAppEntities { get; set; }

        public DbSet<FascicolazioneCartaceaEntity> FascicolazioneCartaceaEntities { get; set; }

        public DbSet<FatturaEntity> FatturaEntities { get; set; }

        public DbSet<FatturaTibcoLogEntity> FatturaTibcoLogEntities { get; set; }

        public DbSet<SchemaProcessoFirmaEntity> SchemaProcessoFirmaEntities { get; set; }

        public DbSet<FattAttivaCodFornitoreEntity> FattAttivaCodFornitoreEntities { get; set; }

        public DbSet<FattAttivaLogPisEntity> FattAttivaLogPisEntities { get; set; }

        public DbSet<FattMailElaborateEntity> FattMailElaborateEntities { get; set; }

        public DbSet<FirmatarioEntity> FirmatarioEntities { get; set; }
        public DbSet<FirmatarioDocEntity> FirmatarioDocEntities { get; set; }

        public DbSet<FirmaElettronicaEntity> FirmaElettronicaEntities { get; set; }

        public DbSet<FirmaVersEntity> FirmaVersEntities { get; set; }

        public DbSet<FlussoMessaggiEntity> FlussoMessaggiEntities { get; set; }

        public DbSet<FlussoProceduraleEntity> FlussoProceduraleEntities { get; set; }

        public DbSet<FormattaFascEntity> FormattaFascEntities { get; set; }

        public DbSet<FormattaSegnEntity> FormattaSegnEntities { get; set; }

        public DbSet<FormatoDocumentoEntity> FormatoDocumentoEntities { get; set; }

        public DbSet<FriendApplicationEntity> FriendApplicationEntities { get; set; }

        public DbSet<FsMigrLogEntity> FsMigrLogEntities { get; set; }

        public DbSet<FunzioneEntity> FunzioneEntities { get; set; }

        public DbSet<GridEntity> GridEntities { get; set; }

        public DbSet<IndexSisEntity> IndexSisEntities { get; set; }

        public DbSet<InfoComuniEntity> InfoComuniEntities { get; set; }

        public DbSet<InfoFileEntity> InfoFileEntities { get; set; }

        public DbSet<InfoFirmaDigitaleEntity> InfoFirmaDigitaleEntities { get; set; }

        public DbSet<IntegrCdsEntity> IntegrCdsEntities { get; set; }

        public DbSet<IstanzaProcFirmaStoEntity> IstanzaProcFirmaStoEntities { get; set; }

        public DbSet<ItemConservazioneEntity> ItemConservazioneEntities { get; set; }

        public DbSet<JobEntity> JobEntities { get; set; }

        public DbSet<LdapConfigEntity> LdapConfigEntities { get; set; }

        public DbSet<LdapSyncHistoryEntity> LdapSyncHistoryEntities { get; set; }

        public DbSet<LegislaturaEntity> LegislaturaEntities { get; set; }

        public DbSet<LetteraDocumentoEntity> LetteraDocumentoEntities { get; set; }

        public DbSet<LibroFirmaEntity> LibroFirmaEntities { get; set; }

        public DbSet<ListeAppoEntity> ListeAppoEntities { get; set; }

        public DbSet<ListeDistrEntity> ListeDistrEntities { get; set; }

        public DbSet<LivelloAnomaliaSegnaturaEntity> LivelloAnomaliaSegnaturaEntities { get; set; }

        public DbSet<LockEntity> LockEntities { get; set; }

        public DbSet<LoginEntity> LoginEntities { get; set; }

        public DbSet<LogAttivatoEntity> LogAttivatoEntities { get; set; }

        public DbSet<LogInstallEntity> LogInstallEntities { get; set; }

        public DbSet<LogSegnaturaProtoEntity> LogSegnaturaProtoEntities { get; set; }

        public DbSet<LogTipoAttoEntity> LogTipoAttoEntities { get; set; }

        public DbSet<LAooUoEntity> LAooUoEntities { get; set; }

        public DbSet<MailElaborataEntity> MailElaborataEntities { get; set; }

        public DbSet<MessaggioEsitoFirmaEntity> MessaggioEsitoFirmaEntities { get; set; }

        public DbSet<MetadatiDocumentoEntity> MetadatiDocumentoEntities { get; set; }

        public DbSet<MetadatiFascicoloEntity> MetadatiFascicoloEntities { get; set; }

        public DbSet<ModelloDelegaEntity> ModelloDelegaEntities { get; set; }

        public DbSet<ModelloDestConNotificaEntity> ModelloDestConNotificaEntities { get; set; }

        public DbSet<ModelloDestConNotOrEntity> ModelloDestConNotOrEntities { get; set; }

        public DbSet<UoRegEntity> UoRegEnties { get; set; }

        public DbSet<NotaEntity> NoteEntities { get; set; }

        public DbSet<StatoInvioEntity> StatoInvioEntities { get; set; }

        public DbSet<ModelloMdAppoEntity> ModelloMdAppoEntities { get; set; }

        public DbSet<ModelloMittDestEntity> ModelloMittDestEntities { get; set; }

        public DbSet<ModelloTrasmEntity> ModelloTrasmEntities { get; set; }

        public DbSet<RelPeopleExtAppsEntity> RelPeopleExtAppsEntities { get; set; }

        public DbSet<MailRegistroAppoEntity> MailRegistroAppoEntities { get; set; }

        public DbSet<NotificaEntity> NotificaEntities { get; set; }

        public DbSet<NotifyEntity> NotifyEntities { get; set; }

        public DbSet<NotifyHistoryEntity> NotifyHistoryEntities { get; set; }

        public DbSet<ObjectSyncPendingEntity> ObjectSyncPendingEntities { get; set; }

        public DbSet<OggettiCustomFascEntity> OggettiCustomFascEntities { get; set; }

        public DbSet<OggettiCustomCompFascEntity> OggettiCustomCompFascEntities { get; set; }

        public DbSet<ParolaEntity> ParolaEntities { get; set; }

        public DbSet<PassoEntity> PassoEntities { get; set; }

        public DbSet<PassoDiFirmaEntity> PassoDiFirmaEntities { get; set; }

        public DbSet<PassoEventoEntity> PassoEventoEntities { get; set; }

        public DbSet<PeopleGroupQualificaEntity> PeopleGroupQualificaEntities { get; set; }

        public DbSet<PeopleOtpResetPasswordEntity> PeopleOtpResetPasswordEntities { get; set; }

        public DbSet<PhaseEntity> PhaseEntities { get; set; }

        public DbSet<PolicyEsecuzioneEntity> PolicyEsecuzioneEntities { get; set; }

        public DbSet<PolicyFascParerEntity> PolicyFascParerEntities { get; set; }

        public DbSet<PolicyParerEntity> PolicyParerEntities { get; set; }

        public DbSet<PrefMobileEntity> PrefMobileEntities { get; set; }

        public DbSet<PregressoEntity> PregressoEntities { get; set; }

        public DbSet<PreviewEntity> PreviewEntities { get; set; }

        public DbSet<SimpInteropReceivedMessageEntity> SimpInteropReceivedMessageEntities { get; set; }

        public DbSet<AppEntity> AppEntities { get; set; }

        public DbSet<InteroperabilitySettingEntity> InteroperabilitySettingEntities { get; set; }

        public DbSet<ProfParolaEntity> ProfParoleEntities { get; set; }

        public DbSet<StatoEntity> StatoEntities { get; set; }

        public DbSet<SwitchServiceEntity> SwitchServiceEntities { get; set; }

        public DbSet<SwitchServiceInteropEntity> SwitchServiceInteropEntities { get; set; }

        public DbSet<ProfilFascStoEntity> ProfilFascStoEntities { get; set; }

        public DbSet<ProfilStoEntity> ProfilStoEntities { get; set; }

        public DbSet<SalvaRicercaEntity> SalvaRicercaEntities { get; set; }

        public DbSet<RespConsAooEntity> RespConsAooEntities { get; set; }

        public DbSet<RegistriRepertorioEntity> RegistriRepertorioEntities { get; set; }

        public DbSet<InstanceAccessEntity> InstanceAccessEntities { get; set; }

        public DbSet<InstanceAccessAttEntity> InstanceAccessAttEntities { get; set; }

        public DbSet<InstanceAccessDocEntity> InstanceAccessDocEntities { get; set; }

        public DbSet<TipoFRuoloEntity> TipoFRuoloEntities { get; set; }

        public DbSet<TipoFunzioneEntity> TipoFunzioneEntities { get; set; }
        public DbSet<MvProspettiDocClassCompAllEntity> MvProspettiDocClassCompAllEntities { get; set; }

        public DbSet<VersamentoFascicoliEntity> VersamentoFascicoliEntities { get; set; }

        public DbSet<VersamentiPolicyFascEntity> VersamentiPolicyFascEntities { get; set; }

        public DbSet<PianoConservazioneEntity> PianoConservazioneEntities { get; set; }

        public DbSet<PianoConsTipoAttoEntity> PianoConsTipoAttoEntities { get; set; }

        public DbSet<PianoConsTipoFascEntity> PianoConsTipoFascEntities { get; set; }

        public DbSet<PianoConsAssTempoConsEntity> PianoConsAssTempoConsEntities { get; set; }

        public DbSet<RegistroStoEntity> RegistroStoEntities { get; set; }

        public DbSet<QualificaCorrispondenteEntity> QualificaCorrispondenteEntities { get; set; }

        public DbSet<TrasmDiagrEntity> TrasmDiagrEntities { get; set; }

        public DbSet<DeletedSecurityEntity> DeletedSecurityEntities { get; set; }

        public DbSet<RoleHistoryEntity> RoleHistoryEntities { get; set; }
        public DbSet<ToDoListEntity> ToDoListEntities { get; set; }

        public DbSet<NetworkAliasesEntity> NetworkAliasesEntities { get; set; }

        public DbSet<SimpInteropDbLogEntity> SimpInteropDbLogEntities { get; set; }

        public DbSet<TimestampDocEntity> TimestampDocEntities { get; set; }

        public DbSet<PdfConvRequestEntity> PdfConvRequestEntities { get; set; }

        public DbSet<PdfConvRequestHandledEntity> PdfConvRequestHandledEntities { get; set; }

        public DbSet<TipoNotificaEntity> TipoNotificaEntities { get; set; }

        public DbSet<SendStoEntity> SendStoEntities { get; set; }

        public DbSet<PisAppsArchivePlanEntity> PisAppsArchivePlanEntities { get; set; }
        public DbSet<UltimiDocVisualizzatiEntity> UltimiDocVisualizzatiEntities { get; set; }

        public DbSet<UoSmistamentoEntity> UoSmistamentoEntities { get; set; }

        public DbSet<ProcessoFirmaVisibilitaEntity> ProcessoFirmaVisibilitaEntities { get; set; }

        public DbSet<VisMailRegistriEntity> VisMailRegistriEntities { get; set; }

        public DbSet<MvDocumentiCustomEntity> MvDocumentiCustomEntities { get; set; }

        public DbSet<StampaRepertoriEntity> StampaRepertoriEntities { get; set; }

        public DbSet<StampaRegistriEntity> StampaRegistriEntities { get; set; }

        public DbSet<VersamentoEntity> VersamentoEntities { get; set; }

        public DbSet<ReportMailboxEntity> ReportMailboxEntities { get; set; }

        public DbSet<ReportVersamentoEntity> ReportVersamentoEntities { get; set; }
        public DbSet<AssProviderLibEntity> AssProviderLibEntities { get; set; }

        public DbSet<VersamentiPolicyEntity> VersamentiPolicyEntities { get; set; }

        public abstract Task BookRegProto(long idRegistro);

        public abstract Task BookProgressivoFascicolo(long idTitolario, long? idRegistro = null);

        public abstract Task BookContatoreRepertorio(long idContatore);

        public abstract Task BookContatoreRepertorioComune(long idOggetto);

        public abstract Task BookContatoreRepertorioFasc(long idContatore);

        public abstract Task<long> GetValCampoProfDocOrderWithNumberCast(long docNumber,long objId);
        public abstract Task<DateTime> GetValCampoProfDocWithDataCast(long docNumber,long objId);
        //public abstract Task UpdateContatoreRepertorio(long idContatore);

        public virtual async Task<SecurityRightTypesEnum> GetSecurityRights(string thing, string idUser, string? idGroup = null)
        {
            var idPeopleAsLong = idUser.AsLong();

            var peopleEntity = await this.PeopleEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == idPeopleAsLong && p.DISABLED == "N")
                .Select(p => new
                {
                    ID_AMM = p.ID_AMM,
                    CHA_AMMINISTRATORE = p.CHA_AMMINISTRATORE
                })
                .FirstAsync();

            if (peopleEntity == null)
            {
                return SecurityRightTypesEnum.Deny;
            }
            else if (!peopleEntity.ID_AMM.HasValue || peopleEntity.ID_AMM == 0)
            {
                return SecurityRightTypesEnum.FullControl;
            }
            else if (string.IsNullOrWhiteSpace(idGroup))
            {
                return SecurityRightTypesEnum.Deny;
            }
            else
            {
                var thingAsLong = thing.AsLong();

                long idGroupAsLong = 0;
                if (!string.IsNullOrWhiteSpace(idGroup))
                    idGroupAsLong = idGroup.AsLong();

                var securityEntities =
                    (await this.SecurityEntities.AsNoTracking()
                            .Where(s => s.THING == thingAsLong
                                    && (s.PERSONORGROUP == idPeopleAsLong || s.PERSONORGROUP == idGroupAsLong)
                                    && s.ACCESSRIGHTS > 0)
                            .Select(s => s.ACCESSRIGHTS)
                            .ToListAsync());

                if (securityEntities.Any(s => s == 255))
                    return SecurityRightTypesEnum.FullControl;
                else if (securityEntities.Any(s => s == 63))
                    return SecurityRightTypesEnum.Write;
                else if (securityEntities.Any(s => s  == 45 || s == 20))
                    return SecurityRightTypesEnum.Read;
                else
                    return SecurityRightTypesEnum.Deny;
            }
        }

        public virtual async Task<SecurityEntity> GetSecurity(string thing, string idUser, string? idGroup = null)
        {
            SecurityEntity security = null!;

            var thingAsLong = thing.AsLong();
            var idPeopleAsLong = idUser.AsLong();
            var idGroupAsLong = idGroup != null ? idGroup.AsLong() : 0;

            var idDocumentoPrincipale = 
                (await this.ProfileEntities
                    .AsNoTracking()
                    .Where(p => p.SYSTEM_ID == thingAsLong)
                    .FirstOrDefaultAsync())?.ID_DOCUMENTO_PRINCIPALE;

            if (idDocumentoPrincipale != null && idDocumentoPrincipale != 0)
                thingAsLong = idDocumentoPrincipale.GetValueOrDefault();

            var securityEntities = await this.SecurityEntities
                        .AsNoTracking()
                        .Where(s => s.THING == thingAsLong 
                            && (s.PERSONORGROUP == idPeopleAsLong || s.PERSONORGROUP == idGroupAsLong))
                        .ToListAsync();

            if (securityEntities != null && securityEntities.Any())
                security = securityEntities.OrderByDescending(s => s.ACCESSRIGHTS).FirstOrDefault();

            return security!;
        }

        public abstract Task<DateTime> GetSystemDateTime();

        public virtual async Task<IReadOnlyList<CorrGlobaliEntity>> GetHierarcy(string idGroup)
        {
            var idGroupAsLong = idGroup.AsLong();

            var current = await(from cg in this.CorrGlobaliEntities.AsNoTracking()
                                join tr in this.TipoRuoloEntities.AsNoTracking() on cg.ID_TIPO_RUOLO equals tr.SYSTEM_ID
                                where cg.ID_GRUPPO == idGroupAsLong
                                select new
                                {
                                    Id = cg.SYSTEM_ID,
                                    IdUO = cg.ID_UO,
                                    Livello = tr.NUM_LIVELLO
                                }).FirstOrDefaultAsync();

            return await this.GetHigherRoles(current.IdUO.ToString(), current.Livello.Value);
        }

        public abstract Task<string> ConvertDegre();

        public virtual async Task<IReadOnlyList<CorrGlobaliEntity>> GetChildren(string id)
        {
            var idAsLong = id.AsLong();

            var current = await this.CorrGlobaliEntities
                .Join(this.TipoRuoloEntities, c => c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (c, t) => new { c.SYSTEM_ID, c.ID_UO, t.NUM_LIVELLO })
                .Where(x => x.SYSTEM_ID  == idAsLong)
                .FirstOrDefaultAsync();


            return await this.GetLowerRoles(current.ID_UO.ToString(), current.NUM_LIVELLO.Value);
        }

        public abstract IQueryable<ProfileFullTextEntity> ProfileFullText(string text);

        public abstract IQueryable<CorrGlobaliFullTextEntity> CorrGlobaliFullText(string text);
        #endregion

        #region Private Members

        protected virtual async Task<IReadOnlyList<CorrGlobaliEntity>> GetHigherRoles(string idUO, long level)
        {
            var idUOAsLong = idUO.AsLong();

            var higherRolesEntities = await (from cg in this.CorrGlobaliEntities.AsNoTracking()
                                             join tr in this.TipoRuoloEntities.AsNoTracking() on cg.ID_TIPO_RUOLO equals tr.SYSTEM_ID
                                             where cg.ID_UO == idUOAsLong
                                             && cg.CHA_TIPO_URP == "R"
                                             && cg.DTA_FINE == null
                                             && tr.NUM_LIVELLO.Value < level
                                             select cg)
                                            .ToListAsync();

            var idParentUO = await this.CorrGlobaliEntities.AsNoTracking()
                            .Where(cg => cg.SYSTEM_ID == idUOAsLong)
                            .Select(cg => cg.ID_PARENT)
                            .FirstOrDefaultAsync();

            if (idParentUO.HasValue && idParentUO > 0)
            {
                var superiors = await this.GetHigherRoles(idParentUO.ToString(), level);
                if (superiors.Count > 0)
                    higherRolesEntities.AddRange(superiors);
            }

            return higherRolesEntities.AsReadOnly();
        }

        protected virtual async Task<IReadOnlyList<CorrGlobaliEntity>> GetLowerRoles(string idParentUo, long level)
        {
            var lowerRolesEntities = new List<CorrGlobaliEntity>();

            var childrenUo = await this.CorrGlobaliEntities.AsNoTracking()
                .Where(x => x.ID_PARENT == idParentUo.AsLong())
                .Select(x => x.SYSTEM_ID)
                .ToListAsync();

            foreach(long idUo in childrenUo)
            {
                lowerRolesEntities.AddRange(await this.CorrGlobaliEntities.AsNoTracking()
                    .Join(this.TipoRuoloEntities.AsNoTracking(), c => c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (c, t) => new { c, t })
                    .Where(x => x.c.ID_UO == idUo
                    && x.c.CHA_TIPO_URP == "R"
                    && x.t.NUM_LIVELLO >= level)
                    .Select(x => x.c)
                    .ToListAsync());

                lowerRolesEntities.AddRange(await this.GetLowerRoles(idUo.ToString(), level));
            }

            return lowerRolesEntities.AsReadOnly();
        }

        #endregion
    }
}
