declare @ultimoID int

begin

Insert into @db_user.DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ( 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)


Insert into @db_user.DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ('AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)

END
GO

--ALTER TABLE DPA_RAGIONE_TRASM ADD (PRIMARY KEY (SYSTEM_ID))

declare @nometabella varchar(100)
declare @nomecolonna varchar(100)
declare @proprietario varchar(32) 
	SET @nometabella = 'DPA_RAGIONE_TRASM'
	SET @nomecolonna = 'SYSTEM_ID'
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'PRIMARY_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
  where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''')'

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
			SET @istruzioneCheck = 'select @cntOUT = COUNT(*) from ['+@proprietario+'].' + @nometabella +'
								group by '+@nomecolonna+' having COUNT(*) > 1'
			EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la PK_'+@nometabella
				  SET @istruzioneSQL = N'ALTER TABLE ['+@proprietario+'].' +@nometabella + ' ADD constraint PK_' +@nometabella+ ' PRIMARY KEY (' +@nomecolonna+') '
					  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione PK su tabella ' + @nometabella 
			end
	end 
END
GO

-- add PK su system_id

declare @nometabella varchar(100)
declare @nomecolonna varchar(100)
declare @proprietario varchar(32) 
	SET @nometabella = 'DPA_TRASMISSIONE'
	SET @nomecolonna = 'SYSTEM_ID'
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'PRIMARY_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''')'

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
			SET @istruzioneCheck = 'select @cntOUT = COUNT(*) from ['+@proprietario+'].' + @nometabella +'
								group by '+@nomecolonna+' having COUNT(*) > 1'
			EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'sto creando la PK_' +@nometabella
				  SET @istruzioneSQL = N'ALTER TABLE ['+@proprietario+'].' +@nometabella + ' ADD constraint PK_' +@nometabella+ ' PRIMARY KEY (' +@nomecolonna+') '
					  execute sp_executesql @istruzioneSQL 

			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione PK_' +@nometabella+' su tabella ' + @nometabella 
			end
	end 
END
GO
--in questo script si fanno: 
--ALTER TABLE DPA_TRASM_SINGOLA ADD (PRIMARY KEY (SYSTEM_ID))
-- FOREIGN KEY DALLA ID_TRASMISSIONE DELLA DPA_TRASM_SINGOLA VERSO LA  SYSTEM_ID DELLA DPA_TRASMISSIONE
-- FOREIGN KEY DALLA ID_RAGIONE DELLA DPA_TRASM_SINGOLA VERSO LA  SYSTEM_ID DELLA DPA_RAGIONE_TRASM



-- primo :
--ALTER TABLE DPA_TRASM_SINGOLA ADD (PRIMARY KEY (SYSTEM_ID))

declare @nometabella varchar(100)
declare @nomecolonna varchar(100)
declare @proprietario varchar(32) 
	SET @nometabella = 'DPA_TRASM_SINGOLA'
	SET @nomecolonna = 'SYSTEM_ID'
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'PRIMARY_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
  where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''')'

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
			SET @istruzioneCheck = 'select @cntOUT = COUNT(*) from ['+@proprietario+'].' + @nometabella +'
								group by '+@nomecolonna+' having COUNT(*) > 1'
			EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la PK_'+@nometabella
				  SET @istruzioneSQL = N'ALTER TABLE ['+@proprietario+'].' +@nometabella + ' ADD constraint PK_' +@nometabella+ ' PRIMARY KEY (' +@nomecolonna+') '
					  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione PK su tabella ' + @nometabella 
			end
	end 
END
GO


-- FOREIGN KEY DALLA ID_TRASMISSIONE DELLA DPA_TRASM_SINGOLA VERSO LA  SYSTEM_ID DELLA DPA_TRASMISSIONE
declare @nometabellaFK varchar(100)
declare @nomecolonnaFK varchar(100)
declare @nometabellaPK varchar(100)
declare @nomecolonnaPK varchar(100)

declare @proprietario varchar(32) 

	SET @nometabellaFK = 'DPA_TRASM_SINGOLA'
	SET @nomecolonnaFK = 'ID_TRASMISSIONE'
	
	SET @nometabellaPK = 'DPA_TRASMISSIONE'
	SET @nomecolonnaPK = 'SYSTEM_ID'
	
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'FOREIGN_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''')'

SET @istruzioneCheck = 'SELECT  @cntOUT=count(*)     --  i1.TABLE_NAME, i2.COLUMN_NAME 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
WHERE i1.CONSTRAINT_TYPE = ''FOREIGN KEY'' and  i1.TABLE_NAME = '''+@nometabellaFK+''' 
and i2.COLUMN_NAME =     '''+@nomecolonnaFK+'''    '

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
		SET @istruzioneCheck = 'SELECT COUNT (*)
           FROM '+@proprietario+'.'+@nometabellaFK+'
          WHERE '+@nomecolonnaFK+' NOT IN (SELECT '+@nomecolonnaPK+'
                                             FROM '+@proprietario +'.'+@nometabellaPK+')'
	
		
		EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la FK: FK_' + @nometabellaFK + @nomecolonnaFK 
				  
				  SET @istruzioneSQL = N'
					ALTER TABLE [' + @proprietario + '].' +@nometabellaFK + ' ADD CONSTRAINT FK_' 
					+ @nometabellaFK + @nomecolonnaFK + ' FOREIGN KEY ' + '(' + @nomecolonnaFK 
					+ ') REFERENCES [' + @proprietario + '].' + @nometabellaPK + '(' + @nomecolonnaPK + ')'
  			  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione FK su tabella ' + @nometabellaFK 
			end
	end 
END
GO


-- FOREIGN KEY DALLA ID_RAGIONE DELLA DPA_TRASM_SINGOLA VERSO LA  SYSTEM_ID DELLA DPA_RAGIONE_TRASM
declare @nometabellaFK varchar(100)
declare @nomecolonnaFK varchar(100)
declare @nometabellaPK varchar(100)
declare @nomecolonnaPK varchar(100)

declare @proprietario varchar(32) 

	SET @nometabellaFK = 'DPA_TRASM_SINGOLA'
	SET @nomecolonnaFK = 'ID_RAGIONE'
	
	SET @nometabellaPK = 'DPA_RAGIONE_TRASM'
	SET @nomecolonnaPK = 'SYSTEM_ID'
	
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'FOREIGN_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''')'

