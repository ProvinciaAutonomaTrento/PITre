--------------------------------------------------------
--  DDL for Function CORRCAT_ADDRESS
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CORRCAT_ADDRESS" (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato varchar(4000);

ind varchar(1000);
item varchar(4000);
tipo_mitt_dest VARCHAR(10);
indirizzo varchar(128);
cap varchar(5);
citta varchar(64);
provincia varchar(2);
LNG INT;

CURSOR cur IS
SELECT c.var_desc_corr, dap.cha_tipo_mitt_dest, dett.VAR_INDIRIZZO, dett.VAR_CAP, dett.VAR_CITTA, dett.VAR_PROVINCIA
FROM DPA_CORR_GLOBALI c left outer join DPA_DETT_GLOBALI dett on c.system_id=dett.id_corr_globali, DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc;

BEGIN
risultato := '';
ind := '';
OPEN cur;
LOOP
FETCH cur INTO item,tipo_mitt_dest,indirizzo,cap,citta,provincia;
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
risultato := risultato||item ||' (M)';
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item ||' (D)';
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item ||' (CC)';
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item ||' (M)';
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'I') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MI)';
ELSE
risultato := risultato||item ||' (MI)';
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item ||' (M)';
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item ||' (D)';
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item ||' (CC)';
END IF;
END IF;

IF (indirizzo IS NOT NULL AND  cap IS NOT NULL AND citta IS NOT NULL AND provincia IS NOT NULL) THEN
BEGIN
ind := indirizzo ||' '|| cap ||' '|| citta ||' '|| provincia;
risultato := risultato ||  ' (' || ind  || ')';
END;
END IF;
END;
END IF;

END LOOP;

RETURN risultato;

END Corrcat_Address; 

/
