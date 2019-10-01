BEGIN
   -- by Veltri per import pregressi
   utl_add_column '3.23','@db_user',
                   'DPA_EL_REGISTRI',
                   'VAR_PREG',
                   'CHAR(1 BYTE)',
                   NULL,NULL,NULL,NULL

   utl_add_column '3.23','@db_user',
                   'DPA_EL_REGISTRI',
                   'ANNO_PREG', 
                   'VARCHAR2(10 BYTE)',
                   NULL,NULL,NULL,NULL
END
GO