SET @istruzioneCheck = 'SELECT  @cntOUT=count(*)     --  i1.TABLE_NAME, i2.COLUMN_NAME 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
WHERE i1.CONSTRAINT_TYPE = ''FOREIGN KEY'' and  i1.TABLE_NAME = '''+@nometabellaFK+''' 
and i2.COLUMN_NAME =     '''+@nomecolonnaFK+'''    '

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
		SET @istruzioneCheck = 'SELECT COUNT (*)
           FROM '+@proprietario+'.'+@nometabellaFK+'
          WHERE '+@nomecolonnaFK+' NOT IN (SELECT '+@nomecolonnaPK+'
                                             FROM '+@proprietario +'.'+@nometabellaPK+')'
	
		
	
	    --print @istruzioneCheck
		EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la FK: FK_' + @nometabellaFK + @nomecolonnaFK 
				  SET @istruzioneSQL = N'
					ALTER TABLE [' + @proprietario + '].' +@nometabellaFK + ' ADD CONSTRAINT FK_' 
					+ @nometabellaFK + @nomecolonnaFK + ' FOREIGN KEY ' + '(' + @nomecolonnaFK 
					+ ') REFERENCES [' + @proprietario + '].' + @nometabellaPK + '(' + @nomecolonnaPK + ')'
  			  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione FK su tabella ' + @nometabellaFK 
			end
	end 
END
GO

declare @nometabellaFK varchar(100)
declare @nomecolonnaFK varchar(100)
declare @nometabellaPK varchar(100)
declare @nomecolonnaPK varchar(100)

declare @proprietario varchar(32) 

	SET @nometabellaFK = 'DPA_TRASM_UTENTE'
	SET @nomecolonnaFK = 'ID_TRASM_SINGOLA'
	
	SET @nometabellaPK = 'DPA_TRASM_SINGOLA'
	SET @nomecolonnaPK = 'SYSTEM_ID'
	
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int
declare @tipocons varchar(32)

SET @tipocons  = 'FOREIGN_KEY_CONSTRAINT'
SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabellaFK +''')'

SET @istruzioneCheck = 'SELECT  @cntOUT=count(*)     --  i1.TABLE_NAME, i2.COLUMN_NAME 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
WHERE i1.CONSTRAINT_TYPE = ''FOREIGN KEY'' and  i1.TABLE_NAME = '''+@nometabellaFK+''' 
and i2.COLUMN_NAME =     '''+@nomecolonnaFK+'''    '

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
		SET @istruzioneCheck = 'SELECT COUNT (*)
           FROM '+@proprietario+'.'+@nometabellaFK+'
          WHERE '+@nomecolonnaFK+' NOT IN (SELECT '+@nomecolonnaPK+'
                                             FROM '+@proprietario +'.'+@nometabellaPK+')'
	
		
	
		-- print @istruzioneCheck
		EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la FK: FK_' + @nometabellaFK + @nomecolonnaFK 
				  SET @istruzioneSQL = N'
					ALTER TABLE [' + @proprietario + '].' +@nometabellaFK + ' ADD CONSTRAINT FK_' 
					+ @nometabellaFK + @nomecolonnaFK + ' FOREIGN KEY ' + '(' + @nomecolonnaFK 
					+ ') REFERENCES [' + @proprietario + '].' + @nometabellaPK + '(' + @nomecolonnaPK + ')'
  			  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione FK su tabella ' + @nometabellaFK 
			end
	end 
END
GO
/*
AUTORE:					  Furnari Samuele Alfredo 
Data creazione:				  12/04/2011
Scopo della modifica:		AGGIUNGERE LA COLONNA MTEXT_FQN  NEL COMPONENTS
				
*/
if not exists(select * from syscolumns where name='MTEXT_FQN' and id in
		(select id from sysobjects where name='COMPONENTS' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)

	declare @nometabella varchar(100)
	declare @nomecolonna varchar(100)
    
    SET @proprietario = '@db_user'

    SET @nometabella = 'COMPONENTS'
    SET @nomecolonna = 'MTEXT_FQN'

   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(500) '
       execute sp_executesql @sqlstatement ;
	
END
GO

--alter table [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] add constraint 
--      UQ_DPA_CHIAVI_CONFIGURAZIONE_var_codice unique (var_codice)


declare @nometabella varchar(100)
declare @nomecolonna varchar(100)
declare @proprietario varchar(32)
declare @tipocons varchar(32)

    SET @nometabella = 'DPA_CHIAVI_CONFIGURAZIONE'
	SET @nomecolonna = 'VAR_CODICE'
    SET @tipocons    = 'UNIQUE_CONSTRAINT'
 
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int

SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''')'

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
			SET @istruzioneCheck = 'select @cntOUT = COUNT(*) from ['+@proprietario+'].' + @nometabella +'
								group by '+@nomecolonna+' having COUNT(*) > 1'
			EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la uk..'
				  SET @istruzioneSQL = N'ALTER TABLE ['+@proprietario+'].' +@nometabella + ' ADD constraint UK_' +@nometabella+ @nomecolonna + ' UNIQUE (' +@nomecolonna+') '
					  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione UK su tabella ' + @nometabella 
			end
	end 
END
GO

/*
AUTORE:					  AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:		AGGIUNGERE LA COLONNA VAR_CHIAVE_AE NEL DPA_CORR_GLOBALI 

modifica del 15 aprile richiesta modifica nome da VAR_CHIAVE_ARCA in VAR_CHIAVE_AE 
modifica del 20 aprile richiesta default ''0''
				
*/
if not exists(select * from syscolumns where name='VAR_CHIAVE_AE' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)

	declare @nometabella varchar(100)
	declare @nomecolonna varchar(100)
    
    SET @proprietario = '@db_user'

    SET @nometabella = 'DPA_CORR_GLOBALI'
    SET @nomecolonna = 'VAR_CHIAVE_AE'
	
   SET @sqlstatement = N'alter table 
   [' + @proprietario + '].[' + @nometabella + '] 
   ADD ' + @nomecolonna +'  varchar(12) default ''0'' '
       execute sp_executesql @sqlstatement ;
	   print 'aggiunta colonna VAR_CHIAVE_AE'
	
END
GO


