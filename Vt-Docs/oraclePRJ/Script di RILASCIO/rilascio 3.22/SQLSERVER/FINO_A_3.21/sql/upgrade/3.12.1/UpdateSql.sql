
-- creazione tabelle per modulo cache Ferro G.
if not exists(select id from sysobjects where name='DPA_CACHE' and xtype='U')
BEGIN
CREATE TABLE [DPA_CACHE](
	[DOCNUMBER] [int] NULL,
	[PATHCACHE] [varchar](500) NULL,
	[idAmministrazione] [varchar](100) NULL,
	[AGGIORNATO] [int] NULL,
	[VERSION_ID] [int] NULL,
	[LOCKED] [varchar](1) NULL,
	[COMPTYPE] [varchar](3) NULL,
	[FILE_SIZE] [int] NULL,
	[ALTERNATE_PATH] [varchar](128) NULL,
	[VAR_IMPRONTA] [varchar](64) NULL,
	[EXT] [char](7) NULL,
	[LAST_ACCESS] [varchar](50) NULL
)
END 
GO 

if not exists(select id from sysobjects where name='DPA_CONFIG_CACHE' and xtype='U')
BEGIN
CREATE TABLE [DPA_CONFIG_CACHE](
	[idAmministrazione] [varchar](100) NULL,
	[caching] [int] NULL,
	[massima_dimensione_caching] [float] NULL,
	[massima_dimensione_file] [float] NULL,
	[doc_root_server] [varchar](255) NULL,
	[ora_inizio_cache] [varchar](10) NULL,
	[ora_fine_cache] [varchar](10) NULL,
	[urlwscaching] [varchar](500) NULL,
	[url_ws_caching_locale] [varchar](500) NULL,
	[doc_root_server_locale] [varchar](255) NULL
) 
END 
GO 
-- fine creazione tabelle per modulo cache Ferro G.




-- STAMPA REPORT NO SECURITY -- F. Veltri
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='STAMPA_REG_NO_SEC')
BEGIN
            INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES ('STAMPA_REG_NO_SEC' , 'Abilita utente a stampare registri senza controllo sulla sicurezza', NULL, 'N')
END
GO



-- modifica #1, colonna COD_RUOLO_DELEGANTE
if exists (
			SELECT * FROM syscolumns 
			WHERE name='COD_RUOLO_DELEGANTE' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_DELEGHE]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_DELEGHE] ALTER COLUMN  COD_RUOLO_DELEGANTE VARCHAR(256)
END
GO

-- modifica #2, colonna COD_RUOLO_DELEGATO
if exists (
			SELECT * FROM syscolumns 
			WHERE name='COD_RUOLO_DELEGATO' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_DELEGHE]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_DELEGHE] ALTER COLUMN  COD_RUOLO_DELEGATO VARCHAR(256)
END
GO

-- modifica #3, colonna COD_PEOPLE_DELEGATO
if exists (
			SELECT * FROM syscolumns 
			WHERE name='COD_PEOPLE_DELEGATO' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_DELEGHE]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_DELEGHE] ALTER COLUMN  COD_PEOPLE_DELEGATO VARCHAR(256)
END
GO

-- modifica #4, colonna COD_PEOPLE_DELEGANTE
if exists (
			SELECT * FROM syscolumns 
			WHERE name='COD_PEOPLE_DELEGANTE' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_DELEGHE]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_DELEGHE] ALTER COLUMN  COD_PEOPLE_DELEGANTE VARCHAR(256)
END
GO

if not exists (
			SELECT * FROM syscolumns 
			WHERE name='VAR_INBOX_IMAP' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_EL_REGISTRI]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_EL_REGISTRI] ADD VAR_INBOX_IMAP VARCHAR(128)
END
GO

if exists (
			SELECT * FROM syscolumns 
			WHERE name='VAR_INBOX_IMAP' and id in 
			(SELECT id FROM sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_EL_REGISTRI]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_EL_REGISTRI] alter column VAR_INBOX_IMAP VARCHAR(128)
END
GO
 
if exists (select id from sysobjects where name='GROUPS' and xtype='U')
-- se esiste la tabella
begin
	if not exists (select id from sysobjects where name='PK_GROUPS' AND xtype='PK')
	-- se non esiste il constraint
	begin 
		if not exists (select SYSTEM_ID, COUNT(*) from [@db_user].[GROUPS] 
						group by SYSTEM_ID
						having COUNT(*) > 1)
		-- se non esistono dati duplicati
		begin
			ALTER TABLE [@db_user].GROUPS ADD CONSTRAINT
				PK_GROUPS PRIMARY KEY CLUSTERED (SYSTEM_ID)
		end
	end 
END
GO

 
 
 if exists (
		 select * from dbo.sysindexes 
		 where name = N'indx_UPPER_VAR_PROF_OGGETTO' 
		 and id = object_id(N'[@db_user].[PROFILE]')
		 )
DROP INDEX [@db_user].[PROFILE].[indx_UPPER_VAR_PROF_OGGETTO] 
GO


 if not exists (
		 select * from dbo.sysindexes 
		 where name = N'DPA_CHIAVI_CONFIG_IDX' 
		 and id = object_id(N'[@db_user].[DPA_CHIAVI_CONFIGURAZIONE]')
		 )
CREATE UNIQUE INDEX [DPA_CHIAVI_CONFIG_IDX] ON [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] (ID_AMM, VAR_CODICE)
GO

	

if not exists (SELECT * FROM syscolumns WHERE name='PATH_MOD_SU' and id in 
       (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
       ALTER TABLE [@db_user].[DPA_TIPO_ATTO]
       ADD PATH_MOD_SU VARCHAR(255)
END
GO

 
 
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_TIMESTAMP_DOC]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_TIMESTAMP_DOC](
	[SYSTEM_ID] [int] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[DOC_NUMBER] [int],
	[VERSION_ID] [int],
	[ID_PEOPLE] [int],
	[DTA_CREAZIONE] [DATETIME],
	[DTA_SCADENZA] [DATETIME],
	[NUM_SERIE] [varchar](64),
	[S_N_CERTIFICATO] [varchar](64),
	[ALG_HASH] [varchar](64),
	[SOGGETTO] [varchar](64),
	[PAESE] [varchar](64),
	[TSR_FILE] [text]
)
END
GO


if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMP_FASC')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('IMP_FASC','Abilita la voce di menu'' Import Fascicoli sotto Ricerca', '', 'N')
END
GO
 
if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMP_DOCS')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('IMP_DOCS','Abilita la voce di menu'' Import Documenti sotto Documenti', '', 'N')
END
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_ELENCO_NOTE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_ELENCO_NOTE] (
	[SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[ID_REG_RF] [int] NULL ,
	[VAR_DESC_NOTA][varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_REG_RF][varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) 
end
go

if exists (select * from dbo.sysindexes where name = N'indx_dpa_elenco_note1' and id = object_id(N'[@db_user].[DPA_ELENCO_NOTE]'))
drop index [@db_user].[DPA_ELENCO_NOTE].[indx_dpa_elenco_note1]
GO
CREATE 
  INDEX [indx_dpa_elenco_note1] ON [@db_user].[DPA_ELENCO_NOTE] ([SYSTEM_ID ])
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='ELENCO_NOTE'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('ELENCO_NOTE','Abilita il sottomenu'' Elenco note dal menu'' Gestione');
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMPORT_ELENCO_NOTE'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('IMPORT_ELENCO_NOTE','Abilita l''import di un elenco di note da foglio excel'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='RICERCA_NOTE_ELENCO'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('RICERCA_NOTE_ELENCO','Abilita la ricerca delle note da un elenco'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='INSERIMENTO_NOTERF'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('INSERIMENTO_NOTERF','Abilita l''inserimento delle note associate ad un dato rf'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_TIMESTAMP'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('DO_TIMESTAMP', 'Abilita l''utente all''utilizzo della funzione del timestamp'); 
END
GO

if not exists(select * from syscolumns where name='IDRFASSOCIATO' and id in 
(select id from sysobjects where name='DPA_NOTE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_NOTE ADD IDRFASSOCIATO INTEGER
END
GO


/*INIZIO VISIBILITA CAMPI PROFILAZIONE DINAMICA*/

--Creazione Tabella DPA_A_R_OGG_CUSTOM_DOC
IF NOT EXISTS (select * from sysobjects where name = 'DPA_A_R_OGG_CUSTOM_DOC' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_A_R_OGG_CUSTOM_DOC] (
  [SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_TEMPLATE] INT,
  [ID_OGGETTO_CUSTOM] INT,
  [ID_RUOLO] INT,
  [INS_MOD] INT,
  [VIS] INT
)
end
GO
 
if exists (select * from dbo.sysindexes where name = N'INDX_DPA_A_R_DOC1' and id = object_id(N'[@db_user].[DPA_A_R_OGG_CUSTOM_DOC]'))
drop index [@db_user].[DPA_A_R_OGG_CUSTOM_DOC].[INDX_DPA_A_R_DOC1]
GO
CREATE INDEX   [INDX_DPA_A_R_DOC1] ON  [@db_user].[DPA_A_R_OGG_CUSTOM_DOC] ([ID_TEMPLATE],[ID_OGGETTO_CUSTOM],[ID_RUOLO])       
GO
 
--Creazione Tabella DPA_A_R_OGG_CUSTOM_FASC
IF NOT EXISTS (select * from sysobjects where name = 'DPA_A_R_OGG_CUSTOM_FASC' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_A_R_OGG_CUSTOM_FASC] (
  [SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_TEMPLATE] INT,
  [ID_OGGETTO_CUSTOM] INT,
  [ID_RUOLO] INT,
  [INS_MOD] INT,
  [VIS] INT
)
end
GO
 
if exists (select * from dbo.sysindexes where name = N'INDX_DPA_A_R_FASC1' and id = object_id(N'[@db_user].[DPA_A_R_OGG_CUSTOM_FASC]'))
drop index [@db_user].[DPA_A_R_OGG_CUSTOM_FASC].[INDX_DPA_A_R_FASC1]
GO
CREATE INDEX   [INDX_DPA_A_R_FASC1] ON  [@db_user].[DPA_A_R_OGG_CUSTOM_FASC] ([ID_TEMPLATE],[ID_OGGETTO_CUSTOM],[ID_RUOLO]) 
GO
 
-- TRADOTTO DA SERPI GABRIELE 03/02/2011

--Buono Paolo 18/02/2011

--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di inserimento e ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti gli vengono assegnati pieni diritti sul campo (Inserimento,Modifica,Visibilit‡)
--DIRITTI 2(Inserimento/Ricerca) SULLA [DOCSADM].dpa_vis_tipo_doc, INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblit‡)=1


--SET ANSI_NULLS ON GO
--SET QUOTED_IDENTIFIER ON GO

 declare   @par_id_template int
 declare   @par_id_oggetto_Custom int
 declare   @par_id_ruolo int
 declare   @par_id_amm int
 declare   @cnt int

 -- primo cursore   
DECLARE c_idTemplate CURSOR LOCAL FOR
    SELECT system_id, id_amm from [@db_user].dpa_tipo_atto where id_amm is not null
  	

-- prende in input il parametro id_template che gli passa il primo cursore
DECLARE c_oggCustom CURSOR LOCAL for
        select asst.id_oggetto
        from [@db_user].dpa_associazione_templates asst, [@db_user].dpa_oggetti_custom     oc
        where asst.id_template = @par_id_template
        and asst.doc_number IS NULL
        and asst.id_oggetto = oc.system_id

-- prende in input il parametro id_template che gli passa il primo cursore
DECLARE c_idRuoli CURSOR LOCAL for
			select id_ruolo
                        from [@db_user].dpa_vis_tipo_doc where id_tipo_doc = @par_id_template and diritti = 2

BEGIN
 -- primo cursore
    OPEN c_idTemplate
        FETCH next from c_idTemplate INTO @par_id_template, @par_id_amm
        while(@@fetch_status=0)
        begin
 -- secondo cursore
        OPEN c_oggCustom
            FETCH next from c_oggCustom INTO @par_id_oggetto_Custom
            while(@@FETCH_STATUS=0)
            begin

			--Inserimento e Ricerca Tipologia
-- terzo cursore
			OPEN c_idRuoli
                FETCH NEXT from c_idRuoli INTO @par_id_ruolo
               while(@@FETCH_STATUS=0)
                begin
                    select  @cnt = count(*) from [@db_user].DPA_A_R_OGG_CUSTOM_DOC
                    where id_template = @par_id_template and id_oggetto_custom = @par_id_Oggetto_Custom
								and id_ruolo = @par_id_ruolo
                    if(@cnt = 0)
                     begin
                        --Inserimento/Modifica e Visibilit‡ sul campo
						insert into [@db_user].DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
						values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, 1, 1)
			         end

                fetch next from c_idRuoli into @par_id_ruolo
                end
                CLOSE c_idRuoli
                --deallocate c_idRuoli

            fetch next from c_oggCustom INTO @par_id_oggetto_Custom
            end
            close c_oggCustom
			--deallocate c_oggCustom

        FETCH next from c_idTemplate INTO @par_id_template, @par_id_amm
        end
        close c_idTemplate
        --deallocate c_idTemplate
end
GO

--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti gli viene assegnato il diritto di visibilit‡ sul campo
--DIRITTI 1(Ricerca) SULLA [DOCSADM].dpa_vis_tipo_doc, INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblit‡)=1

    DECLARE     @par_id_template1 int
    DECLARE     @par_id_oggetto_Custom1 int
    DECLARE     @par_id_ruolo1 int
    DECLARE     @par_id_amm1 int
    DECLARE     @cnt1 int


BEGIN

----- PRIMO CURSORE----
    DECLARE c_idTemplate1 CURSOR LOCAL FOR
    SELECT system_id, id_amm from [@db_user].dpa_tipo_atto where id_amm is not null

 ---------SECONDO CURSORE----------
    DECLARE c_oggCustom1 CURSOR LOCAL FOR
    select asst.id_oggetto
    from [@db_user].dpa_associazione_templates asst, [@db_user].dpa_oggetti_custom      oc
    where asst.id_template = @par_id_template1
    and asst.doc_number IS NULL
    and asst.id_oggetto = oc.system_id
    

-----TERZO CURSORE-----------
    DECLARE c_idRuoli1 CURSOR LOCAL FOR 
    select id_ruolo from [@db_user].dpa_vis_tipo_doc 
    where id_tipo_doc = @par_id_template1 
    and diritti = 1
    
    BEGIN  
    OPEN c_idTemplate1
        FETCH NEXT FROM c_idTemplate1 INTO @par_id_template1, @par_id_amm1
        while(@@fetch_status=0)
        BEGIN  
        
        OPEN c_oggCustom1
            FETCH NEXT FROM c_oggCustom1 INTO @par_id_oggetto_Custom1
            WHILE(@@FETCH_STATUS=0)
            BEGIN 
			
			--Ricerca tipologia
			
			OPEN c_idRuoli1
                FETCH NEXT FROM c_idRuoli1 INTO @par_id_ruolo1
                WHILE(@@FETCH_STATUS=0)
                BEGIN   
                
                    select @cnt1 = count(*)
                    from [@db_user].DPA_A_R_OGG_CUSTOM_DOC 
                    where id_template = @par_id_template1 
                    and id_oggetto_custom = @par_id_Oggetto_Custom1 
                    and id_ruolo = @par_id_Ruolo1
                    if(@cnt1 = 0)
            			BEGIN--Visibilit‡ sul campo
						insert into [@db_user].DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
						values(@par_id_template1, @par_id_Oggetto_Custom1, @par_id_Ruolo1, 0, 1)
					end
                    
               
               ----------chiude il terzo cursore--------
                FETCH NEXT FROM c_idRuoli1 INTO @par_id_ruolo1
                end
               CLOSE c_idRuoli1
              --deallocate c_idRuoli1
            
            ----------chiude il secondo cursore---------
            FETCH NEXT FROM c_oggCustom1 INTO @par_id_oggetto_Custom1
            end
            CLOSE c_oggCustom1
            --deallocate c_oggCustom1    
        
        ----------chiude il primo cursore----------
        FETCH NEXT FROM c_idTemplate1 INTO @par_id_template1, @par_id_amm1
        end
        CLOSE c_idTemplate1
        --deallocate c_idTemplate1
end



--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che non hanno diritti su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--DIRITTI 0(Nessun diritto) SULLA [DOCSADM].dpa_vis_tipo_doc, INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblit‡)=0

	DECLARE     @par_id_template2 int
    DECLARE     @par_id_oggetto_Custom2 int
    DECLARE     @par_id_ruolo2 int
    DECLARE     @par_id_amm2 int
    DECLARE     @cnt2 int



BEGIN
   
   -------------PRIMO CURSORE------------
    DECLARE c_idTemplate2 CURSOR LOCAL FOR 
    SELECT system_id, id_amm from [@db_user].dpa_tipo_atto where id_amm is not null
    
   ---------SECONDO CURSORE------------- 
    DECLARE c_oggCustom2 CURSOR LOCAL FOR
    select asst.id_oggetto 
    from [@db_user].dpa_associazione_templates asst, [@db_user].dpa_oggetti_custom oc
    where asst.id_template = @par_id_template2
    and asst.doc_number IS NULL
    and asst.id_oggetto = oc.system_id

    
    ------------TERZO CURSORE-------------
	DECLARE c_idRuoli2 CURSOR LOCAL FOR
	select id_ruolo from [@db_user].dpa_vis_tipo_doc
	where id_tipo_doc = @par_id_template2 
	and diritti = 0;

      
    BEGIN  
    OPEN c_idTemplate2
        FETCH NEXT FROM c_idTemplate2 INTO @par_id_template2, @par_id_amm2
        WHILE(@@FETCH_STATUS=0)
        BEGIN  
        
			OPEN c_oggCustom2
				FETCH NEXT FROM c_oggCustom2 INTO @par_id_oggetto_Custom2
				WHILE(@@FETCH_STATUS=0)
				BEGIN
			--Nessun diritto sulla tipologia
			
				OPEN c_idRuoli2
					FETCH NEXT FROM c_idRuoli2 INTO @par_id_ruolo2
					WHILE (@@FETCH_STATUS=0) 
					BEGIN
					
                    select @cnt2 = count(*) 
                    from [@db_user].DPA_A_R_OGG_CUSTOM_DOC 
                    where id_template = @par_id_template2 
                    and id_oggetto_custom = @par_id_Oggetto_Custom2 
                    and id_ruolo = @par_id_Ruolo2
                    
                    if(@cnt2 = 0) 
						BEGIN
            			--Nessun diritto sul campo
						insert into [@db_user].DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
						values(@par_id_template2, @par_id_Oggetto_Custom2, @par_id_Ruolo2, 0, 0);
                    end
                    
                ----------chiude il terzo cursore--------
                FETCH NEXT FROM c_idRuoli2 INTO @par_id_ruolo2
                end
               CLOSE c_idRuoli2
              --deallocate c_idRuoli2
            
            ----------chiude il secondo cursore---------
            FETCH NEXT FROM c_oggCustom2 INTO @par_id_oggetto_Custom2
            end
            CLOSE c_oggCustom2
            --deallocate c_oggCustom2    
        
        ----------chiude il primo cursore----------
        FETCH NEXT FROM c_idTemplate2 INTO @par_id_template2, @par_id_amm2
        end
        CLOSE c_idTemplate2
        --deallocate c_idTemplate2
   end


