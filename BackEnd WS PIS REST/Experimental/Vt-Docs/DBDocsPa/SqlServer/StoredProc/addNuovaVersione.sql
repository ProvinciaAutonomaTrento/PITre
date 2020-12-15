SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE  PROCEDURE [@db_user].[addNuovaVersione]
@subVersion  VARCHAR(1),
@idPeople INT,
@docNumber INT,
@descrizione VARCHAR(200),
@cartaceo INT,
@versionID INT OUT

AS


declare @identityVersion INT
declare @version INT

BEGIN

SET @version  =  (SELECT TOP 1 VERSION
FROM VERSIONS
WHERE DOCNUMBER = @docNumber
ORDER BY VERSION DESC)

SET @version =@version+1

BEGIN

INSERT INTO VERSIONS
(

DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR,
TYPIST,
DTA_CREAZIONE,
COMMENTS,
CARTACEO
)
VALUES
(

@docNumber,
@version,
@subVersion,
@version,
@idPeople,
@idPeople,
getdate(),
@descrizione,
@cartaceo
)
IF (@@ROWCOUNT = 0)
BEGIN
SET @versionID=0
RETURN
END

SET @identityVersion = SCOPE_IDENTITY()

SET @versionID = @identityVersion
END



BEGIN

INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE
)
VALUES
(
@versionID,
@docNumber,
0
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @versionID=0
RETURN
END

END

END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO