-- spostati in alto perchè richiamate subito SAB

--*************************************************************************--  
--********************** GIORDANO IACOZZILLI  *****************************--  
--*************************************************************************--     
--05/12/2012: Mancavano le create di tutte le Utl di utilità
--Le aggiungo O_o

--utl_utl_add_column
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_add_column]'))
DROP PROCEDURE [@db_user].[utl_add_column]
GO

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
execute [@db_user].utl_insert_log @nome_utente, null, @comando_richiesto, @versioneCD, @esito
	

END       

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
        print @istruzione
        execute sp_executesql @istruzione
        print @istruzione

END            

GO



--utl_utl_add_foreign_key
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_add_foreign_key]'))
DROP PROCEDURE [@db_user].[utl_add_foreign_key]
GO

CREATE PROCEDURE [@db_user].[utl_add_foreign_key]
		-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
		@nome_tabella_fk Nvarchar(200),
		@nome_colonna_fk Nvarchar(200),
		@condizione_add  Nvarchar(200),
	    @RFU Nvarchar(200)
AS
BEGIN	
		DECLARE @nome_foreign_key   Nvarchar(2000)
		DECLARE @tablePK Nvarchar(2000)
		DECLARE @tableFK Nvarchar(2000)
		DECLARE @colonnaFK Nvarchar(2000)
		
		SET @tablePK = SUBSTRING(@nome_tabella, 1,10)
	    SET @tableFK = SUBSTRING(@nome_tabella_fk, 1,10)
	    SET @colonnaFK = SUBSTRING(@nome_colonna_fk, 1,8)
	   
	   
	   set @nome_foreign_key = 'FK_'+@tableFK+'_'+@colonnaFK+'_'+@tablePK+''
		
	if exists(select * from sysobjects WHERE id = OBJECT_ID(N'['+@nome_utente+'].['+@nome_tabella_fk+']'))
		if not exists(select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY'
    AND TABLE_NAME = ''+@nome_tabella_fk+'' 
    AND TABLE_SCHEMA =''+@nome_utente+''
    AND CONSTRAINT_NAME = ''+@nome_foreign_key+'')
	
	begin	
	print 'non esiste FK'
			
		DECLARE @istruzione Nvarchar(2000)
		DECLARE @insert_log Nvarchar(2000)
		DECLARE @data_eseguito Nvarchar(2000)
		DECLARE @esito Nvarchar(2000)
		DECLARE @comando_richiesto Nvarchar(2000)
		DECLARE @ErrorVar INT
		DECLARE @condizione NVARCHAR(2000)
	
			if exists(select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND TABLE_NAME = ''+@nome_tabella+'' 
    AND TABLE_SCHEMA =''+@nome_utente+''
    AND CONSTRAINT_NAME = 'PK_'+@nome_tabella+'')
			
			
			begin
		print 'esiste PK'
	   							
		SET @istruzione = N'ALTER TABLE ['+@nome_utente+'].' +@nome_tabella_fk + ' 
						  WITH CHECK ADD CONSTRAINT ' +@nome_foreign_key+ ' FOREIGN KEY (' +@nome_colonna_fk+') 
						  REFERENCES '+@nome_utente+'.' +@nome_tabella+ ' ('+@nome_colonna+')'

		
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
        --IF @ErrorVar = 2715
        IF @ErrorVar = 2715
            BEGIN
                SET @esito = @istruzione
            END
        ELSE
        IF @ErrorVar = 547
            BEGIN
                SET @esito = 'Conflitto del PK e FK'
            END
        ELSE
        IF @ErrorVar = 1750
            BEGIN
              SET @esito = 'VERIFICARE CON "'+@istruzione
            END
        ELSE
        IF @ErrorVar = 1778
            BEGIN
                SET @esito = 'Uno delle due colonne non  valido'
            END
        ELSE
            BEGIN
                SET @esito = N'ERROR: error '
                    + RTRIM(CAST(@ErrorVar AS NVARCHAR(10)))
                    + N' occurred.'
            END
        end

-----------------------se non ci sono nessun errore.
			else
		begin 
        set @esito = 'Esito Positivo'
   
	   set @comando_richiesto = 'Added Foreign Key ' +@nome_foreign_key+' on '+@nome_tabella_fk+''
       execute [@db_user].utl_insert_log @nome_utente, getdate, @comando_richiesto, @versioneCD, @esito
     end
		end
			else    
			begin
			print 'non esiste PK'
			set @esito = 'Esito Negativo - Chiave primaria non esistente' 
    
			set @comando_richiesto = 'Adding Foreign Key ' +@nome_foreign_key+' on '+@nome_tabella_fk+''
			execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
			END
	END


	ELSE
---------------------se  gi esistente il FK		
	BEGIN	
		   print 'esiste gi FK'
		   set @esito = 'ESITO NEGATIVO - Foreign Key GIA esistente o Tabella non esistente'
		   set @comando_richiesto = 'Adding Foreign Key ' +@nome_foreign_key+' on '+@nome_tabella_fk+''
	       execute [@db_user].utl_insert_log @nome_utente, getdate, @comando_richiesto, @versioneCD, @esito

		   end
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@nome_tabella_fk,
			@nome_colonna_fk,
			@condizione_add, 
			@RFU
END
   
   
   
   
   
   
   
   
              
---- utl_add_index.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_index('VERSIONE CD', --->es. 3.16.1
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
         
--utl_add_index
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_add_index]'))
DROP PROCEDURE [@db_user].[utl_add_index]
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
       
       execute @db_user.utl_insert_log @nome_utente  , getdate, @comando_richiesto, @versioneCD, @esito

       
          
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
		   execute @db_user.utl_insert_log @nome_utente  , getdate, @comando_richiesto, @versioneCD, @esito
		   end
	
	
END

GO

--utl_add_primary_key
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_add_primary_key]'))
DROP PROCEDURE [@db_user].[utl_add_primary_key]
GO

CREATE PROCEDURE [@db_user].[utl_add_primary_key]
	-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
		@condizione_add  Nvarchar(200),
	    @RFU Nvarchar(200)
AS
BEGIN
	if exists(select * from sysobjects WHERE id = OBJECT_ID(N'['+@nome_utente+'].['+@nome_tabella+']'))
		if not exists(select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND TABLE_NAME = ''+@nome_tabella+'' 
    AND TABLE_SCHEMA =''+@nome_utente+'')
		
	BEGIN
	DECLARE @istruzione Nvarchar(2000)
	DECLARE @insert_log Nvarchar(2000)
	DECLARE @data_eseguito Nvarchar(2000)
	DECLARE @esito Nvarchar(2000)
	DECLARE @comando_richiesto Nvarchar(2000)
	DECLARE @ErrorVar INT
	DECLARE @condizione NVARCHAR(2000)
	DECLARE @nome_primary_key   Nvarchar(200)
	
	
	
	
	----------------se nella condizione "is_index_unique" viene impostato 1	
	  --IF @condizione_add = '1'
	  --SET @condizione = 'UNIQUE'
	  --else
	  --SET @condizione = ''
	  
	   SET @data_eseguito = (select convert (varchar, getdate(), 120))
	   	
	   	SET @nome_primary_key = 'PK_'+@nome_tabella+''
	   							
		SET @istruzione = N'ALTER TABLE ['+@nome_utente+'].' +@nome_tabella + ' ADD constraint ' +@nome_primary_key+ ' PRIMARY KEY (' +@nome_colonna+') '




	begin try
		execute sp_executesql @istruzione
		print @istruzione
	end try
	begin catch
--- Salva il numero dell'errore
  
    select @ErrorVar = ERROR_NUMBER()
    PRINT @ErrorVar
	end catch


-------Condizioni degli errori
		IF @ErrorVar <> 0
    BEGIN
        IF @ErrorVar = 515
            BEGIN
                SET @esito = N'ESITO NEGATIVO - Non  possibile di modificare la colonna NON PIENA'
            END
        ELSE
        IF @ErrorVar = 1750
			begin
				SET @esito = 'VERIFICA CON'+@istruzione
				--SET @esito = N'ESITO NEGATIVO - Controllare i nomi delle colonne'
			end
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
  --     ''Added Primary Key ' +@nome_primary_key+' on '+@nome_tabella+''',
  --     '''+@versioneCD+''',
  --     '''+@esito+''')'

       set @comando_richiesto = 'Added Primary Key ' +@nome_primary_key+' on '+@nome_tabella+''
       --exec(@insert_log)
       execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
       
          
	end
	
	else
---------------------se  gi esistente l'indice		
		   begin	


			   --SET @insert_log = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL VALUES (
			   --'''+@data_eseguito+''',
			   --''Added Primary Key ' +@nome_primary_key+' on '+@nome_tabella+''',
			   --'''+@versioneCD+''',
			   --''ESITO NEGATIVO - Chiave primaria GIA esistente o Tabella non esistente'')'
	           SET @nome_primary_key = 'PK_'+@nome_tabella+''
			   set @comando_richiesto = 'Adding Primary Key ' +@nome_primary_key+' on '+@nome_tabella+''
			   set @esito = 'ESITO NEGATIVO - Chiave primaria GIA esistente o Tabella non esistente'
			   execute [@db_user].utl_insert_log @nome_utente, getdate,@comando_richiesto, @versioneCD, @esito
		   end
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@condizione_add, 
			@RFU
END
              
---- utl_drop_column.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_drop_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_A
--					'CONDIZIONE DROP', --->es. '0' CANCELLA COLONNA SOLO SE E' VUOTO, '1' CANCELLA ANCHE SE NON VUOTO!!
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 18/07/2011
-- Description:	
-- =============================================

SET ANSI_NULLS ON

GO

--utl_drop_column
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_drop_column]'))
DROP PROCEDURE [@db_user].[utl_drop_column]
GO

CREATE PROCEDURE [@db_user].[utl_drop_column]
	-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
	    @condizione_drop Nvarchar(200),
	    @RFU Nvarchar(200)
AS
BEGIN
	DECLARE @parametroCNT Nvarchar(2000)
	DECLARE @conta Nvarchar(2000)
	DECLARE @istruzione Nvarchar(2000)
	DECLARE @insert_log Nvarchar(2000)
	DECLARE @data_eseguito Nvarchar(2000)
	DECLARE @condizione int
	DECLARE @comando_richiesto Nvarchar(2000)
	DECLARE @esito Nvarchar(2000)
	DECLARE @ErrorVar Nvarchar(2000)
	
	
		If exists(select * from syscolumns where name=@nome_colonna and id in
		(select id from sysobjects where name=@nome_tabella and xtype='U'))
		BEGIN
	
	    --- CONTA IL RISULTATO DELLA COLONNA
	    SET @parametroCNT  = N'@condizioneOUT int OUTPUT';
	    SET @conta = N'select @condizioneOUT=COUNT('+@nome_colonna+') from '+@nome_utente+'.'+@nome_tabella+''
	    execute sp_executesql @conta, @parametroCNT, @condizioneOUT = @condizione out
	    --end
	    set @data_eseguito = (select convert (varchar, getdate(), 120))
	   	
	   	SET @istruzione = N'alter table [' + @nome_utente + '].
						[' + @nome_tabella + '] DROP COLUMN ' + @nome_colonna +'' 
		    
       
	 --- CONDIZIONE_DROP = 1 CANCELLA ANCHE SE LA COLONNA NON E' VUOTA, CONDIZIONE_DROP = 0 SOLO SE VUOTA
	   IF ( (@condizione_drop = 0 AND @condizione = 0) OR @condizione_drop = 1)
	BEGIN
		   --execute sp_executesql @istruzione
		begin try
		execute sp_executesql @istruzione
		print @istruzione
		end try
	begin catch
