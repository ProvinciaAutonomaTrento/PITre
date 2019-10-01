SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[createAllegato]
@idDocumentoPrincipale int,
@idPeople int,
@comments varchar(200),
@numeroPagine int,
@idProfile int out,
@versionId int out

AS

BEGIN

DECLARE @idDocType INT
SET @idDocType = (SELECT DOCUMENTTYPE FROM PROFILE WHERE SYSTEM_ID = @idDocumentoPrincipale)

INSERT INTO Profile
(
TYPIST,
AUTHOR,
CHA_TIPO_PROTO,
CHA_DA_PROTO,
DOCUMENTTYPE,
CREATION_DATE,
CREATION_TIME,
ID_DOCUMENTO_PRINCIPALE
)
VALUES
(

@idpeople,
@idpeople,
'G',
'0',
@idDocType,
GETDATE(),
GETDATE(),
@idDocumentoPrincipale
)

SET @idProfile = scope_identity()
UPDATE PROFILE
SET DOCNUMBER = @idProfile
WHERE SYSTEM_ID = @idProfile

IF (@@ROWCOUNT > 0)
BEGIN
INSERT INTO VERSIONS
(
DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR,
TYPIST,
COMMENTS,
NUM_PAG_ALLEGATI,
DTA_CREAZIONE
)
VALUES
(
@idProfile,
1,
'!',
'1',
@idPeople,
@idPeople,
@comments,
@numeroPagine,
GETDATE()
)

SET @versionId = scope_identity()
INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE
)
VALUES
(
@versionId,
@idProfile,
0
)
END
END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO