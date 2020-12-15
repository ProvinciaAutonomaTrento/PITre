CREATE OR REPLACE PROCEDURE @db_user.SPsetDataVista(
p_idPeople IN NUMBER,
p_idOggetto IN NUMBER,
p_idGruppo IN NUMBER,
p_tipoOggetto IN CHAR,
p_resultValue OUT number
) IS

p_cha_tipo_trasm CHAR(1) := NULL;
p_chaTipoDest NUMBER;


BEGIN
p_resultValue:=0;


DECLARE

CURSOR cursorTrasmSingolaDocumento IS

SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = p_idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = p_idPeople))
AND a.ID_PROFILE = p_idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID;

CURSOR cursorTrasmSingolaFascicolo IS
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = p_idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = p_idPeople))
AND a.ID_PROJECT = p_idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID;

BEGIN


IF(p_tipoOggetto='D') THEN

FOR currentTrasmSingola IN cursorTrasmSingolaDocumento
LOOP
BEGIN

IF (currentTrasmSingola.cha_tipo_ragione = 'N' OR currentTrasmSingola.cha_tipo_ragione = 'I') then
-- SE ¿ una trasmissione senza workFlow
begin

-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

begin

IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') then
-- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
begin
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
AND DPA_TRASM_UTENTE.ID_PEOPLE != p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

end if;
end;
ELSE
-- la ragione di trasmissione prevede workflow
BEGIN

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
END;

END IF;

END;

END LOOP;
END IF;

IF(p_tipoOggetto='F') THEN

FOR currentTrasmSingola IN cursorTrasmSingolaFascicolo
LOOP
BEGIN

IF (currentTrasmSingola.cha_tipo_ragione = 'N' OR currentTrasmSingola.cha_tipo_ragione = 'I') then
-- SE ¿ una trasmissione senza workFlow
begin

-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;
begin
IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') then
-- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
begin
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
AND DPA_TRASM_UTENTE.ID_PEOPLE != p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

end if;
end;
ELSE
-- se la ragione di trasmissione prevede workflow
BEGIN

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
END;

END IF;

END;

END LOOP;

END IF;

END;
END SPsetDataVista;
/
