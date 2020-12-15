-- file sql di update per il CD --
---- CREATE_DPA_LOG_INSTALL.MSSQL.sql  marcatore per ricerca ----
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_LOG_INSTALL]') 
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)
BEGIN
CREATE TABLE [@db_user].[DPA_LOG_INSTALL](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DATA_OPERAZIONE] [date] NOT NULL,
	[COMANDO_RICHIESTO] [nvarchar](max) NOT NULL,
	[VERSIONE_CD] [nvarchar](max) NOT NULL,
	[ESITO_OPERAZIONE] [nvarchar](max) NOT NULL
)
END
GO


              
---- utl_add_column.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. DPA_LOG
--					'NOME COLONNA', --->es. COLONNA_A
--					'TIPO DATO', ---> es. INT 4, VARCHAR 200, ECC.
--					'DEFAULT', --->es. 'VERO, FALSO, 0, ECC. MA SI PU LASCIARE VUOTO CON ''
--					'',
--					'',
--					'RFU' ---> per uso futuro")

-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 11/07/2011
-- Description:	
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_add_column]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_add_column]
GO

CREATE PROCEDURE [@db_user].[utl_add_column]
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
	


DECLARE @istruzione Nvarchar(2000)
DECLARE @insert_log Nvarchar(2000)
DECLARE @data_eseguito Nvarchar(2000)
DECLARE @comando_richiesto Nvarchar(2000)
DECLARE @esito Nvarchar(2000)
	
	
	if not exists(select * from syscolumns where name=@nome_colonna and id in
		(select id from sysobjects where name=@nome_tabella and xtype='U'))
		
	BEGIN
	-- aggiungo la colonna 
	   set @data_eseguito = (select convert (varchar, getdate(), 120))
	   	
	   	IF @val_default is not null
		begin
		SET @val_default = 'CONSTRAINT DF_'+@nome_tabella+'_'+@nome_colonna+' DEFAULT ('''+@val_default+''')'
		end
		else
		begin
		set @val_default = ''
		end
	   
	   SET @istruzione = N'alter table [' + @nome_utente + '].
						[' + @nome_tabella + '] ADD ' + @nome_colonna +' 
						'+ @tipo_dato +' '+ @val_default

       execute sp_executesql @istruzione
       
       set @comando_richiesto = 'Added column ' +@nome_colonna+' on '+@nome_tabella+''
       set @esito = 'Esito positivo'
       
       /*SET @insert_log = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL ([DATA_OPERAZIONE]
							  ,[COMANDO_RICHIESTO]
							  ,[VERSIONE_CD]
							  ,[ESITO_OPERAZIONE])
							  VALUES (
							   '''+@data_eseguito+''',
							   ''Added column ' +@nome_colonna+' on '+@nome_tabella+''',
							   '''+@versioneCD+''',
							   ''esito positivo'')'
							   */
      
      execute @db_user.utl_insert_log @nome_utente, null, @comando_richiesto, @versioneCD, @esito
       
      --exec(@insert_log)
          
	end 	else 	BEGIN	
			   set @data_eseguito = (select convert (varchar, getdate(), 120))
			
              		/*
              		SET @insert_log = N'INSERT INTO '+@nome_utente +'.DPA_LOG_INSTALL ([DATA_OPERAZIONE]
							  ,[COMANDO_RICHIESTO]
							  ,[VERSIONE_CD]
							  ,[ESITO_OPERAZIONE])VALUES (
							   '''+@data_eseguito+''',
							   ''Added column ' +@nome_colonna+' on '+@nome_tabella+''',
							   '''+@versioneCD+''',
							   ''ESITO NEGATIVO - Colonna gi ESISTENTE'')'
							   */
	             set @comando_richiesto = 'Added column ' +@nome_colonna+' on '+@nome_tabella+''
				 set @esito = 'ESITO NEGATIVO - Colonna gi ESISTENTE'
	       
	execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
      	  
		   --exec(@insert_log)
		   end
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	@versioneCD, 
			@nome_utente, 
			@nome_tabella, 
			@nome_colonna, 
			@tipo_dato, 
			@val_default, 
			@condizione_modifica_pregresso, 
			@condizione_check, 
			@RFU
END              
---- utl_add_foreign_key.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_foreign_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_TABELLA_A
--					'NOME TABELLA FOREIGN KEY', --->es. TABELLA_B
--					'NOME COLONNA FOREIGN KEY', --->es. COLONNA_TABELLA_B
--					'CONDIZIONE ADD', ---> LASCIARE VUOTO CON ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 27/07/2011
-- Description:	
-- =============================================
-- NOME DEL FOREIGN KEY VIENE NOMINATO AUTOMATICAMENTE 
-- PRELEVANDO NOME TABELLA_A, TABELLA_B E COLONNA_A (FK_'TABELLA'_'COLONNA'_'TABELLAFK')


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_add_foreign_key]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_add_foreign_key]
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
       execute @db_user.utl_insert_log @nome_utente, null, @comando_richiesto, @versioneCD, @esito
     end
		end
			else    
			begin
			print 'non esiste PK'
			set @esito = 'Esito Negativo - Chiave primaria non esistente' 
    
			set @comando_richiesto = 'Adding Foreign Key ' +@nome_foreign_key+' on '+@nome_tabella_fk+''
			execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
			END
	END


	ELSE
---------------------se  gi esistente il FK		
	BEGIN	
		   print 'esiste gi FK'
		   set @esito = 'ESITO NEGATIVO - Foreign Key GIA esistente o Tabella non esistente'
		   set @comando_richiesto = 'Adding Foreign Key ' +@nome_foreign_key+' on '+@nome_tabella_fk+''
	       execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito

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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
       
       execute @db_user.utl_insert_log null, NULL, @comando_richiesto, @versioneCD, @esito

       
          
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
		   execute @db_user.utl_insert_log NULL,null, @comando_richiesto, @versioneCD, @esito
		   end
	
--	SET NOCOUNT ON;

    -- Insert statements for procedure here
--	SELECT	@versioneCD, 			@nome_utente, 			@nome_tabella, 			@is_index_unique,
			-- @nome_colonna1, 			@nome_colonna2, 			@nome_colonna3, 			@RFU
END
GO

              
---- utl_add_primary_key.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_primary_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_A
--					'NOME PRIMARY KEY', --->es. PK_TABELLA_A
--					'CONDIZIONE ADD', --->es. ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 26/07/2011
-- Description:	
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_add_primary_key]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_add_primary_key]
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
       execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
       
          
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
			   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
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
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_drop_column]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_drop_column]
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
			   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
    END
    
    else
    begin
			   set @comando_richiesto = 'Dropping column ' +@nome_colonna+' from '+@nome_tabella+''
			   set @esito = 'Non  possibile eliminare la colonna non vuota'
			   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
    end
    END

	else
		
		BEGIN
		   
			   set @comando_richiesto = 'Dropping column ' +@nome_colonna+' from '+@nome_tabella+''
			   set @esito = 'Esito Negativo - Colonna non esistente'
			   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
			   
		
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
              

              
---- utl_modify_column.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_modify_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. DPA_LOG
--					'NOME COLONNA', --->es. COLONNA_A
--					'TIPO DATO', ---> es. INT 4, VARCHAR 200, ECC.
--					'DEFAULT', --->es. 'VERO, FALSO, 0, ECC. MA SI PU LASCIARE VUOTO CON ''
--					'CONDIZIONE MODIFY', ES. ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 21/07/2011
-- Description:	
-- =============================================


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_modify_column]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_modify_column]
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
      
			  execute @db_user.utl_insert_log @nome_utente, @comando_richiesto, @versioneCD, @esito
          
	end
	
	else
---------------------se non  esistente la colonna da modificare		
		   begin	
		   
			  set @esito = 'ESITO NEGATIVO - Colonna NON esistente'
		      set @comando_richiesto = 'Modifing column ' +@nome_colonna+' on '+@nome_tabella+''
      
			  execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
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
              
---- utl_rename_column.MSSQL.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_rename_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. @db_user
--					'NOME TABELLA', --->es. DPA_LOG
--					'NOME COLONNA', --->es. COLONNA_A
--					'NOME COLONNA NUOVA', --->es. COLONNA_B
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 21/07/2011
-- Description:	
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[utl_rename_column]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[utl_rename_column]
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

       execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
       
	end
	
	else

		   begin	
	   
		   set @esito = 'Esito negativo - Colonna nuova gi esistente'
		   set @comando_richiesto = 'Renaming column ' +@nome_colonna+' to '+@nome_colonna_nuova+' on '+@nome_tabella+''

		   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
		   end
		   END
	ELSE
		   begin	
	   
		   set @esito = 'Esito negativo - Colonna vecchia non esistente'
		   set @comando_richiesto = 'Renaming column ' +@nome_colonna+' to '+@nome_colonna_nuova+' on '+@nome_tabella+''

		   execute @db_user.utl_insert_log @nome_utente,null, @comando_richiesto, @versioneCD, @esito
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
              
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_A_R_OGG_CUSTOM_DOC.MSSQL.SQL  marcatore per ricerca ----
EXEC @db_user.utl_add_column '3.19.1', '@db_user', 'DPA_A_R_OGG_CUSTOM_DOC', 'DEL_REP', 'NUMBER', NULL, NULL, NULL, NULL
GO
              
----------- FINE -
              
---- ALTER_DPA_ASSOCIAZIONE_TEMPLATES.MSSQL.SQL  marcatore per ricerca ----
EXEC @db_user.utl_add_column '3.19.1', '@db_user', 'DPA_ASSOCIAZIONE_TEMPLATES', 'DTA_ANNULLAMENTO', 'DATETIME', NULL, NULL, NULL, NULL
GO              
----------- FINE -
              
---- ALTER_DPA_CHIAVI_CONFIG_TEMPLATE.MSSQL.SQL  marcatore per ricerca ----
EXECUTE @db_user.UTL_ADD_COLUMN '3.19.1', '@db_user', 'DPA_CHIAVI_CONFIG_TEMPLATE', 'CHA_INFASATO', 'VARCHAR(1)', 'N', NULL, NULL, NULL
GO

if exists (select id 
				from sysobjects 
				where name='DPA_CHIAVI_CONFIG_TEMPLATE' 
				and xtype='U')
-- se esiste la tabella
begin
	if not exists (select id from sysobjects 
					where name='DPA_CHIAVI_CONFIG_TEMPLATE_U01' 
					AND xtype='UQ')
	-- se non esiste il constraint
	begin 
		if not exists (select VAR_CODICE, COUNT(*) from [@db_user].[DPA_CHIAVI_CONFIG_TEMPLATE] 
						group by VAR_CODICE
						having COUNT(*) > 1)
		-- se non esistono dati duplicati
		begin
		ALTER TABLE  @db_user.DPA_CHIAVI_CONFIG_TEMPLATE 
			ADD CONSTRAINT DPA_CHIAVI_CONFIG_TEMPLATE_U01 
			UNIQUE (VAR_CODICE)
		end
	end 
END
GO
              
----------- FINE -
              
---- ALTER_DPA_CORR_GLOBALI.MSSQL.SQL  marcatore per ricerca ----
EXECUTE @db_user.UTL_ADD_COLUMN '3.19.1', '@db_user', 'DPA_CORR_GLOBALI', 'VAR_INSERT_BY_INTEROP', 'CHAR(1)', NULL, NULL, NULL, NULL
GO              
----------- FINE -
              
---- ALTER_DPA_CUSTOM_COMP_FASC.MSSQL.SQL  marcatore per ricerca ----
EXEC @db_user.utl_add_column '3.19.1', '@db_user', 'dpa_ogg_custom_comp_fasc', 'Enabledhistory', 'CHAR(1)', NULL, NULL, NULL, NULL
GO              
----------- FINE -
              
---- ALTER_DPA_CUSTOM_COMP.MSSQL.SQL  marcatore per ricerca ----
EXEC @db_user.utl_add_column '3.19.1', '@db_user', 'dpa_ogg_custom_comp', 'Enabledhistory', 'CHAR(1)', NULL, NULL, NULL, NULL
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
              
---- ALTER_DPA_NOTIFICA.MSSQL.sql  marcatore per ricerca ----
--ALTER TABLE DPA_NOTIFICA ADD COLUMN VERSION_ID Number(10,0) default 0 ;

-- default 0 deve valere  anche per eventuali record gi presenti

EXEC @db_user.utl_add_column '3.19', '@db_user', 'DPA_NOTIFICA', 'VERSION_ID', 'Numeric','0','','',''
GO

UPDATE @db_user.DPA_NOTIFICA 
	SET VERSION_ID=0 
	WHERE VERSION_ID is null
GO
              
----------- FINE -
              
---- ALTER_PEOPLE.MSSQL.SQL  marcatore per ricerca ----
EXECUTE @db_user.UTL_ADD_COLUMN '3.19.1', '@db_user', 'PEOPLE', 'ABILITATO_CENTRO_SERVIZI', 'VARCHAR(1)', NULL, NULL, NULL, NULL
GO              
----------- FINE -
              
---- ALTER_SECURITY.MSSQL.sql  marcatore per ricerca ----
if not exists (select * from information_schema.columns 
				where table_name = 'SECURITY' and COLUMN_NAME = 'TS_INSERIMENTO' ) 
BEGIN
	alter table @db_user.SECURITY add TS_INSERIMENTO date default getdate()
END
GO
              
----------- FINE -
              
---- CREATE_DPA_REGISTRI_REPERTORIO.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_REGISTRI_REPERTORIO]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE @db_user.DPA_REGISTRI_REPERTORIO  
        (TipologyId            integer      Not Null,
		  CounterId             integer      Not Null,
		  CounterState          VarChar(1)  Not Null,
		  SettingsType          Varchar(1) ,
		  RegistryId            integer,
		  RfId                  integer,
		  RoleRespId            integer,
		  PrinterRoleRespId     integer,
		  PrinterUserRespId     integer,
		  PrintFreq             Varchar(2) Not Null,
		  TipologyKind          Varchar(2) Not Null,
		  DtaStart              Datetime,
		  DtaFinish             Datetime,
		  DtaNextAutomaticPrint Datetime,
		  DtaLastPrint          Datetime,
		  LastPrintedNumber     integer,
		  RespRights    Varchar(2)
			) 
end
GO

              
----------- FINE -
              
---- CREATE_DPA_STAMPA_REPERTORI.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_STAMPA_REPERTORI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE @db_user.DPA_STAMPA_REPERTORI
  ( SYSTEM_ID     integer,
    ID_REPERTORIO integer,
    NUM_REP_START integer,
    NUM_REP_END   integer,
    NUM_ANNO      integer,
    DOCNUMBER     integer,
    DTA_STAMPA DATE,
    RegistryId Numeric
  )
  
end
GO

              
----------- FINE -
              
