
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_TEMPLATES';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_TEMPLATES' and column_name='EXT_MOD_1';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_TEMPLATES ADD EXT_MOD_1 char(10)';
		end if;
	end if;
	end;
end;
/ 
