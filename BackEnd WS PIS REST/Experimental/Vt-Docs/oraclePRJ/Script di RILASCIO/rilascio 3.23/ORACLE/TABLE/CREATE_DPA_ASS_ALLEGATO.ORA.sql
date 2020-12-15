begin
-- import pregressi by Veltri        
	declare cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_ASS_ALLEGATO';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_ASS_ALLEGATO 
		   START WITH 1 MAXVALUE 999999999999999999999999999 
		   MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_ASS_ALLEGATO';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_ASS_ALLEGATO ( ' ||
			'SYSTEM_ID			INTEGER, ' ||
            'ID_ITEM	 		INTEGER, ' ||
			'ERRORE 			VARCHAR2(1000 BYTE), ' ||
			'ESITO 				CHAR(1 BYTE)    )';
        end if;
    end;    
end;    
/


BEGIN
utl_add_index('3.23','@db_user',
	'DPA_ASS_ALLEGATO','IX_T_DPA_ASS_ALLEGATOK2',null, 
    'ID_ITEM',null,null,null,   -- lista colonne
    'NORMAL', null, null, null     );                
END;
/
