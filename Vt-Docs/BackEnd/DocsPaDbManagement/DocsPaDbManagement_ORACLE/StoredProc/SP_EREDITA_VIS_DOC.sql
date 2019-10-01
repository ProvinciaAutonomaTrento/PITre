CREATE OR REPLACE PROCEDURE Sp_Eredita_Vis_Doc (
	   	  IDCorrGlobaleUO IN NUMBER,
		  IDCorrGlobaleRuolo IN NUMBER,
		  IDGruppo IN NUMBER,
		  LivelloRuolo IN NUMBER,
		  IDRegistro IN NUMBER,
		  PariLivello IN NUMBER,
		  returnValue OUT NUMBER) IS

/*
.........................................................................................
SP_EREDITA_VIS_DOC          		MAIN
.........................................................................................
EREDITARIETA' DELLA VISIBILITA' DEI DOCUMENTI DA PARTE DI UN NUOVO RUOLO IN ORGANIGRAMMA
.........................................................................................
SP per il processo di inserimento della visibilità da ereditare sui documenti.

VALORI DI INPUT:

IDCorrGlobaleUO   	  = [system_id - tabella dpa_corr_globali] UO nella quale è stato
		   		        inserito il nuovo ruolo;

IDCorrGlobaleRuolo	  = [system_id - tabella dpa_corr_globali] Nuovo ruolo inserito

IDGruppo 			  = [system_id - tabella groups] Nuovo ruolo inserito

LivelloRuolo		  = [num_livello - tabella dpa_tipo_uolo] Livello del nuovo ruolo

IDRegistro		      = [system_id - tabella dpa_el_registri] Registro associato al
			            nuovo ruolo

PariLivello		      = chiave sul web.config della WS : EST_VIS_SUP_PARI_LIV
*/

BEGIN
     returnValue := 0;

	 /* STESSA UO */

	 IF PariLivello = 0 THEN
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
            	  		  SELECT /*+ index(s) index(p) */
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 s.ID_GRUPPO_TRASM,
								 'A'
						  FROM SECURITY s,PROFILE p
                          WHERE p.system_id = s.thing
                          AND p.cha_privato = '0'
                          AND (p.id_registro = IDRegistro OR p.id_registro IS NULL)
                          AND s.personorgroup IN (
						  	  SELECT
							  		c.SYSTEM_ID
                        	  FROM  DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
                        	  WHERE a.SYSTEM_ID = b.ID_TIPO_RUOLO
                        	  AND b.id_gruppo = c.system_id
                        	  AND d.id_ruolo_in_uo = b.system_id
                        	  AND b.CHA_TIPO_URP = 'R'
                        	  AND b.CHA_TIPO_IE = 'I'
                        	  AND b.DTA_FINE IS NULL
                        	  AND a.NUM_LIVELLO > LivelloRuolo
                        	  AND b.ID_UO = IDCorrGlobaleUO
                        	  AND d.id_registro = IDRegistro)
                          AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 1;
				 RETURN;
		END;
	 ELSE
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
             	  		  SELECT /*+ index(s) index(p) */
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 s.ID_GRUPPO_TRASM,
								 'A'
						  FROM SECURITY s,PROFILE p
                          WHERE p.system_id = s.thing
                          AND p.cha_privato = '0'
                          AND (p.id_registro = IDRegistro OR p.id_registro IS NULL)
                          AND s.personorgroup IN (
						  	  SELECT
							  		c.SYSTEM_ID
                        	  FROM  DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
                        	  WHERE a.SYSTEM_ID = b.ID_TIPO_RUOLO
                        	  AND b.id_gruppo = c.system_id
                        	  AND d.id_ruolo_in_uo = b.system_id
                        	  AND b.CHA_TIPO_URP = 'R'
                        	  AND b.CHA_TIPO_IE = 'I'
                        	  AND b.DTA_FINE IS NULL
                        	  AND a.NUM_LIVELLO >= LivelloRuolo
                        	  AND b.ID_UO = IDCorrGlobaleUO
                        	  AND d.id_registro = IDRegistro)
                          AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 1;
				 RETURN;
		END;
	 END IF;

	 /* UO INFERIORI */

	 IF PariLivello = 0 THEN
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
            	  		  SELECT /*+ index(s) index(p) */
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 s.ID_GRUPPO_TRASM,
								 'A'
						  FROM SECURITY s,PROFILE p
                          WHERE p.system_id = s.thing
                          AND p.cha_privato = '0'
                          AND (p.id_registro = IDRegistro OR p.id_registro IS NULL)
                          AND s.personorgroup IN (
						  	  SELECT
							  		c.SYSTEM_ID
                        	  FROM  DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
                        	  WHERE a.SYSTEM_ID = b.ID_TIPO_RUOLO
                        	  AND b.id_gruppo = c.system_id
                        	  AND d.id_ruolo_in_uo = b.system_id
                        	  AND b.CHA_TIPO_URP = 'R'
                        	  AND b.CHA_TIPO_IE = 'I'
                        	  AND b.DTA_FINE IS NULL
                        	  AND a.NUM_LIVELLO > LivelloRuolo
                        	  AND d.id_registro = IDRegistro
							  AND b.ID_UO IN (
							  	  	SELECT system_id
									FROM DPA_CORR_GLOBALI
									WHERE cha_tipo_ie = 'I'
									AND dta_fine IS NULL
									AND id_old = 0
									START WITH id_parent = IDCorrGlobaleUO
									CONNECT BY PRIOR SYSTEM_ID = ID_PARENT)
							  )
							  AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id)
                           AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 1;
				 RETURN;
		END;
	 ELSE
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
             	  		  SELECT /*+ index(s) index(p) */
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 s.ID_GRUPPO_TRASM,
								 'A'
						  FROM SECURITY s,PROFILE p
                          WHERE p.system_id = s.thing
                          AND p.cha_privato = '0'
                          AND (p.id_registro = IDRegistro OR p.id_registro IS NULL)
                          AND s.personorgroup IN (
						  	  SELECT
							  		c.SYSTEM_ID
                        	  FROM  DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
                        	  WHERE a.SYSTEM_ID = b.ID_TIPO_RUOLO
                        	  AND b.id_gruppo = c.system_id
                        	  AND d.id_ruolo_in_uo = b.system_id
                        	  AND b.CHA_TIPO_URP = 'R'
                        	  AND b.CHA_TIPO_IE = 'I'
                        	  AND b.DTA_FINE IS NULL
                        	  AND a.NUM_LIVELLO >= LivelloRuolo
                        	  AND d.id_registro = IDRegistro
							  AND b.ID_UO IN (
							  	  	SELECT system_id
									FROM DPA_CORR_GLOBALI
									WHERE cha_tipo_ie = 'I'
									AND dta_fine IS NULL
									AND id_old = 0
									START WITH id_parent = IDCorrGlobaleUO
									CONNECT BY PRIOR SYSTEM_ID = ID_PARENT)
							  )
						 AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 1;
				 RETURN;
		END;
	 END IF;

END;