/*
AUTORE:					  P. Buono 
Data creazione:				  20/05/2011
Scopo della modifica:	     Disabilitazione alla ricezione trasmissione dei ruoli -
colonna CHA_DISABLED_TRASM VARCHAR2(1) NELLA DPA_CORR_GLOBALI
				
*/
if not exists(select * from syscolumns where name='CHA_DISABLED_TRASM' and id in
		(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)

	declare @nometabella varchar(100)
	declare @nomecolonna varchar(100)
    
    SET @proprietario = '@db_user'

    SET @nometabella = 'DPA_CORR_GLOBALI'
    SET @nomecolonna = 'CHA_DISABLED_TRASM'
	
   SET @sqlstatement = N'alter table 
   [' + @proprietario + '].[' + @nometabella + '] 
   ADD ' + @nomecolonna +'  varchar(1) '
       execute sp_executesql @sqlstatement ;
	   print 'aggiunta colonna CHA_DISABLED_TRASM'
	
END
GO

/*
AUTORE:					  AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:	        AGGIUNGERE DUE COLONNE CHA_SESSO E CHA_PROVINCIA_NASCITA NEL DPA_DETT_GLOBALI
				MODIFICHE TRE COLONNE VAR_INDIRIZZO,*CAP,*PROVINCIA

*/

BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)

	declare @nometabella varchar(100)
	declare @nomecolonna varchar(100)
	    
    SET @proprietario = '@db_user'

    SET @nometabella = 'DPA_DETT_GLOBALI'
    

   begin
   SET @nomecolonna = 'CHA_SESSO'
   if not exists(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name='DPA_DETT_GLOBALI' and xtype='U'))
		
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  char(1)  '
       execute sp_executesql @sqlstatement ;
   end    
   
   begin
   SET @nomecolonna = 'CHAR_PROVINCIA_NASCITA'
  
   if not exists(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name='DPA_DETT_GLOBALI' and xtype='U'))
   
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(3)  '
       execute sp_executesql @sqlstatement ;
   end
   
   
   SET @nomecolonna = 'VAR_INDIRIZZO'

   if 
   NOT EXISTS(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name='DPA_DETT_GLOBALI' and xtype='U'))
		
   begin
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(3)  '
	   execute sp_executesql @sqlstatement ;
   end
   
   else 
   
   begin
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ALTER COLUMN ' + @nomecolonna +'  varchar(256)  '
	   execute sp_executesql @sqlstatement ;
   end
      
       
   SET @nomecolonna = 'VAR_CAP'
   if 
   NOT EXISTS(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name='DPA_DETT_GLOBALI' and xtype='U'))
		
   begin
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(9)  '
       execute sp_executesql @sqlstatement ;
   end
   
   else
   
   begin
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ALTER COLUMN ' + @nomecolonna +'  varchar(256)  '
	   execute sp_executesql @sqlstatement ;
   end
   
   
   SET @nomecolonna = 'VAR_PROVINCIA'  
   if 
   NOT EXISTS(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name='DPA_DETT_GLOBALI' and xtype='U'))
		
   begin    
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(3)  '
       execute sp_executesql @sqlstatement ;
   end
   
   else
   
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ALTER COLUMN ' + @nomecolonna +'  varchar(3)  '
       execute sp_executesql @sqlstatement ;
   end
   
GO
-- aggiungere colonna DPA_EL_REGISTRI.FLAG_WSPIA CHAR(1) nullable

if not exists (
			SELECT * FROM syscolumns
			WHERE name='FLAG_WSPIA' and id in
			(SELECT id FROM sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_EL_REGISTRI]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_EL_REGISTRI] ADD FLAG_WSPIA CHAR(1)
END
GO
declare @cnt  int
select @cnt = count(*) from [@db_user].[DPA_TIPO_NOTIFICA]

IF @cnt = 0 
BEGIN	

-- modifica la colonna system_id impostando identity e PK, necessario drop e create
DROP TABLE @db_user.DPA_TIPO_NOTIFICA

CREATE TABLE @db_user.DPA_TIPO_NOTIFICA
                       (SYSTEM_ID            INT    IDENTITY(1,1)  NOT NULL  PRIMARY KEY, 
                        VAR_CODICE_NOTIFICA  VARCHAR(50)           NOT NULL, 
                        VAR_DESCRIZIONE      VARCHAR(255) 
                        )
END
GO

/*
AUTORE:						SPERILLI
Data creazione:				28/02/2011
Scopo della modifica:		INSERITO CAMPO PER TENERE MEMORIA DELLE 
							STAMPE EFFETTUATE DI OGNI PROTOCOLLO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV 3817
*/

if not exists(select * from syscolumns where name='PRINTS_NUM' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].PROFILE ADD PRINTS_NUM INT NULL
	-- bonifica pregresso -- richiesta by C. Ferlito -- 4 maggio 2011
	
END
GO

UPDATE [@db_user].PROFILE SET PRINTS_NUM=1 WHERE PRINTS_NUM IS NULL
GO

--ALTER TABLE SECURITY ADD TS_INSERIMENTO DATE DEFAULT GETDATE()  -- da vedere il default
--Autore: P.De Luca -- storico inserimento

declare @sqlstatement Nvarchar(100)
declare @proprietario  varchar(100)

declare @nometabella varchar(100)
declare @nomecolonna varchar(100)

   SET @proprietario = '@db_user'

   SET @nometabella = 'SECURITY'
   SET @nomecolonna = 'TS_INSERIMENTO'


if not exists(select * from syscolumns where name=@nomecolonna and id in
		(select id from sysobjects where name=@nometabella and xtype='U'))
BEGIN
   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +' DATETIME DEFAULT GETDATE() '
       execute sp_executesql @sqlstatement ;

-- per gestire il pregresso, solo su SQL server, occorre fare l'update esplicito
-- va fatto con execute perch la colonna non  ancora definita prima del runtime!

-- non necessario		update SECURITY set 	   TS_INSERIMENTO =  GETDATE()
--   SET @sqlstatement = N'update [' + @proprietario + '].[' + @nometabella + '] set ' + @nomecolonna + ' = GETDATE()'
--       execute sp_executesql @sqlstatement ;

END
GO
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
/*
AUTORE:					  AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:		CREARE LA TABELLA DPA_CONFIG_ANAGRAFICA_ESTERNA

data modifica:				12/04/2011
modifica:					aggiunta colonna "ENABLED char(1) NOT NULL default 1 
*/

IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_CONFIG_ANAGRAFICA_ESTERNA]')
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)
BEGIN
create table [@db_user].DPA_CONFIG_ANAGRAFICA_ESTERNA (
SYSTEM_ID int identity(1,1)               primary key
,NOME_ANAGRAFICA_ESTERNA varchar(16)      NOT NULL
,WSURL_ANAGRAFICA_ESTERNA varchar(256)    NOT NULL
,INTEGRATION_ADAPTER_ID varchar(100)       NOT NULL
,INTEGRATION_ADAPTER_VERSION varchar(8)   NOT NULL
,CONFIG_PROVENIENZA varchar(2)
,CONFIG_MATRICOLA varchar(8)
,CONFIG_PASSWORD varchar(8)
,CONFIG_APPLICAZIONE varchar(8)
,CONFIG_RUOLO varchar(6)
,NOTE varchar(516)      
,ENABLED char(1) NOT NULL default 1      )
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
/*
AUTORE:					C. Ferlito
Data creazione:				      01/04/2011
Scopo della modifica:		storicizzare modifiche

*/
IF NOT EXISTS (
              SELECT * FROM dbo.sysobjects
              WHERE id = OBJECT_ID(N'[@db_user].[DPA_PROFIL_FASC_STO]')
              AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)

BEGIN

CREATE TABLE [@db_user].[DPA_PROFIL_FASC_STO](
systemId INT IDENTITY(1,1) NOT NULL,
id_template int not null,
dta_modifica datetime not null,
id_project int not null,
id_ogg_custom int not null,
id_people int null,
id_ruolo_in_uo int not null,
var_desc_modifica varchar(2000)
)
END
GO
/*
AUTORE:						SERPI GABRIELE
Data creazione:				      01/04/2011
Scopo della modifica:		CONTROLLARE L'ESISTENZA DELLA TABELLA DPA_PROFIL_STO E CREAZIONE

*/
IF NOT EXISTS (
             SELECT * FROM dbo.sysobjects
             WHERE id = OBJECT_ID(N'[@db_user].[DPA_PROFIL_STO]')
             AND OBJECTPROPERTY(id, N'IsUserTable') = 1)


