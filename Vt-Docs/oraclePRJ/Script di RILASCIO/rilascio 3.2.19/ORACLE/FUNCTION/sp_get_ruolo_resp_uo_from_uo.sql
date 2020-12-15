create or replace PROCEDURE sp_get_ruolo_resp_uo_from_uo (
p_id_uo        IN       NUMBER,
p_tipo_ruolo            CHAR,
p_id_corr               number,
p_result       OUT      NUMBER
)
AS
BEGIN
DECLARE
v_isidparentnull        NUMBER;
v_noruoloresponsabile   NUMBER;
v_numeroresponsabili    NUMBER;
v_idparent              NUMBER;
v_id_uo                 NUMBER;
v_id_uo_appo            NUMBER;
BEGIN
v_isidparentnull := 0;
v_noruoloresponsabile := 0;
v_numeroresponsabili := 0;
v_idparent := 0;
v_id_uo := p_id_uo;

WHILE (v_isidparentnull = 0 AND v_noruoloresponsabile = 0)
LOOP
IF (p_tipo_ruolo = 'R')
THEN
SELECT COUNT (*)
INTO v_numeroresponsabili
FROM dpa_corr_globali
WHERE id_uo = v_id_uo
AND cha_tipo_urp = 'R'
AND cha_responsabile = 1 
AND DTA_FINE IS NULL;

IF (v_numeroresponsabili > 0)
THEN
SELECT system_id
INTO v_id_uo_appo
FROM dpa_corr_globali
WHERE id_uo = v_id_uo
AND cha_tipo_urp = 'R'
AND cha_responsabile = 1
AND DTA_FINE IS NULL;

IF(v_id_uo_appo != p_id_corr)
then
p_result:=v_id_uo_appo;
v_noruoloresponsabile := 1;
else
SELECT id_parent
INTO v_idparent
FROM dpa_corr_globali
WHERE system_id = v_id_uo;
IF (v_idparent > 0)
THEN
v_id_uo := v_idparent;
ELSE
p_result := 0;
v_isidparentnull := 1;
END IF;
END IF;
ELSE
SELECT id_parent
INTO v_idparent
FROM dpa_corr_globali
WHERE system_id = v_id_uo;

IF (v_idparent > 0)
THEN
v_id_uo := v_idparent;
ELSE
p_result := 0;
v_isidparentnull := 1;
END IF;
END IF;
ELSE
SELECT COUNT (*)
INTO v_numeroresponsabili
FROM dpa_corr_globali
WHERE id_uo = v_id_uo AND cha_tipo_urp = 'R' AND DTA_FINE IS NULL
AND cha_segretario = 1;

IF (v_numeroresponsabili > 0)
THEN
SELECT system_id
INTO v_id_uo_appo
FROM dpa_corr_globali
WHERE id_uo = v_id_uo
AND cha_tipo_urp = 'R' AND DTA_FINE IS NULL
AND cha_segretario = 1;

IF(v_id_uo_appo != p_id_corr)
then
p_result:=v_id_uo_appo;
v_noruoloresponsabile := 1;
else
SELECT id_parent
INTO v_idparent
FROM dpa_corr_globali
WHERE system_id = v_id_uo;
IF (v_idparent > 0)
THEN
v_id_uo := v_idparent;
ELSE
p_result := 0;
v_isidparentnull := 1;
END IF;
END IF;
ELSE
SELECT id_parent
INTO v_idparent
FROM dpa_corr_globali
WHERE system_id = v_id_uo;

IF (v_idparent > 0)
THEN
v_id_uo := v_idparent;
ELSE
p_result := 0;
v_isidparentnull := 1;
END IF;
END IF;
END IF;
END LOOP;
END;
END;

