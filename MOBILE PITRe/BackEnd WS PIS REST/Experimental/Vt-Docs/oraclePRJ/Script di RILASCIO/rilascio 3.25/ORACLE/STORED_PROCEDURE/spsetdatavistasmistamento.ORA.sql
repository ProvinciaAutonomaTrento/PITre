begin 
Utl_Backup_Plsql_code ('PROCEDURE','spsetdatavistasmistamento'); 
end;
/

create or replace
PROCEDURE             spsetdatavistasmistamento (
p_idpeople         IN       NUMBER,
p_idoggetto        IN       NUMBER,
p_idgruppo         IN       NUMBER,
p_tipooggetto      IN       CHAR,
p_idtrasmissione   IN       NUMBER,
p_iddelegato       IN       NUMBER,
p_resultvalue      OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
p_cha_tipo_trasm   CHAR (1) := NULL;
p_chatipodest      NUMBER;
tipodiritti        CHAR (1) := NULL;
diritti            NUMBER;
BEGIN
p_resultvalue := 0;

DECLARE
CURSOR cursortrasmsingoladocumento
IS
SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
WHERE a.system_id = p_idtrasmissione
AND a.dta_invio IS NOT NULL
AND a.system_id = b.id_trasmissione
AND (   b.id_corr_globale = (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = p_idgruppo)
OR b.id_corr_globale = (SELECT system_id
FROM dpa_corr_globali
WHERE id_people = p_idpeople)
)
AND a.id_profile = p_idoggetto
AND b.id_ragione = c.system_id;
BEGIN
IF (p_tipooggetto = 'D')
THEN
FOR currenttrasmsingola IN cursortrasmsingoladocumento
LOOP
BEGIN
IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
OR currenttrasmsingola.cha_tipo_ragione = 'I'
)
THEN
-- SE ? una trasmissione senza workFlow
IF (p_iddelegato = 0)
THEN
BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento setto la data di vista
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dpa_trasm_utente.cha_in_todolist = '0'
WHERE            --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
--in caso di delega
BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dpa_trasm_utente.cha_in_todolist = '0'
WHERE            --dpa_trasm_utente.dta_vista IS NULL
-- AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;

-- Impostazione data vista nella trasmissione in todolist
BEGIN
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola = currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
AND id_profile = p_idoggetto;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;

BEGIN
IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
AND currenttrasmsingola.cha_tipo_dest = 'R'
)
THEN
IF (p_iddelegato = 0)
THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_in_todolist = '0'
WHERE      --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.cha_in_todolist = '0'
WHERE
--dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;
END IF;
END;
ELSE
BEGIN
-- la ragione di trasmissione prevede workflow
IF (p_iddelegato = 0)
THEN
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dta_accettata =
(CASE
WHEN dta_accettata IS NULL
THEN SYSDATE
ELSE dta_accettata
END
),
cha_accettata = '1',
cha_valida = '0'                         --,
--cha_in_todolist = '0'
WHERE          --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people = p_idpeople;

UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dta_accettata =
(CASE
WHEN dta_accettata IS NULL
THEN SYSDATE
ELSE dta_accettata
END
),
cha_accettata = '1',
cha_accettata_delegato = '1',
cha_valida = '0'                         --,
--cha_in_todolist = '0'
WHERE          --dpa_trasm_utente.dta_vista IS NULL
--   AND
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people = p_idpeople;

UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;

BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
UPDATE dpa_trasm_utente
SET cha_in_todolist = '0'
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND NOT dpa_trasm_utente.dta_vista IS NULL
AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
AND id_profile = p_idoggetto;
begin
SELECT r.cha_tipo_diritti
INTO tipodiritti
FROM dpa_trasm_singola w,dpa_ragione_trasm r
WHERE w.system_id = currenttrasmsingola.system_id and r.SYSTEM_ID=w.id_ragione;
exception when others then tipodiritti:=0;
end;
IF (tipodiritti IS NOT NULL)
THEN
IF (tipodiritti = 'W')
THEN
diritti := 63;

IF (tipodiritti = 'N')
THEN
diritti := 0;

IF (tipodiritti = 'R')
THEN
diritti := 45;
END IF;
END IF;
END IF;
END IF;

IF (diritti != 0)
THEN
BEGIN
UPDATE security s
SET s.accessrights = diritti,
s.cha_tipo_diritto = 'T'
WHERE s.thing = p_idoggetto
AND s.personorgroup IN (p_idpeople, p_idgruppo)
AND s.accessrights = 20;
EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;
END IF;
END;

BEGIN
-- se la trasm ? con WorkFlow ed ? di tipo UNO e il dest ? Ruolo allora levo la validit? della
-- trasmissione a tutti gli altri utenti del ruolo
IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
AND currenttrasmsingola.cha_tipo_dest = 'R'
)
THEN
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_valida = '0',
dpa_trasm_utente.cha_in_todolist = '0'
WHERE
-- DPA_TRASM_UTENTE.DTA_VISTA IS NULL
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;
END;
END;
END IF;
END;
END LOOP;
END IF;
END;
END spsetdatavistasmistamento; 
/
