-- file sql di update per il CD --
-- SEQ_INSTALL_LOG  (Sequence)
BEGIN
    DECLARE cnt int;
     --cntdati int;
  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_INSTALL_LOG';
    --select max(nvl(system_id,0)) +1 into cntdati from dpa_chiavi_configurazione; 
    IF (cnt = 0) THEN        
          execute immediate ' CREATE SEQUENCE @db_user.SEQ_INSTALL_LOG
		START WITH 1 MINVALUE 1 MAXVALUE 99999999999 
		NOCYCLE   NOCACHE   NOORDER';
    END IF;
    END;
END;
/



---- CREATE_DPA_LOG_INSTALL.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  05/09/2011
Scopo della modifica:        CREARE LA TABELLA DPA_LOG_INSTALL
*/



BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_LOG_INSTALL
             (ID INTEGER  NOT NULL,
  DATA_OPERAZIONE    DATE                       NOT NULL,
  COMANDO_RICHIESTO  VARCHAR2(200 BYTE)         NOT NULL,
  VERSIONE_CD        VARCHAR2(200 BYTE)         NOT NULL,
  ESITO_OPERAZIONE   VARCHAR2(200 BYTE)         NOT NULL)';

      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/



              
---- utl_add_column.ORA.SQL  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_column('VERSIONE CD', --->es. 3.16.1
--                    'NOME UTENTE', --->es. DOCSADM
--                    'NOME TABELLA', --->es. DPA_LOG
--                    'NOME COLONNA', --->es. COLONNA_A
--                    'TIPO DATO', ---> es. INT 4, VARCHAR 200, ECC.
--                    'DEFAULT', --->es. 'VERO, FALSO, 0, ECC. MA SI PU LASCIARE VUOTO CON ''
--                    '',
--                    '',
--                    'RFU' ---> per uso futuro")
-- =============================================
-- Author:        Gabriele Serpi
-- Create date: 11/07/2011
-- Description:    
-- =============================================


CREATE OR REPLACE PROCEDURE @db_user.utl_add_column (
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   tipo_dato                        VARCHAR2,
   val_default                      VARCHAR2,
   condizione_modifica_pregresso    VARCHAR2,
   condizione_check                 VARCHAR2,
   RFU                              VARCHAR2)
IS
   cnt   INT;
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = UPPER(nomeutente);
    
     dbms_output.put_line ('nome_tabella '||nome_tabella);
     dbms_output.put_line ('nomeutente '||nomeutente);
    

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN
   dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND UPPER(owner) = nomeutente;
 dbms_output.put_line ('cnt vale: '||cnt);
      IF (cnt = 0)
      -- ok la colonna non esiste, la aggiungo
      THEN
         dbms_output.put_line ('ok entro nel 2 if ');
         EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' ADD '
                          || nome_colonna
                          || ' '
                          || tipo_dato;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
                         
-- ecco la modifica
        ELSE                         
              utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'colonna gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);
      END IF;
   END IF;
   
    dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                        -- RAISE;
               
END utl_add_column;
/              
---- utl_add_foreign_key.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_foreign_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_TABELLA_A
--					'NOME TABELLA FOREIGN KEY', --->es. TABELLA_B
--					'NOME COLONNA FOREIGN KEY', --->es. COLONNA_TABELLA_B
--					'CONDIZIONE ADD', ---> LASCIARE VUOTO CON ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 11/07/2011
-- Description:	
-- =============================================
-- NOME DEL FOREIGN KEY VIENE NOMINATO AUTOMATICAMENTE 
-- PRELEVANDO NOME TABELLA_A, TABELLA_B E COLONNA_A (FK_'TABELLA'_'COLONNA'_'TABELLAFK')


CREATE OR REPLACE PROCEDURE @db_user.utl_add_foreign_key
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                   VARCHAR2,
   nome_tabella_fk                 VARCHAR2,
   nome_colonna_fk                VARCHAR2,
   condizione_add                  VARCHAR2,
   RFU                                  VARCHAR2)
IS

      istruzione varchar2(2000); 
      cnt       INT;
      cntdati  INT; 
      nome_f VARCHAR2(200) := SUBSTR(nome_tabella,1,10);
      nome_cl_f VARCHAR2(200) := SUBSTR(nome_colonna,1,8);
      nome_tf VARCHAR2(200) := SUBSTR(nome_tabella_fk,1,8);
      nome_foreign_key VARCHAR2(2000) :='FK_'||nome_f||'_'||nome_cl_f||'_'||nome_tf||'';
     errore_msg varchar(255);

   BEGIN
     /*SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check    
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nome_tabella
         and cons.constraint_name = nome_foreign_key ; */
         
       istruzione :=' SELECT COUNT (*) FROM user_constraints
       WHERE constraint_name = '''||nome_foreign_key||'''
         AND constraint_type = ''R'' 
         and owner='''||nomeutente||'''';
   
     EXECUTE IMMEDIATE istruzione  INTO cnt ;      


      IF (cnt = 0)  
      -- ok il vincolo non esiste
      THEN
     istruzione :=     'SELECT COUNT (f.'||nome_colonna||')  
                    FROM '||nomeutente||'.'||nome_tabella||' f
                   WHERE f.'||nome_colonna||' IN 
                   (SELECT tf.'||nome_colonna_fk||'
                   FROM '||nomeutente||'.'||nome_tabella_fk||' tf)' ; 

     EXECUTE IMMEDIATE istruzione  INTO cntdati ;
    
     
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            istruzione :=  'ALTER TABLE '
                                            ||nomeutente||'.'||nome_tabella||' 
                                            ADD CONSTRAINT
                                            '||nome_foreign_key||' 
                                            FOREIGN KEY 
                                            ('||nome_colonna||')  
                                            REFERENCES
                                            '||nomeutente||'.'||nome_tabella_fk||'
                                            ('||nome_colonna_fk||') '||condizione_add||''; 
                                            
            dbms_output.put_line ('istruzione '||istruzione); 
            
            EXECUTE IMMEDIATE istruzione;
                                            
                                            utl_insert_log (nomeutente  -- nome utente
                                                         , NULL -- data
                                                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                                                         , versione_CD
                                                         , 'esito positivo' ); 

        ELSE                         
   
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Ci sono i records che violano la chiave' );
        

         END IF;
         ELSE                         
        
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave esterna gi esistente' );
      

         END IF;

      
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                errore_msg := SUBSTR(SQLERRM,1,255);
                
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_foreign_key;
/

              
---- utl_add_index.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_index('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME INDICE', --->es. IDX_TABELLA
--					'IS_UNIQUE', --->es. 1 (UNIQUE), '' (NON UNIQUE)
--					'NOME COLONNA 1', --->es. COLONNA_A (OBBLIGATORIO 1 COLONNA)
--					'NOME COLONNA 2', --->es. COLONNA_B (PUO' LASCIARE VUOTO '')
--					'NOME COLONNA 3', --->es. COLONNA_C (PUO' LASCIARE VUOTO '')
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 25/07/2011
-- Description:	
-- =============================================

CREATE OR REPLACE PROCEDURE @db_user.utl_add_index
(
 versione_CD VARCHAR2,
 nomeutente VARCHAR2,
 nome_tabella VARCHAR2,
 nome_indice VARCHAR2,
 is_index_unique VARCHAR2,
 nome_colonna1 VARCHAR2,
 nome_colonna2 VARCHAR2,
 nome_colonna3 VARCHAR2,
 RFU VARCHAR2)
IS

 istruzione varchar2(2000); 
 cnt INT;
 cntdati INT; 
 colonne VARCHAR2(2000);
 is_unique VARCHAR(2000);
 errore_msg varchar(255);
 
 
 BEGIN
 
 SELECT COUNT (*) INTO cnt
 FROM all_indexes
 WHERE index_name = upper(nome_indice)
 and owner=nomeutente;
 
 IF (cnt = 0) then
 -- ok il vincolo non esiste
 
 ----------------se nella condizione "is_index_unique" viene impostato 0 per pi di una sola colonna inserita 
 ----------------se la terza colonna viene inserita
 IF nome_colonna3 IS NOT null AND is_index_unique = 0 THEN

 colonne := ''||nome_colonna1|| ' ASC,
 '||nome_colonna2|| ' ASC,
 '||nome_colonna3|| ' ASC';
 is_unique :='';
 
 ----------------se la seconda colonna viene inserita
 elsif nome_colonna2 is not null AND is_index_unique = 0 THEN

 colonne := ''||nome_colonna1||' ASC,
 '||nome_colonna2|| ' ASC';
 is_unique :='';
 else
 ----------------se solo la prima colonna viene inserita e sar CREATE UNIQUE 
 colonne := ''||nome_colonna1|| ' ASC';
 is_unique :='UNIQUE';
END IF;

 
 istruzione := 'CREATE '||is_unique||' INDEX '
 ||nomeutente||'
 .'||nome_indice||' 
 ON
 '||nome_tabella||' 
 ('||colonne||') '; 
 
 dbms_output.put_line ('istruzione '||istruzione); 
 
 EXECUTE IMMEDIATE istruzione;
 
 utl_insert_log (nomeutente -- nome utente
 , NULL -- data
 ,'Added index '||nome_indice||' on table '||nome_tabella
 , versione_CD
 , 'esito positivo' ); 

 ELSE 
 dbms_output.put_line ('errore, esiste gi '); 
 utl_insert_log (nomeutente -- nome utente
 , NULL -- data
 ,'Adding index '||nome_indice||' on table '||nome_tabella
 , versione_CD
 , 'Indice gi esistente' );
 dbms_output.put_line ('cln esiste: '||sqlerrm);

 END IF;
 -- END IF;
 
 dbms_output.put_line ('sqlerrm: '||sqlerrm);
 
EXCEPTION -- qui cominciano le istruzioni da eseguire se le SP va in errore
 WHEN OTHERS
 THEN
 -- Consider logging the error and then re-raise
 
 dbms_output.put_line ('errore '||SQLERRM);
 dbms_output.put_line ('non eseguito ');
 errore_msg := SUBSTR(SQLERRM,1,255);
 
 utl_insert_log (nomeutente -- nome utente
 , NULL -- data
 ,'Adding index '||nome_indice||' on table '||nome_tabella
 , versione_CD
 , 'esito negativo - '||errore_msg||'' ); 
END utl_add_index;
/

              
---- utl_add_primary_key.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_primary_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
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

CREATE OR REPLACE PROCEDURE @db_user.utl_add_primary_key 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                   VARCHAR2,
   condizione_add                  VARCHAR2,
   RFU                                  VARCHAR2)
IS

      istruzione varchar2(2000); 
      cnt       INT;
      cntdati  INT; 
      nome_primary_key             VARCHAR2(2000) :='PK_'||nome_tabella;
      errore_msg varchar(255);
      
   BEGIN
     SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check    
         and cons.constraint_type  = 'P' 
         and cons.owner            = nomeutente
         and cons.table_name       = nome_tabella
         and cons_cols.COLUMN_NAME = nome_colonna ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
     istruzione :=     'SELECT COUNT (*) 
                    FROM '||nomeutente||'.'||nome_tabella||'
                    group by '||nome_colonna||'
                    having count(*) > 1
                    UNION
                    select 0 from dual' ; 
     
     dbms_output.put_line ('istruzione per cnt dati '||istruzione); 
    
            
     EXECUTE IMMEDIATE istruzione                    INTO cntdati ;
       dbms_output.put_line ('cntdati: '||cntdati); 
       dbms_output.put_line ('arriviamo dopo questa ? cntdati: '||cntdati);  
     
     
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            istruzione :=  'ALTER TABLE '
                                            ||nomeutente||'
                                            .'||nome_tabella||' 
                                            ADD CONSTRAINT
                                            '||nome_primary_key||' 
                                            PRIMARY KEY 
                                            ('||nome_colonna||') '; 
            dbms_output.put_line ('istruzione '||istruzione); 
            
            EXECUTE IMMEDIATE istruzione;
                                            
                                            utl_insert_log (nomeutente  -- nome utente
                                                         , NULL -- data
                                                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                                                         , versione_CD
                                                         , 'esito positivo' ); 

        ELSE                         
         dbms_output.put_line ('errore, esiste gi ');    
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave primaria gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);

         END IF;
          ELSE                         
         dbms_output.put_line ('errore, esiste gi ');    
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave primaria gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);
      END IF;
      
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_primary_key;
/

              
---- utl_drop_column.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_drop_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_A
--					'CONDIZIONE DROP', --->es. '0' CANCELLA COLONNA SOLO SE E' VUOTO, '1' CANCELLA ANCHE SE NON VUOTO!!
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 18/07/2011
-- Description:	
-- =============================================
CREATE OR REPLACE PROCEDURE @db_user.utl_drop_column    
(
   versione_CD                      VARCHAR2,
   nomeutente VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   condizione_drop                   VARCHAR2,
   RFU                                      VARCHAR2)
IS
   cnt   INT; -- per vedere se esiste
   ONLYIF_COLUMN_IS_EMPTY INT; -- per vedere se  vuota
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;  

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN

 --SELECT COUNT ( nome_colonna )     INTO cntdati     FROM table_name = UPPER (nome_tabella) AND owner = nomeutente;
     
     EXECUTE IMMEDIATE 'SELECT COUNT ( '|| nome_colonna|| ' )     FROM  '|| nome_tabella INTO ONLYIF_COLUMN_IS_EMPTY;  

     IF ((condizione_drop = 0 AND ONLYIF_COLUMN_IS_EMPTY = 0) OR condizione_drop = 1) 
     THEN
           EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' DROP COLUMN '
                          || nome_colonna;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Droped column '||nome_colonna||' from table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 

   END IF;
   END IF;
   
   
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
EXCEPTION

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
                 dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Dropping column '||nome_colonna||' from table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_drop_column  ;