--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Vengono selezionati tutti i ruoli dell'amministrazione
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--PER TUTTI I RUOLI NON CENSITI NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_DOC, INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblit‡)=1
--Quest'ultimo script serve a dare comunque visibilit‡ sui campi, anche ai ruoli che non possono inserire o ricercare la specifica tipologia
--Questo per evitare che un documento trasmesso ad un utente senza diritti di inserimento e ricerca su una tipologia, si presenti senza campi di profilazione

	DECLARE     @par_id_template3 int
    DECLARE     @par_id_oggetto_Custom3 int
    DECLARE     @par_id_ruolo3 int
    DECLARE     @par_id_amm3 int
    DECLARE     @cnt3 int

    
    ----------PRIMO CURSORE----------
    DECLARE c_idTemplate3 CURSOR LOCAL FOR
    SELECT system_id, id_amm from [@db_user].dpa_tipo_atto where id_amm is not null
    
    ---------SECONDO CURSORE---------
    DECLARE c_oggCustom3 CURSOR LOCAL FOR
    select asst.id_oggetto
    from [@db_user].dpa_associazione_templates asst, [@db_user].dpa_oggetti_custom oc
    where asst.id_template = @par_id_template3
    and asst.doc_number IS NULL
    and asst.id_oggetto = oc.system_id

    ---------TERZO CURSORE-------
	DECLARE c_idRuoli3 CURSOR LOCAL FOR 
	select id_gruppo from [@db_user].dpa_corr_globali 
	where id_amm = @par_id_amm3 
	and id_gruppo is not null 
	and cha_tipo_urp = 'R'

      
    
    BEGIN  
    
		OPEN c_idTemplate3
        FETCH NEXT FROM c_idTemplate3 INTO @par_id_template3, @par_id_amm3
        WHILE (@@FETCH_STATUS=0)
        BEGIN
        
			OPEN c_oggCustom3
				 FETCH NEXT FROM c_oggCustom3 INTO @par_id_oggetto_Custom3
				 WHILE (@@FETCH_STATUS=0)
				 BEGIN
            
			--TUTTI I RUOLI
			
				OPEN c_idRuoli3
					FETCH NEXT FROM c_idRuoli3 INTO @par_id_ruolo3
					WHILE (@@FETCH_STATUS=0)
					BEGIN  
                
                    select @cnt3= count(*) 
                    from [@db_user].DPA_A_R_OGG_CUSTOM_DOC 
                    where id_template = @par_id_template3 
                    and id_oggetto_custom = @par_id_oggetto_Custom3 
                    and id_ruolo = @par_id_ruolo3
                    
                    if(@cnt3 = 0)
						BEGIN     --Visibilit‡ sul campo
							insert into [@db_user].DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
							values(@par_id_template3, @par_id_oggetto_Custom3, @par_id_ruolo3, 0, 1)
						end
                    
					FETCH NEXT FROM c_idRuoli3 INTO @par_id_ruolo3
					end
                CLOSE c_idRuoli3
				--deallocate c_idRuoli3 
           
           
				   FETCH NEXT FROM c_oggCustom3 INTO @par_id_oggetto_Custom3
				   end
			CLOSE c_oggCustom3
			--deallocate c_oggCustom3 
			
			          
     FETCH NEXT FROM c_idTemplate3 INTO @par_id_template3, @par_id_amm3
     end   
     CLOSE c_idTemplate3
     --deallocate c_idTemplate3    
     end
     end
end
GO


--TRADOTTO DA SERPI GABRIELE 03/03/2011


--Buono Paolo 18/02/2011

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di inserimento e ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti gli vengono assegnati pieni diritti sul campo (Inserimento,Modifica,Visibilit‡)
--DIRITTI 2(Inserimento/Ricerca) SULLA [DOCSADM].dpa_vis_tipo_fasc , INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblit‡)=1

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


DECLARE @par_id_template int
    DECLARE @par_id_oggetto_Custom int
    DECLARE @par_id_ruolo int
    DECLARE @par_id_amm int
    DECLARE @cnt int


   ----PRIMO CURSORE------------
   DECLARE c_idTemplate CURSOR LOCAL FOR
   SELECT system_id, id_amm from [@db_user].dpa_tipo_fasc where id_amm is not null

	-----SECONDO CURSORE-------
   DECLARE c_oggCustom CURSOR LOCAL FOR
   select asst.id_oggetto
   from [@db_user].dpa_ass_templates_fasc asst, [@db_user].dpa_oggetti_custom_fasc       oc
   where asst.id_template = @par_id_template
   and asst.id_project IS NULL
   and asst.id_oggetto = oc.system_id
   
   
  ------TERZO CURSORE------------------

  DECLARE c_idRuoli CURSOR LOCAL FOR 
  select id_ruolo from [@db_user].dpa_vis_tipo_fasc
  where id_tipo_fasc = @par_id_template and diritti = 2



BEGIN
    
    BEGIN OPEN c_idTemplate
        FETCH NEXT FROM c_idTemplate INTO @par_id_template, @par_id_amm
        while (@@FETCH_STATUS = 0)
        
        BEGIN  OPEN c_oggCustom
            FETCH NEXT FROM c_oggCustom INTO @par_id_oggetto_Custom
            WHILE (@@FETCH_STATUS=0)
            
            --Inserimento e Ricerca Tipologia-----
        
			BEGIN OPEN c_idRuoli
                FETCH NEXT FROM c_idRuoli INTO @par_id_ruolo
                WHILE (@@FETCH_STATUS=0)  
					BEGIN                
                    select @cnt = count(*) 
                    from [@db_user].DPA_A_R_OGG_CUSTOM_FASC 
                    where id_template = @par_id_template 
                    and id_oggetto_custom = @par_id_Oggetto_Custom 
                    and id_ruolo = @par_id_Ruolo
                    if(@cnt = 0) 
                    BEGIN   
                        --Inserimento/Modifica e Visibilit‡ sul campo
                        insert into [@db_user].DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
                        values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, 1, 1);
                    end
                    
                fetch next from c_idRuoli into @par_id_ruolo
                END
                CLOSE c_idRuoli
                --deallocate c_idRuoli
            
            FETCH NEXT FROM c_oggCustom INTO @par_id_oggetto_Custom
            end
            CLOSE c_oggCustom
            --deallocate c_oggCustom
        
        FETCH next from c_idTemplate INTO @par_id_template, @par_id_amm
        end
        close c_idTemplate
        --deallocate c_idTemplate
        END
end




--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti gli viene assegnato il diritto di visibilit‡ sul campo
--DIRITTI 1(Ricerca) SULLA [DOCSADM].dpa_vis_tipo_fasc , INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblit‡)=1



	DECLARE @par_id_template1 int
    DECLARE @par_id_oggetto_Custom1 int
    DECLARE @par_id_ruolo1 int
    DECLARE @par_id_amm1 int
    DECLARE @cnt1 int

BEGIN
      
    ----------PRIMO CURSORE-----------
    DECLARE c_idTemplate1 CURSOR LOCAL FOR
    SELECT system_id, id_amm from [@db_user].dpa_tipo_fasc where id_amm is not null
    
    -----------SECONDO CURSORE-----------
	DECLARE c_oggCustom1 CURSOR LOCAL FOR
	select asst.id_oggetto
        from [@db_user].dpa_ass_templates_fasc asst,  	[@db_user].dpa_oggetti_custom_fasc oc
        where asst.id_template = @par_id_template1
	and asst.id_project IS NULL
	and asst.id_oggetto = oc.system_id

    
    -----------TERZO CURSORE-----------------
    DECLARE c_idRuoli1 CURSOR LOCAL FOR
    select id_ruolo from [@db_user].dpa_vis_tipo_fasc  
    where id_tipo_fasc = @par_id_template1 and diritti = 1
    
    
        BEGIN  
			OPEN c_idTemplate1
			FETCH NEXT FROM c_idTemplate1 INTO @par_id_template1, @par_id_amm1
			WHILE (@@FETCH_STATUS=0)
        
			BEGIN  
				OPEN c_oggCustom1
				FETCH NEXT FROM c_oggCustom1 INTO @par_id_oggetto_Custom1
				WHILE (@@FETCH_STATUS=0)
            
            --Ricerca tipologia
            
            
				BEGIN 
					OPEN c_idRuoli1;
					FETCH NEXT FROM c_idRuoli1 INTO @par_id_ruolo1
					WHILE (@@FETCH_STATUS=0)  
					BEGIN
						select @cnt1 = count(*) from [@db_user].DPA_A_R_OGG_CUSTOM_FASC 
						where id_template = @par_id_template1 
						and id_oggetto_custom = @par_id_oggetto_Custom1 
						and id_ruolo = @par_id_ruolo1
						if(@cnt1 = 0)
                                        
                        BEGIN --Visibilit‡ sul campo
                        insert into [@db_user].DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) 
                        values(@par_id_template1, @par_id_oggetto_Custom1, @par_id_ruolo1, 0, 1)
                    END
                    
                fetch next from c_idRuoli1 into @par_id_ruolo1
                END
                CLOSE c_idRuoli1
                deallocate c_idRuoli1
            
            FETCH NEXT FROM c_oggCustom1 INTO @par_id_oggetto_Custom1
            END
            CLOSE c_oggCustom1
            --deallocate c_oggCustom
        
        FETCH next from c_idTemplate1 INTO @par_id_template1, @par_id_amm1
        end
        close c_idTemplate1
        --deallocate c_idTemplate1
        END
end



--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che non hanno diritti su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--DIRITTI 0(Nessun diritto) SULLA [DOCSADM].dpa_vis_tipo_fasc , INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblit‡)=0


	DECLARE @par_id_template2 int
    DECLARE @par_id_oggetto_Custom2 int
    DECLARE @par_id_ruolo2 int
    DECLARE @par_id_amm2 int
    DECLARE @cnt2 int


BEGIN
    
    -------PRIMO CURSORE---------
    DECLARE c_idTemplate2 CURSOR LOCAL FOR 
    SELECT system_id, id_amm from [@db_user].dpa_tipo_fasc where id_amm is not null

    -------SECONDO CURSORE----------
	DECLARE c_oggCustom2 CURSOR LOCAL FOR 
	select asst.id_oggetto
	from [@db_user].dpa_ass_templates_fasc asst, [@db_user].dpa_oggetti_custom_fasc oc
	where asst.id_template = @par_id_template2
	and asst.id_project IS NULL
	and asst.id_oggetto = oc.system_id

	-------TERZO CURSORE------------
    DECLARE c_idRuoli2 CURSOR LOCAL FOR 
    select id_ruolo from [@db_user].dpa_vis_tipo_fasc
    where id_tipo_fasc = @par_id_template2 and diritti = 0

    
    BEGIN  
		OPEN c_idTemplate2
        FETCH NEXT FROM c_idTemplate2 INTO @par_id_template2, @par_id_amm2
        WHILE (@@FETCH_STATUS=0)
        
        BEGIN  
			OPEN c_oggCustom2
            FETCH NEXT FROM c_oggCustom2 INTO @par_id_oggetto_Custom2
            WHILE (@@FETCH_STATUS=0)
            
            --Nessun diritto sulla tipologia
            
			BEGIN 
				OPEN c_idRuoli2
                FETCH NEXT FROM c_idRuoli INTO @par_id_ruolo2
                WHILE (@@FETCH_STATUS=0)
                BEGIN
                    select @cnt2 = count(*) from [@db_user].DPA_A_R_OGG_CUSTOM_FASC 
                    where id_template = @par_id_template2 
                    and id_oggetto_custom = @par_id_oggetto_Custom2 
                    and id_ruolo = @par_id_ruolo2
                    if(@cnt2 = 0)
                    
                    BEGIN    
                        --Nessun diritto sul campo
                        insert into [@db_user].DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
                        values(@par_id_template2, @par_id_oggetto_Custom2, @par_id_ruolo2, 0, 0)
                    end
                    
                fetch next from c_idRuoli2 into @par_id_ruolo2
                END
                CLOSE c_idRuoli2
                --deallocate c_idRuoli2
            
            FETCH NEXT FROM c_oggCustom2 INTO @par_id_oggetto_Custom2
            end
            CLOSE c_oggCustom2
            --deallocate c_oggCustom2
        
        FETCH next from c_idTemplate2 INTO @par_id_template2, @par_id_amm2
        end
        close c_idTemplate2
        --deallocate c_idTemplate2
        END
end

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Vengono selezionati tutti i ruoli dell'amministrazione
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede gi‡ dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--PER TUTTI I RUOLI NON CENSITI NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_FASC, INSERIMENTO NELLA [DOCSADM].DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblit‡)=1
--Quest'ultimo script serve a dare comunque visibilit‡ sui campi, anche ai ruoli che non possono inserire o ricercare la specifica tipologia
--Questo per evitare che un fascicolo trasmesso ad un utente senza diritti di inserimento e ricerca su una tipologia, si presenti senza campi di profilazione

    DECLARE @par_id_template3 int
    DECLARE @par_id_oggetto_Custom3 int
    DECLARE @par_id_ruolo3 int
    DECLARE @par_id_amm3 int
    DECLARE @cnt3 int


BEGIN
	----------PRIMO CURSORE---------
    DECLARE c_idTemplate3 CURSOR LOCAL FOR 
    SELECT system_id, id_amm from [@db_user].dpa_tipo_fasc where id_amm is not null

	----------SECONDO CURSORE-------
    DECLARE c_oggCustom3 CURSOR LOCAL FOR
    select asst.id_oggetto
    from [@db_user].dpa_ass_templates_fasc asst, [@db_user].dpa_oggetti_custom_fasc    oc
    where asst.id_template = @par_id_template3
    and asst.id_project IS NULL
    and asst.id_oggetto = oc.system_id
    
    ---------TERZO CURSORE----------
    DECLARE c_idRuoli3 CURSOR LOCAL FOR 
    select id_gruppo from [@db_user].dpa_corr_globali 
    where id_amm = @par_id_amm3 
    and id_gruppo is not null 
    and cha_tipo_urp = 'R'


    BEGIN  
		OPEN c_idTemplate3
        FETCH NEXT FROM c_idTemplate3 INTO @par_id_template3, @par_id_amm3
        WHILE (@@FETCH_STATUS=0)
        
        BEGIN  
			OPEN c_oggCustom3
            FETCH NEXT FROM c_oggCustom3 INTO @par_id_oggetto_Custom3
            WHILE (@@FETCH_STATUS=0)
            
            --TUTTI I RUOLI
			
            BEGIN 
				OPEN c_idRuoli3
                FETCH NEXT FROM c_idRuoli3 INTO @par_id_ruolo3
                WHILE (@@FETCH_STATUS=0)
					BEGIN               
                    select @cnt3 = count(*) from [@db_user].DPA_A_R_OGG_CUSTOM_FASC 
                    where id_template = @par_id_template3 
                    and id_oggetto_custom = @par_id_oggetto_Custom3 
                    and id_ruolo = @par_id_ruolo3
                    if(@cnt3 = 0)
                       BEGIN --Visibilit‡ sul campo
                        insert into [@db_user].DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
                        values(@par_id_template3, @par_id_oggetto_Custom3, @par_id_ruolo3, 0, 1)
                       END
                
                fetch next from c_idRuoli3 into @par_id_ruolo3
                END
                CLOSE c_idRuoli3
                --deallocate c_idRuoli3
            
            FETCH NEXT FROM c_oggCustom3 INTO @par_id_oggetto_Custom3
            END
            CLOSE c_oggCustom3
            --deallocate c_oggCustom3
        
        FETCH next from c_idTemplate3 INTO @par_id_template3, @par_id_amm3
        end
        close c_idTemplate3
        --deallocate c_idTemplate3
        
        END
end

/*FINE VISIBILITA CAMPI PROFILAZIONE DINAMICA*/
 
if exists (SELECT * FROM syscolumns WHERE name='IPERFASCICOLO' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	--EXEC sp_rename 'DPA_TIPO_ATTO.IPERFASCICOLO', 'IPERDOCUMENTO', 'COLUMN'	
	ALTER TABLE [@db_user].DPA_TIPO_ATTO drop COLUMN IPERFASCICOLO
	
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='IPERDOCUMENTO' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].DPA_TIPO_ATTO add IPERDOCUMENTO integer
	
END;
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[GetCountNote]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[GetCountNote] 
go
CREATE   FUNCTION [@db_user].[GetCountNote] (@tipoOggetto char(1), @idOggetto int, @note nvarchar(2000), @idUtente int, @idGruppo int, @tipoRic char(1), @idRegistro int)
RETURNS int AS
BEGIN
declare @ret int	-- variabile intera che conterr√† il risultato da restituire
if(@tipoRic = 'Q')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo) OR 
(n.tipovisibilita = 'F' AND n.idrfassociato in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=@idGruppo ))
))

else if(@tipoRic = 'T')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T')

else if(@tipoRic = 'P')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente)

else if(@tipoRic = 'R')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo)

else if (@tipoRic = 'F' and @idRegistro!=0)
SELECT  @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N 
WHERE  N.TIPOOGGETTOASSOCIATO =  @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND 
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = @idRegistro)

else if (@tipoRic = 'F' and @idRegistro=0)
SELECT  @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N 
WHERE  N.TIPOOGGETTOASSOCIATO =  @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND 
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=@idGruppo )))

