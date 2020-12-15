if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[CREATE_NEW_NODO_TITOLARIO]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[CREATE_NEW_NODO_TITOLARIO]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

setuser N'DOCSADM'
GO

CREATE  PROCEDURE  [DOCSADM].[CREATE_NEW_NODO_TITOLARIO]
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
WHERE ID_AMM = @idAmm
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
CHA_BLOCCA_FASC
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
@bloccaFascicolo
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
CHA_BLOCCA_FASC
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
@bloccaFascicolo
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
CHA_BLOCCA_FASC
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
@bloccaFascicolo
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

setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

