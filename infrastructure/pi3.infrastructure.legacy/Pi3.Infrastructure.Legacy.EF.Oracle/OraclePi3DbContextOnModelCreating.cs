// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Pi3.Infrastructure.Legacy.EF.Oracle
{
    public partial class OraclePi3DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasSequence<int>("SEQ");
            builder.HasSequence("SEQ_AMMINISRAZIONI");
            builder.HasSequence("SEQ_APPS");
            builder.HasSequence("SEQ_CONSERVAZIONE");
            builder.HasSequence("SEQ_DATI_FATTURAZIONE");
            builder.HasSequence("SEQ_DISPOSITIVI_STAMPA");
            builder.HasSequence("SEQ_DPA_A_R_OGG_CUSTOM_DOC");
            builder.HasSequence("SEQ_DPA_A_R_OGG_CUSTOM_FASC");
            builder.HasSequence("SEQ_DPA_ALERT_CONSERVAZIONE");
            builder.HasSequence("SEQ_DPA_AREA_ESIBIZIONE");
            builder.HasSequence("SEQ_DPA_ASS_ALLEGATO");
            builder.HasSequence("SEQ_DPA_ASS_DIAGRAMMI");
            builder.HasSequence("SEQ_DPA_ASS_DOC_MAIL_INTEROP");
            builder.HasSequence("SEQ_DPA_ASS_INDX_SIS");
            builder.HasSequence("SEQ_DPA_ASS_PREGRESSI");
            builder.HasSequence("SEQ_DPA_ASS_TEMPLATES_FASC");
            builder.HasSequence("SEQ_DPA_ASS_VALORI_FASC");
            builder.HasSequence("SEQ_DPA_ASSOCIAZIONE_TEMPLATES");
            builder.HasSequence("SEQ_DPA_ASSOCIAZIONE_VALORI");
            builder.HasSequence("SEQ_DPA_BIG_FILES");
            builder.HasSequence("SEQ_DPA_CHIAVI_CONFIG");
            builder.HasSequence("SEQ_DPA_CHIAVI_CONFIG_TEMPLATE");
            builder.HasSequence("SEQ_DPA_CONS_VERIFICA");
            builder.HasSequence("SEQ_DPA_CONTATORI_DOC");
            builder.HasSequence("SEQ_DPA_CONTATORI_FASC");
            builder.HasSequence("SEQ_DPA_CONV_PDF_SERVER");
            builder.HasSequence("SEQ_DPA_CORR_ABILITATI");
            builder.HasSequence("SEQ_DPA_DATA_ARRIVO_STO");
            builder.HasSequence("SEQ_DPA_DESCRIZIONI_FASC");
            builder.HasSequence("SEQ_DPA_DETT_POLICY_ESECUZIONE");
            builder.HasSequence("SEQ_DPA_DIAGRAMMI");
            builder.HasSequence("SEQ_DPA_DIAGRAMMI_STATO");
            builder.HasSequence("SEQ_DPA_DIAGRAMMI_STO");
            builder.HasSequence("SEQ_DPA_DISSERVIZI");
            builder.HasSequence("SEQ_DPA_ELEMENTO_LIBRO_FIRMA");
            builder.HasSequence("SEQ_DPA_EVENT_DOCUMENT");
            builder.HasSequence("SEQ_DPA_EXT_APPS");
            builder.HasSequence("SEQ_DPA_EXTERNAL_SYSTEMS");
            builder.HasSequence("SEQ_DPA_FIRMA_ELETTRONICA");
            builder.HasSequence("SEQ_DPA_FIRMATARIO_DOC");
            builder.HasSequence("SEQ_DPA_FRIEND_APPLICATION");
            builder.HasSequence("SEQ_DPA_FS_MIGR_LOG");
            builder.HasSequence("SEQ_DPA_GRIDS");
            builder.HasSequence("SEQ_DPA_INDX_SIS");
            builder.HasSequence("SEQ_DPA_INFO_COMUNI");
            builder.HasSequence("SEQ_DPA_INFO_FILE");
            builder.HasSequence("SEQ_DPA_INTEGR_CDS");
            builder.HasSequence("SEQ_DPA_ISTANZA_PASSO_FIRMA");
            builder.HasSequence("SEQ_DPA_ISTANZA_PROC_FIRMA_STO");
            builder.HasSequence("SEQ_DPA_ISTANZA_PROCESSO_FIRMA");
            builder.HasSequence("SEQ_DPA_ITEMS_CONSERVAZIONE");
            builder.HasSequence("SEQ_DPA_ITEMS_ESIBIZIONE");
            builder.HasSequence("SEQ_DPA_LDAP_SYNC_HISTORY");
            builder.HasSequence("SEQ_DPA_LETTERE_DOCUMENTI");
            builder.HasSequence("SEQ_DPA_LISTE_DISTR");
            builder.HasSequence("SEQ_DPA_LISTE_DISTR_STO");
            builder.HasSequence("SEQ_DPA_LOG");
            builder.HasSequence("SEQ_DPA_LOG_SEGNATURA_PROTO");
            builder.HasSequence("SEQ_DPA_MAIL_CORR_ESTERNI");
            builder.HasSequence("SEQ_DPA_MAIL_REGISTRI");
            builder.HasSequence("SEQ_DPA_METADATI_DOCUMENTO");
            builder.HasSequence("SEQ_DPA_METADATI_FASCICOLO");
            builder.HasSequence("SEQ_DPA_MODELLI_MITT_DEST");
            builder.HasSequence("SEQ_DPA_MODELLI_TRASM");
            builder.HasSequence("SEQ_DPA_NOTIFICA");
            builder.HasSequence("SEQ_DPA_OGG_CUSTOM_COMP");
            builder.HasSequence("SEQ_DPA_OGG_CUSTOM_COMP_FASC");
            builder.HasSequence("SEQ_DPA_OGGETTI_CUSTOM");
            builder.HasSequence("SEQ_DPA_OGGETTI_CUSTOM_FASC");
            builder.HasSequence("SEQ_DPA_PASSI");
            builder.HasSequence("SEQ_DPA_PASSO_DI_FIRMA");
            builder.HasSequence("SEQ_DPA_PEOPLE_OTP_RESET_PW");
            builder.HasSequence("SEQ_DPA_PEOPLEGROUPS_QUALIF");
            builder.HasSequence("SEQ_DPA_POLICY_ESECUZIONE");
            builder.HasSequence("SEQ_DPA_POLICY_FASC_PARER");
            builder.HasSequence("SEQ_DPA_PREF_MOBILE");
            builder.HasSequence("SEQ_DPA_PREGRESSI");
            builder.HasSequence("SEQ_DPA_PROFIL_FASC_STO");
            builder.HasSequence("SEQ_DPA_PROFIL_STO");
            builder.HasSequence("SEQ_DPA_PROTO_TIT");
            builder.HasSequence("SEQ_DPA_QUALIFICHE");
            builder.HasSequence("SEQ_DPA_REGISTRO_CONSERVAZIONE");
            builder.HasSequence("SEQ_DPA_REL_PEOPLE_EXTAPPS");
            builder.HasSequence("SEQ_DPA_REPORT_PROCESSI_TICK");
            builder.HasSequence("SEQ_DPA_RISCONTRI_CLASSIFICA");
            builder.HasSequence("SEQ_DPA_SALVA_RICERCHE");
            builder.HasSequence("SEQ_DPA_SCHEMA_PROCESSO_FIRMA");
            builder.HasSequence("SEQ_DPA_SEND_STO");
            builder.HasSequence("SEQ_DPA_STAMPA_CONSERVAZIONE");
            builder.HasSequence("SEQ_DPA_STAMPA_REPERTORI");
            builder.HasSequence("SEQ_DPA_STATI");
            builder.HasSequence("SEQ_DPA_STATO_TASK");
            builder.HasSequence("SEQ_DPA_TASK");
            builder.HasSequence("SEQ_DPA_TEMPLATES");
            builder.HasSequence("SEQ_DPA_TEMPLATES_COMPONENT");
            builder.HasSequence("SEQ_DPA_TIMESTAMP_DOC");
            builder.HasSequence("SEQ_DPA_TIPO_ATTO");
            builder.HasSequence("SEQ_DPA_TIPO_FASC");
            builder.HasSequence("SEQ_DPA_TIPO_NOTIFICA");
            builder.HasSequence("SEQ_DPA_TIPO_OGGETTO");
            builder.HasSequence("SEQ_DPA_TIPO_OGGETTO_FASC");
            builder.HasSequence("SEQ_DPA_TRASM_DIAGR");
            builder.HasSequence("SEQ_DPA_VERIFICA_FORMATI_CONS");
            builder.HasSequence("SEQ_DPA_VERSAMENTO");
            builder.HasSequence("SEQ_DPA_VERSAMENTO_FASCICOLI");
            builder.HasSequence("SEQ_DPA_VIS_MAIL_REGISTRI");
            builder.HasSequence("SEQ_DPA_VIS_TIPO_DOC");
            builder.HasSequence("SEQ_DPA_VIS_TIPO_FASC");
            builder.HasSequence("SEQ_DPA_VOCI_MENU_ADMIN");
            builder.HasSequence("SEQ_ER").IncrementsBy(200);
            builder.HasSequence("SEQ_INST_ACC");
            builder.HasSequence("SEQ_INST_ACC_ATT");
            builder.HasSequence("SEQ_INST_ACC_DOC");
            builder.HasSequence("SEQ_INSTALL_LOG");
            builder.HasSequence("SEQ_NOTIFICATIONCHANNEL");
            builder.HasSequence("SEQ_NOTIFICATIONINSTANCE");
            builder.HasSequence("SEQ_NOTIFICATIONITEM");
            builder.HasSequence("SEQ_NOTIFICATIONITEMCATEGORIES");
            builder.HasSequence("SEQ_P3_PIANO_CONS_TIPO_ATTO");
            builder.HasSequence("SEQ_P3_PIANO_CONS_TIPO_FASC");
            builder.HasSequence("SEQ_P3_PIANO_CONSERVAZIONE");
            builder.HasSequence("SEQ_P3_PIS_APPS_ARCHIVEPLANS");
            builder.HasSequence("SEQ_PGU");
            builder.HasSequence("SEQ_POLICY_CONSERVAZIONE");
            builder.HasSequence("SEQ_PUBBLICAZIONI_DOCUMENTI");
            builder.HasSequence("SEQ_PUBLISHER");
            builder.HasSequence("SEQ_SUBSCRIBER");
            builder.HasSequence("SEQ_SUPPORTO");
            builder.HasSequence("SEQ_UT");
            builder.HasSequence("SEQ_UTL_SYSTEM_LOG");
            builder.HasSequence("SEQROLEHISTORY");
            builder.HasSequence("SEQ_DPA_PDFCONV_REQ");   

            builder.Entity<ChiaveConfigurazioneEntity>().ToTable("DPA_CHIAVI_CONFIGURAZIONE");
            builder.Entity<ChiaveConfigurazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_VISIBILE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_MODIFICABILE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_TIPO_CHIAVE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_GLOBALE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_CONSERVAZIONE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.CHA_CONSERVAZIONE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.VAR_CODICE).HasColumnType("VARCHAR2");
            builder.Entity<ChiaveConfigurazioneEntity>().Property(p => p.VAR_VALORE).HasColumnType("VARCHAR2");


            builder.Entity<AmministrazioneEntity>().ToTable("DPA_AMMINISTRA");
            builder.Entity<AmministrazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AmministrazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_ARCHIVIAZIONE_LOG).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_STR_SEGNATURA).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_SEPARATORE).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_SMTP_STA).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_STR_FISSA).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_SMTP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.CHA_ENABLE_CONS).HasColumnType("VARCHAR2(1)");
            builder.Entity<AmministrazioneEntity>().Property(p => p.ENABLE_NEWS).HasColumnType("CHAR(1)");

            builder.Entity<PeopleEntity>().ToTable("PEOPLE");
            builder.Entity<PeopleEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PeopleEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<PeopleEntity>().Property(p => p.CONNECT_BRIDGED).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_NOTIFICA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_SYSTEM_USER).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.NO_EXP_DATE).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_RESP_ASS).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.ACCETTAZIONE_DISSERV).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_TIPO_FIRMA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.ALLOW_LOGIN).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.DISABLED).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.DR_USER).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.SHOW_RESTORED).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_ASSEGNATARIO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_NOTIFICA_CON_ALLEGATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleEntity>().Property(p => p.CHA_AMMINISTRATORE).HasColumnType("VARCHAR2(1)");

            builder.Entity<GroupEntity>().ToTable("GROUPS");
            builder.Entity<GroupEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<GroupEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<PeopleGroupEntity>().ToTable("PEOPLEGROUPS");
            builder.Entity<PeopleGroupEntity>().HasNoKey();
            builder.Entity<PeopleGroupEntity>().Property(p => p.CHA_PREFERITO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PeopleGroupEntity>().Property(p => p.CHA_UTENTE_RIF).HasColumnType("VARCHAR2(1)");

            builder.Entity<ProfileEntity>().ToTable("PROFILE");
            builder.Entity<ProfileEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ProfileEntity>().Property(p => p.NUM_PROTO).HasColumnType("NUMBER");
            builder.Entity<ProfileEntity>().Property(p => p.DOCNUMBER).HasColumnType("NUMBER");
            builder.Entity<ProfileEntity>().Property(p => p.VAR_PROF_OGGETTO).HasColumnType("VARCHAR2");
            builder.Entity<ProfileEntity>().Property(p => p.VAR_PROF_OGGETTO).HasColumnType("VARCHAR2");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_IN_CESTINO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_MOD_MITT_DEST).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_IMG).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_INVIO_CONFERMA).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_PERSONALE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_INTEROP).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_DOCUMENTO_DA_PEC).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_MOD_OGGETTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_UNLOCKED_FINAL_STATE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_ASSEGNATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_MOD_DEST_OCC).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_IN_ARCHIVIO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_PRIVATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_CONGELATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_MOD_MITT_INT).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_MOD_MITT_DEST).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_EVIDENZA).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_CONSOLIDATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_TIPO_PROTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_DA_PROTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_FASCICOLATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CHA_FIRMATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.IN_LIBROFIRMA).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProfileEntity>().Property(p => p.CREATION_DATE).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.CREATION_TIME).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.DTA_PROTO).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.DTA_PROTO_IN).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.DTA_ANNULLA).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.DTA_PROTO_EME).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.DTA_SCADENZA).HasColumnType("DATE");
            builder.Entity<ProfileEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<VersionEntity>().ToTable("VERSIONS");
            builder.Entity<VersionEntity>().HasKey(p => p.VERSION_ID);
            builder.Entity<VersionEntity>().Property(p => p.DTA_ARRIVO).HasColumnType("DATE");
            builder.Entity<VersionEntity>().Property(p => p.DTA_CREAZIONE).HasColumnType("DATE");
            builder.Entity<VersionEntity>().Property(p => p.VERSION_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<VersionEntity>().Property(p => p.CHA_SEGNATURA).HasColumnType("VARCHAR2(1)");
            builder.Entity<VersionEntity>().Property(p => p.SUBVERSION).HasColumnType("VARCHAR2(1)");
            builder.Entity<VersionEntity>().Property(p => p.CHA_DA_INVIARE).HasColumnType("VARCHAR2(1)");
            builder.Entity<VersionEntity>().Property(p => p.CHA_ALLEGATI_ESTERNO).HasColumnType("CHAR(1)");

            builder.Entity<ComponentEntity>().ToTable("COMPONENTS");
            builder.Entity<ComponentEntity>().HasKey(p => p.VERSION_ID);
            builder.Entity<ComponentEntity>().Property(p => p.DTA_FILE_ACQUIRED).HasColumnType("DATE");
            builder.Entity<ComponentEntity>().Property(p => p.CHA_TIPO_FIRMA).HasColumnType("VARCHAR2");
            builder.Entity<ComponentEntity>().Property(p => p.CHA_FIRMATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ComponentEntity>().Property(p => p.LOCKED).HasColumnType("VARCHAR2(1)");

            builder.Entity<SecurityEntity>().ToTable("SECURITY");
            builder.Entity<SecurityEntity>().HasKey(p => new { p.THING, p.PERSONORGROUP, p.ACCESSRIGHTS });
            builder.Entity<SecurityEntity>().Property(p => p.TS_INSERIMENTO).HasDefaultValue(DateTime.Now);
            builder.Entity<SecurityEntity>().Property(p => p.CHA_TIPO_DIRITTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<SecurityEntity>().Property(p => p.VAR_NOTE_SEC).HasColumnType("VARCHAR2");
            builder.Entity<SecurityEntity>().Property(p => p.CHA_COPIA_VISIBILITA).HasColumnType("CHAR(1)");
            builder.Entity<SecurityEntity>().Property(p => p.HIDE_DOC_VERSIONS).HasColumnType("CHAR(1)");

            builder.Entity<DeletedSecurityEntity>().ToTable("DELETED_SECURITY");
            builder.Entity<DeletedSecurityEntity>().HasKey(p => new { p.THING, p.PERSONORGROUP, p.ACCESSRIGHTS });
            builder.Entity<DeletedSecurityEntity>().Property(p => p.CHA_TIPO_DIRITTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<DeletedSecurityEntity>().Property(p => p.NOTE).HasColumnType("VARCHAR2");
            builder.Entity<DeletedSecurityEntity>().Property(p => p.CHA_COPIA_VISIBILITA).HasColumnType("CHAR(1)");
            builder.Entity<DeletedSecurityEntity>().Property(p => p.HIDE_DOC_VERSIONS).HasColumnType("CHAR(1)");

            builder.Entity<RegistroEntity>().ToTable("DPA_EL_REGISTRI");
            builder.Entity<RegistroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RegistroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_RICEVUTA_PEC).HasColumnType("VARCHAR2");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_DISABILITATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.VAR_MAIL_RIC_PENDENTE).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_IMAP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_POP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_SMTP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_RF).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.VAR_SOLO_MAIL_PEC).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_SMTP_STA).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistroEntity>().Property(p => p.CHA_STATO).HasColumnType("VARCHAR2(1)");

            builder.Entity<ProjectComponentEntity>().ToTable("PROJECT_COMPONENTS");
            builder.Entity<ProjectComponentEntity>().HasKey(p => new { p.PROJECT_ID, p.LINK });
            builder.Entity<ProjectComponentEntity>().Property(p => p.TYPE).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectComponentEntity>().Property(p => p.CHA_FASC_PRIMARIA).HasColumnType("VARCHAR2(1)");



            builder.Entity<ProjectEntity>().ToTable("PROJECT");
            builder.Entity<ProjectEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ProjectEntity>().Property(p => p.DTA_APERTURA).HasColumnType("DATE");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_BLOCCA_FASC).HasColumnType("VARCHAR2");
            builder.Entity<ProjectEntity>().Property(p => p.ICONIZED).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_CONTA_PROT_TIT).HasColumnType("VARCHAR2");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_BLOCCA_FIGLI).HasColumnType("VARCHAR2");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_IN_CESTINO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_STATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_IN_ARCHIVIO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_CONTROLLATO).HasColumnType("VARCHAR2");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_RW).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_PRIVATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_CONSENTI_CLASS).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.DTA_CHIUSURA).HasColumnType("DATE");
            builder.Entity<ProjectEntity>().Property(p => p.DTA_SCADENZA).HasColumnType("DATE");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_TIPO_PROJ).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.CHA_TIPO_FASCICOLO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ProjectEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CorrGlobaliEntity>().ToTable("DPA_CORR_GLOBALI");
            builder.Entity<CorrGlobaliEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CorrGlobaliEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_SYSTEM_ROLE).HasDefaultValue("0");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.VAR_COD_RUBRICA).HasColumnType("VARCHAR2");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.VAR_CODICE).HasColumnType("VARCHAR2");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.VAR_DESC_CORR).HasColumnType("VARCHAR2");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_TIPO_IE).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_TIPO_CORR).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_TIPO_URP).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_TIPO_IE).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.AOO_SOSPESO).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_DETTAGLI).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_RIFERIMENTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_SYSTEM_ROLE).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_SEGRETARIO).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_PA).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_DEFAULT_TRASM).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_RESPONSABILE).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.VAR_INSERT_BY_INTEROP).HasColumnType("VARCHAR2");
            builder.Entity<CorrGlobaliEntity>().Property(p => p.CHA_DISABLED_TRASM).HasColumnType("CHAR(1)");


            builder.Entity<TipoOggettoEntity>().ToTable("DPA_TIPO_OGGETTO");
            builder.Entity<TipoOggettoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoOggettoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<TipoOggettoFascEntity>().ToTable("DPA_TIPO_OGGETTO_FASC");
            builder.Entity<TipoOggettoFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoOggettoFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<OggettiCustomEntity>().ToTable("DPA_OGGETTI_CUSTOM");
            builder.Entity<OggettiCustomEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettiCustomEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_OGGETTI_CUSTOM");
            builder.Entity<OggettiCustomEntity>().Property(p => p.CHA_CONSOLIDAMENTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<OggettiCustomEntity>().Property(p => p.CHA_CONS_REPERTORIO).HasColumnType("VARCHAR2(1)");
            builder.Entity<OggettiCustomEntity>().Property(p => p.RESET_ANNO).HasColumnType("VARCHAR2");
            builder.Entity<OggettiCustomEntity>().Property(p => p.CHA_CONSERVAZIONE).HasColumnType("VARCHAR2(1)");
            builder.Entity<OggettiCustomEntity>().Property(p => p.CHA_TIPO_TAR).HasColumnType("VARCHAR2");

            builder.Entity<OggettiCustomCompEntity>().ToTable("DPA_OGG_CUSTOM_COMP");
            builder.Entity<OggettiCustomCompEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettiCustomCompEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_OGG_CUSTOM_COMP");

            builder.Entity<AssociazioneTemplatesEntity>().ToTable("DPA_ASSOCIAZIONE_TEMPLATES");
            builder.Entity<AssociazioneTemplatesEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssociazioneTemplatesEntity>().Property(p => p.DTA_INS).HasColumnType("DATE");
            builder.Entity<AssociazioneTemplatesEntity>().Property(p => p.DTA_ANNULLAMENTO).HasColumnType("DATE");
            builder.Entity<AssociazioneTemplatesEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASSOCIAZIONE_TEMPLATES");

            builder.Entity<TipoAttoEntity>().ToTable("DPA_TIPO_ATTO");
            builder.Entity<TipoAttoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoAttoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TIPO_ATTO");
            builder.Entity<TipoAttoEntity>().Property(p => p.CHA_PRIVATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<TipoAttoEntity>().Property(p => p.CHA_INVIO_CONSERVAZIONE).HasColumnType("VARCHAR2(1)");
            builder.Entity<TipoAttoEntity>().Property(p => p.CHA_ASSOC_MANUALE).HasColumnType("VARCHAR2(1)");
            builder.Entity<TipoAttoEntity>().Property(p => p.IN_ESERCIZIO).HasColumnType("VARCHAR2");


            builder.Entity<TipoFascEntity>().ToTable("DPA_TIPO_FASC");
            builder.Entity<TipoFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TIPO_FASC");
            builder.Entity<TipoFascEntity>().Property(p => p.VAR_DESC_FASC).HasColumnType("VARCHAR2");
            builder.Entity<TipoFascEntity>().Property(p => p.CHA_PRIVATO).HasColumnType("VARCHAR2(1)");

            builder.Entity<DocArrivoParEntity>().ToTable("DPA_DOC_ARRIVO_PAR");
            builder.Entity<DocArrivoParEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DocArrivoParEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<DocArrivoParEntity>().Property(p => p.CHA_TIPO_MITT_DEST).HasColumnType("VARCHAR2");

            builder.HasSequence<int>("SEQ_DPA_TEXTINDX_REQ");
            builder.Entity<IndexerRequestEntity>().ToTable("DPA_TEXTINDX_REQ");
            builder.Entity<IndexerRequestEntity>().HasKey(p => p.ID);
            builder.Entity<IndexerRequestEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TEXTINDX_REQ");

            builder.Entity<IndexerHandledRequestEntity>().ToTable("DPA_TEXTINDX_REQ_HANDLED");
            builder.Entity<IndexerHandledRequestEntity>().HasKey(p => p.ID);

            builder.Entity<RagioneTrasmissioneEntity>().ToTable("DPA_RAGIONE_TRASM");
            builder.Entity<RagioneTrasmissioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_RISPOSTA).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_TIPO_DEST).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_VIS).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.VAR_NOTIFICA_TRASM).HasColumnType("VARCHAR2");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_TIPO_RISPOSTA).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_TIPO_DIRITTI).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_TIPO_RAGIONE).HasColumnType("VARCHAR2(1)");
            builder.Entity<RagioneTrasmissioneEntity>().Property(p => p.CHA_EREDITA).HasColumnType("VARCHAR2(1)");


            builder.Entity<DocumentTypesEntity>().ToTable("DOCUMENTTYPES");
            builder.Entity<DocumentTypesEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DocumentTypesEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<DocumentTypesEntity>().Property(p => p.DISABLED).HasColumnType("VARCHAR2(1)");
            builder.Entity<DocumentTypesEntity>().Property(p => p.FULL_TEXT).HasColumnType("VARCHAR2(1)");
            builder.Entity<DocumentTypesEntity>().Property(p => p.KEEP_CRITERIA).HasColumnType("VARCHAR2(1)");
            builder.Entity<DocumentTypesEntity>().Property(p => p.CHA_TIPO_CANALE).HasColumnType("VARCHAR2(1)");
            builder.Entity<DocumentTypesEntity>().Property(p => p.STORAGE_TYPE).HasColumnType("VARCHAR2(1)");
            builder.Entity<DocumentTypesEntity>().Property(p => p.RET_2_TYPE).HasColumnType("VARCHAR2(1)");


            builder.Entity<RuoloRegistroEntity>().ToTable("DPA_L_RUOLO_REG");
            builder.Entity<RuoloRegistroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RuoloRegistroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<RuoloRegistroEntity>().Property(p => p.CHA_RIFERIMENTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<RuoloRegistroEntity>().Property(p => p.CHA_PREFERITO).HasColumnType("VARCHAR2(1)");
            builder.Entity<RuoloRegistroEntity>().Property(p => p.CHA_PROTOCOLLO_ABILITATO).HasColumnType("CHAR(1)");
            builder.Entity<RuoloRegistroEntity>().Property(p => p.CHA_PROTOCOLLO_ABILITATO).HasDefaultValue("1");

            builder.Entity<LogEntity>().ToTable("DPA_LOG");
            builder.Entity<LogEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LogEntity>().Property(p => p.DTA_AZIONE).HasColumnType("DATE");
            builder.Entity<LogEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_LOG");
            builder.Entity<LogEntity>().Property(p => p.CHA_ESITO).HasColumnType("VARCHAR2(1)");
            builder.Entity<LogEntity>().Property(p => p.VAR_COD_AZIONE).HasColumnType("VARCHAR2");
            builder.Entity<LogEntity>().Property(p => p.VAR_OGGETTO).HasColumnType("VARCHAR2");

            builder.Entity<LogStoricoEntity>().ToTable("DPA_LOG_STORICO");
            builder.Entity<LogStoricoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LogStoricoEntity>().Property(p => p.DTA_AZIONE).HasColumnType("DATE");
            builder.Entity<LogStoricoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<LogStoricoEntity>().Property(p => p.CHA_ESITO).HasColumnType("VARCHAR2(1)");
            builder.Entity<LogStoricoEntity>().Property(p => p.VAR_COD_AZIONE).HasColumnType("VARCHAR2");
            builder.Entity<LogStoricoEntity>().Property(p => p.VAR_OGGETTO).HasColumnType("VARCHAR2");


            builder.Entity<AreaLavoroEntity>().ToTable("DPA_AREA_LAVORO");
            builder.Entity<AreaLavoroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AreaLavoroEntity>().Property(p => p.DTA_INS).HasColumnType("DATE");
            builder.Entity<AreaLavoroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<AreaLavoroEntity>().Property(p => p.CHA_TIPO_DOC).HasColumnType("VARCHAR2(1)");
            builder.Entity<AreaLavoroEntity>().Property(p => p.CHA_TIPO_FASC).HasColumnType("VARCHAR2(1)");

            builder.Entity<MailCorrEsterniEntity>().ToTable("DPA_MAIL_CORR_ESTERNI");
            builder.Entity<MailCorrEsterniEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MailCorrEsterniEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_MAIL_CORR_ESTERNI");
            builder.Entity<MailCorrEsterniEntity>().Property(p => p.VAR_PRINCIPALE).HasColumnType("VARCHAR2(1)");

            builder.Entity<TipoRuoloEntity>().ToTable("DPA_TIPO_RUOLO");
            builder.Entity<TipoRuoloEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoRuoloEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<OggettiStoEntity>().ToTable("DPA_OGGETTI_STO");
            builder.Entity<OggettiStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettiStoEntity>().Property(p => p.DTA_MODIFICA).HasColumnType("DATE");
            builder.Entity<OggettiStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<OggettarioEntity>().ToTable("DPA_OGGETTARIO");
            builder.Entity<OggettarioEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettarioEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<OggettarioEntity>().Property(p => p.CHA_OCCASIONALE).HasColumnType("VARCHAR2(1)");

            builder.Entity<VisTipoDocEntity>().ToTable("DPA_VIS_TIPO_DOC");
            builder.Entity<VisTipoDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<VisTipoDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_VIS_TIPO_DOC");
            
            builder.Entity<VisTipoFascEntity>().ToTable("DPA_VIS_TIPO_FASC");
            builder.Entity<VisTipoFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<VisTipoFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<AROggCustomDocEntity>().ToTable("DPA_A_R_OGG_CUSTOM_DOC");
            builder.Entity<AROggCustomDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AROggCustomDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_A_R_OGG_CUSTOM_DOC");

            builder.Entity<AROggCustomFascEntity>().ToTable("DPA_A_R_OGG_CUSTOM_FASC");
            builder.Entity<AROggCustomFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AROggCustomFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_A_R_OGG_CUSTOM_FASC");

            builder.Entity<ADLFlashbackEntity>().ToTable("DPA_ADL_FLASHBACK");
            builder.Entity<ADLFlashbackEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ADLFlashbackEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<AlboDocPubbEntity>().ToTable("DPA_ALBO_DOC_PUBB");
            builder.Entity<AlboDocPubbEntity>().HasKey(p => p.DOCNUMBER);

            builder.Entity<AlboPubbVerticaliEntity>().ToTable("DPA_ALBO_PUBB_VERTICALI");
            builder.Entity<AlboPubbVerticaliEntity>().HasKey(p => p.CODEAPP_VERTICALE);

            builder.Entity<AlertConservazioneEntity>().ToTable("DPA_ALERT_CONSERVAZIONE");
            builder.Entity<AlertConservazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AlertConservazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ALERT_CONSERVAZIONE");

            builder.Entity<AnagraficaEventiEntity>().ToTable("DPA_ANAGRAFICA_EVENTI");
            builder.Entity<AnagraficaEventiEntity>().HasKey(p => p.ID_EVENTO);

            builder.Entity<AnagraficaEventDocumentEntity>().ToTable("DPA_ANAGRAFICA_EVENT_DOCUMENT");
            builder.Entity<AnagraficaEventDocumentEntity>().HasKey(p => p.SYSTEM_ID);

            builder.Entity<AnagraficaFunzioniEntity>().ToTable("DPA_ANAGRAFICA_FUNZIONI");
            builder.Entity<AnagraficaFunzioniEntity>().HasKey(p => p.COD_FUNZIONE);
            builder.Entity<AnagraficaFunzioniEntity>().Property(p => p.CHA_TIPO_FUNZ).HasColumnType("VARCHAR2(1)");

            builder.Entity<AnagraficaLogEntity>().ToTable("DPA_ANAGRAFICA_LOG");
            builder.Entity<AnagraficaLogEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AnagraficaLogEntity>().Property(p => p.VAR_CODICE).HasColumnType("VARCHAR2");
            builder.Entity<AnagraficaLogEntity>().Property(p => p.VAR_METODO).HasColumnType("VARCHAR2");
            builder.Entity<AnagraficaLogEntity>().Property(p => p.FOLLOW_CONFIG).HasColumnType("CHAR(1)");
            builder.Entity<AnagraficaLogEntity>().Property(p => p.FOLLOW).HasColumnType("CHAR(1)");

            builder.Entity<AreaConservazioneEntity>().ToTable("DPA_AREA_CONSERVAZIONE");
            builder.Entity<AreaConservazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AreaConservazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_CONSERVAZIONE");

            builder.Entity<AssAllegatoEntity>().ToTable("DPA_ASS_ALLEGATO");
            builder.Entity<AssAllegatoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssAllegatoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_ALLEGATO");

            builder.Entity<AssDiagrammiEntity>().ToTable("DPA_ASS_DIAGRAMMI");
            builder.Entity<AssDiagrammiEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssDiagrammiEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_DIAGRAMMI");

            builder.Entity<AssDocMailInteropEntity>().ToTable("DPA_ASS_DOC_MAIL_INTEROP");
            builder.Entity<AssDocMailInteropEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssDocMailInteropEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_DOC_MAIL_INTEROP");

            builder.Entity<AssGridsEntity>().ToTable("DPA_ASS_GRIDS");
            builder.Entity<AssGridsEntity>().HasKey(p => new { p.GRID_ID, p.USER_ID, p.ROLE_ID });

            builder.Entity<AssIndxSisEntity>().ToTable("DPA_ASS_INDX_SIS");
            builder.Entity<AssIndxSisEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssIndxSisEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_INDX_SIS");

            builder.Entity<AssLettereDocumentiEntity>().ToTable("DPA_ASS_LETTERE_DOCUMENTI");
            builder.Entity<AssLettereDocumentiEntity>().HasKey(p => new { p.ID_AMM, p.ID_LETTERADOC });

            builder.Entity<AssPolicyProfilazioneEntity>().ToTable("DPA_ASS_POLICY_PROFILAZIONE");
            builder.Entity<AssPolicyProfilazioneEntity>().HasNoKey();

            builder.Entity<AssPolicyTypeEntity>().ToTable("DPA_ASS_POLICY_TYPE");
            builder.Entity<AssPolicyTypeEntity>().HasNoKey();

            builder.Entity<AssPregressiEntity>().ToTable("DPA_ASS_PREGRESSI");
            builder.Entity<AssPregressiEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssPregressiEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_PREGRESSI");

            builder.Entity<AssRuoloOggCustomEntity>().ToTable("DPA_ASS_RUOLO_OGG_CUSTOM");
            builder.Entity<AssRuoloOggCustomEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssRuoloOggCustomEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<AssRuoloStatiDiagrammaEntity>().ToTable("DPA_ASS_RUOLO_STATI_DIAGRAMMA");
            builder.Entity<AssRuoloStatiDiagrammaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssRuoloStatiDiagrammaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<AssRuoloStatiDiagrammaEntity>().Property(p => p.CHA_NOT_VIS).HasColumnType("VARCHAR2(1)");

            builder.Entity<AssTemplatesFascEntity>().ToTable("DPA_ASS_TEMPLATES_FASC");
            builder.Entity<AssTemplatesFascEntity>().Property(p => p.DTA_INS).HasColumnType("DATE");
            builder.Entity<AssTemplatesFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssTemplatesFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_TEMPLATES_FASC");

            builder.Entity<AssValoriFascEntity>().ToTable("DPA_ASS_VALORI_FASC");
            builder.Entity<AssValoriFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssValoriFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASS_VALORI_FASC");

            builder.Entity<AssociazioneStatiPhasesEntity>().ToTable("DPA_ASSOCIAZIONE_STATI_PHASES");
            builder.Entity<AssociazioneStatiPhasesEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssociazioneStatiPhasesEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<AssociazioneTemplatesTmpEntity>().ToTable("DPA_ASSOCIAZIONE_TEMPLATES_TMP");
            builder.Entity<AssociazioneTemplatesTmpEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssociazioneTemplatesTmpEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASSOCIAZIONE_TEMPLATES");

            builder.Entity<AssociazioneValoriEntity>().ToTable("DPA_ASSOCIAZIONE_VALORI");
            builder.Entity<AssociazioneValoriEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AssociazioneValoriEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ASSOCIAZIONE_VALORI");

            builder.Entity<TrasmissioneEntity>().ToTable("DPA_TRASMISSIONE");
            builder.Entity<TrasmissioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TrasmissioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<TrasmissioneEntity>().HasMany(e => e.TRASM_SINGOLE).WithOne(e => e.TRASMISSIONE).HasForeignKey(e => e.ID_TRASMISSIONE);
            builder.Entity<TrasmissioneEntity>().Property(p => p.DTA_INVIO).HasColumnType("DATE");
            builder.Entity<TrasmissioneEntity>().Property(p => p.CHA_TIPO_OGGETTO).HasColumnType("VARCHAR2(1)");

            builder.Entity<TrasmSingolaEntity>().ToTable("DPA_TRASM_SINGOLA");
            builder.Entity<TrasmSingolaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TrasmSingolaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<TrasmSingolaEntity>().HasMany(e => e.TRASM_UTENTE).WithOne(e => e.TRASM_SINGOLA).HasForeignKey(e => e.ID_TRASM_SINGOLA);
            builder.Entity<TrasmSingolaEntity>().Property(p => p.CHA_TIPO_DEST).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmSingolaEntity>().Property(p => p.CHA_TIPO_TRASM).HasColumnType("VARCHAR2(1)");

            builder.Entity<TrasmUtenteEntity>().ToTable("DPA_TRASM_UTENTE");
            builder.Entity<TrasmUtenteEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TrasmUtenteEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.DTA_ACCETTATA).HasColumnType("DATE");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.DTA_RIFIUTATA).HasColumnType("DATE");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.DTA_RIMOZIONE_TODOLIST).HasColumnType("DATE");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.DTA_RISPOSTA).HasColumnType("DATE");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.DTA_VISTA).HasColumnType("DATE");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_VISTA).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_ACCETTATA).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_RIFIUTATA).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_VALIDA).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_ACCETTATA_DELEGATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_RIFIUTATA_DELEGATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_VISTA_DELEGATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<TrasmUtenteEntity>().Property(p => p.CHA_IN_TODOLIST).HasColumnType("CHAR(1)");


            builder.Entity<AutorizzRuoliProcessiEntity>().ToTable("DPA_AUTORIZZ_RUOLI_PROCESSI");
            builder.Entity<AutorizzRuoliProcessiEntity>().HasNoKey();

            builder.Entity<BigFilesEntity>().ToTable("DPA_BIG_FILES");
            builder.Entity<BigFilesEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<BigFilesEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CacheEntity>().ToTable("DPA_CACHE");
            builder.Entity<CacheEntity>().HasKey(p => p.DOCNUMBER);

            builder.Entity<CanaliEntity>().ToTable("DPA_CANALI");
            builder.Entity<CanaliEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CanaliEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<CanaliEntity>().Property(p => p.VAR_DESC_CANALE).HasColumnType("VARCHAR2(1)");


            builder.Entity<CanaliRegEntity>().ToTable("DPA_CANALI_REG");
            builder.Entity<CanaliRegEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CanaliRegEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CaratTimbroEntity>().ToTable("DPA_CARAT_TIMBRO");
            builder.Entity<CaratTimbroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CaratTimbroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CheckMailboxEntity>().ToTable("DPA_CHECK_MAILBOX");
            builder.Entity<CheckMailboxEntity>().HasKey(p => p.ID);
            builder.Entity<CheckMailboxEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CheckinCheckoutEntity>().ToTable("DPA_CHECKIN_CHECKOUT");
            builder.Entity<CheckinCheckoutEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CheckinCheckoutEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ChiaviConfigTemplateEntity>().ToTable("DPA_CHIAVI_CONFIG_TEMPLATE");
            builder.Entity<ChiaviConfigTemplateEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ChiaviConfigTemplateEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<ChiaviConfigTemplateEntity>().Property(p => p.CHA_INFASATO).HasColumnType("VARCHAR2(1)");

            builder.Entity<ClassificazioneTipiDocEntity>().ToTable("DPA_CLASSIFICAZIONE_TIPI_DOC");
            builder.Entity<ClassificazioneTipiDocEntity>().HasNoKey();

            builder.Entity<ClientModelProcessorsEntity>().ToTable("DPA_CLIENT_MODEL_PROCESSORS");
            builder.Entity<ClientModelProcessorsEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ClientModelProcessorsEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CollMSpedizDocumentoEntity>().ToTable("DPA_COLL_MSPEDIZ_DOCUMENTO");
            builder.Entity<CollMSpedizDocumentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CollMSpedizDocumentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ColoreTimbroEntity>().ToTable("DPA_COLORE_TIMBRO");
            builder.Entity<ColoreTimbroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ColoreTimbroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ConfigAlertConsEntity>().ToTable("DPA_CONFIG_ALERT_CONS");
            builder.Entity<ConfigAlertConsEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConfigAlertConsEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ConfigAnagraficaEsternaEntity>().ToTable("DPA_CONFIG_ANAGRAFICA_ESTERNA");
            builder.Entity<ConfigAnagraficaEsternaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConfigAnagraficaEsternaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ConfigCacheEntity>().ToTable("DPA_CONFIG_CACHE");
            builder.Entity<ConfigCacheEntity>().HasNoKey();

            builder.Entity<ConfigStampaConsEntity>().ToTable("DPA_CONFIG_STAMPA_CONS");
            builder.Entity<ConfigStampaConsEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConfigStampaConsEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ConfigVersamentoEntity>().ToTable("DPA_CONFIG_VERSAMENTO");
            builder.Entity<ConfigVersamentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConfigVersamentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<ConfigVersamentoEntity>().Property(p => p.VAR_TEMPLATE_XML).HasColumnType("CLOB");

            builder.Entity<ConsVerificaEntity>().ToTable("DPA_CONS_VERIFICA");
            builder.Entity<ConsVerificaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConsVerificaEntity>().Property(p => p.DATA_VER).HasColumnType("DATE");
            builder.Entity<ConsVerificaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_CONS_VERIFICA");
            builder.Entity<ConsVerificaEntity>().Property(p => p.CHA_TIPO_VER).HasColumnType("VARCHAR2(1)");
            builder.Entity<ConsVerificaEntity>().Property(p => p.ESITO).HasColumnType("VARCHAR2(1)");

            builder.Entity<ConsolidatedDocsEntity>().ToTable("DPA_CONSOLIDATED_DOCS");
            builder.Entity<ConsolidatedDocsEntity>().HasKey(p => p.ID);
            builder.Entity<ConsolidatedDocsEntity>().Property(p => p.DTA_ANNULLA).HasColumnType("DATE");
            builder.Entity<ConsolidatedDocsEntity>().Property(p => p.DTA_PROTO).HasColumnType("DATE");
            builder.Entity<ConsolidatedDocsEntity>().Property(p => p.DTA_PROTO_IN).HasColumnType("DATE");
            builder.Entity<ConsolidatedDocsEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ContCustomDocEntity>().ToTable("DPA_CONT_CUSTOM_DOC");
            builder.Entity<ContCustomDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ContCustomDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<ContCustomDocEntity>().Property(p => p.SOSPESO).HasColumnType("VARCHAR2");

            builder.Entity<ContCustomFascEntity>().ToTable("DPA_CONT_CUSTOM_FASC");
            builder.Entity<ContCustomFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ContCustomFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<ContCustomFascEntity>().Property(p => p.SOSPESO).HasColumnType("VARCHAR2");

            builder.Entity<ContatoriDocEntity>().ToTable("DPA_CONTATORI_DOC");
            builder.Entity<ContatoriDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ContatoriDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_CONTATORI_DOC");

            builder.Entity<ContatoriFascEntity>().ToTable("DPA_CONTATORI_FASC");
            builder.Entity<ContatoriFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ContatoriFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_CONTATORI_FASC");

            builder.Entity<ContestoProceduraleEntity>().ToTable("DPA_CONTESTO_PROCEDURALE");
            builder.Entity<ContestoProceduraleEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ContestoProceduraleEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ConvPdfServerEntity>().ToTable("DPA_CONV_PDF_SERVER");
            builder.Entity<ConvPdfServerEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ConvPdfServerEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_CONV_PDF_SERVER");
            
            builder.Entity<CorrAbilitatiEntity>().ToTable("DPA_CORR_ABILITATI");
            builder.Entity<CorrAbilitatiEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CorrAbilitatiEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<CorrAbilitatiEntity>().Property(p => p.CHA_TIPO_URP).HasColumnType("VARCHAR2(1)");
            builder.Entity<CorrAbilitatiEntity>().Property(p => p.CHA_APPLICAZIONE).HasColumnType("VARCHAR2");


            builder.Entity<CorrGruppoEntity>().ToTable("DPA_CORR_GRUPPO");
            builder.Entity<CorrGruppoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CorrGruppoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CorrInteropEntity>().ToTable("DPA_CORR_INTEROP");
            builder.Entity<CorrInteropEntity>().HasKey(p => p.ID_CORR);

            builder.Entity<CorrStoEntity>().ToTable("DPA_CORR_STO");
            builder.Entity<CorrStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CorrStoEntity>().Property(p => p.DTA_MODIFICA).HasColumnType("DATE");
            builder.Entity<CorrStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<CorrStoEntity>().Property(p => p.CHA_TIPO_MITT_DES).HasColumnType("VARCHAR2");


            builder.Entity<CountToDoListEntity>().ToTable("DPA_COUNT_TODOLIST");
            builder.Entity<CountToDoListEntity>().HasKey(p => p.ID_PEOPLE);

            builder.Entity<DataArrivoStoEntity>().ToTable("DPA_DATA_ARRIVO_STO");
            builder.Entity<DataArrivoStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DataArrivoStoEntity>().Property(p => p.DTA_ARRIVO).HasColumnType("DATE");
            builder.Entity<DataArrivoStoEntity>().Property(p => p.DTA_MODIFICA).HasColumnType("DATE");
            builder.Entity<DataArrivoStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<MailRegistriEntity>().ToTable("DPA_MAIL_REGISTRI");
            builder.Entity<MailRegistriEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MailRegistriEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_MAIL_REGISTRI");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_IMAP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_SALVA_MAIL_LOC).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_SMTP_SSL).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.VAR_PRINCIPALE).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_SMTP_STA).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.VAR_SOLO_MAIL_PEC).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_RICEVUTA_PEC).HasColumnType("VARCHAR2");
            builder.Entity<MailRegistriEntity>().Property(e => e.VAR_MAIL_RIC_PENDENTE).HasColumnType("VARCHAR2(1)");
            builder.Entity<MailRegistriEntity>().Property(e => e.CHA_POP_SSL).HasColumnType("VARCHAR2(1)");

            builder.Entity<PosizTimbroEntity>().ToTable("DPA_POSIZ_TIMBRO");
            builder.Entity<PosizTimbroEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PosizTimbroEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<RegProtoEntity>().ToTable("DPA_REG_PROTO");
            builder.Entity<RegProtoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RegProtoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<RegFascEntity>().ToTable("DPA_REG_FASC");
            builder.Entity<RegFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RegFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<IstanzaProcessoFirmaEntity>().ToTable("DPA_ISTANZA_PROCESSO_FIRMA");
            builder.Entity<IstanzaProcessoFirmaEntity>().HasKey(p => p.ID_ISTANZA);
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.CONCLUSO_IL).HasColumnType("DATE");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.ATTIVATO_IL).HasColumnType("DATE");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.STATO).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.DOC_ALL).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.MOTIVO_RESPINGIMENTO).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.NOTIFICA_INTERROTTO).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.NOTIFICA_CONCLUSO).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.DESCRIZIONE).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.NOTE).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.CHA_INTERROTTO_DA).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.CHA_CAMBIO_STATO_DIAG).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.NOTIFICA_ERRORE).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.NOTIFICA_DEST_NON_INTEROP).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaProcessoFirmaEntity>().Property(p => p.ID_ISTANZA).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ISTANZA_PROCESSO_FIRMA");

            builder.Entity<IstanzaPassoFirmaEntity>().ToTable("DPA_ISTANZA_PASSO_FIRMA");
            builder.Entity<IstanzaPassoFirmaEntity>().HasKey(p => p.ID_ISTANZA_PASSO);
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.VAR_ERRORE).HasColumnType("CLOB");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.ESEGUITO_IL).HasColumnType("DATE");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.STATO_PASSO).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.MOTIVO_RESPINGIMENTO).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.DESC_UTENTE_LOCKER).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.TIPO_FIRMA).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.NOTE).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.CHA_AUTOMATICO).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.CHA_POS_SEGNATURA).HasColumnType("CHAR(1)");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.VAR_POS_SEGNATURA).HasColumnType("VARCHAR2");
            builder.Entity<IstanzaPassoFirmaEntity>().Property(p => p.ID_ISTANZA_PASSO).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ISTANZA_PASSO_FIRMA");

            builder.Entity<DatiFatturazioneEntity>().ToTable("DPA_DATI_FATTURAZIONE");
            builder.Entity<DatiFatturazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DatiFatturazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<DatiScaricatiEntity>().ToTable("DPA_DATI_SCARICATI");
            builder.Entity<DatiScaricatiEntity>().HasNoKey();

            builder.Entity<DelegaEntity>().ToTable("DPA_DELEGHE");
            builder.Entity<DelegaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DelegaEntity>().Property(p => p.DATA_SCADENZA).HasColumnType("DATE");
            builder.Entity<DelegaEntity>().Property(p => p.DATA_DECORRENZA).HasColumnType("DATE");
            builder.Entity<DelegaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<DescrizioneFascEntity>().ToTable("DPA_DESCRIZIONI_FASC");
            builder.Entity<DescrizioneFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DescrizioneFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DESCRIZIONI_FASC");

            builder.Entity<DesktopAppEntity>().ToTable("DPA_DESKTOP_APPS");
            builder.Entity<DesktopAppEntity>().HasKey(p => new { p.NOME, p.VERSIONE });

            builder.Entity<DettGlobaliEntity>().ToTable("DPA_DETT_GLOBALI");
            builder.Entity<DettGlobaliEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DettGlobaliEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<DettGlobaliEntity>().Property(p => p.VAR_PROVINCIA).HasColumnType("VARCHAR2");

            builder.Entity<DettPolicyEsecuzioneEntity>().ToTable("DPA_DETT_POLICY_ESECUZIONE");
            builder.Entity<DettPolicyEsecuzioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DettPolicyEsecuzioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DETT_POLICY_ESECUZIONE");

            builder.Entity<DiagrammiEntity>().ToTable("DPA_DIAGRAMMI");
            builder.Entity<DiagrammiEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DiagrammiEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DIAGRAMMI");

            builder.Entity<DiagrammiStatoEntity>().ToTable("DPA_DIAGRAMMI_STATO");
            builder.Entity<DiagrammiStatoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DiagrammiStatoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DIAGRAMMI_STATO");

            builder.Entity<DiagrammiStoEntity>().ToTable("DPA_DIAGRAMMI_STO");
            builder.Entity<DiagrammiStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DiagrammiStoEntity>().Property(p => p.DTA_DATE).HasColumnType("DATE");
            builder.Entity<DiagrammiStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DIAGRAMMI_STO");

            builder.Entity<DispositivoStampaEntity>().ToTable("DPA_DISPOSITIVI_STAMPA");
            builder.Entity<DispositivoStampaEntity>().HasKey(p => p.ID);

            builder.Entity<DisservizioEntity>().ToTable("DPA_DISSERVIZI");
            builder.Entity<DisservizioEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DisservizioEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_DISSERVIZI");

            builder.Entity<DocsPaEntity>().ToTable("DPA_DOCSPA");
            builder.Entity<DocsPaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<DocsPaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<DocCollegamentoEntity>().ToTable("DPA_DOC_COLLEGAMENTI");
            builder.Entity<DocCollegamentoEntity>().HasKey(p => new { p.ID_DOCUMENTO, p.ID_DOC_COLLEGATO, p.ID_TIPO_COLLEGAMENTO });

            builder.Entity<DocPubEntity>().ToTable("DPA_DOC_PUB");
            builder.Entity<DocPubEntity>().HasKey(p => p.ID);
            builder.Entity<DocPubEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ElementoInLibroFirmaStorEntity>().ToTable("DPA_ELEMENTO_IN_LF_STOR");
            builder.Entity<ElementoInLibroFirmaStorEntity>().HasKey(p => p.ID_ELEMENTO);

            builder.Entity<ElementoInLibroFirmaEntity>().ToTable("DPA_ELEMENTO_IN_LIBRO_FIRMA");
            builder.Entity<ElementoInLibroFirmaEntity>().HasKey(p => p.ID_ELEMENTO);
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.DATA_INSERIMENTO).HasColumnType("DATE");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.DTA_ACCETTAZIONE).HasColumnType("DATE");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.DTA_ESECUZIONE).HasColumnType("DATE");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.TIPO_FIRMA).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.STATO_FIRMA).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.NOTE).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.RUOLO_PROPONENTE).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.UTENTE_PROPONENTE).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.MODALITA).HasColumnType("VARCHAR2");
            builder.Entity<ElementoInLibroFirmaEntity>().Property(p => p.ID_ELEMENTO).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<CanaleCorrEntity>().ToTable("DPA_T_CANALE_CORR");
            builder.Entity<CanaleCorrEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<CanaleCorrEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<CanaleCorrEntity>().Property(p => p.CHA_PREFERITO).HasColumnType("VARCHAR2(1)");

            builder.Entity<ElencoNoteEntity>().ToTable("DPA_ELENCO_NOTE");
            builder.Entity<ElencoNoteEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ElencoNoteEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<EsecuzionePolicyParerEntity>().ToTable("DPA_ESECUZIONE_POLICY_PARER");
            builder.Entity<EsecuzionePolicyParerEntity>().HasKey(p => p.ID_POLICY);

            builder.Entity<EsecuzionePolicyFascParerEntity>().ToTable("DPA_ESEC_POLICY_FASC_PARER");
            builder.Entity<EsecuzionePolicyFascParerEntity>().HasKey(p => p.ID_POLICY);

            builder.Entity<EsitoVerificaConsEntity>().ToTable("DPA_ESITO_VERIFICA_CONS");
            builder.Entity<EsitoVerificaConsEntity>().HasKey(p => p.SYSTEM_ID);

            builder.Entity<EventDocumentEntity>().ToTable("DPA_EVENT_DOCUMENT");
            builder.Entity<EventDocumentEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<EventDocumentEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_EVENT_DOCUMENT");

            builder.Entity<EventMonitorEntity>().ToTable("DPA_EVENT_MONITOR");
            builder.Entity<EventMonitorEntity>().HasKey(p => new { p.ID_DOCUMENTO, p.ID_LOG, p.ID_EVENTO, p.ID_GROUP, p.ID_PEOPLE_AZIONE });

            builder.Entity<EventTypeAssertionEntity>().ToTable("DPA_EVENT_TYPE_ASSERTIONS");
            builder.Entity<EventTypeAssertionEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<EventTypeAssertionEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ExternalSystemEntity>().ToTable("DPA_EXTERNAL_SYSTEMS");
            builder.Entity<ExternalSystemEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ExternalSystemEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_EXTERNAL_SYSTEMS");

            builder.Entity<ExtAppEntity>().ToTable("DPA_EXT_APPS");
            builder.Entity<ExtAppEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ExtAppEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_EXT_APPS");

            builder.Entity<FascicolazioneCartaceaEntity>().ToTable("DPA_FASCICOLAZIONE_CARTACEA");
            builder.Entity<FascicolazioneCartaceaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FascicolazioneCartaceaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FatturaEntity>().ToTable("DPA_FATTURA");
            builder.Entity<FatturaEntity>().HasKey(p => new { p.IDAMMINISTRAZIONE, p.IDTIPOATTO });

            builder.Entity<FatturaTibcoLogEntity>().ToTable("DPA_FATTURA_TIBCO_LOG");
            builder.Entity<FatturaTibcoLogEntity>().HasKey(p => p.DOCNUMBER);

            builder.Entity<SchemaProcessoFirmaEntity>().ToTable("DPA_SCHEMA_PROCESSO_FIRMA");
            builder.Entity<SchemaProcessoFirmaEntity>().HasKey(p => p.ID_PROCESSO);
            builder.Entity<SchemaProcessoFirmaEntity>().Property(p => p.DTA_CREAZIONE).HasColumnType("DATE");
            builder.Entity<SchemaProcessoFirmaEntity>().Property(p => p.NOME).HasColumnType("VARCHAR2");
            builder.Entity<SchemaProcessoFirmaEntity>().Property(p => p.TICK).HasColumnType("CHAR(1)");
            builder.Entity<SchemaProcessoFirmaEntity>().Property(p => p.CHA_MODELLO).HasColumnType("CHAR(1)");
            builder.Entity<SchemaProcessoFirmaEntity>().Property(p => p.ID_PROCESSO).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_SCHEMA_PROCESSO_FIRMA");

            builder.Entity<FormatoDocumentoEntity>().ToTable("DPA_FORMATI_DOCUMENTO");
            builder.Entity<FormatoDocumentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FormatoDocumentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FattAttivaCodFornitoreEntity>().ToTable("DPA_FATT_ATTIVA_COD_FORNITORE");
            builder.Entity<FattAttivaCodFornitoreEntity>().HasNoKey();

            builder.Entity<FattAttivaLogPisEntity>().ToTable("DPA_FATT_ATTIVA_LOG_PIS");
            builder.Entity<FattAttivaLogPisEntity>().HasNoKey();

            builder.Entity<FattMailElaborateEntity>().ToTable("DPA_FATT_MAIL_ELABORATE");
            builder.Entity<FattMailElaborateEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FattMailElaborateEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FirmatarioEntity>().ToTable("DPA_FIRMATARI");
            builder.Entity<FirmatarioEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FirmatarioEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FirmatarioDocEntity>().ToTable("DPA_FIRMATARIO_DOC");
            builder.Entity<FirmatarioDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FirmatarioDocEntity>().Property(p => p.DTA_FIRMA).HasColumnType("DATE");
            builder.Entity<FirmatarioDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_FIRMATARIO_DOC");

            builder.Entity<FirmaElettronicaEntity>().ToTable("DPA_FIRMA_ELETTRONICA");
            builder.Entity<FirmaElettronicaEntity>().HasKey(p => p.ID_FIRMA);
            builder.Entity<FirmaElettronicaEntity>().Property(p => p.DATA_APPOSIZIONE).HasColumnType("DATE");
            builder.Entity<FirmaElettronicaEntity>().Property(p => p.ID_FIRMA).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_FIRMA_ELETTRONICA");

            builder.Entity<FirmaVersEntity>().ToTable("DPA_FIRMA_VERS");
            builder.Entity<FirmaVersEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FirmaVersEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FlussoMessaggiEntity>().ToTable("DPA_FLUSSO_MESSAGGI");
            builder.Entity<FlussoMessaggiEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FlussoMessaggiEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FlussoProceduraleEntity>().ToTable("DPA_FLUSSO_PROCEDURALE");
            builder.Entity<FlussoProceduraleEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FlussoProceduraleEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FormattaFascEntity>().ToTable("DPA_FORMATTA_FASC");
            builder.Entity<FormattaFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FormattaFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<FormattaFascEntity>().Property(p => p.CHA_VISUALIZZA).HasColumnType("VARCHAR2(1)");

            builder.Entity<FormattaSegnEntity>().ToTable("DPA_FORMATTA_SEGN");
            builder.Entity<FormattaSegnEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FormattaSegnEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FormatoDocumentoEntity>().ToTable("DPA_FORMATI_DOCUMENTO");
            builder.Entity<FormatoDocumentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FormatoDocumentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<FriendApplicationEntity>().ToTable("DPA_FRIEND_APPLICATION");
            builder.Entity<FriendApplicationEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FriendApplicationEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_FRIEND_APPLICATION");

            builder.Entity<FsMigrLogEntity>().ToTable("DPA_FS_MIGR_LOG");
            builder.Entity<FsMigrLogEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FsMigrLogEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_FS_MIGR_LOG");

            builder.Entity<FunzioneEntity>().ToTable("DPA_FUNZIONI");
            builder.Entity<FunzioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<FunzioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<FunzioneEntity>().Property(p => p.CHA_FLAG_PARENT).HasColumnType("VARCHAR2(1)");
            builder.Entity<FunzioneEntity>().Property(p => p.CHA_TIPO_FUNZ).HasColumnType("VARCHAR2(1)");


            builder.Entity<GridEntity>().ToTable("DPA_GRIDS");
            builder.Entity<GridEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<GridEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_GRIDS");
            builder.Entity<GridEntity>().Property(p => p.SERIALIZED_GRID).HasColumnType("CLOB");

            builder.Entity<IndexSisEntity>().ToTable("DPA_INDX_SIS");
            builder.Entity<IndexSisEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<IndexSisEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_INDX_SIS");

            builder.Entity<InfoComuniEntity>().ToTable("DPA_INFO_COMUNI");
            builder.Entity<InfoComuniEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InfoComuniEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_INFO_COMUNI");

            builder.Entity<InfoFileEntity>().ToTable("DPA_INFO_FILE");
            builder.Entity<InfoFileEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InfoFileEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_INFO_FILE");

            builder.Entity<InfoFirmaDigitaleEntity>().ToTable("DPA_INFO_FIRMA_DIGITALE");
            builder.Entity<InfoFirmaDigitaleEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InfoFirmaDigitaleEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<IntegrCdsEntity>().ToTable("DPA_INTEGR_CDS");
            builder.Entity<IntegrCdsEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<IntegrCdsEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_INTEGR_CDS");

            builder.Entity<IstanzaProcFirmaStoEntity>().ToTable("DPA_ISTANZA_PROC_FIRMA_STO");
            builder.Entity<IstanzaProcFirmaStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<IstanzaProcFirmaStoEntity>().Property(p => p.DTA_DATE).HasColumnType("DATE");
            builder.Entity<IstanzaProcFirmaStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ISTANZA_PROC_FIRMA_STO");

            builder.Entity<ItemConservazioneEntity>().ToTable("DPA_ITEMS_CONSERVAZIONE");
            builder.Entity<ItemConservazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ItemConservazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_ITEMS_CONSERVAZIONE");

            builder.Entity<JobEntity>().ToTable("DPA_JOBS");
            builder.Entity<JobEntity>().HasKey(p => p.ID);
            builder.Entity<JobEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<LdapConfigEntity>().ToTable("DPA_LDAP_CONFIG");
            builder.Entity<LdapConfigEntity>().HasKey(p => p.ID_AMM);

            builder.Entity<LdapSyncHistoryEntity>().ToTable("DPA_LDAP_SYNC_HISTORY");
            builder.Entity<LdapSyncHistoryEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LdapSyncHistoryEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_LDAP_SYNC_HISTORY");

            builder.Entity<LegislaturaEntity>().ToTable("DPA_LEGISLATURE");
            builder.Entity<LegislaturaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LegislaturaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<LetteraDocumentoEntity>().ToTable("DPA_LETTERE_DOCUMENTI");
            builder.Entity<LetteraDocumentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LetteraDocumentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_LETTERE_DOCUMENTI");

            builder.Entity<LibroFirmaEntity>().ToTable("DPA_LIBRO_FIRMA");
            builder.Entity<LibroFirmaEntity>().HasKey(p => new { p.ID_AREA, p.RUOLO_TITOLARE, p.UTENTE_TITOLARE });

            builder.Entity<LicEntity>().ToTable("DPA_LIC");
            builder.Entity<LicEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LicEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ListeAppoEntity>().ToTable("DPA_LISTE_APPO");
            builder.Entity<ListeAppoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ListeAppoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ListeDistrEntity>().ToTable("DPA_LISTE_DISTR");
            builder.Entity<ListeDistrEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ListeDistrEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_LISTE_DISTR");

            builder.Entity<LivelloAnomaliaSegnaturaEntity>().ToTable("DPA_LIVELLO_ANOMALIA_SEGNATURA");
            builder.Entity<LivelloAnomaliaSegnaturaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LivelloAnomaliaSegnaturaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<LockEntity>().ToTable("DPA_LOCK");
            builder.Entity<LockEntity>().HasNoKey();

            builder.Entity<LoginEntity>().ToTable("DPA_LOGIN");
            builder.Entity<LoginEntity>().HasKey(p => p.DST);

            builder.Entity<LogAttivatoEntity>().ToTable("DPA_LOG_ATTIVATI");
            builder.Entity<LogAttivatoEntity>().HasKey(p => new { p.SYSTEM_ID_ANAGRAFICA, p.ID_AMM });

            builder.Entity<LogInstallEntity>().ToTable("DPA_LOG_INSTALL");
            builder.Entity<LogInstallEntity>().HasKey(p => p.ID);
            builder.Entity<LogInstallEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<LogSegnaturaProtoEntity>().ToTable("DPA_LOG_SEGNATURA_PROTO");
            builder.Entity<LogSegnaturaProtoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LogSegnaturaProtoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_LOG_SEGNATURA_PROTO");
            builder.Entity<LogSegnaturaProtoEntity>().Property(p => p.ERROR_LOG).HasColumnType("CLOB");

            builder.Entity<LogTipoAttoEntity>().ToTable("DPA_LOG_TIPO_ATTO");
            builder.Entity<LogTipoAttoEntity>().HasNoKey();

            builder.Entity<LAooUoEntity>().ToTable("DPA_L_AOO_UO");
            builder.Entity<LAooUoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<LAooUoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<MailElaborataEntity>().ToTable("DPA_MAIL_ELABORATE");
            builder.Entity<MailElaborataEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MailElaborataEntity>().Property(p => p.DTA_ELAB).HasColumnType("DATE");
            builder.Entity<MailElaborataEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<MailElaborataEntity>().Property(p => p.CHA_RAGIONE_ELAB).HasColumnType("VARCHAR2(1)");

            builder.Entity<MessaggioEsitoFirmaEntity>().ToTable("DPA_MESSAGGIO_ESITO_FIRMA");
            builder.Entity<MessaggioEsitoFirmaEntity>().HasKey(p => p.ID);
            builder.Entity<MessaggioEsitoFirmaEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<MetadatiDocumentoEntity>().ToTable("DPA_METADATI_DOCUMENTO");
            builder.Entity<MetadatiDocumentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MetadatiDocumentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<MetadatiDocumentoEntity>().Property(p => p.METADATI_XML).HasColumnType("CLOB");

            builder.Entity<MetadatiFascicoloEntity>().ToTable("DPA_METADATI_FASCICOLO");
            builder.Entity<MetadatiFascicoloEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MetadatiFascicoloEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_METADATI_FASCICOLO");
            builder.Entity<MetadatiFascicoloEntity>().Property(p => p.METADATI_XML).HasColumnType("CLOB");

            builder.Entity<UoRegEntity>().ToTable("DPA_UO_REG");
            builder.Entity<UoRegEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<UoRegEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ModelloDelegaEntity>().ToTable("DPA_MODELLI_DELEGA");
            builder.Entity<ModelloDelegaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloDelegaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ModelloDestConNotificaEntity>().ToTable("DPA_MODELLI_DEST_CON_NOTIFICA");
            builder.Entity<ModelloDestConNotificaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloDestConNotificaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ModelloDestConNotOrEntity>().ToTable("DPA_MODELLI_DEST_CON_NOT_OR");
            builder.Entity<ModelloDestConNotOrEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloDestConNotOrEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<StatoInvioEntity>().ToTable("DPA_STATO_INVIO");
            builder.Entity<StatoInvioEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<StatoInvioEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<StatoInvioEntity>().Property(e => e.DTA_SPEDIZIONE).HasColumnType("DATE");
            builder.Entity<StatoInvioEntity>().Property(e => e.VAR_CODICE_AOO).HasColumnType("VARCHAR2");
            builder.Entity<StatoInvioEntity>().Property(e => e.VAR_CODICE_AMM).HasColumnType("VARCHAR2");
            builder.Entity<StatoInvioEntity>().Property(e => e.STATUS_C_MASK).HasColumnType("VARCHAR2");
            builder.Entity<StatoInvioEntity>().Property(p => p.CHA_INTEROP).HasColumnType("VARCHAR2(1)");
            builder.Entity<StatoInvioEntity>().Property(p => p.VAR_PROVINCIA).HasColumnType("VARCHAR2");
            builder.Entity<StatoInvioEntity>().Property(p => p.CHA_ANNULLATO).HasColumnType("VARCHAR2(1)");


            builder.Entity<NotaEntity>(entity =>
            {
                entity.ToTable("DPA_NOTE");

                entity.HasKey(p => p.SYSTEM_ID);
                entity.Property(e => e.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ"); ;
                entity.Property(e => e.DATACREAZIONE).HasColumnType("DATE");
                entity.Property(e => e.IDOGGETTOASSOCIATO);
                entity.Property(e => e.IDPEOPLEDELEGATO);
                entity.Property(e => e.IDRFASSOCIATO);
                entity.Property(e => e.IDRUOLOCREATORE);
                entity.Property(e => e.IDUTENTECREATORE);
                entity.Property(e => e.TIPOOGGETTOASSOCIATO);
                entity.Property(e => e.TIPOVISIBILITA);
                entity.Property(e => e.TIPOVISIBILITA).HasColumnType("CHAR(1)");
                entity.Property(e => e.TIPOOGGETTOASSOCIATO).HasColumnType("CHAR(1)");
            });

            builder.Entity<ModelloMdAppoEntity>().ToTable("DPA_MODELLI_MD_APPO");
            builder.Entity<ModelloMdAppoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloMdAppoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ModelloMittDestEntity>().ToTable("DPA_MODELLI_MITT_DEST");
            builder.Entity<ModelloMittDestEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloMittDestEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_MODELLI_MITT_DEST");
            builder.Entity<ModelloMittDestEntity>().Property(p => p.CHA_TIPO_TRASM).HasColumnType("VARCHAR2(1)");
            builder.Entity<ModelloMittDestEntity>().Property(p => p.CHA_TIPO_URP).HasColumnType("VARCHAR2(1)");
            builder.Entity<ModelloMittDestEntity>().Property(p => p.CHA_TIPO_MITT_DEST).HasColumnType("VARCHAR2");
            builder.Entity<ModelloMittDestEntity>().Property(p => p.CHA_TIPO_MITT_DEST).HasColumnType("CHAR(1)");

            builder.Entity<ModelloTrasmEntity>().ToTable("DPA_MODELLI_TRASM");
            builder.Entity<ModelloTrasmEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ModelloTrasmEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_MODELLI_TRASM");
            builder.Entity<ModelloTrasmEntity>().Property(p => p.CHA_TIPO_OGGETTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<ModelloTrasmEntity>().Property(p => p.NO_NOTIFY).HasColumnType("VARCHAR2(1)");
            builder.Entity<ModelloTrasmEntity>().Property(p => p.SINGLE).HasColumnType("VARCHAR2(1)");

            builder.Entity<RelPeopleExtAppsEntity>().ToTable("DPA_REL_PEOPLE_EXTAPPS");
            builder.Entity<RelPeopleExtAppsEntity>().HasKey(p => new { p.ID_PEOPLE, p.ID_EXT_APP });

            builder.Entity<MailRegistroAppoEntity>().ToTable("DPA_MAIL_REGISTRI_APPO");
            builder.Entity<MailRegistroAppoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<MailRegistroAppoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<NotificaEntity>().ToTable("DPA_NOTIFICA");
            builder.Entity<NotificaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<NotificaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_NOTIFICA");
            builder.Entity<NotificaEntity>().Property(p => p.VAR_ERRORE_ESTESO).HasColumnType("CLOB");

            builder.Entity<NotifyEntity>().ToTable("DPA_NOTIFY");
            builder.Entity<NotifyEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<NotifyEntity>().Property(p => p.DTA_EVENT).HasColumnType("DATE");
            builder.Entity<NotifyEntity>().Property(p => p.DTA_NOTIFY).HasColumnType("DATE");
            builder.Entity<NotifyEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<NotifyHistoryEntity>().ToTable("DPA_NOTIFY_HISTORY");
            builder.Entity<NotifyHistoryEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<NotifyHistoryEntity>().Property(p => p.SPECIALIZED_FIELD).HasColumnType("VARCHAR2");
            builder.Entity<NotifyHistoryEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ObjectSyncPendingEntity>().ToTable("DPA_OBJECTS_SYNC_PENDING");
            builder.Entity<ObjectSyncPendingEntity>().HasKey(p => new { p.ID_DOC_OR_FASC, p.ID_GRUPPO_TO_SYNC, p.TYPE });

            builder.Entity<OggettiCustomFascEntity>().ToTable("DPA_OGGETTI_CUSTOM_FASC");
            builder.Entity<OggettiCustomFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettiCustomFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_OGGETTI_CUSTOM_FASC");
            builder.Entity<OggettiCustomFascEntity>().Property(p => p.CONFIG_OBJ_EST).HasColumnType("CLOB");
            builder.Entity<OggettiCustomFascEntity>().Property(p => p.CHA_TIPO_TAR).HasColumnType("VARCHAR2");

            builder.Entity<OggettiCustomCompFascEntity>().ToTable("DPA_OGG_CUSTOM_COMP_FASC");
            builder.Entity<OggettiCustomCompFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<OggettiCustomCompFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_OGG_CUSTOM_COMP_FASC");

            builder.Entity<ParolaEntity>().ToTable("DPA_PAROLE");
            builder.Entity<ParolaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ParolaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<PassoEntity>().ToTable("DPA_PASSI");
            builder.Entity<PassoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PassoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PASSI");

            builder.Entity<PassoDiFirmaEntity>().ToTable("DPA_PASSO_DI_FIRMA");
            builder.Entity<PassoDiFirmaEntity>().HasKey(p => p.ID_PASSO);
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.TIPO_FIRMA).HasColumnType("VARCHAR2");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.NOTE).HasColumnType("VARCHAR2");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.TICK).HasColumnType("CHAR(1)");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.CHA_AUTOMATICO).HasColumnType("CHAR(1)");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.CHA_FACOLTATIVO).HasColumnType("CHAR(1)");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.VAR_POS_SEGNATURA).HasColumnType("VARCHAR2");
            builder.Entity<PassoDiFirmaEntity>().Property(p => p.ID_PASSO).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PASSO_DI_FIRMA");

            builder.Entity<PassoEventoEntity>().ToTable("DPA_PASSO_DPA_EVENTO");
            builder.Entity<PassoEventoEntity>().HasNoKey();

            builder.Entity<PeopleGroupQualificaEntity>().ToTable("DPA_PEOPLEGROUPS_QUALIFICHE");
            builder.Entity<PeopleGroupQualificaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PeopleGroupQualificaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<PeopleOtpResetPasswordEntity>().ToTable("DPA_PEOPLE_OTP_RESET_PASSWORD");
            builder.Entity<PeopleOtpResetPasswordEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PeopleOtpResetPasswordEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<PhaseEntity>().ToTable("DPA_PHASES");
            builder.Entity<PhaseEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PhaseEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<PolicyEsecuzioneEntity>().ToTable("DPA_POLICY_ESECUZIONE");
            builder.Entity<PolicyEsecuzioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PolicyEsecuzioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_POLICY_ESECUZIONE");

            builder.Entity<PolicyFascParerEntity>().ToTable("DPA_POLICY_FASC_PARER");
            builder.Entity<PolicyFascParerEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PolicyFascParerEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_POLICY_FASC_PARER");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_ATTIVA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_DATA_APERTURA_TIPO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_DATA_CHIUSURA_TIPO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_ATTIVA_NOTIFICA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_PERIODICITA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyFascParerEntity>().Property(p => p.CHA_STATO_VERSAMENTO).HasColumnType("VARCHAR2(1)");

            builder.Entity<PolicyParerEntity>().ToTable("DPA_POLICY_PARER");
            builder.Entity<PolicyParerEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PolicyParerEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_REGISTRO_STAMPA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_STATO_VERSAMENTO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_PERIODICITA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_DOC_DIGITALI).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_DATA_PROTO_TIPO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_POLICY).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_CLASS).HasColumnType("VARCHAR2");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_DATA_STAMPA_TIPO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_PROTO_G).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_PROTO_P).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_UO_SOTTOPOSTE).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_DATA_CREAZIONE_TIPO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_SCADENZA_TIMESTAMP).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_ESCLUDI_FATTURE).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_MARCATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_ATTIVA).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_PROTO_A).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_FIRMATO).HasColumnType("VARCHAR2(1)");
            builder.Entity<PolicyParerEntity>().Property(e => e.CHA_TIPO_PROTO_I).HasColumnType("VARCHAR2(1)");
            

            builder.Entity<PrefMobileEntity>().ToTable("DPA_PREF_MOBILE");
            builder.Entity<PrefMobileEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PrefMobileEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PREF_MOBILE");

            builder.Entity<PregressoEntity>().ToTable("DPA_PREGRESSI");
            builder.Entity<PregressoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PregressoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PREGRESSI");

            builder.Entity<PreviewEntity>().ToTable("DPA_PREVIEW");
            builder.Entity<PreviewEntity>().HasKey(p => new {p.DOC_NUMBER, p.VERSION_ID });

            builder.Entity<SimpInteropReceivedMessageEntity>().ToTable("SIMPINTEROPRECEIVEDMESSAGE");
            builder.Entity<SimpInteropReceivedMessageEntity>().HasKey(p => p.MESSAGEID);

            builder.Entity<AppEntity>().ToTable("APPS");
            builder.Entity<AppEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<AppEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_APPS");
            builder.Entity<AppEntity>().Property(p => p.OPEN_LAUNCH).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.VALID_ON_PROFILE).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.SUPER_APP).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.COUNT_KEYS).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.DISABLED).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.DIRMON_STUBCHECK).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.READ_ONLY).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.INTEGRATED).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.DOS_MONITORING).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.PDFCOMPAT).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.ON_DESKTOP).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.VER_TOLERANT).HasColumnType("VARCHAR2(1)");
            builder.Entity<AppEntity>().Property(p => p.USE_UNCNAME).HasColumnType("VARCHAR2(1)");



            builder.Entity<InteroperabilitySettingEntity>().ToTable("INTEROPERABILITYSETTINGS");
            builder.Entity<InteroperabilitySettingEntity>().HasKey(p => p.REGISTRYID);
            builder.Entity<InteroperabilitySettingEntity>().Property(p => p.MANAGEMENTMODE).HasColumnType("VARCHAR2(1)");

            builder.Entity<StatoEntity>().ToTable("DPA_STATI");
            builder.Entity<StatoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<StatoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_STATI");
            builder.Entity<StatoEntity>().Property(p => p.DISABILITA_SALVA_DOCUMENTO).HasColumnType("VARCHAR2(1)");

            builder.Entity<ProfParolaEntity>().ToTable("DPA_PROF_PAROLE");
            builder.Entity<ProfParolaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<ProfParolaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<SwitchServiceEntity>().ToTable("DPA_SWITCH_SERVICES");
            builder.Entity<SwitchServiceEntity>().HasKey(p => p.SERVICE_NAME);
            builder.Entity<SwitchServiceEntity>().Property(p => p.CHA_USE_DATA_PORTAL_API).HasColumnType("VARCHAR2(1)");
            builder.Entity<SwitchServiceEntity>().Property(p => p.CATEGORY).HasColumnType("VARCHAR2");
            builder.Entity<SwitchServiceEntity>().Property(p => p.RESPONSE_AS_RAW).HasColumnType("CHAR(1)");

            builder.Entity<SwitchServiceInteropEntity>().ToTable("DPA_SWITCH_SERVICE_INTEROP");
            builder.Entity<SwitchServiceInteropEntity>().HasKey(p => p.VAR_INTEROP_URL);

            builder.Entity<ProfilStoEntity>().ToTable("DPA_PROFIL_STO");
            builder.Entity<ProfilStoEntity>().HasKey(p => p.SYSTEMID);
            builder.Entity<ProfilStoEntity>().Property(p => p.DTA_MODIFICA).HasColumnType("DATE");
            builder.Entity<ProfilStoEntity>().Property(p => p.SYSTEMID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PROFIL_STO");

            builder.Entity<ProfilFascStoEntity>().ToTable("DPA_PROFIL_FASC_STO");
            builder.Entity<ProfilFascStoEntity>().HasKey(p => p.SYSTEMID);
            builder.Entity<ProfilFascStoEntity>().Property(p => p.DTA_MODIFICA).HasColumnType("DATE");
            builder.Entity<ProfilFascStoEntity>().Property(p => p.SYSTEMID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PROFIL_STO");

            builder.Entity<SalvaRicercaEntity>().ToTable("DPA_SALVA_RICERCHE");
            builder.Entity<SalvaRicercaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<SalvaRicercaEntity>().Property(p => p.VAR_FILTRI_RIC).HasColumnType("CLOB");
            builder.Entity<SalvaRicercaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_SALVA_RICERCHE");

            builder.Entity<RespConsAooEntity>().ToTable("DPA_RESP_CONS_AOO");
            builder.Entity<RespConsAooEntity>().HasNoKey();

            builder.Entity<RegistriRepertorioEntity>().ToTable("DPA_REGISTRI_REPERTORIO");
            builder.Entity<RegistriRepertorioEntity>().HasNoKey();
            builder.Entity<RegistriRepertorioEntity>().Property(p => p.PRINTFREQ).HasColumnType("VARCHAR2");
            builder.Entity<RegistriRepertorioEntity>().Property(p => p.COUNTERSTATE).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistriRepertorioEntity>().Property(p => p.SETTINGSTYPE).HasColumnType("VARCHAR2(1)");
            builder.Entity<RegistriRepertorioEntity>().Property(p => p.RESPRIGHTS).HasColumnType("VARCHAR2");
            builder.Entity<RegistriRepertorioEntity>().Property(p => p.TIPOLOGYKIND).HasColumnType("VARCHAR2");

            builder.Entity<InstanceAccessEntity>().ToTable("INST_ACC");
            builder.Entity<InstanceAccessEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InstanceAccessEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_INST_ACC");

            builder.Entity<InstanceAccessAttEntity>().ToTable("INST_ACC_ATT");
            builder.Entity<InstanceAccessAttEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InstanceAccessAttEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_INST_ACC_ATT");

            builder.Entity<InstanceAccessDocEntity>().ToTable("INST_ACC_DOC");
            builder.Entity<InstanceAccessDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<InstanceAccessDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_INST_ACC_DOC");

            builder.Entity<TipoFRuoloEntity>().ToTable("DPA_TIPO_F_RUOLO");
            builder.Entity<TipoFRuoloEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoFRuoloEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<TipoFunzioneEntity>().ToTable("DPA_TIPO_FUNZIONE");
            builder.Entity<TipoFunzioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoFunzioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<MvProspettiDocClassCompAllEntity>().ToTable("MV_PROSPETTI_DOCCLASSCOMP_ALL");
            builder.Entity<MvProspettiDocClassCompAllEntity>().HasNoKey();

            builder.Entity<VersamentoEntity>().ToTable("DPA_VERSAMENTO");
            builder.Entity<VersamentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<VersamentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_VERSAMENTO");
            builder.Entity<VersamentoEntity>().Property(p => p.VAR_FILE_RISPOSTA).HasColumnType("CLOB");
            builder.Entity<VersamentoEntity>().Property(p => p.VAR_FILE_METADATI).HasColumnType("CLOB");
            builder.Entity<VersamentoEntity>().Property(p => p.CHA_WARNING).HasColumnType("VARCHAR2(1)");
            builder.Entity<VersamentoEntity>().Property(p => p.CHA_STATO).HasColumnType("VARCHAR2");


            builder.Entity<VersamentoFascicoliEntity>().ToTable("DPA_VERSAMENTO_FASCICOLI");
            builder.Entity<VersamentoFascicoliEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<VersamentoFascicoliEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_VERSAMENTO_FASCICOLI");
            builder.Entity<VersamentoFascicoliEntity>().Property(p => p.VAR_FILE_RISPOSTA).HasColumnType("CLOB");
            builder.Entity<VersamentoFascicoliEntity>().Property(p => p.CHA_WARNING).HasColumnType("VARCHAR2(1)");
            builder.Entity<VersamentoFascicoliEntity>().Property(p => p.CHA_STATO).HasColumnType("VARCHAR2");


            builder.Entity<VersamentiPolicyFascEntity>().ToTable("DPA_VERSAMENTI_POLICY_FASC");
            builder.Entity<VersamentiPolicyFascEntity>().HasNoKey();

            builder.Entity<PianoConservazioneEntity>().ToTable("P3_PIANO_CONSERVAZIONE");
            builder.Entity<PianoConservazioneEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PianoConservazioneEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_P3_PIANO_CONSERVAZIONE");

            builder.Entity<PianoConsTipoAttoEntity>().ToTable("P3_PIANO_CONS_TIPO_ATTO");
            builder.Entity<PianoConsTipoAttoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PianoConsTipoAttoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_P3_PIANO_CONS_TIPO_ATTO");

            builder.Entity<PianoConsTipoFascEntity>().ToTable("P3_PIANO_CONS_TIPO_FASC");
            builder.Entity<PianoConsTipoFascEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PianoConsTipoFascEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_P3_PIANO_CONS_TIPO_FASC");

            builder.Entity<PianoConsAssTempoConsEntity>().ToTable("P3_PIANO_CONS_ASS_TEMPO_CONS");
            builder.Entity<PianoConsAssTempoConsEntity>().HasKey(p => p.SYSTEM_ID);

            builder.Entity<RegistroStoEntity>().ToTable("DPA_REGISTRO_STO");
            builder.Entity<RegistroStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RegistroStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<QualificaCorrispondenteEntity>().ToTable("DPA_QUALIFICA_CORRISPONDENTE");
            builder.Entity<QualificaCorrispondenteEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<QualificaCorrispondenteEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<TrasmDiagrEntity>().ToTable("DPA_TRASM_DIAGR");
            builder.Entity<TrasmDiagrEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TrasmDiagrEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TRASM_DIAGR");

            builder.Entity<RoleHistoryEntity>().ToTable("DPA_ROLE_HISTORY");
            builder.Entity<RoleHistoryEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<RoleHistoryEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ToDoListEntity>().ToTable("DPA_TODOLIST");
            builder.Entity<ToDoListEntity>().HasNoKey();

            builder.Entity<NetworkAliasesEntity>().ToTable("NETWORK_ALIASES");
            builder.Entity<NetworkAliasesEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<NetworkAliasesEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<SimpInteropDbLogEntity>().ToTable("SIMPINTEROPDBLOG");
            builder.Entity<SimpInteropDbLogEntity>().HasNoKey();

            builder.Entity<TimestampDocEntity>().ToTable("DPA_TIMESTAMP_DOC");
            builder.Entity<TimestampDocEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TimestampDocEntity>().Property(p => p.DTA_CREAZIONE).HasColumnType("DATE");
            builder.Entity<TimestampDocEntity>().Property(p => p.DTA_SCADENZA).HasColumnType("DATE");
            builder.Entity<TimestampDocEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TIMESTAMP_DOC");

            builder.Entity<PdfConvRequestEntity>().ToTable("DPA_PDFCONV_REQ");
            builder.Entity<PdfConvRequestEntity>().HasKey(p => p.ID);
            builder.Entity<PdfConvRequestEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_PDFCONV_REQ");
            builder.Entity<PdfConvRequestEntity>().Property(p => p.LAST_ERROR).HasColumnType("CLOB");

            builder.Entity<PdfConvRequestHandledEntity>().ToTable("DPA_PDFCONV_REQ_HANDLED");
            builder.Entity<PdfConvRequestHandledEntity>().HasKey(p => p.ID);

            builder.Entity<TipoNotificaEntity>().ToTable("DPA_TIPO_NOTIFICA");
            builder.Entity<TipoNotificaEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<TipoNotificaEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_TIPO_NOTIFICA");

            builder.Entity<SendStoEntity>().ToTable("DPA_SEND_STO");
            builder.Entity<SendStoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<SendStoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_SEND_STO");
            builder.Entity<SendStoEntity>().Property(e => e.DTA_SPEDIZIONE).HasColumnType("DATE");
            builder.Entity<SendStoEntity>().Property(p => p.ESITO).HasColumnType("VARCHAR2");

            builder.Entity<PisAppsArchivePlanEntity>().ToTable("P3_PIS_APPS_ARCHIVEPLANS");
            builder.Entity<PisAppsArchivePlanEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<PisAppsArchivePlanEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_P3_PIS_APPS_ARCHIVEPLANS");

            builder.Entity<UltimiDocVisualizzatiEntity>().ToTable("DPA_ULTIMI_DOC_VISUALIZZATI");
            builder.Entity<UltimiDocVisualizzatiEntity>().HasKey(u => u.SYSTEM_ID);
            builder.Entity<UltimiDocVisualizzatiEntity>().Property(p => p.DTA_VISUALIZZAZIONE).HasColumnType("DATE");
            builder.Entity<UltimiDocVisualizzatiEntity>().Property(u => u.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");


            builder.Entity<UoSmistamentoEntity>().ToTable("DPA_UO_SMISTAMENTO");
            builder.Entity<UoSmistamentoEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<UoSmistamentoEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ProcessoFirmaVisibilitaEntity>().ToTable("DPA_PROCESSO_FIRMA_VISIBILITA");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().HasKey(p => new { p.ID_PROCESSO, p.ID_GROUPS });
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_CONCLUSO).HasDefaultValue("0");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_INTERROTTO).HasDefaultValue("0");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_ERRORE).HasDefaultValue("1");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.DTA_INIZIO).HasColumnType("DATE");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.DTA_FINE).HasColumnType("DATE");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_TIPO_VISIBILITA).HasColumnType("CHAR(2)");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_CONCLUSO).HasColumnType("CHAR(1)");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_INTERROTTO).HasColumnType("CHAR(1)");
            builder.Entity<ProcessoFirmaVisibilitaEntity>().Property(p => p.CHA_NOTIFICA_ERRORE).HasColumnType("CHAR(1)");

            builder.Entity<VisMailRegistriEntity>().ToTable("DPA_VIS_MAIL_REGISTRI");
            builder.Entity<VisMailRegistriEntity>().HasKey(p => new { p.SYSTEM_ID });
            builder.Entity<VisMailRegistriEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_VIS_MAIL_REGISTRI");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_CONSULTA).HasDefaultValue("1");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_NOTIFICA).HasDefaultValue("1");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_SPEDISCI).HasDefaultValue("1");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_NOTIFICA).HasColumnType("VARCHAR2(1)");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_SPEDISCI).HasColumnType("VARCHAR2(1)");
            builder.Entity<VisMailRegistriEntity>().Property(p => p.CHA_CONSULTA).HasColumnType("VARCHAR2(1)");

            builder.Entity<MvDocumentiCustomEntity>().ToTable("MV_DOCUMENTI_CUSTOM");
            builder.Entity<MvDocumentiCustomEntity>().Property(p => p.DOC_NUMBER).HasColumnType("VARCHAR2");
            builder.Entity<MvDocumentiCustomEntity>().HasNoKey();

            builder.Entity<StampaRepertoriEntity>().ToTable("DPA_STAMPA_REPERTORI");
            builder.Entity<StampaRepertoriEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<StampaRepertoriEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ_DPA_STAMPA_REPERTORI");

            builder.Entity<StampaRegistriEntity>().ToTable("DPA_STAMPAREGISTRI");
            builder.Entity<StampaRegistriEntity>().HasKey(p => p.SYSTEM_ID);
            builder.Entity<StampaRegistriEntity>().Property(p => p.SYSTEM_ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ReportMailboxEntity>().ToTable("DPA_REPORT_MAILBOX");
            builder.Entity<ReportMailboxEntity>().HasKey(p => p.ID);
            builder.Entity<ReportMailboxEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ");

            builder.Entity<ReportVersamentoEntity>().ToTable("DPA_REPORT_VERSAMENTO");
            builder.Entity<ReportVersamentoEntity>().HasNoKey();

            builder.Entity<AssProviderLibEntity>().ToTable("DPA_ASS_PROVIDER_LIB");
            builder.Entity<AssProviderLibEntity>().HasNoKey();

            builder.Entity<VersamentiPolicyEntity>().ToTable("DPA_VERSAMENTI_POLICY");
            builder.Entity<VersamentiPolicyEntity>().HasNoKey();

            builder.Entity<ProfileFullTextEntity>().HasNoKey().ToView(null);

            builder.Entity<CorrGlobaliFullTextEntity>().HasNoKey().ToView(null);

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDescCorr)))
                .HasName("GETDESCCORR");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.VarDescribe)))
                .HasName("VARDESCRIBE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.GetPeopleName)))
               .HasName("GETPEOPLENAME");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr)))
               .HasName("GETCODRUOLOBYIDCORR");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.GetPeopleUserId)))
               .HasName("GETPEOPLEUSERID");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.CorrCatByTipo)))
               .HasName("CORRCATBYTIPO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.GetSegnaturaRepertorio)))
               .HasName("GETSEGNATURAREPERTORIO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.IsCorrispondenteInterno)))
              .HasName("ISCORRISPONDENTEINTERNO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetChaImg)))
              .HasName("GETCHAIMG");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetChaFirmato)))
              .HasName("GETCHAFIRMATO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetNomeOriginale)))
              .HasName("GETNOMEORIGINALE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetImpronta)))
              .HasName("GETIMPRONTA");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetImprontaWithAllegati)))
                .HasName("GETIMPRONTAWITHALLEGATI");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.CorrCat)))
              .HasName("CORRCAT");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetRegDescr)))
              .HasName("GETREGDESCR");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDescTipoDoc)))
              .HasName("GETDESCTIPODOC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodeProject)))
              .HasName("GETCODEPROJECT");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodTit2)))
              .HasName("GETCODTIT2");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodTit)))
              .HasName("GETCODTIT");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.IsOggettoModificato)))
                .HasName("ISOGGETTOMODIFICATO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.ClassCat)))
                .HasName("CLASSCAT");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDescTitolario)))
                .HasName("GETDESCTITOLARIO");
            
            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodRegCorcat)))
                .HasName("GETCODREGCORCAT");
            
            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValProfObjPrj)))
                .HasName("GETVALPROFOBJPRJ");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.HasChildren)))
                .HasName("HAS_CHILDREN");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodReg)))
                .HasName("GETCODREG");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.MailENoteCorrEsterni)))
                .HasName("MAIL_E_NOTE_CORR_ESTERNI");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetChaConsentiClass)))
                .HasName("GETCHACONSENTICLASS");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetChaConsentiFasc)))
                .HasName("GETCHACONSENTIFASC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetInAdl)))
                .HasName("GETINADL");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.IsVersionVisible)))
                .HasName("ISVERSIONVISIBLE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetTestoUltimaNota)))
                .HasName("GETTESTOULTIMANOTA");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetInConservazione)))
                .HasName("GETINCONSERVAZIONE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDescTipoFasc)))
                .HasName("GETDESCTIPOFASC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDateInADL)))
                .HasName("GETDATEINADL");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetMotivoADL)))
                .HasName("GETMOTIVOADL");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetTipologiaFascicoloPianoCons)))
                .HasName("GETTIPOLOGIAFASCICOLOPIANOCONS");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.EsisteNotaVisibile)))
                .HasName("ESISTENOTAVISIBILE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetStatoConservazioneFasc)))
                .HasName("GETSTATOCONSERVAZIONEFASC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDiagrammiStato)))
                .HasName("GETDIAGRAMMISTATO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                 .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValProfObjsPrjAsJson)))
                 .HasName("GET_IDOBJECTJSON");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                 .GetMethod(nameof(IPi3DbContextMappedFunctions.GetInConservazioneNoSec)))
                 .HasName("GETINCONSERVAZIONENOSEC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetContatoreFasc)))
                .HasName("GETCONTATOREFASC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetEsitoPubblicazione)))
                .HasName("GETESITOPUBBLICAZIONE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetDataArrivoDoc)))
              .HasName("GETDATAARRIVODOC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetChaTipoFirma)))
              .HasName("GETCHATIPOFIRMA");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetContatoreDoc)))
              .HasName("GETCONTATOREDOC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetImprontaWithAttachSearch)))
              .HasName("GETIMPRONTAWITHATTACHSEARCH");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetEsitoSpedizione)))
              .HasName("GETESITOSPEDIZIONE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCountRicevuteInterop)))
              .HasName("GETCOUNTRICEVUTEINTEROP");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetStatoConservazione)))
              .HasName("GETSTATOCONSERVAZIONE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetPolicyVersamentoCod)))
              .HasName("GETPOLICYVERSAMENTOCOD");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
              .GetMethod(nameof(IPi3DbContextMappedFunctions.GetPolicyVersamentoCounter)))
              .HasName("GETPOLICYVERSAMENTOCOUNTER");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetPolicyVersamentoDataExec)))
                .HasName("GETPOLICYVERSAMENTODATAEXEC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValProfObjsDocAsJson)))
                .HasName("GET_IDOBJECTJSONDOC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.Contains)))
                .HasName("CONTAINS");
            
            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValCampoProfDoc)))
                .HasName("GETVALCAMPOPROFDOC");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetContatoreFascContatore)))
                .HasName("GETCONTATOREFASCCONTATORE");
            
            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValCampoProfDocOrder)))
                .HasName("GETVALCAMPOPROFDOCORDER");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetContatoreDocOrdinamento)))
                .HasName("GETCONTATOREDOCORDINAMENTO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValProfObjPrjOrder)))
                .HasName("GETVALPROFOBJPRJORDER");
            
            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetContatoreDoc2)))
                .HasName("GETCONTATOREDOC2");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodUo)))
                .HasName("GETCODUO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.CountAllegatiByDocNumber)))
                .HasName("COUNTALLEGATI_BY_DOCNUMBER");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetCodiceRfByProfileId)))
                .HasName("GETCODICERFBYPROFILEID");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetIdAmm)))
                .HasName("GETIDAMM");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.IsDocCartaceo)))
                .HasName("ISDOCCARTACEO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.AtLeastOneFirmato)))
                .HasName("ATLEASTONEFIRMATO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.AtLeastOneMarcato )))
                .HasName("ATLEASTONEMARCATO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.EsisteDestinatarioMaiTrasmesso)))
                .HasName("ESISTEDESTINATARIOMAITRASMESSO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.EsisteDestinatarioMaiSpedito)))
                .HasName("ESISTEDESTINATARIOMAISPEDITO");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
               .GetMethod(nameof(IPi3DbContextMappedFunctions.CompareDate)))
               .HasName("COMPAREDATE");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValCampoProfDocOrderToNumber)))
                .HasName("GETVALCAMPOPROFDOCORDERTONUM");

            builder.HasDbFunction(typeof(IPi3DbContextMappedFunctions)
                .GetMethod(nameof(IPi3DbContextMappedFunctions.GetValCampoProfDocToDate)))
                .HasName("GETVALCAMPOPROFDOCTODATE");
        }
    }
}
