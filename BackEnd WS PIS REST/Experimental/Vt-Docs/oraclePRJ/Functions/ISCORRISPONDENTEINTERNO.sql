--------------------------------------------------------
--  DDL for Function ISCORRISPONDENTEINTERNO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ISCORRISPONDENTEINTERNO" (
idcorrglobali   INT,
idregistro      INT
)
RETURN NUMBER
IS
RESULT   NUMBER;
BEGIN
DECLARE
tipourp                 VARCHAR (1);
tipoie                  VARCHAR (1);
idpeople                INT;
numero_corrispondenti   INT;
BEGIN
tipoie := 'E';
RESULT := 0;

SELECT a.cha_tipo_ie
INTO tipoie
FROM dpa_corr_globali a
WHERE a.system_id = idcorrglobali;

IF (tipoie = 'I')
THEN
SELECT a.cha_tipo_urp
INTO tipourp
FROM dpa_corr_globali a
WHERE a.system_id = idcorrglobali;

IF (tipourp = 'U')
THEN
SELECT COUNT (*)
INTO numero_corrispondenti
FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
WHERE cha_tipo_ie = 'I'
AND cha_tipo_urp = 'R'
AND id_uo = idcorrglobali
AND f.id_ruolo_in_uo = a.system_id
AND f.id_registro = r.system_id
AND r.system_id = idregistro
AND r.cha_rf = '0';
END IF;

IF (tipourp = 'R')
THEN
SELECT COUNT (*)
INTO numero_corrispondenti
FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
WHERE cha_tipo_ie = 'I'
AND cha_tipo_urp = 'R'
AND a.system_id = idcorrglobali
AND f.id_ruolo_in_uo = a.system_id
AND f.id_registro = r.system_id
AND r.system_id = idregistro
AND r.cha_rf = '0';
END IF;

IF (tipourp = 'P')
THEN
BEGIN
SELECT a.id_people
INTO idpeople
FROM dpa_corr_globali a
WHERE a.system_id = idcorrglobali;

SELECT   COUNT (a.system_id)
INTO numero_corrispondenti
FROM dpa_corr_globali a,
peoplegroups b,
dpa_l_ruolo_reg f,
dpa_el_registri r
WHERE a.id_gruppo = b.groups_system_id
AND b.dta_fine IS NULL
AND b.people_system_id = idpeople
AND f.id_ruolo_in_uo = a.system_id
AND f.id_registro = r.system_id
AND r.system_id = idregistro
AND r.cha_rf = '0'
ORDER BY a.system_id DESC;
END;
END IF;
END IF;

IF (numero_corrispondenti > 0)
THEN
RESULT := 1;
END IF;

RETURN RESULT;
END;
END iscorrispondenteinterno;

/
