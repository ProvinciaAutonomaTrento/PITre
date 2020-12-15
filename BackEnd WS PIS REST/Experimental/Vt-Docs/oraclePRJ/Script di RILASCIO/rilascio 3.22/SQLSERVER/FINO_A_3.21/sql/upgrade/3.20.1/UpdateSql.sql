SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- file sql di update per il CD -- 
---- utl_add_column.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='UTL_ADD_COLUMN') 
DROP PROCEDURE @db_user.utl_add_column
go

CREATE  PROCEDURE [@db_user].[utl_add_column]
	-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
		@tipo_dato    Nvarchar(200),
		@val_default  Nvarchar(200),
	    @condizione_modifica_pregresso Nvarchar(200),
	    @condizione_check Nvarchar(200),
	    @RFU Nvarchar(200)
AS
BEGIN

	-- controllo tipo dato da oracle a sql server
	if SUBSTRING(@tipo_dato, 1,8) = 'varchar2' 	begin
		set @tipo_dato = (select replace(@tipo_dato, 'varchar2', 'varchar'))	end
	if SUBSTRING(@tipo_dato, 1,9) = 'nvarchar2'	begin
		set @tipo_dato = (select replace(@tipo_dato, 'nvarchar2', 'nvarchar'))	end
	if SUBSTRING(@tipo_dato, 1,6) = 'number'	begin
		set @tipo_dato = (select replace(@tipo_dato, 'number', 'numeric'))	end
	if SUBSTRING(@tipo_dato, 1,9) = 'timestamp'	begin
		set @tipo_dato = (select replace(@tipo_dato, 'timestamp', 'datetime'))	end
	if SUBSTRING(@tipo_dato, 1,4) = 'blob'		begin
		set @tipo_dato = (select replace(@tipo_dato, 'blob', 'image'))	end
	
DECLARE @istruzione Nvarchar(2000)
DECLARE @insert_log Nvarchar(2000)
DECLARE @data_eseguito Nvarchar(2000)
DECLARE @comando_richiesto Nvarchar(2000)
DECLARE @esito Nvarchar(2000)
	
	
if not exists(select * from syscolumns where name=@nome_colonna and id in
	(select id from sysobjects where name=@nome_tabella and xtype='U'))
	
BEGIN
-- aggiungo la colonna 
   	
   	IF @val_default is not null
	begin
		SET @val_default = 'CONSTRAINT DF_'+@nome_tabella+'_'+@nome_colonna+' DEFAULT ('''+@val_default+''')'
	end 		else 		begin
		set @val_default = ''
	end
   
   SET @istruzione = N'alter table [' + @nome_utente + '].
					[' + @nome_tabella + '] ADD ' + @nome_colonna +' 
					'+ @tipo_dato +' '+ @val_default

   execute sp_executesql @istruzione
   
   set @comando_richiesto = 'Added column ' +@nome_colonna+' on '+@nome_tabella+''
   set @esito = 'Esito positivo'
   
   
      
end 	else 	BEGIN	-- colonna gi esistente
	     set @comando_richiesto = 'Adding column ' +@nome_colonna+' on '+@nome_tabella+''
		 set @esito = 'ESITO NEGATIVO - Colonna gi ESISTENTE'
       
  	   end
execute [@db_user].utl_insert_log @nome_utente, getdate, @comando_richiesto, @versioneCD, @esito
	

END       
GO
           
---- utl_add_index.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_index('VERSIONE CD', --->es. 3.20.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME INDICE', --->es. IDX_TABELLA
--					'IS_UNIQUE', --->es. 1 (UNIQUE), '' (NON UNIQUE)
--					'NOME COLONNA 1', --->es. COLONNA_A (OBBLIGATORIO 1 COLONNA)
--					'NOME COLONNA 2', --->es. COLONNA_B (PER LASCIARE VUOTO, SCRIVA NULL)
--					'NOME COLONNA 3', --->es. COLONNA_C (PER LASCIARE VUOTO, SCRIVA NULL)
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 25/07/2011
-- Description:	
-- =============================================


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_add_index]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_add_index]
GO

CREATE PROCEDURE [@db_user].[utl_add_index]
-- Add the parameters for the stored procedure here
		@versioneCD    Nvarchar(200),
		@nome_utente   Nvarchar(200), 
		@nome_tabella  Nvarchar(200),
		@nome_index    Nvarchar(200),  
		@is_index_unique    Nvarchar(200), -- supply '1' for unique
		@nome_colonna1  Nvarchar(200),
	    @nome_colonna2 Nvarchar(200),
	    @nome_colonna3 Nvarchar(200),
	    @Index_Type Nvarchar(200),		-- supply CLUSTERED or NORMAL for NONCLUSTERED 
		@Ityp_Name Nvarchar(200),		-- n.a. valid only for Oracle 
		@Optional_Ityp_Parameters Nvarchar(200),  -- n.a. valid only for Oracle 
		@RFU Nvarchar(200)
		
AS
BEGIN
	
	    DECLARE @nome_indice   Nvarchar(2000)
		DECLARE @tableI Nvarchar(2000)
		DECLARE @colonnaI Nvarchar(2000)
		
		SET @tableI = SUBSTRING(@nome_tabella, 1,10)
	    SET @colonnaI = SUBSTRING(@nome_colonna1, 1,10)
	   
	   --	   set @nome_indice = 'IDX_'+@tableI+'_'+@colonnaI+''
	
	
	IF NOT EXISTS (SELECT name FROM sys.indexes
            WHERE name = @nome_indice)
		
	BEGIN
	DECLARE @istruzione Nvarchar(2000)
	DECLARE @insert_log Nvarchar(2000)
	DECLARE @data_eseguito Nvarchar(2000)
	DECLARE @esito Nvarchar(2000)
	DECLARE @unique Nvarchar(2000)
	DECLARE @ErrorVar INT
	DECLARE @colonne NVARCHAR(2000)
	DECLARE @comando_richiesto NVARCHAR(2000)  
	DECLARE @tipoindice  NVARCHAR(2000)  
	  
	----------------se la terza colonna viene inserita
	  if @nome_colonna3 is not null
	  begin
		SET @colonne =   @nome_colonna1+ @nome_colonna2+ @nome_colonna3
	  end
	  else
	----------------se la seconda colonna viene inserita
	  IF @nome_colonna2 is not null
	  begin
  		SET @colonne =   @nome_colonna1+@nome_colonna2
	  end
	  else
	  begin
	----------------se solo la prima colonna viene inserita	  
		SET @colonne = @nome_colonna1
	  end				
	  
 
	
	----------------se nella condizione "is_index_unique" viene impostato 1	
	  IF @is_index_unique = '1'
	  SET @unique = 'UNIQUE'
	  else
	  SET @unique = ''
	  
	   set @data_eseguito = (select convert (varchar, getdate(), 120))
	   	
	   	set @tipoindice = case @Index_Type when 'NORMAL' then 'NONCLUSTERED' else @Index_Type end
	   	
	   	SET @istruzione = N'CREATE '+@unique+' '+ @tipoindice  
		+ ' INDEX ['+@nome_indice+'] ON [' + @nome_utente + '].[' + @nome_tabella 
		+ '] ( '		+@colonne+ ' )'


	begin try
		execute sp_executesql @istruzione
		print @istruzione
	end try
	begin catch
--- Salva il numero dell'errore
	SELECT @ErrorVar = @@ERROR
	end catch

-------Condizioni degli errori
		IF @ErrorVar <> 0
    BEGIN
        IF @ErrorVar = 515
            BEGIN
                SET @esito = N'ESITO NEGATIVO - Non  possibile di modificare la colonna NON PIENA'
            END
        ELSE
        IF @ErrorVar = 156
            BEGIN
                SET @esito = @istruzione
            END
        ELSE
            BEGIN
                SET @esito = N'ERROR: error '
                    + RTRIM(CAST(@ErrorVar AS NVARCHAR(10)))
                    + N' occurred.';
            END
            end
-----------------------se non ci sono nessun errore.
	else
	begin 
        set @esito = 'Esito Positivo'
    end

		
		--SET @insert_log = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL VALUES (
	 --  '''+@data_eseguito+''',
  --     ''Added index ' +@nome_indice+' on '+@nome_tabella+''',
  --     '''+@versioneCD+''',
  --     '''+@esito+''')'


	   set @comando_richiesto = 'Added index ' +@nome_index+' on '+@nome_tabella+''
       
       execute @db_user.utl_insert_log NULL,NULL, @comando_richiesto, @versioneCD, @esito

       
          
	end
	
	else
---------------------se  gi esistente l'indice		
		   begin	
			
			   --SET @insert_log = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL VALUES (
			   --'''+@data_eseguito+''',
			   --''Added index ' +@nome_indice+' on '+@nome_tabella+''',
			   --'''+@versioneCD+''',
			   --''ESITO NEGATIVO - Indice GIA esistente'')'
	       
		   set @comando_richiesto = 'Adding index ' +@nome_indice+' on '+@nome_tabella+''
		   set @esito = 'ESITO NEGATIVO - Indice GIA esistente'
		   execute @db_user.utl_insert_log NULL,NULL, @comando_richiesto, @versioneCD, @esito
		   end
	

