--------------------------------------------------------
--  DDL for Function CORRCAT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CORRCAT" (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato clob;

item clob;
tipo_mitt_dest VARCHAR(10);
LNG INT;

CURSOR cur IS
SELECT c.var_desc_corr, dap.cha_tipo_mitt_dest
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item,tipo_mitt_dest;
EXIT WHEN cur%NOTFOUND;

LNG:=LENGTh(risultato);

IF(risultato IS NOT NULL anD LNG>=(3900-128))
tHEN RETURN RISULTATO||'...';
ELSE
BEGIN

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'MD') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MM)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'I') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MI)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'L') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_LISTA(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'F') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| item || '(D) ';
ELSE
risultato := risultato||item;
END IF;
END IF;


END;
END IF;

END LOOP;

RETURN risultato;

END Corrcat;

/
