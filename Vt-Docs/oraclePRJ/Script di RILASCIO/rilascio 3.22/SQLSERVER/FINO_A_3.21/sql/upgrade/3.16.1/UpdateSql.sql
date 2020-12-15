-- file sql di update per il CD -- 
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_COLL_MSPEDIZ_DOCUMENTO.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS(SELECT name FROM sysindexes WHERE name = 'IDX_COLL_MSPEDIZ_DOC_1')
CREATE INDEX IDX_COLL_MSPEDIZ_DOC_1 
ON @db_user.DPA_COLL_MSPEDIZ_DOCUMENTO (ID_DOCUMENTTYPES, ID_PROFILE)
GO              
----------- FINE -
              
---- ALTER_DPA_GRIDS.MSSQL.sql  marcatore per ricerca ----
--MODIFICA DPA_GRIDS.GRID_NAME, aggiunge se non esiste la colonna GRID_NAME
if exists(select * from syscolumns where name='GRID_NAME' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
begin                       
	ALTER TABLE @db_user.DPA_GRIDS ALTER COLUMN GRID_NAME VARCHAR(100)
end
else
	ALTER TABLE @db_user.DPA_GRIDS ADD GRID_NAME VARCHAR(100)
GO


--RINOMINA colonna DPA_GRIDS.USER_ID   in	   USER_ID_creatore
if exists(select * from syscolumns where name='USER_ID' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
                       if not exists(select * from syscolumns where name='USER_ID_creatore' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
begin                       
	execute sp_RENAME '@db_user.DPA_GRIDS.USER_ID' , 'USER_ID_creatore', 'COLUMN'
end
GO

--RINOMINA DPA_GRIDS.USER_ID in ROLE_ID_creatore
if exists(select * from syscolumns where name='ROLE_ID' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
                       if not exists(select * from syscolumns where name='ROLE_ID_creatore' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
BEGIN
	execute sp_RENAME '@db_user.DPA_GRIDS.ROLE_ID' , 'ROLE_ID_creatore', 'COLUMN'
END
GO

--ELIMINA DPA_GRIDS.IS_TEMPORARY
if exists(select * from syscolumns where name='IS_TEMPORARY' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.DPA_GRIDS DROP COLUMN IS_TEMPORARY
END
GO

--ELIMINA DPA_GRIDS.IS_ACTIVE
if exists(select * from syscolumns where name='IS_ACTIVE' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.DPA_GRIDS DROP COLUMN IS_ACTIVE
END
GO

--AGGIUNGE DPA_GRIDS.IS_SEARCH_GRID
if not exists(select * from syscolumns where name='IS_SEARCH_GRID' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.DPA_GRIDS
		ADD IS_SEARCH_GRID VARCHAR(1) DEFAULT 'Y' CHECK (IS_SEARCH_GRID in ('Y','N'))
END
GO

--AGGIUNGE DPA_GRIDS.CHA_VISIBILE_A_UTENTE_O_RUOLO
if not exists(select * from syscolumns where name='CHA_VISIBILE_A_UTENTE_O_RUOLO' and id in
                       (select id from sysobjects where name='DPA_GRIDS' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.DPA_GRIDS
		ADD CHA_VISIBILE_A_UTENTE_O_RUOLO VARCHAR(1) 
END
GO

              
----------- FINE -
              
---- ALTER_DPA_SALVA_RICERCHE.MSSQL.sql  marcatore per ricerca ----
--AGGIUNGE DPA_SALVA_RICERCHE.GRID_ID
if not exists(select * from syscolumns where name='GRID_ID' and id in
                       (select id from sysobjects where name='DPA_SALVA_RICERCHE' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.DPA_SALVA_RICERCHE
		ADD GRID_ID INT
END
GO
              
----------- FINE -
              
---- ALTER_INDX_MODELLI_MITT_DEST_1.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'INDX_MODELLI_MITT_DEST_1') 
CREATE INDEX INDX_MODELLI_MITT_DEST_1 
	ON @db_user.DPA_MODELLI_MITT_DEST
	(ID_MODELLO, HIDE_DOC_VERSIONS)
go              
----------- FINE -
              
---- ALTER_PROFILE.MSSQL.SQL  marcatore per ricerca ----
--AGGIUNGE PROFILE.CHA_COD_T_A
if not exists(select * from syscolumns where name='CHA_COD_T_A' and id in
                       (select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.PROFILE ADD CHA_COD_T_A VARCHAR(1024)
END
GO
              
----------- FINE -
              
---- ALTER_PROJECT.MSSQL.SQL  marcatore per ricerca ----
--AGGIUNGE PROJECT.CHA_COD_T_A
if not exists(select * from syscolumns where name='CHA_COD_T_A' and id in
                       (select id from sysobjects where name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE @db_user.PROJECT	ADD CHA_COD_T_A VARCHAR(1024)
END
GO


              
----------- FINE -
              
---- CREATE_DPA_ASS_GRIDS.MSSQL.sql  marcatore per ricerca ----
if not exists(select id 
				from sysobjects 
				where name='DPA_ASS_GRIDS' and xtype='U')
BEGIN

CREATE TABLE @db_user.[DPA_ASS_GRIDS]
( GRID_ID  INTEGER                        NOT NULL ,
  USER_ID    INTEGER  NOT NULL ,
  ROLE_ID    INTEGER  NOT NULL ,
  ADMINISTRATION_ID              INTEGER,
  TYPE_GRID                      VARCHAR(30) 
)

ALTER TABLE @db_user.DPA_ASS_GRIDS ADD 
  CONSTRAINT DPA_ASS_GRIDS_PK
 PRIMARY KEY
 (GRID_ID, USER_ID, ROLE_ID)

ALTER TABLE @db_user.DPA_ASS_GRIDS ADD 
  CONSTRAINT DPA_ASS_GRIDS_R01
 FOREIGN KEY (USER_ID)
 REFERENCES @db_user.PEOPLE (SYSTEM_ID)

ALTER TABLE @db_user.DPA_ASS_GRIDS ADD 
  CONSTRAINT DPA_ASS_GRIDS_R02
 FOREIGN KEY (ROLE_ID)
 REFERENCES @db_user.GROUPS (SYSTEM_ID)

/*  -- commentata 24 gennaio 2012 -- il vincolo su ROLE_DI  solo verso GROUPS.SYSTEM_ID
ALTER TABLE @db_user.DPA_ASS_GRIDS ADD (
  CONSTRAINT DPA_ASS_GRIDS_R03
 FOREIGN KEY (ROLE_ID)
 REFERENCES @db_user.GROUPS (SYSTEM_ID))
 */

END 
GO 

              
----------- FINE -
              
---- CREATE_DPA_PROFIL_FASC_STO.MSSQL.sql  marcatore per ricerca ----
/*
AUTORE:					  GABRIELE SERPI
Data creazione:				  25/0//2011
Scopo della modifica:		CREARE LA TABELLA DPA_PROFIL_FASC_STO
*/

IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_PROFIL_FASC_STO]')
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)
BEGIN
create table [@db_user].DPA_PROFIL_FASC_STO(
systemId INT PRIMARY KEY NOT NULL,
id_template int not null,
dta_modifica date not null,
id_project int not null,
id_ogg_custom int not null,
id_people int not null,
id_ruolo_in_uo int not null,
var_desc_modifica varchar(2000)
)

ALTER TABLE [@db_user].[DPA_PROFIL_FASC_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_PROFIL_FASC_STO_ID_TEMPLATE] FOREIGN KEY([ID_TEMPLATE])
REFERENCES [@db_user].[DPA_TIPO_FASC] ([SYSTEM_ID])
--GO

ALTER TABLE [@db_user].[DPA_PROFIL_FASC_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_FASC_STO_ID_PROFILE] FOREIGN KEY ([ID_PROJECT])
REFERENCES [@db_user].[PROJECT] ([SYSTEM_ID])
--GO

ALTER TABLE [@db_user].[DPA_PROFIL_FASC_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_PROFIL_FASC_STO_id_ogg_custom] FOREIGN KEY([id_ogg_custom])
REFERENCES [@db_user].[DPA_OGGETTI_CUSTOM] ([SYSTEM_ID])
--GO

ALTER TABLE [@db_user].[DPA_PROFIL_FASC_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_FASC_STO_id_people] FOREIGN KEY ([id_people])
REFERENCES [@db_user].[PEOPLE] ([SYSTEM_ID])
--GO

ALTER TABLE [@db_user].[DPA_PROFIL_FASC_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_FASC_STO_id_ruolo_in_uo] FOREIGN KEY ([id_ruolo_in_uo])
REFERENCES [@db_user].[DPA_CORR_GLOBALI] ([SYSTEM_ID])
--GO

END
GO

              
----------- FINE -
              
---- CREATE_DPA_PROFIL_STO.MSSQL.sql  marcatore per ricerca ----
/*
AUTORE:					  GABRIELE SERPI
Data creazione:				  25/0//2011
Scopo della modifica:		CREARE LA TABELLA DPA_PROFIL_STO
*/

IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_PROFIL_STO]')
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)
BEGIN
create table [@db_user].DPA_PROFIL_STO(
systemId INT PRIMARY KEY NOT NULL,
id_template int not null,
dta_modifica date not null,
id_profile int not null,
id_ogg_custom int not null,
id_people int not null,
id_ruolo_in_uo int not null,
var_desc_modifica varchar(2000)
)

ALTER TABLE [@db_user].[DPA_PROFIL_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_PROFIL_STO_ID_TEMPLATE] FOREIGN KEY([ID_TEMPLATE])
REFERENCES [@db_user].[DPA_TIPO_ATTO] ([SYSTEM_ID])
--GO
ALTER TABLE [@db_user].[DPA_PROFIL_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_STO_ID_PROFILE] FOREIGN KEY ([ID_PROFILE])
REFERENCES [@db_user].[PROFILE] ([SYSTEM_ID])
--GO
ALTER TABLE [@db_user].[DPA_PROFIL_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_PROFIL_STO_id_ogg_custom] FOREIGN KEY([id_ogg_custom])
REFERENCES [@db_user].[DPA_OGGETTI_CUSTOM] ([SYSTEM_ID])
--GO
ALTER TABLE [@db_user].[DPA_PROFIL_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_STO_id_people] FOREIGN KEY ([id_people])
REFERENCES [@db_user].[PEOPLE] ([SYSTEM_ID])
--GO
ALTER TABLE [@db_user].[DPA_PROFIL_STO] WITH CHECK ADD CONSTRAINT [FK_DPA_PROFIL_STO_id_ruolo_in_uo] FOREIGN KEY ([id_ruolo_in_uo])
REFERENCES [@db_user].[DPA_CORR_GLOBALI] ([SYSTEM_ID])
--GO

END
GO

              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- dpa_anagrafica_funzioni.MSSQL.sql  marcatore per ricerca ----
--Creazione nuova funzione DPA_ANAGRAFICA_FUNZIONI

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
	where COD_FUNZIONE='ELIMINA_TIPOLOGIA_DOC'))
BEGIN		
		insert into @db_user.DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE
		, CHA_TIPO_FUNZ, DISABLED)
		values ('ELIMINA_TIPOLOGIA_DOC', 'Consente l''eliminazione di una tipologia di un documento.'
		, NULL, 'N')
END
GO               
----------- FINE -
              
---- dpa_anagrafica_log.MSSQL.sql  marcatore per ricerca ----
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] 
	where VAR_CODICE='DOCUMENTOSPEDISCI'))
BEGIN		
		insert into [@db_user].[dpa_anagrafica_log](var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values ('DOCUMENTOSPEDISCI', 'Spedizione documento'
		, 'DOCUMENTO', 'DOCUMENTOSPEDISCI')
END
GO 

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] 
	where VAR_CODICE='RUB_IMP_NEWCORR_WS'))
BEGIN		
		insert into [@db_user].dpa_anagrafica_log(var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values ('RUB_IMP_NEWCORR_WS', 
		'WS rubrica, creazione nuovo corrispondente', 
		'RUBRICA', 'IMPORTARUBRICACREAWS')
END
GO 

declare @ultimoID int

begin

Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ( 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)


Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ('AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)

END              
----------- FINE -
              
---- dpa_chiavi_config_template.MSSQL.sql  marcatore per ricerca ----

--dopo l'inserimento nella tabella dpa_chiavi_config_template, 
-- occorre eseguire la procedura CREA_KEYS_AMMINISTRA per infasare in DPA_CHIAVI_CONFIGURAZIONE
-- quindi, verificare i record in dpa_chiavi_config_template, prima di eseguire la procedura

-- FE_FASC_RAPIDA_REQUIRED	FALSE
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('FE_FASC_RAPIDA_REQUIRED'
   , 'Obbligatorieta della classificazione o fascicolazione rapida'
   , 'false', 'F', '1', '1');

-- BE_SESSION_REPOSITORY_DISABLED	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('BE_SESSION_REPOSITORY_DISABLED'
   , 'Disabilita (TRUE) o Abilita (FALSE) acquisisci file immagine prima di salva/protocolla'
   , 'false', 'B', '1', '1');


-- BE_ELIMINA_MAIL_ELABORATE	0
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('BE_ELIMINA_MAIL_ELABORATE'
   , 'Eliminazione automatica delle mail pervenute su casella PEC processate da PITRE'
   , '0', 'B', '1', '1');

-- FE_TIPO_ATTO_REQUIRED	0
-- TIPO_ATTO_REQUIRED  stato sostituito da: getTipoDocObbl
--Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
--Values (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_TIPO_ATTO_REQUIRED', 'Obbligatorieta della tipologia documento', '0', 'F', '1', '1');


-- FE_PROTOIN_ACQ_DOC_OBBLIGATORIA 	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('FE_PROTOIN_ACQ_DOC_OBBLIGATORIA '
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1');

--FE_SMISTA_ABILITA_TRASM_RAPIDA	1
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('FE_SMISTA_ABILITA_TRASM_RAPIDA'
   , 'Trasmissione rapida obligatoria  sulla protocollazione semplificata e sullo smistamento'
   , '1', 'F', '1', '1');


-- FE_ENABLE_PROT_RAPIDA_NO_UO	true
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('FE_ENABLE_PROT_RAPIDA_NO_UO'
   , 'Trasmissione obbligatoria sulla protocollazione semplificata'
   , 'true', 'F', '1', '1');

-- BE_ELIMINA_RICEVUTE_PEC	0
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE)
 Values
   ('BE_ELIMINA_RICEVUTE_PEC'
   , 'Eliminazione automatica delle RICEVUTE PEC pervenute su casella PEC processate da PITRE '
   , '0', 'B', '1', '1');

              
----------- FINE -
              
---- dpa_chiavi_configurazione.MSSQL.sql  marcatore per ricerca ----
/*
  FE_PAGING_ROW_DOC         Numero di documenti visualizzati nelle ricerche              
  FE_PAGING_ROW_DOCINFASC   Numero di documenti in fascicoli visualizzati nelle ricerche         
  FE_PAGING_ROW_PROJECT     Numero di fascicoli visualizzati nelle ricerche                     
  FE_IS_PRESENT_NOTE		Imposta un valore Si/No se presente o no una nota nelle ricerche e export documenti e fascicoli
  FE_PAGING_ROW_TRASM		Imposta il numero di risultati della griglia di ricerca trasmissioni
*/

--                FE_PAGING_ROW_DOC              Numero di documenti visualizzati nelle ricerche              
--                FE_PAGING_ROW_DOCINFASC              Numero di documenti in fascicoli visualizzati nelle ricerche         
--                FE_PAGING_ROW_PROJECT     Numero di fascicoli visualizzati nelle ricerche                     
--                FE_IS_PRESENT_NOTE Imposta un valore Si/No se presente o no una nota nelle ricerche e export documenti e fascicoli
--FE_PAGING_ROW_TRASM Imposta il numero di risultati della griglia di ricerca trasmissioni


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_FASC_TUTTI_TIT'))
BEGIN	
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
     VALUES (0
           ,'BE_FASC_TUTTI_TIT'
           ,'Se ad 1 permette la creazione di fascicoli su tutti i titolari anche quelli non attivi'
           ,'0'
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_CHECK_TITOLARIO_ATTIVO'))
BEGIN	
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
     VALUES (0
           ,'FE_CHECK_TITOLARIO_ATTIVO'
           ,'Chiave per l''attivazione di default del  titolario attivo nelle maschere di ricerca fascicoli'
           ,'0'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_MULTI_STAMPA_ETICHETTA'))
BEGIN	
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
     VALUES (0
           ,'FE_MULTI_STAMPA_ETICHETTA'
           ,'Chiave di attivazione della stampa multipla delle etichette nei protocolli in entrata/uscita'
           ,'0'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_PAGING_ROW_DOC'))
BEGIN	
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
     VALUES (0
           ,'FE_PAGING_ROW_DOC'
           ,'Numero di documenti visualizzati nelle ricerche'
           ,'15'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_PAGING_ROW_DOCINFASC'))
BEGIN	
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
     VALUES (0
           ,'FE_PAGING_ROW_DOCINFASC'
           ,'Numero di documenti in fascicoli visualizzati nelle ricerche'
           ,'10'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_PAGING_ROW_PROJECT'))
BEGIN	
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
     VALUES (0
           ,'FE_PAGING_ROW_PROJECT'
           ,'Numero di fascicoli visualizzati nelle ricerche'
           ,'10'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_IS_PRESENT_NOTE'))
BEGIN	
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
     VALUES (0
           ,'FE_IS_PRESENT_NOTE'
           ,'Imposta un valore Si/No se presente o no una nota nelle ricerche e export documenti e fascicoli'
           ,'0'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='FE_PAGING_ROW_TRASM'))
BEGIN	
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
     VALUES (0
           ,'FE_PAGING_ROW_TRASM'
           ,'Numero di fascicoli visualizzati nelle ricerche '
           ,'10'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,NULL)

END
GO 

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC'))
BEGIN	
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
     VALUES (0
           ,'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
           ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL (valore 1) ANZICHE SOLO MAIL (valore 0)'
           ,'0' -- SEGNALAZIONE luned 19/03/2012 12:50 di Palumbo Ch. 
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO 


if (exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC'))
BEGIN	
update [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
-- SEGNALAZIONE luned 19/03/2012 12:50 di Palumbo Ch. 
      set VAR_DESCRIZIONE = 'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL (valore 1) ANZICHE SOLO MAIL (valore 0)'
      where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC'

END
GO               
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- classcat.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[classcat]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[classcat] 
go


CREATE function [@db_user].[classcat] (@docId int)
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)

set @outcome=''

declare cur CURSOR LOCAL
for SELECT DISTINCT A.VAR_CODICE
FROM PROJECT A WITH (NOLOCK)
WHERE A.CHA_TIPO_PROJ = 'F'
AND A.SYSTEM_ID IN
(SELECT A1.ID_FASCICOLO
FROM PROJECT A1 WITH (NOLOCK), PROJECT_COMPONENTS B WITH (NOLOCK)
WHERE A1.SYSTEM_ID=B.PROJECT_ID AND B.LINK=@docId)
open cur
fetch next from cur into @item
while(@@fetch_status=0)
begin
set @outcome=@outcome+@db_user.parsenull(@item)+', '
fetch next from cur into @item
end
close cur
deallocate cur
if (len(@outcome)>0)
begin
set @outcome = substring(@outcome,1,(len(@outcome)-1))
end 

if (len(@outcome)=0)
begin
set @outcome = null
end

return @outcome
end
GO


              
----------- FINE -
              
---- corrcatbytipo.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[corrcatbytipo]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[corrcatbytipo] 
go


CREATE  function [@db_user].[corrcatbytipo] (@docId int, @dirProt varchar(1), @tipocorr varchar(1))
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(2)

set @outcome=''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
from dpa_corr_globali c WITH (NOLOCK), dpa_doc_arrivo_par dap WITH (NOLOCK)
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr
while(@@fetch_status=0)
begin
if(@tipocorr='M')
begin
if (@dirProt='P' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='MD')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MM); '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end
end

if(@tipocorr='D')
begin
if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='P' and @dirCorr='L')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_LISTA(@docId)
end

if (@dirProt='P' and @dirCorr='F')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_RF(@docId)
end
end

fetch next from cur into @item,@dirCorr

end

close cur

deallocate cur

if (len(@outcome)>0)
begin
set @outcome = substring(@outcome,1,(len(@outcome)-1))
end 

if (len(@outcome)=0)
begin
set @outcome = null
end

return @outcome

end
GO



              
----------- FINE -
              
---- corrcat.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[corrcat]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[corrcat] 
go


CREATE function [@db_user].[corrcat] (@docId int, @dirProt varchar(1))
returns varchar(8000)
as
begin 
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(2)

set @outcome =''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
from dpa_corr_globali c WITH (NOLOCK), dpa_doc_arrivo_par dap WITH (NOLOCK)
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr
while(@@fetch_status=0)
begin
	if (@dirProt='P' and @dirCorr='M')
	begin
		if(@item is not null)
		set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
	end

if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='MD')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MM); '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='P' and @dirCorr='L')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_LISTA(@docId)
end

if (@dirProt='P' and @dirCorr='F')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_RF(@docId)
end

fetch next from cur into @item,@dirCorr

end

close cur

deallocate cur

if (len(@outcome)>0)
begin
set @outcome = substring(@outcome,1,(len(@outcome)-1))
end 

if (len(@outcome)=0)
begin
set @outcome = null
end

return @outcome
end 
GO

              
----------- FINE -
              
---- esisteNotaVisibile.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[esisteNotaVisibile]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[esisteNotaVisibile] 
go

CREATE function @db_user.esisteNotaVisibile
(@p_TIPOOGGETTOASSOCIATO VARCHAR(4000), 
      @p_IDOGGETTOASSOCIATO INT,
      @p_ID_RUOLO_IN_UO INT,
      @p_IDUTENTECREATORE INT,
      @p_IDRUOLOCREATORE INT) 
RETURNS VARCHAR(4000) AS
BEGIN 

   DECLARE @ultimanota VARCHAR(2000)
   DECLARE @error INT
   DECLARE @no_data_err INT

   IF (@p_TIPOOGGETTOASSOCIATO  <> 'F' AND @p_TIPOOGGETTOASSOCIATO  <> 'D')
   begin
      SET @ultimanota = '-1'
      RETURN @ultimanota 
   end
          


   IF @p_TIPOOGGETTOASSOCIATO = 'F'
   begin /*+ FIRST_ROWS(1) */ 
-- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
-- N.SYSTEM_ID,
      SELECT  TOP 1 @ultimanota = TESTO
      FROM
      DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROJECT PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      LEFT JOIN DPA_CORR_GLOBALI DP ON N.IDRUOLOCREATORE = DP.ID_GRUPPO
      WHERE   
      N.TIPOOGGETTOASSOCIATO = @p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO   = @p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  DP.ID_GRUPPO = @p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @p_IDRUOLOCREATORE)
      )
      ORDER BY N.DATACREAZIONE DESC
      SELECT   @error = @@ERROR , 
                        @no_data_err = @@ROWCOUNT
   end


   IF @p_TIPOOGGETTOASSOCIATO = 'D'  --join con la profile invece della project
   begin /*+ FIRST_ROWS(1) */ 
-- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
-- N.SYSTEM_ID,
      SELECT  TOP 1 @ultimanota = TESTO
      FROM
      DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN profile PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      LEFT JOIN DPA_CORR_GLOBALI DP ON N.IDRUOLOCREATORE = DP.ID_GRUPPO
      WHERE   
      N.TIPOOGGETTOASSOCIATO = @p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = @p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  DP.ID_GRUPPO = @p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @p_IDRUOLOCREATORE)
      )
      ORDER BY N.DATACREAZIONE DESC
      SELECT   @error = @@ERROR , 
                        @no_data_err = @@ROWCOUNT
   end


   if (@ultimanota is not null)
      SET @ultimanota = 'Si'
else
   SET @ultimanota = 'No'


   return @ultimanota


END
GO

              
----------- FINE -
              
---- getchaimg.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getchaimg]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getchaimg] 
go

CREATE FUNCTION @db_user.getchaimg(@docnum INT)

RETURNS VARCHAR(4000)
AS
BEGIN

   DECLARE @tmpVar VARCHAR(30)
   DECLARE @error INT
   begin
      DECLARE @v_path VARCHAR(500)
      DECLARE @vMaxIdGenerica INT
      begin
         SELECT   @vMaxIdGenerica = MAX(v1.version_id)
         FROM @db_user.VERSIONS v1, @db_user.components c
         WHERE v1.docnumber = @docnum
         AND v1.version_id = c.version_id
         SELECT   @error = @@ERROR
         IF (@error <> 0)
            SET @vMaxIdGenerica = 0
      end
      begin
         select   @v_path = ext 
         from @db_user.components 
         where docnumber = @docnum 
         and version_id = @vMaxIdGenerica
         SELECT   @error = @@ERROR
         IF (@error <> 0)
            SET @tmpVar = '0'
      end
      if(@v_path <> '' OR @v_path is  not null) 
         SET @tmpVar = rtrim(ltrim(@v_path))
   else 
      SET @tmpVar = '0'

   end
   RETURN @tmpVar
END 
GO




              
----------- FINE -
              
---- getcodruolobyidcorr.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getcodruolobyidcorr]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getcodruolobyidcorr] 
go


CREATE function [@db_user].[getcodruolobyidcorr] (@corrId int)
returns varchar(256)
as
begin

declare @varDescCorr varchar(256)

select @varDescCorr = VAR_COD_RUBRICA
from dpa_corr_globali
where system_id=@corrId;

return @varDescCorr

end
GO

              
----------- FINE -
              
---- getContatoreDoc.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getContatoreDoc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getContatoreDoc] 
go


