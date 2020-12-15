declare @db_user varchar

-- SETTARE CORRETTAMETE IL DB_USER
set @db_user = 'DOCSADM'


IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'['+@db_user+'].[DPA_VERIFICA_FORMATI_CONS]') AND type in (N'U'))

BEGIN
DECLARE @sql_scrip varchar(max)

set @sql_scrip='CREATE TABLE ['+@db_user+'].[DPA_VERIFICA_FORMATI_CONS]
					  (
					  SYSTEM_ID FLOAT IDENTITY(1,1) NOT NULL,                                
					  ID_ISTANZA FLOAT NOT NULL,                               
					  ID_ITEM FLOAT,                                           
					  DOCNUMBER FLOAT,                                         
					  ID_PROJECT FLOAT,
					  ID_DOCPRINCIPALE FLOAT,
					  VERSION_ID FLOAT,                                        
					  TIPO_FILE CHAR(1),  
					  ESTENSIONE VARCHAR(32),			  
					  CONSOLIDATO FLOAT
					  CONVERTIBILE FLOAT,                                      
					  MODIFICA FLOAT,                                          
					  UT_PROP FLOAT,                                           
					  RUOLO_PROP FLOAT,    
					  VALIDO FLOAT,
					  AMMESSO FLOAT,
					  FIRMATA FLOAT,
					  MARCATA FLOAT,
					  ERRORE FLOAT,
					  TIPOERRORE FLOAT,
					  DACONVERTIRE FLOAT,
					  CONVERTITO FLOAT,
					  ESITO VARCHAR(400),                                      
					  CONSTRAINT DPA_VERIFICA_FORMATI_CONS_PK PRIMARY KEY                          
					  (                                                         
						SYSTEM_ID                                               
					  )                                                         
           	)'
				             
execute @sql_scrip

END
GO

-- COLONNA DPA_AREA_CONSERVAZIONE
execute docsadm.utl_add_column
            @versioneCD = '3.30',
            @nome_utente = @db_user,
            @nome_tabella = 'DPA_AREA_CONSERVAZIONE',
            @nome_colonna = 'ESITO_VERIFICA',
            @tipo_dato = 'NUMBER',
            @val_default = NULL,
		    @condizione_modifica_pregresso = NULL,
		    @condizione_check = NULL,
		    @RFU = NULL

-- COLONNA policy_conservazione STATO_CONVERSIONE
execute docsadm.utl_add_column
            @versioneCD = '3.30',
            @nome_utente = @db_user,
            @nome_tabella = 'policy_conservazione',
            @nome_colonna = 'STATO_CONVERSIONE',
            @tipo_dato = 'VARCHAR(10)',
            @val_default = NULL,
		    @condizione_modifica_pregresso = NULL,
		    @condizione_check = NULL,
		    @RFU = NULL

-- Tabella di lookup per gli esiti di verifica
begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_ESITO_VERIFICA_CONS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_ESITO_VERIFICA_CONS
			(
			  SYSTEM_ID NUMBER NOT NULL,
			  VALORE VARCHAR2(200) NOT NULL,
			  CONSTRAINT DPA_ESITO_VERIFICA_CONS_PK PRIMARY KEY
			  (
				SYSTEM_ID
			  )
			  ENABLE
			)';
		end if;
	end;
end;
/

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'['+@db_user+'].[DPA_ESITO_VERIFICA_CONS]') AND type in (N'U'))

BEGIN
DECLARE @sql_scrip varchar(max)

set @sql_scrip='CREATE TABLE ['+@db_user+'].[DPA_ESITO_VERIFICA_CONS]
					  (
					  SYSTEM_ID FLOAT IDENTITY(1,1) NOT NULL,                                
					  VALORE VARCHAR(200) NOT NULL,
					  CONSTRAINT DPA_ESITO_VERIFICA_CONS_PK PRIMARY KEY
					  (
						SYSTEM_ID
					  )                                                    
           	)'
				             
execute @sql_scrip

END
GO


INSERT INTO [@db_user].[DPA_ESITO_VERIFICA_CONS] (VALORE) 
	VALUES ('Non Effettuata')
	VALUES ('Successo')
	VALUES ('Documenti direttamente convertibili dal proprietario')
	VALUES ('Documenti non convertibili direttamente dal proprietario')
	VALUES ('Fallita')
	VALUES ('Errore')
	


