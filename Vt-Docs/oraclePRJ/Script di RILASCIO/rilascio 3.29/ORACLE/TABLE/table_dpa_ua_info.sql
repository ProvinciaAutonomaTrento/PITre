begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_UA_INFO';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_UA_INFO
(
  USER_ID          VARCHAR2(32),
  ID_PEOPLE        NUMBER,
  DTA_LOGIN        DATE,
  IP               VARCHAR2(20),
  BROWSER_TYPE     VARCHAR2(20),
  BROWSER_VERSION  VARCHAR2(10),
  ACTIVEX          CHAR(1),
  JAVASCRIPT       CHAR(1),
  JAVAAPPLET       CHAR(1),
  USER_AGENT       VARCHAR2(256)
)';
		end if;
	end;
end;
/



