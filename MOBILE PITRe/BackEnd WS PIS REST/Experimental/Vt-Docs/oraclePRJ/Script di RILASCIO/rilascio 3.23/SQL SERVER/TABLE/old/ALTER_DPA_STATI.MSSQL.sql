BEGIN
-- by C. Ferlito per albo
   utl_add_column '3.23',
                   '@db_user',
                   'DPA_STATI',
                   'Cha_Stato_Sistema',
                   'CHAR(1)',
                   '0',NULL,NULL,NULL
END
GO


