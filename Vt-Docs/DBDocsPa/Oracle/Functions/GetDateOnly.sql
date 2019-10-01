CREATE OR REPLACE FUNCTION @db_user.GetDateOnly ( i_data VARCHAR2 ) Return varchar2 IS
pos_blank  integer;
o_data	     varchar(50);
BEGIN
pos_blank := INSTR(i_data, ' ', 1, 1);
if pos_blank > 0 then
o_data := substr(i_data, 1, pos_blank - 1);
else
o_data := i_data;
end if;

RETURN o_data;
END GetDateOnly;
/