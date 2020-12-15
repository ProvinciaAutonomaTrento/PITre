--ALTER TABLE DOCSADM.PROFILE ADD	COD_EXT_APP varchar(32) NULL
begin
 @db_user.utl_add_column ('3.22', '@db_user'
 , 'PROFILE', 'COD_EXT_APP', 'varchar2(32)', NULL, NULL, NULL, NULL );
end;
/

begin
utl_add_index ('3.22', '@db_user'
 , 'PROFILE', 'COD_EXT_APP',  NULL, NULL, NULL, NULL );
end;
/