RETURN  @ret
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='FILTRO_FASC_EXCEL')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('FILTRO_FASC_EXCEL','Abilita la gestione filtro su foglio excel in ricerca fascicoli', '', 'N')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='COD_MOD_TRASM' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_TIPO_ATTO] ADD COD_MOD_TRASM varchar(128);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='COD_CLASS' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_TIPO_ATTO] ADD COD_CLASS varchar(128);
END;
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_COUNT_TODOLIST_NO_REG]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_COUNT_TODOLIST_NO_REG]
GO
CREATE  PROCEDURE  [@db_user].[SP_COUNT_TODOLIST_NO_REG]
@idPeople int,
@idGroup int
AS
--tabella temporanea
CREATE TABLE [@db_user].[#COUNT_TODOLIST]
(
[TOT_DOC] [int],
[TOT_DOC_NO_LETTI] [int],
[TOT_DOC_NO_ACCETTATI] [int],
[TOT_FASC] [int],
[TOT_FASC_NO_LETTI] [int],
[TOT_FASC_NO_ACCETTATI] [int],
[TOT_DOC_PREDISPOSTI] [int],
) 

-- variabili locali
DECLARE @trasmdoctot int;
DECLARE @trasmdocnonletti int;
DECLARE @trasmdocnonaccettati int;
DECLARE @trasmfasctot int;
DECLARE @trasmfascnonletti int;
DECLARE @trasmfascnonaccettati int;
DECLARE @docpredisposti int;
BEGIN

--SETTING DELLE VARIABILI
SET @trasmdoctot = 0
SET @trasmdocnonletti = 0
SET @trasmdocnonaccettati = 0
SET @trasmfasctot = 0
SET @trasmfascnonletti = 0
SET @trasmfascnonaccettati = 0
SET @docpredisposti = 0
--END SETTING

--numero documenti presenti in todolist
SELECT @trasmdoctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero documenti non letti in todolist
SELECT @trasmdocnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
       AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti non ancora accettati in todolist
SELECT @trasmdocnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
          AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                     AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli presenti in todolist
SELECT @trasmfasctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero fascicoli non letti in todolist
SELECT @trasmfascnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli non ancora accettati in todolist
SELECT @trasmfascnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                    AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti predisposti
SELECT @docpredisposti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE  id_profile > 0
AND id_profile IN (SELECT system_id
FROM PROFILE
WHERE cha_da_proto = '1')
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #COUNT_TODOLIST
(TOT_DOC, TOT_DOC_NO_LETTI, TOT_DOC_NO_ACCETTATI, TOT_FASC, TOT_FASC_NO_LETTI,
TOT_FASC_NO_ACCETTATI, TOT_DOC_PREDISPOSTI)
values
(@trasmdoctot, @trasmdocnonletti, @trasmdocnonaccettati, @trasmfasctot, @trasmfascnonletti,
@trasmfascnonaccettati, @docpredisposti)
END
-- return dei risultati
SELECT * FROM #COUNT_TODOLIST

GO


if not exists(select * from syscolumns where name='CHA_CONTROLLATO' and id in 
(select id from sysobjects where name='PROJECT'  and xtype='U'))
BEGIN
ALTER TABLE [@db_user].project ADD CHA_CONTROLLATO VARCHAR(1) DEFAULT 0 NOT NULL
END
GO


if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='FASC_CONTROLLATO'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_LOG](var_codice, var_descrizione, var_oggetto, var_metodo)
VALUES ('FASC_CONTROLLATO', 'Modificata propriet√† controllato del fascicolo', 'FASCICOLO', 'FASCCONTROLLATO'); 
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='FASC_CONTROLLATO')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('FASC_CONTROLLATO','Permette la creazione/modifica di un fascicolo controllato', '', 'N')
END
GO

if exists (SELECT * FROM syscolumns WHERE name='GROUP_NAME' and id in 
	(SELECT id FROM sysobjects WHERE name='GROUPS' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[GROUPS] ALTER COLUMN GROUP_NAME VARCHAR(256);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_DOC_OBBL' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA] ADD
	[TIPO_DOC_OBBL] char(1) NOT NULL CONSTRAINT [DF_DPA_AMMINISTRA_TIPO_DOC_OBBL] DEFAULT 0;
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='NO_NOTIFY' and id in (SELECT id FROM sysobjects WHERE name='DPA_MODELLI_TRASM' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].[DPA_MODELLI_TRASM] ADD NO_NOTIFY varchar(1);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='COD_DESC_INTEROP' and id in (SELECT id FROM sysobjects WHERE name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[DPA_CORR_GLOBALI] ADD COD_DESC_INTEROP varchar(516);
END;
GO

if exists(select * from syscolumns where name='CHA_TIPO_MITT_DEST' and id in (select id from sysobjects where name='DPA_DOC_ARRIVO_PAR' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_DOC_ARRIVO_PAR ALTER COLUMN CHA_TIPO_MITT_DEST VARCHAR(2)
END
GO

if exists(select * from syscolumns where name='CHA_TIPO_MITT_DES' and id in (select id from sysobjects where name='DPA_CORR_STO' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_CORR_STO ALTER COLUMN CHA_TIPO_MITT_DES VARCHAR(2)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_LOCALITA' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD VAR_LOCALITA VARCHAR(128)	
END
GO


IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_NOTIFICA]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_NOTIFICA]
                        ( 
                        SYSTEM_ID               iNT, 
                        ID_TIPO_NOTIFICA        iNT, 
                        DOCNUMBER               iNT, 
                        VAR_MITTENTE            VARCHAR(255), 
                        VAR_TIPO_DESTINATARIO   VARCHAR(100), 
                        VAR_DESTINATARIO        VARCHAR(255), 
                        VAR_RISPOSTE            VARCHAR(255), 
                        VAR_OGGETTO             VARCHAR(516), 
                        VAR_GESTIONE_EMITTENTE  VARCHAR(255), 
                        VAR_ZONA                VARCHAR(10 ), 
                        VAR_GIORNO_ORA          DATETIME, 
                        VAR_IDENTIFICATIVO      VARCHAR(516), 
                        VAR_MSGID               VARCHAR(516), 
                        VAR_TIPO_RICEVUTA       VARCHAR(516), 
                        VAR_CONSEGNA            VARCHAR(516), 
                        VAR_RICEZIONE           VARCHAR(516), 
                        VAR_ERRORE_ESTESO       TEXT, 
                         VAR_ERRORE_RICEVUTA     VARCHAR(50) 
                        )
END
GO


IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DPA_TIPO_NOTIFICA]')
   AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE DPA_TIPO_NOTIFICA 
                        ( 
                        SYSTEM_ID            INT            IDENTITY(1,1)  NOT NULL PRIMARY KEY , 
                        VAR_CODICE_NOTIFICA  VARCHAR(50)        NOT NULL, 
                        VAR_DESCRIZIONE      VARCHAR(255) 
                        )
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_DOCUMENTO_DA_PEC' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[PROFILE] ADD CHA_DOCUMENTO_DA_PEC varchar(1);
END;
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[spsetdatavista_tv]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[spsetdatavista_tv]
GO
CREATE  PROCEDURE [@db_user].[spsetdatavista_tv]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@idDelegato int,
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)

BEGIN
            SET @resultValue = 0

            DECLARE cursorTrasmSingolaDocumento CURSOR FOR
            SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
            FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
            WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
            (select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
            OR b.id_corr_globale =
            (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
            AND a.ID_PROFILE = @idOggetto and
            b.ID_RAGIONE = c.SYSTEM_ID

            DECLARE  cursorTrasmSingolaFascicolo CURSOR FOR
            SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
            FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
            WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
            (select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
            OR b.id_corr_globale =
            (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
            AND a.ID_PROJECT = @idOggetto and
            b.ID_RAGIONE = c.SYSTEM_ID

            IF(@tipoOggetto='D')
            BEGIN
                        OPEN cursorTrasmSingolaDocumento
                        FETCH NEXT FROM cursorTrasmSingolaDocumento
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        WHILE @@FETCH_STATUS = 0
            
                        begin
                                   IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
                                   BEGIN
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET 
																	   --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                                           end
                                                           else
                                                           --caso in cui si sta esercitando una delega
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       dpa_trasm_utente.cha_vista_delegato = '1',
                                                                       dpa_trasm_utente.id_people_delegato = @idDelegato,
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                                           end


                                                           --update dpa_todolist
                                                           --set DTA_VISTA = getdate()
                                                           --where
                                                           --id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                                           --and id_profile = @idOggetto

                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                                           
                                                           IF (@chaTipoTrasm = 'S' AND @chaTipoDest = 'R')
                                                           BEGIN
                                                                       if (@idDelegato = 0)
                                                                       begin
                                                                                  UPDATE DPA_TRASM_UTENTE SET
                                                                                  --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                                  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                                  WHERE
                                                                                  --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                                  --AND
																				  id_trasm_singola = @sysTrasmSingola
                                                                                  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																				  AND EXISTS (
																					SELECT 'x'
																					FROM dpa_trasm_utente a
																					WHERE a.id_trasm_singola =
																					dpa_trasm_utente.id_trasm_singola
																					AND a.id_people = @idPeople)
                                                                       end
                                                                       else
                                                                       begin
                                                                                  UPDATE DPA_TRASM_UTENTE SET
                                                                                  --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                                  DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                                  DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                                                  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                                  WHERE
                                                                                  --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                                  --AND 
																				  id_trasm_singola = @sysTrasmSingola
                                                                                  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																				  AND EXISTS (
																					SELECT 'x'
																					FROM dpa_trasm_utente a
																					WHERE a.id_trasm_singola =
																					dpa_trasm_utente.id_trasm_singola
																					AND a.id_people = @idPeople)
                                                                       end
                                                                       
                                                           
                                                                       IF (@@ERROR <> 0)
                                                                       BEGIN
                                                                                  SET @resultValue=1
                                                                                  return @resultValue
                                                                       END
                                                           END
                                               end
                                   END
                        ELSE
                                   -- LA TRASMISSIONE PREVEDE WORKFLOW
                                   BEGIN
                                               if (@idDelegato = 0)
                                               begin
                                               UPDATE DPA_TRASM_UTENTE
                                               SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                               --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                               DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                               WHERE
                                               DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                               AND id_trasm_singola = @sysTrasmSingola
                                               and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end
                                               else
                                               --caso di delega
                                               begin
                                               UPDATE DPA_TRASM_UTENTE
                                               SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                               dpa_trasm_utente.cha_vista_delegato = '1',
                                               dpa_trasm_utente.id_people_delegato = @idDelegato,
                                               --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                               DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                               WHERE
                                               DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                               AND id_trasm_singola = @sysTrasmSingola
                                               and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end

                                               -- Rimozione trasmissione da todolist solo se √® stata gi√† accettata o rifiutata
                                                           UPDATE     dpa_trasm_utente
                                                           SET        cha_in_todolist = '0'
                                                           WHERE      id_trasm_singola = @sysTrasmSingola 
                                                AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                                 AND (cha_accettata = '1' OR cha_rifiutata = '1')
                                                 --AND dpa_trasm_utente.id_people = @idPeople

                                               update dpa_todolist
                                               set DTA_VISTA = GETDATE()
                                               where
                                               id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                               and id_profile = @idOggetto;
                                   
                                               IF (@@ERROR <> 0)
                                               BEGIN
                                                           SET @resultValue=1
                                                           return @resultValue
                                               END
                                   END

                        FETCH NEXT FROM cursorTrasmSingolaDocumento
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        END
                        CLOSE cursorTrasmSingolaDocumento
                        DEALLOCATE cursorTrasmSingolaDocumento
            END

            IF(@tipoOggetto='F')
                        begin
                                   OPEN cursorTrasmSingolaFascicolo
                                   FETCH NEXT FROM cursorTrasmSingolaFascicolo
                                   INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                                   WHILE @@FETCH_STATUS = 0
            
                                   BEGIN
            
                                   IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
                                   BEGIN
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                                           end
                                                           else
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                       DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                                           end
                                                           
                                                           update dpa_todolist
                                                           set DTA_VISTA = getdate()
                                                           where
                                                           id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                                           and id_project = @idOggetto;
                                   
                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                               END
            
                                               IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE SET
                                                                       --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																	   AND EXISTS (
																		SELECT 'x'
																		FROM dpa_trasm_utente a
																		WHERE a.id_trasm_singola =
																		dpa_trasm_utente.id_trasm_singola
																		AND a.id_people = @idPeople)

                                                           end
                                                           else
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE SET
                                                                       --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                       DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                                       DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       --AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																	   AND EXISTS (
																			SELECT 'x'
																			FROM dpa_trasm_utente a
																			WHERE a.id_trasm_singola =
																			dpa_trasm_utente.id_trasm_singola
																			AND a.id_people = @idPeople)

                                                           end
                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                               END
                                   END
                        ELSE
                                   -- LA TRASMISSIONE PREVEDE WORKFLOW
                                   BEGIN
                                               if (@idDelegato = 0)
                                               begin
                                                           UPDATE DPA_TRASM_UTENTE
                                                           SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                           --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                                           DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                           WHERE
                                                           DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                           AND id_trasm_singola = @sysTrasmSingola
                                                           and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end
                                               else
                                               begin
                                                           UPDATE DPA_TRASM_UTENTE
                                                           SET --DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                            DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO  = '1',
                                                           DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato
                                                           --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                                           --DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                           WHERE
                                                           --DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                           --AND 
														   id_trasm_singola = @sysTrasmSingola
                                                           and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end

                                                   -- Rimozione trasmissione da todolist solo se √® stata gi√† accettata o rifiutata
                                                   UPDATE     dpa_trasm_utente
                                                   SET        cha_in_todolist = '0'
                                                   WHERE      id_trasm_singola = @sysTrasmSingola
                                                    AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                                    AND (cha_accettata = '1' OR cha_rifiutata = '1')
                                                    AND dpa_trasm_utente.id_people = @idPeople

                                               update dpa_todolist
                                               set DTA_VISTA = getdate()
                                               where
                                               id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                               and id_project = @idOggetto
                                   
                                               IF (@@ERROR <> 0)
                                               BEGIN
                                                           SET @resultValue=1
                                                           return @resultValue
                                               END
                                   END
            
                        FETCH NEXT FROM cursorTrasmSingolaFascicolo
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        END
                        
                        CLOSE cursorTrasmSingolaFascicolo
                        DEALLOCATE cursorTrasmSingolaFascicolo
                        END
            END
RETURN @resultValue

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[spsetdatavista_v2]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[spsetdatavista_v2]
GO
CREATE  PROCEDURE [@db_user].[spsetdatavista_v2]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@idDelegato int,
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)

BEGIN
            SET @resultValue = 0

            DECLARE cursorTrasmSingolaDocumento CURSOR FOR
            SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
            FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
            WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
            (select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
            OR b.id_corr_globale =
            (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
            AND a.ID_PROFILE = @idOggetto and
            b.ID_RAGIONE = c.SYSTEM_ID

            DECLARE  cursorTrasmSingolaFascicolo CURSOR FOR
            SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
            FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
            WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
            (select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
            OR b.id_corr_globale =
            (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
            AND a.ID_PROJECT = @idOggetto and
            b.ID_RAGIONE = c.SYSTEM_ID

            IF(@tipoOggetto='D')
            BEGIN
                        OPEN cursorTrasmSingolaDocumento
                        FETCH NEXT FROM cursorTrasmSingolaDocumento
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        WHILE @@FETCH_STATUS = 0
            
                        begin
                                   IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
                                   BEGIN
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET 
																	   DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                                           end
                                                           else
                                                           --caso in cui si sta esercitando una delega
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       dpa_trasm_utente.cha_vista_delegato = '1',
                                                                       dpa_trasm_utente.id_people_delegato = @idDelegato,
                                                                       DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                                           end


                                                           update dpa_todolist
                                                           set DTA_VISTA = getdate()
                                                           where
                                                           id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                                           and id_profile = @idOggetto

                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                                           
                                                           IF (@chaTipoTrasm = 'S' AND @chaTipoDest = 'R')
                                                           BEGIN
                                                                       if (@idDelegato = 0)
                                                                       begin
                                                                                  UPDATE DPA_TRASM_UTENTE SET
                                                                                  DPA_TRASM_UTENTE.CHA_VISTA = '1'
                                                                                  --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                                  WHERE
                                                                                  DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                                  AND
																				  id_trasm_singola = @sysTrasmSingola
                                                                                  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																				  AND EXISTS (
																					SELECT 'x'
																					FROM dpa_trasm_utente a
																					WHERE a.id_trasm_singola =
																					dpa_trasm_utente.id_trasm_singola
																					AND a.id_people = @idPeople)
                                                                       end
                                                                       else
                                                                       begin
                                                                                  UPDATE DPA_TRASM_UTENTE SET
                                                                                  DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                                  DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                                  DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato
                                                                                  --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                                  WHERE
                                                                                  DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                                  AND 
																				  id_trasm_singola = @sysTrasmSingola
                                                                                  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																				  AND EXISTS (
																					SELECT 'x'
																					FROM dpa_trasm_utente a
																					WHERE a.id_trasm_singola =
																					dpa_trasm_utente.id_trasm_singola
																					AND a.id_people = @idPeople)
                                                                       end
                                                                       
                                                           
                                                                       IF (@@ERROR <> 0)
                                                                       BEGIN
                                                                                  SET @resultValue=1
                                                                                  return @resultValue
                                                                       END
                                                           END
                                               end
                                   END
                        ELSE
                                   -- LA TRASMISSIONE PREVEDE WORKFLOW
                                   BEGIN
                                               if (@idDelegato = 0)
                                               begin
                                               UPDATE DPA_TRASM_UTENTE
                                               SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                               --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                               DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                               WHERE
                                               DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                               AND id_trasm_singola = @sysTrasmSingola
                                               and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end
                                               else
                                               --caso di delega
                                               begin
                                               UPDATE DPA_TRASM_UTENTE
                                               SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                               dpa_trasm_utente.cha_vista_delegato = '1',
                                               dpa_trasm_utente.id_people_delegato = @idDelegato,
                                               --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                               DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                               WHERE
                                               DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                               AND id_trasm_singola = @sysTrasmSingola
                                               and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end

                                               -- Rimozione trasmissione da todolist solo se √® stata gi√† accettata o rifiutata
                                                           UPDATE     dpa_trasm_utente
                                                           SET        cha_in_todolist = '0'
                                                           WHERE      id_trasm_singola = @sysTrasmSingola 
                                                AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                                 AND (cha_accettata = '1' OR cha_rifiutata = '1')
                                                 --AND dpa_trasm_utente.id_people = @idPeople

                                               update dpa_todolist
                                               set DTA_VISTA = GETDATE()
                                               where
                                               id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                               and id_profile = @idOggetto;
                                   
                                               IF (@@ERROR <> 0)
                                               BEGIN
                                                           SET @resultValue=1
                                                           return @resultValue
                                               END
                                   END

                        FETCH NEXT FROM cursorTrasmSingolaDocumento
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        END
                        CLOSE cursorTrasmSingolaDocumento
                        DEALLOCATE cursorTrasmSingolaDocumento
            END

            IF(@tipoOggetto='F')
                        begin
                                   OPEN cursorTrasmSingolaFascicolo
                                   FETCH NEXT FROM cursorTrasmSingolaFascicolo
                                   INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                                   WHILE @@FETCH_STATUS = 0
            
                                   BEGIN
            
                                   IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
                                   BEGIN
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                                           end
                                                           else
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE
                                                                       SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                       DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                                       DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                                           end
                                                           
                                                           update dpa_todolist
                                                           set DTA_VISTA = getdate()
                                                           where
                                                           id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                                           and id_project = @idOggetto;
                                   
                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                               END
            
                                               IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
                                               BEGIN
                                                           if (@idDelegato = 0)
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE SET
                                                                       DPA_TRASM_UTENTE.CHA_VISTA = '1'
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																	   AND EXISTS (
																		SELECT 'x'
																		FROM dpa_trasm_utente a
																		WHERE a.id_trasm_singola =
																		dpa_trasm_utente.id_trasm_singola
																		AND a.id_people = @idPeople)

                                                           end
                                                           else
                                                           begin
                                                                       UPDATE DPA_TRASM_UTENTE SET
                                                                       DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                                       DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                                       DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato
                                                                       --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                                       WHERE
                                                                       DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                                       AND 
																	   id_trasm_singola = @sysTrasmSingola
                                                                       AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
																	   AND EXISTS (
																			SELECT 'x'
																			FROM dpa_trasm_utente a
																			WHERE a.id_trasm_singola =
																			dpa_trasm_utente.id_trasm_singola
																			AND a.id_people = @idPeople)

                                                           end
                                                           IF (@@ERROR <> 0)
                                                           BEGIN
                                                                       SET @resultValue=1
                                                                       return @resultValue
                                                           END
                                               END
                                   END
                        ELSE
                                   -- LA TRASMISSIONE PREVEDE WORKFLOW
                                   BEGIN
                                               if (@idDelegato = 0)
                                               begin
                                                           UPDATE DPA_TRASM_UTENTE
                                                           SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                           --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                                           DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                           WHERE
                                                           DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                           AND id_trasm_singola = @sysTrasmSingola
                                                           and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end
                                               else
                                               begin
                                                           UPDATE DPA_TRASM_UTENTE
                                                           SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                            DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO  = '1',
                                                           DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                           --DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
                                                           DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
                                                           WHERE
                                                           DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                                           AND 
														   id_trasm_singola = @sysTrasmSingola
                                                           and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                               end

                                                   -- Rimozione trasmissione da todolist solo se √® stata gi√† accettata o rifiutata
                                                   UPDATE     dpa_trasm_utente
                                                   SET        cha_in_todolist = '0'
                                                   WHERE      id_trasm_singola = @sysTrasmSingola
                                                    AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                                    AND (cha_accettata = '1' OR cha_rifiutata = '1')
                                                    AND dpa_trasm_utente.id_people = @idPeople

                                               update dpa_todolist
                                               set DTA_VISTA = getdate()
                                               where
                                               id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
                                               and id_project = @idOggetto
                                   
                                               IF (@@ERROR <> 0)
                                               BEGIN
                                                           SET @resultValue=1
                                                           return @resultValue
                                               END
                                   END
            
                        FETCH NEXT FROM cursorTrasmSingolaFascicolo
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                        END
                        
                        CLOSE cursorTrasmSingolaFascicolo
                        DEALLOCATE cursorTrasmSingolaFascicolo
                        END
            END
RETURN @resultValue
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_RIC_CAMPI_COMUNI')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('DO_RIC_CAMPI_COMUNI','Abilita la ricerca per campi comuni', '', 'N')
END
GO

if exists (select id from sysobjects where name='DPA_VOCI_MENU_ADMIN' and xtype='U')
begin
if not exists(select id from sysobjects where name='VAR_CODICE_UK' and xtype='UQ')
ALTER TABLE [@db_user].[DPA_VOCI_MENU_ADMIN]  ADD CONSTRAINT VAR_CODICE_UK UNIQUE(VAR_CODICE)
END
GO

if not exists(select * from @db_user.DPA_VOCI_MENU_ADMIN where VAR_CODICE='Tipi fascicolo')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN] ([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU]) VALUES ('Tipi fascicolo','Tipi fascicolo','ProfilazioneDinamicaFasc') 
END
GO

if not exists(select * from @db_user.DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione Deleghe')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN] ([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU]) VALUES ('Gestione Deleghe','Gestione Deleghe',NULL) 
END
GO

if not exists(select * from @db_user.DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione RF')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN] ([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU]) VALUES ('Gestione RF','Gestione RF',NULL) 
END
GO

if not exists(select * from @db_user.DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione News')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN] ([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU]) VALUES ('Gestione News','Gestione News',NULL) 
END
GO

if not exists(select * from @db_user.DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione Chiavi Config')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN] ([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU]) VALUES ('Gestione Chiavi Config','Gestione Chiavi Config',NULL) 
END
GO

if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in 
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_AMMINISTRA ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PEOPLE ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

ALTER  function [@db_user].[corrcat] (@docId int, @dirProt varchar(1))
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

set @outcome = substring(@outcome,1,(len(@outcome)-1))

return @outcome

end
GO

if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in 
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_AMMINISTRA ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PEOPLE ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='SMART_CLIENT_PDF_CONV_ON_SCAN' and id in
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_AMMINISTRA ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='SMART_CLIENT_PDF_CONV_ON_SCAN' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE @db_user.PEOPLE ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1) NULL
END
GO

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_SIGN')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_SIGN' , 'Abilita l''utente a compiere firme massive', NULL, 'N') 
END 
GO

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_CLASSIFICATION')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_CLASSIFICATION' , 'Abilita l''utente a compiere fascicolazioni massive', NULL, 'N') 
END 
GO

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_TRANSMISSION')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_TRANSMISSION' , 'Abilita l''utente a compiere trasmissioni massive', NULL, 'N') 
END 
GO

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_TIMESTAMP')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_TIMESTAMP' , 'Abilita l''utente a compiere applicazioni massive di timestamp', NULL, 'N') 
END 
GO

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_CONVERSION')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_CONVERSION' , 'Abilita l''utente a compiere conversioni massive', NULL, 'N') 
END 
GO

if not exists(select * from syscolumns where name='LDAP_AUTHENTICATED' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].[PEOPLE] ADD LDAP_AUTHENTICATED CHAR(1) NULL
END
GO

if (select isnullable from syscolumns where name='USERID_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN userid_attribute nvarchar(50) null	
end
go

if (select isnullable from syscolumns where name='EMAIL_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN EMAIL_ATTRIBUTE nvarchar(255) null	
end
go

if (select isnullable from syscolumns where name='MATRICOLA_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN MATRICOLA_ATTRIBUTE nvarchar(50) null	
end
go

if (select isnullable from syscolumns where name='NOME_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN NOME_ATTRIBUTE nvarchar(50) null	
end
go

if (select isnullable from syscolumns where name='COGNOME_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN COGNOME_ATTRIBUTE nvarchar(50) null	
end
go

if (select isnullable from syscolumns where name='SEDE_ATTRIBUTE' and id in 
(select id from sysobjects where name='DPA_LDAP_CONFIG' and xtype='U')) = 0
begin
	ALTER TABLE [@db_user].[DPA_LDAP_CONFIG] ALTER COLUMN SEDE_ATTRIBUTE nvarchar(255) null	
end
go

if not exists (SELECT * FROM syscolumns WHERE name='FORMATO_ORA' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM] ADD FORMATO_ORA VARCHAR(10);	
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='FORMATO_ORA' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC] ADD FORMATO_ORA VARCHAR(10);	
END;
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_MODELLI_DELEGA' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_MODELLI_DELEGA](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_PEOPLE_DELEGANTE] [int] NOT NULL,
	[ID_RUOLO_DELEGANTE] [int] NULL,
	[ID_PEOPLE_DELEGATO] [int] NOT NULL,
	[ID_RUOLO_DELEGATO] [int] NOT NULL,
	[INTERVALLO] [int] NULL,
	[DTA_INIZIO] [datetime] NULL,
	[DTA_FINE] [datetime] NULL,
	[NOME] [varchar](100) NULL,
 CONSTRAINT [PK_DPA_MODELLI_DELEGA] PRIMARY KEY CLUSTERED 
(
	[SYSTEM_ID] ASC
)
) 
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM]
	ADD TIPO_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_OBJ_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM]
	ADD TIPO_OBJ_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD TIPO_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_OBJ_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD TIPO_OBJ_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO] WHERE UPPER(DESCRIZIONE) = 'LINK')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO] (TIPO,DESCRIZIONE) VALUES ('Link','Link')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO_FASC] WHERE UPPER(DESCRIZIONE) = 'LINK')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC] (TIPO,DESCRIZIONE) VALUES ('Link','Link')
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='MASSIVE_INOLTRA')
BEGIN
       INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('MASSIVE_INOLTRA' , 'Abilita l''utente a compiere inoltri massivi', NULL, 'N')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_MANTIENI_LETTURA' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_MODELLI_TRASM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_MODELLI_TRASM] ADD
	CHA_MANTIENI_LETTURA char(1) NULL
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_MANTIENI_LETT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_RAGIONE_TRASM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_RAGIONE_TRASM] ADD
	CHA_MANTIENI_LETT char(1) NULL
