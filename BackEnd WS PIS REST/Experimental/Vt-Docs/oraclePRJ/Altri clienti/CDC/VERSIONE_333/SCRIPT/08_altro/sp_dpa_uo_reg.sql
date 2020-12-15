

CREATE OR REPLACE PROCEDURE sp_dpa_uo_reg (
iduo          IN       NUMBER,
ReturnValue  OUT       NUMBER
)
IS
record_corr_ruolo      NUMBER;
record_corr_registro   NUMBER;
record_trovato         NUMBER;
rec                    NUMBER;

CURSOR cursor_ruoli (system_id_uo NUMBER)
IS
SELECT system_id
FROM dpa_corr_globali
WHERE cha_tipo_urp = 'R'
AND cha_tipo_ie = 'I'
AND dta_fine IS NULL
AND id_old = 0
AND id_uo = system_id_uo;

CURSOR cursor_registri (system_id_ruolo NUMBER)
IS
-- MODIFICATO PER INTRODUZIONE GESTIONE RF
SELECT id_registro
FROM dpa_l_ruolo_reg a, dpa_el_registri b
WHERE a.id_registro = b.system_id
AND a.id_ruolo_in_uo = system_id_ruolo
AND b.cha_rf = '0';
CURSOR cursor_uo
IS
select system_id from dpa_corr_globali where cha_tipo_urp='U' and cha_tipo_ie='I';


BEGIN

declare
uoId number;
begin
BEGIN
DELETE FROM dpa_uo_reg
WHERE id_uo = iduo;
EXCEPTION
WHEN OTHERS
THEN
returnvalue := 1;
RETURN;
END;

-- cicla per tutti i ruoli validi della UO passata
/*open cursor_uo;

loop
FETCH cursor_uo
INTO uoId;

EXIT WHEN cursor_uo%NOTFOUND;

OPEN cursor_ruoli (uoId);
*/

OPEN cursor_ruoli (iduo);
LOOP
FETCH cursor_ruoli
INTO record_corr_ruolo;

EXIT WHEN cursor_ruoli%NOTFOUND;

BEGIN
-- cicla per tutti i registri associati al ruolo in UO
OPEN cursor_registri (record_corr_ruolo);

LOOP
FETCH cursor_registri
INTO record_corr_registro;

EXIT WHEN cursor_registri%NOTFOUND;

BEGIN
record_trovato := 0;

BEGIN
SELECT system_id
INTO rec
FROM dpa_uo_reg
WHERE id_uo = iduo AND id_registro = record_corr_registro;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
record_trovato := 1;
WHEN OTHERS
THEN
returnvalue := 1;
RETURN;
END;

IF record_trovato = 1
THEN
BEGIN
INSERT INTO dpa_uo_reg
(system_id, id_uo, id_registro
)
VALUES (seq.NEXTVAL, iduo, record_corr_registro
);
EXCEPTION
WHEN OTHERS
THEN
returnvalue := 1;
RETURN;
END;
END IF;
END;
END LOOP;

CLOSE cursor_registri;
END;
END LOOP;

CLOSE cursor_ruoli;
--end loop;
--CLOSE cursor_uo;

end;
returnvalue := 0;
END;
/


