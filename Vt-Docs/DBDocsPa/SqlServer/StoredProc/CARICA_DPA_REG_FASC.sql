

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create PROCEDURE [@db_user].[CARICA_DPA_REG_FASC] AS

DECLARE @sysCurrTitolario int
DECLARE @sysCurrRegistro INT
DECLARE @countFasc int
DECLARE @sysCurrAmm int
DECLARE @sysCurrRegistroNodoTit INT


BEGIN


DECLARE currAmm -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
CURSOR LOCAL FOR
SELECT system_id
FROM @db_user.DPA_AMMINISTRA

OPEN currAmm
FETCH NEXT FROM currAmm
INTO @sysCurrAmm


WHILE @@FETCH_STATUS = 0
BEGIN

DECLARE currTit -- CURSORE CHE SCORRE I NODI DI TITOLARIO
CURSOR LOCAL FOR
SELECT system_id, id_registro
FROM @db_user.PROJECT
WHERE CHA_TIPO_PROJ = 'T'
AND ID_AMM = @sysCurrAmm order by id_registro

OPEN currTit
FETCH NEXT FROM currTit
INTO @sysCurrTitolario, @sysCurrRegistroNodoTit

WHILE @@FETCH_STATUS = 0
BEGIN

-- si il modo di titolario ha id_registro NULL allora nella tabella dpa_reg_fasc
--dovranno essere inseriti tanti record quanti sono i registri contentuti nella dpa_el_registri

IF (@sysCurrRegistroNodoTit IS NULL)

BEGIN

DECLARE currReg -- CURSORE CHE SCORRE I REGISTRI
CURSOR LOCAL FOR
SELECT system_id
FROM @db_user.DPA_EL_REGISTRI
WHERE ID_AMM = @sysCurrAmm and cha_rf= '0';

OPEN currReg
FETCH NEXT FROM currReg INTO @sysCurrRegistro

WHILE @@FETCH_STATUS = 0

BEGIN
-- Si calcola il NUMERO DEL FASCICOLO relativo al nodo corrente	SUL REGISTRO CORRENTE
SET @countFasc = (SELECT MAX(NUM_FASCICOLO)
FROM @db_user.PROJECT
WHERE ID_PARENT = @sysCurrTitolario AND CHA_TIPO_FASCICOLO = 'P' AND ANNO_CREAZIONE = YEAR(GETDATE())
and ID_REGISTRO = @sysCurrRegistro)

IF (@countFasc IS NULL)
SET @countFasc = 0

BEGIN
INSERT INTO DPA_REG_FASC
(
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
@sysCurrTitolario,
@sysCurrRegistro,
@countFasc+1 -- QUI  mettiamo il MAX (NUM FASCICOLO) - dafault a 0 per il pregresso

)

FETCH NEXT FROM currReg INTO @sysCurrRegistro

END

END

CLOSE currReg
DEALLOCATE currReg

-- CASO DEL REGISTRO NULL

SET @countFasc = (SELECT MAX(NUM_FASCICOLO)
FROM @db_user.PROJECT
WHERE ID_PARENT = @sysCurrTitolario AND CHA_TIPO_FASCICOLO = 'P' AND ANNO_CREAZIONE = YEAR(GETDATE())
and ID_REGISTRO IS NULL)

IF (@countFasc IS NULL)
SET @countFasc = 0

INSERT INTO DPA_REG_FASC
(
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
@sysCurrTitolario,
NULL,
@countFasc+1 -- QUI mettiamo IL MAX (NUM FASCICOLO)

)
END

ELSE  -- SE IL NODO ? ASSOCIATO AD UN SOLO REGISTRO

BEGIN

SET @countFasc = (SELECT MAX(NUM_FASCICOLO)
FROM @db_user.PROJECT
WHERE ID_PARENT = @sysCurrTitolario AND CHA_TIPO_FASCICOLO = 'P' AND ANNO_CREAZIONE = YEAR(GETDATE())
and ID_REGISTRO= @sysCurrRegistroNodoTit)

IF (@countFasc IS NULL)
SET @countFasc = 0

INSERT INTO DPA_REG_FASC
(
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
@sysCurrTitolario,
@sysCurrRegistroNodoTit,
@countFasc+1

)
END

FETCH NEXT FROM currTit
INTO @sysCurrTitolario, @sysCurrRegistroNodoTit

END
CLOSE currTit
DEALLOCATE currTit

END
FETCH NEXT FROM currAmm
INTO @sysCurrAmm
CLOSE currAmm
DEALLOCATE currAmm
END


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO