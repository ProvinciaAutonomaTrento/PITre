 -- inserito all'inizio da SAB
 ---- HasQualifica.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[HasQualifica]') 			)
	DROP FUNCTION [@db_user].HasQualifica
GO

CREATE FUNCTION [@db_user].[HasQualifica] 
(	@codiceQualifica varchar(64),	@idUO int) 
RETURNS int 
AS
BEGIN
declare @count int
declare @retValue int
	
if @codiceQualifica  is null 
BEGIN
	set @retValue =  0
	return @retValue
END ELSE BEGIN
	select @count = COUNT(pgq.SYSTEM_ID)
	from DPA_PEOPLEGROUPS_QUALIFICHE pgq
	inner join DPA_QUALIFICHE q on pgq.ID_QUALIFICA = q.SYSTEM_ID
		where q.CHA_COD = @codiceQualifica and pgq.ID_UO = @idUO
END	

if (@count > 0)
BEGIN	set @retValue = 1
END else BEGIN
	set @retValue =  0
END
		
	return @retValue
END
GO

              
----------- FINE -
           
IF EXISTS (SELECT * FROM sys.objects where type = 'P' and name = 'Utl_Insert_Chiave_Config') 
DROP PROCEDURE @db_user.[Utl_Insert_Chiave_Config]
GO

CREATE Procedure @db_user.[Utl_Insert_Chiave_Config](
    @Codice               VARCHAR(100) ,
    @Descrizione          VARCHAR(2000) ,
    @Valore               VARCHAR(128) ,
    @Tipo_Chiave          Varchar(1) ,
    @Visibile             VARCHAR(1) ,
    @Modificabile         VARCHAR(1) ,
    @Globale              VARCHAR(1) ,
    @myversione_CD          Varchar(32) ,
    @Codice_Old_Webconfig Varchar(128) ,
    @Forza_Update  VARCHAR(1) , @RFU VARCHAR(10) ) As  

BEGIN



  declare @Cnt Int

  -- controlli lunghezza valori passati
  If @db_user.Utl_IsValore_Lt_Column(@Codice, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_CODICE') = 1 
  begin
     print 'parametro CODICE too large for column VAR_CODICE' 
     return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@Descrizione, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Descrizione') = 1 
  begin
	print 'parametro Descrizione too large for column VAR_Descrizione'  
	return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@Valore, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Valore') = 1 
	begin
    print 'parametro Valore too large for column VAR_VALORE'
    return -1
END 

  -- fine controlli lunghezza valori passati

  SELECT @cnt = COUNT(*)  
  FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
  Where Var_Codice=@Codice

exec  @db_user.UTL_ADD_COLUMN    @myVERSIONE_CD,    '@db_user' ,    'DPA_CHIAVI_CONFIGURAZIONE',    'DTA_INSERIMENTO',    'DATETIME',    'GETDATE()',    Null,    Null,   Null    
exec  @db_user.UTL_ADD_COLUMN    @myVERSIONE_CD,    '@db_user' ,    'DPA_CHIAVI_CONFIGURAZIONE',    'VERSIONE_CD',    'varchar(32)',    NULL,    Null,    Null,   Null    

SET @Codice_Old_Webconfig = ISNULL(@Codice_Old_Webconfig,'NULL')

  If (@Cnt         = 0 and @Globale    = '1') 
  begin -- inserisco la chiave globale non esistente
  INSERT INTO  @db_user.DPA_CHIAVI_CONFIGURAZIONE
    ( Id_Amm,          
    Var_Codice ,
      VAR_DESCRIZIONE ,          
      VAR_VALORE ,
      Cha_Tipo_Chiave,          
      Cha_Visibile ,
      CHA_MODIFICABILE,          
      CHA_GLOBALE ,          
      VAR_CODICE_OLD_WEBCONFIG , 
      DTA_INSERIMENTO, 
      VERSIONE_CD)
    Values        
    ( 0, 
    @Codice ,
    @Descrizione ,
    @Valore ,
    @Tipo_Chiave ,
    @Visibile ,
    @Modificabile,
    @Globale,
    @Codice_Old_Webconfig    ,
    getdate() ,
    @myVERSIONE_CD )
          print 'inserita nuova chiave globale ' + @Codice
	End 
  If (@Cnt         = 0 And @Globale    = '0') 
  begin -- inserisco la chiave non globale non esistente
      -- questa forma evita di dover eseguire la procedura CREA_KEYS_AMMINISTRA di riversamento !
       INSERT      INTO  @db_user.DPA_CHIAVI_CONFIGURAZIONE
        ( Id_Amm,          
        Var_Codice ,
          VAR_DESCRIZIONE ,          
          VAR_VALORE ,
          Cha_Tipo_Chiave,          
          Cha_Visibile ,
          CHA_MODIFICABILE,          
          CHA_GLOBALE ,          
          VAR_CODICE_OLD_WEBCONFIG , 
          DTA_INSERIMENTO, 
          VERSIONE_CD)
      SELECT 
      Amm.System_Id  As Id_Amm, 
      @Codice As Var_Codice,
      @Descrizione As Var_Descrizione , 
      @Valore As Var_Valore ,
      @Tipo_Chiave As Cha_Tipo_Chiave,
      @Visibile As Cha_Visibile ,
      @Modificabile As Cha_Modificabile, 
      @Globale As CHA_GLOBALE ,
      @Codice_Old_Webconfig  as VAR_CODICE_OLD_WEBCONFIG ,
      getdate() ,
      @myVERSIONE_CD
      FROM @db_user.Dpa_Amministra Amm       
      WHERE NOT EXISTS
      (SELECT Id_Amm        FROM @db_user.Dpa_Chiavi_Configurazione
        WHERE var_codice = @Codice)
        
        print 'inserita nuove n chiavi locali per le n amministrazioni: ' + @Codice
  End

  IF (@Cnt                  = 1) 
  begin -- chiave gi esistente
    PRINT 'chiave ' + @Codice + ' gi esistente'
    IF @Forza_Update = '1' 
    begin
      UPDATE @db_user.Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = @descrizione,
        VAR_VALORE        = @valore, 
		Cha_Visibile	  = @Visibile,
		Cha_Modificabile  = @Modificabile,
		cha_Tipo_Chiave	  = @Tipo_Chiave	
      Where Var_Codice    = @Codice       and CHA_MODIFICABILE = '1'
      PRINT  'AGGIORNATO VALORE, visibilit, modificabilit e tipo, per la CHIAVE: ' + @Codice + ' gi esistente' 

    end ELSE begin-- aggiorno solo la descrizione
      UPDATE @db_user.Dpa_Chiavi_Configurazione
      SET Var_Descrizione = @Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    = @Codice and CHA_MODIFICABILE = '1'

    END 
  END 
  
END
GO
---- Utl_Insert_Chiave_Config.MSSQL.sql  marcatore per ricerca ----
-- function used by Utl_Insert_Chiave_Config and other 2 Utl_Insert_Chiave%
IF EXISTS (SELECT * FROM sys.objects where type = 'FN' and name = 'UTL_ISVALORE_LT_COLUMN') 
DROP FUNCTION @db_user.[Utl_Isvalore_Lt_Column]
GO

CREATE Function [@db_user].[Utl_Isvalore_Lt_Column]
(@Valore Varchar(1000)
,@Mytable Varchar(100)
,@mycol varchar(100) )
Returns integer -- returns 0 if lentgh(valore) less then  Data_Length of the column mycol
as
begin 
declare @Cnt Int
declare @returnvalue int 

SET @returnvalue = -1

select @cnt = 
case DATA_TYPE when 'varchar' then CHARACTER_MAXIMUM_LENGTH else NUMERIC_PRECISION end - Len('A') 
	from INFORMATION_SCHEMA.COLUMNS 
	where Table_Name=upper(@Mytable) And Column_Name = upper(@Mycol) 
		
If @Cnt>=0 begin 	
	SET @returnvalue = 0  
End

Return @Returnvalue 
end 
GO



            
---- Utl_insert_chiave_log.MSSQL.sql  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='UTL_INSERT_CHIAVE_LOG')
DROP PROCEDURE @db_user.Utl_insert_chiave_log
go


CREATE Procedure @db_user.[Utl_insert_chiave_log](
    @Codice               VARCHAR(100) ,
    @Descrizione          VARCHAR(2000) ,
    @Oggetto               VARCHAR(128) ,
    @Metodo	          Varchar(128) ,
    @Forza_Aggiornamento  VARCHAR(1) ,
    @myversione_CD          Varchar(32) ,
     @RFU VARCHAR(10) ) As  

BEGIN  declare @Cnt Int

  -- controlli lunghezza valori passati
  If @db_user.Utl_IsValore_Lt_Column(@Codice, 'dpa_anagrafica_log', 'VAR_CODICE') = 1 
  begin
     print 'parametro CODICE too large for column VAR_CODICE' 
     return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@Descrizione, 'dpa_anagrafica_log', 'VAR_Descrizione') = 1 
  begin
	print 'parametro Descrizione too large for column VAR_Descrizione'  
	return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@Oggetto, 'dpa_anagrafica_log', 'VAR_OGGETTO') = 1 
	begin
    print 'parametro Valore too large for column VAR_OGGETTO'
    return -1
	END 

If @db_user.Utl_IsValore_Lt_Column(@Metodo, 'dpa_anagrafica_log', 'VAR_METODO') = 1  
    begin
    print 'parametro Valore too large for column VAR_METODO'  
	return -1
	END 
  -- fine controlli lunghezza valori passati

  SELECT @cnt = COUNT(*)  
  FROM dpa_anagrafica_log
  Where Var_Codice=@Codice


  If (@Cnt         = 0 ) 
  begin -- inserisco la chiave globale non esistente
       Insert  Into Dpa_Anagrafica_Log
        ( System_Id,     var_Codice,      Var_Descrizione
        , Var_Oggetto,Var_Metodo ) 
      Select Max(System_Id) +1 As System_Id       ,@Codice, @Descrizione        
      , @OGGETTO, @METODO
      From dpa_anagrafica_log; 
          print 'inserita nuova chiave globale ' + @Codice
	End 
  
  IF (@Cnt                  = 1 and @Forza_Aggiornamento = '1') 
  begin -- chiave gi esistente
    UPDATE dpa_anagrafica_log
      SET Var_Descrizione = @Descrizione, Var_Oggetto = @Oggetto, Var_Metodo = @Metodo
      WHERE Var_Codice    = @Codice
  END 

END
GO

              
---- Utl_Insert_Chiave_Microfunz.MSSQL.sql  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='UTL_INSERT_CHIAVE_MICROFUNZ')
DROP PROCEDURE @db_user.Utl_Insert_Chiave_Microfunz
go


CREATE Procedure @db_user.Utl_Insert_Chiave_Microfunz(
    @Codice               VARCHAR(100) ,
    @Descrizione          VARCHAR(2000) ,
    @Tipo_Chiave            Varchar(10) ,
    @disabilitata           VARCHAR(1) ,
    @Forza_Disabilitazione  Varchar(1) , 
    @myversione_CD          Varchar(32) ,
    @RFU VARCHAR(10) ) As  

BEGIN

 declare @Cnt Int
 declare @Nomeutente VARCHAR(20)
 declare @stringa_msg VARCHAR(200)
 
 
  -- controlli lunghezza valori passati
  If @db_user.Utl_IsValore_Lt_Column(@Codice, 'dpa_anagrafica_funzioni', 'COD_FUNZIONE') = 1  
  begin
     set @stringa_msg =  'parametro CODICE too large for column COD_FUNZIONE' 
     return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@Descrizione, 'dpa_anagrafica_funzioni', 'VAR_DESC_FUNZIONE') = 1 
  begin
	set @stringa_msg =  'parametro Descrizione too large for column VAR_DESC_FUNZIONE'  
	return -1
  End 
  
  If @db_user.Utl_IsValore_Lt_Column(@disabilitata, 'dpa_anagrafica_funzioni', 'DISABLED') = 1 
	begin
    set @stringa_msg = 'parametro Valore too large for column DISABLED'
    return -1
	END 
	
  -- fine controlli lunghezza valori passati

  SELECT @cnt = COUNT(*)  
  FROM @db_user.dpa_anagrafica_funzioni
  Where COD_FUNZIONE=@Codice


  If (@Cnt         = 0 ) 
  begin -- inserisco la chiave globale non esistente
       INSERT  INTO dpa_anagrafica_funzioni
        ( COD_FUNZIONE,VAR_DESC_FUNZIONE
        ,CHA_TIPO_FUNZ,DISABLED )
        Values
        ( @Codice,@Descrizione          
        ,@Tipo_Chiave          ,@disabilitata  )
        set @stringa_msg =  'inserita nuova micro: ' + @Codice ; 
       
  End 
  
  If (@Cnt                  = 1) begin -- chiave gi esistente
		IF @Forza_Disabilitazione = '1' begin
		  UPDATE dpa_anagrafica_funzioni
		  SET VAR_DESC_FUNZIONE = @Descrizione,
			DISABLED        = @disabilitata
		  Where COD_FUNZIONE    = @Codice
		  set @stringa_msg =  'AGGIORNATO VALORE DISABLED: '+ @disabilitata +' PER micro ' + @Codice + ' gi esistente'
		END ELSE BEGIN -- aggiorno solo la descrizione
		  UPDATE dpa_anagrafica_funzioni
		  SET VAR_DESC_FUNZIONE = @Descrizione 
		  WHERE COD_FUNZIONE    = @Codice
		END 
   END 
SELECT top 1 @Nomeutente =  SCHEMA_NAME(schema_id) FROM SYS.tables 
EXEC @db_user.Utl_Insert_Log  @Nomeutente, GETDATE, @stringa_msg, @Myversione_Cd ,'ok' 
  
END
GO

              
---- utl_INSERT_LOG.MSSQL.SQL  marcatore per ricerca ----
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 11/07/2011
-- Description:	
-- =============================================

	IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='UTL_INSERT_LOG')
DROP PROCEDURE @db_user.[utl_insert_log]
go


CREATE  PROCEDURE @db_user.[utl_insert_log]
	-- Add the parameters for the stored procedure here
		@nome_utente Nvarchar(200), 
		@data_eseguito NVARCHAR(200), 
		@comando_eseguito Nvarchar (200),
		@versione_CD Nvarchar(200),
		@esito    Nvarchar(200)	
		

AS
BEGIN

	DECLARE @istruzione    Nvarchar(2000)
	--DECLARE @data_eseguito Nvarchar (200)
	
	--SET IDENTITY_INSERT [@db_user].DPA_LOG_INSTALL ON
	set @data_eseguito = (select convert (datetime, getdate(), 120))
	
	SET @istruzione = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL VALUES (
		'''+@data_eseguito+''',
        '''+@comando_eseguito+''',
        '''+@versione_CD+''',
        '''+@esito+''')'
        
        execute sp_executesql @istruzione
        print @istruzione

END            
GO
              
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_CORR_GLOBALI.MSSQL.sql  marcatore per ricerca ----
execute @db_user.utl_add_column '3.21.1', '@db_user', 
	'DPA_CORR_GLOBALI', 'INTEROPRFID', 'NUMBER', NULL, NULL, NULL, NULL
GO


-- modifiche by S. Frezza:
-- su tabella DPA_CORR_GLOBALI, aggiunta del campo CLASSIFICA_UO in cui sono inseriti 
--	i dati aggiuntivi che qualificano una unit organizzativa.


execute @db_user.utl_add_column '3.21.1', '@db_user', 'DPA_CORR_GLOBALI', 'CLASSIFICA_UO', 'VARCHAR(50)', NULL, NULL, NULL, NULL
GO


-- l'operazione di inizializzazione successiva va fatta solo all'atto della creazione della nuova colonna! 
-- quindi prima di creare la colonna controllo se la colonna non c'
declare @cntcol int
select @cntcol = COUNT(*)  from syscolumns where name='INTEROPURL' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U')

execute @db_user.utl_add_column '3.21.1', '@db_user', 'DPA_CORR_GLOBALI', 'INTEROPURL', 'VARCHAR(4000)', NULL, NULL, NULL, NULL
IF @cntcol = 0 
BEGIN
--	print @cntcol 
		update @db_user.dpa_corr_globali
		set cha_dettagli = '1'
		where id_rf is not null    

		insert into @db_user.dpa_dett_globali( -- system_id, is identity in sql server 
									id_corr_globali)
		(select --		seq.nextval,
		system_id
		from @db_user.dpa_corr_globali
		where id_rf is not null) 
END
GO


execute @db_user.utl_add_column '3.21.1', '@db_user', 'DPA_CORR_GLOBALI', 'INTEROPREGISTRYID', 'INT', NULL, NULL, NULL, NULL

-- l'operazione di inizializzazione successiva va fatta solo all'atto della creazione della nuova colonna! 
-- quindi prima di creare la colonna controllo se la colonna non c'
declare @cntcol int
select @cntcol = COUNT(*)  from syscolumns where name='VAR_DESC_CORR_OLD' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U')
EXECUTE @db_user.utl_add_column '3.21.1', '@db_user', 'DPA_CORR_GLOBALI', 'VAR_DESC_CORR_OLD', 'VARCHAR(256)', NULL, NULL, NULL, NULL
IF @cntcol = 0 
BEGIN
--	print @cntcol 
	update @db_user.DPA_CORR_GLOBALI 
	set  VAR_DESC_CORR_OLD = VAR_DESC_CORR 
	where VAR_DESC_CORR_OLD is null
END
GO


              
----------- FINE -
              
---- ALTER_DPA_PAROLE.MSSQL.sql  marcatore per ricerca ----
-- ALTER TABLE @db_user.DPA_PAROLE	ADD ID_REGISTRO INT;
EXECUTE @db_user.utl_add_column '3.21.1', '@db_user'	
			, 'DPA_PAROLE', 'ID_REGISTRO', 'INTEGER'	, NULL, NULL, NULL, NULL
GO
              
----------- FINE -
              
---- ALTER_PEOPLE.MSSQL.sql  marcatore per ricerca ----

-- by S. Frezza su tabella PEOPLE, aggiunta del campo MATRICOLA, che ha la stessa dimensione del campo userid.
execute @db_user.utl_add_column '3.21.1', '@db_user'
, 'PEOPLE', 'MATRICOLA', 'VARCHAR(32)', NULL, NULL, NULL, NULL