--- Salva il numero dell'errore
  
    select @ErrorVar = ERROR_NUMBER()
    PRINT @ErrorVar
	end catch


-------Condizioni degli errori
		IF @ErrorVar <> 0
    BEGIN
        IF @ErrorVar = 4922
			begin
				SET @esito = 'VERIFICA CON '+@istruzione
				--SET @esito = N'ESITO NEGATIVO - Controllare i nomi delle colonne'
			end
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
     
			   set @comando_richiesto = 'Droped column ' +@nome_colonna+' from '+@nome_tabella+''
			   execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
    END
    
    else
    begin
			   set @comando_richiesto = 'Dropping column ' +@nome_colonna+' from '+@nome_tabella+''
			   set @esito = 'Non  possibile eliminare la colonna non vuota'
			   execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
    end
    END

	else
		
		BEGIN
		   
			   set @comando_richiesto = 'Dropping column ' +@nome_colonna+' from '+@nome_tabella+''
			   set @esito = 'Esito Negativo - Colonna non esistente'
			   execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
			   
		
	END
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@condizione_drop, 
			@RFU
END
              
---- utl_insert_log.MSSQL.sql  marcatore per ricerca ----
SET ANSI_NULLS ON

GO

--utl_modify_column
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_modify_column]'))
DROP PROCEDURE [@db_user].[utl_modify_column]
GO

CREATE PROCEDURE [@db_user].[utl_modify_column]
	-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
		@tipo_dato    Nvarchar(200),
		@default  Nvarchar(200),
	    @condizione_modify Nvarchar(200),
	    @RFU Nvarchar(200)
AS
BEGIN
	
		-- controllo tipo dato da oracle a sql server
	if SUBSTRING(@tipo_dato, 1,8) = 'varchar2'
	begin
	set @tipo_dato = (select replace(@tipo_dato, 'varchar2', 'varchar'))
	end
	if SUBSTRING(@tipo_dato, 1,9) = 'nvarchar2'
	begin
	set @tipo_dato = (select replace(@tipo_dato, 'nvarchar2', 'nvarchar'))
	end
	if SUBSTRING(@tipo_dato, 1,6) = 'number'
	begin
	set @tipo_dato = (select replace(@tipo_dato, 'number', 'numeric'))
	end
	if SUBSTRING(@tipo_dato, 1,9) = 'timestamp'
	begin
	set @tipo_dato = (select replace(@tipo_dato, 'timestamp', 'datetime'))
	end
	if SUBSTRING(@tipo_dato, 1,4) = 'blob'
	begin
	set @tipo_dato = (select replace(@tipo_dato, 'blob', 'image'))
	end
		
	
	if exists(select * from syscolumns where name=@nome_colonna and id in
		(select id from sysobjects where name=@nome_tabella and xtype='U'))
		
	BEGIN
	DECLARE @istruzione Nvarchar(2000)
	DECLARE @insert_log Nvarchar(2000)
	DECLARE @data_eseguito Nvarchar(2000)
	DECLARE @esito Nvarchar(2000)
	DECLARE @ErrorVar INT;
	DECLARE @comando_richiesto Nvarchar(2000)
	 	   	
	   	SET @istruzione = N'alter table [' + @nome_utente + '].[' + @nome_tabella + '] ALTER COLUMN ' + @nome_colonna +' 
						' + @tipo_dato + ''
	
		begin try
		execute sp_executesql @istruzione
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
 
              set @comando_richiesto = 'Modified column ' +@nome_colonna+' on '+@nome_tabella+''
      
			  execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
          
	end
	
	else
---------------------se non  esistente la colonna da modificare		
		   begin	
		   
			  set @esito = 'ESITO NEGATIVO - Colonna NON esistente'
		      set @comando_richiesto = 'Modifing column ' +@nome_colonna+' on '+@nome_tabella+''
      
			  execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
		   end
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@tipo_dato, 
			@default, 
			@condizione_modify, 
			@RFU
END
              
SET ANSI_NULLS ON

GO

--utl_rename_column
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[utl_rename_column]'))
DROP PROCEDURE [@db_user].[utl_rename_column]
GO

CREATE PROCEDURE [@db_user].[utl_rename_column]
	-- Add the parameters for the stored procedure here
		@versioneCD Nvarchar(200),
		@nome_utente Nvarchar(200), 
		@nome_tabella Nvarchar(200),
		@nome_colonna Nvarchar(200),
		@nome_colonna_nuova   Nvarchar(200),
		@RFU Nvarchar(200)
