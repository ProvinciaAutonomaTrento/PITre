	-- TODOLIST unificata 
if not exists(select * from syscolumns where name='DTA_VISTA' and id in 
(select id from sysobjects where name='DPA_TODOLIST' and xtype='U'))
BEGIN
ALTER TABLE DPA_TODOLIST ADD DTA_VISTA DATETIME DEFAULT ('17530101') NOT NULL
END
GO

if not exists(select * from syscolumns where name='CHA_FIRMATO' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CHA_FIRMATO Varchar(1) DEFAULT ('0') NOT NULL
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[Vardescribe]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[Vardescribe] 
go
CREATE   FUNCTION [@db_user].[Vardescribe] (@sysid INT, @typeTable VARCHAR(1000)) RETURNS VARCHAR (8000)
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
IF (@typeTable = 'DTA_CREAZ')
BEGIN
SET  @outcome =(SELECT year (dta_creazione) FROM project  WHERE system_id = @sysid)
END
IF (@typeTable = 'NUM_FASC')
BEGIN
SET @outcome=(SELECT num_fascicolo FROM project  WHERE system_id = @sysid)
END
IF(@typeTable = 'DESC_OGGETTO')
BEGIN
SET @outcome = (SELECT VAR_PROF_OGGETTO FROM PROFILE WHERE SYSTEM_ID= @sysid)
END
IF(@typeTable = 'DESC_FASC')
BEGIN
SET @outcome = (SELECT DESCRIPTION  FROM @db_user.PROJECT WHERE SYSTEM_ID= @sysid)
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
SET @outcome =(
 SELECT @db_user.getchaimg (docnumber)
                   FROM PROFILE
          WHERE system_id = @sysid   )
END
IF (@typeTable = 'PROFILE_CHA_FIRMATO')
BEGIN 
set @outcome=(SELECT CHA_FIRMATO FROM PROFILE WHERE system_id = @sysid)
END
return @outcome
end
go


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[VardescribeInvio]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[VardescribeInvio] 
go
CREATE   FUNCTION [@db_user].[VardescribeInvio] (@sysid INT,@idCorr int, @typeTable VARCHAR(1000)) RETURNS VARCHAR (8000)
AS
BEGIN
declare @outcome varchar(8000)
set @outcome=''
declare @tipo varchar(1)
declare @num_proto int
DECLARE @TMPVAR VARCHAR(4000)
--MAIN
IF(@typeTable = 'SYSTEM_ID')
BEGIN
SET @outcome = (SELECT SYSTEM_ID FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'VAR_CODICE_AOO')
BEGIN
SET @outcome = (SELECT VAR_CODICE_AOO FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'VAR_CODICE_AMM')
BEGIN
SET @outcome = (SELECT VAR_CODICE_AMM FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'DTA_SPEDIZIONE')
BEGIN
SET @outcome = (SELECT DTA_SPEDIZIONE FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'VAR_PROTO_DEST')
BEGIN
SET @outcome = (SELECT VAR_PROTO_DEST FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'DTA_PROTO_DEST')
BEGIN
SET @outcome = (SELECT DTA_PROTO_DEST FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'ID_CORR_GLOBALE')
BEGIN
SET @outcome = (SELECT ID_CORR_GLOBALE FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'ID_DOCUMENTTYPE')
BEGIN
SET @outcome = (SELECT ID_DOCUMENTTYPE FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)
END
IF(@typeTable = 'CHA_ANNULLATO')
BEGIN
SET @outcome = (SELECT CHA_ANNULLATO FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)

END
IF(@typeTable = 'VAR_MOTIVO_ANNULLA')
BEGIN
SET @outcome = (SELECT VAR_MOTIVO_ANNULLA FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)

END
IF (@typeTable = 'VAR_PROVVEDIMENTO')
BEGIN
SET @outcome = (SELECT VAR_PROVVEDIMENTO FROM dpa_stato_invio WHERE ID_profile = @sysid AND id_corr_globale=@idcorr)

END


return @outcome
end

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SPsetDataVista]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop  PROCEDURE [@db_user].[SPsetDataVista]
go
CREATE  PROCEDURE [@db_user].[SPsetDataVista]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)
BEGIN
SET @resultValue = 0
DECLARE cursorTrasmSingolaDocumento CURSOR FOR
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
AND a.ID_PROFILE = @idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID
DECLARE  cursorTrasmSingolaFascicolo CURSOR FOR
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
AND a.ID_PROJECT = @idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID
IF(@tipoOggetto='D')
BEGIN
OPEN cursorTrasmSingolaDocumento
FETCH NEXT FROM cursorTrasmSingolaDocumento
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
WHILE @@FETCH_STATUS = 0
begin
IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
BEGIN
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
update dpa_todolist 
set DTA_VISTA = getdate()
where 
id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
and id_profile = @idOggetto
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
IF (@chaTipoTrasm = 'S' AND @chaTipoDest = 'R')
BEGIN
UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
end
END
ELSE
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
update dpa_todolist 
set DTA_VISTA = GETDATE()  
where 
 id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
and id_profile = @idOggetto;
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
FETCH NEXT FROM cursorTrasmSingolaDocumento
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
END
CLOSE cursorTrasmSingolaDocumento
DEALLOCATE cursorTrasmSingolaDocumento
END
IF(@tipoOggetto='F')
begin
OPEN cursorTrasmSingolaFascicolo
FETCH NEXT FROM cursorTrasmSingolaFascicolo
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
WHILE @@FETCH_STATUS = 0
BEGIN
IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
BEGIN
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
update dpa_todolist 
set DTA_VISTA = getdate() 
where 
 id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
and id_project = @idOggetto;
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
BEGIN
UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
END
ELSE
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
update dpa_todolist 
set DTA_VISTA = getdate()
where 
id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
and id_project = @idOggetto;
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
FETCH NEXT FROM cursorTrasmSingolaFascicolo
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
END
CLOSE cursorTrasmSingolaFascicolo
DEALLOCATE cursorTrasmSingolaFascicolo
END
END
RETURN @resultValue
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SPsetDataVistaSmistamento]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop  PROCEDURE [@db_user].[SPsetDataVistaSmistamento]
go
CREATE  PROCEDURE [@db_user].[SPsetDataVistaSmistamento]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@idTrasmissione INT,
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)
BEGIN
SET @resultValue = 0
DECLARE cursorTrasmSingolaDocumento CURSOR FOR
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.system_id = @idTrasmissione and a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
AND a.ID_PROFILE = @idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID
IF(@tipoOggetto='D')
OPEN cursorTrasmSingolaDocumento
FETCH NEXT FROM cursorTrasmSingolaDocumento
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
BEGIN
WHILE @@FETCH_STATUS = 0
begin
IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
BEGIN
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
update dpa_todolist 
set DTA_VISTA = getdate()
where 
id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
BEGIN
UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
END
END
ELSE
BEGIN
UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
dpa_trasm_utente.CHA_IN_TODOLIST =  (CASE WHEN dta_accettata is null then '1' else '0' end),
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
DTA_ACCETTATA = (CASE WHEN DTA_ACCETTATA IS NULL THEN  GETDATE() ELSE DTA_ACCETTATA END),
CHA_ACCETTATA = '1'
WHERE
DPA_TRASM_UTENTE.CHA_ACCETTATA = '0'
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
update dpa_todolist 
set DTA_VISTA = getdate()
where 
id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
begin
UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VALIDA= '0',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
id_trasm_singola = @sysTrasmSingola
AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
IF (@@ERROR <> 0)
BEGIN
SET @resultValue=1
return @resultValue
END
end
END
FETCH NEXT FROM cursorTrasmSingolaDocumento
INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
END
CLOSE cursorTrasmSingolaDocumento
DEALLOCATE cursorTrasmSingolaDocumento
END
END
RETURN @resultValue
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

ALTER   TRIGGER [TR_UPDATE_DPA_TODOLIST] ON [@db_user].[DPA_TRASM_UTENTE]
AFTER UPDATE
AS
IF UPDATE(CHA_IN_TODOLIST)
BEGIN
declare @cha_in_todolist varchar(1)
set @cha_in_todolist = (select top 1 cha_in_todolist  from INSERTED)
if(@cha_in_todolist = '0') begin
DELETE @db_user.DPA_TODOLIST WHERE ID_TRASM_UTENTE IN (select system_id from INSERTED)
end
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
-- PROTOCOLLO TITOLARIO

--MODIFICHE PROJECT
--*****************
if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV2' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV2', 'ET_TITOLARIO', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV3' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV3', 'ET_LIVELLO1', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV4' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV4', 'ET_LIVELLO2', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV5' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV5', 'ET_LIVELLO3', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV6' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV6', 'ET_LIVELLO4', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV7' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV7', 'ET_LIVELLO5', 'COLUMN'	
END;

GO

if exists (SELECT * FROM syscolumns WHERE name='VAR_COD_LIV8' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	EXEC sp_rename 'PROJECT.VAR_COD_LIV8', 'ET_LIVELLO6', 'COLUMN'	
END;

GO


if not exists (SELECT * FROM syscolumns WHERE name='NUM_PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD NUM_PROT_TIT INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='DA_VISUALIZZARE_RICERCA' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM] ADD DA_VISUALIZZARE_RICERCA INT;	
END;

GO


if not exists (SELECT * FROM syscolumns WHERE name='DA_VISUALIZZARE_RICERCA' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC] ADD DA_VISUALIZZARE_RICERCA INT;	
END;

GO



if not exists (SELECT * FROM syscolumns WHERE name='CHA_CONTA_PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD CHA_CONTA_PROT_TIT VARCHAR(2) DEFAULT 'NO' NOT NULL;	
	ALTER TABLE [@db_user].[PROJECT] ALTER COLUMN CHA_CONTA_PROT_TIT VARCHAR(2) NULL;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_BLOCCA_FIGLI' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD CHA_BLOCCA_FIGLI VARCHAR(2) DEFAULT 'NO' NOT NULL;	
	ALTER TABLE [@db_user].[PROJECT] ALTER COLUMN CHA_BLOCCA_FIGLI VARCHAR(2) NULL;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='MAX_LIV_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD MAX_LIV_TIT INT DEFAULT 6 NOT NULL;
	ALTER TABLE [@db_user].[PROJECT] ALTER COLUMN MAX_LIV_TIT INT NULL;	
END;

GO

--INIZIO : RIFERIMENTI MITENTE
if not exists (SELECT * FROM syscolumns WHERE name='CHA_RIFF_MITT' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD CHA_RIFF_MITT VARCHAR(255);	
END;
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_RISCONTRI_CLASSIFICA' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_RISCONTRI_CLASSIFICA] (
  [SYSTEM_ID] 	[int]    NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [RIFF_MITT]  	[VARCHAR](255) NULL,
  [ID_CORR_GLOB] [INT] NULL,
  [ID_TITOLARIO_DEST] [INT] NULL,
  [ID_REG_DEST] [INT] NULL,
  [CLASS_DEST] [VARCHAR] (255) NULL,
  [COD_FASC_DEST] [VARCHAR] (255) NULL,
  [PROT_TIT_DEST] [VARCHAR] (255) NULL,
  [DTA_RISCONTRO] [DATETIME] NULL	
)
end
GO
--FINE : RIFERIMENTI MITENTE

--MODIFICHE DPA_AMMINISTRA
--************************
if not exists (SELECT * FROM syscolumns WHERE name='VAR_FORMATO_PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA] ADD VAR_FORMATO_PROT_TIT VARCHAR(255);
END;

GO

--CREAZIONE TABELLA DPA_PROTO_TIT
--*******************************
IF NOT EXISTS (select * from sysobjects where name = 'DPA_PROTO_TIT' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_PROTO_TIT] (
  [SYSTEM_ID] 	[int]    NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_AMM]    	[int] NULL,
  [ID_NODO_TIT] [int] NULL,
  [ID_REGISTRO] [int] NULL,
  [NUM_RIF]	[int] NULL	
)
end
GO

--POPOLAMENTO TABELLA DPA_PROTO_TIT
--*********************************
IF( (select count(*) from dpa_proto_tit) = 0)
BEGIN
	DECLARE @idamm INT
	DECLARE  CURR_AMM CURSOR FOR SELECT SYSTEM_ID FROM dpa_amministra
	BEGIN
		OPEN CURR_AMM
		FETCH NEXT FROM CURR_AMM into @idamm
		WHILE(@@fetch_status=0) 
		BEGIN
			DECLARE @idtitolario INT
			DECLARE  CURR_PROJECT CURSOR FOR select system_id from project where id_titolario = 0 and var_codice = 'T' and ID_AMM = @idamm
			BEGIN
				OPEN CURR_PROJECT
				FETCH NEXT FROM CURR_PROJECT into @idtitolario
				WHILE(@@fetch_status=0) 
				BEGIN
					insert into dpa_proto_tit (ID_AMM, ID_NODO_TIT, ID_REGISTRO, NUM_RIF) values (@idamm, @idtitolario,null,1);
					DECLARE @idregistro INT
					DECLARE  CURR_REG CURSOR FOR select system_id from dpa_el_registri where ID_AMM = @idamm
					BEGIN
						OPEN CURR_REG
						FETCH NEXT FROM CURR_REG into @idregistro
						WHILE(@@fetch_status=0) 
						BEGIN	
							insert into dpa_proto_tit (ID_AMM, ID_NODO_TIT, ID_REGISTRO, NUM_RIF) values (@idamm, @idtitolario,@idregistro,1) 							
						FETCH NEXT FROM CURR_REG into @idregistro
						END	
						DEALLOCATE CURR_REG
					END
				FETCH NEXT FROM CURR_PROJECT into @idtitolario
				END	
				DEALLOCATE CURR_PROJECT
			END		
		FETCH NEXT FROM CURR_AMM into @idamm
		END	
		DEALLOCATE CURR_AMM
	END
END
GO


--MODIFICHE PROFILE
--*****************
if not exists (SELECT * FROM syscolumns WHERE name='PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD PROT_TIT VARCHAR(255);	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='NUM_IN_FASC' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD NUM_IN_FASC INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_FASC_PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD ID_FASC_PROT_TIT INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='NUM_PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD NUM_PROT_TIT INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_TITOLARIO' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD ID_TITOLARIO INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_PROTO_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] ADD DTA_PROTO_TIT DATETIME;	
END;

GO


--MODIFICHE PROJECT_COMPONENTS
--****************************
if not exists (SELECT * FROM syscolumns WHERE name='PROT_TIT' and id in (SELECT id FROM sysobjects WHERE name='PROJECT_COMPONENTS' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT_COMPONENTS] ADD PROT_TIT VARCHAR(255);	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_CLASS' and id in (SELECT id FROM sysobjects WHERE name='PROJECT_COMPONENTS' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT_COMPONENTS] ADD DTA_CLASS DATETIME;	
END;

GO

--MODIFICHE DPA_PROTO_TIT
--***********************
if not exists (SELECT * FROM syscolumns WHERE name='ID_FASC' and id in (SELECT id FROM sysobjects WHERE name='DPA_PROTO_TIT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_PROTO_TIT] ADD ID_FASC INT;	
END;

GO


if not exists (SELECT * FROM syscolumns WHERE name='NUM_DOC_IN_FASC' and id in (SELECT id FROM sysobjects WHERE name='DPA_PROTO_TIT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_PROTO_TIT] ADD NUM_DOC_IN_FASC INT;	
END;

GO

--MODIFICHE STORED CREATE_NEW_NODO_TITOLARIO
--******************************************
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[CREATE_NEW_NODO_TITOLARIO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[CREATE_NEW_NODO_TITOLARIO]
GO
 
CREATE  PROCEDURE  [@db_user].[CREATE_NEW_NODO_TITOLARIO]
@idAmm INT, @livelloNodo INT,
@description VARCHAR(2000),
@codiceNodo VARCHAR(64),
@idRegistroNodo INT,
@idParent INT,
@varCodLiv1 VARCHAR(32),
@mesiConservazione INT,
@idTipoFascicolo INT,
@bloccaFascicolo VARCHAR(2),
@chaRW CHAR(1),
@sysIdTitolario INT,
@noteNodo VARCHAR(2000),
@bloccaFigli VARCHAR(2),
@contatoreAttivo VARCHAR(2),
@numProtTit INT,
@idTitolario INT OUT

AS

DECLARE @secProj INT
DECLARE @secFasc INT
DECLARE @secRoot INT

DECLARE @varChiaveTit VARCHAR(256)
DECLARE @varChiaveFasc VARCHAR(256)
DECLARE @varChiaveRoot VARCHAR(256)
DECLARE @sysCurrReg INT

BEGIN

DECLARE currReg CURSOR FOR
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = @idAmm and cha_rf = '0'
BEGIN
SET @idTitolario=0

if(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveTit= CONVERT(varchar(10), @idAmm) + '_' + @codiceNodo + '_' + CONVERT(varchar(64), @idParent ) + '_0'
else
SET @varChiaveTit= @codiceNodo + '_' +    CONVERT(varchar(64), @idParent ) + '_'  +  CONVERT(varchar(64),@idRegistroNodo)


-- INSERIMENTO RELATIVO AL NODO DI TITOLARIO

BEGIN

INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(

@description,
'Y',
'T',
@codiceNodo,
@idAmm,
@idRegistroNodo,
@livelloNodo,
NULL,
@idParent,
@varCodLiv1,
GETDATE() ,
NULL,
NULL,
@chaRW,
@mesiConservazione,
@varChiaveTit,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit
)
-- Reperimento identity appena immessa
SET @secProj = scope_identity()
SET @idTitolario =  @secProj

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

END


-- INSERIMENTO RELATIVO AL FASCICOLO GENERALE ASSOCIATO AL NODO DI TITOLARIO
BEGIN

IF(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveFasc= @codiceNodo + '_' +  Convert(varchar(64),@idTitolario ) + '_0'
ELSE
SET @varChiaveFasc= @codiceNodo + '_'+  Convert(varchar(64),@idTitolario ) + '_'  +  CONVERT(varchar(64), @idRegistroNodo)


INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(

@description,
'Y',
'F',
@codiceNodo,
@idAmm,
@idRegistroNodo,
NULL,
'G',
@idTitolario,
NULL,
GETDATE(),
'A',
NULL,
@chaRW,
@mesiConservazione,
@varChiaveFasc,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit
)

SET @secFasc = scope_identity()

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END
END


BEGIN


if(@idRegistroNodo IS NULL or @idRegistroNodo = '')
SET @varChiaveRoot= @codiceNodo + '_' + convert( varchar(64),@secFasc ) + '_0'
else
SET @varChiaveRoot= @codiceNodo + '_'  + convert( varchar(64),@secFasc) + '_'  +  CONVERT(varchar(64), @idRegistroNodo)


INSERT INTO PROJECT
(

DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(

'Root Folder',
'Y',
'C',
NULL,
@idAmm,
NULL,
NULL,
NULL,
@secFasc,
NULL,
GETDATE(),
NULL,
@secFasc,
@chaRW,
@mesiConservazione,
@varChiaveRoot,
@idTipoFascicolo,
@bloccaFascicolo,
@sysIdTitolario,
GETDATE(),
@noteNodo,
@bloccaFigli,
@contatoreAttivo,
@numProtTit
)
SET @secRoot = scope_identity()

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END
END

OPEN currReg
FETCH NEXT FROM currReg
INTO @sysCurrReg

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
IF(@idRegistroNodo IS NULL or @idRegistroNodo = '')
BEGIN

WHILE @@FETCH_STATUS = 0

BEGIN
INSERT INTO DPA_REG_FASC
(

id_Titolario,
num_rif,
id_registro
)
VALUES
(

@idTitolario,
1,
@sysCurrReg
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

FETCH NEXT FROM currReg INTO @sysCurrReg
END


-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
INSERT INTO dpa_reg_fasc
(
id_Titolario,
num_rif,
id_registro
)
VALUES
(
@idTitolario,
1,
NULL	-- SE IL NODO ? COMUNE A TUTTI p_idRegistro = NULL
)
END
ELSE -- il nodo creato ? associato a uno solo registro

BEGIN
INSERT INTO dpa_reg_fasc
(
id_Titolario,
num_rif,
id_registro
)
values
(
@idTitolario,
1,
@idRegistroNodo	-- REGISTRO SU CUI ? CRETO IL NODO
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @idTitolario=0
RETURN
END

END

END
CLOSE currReg
DEALLOCATE currReg
END

GO
-- FINE PROTOCOLLO TITOLARIO



if not exists (SELECT * FROM syscolumns WHERE name='CHA_SET_EREDITA' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_TRASM_SINGOLA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_TRASM_SINGOLA] ADD CHA_SET_EREDITA CHAR(1) DEFAULT 1 NOT NULL;
	ALTER TABLE [@db_user].[DPA_TRASM_SINGOLA] ALTER COLUMN CHA_SET_EREDITA CHAR(1) NULL;
END;

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getContatoreDocOrdinamento]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getContatoreDocOrdinamento] 
go


CREATE function [@db_user].[getContatoreDocOrdinamento] (@docNumber INT, @tipoContatore CHAR)
returns int
as
begin

declare @valore_oggetto_db int

select
@valore_oggetto_db = valore_oggetto_db

from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = Cast(@docNumber as varchar(255))
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.cha_tipo_tar = @tipoContatore;

return @valore_oggetto_db


end

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getDescRuolo]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getDescRuolo] 
go


CREATE function [@db_user].[getDescRuolo] (@idGruppo int)
returns varchar(256)
as
begin

declare @varDescCorr varchar(256)

select @varDescCorr = var_desc_corr
from dpa_corr_globali
where id_Gruppo=@idGruppo;

return @varDescCorr

end
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getContatoreDoc]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getContatoreDoc] 
go


CREATE FUNCTION [@db_user].[getContatoreDoc](@docNumber INT, @tipoContatore CHAR)

returns varchar(256)

as

begin

 

declare @valore_oggetto_db varchar(256);

declare @annoContatore varchar(256);

declare @codiceRegRf varchar(256);

declare @rtn varchar(256);

 

 

 

select 

@valore_oggetto_db = valore_oggetto_db,

@annoContatore = anno

from 

dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto

where 

dpa_associazione_templates.doc_number =Cast(@docNumber as varchar(255))

and

dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id

and

dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id

and

dpa_tipo_oggetto.descrizione = 'Contatore'

and

dpa_oggetti_custom.cha_tipo_tar = @tipoContatore;

 

IF(@tipoContatore != 'T')

BEGIN

select 

@codiceRegRf = dpa_el_registri.var_codice

from 

dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri

where 

dpa_associazione_templates.doc_number = Cast(@docNumber as varchar(255))

and

dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id

and

dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id

and

dpa_tipo_oggetto.descrizione = 'Contatore'

and

dpa_oggetti_custom.cha_tipo_tar = @tipoContatore

and 

dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;

END;

 

if(@codiceRegRf is null or @codiceRegRf='')

begin

set @codiceRegRf=' '

end;

 

if(@annoContatore is null or @annoContatore='')

begin

set @annoContatore=' '

end;

 

if(@valore_oggetto_db is null or @valore_oggetto_db='')

begin

set @valore_oggetto_db=' '

end; 

return @codiceRegRf +'-'+ @annoContatore+ '-'+@valore_oggetto_db;

end

go



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getContatoreFasc]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getContatoreFasc] 
go


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
 
CREATE FUNCTION [@db_user].[getContatoreFasc](@docNumber INT, @tipoContatore CHAR)
returns varchar(256)
as
begin
 
declare @valore_oggetto_db varchar(256);
declare @annoContatore varchar(256);
declare @codiceRegRf varchar(256);
declare @rtn varchar(256);
 
select 
@valore_oggetto_db = valore_oggetto_db,
@annoContatore = anno
from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where 
dpa_ass_templates_fasc.id_project = Cast(@docNumber as varchar(255))
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore;
 
 
if (@tipocontatore != 'T')
begin
 
select 
@codiceRegRf = dpa_el_registri.var_codice
from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where 
dpa_ass_templates_fasc.id_project = Cast(@docNumber as varchar(255))
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore
and 
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
end;
 
if(@codiceRegRf is null or @codiceRegRf='')
begin
set @codiceRegRf=' '
end;
 
if(@annoContatore is null or @annoContatore='')
begin
set @annoContatore=' '
end;
 
if(@valore_oggetto_db is null or @valore_oggetto_db='')
begin
set @valore_oggetto_db=' '
end; 
 
return @codiceRegRf +'-'+ @annoContatore+ '-'+@valore_oggetto_db;
end
go

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getContatoreFascContatore]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getContatoreFascContatore] 
go


CREATE function [@db_user].[getContatoreFascContatore] (@systemId INT, @tipoContatore CHAR)
returns int
as
begin

declare @valore_oggetto_db int

select 
@valore_oggetto_db = valore_oggetto_db 

from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where 
dpa_ass_templates_fasc.id_project = @systemId
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.DESCRIZIONE = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore;

return @valore_oggetto_db

end
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getVisibilita]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getVisibilita] 
go
CREATE FUNCTION [@db_user].[getVisibilita](@p_IdAmm INT, @p_Idmodello INT)

RETURNS INT
AS

BEGIN
DECLARE @result INT
DECLARE @ragioneCC INT
DECLARE @ragioneComp INT
DECLARE @ragioneConos INT
DECLARE @ragioneRef INT
DECLARE @ragioneTO INT

    begin
       SET @result = 0
       
       IF (@p_IdAmm IS NOT NULL)
       BEGIN
           SET @result = 1
		   
		   SELECT @ragioneCC = DPA_AMMINISTRA.ID_RAGIONE_CC FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm
           SELECT @ragioneComp = DPA_AMMINISTRA.ID_RAGIONE_COMPETENZA FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm
           SELECT @ragioneConos = DPA_AMMINISTRA.ID_RAGIONE_CONOSCENZA FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm
           SELECT @ragioneRef = DPA_AMMINISTRA.ID_RAGIONE_REFERENTE FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm
           SELECT @ragioneTO = DPA_AMMINISTRA.ID_RAGIONE_TO FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm
           
           IF (@ragioneCC != 0 AND @ragioneCC IS NOT NULL)
		   BEGIN
               SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_CC FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm)
           END
           
           IF (@ragioneComp != 0 AND @ragioneComp IS NOT NULL AND @result < 1)
		   BEGIN
               SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_COMPETENZA FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm)
           END
           
           IF (@ragioneConos != 0 AND @ragioneConos IS NOT NULL AND @result < 1)
		   BEGIN
               SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_CONOSCENZA FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm)
           END
           
           IF (@ragioneRef != 0 AND @ragioneRef IS NOT NULL AND @result < 1)
		   BEGIN
               SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_REFERENTE FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm)
           END
           
           IF (@ragioneTO != 0 AND @ragioneTO IS NOT NULL AND @result < 1)
		   BEGIN
               SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_TO FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = @p_IdAmm)
           END
       END
       
       IF (@p_Idmodello IS NOT NULL AND @result < 1)
	   BEGIN
		   SET @result = 1
		   
           SELECT @result = COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) FROM DPA_RAGIONE_TRASM
           WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
           (SELECT DISTINCT DPA_MODELLI_MITT_DEST.ID_RAGIONE FROM DPA_MODELLI_MITT_DEST
           WHERE DPA_MODELLI_MITT_DEST.ID_MODELLO = @p_Idmodello)
       END
       
    end
    
    IF(@result > 0)
	BEGIN
        SET @result = 1
	END
    ELSE
	BEGIN
        SET @result = 0
    END
    
    RETURN @result
    
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getPeopleUserId]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getPeopleUserId] 
go
CREATE FUNCTION [@db_user].[getPeopleUserId] (@peopleId INT)
RETURNS VARCHAR
AS
BEGIN

