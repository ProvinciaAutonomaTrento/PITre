--------------------------------------------------------
--  DDL for Procedure UTL_DROP_COLUMN
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_DROP_COLUMN" 
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
                         ,'Droped column '||nome_colonna||' from table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_drop_column  ; 

/