BEGIN

CREATE TABLE [@db_user].[DPA_PROFIL_STO](
systemId INT IDENTITY(1,1) NOT NULL,
id_template int not null,
dta_modifica datetime not null,
id_profile int not null,
id_ogg_custom int not null,
id_people int not null,
id_ruolo_in_uo int not null,
var_desc_modifica varchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS
)
END
GO

IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[GETcodUO]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].[GETcodUO]
go
CREATE FUNCTION [@db_user].[GETcodUO] (@idUO INT)
RETURNs VARCHAR(256)
AS
BEGIN
                DECLARE @risultato VARCHAR(256)
                IF(@idUO is null)
                             BEGIN
							   SET @risultato = ' '
				END  ELSE
                               BEGIN
                                               SET @risultato = ' '

                                               SELECT @risultato = var_codice 
												FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = @idUO 
                                               IF(@@ERROR <> 0)
                                                              BEGIN
															   SET @risultato = ''
															   END 
                               END

                RETURN @risultato
END
GO

IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[getFascPrimaria]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].[getFascPrimaria]
go
CREATE function [@db_user].[getFascPrimaria](@iProfile INT, @idFascicolo int)
returns varchar(1)
as
begin
 
declare @fascPrimaria varchar(1)
SET @fascPrimaria = '0'
set @fascPrimaria = (SELECT top 1 isnull(B.CHA_FASC_PRIMARIA,'0') 
					FROM PROJECT A, PROJECT_COMPONENTS B
					WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=@iProfile 
					and ID_FASCICOLO=@idFascicolo)
 
return @fascPrimaria
end
GO

IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[iscorrispondenteinterno]')
			AND xtype in (N'FN', N'IF', N'TF')
			)

DROP  function [@db_user].[iscorrispondenteinterno]
go
CREATE FUNCTION [@db_user].[iscorrispondenteinterno]
( @idcorrglobali   INT,
   @idregistro      INT
)
RETURNS int
AS
BEGIN
      -- Declare the return variable here
   DECLARE
      @tipourp                 VARCHAR (1);
   DECLARE 
        @tipoie                  VARCHAR (1);
   DECLARE 
        @idpeople                INT;
   DECLARE 
        @numero_corrispondenti   INT;
 
 
     SET @tipoie = 'E'
     SET   @numero_corrispondenti = 0
 
      SELECT @tipoie = a.cha_tipo_ie
        FROM dpa_corr_globali a
       WHERE a.system_id = @idcorrglobali;
 
   IF (@tipoie = 'I')
   BEGIN
         SELECT @tipourp = a.cha_tipo_urp
          FROM dpa_corr_globali a
          WHERE a.system_id = @idcorrglobali;
 
         IF (@tipourp = 'U')
             BEGIN
            SELECT @numero_corrispondenti = COUNT (*)
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
              WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND id_uo = @idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = @idregistro
               AND r.cha_rf = '0';
         END
 
          IF (@tipourp = 'R')
          BEGIN
            SELECT @numero_corrispondenti = COUNT (*)
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND a.system_id = @idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = @idregistro
               AND r.cha_rf = '0';
          END 
 
            IF (@tipourp = 'P')
            BEGIN
               SELECT @idpeople=a.id_people
                 FROM dpa_corr_globali a
                WHERE a.system_id = @idcorrglobali;
 
               SELECT @numero_corrispondenti = COUNT (a.system_id)
                   FROM dpa_corr_globali a,
                        peoplegroups b,
                        dpa_l_ruolo_reg f,
                        dpa_el_registri r
                  WHERE a.id_gruppo = b.groups_system_id
                    AND b.dta_fine IS NULL
                    AND b.people_system_id = @idpeople
                    AND f.id_ruolo_in_uo = a.system_id
                    AND f.id_registro = r.system_id
                    AND r.system_id = @idregistro
                    AND r.cha_rf = '0'
            END
         END
 
      IF (@numero_corrispondenti > 0)
         return 1
      
      RETURN 0
 
END
GO


-- =============================================================================

-- Author:        Samuele Furnari / Mario Caropreso
-- Create date:   28 Marzo 2011
-- Description:   Funzione per il recupero del Full Qualified Name di un documento
--                      MText a partire dall'id di versione e dal doc number
-- =============================================================================

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[GetMTextFullQualifiedName]')
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[GetMTextFullQualifiedName]
GO
CREATE  PROCEDURE [@db_user].[GetMTextFullQualifiedName]
(
      @versionId int,
      @docNumber int,
      @fqn varchar(500) out
)
AS
BEGIN
      DECLARE @fullQualifiedName varchar(500)
      set @fullQualifiedName = ''
     
      -- Selezione del valore di interesse
      SELECT @fullQualifiedName = C.MTEXT_FQN  FROM @db_user.COMPONENTS AS C
			WHERE C.VERSION_ID = @versionId AND C.DOCNUMBER = @docNumber
      set @fqn = @fullQualifiedName

END
GO

-- ================================================================================
-- Author:        Samuele Furnari / Mario Caropreso
-- Create date: 28 Marzo 2011
-- Description:   Funzione per il salvataggio del Full Qualified Name di un documento
--                      identifi
-- ================================================================================

if exists ( select * from dbo.sysobjects 
				where id = object_id(N'[@db_user].[SetMTextFullQualifiedName]')
				and OBJECTPROPERTY(id, N'IsProcedure') = 1
				)
drop procedure [@db_user].[SetMTextFullQualifiedName]
GO

CREATE  PROCEDURE [@db_user].[SetMTextFullQualifiedName] (
      @docNumber int,   
      @fqn varchar(500)
	  ,@resultValue  varchar(100)  out
      )
AS
BEGIN
      DECLARE @versionId int;
      -- Reperimento id dell'ultima versione del documento
      select @versionId = MAX(C.VERSION_ID) from @db_user.COMPONENTS AS C
		where c.DOCNUMBER = @docNumber 
      
      -- Salvataggio dell'fqn
      UPDATE @db_user.COMPONENTS
      SET MTEXT_FQN = @fqn
      where DOCNUMBER = @docNumber AND VERSION_ID = @versionId

		SET @resultValue  = 0
	    IF (@@ROWCOUNT <> 1 OR @@ERROR <> 0)
			BEGIN 
		      SET @resultValue  = 1 
			END
      
      
END
GO

-- inserimento in DPA_CHIAVI_CONFIGURAZIONE

-- bonifica record duplicati di FE_ABILITA_GEST_DOCS_ST_FINALE
delete FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE = 'FE_ABILITA_GEST_DOCS_ST_FINALE'


-- inserimento vincolo unicit su FE_ABILITA_GEST_DOCS_ST_FINALE

--alter table [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] add constraint 
--      UQ_DPA_CHIAVI_CONFIGURAZIONE_var_codice unique (var_codice)


