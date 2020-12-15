BEGIN
-- by Ferlito/ Panici 
-- MEV CF P. IVA

--	ALTER TABLE DPA_DETT_GLOBALI ADD (VAR_COD_PI VARCHAR2(11 CHAR) );
   utl_add_column ('3.23',
                   '@db_user',
                   'DPA_DETT_GLOBALI',
                   'VAR_COD_PI',
                   'VARCHAR2(11 CHAR)',
                   NULL,NULL,NULL,NULL);

--	ALTER TABLE DPA_DETT_GLOBALI RENAME COLUMN VAR_COD_FIS TO VAR_COD_FISC;
utl_rename_column 
				('3.23',
                   '@db_user',
                   'DPA_DETT_GLOBALI',
                   'VAR_COD_FIS'  ,
				   'VAR_COD_FISC'  ,
				   NULL);
	END;
/