CREATE FUNCTION [@db_user].[getContatoreDoc](@docNumber INT, @tipoContatore CHAR(2000))

RETURNS VARCHAR(4000) AS
BEGIN



   DECLARE @risultato VARCHAR(255)



   DECLARE @valoreContatore VARCHAR(255)

   DECLARE @annoContatore VARCHAR(255)

   DECLARE @codiceRegRf VARCHAR(255)



   SET @valoreContatore = ''

   SET @annoContatore = ''

   SET @codiceRegRf = ''



   select   @valoreContatore = valore_oggetto_db, @annoContatore = anno
   from
   dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
   where
   dpa_associazione_templates.doc_number = STR(@docNumber)
   and
   dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
   and
   dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
   and
   dpa_tipo_oggetto.descrizione = 'Contatore'
   and
   dpa_oggetti_custom.cha_tipo_tar = @tipoContatore



   IF(@tipoContatore <> 'T')
   BEGIN
      select   @codiceRegRf = dpa_el_registri.var_codice
      from
      dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
      where
      dpa_associazione_templates.doc_number = STR(@docNumber)
      and
      dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
      and
      dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
      and
      dpa_tipo_oggetto.descrizione = 'Contatore'
      and
      dpa_oggetti_custom.cha_tipo_tar = @tipoContatore
      and
      dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id
   END




   SET @risultato = ISNULL(convert(varchar(255),@codiceRegRf),'') +'-'+ ISNULL(convert(varchar(255),@annoContatore),'') +'-'+ ISNULL(convert(varchar(255),@valoreContatore),'')



   RETURN @risultato

