-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'


-- INSERIMENTO NELLA DPA_PREGRESSI DI UNA NUOVA COLONNA DESCRIZIONE 

DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.columns WHERE name = 'DESCRIZIONE' AND OBJECT_ID = (SELECT OBJECT_ID FROM SYS.tables WHERE name = 'DPA_PREGRESSI'))

IF (@COUNT = 0)
BEGIN              
	EXECUTE DOCSADM.utl_add_column '3.23','@db_user','DPA_PREGRESSI','DESCRIZIONE','VARCHAR(1024)',NULL,NULL,NULL,NULL
END
PRINT 'GIA'' ESISTENTE'