CREATE OR REPLACE FUNCTION @db_user.docclasstablefunction (
id_amm       NUMBER,
id_registr   NUMBER,
id_anno      NUMBER,
sede         VARCHAR
)
RETURN docclasstablerow PIPELINED
IS
outrec           docclasstabletype
:= docclasstabletype (NULL, NULL, NULL, NULL, NULL);
totdocclass      FLOAT;
codclass         VARCHAR (255);
descclass        VARCHAR (255);
totdocclassvt    NUMBER;
percdocclassvt   FLOAT;
contatore        FLOAT;
tmpcontatore     FLOAT;
v_var_sede       VARCHAR (100);
system_id_vt     NUMBER;
description_vt   VARCHAR (255);
var_codice_vt    VARCHAR (255);
system_id_fasc   NUMBER;
system_id_fold   NUMBER;

CURSOR c_vocitit (amm NUMBER, id_reg NUMBER)
IS
SELECT   system_id, description, var_codice
FROM project
WHERE var_codice IS NOT NULL
AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
AND id_amm = amm
AND cha_tipo_proj = 'T'
AND (id_registro = id_reg OR id_registro IS NULL)
ORDER BY var_cod_liv1;


CURSOR c_fascicoli (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'F'
AND id_amm = amm
AND (id_registro = reg OR id_registro IS NULL)
AND id_parent = parentid;

CURSOR c_folder (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'C'
AND id_amm = amm
AND id_parent = parentid
AND (id_registro = reg OR id_registro IS NULL);
BEGIN

percdocclassvt := 0;
totdocclass := 0;
contatore := 0;

IF (sede = '')
THEN
v_var_sede := NULL;
ELSE
v_var_sede := sede;
END IF;

IF ((v_var_sede <> ' ') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (PROFILE.system_id)
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND ((id_registro = id_registr) OR (id_registro IS NULL))
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
);
ELSE
SELECT COUNT (PROFILE.system_id)
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND (id_registro = id_registr OR id_registro IS NULL)
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno;
END IF;

OPEN c_vocitit (id_amm, id_registr);

LOOP
FETCH c_vocitit
INTO system_id_vt, description_vt, var_codice_vt;

EXIT WHEN c_vocitit%NOTFOUND;

OPEN c_fascicoli (id_amm, id_registr, system_id_vt);

LOOP
FETCH c_fascicoli
INTO system_id_fasc;

EXIT WHEN c_fascicoli%NOTFOUND;

OPEN c_folder (id_amm, id_registr, system_id_fasc);

LOOP
FETCH c_folder
INTO system_id_fold;

EXIT WHEN c_folder%NOTFOUND;
tmpcontatore := contatore;

IF ((v_var_sede <> ' ') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (PROFILE.system_id)
INTO contatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
)
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (    var_sede IS NOT NULL
AND PROFILE.var_sede = v_var_sede
)
);
ELSE
SELECT COUNT (PROFILE.system_id)
INTO contatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
);
END IF;

contatore := contatore + tmpcontatore;
END LOOP;

CLOSE c_folder;
END LOOP;

CLOSE c_fascicoli;

IF ((contatore <> 0) AND (totdocclass <> 0))
THEN
percdocclassvt := ROUND (((contatore / totdocclass) * 100), 2);
outrec.tot_doc_class := totdocclass;
outrec.cod_class := var_codice_vt;
outrec.desc_class := description_vt;
outrec.tot_doc_class_vt := contatore;
outrec.perc_doc_class_vt := percdocclassvt;
PIPE ROW (outrec);
END IF;

contatore := 0;
percdocclassvt := 0;
END LOOP;

CLOSE c_vocitit;

RETURN;
EXCEPTION
WHEN OTHERS
THEN
RETURN;
END docclasstablefunction;
/