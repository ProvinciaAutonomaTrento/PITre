CREATE OR REPLACE PROCEDURE @db_user.migra_template_trasm
IS
id_modello_trasm       NUMBER        := 0;
id_modello_mitt_dest   NUMBER        := 0;
counter                NUMBER        := 0;
cast_counter           VARCHAR (255);
id_trasm               NUMBER;
id_amm                 NUMBER;
nome                   VARCHAR (255);
cha_tipo_oggetto       VARCHAR (1);
id_reg                 NUMBER;
var_note_generali      VARCHAR (255);
id_people              NUMBER;
idruolo_in_uo         NUMBER;
BEGIN                                                        
DBMS_OUTPUT.ENABLE (1000000);
DBMS_OUTPUT.put_line
('-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER DOCUMENTI PROTOCOLLATI-- '
);

DECLARE
CURSOR c_modelli_trasm (
id_trasm            NUMBER,
id_amm              NUMBER,
nome                VARCHAR,
cha_tipo_oggetto    VARCHAR,
id_reg              NUMBER,
var_note_generali   VARCHAR,
id_people           NUMBER,
idruolo_in_uo      NUMBER
)
IS
SELECT dt.system_id, der.id_amm, dtt.var_template,
dt.cha_tipo_oggetto, p.id_registro, dt.var_note_generali,
dt.id_people, dt.id_ruolo_in_uo
FROM dpa_templ_trasm dtt,
dpa_trasmissione dt,
dpa_el_registri der,
PROFILE p
WHERE dtt.id_trasmissione = dt.system_id
AND der.system_id = p.id_registro
AND dt.id_profile = p.system_id
;
BEGIN                                                         
OPEN c_modelli_trasm (id_trasm,
id_amm,
nome,
cha_tipo_oggetto,
id_reg,
var_note_generali,
id_people,
idruolo_in_uo
);

LOOP
FETCH c_modelli_trasm
INTO id_trasm, id_amm, nome, cha_tipo_oggetto, id_reg,
var_note_generali, id_people, idruolo_in_uo;

EXIT WHEN c_modelli_trasm%NOTFOUND;

BEGIN
counter := counter + 1;

SELECT TO_CHAR (counter)
INTO cast_counter
FROM DUAL;

DBMS_OUTPUT.put_line (   cast_counter
|| ' - Migrazione Template per Documento: '
|| nome
);

SELECT seq_dpa_modelli_trasm.NEXTVAL
INTO id_modello_trasm
FROM DUAL;

INSERT INTO dpa_modelli_trasm
(system_id, id_amm, nome, cha_tipo_oggetto,
id_registro, var_note_generali, id_people, SINGLE
)
VALUES (id_modello_trasm, id_amm, nome, cha_tipo_oggetto,
id_reg, var_note_generali, id_people, 0
);

SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp
)
VALUES (id_modello_mitt_dest, id_modello_trasm, 'M',
0, 0, '',
'', 'R'
);

SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp)
SELECT id_modello_mitt_dest, id_modello_trasm, 'D',
dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,
dts.var_note_sing, dts.cha_tipo_dest
FROM dpa_trasm_singola dts
WHERE id_trasmissione = id_trasm;


UPDATE dpa_modelli_mitt_dest
SET cha_tipo_urp = 'P'
WHERE id_modello = id_modello_trasm
AND cha_tipo_urp = 'U'
AND cha_tipo_mitt_dest = 'D';
END;
END LOOP;
EXCEPTION
WHEN OTHERS
THEN
BEGIN
DBMS_OUTPUT.put_line
(' --- Eccezione nella migrazione - eseguo il rollback ---');
ROLLBACK;
END;

id_modello_trasm := 0;
id_modello_mitt_dest := 0;
id_trasm := 0;
id_amm := 0;
nome := '';
cha_tipo_oggetto := '';
id_reg := 0;
var_note_generali := '';
id_people := 0;
idruolo_in_uo := 0;
cast_counter := '';

CLOSE c_modelli_trasm;

SAVEPOINT templdocprot;
END;                                                          

DBMS_OUTPUT.put_line
('-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER FASCICOLI ASSOCIATI AL REGISTRO -- '
);

DECLARE
CURSOR c_modelli_trasm (
id_trasm            INT,
id_amm              INT,
nome                VARCHAR,
cha_tipo_oggetto    VARCHAR,
id_reg              INT,
var_note_generali   VARCHAR,
id_people           INT,
idruolo_in_uo      INT
)
IS
SELECT dt.system_id, p.id_amm, dtt.var_template,
dt.cha_tipo_oggetto, p.id_registro, dt.var_note_generali,
dt.id_people, dt.id_ruolo_in_uo
FROM dpa_templ_trasm dtt, dpa_trasmissione dt, project p
WHERE dtt.id_trasmissione = dt.system_id
AND dt.id_project = p.system_id
AND p.cha_tipo_proj = 'F'
AND p.cha_tipo_fascicolo = 'P'
AND p.id_registro IS NOT NULL
;
BEGIN                                                       
OPEN c_modelli_trasm (id_trasm,
id_amm,
nome,
cha_tipo_oggetto,
id_reg,
var_note_generali,
id_people,
idruolo_in_uo
);

