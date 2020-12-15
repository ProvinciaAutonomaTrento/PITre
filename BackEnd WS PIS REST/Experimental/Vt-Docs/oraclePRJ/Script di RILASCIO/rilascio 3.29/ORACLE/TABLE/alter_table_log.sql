
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='VAR_CODICE';

if cntcol > 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG MODIFY (VAR_CODICE VARCHAR2(256 CHAR))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='MULTIPLICITY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (MULTIPLICITY CHAR(3 CHAR))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='ID_AMM';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (ID_AMM  NUMBER(10,0) DEFAULT NULL)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='NOTIFICATION';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (NOTIFICATION  CHAR(1 CHAR) DEFAULT ''0'')';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='CONFIGURABLE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (CONFIGURABLE  CHAR(1 CHAR) DEFAULT ''0'')';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='NOTIFICATION_RECIPIENTS';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (NOTIFICATION_RECIPIENTS VARCHAR2(200 CHAR) DEFAULT NULL)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='COLOR';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (COLOR                   VARCHAR2(200 CHAR) DEFAULT ''BLUE'')';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG_ATTIVATI' and column_name='NOTIFY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG_ATTIVATI ADD (NOTIFY VARCHAR2(3 CHAR) DEFAULT ''NN'' )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='ID_TRASM_SINGOLA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (ID_TRASM_SINGOLA NUMBER(10, 0) )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='CHECK_NOTIFY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (CHECK_NOTIFY  CHAR(1 CHAR) DEFAULT 0 )';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='DESC_PRODUCER';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (DESC_PRODUCER VARCHAR2(500 CHAR) DEFAULT '''' )';
end if;

end;
END;
/

