begin
    declare cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_PREGRESSI';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_PREGRESSI START WITH 1 
				MAXVALUE 999999999999999999999999999 
				MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_PREGRESSI';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_PREGRESSI ( ' ||
            'SYSTEM_ID 			INTEGER NOT NULL, ' ||
            'ID_AMM 			INTEGER , ' ||
			'ID_UTENTE_CREATORE INTEGER , ' ||
			'ID_RUOLO_CREATORE 	INTEGER , ' ||
			'DATA_ESECUZIONE 	DATE, ' ||
			'DATA_FINE 			DATE, ' ||
			'NUM_DOC 			INTEGER , ' ||
			'DESCRIZIONE		VARCHAR2(1000 BYTE), '||
            'CONSTRAINT PK_DPA_PREGRESSI PRIMARY KEY (SYSTEM_ID) )';
        end if;
    end;    
end;    
/