LOOP
FETCH c_modelli_trasm
INTO id_trasm, id_amm, nome, cha_tipo_oggetto, id_reg,
var_note_generali, id_people, idruolo_in_uo;

EXIT WHEN c_modelli_trasm%NOTFOUND;

BEGIN
counter := counter + 1;

SELECT TO_CHAR (counter)
INTO cast_counter
FROM DUAL;

DBMS_OUTPUT.put_line (   cast_counter
|| ' - Migrazione Template Per Fascicolo: '
|| nome
);


SELECT seq_dpa_modelli_trasm.NEXTVAL
INTO id_modello_trasm
FROM DUAL;

INSERT INTO dpa_modelli_trasm
(system_id, id_amm, nome, cha_tipo_oggetto,
id_registro, var_note_generali, id_people, SINGLE
)
VALUES (id_modello_trasm, id_amm, nome, cha_tipo_oggetto,
id_reg, var_note_generali, id_people, 0
);

SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp
)
VALUES (id_modello_mitt_dest, id_modello_trasm, 'M',
0, 0, '',
'', 'R'
);


SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp)
SELECT id_modello_mitt_dest, id_modello_trasm, 'D',
dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,
dts.var_note_sing, dts.cha_tipo_dest
FROM dpa_trasm_singola dts
WHERE id_trasmissione = id_trasm;

UPDATE dpa_modelli_mitt_dest
SET cha_tipo_urp = 'P'
WHERE id_modello = id_modello_trasm
AND cha_tipo_urp = 'U'
AND cha_tipo_mitt_dest = 'D';
END;
END LOOP;
EXCEPTION
WHEN OTHERS
THEN
BEGIN
DBMS_OUTPUT.put_line
(' --- Eccezione nella migrazione - eseguo il rollback ---');
ROLLBACK;
END;

--RESET VARIABILI LOCALI
id_modello_trasm := 0;
id_modello_mitt_dest := 0;
id_trasm := 0;
id_amm := 0;
nome := '';
cha_tipo_oggetto := '';
id_reg := 0;
var_note_generali := '';
id_people := 0;
idruolo_in_uo := 0;
cast_counter := '';

CLOSE c_modelli_trasm;

SAVEPOINT templfascreg;
END;                                                    

DBMS_OUTPUT.put_line
('-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER DOCUMENTI GRIGI-- ');

DECLARE
CURSOR c_modelli_trasm (
id_trasm            INT,
nome                VARCHAR,
cha_tipo_oggetto    VARCHAR,
var_note_generali   VARCHAR,
id_people           INT,
idruolo_in_uo      INT
)
IS
SELECT dt.system_id, dtt.var_template, dt.cha_tipo_oggetto,
dt.var_note_generali, dt.id_people, dt.id_ruolo_in_uo
FROM dpa_trasmissione dt, PROFILE p, dpa_templ_trasm dtt
WHERE p.system_id = dt.id_profile
AND p.cha_tipo_proto = 'G'
AND dtt.id_trasmissione = dt.system_id
;
BEGIN                                                       
OPEN c_modelli_trasm (id_trasm,
nome,
cha_tipo_oggetto,
var_note_generali,
id_people,
idruolo_in_uo
);

LOOP
FETCH c_modelli_trasm
INTO id_trasm, nome, cha_tipo_oggetto, var_note_generali,
id_people, idruolo_in_uo;

EXIT WHEN c_modelli_trasm%NOTFOUND;

BEGIN
counter := counter + 1;

SELECT TO_CHAR (counter)
INTO cast_counter
FROM DUAL;

DBMS_OUTPUT.put_line
(   cast_counter
|| ' - Migrazione Template Documento Grigio: '
|| nome
);

--0. prelevo le info mancati
SELECT sys_id, amm
INTO id_reg, id_amm
FROM (SELECT der.system_id AS sys_id, der.id_amm AS amm
FROM dpa_l_ruolo_reg dlrr, dpa_el_registri der
WHERE dlrr.id_ruolo_in_uo = idruolo_in_uo
AND dlrr.id_registro = der.system_id)
WHERE ROWNUM = 1;

--1. inserisco i dati attualizzati nella dpa_modelli_trasm
SELECT seq_dpa_modelli_trasm.NEXTVAL
INTO id_modello_trasm
FROM DUAL;

INSERT INTO dpa_modelli_trasm
(system_id, id_amm, nome, cha_tipo_oggetto,
id_registro, var_note_generali, id_people, SINGLE
)
VALUES (id_modello_trasm, id_amm, nome, cha_tipo_oggetto,
id_reg, var_note_generali, id_people, 0
);

SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp
)
VALUES (id_modello_mitt_dest, id_modello_trasm, 'M',
0, 0, '',
'', 'R'
);

SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp)
SELECT id_modello_mitt_dest, id_modello_trasm, 'D',
dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,
dts.var_note_sing, dts.cha_tipo_dest
FROM dpa_trasm_singola dts
WHERE id_trasmissione = id_trasm;

UPDATE dpa_modelli_mitt_dest
SET cha_tipo_urp = 'P'
WHERE id_modello = id_modello_trasm
AND cha_tipo_urp = 'U'
AND cha_tipo_mitt_dest = 'D';
END;
END LOOP;
EXCEPTION
WHEN OTHERS
THEN
BEGIN
DBMS_OUTPUT.put_line
(' --- Eccezione nella migrazione - eseguo il rollback ---');
ROLLBACK;
END;

id_modello_trasm := 0;
id_modello_mitt_dest := 0;
id_trasm := 0;
id_amm := 0;
nome := '';
cha_tipo_oggetto := '';
id_reg := 0;
var_note_generali := '';
id_people := 0;
idruolo_in_uo := 0;
cast_counter := '';

CLOSE c_modelli_trasm;

SAVEPOINT templdocgrigi;
END;                                                    

DBMS_OUTPUT.put_line
('-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER FASCICOLI IN AMMINISTRAZIONE -- '
);

DECLARE
CURSOR c_modelli_trasm (
id_trasm            INT,
id_amm              INT,
nome                VARCHAR,
cha_tipo_oggetto    VARCHAR,
var_note_generali   VARCHAR,
id_people           INT,
idruolo_in_uo      INT
)
IS
SELECT dt.system_id, p.id_amm, dtt.var_template,
dt.cha_tipo_oggetto, dt.var_note_generali, dt.id_people,
dt.id_ruolo_in_uo
FROM dpa_templ_trasm dtt, dpa_trasmissione dt, project p
WHERE dtt.id_trasmissione = dt.system_id
AND dt.id_project = p.system_id
AND p.cha_tipo_proj = 'F'
AND p.cha_tipo_fascicolo = 'P'
AND p.id_registro IS NULL;
BEGIN                                                         
OPEN c_modelli_trasm (id_trasm,
id_amm,
nome,
cha_tipo_oggetto,
var_note_generali,
id_people,
idruolo_in_uo
);

LOOP
FETCH c_modelli_trasm
INTO id_trasm, id_amm, nome, cha_tipo_oggetto, var_note_generali,
id_people, idruolo_in_uo;

EXIT WHEN c_modelli_trasm%NOTFOUND;

BEGIN
counter := counter + 1;

SELECT TO_CHAR (counter)
INTO cast_counter
FROM DUAL;

DBMS_OUTPUT.put_line (   cast_counter
|| ' - Migrazione Template Fascicolo: '
|| nome
);

--0. prelevo le info mancati
SELECT reg
INTO id_reg
FROM (SELECT der.system_id AS reg
FROM dpa_l_ruolo_reg dlrr, dpa_el_registri der
WHERE dlrr.id_ruolo_in_uo = idruolo_in_uo
AND dlrr.id_registro = der.system_id)
WHERE ROWNUM = 1;

--1. inserisco i dati attualizzati nella dpa_modelli_trasm
SELECT seq_dpa_modelli_trasm.NEXTVAL
INTO id_modello_trasm
FROM DUAL;

INSERT INTO dpa_modelli_trasm
(system_id, id_amm, nome, cha_tipo_oggetto,
id_registro, var_note_generali, id_people, SINGLE
)
VALUES (id_modello_trasm, id_amm, nome, cha_tipo_oggetto,
id_reg, var_note_generali, id_people, 0
);


SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp
)
VALUES (id_modello_mitt_dest, id_modello_trasm, 'M',
0, 0, '',
'', 'R'
);

--3. inserisco i dati dei destinatari
SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO id_modello_mitt_dest
FROM DUAL;

INSERT INTO dpa_modelli_mitt_dest
(system_id, id_modello, cha_tipo_mitt_dest,
id_corr_globali, id_ragione, cha_tipo_trasm,
var_note_sing, cha_tipo_urp)
SELECT id_modello_mitt_dest, id_modello_trasm, 'D',
dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,
dts.var_note_sing, dts.cha_tipo_dest
FROM dpa_trasm_singola dts
WHERE id_trasmissione = id_trasm;


UPDATE dpa_modelli_mitt_dest
SET cha_tipo_urp = 'P'
WHERE id_modello = id_modello_trasm
AND cha_tipo_urp = 'U'
AND cha_tipo_mitt_dest = 'D';
END;
END LOOP;
EXCEPTION
WHEN OTHERS
THEN
BEGIN
DBMS_OUTPUT.put_line
(' --- Eccezione nella migrazione - eseguo il rollback ---');
ROLLBACK;
END;

--RESET VARIABILI LOCALI
id_modello_trasm := 0;
id_modello_mitt_dest := 0;
id_trasm := 0;
id_amm := 0;
nome := '';
cha_tipo_oggetto := '';
id_reg := 0;
var_note_generali := '';
id_people := 0;
idruolo_in_uo := 0;
cast_counter := '';

CLOSE c_modelli_trasm;

SAVEPOINT templfascamm;
END;                                                          
END;                                                           -- END GENERALE
/
