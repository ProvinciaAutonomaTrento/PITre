SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE FUNCTION @db_user.checkSecurity
(
@thing INT,
@idpeople INT,
@idgroup INT
)
RETURNS INT
AS

BEGIN

DECLARE @retValue INT

IF EXISTS(SELECT 'x'
FROM security
WHERE thing = @thing AND
(personorgroup IN (@idgroup, @idpeople)))
SET @retValue = 1
ELSE
SET @retValue = 0

RETURN @retValue

END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO