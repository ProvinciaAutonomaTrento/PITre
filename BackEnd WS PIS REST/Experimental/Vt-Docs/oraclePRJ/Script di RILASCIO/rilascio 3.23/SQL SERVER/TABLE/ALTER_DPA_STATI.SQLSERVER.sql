-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'


-- INSERIMENTO NELLA DPA_STATI DI UNA NUOVA COLONNA Cha_Stato_Sistema 

DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.columns WHERE name = 'Cha_Stato_Sistema' AND OBJECT_ID = (SELECT OBJECT_ID FROM SYS.tables WHERE name = 'DPA_STATI'))

IF (@COUNT = 0)
BEGIN              
	EXECUTE DOCSADM.utl_add_column '3.23',@db_user,'DPA_STATI','Cha_Stato_Sistema','CHAR(1)','0',NULL,NULL,NULL
END