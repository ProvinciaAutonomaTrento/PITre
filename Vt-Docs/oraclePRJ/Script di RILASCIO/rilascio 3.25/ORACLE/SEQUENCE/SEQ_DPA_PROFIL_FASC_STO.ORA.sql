--CREATE SEQUENCE  SEQ_DPA_PROFIL_FASC_STO
--  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1
--  START WITH 302
--  CACHE 20 NOORDER  NOCYCLE ;


BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_PROFIL_FASC_STO';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

	select count(*) INTO cnt from cols 
		where table_name='DPA_PROFIL_FASC_STO' 
		and column_name='SYSTEMID';

		IF (cnt = 1) THEN   -- ok, la colonna esiste 
		
		  Select max(maxid) +1 INTO cnt From (
				Select Max(Systemid) as maxid From Dpa_Profil_Fasc_Sto
				Union Select 0 From Dual ) ;   
			
		   execute immediate 'CREATE SEQUENCE @db_user.SEQ_DPA_PROFIL_FASC_STO
				MINVALUE 1  MAXVALUE 9999999999999999999999999999 
				INCREMENT BY 1  START WITH '||cnt||
				' CACHE 20 NOORDER NOCYCLE ';
	
		END IF;
    END IF;
END;        
END;
/