END 


GO




              
----------- FINE -
              
---- getContatoreDocOrdinamento.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getContatoreDocOrdinamento]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getContatoreDocOrdinamento] 
go


CREATE FUNCTION [@db_user].[getContatoreDocOrdinamento](@docNumber INT, @tipoContatore CHAR(2000))

RETURNS INT AS
BEGIN



   DECLARE @risultato VARCHAR(255)



   select   @risultato = valore_oggetto_db
   from
   @db_user.dpa_associazione_templates, @db_user.dpa_oggetti_custom, @db_user.dpa_tipo_oggetto
   where
   @db_user.dpa_associazione_templates.doc_number = STR(@docNumber)
   and
   @db_user.dpa_associazione_templates.id_oggetto = @db_user.dpa_oggetti_custom.system_id
   and
   @db_user.dpa_oggetti_custom.id_tipo_oggetto = @db_user.dpa_tipo_oggetto.system_id
   and
   @db_user.dpa_tipo_oggetto.descrizione = 'Contatore'
   and
   @db_user.dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA = '1'

   RETURN CAST(@risultato as INT)

END 
GO

              
----------- FINE -
              
---- getContatoreFascContatore.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getContatoreFascContatore]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getContatoreFascContatore] 
go

CREATE FUNCTION [@db_user].[getContatoreFascContatore](@systemId INT, @tipoContatore CHAR(2000))

