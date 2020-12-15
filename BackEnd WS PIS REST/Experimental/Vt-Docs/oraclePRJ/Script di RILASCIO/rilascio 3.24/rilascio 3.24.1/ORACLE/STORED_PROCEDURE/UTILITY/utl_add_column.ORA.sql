begin 
	Utl_Backup_Plsql_code ('PROCEDURE','utl_add_column'); 
end;
/

create or replace
PROCEDURE utl_add_column (
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
   istruzsql varchar2(200); 
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
   --dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND UPPER(owner) = nomeutente;
      IF (cnt = 0)
      -- ok la colonna non esiste, la aggiungo
      THEN
   --      dbms_output.put_line ('ok entro nel 2° if ');

		 if val_default  IS null then 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' ADD '      || nome_colonna|| ' '|| tipo_dato ; 
		  ELSE --  val_default  IS NOT null 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
					|| ' ADD '      || nome_colonna|| ' '|| tipo_dato || ' default '|| val_default   ; 

			end if ; 
         EXECUTE IMMEDIATE  istruzsql  ;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
                         
        ELSE  -- cnt=1,    la colonna esiste già 
             utl_modify_column (versione_CD                      
				   ,nomeutente  ,nome_tabella                     ,nome_colonna                     
				   ,tipo_dato                        
				   ,val_default                      
				   ,condizione_check                 
				   ,RFU                              ) ; 
			  
			  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'column modified, was already there' );
			  
      END IF;
   END IF;
   
            
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      
               dbms_output.put_line ('errore '||SQLERRM);
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                        -- RAISE;
               
End Utl_Add_Column;
/

