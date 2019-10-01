--------------------------------------------------------
--  DDL for Procedure CARICA_DPA_REG_FASC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CARICA_DPA_REG_FASC" IS
BEGIN
DECLARE
sysRegistro number; -- systemId del registro
sysTitolario number;
sysRegistroNodoTit number;
Identity NUMBER;
countFasc NUMBER;

CURSOR currAmm IS
select system_id
from dpa_amministra ORDER BY system_id DESC;

CURSOR currTit(n_id_amm NUMBER) IS
select system_id, id_registro
from PROJECT
WHERE CHA_TIPO_PROJ = 'T'
AND ID_AMM = n_id_amm;

CURSOR currReg(n_id_amm NUMBER) IS
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = n_id_amm and cha_rf = '0';

BEGIN
FOR currentAmm in currAmm
LOOP
FOR currentTit in currTit(currentAmm.system_id) LOOP

BEGIN

-- si il modo di titolario ha id_registro NULL allora nella tabella dpa_reg_fasc
--dovranno essere inseriti tanti record quanti sono i registri contentuti nella dpa_el_registri

IF currentTit.ID_REGISTRO IS NULL THEN

BEGIN

FOR currentReg in currReg(currentAmm.system_id) LOOP

SELECT MAX(NUM_FASCICOLO) INTO countFasc
FROM PROJECT WHERE ID_PARENT = currentTit.system_id and cha_tipo_fascicolo = 'P'
and anno_creazione = TO_CHAR(SYSDATE, 'yyyy') and id_registro = currentReg.SYSTEM_ID;

IF countFasc IS NULL THEN
countFasc := 0;
END IF;

INSERT INTO DPA_REG_FASC
(
SYSTEM_ID,
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
seq.NEXTVAL,
currentTit.system_id,
currentReg.SYSTEM_ID,
countFasc+1 -- QUI  mettiamo il MAX (NUM FASCICOLO) - dafault a 0 per il pregresso

);

END LOOP;


-- CASO DEL REGISTRO NULL
SELECT MAX(NUM_FASCICOLO) INTO countFasc
FROM PROJECT WHERE ID_PARENT = currentTit.system_id and cha_tipo_fascicolo = 'P'
and anno_creazione = TO_CHAR(SYSDATE, 'yyyy') and id_registro IS NULL;

IF countFasc IS NULL THEN
countFasc := 0;
END IF;

INSERT INTO DPA_REG_FASC
(
SYSTEM_ID,
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
seq.NEXTVAL,
currentTit.system_id,
NULL,
countFasc+1 -- QUI mettiamo IL MAX (NUM FASCICOLO)

);

END;


ELSE  -- SE IL NODO ? ASSOCIATO AD UN SOLO REGISTRO

BEGIN
SELECT MAX(NUM_FASCICOLO) INTO countFasc
FROM PROJECT WHERE ID_PARENT = currentTit.system_id and cha_tipo_fascicolo = 'P'
and anno_creazione = TO_CHAR(SYSDATE, 'yyyy') and id_registro = currentTit.id_registro;

IF countFasc IS NULL THEN
countFasc := 0;
END IF;

INSERT INTO DPA_REG_FASC
(
SYSTEM_ID,
ID_TITOLARIO,
ID_REGISTRO,
NUM_RIF

)
VALUES
(
seq.NEXTVAL,
currentTit.system_id,
currentTit.id_registro,
countFasc+1
);
END;
END IF;
END;
END LOOP;

END LOOP;
commit;
END;
END; 

/