RETURN (SELECT user_id FROM people WHERE system_id = @peopleId)

END

go
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_COUNT_TODOLIST]') and 
OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_COUNT_TODOLIST]
GO
CREATE  PROCEDURE [@db_user].[SP_COUNT_TODOLIST]
--parametri di input
@idPeople int,
@idGroup int
AS
--tabella temporanea
CREATE TABLE [@db_user].[#COUNT_TODOLIST]
(
[TOT_DOC] [int],
[TOT_DOC_NO_LETTI] [int],
[TOT_DOC_NO_ACCETTATI] [int],
[TOT_FASC] [int],
[TOT_FASC_NO_LETTI] [int],
[TOT_FASC_NO_ACCETTATI] [int],
[TOT_DOC_PREDISPOSTI] [int],
) ON [PRIMARY]
-- variabili locali
DECLARE @trasmdoctot int;
DECLARE @trasmdocnonletti int;
DECLARE @trasmdocnonaccettati int;
DECLARE @trasmfasctot int;
DECLARE @trasmfascnonletti int;
DECLARE @trasmfascnonaccettati int;
DECLARE @docpredisposti int;
BEGIN
--SETTING DELLE VARIABILI
SET @trasmdoctot = 0
SET @trasmdocnonletti = 0
SET @trasmdocnonaccettati = 0
SET @trasmfasctot = 0
SET @trasmfascnonletti = 0
SET @trasmfascnonaccettati = 0
SET @docpredisposti = 0
--END SETTING
--numero documenti presenti in todolist
   SELECT @trasmdoctot = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          );
--numero documenti non letti in todolist
   SELECT @trasmdocnonletti = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          )
      AND dta_vista = CONVERT(VARCHAR,'17530101',103);
--numero documenti non ancora accettati in todolist
   SELECT @trasmdocnonaccettati = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero fascicoli presenti in todolist
   SELECT @trasmfasctot = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          );
--numero fascicoli non letti in todolist
   SELECT @trasmfascnonletti = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          )
      AND dta_vista = CONVERT(VARCHAR,'17530101',103);
--numero fascicoli non ancora accettati in todolist
   SELECT @trasmfascnonaccettati = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero documenti predisposti
   SELECT @docpredisposti = COUNT (DISTINCT (id_trasmissione))
     FROM dpa_todolist
    WHERE id_profile > 0
      AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
           OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #COUNT_TODOLIST
(TOT_DOC, TOT_DOC_NO_LETTI, TOT_DOC_NO_ACCETTATI, TOT_FASC, TOT_FASC_NO_LETTI, 
TOT_FASC_NO_ACCETTATI, TOT_DOC_PREDISPOSTI)
values
(@trasmdoctot, @trasmdocnonletti, @trasmdocnonaccettati, @trasmfasctot, @trasmfascnonletti, 
@trasmfascnonaccettati, @docpredisposti)
END  
-- return dei risultati
SELECT * FROM #COUNT_TODOLIST
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_SEGRETARIO' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CORR_GLOBALI]
	ADD CHA_SEGRETARIO varchar(1) null;
END;
go

--modelli trasmissione
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_GET_RUOLO_RESP_UO_FROM_UO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_GET_RUOLO_RESP_UO_FROM_UO]
GO
CREATE PROCEDURE [@db_user].[SP_GET_RUOLO_RESP_UO_FROM_UO]
(
@id_UO  int,
@tipo_ruolo char,
@id_corr int,
@result int OUT 
)

AS


DECLARE @isIdParentNull int
DECLARE @noRuoloResponsabile int
DECLARE @numeroResponsabili int
DECLARE @idparent int
DECLARE @Uo_systemId int
DECLARE @id_uo_appo int


SET @isIdParentNull=0
SET @noRuoloResponsabile=0
SET @numeroResponsabili=0
SET @idparent=0
SET @Uo_systemId=@id_UO

begin
while(@isIdParentNull=0 and @noRuoloResponsabile=0)

    BEGIN
    if(@tipo_ruolo='R')
    begin
    	select @numeroResponsabili=count(*) from dpa_corr_globali where id_uo=@Uo_systemId and cha_tipo_urp='R' and cha_responsabile=1

    	if(@numeroResponsabili>0) 
    		BEGIN

                    select @id_uo_appo=system_id from dpa_corr_globali where id_uo=@Uo_systemId and cha_tipo_urp='R' and cha_responsabile=1
					if(@id_uo_appo != @id_corr)
					begin
						set @result=@id_uo_appo
						SET @noRuoloResponsabile=1
					end
					else
					begin
						select @idparent=id_parent from dpa_corr_globali where system_id=@Uo_systemId                	
						if(@idparent>0) 
						begin            
							set @Uo_systemId=@idparent
						end
						else
						begin
							set @result=0           
							set @isIdParentNull=1
						end						
					end
    		END    

    	else
		BEGIN
    
                    select @idparent=id_parent from dpa_corr_globali where system_id=@Uo_systemId                	
					if(@idparent>0) 
					begin
            
                      set @Uo_systemId=@idparent
					end
                   else
					begin
                    set @result=0
           
                     set @isIdParentNull=1
                   end
		END
end
else
begin
    	select @numeroResponsabili=count(*) from dpa_corr_globali where id_uo=@Uo_systemId and cha_tipo_urp='R' and cha_segretario=1

    	if(@numeroResponsabili>0) 
    		BEGIN

                    select @id_uo_appo=system_id from dpa_corr_globali where id_uo=@Uo_systemId and cha_tipo_urp='R' and cha_segretario=1
					
                    if(@id_uo_appo != @id_corr)
					begin
						set @result=@id_uo_appo
						SET @noRuoloResponsabile=1
					end
					else
					begin
						select @idparent=id_parent from dpa_corr_globali where system_id=@Uo_systemId                	
						if(@idparent>0) 
						begin            
							set @Uo_systemId=@idparent
						end
						else
						begin
							set @result=0           
							set @isIdParentNull=1
						end						
					end
    		END    

    	else
		BEGIN
    
                    select @idparent=id_parent from dpa_corr_globali where system_id=@Uo_systemId                	
					if(@idparent>0) 
					begin
            
                      set @Uo_systemId=@idparent
					end
                   else
					begin
                    set @result=0
           
                     set @isIdParentNull=1
                   end
		END

end
     	END
END 

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--ricerca estensione file acquisiti

if not exists (SELECT * FROM syscolumns WHERE name='EXT' and id in 
	(SELECT id FROM sysobjects WHERE name='COMPONENTS' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[COMPONENTS]
	ADD EXT CHAR(7) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_FIRMATO' and id in 
	(SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE]
	ADD CHA_FIRMATO VARCHAR(1) DEFAULT 0 NOT NULL;
END;

GO



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[dpa3_get_hierarchy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[dpa3_get_hierarchy]
GO
CREATE  procedure  [@db_user].[dpa3_get_hierarchy]
@id_amm varchar(64),
@cod varchar(64),
@tipo_ie varchar(2),
@id_Ruolo  varchar(64),
@codes varchar(4000) output
as
declare @c_type varchar(2)
declare @p_cod varchar(64)
declare @system_id int
declare @id_parent int
declare @id_uo int
declare @id_utente int

set @codes = ''
select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @id_utente = id_people, @c_type = cha_tipo_urp from dpa_corr_globali where
var_cod_rubrica=@cod and
cha_tipo_ie=@tipo_ie and
id_amm=@id_amm and
dta_fine is null

while (1 > 0)
begin
if @c_type is null
break

if @c_type = 'U'
begin
if (@id_parent is null or @id_parent = 0)
break
select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_parent and id_amm=@id_amm and dta_fine is null
end
if @c_type = 'R'
begin
if (@id_uo is null or @id_uo = 0)
break
select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_uo and id_amm=@id_amm and dta_fine is null
end
if @c_type = 'P'
begin
select top 1 @p_cod = var_cod_rubrica from dpa_corr_globali where id_gruppo = @id_Ruolo 
and id_amm=@id_amm and dta_fine is null
end
if @p_cod is null
break

select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @c_type = cha_tipo_urp from dpa_corr_globali where var_cod_rubrica=@p_cod and id_amm=@id_amm and dta_fine is null

set @codes = @p_cod + ':' + @codes
end
set @codes = @codes + @cod
GO




IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[Getchaimg]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[Getchaimg] 
go
CREATE function [@db_user].[Getchaimg](@docnum  int)
returns varchar(7)
as
begin
declare @risultato varchar(7)
declare @vpath varchar(128)
declare @vmaxidGenarica int
begin
set @vmaxidGenarica =0

SELECT @vmaxidGenarica =max(v1.version_id) from versions v1, components c where v1.docnumber=@docnum and v1.version_id=c.version_id
end

begin
set @vpath=0
select @vpath=ext from components where docnumber=@docnum and version_id=@vmaxidGenarica
end


if (( @vpath <> '' )OR  (@vpath is not null))
begin
SET @risultato=ltrim(rtrim(@vpath))
end
else
begin set  @risultato='0'
end
return @risultato
end
go

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[putFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[putFile]
GO
create PROCEDURE [@db_user].[putFile]
@versionId int,
@filePath varchar(500),
@fileSize int,
@printThumb varchar(64),
@iscartaceo smallint,
@estensione varchar(7),
@isFirmato char
AS

declare @retValue int
declare @docNumber int

if exists(select version_id from versions where version_id = @versionId)
begin
set @docNumber = (select docnumber from versions where version_id = @versionId)
update 	versions
set 	subversion = 'A',
cartaceo = @iscartaceo
where 	version_id = @versionId

set @retValue = @@rowcount
end
if (@retValue > 0 and exists(select version_id from components where version_id = @versionId))
begin
update 	components
set 	path = @filePath,
file_size = @fileSize,
var_impronta = @printThumb,
ext=@estensione
where 	version_id = @versionId

set @retValue = @@rowcount
end

if (@retValue > 0 and exists(select docnumber from profile where docnumber = @docNumber))
begin

if(@isFirmato='1')
begin
	update 	profile
	set 	cha_img = '1', cha_firmato='1'
	where	docnumber = @docNumber
end
else
begin
	update 	profile
	set 	cha_img = '1'
	where	docnumber = @docNumber
end
set @retValue = @@rowcount
end
return @retValue
go

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO






Insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI]
   (COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED)
 Values
   ('DO_INS_CORR_TUTTI', 'Abilita l''inserimento di un nuovo utente TUTTI nella rubrica', 'N');
   go
Insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI]
   (COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED)
 Values
   ('DO_INS_CORR_RF', 'Abilita l''inserimento di un nuovo utente RF nella rubrica', 'N');
   go
Insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI]
   (COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED)
 Values
   ('DO_INS_CORR_REG', 'Abilita l''inserimento di un nuovo utente REG nella rubrica', 'N');

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_SERVER_IMAP' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_SERVER_IMAP VARCHAR(10) DEFAULT 0 NULL;
END;

GO


if not exists (SELECT * FROM syscolumns WHERE name='ID_REGISTRO' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_MAIL_ELABORATE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_MAIL_ELABORATE]
	ADD ID_REGISTRO INT DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_SOLO_MAIL_PEC' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_SOLO_MAIL_PEC VARCHAR(1) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='NUM_PORTA_IMAP' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD NUM_PORTA_IMAP INT ;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_TIPO_CONNESSIONE' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_TIPO_CONNESSIONE VARCHAR(10) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_INBOX_IMAP' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_INBOX_IMAP VARCHAR(10) DEFAULT 0 NULL;
END;

GO


if not exists(select * from syscolumns where name='ID_RF' and id in 
(select id from sysobjects where name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
ALTER TABLE DPA_CORR_GLOBALI ADD ID_RF INT
END
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[DEST_IN_RF]') and xtype in (N'FN', N'IF', N'TF'))
drop function [@db_user].[DEST_IN_RF]
GO

CREATE function [@db_user].DEST_IN_RF (@docId int)
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)

set @outcome=''

declare cur CURSOR LOCAL
for SELECT B.VAR_DESC_CORR
      FROM DPA_DOC_ARRIVO_PAR L, DPA_L_RUOLO_REG C, DPA_CORR_GLOBALI B, DPA_CORR_GLOBALI D
      WHERE
      L.ID_PROFILE = @docId AND
      L.CHA_TIPO_MITT_DEST = 'F' AND
      L.ID_MITT_DEST = D.SYSTEM_ID AND
      D.ID_RF = C.ID_REGISTRO AND
      C.ID_RUOLO_IN_UO = B.SYSTEM_ID
open cur
fetch next from cur into @item
while(@@fetch_status=0)
begin
set @outcome=@outcome+@db_user.parsenull(@item)+' (D) '
fetch next from cur into @item
end
close cur
deallocate cur

return @outcome
end
go

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[DEST_IN_LISTA]') and xtype in (N'FN', N'IF', N'TF'))
drop function [@db_user].[DEST_IN_LISTA]
GO

CREATE function [@db_user].DEST_IN_LISTA (@docId int)
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)

set @outcome=''

declare cur CURSOR LOCAL
for SELECT L.VAR_DESC_CORR
      FROM DPA_LISTE_DISTR B, DPA_CORR_GLOBALI L, DPA_DOC_ARRIVO_PAR C
      WHERE
      C.ID_PROFILE = @docId AND
      C.CHA_TIPO_MITT_DEST = 'L' AND 
      B.ID_LISTA_DPA_CORR = C.ID_MITT_DEST
      AND L.SYSTEM_ID=B.ID_DPA_CORR
      AND L.DTA_FINE IS NULL
open cur
fetch next from cur into @item
while(@@fetch_status=0)
begin
set @outcome=@outcome+@db_user.parsenull(@item)+' (D) '
fetch next from cur into @item
end
close cur
deallocate cur

return @outcome
end

GO


ALTER  function [@db_user].corrcat (@docId int, @dirProt varchar(1))
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(1)

set @outcome=''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
from dpa_corr_globali c WITH (NOLOCK), dpa_doc_arrivo_par dap WITH (NOLOCK)
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr
while(@@fetch_status=0)
begin
if (@dirProt='P' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+'; '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='P' and @dirCorr='L')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_LISTA(@docId)
end

if (@dirProt='P' and @dirCorr='F')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_RF(@docId)
end

fetch next from cur into @item,@dirCorr

end

close cur

deallocate cur

if (len(@outcome)>0)

set @outcome = substring(@outcome,1,(len(@outcome)-1))

return @outcome

end

GO
if not exists (SELECT * FROM syscolumns WHERE name='VAR_BOX_MAIL_ELABORATE' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_BOX_MAIL_ELABORATE VARCHAR(20) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_MAIL_NON_ELABORATE' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_MAIL_NON_ELABORATE VARCHAR(20) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='VAR_MAIL_NON_ELABORATE' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD VAR_MAIL_NON_ELABORATE VARCHAR(20) DEFAULT 0 NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_IMAP_SSL' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI]
	ADD CHA_IMAP_SSL VARCHAR(1) DEFAULT 0 NULL;
END;

GO

if (not exists (select * from [@db_user].[dpa_el_registri] where (VAR_TIPO_CONNESSIONE ='POP') or (VAR_TIPO_CONNESSIONE ='IMAP')))
BEGIN
	UPDATE @db_user.dpa_el_registri SET VAR_TIPO_CONNESSIONE = 'POP' where (VAR_TIPO_CONNESSIONE IS null) ;
END;

GO

if not exists(select * from syscolumns where name='TIPO' and id in 
(select id from sysobjects where name='DPA_SALVA_RICERCHE' and xtype='U'))
BEGIN
ALTER TABLE DPA_SALVA_RICERCHE ADD TIPO CHAR(1)
END
GO

declare @tt int;
SELECT @tt = count(distinct(tipo)) FROM DPA_SALVA_RICERCHE;
if (@tt = 0)
BEGIN
	UPDATE [@db_user].[DPA_SALVA_RICERCHE]
	SET TIPO = 'D';
END;
GO

if not exists(select * from syscolumns where name='USER_ID_DELEGATO' and id in 
(select id from sysobjects where name='DPA_LOGIN' and xtype='U'))
BEGIN
ALTER TABLE DPA_LOGIN ADD USER_ID_DELEGATO VARCHAR(20)
END
GO

if not exists(select * from syscolumns where name='CHA_DELEGA' and id in 
(select id from sysobjects where name='DPA_LOGIN' and xtype='U'))
BEGIN
ALTER TABLE DPA_LOGIN ADD CHA_DELEGA CHAR(1)
END
GO

if not exists(select * from syscolumns where name='IDPEOPLEDELEGATO' and id in 
(select id from sysobjects where name='DPA_NOTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_NOTE ADD IDPEOPLEDELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='DPA_TODOLIST' and xtype='U'))
BEGIN
ALTER TABLE DPA_TODOLIST ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='DPA_DIAGRAMMI_STO' and xtype='U'))
BEGIN
ALTER TABLE DPA_DIAGRAMMI_STO ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='DPA_TRASM_UTENTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASM_UTENTE ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='DPA_TRASMISSIONE' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASMISSIONE ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='VERSIONS' and xtype='U'))
BEGIN
ALTER TABLE VERSIONS ADD ID_PEOPLE_DELEGATO INTEGER
END
GO


--deleghe
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


ALTER  PROCEDURE [@db_user].[createAllegato]
@idDocumentoPrincipale int,
@idPeople int,
@comments varchar(200),
@numeroPagine int,
@idPeopleDelegato int,
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
ID_DOCUMENTO_PRINCIPALE,
ID_PEOPLE_DELEGATO
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
@idDocumentoPrincipale,
@idPeopleDelegato
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
DTA_CREAZIONE,
CHA_DA_INVIARE,
ID_PEOPLE_DELEGATO
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
GETDATE(),
1,
@idPeopleDelegato
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


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[createDocSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[createDocSP]
GO
CREATE   PROCEDURE [@db_user].[createDocSP]
@idpeople int,
@doctype varchar (128),
@idPeopleDelegato int,
@isFirmato varchar (1),
@systemId int out

AS

DECLARE @Docnum  int
DECLARE @Verid int
DECLARE @IDDocType int

BEGIN

SET @systemId= 0

set @IDDocType =  ( SELECT SYSTEM_ID  FROM
DOCUMENTTYPES
WHERE TYPE_ID = @doctype)

IF (NOT @IDDocType IS NULL)
BEGIN
INSERT INTO Profile
(

TYPIST,
AUTHOR,
DOCUMENTTYPE,
CREATION_DATE,
CREATION_TIME,
ID_PEOPLE_DELEGATO
)
VALUES
(

@idpeople,
@idpeople,
@IDDocType,
GETDATE(),
GETDATE(),
@idPeopleDelegato

)

-- Reperimento identity appena immessa
SET @systemId=scope_identity()
SET @Docnum = @systemId

--AGGIORNO LA PROFILE CON DOCNUMBER=SYSTEMID APPENA INSERITA
UPDATE PROFILE
SET DOCNUMBER = @Docnum
WHERE SYSTEM_ID = @systemId


IF (@@ROWCOUNT = 0)
BEGIN
SET @systemId= 0
RETURN @systemId
END
ELSE

BEGIN

INSERT INTO VERSIONS
(
DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR, TYPIST,
DTA_CREAZIONE, 
ID_PEOPLE_DELEGATO
) VALUES (
@Docnum, 1, '!', '1',@idpeople, @idpeople,  GETDATE(), @idPeopleDelegato
)

SET @Verid = scope_identity()


INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE,
CHA_FIRMATO
) VALUES (
@Verid, @Docnum, 0, @isFirmato
)


INSERT INTO SECURITY
(
THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO
) VALUES (
@systemId, @idpeople, 0, NULL, NULL
)

END

END
ELSE

SET @systemId = 0
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
IF NOT EXISTS (select * from sysobjects where name = 'DPA_DELEGHE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_DELEGHE] (
	[SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[ID_PEOPLE_DELEGANTE] [int] NULL ,
	[ID_RUOLO_DELEGANTE] [int] NULL ,
	[ID_PEOPLE_DELEGATO] [int] NULL ,
	[ID_RUOLO_DELEGATO] [int] NULL ,
	[DATA_DECORRENZA] [datetime] NULL ,
	[DATA_SCADENZA] [datetime] NULL ,
	[CHA_IN_ESERCIZIO] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_PEOPLE_DELEGANTE][varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_RUOLO_DELEGANTE][varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_PEOPLE_DELEGATO][varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_RUOLO_DELEGATO][varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ID_UO_DELEGATO] [int] NULL
) ON [PRIMARY]
end
go

if exists (select * from dbo.sysindexes where name = N'indx_dpa_deleghe1' and id = object_id(N'[@db_user].[DPA_DELEGHE]'))
drop index [@db_user].[DPA_DELEGHE].[indx_dpa_deleghe1]
GO
CREATE 
  INDEX [indx_dpa_deleghe1] ON [@db_user].[DPA_DELEGHE] ([SYSTEM_ID ])
GO

go
if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='GEST_DELEGHE'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('GEST_DELEGHE','Abilita il sottomenu'' Deleghe del menu'' Gestione'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DIRITTO_DELEGA'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('DIRITTO_DELEGA','Abilita i pulsanti NUOVA, REVOCA e MODIFICA nella pagina di gestione delle deleghe'); 
END
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DOCSADM].[TR_INSERT_DPA_TODOLIST]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
BEGIN

EXEC dbo.sp_executesql @statement = N'
ALTER      TRIGGER [TR_INSERT_DPA_TODOLIST] ON [@db_user].[DPA_TRASMISSIONE]
AFTER UPDATE
AS
IF UPDATE(DTA_INVIO)
BEGIN
INSERT INTO DPA_TODOLIST
SELECT DT.system_id, dtu.id_trasm_singola, dtu.system_id,
DT.dta_invio, DT.id_people, DT.id_ruolo_in_uo,
dtu.id_people,dts.id_ragione,DT.var_note_generali,
dts.var_note_sing,dts.dta_scadenza, DT.id_profile,
DT.id_project,
CONVERT (INT,@db_user.vardescribe(dts.id_corr_globale,''ID_GRUPPO'')) AS id_ruolo_dest,
CONVERT (INT,@db_user.vardescribe(DT.id_profile,''PROF_IDREG'')) AS id_registro,
dts.CHA_TIPO_TRASM,
(case when dtu.dta_vista is null then convert (datetime, ''17530101'', 121) else dtu.DTA_VISTA end) as dta_vista,
DT.ID_PEOPLE_DELEGATO
FROM INSERTED DT, DPA_TRASM_SINGOLA dts,DPA_TRASM_UTENTE dtu
WHERE dtu.id_trasm_singola = dts.system_id AND dts.id_trasmissione = DT.system_id AND dtu.cha_in_todolist = 1
END
'
END
ELSE
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE     TRIGGER [TR_INSERT_DPA_TODOLIST] ON [DOCSADM].[DPA_TRASMISSIONE]
AFTER UPDATE
AS
IF UPDATE(DTA_INVIO)
BEGIN
INSERT INTO DPA_TODOLIST
SELECT DT.system_id, dtu.id_trasm_singola, dtu.system_id,
DT.dta_invio, DT.id_people, DT.id_ruolo_in_uo,
dtu.id_people,dts.id_ragione,DT.var_note_generali,
dts.var_note_sing,dts.dta_scadenza, DT.id_profile,
DT.id_project,
CONVERT (INT,DOCSADM.vardescribe(dts.id_corr_globale,''ID_GRUPPO'')) AS id_ruolo_dest,
CONVERT (INT,DOCSADM.vardescribe(DT.id_profile,''PROF_IDREG'')) AS id_registro,
dts.CHA_TIPO_TRASM,
(case when dtu.dta_vista is null then convert (datetime, ''17530101'', 121) else dtu.DTA_VISTA end) as dta_vista,
DT.id_people_delegato
FROM INSERTED DT, DPA_TRASM_SINGOLA dts,DPA_TRASM_UTENTE dtu
WHERE dtu.id_trasm_singola = dts.system_id AND dts.id_trasmissione = DT.system_id AND dtu.cha_in_todolist = 1
END
'
END

GO

--fine deleghe

--AREA CONSERVAZIONE
if exists(select * from syscolumns where name='VAR_OGGETTO' and id in 
		(select id from sysobjects where name='DPA_ITEMS_CONSERVAZIONE' and xtype='U'))
begin		
	alter table @db_user.DPA_ITEMS_CONSERVAZIONE alter column VAR_OGGETTO VARCHAR(2000)
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_ESITO' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ITEMS_CONSERVAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ITEMS_CONSERVAZIONE]
	ADD CHA_ESITO CHAR(1) NULL;
END;

GO


if not exists (SELECT * FROM syscolumns WHERE name='ID_PROFILE_TRASMISSIONE' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AREA_CONSERVAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AREA_CONSERVAZIONE]
	ADD ID_PROFILE_TRASMISSIONE INT NULL;
END;

GO
--conservazione html
if not exists (SELECT * FROM syscolumns WHERE name='VAR_TIPO_ATTO' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ITEMS_CONSERVAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ITEMS_CONSERVAZIONE]
	ADD VAR_TIPO_ATTO VARCHAR(64);
END;


GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_INSERT_AREA_CONS]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_INSERT_AREA_CONS]
GO
CREATE PROCEDURE [@db_user].[SP_INSERT_AREA_CONS]
@idAmm int,
@idPeople int,
@idProfile int,
@idProject int,
@codFasc varchar(64),
@oggetto varchar(64),
@tipoDoc char,
@idGruppo int,
@idRegistro int,
@docNumber int,
@userId varchar(32),
@tipoOggetto char,
@tipoAtto   VARCHAR,
@result int  OUT
AS
BEGIN

DECLARE @idRuoloInUo int
DECLARE @id_cons_1   int
DECLARE @res         int



SET @result = -1
SET @idRuoloInUo = 0
SET @id_cons_1 = 0
SET @res = 0



SELECT @idRuoloInUo = DPA_CORR_GLOBALI.SYSTEM_ID FROM DPA_CORR_GLOBALI
WHERE DPA_CORR_GLOBALI.ID_GRUPPO = @idGruppo



BEGIN
SELECT DISTINCT @res = DPA_AREA_CONSERVAZIONE.SYSTEM_ID
FROM DPA_AREA_CONSERVAZIONE
WHERE
DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo AND
DPA_AREA_CONSERVAZIONE.CHA_STATO = 'N'

END
IF (@res > 0)

BEGIN

INSERT INTO DPA_ITEMS_CONSERVAZIONE(
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
@res,
@idProfile,
@idProject,
@tipoDoc,
@oggetto,
@idRegistro,
getdate(),
'N',
NULL,
@codFasc,
@docNumber,
@tipoOggetto,
@tipoAtto
)

SET @result = SCOPE_IDENTITY()
END
ELSE
BEGIN
INSERT INTO DPA_AREA_CONSERVAZIONE(
ID_AMM,
ID_PEOPLE,
ID_RUOLO_IN_UO,
CHA_STATO,
DATA_APERTURA,
USER_ID,
ID_GRUPPO
)
VALUES
(
@idAmm,
@idPeople,
@idRuoloInUo,
'N',
getdate(),
@userId,
@idGruppo)