--by Lorusso
execute @db_user.utl_add_column '3.21.1', '@db_user'
 , 'PEOPLE', 'ABILITATO_CHIAVI_CONFIG', 'INTEGER', '0', NULL, NULL, NULL 
GO

update @db_user.PEOPLE 
	set ABILITATO_CHIAVI_CONFIG = 1 
	where  cha_amministratore = '2'
GO


              
----------- FINE -
              
---- CREATE_DPA_ASS_DOC_MAIL_INTEROP.MSSQL.SQL  marcatore per ricerca ----
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_DOC_MAIL_INTEROP]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_ASS_DOC_MAIL_INTEROP](
SYSTEM_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
ID_PROFILE INT NOT NULL ,
ID_REGISTRO INT  NOT NULL ,
VAR_EMAIL_REGISTRO VARCHAR(128))
end
GO              
----------- FINE -
              
---- CREATE_DPA_MAIL_CORR_ESTERNI.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_MAIL_CORR_ESTERNI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_MAIL_CORR_ESTERNI](
SYSTEM_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ID_CORR		           INT NOT NULL ,
	VAR_EMAIL_REGISTRO     VARCHAR(128),
    VAR_PRINCIPALE         VARCHAR(1),
    VAR_NOTE               VARCHAR(50))
end
GO
              
----------- FINE -
              
---- CREATE_DPA_MAIL_REGISTRI.MSSQL.SQL  marcatore per ricerca ----
		IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_MAIL_REGISTRI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_MAIL_REGISTRI](
SYSTEM_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
ID_REGISTRO            INT NOT NULL ,
VAR_PRINCIPALE         VARCHAR(1),
VAR_EMAIL_REGISTRO     VARCHAR(128),
VAR_USER_MAIL          VARCHAR(128),
VAR_PWD_MAIL           VARCHAR(32),
VAR_SERVER_SMTP        VARCHAR(64),
CHA_SMTP_SSL           VARCHAR(1),
CHA_POP_SSL            VARCHAR(1),
NUM_PORTA_SMTP         NUMERIC(10) ,
CHA_SMTP_STA           VARCHAR(1),
VAR_SERVER_POP         VARCHAR(64),
NUM_PORTA_POP          NUMERIC(10),
VAR_USER_SMTP          VARCHAR(128),
VAR_PWD_SMTP           VARCHAR(128),
VAR_INBOX_IMAP         VARCHAR(128),
VAR_SERVER_IMAP        VARCHAR(128),
NUM_PORTA_IMAP         NUMERIC(10),
VAR_TIPO_CONNESSIONE   VARCHAR(10),
VAR_BOX_MAIL_ELABORATE VARCHAR(50),
VAR_MAIL_NON_ELABORATE VARCHAR(50),
CHA_IMAP_SSL           VARCHAR(1),
VAR_SOLO_MAIL_PEC      VARCHAR(1) DEFAULT '0',
CHA_RICEVUTA_PEC       VARCHAR(2),
VAR_NOTE               VARCHAR(50))
END
GO              
----------- FINE -
              
---- CREATE_DPA_PEOPLEGROUPS_QUALIFICHE.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_PEOPLEGROUPS_QUALIFICHE]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_PEOPLEGROUPS_QUALIFICHE](
[SYSTEM_ID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
[ID_AMM] [int] NULL,
[ID_UO] [int] NULL,
[ID_GRUPPO] [int] NULL,
[ID_PEOPLE] [int] NULL,
[ID_QUALIFICA] [int] NULL)
end
GO              
----------- FINE -
              
---- CREATE_DPA_QUALIFICHE.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_QUALIFICHE]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_QUALIFICHE](
[SYSTEM_ID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
[CHA_COD] [varchar](64) NULL,
[CHA_DESC] [varchar](255) NULL,
[ID_AMM] [int] NULL)
end
GO              
----------- FINE -
              
---- CREATE_DPA_VIS_MAIL_REGISTRI.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_VIS_MAIL_REGISTRI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[DPA_VIS_MAIL_REGISTRI](
SYSTEM_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ID_REGISTRO        INT NOT NULL ,
    ID_RUOLO_IN_UO     INT NOT NULL ,
    VAR_EMAIL_REGISTRO VARCHAR(128),
    CHA_CONSULTA       VARCHAR(1) DEFAULT '1',
    CHA_NOTIFICA       VARCHAR(1) DEFAULT '1',
    CHA_SPEDISCI       VARCHAR(1) DEFAULT '1')
end
GO              
----------- FINE -
              
---- CREATE_INDEX_DPA_ITEMS_CONSERVAZIONE_ID_PROFILE.MSSQL.sql  marcatore per ricerca ----
--create index PAT_PROD.IND_ITEMS_CNSRVZ_K3 on PAT_PROD.DPA_ITEMS_CONSERVAZIONE (Id_Profile)

begin 
execute @db_user.utl_add_index	'3.21.1'   	
	,'@db_user'  	,'DPA_ITEMS_CONSERVAZIONE' ,'IND_ITEMS_CNSRVZ_K3'  ,NULL  
	,'ID_PROFILE'  	,NULL  	,NULL  	,'NORMAL'  ,NULL  ,NULL  ,NULL  
end 
GO


              
----------- FINE -
              
---- CREATE_INDEX_DPA_NOTIFICA_DOCNUMBER.MSSQL.sql  marcatore per ricerca ----
begin 
execute [@db_user].[utl_add_index] 
	'3.21.1'   
	,'@db_user'  
	,'DPA_NOTIFICA'  
	,'IND_NOTIFICA_K3'  
	,NULL  
	,'DOCNUMBER'  
	,NULL  
	,NULL  
	,'NORMAL'  
	,NULL  ,NULL  ,NULL  
end 
GO


              
----------- FINE -
              
---- Create_NotificationChannel.MSSQL.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONCHANNEL
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NotificationChannel]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NotificationChannel 
			( Id INT IDENTITY(1,1), 
			  Label VarChar(100), 
			  Description VarChar(2000) )
end
GO              
----------- FINE -
              
---- Create_NotificationInstance.MSSQL.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONINSTANCE
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NOTIFICATIONINSTANCE]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NOTIFICATIONINSTANCE 
			( Id INT IDENTITY(1,1), 
			  Description VarChar(2000) )
end
GO

              
----------- FINE -
              
---- Create_NOTIFICATIONINSTANCECHANNELS.MSSQL.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONSETTINGS
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NOTIFICATIONINSTANCECHANNELS]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NOTIFICATIONINSTANCECHANNELS
								( InstanceId INT IDENTITY(1,1),
								 ChannelId INT )
END
GO
              
----------- FINE -
              
---- Create_NotificationItem.MSSQL.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONITEM
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NOTIFICATIONITEM]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NOTIFICATIONITEM
								( Id INT IDENTITY(1,1),
								  Author VarChar(100),
								  Title VarChar(2000), 
								  Text VarChar(2000), 
								  FeedLink VarChar(1000), 
								  LastUpdate Date, 
								  PublishDate Date,
								 MessageId integer, 
								 MESSAGENUMBER integer  )
END
GO

EXECUTE		@db_user.utl_add_column '3.21.1', '@db_user'
 , 'NOTIFICATIONITEM', 'MESSAGENUMBER', ' integer', NULL, NULL, NULL, NULL
 GO
              
----------- FINE -
              
---- Create_NotificationItemCategories.MSSQL.sql  marcatore per ricerca ----
	--Creazione Tabella NOTIFICATIONITEMCATEGORIES
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NOTIFICATIONITEMCATEGORIES]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NOTIFICATIONITEMCATEGORIES
								( ItemId INT IDENTITY(1,1),
								 CategoryId INT )
END
GO

              
----------- FINE -
              
---- Create_NotificationUser.MSSQL.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONITEMCATEGORIES
   IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[NOTIFICATIONUSER]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.NOTIFICATIONUSER
								( ItemId INT IDENTITY(1,1),
								 UserId INT,
								 ViewDate Date,
								InstanceId integer)
END
GO

EXECUTE @db_user.utl_add_column '3.21.1', '@db_user'
 , 'NotificationUser', 'InstanceId', ' integer', NULL, NULL, NULL, NULL 
 GO


              
----------- FINE -
              
---- CREATE_SimpInteropDbLog.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[SIMPINTEROPDBLOG]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[SIMPINTEROPDBLOG](
ProfileId integer,
ErrorMessage Numeric,
Text Varchar(4000))
end
GO
              
----------- FINE -
              
---- CREATE_SimpInteropReceivedMessage.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[SimpInteropReceivedMessage]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE [@db_user].[SimpInteropReceivedMessage](
			  ProfileID int PRIMARY KEY NOT NULL,
    		  MessageId			VarChar(1000),
			  ReceivedPrivate	INT,
			  ReceivedDate		Date,
			  Subject			VarChar(4000),
			  SenderDescription VarChar(4000),
			  SenderUrl			VarChar(2000),
			  SenderAdministrationCode VarChar(2000), 
			  AOOCode			VarChar(2000), 
			  RecordNumber		Numeric, 
			  RecordDate		Date,
  -- added lately
  ReceiverCode VarChar(2000) )
end
GO

EXECUTE @db_user.utl_add_column '3.21.1', '@db_user'
 , 'SimpInteropReceivedMessage', 'ReceiverCode', ' VarChar(2000)', NULL, NULL, NULL, NULL 
 GO

              
----------- FINE -
              
---- CREATE_TABLE_INTEROPERABILITYSETTINGS.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[INTEROPERABILITYSETTINGS]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
Create Table @db_user.INTEROPERABILITYSETTINGS
	( RegistryId                int,
	  RoleId                    int,
	  UserId                    int,
	  IsEnabledInteroperability int,
	  ManagementMode            Varchar(1),
	  KeepPrivate               int)
  END
GO


              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- INSERT_DOCUMENTTYPES.MSSQL.SQL  marcatore per ricerca ----
-- aggiungendo la colonna mancante DELETED
execute @db_user.utl_add_column '3.21.1', '@db_user', 'DOCUMENTTYPES', 'DELETED', 'CHAR(1)', NULL, NULL, NULL, NULL
go



--Se non esiste      TYPE_ID =     'SIMPLIFIEDINTEROPERABILITY', inserisci il record

IF NOT EXISTS (SELECT * FROM @db_user.DOCUMENTTYPES 
				WHERE TYPE_ID='SIMPLIFIEDINTEROPERABILITY')
BEGIN
 
INSERT INTO @db_user.DOCUMENTTYPES
  (
    TYPE_ID,
    DESCRIPTION,
    DISABLED,
    STORAGE_TYPE,
    RETENTION_DAYS,
    MAX_VERSIONS,
    MAX_SUBVERSIONS,
    FULL_TEXT,
    TARGET_DOCSRVR,
    RET_2,
    RET_2_TYPE,
    KEEP_CRITERIA,
    VERSIONS_TO_KEEP,
    CHA_TIPO_CANALE,
    DELETED
  )
  VALUES
  ('SIMPLIFIEDINTEROPERABILITY',
    'Interoperabilit PiTre',
    null,
    'A',
    0,
    99,
    26,
    'N',
    0,
    0,
    'A',
    'L',
    0,
    'S',
    null
  )
  END
  GO
              
----------- FINE -
              
---- INSERT_DPA_ANAGRAFICA_FUNZIONI.MSSQL.SQL  marcatore per ricerca ----
BEGIN
-- by Furnari per IS
execute @db_user.Utl_Insert_Chiave_Microfunz 'NOTIFICATION_CENTER', -- Codice                 
'Visualizza gli item del centro notifiche nella to do list e ne abilita la gestione',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL --Forza_Disabilitazione , Myversione_Cd, RFU  

execute @db_user.Utl_Insert_Chiave_Microfunz 'PRAUISNP', -- Codice                 
'PRotocollazione AUtomatica Interoperabilit Semplificata documenti Non Privati',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL --Forza_Disabilitazione , Myversione_Cd, RFU  
 
execute @db_user.Utl_Insert_Chiave_Microfunz 'PRAUISP', -- Codice                 
'PRotocollazione AUtomatica Interoperabilit Semplificata documenti Privati',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL--Forza_Disabilitazione , Myversione_Cd, RFU  

execute @db_user.Utl_Insert_Chiave_Microfunz 'DELPREDIS', -- Codice                 
'Elimina Predisposto Interoperabilit Semplificata',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL --Forza_Disabilitazione , Myversione_Cd, RFU  
-- fine chiavi by Furnari per IS



execute @db_user.Utl_Insert_Chiave_Microfunz 'ELIMINA_TIPOLOGIA_DOC',
'Consente l''eliminazione di una tipologia di un documento.',
null,	'Y',
NULL, '3.21.1', NULL --Forza_Disabilitazione , Myversione_Cd, RFU  



execute @db_user.Utl_Insert_Chiave_Microfunz 'GEST_REG_MODIFICA',
'Abilita il pulsante Modifica nella pagina di gestione di un registro',
null,	'Y',
NULL, '3.21.1', NULL --Forza_Disabilitazione , Myversione_Cd, RFU  

end
GO

              
----------- FINE -
              
---- INSERT_DPA_CHIAVI_CONFIGURAZIONE.MSSQL.SQL  marcatore per ricerca ----
Begin

execute @db_user.Utl_Insert_Chiave_Config 'USE_TEXT_INDEX'
      , 'Chiave utilizzata per abilitare uso degli indici testuali su oggetto nelle ricerche documenti.'
      , '0', 'B', '1', '1', '1', '3.21.1', NULL, NULL, NULL


-- aggiornamento descrizione by Lorusso
execute @db_user.Utl_Insert_Chiave_Config 'FE_MAX_LENGTH_DESC_TRASM'
, 'Chiave utilizzata per impostazione del numero massimo di caratteri digitabili nelle note generali della Trasmissione (valori accettabili: da 0 a 250)'
, '250', 'F', '1', '1', '0', '3.21.1',	NULL, NULL, NULL 

execute @db_user.Utl_Insert_Chiave_Config 'FE_PROTOIN_ACQ_DOC_OBBLIGATORIA'
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1', '0', '3.21.1',
	NULL, NULL, NULL 

-- by Luciani 
execute @db_user.Utl_Insert_Chiave_Config 'BE_MAIL_PROVIDER'
, 'Mail Provider [c=chilkat; m=ms.net]'
, 'c', 'B', '1', '1', '1','3.21.1',
	NULL, NULL, NULL 

-- by Abbatangeli
 execute @db_user.Utl_Insert_Chiave_Config 'ENABLE_LOW_SECURITY'
 , 'Verifica i diritti solo sul titolario e non sui suoi nodi'
 , '0', 'B', '1', '1', '1'  ,'3.21.1',
	NULL, NULL, NULL 


-- aggiornamento descrizione by A. Marta
execute @db_user.Utl_Insert_Chiave_Config 'FE_ENABLE_PROT_RAPIDA_NO_UO' ,
    'Permette (valore chiave = true), nella protocollazione semplice, di protocollare senza aver scelto una UO a cui smistare o di aver selezionato un modello di trasmissione per la trasmissione rapida',
    'false',    'F',    '1',    '1',    '0' ,'3.21.1',
	NULL, NULL, NULL 

-- by Furnari
execute @db_user.Utl_Insert_Chiave_Config 'INTEROP_SERVICE_ACTIVE' ,
    'Stato di attivazione dell Interoperabilit Semplificata. 1 per attivare, 0 altrimenti',
    '0',    'B',    '1',    '1',    '1' ,'3.21.1',
	NULL, NULL, NULL    -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

execute @db_user.Utl_Insert_Chiave_Config 'ENABLED_NOTIFICATION_CENTER', 'Attivazione del centro notifiche'
, '0','B', '1', '1'  -- valore, Tipo_Chiave
,'1', '3.21.1'			--Globale, versioneCD
, NULL, NULL, NULL    -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

-- by Furnari
execute @db_user.Utl_Insert_Chiave_Config 'FILE_SERVICE_URL', 'Url del servizio di gestione file'
, 'n.d.','B', '1', '1'  -- valore, Tipo_Chiave
,'1', '3.21.1'			--Globale, versioneCD
, NULL, NULL, NULL    -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

-- by Furnari
execute @db_user.Utl_Insert_Chiave_Config 'INTEROP_SERVICE_URL', 'Url del servizio di interoperabilit semplificata'
, 'n.d.','B', '1', '1'  -- valore, Tipo_Chiave
--si pu mettere anche come chiave locale alla singola amministrazione
--, sovrascrivendo il valore della chiave globale
,'1', '3.21.1'			--Globale, versioneCD
, NULL, NULL, NULL    -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

-- by Frezza 
execute @db_user.Utl_Insert_Chiave_Config 'FE_GESTIONE_MATRICOLE', 'La chiave abilita o meno la gestione delle matricole utente'
, '0','F', '1', '1'          -- valore, Tipo_Chiave
,'1', '3.21.1' -- Globale, versioneCD
, NULL, NULL, NULL -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

-- by Frezza 
execute @db_user.Utl_Insert_Chiave_Config 'GESTIONE_QUALIFICHE'
, 'La chiave abilita o meno la gestione delle qualifiche utente'
, '0','B'          -- valore, Tipo_Chiave
, '1', '1','1' --Modificabile, Globale
, '3.21.1'
, NULL, NULL, NULL -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

	
-- by Buono-Alibranti
--FE_PAROLE_CHIAVI_AVANZATE  chiave che PUO' essere locale! 
execute @db_user.Utl_Insert_Chiave_Config 'FE_PAROLE_CHIAVI_AVANZATE', 'Abilita la nuova gestione delle Parole Chiave'
, '0','F' , '1', '1'         -- valore, Tipo_Chiave
,'0', '3.21.1' --Globale
, NULL, NULL, NULL -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU



-- FE_FASC_RAPIDA_REQUIRED
execute @db_user.Utl_Insert_Chiave_Config 'FE_FASC_RAPIDA_REQUIRED', 'Obbligatorieta della classificazione o fascicolazione rapida'
, 'false','F' , '1', '1'         -- valore, Tipo_Chiave
,'0', '3.21.1' --Globale
, NULL, NULL, NULL -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

end
GO


              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- 7.vw_utenti_in_organigramma.MSSQL.sql  marcatore per ricerca ----
IF EXISTS (
		SELECT * FROM dbo.sysobjects
        WHERE id = OBJECT_ID('[@db_user].[vw_utenti_in_organigramma]') 
			)