RETURNS INT AS
BEGIN



   DECLARE @risultato VARCHAR(255)



   select   @risultato = @db_user.dpa_ass_templates_fasc.valore_oggetto_db
   from
   @db_user.dpa_ass_templates_fasc, @db_user.dpa_oggetti_custom_fasc, @db_user.dpa_tipo_oggetto_fasc
   where
   @db_user.dpa_ass_templates_fasc.id_project = STR(@systemId)
   and
   @db_user.dpa_ass_templates_fasc.id_oggetto = @db_user.dpa_oggetti_custom_fasc.system_id
   and
   @db_user.dpa_oggetti_custom_fasc.id_tipo_oggetto = @db_user.dpa_tipo_oggetto_fasc.system_id
   and
   @db_user.dpa_tipo_oggetto_fasc.DESCRIZIONE = 'Contatore'
   and
   @db_user.dpa_oggetti_custom_fasc.DA_VISUALIZZARE_RICERCA = '1'

   RETURN CAST(@risultato as INT)

END 
GO


              
----------- FINE -
              
---- getContatoreFasc.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getContatoreFasc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getContatoreFasc] 
go


CREATE FUNCTION [@db_user].[getContatoreFasc](@systemId INT, @tipoContatore CHAR(2000))

RETURNS VARCHAR(4000) AS
BEGIN



   DECLARE @risultato VARCHAR(255)



   DECLARE @valoreContatore VARCHAR(255)

   DECLARE @annoContatore VARCHAR(255)

   DECLARE @codiceRegRf VARCHAR(255)



   select   @valoreContatore = valore_oggetto_db, @annoContatore = anno
   from
   @db_user.dpa_ass_templates_fasc, @db_user.dpa_oggetti_custom_fasc, @db_user.dpa_tipo_oggetto_fasc
   where
   @db_user.dpa_ass_templates_fasc.id_project = STR(@systemId)
   and
   @db_user.dpa_ass_templates_fasc.id_oggetto = @db_user.dpa_oggetti_custom_fasc.system_id
   and
   @db_user.dpa_oggetti_custom_fasc.id_tipo_oggetto = @db_user.dpa_tipo_oggetto_fasc.system_id
   and
   @db_user.dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
   and
   @db_user.dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore


   if (@tipoContatore <> 'T')
   begin
      select   @codiceRegRf = @db_user.dpa_el_registri.var_codice
      from
      @db_user.dpa_ass_templates_fasc, @db_user.dpa_oggetti_custom_fasc, @db_user.dpa_tipo_oggetto_fasc, @db_user.dpa_el_registri
      where
      @db_user.dpa_ass_templates_fasc.id_project = STR(@systemId)
      and
      @db_user.dpa_ass_templates_fasc.id_oggetto = @db_user.dpa_oggetti_custom_fasc.system_id
      and
      @db_user.dpa_oggetti_custom_fasc.id_tipo_oggetto = @db_user.dpa_tipo_oggetto_fasc.system_id
      and
      @db_user.dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
      and
      @db_user.dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore
      and
      @db_user.dpa_ass_templates_fasc.id_aoo_rf = @db_user.dpa_el_registri.system_id
      SET @risultato = ISNULL(convert(varchar(255),@codiceRegRf),'') +'
      -'+ ISNULL(convert(varchar(255),@annoContatore),'') +'
      -'+ ISNULL(convert(varchar(255),@valoreContatore),'')
   END
