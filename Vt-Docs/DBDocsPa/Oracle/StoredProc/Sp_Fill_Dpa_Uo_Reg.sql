CREATE OR REPLACE PROCEDURE @db_user.Sp_Fill_Dpa_Uo_Reg IS

record_corr_uo NUMBER;
record_corr_ruolo NUMBER;
record_corr_registro NUMBER;

record_trovato NUMBER;
rec NUMBER;

CURSOR cursor_uo IS
SELECT system_id
FROM DPA_CORR_GLOBALI
WHERE
cha_tipo_urp = 'U'
AND cha_tipo_ie = 'I'
AND dta_fine IS NULL
AND id_old = 0;

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
SELECT id_registro
FROM DPA_L_RUOLO_REG A, DPA_EL_REGISTRI B
WHERE A.id_ruolo_in_uo = system_id_ruolo AND A.ID_REGISTRO = B.SYSTEM_ID AND CHA_RF= '0';


BEGIN

--ReturnValue := 0;

-- cicla per tutte le UO valide
OPEN cursor_uo;
LOOP
FETCH cursor_uo INTO record_corr_uo;
EXIT WHEN cursor_uo%NOTFOUND;

BEGIN

-- cicla per tutti i ruoli validi di questa UO
OPEN cursor_ruoli(record_corr_uo);
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
ID_UO = record_corr_uo
AND ID_REGISTRO = record_corr_registro;

EXCEPTION
WHEN NO_DATA_FOUND THEN
record_trovato := 1;
WHEN OTHERS THEN
DBMS_OUTPUT.PUT_LINE('ERRORE NELLA SELECT DPA_UO_REG!');
RETURN;
END;

IF record_trovato = 1 THEN
BEGIN
INSERT INTO DPA_UO_REG
(SYSTEM_ID, ID_UO, ID_REGISTRO)
VALUES
(seq.NEXTVAL, record_corr_uo, record_corr_registro);
EXCEPTION
WHEN OTHERS THEN
DBMS_OUTPUT.PUT_LINE('ERRORE IN INSERIMENTO PER ID_UO: '||record_corr_uo);
RETURN;
END;
END IF;

END;

END LOOP;
CLOSE cursor_registri;

END;

END LOOP;
CLOSE cursor_ruoli;

END;

END LOOP;
CLOSE cursor_uo;

DBMS_OUTPUT.PUT_LINE('TERMINATA!');
END;
/