SET @id_cons_1=SCOPE_IDENTITY()

INSERT INTO DPA_ITEMS_CONSERVAZIONE(
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
@id_cons_1,
@idProfile,
@idProject,
@tipoDoc,
@oggetto,
@idRegistro,
getdate(),
'N',
NULL,
@codFasc,
@docNumber,
@tipoOggetto,
@tipoAtto)

SET @result = SCOPE_IDENTITY()
END
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--conservazione modifiche per nuove verifiche supporti


if not exists (SELECT * FROM syscolumns WHERE name='PERC_VERIFICA' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_SUPPORTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_SUPPORTO]
	ADD PERC_VERIFICA INT NULL;
END;
go


IF NOT EXISTS (select * from sysobjects where name = 'DPA_CONS_VERIFICA' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_CONS_VERIFICA] (
  [SYSTEM_ID] 	    [int]    NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_SUPPORTO]    	[int] NULL,
  [ID_ISTANZA]      [int] NULL,
  [DATA_VER]        [DATETIME] NULL,
  [NUM_VER]	        [int] NULL,
  [VAR_NOTE]        [VARCHAR] (256) NULL,
  [PERCENTUALE]     [INT] NULL,
  [ESITO]           [VARCHAR] (1) NULL
)
end
GO


--BONIFICA CAMPI A CZ MESSI DA MARIA CINQUEPALMI
if exists (SELECT * FROM syscolumns WHERE name='PD_OBJ_TYPE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_OBJ_TYPE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_DOC_BARCODE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_DOC_BARCODE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_FILE_PART' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_FILE_PART;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_BOX' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_BOX;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_SUSPEND' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_SUSPEND;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ACTIONBY_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ACTIONBY_DATE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_FAVOURITES' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_FAVOURITES;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_SUPSEDED' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_SUPSEDED;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_SUPSEDES' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_SUPSEDES;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_VITAL' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_VITAL;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_MEDIA_TYPE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_MEDIA_TYPE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ORGANIZATION' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ORGANIZATION;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_FILE_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_FILE_DATE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_DATE_CREATED' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_DATE_CREATED;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_PUBLISH_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_PUBLISH_DATE;	
END;
GO


if exists (SELECT * FROM syscolumns WHERE name='PD_EMAIL_BCC' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_EMAIL_BCC;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_EMAIL_CC' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_EMAIL_CC;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_EMAIL_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_EMAIL_DATE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ADDRESSEE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ADDRESSEE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ORIGINATOR' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ORIGINATOR;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_CLASSIFICATION' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_CLASSIFICATION;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_SUPSEDED_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_SUPSEDED_DATE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_DOC2CSGMT_LINK' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_DOC2CSGMT_LINK;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ATTACH_CHANGE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ATTACH_CHANGE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_ACTIONED' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_ACTIONED;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_VREVIEW_DATE' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_VREVIEW_DATE;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_PRTO_ACTION' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_PRTO_ACTION;	
END;
GO

if exists (SELECT * FROM syscolumns WHERE name='PD_STATUSES' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROFILE] DROP COLUMN PD_STATUSES;	
END;
GO
-- FINE BONIFICA CAMPI A CZ MESSI DA MARIA CINQUEPALMI


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_INSERT_DPA_SUPPORTO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_INSERT_DPA_SUPPORTO]
GO
CREATE PROCEDURE [@db_user].[SP_INSERT_DPA_SUPPORTO]
@copia             int,
@collFisica        varchar(250),
@dataUltimaVer     datetime,
@dataEliminazione  datetime,
@esitoUltimaVer    int,
@numeroVer         int,
@dataProxVer       datetime,
@dataAppoMarca     datetime,
@dataScadMarca     datetime,
@marca             varchar(3000),
@idCons            int,
@tipoSupp          int,
@stato             char,
@note              varchar(500),
@query             varchar,
@idSupp            int,
@percVerifica      int, 
@result            int  OUTPUT,
@newId             int OUTPUT


as

BEGIN

declare @numSuppProd int
declare @numSuppTotali int

BEGIN
SET @numSuppProd=0
SET @numSuppTotali=0
SET @result=NULL


IF(@query='I')
BEGIN

INSERT INTO DPA_SUPPORTO (
COPIA,
DATA_PRODUZIONE,
VAR_COLLOCAZIONE_FISICA,
DATA_ULTIMA_VERIFICA,
DATA_ELIMINAZIONE,
ESITO_ULTIMA_VERIFICA,
VERIFICHE_EFFETTUATE,
DATA_PROX_VERIFICA,
DATA_APPO_MARCA,
DATA_SCADENZA_MARCA,
VAR_MARCA_TEMPORALE,
ID_CONSERVAZIONE,
ID_TIPO_SUPPORTO,
CHA_STATO,
VAR_NOTE,
PERC_VERIFICA
)
VALUES
(
@copia,
getdate(),
@collFisica,
@dataUltimaVer,
@dataEliminazione,
@esitoUltimaVer,
@numeroVer,
@dataProxVer,
@dataAppoMarca,
@dataScadMarca,
@marca,
@idCons,
@tipoSupp,
@stato,
@note,
@percVerifica
)

SET @newId = SCOPE_IDENTITY()

SELECT @numSuppProd=COUNT(*) FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=@idCons

SELECT @numSuppTotali=COUNT(*) FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=@idCons

IF(@numSuppProd=@numSuppTotali)
BEGIN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=@idCons

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=@idCons

SET @result=1
END
ELSE
BEGIN
SET @result=0
END
END

ELSE
BEGIN

UPDATE DPA_SUPPORTO SET DATA_PRODUZIONE=getdate(), VAR_COLLOCAZIONE_FISICA=@collFisica,
DATA_PROX_VERIFICA=@dataProxVer, CHA_STATO= @stato, VAR_NOTE=@note, DATA_ULTIMA_VERIFICA=getdate(), VERIFICHE_EFFETTUATE=@numeroVer, ESITO_ULTIMA_VERIFICA=@esitoUltimaVer,
PERC_VERIFICA=@percVerifica
WHERE SYSTEM_ID=@idSupp

SET @newId = @idSupp

SELECT @numSuppProd=COUNT(*)  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND
ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

SELECT @numSuppTotali=COUNT(*) FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

IF(@numSuppProd=@numSuppTotali)
BEGIN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

SET @result=1
END
ELSE
BEGIN
SET @result=0
END


END

END

END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER ON 
GO

SET ANSI_NULLS ON 
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[GetSeDocTrasmConRag]') AND xtype in (N'FN', N'IF', N'TF'))
Begin
DROP FUNCTION [GetSeDocTrasmConRag]
end
GO

CREATE function [GetSeDocTrasmConRag](@idprofile INT,@vardescragione VARCHAR(256))
returns INT
as
begin
declare @risultato INT

      if(UPPER(@vardescragione) <> 'TUTTE')
            begin
                  SELECT @risultato = count(tx.system_id) 
                  FROM dpa_trasmissione tx, dpa_trasm_singola ts, dpa_ragione_trasm tr
                        WHERE tx.system_id = ts.id_trasmissione
                  AND ts.id_ragione = tr.system_id
                  AND UPPER(tr.var_desc_ragione) = UPPER(@varDescRagione)
                  AND tx.id_profile = @idprofile
            end
      else
            begin
                  --se  stato trasmesso almeno una volta 
                  SELECT @risultato = count(tx.system_id) 
                  FROM dpa_trasmissione tx
                  WHERE tx.id_profile = @idprofile
            end
      
      if (@risultato > 0) set @risultato = 1 else set @risultato = 0
 
      return @risultato
end
 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


--conservazione rigenera marca temporale

if not exists (SELECT * FROM syscolumns WHERE name='NUM_MARCA' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_SUPPORTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_SUPPORTO]
	ADD NUM_MARCA INT NULL;
END;
go

--

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[Getidamm]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[Getidamm] 
go
CREATE FUNCTION [@db_user].[Getidamm] 
(
@idPeople INT
)
RETURNS INT
AS

BEGIN

DECLARE @retValue INT

select @retValue=id_amm from PEOPLE WHERE SYSTEM_ID = @idPeople


RETURN @retValue
end
go

--INVIO RICEVUTE RITORNO MANUALI
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_INVIO_RICEVUTE')
BEGIN
insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('DO_INVIO_RICEVUTE','Abilita pulsante per invio manuale della ricevuta di ritorno','', 'N')
END
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[sp_ins_dpa_stato_invio]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[sp_ins_dpa_stato_invio]
GO

CREATE  PROCEDURE [@db_user].[sp_ins_dpa_stato_invio]
AS

DECLARE @profileId  INT
DECLARE @idMezzoSped INT

BEGIN 

select @idMezzoSped=system_id  from documenttypes where type_id='INTEROPERABILITA'

DECLARE curr_dpa_stato_invio CURSOR FOR
SELECT ID_PROFILE  FROM dpa_stato_invio
 
OPEN curr_dpa_stato_invio 
fetch next from curr_dpa_stato_invio into  @profileId

WHILE @@FETCH_STATUS = 0

BEGIN

UPDATE dpa_stato_invio SET ID_DOCUMENTTYPE=@idMezzoSped  WHERE ID_PROFILE=@profileId

fetch next from curr_dpa_stato_invio into @profileId

END

close curr_dpa_stato_invio
deallocate curr_dpa_stato_invio

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[sp_ins_documentype_in_profile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[sp_ins_documentype_in_profile]
GO

CREATE  PROCEDURE [@db_user].[sp_ins_documentype_in_profile]
AS

DECLARE @profileId  INT
DECLARE @idMezzoSped INT

BEGIN 

select @idMezzoSped=system_id  from documenttypes where type_id='INTEROPERABILITA'

DECLARE curr_dpa_stato_invio CURSOR FOR
SELECT SYSTEM_ID  FROM PROFILE WHERE cha_invio_conferma='1'
 
OPEN curr_dpa_stato_invio 
fetch next from curr_dpa_stato_invio into  @profileId

WHILE @@FETCH_STATUS = 0

BEGIN

UPDATE PROFILE SET DOCUMENTTYPE=@idMezzoSped WHERE system_id=@profileId

fetch next from curr_dpa_stato_invio into @profileId

END

close curr_dpa_stato_invio
deallocate curr_dpa_stato_invio

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


ALTER   FUNCTION [@db_user].[getcodRubricaCorr]
(
@sys int
) 

RETURNS varchar(128)
AS

BEGIN
DECLARE @tmpVar varchar(128)


SET @tmpVar = ''
      SELECT
      @tmpVar=UPPER(VAR_COD_RUBRICA) 
      FROM DPA_CORR_GLOBALI
      WHERE system_id=@sys
      
 
   
      RETURN @tmpVar
         
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getnumProtoStampa]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getnumProtoStampa]
go
CREATE  FUNCTION [@db_user].[getnumProtoStampa]
(
@docnumber INT,
@anno INT,
@numeroProtStart INT,
@numeroProtEnd INT
)
RETURNS INT AS

BEGIN
DECLARE @cnt INT
DECLARE @retValue INT
SET @cnt = 0

IF(@docnumber IS NOT NULL)
BEGIN
IF(@anno IS NOT NULL)
BEGIN
IF(@numeroProtStart IS NOT NULL)
BEGIN
SELECT @cnt = COUNT(A.SYSTEM_ID)
From DPA_STAMPAREGISTRI A, PROFILE B
where A.NUM_PROTO_START <= @numeroProtStart
AND A.NUM_PROTO_END >= @numeroProtEnd
AND A.NUM_ANNO = @anno
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber=@docnumber

END
ELSE
BEGIN
SELECT @cnt = COUNT(A.SYSTEM_ID)
From DPA_STAMPAREGISTRI A, PROFILE B
where NUM_ANNO = @anno
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber=@docnumber
END
END
ELSE
BEGIN
IF(@numeroProtStart IS NOT NULL)
BEGIN
SELECT @cnt = COUNT(A.SYSTEM_ID)
From DPA_STAMPAREGISTRI A, PROFILE B
where NUM_PROTO_START <= @numeroProtStart
AND NUM_PROTO_END >= @numeroProtEnd
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber=@docnumber
END
ELSE
BEGIN
SELECT @cnt = COUNT(A.SYSTEM_ID)
From DPA_STAMPAREGISTRI A, PROFILE B
WHERE A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber=@docnumber

END
END
END

IF (@cnt > 0)  set @retValue = 1
ELSE	set @retValue = 0

RETURN @retValue

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getRegistroStampa]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  FUNCTION [@db_user].[getRegistroStampa]
go
CREATE FUNCTION [@db_user].[getRegistroStampa]
(
	@docnumber INT,
	@id_registro INT
)  
RETURNS INT AS  

BEGIN
   	DECLARE @cnt INT
	DECLARE @retValue INT
	SET @cnt = 0

IF(@docnumber IS NOT NULL AND @id_registro IS NOT NULL)
BEGIN
	SELECT @cnt = COUNT(A.SYSTEM_ID)
	From DPA_STAMPAREGISTRI A, PROFILE B
	where A.DOCNUMBER = B.SYSTEM_ID
	and A.docnumber=@docnumber
	and a.id_registro=b.id_registro
	and a.id_registro=@id_registro
END

   	IF (@cnt > 0)  set @retValue = 1
   	ELSE	set @retValue = 0

   	RETURN @retValue

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- Update funzione GetCountNote. Ora permette anche una ricerca in base alla visibilità
-- della nota
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [@db_user].[GetCountNote] (@tipoOggetto char(1), @idOggetto int, @note nvarchar(2000), @idUtente int, @idGruppo int, @tipoRic char(1))  
RETURNS int AS
BEGIN 
	declare @ret int	-- variabile intera che conterrà il risultato da restituire

	if(@tipoRic = 'Q')
			SELECT @ret = COUNT(SYSTEM_ID)
				FROM   DPA_NOTE N 
				WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
					   N.IDOGGETTOASSOCIATO = @idOggetto AND
					   upper(N.TESTO) LIKE upper('%'+@note+'%') AND
					  (N.TIPOVISIBILITA = 'T' OR
					  (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente) OR
					  (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo))
	else if(@tipoRic = 'T')
		SELECT @ret = COUNT(SYSTEM_ID)
		FROM   DPA_NOTE N 
		WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
			   N.IDOGGETTOASSOCIATO = @idOggetto AND
			   upper(N.TESTO) LIKE upper('%'+@note+'%') AND
			  (N.TIPOVISIBILITA = 'T')

		else if(@tipoRic = 'P')
			SELECT @ret = COUNT(SYSTEM_ID)
			FROM   DPA_NOTE N 
			WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
				   N.IDOGGETTOASSOCIATO = @idOggetto AND
				   upper(N.TESTO) LIKE upper('%'+@note+'%') AND
				  (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente)

			else if(@tipoRic = 'R')
				SELECT @ret = COUNT(SYSTEM_ID)
				FROM   DPA_NOTE N 
				WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
					   N.IDOGGETTOASSOCIATO = @idOggetto AND
					   upper(N.TESTO) LIKE upper('%'+@note+'%') AND
					  (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo)
	
	RETURN  @ret
END
-- Fine modifica funzione GetCountNote.
go
if not exists (SELECT * FROM syscolumns WHERE name='ID_DOCUMENTTYPES' and id in 
      (SELECT id FROM sysobjects WHERE name='DPA_DOC_ARRIVO_PAR' and xtype='U'))
BEGIN
      ALTER TABLE [@db_user].[DPA_DOC_ARRIVO_PAR]
      ADD ID_DOCUMENTTYPES INT;
END;


GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[dpa_ins_ut_ruolo_in_mod_trasm]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[dpa_ins_ut_ruolo_in_mod_trasm]
GO

CREATE PROCEDURE [@db_user].[dpa_ins_ut_ruolo_in_mod_trasm]
@idPeople int,
@idCorrGlobale int,
@returnvalue int out
AS

declare @ID int
declare @idModello int
declare @IdModelloMittDest int

BEGIN
set @returnvalue = 0

DECLARE modello CURSOR LOCAL FOR
SELECT  distinct g.SYSTEM_ID as ID, g.ID_MODELLO as IdModello, g.ID_MODELLO_MITT_DEST as IdModelloMittDest
from dpa_modelli_dest_con_notifica g 
where g.ID_MODELLO_MITT_DEST in (
SELECT  distinct a.SYSTEM_ID as ID
FROM dpa_modelli_mitt_dest a
WHERE a.CHA_TIPO_MITT_DEST = 'D'
AND a.ID_CORR_GLOBALI = @idCorrGlobale
AND a.CHA_TIPO_URP = 'R')
and g.ID_PEOPLE not in (@idPeople)

OPEN modello
FETCH next from modello into @ID,@idModello,@IdModelloMittDest
while(@@fetch_status=0)
BEGIN

insert into dpa_modelli_dest_con_notifica
(id_modello_mitt_dest, id_people, id_modello)
VALUES
(@IdModelloMittDest, @idPeople, @IdModello);


if @@error <> 0 set @returnvalue = -1

FETCH next from modello into @ID,@idModello,@IdModelloMittDest

END    -- GENERALE
-- fine codice ciclo cursore
close modello
deallocate modello
set @returnvalue = 1


return @returnvalue
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[dpa_del_ut_ruolo_in_mod_trasm]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[dpa_del_ut_ruolo_in_mod_trasm]
GO

CREATE PROCEDURE [@db_user].[dpa_del_ut_ruolo_in_mod_trasm]
@idPeople int,
@idCorrGlobale int,
@returnvalue int out
AS

BEGIN
set @returnvalue = 0

delete from dpa_modelli_dest_con_notifica
where system_id in (
select distinct c.system_id from dpa_modelli_mitt_dest b, dpa_modelli_dest_con_notifica c
where c.ID_PEOPLE = @idPeople
and c.ID_MODELLO = b.ID_MODELLO
and b.CHA_TIPO_MITT_DEST = 'D'
and b.CHA_TIPO_URP = 'R'
and b.ID_CORR_GLOBALI = @idCorrGlobale
)

IF(@@ERROR <> 0)
	set @returnvalue = 0
ELSE
	set @returnvalue = 1

return @returnvalue
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
if (not exists (select * from @db_user.DOCUMENTTYPES where TYPE_ID='INTEROPERABILITA'))
begin
INSERT INTO @db_user.DOCUMENTTYPES (  TYPE_ID, DESCRIPTION, DISABLED, STORAGE_TYPE, RETENTION_DAYS,
MAX_VERSIONS, MAX_SUBVERSIONS, FULL_TEXT, TARGET_DOCSRVR, RET_2, RET_2_TYPE, KEEP_CRITERIA,
VERSIONS_TO_KEEP, CHA_TIPO_CANALE ) VALUES (  'INTEROPERABILITA', 'INTEROPERABILITA', NULL, 'A', 0, 99, 26, 'N', 0, 0, 'A', 'L', 0, 'I')
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='COL_SEGN' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA]
	ADD COL_SEGN char(1)
END
GO

if exists (SELECT * FROM syscolumns WHERE name='COL_SEGN' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	if exists (select * from DPA_AMMINISTRA where COL_SEGN is NULL)
	BEGIN
		UPDATE DPA_AMMINISTRA set COL_SEGN = '0' where COL_SEGN is NULL
	END
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='NEWS' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA]
	ADD NEWS varchar(512)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='ENABLE_NEWS' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA]
	ADD ENABLE_NEWS char(1)
END
GO

if not exists(select id from sysobjects where name='DPA_CHIAVI_CONFIGURAZIONE' and xtype='U')
begin
CREATE TABLE @db_user.DPA_CHIAVI_CONFIGURAZIONE(
    SYSTEM_ID INT IDENTITY PRIMARY KEY,
    ID_AMM INT,
    VAR_CODICE  VARCHAR(32) NOT NULL,
    VAR_DESCRIZIONE  VARCHAR(256),
    VAR_VALORE  VARCHAR(128) NOT NULL,
    CHA_TIPO_CHIAVE CHAR(1),
    CHA_VISIBILE CHAR(1) DEFAULT 1 NOT NULL,
    CHA_MODIFICABILE CHAR(1) DEFAULT 1 NOT NULL,
    CHA_GLOBALE CHAR(1) DEFAULT 1 NOT NULL
)
end
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_LOG_PATH' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (null, 'FE_LOG_PATH', 'path del log del FrontEnd', 'c:/docspa30/logs/frontend/',
    'F','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_LOG_LEVEL' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (null, 'FE_LOG_LEVEL', 'flag attivazione log del FrontEnd (0=disattivato; 2= attivato)', '2',
    'F','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_LOG_PATH' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (null, 'BE_LOG_PATH', 'path del log del BackEnd', 'c:/docspa30/logs/backend/',
    'B','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_LOG_LEVEL' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (null, 'BE_LOG_LEVEL', 'flag attivazione log del BackEnd (0=disattivato; 2= attivato)', '2',
    'B','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_LOG_PATH' and ID_AMM = 0))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_LOG_PATH', 'path del log del FrontEnd', 'c:/docspa30/logs/frontend/',
    'F','1','1', '1')
END
GO
  
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_LOG_LEVEL' and ID_AMM = 0))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_LOG_LEVEL', 'flag attivazione log del FrontEnd (0=disattivato; 2= attivato)', '2',
    'F','1','1', '1')
END
GO
    
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_LOG_PATH' and ID_AMM = 0))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'BE_LOG_PATH', 'path del log del BackEnd', 'c:/docspa30/logs/backend/',
    'B','1','1', '1')
END
GO
    
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_LOG_LEVEL' and ID_AMM = 0))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'BE_LOG_LEVEL', 'flag attivazione log del BackEnd (0=disattivato; 2= attivato)', '2',
    'B','1','1', '1')
END
GO         

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[dpa_ins_tx_new_ut_ruolo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[dpa_ins_tx_new_ut_ruolo]
GO
CREATE PROCEDURE [@db_user].[dpa_ins_tx_new_ut_ruolo]
@idpeople int,
@idcorrglob int,
@returnvalue int out
AS

declare @id_trasmutente int
declare @tmpvar int
declare @sysidtx int
declare @idpeoplemitt int
declare @idruolomitt int

declare @idTrasm int
declare @ID int
declare @sysidtxut int
declare @dtainvio datetime
declare @idpeopletx int
declare @ruolo_in_uo int
declare @idragione int
declare @notegen varchar (250)
declare @note_sing varchar (250)
declare @scadenza datetime
declare @idprofile int
declare @idproject int
declare @idreg int
declare @cha_tipodest varchar(2)
declare @idtscorrglob int


BEGIN
set @returnvalue = 0

DECLARE trasm CURSOR LOCAL FOR
SELECT DISTINCT b.system_id AS ID,A.system_id as idTrasm,
a.id_people as idpeopletx,a.id_ruolo_in_uo as ruolo_in_uo,a.dta_invio as dtainvio
,a.var_note_generali as note_gen,b.id_ragione as idragione,a.id_project as idproj
,a.id_profile as idprof,b.var_note_sing as note_sing ,b.dta_scadenza as scadenza
,b.cha_tipo_dest as cha_tipodest, b.id_corr_globale as idtscorrglob
FROM
dpa_trasmissione a,dpa_trasm_singola b,dpa_trasm_utente c,dpa_ragione_trasm d
WHERE
a.system_id = b.id_trasmissione AND b.system_id = c.id_trasm_singola
AND a.dta_invio IS NOT NULL AND b.id_corr_globale = @idcorrglob
AND (a.cha_tipo_oggetto = 'D' OR a.cha_tipo_oggetto = 'F')
AND b.id_ragione = d.system_id
AND c.id_people NOT IN (@idpeople) and c.cha_in_todolist='1'



OPEN trasm
FETCH next from trasm into @ID,@idTrasm,@idpeopletx,@ruolo_in_uo,@dtainvio,@notegen,@idragione,@idproject,@idprofile,@note_sing,@scadenza,@cha_tipodest,@idtscorrglob
while(@@fetch_status=0)

BEGIN
begin

select @id_trasmutente = system_id
FROM DOCSADM.dpa_trasm_utente
WHERE id_trasm_singola = @ID
AND id_people = @idpeople;
if @@error <> 0 set @returnvalue = null
end
begin
DELETE FROM DOCSADM.dpa_trasm_utente WHERE
id_trasm_singola = @ID AND id_people = @idpeople;
if @@error <> 0 set @returnvalue = -1
end
begin

if(@id_trasmutente=null)
begin
DELETE FROM dpa_todolist WHERE id_trasm_utente = @id_trasmutente;
if @@error <> 0 set @returnvalue = -2
end
end
begin

INSERT INTO dpa_trasm_utente (id_people, id_trasm_singola, cha_vista,
cha_accettata, cha_rifiutata, cha_valida,cha_in_todolist)
VALUES (@idpeople, @ID, '0','0', '0', '1','1')
select @sysidtxut = @@identity
if @@error <> 0 set @returnvalue = -3
end
begin

set @idreg=null;
if(@idprofile is not null)
begin
set @idreg =  convert(int,DOCSADM.vardescribe (@idprofile, 'PROF_IDREG'))
end
INSERT INTO dpa_todolist(id_trasmissione, id_trasm_singola, id_trasm_utente,
dta_invio, id_people_mitt, id_ruolo_mitt,
id_people_dest, id_ragione_trasm, var_note_gen,
var_note_sing, dta_scadenza, id_profile, id_project,
id_ruolo_dest, id_registro, cha_tipo_trasm)
values
(@idTrasm, @ID,@sysidtxut,@dtainvio,@idpeopletx, @ruolo_in_uo,
@idpeople,@idragione, @notegen,
@note_sing, @scadenza, @idprofile, @idproject,
convert(int,DOCSADM.vardescribe (@idtscorrglob,'ID_GRUPPO')),@idreg,@cha_tipodest)
if @@error <> 0 set @returnvalue = -4
end

FETCH next from trasm into @ID,@idTrasm,@idpeopletx,@ruolo_in_uo,@dtainvio,@notegen,@idragione,@idproject,@idprofile,@note_sing,@scadenza,@cha_tipodest,@idtscorrglob

END
close trasm
deallocate trasm

set @returnvalue = 1


return @returnvalue
END
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[I_SMISTAMENTO_SMISTADOC]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[I_SMISTAMENTO_SMISTADOC]
GO

CREATE  PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC]
@IDPeopleMittente int,
@IDCorrGlobaleRuoloMittente int,
@IDGruppoMittente int,
@IDAmministrazioneMittente int,
@IDPeopleDestinatario int,
@IDCorrGlobaleDestinatario int,
@IDDocumento int,
@IDTrasmissione int,
@IDTrasmissioneUtenteMittente int,
@TrasmissioneConWorkflow bit,
@NoteGeneraliDocumento varchar(250),
@NoteIndividuali varchar(250),
@DataScadenza datetime,
@TipoDiritto nchar(1),
@Rights int,
@OriginalRights int,
@IDRagioneTrasm int
AS

DECLARE @resultValue INT
DECLARE @resultValueOut int
DECLARE @ReturnValue int
DECLARE @Identity int
DECLARE @IdentityTrasm int

