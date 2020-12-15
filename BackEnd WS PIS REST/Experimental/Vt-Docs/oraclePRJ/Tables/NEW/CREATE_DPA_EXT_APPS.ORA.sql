
begin
    declare   cnt int;
    begin        
		select count(*) into cnt 
			from all_tables where owner='@db_user' 
				AND table_name='DPA_EXT_APPS';
             if (cnt = 0) then
                    execute immediate   
                    'CREATE TABLE @db_user.DPA_EXT_APPS
						(SYSTEM_ID		int				NOT NULL, 
						 VAR_CODE		varchar2(32)		NOT NULL,
						 DESCRIPTION	varchar2(512)	NOT NULL,
						 constraint PK_DPA_EXT_APPS PRIMARY KEY 	(SYSTEM_ID) 	)';

			end if;
		end; 
end; 
/