/

              
---- utl_INSERT_LOG.ORA.SQL  marcatore per ricerca ----
-- RISERVATO SOLO NELLE ALTRE PROCEDURE (ADD_COLUMN, FK, IDX, DROP COLUMN, RENAME_COLUMN)

CREATE OR REPLACE PROCEDURE @db_user.utl_insert_log (nomeutente VARCHAR2,
 data_eseguito DATE,
 comando_eseguito VARCHAR2,
 versione_CD VARCHAR2,
 esito VARCHAR2)
IS
 sqlcmd VARCHAR2 (2000);
BEGIN
 -- caccia alle virgolette!!
 
 sqlcmd :=
 'INSERT INTO DPA_LOG_INSTALL VALUES ( SEQ_INSTALL_LOG.nextval, 
 to_date('''||TO_CHAR (NVL (data_eseguito, SYSTIMESTAMP), 'dd/mm/yyyy hh24:mi:ss')||''',''dd/mm/yyyy hh24:mi:ss'' )'
 || ', '''
 || comando_eseguito
 || ''', '''
 || versione_CD
 || ''','''
 || esito
 || ''')';
 DBMS_OUTPUT.put_line ('eseguito: ' || sqlcmd);

 EXECUTE IMMEDIATE sqlcmd;
EXCEPTION
 WHEN OTHERS
 THEN
 DBMS_OUTPUT.put_line ('errore da insert' || SQLERRM);
 RAISE; --manda errore a sp chiamante
END utl_insert_log;
/

              
---- utl_modify_column.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_modify_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
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

CREATE OR REPLACE PROCEDURE @db_user.utl_modify_column 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   tipo_dato                        VARCHAR2,
   val_default                      VARCHAR2,
   condizione_check                 VARCHAR2,
   RFU                              VARCHAR2)
IS
   cnt   INT;
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN
   dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND owner = nomeutente;
 dbms_output.put_line ('cnt vale: '||cnt);
      IF (cnt = 1)
      -- ok la colonna esiste, la modifico
      THEN
         dbms_output.put_line ('ok entro nel 2 if ');
         EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' MODIFY '
                          || nome_colonna
                          || ' '
                          || tipo_dato;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
      END IF;
   END IF;
   
    dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
    
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modifying column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                    --   RAISE;
END utl_modify_column;
/

              
---- utl_rename_column.ORA.SQL  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.utl_rename_column 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna_old            VARCHAR2,
   nome_colonna_new           VARCHAR2,
   RFU                                   VARCHAR2)
IS
   cnt   INT;
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN
   dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna_old)
             AND owner = nomeutente;
 dbms_output.put_line ('cnt vale: '||cnt);
      IF (cnt = 1)
      -- ok la colonna esiste, la modifico
      THEN
         dbms_output.put_line ('ok entro nel 2 if ');
         EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' RENAME COLUMN '
                          || nome_colonna_old
                          || ' TO '
                          || nome_colonna_new;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
      END IF;
   END IF;
   
    dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renaming column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_rename_column;
/              
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_CORR_GLOBALI.ORA.SQL  marcatore per ricerca ----
BEGIN
 @db_user.utl_add_column ('3.17.1', '@db_user'
 , 'DPA_CORR_GLOBALI', 'VAR_ORIGINAL_CODE', 'VARCHAR(128)', NULL, NULL, NULL, NULL);

 @db_user.utl_add_column ('3.17.1', '@db_user'
 , 'DPA_CORR_GLOBALI', 'ORIGINAL_ID', 'NUMBER(10,0)', NULL, NULL, NULL, NULL);
END; 
/



              
----------- FINE -
              
---- ALTER_PEOPLE.ORA.sql  marcatore per ricerca ----
BEGIN
-- alter table people add ABILITATO_CENTRO_SERVIZI       CHAR(1 BYTE)
@db_user.utl_add_column ('3.17', '@db_user'
, 'people', 'ABILITATO_CENTRO_SERVIZI', 'CHAR(1)','','','','');
END; 
/
              
----------- FINE -
              
---- ALTER_PROFILE.ORA.SQL  marcatore per ricerca ----
BEGIN
-- AGGIUNGE la colonna CHA_COD_T_A nella tabella PROFILE
 @db_user.utl_add_column ('3.17', '@db_user'
, 'PROFILE', 'CHA_COD_T_A', 'CHAR(1)','','','','');

END; 
/

              
----------- FINE -
              
---- ALTER_PROJECT.ORA.SQL  marcatore per ricerca ----
BEGIN
-- AGGIUNGE la colonna CHA_COD_T_A nella tabella PROJECT
 @db_user.utl_add_column ('3.17', '@db_user'
, 'PROJECT', 'CHA_COD_T_A', 'CHAR(1)','','','','');
END; 
/

              
----------- FINE -
              
---- ALTER_SECURITY.ORA.SQL  marcatore per ricerca ----
BEGIN
--Aggiunta campo VAR_NOTE_SEC nella SECURITY
 @db_user.utl_add_column ('3.17', '@db_user'
,'SECURITY'	,'VAR_NOTE_SEC', 'varchar2(512)', NULL, NULL, NULL, NULL); 

END; 
/
              
----------- FINE -
              
---- CREATE_DPA_LOG_INSTALL.ORA.sql  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM all_sequences 
        where sequence_name='SEQ_INSTALL_LOG';
    IF (cnt = 0) THEN        execute immediate 
'CREATE SEQUENCE @db_user.SEQ_INSTALL_LOG START WITH 1 MINVALUE 1 MAXVALUE 99999999999 NOCYCLE NOCACHE NOORDER';
    END IF;
    END;
END;
/

BEGIN
   DECLARE istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_LOG_INSTALL
             (ID INTEGER  NOT NULL,
			  DATA_OPERAZIONE    DATE                       NOT NULL,
			  COMANDO_RICHIESTO  VARCHAR2(200 BYTE)         NOT NULL,
			  VERSIONE_CD        VARCHAR2(200 BYTE)         NOT NULL,
			  ESITO_OPERAZIONE   VARCHAR2(200 BYTE)         NOT NULL)';

      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/
              
----------- FINE -
              
---- CREATE_DPA_OBJECTS_SYNC_PENDING.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_OBJECTS_SYNC_PENDING
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables where owner=UPPER('@db_user') and table_name='DPA_OBJECTS_SYNC_PENDING';
		if (cnt = 0) then
		  execute immediate	
		'CREATE TABLE @db_user.DPA_OBJECTS_SYNC_PENDING ' ||
		'( ' ||
		'ID_DOC_OR_FASC NUMBER, ' ||
		'TYPE CHAR(1), ' ||
		'ID_GRUPPO_TO_SYNC NUMBER ' ||
		') ' ;
		end if;
	end;	
end;	
/              
----------- FINE -
              
---- CREATE_DPA_ROLE_HISTORY.ORA.SQL  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  15/11/2011
Scopo della modifica:        CREARE LA TABELLA DPA_ROLE_HISTORY
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'CREATE TABLE @db_user.DPA_ROLE_HISTORY
  (
    System_Id Number Not Null Enable Primary Key,
    ORIGINAL_CORR_ID NUMBER NOT NULL ENABLE,
    ACTION           CHAR(1 BYTE) NOT NULL ENABLE,
    ROLE_DESCRIPTION VARCHAR2(384 BYTE) NOT NULL ENABLE,
    ROLE_TYPE_ID     NUMBER NOT NULL ENABLE,
    Action_Date Date Not Null Enable,
    Uo_Id Number(*,0) Not Null Enable,
    UO_DESCRIPTION_ VARCHAR2(4000 BYTE),
    ROLE_TYPE_DESCRIPTION_ VARCHAR2(4000 BYTE),
   ROLE_ID	NUMBER NOT NULL ENABLE
    
  )';

      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/
              
----------- FINE -
              
---- CREATE_DPA_VIS_ANOMALA.ORA.sql  marcatore per ricerca ----
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'CREATE GLOBAL TEMPORARY TABLE @db_user.DPA_VIS_ANOMALA
			(  ID_GRUPPO  NUMBER )
			ON COMMIT DELETE ROWS ';

      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/


/

              
----------- FINE -
              
 
-------------------cartella  SEQUENCE -------------------
              
---- SEQ_INSTALL_LOG.ORA.sql  marcatore per ricerca ----
-- SEQ_INSTALL_LOG  (Sequence) 
BEGIN
    DECLARE cnt int;
     --cntdati int;
  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_INSTALL_LOG';
    --select max(nvl(system_id,0)) +1 into cntdati from dpa_chiavi_configurazione; 
    IF (cnt = 0) THEN        
          execute immediate ' CREATE SEQUENCE @db_user.SEQ_INSTALL_LOG
		START WITH 1 MINVALUE 1 MAXVALUE 99999999999 
		NOCYCLE   NOCACHE   NOORDER';
    END IF;
    END;
END;
/

              
----------- FINE -
              
---- SEQROLEHISTORY.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
     --cntdati int;
  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQROLEHISTORY';
    --select max(nvl(system_id,0)) +1 into cntdati from dpa_chiavi_configurazione; 
    IF (cnt = 0) THEN        
          execute immediate ' CREATE SEQUENCE @db_user.SEQROLEHISTORY 
			MINVALUE 1 MAXVALUE 9999999999999999999999999999 
			INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER NOCYCLE ';
    END IF;
    END;
END;
/

              
----------- FINE -
              
 
-------------------cartella  TRIGGER -------------------
              
---- ROLEHISTORYCREATE.ORA.SQL  marcatore per ricerca ----
create or replace TRIGGER @db_user.ROLEHISTORYCREATE 
BEFORE INSERT ON dpa_corr_globali 
FOR EACH ROW 
WHEN (new.cha_tipo_urp = 'R' and new.original_id = new.system_id) 
DECLARE
    uoDescription varchar (256);
    uoCode varchar (128);
    roleTypeDescription varchar (64);
    roleTypeCode varchar (16);
BEGIN
  /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      ROLEHISTORYCREATE

    PURPOSE:   All'inserimento di un ruolo viene inserita una riga nella
               tabella dello storico
 
  ******************************************************************************/
  
  -- Inserimento di una tupla nella tabella della storia del ruolo
  INSERT
  INTO DPA_ROLE_HISTORY
    (
      SYSTEM_ID ,
      ACTION_DATE ,
      UO_ID ,
      ROLE_TYPE_ID ,
      ORIGINAL_CORR_ID ,
      ACTION ,
      ROLE_DESCRIPTION,
      ROLE_ID
    )
    VALUES
    (
      seqrolehistory.nextval,
      sysdate,
      :new.id_uo,
      :new.id_tipo_ruolo,
      :new.original_id,
      'C',
      :new.var_desc_corr || ' (' || :new.var_codice || ')',
      :new.system_id
    );
END;
/
              
----------- FINE -
              
---- ROLEHISTORYDELETE.ORA.SQL  marcatore per ricerca ----
create or replace TRIGGER @db_user.RoleHistoryDelete
   BEFORE DELETE
   ON dpa_corr_globali
   REFERENCING NEW AS NEW OLD AS OLD
   FOR EACH ROW
   WHEN (OLD.cha_tipo_urp = 'R')
BEGIN
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      RoleHistoryDelete

    PURPOSE:   Ogni volta che viene eliminato un record dalla dpa_corr_globali
               vengono cancellati dallo storico tutti i record relativi alla 
               storia del ruolo eliminati
 
  ******************************************************************************/

   DELETE FROM dpa_role_hisTory 
         WHERE original_corr_id = :OLD.original_id;
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
END;
/              
----------- FINE -
              
---- ROLEHISTORYMODIFY.ORA.SQL  marcatore per ricerca ----
create or replace TRIGGER @db_user.ROLEHISTORYMODIFY 
BEFORE UPDATE ON DPA_CORR_GLOBALI 
FOR EACH ROW 
WHEN (new.cha_tipo_urp = 'R' and
        new.dta_fine is null and
        ( new.id_old != old.id_old or 
          new.var_codice != old.var_codice or 
          new.var_desc_corr != old.var_desc_corr or
          new.id_uo != old.id_uo or
          new.id_tipo_ruolo != old.id_tipo_ruolo)) 
DECLARE
  idLastInsert integer;
BEGIN
  /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      ROLEHISTORYMODIFY

    PURPOSE:   Ogni volta che viene modificato un record nella dpa_corr_globali,
               se il record  relativo ad un Ruolo, se non  stata inserita
               la dta_fine e se  stato modificato almeno uno dei campi 
               monitorati, viene inserita una riga di tipo M nella tabella dello
               storico. Se invece  stato impostato l'id_old, significa che 
               stato storicizzato un ruolo, quindi viene inserito un record di
               tipo S nella tabella dello storico
 
  ******************************************************************************/

  -- Verifica eventuale cambiamento su codice del ruolo
  if :old.id_old != :new.id_old then
    -- Nella dpa_role_history bisogna preventivamente aggiornare il campo
    -- role_id con il nuovo system_id ed in seguito inserire una nuova riga
    -- con id_old uguale a quella del record appena inserito
    UPDATE dpa_role_history
    SET role_id = :new.id_old
    WHERE role_id = :old.system_id;
    
    -- Il ruolo id_old  stato storicizzato (inserimento di un record 
    -- di storicizzazione)
    INSERT
      INTO DPA_ROLE_HISTORY
        (
          SYSTEM_ID ,
          ACTION_DATE ,
          UO_ID ,
          ROLE_TYPE_ID ,
          ORIGINAL_CORR_ID ,
          ACTION ,
          ROLE_DESCRIPTION,
          ROLE_ID
        )
        VALUES
        (
          seqrolehistory.nextval,
          sysdate,
          :new.id_uo,
          :new.id_tipo_ruolo,
          :new.original_id,
          'S',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  else
    INSERT
      INTO DPA_ROLE_HISTORY
        (
          SYSTEM_ID ,
          ACTION_DATE ,
          UO_ID ,
          ROLE_TYPE_ID ,
          ORIGINAL_CORR_ID ,
          ACTION ,
          ROLE_DESCRIPTION,
          ROLE_ID
        )
        VALUES
        (
          seqrolehistory.nextval,
          sysdate,
          :new.id_uo,
          :new.id_tipo_ruolo,
          :new.original_id,
          'M',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  end if;

END;
/
              
----------- FINE -
              

-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- dpa_anagrafica_log.ORA.sql  marcatore per ricerca ----
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l 
	where l.var_codice ='DOCUMENTOSPEDISCI';
    if (cnt = 0) then       
		insert into dpa_anagrafica_log(system_id, var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values (seq.nextval, 'DOCUMENTOSPEDISCI', 'Spedizione documento'
		, 'DOCUMENTO', 'DOCUMENTOSPEDISCI') ; 
    end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l 
	where l.var_codice ='RUB_IMP_NEWCORR_WS';
    if (cnt = 0) then       
		insert into dpa_anagrafica_log(system_id, var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values (SEQ.nextval, 'RUB_IMP_NEWCORR_WS', 
		'WS rubrica, creazione nuovo corrispondente', 
		'RUBRICA', 'IMPORTARUBRICACREAWS');
    end if;
  end;
end;
/

declare 

c_seq number;

c_seq2 number;

cnt1 int;

cnt2 int;


begin


select @db_user.SEQ.nextval into c_seq from dual;

select @db_user.SEQ.nextval into c_seq2 from dual;

 
 begin
select count(*) into cnt1 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGIN';

if (cnt1 = 0) then
Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq, 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN');

Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq, 0);


 end if;
 end;
 
  begin
select count(*) into cnt2 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGOFF';

if (cnt2 = 0) then

Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq2, 'AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF');
Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq2, 0);

end if;
end;
 
END;
/              
----------- FINE -
              
---- ins_DPA_CHIAVI_CONFIGURAZIONE.ORA.SQL  marcatore per ricerca ----
--Creazione chiave di configurazione 
begin
declare cnt int;
begin
select count(*) into cnt from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='ATIPICITA_DOC_FASC';

		if (cnt = 0) then
			insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
			(system_id,id_amm,var_codice
			,			var_descrizione,
			var_valore,cha_tipo_chiave			,cha_visibile
			,cha_modificabile,cha_globale,var_codice_old_webconfig)
			values
			(SEQ_DPA_CHIAVI_CONFIG.nextval,0
			,'ATIPICITA_DOC_FASC',
			'La chiave abilita o meno la verifica di atipicita di un documento o un fascicolo',
			'0','B',			'0', -- richiesta Luciani (non visibile) -- prev was '1'
			'1','1',null);
		else -- se presente, disabilita e rendi non visibile
			update @db_user.DPA_CHIAVI_CONFIGURAZIONE
			set var_valore = '0', cha_visibile = '0'
			where var_codice ='ATIPICITA_DOC_FASC' ; 
		end if;
end;
end;
/


begin
declare cnt int;
begin
select count(*) into cnt from @db_user.DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='GESTIONE_REPERTORI';

if (cnt = 0) then
Insert into DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, ID_AMM, VAR_CODICE
   , VAR_DESCRIZIONE, VAR_VALORE
   , CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
 Values
   (seq_DPA_CHIAVI_CONFIG.nextval, 0, 'GESTIONE_REPERTORI'
   , 'La chiave abilita o meno la gestione dei repertori', '1'
   , 'B'            , '1'           , '1'           , '1');
end if;
end;
end;
/  


-- BE_RIC_MITT_INTEROP_BY_MAIL_DESC
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC';
    IF (cnt = 0) THEN       
		insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
		   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
		   ,VAR_VALORE
		   ,CHA_TIPO_CHIAVE,CHA_VISIBILE
		   ,CHA_MODIFICABILE,CHA_GLOBALE)
				 values
		  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,
		'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
		  ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL ANZICHE SOLO MAIL. VALORI POSSIBILI 0 o 1'
		  ,'0' -- SEGNALAZIONE luned 19/03/2012 12:50 di Palumbo Ch. 
		  ,'B','1'
		  ,'1','1');
    END IF;

	IF (cnt = 1) THEN       
  		update @db_user.DPA_CHIAVI_CONFIGURAZIONE
		set VAR_DESCRIZIONE = 'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL (valore 1) ANZICHE SOLO MAIL (valore 0)'
		where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC' ; 
	END IF;
  
  

    END;
END;
/              
----------- FINE -
              
---- UPDATE_DPA_CORR_GLOBALI.ORA.SQL  marcatore per ricerca ----
BEGIN
DECLARE cnt INT;
BEGIN
	SELECT COUNT ( * )        INTO cnt         
		FROM all_tab_columns
		WHERE table_name = UPPER ('DPA_CORR_GLOBALI')
             AND column_name = UPPER ('ORIGINAL_ID')
             AND UPPER(owner) = UPPER('@db_user');

    IF (cnt = 1) THEN  -- ok la colonna esiste, procedo con update
      update @db_user.dpa_corr_globali 
				set original_id = system_id
				where cha_tipo_urp = 'R'
				and dta_fine is null
				and original_id is null ; -- questo filtro permette di rilanciare l'update pi volte 
	END IF;

	SELECT COUNT ( * )        INTO cnt         
		FROM all_tab_columns
		WHERE table_name = UPPER ('DPA_CORR_GLOBALI')
             AND column_name = UPPER ('VAR_ORIGINAL_CODE')
             AND UPPER(owner) = UPPER('@db_user');

    IF (cnt = 1) THEN  -- ok la colonna esiste, procedo con update
       update @db_user.dpa_corr_globali 
				set VAR_ORIGINAL_CODE = var_cod_rubrica
				where cha_tipo_urp = 'R'
				and dta_fine is null
				and VAR_ORIGINAL_CODE is null; -- questo filtro permette di rilanciare l'update pi volte 
	END IF;


END;
END;
/


              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- GETCORRDESCRIPTION.ORA.SQL  marcatore per ricerca ----
create or replace FUNCTION @db_user.GETCORRDESCRIPTION 
	(  idCorrGlobali integer)
RETURN VARCHAR2 deterministic AS 
  code VARCHAR (128);
  description VARCHAR (256);
BEGIN
    /******************************************************************************
	AUTHOR:    Samuele Furnari
	NAME:      GETCORRDESCRIPTION
	PURPOSE:   Funzione per la costruzione 
				della descrizione completa di un ruolo
   ******************************************************************************/
  
select var_codice, var_desc_corr into code, description 
  from dpa_corr_globali 
  where system_id = idCorrGlobali;
  
  if(code is not null and description is not null) then
    RETURN description || ' (' || code || ')';
  else     RETURN '';
  end if;  
  
END GETCORRDESCRIPTION;
/

              
----------- FINE -
              
---- GETROLETYPEDESCRIPTION.ORA.SQL  marcatore per ricerca ----
create or replace FUNCTION @db_user.GETROLETYPEDESCRIPTION  (
  roleTypeId integer )
RETURN VARCHAR2 deterministic AS 
  code VARCHAR (128);
  description VARCHAR (256);
BEGIN

  
    /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      GETCORRDESCRIPTION

    PURPOSE:   Funzione per la costruzione della descrizione completa di un tipo ruolo
 
  ******************************************************************************/



  select var_codice, var_desc_ruolo into code, description from dpa_tipo_ruolo where system_id = roleTypeId;
  
  if(description is not null and code is not null) then
    RETURN description || ' (' || code || ')';
  else
    RETURN '';
  end if;
  
END GETROLETYPEDESCRIPTION;
/


              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- CalculateAtipDelRole.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.CalculateAtipDelRole(
  -- Id corr globali della UO cui apparteneva il ruolo eliminato
  idUO        In  Number,
  -- Id dell'amministrazione cui appartiene la UO
  idAmm       In  Number,
  -- Id del livello del ruolo eliminato
  roleLevelId   In  Number,
  returnValue Out Number
)
AS BEGIN
    /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     CalculateAtipDelRole

    PURPOSE:  Store per il calcolo dell'atipicit di documenti e fascicoli di un ruolo
              e dei suoi sottoposti. Questo calcolo viene eseguito quando si elimina
              un ruolo

  ******************************************************************************/
  
  -- Id del tipo ruolo del ruolo eliminato
  Declare roleLevel Number;
  keyValue varchar (200) := '';
  
  -- Query per l'estrazione degli id degli oggetti visti dai ruoli presenti nella
  -- UO del ruolo eliminato che abbiano livello inferiore o uguale a quello
  -- del ruolo elminato
  rolesObjects varchar (2000);
  
  Begin
    -- Selezione del livello ruolo
    Select num_livello Into roleLevel From dpa_tipo_ruolo Where system_id = roleLevelId;
    
    rolesObjects := 
      'Select distinct(s.thing) 
      From security s 
      Inner Join profile p
      On p.system_id = s.thing
      Where personorgroup In (
        Select distinct(p.id_gruppo) 
        From dpa_corr_globali p
        Where id_amm = ' || idAmm || '
        And ((Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || roleLevel || ')
        And p.id_uo In (
          Select p.SYSTEM_ID 
          From dpa_corr_globali p
          Start With p.SYSTEM_ID =' || idUO || '
          Connect By Prior
          p.SYSTEM_ID = p.ID_PARENT 
          And p.CHA_TIPO_URP = ''U'' 
          And p.ID_AMM = ' || idAmm || '))';
        
    -- Esecuzione calcolo di atipicit su documenti e fascicoli solo se attiva
     -- Recupero dello stato di abilitazione del calcolo di atipicit
    SELECT var_valore INTO keyValue FROM dpa_chiavi_configurazione WHERE var_codice = 'ATIPICITA_DOC_FASC' AND (id_amm = 0 OR id_amm = idAmm) AND ROWNUM = 1;
    
    If keyValue = '1' Then
      Begin
        vis_doc_anomala_custom(idAmm, rolesObjects);
        vis_fasc_anomala_custom(idamm, rolesObjects);
      End;  
    End If;  
    
    returnValue := 1;
  End;  
END CalculateAtipDelRole;
/

              
----------- FINE -
              
---- COMPUTEATIPICITAINSROLE.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.COMPUTEATIPICITAINSROLE 
(
  -- Id dell'amministrazione
  IdAmm IN NUMBER  
  -- Id della uo in cui  stato inserito il ruolo
, idUo IN NUMBER  
, returnValue OUT NUMBER  
) AS 
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     COMPUTEATIPICITAINSROLE

    PURPOSE:  Store per il calcolo dell'atipicit di documenti e fascicoli eseguita
              nel momento in cui viene inserito un ruolo

  ******************************************************************************/

  BEGIN
    -- Calcolo dell'atipicit sui documenti
    vis_doc_anomala_custom(idamm, 'select distinct(s.thing) 
                                    from security s 
                                    inner join profile p
                                    on p.system_id = s.thing
                                    where personorgroup in (
                                      select distinct(p.id_gruppo) 
                                      from dpa_corr_globali p
                                      where id_amm = ' || idAmm || ' AND 
                                      p.id_uo in (
                                        select p.SYSTEM_ID 
                                        from dpa_corr_globali p
                                        start with p.SYSTEM_ID = ' ||
                                        idUo || ' connect by prior
                                        p.SYSTEM_ID = p.ID_PARENT AND 
                                        p.CHA_TIPO_URP = ''U'' AND 
                                        p.ID_AMM = ' || idAmm || '))');
    -- Calcolo dell'atipicit sui fascicoli
    vis_fasc_anomala_custom(idAmm, 'select distinct(s.thing) 
                                    from security s 
                                    inner join project p
                                    on p.system_id = s.thing
                                    where personorgroup in (
                                      select distinct(p.id_gruppo) 
                                      from dpa_corr_globali p
                                      where id_amm = ' || idAmm || ' AND 
                                      p.id_uo in (
                                        select p.SYSTEM_ID 
                                        from dpa_corr_globali p
                                        start with p.SYSTEM_ID = ' ||
                                        idUo || ' connect by prior
                                        p.SYSTEM_ID = p.ID_PARENT AND 
                                        p.CHA_TIPO_URP = ''U'' AND 
                                        p.ID_AMM = ' || idAmm || ')
                                        and p.cha_tipo_fascicolo = ''P'')');
  END;
  
  returnvalue := 0;
  
  END COMPUTEATIPICITAINSROLE;
/

              
----------- FINE -
              
---- COMPUTEATIPICITA.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.COMPUTEATIPICITA  (
  -- Id della UO in cui  inserito il ruolo
  idUo  IN  NUMBER,
  -- Id del gruppo per cui calcolare l'atipicit
  IdGroup IN VARCHAR2  
  -- Id dell'ammionistrazione
, Idamm In Varchar2
  -- Id del tipo ruolo
, idTipoRuolo Number
  -- Id del veccho tipo ruolo
, idTipoRuoloVecchio Number
  -- Id della vecchia UO
, idVecchiaUo Number
  -- 1 se  stato richiesto di calcolare l'atipicit sui sottoposti
, calcolaSuiSottoposti Number
  -- Valore restitua
, returnValue OUT integer
) AS 
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
  Declare oldLevel Number;
  newLevel Number;
      
  -- Calcolo della atipicit su documenti e fascicoli visti dal ruolo, solo se
  -- attiva per l'amministrazione o per l'installazione e se richiesto
  keyValue VARCHAR (128);
  -- Query custom da eseguire per il calcolo degli id degli oggetti sui cui calcolare l'atipicit
  queryCustomDoc Varchar(2000) := '';
  queryCustomFasc Varchar(2000) := '';
  
  BEGIN
    
    -- Recupero dello stato di abilitazione del calcolo di atipicit
    SELECT var_valore INTO keyValue FROM dpa_chiavi_configurazione WHERE var_codice = 'ATIPICITA_DOC_FASC' AND (id_amm = 0 OR id_amm = idAmm) AND ROWNUM = 1;
    
    -- Recupero dei livelli del ruolo prima e dopo la modifica
    Select num_livello Into oldLevel From dpa_tipo_ruolo Where system_id = idTipoRuoloVecchio;
    Select num_livello Into newLevel From dpa_tipo_ruolo Where system_id = idTipoRuolo;

    If keyValue = '1' Then
      Begin
        -- Calcolo dell'atipicit per gli oggetti del ruolo che ha subito modifiche
        queryCustomDoc := 'Select p.system_id 
                              From security s 
                              Inner Join profile p 
                              On p.system_id = s.thing 
                              Where personorgroup = ' || idGroup || 
                              ' And accessrights > 20 
                              And p.cha_tipo_proto In (''A'',''P'',''I'',''G'')
                              And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'')';
        queryCustomFasc := 'Select p.system_id 
                            From security s 
                            Inner Join project p 
                            On p.system_id = s.thing 
                            Where personorgroup = ' || idGroup ||
                            ' And accessrights > 20 
                            And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' ) 
                            AND p.cha_tipo_fascicolo = ''P''';
        
        -- Esecuzione del calcolo dell'atipicit
        vis_doc_anomala_custom(idAmm, queryCustomDoc);
        vis_fasc_anomala_custom(idAmm, queryCustomFasc);
        
        -- Pulizia delle query
        queryCustomDoc := '';
        queryCustomFasc := '';
    
        -- Se c' stata discesa gerarchica, viene calcolata l'atipicit sui superiori 
        -- che prima della modifica erano sottoposti
        If newLevel > oldLevel Then
          Begin
            queryCustomDoc := ' Select Distinct(s.thing) 
                                From security s 
                                Inner Join profile p
                                On p.system_id = s.thing
                                Where personorgroup In (
                                  Select Distinct(p.id_gruppo)
                                  From dpa_corr_globali p
                                  Where id_amm = ' || idAmm || '
                                  And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldLevel || '
                                  And (Select num_livello from dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) <= ' || newLevel || '
                                  And id_gruppo != ' || idGroup || '
                                  And p.id_uo In (
                                    Select p.SYSTEM_ID
                                    From dpa_corr_globali p
                                    Start With p.SYSTEM_ID = ' || idUo || '
                                    Connect By Prior
                                    p.ID_PARENT = p.SYSTEM_ID And 
                                    p.CHA_TIPO_URP = ''U'' And 
                                    p.ID_AMM = ' || idAmm || '
                                  ))
                                  And s.accessrights > 20 
                                  And p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
                                  And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )';
            queryCustomFasc := '  Select Distinct(s.thing) 
                                  From security s 
                                  Inner Join project p
                                  On p.system_id = s.thing
                                  Where personorgroup In (
                                    Select Distinct(p.id_gruppo)
                                    From dpa_corr_globali p
                                    Where id_amm = ' || idAmm ||
                                    'And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldLevel || '
                                    And (Select num_livello from dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) <= ' || newLevel || '
                                    And id_gruppo != ' || idGroup || '
                                    And p.id_uo In (
                                      Select p.SYSTEM_ID
                                      From dpa_corr_globali p
                                      Start With p.SYSTEM_ID = ' || idUo || '
                                      Connect By Prior
                                      p.ID_PARENT = p.SYSTEM_ID And 
                                      p.CHA_TIPO_URP = ''U'' And 
                                      p.ID_AMM = ' || idAmm || '
                                      ))
                                  AND s.accessrights > 20
                                  And p.cha_tipo_fascicolo = ''P''';                      
          End;
        Else  
            -- Altrimenti se c' stata salita gerarchica o se c' stato spostamento
            -- ed  stato richiesto il calcolo dell'atipicit, viene calcolata
            -- l'atipicit sui sottoposti
            If newLevel < oldLevel Or (idUo != idVecchiaUo And calcolaSuiSottoposti = 1) Then
              Begin
                queryCustomDoc := ' select distinct(s.thing) 
                                    from security s 
                                    inner join profile p
                                    on p.system_id = s.thing
                                    where personorgroup in (
                                      select distinct(p.id_gruppo) 
                                      from dpa_corr_globali p
                                      where id_amm = ' || idAmm || ' AND 
                                      p.id_uo in (
                                        select p.SYSTEM_ID 
                                        from dpa_corr_globali p
                                        start with p.SYSTEM_ID = ' || idUo ||
                                        ' connect by prior
                                        p.SYSTEM_ID = p.ID_PARENT AND 
                                        p.CHA_TIPO_URP = ''U'' AND 
                                        p.ID_AMM = ' || idAmm || '))
                                        AND s.accessrights > 20 
                                        AND p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
                                        AND (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )
                                        And s.personorgroup != ' || idGroup; 
                queryCustomFasc :=  'select distinct(s.thing) 
                                    from security s 
                                    inner join project p
                                    on p.system_id = s.thing
                                    where personorgroup in (
                                      select distinct(p.id_gruppo) 
                                      from dpa_corr_globali p
                                      where id_amm = ' || idAmm || ' AND 
                                      p.id_uo in (
                                        select p.SYSTEM_ID 
                                        from dpa_corr_globali p
                                        start with p.SYSTEM_ID = ' || idUo ||
                                        'connect by prior
                                        p.SYSTEM_ID = p.ID_PARENT AND 
                                        p.CHA_TIPO_URP = ''U'' AND 
                                        p.ID_AMM = ' || idAmm || ')
                                        AND s.accessrights > 20
                                        and p.cha_tipo_fascicolo = ''P'')
                                        And s.personorgroup != ' || idGroup; 
                -- Esecuzione del calcolo dell'atipicit
                vis_doc_anomala_custom(idAmm, queryCustomDoc);
                vis_fasc_anomala_custom(idAmm, queryCustomFasc);
              End;
            End If;  
        End If;  
        
        -- Se  stato compiuto uno spostamento viene ricalcolata l'atipicit anche
        -- sui sottoposti del ruolo nella catena di origine
        If idUo != idVecchiaUO Then
          Begin
          vis_doc_anomala_custom(idAmm,'Select Distinct(s.thing) 
                                        From security s 
                                        Inner Join profile p
                                        On p.system_id = s.thing
                                        Where personorgroup In (
                                          Select Distinct(p.id_gruppo)
                                          From dpa_corr_globali p
                                          Where id_amm = ' || idAmm || '
                                          And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldLevel || '  
                                          And p.id_uo In (
                                            Select p.SYSTEM_ID
                                            From dpa_corr_globali p
                                            Start With p.SYSTEM_ID = ' || idVecchiaUo || '
                                            Connect By Prior
                                            p.system_id = p.id_parent And 
                                            p.CHA_TIPO_URP = ''U'' And 
                                            p.ID_AMM = ' || idAmm || '
                                          ))
                                          And s.accessrights > 20 
                                          And p.cha_tipo_proto in (''A'',''P'',''I'',''G'') 
                                          And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )');
          vis_fasc_anomala_custom(idAmm, '  Select Distinct(s.thing) 
                                            From security s 
                                            Inner Join project p
                                            On p.system_id = s.thing
                                            Where personorgroup In (
                                              Select Distinct(p.id_gruppo)
                                              From dpa_corr_globali p
                                              Where id_amm = ' || idAmm || '
                                              And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldLevel || '  
                                              And p.id_uo In (
                                                Select p.SYSTEM_ID
                                                From dpa_corr_globali p
                                                Start With p.SYSTEM_ID = ' || idVecchiaUo || '
                                                Connect By Prior
                                                p.system_id = p.id_parent And 
                                                p.CHA_TIPO_URP = ''U'' And 
                                                p.ID_AMM = ' || idAmm || '
                                              ))
                                            And s.accessrights > 20
                                            And p.cha_tipo_fascicolo = ''P''');
                                        
          End;
        End If;
      End;
    
    End If;
  
    Returnvalue := 0;
  
  END;
