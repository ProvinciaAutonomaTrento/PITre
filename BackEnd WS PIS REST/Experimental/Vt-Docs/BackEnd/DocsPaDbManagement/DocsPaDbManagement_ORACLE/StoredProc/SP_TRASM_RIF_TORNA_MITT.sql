CREATE OR REPLACE PROCEDURE Sp_Trasm_Rif_Torna_Mitt (
IDPeopleMitt IN NUMBER,
IDRuoloGlobMitt IN NUMBER,
IDAmministrazioneMittente IN NUMBER,
IDTrasmUtente IN NUMBER,
ReturnValue OUT NUMBER)	IS

/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione del rifiuto di trasmissioni ricevute.
-- La trasmissione rifiuatata torna al mittente (tramite una nuova trasmissione utente)
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- da 1 a 9: Errore generico
-------------------------------------------------------------------------------------------------------
*/

IdentityTrasm NUMBER := NULL;
IdentityTrasmSing NUMBER := NULL;

IDRagioneTrasm NUMBER := NULL;
TipoOggetto CHAR(1) := NULL;
IDProfile NUMBER := NULL;
IDProject NUMBER := NULL;
IDPeopleDest NUMBER := NULL;
IDCorrGlobaleDestinatario NUMBER := NULL;
IDCorrGlobaleMittente NUMBER := NULL;
NomeCognomeDest VARCHAR2(128) := NULL;
IDUtente NUMBER := NULL;
CHAValida VARCHAR2(3) := NULL;
NoteRif Varchar2(250):=' ';
recordCorrente NUMBER;

/* Prende gli utenti del ruolo del mittente*/
CURSOR cursor_IDUtenti (ID_CORR_GLOB_RUOLO NUMBER) IS
SELECT 	P.SYSTEM_ID
FROM 	GROUPS G,
PEOPLEGROUPS PG,
PEOPLE P,
DPA_CORR_GLOBALI CG
WHERE 	PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND
PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND
G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = ID_CORR_GLOB_RUOLO) AND
P.DISABLED NOT IN ('Y') AND
P.SYSTEM_ID=CG.ID_PEOPLE AND CG.CHA_TIPO_URP != 'L'
AND CG.DTA_FINE IS NULL AND PG.DTA_FINE IS NULL;

BEGIN

/*

Se una trasmissione viene rifiutata, questa torna al mittente.
Infatti viene effettuata una nuova trasmissione verso il mittente.

Si necessita dei seguenti dati:

+++ per la tabella dpa_trasmissione +++
- ruolo dell'utente mittente (¿ passato come input: [def. IDRuoloGlobMitt])
- system_id della PEOPLE dell'utente mittente (¿ passato come input: [def. IDPeopleMitt])
- del tipo di oggetto rifiutato [def. TipoOggetto]
- della system_id della profile se il tipo oggetto ¿ "documento" [def. IDProfile]
- della system_id della project se il tipo oggetto ¿ "fascicolo" [def. IDProject]

+++ per la tabella dpa_trasm_singola +++
- system_id della ragione di trasmissione RIFIUTO [def. IDRagioneTrasm]
- system_id del ruolo della corr_globali del destinatario [def. IDCorrGlobaleDestinatario]

+++ per la tabella dpa_trasm_utente +++
- system_id della people del destinatario [def. IDPeopleDest]

*/

BEGIN
-- verifica se esiste la ragione di trasmissione RIFIUTO
SELECT SYSTEM_ID INTO IDRagioneTrasm
FROM 	DPA_RAGIONE_TRASM
WHERE 	ID_AMM = IDAmministrazioneMittente
AND
VAR_DESC_RAGIONE = 'RIFIUTO';
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 1;
RETURN;
END;


BEGIN
-- prende il tipo di oggetto rifiutato ("documento" o "fascicolo")
SELECT
cha_tipo_oggetto INTO TipoOggetto
FROM
DPA_TRASMISSIONE
WHERE
system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = IDTrasmUtente));
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 2;
RETURN;
END;

