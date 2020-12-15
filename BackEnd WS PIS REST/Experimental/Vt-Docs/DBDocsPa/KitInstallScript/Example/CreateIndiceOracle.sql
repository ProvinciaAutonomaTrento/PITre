--nomi tabelle e indici sempre maiuscoli !!!!
begin
	declare
		   cnt int;
	begin	
select count(*) into cnt from user_indexes where table_name='PEOPLE' and index_name='IDX_PEOPLE';
		if (cnt = 0) then
			execute immediate 'CREATE INDEX IDX_PEOPLE ON PEOPLE (UPPER(FULL_NAME)) TABLESPACE @ora_idxtblspc_name LOGGING NOPARALLEL';
		end if;
					end;
end;
/