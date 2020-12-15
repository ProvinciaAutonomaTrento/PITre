ALTER FUNCTION [DOCSADM].[checkSecurityDocumento]
(
@idprofile INT,
@idpeople INT,
@idgroup INT,
@idRuoloPubblico INT,
@tipoObjParam VARCHAR
)
RETURNS INT
AS

BEGIN

DECLARE @thing INT
SELECT @thing = ID_DOCUMENTO_PRINCIPALE
FROM PROFILE
WHERE SYSTEM_ID = @idprofile

IF (@thing IS NULL)
SET @thing = @idprofile

RETURN DOCSADM.checkSecurity(@thing, @idpeople, @idgroup, @idRuoloPubblico, @tipoObjParam)
END

GO