---- CREATE_INDEX_DPA_FORMATI_DOCUMENTO.MSSQL.sql  marcatore per ricerca ----
BEGIN
execute @db_user.utl_add_index '3.19.1', '@db_user', 'DPA_FORMATI_DOCUMENTO', '0', 'ID_AMMINISTRAZIONE', NULL, NULL, NULL, NULL, NULL, NULL, NULL
END
GO              
----------- FINE -
              
---- CREATE_PUBBLICAZIONI_DOCUMENTI.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[PUBBLICAZIONI_DOCUMENTI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
BEGIN
CREATE TABLE @db_user.PUBBLICAZIONI_DOCUMENTI  
        (SYSTEM_ID            integer      Not Null,
		  ID_TIPO_DOCUMENTO             integer      Not Null,
		  ID_PROFILE          integer  Not Null,
		  ID_USER          integer ,
		  RegistryId            integer,
		  ID_RUOLO                  INTEGER,
		  DATA_DOC_PUBBLICATO            DATE,
		  DATA_PUBBLICAZIONE_DOCUMENTO     DATE,
		  ESITO_PUBBLICAZIONE     VARCHAR(1),
		  ERRORE_PUBBLICAZIONE    Varchar(255)) 
end
GO

              
----------- FINE -
              
 
-------------------cartella  TRIGGER -------------------
              
---- ModCountDocInMngrTbl.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (SELECT * FROM sys.triggers
WHERE name = 'ModCountDocInMngrTbl')
DROP TRIGGER @db_user.ModCountDocInMngrTbl
GO

create TRIGGER ModCountDocInMngrTbl 
ON @db_user.DPA_OGGETTI_CUSTOM 
AFTER  Update AS
/******************************************************************************

AUTHOR:    Samuele Furnari

NAME:      ModCountInMngrTbl

TRADOTTO: Gabriele Serpi

PURPOSE:   Ogni volta che viene modificato il flag repertorio o il valore 
che indica la tipologia di repertorio, bisogna agire sull'anagrafica

******************************************************************************/

-- Eliminazione dei riferimenti del repertorio dall'anagrafica
   DECLARE @NEW_SYSTEM_ID VARCHAR(255)
   DECLARE @NEW_REPERTORIO VARCHAR(255)
   DECLARE @NEW_CHA_TIPO_TAR VARCHAR(255)
   
   DECLARE @OLD_SYSTEM_ID VARCHAR(255)
   DECLARE @OLD_REPERTORIO VARCHAR(255)
   DECLARE @OLD_CHA_TIPO_TAR VARCHAR(255)
   
   DECLARE @Cursor_for_new CURSOR
   SET @Cursor_for_new = CURSOR  FOR SELECT i.system_id, i.repertorio, i.cha_tipo_tar , d.system_id, d.repertorio, d.cha_tipo_tar 
										FROM inserted i, deleted d where i.system_id = d.system_id
   OPEN @Cursor_for_new
   FETCH NEXT FROM @Cursor_for_new INTO @NEW_SYSTEM_ID,@NEW_REPERTORIO,@NEW_CHA_TIPO_TAR,
										@OLD_SYSTEM_ID,@OLD_REPERTORIO,@OLD_CHA_TIPO_TAR	
   WHILE @@FETCH_STATUS = 0
   begin
      DECLARE @idTipologia INT
      IF (@NEW_REPERTORIO != @OLD_REPERTORIO Or @NEW_CHA_TIPO_TAR != @OLD_CHA_TIPO_TAR)
      begin
         EXECUTE @db_user.DeleteRegistroRepertorio @NEW_SYSTEM_ID
         Select   @idTipologia = ta.system_id
         From dpa_tipo_atto ta
         Inner Join dpa_ogg_custom_comp occ
         On ta.system_id = occ.id_template
         Where occ.id_ogg_custom = @NEW_SYSTEM_ID

-- Se  stato cambiato lo stato del flag repertorio, viene ed  stato passato
-- ad 1, viene eseguito l'inserimento di un riferimento nell'anagrafica
         If @NEW_REPERTORIO = '1'
            EXECUTE @db_user.InsertRegistroRepertorio @idTipologia,@NEW_SYSTEM_ID,@NEW_CHA_TIPO_TAR,'D'
      end
      FETCH NEXT FROM @Cursor_for_new INTO @NEW_SYSTEM_ID,@NEW_REPERTORIO,@NEW_CHA_TIPO_TAR
   end
   CLOSE @Cursor_for_new
GO

              
----------- FINE -
              
---- RemoveRegFromRepertoriTable.MSSQL.SQL  marcatore per ricerca ----
--- Controllo l'esistenza e l'eliminazione del Trigger RemoveRegFromRepertoriTable   
IF EXISTS (SELECT * FROM sys.triggers
WHERE name = 'RemoveRegFromRepertoriTable')
DROP TRIGGER @db_user.RemoveRegFromRepertoriTable
GO

create trigger @db_user.RemoveRegFromRepertoriTable 
on dpa_el_registri 
after delete AS
/******************************************************************************
AUTHOR:    Samuele Furnari
NAME:      RemoveRegFromRepertoriTable
PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
che viene eliminato un record dalla tabella dei registri / rf.

******************************************************************************/
   DECLARE @OLD_SYSTEM_ID VARCHAR(255)
   DECLARE @Cursor_For_OLD CURSOR
   SET @Cursor_For_OLD = CURSOR  FOR SELECT system_id FROM deleted
   OPEN @Cursor_For_OLD
   FETCH NEXT FROM @Cursor_For_OLD INTO @OLD_SYSTEM_ID
   WHILE @@FETCH_STATUS = 0
   begin
      Delete From Dpa_Registri_Repertorio Where RegistryId = @OLD_SYSTEM_ID Or RfId = @OLD_SYSTEM_ID
      FETCH NEXT FROM @Cursor_For_OLD INTO @OLD_SYSTEM_ID
   end
   CLOSE @Cursor_For_OLD
GO

              
----------- FINE -
              
---- UpdateRepertoriTable.MSSQL.SQL  marcatore per ricerca ----
--- Controllo l'esistenza e l'eliminazione del Trigger UpdateRepertoriTable   
IF EXISTS (SELECT * FROM sys.triggers
WHERE name = 'UpdateRepertoriTable')
DROP TRIGGER @db_user.UpdateRepertoriTable
GO

Create Trigger @db_user.UpdateRepertoriTable On @db_user.Dpa_El_Registri After
Insert AS
/******************************************************************************
AUTHOR:    Samuele Furnari
NAME:      UpdateRepertoriTable
TRANSLATED: Gabriele Serpi
PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
che viene aggiunto un registro o un RF e serve per aggiungere ad
ogni contatore di repertorio di tipo AOO / RF, un riferimento
al nuovo registro / RF
******************************************************************************/
-- Cursore per scorrere tutti i repertori di RF
   DECLARE @NEW_CHA_RF VARCHAR(255)
   DECLARE @NEW_SYSTEM_ID VARCHAR(255)
   DECLARE @Cursor_For_NEW CURSOR
   SET @Cursor_For_NEW = CURSOR  FOR SELECT Cha_Rf, System_Id FROM inserted
   OPEN @Cursor_For_NEW
   FETCH NEXT FROM @Cursor_For_NEW INTO @NEW_CHA_RF,@NEW_SYSTEM_ID
   WHILE @@FETCH_STATUS = 0
   begin
      DECLARE @error INT
      Begin
         DECLARE @Mycha_Tipo_Tar VARCHAR(4000)
         DECLARE @RepertoriCursor CURSOR
-- Id del repertorio
         DECLARE @RepId INT
-- Tipo di impostazioni scelte per lo specifico repertorio
         DECLARE @SettType VARCHAR(1)
-- Id del registro e dell'RF
         DECLARE @Registry INT
         DECLARE @Rf       INT
-- Sigla identificativa del tipo di repertorio da modificare (A o R)
         DECLARE @RepType VARCHAR(1)
-- Inizializzazione del cursore per scorrere i repertori a seconda
-- che si stia inserendo un RF o un Registro
         If @NEW_CHA_RF = '1'
            SET @RepType     = 'R'
      Else
         SET @RepType = 'A'
 
         Begin
            SET @Mycha_Tipo_Tar = @RepType
            SET @RepertoriCursor = CURSOR  FOR(Select Oc.System_Id
               From @db_user.Dpa_Oggetti_Custom Oc
               Where Repertorio     = '1'
               And Cha_Tipo_Tar     = @Mycha_Tipo_Tar -- 'R' o 'A'
               And Id_Tipo_Oggetto In(Select System_Id
                  From @db_user.Dpa_Tipo_Oggetto
                  Where Lower(Descrizione) = 'contatore'))
            OPEN @RepertoriCursor
            while 1 = 1
            begin
               Fetch @RepertoriCursor Into @RepId
               if @@FETCH_STATUS <> 0
               BREAK
-- Se si sta inserendo un registro viene inizializzato
-- il parametro registry altrimenti viene valorizzato il
-- parametro rf
               If @NEW_CHA_RF = '0'
               Begin
                  SET @Registry = @NEW_SYSTEM_ID
                  SET @Rf       = Null
               End
            Else
               Begin
                  SET @Registry = Null
                  SET @Rf       = @NEW_SYSTEM_ID
               End
 
-- Selezione delle impostazioni relative al contatore in esame
-- (viene prelevata la prima istanza in quanto una qualsiasi istanza
--  sufficiente per determinare come procedere
               Begin
                  Select  TOP 1 @SettType = SettingsType
                  From @db_user.Dpa_Registri_Repertorio
                  Where CounterId = @RepId
                  SELECT   @error = @@ERROR
                  IF (@error <> 0)
                     SET @SettType = ''
               End
               Begin
                  If @SettType = 'G'
                  Begin
-- Se il tipo di impostazione  G, viene inserita nell'anagrafica una riga uguale
-- alla prima impostazione del repertorio con la data di ultima stampa impostata a null
-- e con l'ultimo numero stampato impostato a 0
 Insert
                     Into @db_user.Dpa_Registri_Repertorio(Tipologyid,
Counterid,
Counterstate,
Settingstype,
Registryid,
Rfid,
Rolerespid,
Printerrolerespid,
Printeruserrespid,
Printfreq,
Tipologykind,
Dtastart,
Dtafinish,
Dtanextautomaticprint,
Dtalastprint,
Lastprintednumber,
Resprights)(Select TOP 1 Tipologyid,
CounterId,
CounterState,
SettingsType,
@Registry,
@Rf,
RolerespId,
PrinterRoleRespId,
PrinterUserRespId,
PrintFreq,
TipologyKind,
DtaStart,
DtaFinish,
DtaNextAutomaticPrint,
Null,
0,
RespRights
                        From @db_user.Dpa_Registri_Repertorio
                        Where CounterId = @RepId)
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                        SET @SettType = ''
                  End
               Else
                  Begin
                     If @SettType = 'S'
                     Begin
-- Altrimenti le impostazioni sono singole per ogni repertorio.
-- In questo caso, viene inserita una riga uguale alla prima istanta
-- di configurazione relativa al repertorio ad esclusione di:
--    - stato che viene impostato ad Aperto
--    - rfid / registry id che vengono impostati a seconda del registro inserito
--    - ultimo numero stampato che viene impostato a 0
--    - date che vengono impostate tutte a null
--    - respondabili che vengono impostati a null
--    - frequenza di stampa automatica che viene impostata ad N
 Insert
                        Into @db_user.Dpa_Registri_Repertorio(Tipologyid,
Counterid,
Counterstate,
Settingstype,
Registryid,
Rfid,
Rolerespid,
Printerrolerespid,
Printeruserrespid,
Printfreq,
Tipologykind,
Dtastart,
Dtafinish,
Dtanextautomaticprint,
Dtalastprint,
Lastprintednumber,
Resprights)(Select TOP 1 Tipologyid,
Counterid,
Counterstate,
Settingstype,
@Registry,
@Rf,
Null,
Null,
Null,
'N',
Tipologykind,
Null,
Null,
Null,
Null,
0,
Resprights
                           From @db_user.Dpa_Registri_Repertorio
                           Where Counterid = @RepId)
                        SELECT   @error = @@ERROR
                        IF (@error <> 0)
                           SET @SettType = ''
                     End

                  End

               End
            end
         End
         Close @RepertoriCursor
      End
      FETCH NEXT FROM @Cursor_For_NEW INTO @NEW_CHA_RF,@NEW_SYSTEM_ID
   end
   CLOSE @Cursor_For_NEW
GO

              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- INS_DPA_ANAGRAFICA_FUNZIONI.MSSQL.SQL  marcatore per ricerca ----
--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.dpa_anagrafica_funzioni 
WHERE VAR_DESC_FUNZIONE ='GEST_REGISTRO_REPERTORIO')
BEGIN
Insert into @db_user.dpa_anagrafica_funzioni(COD_FUNZIONE,
		VAR_DESC_FUNZIONE, 	CHA_TIPO_FUNZ, 	DISABLED)
	Values 
	('GEST_REGISTRO_REPERTORIO', 
	'Permette di abilitare/disabilitare la la voce di menu registro di repertorio', 
	Null, 'N')
END
GO              
----------- FINE -
              
---- INS_DPA_CHIAVI_CONFIG_TEMPLATE.MSSQL.SQL  marcatore per ricerca ----
--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIG_TEMPLATE WHERE var_codice ='VIS_SEGNATURA_REPERTORI')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE
   (VAR_CODICE
   , VAR_DESCRIZIONE, VAR_VALORE
   , CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE
   , CHA_INFASATO)
 Values
   ('VIS_SEGNATURA_REPERTORI'
   , 'Visualizza in to do list la segnatura di repertorio (0 = disattivato; 1 = attivato)', '1'
   , 'B'            , '1'           , '1'           
   , 'N')
END
GO

--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIG_TEMPLATE WHERE var_codice ='CHECK_MITT_INTEROPERANTE')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE
   (VAR_CODICE
   , VAR_DESCRIZIONE, VAR_VALORE
   , CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE
   , CHA_INFASATO)
 Values
   ('CHECK_MITT_INTEROPERANTE'
   , 'Chiave utilizzata per abilitare un controllo qualora un corrispondente esterno scriva per la prima volta alla casella PEC di AOO/RF', '1'
   , 'N'            , '1'           , '1'           
   , 'N')
END
GO

--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIG_TEMPLATE WHERE var_codice ='CHECK_MAILBOX_INTEROPERANTE')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE
   (VAR_CODICE
   , VAR_DESCRIZIONE, VAR_VALORE
   , CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE
   , CHA_INFASATO)
 Values
   ('CHECK_MAILBOX_INTEROPERANTE'
   , 'Chiave utilizzata per abilitare un controllo qualora un corrispondente esterno scriva alla casella PEC di AOO/RF con un indirizzo di posta elettronica identico ad un altro corrispondente censito.', '0'
   , 'N'            , '1'           , '1'           
   , 'N')
END
GO              
----------- FINE -
              
---- INS_DPA_CHIAVI_CONFIGURAZIONE.MSSQL.SQL  marcatore per ricerca ----
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
			WHERE VAR_CODICE ='FE_AUTOMATIC_SCAN')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale)
	values 
	(0,
	'FE_AUTOMATIC_SCAN',
	'Pu assumere i valori 0 o 1 e serve per abilitare la partenza automatica del driver Twain durante l''acquisizione di un documento',
	'0',
	'F',
	'1',
	'1',
	'1')
