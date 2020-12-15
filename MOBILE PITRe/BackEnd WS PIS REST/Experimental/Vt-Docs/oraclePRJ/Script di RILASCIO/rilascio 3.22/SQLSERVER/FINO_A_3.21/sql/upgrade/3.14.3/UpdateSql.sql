-- aggiunge il campo cha_segnatura per tener traccia del documento associato alla
-- versione(se contiene segnatura permanente o meno)
if not exists(select * from syscolumns
			where name='CHA_SEGNATURA' and id in
			(select id from sysobjects where name='VERSIONS'
			 and xtype='U'))
BEGIN
ALTER TABLE [@db_user].[VERSIONS] ADD CHA_SEGNATURA VARCHAR(1) NULL ;
END
GO


IF NOT EXISTS (
              SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
              WHERE VAR_CODICE = 'FE_PERMANENT_DISPLAYS_SEGNATURE'
              )
begin
--insert della chiave FE_PERMANENT_DISPLAYS_SEGNATURE in DPA_CHIAVI_CONFIGURAZIONE
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES
           (0,
           'FE_PERMANENT_DISPLAYS_SEGNATURE',
           'Abilita la segnatura permanente sui documenti acquisiti in formato pdf nei protocolli',
           '0',
           'F',
           '1',
		   '1',
           '1',
           NULL)
END
GO

IF NOT EXISTS (
              SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
              WHERE VAR_CODICE = 'FE_MULTI_STAMPA_ETICHETTA'
              )
begin
--insert della chiave FE_PERMANENT_DISPLAYS_SEGNATURE in DPA_CHIAVI_CONFIGURAZIONE
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES
           (0,
           'FE_MULTI_STAMPA_ETICHETTA',
           'Chiave di attivazione della stampa multipla delle etichette nei protocolli in entrata/uscita',
           '0',
           'F',
           '1',
		   '1',
           '1',
           NULL)
END
GO

IF NOT EXISTS (
              SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
              WHERE VAR_CODICE = 'FE_CHECK_TITOLARIO_ATTIVO'
              )
begin
--insert della chiave FE_PERMANENT_DISPLAYS_SEGNATURE in DPA_CHIAVI_CONFIGURAZIONE
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES
           (0,
           'FE_CHECK_TITOLARIO_ATTIVO',
           'Chiave per l''attivazione di default del  titolario attivo nelle maschere di ricerca fascicoli',
           '0',
           'F',
           '1',
		   '1',
           '1',
           NULL)
END
GO




Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
Values      (getdate(), '3.14.3')
GO

