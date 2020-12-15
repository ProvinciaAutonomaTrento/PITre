CREATE OR REPLACE PROCEDURE I_Smistamento_Smistadoc (
IDPeopleMittente IN NUMBER,
IDCorrGlobaleRuoloMittente IN NUMBER,
IDGruppoMittente IN NUMBER,
IDAmministrazioneMittente IN NUMBER,
IDPeopleDestinatario IN NUMBER,
IDCorrGlobaleDestinatario IN NUMBER,
IDDocumento IN NUMBER,
IDTrasmissione IN NUMBER,
FlagCompetenza IN CHAR,
IDTrasmissioneUtenteMittente IN NUMBER,
TrasmissioneConWorkflow IN CHAR,
NoteGeneraliDocumento IN VARCHAR2,
NoteIndividuali IN VARCHAR2,
DataScadenza IN DATE,
ReturnValue OUT NUMBER)	IS


DescrizioneRagione VARCHAR2(10) := NULL;
Rights NUMBER:= NULL;
IdentityTrasm NUMBER := NULL;
IdentityTrasmSing NUMBER := NULL;
TestoNote VARCHAR2(250) := NULL;

ExistIDRagioneTrasm CHAR(1) := 'Y';
IDRagioneTrasm NUMBER;
idRagTrasmValue NUMBER := NULL;

ExistAccessRights CHAR(1) := 'Y';
AccessRights NUMBER:= NULL;
accessRightsValue NUMBER := NULL;
TipoTrasmSingola CHAR(1) := NULL;

resultValue number;

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
SELECT SYSTEM_ID INTO IDRagioneTrasm
FROM 	DPA_RAGIONE_TRASM
WHERE 	ID_AMM = IDAmministrazioneMittente
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
SELECT seq.NEXTVAL INTO IdentityTrasmSing FROM dual;
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
/* Inserimento in tabella DPA_TRASM_SINGOLA */
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
IdentityTrasmSing,
idRagTrasmValue,
IdentityTrasm,
'U',
IDCorrGlobaleDestinatario,
NoteIndividuali,
'S',
DataScadenza,
NULL
);

EXCEPTION
			WHEN OTHERS THEN  returnvalue:= -3 ;
			RETURN;
			
			
END;

BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
INSERT INTO DPA_TRASM_UTENTE
(
SYSTEM_ID,
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
seq.NEXTVAL,
IdentityTrasmSing,
IDPeopleDestinatario,
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
);

EXCEPTION
			WHEN OTHERS THEN  returnvalue:= -4 ;
			RETURN;
			
END;

BEGIN


--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio.!!!
update dpa_trasmissione set dta_invio=sysdate() where system_id=IdentityTrasm;


SELECT ACCESSRIGHTS INTO AccessRights FROM (
SELECT 	ACCESSRIGHTS
FROM 	SECURITY
WHERE 	THING = IDDocumento
AND
PERSONORGROUP = IDPeopleDestinatario
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
UPDATE 	SECURITY
SET 	ACCESSRIGHTS = Rights
WHERE 	THING = IDDocumento
AND
PERSONORGROUP = IDPeopleDestinatario
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
IDPeopleDestinatario,
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
UPDATE 	DPA_TRASM_UTENTE
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

-- se la trasmissione è senza workflow allora richiamo solo il setDataVista

/*UPDATE 	DPA_TRASM_UTENTE
SET 	DTA_VISTA = SYSDATE(),
CHA_VISTA = '1',
CHA_IN_TODOLIST = '0'
WHERE	SYSTEM_ID = IDTrasmissioneUtenteMittente;*/

SPsetDataVistaSmistamento (IDPeopleMittente, IDDocumento, IDGruppoMittente, 'D', IDTrasmissione , resultValue);

IF(resultValue=1) THEN

 ReturnValue := -5;
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
UPDATE 	DPA_TRASM_UTENTE
SET 	CHA_VALIDA = '0',  cha_in_todolist = '0'
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
ReturnValue := -1;
RETURN;
END IF;

ReturnValue := 0;
END;
/