END
GO


IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
			WHERE VAR_CODICE ='FE_NEW_RUBRICA_VELOCE')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale)
	values 
	(0,
	'FE_NEW_RUBRICA_VELOCE',
	'Utilizzata per abilitare la rubrica ajax sul campo descrizione del corrispondente',
	'0',
	'F',
	'1',
	'1',
	'1')
END
GO

--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
			WHERE VAR_CODICE ='BE_CHECK_INTEROP_DTD')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(0,
	'BE_CHECK_INTEROP_DTD',
	'Verifica DTD online',
	'0',
	'B',
	'1',
	'1',
	'1',
	null)
END
GO

IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE ='GESTIONE_REPERTORI')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(0,
	'GESTIONE_REPERTORI',
	'La chiave abilita o meno la gestione dei repertori',
	'0',
	'B',
	'1',
	'1',
	'1',
	null)
END
GO

--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE ='FE_COPIA_VISIBILITA')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(0,
    'FE_COPIA_VISIBILITA',
    'Abilita il pulsante per la copia della visibilit',
    '0',
    'F',
    '1',
    '1',
    '1',
    null)
END
GO

--Creazione chiave di configurazione 
IF NOT EXISTS (SELECT * FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE ='FE_GEST_RUOLI_AVANZATA')
BEGIN
Insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(0,
    'FE_GEST_RUOLI_AVANZATA',
    'Abilita la gestione avanzata ruoli che consente di modificare il tipo ruolo e di cambiare il tipo ruolo',
    '0',
    'F',
    '1',
    '1',
    '1',
    null)
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
              
---- UPDATE_DPA_OGG_CUSTOM_COMP_FASC.MSSQL.SQL  marcatore per ricerca ----
--- update colonna EnabledHistory
if exists(select * from syscolumns where name=UPPER ('Enabledhistory') and id in
		(select id from sysobjects where name=UPPER ('dpa_ogg_custom_comp_fasc') and xtype='U'))
		begin
		Update @db_user.dpa_ogg_custom_comp
		Set Enabledhistory = '0'
		end
GO              
----------- FINE -
              
---- UPDATE_DPA_OGG_CUSTOM_COMP.MSSQL.SQL  marcatore per ricerca ----
--- update colonna EnabledHistory
if exists(select * from syscolumns where name=UPPER ('Enabledhistory') and id in
		(select id from sysobjects where name=UPPER ('dpa_ogg_custom_comp') and xtype='U'))
		begin
		Update @db_user.dpa_ogg_custom_comp
		Set Enabledhistory = '0'
		end
GO              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- getContatoreDoc.MSSQL.SQL  marcatore per ricerca ----
IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getContatoreDoc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].getContatoreDoc 
go

CREATE  FUNCTION @db_user.getContatoreDoc (@docNumber INT, @tipoContatore CHAR)
RETURNS VARCHAR AS 
BEGIN

declare @valoreContatore VARCHAR(255)
declare @annoContatore	VARCHAR(255)
declare @codiceRegRf	VARCHAR(255)
declare @repertorio		numeric
declare @risultato		VARCHAR(255)

select @valoreContatore = valore_oggetto_db
		, @annoContatore = anno
		, @repertorio = repertorio
from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where dpa_associazione_templates.doc_number = @docNumber
and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
  (dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
and dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'


IF (@repertorio = 1) 
BEGIN
	set @risultato = '#CONTATORE_DI_REPERTORIO#'
	RETURN @risultato
END

IF(@tipoContatore <> 'T') 
BEGIN
	select @codiceRegRf = dpa_el_registri.var_codice
	from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
	where dpa_associazione_templates.doc_number = @docNumber
	and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
	and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
	and
  (dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
and dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
	and dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
END


if @codiceRegRf is null
BEGIN
	set @risultato =  isnull(@valoreContatore,' ') +'-'+ isnull(@annoContatore,' ') 
end else  BEGIN 
	set @risultato =  isnull(@codiceRegRf,' ') +'-'+ isnull(@annoContatore,' ') +'-'+ isnull(@valoreContatore,' ')
end 

RETURN @risultato
end 
GO

              
----------- FINE -
              
---- getValCampoProfDoc.MSSQL.SQL  marcatore per ricerca ----
IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getValCampoProfDoc]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].getValCampoProfDoc 
go

CREATE FUNCTION [@db_user].[getValCampoProfDoc](@DocNumber INT, @CustomObjectId INT)
RETURNS VARCHAR(400) AS 
BEGIN

/*
Si distinguono cinque casi:
1. @tipoOggetto = 'Corrispondente'
2. @tipoOggetto = 'CasellaDiSelezione'
3. @tipoOggetto = 'Contatore' 
4. @tipoOggetto = 'Contatore'
		con campo dpa_oggetti_custom.repertorio = 1
		e campo dpa_associazione_templatesid_oggetto not null 
5. nessuno dei precedenti		
*/

declare @result		VARCHAR(255)
declare @tipoOggetto varchar(255)
declare @tipoCont	varchar(1)
declare @repert		numeric
declare @tipologiaDoc numeric

select @tipoOggetto = b.descrizione
	, @tipoCont = cha_tipo_Tar
	, @repert = a.repertorio
from  dpa_oggetti_custom a, dpa_tipo_oggetto b 
where  a.system_id = @CustomObjectId
and a.id_tipo_oggetto = b.system_id

if (@tipoOggetto = 'Corrispondente') 
BEGIN
      select @result  = cg.var_cod_rubrica + ' - ' + cg.var_DESC_CORR 
      from dpa_CORR_globali cg
		where cg.SYSTEM_ID = (
			select valore_oggetto_db from dpa_associazione_templates 
			where id_oggetto = @CustomObjectId and doc_number = @DocNumber)
return 	@result  
end -- end if(@tipoOggetto = 'Corrispondente') 
    
--Casella di selezione (Per la casella di selezione serve un caso particolare perche i valori sono multipli)
if(@tipoOggetto = 'CasellaDiSelezione')     BEGIN
       declare @item varchar(255)
       declare curCasellaDiSelezione CURSOR LOCAL FOR 
			select  -- @result = 
			valore_oggetto_db from dpa_associazione_templates 
				where id_oggetto = @CustomObjectId 
					and doc_number = @DocNumber
					and valore_oggetto_db is not null 

       OPEN curCasellaDiSelezione
       FETCH NEXT FROM curCasellaDiSelezione INTO @item
       WHILE @@FETCH_STATUS = 0 -- EXIT WHEN curCasellaDiSelezione%NOTFOUND;
       BEGIN
                IF(@result IS NOT NULL) 
                BEGIN
					SET @result = @result + '; ' + @item 
                END ELSE BEGIN
					SET @result = @result + @item
                END 
            FETCH NEXT FROM curCasellaDiSelezione INTO @item
        END
        CLOSE curCasellaDiSelezione
RETURN @result 
end -- end if(@tipoOggetto = 'CasellaDiSelezione')  

IF (@tipoOggetto = 'Contatore' )
begin