declare @nometabella varchar(100)
declare @nomecolonna varchar(100)
declare @proprietario varchar(32)
declare @tipocons varchar(32)

    SET @nometabella = 'DPA_CHIAVI_CONFIGURAZIONE'
	SET @nomecolonna = 'VAR_CODICE'
    SET @tipocons    = 'UNIQUE_CONSTRAINT'
 
	SET @proprietario = '@db_user' -- in test valorizzare con  @db_user
	-- fine variabili da modificare

declare @istruzioneSQL Nvarchar(1000)
declare @istruzioneCheck Nvarchar(1000)
DECLARE @ParamDefinition nvarchar(500)
declare @cntdati int
declare @cnt int

SET @ParamDefinition = N'@cntOUT int OUTPUT';
SET @istruzioneCheck = 'select @cntOUT=1 from sysobjects 
							where ID=OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''') and xtype=''U'' '

EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT

if @cnt = 1
-- se esiste la tabella
begin
	SET @istruzioneCheck = 'SELECT @cntOUT=count(*) FROM sys.objects
	WHERE type_desc = '''+@tipocons+''' and parent_object_id= OBJECT_ID(N''['+@proprietario+'].' + @nometabella +''')'

	EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cnt OUT
	-- se non esiste il constraint
	if @cnt = 0
	begin 
			SET @istruzioneCheck = 'select @cntOUT = COUNT(*) from ['+@proprietario+'].' + @nometabella +'
								group by '+@nomecolonna+' having COUNT(*) > 1'
			EXECUTE sp_executesql @istruzioneCheck, @ParamDefinition, @cntOUT=@cntdati OUT

			if @cntdati is NULL
			-- se non esistono dati duplicati
			begin
				  print 'creo la uk..'
				  SET @istruzioneSQL = N'ALTER TABLE ['+@proprietario+'].' +@nometabella + ' ADD constraint UK_' +@nometabella+ @nomecolonna + ' UNIQUE (' +@nomecolonna+') '
					  execute sp_executesql @istruzioneSQL 
			end
			else
			-- altrimenti non creare 
			begin 
				  print 'errore in creazione UK su tabella ' + @nometabella 
			end
	end 
END
GO

-- fine inserimento vincolo unicit su FE_ABILITA_GEST_DOCS_ST_FINALE

-- inizio inserimento record

-- by S. Furnari

