-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 05/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETNOMERUOLOBYSYSID
-- =============================================


CREATE FUNCTION [DOCSADM].[getnomeRuolobysysid]
(
	@my_system_id	INT
)
RETURNS VARCHAR(256)
AS
BEGIN
	DECLARE @RISULTATO	VARCHAR(256)
	
	SELECT @RISULTATO = GROUP_NAME 
	FROM DOCSADM.GROUPS G
	WHERE G.SYSTEM_ID = @my_system_id
	
	RETURN @RISULTATO
END