else
   SET @risultato = ISNULL(convert(varchar(255),@valoreContatore),'')

   RETURN @risultato

END 
GO



              
----------- FINE -
              
---- getdataarrivodoc.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getdataarrivodoc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getdataarrivodoc] 
go


CREATE function [@db_user].[getdataarrivodoc] (@p_docnumber int)
returns varchar(256)
as
begin

declare @varDataArrivo varchar(256)

select TOP 1 @varDataArrivo = dta_arrivo
from versions
where docnumber = @p_docnumber
ORDER BY version_id DESC;

return convert(varchar(10), convert(datetime, @varDataArrivo, 103), 101)

end
GO

              
----------- FINE -
              
---- getDescTitolario.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getDescTitolario]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getDescTitolario] 
go


CREATE FUNCTION [@db_user].[getDescTitolario](@IdTit INT)
RETURNS VARCHAR(200) as
begin
    declare @risultato varchar(2000)
	declare @varStato varchar(1);
	declare @dataInizio varchar(20);
	declare @dataFine varchar(20);
	
    DECLARE cur CURSOR LOCAL FOR
	select cha_stato, 
	convert(varchar(20),dtA_ATTIVAZIONE) as dataInizio2,
	convert(varchar(20),dta_cessazione) as dataFine2
    from project where system_id=@IdTit;
	
	set @varStato = '0'
	
    open cur
    fetch next from cur into @varStato, @dataInizio, @dataFine
	while @@fetch_status=0
    begin
		IF(@varStato != '0')
			if(@varStato = 'A')
				set @risultato = 'Titolario Attivo'
			else
				set @risultato = 'Titolario valido dal ' + @dataInizio + ' al ' + @dataFine
    
    fetch next from cur into @varStato, @dataInizio, @dataFine
    end
    CLOSE cur
              deallocate cur
      
    RETURN @risultato