END
GO


if exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_ANAGRAFICA_LOG SET VAR_CODICE = 'PUT_FILE' where VAR_CODICE = 'GET_FIRMA';
END;
GO

if exists (select * from [@db_user].[DPA_LOG] where VAR_COD_AZIONE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_LOG SET VAR_COD_AZIONE = 'PUT_FILE' where VAR_COD_AZIONE = 'GET_FIRMA';
END;
GO

if exists (select * from [@db_user].[DPA_LOG_STORICO] where VAR_COD_AZIONE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_LOG_STORICO SET VAR_COD_AZIONE = 'PUT_FILE' where VAR_COD_AZIONE = 'GET_FIRMA';
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_FASC_PRIMARIA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT_COMPONENTS' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[PROJECT_COMPONENTS] ADD CHA_FASC_PRIMARIA varchar(1);
END;
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getFascPrimaria]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[getFascPrimaria] 
go
create function [@db_user].[getFascPrimaria](@iProfile INT, @idFascicolo int)
returns varchar(1)
as
begin

declare @fascPrimaria varchar(1)
SET @fascPrimaria = '0'
set @fascPrimaria = (SELECT isnull(B.CHA_FASC_PRIMARIA,'0') FROM PROJECT A, PROJECT_COMPONENTS B
          WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=@iProfile and ID_FASCICOLO=@idFascicolo)

return @fascPrimaria
end
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_FASC_PRIMARIA')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (0,'FE_FASC_PRIMARIA','Attivazione fascicolazione primaria', '0','F','0','1','1')
END
GO


if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_ESTRAZIONE_LOG')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
   (0, 'FE_ESTRAZIONE_LOG', 'Utilizzata per abilitare estrazione dei log da amministrazione'
    ,'0','F','0','1','1')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_BLOCCA_CLASS')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (0,'FE_BLOCCA_CLASS','Blocca la classificazione sui nodi che hanno nodi figli', '0','F','0','1','0')
END
GO
 
if not exists (SELECT * FROM syscolumns WHERE name='CHA_CONSENTI_CLASS' and id in 
					(SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)
    SET @proprietario = '@db_user'

-- ALTER TABLE [@db_user].[PROJECT] ADD CHA_CONSENTI_CLASS varchar(1) default '1';
	SET @sqlstatement = N'alter table [' + @proprietario + '].[PROJECT] ADD CHA_CONSENTI_CLASS varchar(1) default ''1'' '
    execute sp_executesql @sqlstatement ;

-- per gestire il pregresso, solo su SQL server, occorre fare l'update esplicito
-- va fatto con execute perchÈ la colonna non Ë ancora definita prima del runtime!
-- update PROJECT set 	   CHA_CONSENTI_CLASS  = '1' 
	SET @sqlstatement = N'update [' + @proprietario + '].[PROJECT] set CHA_CONSENTI_CLASS  = ''1'' '
    execute sp_executesql @sqlstatement ;

END;
GO 


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[CREATE_NEW_NODO_TITOLARIO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[CREATE_NEW_NODO_TITOLARIO]
GO
CREATE  PROCEDURE  [@db_user].[CREATE_NEW_NODO_TITOLARIO]
@idAmm INT, @livelloNodo INT,
@description VARCHAR(2000),
@codiceNodo VARCHAR(64),
@idRegistroNodo INT,
@idParent INT,
@varCodLiv1 VARCHAR(32),
@mesiConservazione INT,
@idTipoFascicolo INT,
@bloccaFascicolo VARCHAR(2),
@chaRW CHAR(1),
@sysIdTitolario INT,
@noteNodo VARCHAR(2000),
@bloccaFigli VARCHAR(2),
@contatoreAttivo VARCHAR(2),
@numProtTit INT,
@consentiClassificazione VARCHAR(2),
@bloccaClass varchar(2),
@idTitolario INT OUT

AS

DECLARE @secProj INT
DECLARE @secFasc INT
DECLARE @secRoot INT

DECLARE @varChiaveTit VARCHAR(256)
DECLARE @varChiaveFasc VARCHAR(256)
DECLARE @varChiaveRoot VARCHAR(256)
DECLARE @sysCurrReg INT

BEGIN

DECLARE currReg CURSOR FOR
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = @idAmm and cha_rf = '0'
BEGIN
SET @idTitolario=0

if(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveTit= CONVERT(varchar(10), @idAmm) + '_' + @codiceNodo + '_' + CONVERT(varchar(64), @idParent ) + '_0'
else
SET @varChiaveTit= @codiceNodo + '_' +    CONVERT(varchar(64), @idParent ) + '_'  +  CONVERT(varchar(64),@idRegistroNodo)


-- INSERIMENTO RELATIVO AL NODO DI TITOLARIO

BEGIN

if (@bloccaClass = '1')
begin
update PROJECT set CHA_CONSENTI_CLASS = '0', CHA_RW = 'R' where SYSTEM_ID=@idParent
update PROJECT set CHA_CONSENTI_CLASS = '0', CHA_RW = 'R' where ID_PARENT=@idParent and CHA_TIPO_PROJ='F'
end

INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT, 
CHA_CONSENTI_CLASS
)
VALUES
(

@description,
'Y',
'T',
@codiceNodo,
@idAmm,
@idRegistroNodo,
@livelloNodo,
NULL,
@idParent,
@varCodLiv1,
GETDATE() ,
NULL,
NULL,
@chaRW,
@mesiConservazione,
@varChiaveTit,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit,
@consentiClassificazione
)
-- Reperimento identity appena immessa
SET @secProj = scope_identity()
SET @idTitolario =  @secProj

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

END


-- INSERIMENTO RELATIVO AL FASCICOLO GENERALE ASSOCIATO AL NODO DI TITOLARIO
BEGIN

IF(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveFasc= @codiceNodo + '_' +  Convert(varchar(64),@idTitolario ) + '_0'
ELSE
SET @varChiaveFasc= @codiceNodo + '_'+  Convert(varchar(64),@idTitolario ) + '_'  +  CONVERT(varchar(64), @idRegistroNodo)



INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT,
CHA_CONSENTI_CLASS
)
VALUES
(

@description,
'Y',
'F',
@codiceNodo,
@idAmm,
@idRegistroNodo,
NULL,
'G',
@idTitolario,
NULL,
GETDATE(),
'A',
NULL,
@chaRW,
@mesiConservazione,
@varChiaveFasc,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit,
@consentiClassificazione
)

SET @secFasc = scope_identity()

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END
END


BEGIN


if(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveRoot= @codiceNodo + '_' + convert( varchar(64),@secFasc ) + '_0'
else
SET @varChiaveRoot= @codiceNodo + '_'  + convert( varchar(64),@secFasc) + '_'  +  CONVERT(varchar(64), @idRegistroNodo)


INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT,
CHA_CONSENTI_CLASS
)
VALUES
(

'Root Folder',
'Y',
'C',
NULL,
@idAmm,
NULL,
NULL,
NULL,
@secFasc,
NULL,
GETDATE(),
NULL,
@secFasc,
@chaRW,
@mesiConservazione,
@varChiaveRoot,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit,
@consentiClassificazione
)
SET @secRoot = scope_identity()

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END



END

OPEN currReg
FETCH NEXT FROM currReg
INTO @sysCurrReg

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
IF(@idRegistroNodo IS NULL or @idRegistroNodo = '')
BEGIN

WHILE @@FETCH_STATUS = 0

BEGIN
INSERT INTO DPA_REG_FASC
(

id_Titolario,
num_rif,
id_registro
)
VALUES
(

@idTitolario,
1,
@sysCurrReg
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

FETCH NEXT FROM currReg INTO @sysCurrReg
END


-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
INSERT INTO dpa_reg_fasc
(
id_Titolario,
num_rif,
id_registro
)
VALUES
(
@idTitolario,
1,
NULL	-- SE IL NODO E COMUNE A TUTTI p_idRegistro = NULL
)
END
ELSE -- il nodo creato e associato a uno solo registro

BEGIN
INSERT INTO dpa_reg_fasc
(
id_Titolario,
num_rif,
id_registro
)
values
(
@idTitolario,
1,
@idRegistroNodo	-- REGISTRO SU CUI E CREATO IL NODO
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

END

END
CLOSE currReg
DEALLOCATE currReg
END;
GO

-- INIZIO BATTISTA DEL 18.10.2010

-- aggiunta del campo luogo di nascita nelle info del corrispondente
if not exists (SELECT * FROM syscolumns WHERE name='VAR_LUOGO_NASCITA' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD VAR_LUOGO_NASCITA VARCHAR(128);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_TITOLO' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD VAR_TITOLO VARCHAR(64);	
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_NASCITA' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD DTA_NASCITA VARCHAR(64);	
END;
GO


-- tabella contenente le qualifiche dei corrispondenti

IF NOT EXISTS (select * from sysobjects where name = 'DPA_QUALIFICA_CORRISPONDENTE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_QUALIFICA_CORRISPONDENTE] (
 [SYSTEM_ID] [INT] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
 [VAR_TITOLO] [VARCHAR](64) NULL,
 [DTA_FINE_VALIDITA] [DATETIME] NULL
)
end
GO

-- chiave esterna tra la tabella dei titoli 
if not exists (SELECT * FROM syscolumns WHERE name='ID_QUALIFICA_CORR' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI]  ADD ID_QUALIFICA_CORR INT
	
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD FOREIGN KEY (ID_QUALIFICA_CORR) REFERENCES [@db_user].[DPA_QUALIFICA_CORRISPONDENTE](SYSTEM_ID)
END;
GO

-- popolamento tabella titoli
if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Arch.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Arch.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Avv.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Avv.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Dott.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dott.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Dott.ssa')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dott.ssa');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Dr.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dr.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Geom.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Geom.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Ing.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Ing.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Mo.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Mo.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Mons.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Mons.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'on.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'on.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Prof.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Prof.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Prof.ssa')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Prof.ssa');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Rag.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Rag.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Rev.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Rev.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Sig.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Sig.na')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.na');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Sig.ra')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.ra');
END
GO

if not exists (SELECT * FROM [@db_user].DPA_QUALIFICA_CORRISPONDENTE WHERE VAR_TITOLO = 'Sig.ra/sig.na.')
BEGIN
	INSERT INTO [@db_user].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.ra/sig.na');
END
GO

-- FINE BATTISTA DEL 18.10.2010

IF NOT EXISTS (select * from sysobjects where name = 'DPA_DISSERVIZI' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_DISSERVIZI] (
  [SYSTEM_ID]           [int]    NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [STATO]				[varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_NOTIFICA]      [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_EMAIL_NOTIFICA][varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_PAG_CORTESIA]  [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_EMAIL_RIPRESA] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [NOTIFICATO]          [varchar] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='ACCETTAZIONE_DISSERV' and id in 
	(SELECT id FROM sysobjects WHERE name='PEOPLE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PEOPLE] ADD ACCETTAZIONE_DISSERV VARCHAR(1)
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='CONFIG_OBJ_EST' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM]
	ADD CONFIG_OBJ_EST TEXT
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CONFIG_OBJ_EST' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD CONFIG_OBJ_EST TEXT
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_OBJ_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD TIPO_OBJ_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO] WHERE UPPER(DESCRIZIONE) = 'OGGETTOESTERNO')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO] (TIPO,DESCRIZIONE) VALUES ('Oggetto esterno','OggettoEsterno')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO_FASC] WHERE UPPER(DESCRIZIONE) = 'OGGETTOESTERNO')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC] (TIPO,DESCRIZIONE) VALUES ('Oggetto esterno','OggettoEsterno')
END
GO