BEGIN
	DROP FUNCTION @db_user.[vw_utenti_in_organigramma]
END    

GO

CREATE FUNCTION @db_user.[vw_utenti_in_organigramma]  (@id_root_uo integer, @codice_qualifica_child varchar(64)) 
RETURNS TABLE AS RETURN
(
WITH Gerarchia (ID, PARENT, CODICE, DESCRIZIONE, CLASSIFICA_UO, LIVELLO)         as        (
        select
				cgUOParent.SYSTEM_ID AS ID,
				cgUOParent.ID_PARENT AS PARENT,
				cgUOParent.VAR_CODICE as CODICE,
				cgUOParent.VAR_DESC_CORR AS DESCRIZIONE,
				cgUOParent.CLASSIFICA_UO as CLASSIFICA_UO,
				cgUOParent.NUM_LIVELLO as LIVELLO
        from	@db_user.DPA_CORR_GLOBALI cgUOParent
        where	cgUOParent.SYSTEM_ID =  @id_root_uo 

        UNION ALL

        select	cgUOChild.SYSTEM_ID AS ID,
				cgUOChild.ID_PARENT as PARENT,
				cgUOChild.VAR_CODICE as CODICE,
				cgUOChild.VAR_DESC_CORR AS DESCRIZIONE,
				cgUOChild.CLASSIFICA_UO as CLASSIFICA_UO,
				cgUOChild.NUM_LIVELLO as LIVELLO
        from	@db_user.dpa_corr_globali cgUOChild
				inner join Gerarchia g on cgUOChild.ID_PARENT = g.ID
        where	cgUOChild.DTA_FINE IS NULL 
				and @db_user.HasQualifica(@codice_qualifica_child, cgUOChild.SYSTEM_ID) = 0 --'003'
        )
	select	distinct 
			p.SYSTEM_ID AS ID_UTENTE,
			p.USER_ID as USER_ID,
			p.VAR_COGNOME as COGNOME,
			p.VAR_NOME as NOME,
			p.FULL_NAME AS NOME_COMPLETO,
			p.MATRICOLA as MATRICOLA,
			cgR.CHA_RESPONSABILE AS RESPONSABILE
	from	Gerarchia g
			inner join @db_user.DPA_CORR_GLOBALI cgR on cgR.ID_UO = g.ID
			inner join @db_user.PEOPLEGROUPS pg		on pg.GROUPS_SYSTEM_ID = cgR.ID_GRUPPO
			inner join @db_user.PEOPLE p				on p.SYSTEM_ID = pg.PEOPLE_SYSTEM_ID
	where	cgR.DTA_FINE is null and pg.DTA_FINE is null					
)

go


              
----------- FINE -
              
---- 8.UtenteHasQualifica.MSSQL.sql  marcatore per ricerca ----

IF EXISTS (
		SELECT * FROM dbo.sysobjects
        WHERE id = OBJECT_ID('[@db_user].[UtenteHasQualifica]') 
			)
BEGIN
	DROP FUNCTION @db_user.[UtenteHasQualifica]
END    

GO

CREATE FUNCTION @db_user.[UtenteHasQualifica] 
(
	@codiceQualifica varchar(64),
	@idPeople int
) 
RETURNS int 
AS
BEGIN
declare @count int
declare @retValue int
	
if @codiceQualifica  is null 
BEGIN
	set @retValue =  0
	return @retValue
END ELSE BEGIN
	select @count = COUNT(pgq.SYSTEM_ID)
	from @db_user.DPA_PEOPLEGROUPS_QUALIFICHE pgq
	inner join @db_user.DPA_QUALIFICHE q on pgq.ID_QUALIFICA = q.SYSTEM_ID
	where q.CHA_COD = @codiceQualifica and pgq.ID_PEOPLE = @idPeople
END	

if (@count > 0)
BEGIN	set @retValue = 1
END else BEGIN
	set @retValue =  0
END
		
	return @retValue
END

GO

              
----------- FINE -
              
---- ALTER_getIfModelloAutorizzato.MSSQL.SQL  marcatore per ricerca ----
ALTER FUNCTION [@db_user].[getIfModelloAutorizzato] 

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

         -- con modalit nascondi versioni precedenti

        set @hide_versions = (select count(*) from dpa_modelli_mitt_dest where id_modello = @id_modellotrasm and hide_doc_versions = '1')

                  

 

         if (not @hide_versions is null and @hide_versions > 0)

            begin

                  set @idObject = @system_id

 

                   -- verifica se l'id fornito si riferisce ad un documento o fascicolo

                   set @isDocument = (select count(*) from profile p where p.system_id = @idObject)

 

                   if (@isDocument > 0)

                        begin

                                -- L'istanza su cui si sta applicando il modello  un documento,

                                -- verifica se sia consolidato

                              select @consolidationState = p.consolidation_state

                              from profile p

                              where p.system_id = @idObject

      

                              if (@consolidationState is null or @consolidationState = '0') 

                             begin

                                        -- Il modello prevede di nascondere le versioni di un documento precedenti a quella corrente

                                        -- al destinatario della trasmissione, ma in tal caso il documento non  stato ancora consolidato,

                                        -- pertanto il modello non pu essere utilizzato

                                        set @retval = 0

                             end

 

                              end

            end

 
if @retval = 1
BEGIN
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

 END

RETURN @RetVal

END
GO

              
----------- FINE -
              
---- ALTER_IsValidModelloTrasmissione.MSSQL.SQL  marcatore per ricerca ----
ALTER FUNCTION [@db_user].[IsValidModelloTrasmissione](

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

            and (cg.cha_disabled_trasm = '1' OR cg.DTA_FINE is NOT NULL)

      

      RETURN @retVal

END
GO

              
----------- FINE -
              
---- getseoggattivo.MSSQL.sql  marcatore per ricerca ----
IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[getseoggattivo]') 			)
	DROP FUNCTION [@db_user].getseoggattivo
GO

CREATE FUNCTION @db_user.getseoggattivo (@systemid int, @idtemplate int)
   RETURNS CHAR 
AS  
   
BEGIN
DECLARE @tmpvar  CHAR
DECLARE @cnt		INT

         SELECT @cnt = COUNT (*)
           FROM dpa_associazione_templates
          WHERE id_template = @idtemplate
            AND doc_number IS NULL
            AND id_oggetto = @systemid

            IF (@cnt > 0)
            BEGIN
               SET @tmpvar = '1'
            END ELSE BEGIN
               SET @tmpvar = '0'
            END 

      RETURN @tmpvar
      
END 
GO




              
----------- FINE -
              
---- GetValProfObjPrj.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[GetValProfObjPrj]') 			)
	DROP FUNCTION [@db_user].GetValProfObjPrj
GO

CREATE FUNCTION [@db_user].GetValProfObjPrj(@PrjId INT, @CustomObjectId INT)
RETURNS VARCHAR(255) 
AS 

BEGIN

	DECLARE @result VARCHAR(255)
	DECLARE @tipoOggetto varchar(255)
	DECLARE @tipoCont varchar(1)

	select @tipoOggetto = b.descrizione, @tipoCont = cha_tipo_Tar
	from dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 
	where a.system_id = @CustomObjectId
	and a.id_tipo_oggetto = b.system_id

	if (@tipoOggetto = 'Corrispondente') 
	BEGIN
		  select @result = cg.var_cod_rubrica + ' - ' + cg.var_DESC_CORR 
		  from dpa_CORR_globali cg where cg.SYSTEM_ID = (
		  select valore_oggetto_db 
		  from dpa_ass_templates_fasc 
		  where id_oggetto = @CustomObjectId 
			and id_project = @PrjId)
	end 
	else if (@tipoOggetto = 'CasellaDiSelezione') 
		BEGIN
			declare @item varchar(255)
			declare curCasellaDiSelezione CURSOR LOCAL FOR 
			select valore_oggetto_db 
				from dpa_ass_templates_fasc 
				where id_oggetto = @CustomObjectId and id_project = @PrjId
				and valore_oggetto_db is not null
			begin
				OPEN curCasellaDiSelezione
				FETCH NEXT FROM curCasellaDiSelezione INTO @item
				WHILE @@FETCH_STATUS = 0 
				BEGIN
					IF(@result IS NOT NULL) BEGIN
						SET @result = @result + '; ' +@item 
					end ELSE begin 
            			SET @result = @result + @item 
					END 
				FETCH NEXT FROM curCasellaDiSelezione INTO @item                
				END    
	            
				CLOSE curCasellaDiSelezione
				deallocate curCasellaDiSelezione
			END    
		end
	else if(@tipoOggetto = 'Contatore')     
		begin
			select @result = @db_user.getContatoreFasc(@PrjId,@tipoCont)  
			from dpa_ass_templates_fasc 
				where id_oggetto = @CustomObjectId 
				and id_project = @PrjId
		end
	else 
	--Tutti gli altri casi
		select @result = valore_oggetto_db 
		from dpa_ass_templates_fasc 
		where id_oggetto = @CustomObjectId and id_project = @PrjId
	
	RETURN @result

end 
GO

              
----------- FINE -
              
---- HasQualifica.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[HasQualifica]') 			)
	DROP FUNCTION [@db_user].HasQualifica
GO

CREATE FUNCTION [@db_user].[HasQualifica] 
(	@codiceQualifica varchar(64),	@idUO int) 
RETURNS int 
AS
BEGIN
declare @count int
declare @retValue int
	
if @codiceQualifica  is null 
BEGIN
	set @retValue =  0
	return @retValue
END ELSE BEGIN
	select @count = COUNT(pgq.SYSTEM_ID)
	from DPA_PEOPLEGROUPS_QUALIFICHE pgq
	inner join DPA_QUALIFICHE q on pgq.ID_QUALIFICA = q.SYSTEM_ID
		where q.CHA_COD = @codiceQualifica and pgq.ID_UO = @idUO
END	

if (@count > 0)
BEGIN	set @retValue = 1
END else BEGIN
	set @retValue =  0
END
		
	return @retValue
END
GO

              
----------- FINE -
              
---- vw_UO_in_organigramma.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (
		SELECT * FROM dbo.sysobjects
        WHERE id = OBJECT_ID('[@db_user].[vw_UO_in_organigramma]') 
			)
BEGIN
	DROP FUNCTION [@db_user].[vw_UO_in_organigramma]
END    

GO

CREATE FUNCTION [@db_user].[vw_UO_in_organigramma]  (@id_root_uo integer, @codice_qualifica_child varchar(64)) 
RETURNS TABLE AS RETURN
(
WITH Gerarchia (ID, PARENT, CODICE, DESCRIZIONE, CLASSIFICA_UO, LIVELLO)
        as
        (
        select
				cgUOParent.SYSTEM_ID AS ID,
				cgUOParent.ID_PARENT AS PARENT,
				cgUOParent.VAR_CODICE as CODICE,
				cgUOParent.VAR_DESC_CORR AS DESCRIZIONE,
				cgUOParent.CLASSIFICA_UO as CLASSIFICA_UO,
				cgUOParent.NUM_LIVELLO as LIVELLO
        from	dpa_corr_globali cgUOParent
        where	cgUOParent.SYSTEM_ID = @id_root_uo --16240

        UNION ALL

        select	cgUOChild.SYSTEM_ID AS ID,
				cgUOChild.ID_PARENT as PARENT,
				cgUOChild.VAR_CODICE as CODICE,
				cgUOChild.VAR_DESC_CORR AS DESCRIZIONE,
				cgUOChild.CLASSIFICA_UO as CLASSIFICA_UO,
				cgUOChild.NUM_LIVELLO as LIVELLO
        from	dpa_corr_globali cgUOChild
				inner join Gerarchia g on cgUOChild.ID_PARENT = g.ID
        where	cgUOChild.DTA_FINE IS NULL 
				and @db_user.HasQualifica(@codice_qualifica_child, cgUOChild.SYSTEM_ID) = 0 -- '003'
        )
	select	*
	from	Gerarchia g		
) 

GO

              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- 32.IS_SaveInteroperabilitySettings.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_SaveSettings
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_SaveSettings')
DROP PROCEDURE @db_user.IS_SaveSettings
go

create Procedure @db_user.IS_SaveSettings 
-- Id del registro / RF
@p_RegistryId INT, 
-- Id del ruolo da utilizzare per la creazione del predisposto
@p_RoleId INT, 
-- Id dell'utente da utilizzare per creazione del predisposto
@p_UserId INT, 
-- Flag (1, 0) indicante se per il registro / rf deve essere abilitata l'interoperabilit
@p_IsEnabledInteroperability INT,
-- Modalit di gestione (M per manuale, A per automatica)
@p_ManagementMode VARCHAR(4000),
-- Flag (1, 0) indicante se i documenti in ingresso devono essere manetenuti pendenti
@p_KeepPrivate INT  AS
Begin

/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     SaveInteroperabilitySettings

PURPOSE:  Store per il salvataggio delle impostazioni relative ad un registro

******************************************************************************/

-- Flag utilizzato per indicare se esistono gi delle impostazioni relative
-- al registro rf

-- Verifica se esisono gi delle impostazioni per per il registro / RF
   DECLARE @alreadyExists INT = 0
   Select   @alreadyExists = Count(*) From InteroperabilitySettings Where RegistryId = @p_RegistryId

-- Se non esistono impostazioni per il registro, viene creata una nuova tupla
-- nella tabella delle impostazioni altrimenti viene aggiornata quella esistente
   If(@alreadyExists = 0)
   Begin
 Insert
      Into InteroperabilitySettings(RegistryId,
RoleId,
UserId,
IsEnabledInteroperability,
ManagementMode,
KeepPrivate)
VALUES(@p_RegistryId,
@p_RoleId,
@p_UserId,
@p_IsEnabledInteroperability,
@p_ManagementMode,
@p_KeepPrivate)
   End
Else
   Begin
      update InteroperabilitySettings set IsEnabledInteroperability = @p_IsEnabledInteroperability,RoleId = @p_RoleId,
      UserId = @p_UserId,ManagementMode = @p_ManagementMode,KeepPrivate = @p_KeepPrivate  Where RegistryId = @p_RegistryId
   End

End  

GO

              
----------- FINE -
              
---- 33.IS_LoadInteroperabiiltySettings.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_LoadSettings
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_LoadSettings')
DROP PROCEDURE @db_user.IS_LoadSettings
go



create Procedure @db_user.IS_LoadSettings 
-- Id del registro / RF
@p_RegistryId INT, 
-- Id del ruolo da utilizzare per la creazione del predisposto
@p_RoleId INT OUTPUT , 
-- Id dell'utente da utilizzare per creazione del predisposto
@p_UserId INT OUTPUT , 
-- Flag (1, 0) indicante se per il registro / rf deve essere abilitata l'interoperabilit
@p_IsEnabledInteroperability INT OUTPUT ,
-- Flag (1, 0) indicante se i documenti ricevuti per IS devono essere mantenuti pendenti
@p_KeepPrivate INT OUTPUT ,
-- Modalit (M per manuale, A per automatica) per la gestione dei document ricevuti per interoperabilit
@p_ManagementMode VARCHAR(4000) OUTPUT  AS
Begin

/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     LoadInteroperabilitySettings

PURPOSE:  Store per il caricamento delle impostazioni relative ad un registro

******************************************************************************/

   Select   @p_RoleId = RoleId, 
   @p_UserId = UserId, 
   @p_IsEnabledInteroperability = IsEnabledInteroperability, 
   @p_KeepPrivate = KeepPrivate, 
   @p_ManagementMode = ManagementMode
   From InteroperabilitySettings
   Where RegistryId = @p_RegistryId
End 

GO

              
----------- FINE -
              
---- 34.IS_IsElementInteroperable.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_LoadReceivedPrivateFlag
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_IsElementInteroperable')
DROP PROCEDURE @db_user.IS_IsElementInteroperable
go


create Procedure @db_user.IS_IsElementInteroperable 
-- Id dell'oggetto per cui verificare se  interoperante
@p_ObjectId  INT,
-- Flag (1, 0) indicante se si sta verificando l'interoperabilit di un RF
@p_IsRf INT,
-- Flag (1,0) che indica se l'AOO collegata all'RF specificato  interoperante
@p_IsInteroperable INT OUTPUT  AS
Begin

/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     IsElementInteroperable

PURPOSE:  Store per la verifica dello stato di abilitazione di un elemento
(UO o RF) all'Interoperabilit Semplificata

******************************************************************************/


-- Valore estratto dalla tabella con le impostazioni sull'IS
   DECLARE @isInteroperable VARCHAR(1)
   
   -- Se  un RF, viene verificato se  interoperante la AOO collegata
   If @p_IsRf = 1
      Select   @isInteroperable = IsEnabledInteroperability
      From dpa_el_registri
      Left Join InteroperabilitySettings
      On id_aoo_collegata = RegistryId
      Where system_id = @p_ObjectId
Else
-- Altrimenti bisogna verificare se  abilitato all'interoperabilit l'AOO
-- selezionata come interoperante per la UO
   Select   @isInteroperable = IsEnabledInteroperability
   From dpa_corr_globali
   Left Join InteroperabilitySettings
   On InteropRegistryId = RegistryId
   Where system_id = @p_ObjectId
    

-- Se non  stato estratto alcun valore (magari perch non sono mai state
-- salvate informazioni sul registro legato all'RF specificato, l'RF non
--  interoperante
   if(@isInteroperable Is Null)
      SET @isInteroperable = 0

   SET @p_IsInteroperable = @isInteroperable
End 

GO

              
----------- FINE -
              
---- 61.IS_LoadReceivedPrivateFlag.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_LoadReceivedPrivateFlag
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_LoadReceivedPrivateFlag')
DROP PROCEDURE @db_user.IS_LoadReceivedPrivateFlag
go

Create Procedure @db_user.IS_LoadReceivedPrivateFlag 
@p_ProfileId INT,
@p_ReceivedPrivate INT OUTPUT  AS
Begin

   Select   @p_ReceivedPrivate = ReceivedPrivate From SimpInteropReceivedMessage Where ProfileId = @p_ProfileId

End 

GO
              
----------- FINE -
              
---- 72.IS_InsertDataInReceivedMessage.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_InsertDataInReceivedMsg
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_InsertDataInReceivedMsg')
DROP PROCEDURE @db_user.IS_InsertDataInReceivedMsg
go

create Procedure @db_user.IS_InsertDataInReceivedMsg @p_MessageId VARCHAR(4000),
@p_ReceivedPrivate INT,
@p_Subject VARCHAR(4000),
@p_SenderDescription VARCHAR(4000),
@p_SenderUrl VARCHAR(4000),
@p_SenderAdministrationCode VARCHAR(4000), 
@p_AOOCode VARCHAR(4000), 
@p_RecordNumber INT, 
@p_RecordDate DATETIME , 
@p_ReceiverCode VARCHAR(4000)
AS
Begin
-- Inserimento informazioni sul messaggio ricevuto
 Insert Into SimpInteropReceivedMessage(MessageId,
	ReceivedPrivate,
	ReceivedDate,
	Subject,
	SenderDescription,
	SenderUrl,
	SenderAdministrationCode,
	AOOCode,
	RecordNumber,
	RecordDate, 
	ReceiverCode)
VALUES(@p_MessageId,
	@p_ReceivedPrivate,
	GetDate(),
	@p_Subject,
	@p_SenderDescription,
	@p_SenderUrl,
	@p_SenderAdministrationCode,
	@p_AOOCode,
	@p_RecordNumber,
	@p_RecordDate,
	@p_ReceiverCode) 

End 
GO

              
----------- FINE -
              
---- 73.IS_InsertDataInSimpInteropLog.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_InsertDataInSimpInteropLog
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_InsertDataInSimpInteropLog')
DROP PROCEDURE @db_user.IS_InsertDataInSimpInteropLog
go


create Procedure @db_user.IS_InsertDataInSimpInteropLog 
@p_ProfileId INT,
@p_ErrorMessage INT,
@p_Text VARCHAR(4000)  AS
Begin
-- Inserimento voce di log
 Insert Into SimpInteropDbLog(ProfileId,
ErrorMessage,Text)
VALUES(@p_ProfileId,
@p_ErrorMessage,
@p_Text)


End 

GO

              
----------- FINE -
              
---- 74.IS_SetIdProfForSimpInteropMessage.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_SetIdProfForSimpInteropMsg
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_SetIdProfForSimpInteropMsg')
DROP PROCEDURE @db_user.IS_SetIdProfForSimpInteropMsg
go

create Procedure @db_user.IS_SetIdProfForSimpInteropMsg 
@p_ProfileId INT,
@p_MessageId VARCHAR(4000)  AS
Begin
-- Aggiornamento del campo ProfileId dove compare il MessageId passato per parametro
   update SimpInteropReceivedMessage set ProfileId = @p_ProfileId  
   Where MessageId = @p_MessageId


End 

GO              
----------- FINE -
              
---- 81.IS_LoadSimpInteropRecordInfo.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP IS_LoadSimpInteropRecordInfo
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='IS_LoadSimpInteropRecordInfo')
DROP PROCEDURE @db_user.IS_LoadSimpInteropRecordInfo
go


create Procedure @db_user.IS_LoadSimpInteropRecordInfo 
@p_DocumentId INT,
@p_SenderAdministrationCode VARCHAR(4000) OUTPUT ,
@p_SenderAOOCode VARCHAR(4000) OUTPUT ,
@p_SenderRecordNumber INT OUTPUT ,
@p_SenderRecordDate DATETIME OUTPUT ,
@p_ReceiverAdministrationCode VARCHAR(4000) OUTPUT ,
@p_ReceiverAOOCode VARCHAR(4000) OUTPUT ,
@p_ReceiverRecordNumber INT OUTPUT ,
@p_ReceiverRecordDate DATETIME OUTPUT ,
@p_SenderUrl VARCHAR(4000) OUTPUT ,
-- new parameter added lately  
 @p_ReceiverCode VARCHAR(4000) OUTPUT AS
Begin
-- Caricamento delle informazioni sul protocollo mittente
   Select   @p_SenderAdministrationCode = SenderAdministrationCode
   , @p_SenderAOOCode = AOOCode
   , @p_SenderRecordNumber = RecordNumber
   , @p_SenderRecordDate = RecordDate
   , @p_SenderUrl = SenderUrl
   , @p_ReceiverCode = ReceiverCode
   From SimpInteropReceivedMessage
   Where ProfileId = @p_DocumentId

-- Caricamento informazioni sul protocollo creato nell'amministrazione destinataria
   Select   @p_ReceiverAOOCode =(Select var_codice From dpa_el_registri Where System_Id = Id_Registro), @p_ReceiverRecordNumber = Num_Proto, @p_ReceiverRecordDate = Dta_proto, @p_ReceiverAdministrationCode =(Select var_codice_amm From dpa_amministra Where system_id =(Select id_amm From dpa_corr_globali Where system_id = Id_Uo_Prot))
   From profile
   Where System_id = @p_DocumentId

End 

GO


              
----------- FINE -
              
---- 82.Replace_spsetdatavista_tv.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP spsetdatavista_tv
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='spsetdatavista_tv')
DROP PROCEDURE @db_user.spsetdatavista_tv
go




