begin
-- import pregressi by Veltri    
	declare cnt int;
    begin
        select count(*) into cnt 
		from user_sequences 
			where sequence_name='SEQ_DPA_ASS_PREGRESSI';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_ASS_PREGRESSI 
		   START WITH 1 
		   MAXVALUE 999999999999999999999999999 
		   MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_ASS_PREGRESSI';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_ASS_PREGRESSI ( ' ||
			'SYSTEM_ID			INTEGER , ' ||
            'ID_PREGRESSO 		INTEGER , ' ||
            'ID_REGISTRO 		INTEGER , ' ||
			'ID_DOCUMENTO 		INTEGER , ' ||
			'ID_UTENTE 			INTEGER , ' ||
			'ID_RUOLO 			INTEGER , ' ||
			'TIPO_OPERAZIONE 	CHAR(1 BYTE), ' ||
			'DATA 				DATE, ' ||
			'ERRORE 			VARCHAR2(1000 BYTE), ' ||
			'ESITO 				CHAR(1 BYTE), ' ||
			'ID_NUM_PROTO_EXCEL VARCHAR2(255 BYTE) )';
        end if;
    end;    
end;    
/

BEGIN
utl_add_index( '3.23','@db_user',
			'DPA_ASS_PREGRESSI','IX_T_DPA_ASS_PREGRESSIK1','UNIQUE', 
				'SYSTEM_ID',null,null,null, -- lista colonne
				'NORMAL',null, null, null     );                

utl_add_index('3.23','@db_user',
			'DPA_ASS_PREGRESSI','IX_T_DPA_PREGRESSIK2',null, 
				'ID_PREGRESSO',null,null,null,  -- lista colonne
				'NORMAL', null, null, null     );                
END;
/