END
GO

              
---- Utl_Insert_Chiave_Config.MSSQL.sql  marcatore per ricerca ----
-- function used by Utl_Insert_Chiave_Config and other 2 Utl_Insert_Chiave%
IF EXISTS (SELECT * FROM sys.objects where type = 'FN' and name = 'UTL_ISVALORE_LT_COLUMN') 
DROP FUNCTION @db_user.[Utl_Isvalore_Lt_Column]
GO


--utl_insert_log
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_insert_log]'))
DROP PROCEDURE [@db_user].[utl_insert_log]
GO

CREATE  PROCEDURE [@db_user].[utl_insert_log]
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


EXEC @db_user.utl_add_column  '3.20.1', '@db_user' ,  'DPA_CHIAVI_CONFIGURAZIONE',    'DTA_INSERIMENTO',    'DATETIME',    'GETDATE()',    Null,    Null,   Null    
go
execute  @db_user.utl_add_column  '3.20.1',   '@db_user' ,    'DPA_CHIAVI_CONFIGURAZIONE',    'VERSIONE_CD',    'varchar(32)',    NULL,    Null,    Null,   Null    
go

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


IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='UTL_INSERT_CHIAVE_CONFIG')
DROP PROCEDURE @db_user.[Utl_Insert_Chiave_Config]
go

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
    @Forza_Update  VARCHAR(1) , 
    @RFU VARCHAR(10) ) As  

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

SET @Codice_Old_Webconfig = ISNULL(@Codice_Old_Webconfig,'NULL')

  If (@Cnt         = 0 and @Globale    = '1') 
  begin -- inserisco la chiave globale non esistente
  INSERT INTO  DOCSPA.DPA_CHIAVI_CONFIGURAZIONE
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

BEGIN


  declare @Cnt Int

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
              
---- ALTER_DPA_AREA_CONSERVAZIONE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:				VELTRI
Data creazione:			23/11/2011
Scopo della modifica:		Inserimento nuovi campi per gestire le policy di conservazione

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
------------- tradotto da Gabriele SERPI
begin
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'ID_POLICY', 'NUMERIC(10)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'CONSOLIDA', 'CHAR(1)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'ID_POLICY_VALIDAZIONE', 'NUMERIC(10)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'VAR_FILE_CHIUSURA', 'VARCHAR(MAX)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'VAR_FILE_CHIUSURA_FIRMATO', 'VARCHAR(MAX)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_AREA_CONSERVAZIONE', 'IS_PREFERRED', 'CHAR(1)', NULL, NULL, NULL, NULL
END
GO              
----------- FINE -
              
---- ALTER_DPA_CORR_GLOBALI.MSSQL.sql  marcatore per ricerca ----
-- l'operazione di inizializzazione successiva va fatta solo all'atto della creazione della nuova colonna! 
-- quindi prima di creare la colonna controllo se la colonna non c'
BEGIN
declare @cntcol int
select @cntcol = COUNT(*)  from syscolumns where name='VAR_DESC_CORR_OLD' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U')
EXECUTE @db_user.utl_add_column '3.20.1', '@db_user', 'DPA_CORR_GLOBALI', 'VAR_DESC_CORR_OLD', 'VARCHAR(256)', NULL, NULL, NULL, NULL

		IF @cntcol = 0 
		BEGIN

		--	print @cntcol 
		DISABLE TRIGGER @db_user.RoleHistoryModify on @db_user.DPA_CORR_GLOBALI
		execute sp_executesql	
		N'update @db_user.DPA_CORR_GLOBALI set  VAR_DESC_CORR_OLD = VAR_DESC_CORR where VAR_DESC_CORR_OLD is null'
	
		END
END
GO

ENABLE TRIGGER @db_user.RoleHistoryModify on @db_user.DPA_CORR_GLOBALI
GO

              
----------- FINE -
              
---- ALTER_DPA_FORMATI_DOCUMENTO.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:						FAILLACE
Data creazione:				07/11/2011
Scopo della modifica:		INSERITI CAMPO "FILE_TYPE_VALIDATION" PER LA GESTIONE DELLA VERIFICA TIPO FILE

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
if not exists (SELECT * FROM syscolumns WHERE name='FILE_TYPE_VALIDATION' 	and id in 
(SELECT id FROM sysobjects 	WHERE name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_FORMATI_DOCUMENTO] ADD FILE_TYPE_VALIDATION INTEGER NULL
END
GO              
----------- FINE -
              
---- ALTER_DPA_ITEMS_CONSERVAZIONE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:				VELTRI
Data creazione:			23/11/2011
Scopo della modifica:		Inserimento nuovi campi per gestire le policy di conservazione

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
------------- tradotto da Gabriele SERPI
begin
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_ITEMS_CONSERVAZIONE', 'POLICY_VALIDA', 'CHAR(1)', NULL, NULL, NULL, NULL
execute @db_user.utl_add_column '3.20', '@db_user', 'DPA_ITEMS_CONSERVAZIONE', 'VALIDAZIONE_FIRMA', 'CHAR(1)', NULL, NULL, NULL, NULL
END
GO
              
----------- FINE -
              
---- ALTER_PEOPLE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	INSERITI CAMPO "ABILITATO_CENTRO_SERVIZI" PER LA GESTIONE DELL'ACCESSO DI UN UTENTE AL MODULO CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
------------- tradotto da Gabriele SERPI
begin
execute @db_user.utl_add_column '3.20', '@db_user', 'PEOPLE', 'ABILITATO_CENTRO_SERVIZI', 'CHAR(1)', NULL, NULL, NULL, NULL
END
GO              
----------- FINE -
 
 
 -- sab
 
 SET ARITHABORT ON;
 GO

             
---- ALTER_PROFILE.MSSQL.SQL  marcatore per ricerca ----
--alter table profile add isnull_dta_proto_creation_date as isnull(dta_proto,creation_date) PERSISTED
begin
execute @db_user.utl_add_column '3.20', '@db_user'
, 'PROFILE'
, 'isnull_dta_proto_creation_date'
, ' as isnull(dta_proto,creation_date) PERSISTED ', NULL, NULL, NULL, NULL
END
GO

--create index INDX_isnull_dta_proto_creation_date on Profile (isnull_dta_proto_creation_date desc) 
begin
execute @db_user.utl_add_index '3.20', '@db_user'
, 'PROFILE'
, 'INDX_isnull_dta_proto_creation_date'
, ''
, 'isnull_dta_proto_creation_date desc' , NULL, NULL
, NULL, NULL, NULL, NULL
END
GO


              
----------- FINE -
              
---- CREATE_DPA_ASS_POLICY_PROFILAZIONE.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_POLICY_PROFILAZIONE]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE [@db_user].[DPA_ASS_POLICY_PROFILAZIONE]
                             (ID_POLICY   integer,
							ID_TEMPLATE   integer,
							ID_OBJ_CUSTOM integer,
							VALORE        VARCHAR(2000) ) 
END 
GO


              
----------- FINE -
              
---- CREATE_DPA_ASS_POLICY_TYPE.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_POLICY_TYPE]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.DPA_ASS_POLICY_TYPE
(
  ID_POLICY  INT IDENTITY(1,1),
  ID_TYPE    INT
) 

END
GO              
----------- FINE -
              
---- CREATE_DPA_COPY_LOG.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_COPY_LOG]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.DPA_COPY_LOG( 
						  THING             integer NOT NULL, 
						  ID_RUOLO_DEST     integer NOT NULL, 
						  ID_RUOLO_ORIG     integer NOT NULL, 
						  ACCESSRIGHTS      integer NOT NULL, 
						  CHA_TIPO_DIRITTO  CHAR(1) ,  
						  VAR_NOTE_SEC      VARCHAR(256), 
						  TS_COPY           datetime                DEFAULT CURRENT_TIMESTAMP
						  Constraint PK_Dpa_Copy_Log Primary Key (Thing, Id_Ruolo_Dest, Accessrights)     ) 
						   
END
GO


						              
----------- FINE -
              
---- CREATE_DPA_DETT_POLICY_ESECUZIONE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Veltri
Data creazione:		23/11/2011
Scopo della modifica:	TABELLA PER GESTIONE DELLA POLICY DI CONSERVAZIONE

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_POLICY_ESECUZIONE]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.DPA_POLICY_ESECUZIONE
(
   SYSTEM_ID           INT IDENTITY(1,1),
   ID_POLICY           INT,
   START_EXECUTE_DATE  DATETIME,
   END_EXECUTE_DATE    DATETIME,
   N_OBJ_CONSERVATI    INT,
   ID_ULTIMO_OBJ       INT,
   ID_PRIMO_OBJ        INT,
   ID_ISTANZA          INT,
   ID_AMM              INT
)

END
GO

              
----------- FINE -
              
---- CREATE_DPA_POLICY_ESECUZIONE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Veltri
Data creazione:		23/11/2011
Scopo della modifica:	TABELLA PER GESTIONE DELLA POLICY DI CONSERVAZIONE

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_POLICY_ESECUZIONE]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.DPA_POLICY_ESECUZIONE
(
   SYSTEM_ID           INT,
   ID_POLICY           INT,
   START_EXECUTE_DATE  DATETIME,
   END_EXECUTE_DATE    DATETIME,
   N_OBJ_CONSERVATI    INT,
   ID_ULTIMO_OBJ       INT,
   ID_PRIMO_OBJ        INT,
   ID_ISTANZA          INT,
   ID_AMM              INT
)

