begin
	declare cnt int;
    begin

        select count(*) into cnt from user_tables
				where lower(table_name)='utl_backup_plsql';
        if (cnt = 0) then
          execute immediate
            'CREATE TABLE UTL_backup_plsql
			 (VAR_DICT_OBJ_TYPE VARCHAR2(200 BYTE),
                         VAR_DICT_OBJ_NAME VARCHAR2(200 BYTE),
                         Var_Pitre_Title   Varchar2(200 Byte),
                         Dta_Of_Change Date,
                         CLOB_CODE_SAFE_TO_RESTORE Clob   ) ';

        end if;
    end;    
end;    
/