BEGIN
INSERT INTO DPA_TRASMISSIONE
(
ID_RUOLO_IN_UO,
ID_PEOPLE,
CHA_TIPO_OGGETTO,
ID_PROFILE,
ID_PROJECT,
DTA_INVIO,
VAR_NOTE_GENERALI
)
VALUES
(
@IDCorrGlobaleRuoloMittente,
@IDPeopleMittente,
'D',
@IDDocumento,
NULL,
GETDATE(),
@NoteGeneraliDocumento
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=-2 -- errore inserimento nella dpa_trasmissione
END
ELSE

BEGIN

SET @Identity=scope_identity()
set @IdentityTrasm = @Identity

INSERT INTO DPA_TRASM_SINGOLA
(
ID_RAGIONE,
ID_TRASMISSIONE,
CHA_TIPO_DEST,
ID_CORR_GLOBALE,
VAR_NOTE_SING,
CHA_TIPO_TRASM,
DTA_SCADENZA,
ID_TRASM_UTENTE
)
VALUES
(
@IDRagioneTrasm,
@Identity,
'U',
@IDCorrGlobaleDestinatario,
@NoteIndividuali,
'S',
@DataScadenza,
NULL
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=-3  -- errore inserimento nella dpa_trasm_singola
END
ELSE
BEGIN

SET @Identity=scope_identity()

INSERT INTO DPA_TRASM_UTENTE
(
ID_TRASM_SINGOLA,
ID_PEOPLE,
DTA_VISTA,
DTA_ACCETTATA,
DTA_RIFIUTATA,
DTA_RISPOSTA,
CHA_VISTA,
CHA_ACCETTATA,
CHA_RIFIUTATA,
VAR_NOTE_ACC,
VAR_NOTE_RIF,
CHA_VALIDA,
ID_TRASM_RISP_SING
)
VALUES
(
@Identity,
@IDPeopleDestinatario,
NULL,
NULL,
NULL,
NULL,
'0',
'0',
'0',
NULL,
NULL,
'1',
NULL
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue = - 4  -- errore inserimento nella dpa_trasm_utente
END
ELSE
BEGIN

UPDATE DPA_TRASMISSIONE SET DTA_INVIO = GETDATE() WHERE SYSTEM_ID = @IdentityTrasm

DECLARE @AccessRights int
SET @AccessRights=
(
SELECT 	MAX(ACCESSRIGHTS)
FROM 	SECURITY
WHERE 	THING=@IDDocumento AND
PERSONORGROUP=@IDPeopleDestinatario
)

IF (NOT @AccessRights IS NULL)
BEGIN
IF (@AccessRights < @Rights)
UPDATE 	SECURITY
SET 	ACCESSRIGHTS=@Rights
WHERE 	THING=@IDDocumento AND
PERSONORGROUP=@IDPeopleDestinatario AND
ACCESSRIGHTS=@AccessRights
END
ELSE
BEGIN
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
@IDDocumento,
@IDPeopleDestinatario,
@Rights,
@IDGruppoMittente,
@TipoDiritto
)
END

IF (@TrasmissioneConWorkflow='1')
BEGIN
UPDATE 	DPA_TRASM_UTENTE
SET
dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
cha_vista  =  (case when dta_vista is null  then 1 else 0 end),
DTA_ACCETTATA=GETDATE(),
CHA_ACCETTATA='1',
VAR_NOTE_ACC='Documento accettato e smistato',
CHA_IN_TODOLIST = '0'
WHERE (SYSTEM_ID = @IDTrasmissioneUtenteMittente
OR
SYSTEM_ID = (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
AND TS.CHA_TIPO_DEST= 'U')
)
AND CHA_VALIDA='1'

UPDATE SECURITY SET ACCESSRIGHTS=@OriginalRights, CHA_TIPO_DIRITTO='T'
WHERE PERSONORGROUP IN (@IDPeopleMittente,@IDGruppoMittente)
AND ACCESSRIGHTS=20 AND THING=@IDDocumento
END
ELSE

BEGIN

EXEC DOCSADM.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione,  @resultValue out

SET @resultValueOut= @resultValue

IF(@resultValueOut=1)
BEGIN
SET @ReturnValue = -5;
RETURN
END
END

IF ((SELECT top 1 A.CHA_TIPO_TRASM
FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= @IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente))
ORDER BY CHA_TIPO_DEST
)='S' AND @TrasmissioneConWorkflow='1')

UPDATE 	DPA_TRASM_UTENTE
SET 	CHA_VALIDA = '0', cha_in_todolist = '0'
WHERE ID_TRASM_SINGOLA IN
(SELECT A.SYSTEM_ID
FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente)))
AND SYSTEM_ID NOT IN(@IDTrasmissioneUtenteMittente)

SET @ReturnValue=0
END
END
END
END

RETURN @ReturnValue
GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_FASC' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_PROTO_TIT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_PROTO_TIT]
	ADD ID_FASC int NULL
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='NUM_DOC_IN_FASC' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_PROTO_TIT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_PROTO_TIT]
	ADD NUM_DOC_IN_FASC int NULL
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[checkSecurityProprietario]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[checkSecurityProprietario] 
go

CREATE FUNCTION [@db_user].checkSecurityProprietario
(
	@thingParam INT,
	@idpeopleParam INT,
	@idgroupParam INT
)  
RETURNS INT AS  

BEGIN
  	DECLARE @retValue INT
   	DECLARE @cnt INT
	SET @cnt = 0

	SELECT @cnt = COUNT(*) 
	FROM security s, project b
	WHERE s.thing = @thingParam  AND s.personorgroup IN (@idgroupParam, @idpeopleParam) 
	and s.cha_tipo_diritto='P'
	and s.thing = b.system_id
	AND b.CHA_TIPO_FASCICOLO = 'P'
      
   	IF (@cnt > 0)  set @retValue = 1
   	ELSE	set @retValue = 0


   	RETURN @retValue

END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[checkSecurityUO]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[checkSecurityUO] 
go

CREATE FUNCTION [@db_user].checkSecurityUO 
(
	@thingParam INT,
 	@idUO INT
)  
RETURNS INT AS  
BEGIN 
	declare @cnt int
	declare @gruppoId INT
	declare @recordCount INT
	declare @retValue INT

	set @cnt = 0
	set @gruppoId = 0
	set @recordCount = 0
	set @retValue = 0

             declare cur CURSOR LOCAL 
	for select id_gruppo from dpa_corr_globali where id_uo = @idUo
   
             open cur
	fetch next from cur into @gruppoId
	
	while(@@fetch_status=0)
	
     		 SELECT @cnt = docsadm.checkSecurityProprietario(thingParam,0,gruppoId) FROM dual
     		 set @recordCount = @cnt + @recordCount

    	fetch next from cur into @gruppoId
	close cur
	deallocate cur
   
 
             IF (@recordCount > 0)
       		set @retValue = 1
             ELSE set @retValue = 0

             RETURN @retValue
   
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getContatoreFasc]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getContatoreFasc] 
go

CREATE FUNCTION [@db_user].[getContatoreFasc](@docNumber INT, @tipoContatore CHAR)
returns varchar(256)
as
begin

declare @valore_oggetto_db varchar(256);
declare @annoContatore varchar(256);
declare @codiceRegRf varchar(256);
declare @rtn varchar(256);

select 
@valore_oggetto_db = valore_oggetto_db,
@annoContatore = anno
from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where 
dpa_ass_templates_fasc.id_project = Cast(@docNumber as varchar(255))
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore;


if (@tipocontatore != 'T')
begin

select 
@codiceRegRf = dpa_el_registri.var_codice
from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where 
dpa_ass_templates_fasc.id_project = Cast(@docNumber as varchar(255))
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = @tipoContatore
and 
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
end;

if(@codiceRegRf is null or @codiceRegRf='')

begin

set @codiceRegRf=' '

end;

 

if(@annoContatore is null or @annoContatore='')

begin

set @annoContatore=' '

end;

 

if(@valore_oggetto_db is null or @valore_oggetto_db='')

begin

set @valore_oggetto_db=' '

end; 



return @codiceRegRf +'-'+ @annoContatore+ '-'+@valore_oggetto_db;

end




--
--
--return @codiceRegRf +'-'+ @annoContatore+ '-'+@valore_oggetto_db;
--
--
--end



GO

if not exists (SELECT * FROM syscolumns WHERE name='SPEDIZIONE_AUTO_DOC' and id in (SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA] ADD SPEDIZIONE_AUTO_DOC CHAR(1) NULL;
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='AVVISA_SPEDIZIONE_DOC' and id in (SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA] ADD AVVISA_SPEDIZIONE_DOC CHAR(1) NULL;
END;

GO

-- Vardescribe nuova
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
ALTER   FUNCTION [DOCSADM].[Vardescribe] (@sysid INT, @typeTable VARCHAR(1000)) RETURNS VARCHAR (8000)
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
IF (@typeTable = 'DTA_CREAZ')
BEGIN
SET  @outcome =(SELECT year (dta_creazione) FROM project  WHERE system_id = @sysid)
END
IF (@typeTable = 'NUM_FASC')
BEGIN
SET @outcome=(SELECT num_fascicolo FROM project  WHERE system_id = @sysid)
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
IF(@typeTable = 'SEGNATURA_CODFASC')
	BEGIN
	
	SET @outcome = (SELECT num_proto FROM PROFILE WHERE SYSTEM_ID = @sysid)

	IF (@outcome IS NULL)
		BEGIN
		SET @outcome = (SELECT docnumber FROM PROFILE WHERE system_id = @sysid)
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
SET @outcome =(
SELECT DOCSADM.getchaimg (docnumber)
FROM PROFILE
WHERE system_id = @sysid   )
END
IF (@typeTable = 'PROFILE_CHA_FIRMATO')
BEGIN
set @outcome=(SELECT CHA_FIRMATO FROM PROFILE WHERE system_id = @sysid)
END
return @outcome
end
GO;
-- Fine modifica Vardescribe



-- INIZIO FRIEND APPLICATION
IF NOT EXISTS (select * from sysobjects where name = 'DPA_FRIEND_APPLICATION' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_FRIEND_APPLICATION] (
  [SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [COD_APPLICAZIONE] VARCHAR(256) NULL,
  [COD_REGISTRO] VARCHAR(16) NULL,
  [ID_REGISTRO] INT NULL
)
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_PEOPLE_FACTORY' and id in (SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI] ADD ID_PEOPLE_FACTORY INT;
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_GRUPPO_FACTORY' and id in (SELECT id FROM sysobjects WHERE name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_EL_REGISTRI] ADD ID_GRUPPO_FACTORY INT;
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='FACTORY_USER' and id in (SELECT id FROM sysobjects WHERE name='PEOPLE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PEOPLE] ADD FACTORY_USER CHAR(1);
END;
GO
-- FINE FRIEND APPLICATION

--modifiche conservazione 

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_INSERT_DPA_SUPPORTO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_INSERT_DPA_SUPPORTO]
GO
CREATE  PROCEDURE [@db_user].[SP_INSERT_DPA_SUPPORTO]
@copia             int,
@collFisica        varchar(250),
@dataUltimaVer     datetime,
@dataEliminazione  datetime,
@esitoUltimaVer    int,
@numeroVer         int,
@dataProxVer       datetime,
@dataAppoMarca     datetime,
@dataScadMarca     datetime,
@marca             varchar(3000),
@idCons            int,
@tipoSupp          int,
@stato             char,
@note              varchar(500),
@query             varchar,
@idSupp            int,
@percVerifica      int,
@progressivoMarca  int, 
@result            int  OUTPUT,
@newId             int OUTPUT


as

BEGIN

declare @numSuppProd int
declare @numSuppTotali int

BEGIN
SET @numSuppProd=0
SET @numSuppTotali=0
SET @result=NULL


IF(@query='I')
BEGIN

INSERT INTO DPA_SUPPORTO (
COPIA,
DATA_PRODUZIONE,
VAR_COLLOCAZIONE_FISICA,
DATA_ULTIMA_VERIFICA,
DATA_ELIMINAZIONE,
ESITO_ULTIMA_VERIFICA,
VERIFICHE_EFFETTUATE,
DATA_PROX_VERIFICA,
DATA_APPO_MARCA,
DATA_SCADENZA_MARCA,
VAR_MARCA_TEMPORALE,
ID_CONSERVAZIONE,
ID_TIPO_SUPPORTO,
CHA_STATO,
VAR_NOTE,
PERC_VERIFICA,
NUM_MARCA
)
VALUES
(
@copia,
getdate(),
@collFisica,
@dataUltimaVer,
@dataEliminazione,
@esitoUltimaVer,
@numeroVer,
@dataProxVer,
@dataAppoMarca,
@dataScadMarca,
@marca,
@idCons,
@tipoSupp,
@stato,
@note,
@percVerifica,
@progressivoMarca
)

SET @newId = SCOPE_IDENTITY()

SELECT @numSuppProd=COUNT(*) FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=@idCons

SELECT @numSuppTotali=COUNT(*) FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=@idCons

IF(@numSuppProd=@numSuppTotali)
BEGIN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=@idCons

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=@idCons

SET @result=1
END
ELSE
BEGIN
SET @result=0
END
END

ELSE
BEGIN

UPDATE DPA_SUPPORTO SET DATA_PRODUZIONE=getdate(), VAR_COLLOCAZIONE_FISICA=@collFisica,
DATA_PROX_VERIFICA=@dataProxVer, CHA_STATO= @stato, VAR_NOTE=@note, DATA_ULTIMA_VERIFICA=getdate(), VERIFICHE_EFFETTUATE=@numeroVer, ESITO_ULTIMA_VERIFICA=@esitoUltimaVer,
PERC_VERIFICA=@percVerifica
WHERE SYSTEM_ID=@idSupp

SET @newId = @idSupp

SELECT @numSuppProd=COUNT(*)  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND
ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

SELECT @numSuppTotali=COUNT(*) FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

IF(@numSuppProd=@numSuppTotali)
BEGIN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=@idSupp)

SET @result=1
END
ELSE
BEGIN
SET @result=0
END

END
END
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_ITEMS_CONSERVAZIONE' and type = 'U')
begin

CREATE TABLE [db_user].[DPA_ITEMS_CONSERVAZIONE]
(
  [SYSTEM_ID] [int]  NOT NULL IDENTITY (1, 1),
	[ID_CONSERVAZIONE] [int] NULL ,
	[ID_PROFILE] [int] NULL ,
	[ID_PROJECT] [int] NULL ,
	[CHA_TIPO_DOC] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[VAR_OGGETTO] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ID_REGISTRO] [int] NULL ,
	[DATA_INS] [datetime] NULL ,
	[CHA_STATO] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[VAR_XML_METADATI] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[SIZE_ITEM] [int] NULL ,
	[COD_FASC] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DOCNUMBER] [int] NULL ,
	[VAR_TIPO_FILE] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[NUMERO_ALLEGATI] [int] NULL ,
	[CHA_TIPO_OGGETTO] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[CHA_ESITO] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[VAR_TIPO_ATTO] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
                  CONSTRAINT [INDX_ITEMS_CONS_PK] PRIMARY KEY  CLUSTERED 
	(
		[SYSTEM_ID]
	)  ON [PRIMARY] ,
	CONSTRAINT [DPA_ITEMS_CONSERVAZIONE_R01] FOREIGN KEY 
	(
		[ID_CONSERVAZIONE]
	) REFERENCES [DPA_AREA_CONSERVAZIONE] (
		[SYSTEM_ID]
	)
)ON [PRIMARY]

end

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[getInConservazione]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[getInConservazione] 
GO
CREATE FUNCTION [@db_user].getInConservazione
(
@IDPROFILE int,
@Idproject int,
@typeID char,
@idPeople int,
@idGruppo int
)

RETURNS int
AS

BEGIN

DECLARE   @risultato   int
DECLARE   @res_appo    int
DECLARE   @idRuoloInUo int

SET @risultato=0

BEGIN



SELECT @idRuoloInUo = DPA_CORR_GLOBALI.SYSTEM_ID
FROM DPA_CORR_GLOBALI
WHERE DPA_CORR_GLOBALI.ID_GRUPPO = @idGruppo


IF (@typeID = 'D' AND @Idproject IS NULL)
BEGIN

SELECT @risultato = count(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID)
FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE
WHERE
DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND
DPA_ITEMS_CONSERVAZIONE.ID_PROFILE = @IDPROFILE AND
DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N' AND
DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo AND
DPA_ITEMS_CONSERVAZIONE.ID_PROJECT IS NULL

END
ELSE
BEGIN
IF (@typeID = 'D' AND @Idproject IS NOT NULL)
BEGIN

SELECT @risultato = count(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID)
FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE
WHERE
DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND
DPA_ITEMS_CONSERVAZIONE.ID_PROFILE = @IDPROFILE AND
DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N' AND
DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo AND
DPA_ITEMS_CONSERVAZIONE.ID_PROJECT = @Idproject

END
END

IF (@typeID = 'F')
BEGIN

SELECT @risultato = count(DPA_ITEMS_CONSERVAZIONE.SYSTEM_ID)
FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE
WHERE
DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE = DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND
DPA_ITEMS_CONSERVAZIONE.ID_PROJECT = @Idproject AND
DPA_ITEMS_CONSERVAZIONE.CHA_STATO = 'N' AND
DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo

END

IF (@risultato > 0)
SET @risultato = 1
ELSE
SET @risultato = 0
END

RETURN @risultato
END

GO

--nuovi per deleghe
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SPsetDataVista]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop  PROCEDURE [@db_user].[SPsetDataVista]
go
CREATE   PROCEDURE [@db_user].[SPsetDataVista]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1),
@idDelegato int,
@resultValue int out
AS
DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)