-- restituisce 1 se il documento DocNumber  associato alla tipologia di documento 
--	contenente il contatore di repertorio con id = CustomObjectId
SELECT @tipologiaDoc = case when id_oggetto is not null then 1 else 0 end 
from dpa_associazione_templates
where doc_number=@DocNumber and id_oggetto=@CustomObjectId


			IF @repert = 1 And @tipologiaDoc = 1
			BEGIN
			RETURN '#CONTATORE_DI_REPERTORIO#'
			end elsE BEGIN 
				select @result = @db_user.getContatoreDoc(@DocNumber,@tipoCont)  
				from dpa_associazione_templates 
				where id_oggetto = @CustomObjectId and doc_number = @DocNumber

			RETURN @result
			end
end -- end IF (@tipoOggetto = 'Contatore' )


--Tutti gli altri casi
select @result = valore_oggetto_db
from dpa_associazione_templates
where id_oggetto = @CustomObjectId and doc_number = @DocNumber
RETURN @result

End
GO

              
----------- FINE -
              
---- HAS_CHILDREN.MSSQL.SQL  marcatore per ricerca ----

IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[HAS_CHILDREN]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].HAS_CHILDREN 
go

CREATE FUNCTION [@db_user].HAS_CHILDREN (@corrId integer,@tipoURP char)

RETURNS integer AS 
BEGIN

declare @risultato	numeric
DECLARE @rtnUO1		numeric
declare @rtnRUO numeric
declare @rtnUO2 numeric

select @rtnUO1  = case when count(*) > 0 then 1 else 0 end 
from dpa_corr_globali b 
where b.CHA_TIPO_URP='U' 
and b.CHA_TIPO_IE='I' 
and b.id_parent = @corrId

--oppure

select @rtnUO2 = case when count(*) > 0 then 1 else 0 end 
from dpa_corr_globali b 
where b.CHA_TIPO_URP='U' 
and cha_tipo_ie='I' 
and b.id_uo = @corrId 

--oppure

select @rtnRUO = case when count(*) > 0 then 1 else 0 end 
from dpa_corr_globali b 
where (CHA_TIPO_URP='R')  and cha_tipo_ie='I' 
and exists (select * from peoplegroups
			where groups_system_id in (select id_gruppo from dpa_corr_globali where system_id=@corrId ) 
			and dta_fine is null
		)



set @risultato = @rtnUO1 + @rtnUO2 + @rtnRUO

-- come se fosse un booleano
if(@risultato>0) begin
	set @risultato = 1
end
else begin
	set @risultato = 0
end 

return @risultato


end 
GO


              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- 0.InsertDataInHistoryProf.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CalculateAtipDelRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='InsertDataInHistoryProf')
DROP PROCEDURE @db_user.InsertDataInHistoryProf
go


create Procedure [@db_user].[InsertDataInHistoryProf] @objType VARCHAR(4000),
@Idtemplate  VARCHAR(4000),
@idDocOrFasc VARCHAR(4000),
@Idoggcustom VARCHAR(4000),
@Idpeople VARCHAR(4000),
@Idruoloinuo VARCHAR(4000),
@Descmodifica VARCHAR(4000) as
--@Returnvalue INT OUTPUT  AS
Begin

/*AUTHOR:   Samuele Furnari
NAME:     InsertDataInHistoryProf
PURPOSE:  Store per l'inserimento di una voce nello storico dei campi 
profilati di documenti / fascicoli. 

******************************************************************************/

--   Begin
      DECLARE @enHis CHAR = ''

-- Verifica del flag di attivazione storico per il campo

      if @objType = 'D'
      begin
         Select   @enHis = Enabledhistory From dpa_ogg_custom_comp
         Where id_ogg_custom = @Idoggcustom And id_template = @Idtemplate
      end
   
   -- commentata by Fiordi perch enHis serve solo come filtro per i documenti non per i fascicoli
   --else 
   --   Select   @enHis = Enabledhistory From dpa_ogg_custom_comp_fasc
   --   Where id_ogg_custom = @Idoggcustom And id_template = @Idtemplate
	
-- Se  attiva la storicizzazione del campo, viene inserita una riga nello storico

-- Se l'oggetto da storicizzare  un documento, viene inserita una riga 
-- nello storico dei documenti, altrimenti viene inserita in quella dei fascicoli

         If (@objType = 'D' and @enHis = '1')
         begin
            Insert Into [@db_user].DPA_PROFIL_STO(Id_Template, Dta_Modifica, Id_Profile, Id_Ogg_Custom, Id_People, Id_Ruolo_In_Uo, Var_Desc_Modifica)
			Values(@Idtemplate, GetDate(), @idDocOrFasc, @Idoggcustom, @Idpeople, @Idruoloinuo, @Descmodifica)
		end Else   begin
         Insert Into [@db_user].Dpa_Profil_Fasc_Sto(Id_Template, Dta_Modifica, Id_Project, Id_Ogg_Custom, Id_People, Id_Ruolo_In_Uo, Var_Desc_Modifica)
		Values(@Idtemplate, GetDate(), @idDocOrFasc, @Idoggcustom, @Idpeople, @Idruoloinuo, @Descmodifica)
	      End

   --SET @Returnvalue = 0

--	End
end

GO              
----------- FINE -
              
---- 0.InsertRegistroRepertorio.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CalculateAtipDelRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='InsertRegistroRepertorio')
DROP PROCEDURE @db_user.InsertRegistroRepertorio
go

create Procedure @db_user.InsertRegistroRepertorio 
-- Id della tipologia
@tipologyId  INT,
-- Id del contatore
@counterId INT,
-- Tipo di contatore
@counterType CHAR(2000),
-- Categoria della tipologia documentale cui appartirene il
-- contatore da inserire
@tipologyKind CHAR(2000)  AS
Begin
/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     InsertRegistroRepertorio

PURPOSE:  Store per l'inserimento di un registro di repertorio nell'anagrafica            

******************************************************************************/

-- Cursore sui registri
   DECLARE @cursorRegistries CURSOR

-- Cursore sugli RF
   DECLARE @cursorRf CURSOR  

-- Id del registro / Rf
   DECLARE @registryRfId  INT
   IF(@counterType) = 'T'
-- Se il contatore  di tipologia, viene inserita una sola riga nell'anagrafica
   Insert Into @db_user.dpa_registri_repertorio(TipologyId,
CounterId,
CounterState,
SettingsType,
RegistryId,
RfId,
RoleRespId,
PrinterRoleRespId,
PrinterUserRespId,
PrintFreq,
TipologyKind,
DtaStart,
DtaFinish,
DtaNextAutomaticPrint,
DtaLastPrint,
LastPrintedNumber,
Resprights)
Values(@tipologyId,
@counterId,
'O',
'G',
null,
null,
null,
null,
null,
'N',
@tipologyKind,
null,
null,
null,
null,
0,
'R')


   ELSE IF(@counterType) = 'A'
-- Se  di AOO vengono inserite tante voci quanti sono i registri
   Begin
      SET @cursorRegistries = CURSOR  FOR(Select system_id
         From @db_user.dpa_el_registri
         Where cha_rf Is Null Or cha_rf = '0')
      Open @cursorRegistries
      while 1 = 1
      begin
         Fetch @cursorRegistries INTO @registryRfId
         if @@FETCH_STATUS <> 0
         BREAK
         Insert Into @db_user.dpa_registri_repertorio(TipologyId,
CounterId,
CounterState,
SettingsType,
RegistryId,
RfId,
RoleRespId,
PrinterRoleRespId,
PrinterUserRespId,
PrintFreq,
TipologyKind,
DtaStart,
DtaFinish,
DtaNextAutomaticPrint,
DtaLastPrint,
LastPrintedNumber,
resprights)
Values(@tipologyId,
@counterId,
'O',
'G',
@registryRfId,
null,
null,
null,
null,
'N',
@tipologyKind,
null,
null,
null,
null,
0,
'R')
      end
      Close @cursorRegistries
   End
   ELSE IF(@counterType) = 'R'
