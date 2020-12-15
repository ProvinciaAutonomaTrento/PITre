--------------------------------------------------------
--  DDL for Procedure SP_TRASM_RIF_TORNA_MITT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_TRASM_RIF_TORNA_MITT" (
IDPeopleMitt IN NUMBER,
IDRuoloGlobMitt IN NUMBER,
IDAmministrazioneMittente IN NUMBER,
IDTrasmUtente IN NUMBER,
ReturnValue OUT NUMBER) IS

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
P.SYSTEM_ID=CG.ID_PEOPLE AND CG.CHA_TIPO_URP != 'L'
AND CG.DTA_FINE IS NULL AND PG.DTA_FINE IS NULL;

BEGIN


BEGIN

SELECT SYSTEM_ID INTO IDRagioneTrasm
FROM  DPA_RAGIONE_TRASM
WHERE  ID_AMM = IDAmministrazioneMittente
AND
VAR_DESC_RAGIONE = 'RIFIUTO';
EXCEPTION
WHEN NO_DATA_FOUND THEN
ReturnValue := 1;
RETURN;
END;


BEGIN

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
'Trasmissione rifiutata da '||REPLACE(NomeCognomeDest,'','''')||' in data '||to_date(sysdate,'dd/mm/yyyy')  );

EXCEPTION
WHEN OTHERS THEN
ReturnValue := 7;
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
null,--REPLACE(NoteRif,'',''''),
null--sysdate
);
EXCEPTION
WHEN OTHERS THEN
ReturnValue := 9;
RETURN;
END;

END LOOP;
CLOSE cursor_IDUtenti;

update dpa_trasmissione set dta_invio=sysdate() where system_id=IdentityTrasm;
END;

ReturnValue := 0;
END; 

/