END
GO              
----------- FINE -
              
---- CREATE_INDEX_indx_ass1_temp1.MSSQL.sql  marcatore per ricerca ----

/* AUTORE:					P. De Luca			
--CREATE INDEX @db_user.indx_ass1_temp1 ON @db_user.DPA_ASSOCIAZIONE_TEMPLATES
			(to_number(doc_number))

Data creazione:				31 marzo 2012
Scopo della modifica:		miglioramento select su campi profilati
Indicazione della MEV o dello sviluppo a cui  collegata la modifica: N.A.
-- fine commenti
*/

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'INDX_ASS1_TEMP1') 
DROP INDEX @db_user.DPA_ASSOCIAZIONE_TEMPLATES.INDX_ASS1_TEMP1
go

CREATE INDEX INDX_ASS1_TEMP1 ON @db_user.DPA_ASSOCIAZIONE_TEMPLATES (doc_number )
GO


              
----------- FINE -
              
---- CREATE_PGU_ENTI.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	TABELLA PER PANNELLO GRAFICO UNIFICATO DEL CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/

IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[PGU_ENTI]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE [@db_user].[PGU_ENTI](
	[ID] [int] NOT NULL,
	[NOME] [varchar](50) NOT NULL,
	[URLAPPCONSPROTOCOLLO] [varchar](2000) NULL,
	[URLAPPCONSALTREAPP] [varchar](2000) NULL,
	[URLWSCONSPROTOCOLLO] [varchar](2000) NULL,
	[URLWSCONSALTREAPP] [varchar](2000) NULL,
	[DESCRIZIONE] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(	[ID] ASC)
) 
END

GO

              
----------- FINE -
              
---- CREATE_PGU_ENTI_UTENTI.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	TABELLA PER PANNELLO GRAFICO UNIFICATO DEL CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/

IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[PGU_ENTI_UTENTI]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.PGU_ENTI_UTENTI
(
  ID        INT IDENTITY(1,1) PRIMARY KEY,
  IDENTE    INT                             NOT NULL,
  IDUTENTE  INT                             NOT NULL
)
END
GO


--EXECUTE 	@db_user.UTL_ADD_FOREIGN_KEY '3.20.1', '@db_user', 'PGU_ENTI_UTENTI', 'IDUTENTE', 'PGU_UTENTI', 'ID', 'ON DELETE CASCADE', NULL
--GO 

--EXECUTE	@db_user.UTL_ADD_FOREIGN_KEY '3.20.1', '@db_user', 'PGU_ENTI_UTENTI', 'IDENTE', 'PGU_ENTI', 'ID', 'ON DELETE CASCADE', NULL
--GO
              
----------- FINE -
              
---- CREATE_PGU_UTENTI.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	TABELLA PER PANNELLO GRAFICO UNIFICATO DEL CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/

/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	TABELLA PER PANNELLO GRAFICO UNIFICATO DEL CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/


IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[PGU_UTENTI]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.PGU_UTENTI
(
   ID                   INT  IDENTITY(1,1) PRIMARY KEY,
   NOME                 VARCHAR(50)         NOT NULL,
   DESCRIZIONE          VARCHAR(255),
   PASSWORD             VARCHAR(50)         NOT NULL,
   AMMINISTRATORE       CHAR(1)              NOT NULL,
   SECRETKEY            VARCHAR(255),
   SECRETIV             VARCHAR(255),
   SUPERAMMINISTRATORE  CHAR(1)
)
END
GO


              
----------- FINE -
              
---- CREATE_POLICY_CONSERVAZIONE.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Veltri
Data creazione:		23/11/2011
Scopo della modifica:	TABELLA PER GESTIONE DELLA POLICY DI CONSERVAZIONE

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[POLICY_CONSERVAZIONE]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.POLICY_CONSERVAZIONE
(
  SYSTEM_ID              int,
  TIPO                   VARCHAR(100),
  ID_AMM                 VARCHAR(100),
  NOME                   VARCHAR(200),
  CLASSIFICA             int,
  ID_TEMPLATE            int,
  ID_STATO               int,
  ID_RF                  int,
  ARRIVO                 VARCHAR(1),
  PARTENZA               VARCHAR(1),
  INTERNO                VARCHAR(1),
  GRIGIO                 VARCHAR(1),
  CONSOLIDAZIONE         VARCHAR(1),
  ID_AOO                 int,
  DTA_DATA_CREAZIONE_DA  DATE,
  DTA_DATA_CREAZIONE_A   DATE,
  DTA_DATA_PROTO_DA      DATE,
  DTA_DATA_PROTO_A       DATE,
  PERIODO_TIPO           VARCHAR(100),
  P_G_GIORNI             VARCHAR(10),
  P_G_ORA_HH             VARCHAR(10),
  P_G_ORA_MM             VARCHAR(10),
  P_S_LUNEDI             VARCHAR(1),
  P_S_MARTEDI            VARCHAR(1),
  P_S_MERCOLEDI          VARCHAR(1),
  P_S_GIOVEDI            VARCHAR(1),
  P_S_VENERDI            VARCHAR(1),
  P_S_SABATO             VARCHAR(1),
  P_S_DOMENICA           VARCHAR(1),
  P_S_ORA_HH             VARCHAR(10),
  P_S_ORA_MM             VARCHAR(10),
  P_M_GIORNI             VARCHAR(20),
  P_M_ORA_HH             VARCHAR(10),
  P_M_ORA_MM             VARCHAR(10),
  ID_RUOLO               int,
  ATTIVA                 VARCHAR(1),
  MESI_AVVISO            VARCHAR(100),
  ID_PEOPLE              integer,
  TIPO_CLASSIFICAZIONE   VARCHAR(10),
  TIPO_DATA_CREAZIONE    VARCHAR(100),
  TIPO_DATA_PROTO        VARCHAR(100),
  ID_UO_CREATORE         integer,
  UO_SOTTOPOSTE          varchar(10),
  STATO_INVIATA          varchar(10),
  INCLUDI_SOTTONODI      CHAR(1),
  SOLO_DIGITALI          CHAR(1),
  SOLO_FIRMATI           CHAR(1),
  P_A_GIORNI             varchar(20),
  P_A_MESE               varchar(20),
  P_A_ORE                varchar(20),
  P_A_MINUTI             varchar(20),
  TIPO_CONSERVAZIONE     Nvarchar(200))
END
go

              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- dpa_anagrafica_funzioni.MSSQL.sql  marcatore per ricerca ----
--Creazione nuova funzione DPA_ANAGRAFICA_FUNZIONI

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
					where COD_FUNZIONE='IMP_DOC_MASSIVA_PREG')
	)
BEGIN		
		insert into @db_user.DPA_ANAGRAFICA_FUNZIONI
				(COD_FUNZIONE, VAR_DESC_FUNZIONE
		, CHA_TIPO_FUNZ, DISABLED)
		values ('IMP_DOC_MASSIVA_PREG','Abilita l''importazione dei documenti Pregressi'
		, NULL, 'N')
END
GO 

              
----------- FINE -
              
---- ins_DPA_CHIAVI_CONFIGURAZIONE.MSSQL.SQL  marcatore per ricerca ----
--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
			WHERE VAR_CODICE ='MAX_ROW_SEARCHABLE')
BEGIN
Insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG]
           ,DTA_INSERIMENTO
           ,VERSIONE_CD)
	values 
	(0,
	'MAX_ROW_SEARCHABLE',
	'Numero massimo righe per le ricerche',
	'0',
	'F',
	'1',
	'1',
	'1',
	NULL,
	GETDATE(),
	'3.20.1'
	)
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
              
---- INSERT_DPA_ANAGRAFICA_LOG.MSSQL.SQL  marcatore per ricerca ----
if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='AMM_COPY'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ('AMM_COPY', 'Log copia visibilit', 'UTENTE', 'AMMCOPY')
END
GO 


