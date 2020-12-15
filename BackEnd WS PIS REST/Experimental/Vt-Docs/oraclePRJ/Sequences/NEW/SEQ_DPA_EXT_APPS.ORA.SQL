BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_EXT_APPS';
    IF (cnt = 0) THEN        
       execute immediate 
	   ' CREATE SEQUENCE @db_user.SEQ_DPA_EXT_APPS START WITH 1 INCREMENT BY 1 MINVALUE 1';
    END IF;
END;        
END;
/