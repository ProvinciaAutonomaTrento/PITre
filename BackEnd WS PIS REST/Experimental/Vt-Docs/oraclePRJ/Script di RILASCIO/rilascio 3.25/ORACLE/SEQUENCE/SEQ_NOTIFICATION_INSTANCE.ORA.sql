--CREATE SEQUENCE  SEQ_NOTIFICATIONINSTANCE  
--MINVALUE 1 MAXVALUE 9999999999999999999999999999 
--INCREMENT BY 1 START WITH 12355881 CACHE 20 NOORDER  NOCYCLE ;


BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_NOTIFICATIONINSTANCE';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

	select count(*) INTO cnt from cols 
		where table_name='NOTIFICATIONINSTANCE' 
		and column_name='ID';

		IF (cnt = 1) THEN   -- ok, la colonna esiste 
		
		  Select max(maxid) +1 INTO cnt From (
				Select Max(id) as maxid From NOTIFICATIONINSTANCE
				Union Select 0 From Dual ) ;   
			
		   execute immediate 'CREATE SEQUENCE @db_user.SEQ_NOTIFICATIONINSTANCE
				MINVALUE 1  MAXVALUE 9999999999999999999999999999 
				INCREMENT BY 1  START WITH '||cnt||
				' CACHE 20 NOORDER NOCYCLE ';
	
		END IF;
    END IF;
END;        
END;
/