if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='INVIO_ISTANZA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   (  'INVIO_ISTANZA','Ricezione dell''istanza da parte del Centro Servizi',    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='VERIFICA_INT_FORMATO_FILE'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   (  'VERIFICA_INT_FORMATO_FILE',    'Effettuazione della verifica di conformit dei formati dei file',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='VERIFICA_INT_CONTENUTO_FILE'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ( 'VERIFICA_INT_CONTENUTO_FILE',    'Verifica di conformit del contenuto dei file rispetto ai formati',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='VERIFICA_VALIDITA_FIRMA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ( 'VERIFICA_VALIDITA_FIRMA',    'Verifica di validit della firma digitale',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='VERIFICA_VALIDITA_MARCA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   (  'VERIFICA_VALIDITA_MARCA',    'Verifica della validit della marca temporale',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='ACCETTAZIONE_ISTANZA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ('ACCETTAZIONE_ISTANZA',    'Accettazione dell''istanza da parte del Centro Servizi',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='RIFIUTO_ISTANZA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ( 'RIFIUTO_ISTANZA',    'Rifiuto dell''istanza da parte del Centro Servizi',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='FIRMA_ISTANZA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ( 'FIRMA_ISTANZA',    'Apposizione della firma del responsabile della conservazione',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='MARCA_ISTANZA'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   ( 'MARCA_ISTANZA',    'Apposizione della marca temporale all''istanza da parte del responsabile della conservazione',
    'CONSERVAZIONE',     '')
END
GO 

if (not exists (select * from  [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='CREAZIONE_STORAGE'))
BEGIN
	Insert into  [@db_user].DPA_ANAGRAFICA_LOG
	   (VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
	 Values
	   (  'CREAZIONE_STORAGE',    'Creazione dello storage',
    'CONSERVAZIONE',     '')
END
GO 

              
----------- FINE -
              
---- INSERT_DPA_TIPI_SUPPORTO.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Veltri
Data creazione:		23/11/2011
Scopo della modifica:	Campo per l'abilitazione del pannello "Conservazione" in amministrazione

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
CREATA PER LA MEV "CONSERVAZIONE"
*/
begin
   DECLARE @cnt INT
   select   @cnt = count(*)
   from @db_user.DPA_TIPO_SUPPORTO l
   where l.VAR_TIPO = 'REMOTO'
   if (@cnt = 0)
   begin
      select   @cnt = sum(cntdati)+1 from(select max(SYSTEM_ID) as cntdati 
      from @db_user.DPA_TIPO_SUPPORTO
         union select 1) AS TabAl
      INSERT INTO @db_user.DPA_TIPO_SUPPORTO(SYSTEM_ID,	VAR_TIPO,	VAR_DESCRIZIONE)
VALUES(CASE WHEN (1 > @cnt) THEN 1
   ELSE @cnt
      END,	'REMOTO',	'Supporto remoto')
   end

   select   @cnt = count(*)
   from @db_user.DPA_TIPO_SUPPORTO l
   where l.VAR_TIPO = 'RIMOVIBILE'
   if (@cnt = 0)
   begin
      select   @cnt = sum(cntdati)+1 from(select max(SYSTEM_ID) as cntdati 
      from @db_user.DPA_TIPO_SUPPORTO
         union select 1) AS TabAl
      INSERT INTO @db_user.DPA_TIPO_SUPPORTO(SYSTEM_ID,	VAR_TIPO,	VAR_DESCRIZIONE)
VALUES(CASE WHEN (2 > @cnt) THEN 2
   ELSE @cnt
      END,	'RIMOVIBILE',	'Supporto rimovibile')
   end

end

GO              

              
----------- FINE -
              
---- INSERT_DPA_VOCI_MENU_ADMIN.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Veltri
Data creazione:		23/11/2011
Scopo della modifica:	Campo per l'abilitazione del pannello "Conservazione" in amministrazione

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
--tradotto da Serpi Gabriele

IF NOT EXISTS (SELECT * FROM @db_user.DPA_VOCI_MENU_ADMIN 
			WHERE VAR_CODICE ='FE_ABILITA_POLICY_CONSERVAZIONE')
BEGIN
Insert into @db_user.DPA_VOCI_MENU_ADMIN
		(VAR_CODICE
		,VAR_DESCRIZIONE
		)
		values
		('FE_ABILITA_POLICY_CONSERVAZIONE'
		,'Abilita il men della conservazione')
END
GO
              
----------- FINE -
              
---- INSERT_PGU_UTENTI.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	TABELLA PER PANNELLO GRAFICO UNIFICATO DEL CENTRO SERVIZI

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/

begin
   DECLARE @cnt INT
   select   @cnt = count(*) from @db_user.PGU_UTENTI l where l.NOME = 'sa'
   if (@cnt = 0)
      Insert into @db_user.PGU_UTENTI(NOME, DESCRIZIONE, PASSWORD, AMMINISTRATORE, SECRETKEY, SECRETIV, SUPERAMMINISTRATORE)
Values('sa', 'Amministratore di sistema', 'IFpQ+uuzyoZhG3cxq6+iIg==', '0', 'usVwz06DbY+t7EmcfOpTZde2ukEvl9oMXtwM8wwSQ1U=', 'csCWAN2yfwduCZC2yRsHDA==', '1')
--COMMIT;

END

GO
              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- AtLeastOneCartaceo.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[AtLeastOneCartaceo]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
			DROP FUNCTION @db_user.AtLeastOneCartaceo
GO

CREATE FUNCTION @db_user.AtLeastOneCartaceo(@docnum INT)
RETURNS VARCHAR(4000)
AS
BEGIN

   DECLARE @isCartaceo   VARCHAR(16)
   DECLARE @error INT
   DECLARE @no_data_err INT
   BEGIN
      DECLARE @vmaxidgenerica   INT
      SELECT   @vmaxidgenerica = MAX(v1.version_id)
      FROM VERSIONS v1, components c
      WHERE v1.docnumber = @docnum AND v1.version_id = c.version_id
      SELECT   @isCartaceo = cartaceo
      FROM VERSIONS
      WHERE docnumber = @docnum AND version_id = @vmaxidgenerica
   end

   if(@isCartaceo = '1')
   begin
      DECLARE @item VARCHAR(255)
      DECLARE @maxVersion INT
      DECLARE @curAllegato CURSOR
      SET @curAllegato = CURSOR  FOR select system_id from profile where id_documento_principale = @docnum
      OPEN @curAllegato
      while 1 = 1
      begin
         FETCH @curAllegato INTO @item
         if (@@FETCH_STATUS <> 0 or @isCartaceo = '0')
         BREAK
         SELECT   @maxVersion = MAX(v1.version_id)
         FROM VERSIONS v1, components c
         WHERE v1.docnumber = @item AND v1.version_id = c.version_id
         SELECT   @isCartaceo = cartaceo
         FROM VERSIONS
         WHERE docnumber = @item AND version_id = @maxVersion
      end
      CLOSE @curAllegato
   END


   RETURN @isCartaceo

End 

GO


              
----------- FINE -
              
---- AtLeastOneFirmato.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[AtLeastOneFirmato]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.AtLeastOneFirmato
GO


CREATE FUNCTION @db_user.AtLeastOneFirmato(@docnum INT)
RETURNS VARCHAR(4000)
AS
BEGIN

   DECLARE @isFirmato   VARCHAR(16)
   DECLARE @error INT
   DECLARE @no_data_err INT
   BEGIN
      DECLARE @vmaxidgenerica   INT
      SELECT   @vmaxidgenerica = MAX(v1.version_id)
      FROM VERSIONS v1, components c
      WHERE v1.docnumber = @docnum AND v1.version_id = c.version_id
      SELECT   @isFirmato = cha_firmato
      FROM components
      WHERE docnumber = @docnum AND version_id = @vmaxidgenerica
   end

   if(@isFirmato = '0')
   begin
      DECLARE @item VARCHAR(255)
      DECLARE @maxVersion INT
      DECLARE @curAllegato CURSOR
      SET @curAllegato = CURSOR  FOR select system_id from profile where id_documento_principale = @docnum
      OPEN @curAllegato
      while 1 = 1
      begin
         FETCH @curAllegato INTO @item
         if (@@FETCH_STATUS <> 0 or @isFirmato = '1')
         BREAK
         SELECT   @maxVersion = MAX(v1.version_id)
         FROM VERSIONS v1, components c
         WHERE v1.docnumber = @item AND v1.version_id = c.version_id
         SELECT   @isFirmato = cha_firmato
         FROM components
         WHERE docnumber = @item AND version_id = @maxVersion
      end
      CLOSE @curAllegato
   END


   RETURN @isFirmato

End 

GO




              
----------- FINE -
              
---- getCodFis.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getCodFis]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.getCodFis
GO


CREATE FUNCTION @db_user.getCodFis (@corrId INT)
RETURNS varchar 
AS 

BEGIN
declare @risultato varchar(16)

select  @risultato = a.var_cod_fiscale 
	from dpa_dett_globali a where a.id_corr_globali=@corrId

RETURN @risultato
END 

GO

              
----------- FINE -
              
---- Getdocnameorcodfasc.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[Getdocnameorcodfasc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.Getdocnameorcodfasc
GO


CREATE Function @db_user.Getdocnameorcodfasc
(@Id INT)

RETURNS VARCHAR(4000)   

AS
Begin

/* dato un ID, o  un documento oppure un fascicolo 
INFATTI QUESTA QUERY TORNA ZERO RECORD: 
Select System_Id From ProJECT
Intersect
Select System_Id From Profile */

   DECLARE @Returnvalue VARCHAR(2000) 
   DECLARE @Mytipo_Proj VARCHAR(20)  
   DECLARE @Myid_Fascicolo VARCHAR(20)  
   DECLARE @myId_Parent VARCHAR(20)
   DECLARE @error INT
   DECLARE @no_data_err INT  
   Select   @Returnvalue = ISNULL(Docname,system_id) From Profile
   Where System_Id = @Id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
      return  @Returnvalue
   IF (@error <> 0)
   begin
      SET @Returnvalue = Null -- richiesta esplicita che non si intercetti l'eccezione
      return  @Returnvalue
   end
ELSE IF (@no_data_err = 0)
   begin
      Select   @Mytipo_Proj = cha_tipo_Proj, @Myid_Fascicolo = id_Fascicolo, @myId_Parent = Id_Parent
      From Project   Where System_Id = @Id
      If @Mytipo_Proj = 'F'
         Select   @Returnvalue = 'Fascicolo\' + ISNULL(Var_Codice,Description)
         From Project   Where System_Id = @Id

      If @Mytipo_Proj = 'C'
      begin
         If @Myid_Fascicolo = @myId_Parent
            Select   @Returnvalue = 'CartellaPrincipale\'+ ISNULL(Var_Codice,Description)
            From Project   Where System_Id = @Id

         If @Myid_Fascicolo <> @myId_Parent
            Select   @Returnvalue = 'SottoFascicolo\'+ ISNULL(Var_Codice,Description)
            From Project   Where System_Id = @Id
      end

      return  @Returnvalue
   end
   RETURN 0 
End 

GO






              
----------- FINE -
              
---- getImpronta.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getImpronta]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.[getImpronta]
GO


CREATE FUNCTION [@db_user].[getImpronta] (      @docnum int)
RETURNS varchar (2000)
AS
BEGIN
      DECLARE @vmaxidgenerica int
      Declare @impronta varchar(2000)

      SELECT @vmaxidgenerica = MAX (v1.version_id)
      FROM VERSIONS v1, components c
      WHERE v1.docnumber = @docnum AND v1.version_id = c.version_id

      SELECT @impronta = var_impronta
      FROM components
      WHERE docnumber = @docnum AND version_id = @vmaxidgenerica

      RETURN @impronta

END
GO

              
----------- FINE -
              
---- getInConservazione.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getInConservazione]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.getInConservazione
GO


CREATE FUNCTION  @db_user.getInConservazione
(@IDPROFILE INT,@Idproject INT, 
@typeID CHAR(2000), @idPeople INT, 
@idGruppo INT)

RETURNS INT AS
BEGIN
   DECLARE @risultato INT
   DECLARE @res_appo INT
   DECLARE @idRuoloInUo INT
   DECLARE @error INT
   DECLARE @no_data_err INT
   begin
      SELECT   @idRuoloInUo = DPA_CORR_GLOBALI.SYSTEM_ID 
      FROM DPA_CORR_GLOBALI WHERE DPA_CORR_GLOBALI.ID_GRUPPO = @idGruppo
      SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      IF (@error = 0  and  @no_data_err <> 0)
      begin
         IF (@typeID = 'D' AND @Idproject is null)
         begin
            SELECT   @risultato = COUNT(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID) FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE
            DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROFILE = @IDPROFILE
            AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N' AND DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
            DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo  AND ID_PROJECT IS  NULL
            SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
         end
      ELSE IF (@typeID = 'D' AND @Idproject is NOT null)
         begin
            SELECT   @risultato = COUNT(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID) FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE
            DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROFILE = @IDPROFILE
            AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N' AND DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
            DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo  AND ID_PROJECT = @Idproject
            SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
         end
         IF (@error = 0  and @no_data_err <> 0)
         begin
            IF (@typeID = 'F')
            begin
               SELECT   @risultato = COUNT(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID) FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE
               DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROJECT = @Idproject
               AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N'
               AND DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
               DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo
               SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
            end
            IF (@error = 0  and @no_data_err <> 0)
            IF (@risultato > 0)
               SET @risultato = 1
         ELSE
            SET @risultato = 0
         end
      end
      IF (@error <> 0)
         SET @risultato = 0
   ELSE IF (@no_data_err = 0)
         SET @risultato = 0
   end
   RETURN @risultato
END 

GO




              
----------- FINE -
              
---- getInConservazioneNoSec.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getInConservazioneNoSec]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP FUNCTION @db_user.getInConservazioneNoSec
GO


CREATE FUNCTION @db_user.getInConservazioneNoSec
(@IDPROFILE INT,@Idproject INT, @typeID CHAR(2000))
RETURNS VARCHAR(4000) AS
BEGIN

--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
   DECLARE @result VARCHAR(3000)
   DECLARE @item VARCHAR(3000)
   DECLARE @curCasellaDiSelezione CURSOR
   if (@typeID = 'D')
   BEGIN

      SET @curCasellaDiSelezione = CURSOR  FOR select ID_CONSERVAZIONE  from DPA_ITEMS_CONSERVAZIONE,DPA_AREA_CONSERVAZIONE where DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROFILE = @IDPROFILE AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N'
      OPEN @curCasellaDiSelezione
      while 1 = 1
      begin
         FETCH @curCasellaDiSelezione INTO @item
         if @@FETCH_STATUS <> 0
         BREAK
         IF(@result IS NOT NULL)
            SET @result = @result+'-'+@item 
      ELSE
         SET @result = @result+@item
      end
      CLOSE @curCasellaDiSelezione
   END
else if (@typeID = 'F')
   BEGIN
      SET @curCasellaDiSelezione = CURSOR  FOR select ID_CONSERVAZIONE  from DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROJECT = @Idproject AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N' group by ID_CONSERVAZIONE
      OPEN @curCasellaDiSelezione
      while 1 = 1
      begin
         FETCH @curCasellaDiSelezione INTO @item
         if @@FETCH_STATUS <> 0
         BREAK
         IF(@result IS NOT NULL)
            SET @result = @result+'-'+@item 
      ELSE
         SET @result = @result+@item
      end
      CLOSE @curCasellaDiSelezione
   END

   RETURN @result

END 

GO

              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- CopySecurity.MSSQL.sql  marcatore per ricerca ----
--begin
--Utl_Backup_Sp('CopySecurity', '3.20.1');
--end;
--/ 


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[CopySecurity]') AND type in (N'P', N'PC'))
DROP PROCEDURE [@db_user].[CopySecurity]
GO

create PROCEDURE @db_user.CopySecurity 
-- Id gruppo del ruolo di cui copiare la visibilita'
@sourceGroupId INT ,
-- Id gruppo del ruolo di destinazione
@destinationGroupId INT 
AS
BEGIN
/******************************************************************************

AUTHOR:   Samuele Furnari
NAME:     CopySecurity
PURPOSE:  Store per la copia di alcuni record della security. I record 
copiati avranno come tipo diritto 'A' 

******************************************************************************/


-- Nome della colonna letta dalla tabella dei metadati
   DECLARE @colName VARCHAR(2000)

-- Lista separata da , dei nomi delle colonne in cui eseguire la insert
   DECLARE @colNameList VARCHAR(4000)

-- Lista separata da , dei valori da assegnare alle colonne
   DECLARE @colValuesList VARCHAR(4000)

-- Selezione delle colonne della security dalla tabella dei metadati
   DECLARE @curColumns CURSOR
   SET @curColumns = CURSOR  FOR SELECT cname from col where tname = 'SECURITY' order by colno asc
   OPEN @curColumns
   while 1 = 1
   begin
      FETCH @curColumns INTO @colName
      if @@FETCH_STATUS <> 0
      BREAK

-- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
-- inserito il valore modificato altrimenti viene lasciata com'
      SET @colNameList = @colNameList + ', ' + @colName
      IF(@colName) = 'THING'
      SET @colValuesList = @colValuesList + 'DISTINCT(thing)'
      ELSE IF(@colName) = 'PERSONORGROUP'
      SET @colValuesList = @colValuesList + ', ' + @destinationGroupId
      ELSE IF(@colName) = 'ACCESSRIGHTS'
      SET @colValuesList = @colValuesList + 'DECODE(accessrights, 255, 63, accessrights, accessrights)'
      ELSE IF(@colName) = 'CHA_TIPO_DIRITTO'
      SET @colValuesList = @colValuesList + ', ''A'''
      ELSE
      SET @colValuesList = @colValuesList + ', ' + @colName
   end
   CLOSE @curColumns
   SET @colNameList = SUBSTRING(@colNameList,3,LEN(@colNameList) -3)
   SET @colValuesList = SUBSTRING(@colValuesList,3,LEN(@colValuesList) -3)
   execute('INSERT INTO security (' + @colNameList + ') ( SELECT ' + @colValuesList + ' FROM security s WHERE personorgroup = ' + @sourceGroupId + ' AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' + @destinationGroupId + '))')  
END 

GO

              
----------- FINE -
              
---- create_sp_modify_uo_int_codice.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[sp_modify_uo_int_codice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [@db_user].[sp_modify_uo_int_codice]
GO


CREATE PROCEDURE @db_user.sp_modify_uo_int_codice 
@idcorrglobale     INT       ,
@desc_corr         VARCHAR(4000)       ,
@codice_aoo        VARCHAR(4000)       ,
@codice_amm        VARCHAR(4000)       ,
@indirizzo         VARCHAR(4000)       ,
@cap               VARCHAR(4000)       ,
@provincia         VARCHAR(4000)       ,
@nazione           VARCHAR(4000)       ,
@citta             VARCHAR(4000)       ,
@telefono          VARCHAR(4000)       ,
@telefono2         VARCHAR(4000)       ,
@fax               VARCHAR(4000)       ,
@cod_old           VARCHAR(4000)       ,
@newid             INT OUTPUT      ,
@returnvalue       INT OUTPUT 
AS
BEGIN
   DECLARE @error INT
   DECLARE @no_data_err INT
   DECLARE @cod_rubrica              VARCHAR(128)
   DECLARE @cod_rubrica_old          VARCHAR(128)
   DECLARE @id_reg                   INT
   DECLARE @idamm                    INT
   DECLARE @new_var_cod_rubrica      VARCHAR(128)
   DECLARE @cha_dettaglio            CHAR(1)       = '0'
   DECLARE @cha_tipourp              CHAR(1)
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
   DECLARE @cha_tipocorr             CHAR(1)
   DECLARE @chapa            CHAR(1)
   DECLARE @varcodaoo                VARCHAR(16)
   DECLARE @varcodamm                VARCHAR(32)
   DECLARE @varemail                 VARCHAR(128)
   --<< reperimento_dati2 >>
   BEGIN
      SELECT   @cod_rubrica = var_cod_rubrica, @cha_tipourp = cha_tipo_urp, @id_reg = id_registro, @idamm = id_amm, @chapa = cha_pa, @chaTipoIE = cha_tipo_ie, @numLivello = num_livello, @idParent = id_parent, @idPesoOrg = id_peso_org, @idUO = id_uo, @idTipoRuolo = id_tipo_ruolo, @idGruppo = id_gruppo, @cha_tipocorr = cha_tipo_corr, @cha_dettaglio = cha_dettagli, @varcodaoo = var_codice_aoo, @varcodamm = var_codice_amm, @varemail = var_email
      FROM dpa_corr_globali
      WHERE system_id = @idcorrglobale
      SELECT   @no_data_err = @@ROWCOUNT
      IF (@no_data_err = 0)
      begin
         SET @outvalue = 0
         RETURN
      end
   END 
   SET @cod_rubrica_old = @cod_rubrica
   IF (@cod_rubrica != @cod_old)
      SET @cod_rubrica_old = @cod_old

   IF /* 0 */ @outvalue = 1
-- 1) update del record vecchio nella dpa_corr_globali
   begin
      SET @new_var_cod_rubrica = @cod_rubrica + '_' + STR(@idcorrglobale)
      --<< storicizzazione_corrisp2 >>
      BEGIN
         update dpa_corr_globali set dta_fine = GetDate(),var_cod_rubrica = @new_var_cod_rubrica,var_codice = @new_var_cod_rubrica,
         cha_riferimento = '0',id_parent = NULL  WHERE system_id = @idcorrglobale
         SELECT   @error = @@ERROR
         IF (@error <> 0)
         begin
            SET @outvalue = 0
            RETURN
         end
      END 


-- 3) inserisco il nuovo record nella dpa_corr_globali
      IF /* 2 */ @outvalue = 1
      begin
         SELECT   @newid = @@identity

         --<< inserimento_nuovo_corrisp2 >>
         BEGIN
 INSERT INTO dpa_corr_globali(system_id, num_livello, cha_tipo_ie, id_registro,
id_amm, var_desc_corr,
id_old, dta_inizio, id_parent, var_codice,
cha_tipo_corr, cha_tipo_urp,
var_codice_aoo, var_cod_rubrica, cha_dettagli,
var_codice_amm, cha_pa, id_peso_org,
id_gruppo, id_tipo_ruolo, id_uo, var_email)
VALUES(@newid, @numLivello, @chaTipoIE, @id_reg,
@idamm, @desc_corr,
@idcorrglobale, GetDate(), @idParent, @cod_rubrica_old,
@cha_tipocorr, @cha_tipourp,
@varcodaoo, @cod_rubrica_old, @cha_dettaglio,
@varcodamm, @chapa, @idPesoOrg,
@idGruppo, @idTipoRuolo, @idUO, @varemail)
            SELECT   @error = @@ERROR
            IF (@error <> 0)
            begin
               SET @outvalue = 0
               RETURN
            end
         END 

/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI,*/
         IF /* 3 */ @outvalue = 1
--PRENDO LA SYSTEM_ID APPENA INSERITA
         begin
            SELECT   @identitydettglobali = @@identity

            --<< inserimento_dettaglio_corrisp2 >>
            BEGIN
 INSERT INTO dpa_dett_globali(system_id, id_corr_globali, var_indirizzo,
var_cap, var_provincia, var_nazione,
var_telefono, var_telefono2,
var_citta, var_fax)
VALUES(@identitydettglobali, @newid, @indirizzo,
@cap, @provincia, @nazione,
@telefono, @telefono2,
@citta, @fax)
               SELECT   @error = @@ERROR
               IF (@error <> 0)
               begin
                  SET @outvalue = 0
                  RETURN
               end
            END 
         end
      end  /* 3 */

 /* 2 */

-- 4) update di tutti i ruoli che avevano ID_UO = ID_UO_OLD con l'ID del nuovo record inserito
      IF /* 4 */ @outvalue = 1
      BEGIN
         update DPA_CORR_GLOBALI set ID_UO = @newid  WHERE ID_UO = @idcorrglobale
         SELECT   @error = @@ERROR
         IF (@error <> 0)
         begin
            SET @outvalue = 0
            RETURN
         end
      END
 /* 4 */