BEGIN
-- prende la system_id della profile se il tipo oggetto ¿ "documento"
SELECT
id_profile INTO IDProfile
FROM
DPA_TRASMISSIONE
WHERE
system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = IDTrasmUtente));
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 3;
RETURN;
END;

BEGIN
-- prende la system_id della project se il tipo oggetto ¿ "fascicolo"
SELECT
id_project INTO IDProject
FROM
DPA_TRASMISSIONE
WHERE
system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = IDTrasmUtente));
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 4;
RETURN;
END;

BEGIN
-- prende la system_id della people del destinatario
SELECT
id_people INTO IDPeopleDest
FROM
DPA_TRASMISSIONE
WHERE
system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = IDTrasmUtente));
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 5;
RETURN;
END;

BEGIN
-- prende la system_id della dpa_corr_globali del ruolo del destinatario
SELECT
id_ruolo_in_uo INTO IDCorrGlobaleMittente
FROM
DPA_TRASMISSIONE
WHERE
system_id =
(SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id =
(SELECT id_trasm_singola FROM DPA_TRASM_UTENTE WHERE system_id = IDTrasmUtente));
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 6;
RETURN;
END;

BEGIN
-- prende nome e cognome nella dpa_corr_globali dell'utente che ha rifiutato
SELECT
var_desc_corr INTO NomeCognomeDest
FROM
DPA_CORR_GLOBALI
WHERE
ID_PEOPLE = IDPeopleMitt and cha_tipo_urp!='L';
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 6;
RETURN;
END;

BEGIN
SELECT seq.NEXTVAL INTO IdentityTrasm FROM dual;
END;

BEGIN
SELECT seq.NEXTVAL INTO IdentityTrasmSing FROM dual;
END;

BEGIN

SELECT var_note_rif into NoteRif
FROM DPA_TRASM_UTENTE
WHERE system_id = IDTrasmUtente;
-- Inserimento in tabella DPA_TRASMISSIONE
INSERT INTO DPA_TRASMISSIONE
(
SYSTEM_ID,
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
IdentityTrasm,
IDRuoloGlobMitt,
IDPeopleMitt,
''||TipoOggetto||'',
IDProfile,
IDProject,
SYSDATE(),
'Trasmissione rifiutata da '||REPLACE(NomeCognomeDest,'','''')||'e tornata al mittente.');

EXCEPTION
WHEN OTHERS THEN
ReturnValue := 7;
RETURN;
END;

BEGIN
-- Inserimento in tabella DPA_TRASM_SINGOLA
INSERT INTO DPA_TRASM_SINGOLA
(
SYSTEM_ID,
ID_RAGIONE,
ID_TRASMISSIONE,
CHA_TIPO_DEST,
ID_CORR_GLOBALE,
CHA_TIPO_TRASM,
var_note_sing
)
VALUES
(
IdentityTrasmSing,
IDRagioneTrasm,
IdentityTrasm,
'R',
IDCorrGlobaleMittente,
'S',
REPLACE(NoteRif,'','''')
);
EXCEPTION
WHEN OTHERS THEN
ReturnValue := 8;
RETURN;
END;

BEGIN
OPEN cursor_IDUtenti(IDCorrGlobaleMittente);
LOOP
FETCH cursor_IDUtenti INTO recordCorrente;
EXIT WHEN cursor_IDUtenti%NOTFOUND;
IDUtente := recordCorrente;

BEGIN

IF IDUtente = IDPeopleDest THEN
CHAValida := '1';
ELSE
CHAValida := '0';
END IF;

-- Inserimento in tabella DPA_TRASM_UTENTE
INSERT INTO DPA_TRASM_UTENTE
(
SYSTEM_ID,
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
seq.NEXTVAL,
IdentityTrasmSing,
IDUtente,
'0',
'0',
'0',
CHAValida,
REPLACE(NoteRif,'',''''),
sysdate
);
EXCEPTION
WHEN OTHERS THEN
ReturnValue := 9;
RETURN;
END;

END LOOP;
CLOSE cursor_IDUtenti;

--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio.!!!
update dpa_trasmissione set dta_invio=sysdate() where system_id=IdentityTrasm;
END;

ReturnValue := 0;
END;
/