if exists (
          select * from dbo.sysobjects
          where id = object_id(N'[@db_user].[setsecurityRuoloReg]')
          and OBJECTPROPERTY(id, N'IsProcedure') = 1
          )
drop procedure [@db_user].[setsecurityRuoloReg]
GO
create  PROCEDURE [@db_user].[setsecurityRuoloReg]
	@idCorrGlobali int,
	@idProfile int,
	@diritto int,
	@Idreg int

	AS
	DECLARE @esito int
	DECLARE @idGruppo int

	BEGIN
	SET @esito = -1
	SET @idGruppo = (select id_Gruppo from [@db_user].dpa_corr_globali where system_id = @idCorrGlobali)

		IF (@idGruppo IS NOT NULL)
			BEGIN
				SET @esito = (select max(accessrights) from security where thing = @idProfile and personorgroup = @idGruppo)
				IF (@esito < @diritto )
					BEGIN
				update security set accessrights = @diritto where thing = @idProfile and personorgroup = @idGruppo
				END
				ELSE
					BEGIN
					if (@esito is null)
						BEGIN
						insert into security(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)  
						values(@idProfile,@idGruppo,@diritto,null,'A')
						SET  @esito = @diritto
					END
				    END
				END
		END


	insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )
	select SYSTEM_ID,@idGruppo,@diritto,null,'A' from PROFILE p where ID_REGISTRO=@Idreg
	and num_proto is not null
	and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
	s1.PERSONORGROUP=@idGruppo and s1.ACCESSRIGHTS=@diritto )

RETURN @esito
GO


if not exists (SELECT * FROM syscolumns WHERE name='VAR_CODICE_OLD_WEBCONFIG' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_CHIAVI_CONFIGURAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	ADD  VAR_CODICE_OLD_WEBCONFIG  varchar(64);
END;
GO

--aumenta la lunghezza della colonna VAR_DESCRIZIONE da varchar(256) a 
--varchar(512)

if exists(select * from syscolumns where name='VAR_DESCRIZIONE' and id in 
(select id from sysobjects where name='DPA_CHIAVI_CONFIGURAZIONE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_CHIAVI_CONFIGURAZIONE alter column VAR_DESCRIZIONE varchar(512)
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_MAX_LENGTH_NOTE')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES (N'FE_MAX_LENGTH_NOTE', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Note', N'2000', N'F', N'1', N'1', N'0')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_MAX_LENGTH_OGGETTO')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES (N'FE_MAX_LENGTH_OGGETTO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto', N'2000', N'F', N'1', N'1', N'0')
END;
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_MAX_LENGTH_DESC_FASC')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES (N'FE_MAX_LENGTH_DESC_FASC', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Fascicolo', N'2000', N'F', N'1', N'1', N'0')
end
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_MAX_LENGTH_DESC_TRASM')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES (N'FE_MAX_LENGTH_DESC_TRASM', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Trasmissione', N'2000', N'F', N'1', N'1', N'0')
end
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_MAX_LENGTH_DESC_ALLEGATO')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES (N'FE_MAX_LENGTH_DESC_ALLEGATO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Allegato', N'200', N'F', N'1', N'1', N'0')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_RICEVUTA_PROTOCOLLO_PDF')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) VALUES (N'FE_RICEVUTA_PROTOCOLLO_PDF', N'Chiave per l''impostazione del formato di stampa della ricevuta di un protocollo. Valori: 1= PDF; 0=stampa tramite activex ', N'0', N'F', N'1', N'1', N'0')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE (ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) VALUES(N'0',N'FE_ABILITA_GEST_DOCS_ST_FINALE', N'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE ', N'0',N'F', N'0', N'0', N'1')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_VOCI_MENU_ADMIN] WHERE VAR_CODICE = 'FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
	INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN]([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU])VALUES('FE_ABILITA_GEST_DOCS_ST_FINALE','GESTIONE DOCS STATO FINALE','')
END
---nuova stored per la creazione delle chiavi di configurazione relative alle amministrazioni
GO


if not exists(select * from syscolumns where name='CHA_SMTP_STA' and id in 
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_AMMINISTRA ADD CHA_SMTP_STA CHAR(1) NULL
END
GO

-- aggiunta e valorizzazione colonna 

if not exists(select * from syscolumns where name='FILE_TYPE_SIGNATURE' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_SIGNATURE INT NULL
   
declare @myid  int
declare @myfiletype int

DECLARE @stringaSQL			nchar(200)
declare c_formati cursor local for 
select system_id, file_type_used
           FROM @db_user.DPA_FORMATI_DOCUMENTO

open c_formati
fetch next from c_formati into @myid,@myfiletype
while @@fetch_status=0 
begin
set @stringaSQL = 'UPDATE @db_user.dpa_formati_documento SET FILE_TYPE_SIGNATURE = '+cast(@myfiletype as varchar) +' WHERE system_id = ' +cast(@myid as varchar)
--print @stringaSQL
fetch next from c_formati into @myid,@myfiletype
execute sp_executesql @stringaSQL
end
close c_formati 
deallocate c_formati 
end
GO


-- aggiunta e valorizzazione colonna

if not exists(select * from syscolumns where name='FILE_TYPE_PRESERVATION' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_PRESERVATION INT NULL
   
declare @myid  int
declare @myfiletype int

DECLARE @stringaSQL			nchar(200)
declare c_formati cursor local for 
select system_id, file_type_used
           FROM [@db_user].DPA_FORMATI_DOCUMENTO

open c_formati 
fetch next from c_formati into @myid,@myfiletype
while @@fetch_status=0 
begin
set @stringaSQL = 'UPDATE [@db_user].dpa_formati_documento SET FILE_TYPE_PRESERVATION = '+cast(@myfiletype as varchar) +' WHERE system_id = ' +cast(@myid as varchar)
--print @stringaSQL
fetch next from c_formati into @myid,@myfiletype
execute sp_executesql @stringaSQL
end
close c_formati 
deallocate c_formati 
end
GO

-- INIZIO SCRIPT DEL 18.10.2010 ALESSIO
if not exists (SELECT * FROM syscolumns WHERE name='CODICE_DB' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES]
	ADD CODICE_DB VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MANUAL_INSERT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES]
	ADD MANUAL_INSERT INT
END

if not exists (SELECT * FROM syscolumns WHERE name='CODICE_DB' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC]
	ADD CODICE_DB VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MANUAL_INSERT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC]
	ADD MANUAL_INSERT INT
END

-- FINE SCRPT DEL 18.10.2010 ALESSIO

-- INIZIO SCRIPT DEL 18.10.2010 RICCIUTI
if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_TIMER_DISSERVIZIO')
Begin
insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
           (ID_AMM,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE
           )
     VALUES
           (0, 'FE_TIMER_DISSERVIZIO'
           , 'Configurazione Timer DIsservizio, valore corrispondente a intervallo del Timer in millisecondi'
           ,'60000'
           ,'F','1','1','1'
           )
 end
 GO
-- FINE SCRIPT DEL 18.10.2010 RICCIUTI

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_NUOVO_FASC_DIRECT')
Begin
insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
           (ID_AMM,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE
           )
     VALUES
           (0, 'FE_NUOVO_FASC_DIRECT'
           , 'flag attivazione creazione nuovo fasc da maschera di protocollo/documento (0=disattivato; 1= attivato)'
           ,'0'
           ,'F','0','1','1'
           )
 end
 GO

-- INIZIO SCRIPT DEL 18.10.2010 VELTRI
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='EXP_FASC_COUNT')
BEGIN
            INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('EXP_FASC_COUNT' , 'Abilita l''utente ai report dei fascicoli', NULL, 'N')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_RUOLO_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
            ALTER TABLE [@db_user].[PROJECT] ADD ID_RUOLO_CHIUSURA INT;           
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_UO_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
            ALTER TABLE [@db_user].[PROJECT] ADD ID_UO_CHIUSURA INT;      
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_AUTHOR_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
            ALTER TABLE [@db_user].[PROJECT] ADD ID_AUTHOR_CHIUSURA INT;         
END;

GO
-- FINE SCRIPT DEL 18.10.2010 VELTRI

-- INIZIO SCRIPT DEL 18.10.2010 BUONO
if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO] where DESCRIZIONE = 'ContatoreSottocontatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO](TIPO, DESCRIZIONE) VALUES ('ContatoreSottocontatore', 'ContatoreSottocontatore')
END
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO_FASC] where DESCRIZIONE = 'ContatoreSottocontatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC](TIPO, DESCRIZIONE) VALUES ('ContatoreSottocontatore', 'ContatoreSottocontatore')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MODULO_SOTTOCONTATORE' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC] ADD MODULO_SOTTOCONTATORE INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='MODULO_SOTTOCONTATORE' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM] ADD MODULO_SOTTOCONTATORE INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_CONTATORI_DOC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CONTATORI_DOC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_CONTATORI_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CONTATORI_FASC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_INS' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] ADD DTA_INS DATETIME
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_INS' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC] ADD DTA_INS DATETIME
END;
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO] where DESCRIZIONE = 'Separatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO](TIPO, DESCRIZIONE) VALUES ('Separatore', 'Separatore')
END
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO_FASC] where DESCRIZIONE = 'Separatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC](TIPO, DESCRIZIONE) VALUES ('Separatore', 'Separatore')
END
GO
-- FINE SCRIPT DEL 18.10.2010 BUONO

-- INIZIO SCRIPT DEL 19.10.2010 RICCIUTI
if not exists (SELECT * FROM syscolumns WHERE name='Path_Mod_Exc' and id in 
       (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
       ALTER TABLE [@db_user].[DPA_TIPO_ATTO]
       ADD Path_Mod_Exc VARCHAR(255)
END
GO
-- FINE SCRIPT DEL 19.10.2010 RICCIUTI

-- INIZIO SCRIPT VELTRI DEL 20.10.2010
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_DETTAGLIO_TRASM_TODOLIST' ))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_DETTAGLIO_TRASM_TODOLIST', 'Utilizzata per visualizzare l intero dettaglio della trasmissione dalla todolist', '1',
    'F','1','1', '1')
END
GO
-- FINE SCRIPT VELTRI DEL 20.10.2010

-- INIZIO SCRIPT FREZZA-PALUMBO DEL 13-10-2010
if not exists(select * from syscolumns where name='FILE_TYPE_SIGNATURE' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_SIGNATURE INT NULL
END
GO

if not exists(select * from syscolumns where name='FILE_TYPE_PRESERVATION' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_PRESERVATION INT NULL
END
GO
-- fine SCRIPT FREZZA-PALUMBO DEL 13-10-2010




-- INIZIO SCRIPT RICCIUTI 20.10.2010
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_EXPORT_DA_MODELLO' ))
Begin
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           (ID_AMM
           ,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE
           ,CHA_VISIBILE
           ,CHA_MODIFICABILE
           ,CHA_GLOBALE
           )
     VALUES
           (0
           , 'FE_EXPORT_DA_MODELLO'
           , 'Abilitazione export ricerca da modello (0=disabilitato, 1= abilitato)'
           ,'0'
           ,'F'
           ,'0'
           ,'1'
           ,'1'
           )
 end
 GO
-- FINE SCRIPT RICCIUTI 20.10.2010

-- Inizio SCRIPT De Luca 20.10.2010
if not exists (select * from sysobjects where parent_obj=
object_id(N'[@db_user].[DPA_CHIAVI_CONFIGURAZIONE]') and xtype='C' )
and (select count(*) from @db_user.DPA_CHIAVI_CONFIGURAZIONE where id_amm=0 and cha_globale='0' ) = 0
begin
ALTER TABLE @db_user.DPA_CHIAVI_CONFIGURAZIONE ADD CONSTRAINT
	CK_DPA_CHIAVI_CONFIGURAZIONE CHECK (isnull(cha_globale,1) + isnull(id_amm,1) > 0)
end
GO
-- FINE SCRIPT De Luca 20.10.2010

--Dimitri de filippo 22/10/2010

if
(not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='MODIFICADOCSTATOFINALE'))
Begin

insert into [@db_user].dpa_anagrafica_log
(    var_codice ,
var_descrizione ,
var_oggetto      ,
var_metodo
)
values
( 'MODIFICADOCSTATOFINALE',
'Azione di modifica dei diritti di lettura / scrittura sul documento in stato finale',
'DOCUMENTO',
'MODIFICADOCSTATOFINALE' );

end
GO
----------Nuovo campo per la modifica dei documenti in stato finale
if
not exists(select * from syscolumns where name='CHA_UNLOCKED_FINAL_STATE' and id in
(     select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER            TABLE [@db_user].PROFILE ADD CHA_UNLOCKED_FINAL_STATE VARCHAR(1) NULL
END
GO

---------fine nuovo campo




--creazione tabella template per le chiavi di configurazione non globali

IF
NOT EXISTS (select * from sysobjects where name = 'DPA_CHIAVI_CONFIG_TEMPLATE' and type = 'U')
begin

CREATE  TABLE [@db_user].[DPA_CHIAVI_CONFIG_TEMPLATE](
[SYSTEM_ID] [int]
 IDENTITY(1,1) NOT NULL,
[VAR_CODICE] [varchar] (32) NOT NULL,
[VAR_DESCRIZIONE] [varchar] (512) NULL,
[VAR_VALORE] [varchar]       (128) NOT NULL,
[CHA_TIPO_CHIAVE] [char]     (1) NULL,
[CHA_VISIBILE] [char]        (1) NOT NULL default '1',
[CHA_MODIFICABILE] [char]    (1) NOT NULL default '1',
PRIMARY
KEY CLUSTERED
(
[SYSTEM_ID]
ASC
)

)

end
GO

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_NOTE')
BEGIN
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) 
VALUES ( 'FE_MAX_LENGTH_NOTE', N'Chiave utilizzata per impostazione del numero massimo di caratteri digitabili nei campi Note', '2000', 'F', '1', '1')
END
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_OGGETTO')
BEGIN
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES ( N'FE_MAX_LENGTH_OGGETTO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto', N'2000', N'F', N'1', N'1')
END
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_FASC')
begin
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES ( N'FE_MAX_LENGTH_DESC_FASC', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Fascicolo', N'2000', N'F', N'1', N'1')
end
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_TRASM')
begin
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES ( N'FE_MAX_LENGTH_DESC_TRASM', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Trasmissione', N'2000', N'F', N'1', N'1')
end
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_ALLEGATO')
BEGIN
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES ( N'FE_MAX_LENGTH_DESC_ALLEGATO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Allegato ', N'200', N'F', N'1', N'1')
END
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_RICEVUTA_PROTOCOLLO_PDF')
BEGIN
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES ( N'FE_RICEVUTA_PROTOCOLLO_PDF', N'Chiave per l''impostazione del formato di stampa della ricevuta di un protocollo. Valori: 1= PDF; 0=stampa tramite activex ', N'0', N'F', N'0', N'1')
END
go

IF
NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
INSERT
into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE , VAR_DESCRIZIONE , VAR_VALORE , CHA_TIPO_CHIAVE , CHA_VISIBILE , CHA_MODIFICABILE ) VALUES('FE_ABILITA_GEST_DOCS_ST_FINALE', 'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE ', '1','F', '0', '0')
END
go

IF
NOT EXISTS(SELECT * FROM [@db_user].[DPA_VOCI_MENU_ADMIN] WHERE VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
INSERT
INTO [@db_user].[DPA_VOCI_MENU_ADMIN]([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU])VALUES('FE_ABILITA_GEST_DOCS_ST_FINALE','GESTIONE DOCS STATO FINALE','')
END
go

--Stored che scorre la tabella DPA_CHIAVI_CONFIG_TEMPLATE e

--inserisce le corrispondenti chiavi non globali nella tabella DPA_CHIAVI_CONFIGURAZIONE per ciascuna amministrazione

if
exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[CREA_KEYS_AMMINISTRA]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop
procedure [@db_user].[CREA_KEYS_AMMINISTRA]
GO

CREATE PROCEDURE [@db_user].[CREA_KEYS_AMMINISTRA] AS
BEGIN

--TRY
BEGIN
TRANSACTION


DECLARE
@sysCurrAmm INT;
declare
@ErrorMessage varchar(100);
BEGIN

DECLARE

currAmm -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
CURSOR
LOCAL FOR
SELECT
system_id
FROM
[@db_user].DPA_AMMINISTRA
OPEN

currAmm
FETCH
NEXT FROM currAmm
INTO
@sysCurrAmm
WHILE
@@FETCH_STATUS = 0
BEGIN

---cursore annidato x le chiavi di configurazione

DECLARE

@sysCurrKey varchar(32);
BEGIN

DECLARE
currKey -- CURSORE CHE SCORRE LE CHIAVI CON ID AMMINISTRAZIONE A NULL
CURSOR

LOCAL FOR
SELECT
var_codice
FROM
[@db_user].DPA_CHIAVI_CONFIG_TEMPLATE

OPEN
currKey
FETCH
NEXT FROM currKey
INTO
@sysCurrKey
WHILE
@@FETCH_STATUS = 0
BEGIN


if not exists (select * from [@db_user].DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE=@sysCurrKey and ID_AMM =@sysCurrAmm)
begin
insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
(ID_AMM,
VAR_CODICE ,
VAR_DESCRIZIONE ,
VAR_VALORE       ,
CHA_TIPO_CHIAVE  ,
CHA_VISIBILE     ,
CHA_MODIFICABILE ,
CHA_GLOBALE      )
(select top 1
@sysCurrAmm
as ID_AMM,
VAR_CODICE,
VAR_DESCRIZIONE,
VAR_VALORE     ,
CHA_TIPO_CHIAVE,
CHA_VISIBILE   ,
CHA_MODIFICABILE,
'0'
from [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE=@sysCurrKey)

end
--end

FETCH
NEXT FROM currKey
INTO
@sysCurrKey
END

CLOSE

currKey
DEALLOCATE

currKey
END


--- fine cursore annidato per chiavi di configurazione

FETCH
NEXT FROM currAmm
INTO

@sysCurrAmm
END

CLOSE

currAmm
DEALLOCATE

currAmm
END

COMMIT TRAN
END

--TRY
--BEGIN CATCH
--IF         @@TRANCOUNT > 0

--ROLLBACK TRAN

-- END CATCH
--SET  QUOTED_IDENTIFIER OFF
----fine nuova stored

GO


--fine script Dimitri de filippo 22/10/2010


-- chiavi configurazione by Alessio Di Bartolo
if not exists(select system_id from [@db_user].DPA_CHIAVI_CONFIGURAZIONE
 where VAR_CODICE='FE_MODELLI_DELEGA')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
	   (ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
		CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE, VAR_CODICE_OLD_WEBCONFIG)
	 Values
	   ( 0, 'FE_MODELLI_DELEGA', 'Abilitazione dei modelli di delega', '0', 
		'F', '0', '1', '1', NULL);
end
GO

if not exists(select system_id from [@db_user].DPA_CHIAVI_CONFIGURAZIONE
 where VAR_CODICE='FE_STAMPA_UNIONE')
BEGIN
insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
   (ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, 
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE, VAR_CODICE_OLD_WEBCONFIG)
 Values
   ( 0, 'FE_STAMPA_UNIONE', 'Chiave utilizzata per la gestione della stampa unione', '0', 
    'F', '0', '1', '1', NULL);
end
GO


-- fine aggiunta chiave
-- script Ferro 23-10-2010
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_URL_WA' ))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'BE_URL_WA', 'Utilizzata per identificare il path del Front end per esportare il link tramite web service ', '1',
    'B','1','1', '1')
END
GO
-- fine script Ferro 23-10-2010

-- script Veltri 23-10-2010
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_TAB_TRASM_ALL' ))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_TAB_TRASM_ALL', 'Utilizzata per vedere dal tab classifica tutti i fascicoli', '0',
    'F','0','1', '1')
END
GO
-- fine script Veltri 23-10-2010
 
-- script Frezza  23-10-2010
if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in 
(select id from sysobjects where name='DPA_TRASM_SINGOLA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_TRASM_SINGOLA ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[isVersionVisible]') AND xtype in (N'FN', N'IF', N'TF'))
	DROP  function [isVersionVisible] 
GO

CREATE FUNCTION [isVersionVisible]
(
@versionId INT,
@idpeople INT,
@idgroup INT
)
RETURNS INT
AS

BEGIN
	declare @retValue int
	set @retValue = 0
	
	declare @idProfile int
	set @idProfile = 0

	declare @maxVersionId int
	set @maxVersionId = 0

	declare @hasFullVisibility int
	set @hasFullVisibility = 0

	-- 1) Reperimento IdProfile e DocNumber del documento
	select  @idProfile = p.system_id 
	from    versions v 
	        inner join profile p on v.docnumber = p.docnumber
	where   v.version_id = @versionId

	-- 2) verifica se la versione richiesta del documento Ë l'ultima
	set @maxVersionId = (select max(v.version_id)
		from versions v
		where v.docnumber = (select docnumber from profile where system_id = @idProfile))


	if (@maxVersionId = @versionId) 
		begin
		    -- 2a.) Il versionId si riferisce all'ultima versione del documento, Ë sempre visibile
		    set @retValue = 1
		end
	else
		begin
		    -- 3) Determina se sul documento l'utente dispone dei diritti di visibilit‡ completa su tutte le versioni
		    set @hasFullVisibility = (select @db_user.hasVersionsFullVisibility(@idProfile, @idPeople, @idGroup))

		     if (@hasFullVisibility > 0) 
			begin
			        --  3a) Sul documento si dispongono gi‡ dei diritti di visibilit‡ completa sul documento
			        -- pertanto la versione non deve essere nascosta        
			        set @retValue = 1
			end
		    else
			begin
			        -- 3b) Sul documento non si dispongono dei diritti di visibilit‡ completa sul documento,
			        -- pertanto la versione deve essere nascosta 
				set @retValue = 0
			end
		end
	    

