CREATE OR REPLACE PROCEDURE I_Smistamento_Smistadoc_U(
IDPeopleMittente IN NUMBER,
IDCorrGlobaleRuoloMittente IN NUMBER,
IDGruppoMittente IN NUMBER,
IDAmministrazioneMittente IN NUMBER,
IDCorrGlobaleDestinatario IN NUMBER,
IDDocumento IN NUMBER,
IDTrasmissione IN NUMBER,
FlagCompetenza IN CHAR,
IDTrasmissioneUtenteMittente IN NUMBER,
TrasmissioneConWorkflow IN CHAR,
NoteGeneraliDocumento IN VARCHAR2,
NoteIndividuali IN VARCHAR2,
DataScadenza IN DATE,
returnvalue OUT NUMBER) IS

/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Possibili ragione di trasmissione:
--     COMPETENZA (Accessrights = 63)
--    CONOSCENZA (Accessrights = 45)
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- 1: Non  presente la ragione trasmissione per l'amministrazione corrente (COMPETENZA,CONOSCENZA)
-- 2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- 3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- 4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/

DescrizioneRagione VARCHAR2(10) := NULL;
Rights NUMBER:= NULL;
IdentityTrasm NUMBER := NULL;
SysTrasmSing NUMBER := NULL;
TestoNote VARCHAR2(250) := NULL;


ExistIDRagioneTrasm CHAR(1) := 'Y';
IDRagioneTrasm NUMBER;
idRagTrasmValue NUMBER := NULL;

ExistAccessRights CHAR(1) := 'Y';
AccessRights NUMBER:= NULL;
accessRightsValue NUMBER := NULL;

IDUtente NUMBER;
recordCorrente NUMBER;

IDGroups NUMBER := NULL;
IDGruppo NUMBER;
resultValue number;

TipoTrasmSingola CHAR(1) := NULL;

/* Prende gli utenti del ruolo */
CURSOR cursor_IDUtenti (ID_CORR_GLOB_RUOLO NUMBER) IS
SELECT  P.SYSTEM_ID
FROM  GROUPS G,
PEOPLEGROUPS PG,
PEOPLE P,
DPA_CORR_GLOBALI CG
WHERE  PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND
PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND
G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = ID_CORR_GLOB_RUOLO) AND
P.DISABLED NOT IN ('Y') AND
P.SYSTEM_ID=CG.ID_PEOPLE
AND CG.CHA_TIPO_URP != 'L'
AND CG.DTA_FINE IS NULL
AND PG.DTA_FINE IS NULL;

BEGIN

/* Reperimento ID da tabella "DPA_RAGIONE_TRASM" */
IF FlagCompetenza = '1' THEN
DescrizioneRagione := 'COMPETENZA';
Rights := 63;
ELSE
DescrizioneRagione := 'CONOSCENZA';
Rights := 45;
END IF;

BEGIN
SELECT DISTINCT SYSTEM_ID INTO IDRagioneTrasm
FROM  DPA_RAGIONE_TRASM
WHERE  ID_AMM = IDAmministrazioneMittente
AND
VAR_DESC_RAGIONE = DescrizioneRagione;
EXCEPTION
WHEN NO_DATA_FOUND THEN
ExistIDRagioneTrasm := 'N';
END;

IF ExistIDRagioneTrasm = 'Y' THEN

idRagTrasmValue := IDRagioneTrasm;

BEGIN
SELECT seq.NEXTVAL INTO IdentityTrasm FROM dual;
END;

BEGIN
SELECT seq.NEXTVAL INTO SysTrasmSing FROM dual;
END;

BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
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
IDCorrGlobaleRuoloMittente,
IDPeopleMittente,
'D',
IDDocumento,
NULL,
SYSDATE(),
NoteGeneraliDocumento
);

EXCEPTION
			WHEN OTHERS THEN  returnvalue:= -2 ;
			RETURN;
END;