-- 5) update di tutte le UO che avevano ID_PARENT = ID_UO_OLD con l'ID del nuovo record inserito
      IF /* 5 */ @outvalue = 1
      BEGIN
         update DPA_CORR_GLOBALI set ID_PARENT = @newid  WHERE ID_PARENT = @idcorrglobale
         SELECT   @error = @@ERROR
         IF (@error <> 0)
         begin
            SET @outvalue = 0
            RETURN
         end
      END
 /* 5 */

      IF /* 6 */ @outvalue = 1
      BEGIN
 INSERT INTO DPA_UO_REG(ID_UO, ID_REGISTRO)
         SELECT @newid, ID_REGISTRO FROM DPA_UO_REG WHERE ID_UO = @idcorrglobale
         SELECT   @error = @@ERROR
         IF (@error <> 0)
         begin
            SET @outvalue = 0
            RETURN
         end
      END
  /* 6 */

--se fa parte di una lista, allora la devo aggiornare.
      update dpa_liste_distr  set ID_DPA_CORR = @newid FROM dpa_liste_distr d where d.ID_DPA_CORR = @idcorrglobale
   end
 /* 0 */
   SET @returnvalue = @outvalue
END

GO


              
----------- FINE -
              
---- HistoricizeRole.MSSQL.sql  marcatore per ricerca ----

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[HistoricizeRole]') AND type in (N'P', N'PC'))
DROP PROCEDURE [@db_user].[HistoricizeRole]
GO