RETURN @retValue

END
GO

-- fine script Frezza 23-10-2010

-- script ALessio Di Bartolo 25-10-2010
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_AMM_CONSERVAZIONE_WA'))
BEGIN
       INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
       VALUES('DO_AMM_CONSERVAZIONE_WA', 'Permette l''amministrazione della WA di Conservazione', '', 'N');
END 
GO
-- fine script ALessio Di Bartolo 25-10-2010

-- script consolidamento Frezza 
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_CONSOLIDAMENTO' ))
BEGIN
    insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
    (
    id_amm,
    var_codice,
    var_descrizione,
    var_valore,
    cha_tipo_chiave,
    cha_visibile,
    cha_modificabile,
    cha_globale,
    var_codice_old_webconfig
    )
    values
    (
    0,
    'BE_CONSOLIDAMENTO',
    'Abilitazione della funzione di consolidamento dello stato di un documento',
    '0',
    'B',
    '0',
    '1',
    '1',
    null
    );
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='CONSOLIDADOCUMENTO'))
BEGIN
	insert into [@db_user].dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values
	(
	'CONSOLIDADOCUMENTO',
	'Azione di consolidamento dello stato di un documento',
	'DOCUMENTO',
	'CONSOLIDADOCUMENTO'
	);
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_CONSOLIDAMENTO'))
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI
	VALUES('DO_CONSOLIDAMENTO', 'Abilita il consolidamento di un documento', '', 'N');
END
GO


if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='MASSIVE_REMOVE_VERSIONS'))
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI
	VALUES ('MASSIVE_REMOVE_VERSIONS', 'Abilita la rimozione massiva delle versioni dei doc grigi', null,'N');
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_CONSOLIDAMENTO_METADATI'))
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI
	VALUES('DO_CONSOLIDAMENTO_METADATI', 'Abilita il consolidamento dei metadati di un documento', '', 'N');
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_STATE' and id in
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_STATE CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_AUTHOR' and id in
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_AUTHOR INT NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_ROLE' and id in
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_ROLE INT NULL
END
GO


if not exists(select * from syscolumns where name='CONSOLIDATION_DATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_DATE DATETIME NULL
END
GO

if not exists(select * from syscolumns where name='STATO_CONSOLIDAMENTO' and id in 
(select id from sysobjects where name='DPA_STATI' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL
END
GO

if not exists(select id from sysobjects where name='DPA_CONSOLIDATED_DOCS' and xtype='U')
BEGIN
	CREATE TABLE DPA_CONSOLIDATED_DOCS
	(
	ID                       int,
	DOCNAME                  nvarchar(240),
	CREATION_DATE            datetime,
	DOCUMENTTYPE             int,
	AUTHOR                   int,
	AUTHOR_NAME              nvarchar(4000),
	ID_RUOLO_CREATORE        int,
	RUOLO_CREATORE           nvarchar(4000),
	NUM_PROTO                int,
	NUM_ANNO_PROTO           int,
	DTA_PROTO                datetime,
	ID_PEOPLE_PROT           int,
	PEOPLE_PROT              nvarchar(4000),
	ID_RUOLO_PROT            int,
	RUOLO_PROT               nvarchar(4000),
	ID_REGISTRO              int,
	REGISTRO                 nvarchar(4000),
	CHA_TIPO_PROTO           nvarchar(1),
	VAR_PROTO_IN             nvarchar(128),
	DTA_PROTO_IN             datetime,
	DTA_ANNULLA              datetime,
	ID_OGGETTO               int,
	VAR_PROF_OGGETTO         nvarchar(2000),
	MITT_DEST                nvarchar(4000),
	ID_DOCUMENTO_PRINCIPALE  int
	)     
END 
GO
-- fine script consolidamento Frezza 


-- chiavi di anagrafica funzioni
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='GEST_SU')
BEGIN
            INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('GEST_SU', 'Abilita il sottomenu Stampa unione del menu Gestione', NULL, 'N')
END
GO

-- fine chiavi di anagrafica funzioni

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_GESTIONE_DISSERVIZIO')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (0,'FE_GESTIONE_DISSERVIZIO','Attivazione fascicolazione primaria', '0','F','0','1','1')
END
GO

-- FE_NEW_RUBRICA_VELOCE
if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_NEW_RUBRICA_VELOCE')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (0,'FE_NEW_RUBRICA_VELOCE','Abilita Rubrica Ajax', '0','F','0','1','1')
END
GO

--BE_SALVA_EMAIL_IN_LOCALE
if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'BE_SALVA_EMAIL_IN_LOCALE')
BEGIN
	insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (0,'BE_SALVA_EMAIL_IN_LOCALE','Salva le email delle ricevute PEC come allegati al documento inviato', '0','B','0','1','1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_NUOVO_FASC_DIRECT' ))
BEGIN
INSERT INTO [DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES ('0', 'FE_NUOVO_FASC_DIRECT', 'flag attivazione creazione nuovo fasc da maschera di protocollo/documento (0=disattivato; 1= attivato)', '1',
    'F','1','1', '1')
END
GO





/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in 
(select id from sysobjects where name='DELETED_SECURITY' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DELETED_SECURITY ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in
(select id from sysobjects where name='DPA_MODELLI_MITT_DEST' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_MODELLI_MITT_DEST ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITI CAMPO "STATO_CONSOLIDAMENTO" PER LA GESTIONE DEL CONSOLIDAMENTO DEL DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
if not exists(select * from syscolumns where name='STATO_CONSOLIDAMENTO' and id in 
(select id from sysobjects where name='DPA_STATI' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL
END
GO
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in 
(select id from sysobjects where name='DPA_TRASM_SINGOLA' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].DPA_TRASM_SINGOLA ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO

/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITI CAMPI PER LA GESTIONE DEL CONSOLIDAMENTO DEL DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
if not exists(select * from syscolumns where name='CONSOLIDATION_STATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_STATE CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_AUTHOR' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_AUTHOR INT NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_ROLE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_ROLE INT NULL
END
GO


if not exists(select * from syscolumns where name='CONSOLIDATION_DATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PROFILE ADD CONSOLIDATION_DATE DATETIME NULL
END
GO
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in 
(select id from sysobjects where name='SECURITY' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].SECURITY ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITA NUOVA TABELLA "DPA_CONSOLIDATED_DOCS" IN CUI VENGONO MEMORIZZATI
							I METADATI PRINCIPALI DEI DOCUMENTI CONSOLIDATI IN STATO 2

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
if not exists(select id from sysobjects where name='DPA_CONSOLIDATED_DOCS' and xtype='U')
BEGIN
	CREATE TABLE [@db_user].DPA_CONSOLIDATED_DOCS
	(
	ID                       int,
	DOCNAME                  nvarchar(240),
	CREATION_DATE            datetime,
	DOCUMENTTYPE             int,
	AUTHOR                   int,
	AUTHOR_NAME              nvarchar(4000),
	ID_RUOLO_CREATORE        int,
	RUOLO_CREATORE           nvarchar(4000),
	NUM_PROTO                int,
	NUM_ANNO_PROTO           int,
	DTA_PROTO                datetime,
	ID_PEOPLE_PROT           int,
	PEOPLE_PROT              nvarchar(4000),
	ID_RUOLO_PROT            int,
	RUOLO_PROT               nvarchar(4000),
	ID_REGISTRO              int,
	REGISTRO                 nvarchar(4000),
	CHA_TIPO_PROTO           nvarchar(1),
	VAR_PROTO_IN             nvarchar(128),
	DTA_PROTO_IN             datetime,
	DTA_ANNULLA              datetime,
	ID_OGGETTO               int,
	VAR_PROF_OGGETTO         nvarchar(2000),
	MITT_DEST                nvarchar(4000),
	ID_DOCUMENTO_PRINCIPALE  int
	)
END
GO


-- script Furnari x  griglie custom 29-10-2010
IF NOT EXISTS (
               SELECT * FROM sysobjects
              WHERE name = 'DPA_GRIDS' and OBJECTPROPERTY(id, N'IsUserTable') = 1
              )
BEGIN
CREATE TABLE [@db_user].[DPA_GRIDS](
	[SYSTEM_ID] [int] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[USER_ID] [int],
	[ROLE_ID] [int],
	[ADMINISTRATION_ID] [int],
	[SEARCH_ID] [int],
	[SERIALIZED_GRID] [text],
    [TYPE_GRID] [VARCHAR](30)
)
END
GO

-- fine script Furnari


/*
AUTORE:			Federico Ricciuti
Data creazione:		04-11-10		
Scopo della modifica:    Creazione tabella per lo storico della data e ora arrivo della protocollazione, pi√π relative 
                            microfunzioni per l'abilitazione della modifica e dello storico delle modifiche.
*/

IF NOT EXISTS (select * from sysobjects where name = 'DPA_DATA_ARRIVO_STO' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_DATA_ARRIVO_STO](
	[SYSTEM_ID] [int] IDENTITY(1,2) NOT NULL,
	[DOCNUMBER] [int] NOT NULL,
	[DTA_ARRIVO] [datetime] NULL,
	[ID_GROUP] [int] NOT NULL,
	[ID_PEOPLE] [int] NOT NULL,
	[DTA_MODIFICA] [datetime] NULL,
 CONSTRAINT [PK_DPA_DATA_ARRIVO_STO] PRIMARY KEY CLUSTERED 
	(	[SYSTEM_ID] ASC ) 
)

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_GROUPS] 
FOREIGN KEY([ID_GROUP]) REFERENCES [@db_user].[GROUPS] ([SYSTEM_ID])

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_GROUPS]

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PEOPLE] 
FOREIGN KEY([ID_PEOPLE]) REFERENCES [@db_user].[PEOPLE] ([SYSTEM_ID])

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PEOPLE]

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PROFILE] 
FOREIGN KEY([DOCNUMBER]) REFERENCES [@db_user].[PROFILE] ([SYSTEM_ID])

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PROFILE]
END
GO
--fine creazione tabelle

--Inserimento Microfunzione per abilitare la modifica di ora pervenuto (DO_PROT_DATA_ORA_MODIFICA)
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_PROT_DATA_ORA_MODIFICA')
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI]
           ([COD_FUNZIONE]
           ,[VAR_DESC_FUNZIONE]
           ,[DISABLED])
     VALUES
           ('DO_PROT_DATA_ORA_MODIFICA'
		,'Abilito la modifica di data arrivo e ora pervenuto nella protocollazione'
           , 'N')
END
GO
--fine inserimento Microfunzione DO_PROT_DATA_ORA_MODIFICA

--inserimento Microfunzione per abilitare il tasto dello storico delle modifiche (DO_PROT_DATA_ORA_STORIA)

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_PROT_DATA_ORA_STORIA')
BEGIN
INSERT INTO [@db_user].DPA_ANAGRAFICA_FUNZIONI
           ([COD_FUNZIONE]
           ,[VAR_DESC_FUNZIONE]
           ,[DISABLED])
     VALUES
           ('DO_PROT_DATA_ORA_STORIA'
		,'Abilita il pulsante per lo storico della data arrivo e ora pervenuto nella creazione del protocollo'
            , 'N')
END
GO

---fine inserimento Microfunzione DO_PROT_DATA_ORA_STORIA
----fine script Ricciuti


-- script Gennaro Ferro

