--------------------------------------------------------
--  DDL for Procedure UTL_RENAME_COLUMN
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_RENAME_COLUMN" 
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
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_rename_column; 

/