end
GO


              
----------- FINE -
              
---- getdiagrammistato.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getdiagrammistato]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getdiagrammistato] 
go


CREATE function [@db_user].[getdiagrammistato] (@p_docOrId INT, @p_tipo varchar)
returns varchar(256)
as
begin

declare @varStato varchar(256)

if(@p_tipo='D')
begin
select @varStato = var_descrizione
FROM dpa_stati f, dpa_diagrammi b
    WHERE b.DOC_NUMBER = @p_docOrId
    AND f.system_id = b.ID_STATO ;
end;
if(@p_tipo='F')
begin
SELECT @varStato = var_descrizione
 FROM dpa_stati f, dpa_diagrammi b
    WHERE b.ID_PROJECT = @p_docOrId
    AND f.system_id = b.ID_STATO ;
end;


return @varStato

end
GO

              
----------- FINE -
              
---- getEsitoPubblicazione.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getEsitoPubblicazione]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getEsitoPubblicazione] 
go


CREATE function [@db_user].[getEsitoPubblicazione] (@p_systemId int)
returns varchar(256)
as
begin

declare @varEsitoPubblicazione varchar(256),
@esito varchar(256),
@errore varchar(256)

SELECT @esito=ESITO_PUBBLICAZIONE, @errore=ERRORE_PUBBLICAZIONE
     FROM PUBBLICAZIONI_DOCUMENTI
    WHERE ID_PROFILE = @p_systemId;

