--------------------------------------------------------
--  DDL for Procedure UTL_MODIFY_COLUMN
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_MODIFY_COLUMN" 
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
      istruzsql varchar2(200); 
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;

   IF (cnt = 1) THEN    -- ok la tabella esiste
   
      SELECT COUNT ( * )        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND owner = nomeutente;
  
       IF (cnt = 1) THEN      -- ok la colonna esiste, la modifico
         
		 if val_default  IS null then 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' MODIFY '      || nome_colonna|| ' '|| tipo_dato ; 
		  ELSE --  val_default  IS NOT null 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' MODIFY '      || nome_colonna|| ' '|| tipo_dato || ' default '|| val_default   ; 

			end if ; 
         EXECUTE IMMEDIATE  istruzsql  ;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
      END IF;
   END IF;
   
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
    
               dbms_output.put_line ('errore '||SQLERRM);
               utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                    --   RAISE;
END utl_modify_column;

/