END COMPUTEATIPICITA;
/

              
----------- FINE -
              
---- COPYSECURITY.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.CopySecurity (
  -- Id gruppo del ruolo di cui copiare la visibilit
  sourceGroupId IN NUMBER,
  -- Id gruppo del ruolo di destinazione
  destinationGroupId IN NUMBER
)
AS 
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     CopySecurity

    PURPOSE:  Store per la copia di alcuni record della security. I record 
              copiati avranno come tipo diritto 'A' 

  ******************************************************************************/
  
   -- Nome della colonna letta dalla tabella dei metadati
  DECLARE colName VARCHAR2 (2000);
  
  -- Lista separata da , dei nomi delle colonne in cui eseguire la insert
  colNameList VARCHAR2 (4000);
  
  -- Lista separata da , dei valori da assegnare alle colonne
  colValuesList VARCHAR2 (4000);
  
  -- Selezione delle colonne della security dalla tabella dei metadati
  CURSOR curColumns IS
    SELECT cname from col where tname = 'SECURITY' order by colno asc;
      
  BEGIN OPEN curColumns;
  LOOP FETCH curColumns INTO colName;
  EXIT WHEN curColumns%NOTFOUND;
  
    -- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'
    colNameList := colNameList || ', ' || colName;
    
    CASE (colName)
        WHEN 'THING' THEN
          colValuesList := colValuesList || ', DISTINCT(thing)';
        WHEN 'PERSONORGROUP' THEN
          colValuesList := colValuesList || ', ' || destinationGroupId;
        WHEN 'ACCESSRIGHTS' THEN
          colValuesList := colValuesList || ', DECODE(accessrights, 255, 63, accessrights, accessrights)';
        WHEN 'CHA_TIPO_DIRITTTO' THEN
          colValuesList := colValuesList || ', A';
        ELSE
          colValuesList := colValuesList || ', ' || colName;
    END CASE;
  END LOOP;
  CLOSE curColumns;
  
  colNameList := SUBSTR( colNameList, 3); 
  colValuesList := SUBSTR( colValuesList, 3); 
  
  EXECUTE IMMEDIATE 'INSERT INTO security (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM security s WHERE personorgroup = ' || sourceGroupId || ' AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' || destinationGroupId || '))';