BEGIN
	SET @resultValue = 0

	DECLARE cursorTrasmSingolaDocumento CURSOR FOR
	SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
	FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
	WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
	(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
	OR b.id_corr_globale =
	(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
	AND a.ID_PROFILE = @idOggetto and
	b.ID_RAGIONE = c.SYSTEM_ID

	DECLARE  cursorTrasmSingolaFascicolo CURSOR FOR
	SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
	FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
	WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
	(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
	OR b.id_corr_globale =
	(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
	AND a.ID_PROJECT = @idOggetto and
	b.ID_RAGIONE = c.SYSTEM_ID

	IF(@tipoOggetto='D')
	BEGIN
		OPEN cursorTrasmSingolaDocumento
		FETCH NEXT FROM cursorTrasmSingolaDocumento
		INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
		WHILE @@FETCH_STATUS = 0
	
		begin
			IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
			BEGIN
				BEGIN
					if (@idDelegato = 0)
					begin
						UPDATE DPA_TRASM_UTENTE
						SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
						DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
					end
					else
					--caso in cui si sta esercitando una delega
					begin
						UPDATE DPA_TRASM_UTENTE
						SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
						dpa_trasm_utente.cha_vista_delegato = '1',
						dpa_trasm_utente.id_people_delegato = @idDelegato,
						DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
					end


					update dpa_todolist
					set DTA_VISTA = getdate()
					where
					id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
					and id_profile = @idOggetto

					IF (@@ERROR <> 0)
					BEGIN
						SET @resultValue=1
						return @resultValue
					END
					
					IF (@chaTipoTrasm = 'S' AND @chaTipoDest = 'R')
					BEGIN
						if (@idDelegato = 0)
						begin
							UPDATE DPA_TRASM_UTENTE SET
							DPA_TRASM_UTENTE.CHA_VISTA = '1',
							DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
							WHERE
							DPA_TRASM_UTENTE.DTA_VISTA IS NULL
							AND id_trasm_singola = @sysTrasmSingola
							AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
						end
						else
						begin
							UPDATE DPA_TRASM_UTENTE SET
							DPA_TRASM_UTENTE.CHA_VISTA = '1',
							DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
							DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
							DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
							WHERE
							DPA_TRASM_UTENTE.DTA_VISTA IS NULL
							AND id_trasm_singola = @sysTrasmSingola
							AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
						end
						
					
						IF (@@ERROR <> 0)
						BEGIN
							SET @resultValue=1
							return @resultValue
						END
					END
				end
			END
		ELSE
			-- LA TRASMISSIONE PREVEDE WORKFLOW
			BEGIN
				if (@idDelegato = 0)
				begin
				UPDATE DPA_TRASM_UTENTE
				SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
				--DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
				DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
				WHERE
				DPA_TRASM_UTENTE.DTA_VISTA IS NULL
				AND id_trasm_singola = @sysTrasmSingola
				and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
				end
				else
				--caso di delega
				begin
				UPDATE DPA_TRASM_UTENTE
				SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
				dpa_trasm_utente.cha_vista_delegato = '1',
				dpa_trasm_utente.id_people_delegato = @idDelegato,
				--DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
				DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
				WHERE
				DPA_TRASM_UTENTE.DTA_VISTA IS NULL
				AND id_trasm_singola = @sysTrasmSingola
				and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
				end

				-- Rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
		             		UPDATE     dpa_trasm_utente
		             		SET        cha_in_todolist = '0'
		             		WHERE      id_trasm_singola = @sysTrasmSingola 
		                    	AND NOT  dpa_trasm_utente.dta_vista IS NULL
		                   	 AND (cha_accettata = '1' OR cha_rifiutata = '1')
		                   	 AND dpa_trasm_utente.id_people = @idPeople

				update dpa_todolist
				set DTA_VISTA = GETDATE()
				where
				id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
				and id_profile = @idOggetto;
			
				IF (@@ERROR <> 0)
				BEGIN
					SET @resultValue=1
					return @resultValue
				END
			END

		FETCH NEXT FROM cursorTrasmSingolaDocumento
		INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
		END
		CLOSE cursorTrasmSingolaDocumento
		DEALLOCATE cursorTrasmSingolaDocumento
	END

	IF(@tipoOggetto='F')
		begin
			OPEN cursorTrasmSingolaFascicolo
			FETCH NEXT FROM cursorTrasmSingolaFascicolo
			INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
			WHILE @@FETCH_STATUS = 0
	
			BEGIN
	
			IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
			BEGIN
				BEGIN
					if (@idDelegato = 0)
					begin
						UPDATE DPA_TRASM_UTENTE
						SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
						DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
					end
					else
					begin
						UPDATE DPA_TRASM_UTENTE
						SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
						DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
						DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
						DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
					end
					
					update dpa_todolist
					set DTA_VISTA = getdate()
					where
					id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
					and id_project = @idOggetto;
			
					IF (@@ERROR <> 0)
					BEGIN
						SET @resultValue=1
						return @resultValue
					END
				END
	
				IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R')
				BEGIN
					if (@idDelegato = 0)
					begin
						UPDATE DPA_TRASM_UTENTE SET
						DPA_TRASM_UTENTE.CHA_VISTA = '1',
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
					end
					else
					begin
						UPDATE DPA_TRASM_UTENTE SET
						DPA_TRASM_UTENTE.CHA_VISTA = '1',
						DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO = '1',
						DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
						DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
						WHERE
						DPA_TRASM_UTENTE.DTA_VISTA IS NULL
						AND id_trasm_singola = @sysTrasmSingola
						AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
					end
					IF (@@ERROR <> 0)
					BEGIN
						SET @resultValue=1
						return @resultValue
					END
				END
			END
		ELSE
			-- LA TRASMISSIONE PREVEDE WORKFLOW
			BEGIN
				if (@idDelegato = 0)
				begin
					UPDATE DPA_TRASM_UTENTE
					SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
					--DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
					DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
					WHERE
					DPA_TRASM_UTENTE.DTA_VISTA IS NULL
					AND id_trasm_singola = @sysTrasmSingola
					and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
				end
				else
				begin
					UPDATE DPA_TRASM_UTENTE
					SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
					 DPA_TRASM_UTENTE.CHA_VISTA_DELEGATO  = '1',
					DPA_TRASM_UTENTE.ID_PEOPLE_DELEGATO = @idDelegato,
					--DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
					DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
					WHERE
					DPA_TRASM_UTENTE.DTA_VISTA IS NULL
					AND id_trasm_singola = @sysTrasmSingola
					and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
				end

			                -- Rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
			                UPDATE     dpa_trasm_utente
			                SET        cha_in_todolist = '0'
			                WHERE      id_trasm_singola = @sysTrasmSingola
		                            AND NOT  dpa_trasm_utente.dta_vista IS NULL
		                            AND (cha_accettata = '1' OR cha_rifiutata = '1')
		                            AND dpa_trasm_utente.id_people = @idPeople

				update dpa_todolist
				set DTA_VISTA = getdate()
				where
				id_trasm_singola = @sysTrasmSingola and ID_PEOPLE_DEST = @idPeople
				and id_project = @idOggetto
			
				IF (@@ERROR <> 0)
				BEGIN
					SET @resultValue=1
					return @resultValue
				END
			END
	
		FETCH NEXT FROM cursorTrasmSingolaFascicolo
		INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest
		END
		
		CLOSE cursorTrasmSingolaFascicolo
		DEALLOCATE cursorTrasmSingolaFascicolo
		END
	END
RETURN @resultValue
GO

If not exists(select * from syscolumns where name='ID_PEOPLE_DELEGATO' and id in (select id from sysobjects where name='PROJECT' and xtype='U'))
BEGIN
ALTER TABLE PROJECT ADD ID_PEOPLE_DELEGATO INTEGER
END
GO

if not exists(select * from syscolumns where name='CHA_VISTA_DELEGATO' and id in 
(select id from sysobjects where name='DPA_TRASM_UTENTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASM_UTENTE ADD CHA_VISTA_DELEGATO Varchar(1) DEFAULT ('0') NOT NULL
END
GO

if not exists(select * from syscolumns where name='CHA_ACCETTATA_DELEGATO' and id in 
(select id from sysobjects where name='DPA_TRASM_UTENTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASM_UTENTE ADD CHA_ACCETTATA_DELEGATO Varchar(1) DEFAULT ('0') NOT NULL
END
GO

if not exists(select * from syscolumns where name='CHA_RIFIUTATA_DELEGATO' and id in 
(select id from sysobjects where name='DPA_TRASM_UTENTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASM_UTENTE ADD CHA_RIFIUTATA_DELEGATO Varchar(1) DEFAULT ('0') NOT NULL
END
GO

CREATE PROCEDURE [DOCSADM].[createProjectSP]
@idpeople int,
@description varchar (2000)  ,
@idPeopleDelegato int,
@projectId int out

AS

BEGIN

set @projectId = 0


INSERT INTO PROJECT
(

[DESCRIPTION],
ICONIZED,
ID_PEOPLE_DELEGATO
)
VALUES
(

@description,
'Y',
@idPeopleDelegato
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


--fine nuovi per deleghe

--nuovi per cha_firmato
if not exists(select * from syscolumns where name='CHA_FIRMATO' and id in (select id from sysobjects where name='components'  and xtype='U'))
BEGIN
ALTER TABLE components ADD CHA_FIRMATO VARCHAR(1) DEFAULT 0 NOT NULL
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[GetChaFirmato]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[GetChaFirmato] 
go
CREATE function [@db_user].[GetChaFirmato] (@docNum int)
returns varchar(16)
as
begin
declare @risultato varchar(16)
declare @vmaxidGenarica int

begin
set @vmaxidGenarica =0
SELECT @vmaxidGenarica =max(v1.version_id) from versions v1, components c where v1.docnumber=@docnum and v1.version_id=c.version_id
end

begin
SELECT @risultato=CHA_FIRMATO FROM COMPONENTS  WHERE DOCNUMBER = @docNum and version_id=@vmaxidGenarica
end
return @risultato
end
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_INS_EXT_IN_COMPONENTS]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_INS_EXT_IN_COMPONENTS]
GO


CREATE PROCEDURE [DOCSADM].[SP_INS_EXT_IN_COMPONENTS]

AS
DECLARE   @profile_docnumber   INT
DECLARE   @estensione          VARCHAR(30)
DECLARE   @id_version          INT
DECLARE   @id_last_version     INT
DECLARE   @estensioneappo      VARCHAR(30)
DECLARE   @indice              INT

BEGIN

DECLARE curr_components CURSOR FOR
SELECT docnumber, version_id,
SUBSTRING(PATH, LEN (PATH) - CHARINDEX('.',REVERSE (PATH)) + 2, LEN(PATH))
FROM components

OPEN curr_components
fetch next from curr_components into @profile_docnumber, @id_version, @estensione

WHILE @@FETCH_STATUS = 0

BEGIN

SELECT @id_last_version=MAX(VERSIONS.version_id)
FROM VERSIONS, components
WHERE VERSIONS.docnumber = @profile_docnumber
AND VERSIONS.version_id = components.version_id

IF(UPPER(LTRIM(RTRIM(@estensione)))='P7M')
BEGIN
SELECT  @estensioneappo=SUBSTRING (PATH,LEN (PATH) - CHARINDEX ('\',REVERSE (PATH)) + 2,LEN (PATH))
FROM components
WHERE version_id = @id_version AND docnumber = @profile_docnumber

SET @estensione=SUBSTRING (@estensioneappo,CHARINDEX ('.', @estensioneappo) + 1,LEN (@estensioneappo))

IF(UPPER(LTRIM(RTRIM(@estensione)))='P7M')
BEGIN
UPDATE components
SET ext = UPPER (@estensione)
WHERE version_id = @id_version
AND docnumber = @profile_docnumber
END
ELSE
BEGIN
SET @estensioneappo=SUBSTRING (@estensione,LEN (@estensione)- CHARINDEX('.',REVERSE (@estensione))+ 2, LEN(@estensione))

WHILE(UPPER(LTRIM(RTRIM(@estensioneappo)))='P7M')
BEGIN
SET @estensione=SUBSTRING (@estensione,0,LEN(@estensione)- CHARINDEX('.',REVERSE (@estensione))+1)

SET @estensioneappo=SUBSTRING (@estensione,LEN (@estensione)- CHARINDEX ('.',REVERSE (@estensione))+ 2, len(@estensione))
END

set @indice=CHARINDEX('.',@estensione)
if(@indice>0)
BEGIN
set @estensione=substring(@estensione, charindex('.',@estensione)+1, len(@estensione))
END

IF (LEN (@estensione) > 7)
begin
set @estensione = SUBSTRING (@estensione, 0, 7)
END

UPDATE components
SET ext = UPPER (@estensione)
WHERE  docnumber = @profile_docnumber and version_id = @id_version
END
IF (@id_version = @id_last_version)
BEGIN
UPDATE COMPONENTS
SET cha_firmato = '1'
WHERE docnumber = @profile_docnumber and version_id = @id_version
END
END
ELSE
BEGIN
IF (@id_version = @id_last_version)
BEGIN
UPDATE COMPONENTS
SET cha_firmato = '0'
WHERE docnumber = @profile_docnumber and version_id = @id_version
END

set @indice=CHARINDEX('.',@estensione)
if(@indice>0)
BEGIN
set @estensione=substring(@estensione, charindex('.',@estensione)+1, len(@estensione))
END

IF (LEN (@estensione) > 7)
BEGIN
SET @estensione = SUBSTRING (@estensione, 0, 7)
END

UPDATE components
SET ext = UPPER (@estensione)
WHERE  docnumber = @profile_docnumber and version_id = @id_version

END

fetch next from curr_components into @profile_docnumber, @id_version, @estensione
END

close curr_components
deallocate curr_components

END
GO
BEGIN
EXEC  @db_user.SP_INS_EXT_IN_COMPONENTS
END
go

CREATE   FUNCTION [DOCSADM].[Vardescribe] (@sysid INT, @typeTable VARCHAR(1000)) RETURNS VARCHAR (8000)
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
IF (@typeTable = 'DTA_CREAZ')
BEGIN
SET  @outcome =(SELECT year (dta_creazione) FROM project  WHERE system_id = @sysid)
END
IF (@typeTable = 'NUM_FASC')
BEGIN
SET @outcome=(SELECT num_fascicolo FROM project  WHERE system_id = @sysid)
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
IF(@typeTable = 'SEGNATURA_CODFASC')
	BEGIN
	
	SET @outcome = (SELECT num_proto FROM PROFILE WHERE SYSTEM_ID = @sysid)

	IF (@outcome IS NULL)
		BEGIN
		SET @outcome = (SELECT docnumber FROM PROFILE WHERE system_id = @sysid)
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
SET @outcome =(
SELECT DOCSADM.getchaimg (docnumber)
FROM PROFILE
WHERE system_id = @sysid   )
END
IF (@typeTable = 'PROFILE_CHA_FIRMATO')
BEGIN
set @outcome=(SELECT CHA_FIRMATO FROM PROFILE WHERE system_id = @sysid)
END
IF(@typeTable = 'COMPONENTS_CHA_FIRMATO')
BEGIN
SET @outcome =(
SELECT DOCSADM.getchafirmato (docnumber)
FROM PROFILE
WHERE system_id = @sysid   )
END
return @outcome
end
GO

create PROCEDURE [@db_user].[putFile]
@versionId int,
@filePath varchar(500),
@fileSize int,
@printThumb varchar(64),
@iscartaceo smallint,
@estensione varchar(7),
@isFirmato char
AS

declare @retValue int
declare @docNumber int

if exists(select version_id from versions where version_id = @versionId)
begin
set @docNumber = (select docnumber from versions where version_id = @versionId)
update 	versions
set 	subversion = 'A',
cartaceo = @iscartaceo
where 	version_id = @versionId

set @retValue = @@rowcount
end

if (@retValue > 0 and exists(select version_id from components where version_id = @versionId))
begin
if (@isFirmato='1')
begin
update 	components
set 	path = @filePath,
file_size = @fileSize,
var_impronta = @printThumb,
ext=@estensione,
cha_firmato = '1'
where 	version_id = @versionId
end
else
begin
update 	components
set 	path = @filePath,
file_size = @fileSize,
var_impronta = @printThumb,
ext=@estensione,
cha_firmato = '0'
where 	version_id = @versionId
end
set @retValue = @@rowcount
end

if (@retValue > 0 and exists(select docnumber from profile where docnumber = @docNumber))
begin

if(@isFirmato='1')
begin
update 	profile
set 	cha_img = '1', cha_firmato='1'
where	docnumber = @docNumber
end
else
begin
update 	profile
set 	cha_img = '1',cha_firmato='0'
where	docnumber = @docNumber
end
set @retValue = @@rowcount
end
return @retValue
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[dpa3_get_children]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[dpa3_get_children]
GO
CREATE PROCEDURE [@db_user].[dpa3_get_children]
@id_amm varchar(64),
@cha_tipo_ie varchar(2),
@var_cod_rubrica varchar(64),
@corr_types int = 7
as

declare @tipo varchar(2)
declare @system_id int
declare @id_gruppo int

select
@tipo = cha_tipo_urp, @system_id = system_id, @id_gruppo = id_gruppo
from
dpa_corr_globali
where
id_amm = @id_amm and
cha_tipo_ie=@cha_tipo_ie and
var_cod_rubrica = @var_cod_rubrica

if (@tipo = 'U')
begin
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
id_parent = @system_id and
dta_fine is null and
((@corr_types & 1) > 0)
union
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
cha_tipo_urp='R' and id_uo = @system_id and
dta_fine is null and
((@corr_types & 2) > 0)
end
else
begin
SELECT DISTINCT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
id_people in (
select people_system_id from peoplegroups where groups_system_id = @id_gruppo and dta_fine is null)
and dta_fine is null and CHA_TIPO_URP != 'L' and
((@corr_types & 4) > 0)
end

GO

--fine nuovi per cha_firmato




if exists(select * from syscolumns where name='file_extension' and id in 
		(select id from sysobjects where name='dpa_formati_documento' and xtype='U'))
begin		
	ALTER TABLE [@db_user].[dpa_formati_documento] alter column file_extension nvarchar(256)
end
GO

if exists(select * from syscolumns where name='ext' and id in 
		(select id from sysobjects where name='components' and xtype='U'))
begin		
	ALTER TABLE [@db_user].[components] alter column ext char(256)
end
GO



if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='FILTRO_FASC_EXCEL'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED)
VALUES ('FILTRO_FASC_EXCEL','Abilita la ricerca fascicolo filtrata con un elenco di valori su file excel nella pagina di ricerca fascicoli','N'); 
END
GO

CREATE  PROCEDURE [@db_user].[SP_COUNT_TODOLIST]
--parametri di input
@idPeople int,
@idGroup int
AS
--tabella temporanea
CREATE TABLE [@db_user].[#COUNT_TODOLIST]
(
[TOT_DOC] [int],
[TOT_DOC_NO_LETTI] [int],
[TOT_DOC_NO_ACCETTATI] [int],
[TOT_FASC] [int],
[TOT_FASC_NO_LETTI] [int],
[TOT_FASC_NO_ACCETTATI] [int],
[TOT_DOC_PREDISPOSTI] [int],
) ON [PRIMARY]
-- variabili locali
DECLARE @trasmdoctot int;
DECLARE @trasmdocnonletti int;
DECLARE @trasmdocnonaccettati int;
DECLARE @trasmfasctot int;
DECLARE @trasmfascnonletti int;
DECLARE @trasmfascnonaccettati int;
DECLARE @docpredisposti int;
BEGIN
--SETTING DELLE VARIABILI
SET @trasmdoctot = 0
SET @trasmdocnonletti = 0
SET @trasmdocnonaccettati = 0
SET @trasmfasctot = 0
SET @trasmfascnonletti = 0
SET @trasmfascnonaccettati = 0
SET @docpredisposti = 0
--END SETTING
--numero documenti presenti in todolist
SELECT @trasmdoctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);
--numero documenti non letti in todolist
SELECT @trasmdocnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);
--numero documenti non ancora accettati in todolist
SELECT @trasmdocnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND id_trasm_utente IN (SELECT system_id
FROM dpa_trasm_utente
WHERE cha_accettata = '0');
--numero fascicoli presenti in todolist
SELECT @trasmfasctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);
--numero fascicoli non letti in todolist
SELECT @trasmfascnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);
--numero fascicoli non ancora accettati in todolist
SELECT @trasmfascnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND id_trasm_utente IN (SELECT system_id
FROM dpa_trasm_utente
WHERE cha_accettata = '0');
--numero documenti predisposti
SELECT @docpredisposti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE  (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND id_profile IN (SELECT system_id
FROM PROFILE
WHERE cha_da_proto = '1')
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND id_trasm_utente IN (SELECT system_id
FROM dpa_trasm_utente
WHERE cha_accettata = '0');
--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #COUNT_TODOLIST
(TOT_DOC, TOT_DOC_NO_LETTI, TOT_DOC_NO_ACCETTATI, TOT_FASC, TOT_FASC_NO_LETTI,
TOT_FASC_NO_ACCETTATI, TOT_DOC_PREDISPOSTI)
values
(@trasmdoctot, @trasmdocnonletti, @trasmdocnonaccettati, @trasmfasctot, @trasmfascnonletti,
@trasmfascnonaccettati, @docpredisposti)
END
-- return dei risultati
SELECT * FROM #COUNT_TODOLIST
GO
GO






CREATE    PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC]
@IDPeopleMittente int,
@IDCorrGlobaleRuoloMittente int,
@IDGruppoMittente int,
@IDAmministrazioneMittente int,
@IDPeopleDestinatario int,
@IDCorrGlobaleDestinatario int,
@IDDocumento int,
@IDTrasmissione int,
@IDTrasmissioneUtenteMittente int,
@TrasmissioneConWorkflow bit,
@NoteGeneraliDocumento varchar(250),
@NoteIndividuali varchar(250),
@DataScadenza datetime,
@TipoDiritto nchar(1),
@Rights int,
@OriginalRights int,
@IDRagioneTrasm int,
@idpeopledelegato int
AS

DECLARE @resultValue INT
DECLARE @resultValueOut int
DECLARE @ReturnValue int
DECLARE @Identity int
DECLARE @IdentityTrasm int
DECLARE @isAccettata nvarchar(1)
DECLARE @isAccettataDelegato nvarchar(1) 
DECLARE @isVista nvarchar(1) 
DECLARE @isVistaDelegato nvarchar(1) 

BEGIN

	set @isAccettata = '0'
	set @isAccettataDelegato = '0'
	set @isVista = '0'
	set @isVistaDelegato = '0'
	
	INSERT INTO DPA_TRASMISSIONE
	(
		ID_RUOLO_IN_UO,
		ID_PEOPLE,
		CHA_TIPO_OGGETTO,
		ID_PROFILE,
		ID_PROJECT,
		DTA_INVIO,
		VAR_NOTE_GENERALI	
	)
	VALUES
	(
		@IDCorrGlobaleRuoloMittente,
		@IDPeopleMittente,
		'D',
		@IDDocumento,
		NULL,
		GETDATE(),
		@NoteGeneraliDocumento
	)

	IF (@@ROWCOUNT = 0)
		BEGIN
			SET @ReturnValue=-2 -- errore inserimento nella dpa_trasmissione
		END
	ELSE
		BEGIN
			-- Inserimento in tabella DPA_TRASM_SINGOLA
			SET @Identity=scope_identity()
			set @IdentityTrasm = @Identity
			
			INSERT INTO DPA_TRASM_SINGOLA
			(
				ID_RAGIONE,
				ID_TRASMISSIONE,
				CHA_TIPO_DEST,
				ID_CORR_GLOBALE,
				VAR_NOTE_SING,
				CHA_TIPO_TRASM,
				DTA_SCADENZA,
				ID_TRASM_UTENTE
			)
			VALUES
			(
				@IDRagioneTrasm,
				@Identity,
				'U',
				@IDCorrGlobaleDestinatario,
				@NoteIndividuali,
				'S',
				@DataScadenza,
				NULL
			)

			IF (@@ROWCOUNT = 0)
				BEGIN
					SET @ReturnValue=-3  -- errore inserimento nella dpa_trasm_singola
				END
			ELSE
				BEGIN
					-- Inserimento in tabella DPA_TRASM_UTENTE
					SET @Identity=scope_identity()

					INSERT INTO DPA_TRASM_UTENTE
					(
						ID_TRASM_SINGOLA,
						ID_PEOPLE,
						DTA_VISTA,
						DTA_ACCETTATA,
						DTA_RIFIUTATA,
						DTA_RISPOSTA,
						CHA_VISTA,
						CHA_ACCETTATA,
						CHA_RIFIUTATA,
						VAR_NOTE_ACC,
						VAR_NOTE_RIF,
						CHA_VALIDA,
						ID_TRASM_RISP_SING
					)
					VALUES
					(
						@Identity,
						@IDPeopleDestinatario,
						NULL,
						NULL,
						NULL,
						NULL,
						'0',
						'0',
						'0',
						NULL,
						NULL,
						'1',
						NULL
					)

					IF (@@ROWCOUNT = 0)
						BEGIN
							SET @ReturnValue = - 4  -- errore inserimento nella dpa_trasm_utente
						END
					ELSE
						BEGIN

							UPDATE DPA_TRASMISSIONE 
								SET DTA_INVIO = GETDATE() 
							WHERE SYSTEM_ID = @IdentityTrasm

							DECLARE @AccessRights int

							SET @AccessRights=
							(
								SELECT 	MAX(ACCESSRIGHTS)
								FROM 	SECURITY
								WHERE 	THING=@IDDocumento AND
								PERSONORGROUP=@IDPeopleDestinatario
							)

							IF (NOT @AccessRights IS NULL)
								BEGIN
									IF (@AccessRights < @Rights)
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS=@Rights
										WHERE 	THING=@IDDocumento AND
											PERSONORGROUP=@IDPeopleDestinatario AND
											ACCESSRIGHTS=@AccessRights
								END
							ELSE		
								BEGIN
									-- inserimento Rights
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
										@IDDocumento,
										@IDPeopleDestinatario,
										@Rights,
										@IDGruppoMittente,
										@TipoDiritto
									)
								END
							
							IF (@TrasmissioneConWorkflow='1')
								BEGIN
									-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
									SET @isAccettata = 
										(SELECT cha_accettata
									            FROM dpa_trasm_utente 
									            WHERE system_id = @idtrasmissioneutentemittente)
								        SET @isVista = 
										(SELECT cha_vista
									            FROM dpa_trasm_utente 
									            WHERE system_id = @idtrasmissioneutentemittente)
					
									IF (@idPeopleDelegato > 0)
										BEGIN
											-- Impostazione dei flag per la gestione del delegato
											SET @isVistaDelegato = '1'
									                SET @isAccettataDelegato = '1'
										END

									IF (@isAccettata = '1')
										BEGIN
										-- caso in cui la trasmissione risulta già accettata
											IF (@isVista = '0')
												BEGIN
									                            -- l'oggetto trasmesso non risulta ancora visto,
									                            -- pertanto vengono impostati i dati di visualizzazione
									                            -- e viene rimossa la trasmissione dalla todolist
													UPDATE dpa_trasm_utente
													SET	dta_vista = (CASE WHEN dta_vista IS NULL THEN GETDATE() ELSE dta_vista END),
														cha_vista = (case when dta_vista is null  then 1 else 0 end),
									                                    	cha_vista_delegato = @isVistaDelegato,
									                                   	 cha_in_todolist = '0',
									                                    	cha_valida = '0'
									                                 WHERE (   system_id = @idtrasmissioneutentemittente
									                                     	OR system_id =
									                                           (SELECT tu.system_id
									                                              FROM dpa_trasm_utente tu,
									                                                   dpa_trasmissione tx,
									                                                   dpa_trasm_singola ts
									                                             WHERE tu.id_people = @idpeoplemittente
									                                               AND tx.system_id = ts.id_trasmissione
									                                               AND tx.system_id = @idtrasmissione
									                                               AND ts.system_id = tu.id_trasm_singola
									                                               AND ts.cha_tipo_dest = 'U')
									                                    )
												END
											ELSE
												BEGIN
													-- l'oggetto trasmesso visto,
									                            	-- pertanto la trasmissione viene solo rimossa dalla todolist

													UPDATE dpa_trasm_utente
										                                SET cha_in_todolist = '0',
										                                    cha_valida = '0'
										                            WHERE (   system_id = @idtrasmissioneutentemittente
										                                     OR system_id =
										                                           (SELECT tu.system_id
										                                              FROM dpa_trasm_utente tu,
										                                                   dpa_trasmissione tx,
										                                                   dpa_trasm_singola ts
										                                             WHERE tu.id_people = @idpeoplemittente
										                                               AND tx.system_id = ts.id_trasmissione
										                                               AND tx.system_id = @idtrasmissione
										                                               AND ts.system_id = tu.id_trasm_singola
										                                               AND ts.cha_tipo_dest = 'U')
										                                    )
					
												END
										END
									ELSE
										BEGIN

									                -- la trasmissione ancora non risulta accettata, pertanto:
									                -- 1) viene accettata implicitamente, 
									                -- 2) l'oggetto trasmesso impostato come visto,
									                -- 3) la trasmissione rimossa la trasmissione da todolist
											UPDATE dpa_trasm_utente
									                    SET dta_vista = (CASE WHEN dta_vista IS NULL THEN GETDATE() ELSE dta_vista END),
												cha_vista = (case when dta_vista is null  then 1 else 0 end),
								                                cha_vista_delegato = @isVistaDelegato,
									                        dta_accettata = GETDATE(),
									                        cha_accettata = '1',
									                        cha_accettata_delegato = @isAccettataDelegato,
									                        var_note_acc = 'Documento accettato e smistato',                        
									                        cha_in_todolist = '0',
									                        cha_valida = '0'
									                  WHERE (   system_id = @idtrasmissioneutentemittente
									                         OR system_id =
									                               (SELECT tu.system_id
									                                  FROM dpa_trasm_utente tu,
									                                       dpa_trasmissione tx,
									                                       dpa_trasm_singola ts
									                                 WHERE tu.id_people = @idpeoplemittente
									                                   AND tx.system_id = ts.id_trasmissione
									                                   AND tx.system_id = @idtrasmissione
									                                   AND ts.system_id = tu.id_trasm_singola
									                                   AND ts.cha_tipo_dest = 'U')
									                        ) 
									                        AND cha_valida = '1'
										END

									-- update security se diritti  trasmssione in accettazione =20
								        UPDATE security
								        SET     accessrights = @originalrights,
								                cha_tipo_diritto = 'T'
								        WHERE thing=@IDDocumento and   personorgroup IN (@idpeoplemittente, @idgruppomittente)
								                AND accessrights = 20

								END
							ELSE
								
								BEGIN
								
									EXEC DOCSADM.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione, @idPeopleDelegato,  @resultValue out
									
									SET @resultValueOut= @resultValue
									
									IF(@resultValueOut=1)
										BEGIN
											SET @ReturnValue = -4;
											RETURN
										END
								END

							-- se la trasmissione era destinata a SINGOLO, 
							-- allora toglie la validità della trasmissione 
							-- a tutti gli altri utenti del ruolo (tranne a quella del mittente)
							IF ((SELECT top 1 A.CHA_TIPO_TRASM
								FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
								WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
								AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
								DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= @IDPeopleMittente AND
								TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
								and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente))
								ORDER BY CHA_TIPO_DEST
								)='S' AND @TrasmissioneConWorkflow='1')
									-- se la trasmissione era destinata a SINGOLO, allora toglie la validità della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente)
									UPDATE 	DPA_TRASM_UTENTE
									SET 	CHA_VALIDA = '0', cha_in_todolist = '0'
									WHERE 	ID_TRASM_SINGOLA IN
										(SELECT A.SYSTEM_ID
										FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
										WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
										AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
										DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
										TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
										and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente)))
										AND SYSTEM_ID NOT IN(@IDTrasmissioneUtenteMittente)

							SET @ReturnValue=0
						END
					END
				END
		END

RETURN @ReturnValue
GO


CREATE     PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC_U]
@IDPeopleMittente int,
@IDCorrGlobaleRuoloMittente int,
@IDGruppoMittente int,
@IDAmministrazioneMittente int,
@IDCorrGlobaleDestinatario int,
@IDDocumento int,
@IDTrasmissione int,
@IDTrasmissioneUtenteMittente int,
@TrasmissioneConWorkflow bit,
@NoteGeneraliDocumento varchar(250),
@NoteIndividuali varchar(250),
@DataScadenza datetime,
@TipoTrasmissione nchar(1),
@TipoDiritto nchar(1),
@Rights int,
@OriginalRights int,
@IDRagioneTrasm int,
@idpeopledelegato int

AS

DECLARE @ReturnValue int
DECLARE @resultValue int
DECLARE @resultValueOut int
DECLARE @Identity int
DECLARE @IDGroups int
DECLARE @AccessRights int
DECLARE @isAccettata nvarchar(1)
DECLARE @isAccettataDelegato nvarchar(1) 
DECLARE @isVista nvarchar(1) 
DECLARE @isVistaDelegato nvarchar(1) 

