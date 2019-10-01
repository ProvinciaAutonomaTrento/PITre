begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='groups';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='groups' and column_name='group_name';
  		if (cnt != 0) then
        	execute immediate 'alter table groups modify group_name VARCHAR2(128)';
		end if;
	end if;
	end;
end;
/        
