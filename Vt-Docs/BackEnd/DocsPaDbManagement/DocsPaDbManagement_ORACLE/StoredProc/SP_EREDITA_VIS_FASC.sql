CREATE OR REPLACE PROCEDURE Sp_Eredita_Vis_Fasc (
	   	  IDCorrGlobaleUO IN NUMBER,
		  IDCorrGlobaleRuolo IN NUMBER,
		  IDGruppo IN NUMBER,
		  LivelloRuolo IN NUMBER,
		  IDRegistro IN NUMBER,
		  PariLivello IN NUMBER,
		  returnValue OUT NUMBER) IS

/*
.........................................................................................
SP_EREDITA_VIS_FASC
.........................................................................................
EREDITARIETA' DELLA VISIBILITA' DEL TITOLARIO DA PARTE DI UN NUOVO RUOLO IN ORGANIGRAMMA
.........................................................................................
SP per il processo di inserimento della visibilità da ereditare sul titolario.

VALORI DI INPUT:

IDCorrGlobaleUO   	  = [system_id - tabella dpa_corr_globali] UO nella quale è stato
		   		        inserito il nuovo ruolo;

IDCorrGlobaleRuolo	  = [system_id - tabella dpa_corr_globali] Nuovo ruolo inserito

IDGruppo 			  = [system_id - tabella groups] Nuovo ruolo inserito

LivelloRuolo		  = [num_livello - tabella dpa_tipo_uolo] Livello del nuovo ruolo

IDRegistro		      = [system_id - tabella dpa_el_registri] Registro associato al
			            nuovo ruolo

PariLivello		      = chiave sul web.config della WS : EST_VIS_SUP_PARI_LIV
					  	(ATTENZIONE! è sempre uguale a ZERO)
*/

