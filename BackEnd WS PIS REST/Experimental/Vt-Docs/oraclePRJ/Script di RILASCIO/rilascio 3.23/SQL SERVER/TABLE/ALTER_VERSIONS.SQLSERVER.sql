-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'


-- INSERIMENTO NELLA VERSIONS DI UNA NUOVA COLONNA CHA_ALLEGATI_ESTERNO 
-- flag gestione allegati da applicativo esterno
DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.columns WHERE name = 'CHA_ALLEGATI_ESTERNO' AND OBJECT_ID = (SELECT OBJECT_ID FROM SYS.tables WHERE name = 'VERSIONS'))

IF (@COUNT = 0)
BEGIN              
	EXECUTE DOCSADM.utl_add_column '3.23',@db_user,'VERSIONS','CHA_ALLEGATI_ESTERNO','CHAR(1)','0',NULL,NULL,NULL
END