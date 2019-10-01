-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'

--	ALTER TABLE DPA_DETT_GLOBALI ADD (VAR_COD_PI VARCHAR2(11 CHAR) );
	EXECUTE DOCSADM.utl_add_column '3.23',@db_user,'DPA_DETT_GLOBALI','VAR_COD_PI','VARCHAR(11)',NULL,NULL,NULL,NULL

--	ALTER TABLE DPA_DETT_GLOBALI RENAME COLUMN VAR_COD_FIS TO VAR_COD_FISC;
	EXECUTE DOCSADM.utl_rename_column '3.23',@db_user,'DPA_DETT_GLOBALI','VAR_COD_FIS','VAR_COD_FISC', NULL
