CREATE PROCEDURE DOCSADM.Sp_Trasm_Rif_Torna_Mitt
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

DECLARE @Identity int
DECLARE @IdentityTrasm int

DECLARE @ReturnValue int

SET @IDRagioneTrasm =(SELECT 	SYSTEM_ID
FROM 	DPA_RAGIONE_TRASM
WHERE 	ID_AMM = @IDAmministrazioneMittente AND
VAR_DESC_RAGIONE = 'RIFIUTO')

IF (NOT @IDRagioneTrasm IS NULL)
BEGIN
-- prende il tipo di oggetto rifiutato ("documento" o "fascicolo")
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
-- prende la system_id della profile se il tipo oggetto  "documento"
SET @IDProfile = (SELECT	id_profile
FROM		DPA_TRASMISSIONE
WHERE 	system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))

-- prende la system_id della project se il tipo oggetto  "fascicolo"
SET @IDProject = (SELECT	id_project
FROM		DPA_TRASMISSIONE
WHERE		system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = @IDTrasmUtente)))


-- prende la system_id della people del destinatario
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

-- prende la system_id della dpa_corr_globali del ruolo del destinatario
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

-- prende nome e cognome nella dpa_corr_globali dell'utente che ha rifiutato
SET @NomeCognomeDest = (SELECT	var_desc_corr
FROM	DPA_CORR_GLOBALI
WHERE	ID_PEOPLE = @IDPeopleMitt)
IF (@NomeCognomeDest IS NULL)
BEGIN
SET @ReturnValue=6
END


SET @NoteRif = (SELECT var_note_rif
FROM DPA_TRASM_UTENTE
WHERE system_id = @IDTrasmUtente)



-- Inserimento in tabella DPA_TRASMISSIONE
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
'Trasmissione rifiutata da '+REPLACE(@NomeCognomeDest,'','''')+'e tornata al mittente.'
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=7
END
ELSE
BEGIN
-- Reperimento identity appena immessa
SET @Identity=scope_identity()
SET @IdentityTrasm  = @Identity

-- Inserimento in tabella DPA_TRASM_SINGOLA
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

-- Reperimento identity appena immessa
SET @Identity=scope_identity()

-- Prende gli utenti del ruolo
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

-- Inserimento in tabella DPA_TRASM_UTENTE
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
REPLACE(@noterif,'',''''),
getdate()
)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=9
END

-- Posizionamento sul record successivo del cursore temporaneo
FETCH NEXT FROM cursor_IDUtenti
INTO 	@IDUtente

END


-- Chiusura e deallocazione cursore
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