-- FE_AUTOMATIC_SCAN
if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
				WHERE VAR_CODICE = 'FE_AUTOMATIC_SCAN')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
	(ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES ('0','FE_AUTOMATIC_SCAN'
	,'Pu assumere i valori 0 o 1 e serve per abilitare la partenza automatica del driver Twain durante l''acquisizione di un documento'
	, '1' -- attivo
    ,'F','1','1','1')
END
GO

-- FE_RIPROPONI_CON_CONOSCENZA
if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
				WHERE VAR_CODICE = 'FE_RIPROPONI_CON_CONOSCENZA')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
	(ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES ('0','FE_RIPROPONI_CON_CONOSCENZA'
	,'Pu assumere 0 o 1 e serve per abilitare il tasto riproponi per documenti in sola lettura'
	, '1' -- attivo
    ,'F','1','1','1')
END
GO


-- by C. Ferlito

if not exists (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
				WHERE VAR_CODICE = 'FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
	INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
	(ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE) 
	VALUES ('0','FE_ABILITA_GEST_DOCS_ST_FINALE'
	,'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE '
	,'0' -- non attiva
	,'F', '0', '0', '1')
END
GO


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_SALVA_EMAIL_IN_LOCALE'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values 
  (0,'BE_SALVA_EMAIL_IN_LOCALE','Abilita il salvatggio delle mail in locale ( 1 attivo 0 no)'
  , '1' -- attivo
  ,'B','1','1','1');
END
GO

if (not exists (select * from [@db_user].DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_VISUAL_DOC_SMISTAMENTO'))
BEGIN
  insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values 
  (0,'FE_VISUAL_DOC_SMISTAMENTO','Determina il default per la checkbox su Visualizza Documento su smistamento (1=checked, 0=not checked)'
  , '1' -- attivo
  ,'F','1','1','1');
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_DBLCLICK_INVIO_TITOLARIO'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)  values
  (0,'FE_DBLCLICK_INVIO_TITOLARIO','Chiave di attivazione degli eventi doppio click ed invio da tastiera sull''albero del titolario'
	, '1' -- attivo
	,'F','1','1','1');
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_VISUAL_DOC_SMISTAMENTO'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE) values
  (0,'FE_VISUAL_DOC_SMISTAMENTO','Determina il default (checked/not checked) per la checkbox su Visualizza Documento su smistamento (1=checked, 0=not checked)'
   , '1'
   ,'F','1','1','1');
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_ORARIO_CONN_UTE_H24'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)  values
  (0,'BE_ORARIO_CONN_UTE_H24','Chiave di attivazione del formato H24 per la maschera degli utenti connessi, in amministrazione'
    , '1'
	,'B','1','1','1');
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_CHECK_TITOLARIO_ATTIVO'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)  values
  (0,'FE_CHECK_TITOLARIO_ATTIVO','Se a 1 viene selezionato di default il titolario attivo nelle maschere di ricerca fascicoli'
    , '1'
	,'F','1','1','1');
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_MULTI_STAMPA_ETICHETTA'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)  values
  (0,'FE_MULTI_STAMPA_ETICHETTA','Chiave utilizzata per attivare/disattivare la stampa multipla delle etichette'
    , '1'
	,'F','1','1','1');
END
GO


-- by A. Andreacchio
IF (NOT EXISTS (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE='BE_ANAGRAFICA_ESTERNA'))
BEGIN
	INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] ([ID_AMM], [VAR_CODICE], [VAR_DESCRIZIONE], [VAR_VALORE], [CHA_TIPO_CHIAVE], [CHA_VISIBILE], [CHA_MODIFICABILE], [CHA_GLOBALE], [VAR_CODICE_OLD_WEBCONFIG])
        VALUES (0, 'BE_ANAGRAFICA_ESTERNA', 'Abilitazione dell''anagrafica esterna'
		, 'true'
		, 'B', '1', '1', '1', NULL)
END
GO

IF (NOT EXISTS (SELECT * FROM [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] WHERE VAR_CODICE='FE_RUBRICA_RICERCA_ESTESA'))
BEGIN
	INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] ([ID_AMM], [VAR_CODICE], [VAR_DESCRIZIONE], [VAR_VALORE], [CHA_TIPO_CHIAVE], [CHA_VISIBILE], [CHA_MODIFICABILE], [CHA_GLOBALE], [VAR_CODICE_OLD_WEBCONFIG])
        VALUES (0, 'FE_RUBRICA_RICERCA_ESTESA', 'Possibilit di ricercare nella rubrica, utilizzando ulteriori campi'
		, 'true'
		, 'F', '1', '1', '1', NULL)
END
GO

-- fine inserimento in DPA_CHIAVI_CONFIGURAZIONE

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[usp_GetErrorInfo]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[usp_GetErrorInfo]
GO
CREATE PROCEDURE [@db_user].[usp_GetErrorInfo]
AS
SELECT
    ERROR_NUMBER() AS ErrorNumber
    ,ERROR_SEVERITY() AS ErrorSeverity
    ,ERROR_STATE() AS ErrorState
    ,ERROR_PROCEDURE() AS ErrorProcedure
    ,ERROR_LINE() AS ErrorLine
    ,ERROR_MESSAGE() AS ErrorMessage;
GO

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[I_Smistamento_Smistadoc_U]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[I_Smistamento_Smistadoc_U]
GO
CREATE PROCEDURE [@db_user].[I_Smistamento_Smistadoc_U]
@idpeoplemittente               INT       ,
@idcorrglobaleruolomittente     INT       ,
@idgruppomittente               INT       ,
@idamministrazionemittente      INT       ,
@idcorrglobaledestinatario      INT       ,
@iddocumento                    INT       ,
@idtrasmissione                 INT       ,
@idtrasmissioneutentemittente   INT       ,
@trasmissioneconworkflow        CHAR(2000)       ,
@notegeneralidocumento          VARCHAR(4000)       ,
@noteindividuali                VARCHAR(4000)       ,
@datascadenza                   DATETIME       ,
@tipotrasmissione               NCHAR(1)       ,
@tipodiritto                    NCHAR(1)       ,
@rights                         INT       ,
@originalrights                 INT       ,
@idragionetrasm                 INT       ,
@idpeopledelegato               INT       ,
@returnvalue                    INT OUTPUT
AS
/* SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE

-- la SP chiama spsetdatavistasmistamento
-------------------------------------------------------------------------------------------------------
*/
BEGIN
   DECLARE @identitytrasm         INT       = NULL
   DECLARE @systrasmsing          INT       = NULL
   DECLARE @existaccessrights     CHAR(1)     = 'Y'
   DECLARE @accessrights          INT       = NULL
   DECLARE @accessrightsvalue     INT       = NULL
   DECLARE @idutente              INT
   DECLARE @recordcorrente        INT
   DECLARE @idgroups              INT       = NULL
   DECLARE @idgruppo              INT
   DECLARE @resultvalue           INT
   DECLARE @tipotrasmsingola      CHAR(1)     = NULL
   DECLARE @isaccettata           NVARCHAR(1) = '0'
   DECLARE @isaccettatadelegato   NVARCHAR(1) = '0'
   DECLARE @isvista               NVARCHAR(1) = '0'
   DECLARE @isvistadelegato       NVARCHAR(1) = '0'
   DECLARE @TipoRag                        NVARCHAR(1)
   DECLARE @Check_errore             INT
   DECLARE @Check_no_data_err   INT
   
   DECLARE  @val_idpeopledelegato  INT;

   -- @identitytrasm genera l'errore 'Cannot insert explicit value for identity column in table 'DPA_TRASMISSIONE' when IDENTITY_INSERT is set to OFF'
   -- @systrasmsing genera l'errore 'Cannot insert explicit value for identity column in table 'DPA_TRASMISSIONE_SINGOLA' when IDENTITY_INSERT is set to OFF'
   /*BEGIN
      SELECT   @identitytrasm = @@identity

   END

   BEGIN
      SELECT   @systrasmsing = @@identity

   END*/
   
   BEGIN
   
-- inizio modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
/* Inserimento in tabella DPA_TRASMISSIONE */

-- la procedura riceve in input il valore = 0 per il campo idpeopledelegato, in caso di senza delega 
-- e il valore = 1 in caso di delega
-- nel primo caso si vuole avere comunque il valore NULL nel campo idpeopledelegato
IF (@idpeopledelegato > 0)
      BEGIN  
	  SET @val_idpeopledelegato = @idpeopledelegato
      END    ELSE BEGIN
        SET @val_idpeopledelegato = NULL
      END 

INSERT INTO dpa_trasmissione
(
      --system_id,
      id_ruolo_in_uo,
      id_people, 
      cha_tipo_oggetto, 
      id_profile, 
      id_project,
      dta_invio, 
      var_note_generali
	  , id_people_delegato
)
VALUES
(
      --@identitytrasm, 
      @idcorrglobaleruolomittente,
      @idpeoplemittente, 
      'D',
      @iddocumento, 
      NULL,
      GetDate(), 
      @notegeneralidocumento
	  , @val_idpeopledelegato
)
-- fine modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega

      SELECT   @Check_errore = @@ERROR
      IF (@Check_errore <> 0)
      begin
         SET @returnvalue = -2
         RETURN
      end
   END

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
INSERT INTO dpa_trasm_singola
(
      --system_id
      id_ragione, 
       id_trasmissione, 
       cha_tipo_dest,
      id_corr_globale, 
      var_note_sing,
      cha_tipo_trasm, 
      dta_scadenza,
      id_trasm_utente
)
VALUES
(
    --@systrasmsing
      @idragionetrasm,
      @identitytrasm, 
      'R',
      @idcorrglobaledestinatario, 
      @noteindividuali,
      @tipotrasmissione, 
      @datascadenza, 
      NULL
)

      SELECT   @Check_errore = @@ERROR
      IF (@Check_errore = 0)
         SET @returnvalue = 0 -- SET @returnvalue = @systrasmsing, @identitytrasm non viene definita poich @@identity  settata ad off
      IF (@Check_errore <> 0)
      begin
         SET @returnvalue = -3
         RETURN
      end
   END

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT   @idgroups = a.id_gruppo
      FROM dpa_corr_globali a
      WHERE a.system_id = @idcorrglobaledestinatario
   END

   SET @idgruppo = @idgroups

   BEGIN
      SELECT  TOP 1 @accessrights = accessrights
      FROM (SELECT accessrights
         FROM security
         WHERE thing = @iddocumento AND personorgroup = @idgruppo) AS TabAl
      SELECT   @Check_no_data_err = @@ROWCOUNT
      IF (@Check_no_data_err = 0)
         SET @existaccessrights = 'N'
   END

   IF @existaccessrights = 'Y'
   begin
      SET @accessrightsvalue = @accessrights
      IF @accessrightsvalue < @rights
      BEGIN
/* aggiornamento a Rights */
         update security set @accessrights = @rights  WHERE thing = @iddocumento
         AND personorgroup = @idgruppo
         AND @accessrights = @accessrightsvalue
         SELECT   @Check_errore = @@ERROR
      END
   end
ELSE
   BEGIN
/* inserimento a Rights */
INSERT INTO security
(
thing, 
 personorgroup, 
 accessrights, 
 id_gruppo_trasm,
cha_tipo_diritto
)
VALUES
(
@iddocumento, 
@idgruppo, 
@rights, 
@idgruppomittente,
@tipodiritto
)
      SELECT   @Check_errore = @@ERROR
   END


/* Aggiornamento trasmissione del mittente */
   IF (@trasmissioneconworkflow = '1')
   BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
      SELECT   @isaccettata = cha_accettata
      FROM dpa_trasm_utente
      WHERE system_id = @idtrasmissioneutentemittente
      SELECT   @Check_errore = @@ERROR
      IF (@Check_errore = 0)
      begin
         SELECT   @isvista = cha_vista
         FROM dpa_trasm_utente
         WHERE system_id = @idtrasmissioneutentemittente
         SELECT   @Check_errore = @@ERROR
         IF (@Check_errore = 0)
         begin
            SELECT   @TipoRag = cha_tipo_ragione from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
            where tsu.system_id = @idtrasmissioneutentemittente and ts.system_id = tsu.ID_TRASM_SINGOLA and rs.system_id = ts.ID_RAGIONE 
            SELECT   @Check_errore = @@ERROR
            IF (@Check_errore = 0)
            begin
               IF (@idpeopledelegato > 0)
               BEGIN
-- Impostazione dei flag per la gestione del delegato
                  SET @isvistadelegato = '1'
                  SET @isaccettatadelegato = '1'
               END




-- se diritti  trasmssione in accettazione =20
               IF (@isaccettata = '1')
               BEGIN
-- caso in cui la trasmissione risulta gi? accettata
                  IF (@isvista = '0')
                  BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
                     update dpa_trasm_utente 
                     set dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END),cha_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN 1
                  ELSE 0
                     END),cha_vista_delegato = @isvistadelegato,
                     cha_in_todolist = '0',cha_valida = '0'  WHERE (system_id = @idtrasmissioneutentemittente
                     OR system_id =(SELECT tu.system_id
                     FROM dpa_trasm_utente tu,
                             dpa_trasmissione tx,
                             dpa_trasm_singola ts
                     WHERE tu.id_people = @idpeoplemittente
                     AND tx.system_id = ts.id_trasmissione
                     AND tx.system_id = @idtrasmissione
                     AND ts.system_id = tu.id_trasm_singola
                     AND ts.cha_tipo_dest = 'U'))
                  END
               ELSE
                  BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
                     update dpa_trasm_utente 
                     set cha_in_todolist = '0',
                     cha_valida = '0'  
                     WHERE (
                     system_id = @idtrasmissioneutentemittente
                     OR 
                     system_id =(SELECT tu.system_id
                     FROM dpa_trasm_utente tu,
                                   dpa_trasmissione tx,
                                   dpa_trasm_singola ts
                     WHERE tu.id_people = @idpeoplemittente
                     AND tx.system_id = ts.id_trasmissione
                     AND tx.system_id = @idtrasmissione
                     AND ts.system_id = tu.id_trasm_singola
                     AND ts.cha_tipo_dest = 'U'))
                  END

               END
            ELSE
               begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

                  if(@TipoRag = 'W')
                  BEGIN
                     update dpa_trasm_utente 
                     set dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END),cha_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN 1
                  ELSE 0
                     END),cha_vista_delegato = @isvistadelegato,
                     dta_accettata = GetDate(),
                     cha_accettata = '1',
                     cha_accettata_delegato = @isaccettatadelegato,var_note_acc = ' e smistato',
                     cha_in_todolist = '0',
                     cha_valida = '0'  
                     WHERE (
                     system_id = @idtrasmissioneutentemittente
                     OR system_id =(SELECT tu.system_id
                     FROM dpa_trasm_utente tu,
                                   dpa_trasmissione tx,
                                   dpa_trasm_singola ts
                     WHERE tu.id_people = @idpeoplemittente
                     AND tx.system_id = ts.id_trasmissione
                     AND tx.system_id = @idtrasmissione
                     AND ts.system_id = tu.id_trasm_singola
                     AND ts.cha_tipo_dest = 'U'))
                     AND cha_valida = '1'
                  END
               else --no workflow
                  BEGIN

