SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[SP_EREDITA_VIS_FASC]
@IDCorrGlobaleUO int,
@IDCorrGlobaleRuolo int,
@IDGruppo int,
@LivelloRuolo int,
@IDRegistro int,
@PariLivello int
AS

DECLARE @retValue int
DECLARE @id_thing int

DECLARE c1 CURSOR FOR
(SELECT DISTINCT a.system_id AS THING
FROM PROJECT a
WHERE ((a.cha_tipo_proj = 'T' AND (a.id_registro = @IDRegistro or id_registro is null)) OR (a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'G' AND (a.id_registro = @IDRegistro or a.id_registro is null))))
UNION
(SELECT DISTINCT b.system_id AS THING
FROM PROJECT b
WHERE b.cha_tipo_proj = 'C'
AND id_fascicolo IN (
SELECT system_id
FROM PROJECT
WHERE cha_tipo_proj = 'F'
AND cha_tipo_fascicolo = 'G'
AND (id_registro = @IDRegistro or id_registro is null)))

IF (@@ERROR <> 0)
RETURN(1)

BEGIN

SET @retValue = 0

OPEN c1
FETCH NEXT FROM c1
INTO @id_thing

WHILE @@FETCH_STATUS = 0
BEGIN
IF NOT EXISTS
(SELECT *
FROM SECURITY
WHERE THING = @id_thing AND
PERSONORGROUP = @IDGruppo AND
ACCESSRIGHTS = 255)
INSERT  INTO SECURITY
(THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO)
VALUES
(@id_thing,
@IDGruppo,
255,
NULL,
'P')

IF(@@ERROR <> 0)
CONTINUE

FETCH NEXT FROM c1
INTO @id_thing
END

CLOSE c1
DEALLOCATE c1

EXECUTE @retValue=SP_EREDITA_VIS_FASC_E1 @IDCorrGlobaleUO, @IDCorrGlobaleRuolo, @IDGruppo, @LivelloRuolo, @IDRegistro, @PariLivello

RETURN @retValue
END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO