DROP PROCEDURE @db_user@.I_SMISTAMENTO_SMISTADOC_U;

CREATE OR REPLACE PROCEDURE @db_user@.i_smistamento_smistadoc_u (
idpeoplemittente               IN       NUMBER,
idcorrglobaleruolomittente     IN       NUMBER,
idgruppomittente               IN       NUMBER,
idamministrazionemittente      IN       NUMBER,
idcorrglobaledestinatario      IN       NUMBER,
iddocumento                    IN       NUMBER,
idtrasmissione                 IN       NUMBER,
idtrasmissioneutentemittente   IN       NUMBER,
trasmissioneconworkflow        IN       CHAR,
notegeneralidocumento          IN       VARCHAR2,
noteindividuali                IN       VARCHAR2,
datascadenza                   IN       DATE,
tipotrasmissione               IN       CHAR,
tipodiritto                    IN       CHAR,
rights                         IN       NUMBER,
originalrights                 IN       NUMBER,
idragionetrasm                 IN       NUMBER,
idpeopledelegato               IN       NUMBER,
returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
identitytrasm         NUMBER       := NULL;
systrasmsing          NUMBER       := NULL;
existaccessrights     CHAR (1)     := 'Y';
accessrights          NUMBER       := NULL;
accessrightsvalue     NUMBER       := NULL;
idutente              NUMBER;
recordcorrente        NUMBER;
idgroups              NUMBER       := NULL;
idgruppo              NUMBER;
resultvalue           NUMBER;
tipotrasmsingola      CHAR (1)     := NULL;
isaccettata           VARCHAR2 (1) := '0';
isaccettatadelegato   VARCHAR2 (1) := '0';
isvista               VARCHAR2 (1) := '0';
isvistadelegato       VARCHAR2 (1) := '0';
val_idpeopledelegato  NUMBER;
TipoRag VARCHAR2 (1);
BEGIN
BEGIN
SELECT seq.NEXTVAL
INTO identitytrasm
FROM DUAL;
END;

BEGIN
SELECT seq.NEXTVAL
INTO systrasmsing
FROM DUAL;
END;

BEGIN
-- inizio modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
/* Inserimento in tabella DPA_TRASMISSIONE */

-- la procedura riceve in input il valore = 0 per il campo idpeopledelegato, in caso di senza delega
-- e il valore = 1 in caso di delega
-- nel primo caso si vuole avere comunque il valore NULL nel campo idpeopledelegato
IF (idpeopledelegato > 0)
THEN
val_idpeopledelegato := idpeopledelegato ;
ELSE
val_idpeopledelegato := NULL;
END IF;
INSERT INTO dpa_trasmissione
(system_id, id_ruolo_in_uo,
id_people, cha_tipo_oggetto, id_profile, id_project,
dta_invio, var_note_generali, id_people_delegato
)
VALUES (identitytrasm, idcorrglobaleruolomittente,
idpeoplemittente, 'D', iddocumento, NULL,
SYSDATE (), notegeneralidocumento, val_idpeopledelegato
);
-- precedente era
/*
INSERT INTO dpa_trasmissione
(system_id, id_ruolo_in_uo,
id_people, cha_tipo_oggetto, id_profile, id_project,
dta_invio, var_note_generali
)
VALUES (identitytrasm, idcorrglobaleruolomittente,
idpeoplemittente, 'D', iddocumento, NULL,
SYSDATE , notegeneralidocumento
);
*/
-- fine modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega

EXCEPTION
WHEN OTHERS
THEN
returnvalue := -2;
RETURN;
END;

BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
INSERT INTO dpa_trasm_singola
(system_id, id_ragione, id_trasmissione, cha_tipo_dest,
id_corr_globale, var_note_sing,
cha_tipo_trasm, dta_scadenza, id_trasm_utente
)
VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
idcorrglobaledestinatario, noteindividuali,
tipotrasmissione, datascadenza, NULL
);

returnvalue := systrasmsing;
EXCEPTION
WHEN OTHERS
THEN
returnvalue := -3;
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
SELECT a.id_gruppo
INTO idgroups
FROM dpa_corr_globali a
WHERE a.system_id = idcorrglobaledestinatario;
END;

idgruppo := idgroups;

BEGIN
SELECT accessrights
INTO accessrights
FROM (SELECT accessrights
FROM security
WHERE thing = iddocumento AND personorgroup = idgruppo)
WHERE ROWNUM = 1;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
existaccessrights := 'N';
END;

IF existaccessrights = 'Y'
THEN
accessrightsvalue := accessrights;

