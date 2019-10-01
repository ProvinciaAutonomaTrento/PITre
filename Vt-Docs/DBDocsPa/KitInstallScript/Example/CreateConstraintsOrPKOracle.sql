--nomi tabelle e indici sempre maiuscoli !!!!

-- constraint_type='U'  = UNIQUE
-- constraint_type='C'   = Generic (es. Not Null)
-- constraint_type='R'   = reference 
-- constraint_type='P'   =  Primary Key
begin
	declare
		   cnt int;
	begin
select count(*) into cnt from user_constraints, user_cons_columns where user_cons_columns.table_name = 'DPA_CANALI_REG' and  constraint_type='P' and user_cons_columns.constraint_name = user_constraints.constraint_name;
		if (cnt = 0) then  	  
			execute immediate 'ALTER TABLE DPA_CANALI_REG ADD PRIMARY KEY (SYSTEM_ID)';
		end if;
					end;
end;
/

--oppure con controllo sulle colonne , che è più sicuro
begin
	declare
		   cnt int;
	begin
select count(*) into cnt from user_constraints, user_cons_columns where 
				user_cons_columns.table_name = 'SECURITY' and  
				column_name IN ( 'THING',
				'PERSONORGROUP' 
				, 'ACCESSRIGHTS' ) and
				constraint_type='U' and 
				user_cons_columns.constraint_name = user_constraints.constraint_name;
		if (cnt = 0) then  	  
			execute immediate 'ALTER TABLE SECURITY ADD CONSTRAINT INDX_SEC_PK primary key (THING, PERSONORGROUP, ACCESSRIGHTS)';
		end if;  
							end;
end;
/