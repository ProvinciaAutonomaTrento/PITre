-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'


-- INSERIMENTO NELLA Dpa_Mail_Corr_Esterni DI UN NUOVO INDICE IX_Dpa_Mail_Corr_Esterni_K2 

DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.indexes WHERE name = 'IX_Dpa_Mail_Corr_Esterni_K2')

IF (@COUNT = 0)
BEGIN              
	EXECUTE DOCSADM.utl_add_index '3.23',@db_user,'Dpa_Mail_Corr_Esterni','IX_Dpa_Mail_Corr_Esterni_K2',null,'ID_CORR',null,null,null,'NORMAL', null,null,null     
END