BEGIN
	set @isAccettata = '0'
	set @isAccettataDelegato = '0'
	set @isVista = '0'
	set @isVistaDelegato = '0'

	INSERT INTO DPA_TRASMISSIONE
	(
	ID_RUOLO_IN_UO,
	ID_PEOPLE,
	CHA_TIPO_OGGETTO,
	ID_PROFILE,
	ID_PROJECT,
	DTA_INVIO,
	VAR_NOTE_GENERALI
	)
	VALUES
	(
	@IDCorrGlobaleRuoloMittente,
	@IDPeopleMittente,
	'D',
	@IDDocumento,
	NULL,
	GETDATE(),
	@NoteGeneraliDocumento
	)

	IF (@@ROWCOUNT = 0)
		BEGIN
			SET @ReturnValue=-2
		END
	ELSE
		BEGIN
			SET @Identity=scope_identity()
	
			INSERT INTO DPA_TRASM_SINGOLA
			(
			ID_RAGIONE,
			ID_TRASMISSIONE,
			CHA_TIPO_DEST,
			ID_CORR_GLOBALE,
			VAR_NOTE_SING,
			CHA_TIPO_TRASM,
			DTA_SCADENZA,
			ID_TRASM_UTENTE
			)
			VALUES
			(
			@IDRagioneTrasm,
			@Identity,
			'R',
			@IDCorrGlobaleDestinatario,
			@NoteIndividuali,
			@TipoTrasmissione,
			@DataScadenza,
			NULL
			)

			SET @Returnvalue = scope_identity()

			IF (@@ROWCOUNT = 0)
				BEGIN
					SET @ReturnValue=-3
				END
			ELSE
				BEGIN
					SET @IDGroups =
					(
					SELECT A.ID_GRUPPO
					FROM DPA_CORR_GLOBALI A
					WHERE A.SYSTEM_ID = @IDCorrGlobaleDestinatario
					)

					SET @AccessRights=
					(
					SELECT 	MAX(ACCESSRIGHTS)
					FROM 	SECURITY
					WHERE 	THING = @IDDocumento
					AND
					PERSONORGROUP = @IDGroups
					)

					IF (NOT @AccessRights IS NULL)
						BEGIN
							IF (@AccessRights < @Rights)
								UPDATE 	SECURITY
								SET 	ACCESSRIGHTS=@Rights
								WHERE 	THING = @IDDocumento
								AND
								PERSONORGROUP = @IDGroups
								AND
								ACCESSRIGHTS=@AccessRights
						END
					ELSE
						BEGIN
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
							@IDDocumento,
							@IDGroups,
							@Rights,
							@IDGruppoMittente,
							@TipoDiritto
							)
						END

					IF (@TrasmissioneConWorkflow='1')
						BEGIN
							-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
							SET @isAccettata = (SELECT cha_accettata 
										FROM dpa_trasm_utente 
										WHERE system_id = @IDTrasmissioneUtenteMittente)
							
						        SET @isVista = (SELECT cha_vista
								        FROM dpa_trasm_utente 
									where system_id = @IDTrasmissioneUtenteMittente) 
							
 					            	if (@idPeopleDelegato > 0)
						                begin
						                    -- Impostazione dei flag per la gestione del delegato
						                    set @isVistaDelegato = '1'
						                    set @isAccettataDelegato = '1'
						                end

							if (@isAccettata = '1')
								begin
									-- caso in cui la trasmissione risulta già accettata
									if (@isVista = '0')
										begin
											-- l'oggetto trasmesso non risulta ancora visto,
								                        -- pertanto vengono impostati i dati di visualizzazione
								                        -- e viene rimossa la trasmissione dalla todolist
							                             UPDATE 	dpa_trasm_utente
							                                SET 	dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
												cha_vista  =  (case when dta_vista is null  then 1 else 0 end),
												cha_vista_delegato = @isVistaDelegato,
							                                    	cha_in_todolist = '0',
							                                    cha_valida = '0'
							                              WHERE (   system_id = @IDTrasmissioneUtenteMittente
							                                     OR system_id =
							                                           (SELECT tu.system_id
							                                              FROM dpa_trasm_utente tu,
							                                                   dpa_trasmissione tx,
							                                                   dpa_trasm_singola ts
							                                             WHERE tu.id_people = @idpeoplemittente
							                                               AND tx.system_id = ts.id_trasmissione
							                                               AND tx.system_id = @idtrasmissione
							                                               AND ts.system_id = tu.id_trasm_singola
							                                               AND ts.cha_tipo_dest = 'U')
							                                    )
										end
									else
										begin	
							                            -- l'oggetto trasmesso risulta visto,
							                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
										    UPDATE dpa_trasm_utente
							                                SET cha_in_todolist = '0',
							                                    cha_valida = '0'
							                            WHERE (   system_id = @IDTrasmissioneUtenteMittente
							                                     OR system_id =
							                                           (SELECT tu.system_id
							                                              FROM dpa_trasm_utente tu,
							                                                   dpa_trasmissione tx,
							                                                   dpa_trasm_singola ts
							                                             WHERE tu.id_people = @idpeoplemittente
							                                               AND tx.system_id = ts.id_trasmissione
							                                               AND tx.system_id = @idtrasmissione
							                                               AND ts.system_id = tu.id_trasm_singola
							                                               AND ts.cha_tipo_dest = 'U')
							                                    )
										end
								end
							else
								begin
									
									-- la trasmissione ancora non risulta accettata, pertanto:
							                -- 1) viene accettata implicitamente, 
							                -- 2) l'oggetto trasmesso impostato come visto,
							                -- 3) la trasmissione rimossa la trasmissione da todolist
									UPDATE dpa_trasm_utente
					                                SET 	dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
										cha_vista = (case when dta_vista is null  then 1 else 0 end),
										cha_vista_delegato = @isVistaDelegato,
							                        dta_accettata = getdate(),
							                        cha_accettata = '1',
							                        cha_accettata_delegato = @isAccettataDelegato,
							                        var_note_acc = 'Documento accettato e smistato',
							                        cha_in_todolist = '0',
							                        cha_valida = '0'
							                  WHERE (   system_id = @IDTrasmissioneUtenteMittente
							                         OR system_id =
							                               (SELECT tu.system_id
							                                  FROM dpa_trasm_utente tu,
							                                       dpa_trasmissione tx,
							                                       dpa_trasm_singola ts
							                                 WHERE tu.id_people = @idpeoplemittente
							                                   AND tx.system_id = ts.id_trasmissione
							                                   AND tx.system_id = @idtrasmissione
							                                   AND ts.system_id = tu.id_trasm_singola
							                                   AND ts.cha_tipo_dest = 'U')
							                        ) 
							                        AND cha_valida = '1'
								end
							
					            	--update security se diritti  trasmssione in accettazione =20
					            	UPDATE 	security 
					            	SET     accessrights = @originalrights,
					            	        cha_tipo_diritto = 'T'
					           	WHERE thing=@IDDocumento and personorgroup IN (@idpeoplemittente, @idgruppomittente) 
								AND accessrights = 20
						END
					ELSE
						BEGIN
							EXEC DOCSADM.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione, @idPeopleDelegato,  @resultValue out

							SET @resultValueOut = @resultValue

							IF(@resultValueOut=1)
								BEGIN
									SET @ReturnValue = -4;
									RETURN
								END
						END

					/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
					IF (  
						(SELECT 	top 1 
								A.CHA_TIPO_TRASM
						 FROM 		DPA_TRASM_SINGOLA A, 
								DPA_TRASM_UTENTE B
						 WHERE 		A.SYSTEM_ID=B.ID_TRASM_SINGOLA
								AND B.SYSTEM_ID IN 
								(
									SELECT 	TU.SYSTEM_ID 
									FROM 	DPA_TRASM_UTENTE TU,
										DPA_TRASMISSIONE TX,
										DPA_TRASM_SINGOLA TS 
									WHERE 	TU.ID_PEOPLE= @IDPeopleMittente AND
										TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND 
										TX.SYSTEM_ID=@IDTrasmissione AND 
										TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA and 
										TS.SYSTEM_ID = 
										(
											SELECT ID_TRASM_SINGOLA 
											FROM DPA_TRASM_UTENTE 
											WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente
										)
								)
						ORDER BY CHA_TIPO_DEST) = 'S' AND 
						@TrasmissioneConWorkflow='1')

					BEGIN
						UPDATE 		DPA_TRASM_UTENTE
						SET 		CHA_VALIDA = '0', 
								cha_in_todolist = '0'
						WHERE 		ID_TRASM_SINGOLA IN
								(SELECT A.SYSTEM_ID
								FROM DPA_TRASM_SINGOLA A, 
									DPA_TRASM_UTENTE B
								WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
									AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
									DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
									TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
									and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente)))
									AND SYSTEM_ID NOT IN( @IDTrasmissioneUtenteMittente)
					END
				END
			END
		END

RETURN @ReturnValue
GO

CREATE  PROCEDURE [@db_user].[SP_COUNT_TODOLIST]
--parametri di input
@idPeople int,
@idGroup int
AS
--tabella temporanea
CREATE TABLE [DOCSADM].[#COUNT_TODOLIST]
(
[TOT_DOC] [int],
[TOT_DOC_NO_LETTI] [int],
[TOT_DOC_NO_ACCETTATI] [int],
[TOT_FASC] [int],
[TOT_FASC_NO_LETTI] [int],
[TOT_FASC_NO_ACCETTATI] [int],
[TOT_DOC_PREDISPOSTI] [int],
) ON [PRIMARY]

-- variabili locali
DECLARE @trasmdoctot int;
DECLARE @trasmdocnonletti int;
DECLARE @trasmdocnonaccettati int;
DECLARE @trasmfasctot int;
DECLARE @trasmfascnonletti int;
DECLARE @trasmfascnonaccettati int;
DECLARE @docpredisposti int;
BEGIN

--SETTING DELLE VARIABILI
SET @trasmdoctot = 0
SET @trasmdocnonletti = 0
SET @trasmdocnonaccettati = 0
SET @trasmfasctot = 0
SET @trasmfascnonletti = 0
SET @trasmfascnonaccettati = 0
SET @docpredisposti = 0
--END SETTING

--numero documenti presenti in todolist
SELECT @trasmdoctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero documenti non letti in todolist
SELECT @trasmdocnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti non ancora accettati in todolist
SELECT @trasmdocnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                     AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli presenti in todolist
SELECT @trasmfasctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero fascicoli non letti in todolist
SELECT @trasmfascnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli non ancora accettati in todolist
SELECT @trasmfascnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                    AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti predisposti
SELECT @docpredisposti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE  (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo =@idGroup))
                OR id_registro = 0
               )
          )
AND id_profile IN (SELECT system_id
FROM PROFILE
WHERE cha_da_proto = '1')
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #COUNT_TODOLIST
(TOT_DOC, TOT_DOC_NO_LETTI, TOT_DOC_NO_ACCETTATI, TOT_FASC, TOT_FASC_NO_LETTI,
TOT_FASC_NO_ACCETTATI, TOT_DOC_PREDISPOSTI)
values
(@trasmdoctot, @trasmdocnonletti, @trasmdocnonaccettati, @trasmfasctot, @trasmfascnonletti,
@trasmfascnonaccettati, @docpredisposti)
END
-- return dei risultati
SELECT * FROM #COUNT_TODOLIST
GO

/* Definizione funzione per espoertazione documenti in fascicolo */
IF NOT EXISTS (SELECT * FROM @db_user.DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE ='EXP_DOC_MASSIVA')
    BEGIN
        insert into DPA_ANAGRAFICA_FUNZIONI VALUES('EXP_DOC_MASSIVA', 'Abilita il pulsante esporta documenti in fascicolo nella pagina di dettaglio del fascicolo', '', 'N');
    END
GO



/*INIZIO VISIBILITA CAMPI PROFILAZIONE DINAMICA*/
--Creazione Tabella DPA_A_R_OGG_CUSTOM_DOC
IF NOT EXISTS (select * from sysobjects where name = 'DPA_A_R_OGG_CUSTOM_DOC' and type = 'U')
begin
CREATE TABLE [docsadm].[DPA_A_R_OGG_CUSTOM_DOC] (
  [SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_TEMPLATE] INT,
  [ID_OGGETTO_CUSTOM] INT,
  [ID_RUOLO] INT,
  [INS_MOD] INT,
  [VIS] INT
)
end
GO

if exists (select * from dbo.sysindexes where name = N'INDX_DPA_A_R_DOC1' and id = object_id(N'[@db_user].[DPA_A_R_OGG_CUSTOM_DOC]'))
drop index [@db_user].[DPA_A_R_OGG_CUSTOM_DOC].[INDX_DPA_A_R_DOC1]
GO
CREATE INDEX   [INDX_DPA_A_R_DOC1] ON  [@db_user].[DPA_A_R_OGG_CUSTOM_DOC] ([ID_TEMPLATE],[ID_OGGETTO_CUSTOM],[ID_RUOLO])  	
GO

--Creazione Tabella DPA_A_R_OGG_CUSTOM_FASC
IF NOT EXISTS (select * from sysobjects where name = 'DPA_A_R_OGG_CUSTOM_FASC' and type = 'U')
begin
CREATE TABLE [docsadm].[DPA_A_R_OGG_CUSTOM_FASC] (
  [SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [ID_TEMPLATE] INT,
  [ID_OGGETTO_CUSTOM] INT,
  [ID_RUOLO] INT,
  [INS_MOD] INT,
  [VIS] INT
)
end
GO

if exists (select * from dbo.sysindexes where name = N'INDX_DPA_A_R_FASC1' and id = object_id(N'[@db_user].[DPA_A_R_OGG_CUSTOM_FASC]'))
drop index [@db_user].[DPA_A_R_OGG_CUSTOM_FASC].[INDX_DPA_A_R_FASC1]
GO
CREATE INDEX   [INDX_DPA_A_R_FASC1] ON  [@db_user].[DPA_A_R_OGG_CUSTOM_FASC] ([ID_TEMPLATE],[ID_OGGETTO_CUSTOM],[ID_RUOLO])	
GO

--Popolazione DPA_A_R_OGG_CUSTOM_DOC - DPA_A_R_OGG_CUSTOM_FASC con la visibilità dei contatori con CONTA_DOPO attivo
BEGIN
DECLARE @par_id_Oggetto_Custom int;
DECLARE @par_id_Ruolo int;
DECLARE @par_inserimento int;
DECLARE @par_id_template int;
DECLARE c_idOggCustom CURSOR FOR SELECT id_Oggetto_Custom,id_Ruolo,inserimento FROM DPA_ASS_RUOLO_OGG_CUSTOM;
OPEN c_idOggCustom
FETCH next from c_idOggCustom into @par_id_Oggetto_Custom, @par_id_Ruolo, @par_inserimento
while(@@fetch_status=0) 
	BEGIN
		--Verifico se l'oggetto custom è di una tipologia fascicolo
		if( (select count(*) from dpa_oggetti_custom where system_id = @par_id_Oggetto_Custom and conta_dopo = 1) = 1) 
		begin
			if( (select count(*) from DPA_A_R_OGG_CUSTOM_DOC where id_oggetto_custom = @par_id_Oggetto_Custom and id_ruolo = @par_id_Ruolo) = 0)
			begin
				set @par_id_template = (select id_template from dpa_associazione_templates where id_oggetto = @par_id_Oggetto_Custom and doc_number = '');
				insert into DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, @par_inserimento, 1);
			end
		end
		
		--Verifico se l'oggetto custom è di una tipologia documento
		if( (select count(*) from dpa_oggetti_custom_fasc where system_id = @par_id_Oggetto_Custom and conta_dopo = 1) = 1) 
		begin
			if( (select count(*) from DPA_A_R_OGG_CUSTOM_FASC where id_oggetto_custom = @par_id_Oggetto_Custom and id_ruolo = @par_id_Ruolo) = 0)
			begin
				set @par_id_template = (select id_template from dpa_ass_templates_fasc where id_oggetto = @par_id_Oggetto_Custom and id_project = '');
				insert into DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, @par_inserimento, 1);
			end
		end
	FETCH next from c_idOggCustom into @par_id_Oggetto_Custom, @par_id_Ruolo, @par_inserimento
	END
DEALLOCATE c_idOggCustom
END
GO

--Popolazione DPA_A_R_OGG_CUSTOM_DOC con la visibilità dei campi profilati escluso i contatori con CONTA_DOPO attivo
BEGIN
DECLARE @par_id_template int;
DECLARE @par_id_oggetto_Custom int;
DECLARE @par_id_ruolo int;
DECLARE @par_id_amm int;
--DECLARE c_idTemplate CURSOR FOR SELECT system_id from dpa_tipo_atto;
DECLARE c_idTemplate CURSOR FOR SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
OPEN c_idTemplate
--FETCH next from c_idTemplate into @par_id_template
FETCH next from c_idTemplate into @par_id_template, @par_id_amm
while(@@fetch_status=0) 
	BEGIN

		DECLARE c_oggCustom CURSOR FOR select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = @par_id_template and dpa_associazione_templates.doc_number = '' and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id and dpa_oggetti_custom.conta_dopo <> 1;
		OPEN c_oggCustom
		FETCH next from c_oggCustom into @par_id_oggetto_Custom
		while(@@fetch_status=0) 
			BEGIN
				
				--DECLARE c_idRuoli CURSOR FOR select id_ruolo from dpa_vis_tipo_doc where id_tipo_doc = @par_id_template; --and id_ruolo <> 0;
				DECLARE c_idRuoli CURSOR FOR select id_gruppo from dpa_corr_globali where id_amm = @par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
				OPEN c_idRuoli	
				FETCH next from c_idRuoli into @par_id_ruolo
				while(@@fetch_status=0) 
				BEGIN

					if( (select count(*) from DPA_A_R_OGG_CUSTOM_DOC where id_template = @par_id_template and id_oggetto_custom = @par_id_Oggetto_Custom and id_ruolo = @par_id_Ruolo) = 0)
					begin
						insert into DPA_A_R_OGG_CUSTOM_DOC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, 1, 1);
					end
			
				FETCH next from c_idRuoli into @par_id_ruolo
				END
				DEALLOCATE c_idRuoli	
		
			FETCH next from c_oggCustom into @par_id_oggetto_Custom
			END
			DEALLOCATE c_oggCustom		

	--FETCH next from c_idTemplate into @par_id_template
	FETCH next from c_idTemplate into @par_id_template, @par_id_amm
	END
	DEALLOCATE c_idTemplate
END
GO	

--Popolazione DPA_A_R_OGG_CUSTOM_FASC con la visibilità dei campi profilati escluso i contatori con CONTA_DOPO attivo
BEGIN
DECLARE @par_id_template int;
DECLARE @par_id_oggetto_Custom int;
DECLARE @par_id_ruolo int;
DECLARE @par_id_amm int;
--DECLARE c_idTemplate CURSOR FOR SELECT system_id from dpa_tipo_fasc;
DECLARE c_idTemplate CURSOR FOR SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
OPEN c_idTemplate
--FETCH next from c_idTemplate into @par_id_template
FETCH next from c_idTemplate into @par_id_template, @par_id_amm
while(@@fetch_status=0) 
	BEGIN

		DECLARE c_oggCustom CURSOR FOR select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = @par_id_template and dpa_ass_templates_fasc.id_project = '' and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id and dpa_oggetti_custom_fasc.conta_dopo <> 1;
		OPEN c_oggCustom
		FETCH next from c_oggCustom into @par_id_oggetto_Custom
		while(@@fetch_status=0) 
			BEGIN
				
				--DECLARE c_idRuoli CURSOR FOR select id_ruolo from dpa_vis_tipo_fasc where id_tipo_fasc = @par_id_template; --and id_ruolo <> 0;
				DECLARE c_idRuoli CURSOR FOR select id_gruppo from dpa_corr_globali where id_amm = @par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
				OPEN c_idRuoli	
				FETCH next from c_idRuoli into @par_id_ruolo
				while(@@fetch_status=0) 
				BEGIN
					
					if( (select count(*) from DPA_A_R_OGG_CUSTOM_FASC where id_template = @par_id_template and id_oggetto_custom = @par_id_Oggetto_Custom and id_ruolo = @par_id_Ruolo) = 0)
					begin
						insert into DPA_A_R_OGG_CUSTOM_FASC (id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(@par_id_template, @par_id_Oggetto_Custom, @par_id_Ruolo, 1, 1);
					end
					
				FETCH next from c_idRuoli into @par_id_ruolo
				END
				DEALLOCATE c_idRuoli	
		
			FETCH next from c_oggCustom into @par_id_oggetto_Custom
			END
			DEALLOCATE c_oggCustom		

	--FETCH next from c_idTemplate into @par_id_template
	FETCH next from c_idTemplate into @par_id_template, @par_id_amm
	END
	DEALLOCATE c_idTemplate
END
GO	
/*FINE VISIBILITA CAMPI PROFILAZIONE DINAMICA*/


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[iscorrispondenteinterno]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[iscorrispondenteinterno] 
go
CREATE FUNCTION [@db_user].[iscorrispondenteinterno]
( @idcorrglobali   INT,
   @idregistro      INT
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
   DECLARE
      @tipourp                 VARCHAR (1);
   DECLARE 
	  @tipoie                  VARCHAR (1);
   DECLARE 
	  @idpeople                INT;
   DECLARE 
	  @numero_corrispondenti   INT;


     SET @tipoie = 'E'
     SET   @numero_corrispondenti = 0

      SELECT @tipoie = a.cha_tipo_ie
        FROM dpa_corr_globali a
       WHERE a.system_id = @idcorrglobali;

   IF (@tipoie = 'I')
   BEGIN
         SELECT @tipourp = a.cha_tipo_urp
          FROM dpa_corr_globali a
          WHERE a.system_id = @idcorrglobali;

         IF (@tipourp = 'U')
		 BEGIN
            SELECT @numero_corrispondenti = COUNT (*)
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
              WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND id_uo = @idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = @idregistro
               AND r.cha_rf = '0';
         END

          IF (@tipourp = 'R')
          BEGIN
            SELECT @numero_corrispondenti = COUNT (*)
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND a.system_id = @idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = @idregistro
               AND r.cha_rf = '0';
          END 

            IF (@tipourp = 'P')
            BEGIN
               SELECT @idpeople=a.id_people
                 FROM dpa_corr_globali a
                WHERE a.system_id = @idcorrglobali;

               SELECT @numero_corrispondenti = COUNT (a.system_id)
                   FROM dpa_corr_globali a,
                        peoplegroups b,
                        dpa_l_ruolo_reg f,
                        dpa_el_registri r
                  WHERE a.id_gruppo = b.groups_system_id
                    AND b.dta_fine IS NULL
                    AND b.people_system_id = @idpeople
                    AND f.id_ruolo_in_uo = a.system_id
                    AND f.id_registro = r.system_id
                    AND r.system_id = @idregistro
                    AND r.cha_rf = '0'
            END
         END

      IF (@numero_corrispondenti > 0)
         return 1
      
      RETURN 0

END
GO

/*Inserimento per cambio etichette protocollatura */

IF NOT EXISTS (select * from sysobjects where name = 'DPA_LETTERE_DOCUMENTI' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_LETTERE_DOCUMENTI](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[CODICE] [varchar](100) NOT NULL,
	[DESCRIZIONE] [varchar](255) NOT NULL,
	[ETICHETTA] [varchar](255) NOT NULL
) ON [PRIMARY]
end
GO


begin
	declare @cnt int; 
	set @cnt = (select count(*) from [@db_user].[DPA_LETTERE_DOCUMENTI]);
	if (@cnt = 0) 	
		insert into [@db_user].[DPA_LETTERE_DOCUMENTI] VALUES('A','A','Ingresso');
		insert into [@db_user].[DPA_LETTERE_DOCUMENTI] VALUES('P','P','Uscita');
		insert into [@db_user].[DPA_LETTERE_DOCUMENTI] VALUES('I','I','Interno');
		insert into [@db_user].[DPA_LETTERE_DOCUMENTI] VALUES('G','NP','NP');
		insert into [@db_user].[DPA_LETTERE_DOCUMENTI] VALUES('ALL','ALL','Allegato');	
end;
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_ASS_LETTERE_DOCUMENTI' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_ASS_LETTERE_DOCUMENTI](
	[ID_AMM] [int] NOT NULL,
	[ID_LETTERADOC] [int] NOT NULL,
	[DESCRIZIONE] [varchar](255) NOT NULL,
	[ETICHETTA] [varchar](255) NOT NULL
)  ON [PRIMARY]
end
GO

/* Inserimento voci per definizione funzioni di importazione fascicoli / documenti / RDE */
if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMP_FASC')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('IMP_FASC','Abilita la voce di menu'' Import Fascicoli sotto Ricerca', '', 'N')
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMP_DOCS')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('IMP_DOCS','Abilita la voce di menu'' Import Documenti sotto Documenti', '', 'N')
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='IMP_RDE')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('IMP_RDE','Abilita la voce di menu'' Import Emergenza sotto Gestione', '', 'N')
END
GO

/* Fine dichiarazione voci per definizione funzioni */

-- Elenco note
IF NOT EXISTS (select *  from sysobjects where name = 'DPA_ELENCO_NOTE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_ELENCO_NOTE] (
	[SYSTEM_ID] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[ID_REG_RF] [int] NULL ,
	[VAR_DESC_NOTA][varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[COD_REG_RF][varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
end
go

if exists (select *  from dbo.sysindexes where name = N'indx_dpa_elenco_note1' and id = object_id(N'[@db_user].[DPA_ELENCO_NOTE]'))
drop index [@db_user].[DPA_ELENCO_NOTE].[indx_dpa_elenco_note1]
GO
CREATE 
  INDEX [indx_dpa_elenco_note1] ON [@db_user].[DPA_ELENCO_NOTE] ([SYSTEM_ID ])
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='GEST_DELEGHE'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('ELENCO_NOTE','Abilita il sottomenu'' Elenco note dal menu'' Gestione');
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DIRITTO_DELEGA'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('IMPORT_ELENCO_NOTE','Abilita l''import di un elenco di note da foglio excel'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DIRITTO_DELEGA'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('RICERCA_NOTE_ELENCO','Abilita la ricerca delle note da un elenco'); 
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DIRITTO_DELEGA'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI](COD_FUNZIONE, VAR_DESC_FUNZIONE)
VALUES ('INSERIMENTO_NOTERF','Abilita l''inserimento delle note associate ad un dato rf'); 
END
GO

if not exists(select * from syscolumns where name='IDRFASSOCIATO' and id in 
(select id from sysobjects where name='DPA_NOTE' and xtype='U'))
BEGIN
ALTER TABLE DPA_NOTE ADD IDRFASSOCIATO INTEGER
END
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[GetCountNote]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[GetCountNote] 
go
CREATE   FUNCTION [@db_user].[GetCountNote] (@tipoOggetto char(1), @idOggetto int, @note nvarchar(2000), @idUtente int, @idGruppo int, @tipoRic char(1), @idRegistro int)
RETURNS int AS
BEGIN
declare @ret int	-- variabile intera che conterrà il risultato da restituire
if(@tipoRic = 'Q')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo))
else if(@tipoRic = 'T')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T')

else if(@tipoRic = 'P')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente)

else if(@tipoRic = 'R')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo)

else if (@tipoRic = 'F')
SELECT  @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N 
WHERE  N.TIPOOGGETTOASSOCIATO =  @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND 
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = @idRegistro)

RETURN  @ret
END



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[GetCountNote]') AND xtype in (N'FN', N'IF', N'TF'))
DROP  function [@db_user].[GetCountNote] 
go
CREATE   FUNCTION [@db_user].[GetCountNote] (@tipoOggetto char(1), @idOggetto int, @note nvarchar(2000), @idUtente int, @idGruppo int, @tipoRic char(1), @idRegistro int)
RETURNS int AS
BEGIN
declare @ret int	-- variabile intera che conterrà il risultato da restituire
if(@tipoRic = 'Q')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo) OR 
(n.tipovisibilita = 'F' AND n.idrfassociato in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=@idGruppo ))
))

else if(@tipoRic = 'T')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'T')

else if(@tipoRic = 'P')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @idUtente)

else if(@tipoRic = 'R')
SELECT @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @idGruppo)

else if (@tipoRic = 'F' and idRegistro!=0)
SELECT  @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N 
WHERE  N.TIPOOGGETTOASSOCIATO =  @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND 
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = @idRegistro)

else if (@tipoRic = 'F' and idRegistro=0)
SELECT  @ret = COUNT(SYSTEM_ID)
FROM   DPA_NOTE N 
WHERE  N.TIPOOGGETTOASSOCIATO =  @tipoOggetto AND
N.IDOGGETTOASSOCIATO = @idOggetto AND
upper(N.TESTO) LIKE upper('%'+@note+'%') AND 
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=@idGruppo )))

RETURN  @ret
END
--fine elenco note

--INIZIO IPERDOCUMENTO
if exists (SELECT * FROM syscolumns WHERE name='IPERFASCICOLO' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	EXEC sp_rename 'DPA_TIPO_ATTO.IPERFASCICOLO', 'IPERDOCUMENTO', 'COLUMN'	
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='COD_MOD_TRASM' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [DPA_TIPO_ATTO] ADD COD_MOD_TRASM varchar(128);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='COD_CLASS' and id in (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [DPA_TIPO_ATTO] ADD COD_CLASS varchar(128);
END;
GO

--FINE IPERDOCUMENTO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_COUNT_TODOLIST_NO_REG]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_COUNT_TODOLIST_NO_REG]
GO
CREATE  PROCEDURE  [@db_user].[SP_COUNT_TODOLIST_NO_REG]
@idPeople int,
@idGroup int
AS
--tabella temporanea
CREATE TABLE [@db_user].[#COUNT_TODOLIST]
(
[TOT_DOC] [int],
[TOT_DOC_NO_LETTI] [int],
[TOT_DOC_NO_ACCETTATI] [int],
[TOT_FASC] [int],
[TOT_FASC_NO_LETTI] [int],
[TOT_FASC_NO_ACCETTATI] [int],
[TOT_DOC_PREDISPOSTI] [int],
) ON [PRIMARY]

-- variabili locali
DECLARE @trasmdoctot int;
DECLARE @trasmdocnonletti int;
DECLARE @trasmdocnonaccettati int;
DECLARE @trasmfasctot int;
DECLARE @trasmfascnonletti int;
DECLARE @trasmfascnonaccettati int;
DECLARE @docpredisposti int;
BEGIN

--SETTING DELLE VARIABILI
SET @trasmdoctot = 0
SET @trasmdocnonletti = 0
SET @trasmdocnonaccettati = 0
SET @trasmfasctot = 0
SET @trasmfascnonletti = 0
SET @trasmfascnonaccettati = 0
SET @docpredisposti = 0
--END SETTING

--numero documenti presenti in todolist
SELECT @trasmdoctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
      AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero documenti non letti in todolist
SELECT @trasmdocnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
       AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti non ancora accettati in todolist
SELECT @trasmdocnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_profile > 0
          AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                     AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli presenti in todolist
SELECT @trasmfasctot = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--numero fascicoli non letti in todolist
SELECT @trasmfascnonletti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero fascicoli non ancora accettati in todolist
SELECT @trasmfascnonaccettati = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                    AND dta_vista = CONVERT(VARCHAR,'17530101',103);

--numero documenti predisposti
SELECT @docpredisposti = COUNT (DISTINCT (id_trasmissione))
FROM dpa_todolist
WHERE  id_profile > 0
AND id_profile IN (SELECT system_id
FROM PROFILE
WHERE cha_da_proto = '1')
AND (   (id_people_dest = @idPeople AND id_ruolo_dest = @idGroup)
OR (id_people_dest = @idPeople AND id_ruolo_dest = 0)
);

--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #COUNT_TODOLIST
(TOT_DOC, TOT_DOC_NO_LETTI, TOT_DOC_NO_ACCETTATI, TOT_FASC, TOT_FASC_NO_LETTI,
TOT_FASC_NO_ACCETTATI, TOT_DOC_PREDISPOSTI)
values
(@trasmdoctot, @trasmdocnonletti, @trasmdocnonaccettati, @trasmfasctot, @trasmfascnonletti,
@trasmfascnonaccettati, @docpredisposti)
END
-- return dei risultati
SELECT * FROM #COUNT_TODOLIST
GO

--Inizio FASCICOLI CONTROLLATI
if not exists(select * from syscolumns where name='CHA_CONTROLLATO' and id in 
(select id from sysobjects where name='PROJECT'  and xtype='U'))
BEGIN
ALTER TABLE project ADD CHA_CONTROLLATO VARCHAR(1) DEFAULT 0 NOT NULL
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='FASC_CONTROLLATO'))
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_LOG](var_codice, var_descrizione, var_oggetto, var_metodo)
VALUES ('FASC_CONTROLLATO', 'Modificata proprietà controllato del fascicolo', 'FASCICOLO', 'FASCCONTROLLATO'); 
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='FASC_CONTROLLATO')
BEGIN
insert into [@db_user].[DPA_ANAGRAFICA_FUNZIONI] VALUES('FASC_CONTROLLATO','Permette la creazione/modifica di un fascicolo controllato', '', 'N')
END
GO
-- FINE FASCICOLI CONTROLLATI

