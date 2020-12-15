USE PCM_DEPOSITO_1
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ====================================================================================
-- Author:		Giovanni Olivari
-- Create date: 06/06/2013
-- Description:	Restituisce il System_ID della Tabella GROUPS per il ruolo Consultatore
-- ====================================================================================
CREATE FUNCTION DOCSADM.fn_ARCHIVE_getGroupsIDForRuoloConsultatore
(
)
RETURNS INT
AS
BEGIN

	RETURN 1

END
GO