return @varEsitoPubblicazione

end
GO

              
----------- FINE -
              
---- getValCampoProfDoc.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getValCampoProfDoc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getValCampoProfDoc] 
go


CREATE FUNCTION [@db_user].[getValCampoProfDoc] (@DocNumber INT, @CustomObjectId INT)

RETURNS VARCHAR(4000) AS
BEGIN



/*
select b.descrizione

into tipoOggetto

from

dpa_oggetti_custom a, dpa_tipo_oggetto b

where

a.system_id = CustomObjectId

and

a.id_tipo_oggetto = b.system_id;

*/







   DECLARE @result VARCHAR(255)



   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @tipoCont VARCHAR(1)



   select   @tipoOggetto = b.descrizione, @tipoCont = cha_tipo_Tar
   from
   dpa_oggetti_custom a, dpa_tipo_oggetto b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id



--CasellaDiSelezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)

   if (@tipoOggetto = 'CasellaDiSelezione')
   BEGIN
      DECLARE @item VARCHAR(255)
      DECLARE @curCasellaDiSelezione CURSOR
      SET @curCasellaDiSelezione = 
      CURSOR  FOR select valore_oggetto_db  
      from dpa_associazione_templates 
      where id_oggetto = @CustomObjectId and doc_number = @DocNumber
      OPEN @curCasellaDiSelezione
      while 1 = 1
      begin
         FETCH @curCasellaDiSelezione INTO @item
         if @@FETCH_STATUS <> 0
         BREAK
         IF(@result IS NOT NULL)
            SET @result = @result+'; '+@item 
      ELSE
         SET @result = @result+@item
      end
      CLOSE @curCasellaDiSelezione
   END
