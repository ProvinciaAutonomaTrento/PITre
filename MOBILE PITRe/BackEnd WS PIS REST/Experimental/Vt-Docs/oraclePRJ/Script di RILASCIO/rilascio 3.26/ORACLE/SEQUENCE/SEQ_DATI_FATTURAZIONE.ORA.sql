

--Create Sequence  SEQ_DATI_FATTURAZIONE  
   --Minvalue 1 Maxvalue 999999999999999999999999999 
   --Increment By 1 Start With 1 Cache 20 Noorder  Nocycle ;

BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DATI_FATTURAZIONE';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

	select count(*) INTO cnt from cols 
		where table_name='DPA_DATI_FATTURAZIONE' 
		and column_name='SYSTEM_ID';

		IF (cnt = 1) THEN   -- ok, la colonna esiste 
		
		  Select max(maxid) +1 INTO cnt From (
				Select Max(System_id) as maxid From DPA_DATI_FATTURAZIONE
				Union Select 0 From Dual ) ;   
			
		   execute immediate 'CREATE SEQUENCE @db_user.SEQ_DATI_FATTURAZIONE  
					Minvalue 1 Maxvalue 999999999999999999999999999 
					Increment By 1 Start With '||cnt||' Cache 20 Noorder  Nocycle ';
	END IF;
    END IF;
END;        
END;
/