-- Se  di RF vengono inserite tante voci quanti sono gli RF
   Begin
      SET @cursorRf = CURSOR  FOR(Select system_id
         From @db_user.dpa_el_registri
         Where cha_rf = '1')
      OPEN @cursorRf
      while 1 = 1
      begin
         FETCH @cursorRf INTO @registryRfId
         if @@FETCH_STATUS <> 0
         BREAK
         Insert Into @db_user.dpa_registri_repertorio(TipologyId,
CounterId,
CounterState,
SettingsType,
RegistryId,
RfId,
RoleRespId,
PrinterRoleRespId,
PrinterUserRespId,
PrintFreq,
TipologyKind,
DtaStart,
DtaFinish,
DtaNextAutomaticPrint,
DtaLastPrint,
LastPrintedNumber,
resprights)
Values(@tipologyId,
@counterId,
'O',
'G',
null,
@registryRfId,
null,
null,
null,
'N',
@tipologyKind,
null,
null,
null,
null,
0,
'R')
      end
      Close @cursorRf
   End
End   

GO              
----------- FINE -
              
---- 0.SaveCounterSettings.MSSQL.sql  marcatore per ricerca ----
-- Controllo esistenza SP CalculateAtipDelRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='SaveCounterSettings')
DROP PROCEDURE @db_user.SaveCounterSettings
go


create PROCEDURE @db_user.SaveCounterSettings 
-- Id del contatore
@countId   INT,
-- Tipo di impostazioni specificato per un contatore (G o S)
@settingsType VARCHAR(4000),
-- Id del ruolo stampatore
@roleIdGroup  INT,
-- Id dell'utente stampatore
@userIdPeople  INT,
-- Id del ruolo responsabile
@roleRespIdGroup INT,
-- Frequenza di stampa
@printFrequency VARCHAR(4000),
-- Data di partenza del servizio di stampa automatica
@dateAutomaticPrintStart DATETIME,
-- Data di stop del servizio di stampa automatica
@dateAutomaticPrintFinish  DATETIME,
-- Data prevista per la prossima stampa automatica
@dateNextAutomaticPrint  DATETIME,
-- Id del registro cui si riferiscono le impostazioni da salvare
@reg INT,
-- Id dell'RF cui si riferscono le impostazioni da salvare
@rf INT,
-- Sigla identificativa della tipologia in cui  definito il contatore (D, F)
@tipology VARCHAR(4000),
-- Stato del contatore di repertorio (O, C)
@state VARCHAR(4000),
-- Diritti da concedere al responsabile (R o RW)
@rights VARCHAR(4000),
-- Valore di ritorno
@returnValue INT OUTPUT  AS
BEGIN
/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     SaveCounterSettings

PURPOSE:  Store per il salvataggio delle modifiche apportate alle impostazioni
di stampa per un determinato contatore di repertorio

******************************************************************************/

-- Tipologia di impostazioni impostata per il contatore


-- Se il tipo di impostazione scelta  G, vengono aggiornate le properiet per tutte le istanze
-- del contatore counterId
   DECLARE @actualSettingsType CHAR
   IF @settingsType = 'G'
   Begin
      update @db_user.dpa_registri_repertorio set @settingsType = 'G',PrinterRoleRespId = @roleIdGroup,PrinterUserRespId = @userIdPeople,
      RoleRespId = @roleRespIdGroup,PrintFreq = @printFrequency,
      DtaStart = @dateAutomaticPrintStart,DtaFinish = @dateAutomaticPrintFinish,
      DtaNextAutomaticPrint = @dateNextAutomaticPrint,CounterState = @state,
      Resprights = @rights  Where CounterId = @countId And TipologyKind = @tipology
   End
Else
-- Altrimenti, se prima il tipo di impostazioni era G, vengono aggiornate tutte
-- le istanze del contatore ad S ed in seguito vengono salvare le informazioni 
-- per la specifica istanza specificata
-- da registro / RF specificato
   Begin
-- Valorizzazione corretta per l'id gruppo del ruolo responsabile
      DECLARE @decodedRoleRespIdGroup VARCHAR(100)
-- Valorizzazione corretta per l'id gruppo dello stampatore
      DECLARE @decodedRoleIdGroup VARCHAR(100)
-- Valorizzazione corretta per l'id utente dello stampatore
  DECLARE @decodedUserIdPeople VARCHAR(100)
      Select  TOP 1 @actualSettingsType = @settingsType From @db_user.dpa_registri_repertorio 
      Where counterId = @countId 
      If @actualSettingsType != @settingsType And @settingsType = 'S'
/*,
RoleRespId = null,
PrinterRoleRespId = null,
PrinterUserRespId = null,
PrintFreq = 'N',
DtaStart = null,
DtaFinish = null,
DtaNextAutomaticPrint = null,
CounterState = 'O'*/
         update @db_user.dpa_registri_repertorio set @settingsType = 'S'  
         Where CounterId = @countId And TipologyKind = @tipology

      If @roleRespIdGroup is null
         SET @decodedRoleRespIdGroup = 'null'
   Else
      SET @decodedRoleRespIdGroup = @roleRespIdGroup

      If @roleIdGroup is null
         SET @decodedRoleIdGroup = 'null'
   Else
      SET @decodedRoleIdGroup = @roleIdGroup

      If @userIdPeople is null
         SET @decodedUserIdPeople = 'null'
   Else
      SET @decodedUserIdPeople = @userIdPeople

      Begin
         DECLARE @updateQuery VARCHAR(2000) = 'Update @db_user.dpa_registri_repertorio
					Set RoleRespId = ' + @decodedRoleRespIdGroup + '
					PrinterRoleRespId = ' + @decodedRoleIdGroup + ' 
					PrinterUserRespId = ' + @decodedUserIdPeople + '
					PrintFreq ''' + @printFrequency + '
					DtaStart = to_date(''' + CONVERT( VARCHAR(30), @dateAutomaticPrintStart, 110) + '
					DtaFinish = to_date(''' + CONVERT( VARCHAR(30), @dateAutomaticPrintFinish, 110) + '
					DtaNextAutomaticPrint = to_date(''' + CONVERT( VARCHAR(30), @dateNextAutomaticPrint, 110) + '
					CounterState = ''' + @state + '
					Resprights = ''' + @rights + '
					Where CounterId = ' + @countId + '
					And TipologyKind = ''' + @tipology + '''
And '
         IF @reg is not null And CAST(@reg as INT) > 0
            SET @updateQuery = @updateQuery + ' RegistryId = ' + @reg + ' And '
      Else
         SET @updateQuery = @updateQuery + 'RegistryId is null And'
 
         IF @rf is not null And CAST(@rf as INT) > 0
            SET @updateQuery = @updateQuery + ' RfId = ' + @rf
      Else
         SET @updateQuery = @updateQuery + ' RfId is null'
 
         execute(@updateQuery)
      End
   End
 

-- Impostazione del valore di ritorno
   SET @returnValue = 1  
END 

GO
              
----------- FINE -
              
---- 0.SetRightsForRole.MSSQL.SQL  marcatore per ricerca ----
-- Controllo esistenza SP CalculateAtipDelRole
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='SetRightsForRole')
DROP PROCEDURE @db_user.SetRightsForRole
go