BEGIN
INSERT INTO DPA_TRASM_SINGOLA
(
SYSTEM_ID,
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
SysTrasmSing,
idRagTrasmValue,
IdentityTrasm,
'R',
IDCorrGlobaleDestinatario,
NoteIndividuali,
'S',
DataScadenza,
NULL
);

returnvalue := SysTrasmSing;

EXCEPTION
			WHEN OTHERS THEN  returnvalue:= -3 ;
			RETURN;
			
END;


-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
BEGIN
SELECT A.ID_GRUPPO INTO IDGroups
FROM DPA_CORR_GLOBALI A
WHERE A.SYSTEM_ID = IDCorrGlobaleDestinatario;
END;

IDGruppo := IDGroups;

BEGIN
SELECT ACCESSRIGHTS INTO AccessRights FROM (
SELECT  ACCESSRIGHTS
FROM  SECURITY
WHERE  THING = IDDocumento
AND
PERSONORGROUP = IDGruppo
AND
CHA_TIPO_DIRITTO = 'T'
) WHERE ROWNUM = 1;
EXCEPTION
WHEN NO_DATA_FOUND THEN
ExistAccessRights := 'N';
END;

IF ExistAccessRights = 'Y' THEN

accessRightsValue := AccessRights;

IF accessRightsValue < Rights THEN
BEGIN
/* aggiornamento a Rights */
UPDATE  SECURITY
SET  ACCESSRIGHTS = Rights
WHERE  THING = IDDocumento
AND
PERSONORGROUP = IDGruppo
AND
CHA_TIPO_DIRITTO = 'T'
AND ACCESSRIGHTS = accessRightsValue;
EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;
END IF;
ELSE
BEGIN
/* inserimento a Rights */
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
IDDocumento,
IDGruppo,
Rights,
IDGruppoMittente,
'T'
);
EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;
END IF;

/* Aggiornamento trasmissione del mittente */
IF TrasmissioneConWorkflow = '1' THEN
BEGIN
UPDATE  DPA_TRASM_UTENTE
SET
dta_vista = (case when dta_vista is null then sysdate else dta_vista end),
cha_vista  =  (case when dta_vista is null  then 1 else 0 end),
DTA_ACCETTATA = SYSDATE(),
CHA_ACCETTATA = '1',
VAR_NOTE_ACC = 'Documento accettato e smistato',
CHA_IN_TODOLIST = '0'
WHERE (SYSTEM_ID = IDTrasmissioneUtenteMittente
OR
SYSTEM_ID = (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
AND TS.CHA_TIPO_DEST= 'U')
)
AND CHA_VALIDA='1';
END;
ELSE
BEGIN

/*UPDATE  DPA_TRASM_UTENTE
SET  DTA_VISTA = SYSDATE(),
CHA_VISTA = '1',
CHA_IN_TODOLIST = '0'
WHERE SYSTEM_ID = IDTrasmissioneUtenteMittente;
*/

SPsetDataVistaSmistamento (IDPeopleMittente, IDDocumento, IDGruppoMittente, 'D', IDTrasmissione,  resultValue);

IF(resultValue=1) THEN

 ReturnValue:= -4;
 return ;
 
END IF;

END;
END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
BEGIN


select * INTO TipoTrasmSingola from (
SELECT A.CHA_TIPO_TRASM
FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =IDTrasmissioneUtenteMittente))
ORDER BY CHA_TIPO_DEST) where rownum = 1;
END;

IF TipoTrasmSingola = 'S' AND TrasmissioneConWorkflow = '1' THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
BEGIN
UPDATE  DPA_TRASM_UTENTE
SET  CHA_VALIDA = '0', cha_in_todolist = '0'
WHERE ID_TRASM_SINGOLA IN
(SELECT A.SYSTEM_ID
FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=IDPeopleMittente AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =IDTrasmissioneUtenteMittente)))
AND SYSTEM_ID NOT IN( IDTrasmissioneUtenteMittente);

END;
END IF;
ELSE
/* non  stata trovata la ragione di trasmissione */

   returnvalue := -1;

END IF;



END;
/
