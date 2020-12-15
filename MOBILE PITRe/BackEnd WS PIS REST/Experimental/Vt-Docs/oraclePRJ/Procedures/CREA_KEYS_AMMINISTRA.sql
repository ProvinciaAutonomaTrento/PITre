--------------------------------------------------------
--  DDL for Procedure CREA_KEYS_AMMINISTRA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CREA_KEYS_AMMINISTRA" 
IS
syscurramm   INT;
syscurrkey   VARCHAR (32);
cnt          INT;

CURSOR curramm                    -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
IS
SELECT system_id
FROM dpa_amministra;

CURSOR currkey
-- CURSORE CHE SCORRE LE CHIAVI della tabella DPA_CHIAVI_CONFIG_TEMPLATE
IS
SELECT var_codice
FROM dpa_chiavi_config_template;
BEGIN
OPEN curramm;

LOOP
FETCH curramm
INTO syscurramm;

EXIT WHEN curramm%NOTFOUND;

BEGIN
---cursore annidato x le chiavi di configurazione
BEGIN
OPEN currkey;

LOOP
FETCH currkey
INTO syscurrkey;

EXIT WHEN currkey%NOTFOUND;

BEGIN
SELECT COUNT (*)
INTO cnt
FROM dpa_chiavi_configurazione
WHERE var_codice = syscurrkey AND id_amm = syscurramm;
END;

BEGIN
IF (cnt = 0)
THEN
INSERT INTO dpa_chiavi_configurazione
(system_id, id_amm, var_codice,
var_descrizione, var_valore,
cha_tipo_chiave, cha_visibile,
cha_modificabile, cha_globale)
(SELECT (SELECT MAX (system_id) + 1
FROM dpa_chiavi_configurazione),
syscurramm AS id_amm, var_codice,
var_descrizione, var_valore, cha_tipo_chiave,
cha_visibile, cha_modificabile, '0'
FROM dpa_chiavi_config_template
WHERE var_codice = syscurrkey AND ROWNUM = 1);
END IF;
END;
END LOOP;

CLOSE currkey;
END;
--- fine cursore annidato per chiavi di configurazione
END;
END LOOP;

CLOSE curramm;

COMMIT;
END; 

/
