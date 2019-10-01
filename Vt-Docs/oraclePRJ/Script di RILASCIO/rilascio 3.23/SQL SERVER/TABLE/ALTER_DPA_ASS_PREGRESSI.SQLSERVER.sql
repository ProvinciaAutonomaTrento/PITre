-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 21/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

DECLARE @db_user	VARCHAR(1024)
SET @db_user = 'DOCSADM'

-- CREAZIONE DI DUE INDICI SULLA DPA_ASS_PREGRESSI (GIA' CREATI IN FASE DI CREAZIONE DELLA TABELLA)

DECLARE @COUNT INT

SET @COUNT = (SELECT COUNT(*) FROM SYS.indexes WHERE name = 'IX_T_DPA_ASS_PREGRESSIK2')

IF (@COUNT = 0)
BEGIN
	EXECUTE DOCSADM.utl_add_index '3.23',@db_user,'DPA_ASS_PREGRESSI','IX_T_DPA_ASS_PREGRESSIK2','UNIQUE','SYSTEM_ID',null,null,null,'NORMAL', null,null,null
END


SET @COUNT = (SELECT COUNT(*) FROM SYS.indexes WHERE name = 'IX_T_DPA_ASS_PREGRESSIK1')

IF (@COUNT = 0)
BEGIN
	EXECUTE DOCSADM.utl_add_index '3.23',@db_user,'DPA_ASS_PREGRESSI','IX_T_DPA_ASS_PREGRESSIK1',null,'ID_PREGRESSO',null,null,null,'NORMAL', null,null,null
END