if exists (SELECT * FROM syscolumns WHERE name='VAR_PWD_MAIL' and id in 
      (SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[DPA_EL_REGISTRI] 
	  ALTER COLUMN  [VAR_PWD_MAIL] varchar(128) ;
END;
GO
 
if exists (SELECT * FROM syscolumns WHERE name='VAR_SERVER_IMAP' and id in 
      (SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[DPA_EL_REGISTRI] ALTER COLUMN 
        [VAR_SERVER_IMAP] varchar(64) ;
END;
GO

-- fine script Gennaro Ferro 


-- script Buono P.

-- CREATE INDEX DPA_ASSOCIAZIONE_TEMPL_INDX01 ON DPA_ASSOCIAZIONE_TEMPLATES(ID_OGGETTO)
if not exists (
		 select * from dbo.sysindexes
		 where name = N'DPA_ASSOCIAZIONE_TEMPL_INDX01'
		 and id = object_id(N'[@db_user].[DPA_ASSOCIAZIONE_TEMPLATES]')
		 )
begin
	CREATE INDEX [DPA_ASSOCIAZIONE_TEMPL_INDX01] ON [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] (ID_OGGETTO)
end
GO

--CREATE INDEX DPA_ASS_TEMPL_FASC_INDX01 ON  dpa_ass_templates_fasc(ID_OGGETTO)
if not exists (
		 select * from dbo.sysindexes 
		 where name = N'DPA_ASS_TEMPL_FASC_INDX01'
		 and id = object_id(N'[@db_user].[dpa_ass_templates_fasc]')
		 )
begin
	CREATE INDEX [DPA_ASS_TEMPL_FASC_INDX01] ON [@db_user].[dpa_ass_templates_fasc] (ID_OGGETTO)
end
GO


IF  EXISTS (		SELECT * FROM dbo.sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[getValCampoProfDoc]')
			AND xtype in (N'FN', N'IF', N'TF')
	)
DROP  function [@db_user].[getValCampoProfDoc]
go
CREATE  FUNCTION [@db_user].[getValCampoProfDoc](@DocNumber INT, @CustomObjectId INT)
RETURNS VARCHAR as
begin
    declare @tipoOggetto varchar(255)
    DECLARE @item varchar(255)
    DECLARE @result VARCHAR(255)
    DECLARE c3 CURSOR LOCAL FOR
    select valore_oggetto_db
    from dpa_associazione_templates
    where id_oggetto = @CustomObjectId and doc_number = @DocNumber

    select @tipoOggetto = b.descrizione
    from dpa_oggetti_custom a, dpa_tipo_oggetto b
    where a.system_id = @CustomObjectId
    and a.id_tipo_oggetto = b.system_id

    --Casella di selezione (Per la casella di selezione serve un caso particolare perch√® i valori sono multipli)
    if (@tipoOggetto = 'CasellaDiSelezione')
    BEGIN
         open c3
         fetch next from c3 into @result
         while @@fetch_status=0
              begin
                  IF(@result IS NOT NULL)
                    BEGIN
                    SET  @result = @result + '; ' + @item

                    END
                    ELSE
                    SET  @result = @result + @item
              end
              CLOSE c3
              deallocate c3
      END
      else
      --Tutti gli altri
          select @result = valore_oggetto_db
          from dpa_associazione_templates
          where id_oggetto = @CustomObjectId and doc_number = @DocNumber

    RETURN @result

end
GO



IF  EXISTS (SELECT * FROM dbo.sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[GetValProfObjPrj ]')
			AND xtype in (N'FN', N'IF', N'TF')
	)
DROP  function [@db_user].[GetValProfObjPrj]
go
CREATE FUNCTION [@db_user].GetValProfObjPrj (
   @PrjId            INT,
   @CustomObjectId   INT
)
   RETURNS VARCHAR
AS
BEGIN
   declare @RESULT       VARCHAR (255)
   
-- Tipo di oggetto su cui compiere la selezione
   declare @objectType  VARCHAR (255)
   DECLARE @item		VARCHAR (255)
   DECLARE curcaselladiselezione CURSOR LOCAL  FOR
            SELECT valore_oggetto_db
                FROM dpa_ass_templates_fasc
                WHERE id_oggetto = @CustomObjectId AND ID_PROJECT = @PrjId
                
-- Selezione del tipo di oggetto
   SELECT @objectType = b.DESCRIZIONE
     FROM dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b
    WHERE a.SYSTEM_ID = @customobjectid AND a.id_tipo_oggetto = b.system_id

--Casella di selezione (Per la casella di selezione serve un caso particolare 
-- perch√® i valori sono multipli)
   IF (@objectType = 'CasellaDiSelezione')
   BEGIN
         -- Concatenazione dei valori definiti per la casella di selezione
         
         BEGIN
            OPEN curcaselladiselezione
			FETCH NEXT from curcaselladiselezione into @item
            while @@fetch_status=0
			BEGIN
               
               IF (@RESULT IS NOT NULL)
               BEGIN
                  SET @RESULT = @RESULT + '; ' + @item
               END 
			ELSE
                  SET @RESULT = @RESULT + @item

            
         END
         CLOSE curcaselladiselezione
         deallocate curcaselladiselezione
      END
   
   END 
ELSE
--Tutti gli altri
      SELECT @RESULT = valore_oggetto_db
        FROM dpa_ass_templates_fasc
       WHERE id_oggetto = @customobjectid AND id_project = @PrjId
   
   RETURN @RESULT

END 
GO
-- fine script Buono P.

/*
 * Autore:					FURNARI
 * Data creazione:			18/11/2010
 * Scopo dell'inserimento:	Microfunzione per abilitare un ruolo alla personalizzazione delle griglie di ricerca documenti / fascicolo.
 *
*/
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='GRID_PERSONALIZATION'))
BEGIN
    INSERT INTO [@db_user].DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
	VALUES ('GRID_PERSONALIZATION', 'Abilita ruolo alla personalizzazione delle griglie di ricerca.', '', 'N');
END
GO
-- Fine scrip Furnari

                    /*
AUTORE:						BUONO
Data creazione:				15/11/2010
Scopo dell'inserimento:		INSERIMENTO CHIAVE DI CONFIGURAZIONE PER ATTIVARE LA GESTIONE DEI CONTATORI CON SOTTOCONTATORE DEI DOCUMENTI
Sviluppi CDC
*/
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='GEST_CONTATORI'))
BEGIN
insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
(
id_amm,
var_codice,
var_descrizione,
var_valore,
cha_tipo_chiave,
cha_visibile,
cha_modificabile,
cha_globale,
var_codice_old_webconfig
)
values
(
0,
'GEST_CONTATORI',
'Abilitazione della funzione di gestione contatori',
'0',
'F',
'0',
'1',
'1',
null
)
END
GO

/*

Convenzione per il nome del file: 0.getChaConsentiClass.sql

----------
Inserire SEMPRE i seguenti COMMENTI nel CORPO della funzione (non dello script): 
AUTORE:		GIAMBOI VERONICA
Data creazione: 18/11/2010
Scopo della funzione: Recuperare il campo consenti classificazione per un dato fascicolo o sottofascicolo

Indicazione della MEV o dello sviluppo a cui √® collegata la funzione: INPS - MEV 3491
*/

IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getChaConsentiClass]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].[getChaConsentiClass] 
go

CREATE FUNCTION [@db_user].[getChaConsentiClass](@id_parent INT,@cha_tipo_proj varchar(1), @id_fascicolo int)
RETURNS varchar(1)
as
begin
declare @risultato varchar(1)

SET @risultato = '0'

if (@cha_tipo_proj = 'F')
	set @risultato = (select cha_consenti_class from project where system_id = @id_parent)
	
if (@cha_tipo_proj = 'C')
	set @risultato = (select cha_consenti_class from project 
		where system_id in (select id_parent from project where system_id = @id_fascicolo))

return @risultato
end
GO

/*
AUTORE:						lorusso
Data creazione:				12/11/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA log PER MONITORARE IL RIMUOVI DALLA TODOLIST.

Indicazione della MEV o dello sviluppo a cui √® collegata la modifica:
							
*/
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='SVUOTA_TDL'))
BEGIN
	insert into [@db_user].dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'SVUOTA_TDL',
	'Rimozione documenti o fascicoli dalla todolist',
	'TRASMISSIONE',
	'SVUOTA_TDL'
	);
END
GO  


BEGIN
EXEC  [@db_user].[CREA_KEYS_AMMINISTRA]
END

GO 


IF  EXISTS (SELECT * FROM sysobjects
     WHERE name = 'hasVersionsFullVisibility' AND --id = OBJECT_ID(N'[hasVersionsFullVisibility]') AND 
     xtype in (N'FN', N'IF', N'TF'))
	DROP  function [@db_user].[hasVersionsFullVisibility]
GO

CREATE FUNCTION [@db_user].[hasVersionsFullVisibility]
(
@idProfile INT,
@idpeople INT,
@idgroup INT
)
RETURNS INT
AS

BEGIN
	declare @retValue int
	declare @idDocument int
	declare @idDocumentoPrincipale int
	declare @hasSecurity int
	
	set @idDocument = @idProfile
	set @retValue = 0
	
	-- 1a) Determina se il documento Ë un allegato
	select @idDocumentoPrincipale = p.id_documento_principale
	from profile p
	where p.system_id = @idDocument

	if (not @idDocumentoPrincipale is null and @idDocumentoPrincipale > 0)
		begin
		    -- L'allegato non ha security, pertanto viene impostato l'id documento principale nell'id profile 
		    set @idDocument = @idDocumentoPrincipale
		end

	-- 2) Verifica se l'utente dispone della visibilit‡ sul documento
	set @hasSecurity = (select count(*) from security s where s.thing = @idDocument and s.personorgroup in (@idPeople, @idGroup))

	if (@hasSecurity > 0)
		begin
		    -- 4) verifica in sercurity se sul doc dispongo di diritti  
		    -- superiori a quelli inviati per tramsmissione
		    set @retValue = (select count(*) 
			    from security s 
			    where s.thing = @idDocument
			            and s.personorgroup in (@idPeople, @idGroup)
			            and (s.hide_doc_versions is null or s.hide_doc_versions = '0'))
		end
	else
		begin
		    -- 2a) L'utente non dispone di alcun diritto di visibilit‡ sul documento, versione non visibile a prescindere 
		    set @retValue = 0
		end


RETURN @retValue

END
GO

/*
AUTORE:						FURNARI
Data creazione:				23/11/2010
Scopo della funzione:		Funzione per il recupero del nome di una ragione di trasmissione a partire dal suo id

Principali funzionalit‡ che attivano la chiamata alla funzione:
							Odinamento per ragione di trasmissione delle trasmissioni ricevute ed effettuate

*/
IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GetTransReasonDesc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].[GetTransReasonDesc] 
go
CREATE function [@db_user].[GetTransReasonDesc] 
(@idReason int)
returns varchar(8000) as
begin
declare @outcome varchar(8000)
SELECT @outcome = R.VAR_DESC_RAGIONE FROM DPA_RAGIONE_TRASM R WHERE R.SYSTEM_ID = @idReason
return @outcome
end
go

-- fine modifica SP_MODIFY_CORR_ESTERNO

-- modifica funzione ifModelloAutorizzato
IF  EXISTS (SELECT * FROM sysobjects
           WHERE name =  'getIfModelloAutorizzato' 
           and  -- id = OBJECT_ID(N'[getIfModelloAutorizzato]') AND
           xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[getIfModelloAutorizzato]
GO

CREATE FUNCTION [@db_user].[getIfModelloAutorizzato]
(
                @id_ruolo int,
                @id_people  int, 
                @system_id  int, 
                @id_modelloTrasm int,
                @accessRigth int
)
RETURNS INT as 

BEGIN 

DECLARE @RetVal INT
DECLARE @idRagione INT
DECLARE @tipo_diritto char

declare @hide_versions INT
declare @isDocument INT 
declare @consolidationState INT 
declare @idObject INT

DECLARE c3 CURSOR LOCAL FOR
            select distinct ID_RAGIONE FROM dpa_modelli_mitt_dest WHERE id_modello=@id_modelloTrasm and CHA_TIPO_MITT_DEST <> 'M'

            SET @RetVal=1

	   -- Verifica se il modello trasmissione includa almeno una trasmissione singola
	   -- con modalit‡ nascondi versioni precedenti
 	   set @hide_versions = (select count(*) from dpa_modelli_mitt_dest where id_modello = @id_modellotrasm and hide_doc_versions = '1')
			

	   if (not @hide_versions is null and @hide_versions > 0)
		begin
			set @idObject = @system_id

		       -- verifica se l'id fornito si riferisce ad un documento o fascicolo
		       set @isDocument = (select count(*) from profile p where p.system_id = @idObject)

		       if (@isDocument > 0)
				begin
				        -- L'istanza su cui si sta applicando il modello Ë un documento,
				        -- verifica se sia consolidato
			            select @consolidationState = p.consolidation_state
			            from profile p
			            where p.system_id = @idObject
	
			            if (@consolidationState is null or @consolidationState = '0') 
					begin
				                -- Il modello prevede di nascondere le versioni di un documento precedenti a quella corrente
				                -- al destinatario della trasmissione, ma in tal caso il documento non Ë stato ancora consolidato,
				                -- pertanto il modello non puÚ essere utilizzato
				                set @retval = 0
					end

		            	end
		end

  if(@accessRigth=45)
   BEGIN
   open c3
   fetch next from c3 into @idRagione
   while @@fetch_status=0     
		BEGIN
			fetch next from c3 into @idRagione
            select @tipo_diritto = CHA_TIPO_DIRITTI from DPA_RAGIONE_TRASM where system_id=@idRagione
            if(@tipo_diritto != 'R' and @tipo_diritto != 'C')
                         begin
                         SET @retVal=0                                
                         end                                                                      
		END              
	close c3
	deallocate c3
	END

RETURN @RetVal
END
go

-- fine modifica funzione ifModelloAutorizzato

-- versione della SP SP_MODIFY_CORR_ESTERNO come presente in ambiente di TEST
if exists (select * from sysobjects 
   where name = 'SP_MODIFY_CORR_ESTERNO' and --id = object_id(N'[@db_user].[SP_MODIFY_CORR_ESTERNO]') and
   OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_MODIFY_CORR_ESTERNO]
GO

CREATE PROCEDURE [@db_user].[SP_MODIFY_CORR_ESTERNO]
@IDCorrGlobale INT,
@desc_corr  VARCHAR(128),
@nome  VARCHAR(50),
@cognome  VARCHAR(50),
@codice_aoo VARCHAR(16),
@codice_amm VARCHAR(32),
@email  VARCHAR(128),
@indirizzo VARCHAR(128),
@cap VARCHAR(5),
@provincia VARCHAR(2),
@nazione VARCHAR(32),
@citta  VARCHAR(64),
@cod_fiscale VARCHAR(16),
@telefono VARCHAR(16),
@telefono2 VARCHAR(16),
@note VARCHAR(250),
@fax VARCHAR(16),
@var_idDocType INT,
@inrubricacomune VARCHAR(1),
@tipourp VARCHAR(1),
@localita VARCHAR(128),
@luogoNascita VARCHAR(128),
@dataNascita VARCHAR(64),
@titolo VARCHAR(64),
@newid INTEGER OUTPUT
AS

/*
1)	update del corrispondente vecchio:
- settando var_cod_rubrica = var_cod_rubrica_system_id;
- settando dta_fine = GETDATE();

2)	insert del nuovo corrispondente (DPA_CORR_GLOBALI):
- codice rubrica = codice rubrica del corrispondente che ? stato storicizzato al punto 1)
- id_old = system_id del corrispondente storicizzato al punto 1)

2.1) insert del dettaglio del  nuovo corrispondente (DPA_DETT_GLOBALI) solo per UTENTI e UO

*/

DECLARE @ReturnValue INT
DECLARE @cod_rubrica VARCHAR(128)
DECLARE @id_reg INT
DECLARE @idAmm INT
DECLARE @new_var_cod_rubrica VARCHAR(128)
DECLARE @cha_dettaglio VARCHAR(1)
DECLARE @cha_tipo_urp VARCHAR(1)
DECLARE @v_id_docType INT
DECLARE @cha_pa VARCHAR(1)
DECLARE @cha_TipoIE VARCHAR(1)
DECLARE @num_livello INT
DECLARE @id_parent INT
DECLARE @id_peso_org INT
DECLARE @id_uo INT
DECLARE @id_tipo_ruolo INT
DECLARE @id_gruppo INT

BEGIN

if(@tipourp is not null and @tipourp != '' and @cha_tipo_urp is not null and @cha_tipo_urp!=@tipourp)
begin
set @cha_tipo_urp = @tipourp
end

SELECT
@cod_rubrica = VAR_COD_RUBRICA,
@cha_tipo_urp = CHA_TIPO_URP,
@id_reg = ID_REGISTRO,
@idAmm = ID_AMM,
@cha_pa = CHA_PA, 
@cha_TipoIE = CHA_TIPO_IE,
@num_livello = NUM_LIVELLO, 
@id_parent = ID_PARENT, 
@id_peso_org = ID_PESO_ORG, 
@id_uo = ID_UO, 
@id_tipo_ruolo = ID_TIPO_RUOLO, 
@id_gruppo = ID_GRUPPO
FROM DPA_CORR_GLOBALI
WHERE system_id = @IDCorrGlobale

IF @@ROWCOUNT > 0  --1
BEGIN

SELECT @v_id_docType = ID_DOCUMENTTYPE
FROM DPA_T_CANALE_CORR
WHERE ID_CORR_GLOBALE = @IDCorrGlobale

IF @@ROWCOUNT > 0
BEGIN
-- calcolo il nuovo codice rubrica
--SET @new_var_cod_rubrica = @var_cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale)
SET @new_var_cod_rubrica = @cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale )

SET @cha_dettaglio = '0' -- default

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
SET @cha_dettaglio = '1'

BEGIN

--VERIFICO se il corrisp ? stato utilizzato come dest/mitt di protocolli
SELECT ID_PROFILE
FROM DPA_DOC_ARRIVO_PAR
WHERE ID_MITT_DEST =  @IDCorrGlobale

-- 1) non ? stato mai utilizzato come corrisp in un protocollo
IF (@@ROWCOUNT = 0)
BEGIN
--non devo storicizzare, aggiorno solamente i dati
UPDATE 	DPA_CORR_GLOBALI
SET 	VAR_CODICE_AOO= @codice_aoo,
VAR_CODICE_AMM = @codice_amm,
VAR_EMAIL = @email,
VAR_DESC_CORR = @desc_corr,
VAR_NOME = @nome,
				VAR_COGNOME= @cognome,
				CHA_PA=@cha_pa,
				CHA_TIPO_URP=@cha_tipo_urp
WHERE 	SYSTEM_ID = @IDCorrGlobale

IF(@@ROWCOUNT > 0) -- SE UPDATE ? andata a buon fine
BEGIN
begin
--per utenti e Uo aggiorno il dettaglio
IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )

BEGIN

UPDATE DPA_DETT_GLOBALI
SET 	VAR_INDIRIZZO = @indirizzo,
VAR_CAP = @cap,
VAR_PROVINCIA = @provincia,
VAR_NAZIONE = @nazione,
VAR_COD_FISCALE = @cod_fiscale,
VAR_TELEFONO = @telefono,
VAR_TELEFONO2 = @telefono2,
VAR_NOTE= @note,
VAR_CITTA= @citta,
VAR_FAX = @fax,
VAR_LOCALITA = @localita,
VAR_LUOGO_NASCITA = @luogoNascita,
DTA_NASCITA = @dataNascita,
VAR_TITOLO = @titolo

WHERE ID_CORR_GLOBALI = @IDCorrGlobale

IF(@@ROWCOUNT > 0)
SET @ReturnValue = 1
ELSE
SET @ReturnValue = 0

END

ELSE
SET @ReturnValue = 1 -- CASO RUOLI
end

IF (@ReturnValue = 1)

BEGIN

UPDATE DPA_T_CANALE_CORR
SET  ID_DOCUMENTTYPE =  @var_idDocType
WHERE ID_CORR_GLOBALE = @IDCorrGlobale

IF(@@ROWCOUNT > 0)
SET @ReturnValue = 1
ELSE
SET @ReturnValue = 0
END

END

ELSE
SET @ReturnValue = 0

END
ELSE

-- caso 2) Il corrisp ? stato utilizzato come destinatario
BEGIN
--  INIZIO STORICIZZAZIONE DEL CORRISPONDENTE

UPDATE	DPA_CORR_GLOBALI
SET 	DTA_FINE = GETDATE(),  
VAR_COD_RUBRICA =@new_var_cod_rubrica,  
VAR_CODICE =@new_var_cod_rubrica,
ID_PARENT = NULL
WHERE 	SYSTEM_ID = @IDCorrGlobale

-- se la storicizzazione ? andata a buon fine,
--posso inserire il nuovo corrispondente
IF @@ROWCOUNT > 0

BEGIN
DECLARE @cha_tipo_corr VARCHAR(1)
IF (@inrubricacomune = '1')
SET @cha_tipo_corr = 'C'
ELSE
SET @cha_tipo_corr = 'S'

INSERT INTO [@db_user].DPA_CORR_GLOBALI (
						NUM_LIVELLO,
						CHA_TIPO_IE,
						ID_REGISTRO,
						ID_AMM,
						VAR_DESC_CORR,
						VAR_NOME,
						VAR_COGNOME,
						ID_OLD,
						DTA_INIZIO,
						ID_PARENT,
						VAR_CODICE,
						CHA_TIPO_CORR,
						CHA_TIPO_URP,
						VAR_CODICE_AOO,
						VAR_COD_RUBRICA,
						CHA_DETTAGLI,
						VAR_EMAIL,
						VAR_CODICE_AMM,
						CHA_PA,
						ID_PESO_ORG,
						ID_GRUPPO,
						ID_TIPO_RUOLO,
						ID_UO
)
VALUES (
						@num_livello,
						@cha_TipoIE,
						@id_reg,
						@idAmm,
						@desc_corr,
						@nome,
						@cognome,
						@IDCorrGlobale,
						GETDATE(),
						@id_parent,
						@cod_rubrica,
						@cha_tipo_corr,
						@cha_tipo_urp,
						@codice_aoo,
						@cod_rubrica,
						@cha_dettaglio,
						@email,
						@codice_amm,
						@cha_pa,
						@id_peso_org,
						@id_gruppo,
						@id_tipo_ruolo,
						@id_uo
)