else if (@tipoOggetto = 'Contatore')
   begin
      select   @result = @db_user.getContatoreDoc(@DocNumber,@tipoCont) 
      from dpa_associazione_templates 
      where id_oggetto = @CustomObjectId and doc_number = @DocNumber
   end
else





--Tutti gli altri

   select   @result = valore_oggetto_db from dpa_associazione_templates where id_oggetto = @CustomObjectId and doc_number = @DocNumber




   RETURN @result



END 
GO
              
----------- FINE -
              
---- getValCampoProfDocOrder.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getValCampoProfDocOrder]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getValCampoProfDocOrder] 
go


CREATE FUNCTION getValCampoProfDocOrder(@DocNumber INT, @CustomObjectId INT)
RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)

   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @error INT
   DECLARE @no_data_err INT

   select   @tipoOggetto = b.descrizione
   from
   dpa_oggetti_custom a, dpa_tipo_oggetto b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from dpa_CORR_globali cg 
         where cg.SYSTEM_ID =
         (select valore_oggetto_db 
         from dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber))
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if(@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = 
         CURSOR  FOR select  valore_oggetto_db  
         from dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = 
         STR(@DocNumber) and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else 
--Tutti gli altri
      begin
         select   @result = valore_oggetto_db 
         from dpa_associazione_templates 
         where id_oggetto = @CustomObjectId 
         and doc_number = @DocNumber
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'errore '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber 
      RETURN @result
   end
   RETURN 0

END 

GO
              
----------- FINE -
              
---- GetValProfObjPrj.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GetValProfObjPrj]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[GetValProfObjPrj] 
go


CREATE FUNCTION [@db_user].GetValProfObjPrj(@PrjId INT, @CustomObjectId INT)
RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)

   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @tipoCont VARCHAR(1)
   DECLARE @error INT
   DECLARE @no_data_err INT

   select   @tipoOggetto = b.descrizione, @tipoCont = cha_tipo_Tar
   from
   dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from dpa_CORR_globali cg where cg.SYSTEM_ID =
         (select valore_oggetto_db 
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId)
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if (@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = 
         CURSOR  FOR select valore_oggetto_db  
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = 
         STR(@PrjId) and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else if (@tipoOggetto = 'Contatore')
      begin
         select   @result = @db_user.getContatoreFasc(@PrjId,@tipoCont) 
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId
      end
   else 
--Tutti gli altri
      begin
         select   @result = valore_oggetto_db 
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'errore '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber 
      RETURN @result
   end
   RETURN 0

END 

GO

              
----------- FINE -
              
---- GetValProfObjPrjOrder.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GetValProfObjPrjOrder]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[GetValProfObjPrjOrder] 
go


CREATE FUNCTION GetValProfObjPrjOrder(@PrjId INT, @CustomObjectId INT)
RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)

   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @error INT
   DECLARE @no_data_err INT

   select   @tipoOggetto = b.descrizione
   from
   dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from dpa_CORR_globali cg where cg.SYSTEM_ID =
         (select valore_oggetto_db 
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId)
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if(@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = 
         CURSOR  FOR select valore_oggetto_db  
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = 
         STR(@PrjId) and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else 
--Tutti gli altri
      begin
         select   @result = valore_oggetto_db 
         from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'errore '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber
      RETURN @result
   end
   RETURN 0

END 

GO
              
----------- FINE -
              
---- IsValidModelloTrasmissione.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[IsValidModelloTrasmissione]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[IsValidModelloTrasmissione] 
go

CREATE FUNCTION [@db_user].[IsValidModelloTrasmissione](
    -- Id del template da analizzare
    @templateId int
) 
RETURNS int 
AS
BEGIN
      DECLARE @retVal int = 0
      
      -- Conteggio dei destinatari inibiti alla ricezione di trasmissioni
      select @retVal = count('x')
      from dpa_modelli_mitt_dest as md, dpa_corr_globali as cg
      where md.ID_MODELLO = @templateId and md.CHA_TIPO_URP = 'R'
            and md.cha_tipo_mitt_dest = 'D' 
            and md.id_corr_globali = cg.system_id 
            and cg.cha_disabled_trasm = '1'
      
      RETURN @retVal
END
GO
              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.16.1')
GO

              