-- = SYSDATE (),
-- = '1',
-- = isaccettatadelegato,
-- 'Documento accettato e smistato',
                     update dpa_trasm_utente set dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END),cha_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN 1
                  ELSE 0
                     END),cha_vista_delegato = @isvistadelegato,
                     cha_in_todolist = '0',cha_valida = '0'  
                     WHERE (system_id = @idtrasmissioneutentemittente
                     OR system_id =(SELECT tu.system_id
                     FROM dpa_trasm_utente tu,
                                   dpa_trasmissione tx,
                                   dpa_trasm_singola ts
                     WHERE tu.id_people = @idpeoplemittente
                     AND tx.system_id = ts.id_trasmissione
                     AND tx.system_id = @idtrasmissione
                     AND ts.system_id = tu.id_trasm_singola
                     AND ts.cha_tipo_dest = 'U'  
                     AND cha_valida = '1'))
                  END

               end
               IF (@Check_errore = 0)
               begin
                  update security  
                  set accessrights = @originalrights,
                  cha_tipo_diritto = 'T' 
                  FROM security s 
                  WHERE s.thing = @iddocumento
                  AND s.personorgroup 
                  IN(@idpeoplemittente,
                             @idgruppomittente)
                  AND s.accessrights = 20
                  SELECT   @Check_errore = @@ERROR
               end
            end
         end
      end
   END
ELSE
   BEGIN
      EXECUTE  [@db_user].spsetdatavistasmistamento @idpeoplemittente,@iddocumento,@idgruppomittente,'D',@idtrasmissione,@idpeopledelegato,
      @resultvalue
      IF (@resultvalue = 1)
      begin
         SET @returnvalue = -4
         RETURN
      end

   END


/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
     SELECT  TOP 1 @tipotrasmsingola = ts.cha_tipo_trasm
                  FROM dpa_trasm_singola ts, 
                  dpa_trasm_utente tu
                 WHERE ts.system_id = 
                 tu.id_trasm_singola
                   AND tu.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tx.system_id = 
                                               ts.id_trasmissione
                                         AND ts.system_id = tu.id_trasm_singola
                             AND tu.id_people = @idpeoplemittente
                             AND tx.system_id = @idtrasmissione
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                @idtrasmissioneutentemittente))
      ORDER BY cha_tipo_dest --FETCH FIRST 1 ROWS ONLY
   END

   IF @tipotrasmsingola = 'S' AND @trasmissioneconworkflow = '1'
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
   BEGIN
      update dpa_trasm_utente 
      set cha_valida = '0',
      cha_in_todolist = '0'  
      WHERE id_trasm_singola 
      IN(SELECT a.system_id
      FROM dpa_trasm_singola a, 
      dpa_trasm_utente b
      WHERE a.system_id = b.id_trasm_singola
      AND b.system_id IN(SELECT tu.system_id
         FROM dpa_trasm_utente tu,
                        dpa_trasmissione tx,
                        dpa_trasm_singola ts
         WHERE tu.id_people = @idpeoplemittente
         AND tx.system_id = ts.id_trasmissione
         AND tx.system_id = @idtrasmissione
         AND ts.system_id = tu.id_trasm_singola
         AND ts.system_id =(SELECT id_trasm_singola
            FROM dpa_trasm_utente
            WHERE system_id =
            @idtrasmissioneutentemittente)))
      AND system_id NOT IN(@idtrasmissioneutentemittente)
   END

