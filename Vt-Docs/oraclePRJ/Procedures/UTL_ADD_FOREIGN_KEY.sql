--------------------------------------------------------
--  DDL for Procedure UTL_ADD_FOREIGN_KEY
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_ADD_FOREIGN_KEY" 
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
                                            ('||nome_colonna_fk||')'; 
                                            
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
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Ci sono i records che violano la chiave' );
        

         END IF;
         ELSE                         
        
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
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
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_foreign_key; 

/
