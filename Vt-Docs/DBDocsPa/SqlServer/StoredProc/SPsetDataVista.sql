SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create  PROCEDURE [@db_user].[SPsetDataVista]
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
and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople

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
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople

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
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = @sysTrasmSingola
and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople

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

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO