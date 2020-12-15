begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_FATT_ATTIVA_COD_FORNITORE';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_FATT_ATTIVA_COD_FORNITORE
			(
				ID_AMM    			NUMBER(12, 0) NOT NULL ,
				ID_REGISTRO			NUMBER(12, 0) NOT NULL ,
				COD_FORNITORE       VARCHAR2(255 CHAR),
				COD_FASCICOLO 		VARCHAR2(255 CHAR),
				CODICE_AMM_IPA		VARCHAR2(255 CHAR)
			)';
		end if;
	end;
end;
/