if exists (SELECT * FROM syscolumns WHERE name='GROUP_NAME' and id in 
	(SELECT id FROM sysobjects WHERE name='GROUPS' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[GROUPS] ALTER COLUMN GROUP_NAME VARCHAR(256);
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_DOC_OBBL' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA] ADD
	[TIPO_DOC_OBBL] char(1) NOT NULL CONSTRAINT [DF_DPA_AMMINISTRA_TIPO_DOC_OBBL] DEFAULT 0;
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='NO_NOTIFY' and id in (SELECT id FROM sysobjects WHERE name='DPA_MODELLI_TRASM' and xtype='U'))
BEGIN
      ALTER TABLE [@dbuser].[DPA_MODELLI_TRASM] ADD NO_NOTIFY varchar(1);
END;
GO

--Creazione Tabella Storico Data Arrivo

IF NOT EXISTS (select * from sysobjects where name = 'DPA_DATA_ARRIVO_STO' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_DATA_ARRIVO_STO](
	[SYSTEM_ID] [int] IDENTITY(1,2) NOT NULL,
	[DOCNUMBER] [int] NOT NULL,
	[DTA_ARRIVO] [datetime] NULL,
	[ID_GROUP] [int] NOT NULL,
	[ID_PEOPLE] [int] NOT NULL,
	[DTA_MODIFICA] [datetime] NULL,
 CONSTRAINT [PK_DPA_DATA_ARRIVO_STO] PRIMARY KEY CLUSTERED 
(
	[SYSTEM_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_GROUPS] FOREIGN KEY([ID_GROUP])
REFERENCES [@db_user].[GROUPS] ([SYSTEM_ID])
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_GROUPS]
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PEOPLE] FOREIGN KEY([ID_PEOPLE])
REFERENCES [@db_user].[PEOPLE] ([SYSTEM_ID])
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PEOPLE]
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO]  WITH CHECK ADD  CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PROFILE] FOREIGN KEY([DOCNUMBER])
REFERENCES [@db_user].[PROFILE] ([SYSTEM_ID])
GO

ALTER TABLE [@db_user].[DPA_DATA_ARRIVO_STO] CHECK CONSTRAINT [FK_DPA_DATA_ARRIVO_STO_PROFILE]
GO
--fine creazione tabelle

--Inserimento Microfunzione per abilitare la modifica di ora pervenuto (DO_PROT_DATA_ORA_MODIFICA)
if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_PROT_DATA_ORA_MODIFICA')
BEGIN
INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI]
           ([COD_FUNZIONE]
           ,[VAR_DESC_FUNZIONE]
           ,[DISABLED])
     VALUES
           ('DO_PROT_DATA_ORA_MODIFICA','Abilito la modifica di data arrivo e ora pervenuto nella protocollazione'
           , 'N')
END
GO
--fine inserimento Microfunzione DO_PROT_DATA_ORA_STORIA

--inserimento Microfunzione DO_PROT_DATA_ORA_STORIA

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_PROT_DATA_ORA_STORIA')
BEGIN
INSERT INTO [@dbuser].DOCSADM.DPA_ANAGRAFICA_FUNZIONI
           ([COD_FUNZIONE]
           ,[VAR_DESC_FUNZIONE]
           ,[DISABLED])
     VALUES
           ('DO_PROT_DATA_ORA_STORIA','Abilita il pulsante per lo storico della data arrivo e ora pervenuto nella creazione del protocollo'
           , 'N')
END
GO

GO

if not exists (SELECT * FROM syscolumns WHERE name='NO_NOTIFY' and id in (SELECT id FROM sysobjects WHERE name='DPA_MODELLI_TRASM' and xtype='U'))
BEGIN
      ALTER TABLE [@dbuser].[DPA_MODELLI_TRASM] ADD NO_NOTIFY varchar(1);
END;
GO


if not exists (SELECT * FROM syscolumns WHERE name='COD_DESC_INTEROP' and id in (SELECT id FROM sysobjects WHERE name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
      ALTER TABLE [@dbuser].[DPA_CORR_GLOBALI] ADD COD_DESC_INTEROP varchar(516);
END;
GO

--INIZIO MITTENTI MULTIPLI
if exists(select * from syscolumns where name='CHA_TIPO_MITT_DEST' and id in (select id from sysobjects where name='DPA_DOC_ARRIVO_PAR' and xtype='U'))
BEGIN
ALTER TABLE DPA_DOC_ARRIVO_PAR ALTER COLUMN CHA_TIPO_MITT_DEST VARCHAR(2)
END
GO

if exists(select * from syscolumns where name='CHA_TIPO_MITT_DES' and id in (select id from sysobjects where name='DPA_CORR_STO' and xtype='U'))
BEGIN
ALTER TABLE DPA_CORR_STO ALTER COLUMN CHA_TIPO_MITT_DES VARCHAR(2)
END
GO
--FINE MITTENTI MULTIPLI

--TIMESTAMP DOCUMENTI
if not exists (SELECT * FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE = 'DO_TIMESTAMP')
BEGIN
	INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES('DO_TIMESTAMP','Abilita la generazione di un timestamp per i documenti',null,'N')
END;
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[db_user].[DPA_TIMESTAMP_DOC]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_TIMESTAMP_DOC](
	[SYSTEM_ID] [int] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[DOC_NUMBER] [int],
	[VERSION_ID] [int],
	[ID_PEOPLE] [int],
	[DTA_CREAZIONE] [DATETIME],
	[DTA_SCADENZA] [DATETIME],
	[NUM_SERIE] [varchar](64),
	[S_N_CERTIFICATO] [varchar](64),
	[ALG_HASH] [varchar](64),
	[SOGGETTO] [varchar](64),
	[PAESE] [varchar](64),
	[TSR_FILE] [text]
)
END
GO
--FINE TIMESTAMP DOCUMENTI

---
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_NOTIFICA]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_NOTIFICA]
				( 
				SYSTEM_ID               INT, 
				ID_TIPO_NOTIFICA        INT, 
				DOCNUMBER               INT, 
				VAR_MITTENTE            VARCHAR(255), 
				VAR_TIPO_DESTINATARIO   VARCHAR(100), 
				VAR_DESTINATARIO        VARCHAR(255), 
				VAR_RISPOSTE            VARCHAR(255), 
				VAR_OGGETTO             VARCHAR(516), 
				VAR_GESTIONE_EMITTENTE  VARCHAR(255), 
				VAR_ZONA                VARCHAR(10 ), 
				VAR_GIORNO_ORA          DATETIME, 
				VAR_IDENTIFICATIVO      VARCHAR(516), 
				VAR_MSGID               VARCHAR(516), 
				VAR_TIPO_RICEVUTA       VARCHAR(516), 
				VAR_CONSEGNA            VARCHAR(516), 
				VAR_RICEZIONE           VARCHAR(516), 
				VAR_ERRORE_ESTESO       TEXT, 
				 VAR_ERRORE_RICEVUTA     VARCHAR(50) 
				)
END
GO


IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_TIPO_NOTIFICA]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE DPA_TIPO_NOTIFICA 
				( 
				SYSTEM_ID            INT                NOT NULL  , 
				VAR_CODICE_NOTIFICA  VARCHAR(50)        NOT NULL, 
				VAR_DESCRIZIONE      VARCHAR(255) 
				)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_DOCUMENTO_DA_PEC' and id in (SELECT id FROM sysobjects WHERE name='PROFILE' and xtype='U'))
BEGIN
      ALTER TABLE [@dbuser].[PROFILE] ADD CHA_DOCUMENTO_DA_PEC varchar(1);
END;
GO
-- LOCALITA'
-- MODIFICA STORE PROCEDURE
if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[SP_MODIFY_CORR_ESTERNO]') and 
OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[SP_MODIFY_CORR_ESTERNO]
GO

CREATE PROCEDURE [@db_user].[SP_MODIFY_CORR_ESTERNO]
@IDCorrGlobale INT,
@desc_corr  VARCHAR(128),
@nome  VARCHAR(50),
@cognome  VARCHAR(50),
@codice_aoo VARCHAR(16),
@codice_amm VARCHAR(32),
@email  VARCHAR(128),
@indirizzo VARCHAR(128),
@cap VARCHAR(5),
@provincia VARCHAR(2),
@nazione VARCHAR(32),
@citta  VARCHAR(64),
@cod_fiscale VARCHAR(16),
@telefono VARCHAR(16),
@telefono2 VARCHAR(16),
@note VARCHAR(250),
@fax VARCHAR(16),
@var_idDocType INT,
@inrubricacomune VARCHAR(1),
@tipourp VARCHAR(1),
@localita VARCHAR(128),
@luogoNascita VARCHAR(128),
@dataNascita VARCHAR(64),
@titolo VARCHAR(64),
@newid INTEGER OUTPUT
AS

/*
1)	update del corrispondente vecchio:
- settando var_cod_rubrica = var_cod_rubrica_system_id;
- settando dta_fine = GETDATE();

2)	insert del nuovo corrispondente (DPA_CORR_GLOBALI):
- codice rubrica = codice rubrica del corrispondente che ? stato storicizzato al punto 1)
- id_old = system_id del corrispondente storicizzato al punto 1)

2.1) insert del dettaglio del  nuovo corrispondente (DPA_DETT_GLOBALI) solo per UTENTI e UO

*/

DECLARE @ReturnValue INT
DECLARE @cod_rubrica VARCHAR(128)
DECLARE @id_reg INT
DECLARE @idAmm INT
DECLARE @new_var_cod_rubrica VARCHAR(128)
DECLARE @cha_dettaglio VARCHAR(1)
DECLARE @cha_tipo_urp VARCHAR(1)
DECLARE @v_id_docType INT
DECLARE @cha_pa VARCHAR(1)
DECLARE @cha_TipoIE VARCHAR(1)
DECLARE @num_livello INT
DECLARE @id_parent INT
DECLARE @id_peso_org INT
DECLARE @id_uo INT
DECLARE @id_tipo_ruolo INT
DECLARE @id_gruppo INT

BEGIN

if(@tipourp is not null and @tipourp != '' and @cha_tipo_urp is not null and @cha_tipo_urp!=@tipourp)
begin
set @cha_tipo_urp = @tipourp
end

SELECT
@cod_rubrica = VAR_COD_RUBRICA,
@cha_tipo_urp = CHA_TIPO_URP,
@id_reg = ID_REGISTRO,
@idAmm = ID_AMM,
@cha_pa = CHA_PA, 
@cha_TipoIE = CHA_TIPO_IE,
@num_livello = NUM_LIVELLO, 
@id_parent = ID_PARENT, 
@id_peso_org = ID_PESO_ORG, 
@id_uo = ID_UO, 
@id_tipo_ruolo = ID_TIPO_RUOLO, 
@id_gruppo = ID_GRUPPO
FROM DPA_CORR_GLOBALI
WHERE system_id = @IDCorrGlobale

IF @@ROWCOUNT > 0  --1
BEGIN

SELECT @v_id_docType = ID_DOCUMENTTYPE
FROM DPA_T_CANALE_CORR
WHERE ID_CORR_GLOBALE = @IDCorrGlobale

IF @@ROWCOUNT > 0
BEGIN
-- calcolo il nuovo codice rubrica
--SET @new_var_cod_rubrica = @var_cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale)
SET @new_var_cod_rubrica = @cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale )

SET @cha_dettaglio = '0' -- default

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
SET @cha_dettaglio = '1'

BEGIN


--VERIFICO se il corrisp ? stato utilizzato come dest/mitt di protocolli
SELECT ID_PROFILE
FROM DPA_DOC_ARRIVO_PAR
WHERE ID_MITT_DEST =  @IDCorrGlobale

-- 1) non ? stato mai utilizzato come corrisp in un protocollo
IF (@@ROWCOUNT = 0)
BEGIN
--non devo storicizzare, aggiorno solamente i dati
UPDATE 	DPA_CORR_GLOBALI
SET 	VAR_CODICE_AOO= @codice_aoo,
VAR_CODICE_AMM = @codice_amm,
VAR_EMAIL = @email,
VAR_DESC_CORR = @desc_corr,
VAR_NOME = @nome,
				VAR_COGNOME= @cognome,
				CHA_PA=@cha_pa,
				CHA_TIPO_URP=@cha_tipo_urp
WHERE 	SYSTEM_ID = @IDCorrGlobale

IF(@@ROWCOUNT > 0) -- SE UPDATE ? andata a buon fine
BEGIN
begin
--per utenti e Uo aggiorno il dettaglio
IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )

BEGIN

UPDATE DPA_DETT_GLOBALI
SET 	VAR_INDIRIZZO = @indirizzo,
VAR_CAP = @cap,
VAR_PROVINCIA = @provincia,
VAR_NAZIONE = @nazione,
VAR_COD_FISCALE = @cod_fiscale,
VAR_TELEFONO = @telefono,
VAR_TELEFONO2 = @telefono2,
VAR_NOTE= @note,
VAR_CITTA= @citta,
VAR_FAX = @fax,
VAR_LOCALITA = @localita,
VAR_LUOGO_NASCITA = @luogoNascita,
DTA_NASCITA = @dataNascita,
VAR_TITOLO = @titolo

WHERE ID_CORR_GLOBALI = @IDCorrGlobale

IF(@@ROWCOUNT > 0)
SET @ReturnValue = 1
ELSE
SET @ReturnValue = 0

END

ELSE
SET @ReturnValue = 1 -- CASO RUOLI
end

IF (@ReturnValue = 1)

BEGIN

UPDATE DPA_T_CANALE_CORR
SET  ID_DOCUMENTTYPE =  @var_idDocType
WHERE ID_CORR_GLOBALE = @IDCorrGlobale

IF(@@ROWCOUNT > 0)
SET @ReturnValue = 1
ELSE
SET @ReturnValue = 0
END

END

ELSE
SET @ReturnValue = 0

END
ELSE

-- caso 2) Il corrisp ? stato utilizzato come destinatario
BEGIN
--  INIZIO STORICIZZAZIONE DEL CORRISPONDENTE

UPDATE	DPA_CORR_GLOBALI
SET 	DTA_FINE = GETDATE(),  
VAR_COD_RUBRICA =@new_var_cod_rubrica,  
VAR_CODICE =@new_var_cod_rubrica,
ID_PARENT = NULL
WHERE 	SYSTEM_ID = @IDCorrGlobale

-- se la storicizzazione ? andata a buon fine,
--posso inserire il nuovo corrispondente
IF @@ROWCOUNT > 0

BEGIN
DECLARE @cha_tipo_corr VARCHAR(1)
IF (@inrubricacomune = '1')
SET @cha_tipo_corr = 'C'
ELSE
SET @cha_tipo_corr = 'S'

INSERT INTO DPA_CORR_GLOBALI (
						NUM_LIVELLO,
						CHA_TIPO_IE,
						ID_REGISTRO,
						ID_AMM,
						VAR_DESC_CORR,
						VAR_NOME,
						VAR_COGNOME,
						ID_OLD,
						DTA_INIZIO,
						ID_PARENT,
						VAR_CODICE,
						CHA_TIPO_CORR,
						CHA_TIPO_URP,
						VAR_CODICE_AOO,
						VAR_COD_RUBRICA,
						CHA_DETTAGLI,
						VAR_EMAIL,
						VAR_CODICE_AMM,
						CHA_PA,
						ID_PESO_ORG,
						ID_GRUPPO,
						ID_TIPO_RUOLO,
						ID_UO
)
VALUES (
						@num_livello,
						@cha_TipoIE,
						@id_reg,
						@idAmm,
						@desc_corr,
						@nome,
						@cognome,
						@IDCorrGlobale,
						GETDATE(),
						@id_parent,
						@cod_rubrica,
						@cha_tipo_corr,
						@cha_tipo_urp,
						@codice_aoo,
						@cod_rubrica,
						@cha_dettaglio,
						@email,
						@codice_amm,
						@cha_pa,
						@id_peso_org,
						@id_gruppo,
						@id_tipo_ruolo,
						@id_uo
)

--prendo la systemId appena inserita
SET @newid = @@identity

IF @@ROWCOUNT > 0 -- se l'inserimento del nuovo corrisp ? andato a buon fine

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' ) -- CASO UTENTE/UO: inserisco il dettaglio
BEGIN

INSERT INTO DPA_DETT_GLOBALI
(
ID_CORR_GLOBALI,
VAR_INDIRIZZO,
VAR_CAP,
VAR_PROVINCIA,
VAR_NAZIONE,
VAR_COD_FISCALE,
VAR_TELEFONO,
VAR_TELEFONO2,
VAR_NOTE,
VAR_CITTA,
VAR_FAX,
VAR_LOCALITA,
VAR_LUOGO_NASCITA,
DTA_NASCITA,
VAR_TITOLO
)
VALUES
(
@newid,
@indirizzo,
@cap,
@provincia,
@nazione,
@cod_fiscale,
@telefono,
@telefono2,
@note,
@citta,
@fax,
@localita,
@luogoNascita,
@dataNascita,
@titolo
)

IF @@ROWCOUNT > 0
-- se la insert su dpa_dett_globali ? andata a buon fine
SET @ReturnValue = 1 -- valore ritornato 1
ELSE
-- se la insert su dpa_dett_globali non ? andata a buon fine
SET @ReturnValue = 0 -- valore ritornato 0
END

ELSE  -- CASO RUOLO: non inserisco il dettaglio
-- vuol dire che il corrispondente ? un RUOLO (quindi non deve essere fatta la insert sulla dpa_dett_globali)
--valore ritornato 1 perch? significa che l'operazione di inserimento del nuovo ruolo ? andato a buon fine
SET @ReturnValue = 1

IF (@ReturnValue = 1)

BEGIN

INSERT INTO DPA_T_CANALE_CORR
(
ID_CORR_GLOBALE,
ID_DOCUMENTTYPE,
CHA_PREFERITO
)
VALUES
(
@newid,
@var_idDocType,
'1'
)

IF @@ROWCOUNT > 0
-- se la insert su DPA_T_CANALE_CORR ? andata a buon fine
SET @ReturnValue = 1 -- valore ritornato 1
ELSE
-- se la insert su DPA_T_CANALE_CORRnon ? andata a buon fine
SET @ReturnValue = 0 -- valore ritornato 0
END
ELSE

SET @ReturnValue = 0 -- inserimento non andato a buon fine: ritorno 0 ed esco

-- FINE STORICIZZAZIONE

END

END

END

END
ELSE
SET @ReturnValue = 0

END

ELSE
SET @ReturnValue = 0 -- la storicizzazione del corrispondente ? andata male:  ritorno 0 ed esco -- END 1

END

RETURN @ReturnValue

GO


-- tabella contenente le qualifiche dei corrispondenti


IF NOT EXISTS (select * from sysobjects where name = 'DPA_QUALIFICA_CORRISPONDENTE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_QUALIFICA_CORRISPONDENTE] (
 [SYSTEM_ID] [INT] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
 [VAR_TITOLO] [VARCHAR](64) NULL,
 [DTA_FINE_VALIDITA] [DATE] NULL
)
end
GO

-- chiave esterna tra la tabella dei titoli 
if not exists (SELECT * FROM syscolumns WHERE name='ID_QUALIFICA_CORR' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN

	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI]  ADD ID_QUALIFICA_CORR INT
	
	ALTER TABLE [@db_user].[DPA_DETT_GLOBALI] ADD FOREIGN KEY (ID_QUALIFICA_CORR) REFERENCES [@db_user].[DPA_QUALIFICA_CORRISPONDENTE](SYSTEM_ID)
	
END;

GO

-- FINE LOCALITA'

--NUOVA FUNZIONE getIfModelloAutorizzato
ALTER FUNCTION [@db_user].[getIfModelloAutorizzato]
(	
	@id_ruolo int,
	@id_people  int, 
	@system_id  int, 
	@id_modelloTrasm int,
	@accessRigth int
)

RETURNS INT as 
 
BEGIN 
DECLARE @RetVal INT
--DECLARE @accessRigth INT
DECLARE @idRagione INT
DECLARE @tipo_diritto char
DECLARE c3 CURSOR LOCAL FOR
	select distinct ID_RAGIONE FROM dpa_modelli_mitt_dest WHERE id_modello=@id_modelloTrasm and CHA_TIPO_MITT_DEST <> 'M'
	
	SET @RetVal=1
	--SET @accessRigth = ( SELECT DOCSADM.getAccessRights(@id_ruolo, @id_people, @system_id))
	if(@accessRigth=45)
		BEGIN
		open c3
			fetch next from c3 into @idRagione
			while @@fetch_status=0
			BEGIN
			while(@retVal!=0)    
				BEGIN 		
				select @tipo_diritto = CHA_TIPO_DIRITTI from DPA_RAGIONE_TRASM where system_id=@idRagione
					if(@tipo_diritto != 'R' and @tipo_diritto != 'C')
						begin
						SET @retVal=0                                
						end					
				END
			END              
	close cur
	deallocate cur
	END
	RETURN @RetVal

END
--FINE NUOVA FUNZIONE getIfModelloAutorizzato


--INIZIO NEW CORRCAT
ALTER  function [@db_user].[corrcat] (@docId int, @dirProt varchar(1))
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(2)

set @outcome=''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
from dpa_corr_globali c WITH (NOLOCK), dpa_doc_arrivo_par dap WITH (NOLOCK)
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr
while(@@fetch_status=0)
begin
if (@dirProt='P' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='MD')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MM); '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (M); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].parsenull(@item)+' (CC); '
end

if (@dirProt='P' and @dirCorr='L')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_LISTA(@docId)
end

if (@dirProt='P' and @dirCorr='F')
begin
if(@item is not null)
set @outcome=@outcome+[@db_user].DEST_IN_RF(@docId)
end

fetch next from cur into @item,@dirCorr

end

close cur

deallocate cur

if (len(@outcome)>0)

set @outcome = substring(@outcome,1,(len(@outcome)-1))

return @outcome

end
--FINE NEW CORRCAT




if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in 
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE DPA_AMMINISTRA ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='IS_ENABLED_SMART_CLIENT' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE PEOPLE ADD IS_ENABLED_SMART_CLIENT CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='SMART_CLIENT_PDF_CONV_ON_SCAN' and id in 
(select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
ALTER TABLE DPA_AMMINISTRA ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='SMART_CLIENT_PDF_CONV_ON_SCAN' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE PEOPLE ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1) NULL
END
GO






if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='MODIFICADOCSTATOFINALE'))
Begin
                insert into dpa_anagrafica_log
                (
                var_codice,
                var_descrizione,
                var_oggetto,
                var_metodo
                )
                values 
                (
                'MODIFICADOCSTATOFINALE',
                'Azione di modifica dei diritti di lettura / scrittura sul documento in stato finale',
                'DOCUMENTO',
                'MODIFICADOCSTATOFINALE'
                );
 end
 GO