END
GO

ALTER  PROCEDURE [@db_user].[SPsetDataVistaSmistamento]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@idTrasmissione INT,
@iddelegato  INT,
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)


BEGIN TRY

SET @resultValue = 0

DECLARE cursorTrasmSingolaDocumento CURSOR LOCAL FOR
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.system_id = @idTrasmissione and a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
AND a.ID_PROFILE = @idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID

IF(@tipoOggetto='D')
BEGIN
              
      OPEN cursorTrasmSingolaDocumento
      FETCH NEXT FROM cursorTrasmSingolaDocumento
      INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
      BEGIN
            WHILE @@FETCH_STATUS = 0
                  BEGIN
                        IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
                        -- SE  una trasmissione senza workFlow
                        BEGIN
                             IF (@iddelegato = 0)
                                   BEGIN
                                         -- nella trasmissione utente relativa all'utente che sta vedendo il documento setto la data di vista
                                         UPDATE DPA_TRASM_UTENTE
                                         SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                         DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                         DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                         WHERE
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011
                                         id_trasm_singola = @sysTrasmSingola
                                         and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                         
                                         IF (@@ERROR <> 0)
                                         BEGIN
                                               SET @resultValue=1
                                               return @resultValue
                                         END
                                   END
                             ELSE
                                   BEGIN
                                         --in caso di delega
                                         UPDATE DPA_TRASM_UTENTE
                                          SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                  dpa_trasm_utente.cha_vista_delegato = '1',
                                                  dpa_trasm_utente.id_people_delegato = @iddelegato,
                                         DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                         DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                         WHERE
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011 
                                         id_trasm_singola = @sysTrasmSingola
                                         and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
                                         
                                         IF (@@ERROR <> 0)
                                         BEGIN
                                               SET @resultValue=1
                                               return @resultValue
                                         END
                                   END
                        
                        
                             -- Impostazione data vista nella trasmissione in todolist
                             update      dpa_todolist
                             set   DTA_VISTA = getdate()
                             where id_trasm_singola = @sysTrasmSingola and 
                                   ID_PEOPLE_DEST = @idPeople AND
                                   id_profile = @idoggetto;

                             IF (@@ERROR <> 0)
                                   BEGIN
                                         SET @resultValue=1
                                         return @resultValue
                                   END
                        
                             IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
                                   BEGIN
                                         IF (@iddelegato = 0) 
                                               BEGIN
                                                     UPDATE DPA_TRASM_UTENTE SET
                                                     DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                     DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                     WHERE
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011         
                                                     id_trasm_singola = @sysTrasmSingola
                                                     AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
                  
                                                     IF (@@ERROR <> 0)
                                                           BEGIN
                                                                 SET @resultValue=1
                                                                 return @resultValue
                                                           END

                                               END
                                         ELSE
                                               BEGIN
                                                     UPDATE DPA_TRASM_UTENTE SET
                                                     DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                                     DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                                     DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
                                                     DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                                     WHERE
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011                     
                                                     id_trasm_singola = @sysTrasmSingola
                                                     AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
                  
                                                     IF (@@ERROR <> 0)
                                                           BEGIN
                                                                 SET @resultValue=1
                                                                 return @resultValue
                                                           END
                                                     END
                                               END
                                   END

                        ELSE

                        -- la ragione di trasmissione prevede workflow
                        BEGIN
                             IF (@iddelegato = 0) 
                                   BEGIN
                                         UPDATE DPA_TRASM_UTENTE
                                         SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                         DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                         DTA_ACCETTATA = (CASE WHEN DTA_ACCETTATA IS NULL THEN  GETDATE() ELSE DTA_ACCETTATA END),
                                         CHA_ACCETTATA = '1',
                                         CHA_VALIDA = '0'
                                         WHERE 
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011 
                                         id_trasm_singola = @sysTrasmSingola
                                         and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                   END
        -- Codice Remmato: abianchi/cferlito - 08/06/2011  -- ETT000000016597
        -- questo IF ((@@ERROR) genera problemi facendo entrare il cursore sia 
        -- nell IF di sopra sia nel ELSE di sotto (blocco delegato)
                                 
                                   --IF (@@ERROR <> 0)
                                   --      BEGIN
                                   --                              SET @resultValue=1
                                   --                              return @resultValue
                                   --                        END
            -- Codice Remmato: abianchi/cferlito - 08/06/2011  -- ETT000000016597
                                         
                             ELSE
                                   BEGIN
                                         UPDATE DPA_TRASM_UTENTE
                                         SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
                                         DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
                                         DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @iddelegato,
                                         DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
                                         DTA_ACCETTATA = (CASE WHEN DTA_ACCETTATA IS NULL THEN  GETDATE() ELSE DTA_ACCETTATA END),
                                         CHA_ACCETTATA = '1',
                                         CHA_ACCETTATA_DELEGATO = '1',
                                         CHA_VALIDA = '0'
                                         WHERE 
-- modifica del aprile 2011
--DPA_TRASM_UTENTE.DTA_VISTA IS NULL     AND
-- fine modifica del aprile 2011
                                          id_trasm_singola = @sysTrasmSingola
                                         and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
                                   END
                                   IF (@@ERROR <> 0)
                                         BEGIN
                                                                 SET @resultValue=1
                                                                 return @resultValue
                                                           END

                             -- Rimozione trasmissione da todolist solo se  stata gi  accettata o rifiutata
                                     UPDATE     dpa_trasm_utente
                                     SET        cha_in_todolist = '0'
                                     WHERE      id_trasm_singola = @sysTrasmSingola 
                                            AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                            AND (cha_accettata = '1' OR cha_rifiutata = '1');

                             IF (@@ERROR <> 0)
                                   BEGIN
                                         SET @resultValue=1
                                         return @resultValue
                                   END

                                     UPDATE dpa_todolist
                                        SET dta_vista = GETDATE()
                                      WHERE id_trasm_singola = @sysTrasmSingola
                                        AND id_people_dest = @idPeople
                                        AND id_profile = @idoggetto;

                             IF (@@ERROR <> 0)
                             BEGIN
                                   SET @resultValue=1
                                   return @resultValue
                             END

                             IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
                             begin
                                   UPDATE DPA_TRASM_UTENTE SET
                                   DPA_TRASM_UTENTE.CHA_VALIDA= '0',
                                   DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
                                   WHERE
                                   id_trasm_singola = @sysTrasmSingola
                                   AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople

                                   IF (@@ERROR <> 0)
                                         BEGIN
                                         SET @resultValue=1
                                         return @resultValue
                                   END
                             end
                        END

                        FETCH NEXT FROM cursorTrasmSingolaDocumento
                        INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
                  END
                  CLOSE cursorTrasmSingolaDocumento
                  DEALLOCATE cursorTrasmSingolaDocumento
            END
END      
RETURN @resultValue
END TRY
BEGIN CATCH
    -- Execute error retrieval routine.
    EXECUTE usp_GetErrorInfo;
END CATCH;


Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.14.1')
GO