-- =============================================
-- Author:		Furnari Samuele
-- Name:		HistoricizeRole 
-- Description:	Store per la storicizzazione di un ruolo.
-- =============================================
create PROCEDURE [@db_user].[HistoricizeRole] 
	@idCorrGlobRole INTEGER  ,
	@newRoleCode   NVARCHAR,
	@newRoleDescription  NVARCHAR, 
	@newRoleUoId   NVARCHAR,
	@newRoleTypeId INTEGER,
	@oldIdCorrGlobId INTEGER OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Nome della colonna letta dalla tabella dei metadati
	DECLARE @colName NVARCHAR (2000)
  
	-- Lista separata da , dei nomi delle colonne in cui eseguire la insert
	DECLARE @colNameList NVARCHAR (4000)
	  
	-- Lista separata da , dei valori da assegnare alle colonne
	DECLARE @colValuesList NVARCHAR (4000)
  
	-- Selezione delle colonne della corr globali dalla tabella dei metadati
	DECLARE curColumns CURSOR FOR
		SELECT c.name 
		FROM sys.columns AS c 
		INNER JOIN sys.tables t
		ON c.object_id = t.object_id
		WHERE t.name='DPA_CORR_GLOBALI'
		ORDER BY c.column_id ASC
      
	BEGIN OPEN curColumns
	FETCH curColumns INTO @colName
	
	WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
			-- inserito il valore modificato altrimenti viene lasciata com'
			IF(@colName != 'SYSTEM_ID')
				SET @colNameList = @colNameList + ', ' + @colName;
		    
		    SET @colValuesList =
				CASE @colName
					WHEN 'SYSTEM_ID'THEN @colValuesList
					WHEN 'VAR_CODICE' THEN @colValuesList + ', ' + 'VAR_CODICE + ''_'' + SYSTEM_ID'
					WHEN 'VAR_COD_RUBRICA' THEN @colValuesList + ', ' + 'VAR_COD_RUBRICA + ''_'' + SYSTEM_ID'
					WHEN 'DTA_FINE' THEN @colValuesList + ', ' + 'GETDATE()'
					ELSE @colValuesList + ', ' + @colName
		END
	
	CLOSE curColumns
	DEALLOCATE curColumns
  
	SET @colNameList = SUBSTRING( @colNameList, 3, len(@colNameList) - 3)
	SET @colValuesList = SUBSTRING( @colValuesList, 3, len(@colValuesList) - 3)
  
	DECLARE @query AS nvarchar
	set @query = 'INSERT INTO dpa_corr_globali (' + @colNameList + ') ( SELECT ' + @colValuesList + ' FROM dpa_corr_globali WHERE system_id = ' + @idCorrGlobRole + ')'
	
	EXECUTE @query
	
	SELECT @oldIdCorrGlobId = MAX(system_id) FROM @db_user.dpa_corr_globali WHERE original_id = @idCorrGlobRole
  
	-- Aggiornamento dei dati relativi al nuovo ruolo e impostazione dell'id_old
	update @db_user.DPA_CORR_GLOBALI  
	set	id_old = @oldIdCorrGlobId,
		var_codice = @newRoleCode,
		var_desc_corr = @newRoleDescription,
		id_uo = @newRoleUoId,
		id_tipo_ruolo = @newRoleTypeId
	where system_id = @idCorrGlobRole
	  
	  -- Cancellazione dell'id gruppo per il gruppo storicizzato
	  update @db_user.dpa_corr_globali 
	  set id_gruppo = null 
	  where  system_id = @oldIdCorrGlobId
  
	  -- Aggiornamento degli id mittente e destinatario relativi al ruolo
	  update @db_user.dpa_doc_arrivo_par
	  set id_mitt_dest = @oldIdCorrGlobId
	  where id_mitt_dest = @idCorrGlobRole
	  
	  -- Aggiornamento degli id dei ruoli creatori per documenti e fascicoli
	  update @db_user.profile 
	  set id_ruolo_creatore = @oldIdCorrGlobId 
	  where id_ruolo_creatore = @idCorrGlobRole
  
	  update @db_user.PROJECT 
	  set id_ruolo_creatore = @oldIdCorrGlobId 
	  where id_ruolo_creatore = @idCorrGlobRole
  
	  -- Aggiornamento delle trasmissioni
	  update @db_user.dpa_trasmissione
	  set id_ruolo_in_uo = @oldIdCorrGlobId
	  where id_ruolo_in_uo = @idCorrGlobRole
  
	  update @db_user.dpa_trasm_singola
	  set id_corr_globale = @oldIdCorrGlobId
	  where id_corr_globale= @idCorrGlobRole
	END