create PROCEDURE @db_user.spsetdatavista_tv 
@p_idpeople      INT       ,
@p_idoggetto     INT       ,
@p_idgruppo      INT       ,
@p_tipooggetto   CHAR(2000)       ,
@p_iddelegato    INT       ,
@p_resultvalue   INT OUTPUT 
AS
/*
----------------------------------------------------------------------------------------

RICHIAMATA SOLO DAL TASTO VISTO, agisce solo sulle trasmissioni NO WKFL. TOGLIENDOLE DALLA TDL 
NON SETTA DATA VISTA PERCH LO FA la SP_SET_DATAVISTA_V2


dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
BEGIN
   DECLARE @p_cha_tipo_trasm   CHAR(1) = NULL
   DECLARE @p_chatipodest      INT
   DECLARE @error INT
   SET @p_resultvalue = 0

   BEGIN
      DECLARE @cursortrasmsingoladocumento CURSOR
      DECLARE @cursortrasmsingolafascicolo CURSOR
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest VARCHAR(255)
      IF (@p_tipooggetto = 'D')
      begin
         SET @cursortrasmsingoladocumento = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @p_idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @p_idpeople))
         AND a.id_profile = @p_idoggetto
         AND b.id_ragione = c.system_id
         OPEN @cursortrasmsingoladocumento
         FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               OR @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,cha_in_todolist = '0'  WHERE
                     id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END


/* BEGIN
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola = currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
AND id_profile = p_idoggetto;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END; */

                  BEGIN
                     IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest = 'R')
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (@p_iddelegato = 0)
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE
                           id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_vista_delegato = '1',id_people_delegato =
                           @p_iddelegato,cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE
                           id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- la ragione di trasmissione prevede workflow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata

                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND id_people_dest = @p_idpeople
                        AND id_profile = @p_idoggetto
                        SELECT   @error = @@ERROR
                     end
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingoladocumento
      end

      IF (@p_tipooggetto = 'F')
      begin
         SET @cursortrasmsingolafascicolo = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @p_idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @p_idpeople))
         AND a.id_project = @p_idoggetto
         AND b.id_ragione = c.system_id
         OPEN @cursortrasmsingolafascicolo
         FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--caso in cui si sta esercitando una delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

/*  BEGIN
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola = currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
AND id_project = p_idoggetto;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END; */

                  BEGIN
                     IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest = 'R')
                        IF (@p_iddelegato = 0)
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE 
                           id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_vista_delegato = '1',id_people_delegato =
                           @p_iddelegato,cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE 
                           id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- se la ragione di trasmissione prevede workflow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL and
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
-- caso in cui si sta esercitando una delega
                  BEGIN
 
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato  WHERE
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND id_people_dest = @p_idpeople
                        AND id_project = @p_idoggetto
                        SELECT   @error = @@ERROR
                     end
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingolafascicolo
      end

   END
END 

GO
              
----------- FINE -
              
---- 83.Replace_spsetdatavista_v2.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP sp_modify_corr_esterno
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='spsetdatavista_v2')
DROP PROCEDURE @db_user.spsetdatavista_v2
go



create PROCEDURE @db_user.spsetdatavista_v2 
@p_idpeople      INT       ,
@p_idoggetto     INT       ,
@p_idgruppo      INT       ,
@p_tipooggetto   CHAR(2000)       ,
@p_iddelegato    INT       ,
@p_resultvalue   INT OUTPUT 
AS
/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
BEGIN
   DECLARE @p_cha_tipo_trasm   CHAR(1) = NULL
   DECLARE @p_chatipodest      INT
   DECLARE @error INT
   SET @p_resultvalue = 0

   BEGIN
      DECLARE @cursortrasmsingoladocumento CURSOR
      DECLARE @cursortrasmsingolafascicolo CURSOR
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest VARCHAR(255)
      IF (@p_tipooggetto = 'D')
      begin
         SET @cursortrasmsingoladocumento = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @p_idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @p_idpeople))
         AND a.id_profile = @p_idoggetto
         AND b.id_ragione = c.system_id
         OPEN @cursortrasmsingoladocumento
         FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               Or @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  
                     --  dpa_trasm_utente.cha_in_todolist = '0'
                     WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  
                     -- dpa_trasm_utente.cha_in_todolist = '0'
                     WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
                     update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                     AND id_people_dest = @p_idpeople
                     AND id_profile = @p_idoggetto
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
                  BEGIN
                     IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest = 'R')
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (@p_iddelegato = 0)
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

                           update dpa_trasm_utente  set cha_vista = '1' FROM dpa_trasm_utente dpa_trasm_utente WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           --  dpa_trasm_utente.cha_in_todolist = '0'
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

                           update dpa_trasm_utente  set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                           @p_iddelegato FROM dpa_trasm_utente dpa_trasm_utente WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- la ragione di trasmissione prevede workflow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata


--AND dpa_trasm_utente.id_people = p_idpeople;
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND id_people_dest = @p_idpeople
                        AND id_profile = @p_idoggetto
                        SELECT   @error = @@ERROR
                     end
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingoladocumento
      end

      IF (@p_tipooggetto = 'F')
      begin
         SET @cursortrasmsingolafascicolo = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @p_idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @p_idpeople))
         AND a.id_project = @p_idoggetto
         AND b.id_ragione = c.system_id
         OPEN @cursortrasmsingolafascicolo
         FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  
                     --dpa_trasm_utente.cha_in_todolist = '0'
                     WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--caso in cui si sta esercitando una delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  
                     -- dpa_trasm_utente.cha_in_todolist = '0'
                     WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
                     update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                     AND id_people_dest = @p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                     AND id_project = @p_idoggetto
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
                  BEGIN
                     IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest = 'R')
                        IF (@p_iddelegato = 0)
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_vista = '1' FROM dpa_trasm_utente dpa_trasm_utente 
                           -- dpa_trasm_utente.cha_in_todolist = '0'
                           WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                           @p_iddelegato FROM dpa_trasm_utente dpa_trasm_utente 
                           -- dpa_trasm_utente.cha_in_todolist = '0'
                           WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @p_idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @p_idpeople)
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @p_resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- se la ragione di trasmissione prevede workflow
               begin
                  IF (@p_iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
-- caso in cui si sta esercitando una delega
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @p_iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     AND dpa_trasm_utente.id_people = @p_idpeople
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND id_people_dest = @p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = @p_idoggetto
                        SELECT   @error = @@ERROR
                     end
                     IF (@error <> 0)
                     begin
                        SET @p_resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingolafascicolo
      end

   END
END 

GO

              
----------- FINE -
              
---- execute_only_ONCE_INITPEC3.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP INITPEC3
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='INITPEC3')
DROP PROCEDURE @db_user.INITPEC3
go

/*
Questa SP inizializza le tabelle coinvolte nello sviluppo PEC3 by C. Ferlito 
Eseguire SOLO una volta, con il comando:

EXECUTE INITPEC3;  
se non torna errori, l'esecuzione  avvenuta correttamente
*/

/* correzione di Samuele messa da Sab */

CREATE PROCEDURE [@db_user].[INITPEC3]  
AS
	Declare
	@cntinteropinterna      As int,
	@system_id              As int,
	@var_email_registro     As VARCHAR(128),
	@var_user_mail          As VARCHAR(128),
	@var_pwd_mail           VARCHAR(64),
	@var_server_smtp        VARCHAR(64),
	@num_porta_smtp         int,
	@var_server_pop         VARCHAR(64),
	@num_porta_pop          int,
	@var_user_smtp          VARCHAR(128),
	@var_pwd_smtp           VARCHAR(128),
	@cha_smtp_ssl           VARCHAR(1),
	@cha_pop_ssl            VARCHAR(1),
	@cha_smtp_sta           VARCHAR(1),
	@var_server_imap        VARCHAR(128),
	@num_porta_imap         int,
	@var_tipo_connessione   VARCHAR(10),
	@var_inbox_imap         VARCHAR(128),
	@var_box_mail_elaborate VARCHAR(50),
	@var_mail_non_elaborate VARCHAR(50),
	@cha_imap_ssl           VARCHAR(1),
	@cha_ricevuta_pec       VARCHAR(2),
	@var_solo_mail_pec      VARCHAR(1),
	@cha_consulta           VARCHAR(1),
	@cha_spedisci           VARCHAR(1),
	@cha_notifica           VARCHAR(1),
	@system_id_reg          int,
	@id_ruolo               int,
	@id_amm                 int,
	@email                  VARCHAR(128),
	@cha_rf					VARCHAR(1),
	--per dpa_ass_doc_mail_interop
	@id_profile				int,
	@var_email				VARCHAR(128),
	@id_reg					int,
	--per dpa_mail_corr_esterni
	@id_corr_esterno        int,
	@var_email_corr_esterno VARCHAR(128),
	@INFO_MAIL_REG			Cursor,
	@INFO_VIS_REG			Cursor,
	@ASS_DOC_MAIL			Cursor,
	@MAIL_CORR_ESTERNI		Cursor

