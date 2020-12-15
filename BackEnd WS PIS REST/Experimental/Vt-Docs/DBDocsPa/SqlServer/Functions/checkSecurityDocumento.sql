

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE FUNCTION @db_user.checkSecurityDocumento
(
@idprofile INT,
@idpeople INT,
@idgroup INT
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

RETURN @db_user.checkSecurity(@thing, @idpeople, @idgroup)
END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO