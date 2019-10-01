begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_FATTURA_TIBCO_LOG';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_FATTURA_TIBCO_LOG
			(
				ID_TIBCO    			VARCHAR2(255 CHAR),
				ID_SDI					VARCHAR2(255 CHAR),
				DOCNUMBER          		NUMBER(12, 0) NOT NULL ,
				PASSOERRORE				VARCHAR2(255 CHAR),
				ESITO					VARCHAR2(255 CHAR),
				
				CONSTRAINT "DPA_INTEGR_FATTURA_S_PK" PRIMARY KEY ("DOCNUMBER") ENABLE
			)';
		end if;
	end;
end;
/