BEGIN
	--cursore per popolare DPA_MAIL_REGISTRI
	Set @INFO_MAIL_REG = Cursor For
	SELECT	SYSTEM_ID ,
			VAR_EMAIL_REGISTRO ,
			VAR_USER_MAIL ,
			VAR_PWD_MAIL ,
			VAR_SERVER_SMTP ,
			NUM_PORTA_SMTP ,
			VAR_SERVER_POP ,
			NUM_PORTA_POP ,
			VAR_USER_SMTP ,
			VAR_PWD_SMTP ,
			CHA_SMTP_SSL ,
			CHA_POP_SSL ,
			CHA_SMTP_STA ,
			VAR_SERVER_IMAP ,
			NUM_PORTA_IMAP ,
			VAR_TIPO_CONNESSIONE ,
			VAR_INBOX_IMAP ,
			VAR_BOX_MAIL_ELABORATE ,
			VAR_MAIL_NON_ELABORATE ,
			CHA_IMAP_SSL ,
			CHA_RICEVUTA_PEC ,
			VAR_SOLO_MAIL_PEC
	FROM DPA_EL_REGISTRI
	WHERE VAR_EMAIL_REGISTRO IS NOT NULL
	UNION
	SELECT	SYSTEM_ID ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			0 ,
			NULL ,
			0 ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			0 ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			NULL ,
			NULL
	FROM DPA_EL_REGISTRI
	
	-- cursore per popolare DPA_VIS_MAIL_REGISTRI
	Set @INFO_VIS_REG = Cursor For
		SELECT	rr.id_registro ,
				rr.id_ruolo_in_uo ,
				el.id_amm ,
				el.var_email_registro ,
				el.cha_rf
		FROM DPA_L_RUOLO_REG rr, DPA_EL_REGISTRI el
		WHERE	rr.id_registro       = el.system_id
				AND el.var_email_registro IS NOT NULL
	
	--cursore per popolare DPA_ASS_DOC_MAIL_INTEROP
	Set @ASS_DOC_MAIL = Cursor For
		SELECT	p.docnumber,
				r.var_email_registro,
				r.system_id
		FROM	profile p,
				dpa_doc_arrivo_par d,
				dpa_corr_globali c,
				dpa_el_registri r
		WHERE	p.cha_interop      = 'S'
				AND p.docnumber          = d.id_profile
				AND d.cha_tipo_mitt_dest = 'M'
				AND d.id_mitt_dest       = c.system_id
				AND c.id_registro        = r.system_id
	--cursore per DPA_MAIL_CORR_ESTERNI
	Set @MAIL_CORR_ESTERNI = Cursor For
		SELECT	system_id,
				var_email
		FROM	DPA_CORR_GLOBALI
		WHERE	CHA_TIPO_IE = 'E'
				AND var_email    IS NOT NULL
				AND var_email    <> '';

	-- Popola la DPA_MAIL_REGISTRI
	OPEN @INFO_MAIL_REG
	FETCH @INFO_MAIL_REG
    INTO	@system_id ,
			@var_email_registro ,
			@var_user_mail ,
			@var_pwd_mail ,
			@var_server_smtp ,
			@num_porta_smtp ,
			@var_server_pop ,
			@num_porta_pop ,
			@var_user_smtp ,
			@var_pwd_smtp ,
			@cha_smtp_ssl ,
			@cha_pop_ssl ,
			@cha_smtp_sta ,
			@var_server_imap ,
			@num_porta_imap ,
			@var_tipo_connessione ,
			@var_inbox_imap ,
			@var_box_mail_elaborate ,
			@var_mail_non_elaborate ,
			@cha_imap_ssl ,
			@cha_ricevuta_pec ,
			@var_solo_mail_pec
    while @@FETCH_STATUS = 0
		Begin
			INSERT
			INTO DPA_MAIL_REGISTRI VALUES
			  (
				@system_id ,
				'1' ,
				@var_email_registro ,
				@var_user_mail ,
				@var_pwd_mail ,
				@var_server_smtp ,
				@cha_smtp_ssl ,
				@cha_pop_ssl ,
				@num_porta_smtp ,
				@cha_smtp_sta ,
				@var_server_pop ,
				@num_porta_pop ,
				@var_user_smtp ,
				@var_pwd_smtp ,
				@var_inbox_imap ,
				@var_server_imap ,
				@num_porta_imap ,
				@var_tipo_connessione ,
				@var_box_mail_elaborate ,
				@var_mail_non_elaborate ,
				@cha_imap_ssl ,
				@var_solo_mail_pec ,
				@cha_ricevuta_pec,
				''
			  )
		  
			FETCH @INFO_MAIL_REG
			INTO	@system_id ,
					@var_email_registro ,
					@var_user_mail ,
					@var_pwd_mail ,
					@var_server_smtp ,
					@num_porta_smtp ,
					@var_server_pop ,
					@num_porta_pop ,
					@var_user_smtp ,
					@var_pwd_smtp ,
					@cha_smtp_ssl ,
					@cha_pop_ssl ,
					@cha_smtp_sta ,
					@var_server_imap ,
					@num_porta_imap ,
					@var_tipo_connessione ,
					@var_inbox_imap ,
					@var_box_mail_elaborate ,
					@var_mail_non_elaborate ,
					@cha_imap_ssl ,
					@cha_ricevuta_pec ,
					@var_solo_mail_pec
		End
	CLOSE @INFO_MAIL_REG
	
	Print('finito insert DPA_MAIL_REGISTRI ')
	
	--Popolo la DPA_VIS_MAIL_REGISTRI
	OPEN @INFO_VIS_REG
	FETCH @INFO_VIS_REG INTO @system_id_reg , @id_ruolo , @id_amm , @email , @cha_rf
	While @@FETCH_STATUS = 0
		Begin
			Set @cha_consulta = '0'
			Set	@cha_spedisci = '0'
			Set @cha_notifica = '0'
    
			--aggiorna flag notifica per fegistro/rf
			IF @cha_rf = '1' 
			  SELECT @cha_notifica = COUNT(*)
			  FROM dpa_tipo_f_ruolo fr,
				dpa_tipo_funzione tf
			  WHERE fr.id_ruolo_in_uo    = @id_ruolo
			  AND fr.id_tipo_funz        = tf.system_id
			  AND UPPER(tf.var_cod_tipo) = 'PRAU_RF'
			ELSE
			  SELECT @cha_notifica = COUNT(*)
			  FROM dpa_tipo_f_ruolo fr,
				dpa_tipo_funzione tf
			  WHERE fr.id_ruolo_in_uo    = @id_ruolo
			  AND Fr.Id_Tipo_Funz        = Tf.System_Id
			  AND UPPER(tf.var_cod_tipo) = 'PRAU'
			
			-- chiude
			IF @cha_notifica > 1 
			  Set @cha_notifica = 1
	    
			--aggiorna flag spedisci per fegistro/rf
			SELECT @cha_spedisci = COUNT(*)
			FROM dpa_tipo_f_ruolo fr,
			  dpa_tipo_funzione tf,
			  dpa_funzioni f
			WHERE fr.id_ruolo_in_uo = @id_ruolo
			AND fr.id_tipo_funz     = tf.system_id
			AND tf.system_id        = f.id_tipo_funzione
			AND f.cod_funzione      = 'DO_OUT_SPEDISCI'
	    
			IF @cha_spedisci         > 1 
			  Set @cha_spedisci      = 1
			
			--aggiorna flag consulta per fegistro/rf
			IF @cha_rf = '1'
			  SELECT @cha_consulta = COUNT(*)
			  FROM dpa_tipo_f_ruolo fr,
				dpa_tipo_funzione tf,
				dpa_funzioni f
			  WHERE fr.id_ruolo_in_uo = @id_ruolo
			  AND fr.id_tipo_funz     = tf.system_id
			  AND tf.system_id        = f.id_tipo_funzione
			  AND f.cod_funzione      = 'GEST_CASELLA_IST_RF'
			ELSE
			  SELECT @cha_consulta = COUNT(*)
			  FROM dpa_tipo_f_ruolo fr,
				dpa_tipo_funzione tf,
				dpa_funzioni f
			  WHERE fr.id_ruolo_in_uo = @id_ruolo
			  AND fr.id_tipo_funz     = tf.system_id
			  AND tf.system_id        = f.id_tipo_funzione
			  AND f.cod_funzione      = 'GEST_CASELLA_IST'
	    
			IF @cha_consulta > 1 
				Set @cha_consulta = 1
			
			INSERT
			INTO DPA_VIS_MAIL_REGISTRI VALUES
			  (
				@system_id_reg ,
				@id_ruolo ,
				@email ,
				@cha_consulta ,
				@cha_notifica,
				@cha_spedisci
			  )
			
			SELECT @cntinteropinterna = COUNT(*)
			FROM DPA_VIS_MAIL_REGISTRI
			WHERE ID_REGISTRO       = @system_id_reg
			AND ID_RUOLO_IN_UO      = @id_ruolo
			AND VAR_EMAIL_REGISTRO IS NOT NULL 
			
			IF @cntinteropinterna    = 1 
				Begin
					IF @system_id_reg      = 1110 
						Print('cntinteropinterna =1  Id_Ruolo:' + @Id_Ruolo)
					INSERT
					INTO Dpa_Vis_Mail_Registri VALUES
					(
					  @system_id_reg ,
					  @id_ruolo ,
					  '' ,-- 4° campo mail
					  @cha_consulta ,
					  @cha_notifica ,
					  @cha_spedisci
					)
				End
				
			FETCH @INFO_VIS_REG INTO @system_id_reg , @id_ruolo , @id_amm , @email , @cha_rf
		End
		CLOSE @Info_Vis_Reg
  
		Print('finito insert DPA_VIS_MAIL_REGISTRI ')
		OPEN @ASS_DOC_MAIL
		FETCH @ASS_DOC_MAIL INTO @id_profile, @var_email, @id_reg
		While @@FETCH_STATUS = 0
			Begin
				INSERT
				INTO DPA_ASS_DOC_MAIL_INTEROP VALUES
				  (
					@id_profile ,
					@id_reg ,
					@var_email
				  )
				FETCH @ASS_DOC_MAIL INTO @id_profile, @var_email, @id_reg
			End
  
		CLOSE @Ass_Doc_Mail
		Print('finito insert DPA_ASS_DOC_MAIL_INTEROP ')
		OPEN @MAIL_CORR_ESTERNI
		FETCH @MAIL_CORR_ESTERNI INTO @id_corr_esterno, @var_email_corr_esterno
		
		While @@FETCH_STATUS = 0
			Begin
				INSERT
				INTO DPA_MAIL_CORR_ESTERNI VALUES
				  (
					@id_corr_esterno ,
					@var_email_corr_esterno,
					'1',
					''
				  )
				FETCH @MAIL_CORR_ESTERNI INTO @id_corr_esterno, @var_email_corr_esterno
			End
		CLOSE @MAIL_CORR_ESTERNI
		Print('finito insert DPA_MAIL_CORR_ESTERNI ')
	Print('fatto  commit ');
End
GO

              
----------- FINE -
              
---- package_NotificationCenter.MSSQL.sql  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadChannelsRelatedToItem')
DROP PROCEDURE @db_user.[NC_LoadChannelsRelatedToItem]
go

create PROCEDURE [@db_user].[NC_LoadChannelsRelatedToItem] @p_itemId INT  AS
Begin
   execute('Select * From NotificationChannel Where Exists (Select ''x'' From NotificationItemCategories Where NotificationChannel.Id = NotificationItemCategories.CategoryId And NotificationItemCategories.ItemId = ' + @p_itemId +')')

End
GO
 
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadChannelsRelatedToInstance')
DROP PROCEDURE @db_user.[NC_LoadChannelsRelatedToInstance]
go


create PROCEDURE [@db_user].[NC_LoadChannelsRelatedToInstance] 
	@p_InstanceId INT  AS
Begin

   execute('Select * From NotificationChannel Where Exists (Select ''x'' From NotificationInstanceChannels Where InstanceId = ' + @p_InstanceId + ')')
End
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchItemByMessageIdRange')
DROP PROCEDURE @db_user.[NC_SearchItemByMessageIdRange]
go


create PROCEDURE [@db_user].[NC_SearchItemByMessageIdRange] @p_LowMessageId INT,  @p_HightMessageId INT, @p_InstanceId Integer, @p_UserId Integer  AS
Begin
   declare @ExecStr varchar(500)
   SET @ExecStr = 'Select * From NotificationItem 
		  Inner Join NotificationItemCategories
		  On Id = ItemId
		  Inner Join NotificationInstanceChannels
		  On ChannelId = CategoryId
		  Where MessageId >= ' + convert(varchar(255),@p_LowMessageId) + 
		  ' And MessageId <= ' + convert(varchar(255),@p_HightMessageId) + 
		  ' And InstanceId = ' + convert(varchar(255), @p_InstanceId) +
		  ' And Exists (Select ''x'' From NotificationUser nu Where nu.UserId = ' + CONVERT(varchar, @p_UserId) +
		  ' And nu.InstanceId = ' + CONVERT(varchar, @p_InstanceId) + ')'
   execute(@ExecStr)

End
GO

IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchItemByDateRange')
DROP PROCEDURE @db_user.[NC_SearchItemByDateRange]
go


