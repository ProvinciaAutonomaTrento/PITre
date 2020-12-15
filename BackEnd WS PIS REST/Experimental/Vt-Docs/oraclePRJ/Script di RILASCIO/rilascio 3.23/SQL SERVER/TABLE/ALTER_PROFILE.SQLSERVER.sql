-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'


-- INSERIMENTO NELLA PROFILE DI UNA NUOVA COLONNA ID_VECCHIO_DOCUMENTO 

DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.columns WHERE name = 'ID_VECCHIO_DOCUMENTO' AND OBJECT_ID = (SELECT OBJECT_ID FROM SYS.tables WHERE name = 'PROFILE'))

IF (@COUNT = 0)
BEGIN              
	EXECUTE DOCSADM.utl_add_column '3.23',@db_user,'PROFILE','ID_VECCHIO_DOCUMENTO','VARCHAR(200)',NULL,NULL,NULL,NULL
END