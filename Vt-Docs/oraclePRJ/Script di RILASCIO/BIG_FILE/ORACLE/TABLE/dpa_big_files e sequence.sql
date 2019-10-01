begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_BIG_FILES';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_BIG_FILES
			(
				SYSTEM_ID          		NUMBER(12, 0) NOT NULL ,
				IdProfile    			NUMBER(12, 0) NOT NULL ,
				Id_amm					NUMBER(12, 0) NOT NULL ,
				Id_group	     		NUMBER(12, 0) NOT NULL ,
				Id_people	 			NUMBER(12, 0) NOT NULL ,
				FileName		       	VARCHAR2(2000 CHAR),
				FileSize				VARCHAR2(2000 CHAR),
				HashFile				VARCHAR2(2000 CHAR),
				PathFTP					VARCHAR2(2000 CHAR),
				FileUpStatus			VARCHAR2(256 CHAR),
				VAR_ERRORMESSAGE		VARCHAR2(2000 CHAR),
				VersionId				NUMBER(12, 0) NOT NULL ,
				Path_FS					VARCHAR2(2000 CHAR),
				DTA_CODA				DATE,
				DTA_UPLOAD				DATE,
				CONSTRAINT "DPA_BIG_FILES" PRIMARY KEY ("SYSTEM_ID") ENABLE
			)';
		end if;
	end;
end;
/


CREATE SEQUENCE SEQ_DPA_BIG_FILES INCREMENT BY 1 START WITH 1;