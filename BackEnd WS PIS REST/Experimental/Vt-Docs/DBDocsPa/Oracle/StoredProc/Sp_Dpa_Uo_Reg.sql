CREATE OR REPLACE PROCEDURE @db_user.Sp_Dpa_Uo_Reg (
idUO IN NUMBER,
returnValue OUT NUMBER) IS

record_corr_ruolo NUMBER;
record_corr_registro NUMBER;

record_trovato NUMBER;
rec NUMBER;

CURSOR cursor_ruoli(system_id_uo NUMBER) IS
SELECT system_id
FROM DPA_CORR_GLOBALI
WHERE
cha_tipo_urp = 'R'
AND cha_tipo_ie = 'I'
AND dta_fine IS NULL
AND id_old = 0
AND id_uo = system_id_uo;

CURSOR cursor_registri(system_id_ruolo NUMBER) IS
-- MODIFICATO PER INTRODUZIONE GESTIONE RF
SELECT id_registro
FROM DPA_L_RUOLO_REG A, DPA_EL_REGISTRI B
WHERE A.ID_REGISTRO = B.SYSTEM_ID AND  A.id_ruolo_in_uo = system_id_ruolo
AND B.CHA_RF = '0';

BEGIN

BEGIN
DELETE FROM DPA_UO_REG WHERE id_uo = idUO;
EXCEPTION
WHEN OTHERS THEN
ReturnValue := 1;
RETURN;
END;

-- cicla per tutti i ruoli validi della UO passata
OPEN cursor_ruoli(idUO);
LOOP
FETCH cursor_ruoli INTO record_corr_ruolo;
EXIT WHEN cursor_ruoli%NOTFOUND;

BEGIN

-- cicla per tutti i registri associati al ruolo in UO
OPEN cursor_registri(record_corr_ruolo);
LOOP
FETCH cursor_registri INTO record_corr_registro;
EXIT WHEN cursor_registri%NOTFOUND;

BEGIN

record_trovato := 0;

BEGIN

SELECT
system_id INTO rec
FROM
DPA_UO_REG
WHERE
ID_UO = idUO
AND ID_REGISTRO = record_corr_registro;

EXCEPTION
WHEN NO_DATA_FOUND THEN
record_trovato := 1;
WHEN OTHERS THEN
ReturnValue := 1;
RETURN;
END;

IF record_trovato = 1 THEN
BEGIN
INSERT INTO DPA_UO_REG
(SYSTEM_ID, ID_UO, ID_REGISTRO)
VALUES
(seq.NEXTVAL, idUO, record_corr_registro);
EXCEPTION
WHEN OTHERS THEN
ReturnValue := 1;
RETURN;
END;
END IF;

END;

END LOOP;
CLOSE cursor_registri;

END;

END LOOP;
CLOSE cursor_ruoli;

ReturnValue := 0;

END;
/