create PROCEDURE @db_user.SetRightsForRole @objId INT,
@idRole INT,
@rightsToAssign INT,
@returnValue INT OUTPUT  AS
BEGIN
/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     SetRightsForRole

PURPOSE:  Store per l'assegnazione dei diritti di tipo A ad un ruolo (solo
se il ruolo non li possiede gi o se non possiede diritti superiori)

******************************************************************************/


-- Diritti posseduti dal ruolo

-- Selezione degli eventuali diritti posseduti da un ruolo
   DECLARE @rights VARCHAR(2000)
   Select   @rights = Max(AccessRights) From  security Where Thing = @objId And PersonOrGroup = @idRole

-- Se i diritti sono ci sono, si procede con un inserimento
   IF @rights Is Null
      Insert Into security(THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO,
HIDE_DOC_VERSIONS,
TS_INSERIMENTO,
VAR_NOTE_SEC)
VALUES(@objId,
@idRole,
@rightsToAssign,
null,
'A',
null,
GetDate(),
null)

Else
   Begin
-- Se i diritti posseduti dal ruolo sono minori di quelli che si vogliono concedere,
-- si procede con un aggiornamento del diritto
      IF CAST(@rights as INT) != 0 And @rightsToAssign > CAST(@rights as INT)
         update security set ACCESSRIGHTS = @rightsToAssign,CHA_TIPO_DIRITTO = 'A'  Where Thing = @objId
 
   End
 
   SET @returnValue = 1
End 

GO              
----------- FINE -
              
---- 0.vis_doc_anomala_custom.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[vis_doc_anomala_custom ]') 
		AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
drop procedure [@db_user].[vis_doc_anomala_custom ]
GO

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
                  SET @ruoli_sup = CURSOR  FOR SELECT dpa_corr_globali.id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                  ON id_tipo_ruolo =
                  dpa_tipo_ruolo.system_id
                  WHERE id_uo IN(SELECT     dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE dta_fine IS NULL)
                  AND cha_tipo_urp = 'R'
                  AND dpa_corr_globali.id_amm = @p_id_amm
                  AND dta_fine IS NULL
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
                     FROM dpa_corr_globali [INNER] JOIN dpa_tipo_ruolo
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
                  SET @ruoli_sup = CURSOR  FOR SELECT id_gruppo
                  FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                  ON id_tipo_ruolo =
                  dpa_tipo_ruolo.system_id
                  WHERE id_uo IN(SELECT     dpa_corr_globali.system_id
                     FROM dpa_corr_globali
                     WHERE dpa_corr_globali.dta_fine IS NULL)
                  AND cha_tipo_urp = 'R'
                  AND dpa_corr_globali.id_amm = @p_id_amm
                  AND dta_fine IS NULL
                  AND dpa_tipo_ruolo.num_livello <(SELECT dpa_tipo_ruolo.num_livello
                     FROM dpa_corr_globali [INNER] JOIN dpa_tipo_ruolo
                     ON id_tipo_ruolo =
                     dpa_tipo_ruolo.system_id
                     WHERE id_gruppo =
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
              
---- 1.InitRegRepertorio.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='InitRegRepertorio')
DROP PROCEDURE @db_user.InitRegRepertorio
go

create PROCEDURE @db_user.InitRegRepertorio  AS
BEGIN
/******************************************************************************
AUTHOR:   Samuele Furnari
NAME:     InitRegRepertorio
PURPOSE:  Store per l'inizializzazione della tabella dell'anagrafica dei 
registri di repertorio.
******************************************************************************/

-- Preventiva pulizia dell'anagrafica dei registri di repertorio
   Delete From dpa_registri_repertorio

-- Id della tipologia, id del contatore, tipo di contatore definito e id del registro
   Begin
-- Cursore per scrorrere le informazioni sui contatori definiti per le tipologie
-- documento
      DECLARE @tipologyId INT
      DECLARE @counterId INT
      DECLARE @counterType CHAR
      DECLARE @registryRfId INT
      DECLARE @cursorCounters CURSOR

      SET @cursorCounters = CURSOR  FOR(Select
         ta.system_id TipologyId,
		oc.system_id as CounterId,
		oc.cha_tipo_tar as CounterType
         From dpa_tipo_atto ta
         Inner Join dpa_ogg_custom_comp occ
         On ta.system_id = occ.id_template
         Inner Join dpa_oggetti_custom oc
         On occ.id_ogg_custom = oc.system_id
         Inner Join dpa_tipo_oggetto tobj
         On oc.id_tipo_oggetto = tobj.system_id
         Where lower(tobj.descrizione) = 'contatore'
         And oc.repertorio = 1)
      OPEN @cursorCounters
      while 1 = 1
      begin
         FETCH @cursorCounters INTO @tipologyId,@counterId,@counterType
         if @@FETCH_STATUS <> 0
         BREAK
-- Inserimento delle informazioni sul registro nell'anagrafica
         EXECUTE InsertRegistroRepertorio @tipologyId,@counterId,@counterType,'D'
      end
      CLOSE @cursorCounters
/*
Begin
-- Cursore per scorrere le informazioni sui contatori definiti per le tipologie
-- fascicoli
Declare Cursor cursorCountersFasc Is (
Select 
tf.system_id TipologyId,
oc.system_id as CounterId,
oc.cha_tipo_tar as CounterType
From dpa_tipo_fasc tf
Inner Join dpa_ogg_custom_comp_fasc occ
On tf.system_id = occ.id_template
Inner Join dpa_oggetti_custom_fasc oc
On occ.id_ogg_custom = oc.system_id
Inner Join dpa_tipo_oggetto_fasc tobj
On oc.id_tipo_oggetto = tobj.system_id
Where lower(tobj.descrizione) = 'contatore'
And oc.repertorio = 1
);

BEGIN OPEN cursorCountersFasc;
LOOP FETCH cursorCountersFasc INTO tipologyId, counterId, counterType;
EXIT WHEN cursorCountersFasc%NOTFOUND;
-- Inserimento delle informazioni sul registro nell'anagrafica
InsertRegistroRepertorio(tipologyId, counterId, counterType, 'F');

END LOOP;
CLOSE cursorCountersFasc;
End;  */
   End  
end 

GO


-- questa istruzione si esegue solo una volta - FUORI CD - con questa istruzione 
-- begin	@db_user.INITREGREPERTORIO; end;


              
----------- FINE -
              
---- DeleteRegistroRepertorio.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='DeleteRegistroRepertorio')
DROP PROCEDURE @db_user.DeleteRegistroRepertorio
go

create Procedure @db_user.DeleteRegistroRepertorio 
-- Id del contatore
@countId INT  AS
Begin
/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     DeleteRegistroRepertorio

PURPOSE:  Store per l'eliminazione di un registro di repertorio nell'anagrafica            

******************************************************************************/
Delete From @db_user.dpa_registri_repertorio Where CounterId = @countId

End 

GO

              
----------- FINE -
              
---- InsertRepertorioFromCode.MSSQL.SQL  marcatore per ricerca ----
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' and name ='InsertRepertorioFromCode')
DROP PROCEDURE @db_user.InsertRepertorioFromCode
go

create PROCEDURE @db_user.InsertRepertorioFromCode 
@tipologyId INT ,
@counterId INT ,
@counterType VARCHAR(4000) ,
@returnValue INT OUTPUT  AS
BEGIN
   EXECUTE @db_user.InsertRepertorioFromCode @tipologyId,@counterId,@counterType,'D'

   SET @returnValue = 0

End 

GO

              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.19.1')
GO

              