BEGIN
     returnValue := 0;
	 BEGIN
		 /* inserisce prima tutti i nodi di titolario associati al registro*/
		 INSERT  INTO SECURITY
		 (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
				SELECT THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO FROM (
					   SELECT /*+ index (a) */ DISTINCT a.system_id AS THING,IDGruppo AS PERSONORGROUP,255 AS ACCESSRIGHTS,NULL AS ID_GRUPPO_TRASM,'P' AS CHA_TIPO_DIRITTO
				       FROM PROJECT a
					   WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=a.system_id)
					   AND ((a.cha_tipo_proj = 'T' AND a.id_registro = IDRegistro) OR (a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'G' AND a.id_registro = IDRegistro))
				) UNION (
				  	   SELECT /*+ index (b) */DISTINCT b.system_id AS THING,IDGruppo AS PERSONORGROUP,255 AS ACCESSRIGHTS,NULL AS ID_GRUPPO_TRASM,'P' AS CHA_TIPO_DIRITTO
				       FROM PROJECT b
				       WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=b.system_id)
					   AND b.cha_tipo_proj = 'C'
				       AND id_parent IN (
				             SELECT /*+ index (project) */ system_id
				             FROM PROJECT
				             WHERE cha_tipo_proj = 'F'
				             AND cha_tipo_fascicolo = 'G'
				             AND id_registro = IDRegistro)
				);

	  EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 1;
				 RETURN;
	  END;

	 /* STESSA UO */
	 IF PariLivello = 0 THEN
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
            	  		  SELECT /*+ index(s) index(p) */ DISTINCT
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 NULL,
								 'A'
						  FROM SECURITY s,PROJECT p
                          WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=p.system_id)
						  AND p.system_id = s.thing
                          AND p.system_id IN (
						  	    SELECT * FROM (
									   SELECT /*+ index (a ) */ a.system_id
								       FROM PROJECT a
									   WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND a.id_registro = IDRegistro)
								) UNION (
								  	   SELECT /*+ index (b )  */ b.system_id
								       FROM PROJECT b
								       WHERE b.cha_tipo_proj = 'C'
								       AND b.id_parent IN (
								             SELECT /*+ index (project) */ system_id
								             FROM PROJECT
								             WHERE cha_tipo_proj = 'F'
								             AND cha_tipo_fascicolo = 'P'
								             AND id_registro = IDRegistro)
								)
						  )
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
                        	  AND d.id_registro = IDRegistro);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 2;
				 RETURN;
		END;
	 ELSE
	 	BEGIN
			 INSERT  INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
             	  		  SELECT /*+ index(s) index(p) */ DISTINCT
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 NULL,
								 'A'
						  FROM SECURITY s,PROJECT p
                          WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=p.system_id)
					      AND p.system_id = s.thing
                          AND p.system_id IN (
						  	    SELECT * FROM (
									   SELECT /*+ index (a ) */ a.system_id
								       FROM PROJECT a
									   WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND a.id_registro = IDRegistro)
								) UNION (
								  	   SELECT /*+ index (b )  */ b.system_id
								       FROM PROJECT b
								       WHERE b.cha_tipo_proj = 'C'
								       AND b.id_parent IN (
								             SELECT /*+ index (project) */ system_id
								             FROM PROJECT
								             WHERE cha_tipo_proj = 'F'
								             AND cha_tipo_fascicolo = 'P'
								             AND id_registro = IDRegistro)
								)
						  )
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
                        	  AND d.id_registro = IDRegistro);
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 3;
				 RETURN;
		END;
	 END IF;


	 /* UO INFERIORI */
	 IF PariLivello = 0 THEN
	 	BEGIN
			 INSERT INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
            	  		  SELECT /*+ index(s) index(p) */ DISTINCT
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 NULL,
								 'A'
						  FROM SECURITY s,PROJECT p
                          WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=p.system_id)
					      AND p.system_id = s.thing
                          AND p.system_id IN (
						  	    SELECT * FROM (
									   SELECT /*+ index (a ) */ a.system_id
								       FROM PROJECT a
									   WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND a.id_registro = IDRegistro)
								) UNION (
								  	   SELECT /*+ index (b )  */ b.system_id
								       FROM PROJECT b
								       WHERE b.cha_tipo_proj = 'C'
								       AND b.id_parent IN (
								             SELECT /*+ index (project) */ system_id
								             FROM PROJECT
								             WHERE cha_tipo_proj = 'F'
								             AND cha_tipo_fascicolo = 'P'
								             AND id_registro = IDRegistro)
								)
						  )
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
							  );
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 4;
				 RETURN;
		END;
	 ELSE
	 	BEGIN
			 INSERT  INTO SECURITY
                  (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
             	  		  SELECT /*+ index(s) index(p) */ DISTINCT
						  		 s.THING,
								 IDGruppo,
						  		 (CASE WHEN
								 	   (s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
                     			 THEN
								 	  63
								 ELSE
								 	  s.accessrights
								 END)  AS acr,
								 NULL,
								 'A'
						  FROM SECURITY s,PROJECT p
                          WHERE not exists (select 'x' from security s1 where s1.PERSONORGROUP=IDGruppo and s1.THING=p.system_id)
					      AND p.system_id = s.thing
                          AND p.system_id IN (
						  	    SELECT * FROM (
									   SELECT /*+ index (a ) */ a.system_id
								       FROM PROJECT a
									   WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND a.id_registro = IDRegistro)
								) UNION (
								  	   SELECT /*+ index (b )  */ b.system_id
								       FROM PROJECT b
								       WHERE b.cha_tipo_proj = 'C'
								       AND b.id_parent IN (
								             SELECT /*+ index (project) */ system_id
								             FROM PROJECT
								             WHERE cha_tipo_proj = 'F'
								             AND cha_tipo_fascicolo = 'P'
								             AND id_registro = IDRegistro)
								)
						  )
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
							  );
		EXCEPTION
	   		WHEN DUP_VAL_ON_INDEX THEN
				 NULL;
			WHEN OTHERS THEN
				 returnValue := 5;
				 RETURN;
		END;
	 END IF;

END;
/