IF accessrightsvalue < rights
THEN
BEGIN
/* aggiornamento a Rights */
UPDATE security
SET accessrights = rights,
cha_tipo_diritto = 'T'
WHERE thing = iddocumento
AND personorgroup = idgruppo
AND accessrights = accessrightsvalue;
EXCEPTION
WHEN DUP_VAL_ON_INDEX
THEN
NULL;
END;
END IF;
ELSE
BEGIN
/* inserimento a Rights */
INSERT INTO security
(thing, personorgroup, accessrights, id_gruppo_trasm,
cha_tipo_diritto
)
VALUES (iddocumento, idgruppo, rights, idgruppomittente,
tipodiritto
);
EXCEPTION
WHEN DUP_VAL_ON_INDEX
THEN
NULL;
END;
END IF;

/* Aggiornamento trasmissione del mittente */
IF (trasmissioneconworkflow = '1')
THEN
BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
SELECT cha_accettata
INTO isaccettata
FROM dpa_trasm_utente
WHERE system_id = idtrasmissioneutentemittente;

SELECT cha_vista
INTO isvista
FROM dpa_trasm_utente
WHERE system_id = idtrasmissioneutentemittente;

SELECT cha_tipo_ragione into TipoRag from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
where tsu.system_id=idtrasmissioneutentemittente and ts.system_id=tsu.ID_TRASM_SINGOLA and rs.system_id=ts.ID_RAGIONE ;

IF (idpeopledelegato > 0)
THEN
BEGIN
-- Impostazione dei flag per la gestione del delegato
isvistadelegato := '1';
isaccettatadelegato := '1';
END;
END IF;

IF (isaccettata = '1')
THEN
BEGIN
-- caso in cui la trasmissione risulta gi? accettata
IF (isvista = '0')
THEN
BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
UPDATE dpa_trasm_utente
SET dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
cha_vista =
(CASE
WHEN dta_vista IS NULL
THEN 1
ELSE 0
END
),
cha_vista_delegato = isvistadelegato,
cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
);
END;
ELSE
BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
UPDATE dpa_trasm_utente
SET cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
);
END;
END IF;
END;
ELSE

begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

if(TipoRAG='W') then
BEGIN
UPDATE dpa_trasm_utente
SET dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
cha_vista = (CASE
WHEN dta_vista IS NULL
THEN 1
ELSE 0
END),
cha_vista_delegato = isvistadelegato,
dta_accettata = SYSDATE,
cha_accettata = '1',
cha_accettata_delegato = isaccettatadelegato,
var_note_acc = 'Documento accettato e smistato',
cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
)
AND cha_valida = '1';
END;
else --no workflow
BEGIN
UPDATE dpa_trasm_utente
SET dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
cha_vista = (CASE
WHEN dta_vista IS NULL
THEN 1
ELSE 0
END),
cha_vista_delegato = isvistadelegato,
-- dta_accettata = SYSDATE (),
--  cha_accettata = '1',
-- cha_accettata_delegato = isaccettatadelegato,
-- var_note_acc = 'Documento accettato e smistato',
cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U'  AND cha_valida = '1')
)
;
END;



end if;
end;

END IF;



--update security se diritti  trasmssione in accettazione =20
UPDATE security s
SET s.accessrights = originalrights,
s.cha_tipo_diritto = 'T'
WHERE s.thing = iddocumento
AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
AND s.accessrights = 20;

EXCEPTION
WHEN DUP_VAL_ON_INDEX
THEN
-- visibilit gi esistente, ignora e continua con eventuali altri inserimenti
NULL;
END;
ELSE
BEGIN
spsetdatavistasmistamento (idpeoplemittente,
iddocumento,
idgruppomittente,
'D',
idtrasmissione,
idpeopledelegato,
resultvalue
);

IF (resultvalue = 1)
THEN
returnvalue := -4;
RETURN;
END IF;
END;
END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
BEGIN
SELECT *
INTO tipotrasmsingola
FROM (SELECT   a.cha_tipo_trasm
FROM dpa_trasm_singola a, dpa_trasm_utente b
WHERE a.system_id = b.id_trasm_singola
AND b.system_id IN (
SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.system_id =
(SELECT id_trasm_singola
FROM dpa_trasm_utente
WHERE system_id =
idtrasmissioneutentemittente))
ORDER BY cha_tipo_dest)
WHERE ROWNUM = 1;
END;

IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
BEGIN
UPDATE dpa_trasm_utente
SET cha_valida = '0',
cha_in_todolist = '0'
WHERE id_trasm_singola IN (
SELECT a.system_id
FROM dpa_trasm_singola a, dpa_trasm_utente b
WHERE a.system_id = b.id_trasm_singola
AND b.system_id IN (
SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.system_id =
(SELECT id_trasm_singola
FROM dpa_trasm_utente
WHERE system_id =
idtrasmissioneutentemittente)))
AND system_id NOT IN (idtrasmissioneutentemittente);
END;
END IF;
END;
/