END
END

GO


              
----------- FINE -
              
---- PACKAGE_PGU.MSSQL.SQL  marcatore per ricerca ----
    -- Reperimento utenti
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[PGU_SP_GETUTENTI]') AND type in (N'P', N'PC'))
DROP PROCEDURE [@db_user].[PGU_SP_GETUTENTI]
GO
CREATE PROCEDURE @db_user.PGU_SP_GetUtenti  
AS
BEGIN
   SELECT * FROM PGU_UTENTI
   ORDER BY Id ASC       

END 

GO

    -- Reperimento utente
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETUTENTE]
GO

CREATE PROCEDURE PGU_SP_GETUTENTE @pId INT 
AS
BEGIN
   SELECT * FROM PGU_UTENTI
   WHERE Id = @pId
   ORDER BY Nome ASC  

END 

GO

    -- Reperimento associazioni
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETASSOCIAZIONIUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETASSOCIAZIONIUTENTE]
GO
CREATE PROCEDURE PGU_SP_GetAssociazioniUtente @pId INT 
AS
BEGIN
   DECLARE @pCount INT
   SELECT   EU.ID,
E.ID AS IDENTE,
E.NOME AS ENTE,
U.ID AS IDUTENTE,
U.NOME AS UTENTE
   from pgu_enti e
   left join pgu_enti_utenti eu on e.id = eu.idente
   left join pgu_utenti u on eu.idutente = u.id
   where u.id = @pId
   order by e.Nome asc            

END  

GO

-- Verifica esistenza utente
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_CONTAINSUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_CONTAINSUTENTE]
GO
CREATE PROCEDURE PGU_SP_ContainsUtente @pNome NVARCHAR(2000), @pRet INT OUTPUT 
AS
BEGIN
   SELECT   @pRet = COUNT(*)
   FROM PGU_UTENTI
   WHERE UPPER(NOME) = UPPER(@pNome)

END  

GO

    -- Inserimento di un utente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_INSERTUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_INSERTUTENTE]
GO
CREATE PROCEDURE PGU_SP_InsertUtente @pNome NVARCHAR(2000),
@pDescrizione NVARCHAR(2000), 
@pPassword NVARCHAR(2000), 
@pSecretKey NVARCHAR(2000),
@pSecretIV NVARCHAR(2000),
@pAmministratore CHAR(2000),
@pId INT OUTPUT 
AS
BEGIN


 INSERT INTO PGU_UTENTI(NOME, DESCRIZIONE, PASSWORD, SECRETKEY, SECRETIV, AMMINISTRATORE)
VALUES(@pNome, @pDescrizione, @pPassword, @pSecretKey, @pSecretIV, @pAmministratore)
   select   @pId = @@identity

END    

GO       

        -- Modifica password utente                          
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_MODIFICAPASSWORDUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_MODIFICAPASSWORDUTENTE]
GO
CREATE PROCEDURE PGU_SP_ModificaPasswordUtente @pId INT,
@pOldPassword NVARCHAR(2000),
@pPassword NVARCHAR(2000), 
@pSecretKey NVARCHAR(2000),
@pSecretIV NVARCHAR(2000) 
AS
BEGIN
   update PGU_UTENTI set PASSWORD = @pPassword,SECRETKEY = @pSecretKey,SECRETIV = @pSecretIV  WHERE   ID = @pId
   AND PASSWORD = @pOldPassword

END  

GO                              


    -- Rimozione associazioni utente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_CLEARASSOCIAZIONIUTENTEENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_CLEARASSOCIAZIONIUTENTEENTE]
GO
CREATE PROCEDURE PGU_SP_ClearAssociazioniUtenteEnte @pIdUtente INT 
AS
BEGIN
   DELETE FROM PGU_ENTI_UTENTI WHERE IDUTENTE = @pIdUtente

END  

GO


    -- Inserimento associazione utente / ente 
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_INSERTASSOCIAZIONEUTENTEENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_INSERTASSOCIAZIONEUTENTEENTE]
GO                           
CREATE PROCEDURE PGU_SP_InsertAssociazioneUtenteEnte @pIdEnte INT,
@pIdUtente INT,
@pId INT OUTPUT 
AS
BEGIN


 INSERT INTO PGU_ENTI_UTENTI(IDENTE, IDUTENTE)
VALUES(@pIdEnte, @pIdUtente)
   select   @pId = @@identity

END 

GO          

    -- Modifica di un utente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_UPDATEUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_UPDATEUTENTE]
GO
CREATE PROCEDURE PGU_SP_UpdateUtente @pId INT,
@pDescrizione NVARCHAR(2000), 
@pAmministratore CHAR(2000) 
AS
BEGIN
   update PGU_UTENTI set DESCRIZIONE = @pDescrizione,AMMINISTRATORE = @pAmministratore  WHERE   ID = @pId

END 

GO

    -- Rimozione utente
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_DELETEUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_DELETEUTENTE]
GO
CREATE PROCEDURE PGU_SP_DeleteUtente @pId INT 
AS
BEGIN
   DELETE FROM PGU_UTENTI WHERE ID = @pId

END 

GO


    -- Reperimento degli enti
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETENTI]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETENTI]
GO
CREATE PROCEDURE PGU_SP_GetEnti  
AS
BEGIN

   SELECT * FROM PGU_ENTI

END 

GO

     -- Reperimento chiavi private 
     IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETPASSWORDPRIVATEKEYS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETPASSWORDPRIVATEKEYS]
GO
CREATE PROCEDURE PGU_SP_GetPasswordPrivateKeys @pNome NVARCHAR(2000) 
AS
BEGIN
   SELECT   SECRETKEY, SECRETIV
   FROM PGU_UTENTI
   WHERE UPPER(NOME) = UPPER(@pNome)      


END 

GO

    -- LoginUtente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_LOGIN]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_LOGIN]
GO
CREATE PROCEDURE PGU_SP_Login @pNome NVARCHAR(2000),  @pPassword NVARCHAR(2000) 
AS
BEGIN
   SELECT * FROM PGU_UTENTI
   WHERE UPPER(NOME) = UPPER(@pNome)
   AND PASSWORD = @pPassword

END 

GO

    -- Reperimento degli enti cui ha visibilit l'utente
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETENTIUTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETENTIUTENTE]
GO
CREATE PROCEDURE PGU_SP_GetEntiUtente @pIdUtente INT 
AS
BEGIN
   SELECT * FROM PGU_ENTI E
   INNER JOIN PGU_ENTI_UTENTI EU ON E.ID = EU.IDENTE
   WHERE EU.IDUTENTE = @pIdUtente
   ORDER BY E.ID ASC

END 

GO


    -- Reperimento ente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_GETENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_GETENTE]
GO
CREATE PROCEDURE PGU_SP_GetEnte @pId INT 
AS
BEGIN
   SELECT * FROM PGU_ENTI
   WHERE ID = @pId
   ORDER BY ID ASC

END  

GO

    -- Verifica esistenza ente
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_CONTAINSENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_CONTAINSENTE]
GO
CREATE PROCEDURE PGU_SP_ContainsEnte @pNome NVARCHAR(2000), @pRet INT OUTPUT 
AS
BEGIN
   SELECT   @pRet = COUNT(*)
   FROM PGU_ENTI
   WHERE UPPER(NOME) = UPPER(@pNome)    
END 

