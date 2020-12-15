--ALTER TABLE DOCSADM.PROJECT ADD	COD_EXT_APP varchar(32) NULL
begin
 @db_user.utl_add_column ('3.22', '@db_user'
 , 'PROJECT', 'COD_EXT_APP', 'varchar2(32)', NULL, NULL, NULL, NULL );
end;
/