--prendo la systemId appena inserita
SET @newid = @@identity

IF @@ROWCOUNT > 0 -- se l'inserimento del nuovo corrisp ? andato a buon fine

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' ) -- CASO UTENTE/UO: inserisco il dettaglio
BEGIN

INSERT INTO DPA_DETT_GLOBALI
(
ID_CORR_GLOBALI,
VAR_INDIRIZZO,
VAR_CAP,
VAR_PROVINCIA,
VAR_NAZIONE,
VAR_COD_FISCALE,
VAR_TELEFONO,
VAR_TELEFONO2,
VAR_NOTE,
VAR_CITTA,
VAR_FAX,
VAR_LOCALITA,
VAR_LUOGO_NASCITA,
DTA_NASCITA,
VAR_TITOLO
)
VALUES
(
@newid,
@indirizzo,
@cap,
@provincia,
@nazione,
@cod_fiscale,
@telefono,
@telefono2,
@note,
@citta,
@fax,
@localita,
@luogoNascita,
@dataNascita,
@titolo
)

IF @@ROWCOUNT > 0
-- se la insert su dpa_dett_globali ? andata a buon fine
SET @ReturnValue = 1 -- valore ritornato 1
ELSE
-- se la insert su dpa_dett_globali non ? andata a buon fine
SET @ReturnValue = 0 -- valore ritornato 0
END

ELSE  -- CASO RUOLO: non inserisco il dettaglio
-- vuol dire che il corrispondente ? un RUOLO (quindi non deve essere fatta la insert sulla dpa_dett_globali)
--valore ritornato 1 perch? significa che l'operazione di inserimento del nuovo ruolo ? andato a buon fine
SET @ReturnValue = 1

IF (@ReturnValue = 1)

BEGIN

INSERT INTO DPA_T_CANALE_CORR
(
ID_CORR_GLOBALE,
ID_DOCUMENTTYPE,
CHA_PREFERITO
)
VALUES
(
@newid,
@var_idDocType,
'1'
)

IF @@ROWCOUNT > 0
-- se la insert su DPA_T_CANALE_CORR ? andata a buon fine
SET @ReturnValue = 1 -- valore ritornato 1
ELSE
-- se la insert su DPA_T_CANALE_CORRnon ? andata a buon fine
SET @ReturnValue = 0 -- valore ritornato 0
END
ELSE

SET @ReturnValue = 0 -- inserimento non andato a buon fine: ritorno 0 ed esco

-- FINE STORICIZZAZIONE

END

END

END

END
ELSE
SET @ReturnValue = 0

END

ELSE
SET @ReturnValue = 0 -- la storicizzazione del corrispondente ? andata male:  ritorno 0 ed esco -- END 1

END

RETURN @ReturnValue
GO

/*
AUTORE:						lorusso
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA log PER TIMESTAMP DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui Ë collegata la modifica:
							
*/
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOCONSERVAZIONE'))
BEGIN
	insert into [@db_user].dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOCONSERVAZIONE',
	'Invio in conservazione del documento',
	'DOCUMENTO',
	'DOCUMENTOCONSERVAZIONE'
	);
END
GO  

/*
AUTORE:						lorusso
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA log PER CONVERSIONE PDF DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui Ë collegata la modifica:
							
*/
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOCONVERSIONEPDF'))
BEGIN
	insert into [@db_user].dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOCONVERSIONEPDF',
	'Conversione in pdf del documento',
	'DOCUMENTO',
	'DOCUMENTOCONVERSIONEPDF'
	);
END
GO  

/*
AUTORE:						lorusso
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA log PER TIMESTAMP DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui Ë collegata la modifica:
							
*/
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOTIMESTAMP'))
BEGIN
	insert into [@db_user].dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOTIMESTAMP',
	'Marca Temporale del documento',
	'DOCUMENTO',
	'DOCUMENTOTIMESTAMP'
	);
END
GO  

if not exists(select * from syscolumns where name='FORWARDING_SOURCE' and id in
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN

	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)
    
    SET @proprietario = '@db_user'

-- ALTER TABLE PROFILE ADD FORWARDING_SOURCE int DEFAULT -1
	   SET @sqlstatement = N'alter table ' + @proprietario + '.[PROFILE] ADD FORWARDING_SOURCE int DEFAULT -1'
	   execute sp_executesql @sqlstatement ;

-- per gestire il pregresso, solo su SQL server, occorre fare l'update esplicito
-- va fatto con execute perchÈ la colonna non Ë ancora definita prima del runtime!
-- update profile set 	   FORWARDING_SOURCE = '-1'
	   SET @sqlstatement = N'update ' +@proprietario + '.[PROFILE] set FORWARDING_SOURCE = ''-1'' '
       execute sp_executesql @sqlstatement ;
	


END
GO

if not exists(select * from syscolumns where name='LAST_FORWARD' and id in
		(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)
    
    SET @proprietario = '@db_user'

-- ALTER TABLE PROFILE ADD LAST_FORWARD int DEFAULT -1
	   SET @sqlstatement = N'alter table '+ @proprietario +'.[PROFILE] ADD LAST_FORWARD int DEFAULT -1'
       execute sp_executesql @sqlstatement ;

-- per gestire il pregresso, solo su SQL server, occorre fare l'update esplicito
-- update profile set 	   LAST_FORWARD  = '-1'
	   SET @sqlstatement = N'update '+ @proprietario +'.[PROFILE] set LAST_FORWARD  = ''-1'' '
       execute sp_executesql @sqlstatement ;
	
END
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[Report_Annuale_Doc_Class_compatta]') and 
OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[Report_Annuale_Doc_Class_compatta]
GO

CREATE PROCEDURE [@db_user].[Report_Annuale_Doc_Class_compatta]

--RAPPORTO SUI DOCUMENTI CLASSIFICATI
-------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Fornisce informazioni sulla composizione dei documenti dell'intera AOO che sono stati classificati suddivisi in
-- base alle singole voci di titolario. il prospetto riporta:
-- numero totale dei documenti classificati
-- per ogni voce di titolario:
--- Codice voce di titolario;
--- descrizione voce di titolario;
--- numero totale di documenti classificati in fascicoli associati  alla voce di titolario;
--- Percentuale dei documenti classificati in fascicoli associati alla voce di titolario.

--PARAMETRI DI INPUT
@ID_AMM int,
@ID_REGISTRO int,
@ID_ANNO int,
@VAR_SEDE varchar (255) ='',
@ID_TITOLARIO int

AS

--DICHIARAZIONI VARIABILI
declare @TotDocClass float
declare @CodClass varchar (255)
declare @DescClass varchar (255)
declare @TotDocClassVT int
declare @PercDocClassVT float
declare @Contatore float

--SETTAGGIO INIZIALE VARIABILI
set @PercDocClassVT = 0
set @TotDocClass = 0
set @Contatore = 0


--SELECT PER LA CONTA DI TUTTI I DOCUMENTI CLASSIFICATI RELATIVAMENTE AD UNA AMMINISTRAZIONE
if(@var_sede <> '')
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = [@db_user].[dpa_l_ruolo_reg].id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = [@db_user].[dpa_l_ruolo_reg].id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno)
end

--TABELLA TEMPORANEA ALLOCAZIONE RISULTATI
CREATE TABLE [@db_user].[#TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI]
(
[TOT_DOC_CLASS ] int,
[COD_CLASS] [varchar] (255),
[DESC_CLASS] [varchar] (255),
[TOT_DOC_CLASS_VT] float,
[PERC_DOC_CLASS_VT] float,
[NUM_LIVELLO] [varchar] (255)
)

-- variabili ausiliarie per il cursore che recupera le voci di titolario
DECLARE @SYSTEM_ID_VT INT
DECLARE @DESCRIPTION_VT VARCHAR (255)
DECLARE @VAR_CODICE_VT VARCHAR (255)
DECLARE @NUM_LIVELLO1 VARCHAR (255)
DECLARE @TOT_PRIMO_LIVELLO int
DECLARE @VAR_CODICE_LIVELLO1 VARCHAR (255)
DECLARE @DESCRIPTION__LIVELLO1 VARCHAR (255)
-- variabili ausiliarie per il cursore che re+cupera la lista dei fascicoli
DECLARE @SYSTEM_ID_FASC INT

-- variabili ausiliarie per il cursore che recupera la lista dei folder
DECLARE @SYSTEM_ID_FOLD INT
--inizializzazione del valore contenente la somma dei documenti classificati nel ramo di titolario
set @TOT_PRIMO_LIVELLO = 0

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
DECLARE c_VociTit CURSOR LOCAL FOR -- contiene tutte le voci di titolario (TIPO "T")
select system_id,description,var_codice,num_livello from project where var_codice is not null 
and id_titolario = @ID_TITOLARIO
--id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
 and id_amm =@ID_AMM and cha_tipo_proj = 'T'
and (id_registro = @id_registro OR id_registro is null)
order by var_cod_liv1
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1
while(@@fetch_status=0)
BEGIN
if(@NUM_LIVELLO1 = 1)
begin
set @VAR_CODICE_LIVELLO1 = @VAR_CODICE_VT
set @DESCRIPTION__LIVELLO1 = @DESCRIPTION_VT
end
--------2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
DECLARE c_Fascicoli CURSOR LOCAL FOR -- contiene tutti i fascicoli (TIPO "F")
select system_id
from project
where cha_tipo_proj = 'F' and id_amm = @ID_AMM
and (id_registro = @id_registro or id_registro is null)
and id_parent = @SYSTEM_ID_VT
OPEN c_Fascicoli
FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
while(@@fetch_status=0)
BEGIN
-----------------3 QUERY--Selezione di tutti i folder del fascicolo preselezionato - (input @id_amm)
DECLARE c_Folder CURSOR LOCAL FOR --contiene tutti i folder (TIPO "C")
select system_id from project
where cha_tipo_proj = 'C' and id_amm = @ID_AMM
and id_parent =  @SYSTEM_ID_FASC
and (id_registro = @id_registro or id_registro is null)
OPEN c_Folder
FETCH next from c_Folder into @SYSTEM_ID_FOLD
while(@@fetch_status=0)
BEGIN --(3 ciclo - calcolo paraziale dei doc classificati in ogni folder)
if(@var_sede <> '')
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile ,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo )
end

FETCH next from c_Folder into @SYSTEM_ID_FOLD
END
--(FINE 3 ciclo)
DEALLOCATE c_Folder

FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
END
--(FINE 2 ciclo)
DEALLOCATE c_Fascicoli
-- al termine della conta di tutti i documenti classificati nei fascicoli, prima di cambiare voce di titolario
-- devo inserire le informazioni nella tabella temporanea e resettare il contatore.
---- calcolo delle percentuale dei documenti classificati nella voce di titolario rispetto al totale dei classificati
set @TOT_PRIMO_LIVELLO = @TOT_PRIMO_LIVELLO + @Contatore


---- inserisco le informazioni nella tabella temporanea

--INSERT INTO #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI (TOT_DOC_CLASS,COD_CLASS,DESC_CLASS,TOT_DOC_CLASS_VT,PERC_DOC_CLASS_VT,NUM_LIVELLO)
--VALUES (@TotDocClass,@VAR_CODICE_VT,@DESCRIPTION_VT,@Contatore,@PercDocClassVT,@NUM_LIVELLO1)
---- reset del contatore parziale per la conta della prossima voce di titolario
set @Contatore = 0
---- reset del valore percentuale

FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1
if(@NUM_LIVELLO1=1 or @@fetch_status<>0)
begin
if ((@TOT_PRIMO_LIVELLO <> 0) and (@TotDocClass <>0))
begin
set @PercDocClassVT = ROUND(((@TOT_PRIMO_LIVELLO / @TotDocClass) * 100),2)
end
if (@TotDocClass<>0)
begin
INSERT INTO #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI (TOT_DOC_CLASS,COD_CLASS,DESC_CLASS,TOT_DOC_CLASS_VT,PERC_DOC_CLASS_VT,NUM_LIVELLO)
VALUES (@TotDocClass,@VAR_CODICE_LIVELLO1,@DESCRIPTION__LIVELLO1,@TOT_PRIMO_LIVELLO,@PercDocClassVT,'1')
end
set @TOT_PRIMO_LIVELLO = 0
set @PercDocClassVT = 0
set @PercDocClassVT = 0
end
END
-- (FINE 1 ciclo)
DEALLOCATE c_VociTit

-- restituzione informazioni richieste
select * from #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI

GO



if exists (select * from sysobjects
          where name = 'Report_Annuale_Doc_Class' and -- id = object_id(N'[@db_user].[Report_Annuale_Doc_Class]') and
          OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[Report_Annuale_Doc_Class]
GO

CREATE PROCEDURE [@db_user].[Report_Annuale_Doc_Class]

--RAPPORTO SUI DOCUMENTI CLASSIFICATI
-------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Fornisce informazioni sulla composizione dei documenti dell'intera AOO che sono stati classificati suddivisi in
-- base alle singole voci di titolario. il prospetto riporta:
-- numero totale dei documenti classificati
-- per ogni voce di titolario:
--- Codice voce di titolario;
--- descrizione voce di titolario;
--- numero totale di documenti classificati in fascicoli associati  alla voce di titolario;
--- Percentuale dei documenti classificati in fascicoli associati alla voce di titolario.

--PARAMETRI DI INPUT
@ID_AMM int,
@ID_REGISTRO int,
@ID_ANNO int,
@VAR_SEDE varchar (255) ='',
@ID_TITOLARIO int

AS

--DICHIARAZIONI VARIABILI
declare @TotDocClass float
declare @CodClass varchar (255)
declare @DescClass varchar (255)
declare @TotDocClassVT int
declare @PercDocClassVT float
declare @Contatore float

--SETTAGGIO INIZIALE VARIABILI
set @PercDocClassVT = 0
set @TotDocClass = 0
set @Contatore = 0


--SELECT PER LA CONTA DI TUTTI I DOCUMENTI CLASSIFICATI RELATIVAMENTE AD UNA AMMINISTRAZIONE
if(@var_sede <> '' and @var_sede is not null)
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = [@db_user].[dpa_l_ruolo_reg].id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = [@db_user].[dpa_l_ruolo_reg].id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno)
end
--TABELLA TEMPORANEA ALLOCAZIONE RISULTATI
CREATE TABLE [@db_user].[#TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI]
(
[TOT_DOC_CLASS ] int,
[COD_CLASS] [varchar] (255),
[DESC_CLASS] [varchar] (255),
[TOT_DOC_CLASS_VT] float,
[PERC_DOC_CLASS_VT] float,

) 

-- variabili ausiliarie per il cursore che recupera le voci di titolario
DECLARE @SYSTEM_ID_VT INT
DECLARE @DESCRIPTION_VT VARCHAR (255)
DECLARE @VAR_CODICE_VT VARCHAR (255)

-- variabili ausiliarie per il cursore che re+cupera la lista dei fascicoli
DECLARE @SYSTEM_ID_FASC INT

-- variabili ausiliarie per il cursore che recupera la lista dei folder
DECLARE @SYSTEM_ID_FOLD INT

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
DECLARE c_VociTit CURSOR LOCAL FOR -- contiene tutte le voci di titolario (TIPO "T")
select system_id,description,var_codice from project where var_codice is not null
--AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
AND id_titolario = @ID_TITOLARIO
AND id_amm =@ID_AMM and cha_tipo_proj = 'T'
and (id_registro = @id_registro OR id_registro is null)
order by VAR_COD_LIV1
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT
while(@@fetch_status=0)
BEGIN
--------2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
DECLARE c_Fascicoli CURSOR LOCAL FOR -- contiene tutti i fascicoli (TIPO "F")
select system_id
from project
where cha_tipo_proj = 'F' and id_amm = @ID_AMM
and (id_registro = @id_registro or id_registro is null)
and id_parent = @SYSTEM_ID_VT
OPEN c_Fascicoli
FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
while(@@fetch_status=0)
BEGIN
-----------------3 QUERY--Selezione di tutti i folder del fascicolo preselezionato - (input @id_amm)
DECLARE c_Folder CURSOR LOCAL FOR --contiene tutti i folder (TIPO "C")
select system_id from project
where cha_tipo_proj = 'C' and id_amm = @ID_AMM
and id_parent =  @SYSTEM_ID_FASC
and (id_registro = @id_registro or id_registro is null)
OPEN c_Folder
FETCH next from c_Folder into @SYSTEM_ID_FOLD
while(@@fetch_status=0)
BEGIN --(3 ciclo - calcolo paraziale dei doc classificati in ogni folder)
if(@var_sede <> '' and @var_sede is not null)
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile ,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo )
end
FETCH next from c_Folder into @SYSTEM_ID_FOLD
END
--(FINE 3 ciclo)
DEALLOCATE c_Folder

FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
END
--(FINE 2 ciclo)
DEALLOCATE c_Fascicoli
-- al termine della conta di tutti i documenti classificati nei fascicoli, prima di cambiare voce di titolario
-- devo inserire le informazioni nella tabella temporanea e resettare il contatore.
---- calcolo delle percentuale dei documenti classificati nella voce di titolario rispetto al totale dei classificati
if ((@Contatore <> 0) and (@TotDocClass <>0))
begin
set @PercDocClassVT = ROUND(((@Contatore / @TotDocClass) * 100),2)
end
---- inserisco le informazioni nella tabella temporanea
if (@Contatore <> 0)
begin
INSERT INTO #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI (TOT_DOC_CLASS,COD_CLASS,DESC_CLASS,TOT_DOC_CLASS_VT,PERC_DOC_CLASS_VT)
VALUES (@TotDocClass,@VAR_CODICE_VT,@DESCRIPTION_VT,@Contatore,@PercDocClassVT)
end
---- reset del contatore parziale per la conta della prossima voce di titolario
set @Contatore = 0
---- reset del valore percentuale
set @PercDocClassVT = 0
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT
END
-- (FINE 1 ciclo)
DEALLOCATE c_VociTit

-- restituzione informazioni richieste
select * from #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI

GO

Insert into [@db_user].[DPA_DOCSPA]
   ( DTA_UPDATE, ID_VERSIONS_U)
 Values
   ( getdate(), '3.12')
GO