END;  
END CopySecurity;
/

              
----------- FINE -
              
---- CopySecurityWithoutAtipici.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.CopySecurityWithoutAtipici (
  -- Id gruppo del ruolo di cui copiare la visibilit
  sourceGroupId IN NUMBER,
  -- Id gruppo del ruolo di destinazione
  destinationGroupId IN NUMBER
)
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
  DECLARE colName VARCHAR2 (2000);
  
  -- Lista separata da , dei nomi delle colonne in cui eseguire la insert
  colNameList VARCHAR2 (4000);
  
  -- Lista separata da , dei valori da assegnare alle colonne
  colValuesList VARCHAR2 (4000);
  
  -- Selezione delle colonne della security dalla tabella dei metadati
  CURSOR curColumns IS
    SELECT cname from col where tname = 'SECURITY' order by colno asc;
      
  BEGIN OPEN curColumns;
  LOOP FETCH curColumns INTO colName;
  EXIT WHEN curColumns%NOTFOUND;
  
    -- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'
    colNameList := colNameList || ', ' || colName;
    
    CASE (colName)
        WHEN 'THING' THEN
          colValuesList := colValuesList || ', DISTINCT(thing)';
        WHEN 'PERSONORGROUP' THEN
          colValuesList := colValuesList || ', ' || destinationGroupId;
        WHEN 'ACCESSRIGHTS' THEN
          colValuesList := colValuesList || ', DECODE(accessrights, 255, 63, accessrights, accessrights)';
        WHEN 'CHA_TIPO_DIRITTTO' THEN
          colValuesList := colValuesList || ', A';
        ELSE
          colValuesList := colValuesList || ', ' || colName;
    END CASE;
  END LOOP;
  CLOSE curColumns;
  
  colNameList := SUBSTR( colNameList, 3); 
  colValuesList := SUBSTR( colValuesList, 3); 
  
  EXECUTE IMMEDIATE 'INSERT INTO security (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM security s inner join profile p on s.thing = p.system_id where s.personorgroup = ' || sourcegroupid || ' and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' || destinationGroupId || '))';

  EXECUTE IMMEDIATE 'INSERT INTO security (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM security s inner join project p on s.thing = p.system_id where s.personorgroup = ' || sourcegroupid || ' and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' || destinationGroupId || '))';