AS
BEGIN
	if exists(select * from syscolumns where name=@nome_colonna and id in
		(select id from sysobjects where name=@nome_tabella and xtype='U'))
		BEGIN
		if not exists(select * from syscolumns where name=@nome_colonna_nuova and id in
		(select id from sysobjects where name=@nome_tabella and xtype='U'))
		
	BEGIN
	DECLARE @istruzione Nvarchar(2000)
	DECLARE @insert_log Nvarchar(2000)
	DECLARE @data_eseguito Nvarchar(2000)
	DECLARE @esito Nvarchar(2000)
	DECLARE @comando_richiesto Nvarchar(2000)
	   	   	
	   	SET @istruzione = N'EXEC SP_RENAME ''' + @nome_utente + '.' + @nome_tabella + '.'+ @nome_colonna +''', ''' + @nome_colonna_nuova + ''', ''COLUMN'';'
		
		PRINT @istruzione

        

       execute sp_executesql @istruzione
       
       set @esito = 'Esito positivo'
       set @comando_richiesto = 'Renamed column ' +@nome_colonna+' to '+@nome_colonna_nuova+' on '+@nome_tabella+''

       execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
       
	end
	
	else

		   begin	
	   
		   set @esito = 'Esito negativo - Colonna nuova gi esistente'
		   set @comando_richiesto = 'Renaming column ' +@nome_colonna+' to '+@nome_colonna_nuova+' on '+@nome_tabella+''

		   execute [@db_user].utl_insert_log @nome_utente, getdate,@comando_richiesto, @versioneCD, @esito
		   end
		   END
	ELSE
		   begin	
	   
		   set @esito = 'Esito negativo - Colonna vecchia non esistente'
		   set @comando_richiesto = 'Renaming column ' +@nome_colonna+' to '+@nome_colonna_nuova+' on '+@nome_tabella+''

		   execute [@db_user].utl_insert_log @nome_utente,getdate, @comando_richiesto, @versioneCD, @esito
		   end
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@nome_colonna_nuova,
			@RFU
END

GO

--create DPA_LOG_INSTALL
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[DPA_LOG_INSTALL]'))
DROP TABLE [@db_user].[DPA_LOG_INSTALL]
GO

CREATE TABLE [@db_user].[DPA_LOG_INSTALL](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DATA_OPERAZIONE] [smalldatetime] NOT NULL,
	[COMANDO_RICHIESTO] [nvarchar](max) NOT NULL,
	[VERSIONE_CD] [nvarchar](max) NOT NULL,
	[ESITO_OPERAZIONE] [nvarchar](max) NOT NULL
) ON [PRIMARY]

GO
--*************************************************************************--  
--********************** FINE  ********************************************--  
--*************************************************************************--  

---- ALTER_DPA_CORR_GLOBALI.MSSQL.SQL  marcatore per ricerca ----
EXEC @db_user.utl_add_column '3.17.1', '@db_user'
, 'DPA_CORR_GLOBALI', 'VAR_ORIGINAL_CODE', 'VARCHAR(128)', NULL, NULL, NULL, NULL
GO

EXEC @db_user.utl_add_column '3.17.1', '@db_user'
, 'DPA_CORR_GLOBALI', 'ORIGINAL_ID', 'INTEGER', NULL, NULL, NULL, NULL
GO           
---- ALTER_PEOPLE.MSSQL.sql  marcatore per ricerca ----
-- alter table people add ABILITATO_CENTRO_SERVIZI       CHAR(1 BYTE)
EXEC @db_user.utl_add_column '3.17', '@db_user'
, 'people', 'ABILITATO_CENTRO_SERVIZI', 'CHAR(1)','','','',''              
----------- FINE -
GO       
---- ALTER_PROFILE.MSSQL.SQL  marcatore per ricerca ----
-- AGGIUNGE la colonna CHA_COD_T_A nella tabella PROFILE
EXEC @db_user.utl_add_column '3.17', '@db_user', 'PROFILE', 'CHA_COD_T_A', 'CHAR(1)','','','',''          
----------- FINE -
GO     
---- ALTER_PROJECT.MSSQL.SQL  marcatore per ricerca ----
-- AGGIUNGE la colonna CHA_COD_T_A nella tabella PROJECT
EXEC @db_user.utl_add_column '3.17', '@db_user', 'PROJECT', 'CHA_COD_T_A', 'CHAR(1)','','','',''
GO                  
----------- FINE -
              
---- ALTER_SECURITY.MSSQL.SQL  marcatore per ricerca ----
--Aggiunta campo VAR_NOTE_SEC nella SECURITY
exec @db_user.utl_add_column '3.17', '@db_user','SECURITY','VAR_NOTE_SEC', 'VARCHAR(255)', NULL, NULL, NULL, NULL 

--if not exists (SELECT * FROM syscolumns WHERE name='VAR_NOTE_SEC' and id in (SELECT id FROM sysobjects WHERE name='SECURITY' and xtype='U'))
--BEGIN
--	ALTER TABLE [SECURITY] ADD VAR_NOTE_SEC VARCHAR(255);	
--END;
GO                 
---- ROLEHISTORYCREATE.MSSQL.SQL  marcatore per ricerca ----
--- Controllo l'esistenza e l'eliminazione del Trigger ROLEHISTORYCREATE    
IF EXISTS (SELECT * FROM sys.triggers
			WHERE name = 'ROLEHISTORYCREATE')
DROP TRIGGER [@db_user].ROLEHISTORYCREATE
GO

create TRIGGER [@db_user].[ROLEHISTORYCREATE] 
	ON [@db_user].[DPA_CORR_GLOBALI] 
FOR  INSERT AS
  /******************************************************************************

    AUTHOR:    Samuele Furnari -- tradotte da De Luca Serpi
    NAME:      ROLEHISTORYCREATE
    PURPOSE:   All'inserimento di un ruolo viene inserita una riga nella
               tabella dello storico
  ******************************************************************************/
  
  -- Inserimento di una tupla nella tabella della storia del ruolo
   DECLARE @uoDescription VARCHAR(256)
   DECLARE @uoCode VARCHAR(128)
   DECLARE @roleTypeDescription VARCHAR(64)
   DECLARE @roleTypeCode VARCHAR(16)

IF exists (select 'x' FROM INSERTED ins
	   , deleted del
        where 
			ins.DTA_FINE is null and
        ( ins.ID_OLD != del.id_old or 
          ins.var_codice != del.var_codice or 
          ins.var_desc_corr != del.var_desc_corr or
          ins.id_uo != del.id_uo or
          ins.id_tipo_ruolo != del.id_tipo_ruolo) )
BEGIN 
	INSERT
	   INTO @db_user.DPA_ROLE_HISTORY
	   (ACTION_DATE, UO_ID, ROLE_TYPE_ID, ORIGINAL_CORR_ID, ACTION, ROLE_DESCRIPTION, ROLE_ID)
	   SELECT GetDate(), ins.id_uo, ins.id_tipo_ruolo, ins.original_id, 'C', 
	   ins.var_desc_corr + ' (' + ins.var_codice + ')', ins.system_id FROM INSERTED ins
END
GO              
----------- FINE -
              
---- RoleHistoryDelete.MSSQL.SQL  marcatore per ricerca ----
--- Controllo l'esistenza e l'eliminazione del Trigger RoleHistoryDelete    
IF EXISTS (SELECT * FROM sys.triggers
WHERE name = 'RoleHistoryDelete')
DROP TRIGGER [@db_user].RoleHistoryDelete
GO


--- Ora si crea TRIGGER
create TRIGGER [@db_user].[RoleHistoryDelete]
ON [@db_user].[DPA_CORR_GLOBALI]
FOR  DELETE
   AS
  /******************************************************************************
    AUTHOR:    Samuele Furnari -- tradotte da De Luca Serpi
    NAME:      RoleHistoryDelete
    PURPOSE:   Ogni volta che viene eliminato un record dalla dpa_corr_globali
               vengono cancellati dallo storico tutti i record relativi alla 
               storia del ruolo eliminati
  ******************************************************************************/

   DECLARE @OLD_ORIGINAL_ID INT
   DECLARE @error INT

   begin
   select @OLD_ORIGINAL_ID = ORIGINAL_ID FROM deleted del where del.CHA_TIPO_URP = 'R'
   
      IF exists (select 'x' FROM deleted del where del.CHA_TIPO_URP = 'R') 
      begin
         DELETE FROM @db_user.DPA_ROLE_HISTORY
         WHERE ORIGINAL_CORR_ID = @OLD_ORIGINAL_ID
         SELECT   @error = @@ERROR
         IF (@error <> 0)
            RAISERROR('Error',16,1)
      end
   end
GO
              
----------- FINE -
              
---- RoleHistoryModify.MSSQL.sql  marcatore per ricerca ----
--- Controllo l'esistenza e l'eliminazione del Trigger ROLEHISTORYCREATE    
IF EXISTS (SELECT * FROM sys.triggers
			WHERE name = 'ROLEHISTORYMODIFY')
DROP TRIGGER [@db_user].[RoleHistoryModify]
GO

create TRIGGER [@db_user].[RoleHistoryModify]  
   ON  [@db_user].[DPA_CORR_GLOBALI] 
   FOR UPDATE
   AS
   IF EXISTS (SELECT * FROM inserted, deleted WHERE
			  inserted.cha_tipo_urp = 'R' AND
			  -- Giordano Iacozzilli
			  -- Aggiunta condizione per prendere solo gli interni.
			  inserted.cha_tipo_ie = 'I' and
			  inserted.dta_fine is null and
			  ( inserted.id_old != deleted.id_old or 
			  inserted.var_codice != deleted.var_codice or 
			  inserted.var_desc_corr != deleted.var_desc_corr or
			  inserted.id_uo != deleted.id_uo or
			  inserted.id_tipo_ruolo != deleted.id_tipo_ruolo))

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @insIdOld integer
	DECLARE	@delIdOld integer
	
	SELECT @insIdOld = i.ID_OLD, @delIdOld = d.ID_OLD
	FROM inserted i
	INNER JOIN deleted d
	ON d.SYSTEM_ID = i.SYSTEM_ID
	
	-- Verifica eventuale cambiamento su codice del ruolo
	if(@delIdOld != @insIdOld)
		BEGIN
			-- Nella dpa_role_history bisogna preventivamente aggiornare il campo
			-- role_id con il nuovo system_id ed in seguito inserire una nuova riga
			-- con id_old uguale a quella del record appena inserito
			UPDATE @db_user.dpa_role_history
			SET role_id = @insIdOld
			WHERE role_id = @delIdOld
	    
			-- Il ruolo id_old  stato storicizzato (inserimento di un record 
			-- di storicizzazione)
			INSERT
			  INTO @db_user.DPA_ROLE_HISTORY
				(
				  ACTION_DATE ,
				  UO_ID ,
				  ROLE_TYPE_ID ,
				  ORIGINAL_CORR_ID ,
				  ACTION ,
				  ROLE_DESCRIPTION,
				  ROLE_ID
				)
				SELECT
				  GETDATE(),
				  inserted.id_uo,
				  ISNULL(inserted.id_tipo_ruolo,0),
				  inserted.original_id,
				  'S',
				  inserted.var_desc_corr + ' (' + inserted.var_codice + ')',
				  inserted.system_id
				FROM inserted
		END
	else
		INSERT
		  INTO @db_user.DPA_ROLE_HISTORY
			(
			  ACTION_DATE ,
			  UO_ID ,
			  ROLE_TYPE_ID ,
			  ORIGINAL_CORR_ID ,
			  ACTION ,
			  ROLE_DESCRIPTION,
			  ROLE_ID
			)
			SELECT
			  GETDATE(),
			  inserted.id_uo,
			   ISNULL(inserted.id_tipo_ruolo,0),
			  inserted.original_id,
			  'M',
			  inserted.var_desc_corr + ' (' + inserted.var_codice + ')',
			  inserted.system_id
			FROM inserted
END              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- DPA_ANAGRAFICA_LOG.MSSQL.sql  marcatore per ricerca ----
declare @ultimoID int

begin

Insert into DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ( 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)


Insert into DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ('AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)

END
              
----------- FINE -
              
---- ins_DPA_CHIAVI_CONFIGURAZIONE.MSSQL.SQL  marcatore per ricerca ----
-- se presente ATIPICITA_DOC_FASC, disabilita e rendi non visibile
IF EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE WHERE var_codice ='ATIPICITA_DOC_FASC')
BEGIN
	update @db_user.DPA_CHIAVI_CONFIGURAZIONE
	set var_valore = '0', cha_visibile = '0'
	where var_codice ='ATIPICITA_DOC_FASC'
END
GO






  -- file sql di update per il CD -- 

----------- FINE -
              
---- CREATE_DPA_OBJECTS_SYNC_PENDING.MSSQL.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_OBJECTS_SYNC_PENDING
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_OBJECTS_SYNC_PENDING]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_OBJECTS_SYNC_PENDING](
	ID_DOC_OR_FASC INTEGER,
	TYPE CHAR(1),
	ID_GRUPPO_TO_SYNC INTEGER
)
END
GO              
----------- FINE -
              
---- CREATE_DPA_ROLE_HISTORY.MSSQL.SQL  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  15/11/2011
Scopo della modifica:        CREARE LA TABELLA DPA_ROLE_HISTORY
*/


IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_ROLE_HISTORY]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
) 
BEGIN
CREATE TABLE @db_user.DPA_ROLE_HISTORY
(
   System_Id INT IDENTITY(1,1)  Not Null  Primary Key,
   ORIGINAL_CORR_ID INT  NOT NULL,
   ACTION           CHAR(1)  NOT NULL,
   ROLE_DESCRIPTION VARCHAR(384)  NOT NULL,
   ROLE_TYPE_ID     INT  NOT NULL,
   Action_Date DATETIME  Not Null,
   Uo_Id			INT  Not Null,
   UO_DESCRIPTION_ VARCHAR(4000),
   ROLE_TYPE_DESCRIPTION_ VARCHAR(4000),
   ROLE_ID	INT  NOT NULL 
)

END
GO              
----------- FINE -
              
---- CREATE_DPA_VIS_ANOMALA.MSSQL.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_VIS_ANOMALA

--Problema nella ricerca della tabella temporanea, 
--Ora la trova, ho inserito la sintassi corretta.
-- Modifica
--OLD CODE:
--IF NOT EXISTS 
--(SELECT * FROM tempdb.dbo.sysobjects 
--	WHERE id = OBJECT_ID(N'[@db_user].[DPA_VIS_ANOMALA]') 
--	AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
--NEW CODE:
IF NOT EXISTS 
(SELECT * FROM tempdb.dbo.sysobjects 
WHERE id =(SELECT top 1 id FROM tempdb.dbo.sysobjects 
WHERE name like ('%#DPA_VIS_ANOMALA%') 
AND type in (N'U')))

BEGIN
CREATE TABLE [@db_user].#DPA_VIS_ANOMALA
	(	ID_GRUPPO  INTEGER	)
END
GO          


--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE WHERE var_codice ='ATIPICITA_DOC_FASC')
BEGIN
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,var_codice,	var_descrizione,
	var_valore,cha_tipo_chiave,	cha_visibile,
	cha_modificabile,cha_globale,var_codice_old_webconfig)
	values
	(0,'ATIPICITA_DOC_FASC',	'La chiave abilita o meno la verifica di atipicita di un documento o un fascicolo',
	'0',	'B',	'0', -- richiesta Luciani (non visibile) -- prev was '1'
	'1','1',null)
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
              
---- UPDATE_DPA_CORR_GLOBALI.MSSQL.SQL  marcatore per ricerca ----
BEGIN
BEGIN
if exists(select * from syscolumns where name='ORIGINAL_ID' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U'))
begin
declare @istruzione nvarchar(1000)

set @istruzione = N'update [@db_user].dpa_corr_globali 
		set original_id = system_id
		where cha_tipo_urp = ''R'''
EXEC sp_executesql @istruzione
END
END
END
GO              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
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
      N.IDOGGETTOASSOCIATO	 = @p_IDOGGETTOASSOCIATO AND
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
              
---- GETCORRDESCRIPTION.MSSQL.SQL  marcatore per ricerca ----
IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GETCORRDESCRIPTION]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].GETCORRDESCRIPTION 
go

