SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create PROCEDURE [@db_user].[createProjectSP]
@idpeople int,
@description varchar (2000)  ,
@projectId int out

AS

BEGIN

set @projectId = 0


INSERT INTO PROJECT
(

[DESCRIPTION],
ICONIZED
)
VALUES
(

@description,
'Y'
)


-- Reperimento identity appena immessa
SET @projectId = scope_identity()

if(@@ROWCOUNT > 0)

INSERT INTO SECURITY
(
THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO
)
VALUES
(
@projectId,
@idpeople,
0,
NULL,
NULL
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @projectId=0
END

END


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO