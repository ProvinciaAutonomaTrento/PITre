BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY_HISTORY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY_HISTORY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/