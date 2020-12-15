CREATE OR REPLACE FUNCTION Corrcat (docId INT, tipo_proto VARCHAR)
RETURN VARCHAR IS risultato VARCHAR(4000);

item VARCHAR(4000);
tipo_mitt_dest VARCHAR(10);

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
	END LOOP;

	RETURN risultato;

END Corrcat;