END;  
END CopySecurityWithoutAtipici;
/

              
----------- FINE -
              
---- ExtendVisibilityToHigherRoles.ORA.SQL  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.ExtendVisibilityToHigherRoles(
  -- Id dell'amministrazione 
  idAmm IN INTEGER,
  -- Id del gruppo da analizzare e di cui estendere la visibilit
  idGroup IN INTEGER,
  -- Indicatore dello scope della procedura di estensione di visibilit
  -- A -> Tutti, E -> Esclusione degli atipici
  extendScope IN VARCHAR,
  -- Flag utilizzato per indicare se bisogna copiare gli id di documenti e
  -- fascicoli nella tabella temporanea per l'allineamento asincrono con Documentum
  copyIdToTempTable IN INTEGER,
  returnValue OUT INTEGER
) AS 
BEGIN
   /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     ExtendVisibilityToHigherRoles

    PURPOSE:  Store per l'estensione della visibilit ai ruoli superiori. 

  ******************************************************************************/

  -- Selezione dei superiori dei ruolo
  DECLARE CURSOR higherRoles IS
    SELECT
      dpa_corr_globali.id_gruppo
      FROM dpa_corr_globali
      INNER JOIN dpa_tipo_ruolo
      ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
      WHERE dpa_corr_globali.id_uo IN
      (SELECT dpa_corr_globali.system_id
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.dta_fine IS NULL
      CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id
      START WITH dpa_corr_globali.system_id =
      (SELECT dpa_corr_globali.id_uo
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.id_gruppo = idGroup
      )
      )
      AND dpa_corr_globali.CHA_TIPO_URP = 'R'
      AND dpa_corr_globali.ID_AMM = idAmm
      AND dpa_corr_globali.DTA_FINE IS NULL
      AND dpa_tipo_ruolo.num_livello <
      (SELECT dpa_tipo_ruolo.num_livello
      FROM dpa_corr_globali
      INNER JOIN dpa_tipo_ruolo
      ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
      WHERE dpa_corr_globali.id_gruppo  = idGroup
      );
      
  
  -- Id gruppo da analizzare
  idGroupToAnalyze  INTEGER;
  
  BEGIN OPEN higherRoles;
  LOOP FETCH higherRoles INTO idGroupToAnalyze;
  EXIT WHEN higherRoles%NOTFOUND;  
  
    -- Per ogni ruolo superiore, viene effettuata una operazione diversa 
    -- a seconda del tipo di estensione da eseguire
    CASE (extendScope)
        WHEN 'A' THEN
          CopySecurity(idGroup, idGroupToAnalyze);
        WHEN 'E' THEN
          CopySecurityWithoutAtipici(idGroup, idGroupToAnalyze);
    END CASE;
    
    -- Se richiesto, viene aggiornata la tabella delle sincronizzazione pending
    IF copyIdToTempTable = 1 THEN
      BEGIN
        INSERT INTO dpa_objects_sync_pending
        (
          id_doc_or_fasc,
          type,
          id_gruppo_to_sync
        )
        (
          SELECT p.system_id, 'D', idGroupToAnalyze
          FROM security s 
          Inner Join Profile P On P.System_Id = S.Thing 
          And (P.Cha_Privato Is Null Or P.Cha_Privato != 0)
          And (P.Cha_Personale is null Or P.Cha_Personale != 0)
          WHERE personorgroup = idGroupToAnalyze and 
          not exists( select 'x' 
            from dpa_objects_sync_pending sp 
            where sp.id_doc_or_fasc = s.thing AND
                  sp.id_gruppo_to_sync = idGroupToAnalyze
            ));
        
        INSERT INTO dpa_objects_sync_pending
        (
          id_doc_or_fasc,
          type,
          id_gruppo_to_sync
        )
        (
          SELECT p.system_id, 'F', idGroupToAnalyze
          FROM security s 
          INNER JOIN project p ON p.system_id = s.thing 
          Where Personorgroup = Idgrouptoanalyze And 
          (P.Cha_Privato Is Null Or P.Cha_Privato != 0) And
          not exists( select 'x' 
            from dpa_objects_sync_pending sp 
            where sp.id_doc_or_fasc = s.thing AND
                  sp.id_gruppo_to_sync = idGroupToAnalyze
          ));
      END;
    END IF;
  
  END LOOP;
  CLOSE higherRoles;

  returnValue := 0;
END;
END ExtendVisibilityToHigherRoles;
/

              
----------- FINE -
              
---- HistoricizeRole.ORA.sql  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.HistoricizeRole 
(
  -- Id corr globali del ruolo da storicizzare
  idCorrGlobRole IN INTEGER  ,
  -- Eventuale nuovo codice da assegnare al ruolo
  newRoleCode   IN VARCHAR2,
  -- Eventuale nuova descrizione da assegnare al ruolo
  newRoleDescription  IN VARCHAR2, 
  -- Identificativo dell'eventuale nuova UO in cui deve essere inserito il ruolo
  newRoleUoId   IN VARCHAR2,
  -- Identificativo dell'eventuale nuovo tipo ruolo da assegnare al ruolo
  newRoleTypeId in number,
  -- Identificativo del record storicizzato
  oldIdCorrGlobId OUT INTEGER,
  -- Risultato dell'operazione
  returnValue OUT INTEGER
) AS 
BEGIN
    /******************************************************************************

    AUTHOR:   Samuele Furnari
	modifiche by Luciani - apr 2012

    NAME:     HistoricizeRole 

    PURPOSE:  Store per la storicizzazione di un ruolo. Per ridurre al minimo 
              le movimentazioni di dati, specialmente nella securiy, viene
              adottata una tecnica di storicizzazione "verso l'alto" ovvero,
              quando si deve storicizzare un ruolo R con system id S e 
              codice rubrica C, viene inserita nella DPA_CORR_GLOBALI una tupla 
              che si differenzia da quella di R solo per S che sar un nuovo 
              numero assegnato dalla sequence, var_codice e var_cod_rubrica
              che saranno uguali a quelli di R con l'aggiunta di _S, ed id_old 
              che sar impostato ad S (Attenzione! S  il system id di R).
              A questo punto, le eventuali modifiche ad attributi del ruolo
              verranno salvate sul record di R e tutte le tuple di documenti
              (doc_arrivo_par), le informazioni sul creatore di documenti e
              fascioli e le informazioni sulle trasmissioni che referenziano R, 
              vengono aggiornate in modo che facciano riferimento al nuovo 
              record della DPA_CORR_GLOBALI appena inserito.
              
              Fare riferimento al corpo della store per maggiori dettagli.

  ******************************************************************************/
  
  -- Nome della colonna letta dalla tabella dei metadati
  DECLARE colName VARCHAR2 (2000);
  
  -- Lista separata da , dei nomi delle colonne in cui eseguire la insert
  colNameList VARCHAR2 (4000);
  
  -- Lista separata da , dei valori da assegnare alle colonne
  colValuesList VARCHAR2 (4000);
  
  -- Selezione delle colonne della corr globali dalla tabella dei metadati
  CURSOR curColumns IS
    SELECT cname from col where tname = 'DPA_CORR_GLOBALI' order by colno asc;
      
  BEGIN OPEN curColumns;
  LOOP FETCH curColumns INTO colName;
  EXIT WHEN curColumns%NOTFOUND;
  
    -- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'
    colNameList := colNameList || ', ' || colName;
    
    CASE (colName)
        WHEN 'SYSTEM_ID' THEN
          colValuesList := colValuesList || ', ' || 'SEQ.NEXTVAL';
        WHEN 'VAR_CODICE' THEN
          colValuesList := colValuesList || ', ' || nq'#VAR_CODICE || '_' || SEQ.CURRVAL#';
        WHEN 'VAR_COD_RUBRICA' THEN
          colValuesList := colValuesList || ', ' || nq'#VAR_COD_RUBRICA || '_' || SEQ.CURRVAL#';
        WHEN 'DTA_FINE' THEN
          colValuesList := colValuesList || ', ' || 'SYSDATE';
        ELSE
          colValuesList := colValuesList || ', ' || colName;
    END CASE;
  END LOOP;
  CLOSE curColumns;
  
  colNameList := SUBSTR( colNameList, 3); 
  colValuesList := SUBSTR( colValuesList, 3); 
  
  EXECUTE IMMEDIATE 'INSERT INTO dpa_corr_globali (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM dpa_corr_globali WHERE system_id = ' || idCorrGlobRole || ')';
  
  SELECT MAX(system_id) INTO oldIdCorrGlobId FROM dpa_corr_globali WHERE original_id = idCorrGlobRole;
  
  
  -- Aggiornamento dei dati relativi al nuovo ruolo e impostazione dell'id_old
  update dpa_corr_globali 
  set id_old = oldidcorrglobid,
      var_codice = newRoleCode,
      var_desc_corr = newRoleDescription,
      id_uo = newRoleUoId,
      id_tipo_ruolo = newRoleTypeId
  where system_id = idcorrglobrole;
  
  -- Cancellazione dell'id gruppo per il gruppo storicizzato
  update dpa_corr_globali 
  set id_gruppo = null 
  where  system_id = oldidcorrglobid;
  
  -- Aggiornamento degli id mittente e destinatario relativi al ruolo
  update dpa_doc_arrivo_par
  set id_mitt_dest = oldidcorrglobid
  where id_mitt_dest = idcorrglobrole;
  
  -- Aggiornamento degli id dei ruoli creatori per documenti e fascicoli
  update profile 
  set id_ruolo_creatore = oldidcorrglobid 
  where id_ruolo_creatore = idcorrglobrole;
  
  update project 
  set id_ruolo_creatore = oldidcorrglobid 
  where id_ruolo_creatore = idcorrglobrole;
  
  -- Aggiornamento delle trasmissioni
  update dpa_trasmissione
  set id_ruolo_in_uo = oldidcorrglobid
  where id_ruolo_in_uo = idcorrglobrole;
  
  update dpa_trasm_singola
  set id_corr_globale = oldidcorrglobid
  where id_corr_globale= idcorrglobrole;

  
  -- modifiche by Luciani
  
  --per le policy di conservazione
  update policy_conservazione pc set pc.ID_RUOLO=oldIdCorrGlobId where id_ruolo=idCorrGlobRole;
  
  
  -- Aggiornamento corrispondenti dei campi profilati
  update DPA_ASSOCIAZIONE_TEMPLATES ASSTEMP
  SET assTemp.VALORE_OGGETTO_DB = OLDIDCORRGLOBID 
  WHERE EXISTS
    (SELECT 'x'
    FROM DPA_OGGETTI_CUSTOM OC
    WHERE oc.ID_TIPO_OGGETTO =
      (SELECT tObj.SYSTEM_ID
      FROM DPA_TIPO_OGGETTO TOBJ
      WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente'      )
    AND OC.SYSTEM_ID = ASSTEMP.ID_OGGETTO    )
  AND ASSTEMP.VALORE_OGGETTO_DB = '' || IDCORRGLOBROLE || '';

  update DPA_ASS_TEMPLATES_FASC ASSTEMP
  SET assTemp.VALORE_OGGETTO_DB = OLDIDCORRGLOBID 
  WHERE EXISTS
    (SELECT 'x'
    FROM DPA_OGGETTI_CUSTOM_FASC OC
    WHERE oc.ID_TIPO_OGGETTO =
      (SELECT tObj.SYSTEM_ID
      FROM DPA_TIPO_OGGETTO_FASC TOBJ
      WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente'      )
    AND OC.SYSTEM_ID = ASSTEMP.ID_OGGETTO    )
  AND ASSTEMP.VALORE_OGGETTO_DB = '' || IDCORRGLOBROLE || '';

  -- fine modifiche by Luciani

  returnValue := 1;
  EXCEPTION
    WHEN OTHERS THEN
      retuRnValue := - 1;
	  RAISE;
  
END;   
END HistoricizeRole;
/
              
----------- FINE -
              
---- INITIALIZEROLEHISTORY.ORA.SQL  marcatore per ricerca ----
create or replace Procedure @db_user.Initializerolehistory As 
idRole number;
idUo number;
idRoleType number;
originalId number;
startTime date;
description varchar (256);
code varchar (128);
BEGIN
    
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      InitializeRoleHistory

    PURPOSE:   Store procedure per l'inizializzazione della tabella dello storico
               dei ruoli
 
  ******************************************************************************/

    
    Declare Cursor Cursore Is 
      select id_uo, id_tipo_ruolo, original_id, dta_inizio, var_codice, var_desc_corr, System_Id
      from dpa_corr_globali 
      where id_old = '0' and system_id = original_id 
            and id_uo is not null
            and id_tipo_ruolo is not null;
            
    Begin Open Cursore;
    LOOP FETCH cursore INTO idUo, idRoleType, originalId, startTime, code, description, idRole ;
    EXIT WHEN cursore%NOTFOUND;
      
      insert
      INTO DPA_ROLE_HISTORY
      (
        SYSTEM_ID ,
        ACTION_DATE ,
        UO_ID ,
        ROLE_TYPE_ID ,
        ORIGINAL_CORR_ID ,
        Action ,
        Role_Description,
        ROLE_ID
      )
      VALUES
      (
        seqrolehistory.nextval ,
        startTime ,
        idUo ,
        idroletype ,
        originalid ,
        'C' ,
        Description || ' (' || Code || ')',
        Idrole
      );
    end loop;
    close cursore;
end;    
End Initializerolehistory;
/

BEGIN
Initializerolehistory;  
End ;
/
              
----------- FINE -
              
---- I_SMISTAMENTO_SMISTADOC_R_2.sql  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.I_Smistamento_Smistadoc_R_2(
IDPeopleMittente IN NUMBER,
IDCorrGlobaleRuoloMittente IN NUMBER,
IDGruppoMittente IN NUMBER,
IDAmministrazioneMittente IN NUMBER,
IDCorrGlobaleDestinatario IN NUMBER,
IDDocumento IN NUMBER,
TipoDiritto IN CHAR,
Rights IN NUMBER,
IDRagioneTrasm IN NUMBER,
ReturnValue OUT NUMBER)    IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni a ruolo nella protocollazione semplificata
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-------------------------------------------------------------------------------------------------------
*/

IdentityTrasm NUMBER := NULL;
IdentityTrasmSing NUMBER := NULL;

ExistAccessRights CHAR(1) := 'Y';
AccessRights NUMBER:= NULL;
accessRightsValue NUMBER := NULL;

IDUtente NUMBER;
recordCorrente NUMBER;

IDGroups NUMBER := NULL;
IDGruppo NUMBER;

/* Prende gli utenti del ruolo */
CURSOR cursor_IDUtenti (ID_CORR_GLOB_RUOLO NUMBER) IS
SELECT     P.SYSTEM_ID
FROM     GROUPS G,
PEOPLEGROUPS PG,
PEOPLE P,
DPA_CORR_GLOBALI CG
WHERE     PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND
PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND
G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = ID_CORR_GLOB_RUOLO) AND
P.DISABLED NOT IN ('Y') AND
P.SYSTEM_ID=CG.ID_PEOPLE
AND CG.CHA_TIPO_URP != 'L'
AND CG.DTA_FINE IS NULL
AND PG.DTA_FINE IS NULL;

BEGIN

BEGIN
SELECT seq.NEXTVAL INTO IdentityTrasm FROM dual;
END;

BEGIN
SELECT seq.NEXTVAL INTO IdentityTrasmSing FROM dual;
END;

BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
INSERT INTO DPA_TRASMISSIONE
(
SYSTEM_ID,
ID_RUOLO_IN_UO,
ID_PEOPLE,
CHA_TIPO_OGGETTO,
ID_PROFILE,
ID_PROJECT,
DTA_INVIO,
VAR_NOTE_GENERALI
)
VALUES
(
IdentityTrasm,
IDCorrGlobaleRuoloMittente,
IDPeopleMittente,
'D',
IDDocumento,
NULL,
SYSDATE(),
NULL
);
END;

BEGIN
INSERT INTO DPA_TRASM_SINGOLA
(
SYSTEM_ID,
ID_RAGIONE,
ID_TRASMISSIONE,
CHA_TIPO_DEST,
ID_CORR_GLOBALE,
VAR_NOTE_SING,
CHA_TIPO_TRASM,
DTA_SCADENZA,
ID_TRASM_UTENTE
)
VALUES
(
IdentityTrasmSing,
IDRagioneTrasm,
IdentityTrasm,
'R',
IDCorrGlobaleDestinatario,
NULL,
'S',
NULL,
NULL
);
END;

BEGIN

OPEN cursor_IDUtenti(IDCorrGlobaleDestinatario);

LOOP

FETCH cursor_IDUtenti INTO recordCorrente;
EXIT WHEN cursor_IDUtenti%NOTFOUND;
IDUtente := recordCorrente;

BEGIN
-- Inserimento in tabella DPA_TRASM_UTENTE
INSERT INTO DPA_TRASM_UTENTE
(
SYSTEM_ID,
ID_TRASM_SINGOLA,
ID_PEOPLE,
DTA_VISTA,
DTA_ACCETTATA,
DTA_RIFIUTATA,
DTA_RISPOSTA,
CHA_VISTA,
CHA_ACCETTATA,
CHA_RIFIUTATA,
VAR_NOTE_ACC,
VAR_NOTE_RIF,
CHA_VALIDA,
ID_TRASM_RISP_SING
)
VALUES
(
seq.NEXTVAL,
IdentityTrasmSing,
IDUtente,
NULL,
NULL,
NULL,
NULL,
'0',
'0',
'0',
NULL,
NULL,
'1',
NULL
);
END;

END LOOP;

CLOSE cursor_IDUtenti;

END;

-- Verifica se non vi sia gia una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
--    1) se ACCESSRIGHT < Rights
--       viene fatto un'aggiornamento impostandone il valore a Rights
--    2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
BEGIN

--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio.!!!
update dpa_trasmissione set dta_invio=sysdate() where system_id=IdentityTrasm;

SELECT A.ID_GRUPPO INTO IDGroups
FROM DPA_CORR_GLOBALI A
WHERE A.SYSTEM_ID = IDCorrGlobaleDestinatario;

END;

IDGruppo := IDGroups;

BEGIN
SELECT ACCESSRIGHTS INTO AccessRights FROM (
SELECT   ACCESSRIGHTS
FROM     SECURITY
WHERE    THING = IDDocumento
AND      PERSONORGROUP = IDGruppo
) WHERE ROWNUM = 1;

EXCEPTION
WHEN NO_DATA_FOUND THEN
ExistAccessRights := 'N';

END;

IF ExistAccessRights = 'Y' THEN

accessRightsValue := AccessRights;

IF accessRightsValue < Rights THEN

BEGIN
/* aggiornamento a Rights */
UPDATE     SECURITY
SET     ACCESSRIGHTS = Rights,
        cha_tipo_diritto = 'T'
WHERE     THING = IDDocumento
AND
PERSONORGROUP = IDGruppo
AND ACCESSRIGHTS = accessRightsValue;

EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;

END IF;

ELSE

BEGIN
/* inserimento a Rights */
INSERT INTO SECURITY
(
THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO
)
VALUES
(
IDDocumento,
IDGruppo,
Rights,
IDGruppoMittente,
TipoDiritto
);

EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;

END IF;

ReturnValue := 0;

END;
/
              
----------- FINE -
              
---- I_SMISTAMENTO_SMISTADOC.sql  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.i_smistamento_smistadoc (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idpeopledestinatario           IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-- -5: Errore in SPsetDataVistaSmistamento
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   identitytrasmsing   NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   resultvalue         NUMBER;   
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;
 
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasmsing
        FROM DUAL;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO @db_user.dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO @db_user.dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing, cha_tipo_trasm,
                   dta_scadenza, id_trasm_utente
                  )
           VALUES (identitytrasmsing, idragionetrasm, identitytrasm, 'U',
                   idcorrglobaledestinatario, noteindividuali, 'S',
                   datascadenza, NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO @db_user.dpa_trasm_utente
                  (system_id, id_trasm_singola, id_people,
                   dta_vista, dta_accettata, dta_rifiutata, dta_risposta,
                   cha_vista, cha_accettata, cha_rifiutata, var_note_acc,
                   var_note_rif, cha_valida, id_trasm_risp_sing
                  )
           VALUES (seq.NEXTVAL, identitytrasmsing, idpeopledestinatario,
                   NULL, NULL, NULL, NULL,
                   '0', '0', '0', NULL,
                   NULL, '1', NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;
 
   BEGIN
--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
      UPDATE dpa_trasmissione
         SET dta_invio = SYSDATE ()
       WHERE system_id = identitytrasm;
 
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento
                 AND personorgroup = idpeopledestinatario)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;
 
   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;
 
      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET  accessrights = rights,
                    cha_tipo_diritto = 'T'
             WHERE thing = iddocumento
               AND personorgroup = idpeopledestinatario
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights,
                      id_gruppo_trasm, cha_tipo_diritto
                     )
              VALUES (iddocumento, idpeopledestinatario, rights,
                      idgruppomittente, tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;
 
/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;            
        
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;
 
            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta gia accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',                        
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.thing=iddocumento and s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );
 
         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;
 
/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;
 
   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
 
   returnvalue := 0;
END; 
/
              
----------- FINE -
              
---- I_SMISTAMENTO_SMISTADOC_U.sql  marcatore per ricerca ----
create or replace PROCEDURE @db_user.i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm         NUMBER       := NULL;
   systrasmsing          NUMBER       := NULL;
   existaccessrights     CHAR (1)     := 'Y';
   accessrights          NUMBER       := NULL;
   accessrightsvalue     NUMBER       := NULL;
   idutente              NUMBER;
   recordcorrente        NUMBER;
   idgroups              NUMBER       := NULL;
   idgruppo              NUMBER;
   resultvalue           NUMBER;
   tipotrasmsingola      CHAR (1)     := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   val_idpeopledelegato  NUMBER;
   TipoRag VARCHAR2 (1);
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
-- inizio modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
/* Inserimento in tabella DPA_TRASMISSIONE */

-- la procedura riceve in input il valore = 0 per il campo idpeopledelegato, in caso di senza delega 
-- e il valore = 1 in caso di delega
-- nel primo caso si vuole avere comunque il valore NULL nel campo idpeopledelegato
      IF (idpeopledelegato > 0)
      THEN
        val_idpeopledelegato := idpeopledelegato ; 
          ELSE 
        val_idpeopledelegato := NULL;
      END IF;  
         INSERT INTO dpa_trasmissione
                     (system_id, id_ruolo_in_uo,
                      id_people, cha_tipo_oggetto, id_profile, id_project,
                      dta_invio, var_note_generali, id_people_delegato
                     )
              VALUES (identitytrasm, idcorrglobaleruolomittente,
                      idpeoplemittente, 'D', iddocumento, NULL,
                      SYSDATE (), notegeneralidocumento, val_idpeopledelegato
                     );
-- precedente era
/*
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE , notegeneralidocumento
                  );
*/
-- fine modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega

   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights,
                   cha_tipo_diritto = 'T'
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1')
   THEN
      BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
         SELECT cha_accettata
           INTO isaccettata
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

         SELECT cha_vista
           INTO isvista
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

          SELECT cha_tipo_ragione into TipoRag from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
           where tsu.system_id=idtrasmissioneutentemittente and ts.system_id=tsu.ID_TRASM_SINGOLA and rs.system_id=ts.ID_RAGIONE ;

         IF (idpeopledelegato > 0)
         THEN
            BEGIN
-- Impostazione dei flag per la gestione del delegato
               isvistadelegato := '1';
               isaccettatadelegato := '1';
            END;
         END IF;

         IF (isaccettata = '1')
         THEN
            BEGIN
-- caso in cui la trasmissione risulta gi? accettata
               IF (isvista = '0')
               THEN
                  BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
                     UPDATE dpa_trasm_utente
                        SET dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            cha_vista =
                                       (CASE
                                           WHEN dta_vista IS NULL
                                              THEN 1
                                           ELSE 0
                                        END
                                       ),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               ELSE
                  BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               END IF;
            END;
         ELSE

         begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

if(TipoRAG='W') then
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE,
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U')
                      )
                  AND cha_valida = '1';
            END;
         else --no workflow
             BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                     -- dta_accettata = SYSDATE (),
                    --  cha_accettata = '1',
                     -- cha_accettata_delegato = isaccettatadelegato,
                     -- var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U'  AND cha_valida = '1')
                      )
                 ;
            END;



         end if;
         end;

         END IF;



--update security se diritti  trasmssione in accettazione =20
         UPDATE security s
            SET s.accessrights = originalrights,
                s.cha_tipo_diritto = 'T'
          WHERE s.thing = iddocumento
            AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
            AND s.accessrights = 20;

        EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
      -- visibilit gi esistente, ignora e continua con eventuali altri inserimenti 
              NULL;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END; 
/
              
----------- FINE -
              
---- RemoveTransModelVisibileToRole.ORA.SQL  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.RemoveTransModelVisibileToRole(
    -- Id corr globali del ruolo di cui eleminare i modelli
    roleCorrBGlobId     IN      INTEGER
) IS

/******************************************************************************
   NAME:       RemoveTransModelVisibileToRole
   AUTHOR:     Samuele Furnari
   PURPOSE:    Store procedure per la cancellazione di modelli di trasmissione
               visibili solo al ruolo con id corr globali pari a quello
               passato per parametro

******************************************************************************/
idModello number;
countIdModello number;
    
-- Cursore per scorrere sugli id di modelli di trasmissione che hanno 
-- esattamente un solo mittente.
cursor models is
    select md.ID_MODELLO, count(*) as numMitt 
    from dpa_modelli_mitt_dest md 
    where md.CHA_TIPO_MITT_DEST = 'M' and md.ID_CORR_GLOBALI = roleCorrBGlobId
    group by md.id_modello order by md.ID_MODELLO;
      
begin   
    -- Apertura cursore
    open models;
    
    loop 
    fetch models into idModello, countIdModello;
    EXIT WHEN models%NOTFOUND;
    
    if countIdModello = 1 then
    
        begin
            -- Cancellazione delle righe della DPA_MODELLI_DEST_CON_NOTIFICA
            delete dpa_modelli_dest_con_notifica where id_modello = idModello;
            
            -- Cancellazione delle righe della DPA_MODELLI_MITT_DEST
            delete dpa_modelli_mitt_dest where id_modello = idModello;
            
            -- Cancellazione della tupla da DPA_MODELLI_TRASM
            delete dpa_modelli_trasm where system_id = idModello;
        end; 
        
    end if;
    
    END LOOP;
    
    CLOSE models;
    
    EXCEPTION when others then
    RETURN;

