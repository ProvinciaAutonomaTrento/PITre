--------------------------------------------------------
--  DDL for Procedure UTL_ADD_PRIMARY_KEY
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_ADD_PRIMARY_KEY" 
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
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave primaria gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);

         END IF;
          ELSE                         
         dbms_output.put_line ('errore, esiste gi ');    
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
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
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_primary_key; 

/
