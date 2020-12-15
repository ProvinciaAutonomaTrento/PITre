begin
	declare
		   cnt int;
	begin	
		select count(*) into cnt from user_sequences where sequence_name='SEQ';
		if (cnt = 0) then
		   select nvl((max(last_number)+1),1) into cnt from user_sequences;
		   execute immediate 'CREATE SEQUENCE SEQ START WITH ' || cnt ||' MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
		end if;	
			end;
end;
/