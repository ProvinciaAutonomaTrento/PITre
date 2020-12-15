

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create  PROCEDURE [@db_user].[Sp_Trasm_Rif_Torna_Mitt]
@IDPeopleMitt int,
@IDRuoloGlobMitt int,
@IDAmministrazioneMittente int,
@IDTrasmUtente int
AS

DECLARE @IDRagioneTrasm int
DECLARE @TipoOggetto varchar(1)
DECLARE @IDProfile int
DECLARE @IDProject int
DECLARE @IDPeopleDest int
DECLARE @IDCorrGlobaleDestinatario int
DECLARE @NomeCognomeDest varchar(128)
DECLARE @NoteRif varchar(250)
DECLARE @IDCorrGlobaleMittente int
DECLARE @CHAValida varchar(1)
declare @datenow varchar
DECLARE @Identity int
DECLARE @IdentityTrasm int

DECLARE @ReturnValue int

SET @IDRagioneTrasm =(SELECT 	SYSTEM_ID
FROM 	DPA_RAGIONE_TRASM
WHERE 	ID_AMM = @IDAmministrazioneMittente AND
VAR_DESC_RAGIONE = 'RIFIUTO')

IF (NOT @IDRagioneTrasm IS NULL)
BEGIN

SET @TipoOggetto = (SELECT 	cha_tipo_oggetto
FROM  	DPA_TRASMISSIONE
WHERE   	system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))
IF (@TipoOggetto IS NULL)
BEGIN
SET @ReturnValue=2
END

ELSE
BEGIN

SET @IDProfile = (SELECT	id_profile
FROM		DPA_TRASMISSIONE
WHERE 	system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))

SET @IDProject = (SELECT	id_project
FROM		DPA_TRASMISSIONE
WHERE		system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))

SET @IDPeopleDest = (SELECT	id_people
FROM	DPA_TRASMISSIONE
WHERE	system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))
IF (@IDPeopleDest IS NULL)
BEGIN
SET @ReturnValue=5
END

ELSE
BEGIN

SET @IDCorrGlobaleMittente  = (SELECT	id_ruolo_in_uo
FROM	DPA_TRASMISSIONE
WHERE	system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))
IF (@IDCorrGlobaleMittente IS NULL)
BEGIN
SET @ReturnValue=6
END

ELSE
BEGIN

SET @NomeCognomeDest = (SELECT	var_desc_corr
FROM	DPA_CORR_GLOBALI
WHERE	ID_PEOPLE = @IDPeopleMitt)
IF (@NomeCognomeDest IS NULL)
BEGIN
SET @ReturnValue=6
END

set @datenow= convert(varchar,getdate(),103)
SET @NoteRif = (SELECT var_note_rif
FROM DPA_TRASM_UTENTE
WHERE system_id = @IDTrasmUtente)


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
@IDRuoloGlobMitt,
@IDPeopleMitt,
@TipoOggetto,
@IDProfile,
@IDProject,
GETDATE(),
'Trasmissione rifiutata da '+REPLACE(@NomeCognomeDest,'','''')+' in data '+convert(varchar,getdate(),103)
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=7
END
ELSE
BEGIN
SET @Identity=scope_identity()
SET @IdentityTrasm  = @Identity

INSERT INTO DPA_TRASM_SINGOLA
(
ID_RAGIONE,
ID_TRASMISSIONE,
CHA_TIPO_DEST,
ID_CORR_GLOBALE,
CHA_TIPO_TRASM,
Var_note_sing
)
VALUES
(
@IDRagioneTrasm,
@Identity,
'R',
@IDCorrGlobaleMittente,
'S',
REPLACE(@NoteRif,'','''')
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=8
END
ELSE
BEGIN

SET @Identity=scope_identity()

DECLARE cursor_IDUtenti

CURSOR FOR

SELECT 	P.SYSTEM_ID
FROM 	GROUPS G,
PEOPLEGROUPS PG,
PEOPLE P,
DPA_CORR_GLOBALI CG
WHERE 	PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND
PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND
G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = @IDCorrGlobaleMittente) AND
P.DISABLED NOT IN ('Y') AND
P.SYSTEM_ID=CG.ID_PEOPLE AND CG.CHA_TIPO_URP != 'L'
AND CG.DTA_FINE IS NULL
AND PG.DTA_FINE IS NULL

OPEN cursor_IDUtenti

DECLARE @IDUtente int

FETCH NEXT FROM cursor_IDUtenti
INTO 	@IDUtente


WHILE @@FETCH_STATUS = 0
BEGIN

IF (@IDUtente = @IDPeopleDest)
BEGIN
SET @CHAValida = '1'
END
ELSE
BEGIN
SET @CHAValida = '0'
END

INSERT INTO DPA_TRASM_UTENTE
(
ID_TRASM_SINGOLA,
ID_PEOPLE,
CHA_VISTA,
CHA_ACCETTATA,
CHA_RIFIUTATA,
CHA_VALIDA,
var_note_rif,
dta_rifiutata
)
VALUES
(
@Identity,
@IDUtente,
'0',
'0',
'0',
@CHAValida,
null,
null
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=9
END

FETCH NEXT FROM cursor_IDUtenti
INTO 	@IDUtente

END

CLOSE cursor_IDUtenti
DEALLOCATE cursor_IDUtenti

UPDATE DPA_TRASMISSIONE SET DTA_INVIO = GETDATE() WHERE SYSTEM_ID = @IdentityTrasm

END
END

END
END
END
END
ELSE

BEGIN
SET @ReturnValue=1
END

RETURN @ReturnValue
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO