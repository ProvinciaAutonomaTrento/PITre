
begin
    declare   cnt int;
    begin        
select count(*) into cnt from all_tables where owner='@db_user' 
	AND table_name='DPA_REL_PEOPLE_EXTAPPS';
             if (cnt = 0) then
                    execute immediate   
                           'CREATE TABLE @db_user.DPA_REL_PEOPLE_EXTAPPS
					(ID_PEOPLE  int		NOT NULL,
					 ID_EXT_APP int		NOT NULL,
					 CONSTRAINT PK_DPA_REL_PEOPLE_EXTAPPS 
					  PRIMARY KEY 	(ID_PEOPLE,	ID_EXT_APP) )';

			end if;
	end;
end;
/

