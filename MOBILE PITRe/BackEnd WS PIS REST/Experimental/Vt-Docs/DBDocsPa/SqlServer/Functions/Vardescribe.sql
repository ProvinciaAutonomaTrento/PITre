
CREATE   FUNCTION @db_user.[Vardescribe] (@sysid INT, @typeTable VARCHAR(1000)) RETURNS VARCHAR (8000)
AS
BEGIN
declare @outcome varchar(8000)
set @outcome=''
declare @tipo varchar(1)
declare @num_proto int
DECLARE @TMPVAR VARCHAR(4000)

--MAIN
IF(@typeTable = 'PEOPLENAME')
BEGIN
SET @outcome = (SELECT VAR_DESC_CORR FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = @sysid AND CHA_TIPO_URP='P' AND CHA_TIPO_IE = 'I')
END
IF(@typeTable = 'GROUPNAME')
BEGIN
SET @outcome = (SELECT VAR_DESC_CORR  FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = @sysid AND CHA_TIPO_URP='R')
END
IF(@typeTable = 'DESC_RUOLO')
BEGIN
SET @outcome = (SELECT VAR_DESC_CORR  FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO= @sysid AND CHA_TIPO_URP='R')
END
IF(@typeTable = 'RAGIONETRASM')
BEGIN
SET @outcome = (SELECT VAR_DESC_RAGIONE  FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'TIPO_RAGIONE')
BEGIN
SET @outcome = (SELECT CHA_TIPO_RAGIONE FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'DATADOC')
BEGIN
begin
SET @tipo = (SELECT CHA_TIPO_PROTO  FROM PROFILE WHERE SYSTEM_ID= @sysid)
set @num_proto =(SELECT isnull(num_proto,0)  FROM PROFILE WHERE SYSTEM_ID= @sysid)
end
IF(@tipo is not null and (@tipo  IN ('A','P','I') and @num_proto < > 0))
BEGIN
SET @outcome = (SELECT convert(varchar,DTA_PROTO,103) FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
ELSE
BEGIN
SET @outcome = (SELECT convert(varchar,CREATION_DATE,103) FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
END
IF(@typeTable = 'CHA_TIPO_PROTO')
BEGIN
SET @outcome = (SELECT CHA_TIPO_PROTO FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'NUMPROTO')
BEGIN
SET @outcome = (SELECT NUM_PROTO FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'CODFASC')
BEGIN
SET @outcome = (SELECT VAR_CODICE FROM PROJECT WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'DESC_OGGETTO')
BEGIN
SET @outcome = (SELECT VAR_PROF_OGGETTO FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'DESC_FASC')
BEGIN
SET @outcome = (SELECT DESCRIPTION  FROM DOCSADM.PROJECT WHERE SYSTEM_ID= @sysid)
IF ((@outcome = '')or (@outcome is null)) set @outcome  = ''
END
IF(@typeTable = 'PROF_IDREG')
BEGIN
IF (@sysid IS NOT NULL)
BEGIN
SET @outcome = (SELECT ID_REGISTRO  FROM PROFILE WHERE SYSTEM_ID= @sysid)
IF (@outcome IS NULL) set @outcome = '0'
END
ELSE set @outcome = '0'
END
IF(@typeTable = 'ID_GRUPPO')
BEGIN
IF @sysid IS NOT NULL
BEGIN
SET @outcome = (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE system_id = @sysid)
IF (@outcome IS NULL) set @outcome = '0'
END
ELSE set @outcome = '0'
END
IF(@typeTable = 'SEGNATURA_DOCNUMBER')
BEGIN
SET @outcome =  (SELECT VAR_SEGNATURA FROM PROFILE WHERE SYSTEM_ID= @sysid)
IF (@outcome IS NULL)
BEGIN
SET @outcome = (SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
END
IF(@typeTable = 'OGGETTO_MITTENTE')
BEGIN
-- OGGETTO
SET @outcome = (SELECT  TOP 1 VAR_PROF_OGGETTO FROM PROFILE WHERE SYSTEM_ID= @sysid)
BEGIN
SET @TMPVAR = (SELECT TOP 1 var_desc_corr FROM DPA_CORR_GLOBALI a, DPA_DOC_ARRIVO_PAR b WHERE b.id_mitt_dest=a.system_id AND b.cha_tipo_mitt_dest='M'
AND b.id_profile=@sysid)
END
IF (@TMPVAR IS NOT NULL)
BEGIN
SET @outcome = @outcome + '@@' + @TMPVAR
END
END
IF(@typeTable = 'PROFILE_CHA_IMG')
BEGIN
SET @outcome = (SELECT CHA_IMG FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
return @outcome
end