END RemoveTransModelVisibileToRole;
/
              
----------- FINE -
              
---- setdatareg.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.setdatareg
IS
BEGIN
   DECLARE
      CURSOR c       IS
         SELECT a.dta_open, b.num_rif, a.system_id AS sysid
           FROM dpa_el_registri a, dpa_reg_proto b
          WHERE a.system_id = b.id_registro
            AND a.cha_automatico = '1'
            AND cha_stato = 'A';

      c1   c%ROWTYPE;
      myesito varchar2(200); 
   BEGIN
      OPEN c;
      LOOP          FETCH c INTO c1;
         EXIT WHEN c%NOTFOUND;

         BEGIN
--INSERT INTO DPA_REGISTRI_STO
            INSERT INTO dpa_registro_sto
                        (system_id, dta_open, dta_close, num_rif,
                         id_registro, id_people, id_ruolo_in_uo)
               SELECT (seq.NEXTVAL), a.dta_open, SYSDATE, b.num_rif,
                      a.system_id, 1, 1
                 FROM dpa_el_registri a, dpa_reg_proto b
                WHERE a.system_id = b.id_registro AND a.system_id = c1.sysid;

            UPDATE dpa_el_registri
               SET dta_open = SYSDATE,
                   cha_stato = 'A',
                   dta_close = NULL
             WHERE system_id = c1.sysid;

            UPDATE dpa_reg_proto
               SET num_rif = 1
             WHERE TO_CHAR (SYSDATE, 'dd/mm') = '01/01'
-- agisce solo per quei registri che hanno autopertura impostata, cha_automatico = '1'
               AND c1.sysid = id_registro;
         EXCEPTION
            WHEN OTHERS
            THEN
            myesito := substr ('ko' || SQLERRM, 1, 200); 
               INSERT INTO dpa_log_install
                           (ID, data_operazione,
                            comando_richiesto, versione_cd
                            , esito_operazione)
                    VALUES (seq_install_log.NEXTVAL, SYSDATE,
                            'exec setdatareg', 'n.a.'
                            , myesito );
         END;
      END LOOP;

      CLOSE c;

      COMMIT;
   END;
END setdatareg;
/

              
----------- FINE -
              
---- SP_EREDITA_VIS_DOC_ATIPICITA.sql  marcatore per ricerca ----
create or replace PROCEDURE @db_user.sp_eredita_vis_doc_atipicita(
    idcorrglobaleuo    IN NUMBER,
    idcorrglobaleruolo IN NUMBER,
    idgruppo           IN NUMBER,
    livelloruolo       IN NUMBER,
    idregistro         IN NUMBER,
    parilivello        IN NUMBER,
    atipicita          IN VARCHAR,
    returnvalue OUT NUMBER )
IS
BEGIN

--DICHIARAZIONE DELLA STRINGA INSERT INTO SECURITY CHA VALE PER TUTTE LE INSERT DELLA STORED
declare ins_security varchar2(1000):= 'INSERT INTO security (thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto) ';

--DICHIARAZIONE DELLA SELECT PER L'INSERIMENTO CHE VALE PER TUTTE LE INSERT DELLA STORED
select_for_ins varchar2(1000):= 
'SELECT /*+ index(s) index(p) */ 
DISTINCT s.thing, 
'||idgruppo||',
(CASE WHEN (s.accessrights = 255 AND (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''A'')) THEN 63 ELSE s.accessrights END) AS acr,
NULL,
''A''
FROM security s, PROFILE p ';

 --DICHIARAZIONE DELLA SELECT PER L'EVENTUALE STORED DI ATIPICITA' CHE VALE PER TUTTE LE PROCEDURE DI ATIPICITA EVENTUALMENTE LANCIATE NELLA STORED
select_for_ins_store varchar2(1000):= 'SELECT /*+ index(s) index(p) */ DISTINCT s.thing FROM security s, PROFILE p ';

id_amministrazione number;

  BEGIN
    returnvalue   := 0;
    select id_amm into id_amministrazione from dpa_el_registri where system_id = idregistro;
    
    IF parilivello = 0 THEN
      
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL PRIMO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id = s.thing
      AND p.cha_privato = ''0''
      '
      || atipicita ||      
      '
      AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a, dpa_corr_globali b, GROUPS c, dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = ''R''
        AND b.cha_tipo_ie    = ''I''
        AND b.dta_fine      IS NULL
        AND a.num_livello    > '||livelloruolo||'
        AND b.id_uo          = '||idcorrglobaleuo||'
        AND d.id_registro    = '||idregistro||'
        )
      AND NOT EXISTS
        (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
        
        
      condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id = s.thing
      AND p.cha_privato = ''0''
      AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a, dpa_corr_globali b, GROUPS c, dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = ''R''
        AND b.cha_tipo_ie    = ''I''
        AND b.dta_fine      IS NULL
        AND a.num_livello    > '||livelloruolo||'
        AND b.id_uo          = '||idcorrglobaleuo||'
        AND d.id_registro    = '||idregistro||'
        )
      AND NOT EXISTS
        (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
      
      BEGIN
      
        -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
        -- includere nell'estensione tutti quei documenti che sono atipici 
        -- a causa della presenza del ruolo per cui si sta calcolando la
        -- visibilit
        If atipicita is not null And length(atipicita) > 2 Then
          Begin
            -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
            Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
            
            -- Calcolo atipicit
            VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
            
            -- Riabilitazione ruolo
            Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
          End;
        End If;
      
        --INIZIO PRIMA INSERT
        execute immediate(ins_security || select_for_ins || condizioni_select);
        --FINE PRIMA INSERT
        
        --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
        VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
        --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA        
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    ELSE
      
        --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL SECONDO INSERIMENTO
        declare condizioni_select varchar2(10000):=
        'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          ( SELECT DISTINCT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '|| idgruppo ||' AND s1.thing = p.system_id) ';
        
        condizioni_select_1 varchar2(10000):=
        'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          ( SELECT DISTINCT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '|| idgruppo ||' AND s1.thing = p.system_id) ';
        
        BEGIN
          -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
          -- includere nell'estensione tutti quei documenti che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
        
        
          --INIZIO SECONDA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE SECONDA INSERT
          
          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    END IF;
    /* UO INFERIORI */
    IF parilivello = 0 THEN
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL TERZO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0'' 
        '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie  = ''I''
            AND dta_fine       IS NULL
            AND id_old         = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
      condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0'' 
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie  = ''I''
            AND dta_fine       IS NULL
            AND id_old         = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
      
      BEGIN
          -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
          -- includere nell'estensione tutti quei documenti che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
      
         --INIZIO TERZA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE TERZA INSERT
          
          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    ELSE
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUARTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
         '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie = ''I''
            AND dta_fine      IS NULL
            AND id_old        = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
        condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie = ''I''
            AND dta_fine      IS NULL
            AND id_old        = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
      BEGIN
        -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
        -- includere nell'estensione tutti quei documenti che sono atipici 
        -- a causa della presenza del ruolo per cui si sta calcolando la
        -- visibilit
        If atipicita is not null And length(atipicita) > 2 Then
          Begin
            -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
            Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
            
            -- Calcolo atipicit
            VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
            
            -- Riabilitazione ruolo
            Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
          End;
        End If;
      
        --INIZIO QUARTA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE QUARTA INSERT
          
          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    END IF;
  END;
END;
/
              
----------- FINE -
              
---- SP_EREDITA_VIS_FASC_ATIPICITA.sql  marcatore per ricerca ----
create or replace PROCEDURE @db_user.sp_eredita_vis_fasc_atipicita(
    idcorrglobaleuo    IN NUMBER,
    idcorrglobaleruolo IN NUMBER,
    idgruppo           IN NUMBER,
    livelloruolo       IN NUMBER,
    idregistro         IN NUMBER,
    parilivello        IN NUMBER,
    atipicita          IN VARCHAR,
    returnvalue OUT NUMBER )
IS
BEGIN

--DICHIARAZIONE DELLA STRINGA INSERT INTO SECURITY CHA VALE PER TUTTE LE INSERT DELLA STORED
declare ins_security varchar2(1000):= 'INSERT INTO security(thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto) ';

--DICHIARAZIONE DELLA SELECT PER L'INSERIMENTO CHE VALE PER TUTTE LE INSERT DELLA STORED
select_for_ins varchar2(1000):=
'SELECT /*+ index(s) index(p) */
DISTINCT s.thing,
'||idgruppo||',
(CASE WHEN ( s.accessrights = 255 AND ( s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''A'' OR s.cha_tipo_diritto = ''F'' ) ) THEN 63 ELSE s.accessrights END ) AS acr,
NULL,
''A''
FROM security s, project p ';
          
--DICHIARAZIONE DELLA SELECT PER L'EVENTUALE STORED DI ATIPICITA' CHE VALE PER TUTTE LE PROCEDURE DI ATIPICITA EVENTUALMENTE LANCIATE NELLA STORED
select_for_ins_store varchar2(1000):= 'SELECT /*+ index(s) index(p) */ DISTINCT s.thing FROM security s, project p ';

id_amministrazione number;

  BEGIN
    returnvalue := 0;
    select id_amm into id_amministrazione from dpa_el_registri where system_id = idregistro;
    
      --ATTENZIONE QUESTA PRIMA INSERT E' UN CASO SPECFICO NON GESTITO CON LE VARIABILI SOPRA DEFINITE
      
      --DICHIARAZIONE SELECT SPECIFICCHE PER QUESTO PRIMA INSERT
      declare select_for_ins_1 varchar2(1000):='SELECT thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto FROM ';
      
      select_for_ins_2 varchar2(1000):=
      '(SELECT 
      /*+ index (a) */ 
      DISTINCT a.system_id AS thing, 
      '||idgruppo||' AS personorgroup, 
      255 AS accessrights,
      NULL AS id_gruppo_trasm,
      ''P'' AS cha_tipo_diritto
      FROM project a ';
       
      select_for_ins_3 varchar2(1000):= 
      '(SELECT
      /*+ index (b) */
      DISTINCT b.system_id AS thing,
      '||idgruppo||' AS personorgroup,
      255 AS accessrights,
      NULL AS id_gruppo_trasm,
      ''P'' AS cha_tipo_diritto
      FROM project b ';
        
      --DICHIARAZIONE SELECT PER L'EVENTUALE STORED DI ATIPICITA' SPECIFICHE PER QUESTA INSERT
      select_for_ins_atipicita_1 varchar2(1000):='SELECT thing FROM '; 
       
      select_for_ins_atipicita_2 varchar2(1000):='(SELECT /*+ index (a) */ DISTINCT a.system_id AS thing FROM project a ';
      
      select_for_ins_atipicita_3 varchar2(1000):='(SELECT /*+ index (b) */ DISTINCT b.system_id AS thing FROM project b ';
      
      --DICHIARAZIONE CONDIZIONI DELLA SELECT
      condizioni_select_1 varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=a.system_id)
        AND ( ( a.cha_tipo_proj  = ''T''
        AND ( a.id_registro      = '||idregistro||' OR a.id_registro IS NULL ) )
        OR ( a.cha_tipo_proj     = ''F''
        AND a.cha_tipo_fascicolo = ''G''
        AND ( a.id_registro      = '||idregistro||' OR a.id_registro IS NULL ) ) )
        '
        || atipicita ||      
        '
        )
      UNION ';
      
      condizioni_select_2 varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=b.system_id)
        AND b.cha_tipo_proj = ''C''
        '
        || atipicita ||      
        '
        AND id_parent      IN
          (SELECT
            /*+ index (project) */
            system_id
          FROM project
          WHERE cha_tipo_proj    = ''F''
          AND cha_tipo_fascicolo = ''G''
          AND ( id_registro      = '||idregistro||' OR id_registro IS NULL ))
        ) ';       
        
    BEGIN
      
      --INIZIO PRIMA INSERT       
       execute immediate(ins_security || select_for_ins_1 || select_for_ins_2 || condizioni_select_1 || select_for_ins_3 || condizioni_select_2);
      --FINE SECONDA INSERT
      
      --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      VIS_FASC_ANOMALA_CUSTOM(id_amministrazione,select_for_ins_atipicita_1 || select_for_ins_atipicita_2 || condizioni_select_1 || select_for_ins_atipicita_3 || condizioni_select_2);
      --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA 
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      NULL;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
    IF parilivello = 0 THEN
      
        --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL SECONDO INSERIMENTO
        declare condizioni_select varchar2(10000):=
        'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';      
          
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
          AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';      
          
          BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
          
          --INIZIO SECONDA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE SECONDA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA      
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 2;
        RETURN;
      END;
    ELSE
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL TERZO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id )
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';
    
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id )
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
          AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';
      
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
          
          --INIZIO TERZA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE TERZA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA      
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 3;
        RETURN;
      END;
    END IF;
    IF parilivello = 0 THEN
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUARTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
    
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
          AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
    
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
          
          --INIZIO QUARTA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE QUARTA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA      
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 4;
        RETURN;
      END;
    ELSE
      
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUINTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      ' WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.id_parent IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
          
          condizioni_select_1 varchar2(10000):=
          ' WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.id_parent IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
          AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
          
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
          
          --INIZIO QUINTA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE QUINTA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA           
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 5;
        RETURN;
      END;
    END IF;
  END;
END;
/
              
----------- FINE -
              
---- VIS_DOC_ANOMALA_DOC_NUMBER.ORA.SQL  marcatore per ricerca ----
create or replace procedure @db_user.VIS_DOC_ANOMALA_DOC_NUMBER(p_id_amm NUMBER, p_doc_number in NUMBER, p_codice_atipicita out VARCHAR) IS

--DICHIARAZIONI
s_idg_security NUMBER;
s_ar_security NUMBER;
s_td_security VARCHAR(2);
s_vn_security VARCHAR(255);
s_idg_r_sup NUMBER;
s_id_fascicolo NUMBER;
s_cha_cod_t_a_fasc VARCHAR(1024);
n_id_gruppo NUMBER;

BEGIN

--Cursore sulla security per lo specifico documento
DECLARE CURSOR c_idg_security IS 
SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec 
FROM security 
WHERE 
thing = p_doc_number
AND
accessrights > 20;  
BEGIN OPEN c_idg_security;
LOOP FETCH c_idg_security INTO s_idg_security, s_ar_security, s_td_security, s_vn_security;
EXIT WHEN c_idg_security%NOTFOUND;

    --Gerachia ruolo proprietario documento
    IF(upper(s_td_security) = 'P') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || p_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_doc_number
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita,'AGRP'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGRP-';
            END IF;
        END;
        --COMMIT; 
    END IF;
        
    --Gerarchia destinatario trasmissione
    IF(upper(s_td_security) = 'T') THEN
        DECLARE CURSOR ruoli_sup IS
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
                        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;                   
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || p_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);          
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_doc_number
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGDT'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGDT-';
            END IF;
        END;
		-- can't commit! commit breaks transaction on create protocollo !
        --COMMIT; 
    END IF;
        
    --Fascicolazione documento
    IF(upper(s_td_security) = 'F') THEN
        DECLARE CURSOR fascicoli IS 
        select system_id from project where system_id in(
            select id_fascicolo from project where system_id in (
                select project_id from project_components where link = p_doc_number
                )
        ) and cha_tipo_fascicolo = 'P';
        BEGIN OPEN fascicoli;
        LOOP FETCH fascicoli INTO s_id_fascicolo;
        EXIT WHEN fascicoli%NOTFOUND;
            SELECT cha_cod_t_a INTO s_cha_cod_t_a_fasc FROM project WHERE system_id = s_id_fascicolo;
            IF(s_cha_cod_t_a_fasc is not null AND upper(s_cha_cod_t_a_fasc) <> 'T') THEN
                IF(nvl(instr(p_codice_atipicita, 'AFCD'), 0) = 0) THEN
                    p_codice_atipicita := p_codice_atipicita || 'AFCD-';
                END IF;    
            END IF;
        END LOOP;
        CLOSE fascicoli;
        END;
    END IF;
        
    --Gerarchia ruolo destinatario di copia visibilit
    IF(upper(s_td_security) = 'A' AND upper(s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || p_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);   
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_doc_number
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGCV'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGCV-';
            END IF;
        END;
        --COMMIT;  
    END IF;    

END LOOP;
CLOSE c_idg_security;
END;  

--Restituzione codice di atipicit
IF(p_codice_atipicita is null) THEN
    p_codice_atipicita := 'T';
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || p_doc_number || ' - ' || p_codice_atipicita);
    update PROFILE set CHA_COD_T_A = p_codice_atipicita where DOCNUMBER = p_doc_number;
    --COMMIT;
    RETURN;       
END IF;

IF(substr(p_codice_atipicita, length(p_codice_atipicita)) = '-') THEN
    p_codice_atipicita := substr(p_codice_atipicita, 0, length(p_codice_atipicita)-1);
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || p_doc_number || ' - ' || p_codice_atipicita);
    update PROFILE set CHA_COD_T_A = p_codice_atipicita where DOCNUMBER = p_doc_number;
    
	--COMMIT;
    RETURN;       
END IF;

 
EXCEPTION 
WHEN others THEN
RAISE; 
DBMS_OUTPUT.PUT_LINE('Errore nell''esecuzione della procedura');

END;
/

              
----------- FINE -
              
---- VIS_DOC_ANOMALA_INT_DATE.ORA.SQL  marcatore per ricerca ----
create or replace procedure @db_user.VIS_DOC_ANOMALA_INT_DATE(p_id_amm NUMBER, p_start_date VARCHAR, p_end_date VARCHAR) IS

--DICHIARAZIONI
s_idg_security NUMBER;
s_ar_security NUMBER;
s_td_security VARCHAR(2);
s_vn_security VARCHAR(255);
s_idg_r_sup NUMBER;
s_doc_number NUMBER;
s_id_fascicolo NUMBER;
s_cha_cod_t_a_fasc VARCHAR(1024);
n_id_gruppo NUMBER;
codice_atipicita VARCHAR(255);

BEGIN

--CURSORE DOCUMENTI
DECLARE CURSOR documenti IS select profile.docnumber from profile, people where creation_time between TO_DATE(p_start_date, 'dd/mm/yyyy hh24.mi.ss') and TO_DATE(p_end_date, 'dd/mm/yyyy hh24.mi.ss') AND profile.author = people.system_id AND people.id_amm = p_id_amm;
   
BEGIN OPEN documenti;
LOOP FETCH documenti INTO s_doc_number;
EXIT WHEN documenti%NOTFOUND;

    --Cursore sulla security per lo specifico documento
    DECLARE CURSOR c_idg_security IS 
    SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec 
    FROM security 
    WHERE 
    thing = s_doc_number
    AND
    accessrights > 20;  
    BEGIN OPEN c_idg_security;
    LOOP FETCH c_idg_security INTO s_idg_security, s_ar_security, s_td_security, s_vn_security;
    EXIT WHEN c_idg_security%NOTFOUND;

        --Gerachia ruolo proprietario documento
        IF(upper(s_td_security) = 'P') THEN
            DECLARE CURSOR ruoli_sup IS 
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;
                --DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || s_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_doc_number
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita,'AGRP'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGRP-';
                END IF;
            END;
            COMMIT; 
        END IF;
        
        --Gerarchia destinatario trasmissione
        IF(upper(s_td_security) = 'T') THEN
            DECLARE CURSOR ruoli_sup IS
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
                        
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;                   
                --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);          
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_doc_number
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita, 'AGDT'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGDT-';
                END IF;
            END;
            COMMIT; 
        END IF;
        
        --Fascicolazione documento
        IF(upper(s_td_security) = 'F') THEN
            DECLARE CURSOR fascicoli IS 
            select system_id from project where system_id in(
                select id_fascicolo from project where system_id in (
                    select project_id from project_components where link = s_doc_number
                    )
            ) and cha_tipo_fascicolo = 'P';
            BEGIN OPEN fascicoli;
            LOOP FETCH fascicoli INTO s_id_fascicolo;
            EXIT WHEN fascicoli%NOTFOUND;
                SELECT cha_cod_t_a INTO s_cha_cod_t_a_fasc FROM project WHERE system_id = s_id_fascicolo;
                IF(s_cha_cod_t_a_fasc is not null AND upper(s_cha_cod_t_a_fasc) <> 'T') THEN
                    IF(nvl(instr(codice_atipicita, 'AFCD'), 0) = 0) THEN
                        codice_atipicita := codice_atipicita || 'AFCD-';
                    END IF;    
                END IF;
            END LOOP;
            CLOSE fascicoli;
            END;
        END IF;
        
        --Gerarchia ruolo destinatario di copia visibilit
        IF(upper(s_td_security) = 'A' AND upper(s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA') THEN
            DECLARE CURSOR ruoli_sup IS 
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;
                --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);   
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_doc_number
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita, 'AGCV'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGCV-';
                END IF;
            END;
            COMMIT;  
        END IF;    

    END LOOP;
    CLOSE c_idg_security;
    END;  

    --Restituzione codice di atipicit
    IF(codice_atipicita is null) THEN
        codice_atipicita := 'T';
        --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || s_doc_number || ' - ' || codice_atipicita);
        update PROFILE set CHA_COD_T_A = codice_atipicita where DOCNUMBER = s_doc_number;
        COMMIT;
        codice_atipicita := null;        
    END IF;

    IF(substr(codice_atipicita, length(codice_atipicita)) = '-') THEN
        codice_atipicita := substr(codice_atipicita, 0, length(codice_atipicita)-1);
        --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Documento ' || s_doc_number || ' - ' || codice_atipicita);
        update PROFILE set CHA_COD_T_A = codice_atipicita where DOCNUMBER = s_doc_number;
        COMMIT;
        codice_atipicita := null;        
    END IF;

END LOOP;
CLOSE documenti;
END;
    
EXCEPTION 
WHEN others THEN
DBMS_OUTPUT.PUT_LINE('Errore nell''esecuzione della procedura');

END;
/              
----------- FINE -
              
---- VIS_FASC_ANOMALA_CUSTOM.ORA.SQL  marcatore per ricerca ----
create or replace procedure @db_user.VIS_FASC_ANOMALA_CUSTOM (p_id_amm NUMBER, p_queryFasc VARCHAR) IS

--DICHIARAZIONI
s_idg_security NUMBER;
s_ar_security NUMBER;
s_td_security VARCHAR(2);
s_vn_security VARCHAR(255);
s_idg_r_sup NUMBER;
n_id_gruppo NUMBER;
s_id_fascicolo NUMBER;
codice_atipicita VARCHAR(255);

BEGIN

--CURSORE FASCICOLI
DECLARE
   TYPE EmpCurTyp IS REF CURSOR;  
   fascicoli   EmpCurTyp;
BEGIN
   OPEN fascicoli FOR p_queryFasc;
   
LOOP FETCH fascicoli INTO s_id_fascicolo;
EXIT WHEN fascicoli%NOTFOUND;
 
    --Cursore sulla security per lo specifico fascicolo
    DECLARE CURSOR c_idg_security IS 
    SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec 
    FROM security 
    WHERE 
    thing = s_id_fascicolo
    AND
    accessrights > 20;  
    BEGIN OPEN c_idg_security;
    LOOP FETCH c_idg_security INTO s_idg_security, s_ar_security, s_td_security, s_vn_security;
    EXIT WHEN c_idg_security%NOTFOUND;

        --Gerachia ruolo proprietario del fascicolo
        IF(upper(s_td_security) = 'P') THEN
            DECLARE CURSOR ruoli_sup IS 
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
            
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;
                --DBMS_OUTPUT.PUT_LINE('FASCICOLO : ' || s_id_fascicolo || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_id_fascicolo
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita,'AGRP'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGRP-';
                END IF;
            END;
            COMMIT; 
        END IF;

        
        --Gerarchia destinatario trasmissione
        IF(upper(s_td_security) = 'T') THEN
            DECLARE CURSOR ruoli_sup IS
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
                            
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;                   
                --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);          
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_id_fascicolo
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita, 'AGDT'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGDT-';
                END IF;
            END;
            COMMIT; 
        END IF;


        --Gerarchia ruolo destinatario di copia visibilit
        IF(upper(s_td_security) = 'A' AND upper(s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA') THEN
            DECLARE CURSOR ruoli_sup IS 
            SELECT dpa_corr_globali.id_gruppo 
            FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
            WHERE
                dpa_corr_globali.id_uo in (
                    SELECT dpa_corr_globali.system_id
                    FROM dpa_corr_globali
                    WHERE 
                    dpa_corr_globali.dta_fine IS NULL
                    CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                    )
            AND
            dpa_corr_globali.CHA_TIPO_URP = 'R'
            AND
            dpa_corr_globali.ID_AMM = p_id_amm
            AND
            dpa_corr_globali.DTA_FINE IS NULL
            AND 
            dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
            
            BEGIN OPEN ruoli_sup;
            LOOP FETCH ruoli_sup INTO s_idg_r_sup;
            EXIT WHEN ruoli_sup%NOTFOUND;
                --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);   
            END LOOP;
            CLOSE ruoli_sup;
            END;
            --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
            --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
            BEGIN
                n_id_gruppo := 0;
                SELECT COUNT(*) INTO n_id_gruppo FROM
                (
                SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
                MINUS
                SELECT PERSONORGROUP FROM SECURITY WHERE THING = s_id_fascicolo
                );
                IF(n_id_gruppo <> 0 AND nvl(instr(codice_atipicita, 'AGCV'), 0) = 0) THEN
                    codice_atipicita := codice_atipicita || 'AGCV-';
                END IF;
            END;
            COMMIT;  
        END IF;    


    END LOOP;
    CLOSE c_idg_security;
    END; 

    --Restituzione codice di atipicit
    IF(codice_atipicita is null) THEN
        codice_atipicita := 'T';
        --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
        update PROJECT set CHA_COD_T_A = codice_atipicita where SYSTEM_ID = s_id_fascicolo;
        COMMIT;
        codice_atipicita := null;    
    END IF;

    IF(substr(codice_atipicita, length(codice_atipicita)) = '-') THEN
        codice_atipicita := substr(codice_atipicita, 0, length(codice_atipicita)-1);
        --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
        update PROJECT set CHA_COD_T_A = codice_atipicita where SYSTEM_ID = s_id_fascicolo;
        COMMIT;
        codice_atipicita := null;      
    END IF;

END LOOP;
CLOSE fascicoli;
END;

EXCEPTION 
WHEN others THEN
DBMS_OUTPUT.PUT_LINE('Errore nell''esecuzione della procedura');

END;
/

              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
Values    (seq.nextval, sysdate, '3.17.1');
end;
/              