create  FUNCTION [@db_user].GETCORRDESCRIPTION(@idCorrGlobali INT)
RETURNS VARCHAR(4000)  AS
BEGIN


   DECLARE @output VARCHAR(4000)  
   select @output  = var_desc_corr  + ' (' + var_codice + ')' 
	from dpa_corr_globali where system_id = @idCorrGlobali

   RETURN @output  

END
GO              
----------- FINE -
              
---- GETROLETYPEDESCRIPTION.MSSQL.SQL  marcatore per ricerca ----
IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GETROLETYPEDESCRIPTION]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].GETROLETYPEDESCRIPTION 
go

create FUNCTION @db_user.GETROLETYPEDESCRIPTION(@roleTypeId INT)
RETURNS VARCHAR(4000)  AS
BEGIN


   DECLARE @code VARCHAR(128)
   DECLARE @description VARCHAR(256)
   select   @code = var_codice, @description = var_desc_ruolo 
   from dpa_tipo_ruolo where system_id = @roleTypeId

   if(@description is not null and @code is not null)
      RETURN @description + ' (' + @code + ')'
else
   RETURN ''
   RETURN NULL

END
GO              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- 0.vis_doc_anomala_custom.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='VIS_DOC_ANOMALA_CUSTOM')
DROP PROCEDURE @db_user.vis_doc_anomala_custom
go

CREATE PROCEDURE @db_user.vis_doc_anomala_custom @p_id_amm INT, @p_querydoc VARCHAR(4000) 

AS

--DICHIARAZIONI

BEGIN

--CURSORE DOCUMENTI

   DECLARE @s_idg_security       INT

   DECLARE @s_ar_security        INT

   DECLARE @s_td_security        VARCHAR(2)

   DECLARE @s_vn_security        VARCHAR(255)

   DECLARE @s_idg_r_sup          INT

   DECLARE @s_doc_number         INT

   DECLARE @s_id_fascicolo       INT

   DECLARE @s_cha_cod_t_a_fasc   VARCHAR(1024)

   DECLARE @n_id_gruppo          INT

   DECLARE @codice_atipicita     VARCHAR(255)
   DECLARE @error INT


   execute(@p_querydoc)
   while 1 = 1
   begin
      FETCH documenti
      INTO @s_doc_number
      if @@FETCH_STATUS <> 0
      BREAK



--Cursore sulla security per lo specifico documento

      BEGIN
         DECLARE @c_idg_security CURSOR
         SET @c_idg_security = CURSOR  FOR SELECT personorgroup, accessrights, cha_tipo_diritto,
var_note_sec
         FROM security
         WHERE thing = @s_doc_number AND accessrights > 20
         OPEN @c_idg_security
         while 1 = 1
         begin
            FETCH @c_idg_security
            INTO @s_idg_security,@s_ar_security,@s_td_security,@s_vn_security
            if @@FETCH_STATUS <> 0
            BREAK



--Gerachia ruolo proprietario documento

            IF (UPPER(@s_td_security) = 'P')
            begin
               BEGIN
                  DECLARE @ruoli_sup CURSOR
                  SET @ruoli_sup = CURSOR  FOR SELECT @db_user.dpa_corr_globali.id_gruppo
                  FROM @db_user.dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                  ON dpa_corr_globali.id_tipo_ruolo =
                  dpa_tipo_ruolo.system_id
                  WHERE dpa_corr_globali.id_uo IN(SELECT     dpa_corr_globali.system_id
                     FROM @db_user.dpa_corr_globali
                     WHERE dpa_corr_globali.dta_fine IS NULL)
                  AND dpa_corr_globali.cha_tipo_urp = 'R'
                  AND dpa_corr_globali.id_amm = @p_id_amm
                  AND dpa_corr_globali.dta_fine IS NULL
                  AND dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello
                     FROM @db_user.dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                     ON dpa_corr_globali.id_tipo_ruolo =
                     dpa_tipo_ruolo.system_id
                     WHERE dpa_corr_globali.id_gruppo =
                     @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup
                     INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK



--DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || s_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);

                     INSERT INTO @db_user.dpa_vis_anomala(id_gruppo)
VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END



--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

--Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie


               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*)
                  FROM(SELECT id_gruppo
                     FROM dpa_vis_anomala
                     EXCEPT
                     SELECT personorgroup
                     FROM security
                     WHERE thing = @s_doc_number) AS TabAl
                  IF (@n_id_gruppo <> 0
                  AND ISNULL(CHARINDEX('AGRP',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGRP-'

               END
               COMMIT
            end




--Gerarchia destinatario trasmissione

            IF (UPPER(@s_td_security) = 'T')
            begin
               BEGIN
                  --DECLARE @ruoli_sup CURSOR
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                  ON dpa_corr_globali.id_tipo_ruolo =
                  dpa_tipo_ruolo.system_id
                  WHERE dpa_corr_globali.id_uo IN(SELECT     dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE dpa_corr_globali.dta_fine IS NULL)
                  AND dpa_corr_globali.cha_tipo_urp = 'R'
                  AND dpa_corr_globali.id_amm = @p_id_amm
                  AND dpa_corr_globali.dta_fine IS NULL
                  AND dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello
                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                     ON dpa_corr_globali.id_tipo_ruolo =
                     dpa_tipo_ruolo.system_id
                     WHERE dpa_corr_globali.id_gruppo =
                     @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup
                     INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK



--DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                     INSERT INTO dpa_vis_anomala(id_gruppo)
VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END



--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

--Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie


               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*)
                  FROM(SELECT id_gruppo
                     FROM dpa_vis_anomala
                     EXCEPT
                     SELECT personorgroup
                     FROM security
                     WHERE thing = @s_doc_number) AS TabAl
                  IF (@n_id_gruppo <> 0
                  AND ISNULL(CHARINDEX('AGDT',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGDT-'

               END
               COMMIT
            end




--Fascicolazione documento

            IF (UPPER(@s_td_security) = 'F')
            BEGIN
               DECLARE @fascicoli CURSOR
               SET @fascicoli = CURSOR  FOR SELECT system_id
               FROM project
               WHERE system_id IN(SELECT id_fascicolo
                  FROM project
                  WHERE system_id IN(SELECT project_id
                     FROM project_components
                     WHERE LINK =
                     @s_doc_number))
               AND cha_tipo_fascicolo = 'P'
               OPEN @fascicoli
               while 1 = 1
               begin
                  FETCH @fascicoli
                  INTO @s_id_fascicolo
                  if @@FETCH_STATUS <> 0
                  BREAK
                  SELECT   @s_cha_cod_t_a_fasc = cha_cod_t_a
                  FROM project
                  WHERE system_id = @s_id_fascicolo
                  IF (@s_cha_cod_t_a_fasc IS NOT NULL
                  AND UPPER(@s_cha_cod_t_a_fasc) <> 'T')
                     IF (ISNULL(CHARINDEX('AFCD',@codice_atipicita),0) = 0)
                        SET @codice_atipicita = @codice_atipicita + 'AFCD-'

               end
               CLOSE @fascicoli
            END




--Gerarchia ruolo destinatario di copia visibilita

            IF (UPPER(@s_td_security) = 'A'
            AND UPPER(@s_vn_security) =
            'ACQUISITO PER COPIA VISIBILITA')
            begin
               BEGIN
                  --DECLARE @ruoli_sup CURSOR
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                  ON dpa_corr_globali.id_tipo_ruolo =
                  dpa_tipo_ruolo.system_id
                  WHERE dpa_corr_globali.id_uo IN(SELECT     dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE dpa_corr_globali.dta_fine IS NULL)
                  AND dpa_corr_globali.cha_tipo_urp = 'R'
                  AND dpa_corr_globali.id_amm = @p_id_amm
                  AND dpa_corr_globali.dta_fine IS NULL
                  AND dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello
                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                     ON dpa_corr_globali.id_tipo_ruolo =
                     dpa_tipo_ruolo.system_id
                     WHERE dpa_corr_globali.id_gruppo =
                     @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup
                     INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK



--DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                     INSERT INTO dpa_vis_anomala(id_gruppo)
VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END



--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

--Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie

               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*)
                  FROM(SELECT id_gruppo
                     FROM dpa_vis_anomala
                     EXCEPT
                     SELECT personorgroup
                     FROM security
                     WHERE thing = @s_doc_number) AS TabAl
                  IF (@n_id_gruppo <> 0
                  AND ISNULL(CHARINDEX('AGCV',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGCV-'

               END
               COMMIT
            end
         end
         CLOSE @c_idg_security
      END



--Restituzione codice di atipicita

      IF (@codice_atipicita IS NULL)
      begin
         SET @codice_atipicita = 'T'



--DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

         update PROFILE set cha_cod_t_a = @codice_atipicita  WHERE docnumber = @s_doc_number
         COMMIT
         SET @codice_atipicita = NULL
      end

      IF (SUBSTRING(@codice_atipicita,LEN(@codice_atipicita),LEN(@codice_atipicita) -LEN(@codice_atipicita)) = '-')
      begin
         SET @codice_atipicita = SUBSTRING(@codice_atipicita,0,LEN(@codice_atipicita) -1)



--DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

         update PROFILE set cha_cod_t_a = @codice_atipicita  WHERE docnumber = @s_doc_number
         COMMIT
         SET @codice_atipicita = NULL
      end
   end
   CLOSE documenti

END


GO              
----------- FINE -
              
---- CalculateAtipDelRole.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CalculateAtipDelRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='CalculateAtipDelRole')
DROP PROCEDURE @db_user.CalculateAtipDelRole
go


create PROCEDURE @db_user.CalculateAtipDelRole 
	-- Id corr globali della UO cui apparteneva il ruolo eliminato
@idUO        INT  ,
	-- Id dell'amministrazione cui appartiene la UO
@idAmm       INT  ,
	-- Id del livello del ruolo eliminato
@roleLevelId   INT  ,
@returnValue INT OUTPUT 
AS
BEGIN
/*
AUTORE:   Samuele Furnari

NAME:     CalculateAtipDelRole

PURPOSE:  Store per il calcolo dell'atipicit di documenti e fascicoli di un ruolo
e dei suoi sottoposti. Questo calcolo viene eseguito quando si elimina
un ruolo

******************************************************************************/

  -- Id del tipo ruolo del ruolo eliminato
   DECLARE @roleLevel INT
   DECLARE @keyValue VARCHAR(200) = ''

  -- Query per l'estrazione degli id degli oggetti visti dai ruoli presenti nella
  -- UO del ruolo eliminato che abbiano livello inferiore o uguale a quello
  -- del ruolo elminato
   DECLARE @rolesObjects VARCHAR(2000)
   Select   @roleLevel = num_livello From dpa_tipo_ruolo Where system_id = @roleLevelId
   SET @rolesObjects = 'Select distinct(s.thing)  
						From security s 
						Inner Join profile p
						On p.system_id = s.thing
						Where personorgroup In (
						Select distinct(p.id_gruppo) 
						From dpa_corr_globali p
						Where id_amm = ' + @idAmm + '
						And ((Select num_livello From dpa_tipo_ruolo 
						Where system_id = p.id_tipo_ruolo) >= ' + @roleLevel + ')
						And p.id_uo In (
						Select p.SYSTEM_ID 
						From dpa_corr_globali p
						Start With p.SYSTEM_ID =' + @idUO + '
						Connect By Prior
						p.SYSTEM_ID = p.ID_PARENT 
						And p.CHA_TIPO_URP = ''U'' 
						And p.ID_AMM = ' + @idAmm + '))'

	-- Esecuzione calcolo di atipicit su documenti e fascicoli solo se attiva
    -- Recupero dello stato di abilitazione del calcolo di atipicit
   SELECT  TOP 1 @keyValue = var_valore FROM dpa_chiavi_configurazione 
   WHERE var_codice = 'ATIPICITA_DOC_FASC' AND (id_amm = 0 OR id_amm = @idAmm) 
   If @keyValue = '1'
   Begin
      EXECUTE vis_doc_anomala_custom @idAmm,@rolesObjects
      EXECUTE vis_fasc_anomala_custom @idAmm,@rolesObjects
   End

   SET @returnValue = 1  
END 
GO
              
----------- FINE -
              
---- COMPUTEATIPICITAINSROLE.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP COMPUTEATIPICITAINSROLE
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='COMPUTEATIPICITAINSROLE')
DROP PROCEDURE @db_user.COMPUTEATIPICITAINSROLE
go



create PROCEDURE @db_user.COMPUTEATIPICITAINSROLE 
  -- Id dell'amministrazione
@IdAmm INT   
  -- Id della uo in cui  stato inserito il ruolo
, @idUo INT   
, @returnValue INT OUTPUT  AS
BEGIN
/******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     COMPUTEATIPICITAINSROLE

    PURPOSE:  Store per il calcolo dell'atipicit di documenti e fascicoli eseguita
              nel momento in cui viene inserito un ruolo

  ******************************************************************************/

   BEGIN
    -- Calcolo dell'atipicit sui documenti
      DECLARE @vis_doc_anomala_custom VARCHAR(255)
      DECLARE @vis_fasc_anomala_custom VARCHAR(255)
      
      
      set @vis_doc_anomala_custom = 'select distinct(s.thing) 
									from security s 
									inner join profile p
									on p.system_id = s.thing
									where personorgroup in (
									select distinct(p.id_gruppo) 
									from dpa_corr_globali p
									where id_amm = ' + @IdAmm + ' AND 
                                    p.id_uo in (
									select p.SYSTEM_ID 
									from dpa_corr_globali p
									start with p.SYSTEM_ID = ' +
									@idUo + 'connect by prior
                                    p.SYSTEM_ID= p.ID_PARENT AND 
									p.CHA_TIPO_URP = ''U'' AND 
									p.ID_AMM = ' + @IdAmm + '))'
      
      EXECUTE vis_doc_anomala_custom @IdAmm,@vis_doc_anomala_custom
      
    -- Calcolo dell'atipicit sui fascicoli
      set @vis_fasc_anomala_custom = 'select distinct(s.thing)
									from security s 
									inner join project p
									on p.system_id = s.thing
									where personorgroup in (
									select distinct(p.id_gruppo) 
									from dpa_corr_globali p
									where id_amm = ' + @IdAmm + 'AND 
                                     p.id_uo in (
									select p.SYSTEM_ID 
									from dpa_corr_globali p
									start with p.SYSTEM_ID = ' +
									@idUo + ' connect by prior
                                    p.SYSTEM_ID = p.ID_PARENT AND 
									p.CHA_TIPO_URP = ''U'' AND 
									p.ID_AMM = ' + @IdAmm + ')
                                    and p.cha_tipo_fascicolo = ''P'')'
                                    
      EXECUTE vis_fasc_anomala_custom @IdAmm,@vis_fasc_anomala_custom
   END

   SET @returnValue = 0

END 
GO
              
----------- FINE -
              
---- COMPUTEATIPICITA.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP COMPUTEATIPICITA
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='COMPUTEATIPICITA')
DROP PROCEDURE @db_user.COMPUTEATIPICITA
go


create PROCEDURE @db_user.COMPUTEATIPICITA 
  -- Id della UO in cui  inserito il ruolo
@idUo  INT  ,
  -- Id del gruppo per cui calcolare l'atipicit
@IdGroup VARCHAR(4000)   
  -- Id dell'ammionistrazione
, @Idamm VARCHAR(4000) 
  -- Id del tipo ruolo
, @idTipoRuolo INT
  -- Id del veccho tipo ruolo
, @idTipoRuoloVecchio INT
  -- Id della vecchia UO
, @idVecchiaUo INT
  -- 1 se  stato richiesto di calcolare l'atipicit sui sottoposti
, @calcolaSuiSottoposti INT
  -- Valore restitua
, @returnValue INT OUTPUT  AS
BEGIN
/******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     COMPUTEATIPICITA

    PURPOSE:  Store per il calcolo dell'atipicit di documenti e fascicoli di un ruolo
              ATTENZIONE! Questa procedura non deve essere utilizzata per calcolare
              l'atipicit di un ruolo appena inserito. Per il calcolo di atipicit
              su ruoli appena inseriti utilizzare la store COMPUTEATIPICITAINSROLE

  ******************************************************************************/
  
  -- Livello del ruolo prima e dopo la modifica
   DECLARE @oldLevel INT
   DECLARE @newLevel INT

  -- Calcolo della atipicit su documenti e fascicoli visti dal ruolo, solo se
  -- attiva per l'amministrazione o per l'installazione e se richiesto
   DECLARE @keyValue VARCHAR(128)
 -- Query custom da eseguire per il calcolo degli id degli oggetti sui cui calcolare l'atipicit
   DECLARE @queryCustomDoc VARCHAR(2000) = ''
   DECLARE @queryCustomFasc VARCHAR(2000) = ''
 -- Recupero dello stato di abilitazione del calcolo di atipicit
   SELECT  TOP 1 @keyValue = var_valore FROM dpa_chiavi_configurazione 
   WHERE var_codice = 'ATIPICITA_DOC_FASC' AND (id_amm = 0 OR id_amm = @Idamm) 

    -- Recupero dei livelli del ruolo prima e dopo la modifica
   Select   @oldLevel = num_livello From dpa_tipo_ruolo Where system_id = @idTipoRuoloVecchio
   Select   @newLevel = num_livello From dpa_tipo_ruolo Where system_id = @idTipoRuolo
   If @keyValue = '1'
   Begin
        -- Calcolo dell'atipicit per gli oggetti del ruolo che ha subito modifiche
      SET @queryCustomDoc = 'Select p.system_id 
							From security s 
							Inner Join profile p 
							On p.system_id = s.thing 
							Where personorgroup = ' + @IdGroup +
							' And accessrights > 20 
							And p.cha_tipo_proto In (''A'',''P'',''I'',''G'')
							And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'')'
      
      SET @queryCustomFasc = 'Select p.system_id  
							From security s 
							Inner Join project p 
							On p.system_id = s.thing 
							Where personorgroup = ' + @IdGroup +
							' And accessrights > 20 
							And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' ) 
							AND p.cha_tipo_fascicolo = ''P'''

        -- Esecuzione del calcolo dell'atipicit
      EXECUTE vis_doc_anomala_custom @Idamm,@queryCustomDoc
      EXECUTE vis_fasc_anomala_custom @Idamm,@queryCustomFasc

        -- Pulizia delle query
      SET @queryCustomDoc = ''
      SET @queryCustomFasc = ''

        -- Se c' stata discesa gerarchica, viene calcolata l'atipicit sui superiori 
        -- che prima della modifica erano sottoposti
      If @newLevel > @oldLevel
      Begin
         SET @queryCustomDoc = ' Select Distinct(s.thing)
								From security s 
								Inner Join profile p
								On p.system_id = s.thing
								Where personorgroup In (
								Select Distinct(p.id_gruppo)
								From dpa_corr_globali p
								Where id_amm = ' + @Idamm + ' 
								And (Select num_livello  From dpa_tipo_ruolo 
								Where system_id = p.id_tipo_ruolo) >= ' + @oldLevel + '
								And (Select num_livello from dpa_tipo_ruolo 
								Where system_id = p.id_tipo_ruolo) <= ' + @newLevel + '
								And id_gruppo != ' + @IdGroup + '
								And p.id_uo In (
								Select p.SYSTEM_ID
								From dpa_corr_globali p
								Start With p.SYSTEM_ID = ' + @idUo + '
								Connect By Prior
								p.ID_PARENT = p.SYSTEM_ID And 
								p.CHA_TIPO_URP = ''U'' And 
								p.ID_AMM = ' + @Idamm + '))
								And s.accessrights > 20 
								And p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
								And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )'
         
         SET @queryCustomFasc = 'Select Distinct(s.thing) 
								From security s 
								Inner Join project p
								On p.system_id = s.thing
								Where personorgroup In (
								Select Distinct(p.id_gruppo)
								From dpa_corr_globali p
								Where id_amm = ' + @Idamm +
								'And (Select num_livello From dpa_tipo_ruolo 
								Where system_id = p.id_tipo_ruolo) >= ' + @oldLevel + '
								And (Select num_livello from dpa_tipo_ruolo 
								Where system_id = p.id_tipo_ruolo) <= ' + @newLevel + '
								And id_gruppo != ' + @IdGroup + '
								And p.id_uo In (
								Select p.SYSTEM_ID
								From dpa_corr_globali p
								Start With p.SYSTEM_ID = ' + @idUo + '
								Connect By Prior
                                p.ID_PARENT = p.SYSTEM_ID And 
								p.CHA_TIPO_URP = ''U'' And 
								p.ID_AMM = ' + @Idamm + '))
                                AND s.accessrights > 20
								And p.cha_tipo_fascicolo = ''P'''
      End
   Else 
   
               -- Altrimenti se c' stata salita gerarchica o se c' stato spostamento
            -- ed  stato richiesto il calcolo dell'atipicit, viene calcolata
            -- l'atipicit sui sottoposti
   If @newLevel < @oldLevel Or (@idUo != @idVecchiaUo And @calcolaSuiSottoposti = 1)
      Begin
         SET @queryCustomDoc = 'select distinct(s.thing) 
								from security s 
								inner join profile p
								on p.system_id = s.thing
								where personorgroup in (
								select distinct(p.id_gruppo) 
								from dpa_corr_globali p
								where id_amm = ' + @Idamm + ' AND
                                p.id_uo in (
								select p.SYSTEM_ID 
								from dpa_corr_globali p
								start with p.SYSTEM_ID = ' + @idUo +'
								connect by prior
                                p.SYSTEM_ID = p.ID_PARENT AND 
								p.CHA_TIPO_URP = ''U'' AND 
								p.ID_AMM = ' + @Idamm + '))
                                AND s.accessrights > 20
								AND p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
								AND (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )
								And s.personorgroup != ' + @IdGroup
         
         SET @queryCustomFasc = 'select distinct(s.thing) 
								from security s 
								inner join project p
								on p.system_id = s.thing
								where personorgroup in (
								select distinct(p.id_gruppo) 
								from dpa_corr_globali p
								where id_amm = ' + @Idamm + '
								AND p.id_uo in (
								select p.SYSTEM_ID 
								from dpa_corr_globali p
								start with p.SYSTEM_ID = ' + @idUo +
								'connect by prior
                                p.SYSTEM_ID  = p.ID_PARENT AND 
								p.CHA_TIPO_URP = ''U'' AND 
								p.ID_AMM = ' + @Idamm + ')
                                AND s.accessrights > 20
								and p.cha_tipo_fascicolo = ''P'')
								And s.personorgroup != ' + @IdGroup 


                -- Esecuzione del calcolo dell'atipicit
         EXECUTE vis_doc_anomala_custom @Idamm,@queryCustomDoc
         EXECUTE vis_fasc_anomala_custom @Idamm,@queryCustomFasc
      End

  

        -- Se  stato compiuto uno spostamento viene ricalcolata l'atipicit anche
        -- sui sottoposti del ruolo nella catena di origine
      If @idUo != @idVecchiaUo
      Begin
         DECLARE @vis_doc_anomala_custom VARCHAR(255)
         DECLARE @vis_fasc_anomala_custom VARCHAR(255)
         set @vis_doc_anomala_custom = 'Select Distinct(s.thing) 
										From security s 
										Inner Join profile p
										On p.system_id = s.thing
										Where personorgroup In (
										Select Distinct(p.id_gruppo)
										From dpa_corr_globali p
										Where id_amm = ' + @Idamm + '
										And (Select num_livello From dpa_tipo_ruolo 
										Where system_id = p.id_tipo_ruolo) >= ' + @oldLevel + '
										And p.id_uo In (
										Select p.SYSTEM_ID
										From dpa_corr_globali p
										Start With p.SYSTEM_ID = ' + @idVecchiaUo + '
										Connect By Prior
                                         p.system_id = p.id_parent And 
										p.CHA_TIPO_URP = ''U'' And 
										p.ID_AMM = ' + @Idamm + '))
                                        And s.accessrights > 20
										And p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
										And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )'
         
         EXECUTE vis_doc_anomala_custom @Idamm,@vis_doc_anomala_custom
         
         set @vis_fasc_anomala_custom = 'Select Distinct(s.thing) 
										From security s 
										Inner Join project p
										On p.system_id = s.thing
										Where personorgroup In (
										Select Distinct(p.id_gruppo)
										From dpa_corr_globali p
										Where id_amm = ' + @Idamm + '
										And (Select num_livello From dpa_tipo_ruolo 
										Where system_id = p.id_tipo_ruolo) >= ' + @oldLevel + '
										And p.id_uo In (
										Select p.SYSTEM_ID
										From dpa_corr_globali p
										Start With p.SYSTEM_ID = ' + @idVecchiaUo + '
										Connect By Prior
                                        p.system_id = p.id_parent And 
										p.CHA_TIPO_URP = ''U'' And 
										p.ID_AMM = ' + @Idamm + '))
										And s.accessrights > 20
										And p.cha_tipo_fascicolo = ''P'''
										
										
         EXECUTE vis_fasc_anomala_custom @Idamm,@vis_fasc_anomala_custom
      End

   End

   SET @returnValue = 0
END 

GO
              
----------- FINE -
              
---- COPYSECURITY.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CopySecurity
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='CopySecurity')
DROP PROCEDURE @db_user.CopySecurity
go



create PROCEDURE @db_user.CopySecurity 
  -- Id gruppo del ruolo di cui copiare la visibilit
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
      SET @colValuesList = @colValuesList + ', DISTINCT(thing)'
      ELSE IF(@colName) = 'PERSONORGROUP'
      SET @colValuesList = @colValuesList + ', ' + @destinationGroupId
      ELSE IF(@colName) = 'ACCESSRIGHTS'
      SET @colValuesList = @colValuesList + ', DECODE(accessrights, 255, 63, accessrights, accessrights)'
      ELSE IF(@colName) = 'CHA_TIPO_DIRITTTO'
      SET @colValuesList = @colValuesList + ', A'
      ELSE
      SET @colValuesList = @colValuesList + ', ' + @colName
   end
   CLOSE @curColumns
   SET @colNameList = SUBSTRING(@colNameList,3,LEN(@colNameList) -3)
   SET @colValuesList = SUBSTRING(@colValuesList,3,LEN(@colValuesList) -3)
   execute('INSERT INTO security (' + @colNameList + ') ( SELECT ' + @colValuesList + '
													FROM security s WHERE personorgroup = 
													' + @sourceGroupId + ' AND NOT EXISTS  
													(select ''x'' from security 
													where thing = s.thing and 
													personorgroup = ' + @destinationGroupId + '))')  
END 

GO
              
----------- FINE -
              
---- CopySecurityWithoutAtipici.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CopySecurityWithoutAtipici
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='CopySecurityWithoutAtipici')
DROP PROCEDURE @db_user.CopySecurityWithoutAtipici
go



create PROCEDURE @db_user.CopySecurityWithoutAtipici 
  -- Id gruppo del ruolo di cui copiare la visibilit
@sourceGroupId INT ,
  -- Id gruppo del ruolo di destinazione
@destinationGroupId INT 
AS
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     CopySecurityWithoutAtipici

    PURPOSE:  Store per la copia di alcuni record della security esclusi 
              i record relativi a documenti e fascicoli atipici. I record 
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
      SET @colValuesList = @colValuesList + ', DISTINCT(thing)'
      ELSE IF(@colName) = 'PERSONORGROUP'
      SET @colValuesList = @colValuesList + ', ' + @destinationGroupId
      ELSE IF(@colName) = 'ACCESSRIGHTS'
      SET @colValuesList = @colValuesList + ', DECODE(accessrights, 255, 63, accessrights, accessrights)'
      ELSE IF(@colName) = 'CHA_TIPO_DIRITTTO'
      SET @colValuesList = @colValuesList + ', A'
      ELSE
      SET @colValuesList = @colValuesList + ', ' + @colName
   end
   CLOSE @curColumns
   SET @colNameList = SUBSTRING(@colNameList,3,LEN(@colNameList) -3)
   SET @colValuesList = SUBSTRING(@colValuesList,3,LEN(@colValuesList) -3)
   execute('INSERT INTO security (' + @colNameList + ') 
			( SELECT ' + @colValuesList + ' FROM security s inner join profile p on 
			s.thing = p.system_id where s.personorgroup = ' + @sourceGroupId + ' 
			and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS 
			(select ''x'' from security where thing = s.thing and personorgroup = 
			' + @destinationGroupId + '))')
			
   execute('INSERT INTO security (' + @colNameList + ') 
			( SELECT ' + @colValuesList + ' FROM security s inner join project p on 
			s.thing = p.system_id where s.personorgroup = ' + @sourceGroupId + ' 
			and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS 
			(select ''x'' from security where thing = s.thing and personorgroup = 
			' + @destinationGroupId + '))')  
			
END 
GO
              
----------- FINE -
              
---- ExtendVisibilityToHigherRoles.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP ExtendVisibilityToHigherRoles
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='ExtendVisibilityToHigherRoles')
DROP PROCEDURE @db_user.ExtendVisibilityToHigherRoles
go


create PROCEDURE @db_user.ExtendVisibilityToHigherRoles 
  -- Id dell'amministrazione 
@idAmm INT ,
  -- Id del gruppo da analizzare e di cui estendere la visibilit
@idGroup INT ,
  -- Indicatore dello scope della procedura di estensione di visibilit
  -- A -> Tutti, E -> Esclusione degli atipici
@extendScope VARCHAR(4000) ,
  -- Flag utilizzato per indicare se bisogna copiare gli id di documenti e
  -- fascicoli nella tabella temporanea per l'allineamento asincrono con Documentum
@copyIdToTempTable INT ,
@returnValue INT OUTPUT  AS
BEGIN
   /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     ExtendVisibilityToHigherRoles

    PURPOSE:  Store per l'estensione della visibilit ai ruoli superiori. 

  ******************************************************************************/

  -- Selezione dei superiori dei ruolo
   DECLARE @higherRoles CURSOR
  -- Id gruppo da analizzare
   DECLARE @idGroupToAnalyze  INT
   
   
   SET @higherRoles = CURSOR  FOR SELECT
   dpa_corr_globali.id_gruppo
   FROM dpa_corr_globali
   INNER JOIN dpa_tipo_ruolo
   ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
   WHERE dpa_corr_globali.id_uo IN(SELECT dpa_corr_globali.system_id
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.dta_fine IS NULL)
   AND dpa_corr_globali.CHA_TIPO_URP = 'R'
   AND dpa_corr_globali.ID_AMM = @idAmm
   AND dpa_corr_globali.DTA_FINE IS NULL
   AND dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello
      FROM dpa_corr_globali
      INNER JOIN dpa_tipo_ruolo
      ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
      WHERE dpa_corr_globali.id_gruppo  = @idGroup)
   OPEN @higherRoles
   while 1 = 1
   begin
      FETCH @higherRoles INTO @idGroupToAnalyze
      if @@FETCH_STATUS <> 0
      BREAK  

    -- Per ogni ruolo superiore, viene effettuata una operazione diversa 
    -- a seconda del tipo di estensione da eseguire
      IF(@extendScope) = 'A'
      EXECUTE CopySecurity @idGroup,@idGroupToAnalyze
      ELSE IF(@extendScope) = 'E'
      EXECUTE CopySecurityWithoutAtipici @idGroup,@idGroupToAnalyze

    -- Se richiesto, viene aggiornata la tabella delle sincronizzazione pending
      IF @copyIdToTempTable = 1
      BEGIN
 INSERT INTO dpa_objects_sync_pending(id_doc_or_fasc,
type,
id_gruppo_to_sync)(SELECT p.system_id, 'D', @idGroupToAnalyze
            FROM security s
            Inner Join Profile P On P.System_Id = S.Thing
            And (P.Cha_Privato Is Null Or P.Cha_Privato != 0)
            And (P.Cha_Personale is null Or P.Cha_Personale != 0)
            WHERE personorgroup = @idGroupToAnalyze and
            not exists(select 'x'
               from dpa_objects_sync_pending sp
               where sp.id_doc_or_fasc = s.thing AND
               sp.id_gruppo_to_sync = @idGroupToAnalyze))
         INSERT INTO dpa_objects_sync_pending(id_doc_or_fasc,
type,
id_gruppo_to_sync)(SELECT p.system_id, 'F', @idGroupToAnalyze
            FROM security s
            INNER JOIN project p ON p.system_id = s.thing
            Where Personorgroup = @idGroupToAnalyze And
(P.Cha_Privato Is Null Or P.Cha_Privato != 0) And
            not exists(select 'x'
               from dpa_objects_sync_pending sp
               where sp.id_doc_or_fasc = s.thing AND
               sp.id_gruppo_to_sync = @idGroupToAnalyze))
      END
   end
   CLOSE @higherRoles
   SET @returnValue = 0
END 
GO
              
----------- FINE -
              
---- INITIALIZEROLEHISTORY.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP Initializerolehistory
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='Initializerolehistory')
DROP PROCEDURE @db_user.Initializerolehistory
go

create Procedure @db_user.Initializerolehistory  AS
BEGIN

  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      InitializeRoleHistory

    PURPOSE:   Store procedure per l'inizializzazione della tabella dello storico
               dei ruoli
 
  ******************************************************************************/

    

   DECLARE @idRole INT
   DECLARE @idUo INT
   DECLARE @idRoleType INT
   DECLARE @originalId INT
   DECLARE @startTime DATETIME
   DECLARE @description VARCHAR(256)
   DECLARE @code VARCHAR(128)
   DECLARE @Cursore CURSOR

   SET @Cursore = CURSOR  FOR select id_uo, id_tipo_ruolo, 
										original_id, dta_inizio, var_codice, 
										var_desc_corr, System_Id
   from dpa_corr_globali
   where id_old = '0' and system_id = original_id
   and id_uo is not null
   and id_tipo_ruolo is not null
   Open @Cursore
   while 1 = 1
   begin
      FETCH @Cursore INTO @idUo,@idRoleType,@originalId,@startTime,@code,@description,@idRole 
      if @@FETCH_STATUS <> 0
      BREAK
      insert
      INTO DPA_ROLE_HISTORY(ACTION_DATE, UO_ID, ROLE_TYPE_ID, ORIGINAL_CORR_ID, Action, Role_Description, ROLE_ID)
VALUES(@startTime, @idUo, @idRoleType, @originalId, 'C', @description + ' (' + @code + ')', @idRole)
   end
   close @Cursore    
End 
GO              
----------- FINE -
              
---- RemoveTransModelVisibileToRole.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP RemoveTransModelVisibileToRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='RemoveTransModelVisibileToRole')
DROP PROCEDURE @db_user.RemoveTransModelVisibileToRole
go


create PROCEDURE @db_user.RemoveTransModelVisibileToRole 
    -- Id corr globali del ruolo di cui eleminare i modelli
@roleCorrBGlobId     INT  AS

/******************************************************************************
   NAME:       RemoveTransModelVisibileToRole
   AUTHOR:     Samuele Furnari
   PURPOSE:    Store procedure per la cancellazione di modelli di trasmissione
               visibili solo al ruolo con id corr globali pari a quello
               passato per parametro

******************************************************************************/

begin   

   DECLARE @idModello INT
   DECLARE @countIdModello INT

-- Cursore per scorrere sugli id di modelli di trasmissione che hanno 
-- esattamente un solo mittente.
   DECLARE @models CURSOR
   DECLARE @SWV_error INT

       -- Apertura cursore
   SET @models = CURSOR  FOR select md.ID_MODELLO, count(*) as numMitt
   from dpa_modelli_mitt_dest md
   where md.CHA_TIPO_MITT_DEST = 'M' and md.ID_CORR_GLOBALI = @roleCorrBGlobId
   group by md.id_modello order by md.ID_MODELLO
   open @models

   while 1 = 1
   begin
      fetch @models into @idModello,@countIdModello
      if @@FETCH_STATUS <> 0
      BREAK
      if @countIdModello = 1
      begin
            -- Cancellazione delle righe della DPA_MODELLI_DEST_CON_NOTIFICA
         delete dpa_modelli_dest_con_notifica where id_modello = @idModello

            -- Cancellazione delle righe della DPA_MODELLI_MITT_DEST
         delete dpa_modelli_mitt_dest where id_modello = @idModello

            -- Cancellazione della tupla da DPA_MODELLI_TRASM
         delete dpa_modelli_trasm where system_id = @idModello
      end
   end

   CLOSE @models

END 
GO
              
----------- FINE -
              
---- set_data_reg.MSSQL.sql  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='SET_DATA_REG')
DROP PROCEDURE @db_user.SET_DATA_REG
go
CREATE PROCEDURE @db_user.SET_DATA_REG AS

declare @sysid int
BEGIN

DECLARE currReg CURSOR FOR
select  a.system_id
	from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
	and a.cha_automatico='1' and cha_stato='A'
begin
SET @sysid=0

OPEN currReg
FETCH NEXT FROM currReg 	INTO @sysid
WHILE @@FETCH_STATUS = 0
BEGIN
	insert into dpa_registro_sto(dta_open,dta_close,num_rif,id_registro,id_people,id_ruolo_in_uo)
	SELECT a.dta_open,getdate(),b.num_rif,a.system_id,1,1
	from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
	and a.system_id = @sysid

	update dpa_el_registri set dta_open =getdate(),cha_stato ='A',dta_close = null
	WHERE SYSTEM_ID=@sysid

	update dpa_reg_proto set num_rif=1 
		where  substring(convert(varchar,getdate(),103),1,5)='01/01'
		-- agisce solo per quei registri che hanno autopertura impostata, cha_automatico = '1'  
				 and id_registro = @sysid

	FETCH NEXT FROM currReg  INTO  @sysid
END


CLOSE currReg
DEALLOCATE currReg
END
END
GO
              
----------- FINE -
              
---- VIS_DOC_ANOMALA_DOC_NUMBER,MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP RemoveTransModelVisibileToRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='VIS_DOC_ANOMALA_INT_DATE')
DROP PROCEDURE @db_user.VIS_DOC_ANOMALA_INT_DATE
go

create procedure @db_user.VIS_DOC_ANOMALA_INT_DATE 
@p_id_amm INT, 
@p_doc_number INT , 
@p_codice_atipicita VARCHAR(4000) 
OUTPUT  AS

--DICHIARAZIONI
BEGIN

--Cursore sulla security per lo specifico documento
   DECLARE @s_idg_security INT
   DECLARE @s_ar_security INT
   DECLARE @s_td_security VARCHAR(2)
   DECLARE @s_vn_security VARCHAR(255)
   DECLARE @s_idg_r_sup INT
   DECLARE @s_id_fascicolo INT
   DECLARE @s_cha_cod_t_a_fasc VARCHAR(1024)
   DECLARE @n_id_gruppo INT
   DECLARE @error INT
   DECLARE @ruoli_sup CURSOR

   BEGIN 
      DECLARE @c_idg_security CURSOR
      SET @c_idg_security = CURSOR  FOR SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec
      FROM security
      WHERE
      thing = @p_doc_number
      AND
      accessrights > 20
      OPEN @c_idg_security
      while 1 = 1
      begin
         FETCH @c_idg_security INTO @s_idg_security,@s_ar_security,@s_td_security,@s_vn_security
         if @@FETCH_STATUS <> 0
         BREAK

--Gerachia ruolo proprietario documento
         IF(upper(@s_td_security) = 'P')
         begin
            BEGIN 
               SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
               FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
               WHERE
               dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                  FROM dpa_corr_globali
                  WHERE
                  dpa_corr_globali.dta_fine IS NULL)
               AND
               dpa_corr_globali.CHA_TIPO_URP = 'R'
               AND
               dpa_corr_globali.ID_AMM = @p_id_amm
               AND
               dpa_corr_globali.DTA_FINE IS NULL
               AND
               dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
               OPEN @ruoli_sup
               while 1 = 1
               begin
                  FETCH @ruoli_sup INTO @s_idg_r_sup
                  if @@FETCH_STATUS <> 0
                  BREAK
--DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || p_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);                  
				INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
               end
               CLOSE @ruoli_sup
            END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
               SET @n_id_gruppo = 0
               SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                  EXCEPT
                  SELECT PERSONORGROUP FROM SECURITY WHERE THING = @p_doc_number) AS TabAl
               IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGRP',@p_codice_atipicita),0) = 0)
                  SET @p_codice_atipicita = @p_codice_atipicita + 'AGRP-'

            END
            COMMIT
         end


--Gerarchia destinatario trasmissione         
		IF(upper(@s_td_security) = 'T')
         begin
            BEGIN 
               SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
               FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
               WHERE
               dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                  FROM dpa_corr_globali
                  WHERE
                  dpa_corr_globali.dta_fine IS NULL)
               AND
               dpa_corr_globali.CHA_TIPO_URP = 'R'
               AND
               dpa_corr_globali.ID_AMM = @p_id_amm
               AND
               dpa_corr_globali.DTA_FINE IS NULL
               AND
               dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
               OPEN @ruoli_sup
               while 1 = 1
               begin
                  FETCH @ruoli_sup INTO @s_idg_r_sup
                  if @@FETCH_STATUS <> 0
                  BREAK                   
--DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || p_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);                  
				INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
               end
               CLOSE @ruoli_sup
            END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
               SET @n_id_gruppo = 0
               SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                  EXCEPT
                  SELECT PERSONORGROUP FROM SECURITY WHERE THING = @p_doc_number) AS TabAl
               IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGDT',@p_codice_atipicita),0) = 0)
                  SET @p_codice_atipicita = @p_codice_atipicita + 'AGDT-'

            END
            COMMIT
         end


--Fascicolazione documento
         IF(upper(@s_td_security) = 'F')
         BEGIN 
            DECLARE @fascicoli CURSOR
            SET @fascicoli = CURSOR  FOR select system_id from project where system_id in(select id_fascicolo from project where system_id in(select project_id from project_components where link = @p_doc_number)) and cha_tipo_fascicolo = 'P'
            OPEN @fascicoli
            while 1 = 1
            begin
               FETCH @fascicoli INTO @s_id_fascicolo
               if @@FETCH_STATUS <> 0
               BREAK
               SELECT   @s_cha_cod_t_a_fasc = cha_cod_t_a FROM project WHERE system_id = @s_id_fascicolo
               IF(@s_cha_cod_t_a_fasc is not null AND upper(@s_cha_cod_t_a_fasc) <> 'T')
                  IF(ISNULL(CHARINDEX('AFCD',@p_codice_atipicita),0) = 0)
                     SET @p_codice_atipicita = @p_codice_atipicita + 'AFCD-'

            end
            CLOSE @fascicoli
         END


--Gerarchia ruolo destinatario di copia visibilit
         IF(upper(@s_td_security) = 'A' AND upper(@s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA')
         begin
            BEGIN 
               SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
               FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
               WHERE
               dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                  FROM dpa_corr_globali
                  WHERE
                  dpa_corr_globali.dta_fine IS NULL)
               AND
               dpa_corr_globali.CHA_TIPO_URP = 'R'
               AND
               dpa_corr_globali.ID_AMM = @p_id_amm
               AND
               dpa_corr_globali.DTA_FINE IS NULL
               AND
               dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
               OPEN @ruoli_sup
               while 1 = 1
               begin
                  FETCH @ruoli_sup INTO @s_idg_r_sup
                  if @@FETCH_STATUS <> 0
                  BREAK
--DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || p_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                  INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
               end
               CLOSE @ruoli_sup
            END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
               SET @n_id_gruppo = 0
               SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                  EXCEPT
                  SELECT PERSONORGROUP FROM SECURITY WHERE THING = @p_doc_number) AS TabAl
               IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGCV',@p_codice_atipicita),0) = 0)
                  SET @p_codice_atipicita = @p_codice_atipicita + 'AGCV-'

            END
            COMMIT
         end
      end
      CLOSE @c_idg_security
   END  

--Restituzione codice di atipicit
   IF(@p_codice_atipicita is null)
   begin
      SET @p_codice_atipicita = 'T'
--DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || p_doc_number || ' - ' || p_codice_atipicita);
      update PROFILE set CHA_COD_T_A = @p_codice_atipicita  where DOCNUMBER = @p_doc_number
      SELECT   @error = @@ERROR
      IF (@error = 0)
      begin
         COMMIT
         RETURN
      end
   end


   IF(SUBSTRING(@p_codice_atipicita,LEN(@p_codice_atipicita),LEN(@p_codice_atipicita) -LEN(@p_codice_atipicita)) = '-')
   begin
      SET @p_codice_atipicita = SUBSTRING(@p_codice_atipicita,0,LEN(@p_codice_atipicita) -1)
--DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || p_doc_number || ' - ' || p_codice_atipicita);
      update PROFILE set CHA_COD_T_A = @p_codice_atipicita  where DOCNUMBER = @p_doc_number
      SELECT   @error = @@ERROR
      IF (@error = 0)
      begin
         COMMIT
         RETURN
      end
   end


END
GO
              
----------- FINE -
              
---- VIS_DOC_ANOMALA_INT_DATE.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP RemoveTransModelVisibileToRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='VIS_DOC_ANOMALA_INT_DATE')
DROP PROCEDURE @db_user.VIS_DOC_ANOMALA_INT_DATE
go


create procedure @db_user.VIS_DOC_ANOMALA_INT_DATE 

@p_id_amm INT, 
@p_start_date VARCHAR(4000), 
@p_end_date VARCHAR(4000)  
AS

--DICHIARAZIONI
BEGIN

--CURSORE DOCUMENTI
   DECLARE @s_idg_security INT
   DECLARE @s_ar_security INT
   DECLARE @s_td_security VARCHAR(2)
   DECLARE @s_vn_security VARCHAR(255)
   DECLARE @s_idg_r_sup INT
   DECLARE @s_doc_number INT
   DECLARE @s_id_fascicolo INT
   DECLARE @s_cha_cod_t_a_fasc VARCHAR(1024)
   DECLARE @n_id_gruppo INT
   DECLARE @codice_atipicita VARCHAR(255)
   DECLARE @error INT
   DECLARE @documenti CURSOR
   DECLARE @ruoli_sup CURSOR

   SET @documenti = CURSOR  FOR select profile.docnumber from profile, people where creation_time between CONVERT(DATETIME,@p_start_date) and CONVERT(DATETIME,@p_end_date) AND profile.author = people.system_id AND people.id_amm = @p_id_amm
   OPEN @documenti
   while 1 = 1
   begin
      FETCH @documenti INTO @s_doc_number
      if @@FETCH_STATUS <> 0
      BREAK

--Cursore sulla security per lo specifico documento
      BEGIN 
         DECLARE @c_idg_security CURSOR
         SET @c_idg_security = CURSOR  FOR SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec
         FROM security
         WHERE
         thing = @s_doc_number
         AND
         accessrights > 20
         OPEN @c_idg_security
         while 1 = 1
         begin
            FETCH @c_idg_security INTO @s_idg_security,@s_ar_security,@s_td_security,@s_vn_security
            if @@FETCH_STATUS <> 0
            BREAK

--Gerachia ruolo proprietario documento
            IF(upper(@s_td_security) = 'P')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK
--DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || s_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_doc_number) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGRP',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGRP-'

               END
               COMMIT
            end


--Gerarchia destinatario trasmissione
            IF(upper(@s_td_security) = 'T')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK                   
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_doc_number) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGDT',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGDT-'

               END
               COMMIT
            end


--Fascicolazione documento
            IF(upper(@s_td_security) = 'F')
            BEGIN 
               DECLARE @fascicoli CURSOR
               SET @fascicoli = CURSOR  FOR select system_id from project where system_id in(select id_fascicolo from project where system_id in(select project_id from project_components where link = @s_doc_number)) and cha_tipo_fascicolo = 'P'
               OPEN @fascicoli
               while 1 = 1
               begin
                  FETCH @fascicoli INTO @s_id_fascicolo
                  if @@FETCH_STATUS <> 0
                  BREAK
                  SELECT   @s_cha_cod_t_a_fasc = cha_cod_t_a FROM project WHERE system_id = @s_id_fascicolo
                  IF(@s_cha_cod_t_a_fasc is not null AND upper(@s_cha_cod_t_a_fasc) <> 'T')
                     IF(ISNULL(CHARINDEX('AFCD',@codice_atipicita),0) = 0)
                        SET @codice_atipicita = @codice_atipicita + 'AFCD-'

               end
               CLOSE @fascicoli
            END


--Gerarchia ruolo destinatario di copia visibilit
            IF(upper(@s_td_security) = 'A' AND upper(@s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie

               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_doc_number) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGCV',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGCV-'

               END
               COMMIT
            end
         end
         CLOSE @c_idg_security
      END  

--Restituzione codice di atipicit
      IF(@codice_atipicita is null)
      begin
         SET @codice_atipicita = 'T'
         update PROFILE set CHA_COD_T_A = @codice_atipicita  where DOCNUMBER = @s_doc_number
         COMMIT
         SET @codice_atipicita = null
      end

      IF(SUBSTRING(@codice_atipicita,LEN(@codice_atipicita),LEN(@codice_atipicita) -LEN(@codice_atipicita)) = '-')
      begin
         SET @codice_atipicita = SUBSTRING(@codice_atipicita,0,LEN(@codice_atipicita) -1)

         update PROFILE set CHA_COD_T_A = @codice_atipicita  where DOCNUMBER = @s_doc_number
         COMMIT
         SET @codice_atipicita = null
      end
   end
   CLOSE @documenti

END 
GO              
----------- FINE -
              
---- VIS_FASC_ANOMALA_CUSTOM.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP VIS_FASC_ANOMALA_CUSTOM
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='VIS_FASC_ANOMALA_CUSTOM')
DROP PROCEDURE @db_user.VIS_FASC_ANOMALA_CUSTOM
go


create procedure @db_user.VIS_FASC_ANOMALA_CUSTOM 

@p_id_amm INT, 
@p_queryFasc VARCHAR(4000)  
AS

--DICHIARAZIONI
BEGIN

--CURSORE FASCICOLI
   DECLARE @s_idg_security INT
   DECLARE @s_ar_security INT
   DECLARE @s_td_security VARCHAR(2)
   DECLARE @s_vn_security VARCHAR(255)
   DECLARE @s_idg_r_sup INT
   DECLARE @n_id_gruppo INT
   DECLARE @s_id_fascicolo INT
   DECLARE @codice_atipicita VARCHAR(255)
   DECLARE @error INT
   DECLARE @ruoli_sup CURSOR

   execute(@p_queryFasc)
   while 1 = 1
   begin
      FETCH fascicoli INTO @s_id_fascicolo
      if @@FETCH_STATUS <> 0
      BREAK

--Cursore sulla security per lo specifico fascicolo
      BEGIN 
         DECLARE @c_idg_security CURSOR
         SET @c_idg_security = CURSOR  FOR SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec
         FROM security
         WHERE
         thing = @s_id_fascicolo
         AND
         accessrights > 20
         OPEN @c_idg_security
         while 1 = 1
         begin
            FETCH @c_idg_security INTO @s_idg_security,@s_ar_security,@s_td_security,@s_vn_security
            if @@FETCH_STATUS <> 0
            BREAK

--Gerachia ruolo proprietario del fascicolo
            IF(upper(@s_td_security) = 'P')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_id_fascicolo) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGRP',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGRP-'

               END
               COMMIT
            end

--Gerarchia destinatario trasmissione
            IF(upper(@s_td_security) = 'T')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK                   
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_id_fascicolo) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGDT',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGDT-'

               END
               COMMIT
            end



--Gerarchia ruolo destinatario di copia visibilit
            IF(upper(@s_td_security) = 'A' AND upper(@s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA')
            begin
               BEGIN 
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
                  WHERE
                  dpa_corr_globali.id_uo in(SELECT dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE
                     dpa_corr_globali.dta_fine IS NULL)
                  AND
                  dpa_corr_globali.CHA_TIPO_URP = 'R'
                  AND
                  dpa_corr_globali.ID_AMM = @p_id_amm
                  AND
                  dpa_corr_globali.DTA_FINE IS NULL
                  AND
                  dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = @s_idg_security)
                  OPEN @ruoli_sup
                  while 1 = 1
                  begin
                     FETCH @ruoli_sup INTO @s_idg_r_sup
                     if @@FETCH_STATUS <> 0
                     BREAK
                     INSERT INTO DPA_VIS_ANOMALA(ID_GRUPPO) VALUES(@s_idg_r_sup)
                  end
                  CLOSE @ruoli_sup
               END
--Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
--Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
               BEGIN
                  SET @n_id_gruppo = 0
                  SELECT   @n_id_gruppo = COUNT(*) FROM(SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                     EXCEPT
                     SELECT PERSONORGROUP FROM SECURITY WHERE THING = @s_id_fascicolo) AS TabAl
                  IF(@n_id_gruppo <> 0 AND ISNULL(CHARINDEX('AGCV',@codice_atipicita),0) = 0)
                     SET @codice_atipicita = @codice_atipicita + 'AGCV-'

               END
               COMMIT
            end
         end
         CLOSE @c_idg_security
      END 

--Restituzione codice di atipicit
      IF(@codice_atipicita is null)
      begin
          update PROJECT set CHA_COD_T_A = @codice_atipicita  where SYSTEM_ID = @s_id_fascicolo
         COMMIT
         SET @codice_atipicita = null
      end

      IF(SUBSTRING(@codice_atipicita,LEN(@codice_atipicita),LEN(@codice_atipicita) -LEN(@codice_atipicita)) = '-')
      begin
         SET @codice_atipicita = SUBSTRING(@codice_atipicita,0,LEN(@codice_atipicita) -1)
         update PROJECT set CHA_COD_T_A = @codice_atipicita  where SYSTEM_ID = @s_id_fascicolo
         COMMIT
         SET @codice_atipicita = null
      end
   end
   CLOSE fascicoli

END
GO
              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.17.1')
GO

              
