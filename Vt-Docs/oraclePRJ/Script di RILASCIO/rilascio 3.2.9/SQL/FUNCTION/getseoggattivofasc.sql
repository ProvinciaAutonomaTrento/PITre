CREATE FUNCTION getseoggattivofasc (@systemid int, @idtemplate int)
RETURNS CHAR
AS

BEGIN
DECLARE @tmpvar  CHAR
DECLARE @cnt		INT

SELECT @cnt = COUNT (*)
FROM dpa_ass_templates_fasc
WHERE id_template = @idtemplate
AND id_project IS NULL
AND id_oggetto = @systemid

IF (@cnt > 0)
BEGIN
SET @tmpvar = '1'
END ELSE BEGIN
SET @tmpvar = '0'
END

RETURN @tmpvar

END