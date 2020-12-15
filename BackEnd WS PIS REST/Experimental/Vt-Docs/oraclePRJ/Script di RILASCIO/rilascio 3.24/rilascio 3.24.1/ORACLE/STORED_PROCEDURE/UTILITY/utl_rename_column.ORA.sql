begin 
	utl_backup_sp ('UTL_RENAME_COLUMN','3.24');
end;
/

create or replace PROCEDURE             UTL_RENAME_COLUMN 
(  versione_CD                      VARCHAR2,
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
   Then
   -- dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna_old)
             And Owner = Nomeutente;
    --dbms_output.put_line ('cnt vale: '||cnt);
      IF (cnt = 1) THEN       -- ok la colonna esiste, la modifico
      --   dbms_output.put_line ('ok entro nel 2 if ');
         EXECUTE IMMEDIATE   'ALTER TABLE ' || nomeutente || '.'
                          || nome_tabella || ' RENAME COLUMN ' || nome_colonna_old
                          || ' TO ' || nome_colonna_new;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD , 'esito positivo' ); 
      END IF;
   END IF;
   
    dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  
   WHEN OTHERS   THEN
               Dbms_Output.Put_Line ('errore '||Sqlerrm);
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD                         , 'esito negativo' ); 
                         -- RAISE;
End Utl_Rename_Column; 
/
