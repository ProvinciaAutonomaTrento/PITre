begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_FATT_ATTIVA_LOG_PIS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_FATT_ATTIVA_LOG_PIS
			(
				ID_PROFILE			NUMBER(12, 0) NOT NULL ,
				ID_UO				NUMBER(12, 0) NOT NULL ,
				COD_UO		        VARCHAR2(255 CHAR),
				DTA_CREAZIONE 		DATE
			)';
		end if;
	end;
end;
/