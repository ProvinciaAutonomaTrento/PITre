-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 05/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETFASCPRIMARIA
-- =============================================


ALTER FUNCTION [DOCSADM].[getFascPrimaria]
(
	@iProfile		INT
	, @idFascicolo	INT
)
RETURNS VARCHAR(1)
AS
BEGIN
 
	DECLARE @fascPrimaria VARCHAR(1)

	SET @fascPrimaria = '0'

	SET @fascPrimaria = (SELECT TOP 1 ISNULL(B.CHA_FASC_PRIMARIA,'0') 
							FROM DOCSADM.PROJECT AS A
								, DOCSADM.PROJECT_COMPONENTS AS B
							WHERE A.SYSTEM_ID = B.PROJECT_ID 
								AND B.LINK = @iProfile 
								AND ID_FASCICOLO = @idFascicolo)
 
RETURN @fascPrimaria
END



