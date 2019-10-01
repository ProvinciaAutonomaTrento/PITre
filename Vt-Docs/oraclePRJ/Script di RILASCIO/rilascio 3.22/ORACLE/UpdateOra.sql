-- file sql di update per il CD --
 
-------------------cartella  TABLE -------------------
              
---- ALTER_PROFILE.ORA.sql  marcatore per ricerca ----
--ALTER TABLE DOCSADM.PROFILE ADD	COD_EXT_APP varchar(32) NULL
begin
 @db_user.utl_add_column ('3.22', '@db_user'
 , 'PROFILE', 'COD_EXT_APP', 'varchar2(32)', NULL, NULL, NULL, NULL );
end;
/

              
----------- FINE -

---- ALTER_PROJECT.ORA.sql  marcatore per ricerca ----
--ALTER TABLE DOCSADM.PROJECT ADD	COD_EXT_APP varchar(32) NULL
begin
 @db_user.utl_add_column ('3.22', '@db_user'
 , 'PROJECT', 'COD_EXT_APP', 'varchar2(32)', NULL, NULL, NULL, NULL );
end;
/
              
----------- FINE -
              
---- CREATE_DPA_EXT_APPS.ORA.sql  marcatore per ricerca ----

begin
    declare   cnt int;
    begin        
		select count(*) into cnt 
			from all_tables where owner='@db_user' 
				AND table_name='DPA_EXT_APPS';
             if (cnt = 0) then
                    execute immediate   
                    'CREATE TABLE @db_user.DPA_EXT_APPS
						(SYSTEM_ID		int			NOT NULL,
						 VAR_CODE		varchar2(32)		NOT NULL,
						 DESCRIPTION	varchar2(512)	NOT NULL,
						constraint PK_DPA_EXT_APPS PRIMARY KEY 	(SYSTEM_ID) 	)';

			end if;
		end; 
end; 
/


              
----------- FINE -
              
---- CREATE_DPA_REL_PEOPLE_EXTAPPS.ORA.sql  marcatore per ricerca ----

begin
    declare   cnt int;
    begin        
select count(*) into cnt from all_tables where owner='@db_user' 
	AND table_name='DPA_REL_PEOPLE_EXTAPPS';
             if (cnt = 0) then
                    execute immediate   
                           'CREATE TABLE @db_user.DPA_REL_PEOPLE_EXTAPPS
					(ID_PEOPLE  int		NOT NULL,
					 ID_EXT_APP int		NOT NULL,
					 CONSTRAINT PK_DPA_REL_PEOPLE_EXTAPPS 
					  PRIMARY KEY 	(ID_PEOPLE,	ID_EXT_APP) )';

			end if;
	end;
end;
/

              
----------- FINE -
              
 
-------------------cartella  SEQUENCE -------------------
              
---- SEQ_DPA_EXT_APPS.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_EXT_APPS';
    IF (cnt = 0) THEN        
       execute immediate 
	   ' CREATE SEQUENCE @db_user.SEQ_DPA_EXT_APPS START WITH 1 INCREMENT BY 1 MINVALUE 1';
    END IF;
END;        
END;
/              
----------- FINE -
              
---- SEQ_DPA_REL_PEOPLE_EXTAPPS.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_REL_PEOPLE_EXTAPPS';
    IF (cnt = 0) THEN        
       execute immediate 
	   ' CREATE SEQUENCE @db_user.SEQ_DPA_REL_PEOPLE_EXTAPPS START WITH 1 INCREMENT BY 1 MINVALUE 1';
    END IF;
END;        
END;
/              
----------- FINE -

 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- INSERT_DPA_CHIAVI_CONFIGURAZIONE.ORA.SQL  marcatore per ricerca ----
BEGIN
	Utl_Insert_Chiave_Config('ENABLE_GEST_EXT_APPS'
	,'Abilita o disabilita la gestione delle applicazioni esterne'
	,'0','F','1','1','1','3.22',NULL,NULL,NULL);
end;
/



              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values    (seq.nextval, sysdate, '3.22');
end;
/              