GO

    -- Inserimento ente    
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_INSERTENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_INSERTENTE]
GO
CREATE PROCEDURE PGU_SP_InsertEnte @pNome NVARCHAR(2000),
@pDescrizione NVARCHAR(2000),
@pUrlAppConsProtocollo NVARCHAR(2000),
@pUrlAppConsAltreApp NVARCHAR(2000),
@pUrlWsConsProtocollo NVARCHAR(2000),
@pUrlWsConsAltreApp NVARCHAR(2000),
@pId INT OUTPUT 
AS
BEGIN



 INSERT INTO PGU_ENTI(NOME, DESCRIZIONE, URLAPPCONSPROTOCOLLO, URLAPPCONSALTREAPP, URLWSCONSPROTOCOLLO, URLWSCONSALTREAPP)
VALUES(@pNome, @pDescrizione, @pUrlAppConsProtocollo, @pUrlAppConsAltreApp, @pUrlWsConsProtocollo, @pUrlWsConsAltreApp)
   select   @pId = @@identity

        -- Inserimento record associativi
   INSERT INTO PGU_ENTI_UTENTI
   SELECT @pId, U.ID
   FROM PGU_UTENTI U

END                      

GO

    -- Aggiornamento ente  
    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_UPDATEENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_UPDATEENTE]
GO   
CREATE PROCEDURE PGU_SP_UpdateEnte @pId INT,
@pNome NVARCHAR(2000),
@pDescrizione NVARCHAR(2000),
@pUrlAppConsProtocollo NVARCHAR(2000),
@pUrlAppConsAltreApp NVARCHAR(2000),
@pUrlWsConsProtocollo NVARCHAR(2000),
@pUrlWsConsAltreApp NVARCHAR(2000) 
AS
BEGIN

   update PGU_ENTI set NOME = @pNome,DESCRIZIONE = @pDescrizione,URLAPPCONSPROTOCOLLO = @pUrlAppConsProtocollo,
   URLAPPCONSALTREAPP = @pUrlAppConsAltreApp,URLWSCONSPROTOCOLLO = @pUrlWsConsProtocollo,
   URLWSCONSALTREAPP = @pUrlWsConsAltreApp  WHERE ID = @pId 


END    

GO                  


-- Rimozione ente
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PGU_SP_DELETEENTE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [PGU_SP_DELETEENTE]
GO
CREATE PROCEDURE PGU_SP_DeleteEnte @pId INT 
AS
BEGIN
   DELETE FROM PGU_ENTI WHERE ID = @pId

END      


GO

              
----------- FINE -
              
---- SP_INSERT_AREA_CONS.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:			Frezza
Data creazione:		21/11/2011
Scopo della modifica:	MODIFICA PER SUPPORTARE LA POSSIBILITA' DI INSERIRE L'ID DELLA CONSERVAZIONE PARENT E L'ID DELLA POLICY CON CUI E' STATA CREATA

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = (SELECT OBJECT_ID FROM sys.objects WHERE  name = 'SP_INSERT_AREA_CONS'))
DROP PROCEDURE @db_user.[SP_INSERT_AREA_CONS]
GO

CREATE PROCEDURE [@db_user].[SP_INSERT_AREA_CONS]
@idAmm int,
@idPeople int,
@idProfile int,
@idProject int,
@codFasc varchar(64),
@oggetto varchar(64),
@tipoDoc char,
@idGruppo int,
@idRegistro int,
@docNumber int,
@userId varchar(32),
@tipoOggetto char,
@tipoAtto   VARCHAR,
@result int  OUT
AS
BEGIN

DECLARE @idRuoloInUo int
DECLARE @id_cons_1   int
DECLARE @res         int



SET @result = -1
SET @idRuoloInUo = 0
SET @id_cons_1 = 0
SET @res = 0



SELECT @idRuoloInUo = DPA_CORR_GLOBALI.SYSTEM_ID FROM DPA_CORR_GLOBALI
WHERE DPA_CORR_GLOBALI.ID_GRUPPO = @idGruppo



BEGIN
SELECT DISTINCT @res = DPA_AREA_CONSERVAZIONE.SYSTEM_ID
FROM DPA_AREA_CONSERVAZIONE
WHERE
DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo AND
DPA_AREA_CONSERVAZIONE.CHA_STATO = 'N'

END
IF (@res > 0)

BEGIN

INSERT INTO DPA_ITEMS_CONSERVAZIONE(
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
@res,
@idProfile,
@idProject,
@tipoDoc,
@oggetto,
@idRegistro,
getdate(),
'N',
NULL,
@codFasc,
@docNumber,
@tipoOggetto,
@tipoAtto
)

SET @result = SCOPE_IDENTITY()
END
ELSE
BEGIN
INSERT INTO DPA_AREA_CONSERVAZIONE(
ID_AMM,
ID_PEOPLE,
ID_RUOLO_IN_UO,
CHA_STATO,
DATA_APERTURA,
USER_ID,
ID_GRUPPO
)
VALUES
(
@idAmm,
@idPeople,
@idRuoloInUo,
'N',
getdate(),
@userId,
@idGruppo)

SET @id_cons_1=SCOPE_IDENTITY()

INSERT INTO DPA_ITEMS_CONSERVAZIONE(
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
@id_cons_1,
@idProfile,
@idProject,
@tipoDoc,
@oggetto,
@idRegistro,
getdate(),
'N',
NULL,
@codFasc,
@docNumber,
@tipoOggetto,
@tipoAtto)

SET @result = SCOPE_IDENTITY()
END
END


GO


              
----------- FINE -
              
---- Utl_Insert_Chiave_Config.MSSQL.sql  marcatore per ricerca ----
IF  EXISTS (SELECT * FROM dbo.sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[Utl_Isvalore_Lt_Column]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[Utl_Isvalore_Lt_Column]
go

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

-- Controllo esistenza SP Utl_Insert_Chiave_Config
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='Utl_Insert_Chiave_Config')
DROP PROCEDURE @db_user.Utl_Insert_Chiave_Config
go

create Procedure @db_user.Utl_Insert_Chiave_Config(
    @Codice               VARCHAR(100) ,
    @Descrizione          VARCHAR(2000) ,
    @Valore               VARCHAR(128) ,
    @Tipo_Chiave          Varchar(1) ,
    @Visibile             VARCHAR(1) ,
    @Modificabile         VARCHAR(1) ,
    @Globale              VARCHAR(1) ,
    @myversione_CD          Varchar(32) ,
    @Codice_Old_Webconfig Varchar(128) ,
    @Forza_Update_Valore  VARCHAR(1) , @RFU VARCHAR(10) ) As  

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
  FROM DPA_CHIAVI_CONFIGURAZIONE
  Where Var_Codice=@Codice




  If (@Cnt         = 0 and @Globale    = 1) 
  begin -- inserisco la chiave globale non esistente
      EXEc (
       'INSERT      INTO DPA_CHIAVI_CONFIGURAZIONE
        ( Id_Amm,          Var_Codice ,
          VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE ,          
          VAR_CODICE_OLD_WEBCONFIG , VERSIONE_CD)
        Values        ( 0,          '+@Codice +' ,'
        +@Descrizione+' ,'        +@Valore+' ,'
        +@Tipo_Chiave+',          '+@Visibile+' ,'
        +@Modificabile+',          '+@Globale+' ,'
        +@Codice_Old_Webconfig+'    , '+@myVERSIONE_CD+') ' )
          print 'inserita nuova chiave globale ' + @Codice
	End 
  If (@Cnt         = 0 And @Globale    = 0) 
  begin -- inserisco la chiave non globale non esistente
      -- questa forma evita di dover eseguire la procedura CREA_KEYS_AMMINISTRA di riversamento !
      EXEc ('INSERT      INTO dpa_chiavi_configurazione
        ( Id_Amm,          Var_Codice ,
          VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE ,          
		  VAR_CODICE_OLD_WEBCONFIG  , VERSIONE_CD)
      SELECT Amm.System_Id                      As Id_Amm,        '
      +@Codice+' As Var_Codice,'
      +@Descrizione+' As Var_Descrizione ,      '+@Valore+' As Var_Valore ,'
      +@Tipo_Chiave+' As Cha_Tipo_Chiave,       '+@Visibile+' As Cha_Visibile ,'
      +@Modificabile+' As Cha_Modificabile,     '+@Globale+' as CHA_GLOBALE , '
      +@Codice_Old_Webconfig+' as VAR_CODICE_OLD_WEBCONFIG , '+@myVERSIONE_CD+'
      FROM Dpa_Amministra Amm       WHERE NOT EXISTS
        (SELECT Id_Amm        FROM Dpa_Chiavi_Configurazione
        WHERE var_codice = '+@Codice+')' )
        
        print 'inserita nuove n chiavi locali per le n amministrazioni: ' + @Codice
  End

  IF (@Cnt                  = 1) 
  begin -- chiave gi esistente
    PRINT 'chiave ' + @Codice + ' gi esistente'
    IF @Forza_Update_Valore = '1' 
    begin
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = @Descrizione,
        VAR_VALORE        = @Valore
      Where Var_Codice    =@Codice
      PRINT 'AGGIORNATO VALORE PER CHIAVE ' + @Codice + ' gi esistente'
    end ELSE begin-- aggiorno solo la descrizione
      UPDATE Dpa_Chiavi_Configurazione
      SET Var_Descrizione = @Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    =@Codice 
    END 
  END 
  
END
GO


              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
    Values      (getdate(), '3.20.1')
GO

              