----------Nuovo campo per la modifica dei documenti in stato finale
if not exists(select * from syscolumns where name='CHA_UNLOCKED_FINAL_STATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CHA_UNLOCKED_FINAL_STATE VARCHAR(1) NULL
END
GO

---------fine nuovo campo

-----inizio chiavi configurazione


--aggiunge la colonna “VAR_CODICE_OLD_WEBCONFIG” alla tabella
--“DPA_CHIAVI_CONFIGURAZIONE” per tenere traccia del vecchio codice della chiave
-- nella vecchia gestione da Web.config
if not exists (SELECT * FROM syscolumns WHERE name='VAR_CODICE_OLD_WEBCONFIG' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_CHIAVI_CONFIGURAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [@dbuser].[DPA_CHIAVI_CONFIGURAZIONE]
	ADD  VAR_CODICE_OLD_WEBCONFIG  varchar(64);
END;


--aumenta la lunghezza della colonna VAR_DESCRIZIONE da varchar(256) a 
--varchar(512)

ALTER TABLE [@dbuser].[DPA_CHIAVI_CONFIGURAZIONE]
 alter column VAR_DESCRIZIONE   varchar(512)


--Dimitri de filippo 22/10/2010
--creazione tabella template per le chiavi di configurazione non globali

IF NOT EXISTS (select * from sysobjects where name = 'DPA_CHIAVI_CONFIG_TEMPLATE' and type = 'U')
begin

CREATE TABLE [@db_user].[DPA_CHIAVI_CONFIG_TEMPLATE](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	
	[VAR_CODICE] [varchar](32) NOT NULL,
	[VAR_DESCRIZIONE] [varchar](512) NULL,
	[VAR_VALORE] [varchar](128) NOT NULL,
	[CHA_TIPO_CHIAVE] [char](1) NULL,
	[CHA_VISIBILE] [char](1) NOT NULL default '1',
	[CHA_MODIFICABILE] [char](1) NOT NULL default '1',
	
	
PRIMARY KEY CLUSTERED 
(
	[SYSTEM_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) 
end

GO





 
IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_NOTE')
BEGIN
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE (    VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) VALUES ( 'FE_MAX_LENGTH_NOTE', N'Chiave utilizzata per impostazione del numero massimo di caratteri digitabili nei campi Note ', '2000', 'F', '1', '1')
END;

IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_OGGETTO')
BEGIN
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE (   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) VALUES (  N'FE_MAX_LENGTH_OGGETTO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto ', N'2000', N'F', N'1', N'1')
END;

IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_FASC')
begin
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE (   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) VALUES ( N'FE_MAX_LENGTH_DESC_FASC', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Fascicolo ', N'2000', N'F', N'1', N'1')
end

IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_TRASM')
begin
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) VALUES ( N'FE_MAX_LENGTH_DESC_TRASM', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Trasmissione ', N'2000', N'F', N'1', N'1')
end

IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_MAX_LENGTH_DESC_ALLEGATO')
BEGIN
INSERT into [@db_user].DPA_CHIAVI_CONFIGURAZIONE ( VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) VALUES ( N'FE_MAX_LENGTH_DESC_ALLEGATO', N'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Allegato ', N'200', N'F', N'1', N'1')
END

IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_RICEVUTA_PROTOCOLLO_PDF')
BEGIN
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) VALUES ( N'FE_RICEVUTA_PROTOCOLLO_PDF', N'Chiave per l''impostazione del formato di stampa della ricevuta di un protocollo. Valori: 1= PDF; 0=stampa tramite activex ', N'0', N'F', N'1', N'1')
END


IF NOT EXISTS(SELECT * FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE WHERE VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
INSERT into [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE ( VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) VALUES('FE_ABILITA_GEST_DOCS_ST_FINALE', 'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE ', '1','F', '0', '0')
END


IF NOT EXISTS(SELECT * FROM [@db_user].[DPA_VOCI_MENU_ADMIN] WHERE VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE')
BEGIN
INSERT INTO [@db_user].[DPA_VOCI_MENU_ADMIN]([VAR_CODICE],[VAR_DESCRIZIONE],[VAR_VISIBILITA_MENU])VALUES('FE_ABILITA_GEST_DOCS_ST_FINALE','GESTIONE DOCS STATO FINALE','')

END

--Dimitri de Filippo
--Stored che scorre la tabella DPA_CHIAVI_CONFIG_TEMPLATE e 
--inserisce le corrispondenti chiavi non globali nella tabella DPA_CHIAVI_CONFIGURAZIONE per ciascuna amministrazione

if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[CREA_KEYS_AMMINISTRA]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[CREA_KEYS_AMMINISTRA]
GO
CREATE  PROCEDURE [@db_user].[CREA_KEYS_AMMINISTRA] AS

BEGIN TRY 

BEGIN TRANSACTION 



DECLARE @sysCurrAmm INT;
declare @ErrorMessage varchar(100);
BEGIN


DECLARE currAmm -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
CURSOR LOCAL FOR
SELECT system_id
FROM [@db_user].DPA_AMMINISTRA

OPEN currAmm
FETCH NEXT FROM currAmm
INTO @sysCurrAmm

WHILE @@FETCH_STATUS = 0

BEGIN

---cursore annidato x le chiavi di configurazione

DECLARE @sysCurrKey varchar(32);
BEGIN


DECLARE currKey -- CURSORE CHE SCORRE LE CHIAVI CON ID AMMINISTRAZIONE A NULL
CURSOR LOCAL FOR
SELECT var_codice
FROM [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE


OPEN currKey
FETCH NEXT FROM currKey
INTO @sysCurrKey

WHILE @@FETCH_STATUS = 0
BEGIN

	if not exists (select * from [@db_user].DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE=@sysCurrKey and ID_AMM =@sysCurrAmm)
		begin
		insert into [@db_user].DPA_CHIAVI_CONFIGURAZIONE
		(ID_AMM, 
		VAR_CODICE, 
		VAR_DESCRIZIONE, 
		VAR_VALORE, 
		CHA_TIPO_CHIAVE, 
		CHA_VISIBILE, 
		CHA_MODIFICABILE, 
		CHA_GLOBALE 
		)
		(select top 1
		@sysCurrAmm as ID_AMM,
		 VAR_CODICE, 
		VAR_DESCRIZIONE, 
		VAR_VALORE, 
		CHA_TIPO_CHIAVE, 
		CHA_VISIBILE, 
		CHA_MODIFICABILE, 
		'0'

		from [@db_user].DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE=@sysCurrKey)
		end
--end

FETCH NEXT FROM currKey
INTO @sysCurrKey
END
CLOSE currKey
DEALLOCATE currKey
END


--- fine cursore annidato per chiavi di configurazione
FETCH NEXT FROM currAmm
INTO @sysCurrAmm
END
CLOSE currAmm
DEALLOCATE currAmm
END



COMMIT TRAN 

END TRY

BEGIN CATCH

IF @@TRANCOUNT > 0
--raiserror(@errormessage,10,1,'errore nella procedura di inserimento delle chiavi di configurazione relative all''amministrazione')

ROLLBACK TRAN 



END CATCH




SET QUOTED_IDENTIFIER OFF

----fine nuova stored


---------------------------------fine chiavi configurazione

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='MASSIVE_INOLTRA')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('MASSIVE_INOLTRA' , 'Abilita l''utente a compiere inoltri massivi', NULL, 'N')
END
GO

if not exists(select * from syscolumns where name='LDAP_AUTHENTICATED' and id in 
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].[PEOPLE] ADD LDAP_AUTHENTICATED CHAR(1) NULL
END
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[setsecurityRuoloReg]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[setsecurityRuoloReg]
GO
create  PROCEDURE [@db_user].[setsecurityRuoloReg]
@idCorrGlobali int,
@idProfile int,
@diritto int,
@Idreg int

AS
DECLARE @esito int
DECLARE @idGruppo int

BEGIN
SET @esito = -1
SET @idGruppo = (select id_Gruppo from dpa_corr_globali where system_id = @idCorrGlobali)

IF (@idGruppo IS NOT NULL)
BEGIN
SET @esito = (select max(accessrights) from security where thing = @idProfile and personorgroup = @idGruppo)
IF (@esito < @diritto )
BEGIN
update security set accessrights = @diritto where thing = @idProfile and personorgroup = @idGruppo
END
ELSE
BEGIN
if (@esito is null)
BEGIN
insert into security values(@idProfile,@idGruppo,@diritto,null,'A')
SET  @esito = @diritto
END
END
END
END


insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )  
select SYSTEM_ID,@idGruppo,@diritto,null,'A' from PROFILE p where ID_REGISTRO=@Idreg
and num_proto is not null
and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
s1.PERSONORGROUP=@idGruppo and s1.ACCESSRIGHTS=@diritto )

RETURN @esito

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_MODELLI_DELEGA' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_MODELLI_DELEGA](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_PEOPLE_DELEGANTE] [int] NOT NULL,
	[ID_RUOLO_DELEGANTE] [int] NULL,
	[ID_PEOPLE_DELEGATO] [int] NOT NULL,
	[ID_RUOLO_DELEGATO] [int] NOT NULL,
	[INTERVALLO] [int] NULL,
	[DTA_INIZIO] [datetime] NULL,
	[DTA_FINE] [datetime] NULL,
	[NOME] [varchar](100) NULL,
 CONSTRAINT [PK_DPA_MODELLI_DELEGA] PRIMARY KEY CLUSTERED 
(
	[SYSTEM_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM]
	ADD TIPO_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_OBJ_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM]
	ADD TIPO_OBJ_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD TIPO_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='TIPO_OBJ_LINK' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC]
	ADD TIPO_OBJ_LINK VARCHAR(50)
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO] WHERE UPPER(DESCRIZIONE) = 'LINK')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO] (TIPO,DESCRIZIONE) VALUES ('Link','Link')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_TIPO_OGGETTO_FASC] WHERE UPPER(DESCRIZIONE) = 'LINK')
BEGIN
	INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC] (TIPO,DESCRIZIONE) VALUES ('Link','Link')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_MANTIENI_LETTURA' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_MODELLI_TRASM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_MODELLI_TRASM] ADD
	CHA_MANTIENI_LETTURA char(1) NULL
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='CHA_MANTIENI_LETT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_RAGIONE_TRASM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_RAGIONE_TRASM] ADD
	CHA_MANTIENI_LETT char(1) NULL
END
GO

if exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_ANAGRAFICA_LOG SET VAR_CODICE = 'PUT_FILE' where VAR_CODICE = 'GET_FIRMA';
END;
GO

if exists (select * from [@db_user].[DPA_LOG] where VAR_COD_AZIONE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_LOG SET VAR_COD_AZIONE = 'PUT_FILE' where VAR_COD_AZIONE = 'GET_FIRMA';
END;
GO

if exists (select * from [@db_user].[DPA_LOG_STORICO] where VAR_COD_AZIONE ='GET_FIRMA')
BEGIN
	UPDATE [@db_user].DPA_LOG_STORICO SET VAR_COD_AZIONE = 'PUT_FILE' where VAR_COD_AZIONE = 'GET_FIRMA';
END;
GO

IF NOT EXISTS (select * from sysobjects where name = 'DPA_DISSERVIZI' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_DISSERVIZI] (
  [SYSTEM_ID]           [int]    NOT NULL IDENTITY (1, 1) PRIMARY KEY,
  [STATO]				[varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_NOTIFICA]      [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_EMAIL_NOTIFICA][varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_PAG_CORTESIA]  [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [TESTO_EMAIL_RIPRESA] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [NOTIFICATO]          [varchar] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)ON [PRIMARY]
end
GO

if not exists (SELECT * FROM syscolumns WHERE name='ACCETTAZIONE_DISSERV' and id in 
	(SELECT id FROM sysobjects WHERE name='PEOPLE' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PEOPLE] ADD ACCETTAZIONE_DISSERV VARCHAR(1)
END;
GO


-- aggiunta del campo luogo di nascita nelle info del corrispondente
if not exists (SELECT * FROM syscolumns WHERE name='VAR_LUOGO_NASCITA' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [docsadm].[DPA_DETT_GLOBALI] ADD VAR_LUOGO_NASCITA VARCHAR(128);
	
END;

GO

-- aggiunta del campo titolo nelle info del corrispondente
if not exists (SELECT * FROM syscolumns WHERE name='VAR_TITOLO' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	
	ALTER TABLE [docsadm].[DPA_DETT_GLOBALI] ADD VAR_TITOLO VARCHAR(64);	
END;

GO

-- aggiunta del campo data di nascita nelle info del corrispondente
if not exists (SELECT * FROM syscolumns WHERE name='DTA_NASCITA' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN
	
	ALTER TABLE [docsadm].[DPA_DETT_GLOBALI] ADD DTA_NASCITA VARCHAR(64);	
END;

GO


-- tabella contenente le qualifiche dei corrispondenti


IF NOT EXISTS (select * from sysobjects where name = 'DPA_QUALIFICA_CORRISPONDENTE' and type = 'U')
begin
CREATE TABLE [docsadm].[DPA_QUALIFICA_CORRISPONDENTE] (
 [SYSTEM_ID] [INT] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
 [VAR_TITOLO] [VARCHAR](64) NULL,
 [DTA_FINE_VALIDITA] [DATE] NULL
)
end
GO

-- chiave esterna tra la tabella dei titoli 
if not exists (SELECT * FROM syscolumns WHERE name='ID_QUALIFICA_CORR' and id in (SELECT id FROM sysobjects WHERE name='DPA_DETT_GLOBALI' and xtype='U'))
BEGIN

	ALTER TABLE [docsadm].[DPA_DETT_GLOBALI]  ADD ID_QUALIFICA_CORR INT
	
	ALTER TABLE [docsadm].[DPA_DETT_GLOBALI] ADD FOREIGN KEY (ID_QUALIFICA_CORR) REFERENCES [docsadm].[DPA_QUALIFICA_CORRISPONDENTE](SYSTEM_ID)
	
END;
GO


-- popolamento tabella titoli
BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Arch.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Avv.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dott.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dott.ssa');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Dr.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Geom.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Ing.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Mo.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Mons.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'on.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Prof.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Prof.ssa');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Rag.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Rev.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Arch.');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.na');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.ra');

END

BEGIN

INSERT INTO [DOCSADM].DPA_QUALIFICA_CORRISPONDENTE(VAR_TITOLO) VALUES ( 'Sig.ra/sig.na');

END
Begin
INSERT INTO [@db_user].DPA_CHIAVI_CONFIGURAZIONE
           (ID_AMM
           ,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE
           ,CHA_VISIBILE
           ,CHA_MODIFICABILE
           ,CHA_GLOBALE
           )
     VALUES
           (0
           , 'FE_TIMER_DISSERVIZIO'
           , 'Configurazione Timer DIsservizio, valore corrispondente a intervallo del Timer in millisecondi'
           ,'60000'
           ,'F'
           ,'1'
           ,'1'
           ,'0'
           )
end
go

if not exists(select * from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='EXP_FASC_COUNT')
BEGIN
	INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('EXP_FASC_COUNT' , 'Abilita l''utente ai report dei fascicoli', NULL, 'N')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_RUOLO_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD ID_RUOLO_CHIUSURA INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_UO_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD ID_UO_CHIUSURA INT;	
END;

GO

if not exists (SELECT * FROM syscolumns WHERE name='ID_AUTHOR_CHIUSURA' and id in (SELECT id FROM sysobjects WHERE name='PROJECT' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[PROJECT] ADD ID_AUTHOR_CHIUSURA INT;	
END;

GO
go

if not exists(select * from syscolumns where name='FILE_TYPE_SIGNATURE' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_SIGNATURE INT NULL
END
GO

if not exists(select * from syscolumns where name='FILE_TYPE_PRESERVATION' and id in 
(select id from sysobjects where name='DPA_FORMATI_DOCUMENTO' and xtype='U'))
BEGIN
ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_PRESERVATION INT NULL
END
GO

//Corte dei Conti
if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO] where DESCRIZIONE = 'ContatoreSottocontatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO](TIPO, DESCRIZIONE) VALUES ('ContatoreSottocontatore', 'ContatoreSottocontatore')
END
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO_FASC] where DESCRIZIONE = 'ContatoreSottocontatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC](TIPO, DESCRIZIONE) VALUES ('ContatoreSottocontatore', 'ContatoreSottocontatore')
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MODULO_SOTTOCONTATORE' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM_FASC] ADD MODULO_SOTTOCONTATORE INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='MODULO_SOTTOCONTATORE' and id in (SELECT id FROM sysobjects WHERE name='DPA_OGGETTI_CUSTOM' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_OGGETTI_CUSTOM] ADD MODULO_SOTTOCONTATORE INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_CONTATORI_DOC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CONTATORI_DOC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_CONTATORI_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CONTATORI_FASC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='VALORE_SC' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC] ADD VALORE_SC INT
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_INS' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] ADD DTA_INS DATETIME
END;
GO

if not exists (SELECT * FROM syscolumns WHERE name='DTA_INS' and id in (SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC] ADD DTA_INS DATETIME
END;
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO] where DESCRIZIONE = 'Separatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO](TIPO, DESCRIZIONE) VALUES ('Separatore', 'Separatore')
END
GO

if (not exists (select * from [@db_user].[DPA_TIPO_OGGETTO_FASC] where DESCRIZIONE = 'Separatore'))
BEGIN
INSERT INTO [@db_user].[DPA_TIPO_OGGETTO_FASC](TIPO, DESCRIZIONE) VALUES ('Separatore', 'Separatore')
END
GO
//Fine Corte dei Conti

if not exists (SELECT * FROM syscolumns WHERE name='CODICE_DB' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES]
	ADD CODICE_DB VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MANUAL_INSERT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES]
	ADD MANUAL_INSERT INT
END

if not exists (SELECT * FROM syscolumns WHERE name='CODICE_DB' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC]
	ADD CODICE_DB VARCHAR(50)
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='MANUAL_INSERT' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_ASS_TEMPLATES_FASC' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_ASS_TEMPLATES_FASC]
	ADD MANUAL_INSERT INT
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='PATH_MOD_SU' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_TIPO_ATTO]
	ADD PATH_MOD_SU VARCHAR(255)
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_ANAGRAFICA_FUNZIONI] WHERE UPPER(COD_FUNZIONE) = 'GEST_SU')
BEGIN
	INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED) VALUES ('GEST_SU', 'Abilita il sottomenu Stampa unione del menu Gestione', null,'N')
END
GO

if not exists (SELECT * FROM [@db_user].[DPA_ANAGRAFICA_FUNZIONI] WHERE UPPER(COD_FUNZIONE) = 'MASSIVE_REMOVE_VERSIONS')
BEGIN
	INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED) VALUES ('MASSIVE_REMOVE_VERSIONS', 'Abilita la rimozione massiva delle versioni dei doc grigi', null,'N')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_DETTAGLIO_TRASM_TODOLIST' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_DETTAGLIO_TRASM_TODOLIST', 'Utilizzata per visualizzare l intero dettaglio della trasmissione dalla todolist', '1',
    'F','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_EXPORT_DA_MODELLO' and ID_AMM IS NULL))
Begin
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           (ID_AMM
           ,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE
           ,CHA_VISIBILE
           ,CHA_MODIFICABILE
           ,CHA_GLOBALE
           )
     VALUES
           (0
           , 'FE_EXPORT_DA_MODELLO'
           , 'Abilitazione export ricerca da modello (0=disabilitato, 1= abilitato)'
           ,'1'
           ,'F'
           ,'1'
           ,'1'
           ,'0'
           )
 end
 GO 
 
if not exists (SELECT * FROM syscolumns WHERE name='Path_Mod_Exc' and id in
       (SELECT id FROM sysobjects WHERE name='DPA_TIPO_ATTO' and xtype='U'))
BEGIN
       ALTER TABLE [@db_user].[DPA_TIPO_ATTO]
       ADD Path_Mod_Exc VARCHAR(255)
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_ESTRAZIONE_LOG' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_ESTRAZIONE_LOG', 'Utilizzata per abilitare estrazione dei log da amministrazione', '1',
    'F','1','1', '1')
END
GO

if (not exists (select * from [DOCSADM].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_CONSOLIDAMENTO' and ID_AMM IS NULL))
BEGIN
    insert into DPA_CHIAVI_CONFIGURAZIONE
    (
    id_amm,
    var_codice,
    var_descrizione,
    var_valore,
    cha_tipo_chiave,
    cha_visibile,
    cha_modificabile,
    cha_globale,
    var_codice_old_webconfig
    )
    values 
    (
    0,
    'BE_CONSOLIDAMENTO',
    'Abilitazione della funzione di consolidamento dello stato di un documento',
    '1',
    'B',
    '1',
    '1',
    '1',
    null
    );
END 
GO

if (not exists (select * from [DOCSADM].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='CONSOLIDADOCUMENTO'))
BEGIN
	insert into dpa_anagrafica_log
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'CONSOLIDADOCUMENTO',
	'Azione di consolidamento dello stato di un documento',
	'DOCUMENTO',
	'CONSOLIDADOCUMENTO'
	);
END
GO  

if (not exists (select * from [DOCSADM].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_CONSOLIDAMENTO'))
BEGIN
	INSERT INTO DPA_ANAGRAFICA_FUNZIONI 
	VALUES('DO_CONSOLIDAMENTO', 'Abilita il consolidamento di un documento', '', 'N');
END 
GO


if (not exists (select * from [DOCSADM].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_CONSOLIDAMENTO_METADATI'))
BEGIN
	INSERT INTO DPA_ANAGRAFICA_FUNZIONI 
	VALUES('DO_CONSOLIDAMENTO_METADATI', 'Abilita il consolidamento dei metadati di un documento', '', 'N');
END 
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_STATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CONSOLIDATION_STATE CHAR(1) NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_AUTHOR' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CONSOLIDATION_AUTHOR INT NULL
END
GO

if not exists(select * from syscolumns where name='CONSOLIDATION_ROLE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CONSOLIDATION_ROLE INT NULL
END
GO


if not exists(select * from syscolumns where name='CONSOLIDATION_DATE' and id in 
(select id from sysobjects where name='PROFILE' and xtype='U'))
BEGIN
ALTER TABLE PROFILE ADD CONSOLIDATION_DATE DATE NULL
END
GO

if not exists(select * from syscolumns where name='STATO_CONSOLIDAMENTO' and id in 
(select id from sysobjects where name='DPA_STATI' and xtype='U'))
BEGIN
ALTER TABLE DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL
END
GO

if not exists(select id from sysobjects where name='DPA_CONSOLIDATED_DOCS' and xtype='U')
BEGIN
	CREATE TABLE DPA_CONSOLIDATED_DOCS
	(
	ID                       int,
	DOCNAME                  nvarchar(240),
	CREATION_DATE            datetime,
	DOCUMENTTYPE             int,
	AUTHOR                   int,
	AUTHOR_NAME              nvarchar(4000),
	ID_RUOLO_CREATORE        int,
	RUOLO_CREATORE           nvarchar(4000),
	NUM_PROTO                int,
	NUM_ANNO_PROTO           int,
	DTA_PROTO                datetime,
	ID_PEOPLE_PROT           int,
	PEOPLE_PROT              nvarchar(4000),
	ID_RUOLO_PROT            int,
	RUOLO_PROT               nvarchar(4000),
	ID_REGISTRO              int,
	REGISTRO                 nvarchar(4000),
	CHA_TIPO_PROTO           nvarchar(1),
	VAR_PROTO_IN             nvarchar(128),
	DTA_PROTO_IN             datetime,
	DTA_ANNULLA              datetime,
	ID_OGGETTO               int,
	VAR_PROF_OGGETTO         nvarchar(2000),
	MITT_DEST                nvarchar(4000),
	ID_DOCUMENTO_PRINCIPALE  int
	)     
END 
GO   

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='FE_TAB_TRASM_ALL' and ID_AMM IS NULL))
BEGIN
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
VALUES (0, 'FE_TAB_TRASM_ALL', 'Utilizzata per vedere dal tab classifica tutti i fascicoli', '1',
    'F','1','1', '1')
END
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='BE_URL_WA' and ID_AMM IS NULL))

BEGIN

INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE](ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,

    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)

VALUES (0, 'BE_URL_WA', 'Utilizzata per identificare il path del Front end per esportare il link tramite web service ', '1',

    'B','1','1', '1')

END

GO

if not exists(select * from syscolumns where name='HIDE_DOC_VERSIONS' and id in 
(select id from sysobjects where name='DPA_TRASM_SINGOLA' and xtype='U'))
BEGIN
ALTER TABLE DPA_TRASM_SINGOLA ADD HIDE_DOC_VERSIONS CHAR(1) NULL
END
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[isVersionVisible]') AND xtype in (N'FN', N'IF', N'TF'))
	DROP  function [@db_user].[isVersionVisible] 
GO
	CREATE FUNCTION [@db_user].[isVersionVisible]
	(
	@versionId INT,
	@idpeople INT,
	@idgroup INT
	)
	RETURNS INT
	AS

	BEGIN

	DECLARE @retValue INT
	DECLARE @idProfile INT 
	DECLARE @maxVersionId INT 
	DECLARE @hideVersions INT 
	DECLARE @ownership INT 

	-- 1) Reperimento IdProfile e DocNumber del documento
	select  @idProfile = p.system_id
	from    versions v 
			inner join profile p on v.docnumber = p.docnumber
	where   v.version_id = @versionId

	-- 2) verifica se la versione richiesta è l'ultima
	select @maxVersionId = max(v.version_id)
	from versions v
	where v.docnumber = (select docnumber from profile where system_id = @idProfile)

	if (@maxVersionId = @versionId)
		begin
			-- 2a.) Il versionId si riferisce all'ultima versione del documento, è sempre visibile
			set @retValue = 1
		end
	else
		begin
			-- 3) verifica se il documento è stato trasmesso a me o al mio ruolo e 
			-- se tale trasmissione prevede le versioni precedenti nascoste 
			select @hideVersions = count(*)
			from dpa_trasmissione t
				  inner join dpa_trasm_singola ts on t.system_id = ts.id_trasmissione
				where t.id_profile = @idProfile
				and ts.id_corr_globale in 
					(
						(select system_id from dpa_corr_globali where id_people = @idPeople), 
						(select system_id from dpa_corr_globali where id_gruppo = @idGroup)
					)
				and ts.hide_doc_versions = '1'
			
				if (@hideVersions > 0)
				begin
						-- 4) verifica in sercurity se sul doc non dispongo dei diritti di ownership 
						-- (trasmissione a me stesso) oppure abbia già acquisito i diritti di visibilità
						-- (es. superiore gerarchico)
						select @ownership = count(*) 
						from security s 
						where thing = @idProfile
								and personorgroup in (@idPeople, @idGroup)
								and (cha_tipo_diritto = 'P' or cha_tipo_diritto = 'A');
				        
						if (@ownership = 0) 
						begin
								-- Sul documento non si dispongono i diritti di ownership,
								-- pertanto la versione deve essere nascosta 
								set @retValue = 0
						end
						else
						begin
								-- Sul documento si dispongono già dei diritti di ownership,
								-- pertanto la versione non deve essere nascosta        
							set @retValue = 1
						end

				end 
				else
				begin
						-- 3a) la tx non prevede di nascondere le versioni, quindi la versione è sempre visibile
					set @retValue = 1
				end
		end

	RETURN @retValue

	END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='DO_AMM_CONSERVAZIONE_WA'))
BEGIN
	INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
	VALUES('DO_AMM_CONSERVAZIONE_WA', 'Permette l''amministrazione della WA di Conservazione', '', 'N');
END 
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[DPA_GRIDS]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_GRIDS](
	[SYSTEM_ID] [int] NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[USER_ID] [int],
	[ROLE_ID] [int],
	[ADMINISTRATION_ID] [int],
	[SEARCH_ID] [int],
	[SERIALIZED_GRID] [text],
    [TYPE_GRID] [VARCHAR](30)
)
END
GO

if not exists(select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] where COD_FUNZIONE='STAMPA_REG_NO_SEC')

BEGIN

            INSERT INTO [@db_user].[DPA_ANAGRAFICA_FUNZIONI] (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 

        VALUES ('STAMPA_REG_NO_SEC' , 'Abilita utente a stampare registri senza controllo sulla sicurezza', NULL, 'N')

END

GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOTIMESTAMP'))
BEGIN
	insert into [@db_user].[dpa_anagrafica_log]
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOTIMESTAMP',
	'Marca Temporale del documento',
	'DOCUMENTO',
	'DOCUMENTOTIMESTAMP'
	);
END
GO 

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOCONVERSIONEPDF'))
BEGIN
	insert into [@db_user].[dpa_anagrafica_log]
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOCONVERSIONEPDF',
	'Conversione in pdf del documento',
	'DOCUMENTO',
	'DOCUMENTOCONVERSIONEPDF'
	);
END
GO  

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG] where VAR_CODICE='DOCUMENTOCONSERVAZIONE'))
BEGIN
	insert into [@db_user].[dpa_anagrafica_log]
	(
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	'DOCUMENTOCONSERVAZIONE',
	'Invio in conservazione del documento',
	'DOCUMENTO',
	'DOCUMENTOCONSERVAZIONE'
	);
END
GO  

Insert into [@db_user].[DPA_DOCSPA]
   ( DTA_UPDATE, ID_VERSIONS_U)
 Values
   ( getdate(), '3.12.0')
GO

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where colonna='BE_SALVA_EMAIL_IN_LOCALE'))
BEGIN
  insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] 
   (ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values 
  (0,'BE_SALVA_EMAIL_IN_LOCALE','Abilita il salvatggio delle mail in locale ( 1 attivo 

0 no)', '1','B','1','1','1');
END
GO
