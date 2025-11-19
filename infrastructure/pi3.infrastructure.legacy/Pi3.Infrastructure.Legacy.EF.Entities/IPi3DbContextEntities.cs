// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.Infrastructure.Legacy.EF
{
    public interface IPi3DbContextEntities
    {
        DbSet<ChiaveConfigurazioneEntity> ChiaviConfigurazioneEntities { get; set; }

        DbSet<AmministrazioneEntity> AmministraEntities { get; set; }

        DbSet<PeopleEntity> PeopleEntities { get; set; }

        DbSet<GroupEntity> GroupEntities { get; set; }

        DbSet<PeopleGroupEntity> PeopleGroupEntities { get; set; }

        DbSet<ProfileEntity> ProfileEntities { get; set; }

        DbSet<VersionEntity> VersionEntities { get; set; }

        DbSet<ComponentEntity> ComponentEntities { get; set; }

        DbSet<SecurityEntity> SecurityEntities { get; set; }

        DbSet<RegistroEntity> RegistroEntities { get; set; }

        DbSet<ProjectComponentEntity> ProjectComponentEntities { get; set; }

        DbSet<ProjectEntity> ProjectEntities { get; set; }

        DbSet<CorrGlobaliEntity> CorrGlobaliEntities { get; set; }

        DbSet<TipoOggettoEntity> TipoOggettoEntities { get; set; }
        DbSet<TipoOggettoFascEntity> TipoOggettoFascEntities { get; set; }

        DbSet<OggettiCustomEntity> OggettiCustomEntities { get; set; }

        DbSet<OggettiCustomCompEntity> OggettiCustomCompEntities { get; set; }

        DbSet<AssociazioneTemplatesEntity> AssociazioneTemplatesEntities { get; set; }

        DbSet<TipoAttoEntity> TipoAttoEntities { get; set; }
        DbSet<TipoFascEntity> TipoFascEntities { get; set; }

        DbSet<DocArrivoParEntity> DocArrivoParEntities { get; set; }

        DbSet<IndexerHandledRequestEntity> IndexerHandledRequests { get; set; }

        DbSet<IndexerRequestEntity> IndexerRequests { get; set; }

        DbSet<RagioneTrasmissioneEntity> RagioneTrasmissioneEntities { get; set; }

        DbSet<DocumentTypesEntity> DocumentTypesEntities { get; set; }

        DbSet<RuoloRegistroEntity> RuoloRegistroEntities { get; set; }

        DbSet<LogEntity> LogEntities { get; set; }

        DbSet<LogStoricoEntity> LogStoricoEntities { get; set; }

        DbSet<OggettarioEntity> OggettarioEntities { get; set; }

        DbSet<AreaLavoroEntity> AreaLavoroEntities { get; set; }

        DbSet<MailCorrEsterniEntity> MailCorrEsterniEntities { get; set; }

        DbSet<AROggCustomDocEntity> AROggCustomDocEntities { get; set; }

        DbSet<ADLFlashbackEntity> ADLFlashbackEntities { get; set; }

        DbSet<AROggCustomFascEntity> AROggCustomFascEntityEntities { get; set; }

        DbSet<AlboDocPubbEntity> AlboDocPubbEntities { get; set; }

        DbSet<AlboPubbVerticaliEntity> AlboPubbVerticaliEntities { get; set; }

        DbSet<AlertConservazioneEntity> AlertConservazioneEntities { get; set; }

        DbSet<AnagraficaEventDocumentEntity> AnagraficaEventDocumentEntities { get; set; }

        DbSet<AnagraficaEventiEntity> AnagraficaEventiEntities { get;set; }

        DbSet<AnagraficaFunzioniEntity> AnagraficaFunzioniEntities { get; set; }

        DbSet<AnagraficaLogEntity> AnagraficaLogEntities { get; set; }

        DbSet<TipoRuoloEntity> TipoRuoloEntities { get; set; }

        DbSet<OggettiStoEntity> OggettiStoEntities { get; set; }

        DbSet<AreaConservazioneEntity> AreaConservazioneEntities { get; set; }

        DbSet<AssAllegatoEntity> AssAllegatoEntities {  get; set; }

        DbSet<AssDiagrammiEntity> AssDiagrammiEntities { get; set; }

        DbSet<AssDocMailInteropEntity> AssDocMailInteropEntities { get; set; }

        DbSet<AssGridsEntity> AssGridsEntities { get; set; }

        DbSet<AssIndxSisEntity> AssIndxSisEntities { get; set; }

        DbSet<AssLettereDocumentiEntity> AssLettereDocumentiEntities { get; set; }

        DbSet<AssPolicyProfilazioneEntity> AssPolicyProfilazioneEntities { get; set; }

        DbSet<AssPolicyTypeEntity> AssPolicyTypeEntities { get; set; }

        DbSet<AssPregressiEntity> AssPregressiEntities { get; set; }

        DbSet<AssRuoloOggCustomEntity> AssRuoloOggCustomEntities { get;set; }

        DbSet<AssRuoloStatiDiagrammaEntity> AssRuoloStatiDiagrammaEntities { get; set; }

        DbSet<AssTemplatesFascEntity> AssTemplatesFascEntities { get; set; }

        DbSet<VisTipoDocEntity> VisTipoDocEntities { get; set; }
        DbSet<VisTipoFascEntity> VisTipoFascEntities { get; set; }

        DbSet<AssValoriFascEntity> AssValoriFascEntities { get; set; }

        DbSet<AssociazioneStatiPhasesEntity> AssociazioneStatiPhasesEntities { get; set; }

        DbSet<AssociazioneTemplatesTmpEntity> AssociazioneTemplatesTmpEntities { get; set; }
    
        DbSet<AssociazioneValoriEntity> AssociazioneValoriEntities { get; set; }

        DbSet<TrasmSingolaEntity> TrasmSingolaEntities { get; set; }
        
        DbSet<TrasmUtenteEntity> TrasmUtenteEntities { get; set; }

        DbSet<TrasmissioneEntity> TrasmissioneEntities { get; set; }

        DbSet<AutorizzRuoliProcessiEntity> AutorizzRuoliProcessiEntities { get; set; }

        DbSet<BigFilesEntity> BigFilesEntities { get; set; }

        DbSet<CacheEntity> CacheEntities { get; set; }

        DbSet<CanaliEntity> CanaliEntities { get; set; }

        DbSet<CanaliRegEntity> CanaliRegEntities { get; set; }

        DbSet<CaratTimbroEntity> CaratTimbroEntities { get; set; }

        DbSet<CheckMailboxEntity> CheckMailboxEntities { get; set; }

        DbSet<CheckinCheckoutEntity> CheckinCheckoutEntities { get; set; }

        DbSet<ChiaviConfigTemplateEntity> ChiaviConfigTemplateEntities { get; set; }

        DbSet<ClassificazioneTipiDocEntity> ClassificazioneTipiDocEntities { get; set; }

        DbSet<ClientModelProcessorsEntity> ClientModelProcessorsEntities { get; set; }

        DbSet<CollMSpedizDocumentoEntity> CollMSpedizDocumentoEntities { get; set; }

        DbSet<ColoreTimbroEntity> ColoreTimbroEntities { get; set; }

        DbSet<ConfigAlertConsEntity> ConfigAlertConsEntities { get; set; }

        DbSet<ConfigAnagraficaEsternaEntity> ConfigAnagraficaEsternaEntities { get; set; }

        DbSet<ConfigCacheEntity> ConfigCacheEntities { get; set; }

        DbSet<ConfigStampaConsEntity> ConfigStampaConsEntities { get; set; }

        DbSet<ConfigVersamentoEntity> ConfigVersamentoEntities { get; set; }

        DbSet<ConsVerificaEntity> ConsVerificaEntities { get; set; }

        DbSet<ConsolidatedDocsEntity> ConsolidatedDocsEntities { get; set; }

        DbSet<ContCustomDocEntity> ContCustomDocEntities { get; set; }

        DbSet<ContCustomFascEntity> ContCustomFascEntities { get; set; }

        DbSet<ContatoriDocEntity> ContatoriDocEntities { get; set; }

        DbSet<ContatoriFascEntity> ContatoriFascEntities { get; set; }

        DbSet<ContestoProceduraleEntity> ContestoProceduraleEntities { get; set; }

        DbSet<ConvPdfServerEntity> ConvPdfServerEntities { get; set; }

        DbSet<CorrAbilitatiEntity> CorrAbilitatiEntities { get; set; }

        DbSet<CorrGruppoEntity> CorrGruppoEntities { get; set; }

        DbSet<CorrInteropEntity> CorrInteropEntities { get; set; }

        DbSet<CorrStoEntity> CorrStoEntities { get; set; }

        DbSet<CountToDoListEntity> CountToDoListEntities { get; set; }

        DbSet<DataArrivoStoEntity> DataArrivoStoEntities { get; set; }

        DbSet<MailRegistriEntity> MailRegistriEntities { get; set; }

        DbSet<DiagrammiStoEntity> DiagrammiStoEntities { get; set; }   

        DbSet<PosizTimbroEntity> PosizTimbroEntities { get; set; }   

        DbSet<RegProtoEntity> RegProtoEntities { get; set; }

        DbSet<RegFascEntity> RegFascEntities { get; set; }

        DbSet<IstanzaProcessoFirmaEntity> IstanzaProcessoFirmaEntities { get; set; }

        DbSet<IstanzaPassoFirmaEntity> IstanzaPassoFirmaEntities { get; set; }

        DbSet<DatiFatturazioneEntity> DatiFatturazioneEntities { get; set; }

        DbSet<DatiScaricatiEntity> DatiScaricatiEntities { get; set; }

        DbSet<DelegaEntity> DelegheEntities { get; set; }

        DbSet<DescrizioneFascEntity> DescrizioneFascEntities { get; set; }

        DbSet<DesktopAppEntity> DesktopAppEntities { get; set; }

        DbSet<DettGlobaliEntity> DettGlobaliEntities { get; set; }

        DbSet<DettPolicyEsecuzioneEntity> DettPolicyEsecuzioneEntities { get; set; }

        DbSet<DiagrammiEntity> DiagrammiEntities { get; set; }

        DbSet<DiagrammiStatoEntity> DiagrammiStatoEntities { get; set; }

        DbSet<DispositivoStampaEntity> DispositivoStampaEntities { get; set; }

        DbSet<DisservizioEntity> DisservizioEntities { get; set; }

        DbSet<DocsPaEntity> DocsPaEntities { get; set; }

        DbSet<DocCollegamentoEntity> DocCollegamentoEntities { get; set; }

        DbSet<DocPubEntity> DocPubEntities { get; set; }

        DbSet<ElementoInLibroFirmaStorEntity> ElementoInLibroFirmaStorEntities { get; set; }

        DbSet<ElementoInLibroFirmaEntity> ElementoInLibroFirmaEntities { get; set; }

        DbSet<CanaleCorrEntity> CanaleCorrEntities { get; set; }

        DbSet<ElencoNoteEntity> ElencoNoteEntities { get; set; }

        DbSet<EsecuzionePolicyParerEntity> EsecuzionePolicyParerEntities { get; set; }

        DbSet<EsecuzionePolicyFascParerEntity> ExecuzionePolicyFascParerEntities { get; set; }
        
        DbSet<EsitoVerificaConsEntity> EsitoVerificaConsEntities { get; set; }

        DbSet<EventDocumentEntity> EventDocumentEntities { get; set; }

        DbSet<EventMonitorEntity> EventMonitorEntities { get; set; }

        DbSet<EventTypeAssertionEntity> EventTypeAssertionEntities { get; set; }

        DbSet<ExternalSystemEntity> ExternalSystemEntities { get; set; }

        DbSet<ExtAppEntity> ExtAppEntities { get; set; }

        DbSet<FascicolazioneCartaceaEntity> FascicolazioneCartaceaEntities { get; set; }

        DbSet<FatturaEntity> FatturaEntities { get; set; }

        DbSet<FatturaTibcoLogEntity> FatturaTibcoLogEntities { get; set; }

        DbSet<SchemaProcessoFirmaEntity> SchemaProcessoFirmaEntities { get; set; }

        DbSet<FattAttivaCodFornitoreEntity> FattAttivaCodFornitoreEntities { get; set; }

        DbSet<FattAttivaLogPisEntity> FattAttivaLogPisEntities { get; set; }

        DbSet<FattMailElaborateEntity> FattMailElaborateEntities { get; set; }

        DbSet<FirmatarioEntity> FirmatarioEntities { get; set; }

        DbSet<FirmatarioDocEntity> FirmatarioDocEntities { get; set; }

        DbSet<FirmaElettronicaEntity> FirmaElettronicaEntities { get; set; }

        DbSet<FirmaVersEntity> FirmaVersEntities { get; set; }

        DbSet<FlussoMessaggiEntity> FlussoMessaggiEntities { get; set; }

        DbSet<FlussoProceduraleEntity> FlussoProceduraleEntities { get; set; }

        DbSet<FormattaFascEntity> FormattaFascEntities { get; set; }

        DbSet<FormattaSegnEntity> FormattaSegnEntities { get; set; }

        DbSet<FriendApplicationEntity> FriendApplicationEntities { get; set; }

        DbSet<FsMigrLogEntity> FsMigrLogEntities { get; set; }

        DbSet<FunzioneEntity> FunzioneEntities { get; set; }

        DbSet<GridEntity> GridEntities { get; set; }

        DbSet<IndexSisEntity> IndexSisEntities { get; set; }

        DbSet<InfoComuniEntity> InfoComuniEntities { get; set; }

        DbSet<InfoFileEntity> InfoFileEntities { get; set; }

        DbSet<InfoFirmaDigitaleEntity> InfoFirmaDigitaleEntities { get; set; }

        DbSet<IntegrCdsEntity> IntegrCdsEntities { get; set; }

        DbSet<IstanzaProcFirmaStoEntity> IstanzaProcFirmaStoEntities { get; set; }

        DbSet<ItemConservazioneEntity> ItemConservazioneEntities { get; set; }

        DbSet<JobEntity> JobEntities { get; set; }

        DbSet<LdapConfigEntity> LdapConfigEntities { get; set; }

        DbSet<LdapSyncHistoryEntity> LdapSyncHistoryEntities { get; set; }

        DbSet<LegislaturaEntity> LegislaturaEntities { get; set; }

        DbSet<LetteraDocumentoEntity> LetteraDocumentoEntities { get; set; }

        DbSet<LibroFirmaEntity> LibroFirmaEntities { get; set; }

        DbSet<ListeAppoEntity> ListeAppoEntities { get; set; }

        DbSet<ListeDistrEntity> ListeDistrEntities { get; set; }

        DbSet<LivelloAnomaliaSegnaturaEntity> LivelloAnomaliaSegnaturaEntities { get; set; }

        DbSet<LockEntity> LockEntities { get; set; }

        DbSet<LoginEntity> LoginEntities { get; set; }

        DbSet<LogAttivatoEntity> LogAttivatoEntities { get; set; }

        DbSet<LogInstallEntity> LogInstallEntities { get; set; }

        DbSet<LogSegnaturaProtoEntity> LogSegnaturaProtoEntities { get; set; }

        DbSet<LogTipoAttoEntity> LogTipoAttoEntities { get; set; }

        DbSet<LAooUoEntity> LAooUoEntities { get; set; }

        DbSet<MailElaborataEntity> MailElaborataEntities { get; set; }

        DbSet<MessaggioEsitoFirmaEntity> MessaggioEsitoFirmaEntities { get; set; }

        DbSet<MetadatiDocumentoEntity> MetadatiDocumentoEntities { get; set; }

        DbSet<MetadatiFascicoloEntity> MetadatiFascicoloEntities { get; set; }

        DbSet<UoRegEntity> UoRegEnties { get; set; }

        DbSet<FormatoDocumentoEntity> FormatoDocumentoEntities { get; set; }

        DbSet<ModelloDelegaEntity> ModelloDelegaEntities { get; set; }

        DbSet<ModelloDestConNotificaEntity> ModelloDestConNotificaEntities { get; set; }

        DbSet<ModelloDestConNotOrEntity> ModelloDestConNotOrEntities { get; set; }

        DbSet<NotaEntity> NoteEntities { get; set; }

        DbSet<StatoInvioEntity> StatoInvioEntities { get; set; }

        DbSet<ModelloMdAppoEntity> ModelloMdAppoEntities { get; set; }

        DbSet<ModelloMittDestEntity> ModelloMittDestEntities { get; set; }

        DbSet<ModelloTrasmEntity> ModelloTrasmEntities { get; set; }

        DbSet<RelPeopleExtAppsEntity> RelPeopleExtAppsEntities { get; set; }

        DbSet<MailRegistroAppoEntity> MailRegistroAppoEntities { get; set; }

        DbSet<NotificaEntity> NotificaEntities { get; set; }

        DbSet<NotifyEntity> NotifyEntities { get; set; }

        DbSet<NotifyHistoryEntity> NotifyHistoryEntities { get; set; }

        DbSet<ObjectSyncPendingEntity> ObjectSyncPendingEntities { get; set; }

        DbSet<OggettiCustomFascEntity> OggettiCustomFascEntities { get; set; }

        DbSet<OggettiCustomCompFascEntity> OggettiCustomCompFascEntities { get; set; }

        DbSet<ParolaEntity> ParolaEntities { get; set; }

        DbSet<PassoEntity> PassoEntities { get; set; }

        DbSet<PassoDiFirmaEntity> PassoDiFirmaEntities { get; set; }

        DbSet<PassoEventoEntity> PassoEventoEntities { get; set; }

        DbSet<PeopleGroupQualificaEntity> PeopleGroupQualificaEntities { get; set; }

        DbSet<PeopleOtpResetPasswordEntity> PeopleOtpResetPasswordEntities { get; set; }

        DbSet<PhaseEntity> PhaseEntities { get; set; }

        DbSet<PolicyEsecuzioneEntity> PolicyEsecuzioneEntities { get; set; }

        DbSet<PolicyFascParerEntity> PolicyFascParerEntities { get; set; }

        DbSet<PolicyParerEntity> PolicyParerEntities { get; set; }

        DbSet<PrefMobileEntity> PrefMobileEntities { get; set; }

        DbSet<PregressoEntity> PregressoEntities { get; set; }

        DbSet<PreviewEntity> PreviewEntities { get; set; }

        DbSet<SimpInteropReceivedMessageEntity> SimpInteropReceivedMessageEntities { get; set; }

        DbSet<AppEntity> AppEntities { get; set; }

        DbSet<InteroperabilitySettingEntity> InteroperabilitySettingEntities { get; set; }

        DbSet<StatoEntity> StatoEntities { get; set; }

        DbSet<ProfParolaEntity> ProfParoleEntities { get; set; }

        DbSet<SwitchServiceEntity> SwitchServiceEntities { get; set; }
        DbSet<SwitchServiceInteropEntity> SwitchServiceInteropEntities { get; set; }

        DbSet<ProfilFascStoEntity> ProfilFascStoEntities { get; set; }

        DbSet<ProfilStoEntity> ProfilStoEntities { get; set; }
        DbSet<SalvaRicercaEntity> SalvaRicercaEntities { get; set; }

        DbSet<RespConsAooEntity> RespConsAooEntities { get; set; }

        DbSet<RegistriRepertorioEntity> RegistriRepertorioEntities { get; set; }

        DbSet<InstanceAccessEntity> InstanceAccessEntities { get; set; }

        DbSet<InstanceAccessAttEntity> InstanceAccessAttEntities { get; set; }

        DbSet<InstanceAccessDocEntity> InstanceAccessDocEntities { get; set; }

        DbSet<TipoFRuoloEntity> TipoFRuoloEntities { get; set; }

        DbSet<TipoFunzioneEntity> TipoFunzioneEntities { get; set; }

        DbSet<MvProspettiDocClassCompAllEntity> MvProspettiDocClassCompAllEntities { get; set; }

        DbSet<VersamentoFascicoliEntity> VersamentoFascicoliEntities { get; set; }

        DbSet<VersamentiPolicyFascEntity> VersamentiPolicyFascEntities { get; set; }

        DbSet<PianoConservazioneEntity> PianoConservazioneEntities { get; set; }

        DbSet<PianoConsTipoAttoEntity> PianoConsTipoAttoEntities { get; set; }

        DbSet<PianoConsTipoFascEntity> PianoConsTipoFascEntities { get; set; }

        DbSet<PianoConsAssTempoConsEntity> PianoConsAssTempoConsEntities { get; set; }

        DbSet<RegistroStoEntity> RegistroStoEntities { get; set; }

        DbSet<QualificaCorrispondenteEntity> QualificaCorrispondenteEntities { get; set; }

        DbSet<TrasmDiagrEntity> TrasmDiagrEntities { get; set; }
       

        DbSet<DeletedSecurityEntity> DeletedSecurityEntities { get; set; }

        DbSet<RoleHistoryEntity> RoleHistoryEntities { get; set; }
        DbSet<ToDoListEntity> ToDoListEntities { get; set; }

        DbSet<NetworkAliasesEntity> NetworkAliasesEntities { get; set; }

        DbSet<SimpInteropDbLogEntity> SimpInteropDbLogEntities { get; set; }

        DbSet<TimestampDocEntity> TimestampDocEntities { get; set; }

        DbSet<PdfConvRequestEntity> PdfConvRequestEntities { get; set; }

        DbSet<PdfConvRequestHandledEntity> PdfConvRequestHandledEntities { get; set; }

        DbSet<TipoNotificaEntity> TipoNotificaEntities { get; set; }

        DbSet<SendStoEntity> SendStoEntities { get; set; }

        DbSet<PisAppsArchivePlanEntity> PisAppsArchivePlanEntities { get; set; }
        DbSet<UltimiDocVisualizzatiEntity> UltimiDocVisualizzatiEntities { get; set; }

        DbSet<UoSmistamentoEntity> UoSmistamentoEntities { get; set; }

        DbSet<ProcessoFirmaVisibilitaEntity> ProcessoFirmaVisibilitaEntities { get; set; }

        DbSet<VisMailRegistriEntity> VisMailRegistriEntities { get; set; }

        DbSet<MvDocumentiCustomEntity> MvDocumentiCustomEntities { get; set; }

        DbSet<StampaRepertoriEntity> StampaRepertoriEntities { get; set; }

        DbSet<StampaRegistriEntity> StampaRegistriEntities { get; set; }

        DbSet<VersamentoEntity> VersamentoEntities { get; set; }

        DbSet<ReportMailboxEntity> ReportMailboxEntities { get; set; }

        DbSet<ReportVersamentoEntity> ReportVersamentoEntities { get; set; }
        DbSet<AssProviderLibEntity> AssProviderLibEntities { get; set; }

        DbSet<VersamentiPolicyEntity> VersamentiPolicyEntities { get; set; }
    }
}