begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_JOBS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_JOBS
  (
    ID NUMBER(10, 0) NOT NULL ,
    CONSTRAINT DPA_JOBS_PK PRIMARY KEY ( ID ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_CHECK_MAILBOX';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_CHECK_MAILBOX
  (
    ID           NUMBER(10, 0) NOT NULL ,
    IDJOB        NUMBER(10, 0) NOT NULL ,
    IDUSER       NUMBER(10, 0) NOT NULL ,
    IDROLE       NUMBER(10, 0) NOT NULL ,
    IDREG        NUMBER(10, 0) NOT NULL ,
    MAIL         VARCHAR2(200 CHAR) NOT NULL ,
    ELABORATE    NUMBER(5, 0) DEFAULT 0 ,
    TOTAL        NUMBER(5, 0) DEFAULT 0 ,
    CONCLUDED    VARCHAR2(1 CHAR) DEFAULT 0 NOT NULL ,
    MAILUSERID   VARCHAR2(1000 CHAR) ,
    ERRORMESSAGE VARCHAR2(4000 BYTE) ,
	MAILSERVER VARCHAR2(1000 CHAR),
    CONSTRAINT DPA_CHECK_MAIL_PK PRIMARY KEY ( ID ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	execute immediate
		'ALTER TABLE DPA_CHECK_MAILBOX ADD CONSTRAINT DPA_CHECK_MAILBOX_FK FOREIGN KEY ( IDJOB ) REFERENCES DPA_JOBS ( ID ) ON
DELETE CASCADE ENABLE';
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_REPORT_MAILBOX';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_REPORT_MAILBOX
  (
    ID               NUMBER(10, 0) NOT NULL ,
    ID_CHECK_MAILBOX NUMBER(10, 0) NOT NULL ,
    MAILID           VARCHAR2(200 CHAR) ,
    TYPE             VARCHAR2(100 CHAR) ,
    RECEIPT          VARCHAR2(200) ,
    DATE_MAIL DATE ,
    FROM_MAIL         VARCHAR2(200 CHAR) ,
    ERROR             VARCHAR2(4000 BYTE) ,
    COUNT_ATTACHMENTS NUMBER(5, 0) ,
    SUBJECT           VARCHAR2(4000 BYTE) ,
    CONSTRAINT DPA_REPORT_MAILBOX_PK PRIMARY KEY ( ID , ID_CHECK_MAILBOX ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	execute immediate
		'ALTER TABLE DPA_REPORT_MAILBOX ADD CONSTRAINT DPA_REPORT_MAILBOX_FK FOREIGN KEY ( ID_CHECK_MAILBOX ) REFERENCES DPA_CHECK_MAILBOX ( ID ) ON
DELETE CASCADE ENABLE';
end;
/