create PROCEDURE [@db_user].[NC_SearchItemByDateRange] @p_LowDate DateTime, @p_HightDate DateTime, @p_InstanceId Integer, @p_UserId Integer AS
Begin
   declare @ExecStr varchar(500)
   SET @ExecStr = '	Select * 
					From NotificationItem 
					Inner Join NotificationItemCategories 
					On Id = ItemId 
					Inner Join NotificationInstanceChannels 
					On ChannelId = CategoryId 
					Where PublishDate >= convert(datetime, ''' + convert(varchar, @p_LowDate, 101) + ' 00:00:00'')
					And PublishDate <= convert(datetime, ''' + convert(varchar, @p_HightDate, 101) + ' 23:59:59'')
					And InstanceId = ' + convert(varchar, @p_InstanceId) + ' And Exists 
					(Select ''x'' From NotificationUser nu Where nu.UserId = ' + CONVERT(varchar, @p_UserId) + ' And nu.InstanceId = ' +
					CONVERT(varchar, @p_InstanceId) + ')'
   
   print @ExecStr
   
   execute(@ExecStr)
End
GO

IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchItemByChannelId')
DROP PROCEDURE @db_user.NC_SearchItemByChannelId
go


create PROCEDURE [@db_user].[NC_SearchItemByChannelId] @p_ChannelId Integer, @p_InstanceId Integer, @p_UserId Integer AS
Begin
   execute('Select *
            From NotificationItem ni
            Where Exists
              (Select ''x''
              From NotificationItemCategories nic
              Where nic.CategoryId = ' + @p_ChannelId + 
              ' And nic.ItemId       = ni.Id
              And Exists
                (Select ''x''
                From NotificationInstanceChannels nic1
                Where nic1.InstanceId  = ' + @p_InstanceId +
                ' And  nic1.ChannelId = nic.CategoryId And Exists
					(Select ''x''
					From NotificationUser nu
					Where nu.UserId = ' + @p_userId + ' And nu.InstanceId = ' + @p_InstanceId + 
                '))
              )')
End
GO

-- =============================================
-- Author:		Samuele Furnari
-- Description:	Ricerca di item con filtri su data, id messaggio, testo contenuto nella notifica
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchItem')
DROP PROCEDURE @db_user.NC_SearchItem
go


create PROCEDURE [@db_user].[NC_SearchItem]
	@p_UserId Integer, @p_SearchForMessageNumber Integer, @p_LowMessageNumber Integer, @p_HightMessageNumber Integer, 
    @p_SearchForDate Integer, @p_LowDate DateTime, @p_HightDate DateTime, 
    @p_SearchForTitle Integer, @p_ItemText NVarChar(2000), @p_InstanceId Integer
AS
BEGIN
	-- Query da eseguire per effettuare la ricerca 
    Declare @queryToExecute NVarChar (4000) = 'Select ni.* From NotificationItem ni Inner Join NotificationUser nu On ni.Id = nu.ItemId Where nu.UserId = ' + Convert(varchar, @p_UserId) + ' And nu.InstanceId = ' + Convert(varchar, @p_InstanceId)
    
    -- Inserimento condizione sull'id del messaggio
    If @p_SearchForMessageNumber = 1
	  Begin
      Set @queryToExecute = @queryToExecute + ' And MessageNumber >= ' + Convert(varchar, @p_LowMessageNumber)
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If @p_HightMessageNumber >= @p_LowMessageNumber
        Set @queryToExecute = @queryToExecute + ' And MessageNumber <= ' + CONVERT(varchar, @p_HightMessageNumber)
      End
      
    -- Inserimento della condizione per data
    If @p_SearchForDate = 1
	  Begin
      Set @queryToExecute = @queryToExecute + ' And PublishDate >= convert(datetime, ''' + convert(varchar, @p_LowDate, 101) + ' 00:00:00'')'
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If @p_HightDate >= @p_LowDate
        Set @queryToExecute = @queryToExecute + ' And PublishDate <= convert(datetime, ''' + convert(varchar, @p_HightDate, 101) +  ' 23:59:59'')'
      End
      
    
    -- Inserimento della condizione sul contenuto dell'item
    If @p_SearchForTitle = 1
      Set @queryToExecute = @queryToExecute + ' And Upper(Title) Like Upper(''%' + @p_ItemText + '%'')'
    
    Execute(@queryToExecute)
    
END
GO

-- =============================================
-- Author:		Samuele Furnari
-- Description:	Procedura per il recupero dei dati di una istanza a partire dal nome
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchInstancesByName')
DROP PROCEDURE @db_user.NC_SearchInstancesByName
go


create PROCEDURE [@db_user].[NC_SearchInstancesByName] 
	@p_Name NVarChar(4000)
AS
BEGIN
	execute('Select * From NotificationInstance Where Upper(Description) = Upper(''' + @p_Name + ''')')
END
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadItem')
DROP PROCEDURE @db_user.NC_LoadItem
go

create PROCEDURE [@db_user].[NC_LoadItem] @p_itemId INT, @p_Author NVARCHAR(2000) OUTPUT , @p_Title NVARCHAR(2000) OUTPUT , @p_Text NVARCHAR(2000) OUTPUT , @p_LastUpdate DATETIME OUTPUT , @p_PublishDate DATETIME OUTPUT , @p_MessageId INT OUTPUT, @p_MessageNumber Int Out  AS
Begin
   SELECT   @p_Author = AUTHOR, @p_Title = TITLE, @p_Text = TEXT, @p_LastUpdate = LASTUPDATE, @p_PublishDate = PUBLISHDATE, @p_MessageId = MESSAGEID, @p_MessageNumber = MessageNumber
   FROM NOTIFICATIONITEM 

End
GO



IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadInstances')
DROP PROCEDURE @db_user.NC_LoadInstances
go

create PROCEDURE [@db_user].[NC_LoadInstances] AS
Begin
    Select * From NotificationInstance
End
GO



IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadChannels')
DROP PROCEDURE @db_user.NC_LoadChannels
go

create PROCEDURE [@db_user].[NC_LoadChannels]   AS
Begin
   Select * From NotificationChannel

End
GO

-- =============================================
-- Author:		Samuele Furnari
-- Description:	Procedura per il caricamento dei dati di un canale a partire dalla sua etichetta
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_LoadChannelByLabel')
DROP PROCEDURE @db_user.NC_LoadChannelByLabel
go

create PROCEDURE [@db_user].[NC_LoadChannelByLabel] 
	@p_LabelToSearch NVarChar(100), 
	@p_Id Integer Output, 
	@p_Label NVarChar(100) Output, 
	@p_Description NVarChar(2000) Output
AS
BEGIN
	Select @p_Id = Id, @p_Label = Label, @p_Description = Description From NotificationChannel Where Label = @p_LabelToSearch

END
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_InsertUser')
DROP PROCEDURE @db_user.NC_InsertUser
go

create PROCEDURE [@db_user].[NC_InsertUser] @p_UserId Integer, @p_ItemId Integer, @p_InstanceId Integer  AS
Begin
 INSERT INTO NOTIFICATIONUSER
    (ITEMID, USERID, InstanceId    ) VALUES
    ( @p_ItemId,
      @p_UserId,
      @p_InstanceId    )

End
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_InsertItem')
DROP PROCEDURE @db_user.NC_InsertItem
go

create PROCEDURE [@db_user].[NC_InsertItem] 
	@p_Author NVarchar(100), 
	@p_Title NVarChar(2000), 
	@p_Text NVarChar(2000), 
	@p_ChannelId Integer, 
	@p_MessageId Integer, 
	@p_MessageNumber Integer,
	@p_ItemId Integer Output As
Begin
    
    INSERT
    INTO NOTIFICATIONITEM
      (
        AUTHOR,
        TITLE,
        TEXT,
        LASTUPDATE,
        PUBLISHDATE,
        MESSAGEID,
        MessageNumber
      )
      VALUES
      (
        @p_Author,
        @p_Title,
        @p_Text,
        GETDATE(),
        GETDATE(),
        @p_MessageId,
        @p_MessageNumber
      )
      
      SET @p_itemId = @@identity
      
      INSERT INTO NOTIFICATIONITEMCATEGORIES
      (ITEMID, CATEGORYID
      ) VALUES
      (
        @p_itemId,
        @p_ChannelId
      )
      
      

End
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_InsertInstance')
DROP PROCEDURE @db_user.NC_InsertInstance
go

create PROCEDURE [@db_user].[NC_InsertInstance] @p_Description NVARCHAR(2000)  AS
Begin
 INSERT INTO NOTIFICATIONINSTANCE(DESCRIPTION) VALUES(@p_Description)

End
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_InsertChannel')
DROP PROCEDURE @db_user.NC_InsertChannel
go

create PROCEDURE [@db_user].[NC_InsertChannel] @p_Label NVARCHAR(2000), @p_Description VARCHAR(4000)  AS
Begin

 INSERT
   INTO NOTIFICATIONCHANNEL(LABEL, DESCRIPTION)
VALUES(@p_Label, @p_Description)

End
GO


-- =============================================
-- Author:		Samuele Furnari
-- Description:	Conteggio degli item che devono ancora essere visualizzati da un utente
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_CountNotViewedItems')
DROP PROCEDURE @db_user.NC_CountNotViewedItems
go

create PROCEDURE [@db_user].[NC_CountNotViewedItems]
	@p_UserId Integer, 
	@p_InstanceId Integer,
	@p_Count Integer Out
AS
BEGIN
    Select @p_Count = count(*)
      From NotificationItem ni
      Inner Join NotificationItemCategories nic
      On ni.Id = nic.ItemId
      Inner Join NotificationUser nu
      On nu.ItemId = ni.Id
      Where nu.ViewDate Is Null And nu.UserId = @p_UserId And nu.InstanceId = @p_InstanceId
  
END
GO


-- =============================================
-- Author:		Samuele Furnari
-- Description:	Store procedure per la pulizia delle tabelle relative al centro notifiche
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_CleanData')
DROP PROCEDURE @db_user.NC_CleanData
go

create PROCEDURE [@db_user].[NC_CleanData]
	
AS
BEGIN
	-- Pulizia della tabella degli utenti
	Delete NOTIFICATIONUSER
	
	-- Pulizia della tabella che lega una istanza ad un canale
	Delete NotificationInstanceChannels
	
	-- Pulizia della tabella che lega un item ad una categoria
	Delete NOTIFICATIONITEMCATEGORIES
	
	-- Pulizia della tabella degli item
	Delete NOTIFICATIONITEM
	
	-- Pulizia della tabella dei canali
	Delete NotificationChannel
	
	-- Pulizia della tabella delle istanza
	Delete NOTIFICATIONINSTANCE
	
END
GO

IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_AssociateItemToChannel')
DROP PROCEDURE @db_user.NC_AssociateItemToChannel
go

create PROCEDURE [@db_user].[NC_AssociateItemToChannel] @p_itemId INT, @p_ChannelId INT  AS
Begin
 INSERT INTO NOTIFICATIONITEMCATEGORIES(ITEMID, CATEGORYID) VALUES(@p_itemId,
@p_ChannelId)

End
GO

-- =============================================
-- Author:		Samuele Furnari
-- Description:	Procedura per l'associazione di un canale ad una istanza
-- =============================================
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_AssociateChannelToInstance')
DROP PROCEDURE @db_user.NC_AssociateChannelToInstance
go

create PROCEDURE [@db_user].[NC_AssociateChannelToInstance](
	@p_ChannelId Integer, 
	@p_InstanceId Integer)
AS
BEGIN
	Declare @exist integer
    
    -- L'associazione fra canale e istanza viene fatta solo se non esiste gi
    Select @exist = Count(*) From NotificationInstanceChannels Where InstanceId = @p_InstanceId And ChannelId = @p_ChannelId
    
    If (@exist = 0)
      INSERT INTO NotificationInstanceChannels
      (INSTANCEID, CHANNELID
      ) VALUES
      (
        @p_InstanceId,
        @p_ChannelId
      )
    
END
GO
/****** Object:  StoredProcedure [@db_user].[NC_SetItemViewed]    Script Date: 05/22/2012 12:14:18 ******/


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SetItemViewed')
DROP PROCEDURE @db_user.NC_SetItemViewed
go

create PROCEDURE [@db_user].[NC_SetItemViewed] @p_ItemId Integer, @p_UserId Integer, @p_InstanceId Integer  AS
Begin
	Update NOTIFICATIONUSER
	Set ViewDate = GETDATE()
    Where ItemId = @p_ItemId And UserId = @p_UserId And InstanceId = @p_InstanceId
End
GO


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='NC_SearchItemsNotViewedByUser')
DROP PROCEDURE @db_user.NC_SearchItemsNotViewedByUser
go

create PROCEDURE [@db_user].[NC_SearchItemsNotViewedByUser] @p_UserId Integer, @p_ChannelId Integer, @p_PageSize Integer, @p_PageNumber Integer, @p_Count Integer Out, @p_InstanceId Integer AS
Begin
	-- Calcolo degli indici minimo e massimo degli elementi da visualizzare
    Declare @lowRowNum Integer
    Declare @hightRowNum Integer
    
    Set @hightRowNum = (@p_PageNumber * @p_PageSize)
    Set @lowRowNum = (@hightRowNum - @p_PageSize) + 1
      
	-- Calcolo dl numero di item totali
	Select @p_Count = count(*)
	From NotificationItem ni
	Inner Join NotificationItemCategories nic
	On ni.Id = nic.ItemId
	Inner Join NotificationUser nu
	On nu.ItemId = ni.Id
	Where nu.ViewDate Is Null And nic.CategoryId =  @p_ChannelId And nu.UserId = @p_UserId  And nu.InstanceId = @p_InstanceId
	
	Execute('  
	  Select * From
	  (Select ROW_NUMBER() Over (Order By ni.Id Desc) As RowNumber,
	  ni.*
	  From NotificationItem ni
	  Inner Join NotificationItemCategories nic
	  On ni.Id = nic.ItemId
	  Inner Join NotificationUser nu
	  On nu.ItemId = ni.Id
	  Where nu.ViewDate Is Null And nic.CategoryId = ' + @p_ChannelId + ' And nu.UserId = ' + @p_UserId +
	  ' And nu.InstanceId = ' + @p_InstanceId + ') As ItemsToreturn
	  Where (ItemsToreturn.RowNumber >= ' + @lowRowNum + ' And ItemsToreturn.RowNumber <= ' + @hightRowNum + ')')  

End
GO
              
----------- FINE -
              
---- sp_modify_corr_esterno.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP sp_modify_corr_esterno
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='sp_modify_corr_esterno')
DROP PROCEDURE @db_user.sp_modify_corr_esterno
go


create PROCEDURE  @db_user.sp_modify_corr_esterno 
@idcorrglobale     INT       ,
@desc_corr         VARCHAR(4000)       ,
@nome              VARCHAR(4000)       ,
@cognome           VARCHAR(4000)       ,
@codice_aoo        VARCHAR(4000)       ,
@codice_amm        VARCHAR(4000)       ,
@email             VARCHAR(4000)       ,
@indirizzo         VARCHAR(4000)       ,
@cap               VARCHAR(4000)       ,
@provincia         VARCHAR(4000)       ,
@nazione           VARCHAR(4000)       ,
@citta             VARCHAR(4000)       ,
@cod_fiscale       VARCHAR(4000)       ,
@telefono          VARCHAR(4000)       ,
@telefono2         VARCHAR(4000)       ,
@note              VARCHAR(4000)       ,
@fax               VARCHAR(4000)       ,
@var_iddoctype     INT       ,
@inrubricacomune   CHAR(2000)       ,
@tipourp           CHAR(2000)       ,
@localita             VARCHAR(4000)          ,
@luogoNascita      VARCHAR(4000)       ,
@dataNascita       VARCHAR(4000)       ,
@titolo            VARCHAR(4000)       ,
@newid             INT OUTPUT      ,  
@returnvalue       INT OUTPUT 
AS
BEGIN
   DECLARE @error INT
   DECLARE @no_data_error INT
   DECLARE @cnt   INT
   DECLARE @cod_rubrica              VARCHAR(128)
   DECLARE @id_reg                   INT
   DECLARE @idamm                    INT
   DECLARE @new_var_cod_rubrica      VARCHAR(128)
   DECLARE @cha_dettaglio            CHAR(1) = '0'
   DECLARE @cha_tipourp             CHAR(1)
   DECLARE @myprofile                INT
   DECLARE @new_idcorrglobale        INT
   DECLARE @identitydettglobali      INT
   DECLARE @outvalue                 INT         = 1
   DECLARE @rtn                      INT
   DECLARE @v_id_doctype             INT
   DECLARE @identitydpatcanalecorr   INT
   DECLARE @chaTipoIE                CHAR(1)
   DECLARE @numLivello               INT          = 0
   DECLARE @idParent                 INT
   DECLARE @idPesoOrg                INT
   DECLARE @idUO                     INT
   DECLARE @idGruppo                 INT
   DECLARE @idTipoRuolo              INT
   DECLARE @cha_tipo_corr            CHAR(1)
   DECLARE @chapa                    CHAR(1)
   DECLARE @var_desc_old             VARCHAR(256)
   
   --<< reperimento_dati2 >>
   BEGIN
      SELECT   @cod_rubrica = var_cod_rubrica, @cha_tipourp = cha_tipo_urp, @id_reg = id_registro, @idamm = id_amm, @chapa = cha_pa, @chaTipoIE = cha_tipo_ie, @numLivello = num_livello, @idParent = id_parent, @idPesoOrg = id_peso_org, @idUO = id_uo, @idTipoRuolo = id_tipo_ruolo, @idGruppo = id_gruppo, @var_desc_old = var_desc_corr_old
      FROM @db_user.dpa_corr_globali
      WHERE system_id = @idcorrglobale
      SELECT   @no_data_error = @@ROWCOUNT
      IF (@no_data_error <> 0)
         PRINT 'select effettuata' 
      IF (@no_data_error = 0)
      begin
         PRINT 'primo blocco eccezione' 
         SET @outvalue = 0
         RETURN
      end
   END 
   if(@tipourp is not null and @cha_tipourp is not null and @cha_tipourp != @tipourp)
      SET @cha_tipourp = @tipourp

   --<< dati_canale_utente2 >>
   if @cha_tipourp = 'P'
   BEGIN
      SELECT   @v_id_doctype = id_documenttype
      FROM @db_user.dpa_t_canale_corr
      WHERE id_corr_globale = @idcorrglobale
      SELECT   @no_data_error = @@ROWCOUNT
      IF (@no_data_error = 0)
      begin
         PRINT '2do blocco eccezione' 
         SET @outvalue = 2
      end
   END 

   IF /* 0 */ @outvalue = 1
   begin
      IF /* 1 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'
         SET @cha_dettaglio = '1'
                                                       /* 1 */

--VERIFICO se il corrisp ?? stato utilizzato come dest/mitt di protocolli
      SELECT   @myprofile = COUNT(id_profile)
      FROM @db_user.dpa_doc_arrivo_par
      WHERE id_mitt_dest = @idcorrglobale

-- 1) non ?? stato mai utilizzato come corrisp in un protocollo
      IF /* 2 */ (@myprofile = 0)
      begin
         BEGIN
            update @db_user.dpa_corr_globali set var_codice_aoo = @codice_aoo,var_codice_amm = @codice_amm,var_email = @email,
            var_desc_corr = @desc_corr,var_nome = @nome,var_cognome = @cognome,
            cha_pa = @chapa,cha_tipo_urp = @cha_tipourp  WHERE system_id = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '3o blocco eccezione' 
               SET @outvalue = 3
               RETURN
            end
         END

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
         IF /* 3 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'
         --<< update_dpa_dett_globali2 >>
         BEGIN
            DECLARE @PrintVar VARCHAR(4000)
            SELECT   @cnt = count(*)
            FROM @db_user.dpa_dett_globali
            WHERE id_corr_globali = @idcorrglobale 
            SELECT   @error = @@ERROR
            IF (@error = 0)
            begin
               IF (@cnt = 0)
               begin
                  SET @PrintVar = 'sono nella INSERT,id_corr_globali =  '+@idcorrglobale
                  PRINT @PrintVar
                  INSERT INTO @db_user.dpa_dett_globali
                  (id_corr_globali, var_indirizzo, var_cap, 
                  var_provincia, var_nazione, var_cod_fiscale, 
                  var_telefono, var_telefono2, var_note, var_citta, 
                  var_fax, var_localita, var_luogo_nascita, dta_nascita, 
                  var_titolo)
					VALUES(@idcorrglobale, @indirizzo, @cap, 
					@provincia, @nazione, @cod_fiscale, @telefono, 
					@telefono2, @note, @citta, @fax, @localita, 
					@luogoNascita, @dataNascita, @titolo)
                  SELECT   @error = @@ERROR
               end
               IF (@error = 0)
               begin

/* MERGE INTO dpa_dett_globali
                        USING  (
SELECT system_id as id_interno
FROM dpa_dett_globali
WHERE id_corr_globali = idcorrglobale) select_interna
ON (system_id = select_interna.id_interno)
WHEN MATCHED THEN
UPDATE SET
var_indirizzo = indirizzo,
var_cap = cap,
var_provincia = provincia,
var_nazione = nazione,
var_cod_fiscale = cod_fiscale,
var_telefono = telefono,
var_telefono2 = telefono2,
var_note = note,
var_citta = citta,
var_fax = fax,
var_localita = localita,
var_luogo_nascita = luogoNascita,
dta_nascita = dataNascita,
var_titolo = titolo
WHERE (id_corr_globali = idcorrglobale) 
WHEN NOT MATCHED THEN
INSERT (
system_id,
id_corr_globali, 
var_indirizzo ,
var_cap ,
var_provincia ,
var_nazione ,
var_cod_fiscale ,
var_telefono ,
var_telefono2 ,
var_note ,
var_citta ,
var_fax ,
var_localita,
var_luogo_nascita,
dta_nascita,
var_titolo)
VALUES (
seq.nextval,
idcorrglobale,
indirizzo,
cap,
provincia,
nazione,
cod_fiscale,
telefono,
telefono2,
note,
citta,
fax,
localita,
luogoNascita,
dataNascita,
titolo);


*/
                  IF (@cnt = 1)
                  begin
                     PRINT 'sono nella UPDATE' 
                     update @db_user.dpa_dett_globali 
                     set var_indirizzo = @indirizzo,
                     var_cap = @cap,var_provincia = @provincia,
                     var_nazione = @nazione,
                     var_cod_fiscale = @cod_fiscale,
                     var_telefono = @telefono,
                     var_telefono2 = @telefono2,
                     var_note = @note,var_citta = @citta,
                     var_fax = @fax,var_localita = @localita,
                     var_luogo_nascita = @luogoNascita,
                     dta_nascita = @dataNascita,
                     var_titolo = @titolo  
                     WHERE (id_corr_globali = @idcorrglobale)
                     SELECT   @error = @@ERROR
                  end
                  IF (@error = 0)
                  begin
                     commit
                     PRINT 'sono nella merge' 
                  end
               end
            end
            IF (@error <> 0)
            begin
               SET @PrintVar = '4o blocco eccezione'+'SQLWAYS_EVAL# '+ str(@error)
               PRINT @PrintVar
               SET @outvalue = 4
               RETURN
            end
         END 
                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
         BEGIN
            update @db_user.dpa_t_canale_corr set id_documenttype = @var_iddoctype  WHERE id_corr_globale = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '5o blocco eccezione' 
               SET @outvalue = 5
               RETURN
            end
         END
      end
   ELSE
-- caso 2) Il corrisp ?? stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
      begin
         SET @new_var_cod_rubrica = @cod_rubrica + '_' + STR(@idcorrglobale)
         --<< storicizzazione_corrisp2 >>
         BEGIN
            update @db_user.dpa_corr_globali set dta_fine = GetDate(),var_cod_rubrica = @new_var_cod_rubrica,var_codice = @new_var_cod_rubrica,
            id_parent = NULL  WHERE system_id = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '6o blocco eccezione' 
               SET @outvalue = 6
               RETURN
            end
         END 
         SELECT   @newid = @@identity


/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
         --<< inserimento_nuovo_corrisp2 >>
         BEGIN
            IF (@inrubricacomune = '1')
               SET @cha_tipo_corr = 'C'
         ELSE
            SET @cha_tipo_corr = 'S'

            INSERT INTO @db_user.dpa_corr_globali(system_id, num_livello, cha_tipo_ie, id_registro,
id_amm, var_desc_corr, var_nome, var_cognome,
id_old, dta_inizio, id_parent, var_codice,
cha_tipo_corr, cha_tipo_urp,
var_codice_aoo, var_cod_rubrica, cha_dettagli,
var_email, var_codice_amm, cha_pa, id_peso_org,
id_gruppo, id_tipo_ruolo, id_uo,var_desc_corr_old)
VALUES(@newid, @numLivello, @chaTipoIE, @id_reg,
@idamm, @desc_corr, @nome, @cognome,
@idcorrglobale, GetDate(), @idParent, @cod_rubrica,
@cha_tipo_corr, @cha_tipourp,
@codice_aoo, @cod_rubrica, @cha_dettaglio,
@email, @codice_amm, @chapa, @idPesoOrg,
@idGruppo, @idTipoRuolo, @idUO, @var_desc_old)
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '7o blocco eccezione' 
               SET @outvalue = 7
               RETURN
            end
         END 


/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
         IF /* 4 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'
--PRENDO LA SYSTEM_ID APPENA INSERITA
         begin
            SELECT   @identitydettglobali = @@identity

            --<< inserimento_dettaglio_corrisp2 >>
            BEGIN
 INSERT INTO @db_user.dpa_dett_globali(system_id, id_corr_globali, var_indirizzo,
var_cap, var_provincia, var_nazione,
var_cod_fiscale, var_telefono, var_telefono2,
var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo)
VALUES(@identitydettglobali, @newid, @indirizzo,
@cap, @provincia, @nazione,
@cod_fiscale, @telefono, @telefono2,
@note, @citta, @fax, @localita, @luogoNascita, @dataNascita, @titolo)
               SELECT   @error = @@ERROR
               IF (@error <> 0)
               begin
                  PRINT '8o blocco eccezione' 
                  SET @outvalue = 8
                  RETURN
               end
            END 
         end
                                                    /* 4 */

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
         --<< inserimento_dpa_t_canale_corr2 >>
         BEGIN

 INSERT INTO @db_user.dpa_t_canale_corr(id_corr_globale, id_documenttype, cha_preferito)
VALUES(@newid, @var_iddoctype, '1')
            SELECT   @error = @@ERROR, @identitydpatcanalecorr = @@identity
            IF (@error <> 0)
            begin
               PRINT '9o blocco eccezione' 
               SET @outvalue = 9
               RETURN
            end
         END 
      end
   


--se fa parte di una lista, allora la devo aggiornare.
      if @newid IS NOT NULL
         update @db_user.dpa_liste_distr  set ID_DPA_CORR = @newid FROM @db_user.dpa_liste_distr d where d.ID_DPA_CORR = @idcorrglobale
   end



/* 2 */
 /* 0 */
   SET @returnvalue = @outvalue
END

GO              
----------- FINE -
              
---- sp_modify_corr_esterno_IS.MSSQL.sql  marcatore per ricerca ----

-- Controllo esistenza SP sp_modify_corr_esterno_IS
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='sp_modify_corr_esterno_IS')
DROP PROCEDURE @db_user.sp_modify_corr_esterno_IS
go

create PROCEDURE  @db_user.sp_modify_corr_esterno_IS 
/*
versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:
oltre a introdurre e gestire il nuovo parametro SimpInteropUrl , 
recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
per gestire il nuovo campo var_desc_corr_old
*/(
@idcorrglobale     INT       ,
@desc_corr         VARCHAR(4000)       ,
@nome              VARCHAR(4000)       ,
@cognome           VARCHAR(4000)       ,
@codice_aoo        VARCHAR(4000)       ,
@codice_amm        VARCHAR(4000)       ,
@email             VARCHAR(4000)       ,
@indirizzo         VARCHAR(4000)       ,
@cap               VARCHAR(4000)       ,
@provincia         VARCHAR(4000)       ,
@nazione           VARCHAR(4000)       ,
@citta             VARCHAR(4000)       ,
@cod_fiscale       VARCHAR(4000)       ,
@telefono          VARCHAR(4000)       ,
@telefono2         VARCHAR(4000)       ,
@note              VARCHAR(4000)       ,
@fax               VARCHAR(4000)       ,
@var_iddoctype     INT       ,
@inrubricacomune   CHAR(2000)       ,
@tipourp           CHAR(2000)       ,
@localita             VARCHAR(4000)          ,
@luogoNascita      VARCHAR(4000)       ,
@dataNascita       VARCHAR(4000)       ,
@titolo            VARCHAR(4000)       ,
-- aggiunto questo parametro e la gestione relativa rispetto alla vecchia versione
@SimpInteropUrl    VARCHAR(4000)       ,
@newid             INT OUTPUT  ) AS

BEGIN


--<<reperimento_dati>
   DECLARE @error INT
   DECLARE @no_data_error INT
   DECLARE @cnt   INT
   DECLARE @cod_rubrica              VARCHAR(128)
   DECLARE @id_reg                   INT
   DECLARE @idamm                    INT
   DECLARE @new_var_cod_rubrica      VARCHAR(128)
   DECLARE @cha_dettaglio            CHAR(1) = '0'
   DECLARE @cha_tipourp             CHAR(1)
   DECLARE @myprofile                INT
   DECLARE @new_idcorrglobale        INT
   DECLARE @identitydettglobali      INT
   DECLARE @outvalue                 INT         = 1
   DECLARE @rtn                      INT
   DECLARE @v_id_doctype             INT
   DECLARE @identitydpatcanalecorr   INT
   DECLARE @chaTipoIE                CHAR(1)
   DECLARE @numLivello               INT          = 0
   DECLARE @idParent                 INT
   DECLARE @idPesoOrg                INT
   DECLARE @idUO                     INT
   DECLARE @idGruppo                 INT
   DECLARE @idTipoRuolo              INT
   DECLARE @cha_tipo_corr            CHAR(1)
   DECLARE @chapa                    CHAR(1)
   DECLARE @var_desc_old             VARCHAR(256)
   DECLARE @url                    VARCHAR(4000)
   BEGIN
      SELECT   @cod_rubrica = var_cod_rubrica, @cha_tipourp = cha_tipo_urp, @id_reg = id_registro, @idamm = id_amm, @chapa = cha_pa, @chaTipoIE = cha_tipo_ie, @numLivello = num_livello, @idParent = id_parent, @idPesoOrg = id_peso_org, @idUO = id_uo, @idTipoRuolo = id_tipo_ruolo, @idGruppo = id_gruppo, @var_desc_old = var_desc_corr_old, @url = InteropUrl
      FROM @db_user.dpa_corr_globali
      WHERE system_id = @idcorrglobale
      SELECT   @no_data_error = @@ROWCOUNT
      IF (@no_data_error <> 0)
         PRINT 'select effettuata' 
      IF (@no_data_error = 0)
      begin
         PRINT 'primo blocco eccezione' 
         SET @outvalue = 0
         RETURN
      end
   END 
   if(@tipourp is not null and @cha_tipourp is not null and @cha_tipourp != @tipourp)
      SET @cha_tipourp = @tipourp

   --<< dati_canale_utente >>
   if @cha_tipourp = 'P'
   BEGIN
      SELECT   @v_id_doctype = id_documenttype
      FROM dpa_t_canale_corr
      WHERE id_corr_globale = @idcorrglobale
      SELECT   @no_data_error = @@ROWCOUNT
      IF (@no_data_error = 0)
      begin
         PRINT '2do blocco eccezione' 
         SET @outvalue = 2
      end
   END 

   IF /* 0 */ @outvalue = 1
   begin
      IF /* 1 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'
         SET @cha_dettaglio = '1'
                                                       /* 1 */

--VERIFICO se il corrisp ?? stato utilizzato come dest/mitt di protocolli
      SELECT   @myprofile = COUNT(id_profile)
      FROM dpa_doc_arrivo_par
      WHERE id_mitt_dest = @idcorrglobale

-- 1) non ?? stato mai utilizzato come corrisp in un protocollo
      IF /* 2 */ (@myprofile = 0)
      begin
         BEGIN
            update dpa_corr_globali set var_codice_aoo = @codice_aoo,var_codice_amm = @codice_amm,var_email = @email,
            var_desc_corr = @desc_corr,var_nome = @nome,var_cognome = @cognome,
            cha_pa = @chapa,cha_tipo_urp = @cha_tipourp,InteropUrl = @SimpInteropUrl  WHERE system_id = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '3o blocco eccezione' 
               SET @outvalue = 3
               RETURN
            end
         END

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
         IF /* 3 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'     OR @cha_tipourp = 'F'
         --<< update_dpa_dett_globali2 >>
         BEGIN
            DECLARE @PrintVar VARCHAR(4000)
            SELECT   @cnt = count(*)
            FROM dpa_dett_globali
            WHERE id_corr_globali = @idcorrglobale 
            SELECT   @error = @@ERROR
            IF (@error = 0)
            begin
               IF (@cnt = 0)
               begin
                  SET @PrintVar = 'sono nella INSERT,id_corr_globali =  '+@idcorrglobale
                  PRINT @PrintVar
                  INSERT INTO dpa_dett_globali(id_corr_globali, var_indirizzo, var_cap, var_provincia, var_nazione, var_cod_fiscale, var_telefono, var_telefono2, var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo)
VALUES(@idcorrglobale, @indirizzo, @cap, @provincia, @nazione, @cod_fiscale, @telefono, @telefono2, @note, @citta, @fax, @localita, @luogoNascita, @dataNascita, @titolo)
                  SELECT   @error = @@ERROR
               end
               IF (@error = 0)
               begin

/*

MERGE INTO dpa_dett_globali
USING (
SELECT system_id as id_interno
FROM dpa_dett_globali
WHERE id_corr_globali = idcorrglobale) select_interna
ON (system_id = select_interna.id_interno)
WHEN MATCHED THEN
UPDATE SET
var_indirizzo = indirizzo,
var_cap = cap,
var_provincia = provincia,
var_nazione = nazione,
var_cod_fiscale = cod_fiscale,
var_telefono = telefono,
var_telefono2 = telefono2,
var_note = note,
var_citta = citta,
var_fax = fax,
var_localita = localita,
var_luogo_nascita = luogoNascita,
dta_nascita = dataNascita,
var_titolo = titolo
WHERE (id_corr_globali = idcorrglobale) 
WHEN NOT MATCHED THEN
INSERT (
system_id,
id_corr_globali, 
var_indirizzo ,
var_cap ,
var_provincia ,
var_nazione ,
var_cod_fiscale ,
var_telefono ,
var_telefono2 ,
var_note ,
var_citta ,
var_fax ,
var_localita,
var_luogo_nascita,
dta_nascita,
var_titolo)
VALUES (
seq.nextval,
idcorrglobale,
indirizzo,
cap,
provincia,
nazione,
cod_fiscale,
telefono,
telefono2,
note,
citta,
fax,
localita,
luogoNascita,
dataNascita,
titolo);


*/
                  IF (@cnt = 1)
                  begin
                     PRINT 'sono nella UPDATE' 
                     update dpa_dett_globali set var_indirizzo = @indirizzo,var_cap = @cap,var_provincia = @provincia,var_nazione = @nazione,
                     var_cod_fiscale = @cod_fiscale,var_telefono = @telefono,
                     var_telefono2 = @telefono2,var_note = @note,var_citta = @citta,
                     var_fax = @fax,var_localita = @localita,var_luogo_nascita = @luogoNascita,
                     dta_nascita = @dataNascita,var_titolo = @titolo  WHERE (id_corr_globali = @idcorrglobale)
                     SELECT   @error = @@ERROR
                  end
                  IF (@error = 0)
                  begin
                    
                     PRINT 'sono nella merge' 
                  end
               end
            end
            IF (@error <> 0)
            begin
               SET @PrintVar = '4o blocco eccezione'+ str(@error)
               PRINT @PrintVar
               SET @outvalue = 4
               RETURN
            end
         END 
                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
         BEGIN
            update dpa_t_canale_corr set id_documenttype = @var_iddoctype  WHERE id_corr_globale = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '5o blocco eccezione' 
               SET @outvalue = 5
               RETURN
            end
         END
      end
   ELSE
-- caso 2) Il corrisp ?? stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
      begin
         SET @new_var_cod_rubrica = @cod_rubrica + '_' + STR(@idcorrglobale)
         --<< storicizzazione_corrisp2 >>
         BEGIN
            update dpa_corr_globali set dta_fine = GetDate(),var_cod_rubrica = @new_var_cod_rubrica,var_codice = @new_var_cod_rubrica,
            id_parent = NULL  WHERE system_id = @idcorrglobale
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '6o blocco eccezione' 
               SET @outvalue = 6
               RETURN
            end
         END 
         SELECT   @newid = @@identity


/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
         --<< inserimento_nuovo_corrisp2 >>
         BEGIN
            IF (@inrubricacomune = '1')
               SET @cha_tipo_corr = 'C'
         ELSE
            SET @cha_tipo_corr = 'S'

            INSERT INTO dpa_corr_globali(system_id, num_livello, cha_tipo_ie, id_registro,
id_amm, var_desc_corr, var_nome, var_cognome,
id_old, dta_inizio, id_parent, var_codice,
cha_tipo_corr, cha_tipo_urp,
var_codice_aoo, var_cod_rubrica, cha_dettagli,
var_email, var_codice_amm, cha_pa, id_peso_org,
id_gruppo, id_tipo_ruolo, id_uo,var_desc_corr_old     , InteropUrl)
VALUES(@newid, @numLivello, @chaTipoIE, @id_reg,
@idamm, @desc_corr, @nome, @cognome,
@idcorrglobale, GetDate(), @idParent, @cod_rubrica,
@cha_tipo_corr, @cha_tipourp,
@codice_aoo, @cod_rubrica, @cha_dettaglio,
@email, @codice_amm, @chapa, @idPesoOrg,
@idGruppo, @idTipoRuolo, @idUO, @var_desc_old , @SimpInteropUrl)
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               PRINT '7o blocco eccezione' 
               SET @outvalue = 7
               RETURN
            end
         END 


/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
         IF /* 4 */ @cha_tipourp = 'U' OR @cha_tipourp = 'P'
--PRENDO LA SYSTEM_ID APPENA INSERITA
         begin
            SELECT   @identitydettglobali = @@identity

            --<< inserimento_dettaglio_corrisp2 >>
            BEGIN
 INSERT INTO dpa_dett_globali(system_id, id_corr_globali, var_indirizzo,
var_cap, var_provincia, var_nazione,
var_cod_fiscale, var_telefono, var_telefono2,
var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo)
VALUES(@identitydettglobali, @newid, @indirizzo,
@cap, @provincia, @nazione,
@cod_fiscale, @telefono, @telefono2,
@note, @citta, @fax, @localita, @luogoNascita, @dataNascita, @titolo)
               SELECT   @error = @@ERROR
               IF (@error <> 0)
               begin
                  PRINT '8o blocco eccezione' 
                  SET @outvalue = 8
                  RETURN
               end
            END 
         end
                                                    /* 4 */

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
         --<< inserimento_dpa_t_canale_corr2 >>
         BEGIN

 INSERT INTO dpa_t_canale_corr(id_corr_globale, id_documenttype, cha_preferito)
VALUES(@newid, @var_iddoctype, '1')
            SELECT   @error = @@ERROR, @identitydpatcanalecorr = @@identity
            IF (@error <> 0)
            begin
               PRINT '9o blocco eccezione' 
               SET @outvalue = 9
               RETURN
            end
         END 
      end
   


--se fa parte di una lista, allora la devo aggiornare.
      if @newid IS NOT NULL
         update dpa_liste_distr  set ID_DPA_CORR = @newid FROM dpa_liste_distr d where d.ID_DPA_CORR = @idcorrglobale
   end



/* 2 */
 /* 0 */
    RETURN @outvalue
END

GO


              
----------- FINE -
              
---- ZZ.SP_DELETE_CORR_ESTERNO.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP SP_DELETE_CORR_ESTERNO
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='SP_DELETE_CORR_ESTERNO')
DROP PROCEDURE @db_user.SP_DELETE_CORR_ESTERNO
go

-- deve essere l'ultima function del gruppo per non impattare su successive compilazioni
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create PROCEDURE  @db_user.[SP_DELETE_CORR_ESTERNO] @IDCorrGlobale INT, @liste INT  AS

/*
-------------------------------------------------------------------------------------------------------
SP per la Cancellazione corrispondente

Valori di ritorno gestiti:

0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
1: DISABILITAZIONE EFFETTUATA - il corrispondente  presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato

2: CORRISPONDENTE NON RIMOSSO - il corrispondente  presente nella lista di distribuzione e non posso rimuoverlo

3: ERRORE: la DELETE sulla dpa_corr_globali NON  andata a buon fine
4: ERRORE: la DELETE sulla dpa_dett_globali NON  andata a buon fine
5: ERRORE: l' UPDATE sulla dpa_corr_globali NON  andata a buon fine
6: ERRORE: la DELETE sulla dpa_liste_distr NON  andata a buon fine
-------------------------------------------------------------------------------------------------------

*/

DECLARE @countDoc int   -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
DECLARE @ReturnValue int
DECLARE @cha_tipo_urp varchar(1)
DECLARE @var_inLista varchar(1) -- valore 'N' (il corr non  presente in nessuna lista di sistribuzione), 'Y' altrimenti
BEGIN

SET @cha_tipo_urp = (select cha_tipo_urp from dpa_corr_globali where system_id = @IDCorrGlobale)

SET @var_inLista = 'N' -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

SELECT SYSTEM_ID  FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale

IF @@ROWCOUNT > 0
BEGIN
IF (@liste = 1)
--- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente  in una lista di distibuzione, in caso affermativo non posso rimuoverlo
BEGIN
-- CASO 1.1 - Il corrispondente  predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).

RETURN 2
END
ELSE
-- CASO 2 - Le liste di distribuzione NON SONO abilitate

BEGIN
SET @var_inLista = 'Y'
END

END



-- Se la procedura va avanti, cio' significa che:
-- Le liste di distribuzione non sono abilitate (@liste = 0), oppure sono abilitate (@liste=1) ma il corrispondente che si tenta di rimuovere non  contenuto in una lista

SELECT ID_PROFILE  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST =  @IDCorrGlobale

IF (@@ROWCOUNT = 0)
-- CASO 3 -  il corrispondente non  stato mai utilizzato come mitt/dest di protocolli
BEGIN

-- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = @IDCorrGlobale

IF @@ROWCOUNT = 0
-- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).
BEGIN
SET @ReturnValue=3
END
ELSE
BEGIN
SET @ReturnValue=0

DELETE FROM DPA_T_CANALE_CORR WHERE ID_CORR_GLOBALE = @IDCorrGlobale

-- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poich in fase di creazione di un ruolo
-- non viene fatta la insert in tale tabella
IF( @cha_tipo_urp != 'R')

BEGIN

-- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = @IDCorrGlobale

IF @@ROWCOUNT = 0
-- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
BEGIN
SET @ReturnValue=4
END
ELSE
-- CAS0 3.1.2.2 - la rimozione da DPA_DETT_GLOBALI VAa buon fine  (VALORE RITORNATO = 0).
BEGIN
SET @ReturnValue=0  -- operazione andata a buon fine
END
END

IF (@ReturnValue=0 AND @liste = 0 AND @var_inLista = 'Y')
BEGIN
--se:
-- 1) sono andate bene le DELETE precedenti
-- 2) sono disabilitate le liste di distribuzione
-- 3) il corrispondente  nella DPA_LISTE_DISTR

-- rimuovo il corrispondente dalla DPA_LISTE_DISTR
DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale

IF @@ROWCOUNT = 0
-- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).
BEGIN
SET @ReturnValue=6
END

END
END

END

ELSE
-- CASO 4 -  il corrispondente   stato utilizzato come mitt/dest di protocolli
BEGIN
-- 4.1) disabilitazione del corrispondente
UPDATE DPA_CORR_GLOBALI SET DTA_FINE = GETDATE() WHERE SYSTEM_ID = @IDCorrGlobale

IF @@ROWCOUNT = 0
-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
BEGIN
SET @ReturnValue=5
END

ELSE
-- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
BEGIN
SET @ReturnValue=1
END
END

END

RETURN @ReturnValue
GO

              
----------- FINE -
     
     
     /* correzione alla store che elimina i documenti e tutti i collegamenti */
     
IF EXISTS (SELECT * FROM sys.objects where type = 'P' and name = 'SP_RIMUOVI_DOCUMENTI') 
DROP PROCEDURE @db_user.[SP_RIMUOVI_DOCUMENTI]
GO

CREATE Procedure @db_user.[SP_RIMUOVI_DOCUMENTI]
@idProfile INT ,
@ReturnValue INT OUTPUT  AS
BEGIN

DELETE FROM @db_user.DPA_TRASM_UTENTE
WHERE id_trasm_singola in
(SELECT system_id FROM @db_user.DPA_TRASM_SINGOLA
WHERE id_trasmissione in
(select t.system_id
from @db_user.dpa_trasmissione t
where t.id_profile = @idProfile
)
)

DELETE FROM @db_user.DPA_TRASM_SINGOLA
WHERE id_trasmissione in
(select t.system_id from @db_user.dpa_trasmissione t
where t.id_profile = @idProfile
)


DELETE FROM @db_user.DPA_TRASMISSIONE WHERE id_profile = @idProfile

DELETE FROM @db_user.project_components where LINK = @idProfile

DELETE FROM @db_user.VERSIONS WHERE DOCNUMBER = @idProfile

DELETE FROM @db_user.COMPONENTS WHERE DOCNUMBER = @idProfile

DELETE FROM @db_user.DPA_AREA_LAVORO WHERE ID_PROFILE = @idProfile

DELETE FROM @db_user.DPA_PROF_PAROLE WHERE ID_PROFILE = @idProfile

DELETE FROM @db_user.PROFILE WHERE DOCNUMBER = @idProfile

DELETE FROM @db_user.SECURITY WHERE THING = @idProfile

DELETE from @db_user.dpa_todolist where id_profile = @idProfile

DELETE FROM @db_user.DPA_DIAGRAMMI WHERE DOC_NUMBER = @idProfile

BEGIN
DECLARE @cnt INT
SELECT   @cnt = COUNT(*) FROM @db_user.DPA_ASSOCIAZIONE_TEMPLATES
WHERE DOC_NUMBER = @idProfile
IF (@cnt != 0 and (@idProfile !='' and @idProfile !=null ))
DELETE FROM @db_user.DPA_ASSOCIAZIONE_TEMPLATES
WHERE DOC_NUMBER = @idProfile

END
SET @ReturnValue = 1
END
              
GO


----  SAB: store provedure e function mancanti

IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[GetNumMittentiModelliTrasm]') 			)
	DROP FUNCTION [@db_user].GetNumMittentiModelliTrasm
GO

CREATE FUNCTION [@db_user].[GetNumMittentiModelliTrasm] 
(
      -- Id del modello di cui contare i mittenti
      @TemplateId   int
)
RETURNS int
AS
BEGIN
      -- Valore da restituire
      DECLARE @ResultVar int

      -- Conteggio del numero di mittenti del modello di trasmissione
      -- con id pari a quello passato per parametro
      SELECT @ResultVar = COUNT('X')
      FROM @db_user.dpa_modelli_trasm AS mt
            INNER JOIN @db_user.dpa_modelli_mitt_dest md
            ON mt.system_id = md.id_modello
      WHERE mt.system_id = @TemplateId
              AND md.CHA_TIPO_MITT_DEST = 'M'

      -- Return the result of the function
      RETURN @ResultVar

END
GO

----   getGerarchia

IF EXISTS (		SELECT * FROM dbo.sysobjects
				WHERE id = OBJECT_ID('[@db_user].[getGerarchia]') 			)
	DROP FUNCTION [@db_user].getGerarchia
GO

CREATE FUNCTION [@db_user].[getGerarchia] (@id_amm varchar(64), @codRubrica varchar(64), @tipoCorr varchar(200), @id_ruolo int, @tipo varchar(200))
RETURNS varchar(4000) AS
BEGIN

declare @codes varchar(4000)
declare @c_type varchar(2)
declare @p_cod varchar(64)
declare @system_id int
declare @id_parent int
declare @id_uo int
declare @id_utente int
declare @mydebug varchar(2)

--select 
--@tipoCorr = CHA_TIPO_URP from dpa_corr_globali
--where
--UPPER(var_cod_rubrica) = UPPER(@codRubrica) and cha_tipo_ie = @tipo  and
--id_amm = @id_amm  and dta_fine is null 

set @codes = ''
select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @id_utente = id_people, @c_type = cha_tipo_urp 
from dpa_corr_globali 
where
var_cod_rubrica=@codRubrica and
cha_tipo_ie=@tipo  and
id_amm=@id_amm and
dta_fine is null
 
while (1 > 0)
begin
	if @c_type is null
	break
 
	if @c_type = 'U'
	begin
		if (@id_parent is null or @id_parent = 0)
		break
	select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_parent and id_amm=@id_amm and dta_fine is null
	end

	if @c_type = 'R'
	begin
		if (@id_uo is null or @id_uo = 0)
		break
		select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_uo and id_amm=@id_amm and dta_fine is null
	end

	if @c_type = 'P'
	begin
		select top 1 @p_cod = var_cod_rubrica 
		--,    @mydebug = '__' 
		from dpa_corr_globali 
		where id_gruppo = @id_Ruolo 
		and id_amm=@id_amm and dta_fine is null
		--set 
	end

	if @p_cod is null
	break
 
	select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @c_type = cha_tipo_urp 
	from dpa_corr_globali where var_cod_rubrica=@p_cod and id_amm=@id_amm and dta_fine is null
	 
	set @codes = @p_cod + ':'+ @codes 
end

set @codes = @codes + @codRubrica

RETURN  @codes
END

GO
----

IF EXISTS (SELECT * FROM sys.objects where type = 'P' and name = 'sp_modify_corr_esterno_IS') 
DROP PROCEDURE @db_user.[sp_modify_corr_esterno_IS]
GO


CREATE PROCEDURE [@db_user].[sp_modify_corr_esterno_IS] 
  /*  
  versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:  
  oltre a introdurre e gestire il nuovo parametro SimpInteropUrl ,   
  recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
    per gestire il nuovo campo var_desc_corr_old  
  */ 
  
  /* Da testare */
  @idcorrglobale   INT, 
  @desc_corr       VARCHAR(4000), 
  @nome            VARCHAR(4000), 
  @cognome         VARCHAR(4000), 
  @codice_aoo      VARCHAR(4000), 
  @codice_amm      VARCHAR(4000), 
  @email           VARCHAR(4000), 
  @indirizzo       VARCHAR(4000), 
  @cap             VARCHAR(4000), 
  @provincia       VARCHAR(4000), 
  @nazione         VARCHAR(4000), 
  @citta           VARCHAR(4000), 
  @cod_fiscale     VARCHAR(4000), 
  @telefono        VARCHAR(4000), 
  @telefono2       VARCHAR(4000), 
  @note            VARCHAR(4000), 
  @fax             VARCHAR(4000), 
  @var_iddoctype   INT, 
  @inrubricacomune CHAR(2000), 
  @tipourp         CHAR(2000), 
  @localita        VARCHAR(4000), 
  @luogoNascita    VARCHAR(4000), 
  @dataNascita     VARCHAR(4000), 
  @titolo          VARCHAR(4000), 

  -- aggiunto questo parametro e la gestione relativa rispetto alla vecchia versione
   @SimpInteropUrl  VARCHAR(4000), 
  @newid           INT output 
AS 
  BEGIN 
      DECLARE @error INT 
      DECLARE @no_data_error INT 
      DECLARE @cnt INT 
      DECLARE @cod_rubrica VARCHAR(128) 
      DECLARE @id_reg INT 
      DECLARE @idamm INT 
      DECLARE @new_var_cod_rubrica VARCHAR(128) 
      DECLARE @cha_dettaglio CHAR(1) = '0' 
      DECLARE @cha_tipourp CHAR(1) 
      DECLARE @myprofile INT 
      DECLARE @new_idcorrglobale INT 
      DECLARE @identitydettglobali INT 
      DECLARE @outvalue INT = 1 
      DECLARE @rtn INT 
      DECLARE @v_id_doctype INT 
      DECLARE @identitydpatcanalecorr INT 
      DECLARE @chaTipoIE CHAR(1) 
      DECLARE @numLivello INT = 0 
      DECLARE @idParent INT 
      DECLARE @idPesoOrg INT 
      DECLARE @idUO INT 
      DECLARE @idGruppo INT 
      DECLARE @idTipoRuolo INT 
      DECLARE @cha_tipo_corr CHAR(1) 
      DECLARE @chapa CHAR(1) 
      DECLARE @var_desc_old VARCHAR(256) 
      DECLARE @url VARCHAR(4000) 

      BEGIN 
          SELECT @cod_rubrica = var_cod_rubrica, 
                 @cha_tipourp = cha_tipo_urp, 
                 @id_reg = id_registro, 
                 @idamm = id_amm, 
                 @chapa = cha_pa, 
                 @chaTipoIE = cha_tipo_ie, 
                 @numLivello = num_livello, 
                 @idParent = id_parent, 
                 @idPesoOrg = id_peso_org, 
                 @idUO = id_uo, 
                 @idTipoRuolo = id_tipo_ruolo, 
                 @idGruppo = id_gruppo, 
                 @var_desc_old = var_desc_corr_old 
          FROM   dpa_corr_globali 
          WHERE  system_id = @idcorrglobale 

          SELECT @no_data_error = @@ROWCOUNT 

          IF ( @no_data_error <> 0 ) 
            PRINT 'select effettuata' 

          IF ( @no_data_error = 0 ) 
            BEGIN 
                PRINT 'primo blocco eccezione' 

                SET @outvalue = 0 

                RETURN 
            END 
      END 

      IF( @tipourp IS NOT NULL 
          AND @cha_tipourp IS NOT NULL 
          AND @cha_tipourp != @tipourp ) 
        SET @cha_tipourp = @tipourp 

      --<< dati_canale_utente >>  
      IF @cha_tipourp = 'P' 
        BEGIN 
            SELECT @v_id_doctype = id_documenttype 
            FROM   dpa_t_canale_corr 
            WHERE  id_corr_globale = @idcorrglobale 

            SELECT @no_data_error = @@ROWCOUNT 

            IF ( @no_data_error = 0 ) 
              BEGIN 
                  PRINT '2do blocco eccezione' 

                  SET @outvalue = 2 
              END 
        END 

      IF /* 0 */ @outvalue = 1 
        BEGIN 
            IF /* 1 */ @cha_tipourp = 'U' 
                        OR @cha_tipourp = 'P' 
              SET @cha_dettaglio = '1' 

        /* 1 */ 
            --VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli
            SELECT @myprofile = Count(id_profile) 
            FROM   dpa_doc_arrivo_par 
            WHERE  id_mitt_dest = @idcorrglobale 

            -- 1) non è stato mai utilizzato come corrisp in un protocollo  
            IF /* 2 */ ( @myprofile = 0 ) 
              BEGIN 
                  UPDATE dpa_corr_globali 
                  SET    var_codice_aoo = @codice_aoo, 
                         var_codice_amm = @codice_amm, 
                         var_email = @email, 
                         var_desc_corr = @desc_corr, 
                         var_nome = @nome, 
                         var_cognome = @cognome, 
                         cha_pa = @chapa, 
                         cha_tipo_urp = @cha_tipourp, 
                         interopurl = @SimpInteropUrl 
                  WHERE  system_id = @idcorrglobale 

                  SELECT @error = @@ERROR 

                  IF ( @error <> 0 ) 
                    BEGIN 
                        PRINT '3o blocco eccezione' 

                        SET @outvalue = 3 

                        RETURN 
                    END 

                  /* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE  
                  PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
                    */ 
                  IF /* 3 */ @cha_tipourp = 'U' 
                              OR @cha_tipourp = 'P' 
                              OR @cha_tipourp = 'F' 
                    --<< update_dpa_dett_globali2 >>  
                    BEGIN 
                        DECLARE @PrintVar VARCHAR(4000) 

                        SELECT @cnt = Count(*) 
                        FROM   dpa_dett_globali 
                        WHERE  id_corr_globali = @idcorrglobale 

                        SELECT @error = @@ERROR 

                        IF ( @error = 0 ) 
                          BEGIN 
                              IF ( @cnt = 0 ) 
                                BEGIN 
                                    SET @PrintVar = 
                                    'sono nella INSERT,id_corr_globali =  ' 
                                    + @idcorrglobale 

                                    PRINT @PrintVar 

                                    INSERT INTO dpa_dett_globali 
                                                (id_corr_globali, 
                                                 var_indirizzo, 
                                                 var_cap, 
                                                 var_provincia, 
                                                 var_nazione, 
                                                 var_cod_fiscale, 
                                                 var_telefono, 
                                                 var_telefono2, 
                                                 var_note, 
                                                 var_citta, 
                                                 var_fax, 
                                                 var_localita, 
                                                 var_luogo_nascita, 
                                                 dta_nascita, 
                                                 var_titolo) 
                                    VALUES      ( @idcorrglobale, 
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
                                                  @titolo ) 

                                    SELECT @error = @@ERROR 
                                END 

                              IF ( @error = 0 ) 
                                BEGIN 
                                    IF ( @cnt = 1 ) 
                                      BEGIN 
                                          PRINT 'sono nella UPDATE' 

                                          UPDATE dpa_dett_globali 
                                          SET    var_indirizzo = @indirizzo, 
                                                 var_cap = @cap, 
                                                 var_provincia = @provincia, 
                                                 var_nazione = @nazione, 
                                                 --var_cod_fisc         = cod_fiscale,
                                                  --var_cod_pi           = partita_iva,
                                                  var_telefono = @telefono, 
                                                 var_telefono2 = @telefono2, 
                                                 var_note = @note, 
                                                 var_citta = @citta, 
                                                 var_fax = @fax, 
                                                 var_localita = @localita, 
                                                 var_luogo_nascita = @luogoNascita, 
                                                 dta_nascita = @dataNascita, 
											     var_titolo = @titolo 
											WHERE  ( id_corr_globali = @idcorrglobale ) 

  SELECT @error = @@ERROR 
  END 

  IF ( @error = 0 ) 
  BEGIN 
  PRINT 'sono nella merge' 
  END 
  END 
  END 

  IF ( @error <> 0 ) 
  BEGIN 
  SET @PrintVar = '4o blocco eccezione' + Str(@error) 

  PRINT @PrintVar 

  SET @outvalue = 4 

  RETURN 
  END 
  END 

  /* 3 */ 
  --METTI QUI UPDATE SU DPA_T_CANALE_CORR  
  --AGGIORNO LA DPA_T_CANALE_CORR  
  UPDATE dpa_t_canale_corr 
  SET    id_documenttype = @var_iddoctype 
  WHERE  id_corr_globale = @idcorrglobale 

  SELECT @error = @@ERROR 

  IF ( @error <> 0 ) 
  BEGIN 
  PRINT '5o blocco eccezione' 

  SET @outvalue = 5 

  RETURN 
  END 
  END 
		  ELSE 
		  -- caso 2) Il corrisp è stato utilizzato come corrisp in un protocollo  
		  BEGIN 
		  -- NUOVO CODICE RUBRICA  
		  SET @new_var_cod_rubrica = @cod_rubrica + '_' + CONVERT(varchar, @idcorrglobale) 

		  --<< storicizzazione_corrisp2 >>  
		  UPDATE dpa_corr_globali 
		  SET    dta_fine = Getdate(), 
		  var_cod_rubrica = @new_var_cod_rubrica, 
		  var_codice = @new_var_cod_rubrica, 
		  id_parent = NULL 
		  WHERE  system_id = @idcorrglobale 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '6o blocco eccezione' 

		  SET @outvalue = 6 

		  RETURN 
		  END 

		  /* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO  
		  INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */ 
		  --<< inserimento_nuovo_corrisp2 >>  
		  BEGIN 
		  IF ( @inrubricacomune = '1' ) 
		  SET @cha_tipo_corr = 'C' 
		  ELSE 
		  SET @cha_tipo_corr = 'S' 

		  INSERT INTO dpa_corr_globali 
		  (num_livello, 
		  cha_tipo_ie, 
		  id_registro, 
		  id_amm, 
		  var_desc_corr, 
		  var_nome, 
		  var_cognome, 
		  id_old, 
		  dta_inizio, 
		  id_parent, 
		  var_codice, 
		  cha_tipo_corr, 
		  cha_tipo_urp, 
		  var_codice_aoo, 
		  var_cod_rubrica, 
		  cha_dettagli, 
		  var_email, 
		  var_codice_amm, 
		  cha_pa, 
		  id_peso_org, 
		  id_gruppo, 
		  id_tipo_ruolo, 
		  id_uo, 
		  var_desc_corr_old, 
		  interopurl) 
		  VALUES      ( @numLivello, 
		  @chaTipoIE, 
		  @id_reg, 
		  @idamm, 
		  @desc_corr, 
		  @nome, 
		  @cognome, 
		  @idcorrglobale, 
		  Getdate(), 
		  @idParent, 
		  @cod_rubrica, 
		  @cha_tipo_corr, 
		  @cha_tipourp, 
		  @codice_aoo, 
		  @cod_rubrica, 
		  @cha_dettaglio, 
		  @email, 
		  @codice_amm, 
		  @chapa, 
		  @idPesoOrg, 
		  @idGruppo, 
		  @idTipoRuolo, 
		  @idUO, 
		  @var_desc_old, 
		  @SimpInteropUrl ) 

			SELECT @newid = @@identity 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '7o blocco eccezione' 

		  SET @outvalue = 7 

		  RETURN 
		  END 
		  END 

		  /* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL  
		  RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI  
		  E UNITA' ORGANIZZATIVE */ 
		  IF /* 4 */ @cha_tipourp = 'U' 
		  OR @cha_tipourp = 'P' 
		  --PRENDO LA SYSTEM_ID APPENA INSERITA  
		  BEGIN 
		  SELECT @identitydettglobali = @@identity 

		  --<< inserimento_dettaglio_corrisp2 >>  
		  INSERT INTO dpa_dett_globali 
		  (id_corr_globali, 
		  var_indirizzo, 
		  var_cap, 
		  var_provincia, 
		  var_nazione, 
		  var_cod_fiscale, 
		  var_telefono, 
		  var_telefono2, 
		  var_note, 
		  var_citta, 
		  var_fax, 
		  var_localita, 
		  var_luogo_nascita, 
		  dta_nascita, 
		  var_titolo) 
		  VALUES      ( @newid, 
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
		  @titolo ) 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '8o blocco eccezione' 

		  SET @outvalue = 8 

		  RETURN 
		  END 
		  END 

		  /* 4 */ 
		  --INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
		   --<< inserimento_dpa_t_canale_corr2 >>  
		  BEGIN 
		  INSERT INTO dpa_t_canale_corr 
		  (id_corr_globale, 
		  id_documenttype, 
		  cha_preferito) 
		  VALUES      (@newid, 
		  @var_iddoctype, 
		  '1') 

		  SELECT @error = @@ERROR, 
		  @identitydpatcanalecorr = @@identity 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '9o blocco eccezione' 

		  SET @outvalue = 9 

		  RETURN 
		  END 
		  END 
		  END 

		  --se fa parte di una lista, allora la devo aggiornare.  
		  IF @newid IS NOT NULL 
		  UPDATE dpa_liste_distr 
		  SET    id_dpa_corr = @newid 
		  FROM   dpa_liste_distr d 
		  WHERE  d.id_dpa_corr = @idcorrglobale 
		END 

  /* 2 */ 
      /* 0 */ 
      RETURN @outvalue 
  END  
GO




-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.21.1')
GO

              
