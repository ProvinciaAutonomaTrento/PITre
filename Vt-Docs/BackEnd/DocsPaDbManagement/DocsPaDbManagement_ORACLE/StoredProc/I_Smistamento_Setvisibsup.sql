CREATE OR REPLACE PROCEDURE I_Smistamento_Setvisibsup (
		IDCorrGlobaleRuolo IN NUMBER,
		IDGruppoMittente IN NUMBER,
		LivelloRuoloMittente IN NUMBER,
		IDDocumento IN NUMBER,
		PariLivello IN NUMBER,
		DirittoDaEred IN NUMBER,
		ReturnValue OUT NUMBER)	IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione della visibilità dei ruoli superiori gerarchici
-- nello smistamento di un documento.
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- 1: Altrimenti
-------------------------------------------------------------------------------------------------------
*/

ExistIDGruppiSuperiori CHAR(1) := 'Y';
recordCorrente NUMBER;
idGruppoSuperiore NUMBER;
ExistAccessRights CHAR(1) := 'Y';
AccessRights NUMBER:= NULL;
accessRightsValue NUMBER := NULL;
MyUo NUMBER := NULL;
IDParent NUMBER := NULL;
IDUnitaOrganizzativa NUMBER := NULL;
LivelloRuolo NUMBER := NULL;
IDRegistro NUMBER := NULL;

		/* prende tutti i ruoli di livello superiore a quello passato (REGISTRO ESISTENTE) */
		CURSOR cursor_IDGruppiSuperioriReg (IDUOValue NUMBER, LivelloRuoloMittente NUMBER) IS
			SELECT 		CG.ID_GRUPPO
			FROM		DPA_CORR_GLOBALI CG,
						DPA_TIPO_RUOLO TR,
						DPA_L_RUOLO_REG RR
			WHERE		CG.ID_UO = IDUOValue
						AND
						CG.CHA_TIPO_URP = 'R'
						AND
						CG.DTA_FINE IS NULL
						AND
						CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
						AND
						CG.SYSTEM_ID = RR.ID_RUOLO_IN_UO
						AND
						RR.ID_REGISTRO = IDRegistro
						AND
						TR.NUM_LIVELLO < LivelloRuoloMittente;
		
		/* prende tutti i ruoli di livello superiore e uguale a quello passato (REGISTRO ESISTENTE) */						
		CURSOR cursor_IDGruppiSup_PariLivReg (IDUOValue NUMBER, LivelloRuoloMittente NUMBER) IS
			SELECT 		CG.ID_GRUPPO
			FROM		DPA_CORR_GLOBALI CG,
						DPA_TIPO_RUOLO TR,
						DPA_L_RUOLO_REG RR
			WHERE		CG.ID_UO = IDUOValue
						AND
						CG.CHA_TIPO_URP = 'R'
						AND
						CG.DTA_FINE IS NULL
						AND
						CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
						AND
						CG.SYSTEM_ID = RR.ID_RUOLO_IN_UO
						AND
						RR.ID_REGISTRO = IDRegistro
						AND
						TR.NUM_LIVELLO <= LivelloRuoloMittente;
						
        /* prende tutti i ruoli di livello superiore a quello passato (NO REGISTRO) */
		CURSOR cursor_IDGruppiSuperiori (IDUOValue NUMBER, LivelloRuoloMittente NUMBER) IS
			SELECT 		CG.ID_GRUPPO
			FROM		DPA_CORR_GLOBALI CG,
						DPA_TIPO_RUOLO TR
			WHERE		ID_UO = IDUOValue
						AND
						CHA_TIPO_URP = 'R'
						AND
						DTA_FINE IS NULL
						AND
						CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
						AND
						TR.NUM_LIVELLO < LivelloRuoloMittente;

        /* prende tutti i ruoli di livello superiore e uguale a quello passato (NO REGISTRO) */						
		CURSOR cursor_IDGruppiSup_PariLiv (IDUOValue NUMBER, LivelloRuoloMittente NUMBER) IS
			SELECT 		CG.ID_GRUPPO
			FROM		DPA_CORR_GLOBALI CG,
						DPA_TIPO_RUOLO TR
			WHERE		ID_UO = IDUOValue
						AND
						CHA_TIPO_URP = 'R'
						AND
						DTA_FINE IS NULL
						AND
						CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
						AND
						TR.NUM_LIVELLO <= LivelloRuoloMittente;

BEGIN
	 /* prende il registro del documento */
	 BEGIN
	 	  SELECT ID_REGISTRO INTO IDRegistro
		  FROM PROFILE
		  WHERE SYSTEM_ID = IDDocumento;	
	 EXCEPTION
		WHEN NO_DATA_FOUND THEN
			 IDRegistro := NULL;
	 END;

	 /* visibilita' ai ruoli superiori della stessa UO ------------------------------------------ */	 
	 BEGIN
	 	 SELECT id_uo INTO MyUO
		 FROM DPA_CORR_GLOBALI
		 WHERE system_id = IDCorrGlobaleRuolo;
	 END;

	 BEGIN
		SELECT TR.NUM_LIVELLO INTO LivelloRuolo
		FROM DPA_CORR_GLOBALI CG,
			 DPA_TIPO_RUOLO TR
		WHERE CG.SYSTEM_ID = IDCorrGlobaleRuolo
			  AND
			  CG.ID_TIPO_RUOLO = TR.SYSTEM_ID;
	 END;

	 BEGIN	 
	 	 IF IDRegistro = NULL THEN		 
		 	BEGIN
				 /* SENZA REGISTRO (DOC. GRIGIO) */
			 	 OPEN cursor_IDGruppiSuperiori(MyUO,LivelloRuolo);
				 LOOP
				 	 FETCH cursor_IDGruppiSuperiori INTO recordCorrente;
					 EXIT WHEN cursor_IDGruppiSuperiori%NOTFOUND;
					 idGruppoSuperiore := recordCorrente;
		
					 -- Verifica se non vi sia già una trasmissione per il documento:
					-- - se presente, si distinguono 2 casi:
					--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
					--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
					--	2) altrimenti non fa nulla
					-- - se non presente viene fatta in ogni caso la insert con
					--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
					BEGIN
						SELECT ACCESSRIGHTS INTO AccessRights FROM (
							SELECT 	ACCESSRIGHTS
							FROM 	SECURITY
							WHERE 	THING = IDDocumento
									AND
									PERSONORGROUP = idGruppoSuperiore
									AND
									CHA_TIPO_DIRITTO = 'A'
									ORDER BY ACCESSRIGHTS DESC
						) WHERE ROWNUM = 1;
					EXCEPTION
						WHEN NO_DATA_FOUND THEN
							 ExistAccessRights := 'N';
					END;
		
					IF ExistAccessRights = 'Y' THEN
		
					   	accessRightsValue := AccessRights;
		
						IF accessRightsValue < DirittoDaEred THEN
						   BEGIN
								/* aggiornamento a Diritto */
								UPDATE 	SECURITY
								SET 	ACCESSRIGHTS = DirittoDaEred
								WHERE 	THING = IDDocumento
										AND
										PERSONORGROUP = idGruppoSuperiore
										AND
										CHA_TIPO_DIRITTO = 'A'
										AND ACCESSRIGHTS = accessRightsValue;
							EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;
							END;
						END IF;
					ELSE
						BEGIN
							/* inserimento a Diritto */
							INSERT INTO SECURITY
							(
								THING,
								PERSONORGROUP,
								ACCESSRIGHTS,
								ID_GRUPPO_TRASM,
								CHA_TIPO_DIRITTO
							)
							VALUES
							(
								IDDocumento,
								idGruppoSuperiore,
								DirittoDaEred,
								IDGruppoMittente,
								'A'
							);
						EXCEPTION
							WHEN DUP_VAL_ON_INDEX THEN
								 NULL;							
						END;
					END IF;
		
				 END LOOP;
				 CLOSE cursor_IDGruppiSuperiori;
			END;
		
		ELSE
		
			BEGIN
 				 /* ESISTE UN REGISTRO (DOC. PROTOCOLLATO) */
				 OPEN cursor_IDGruppiSuperioriReg(MyUO,LivelloRuolo);
				 LOOP
				 	 FETCH cursor_IDGruppiSuperioriReg INTO recordCorrente;
					 EXIT WHEN cursor_IDGruppiSuperioriReg%NOTFOUND;
					 idGruppoSuperiore := recordCorrente;
		
					 -- Verifica se non vi sia già una trasmissione per il documento:
					-- - se presente, si distinguono 2 casi:
					--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
					--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
					--	2) altrimenti non fa nulla
					-- - se non presente viene fatta in ogni caso la insert con
					--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
					BEGIN
						SELECT ACCESSRIGHTS INTO AccessRights FROM (
							SELECT 	ACCESSRIGHTS
							FROM 	SECURITY
							WHERE 	THING = IDDocumento
									AND
									PERSONORGROUP = idGruppoSuperiore
									AND
									CHA_TIPO_DIRITTO = 'A'
									ORDER BY ACCESSRIGHTS DESC
						) WHERE ROWNUM = 1;
					EXCEPTION
						WHEN NO_DATA_FOUND THEN
							 ExistAccessRights := 'N';
					END;
		
					IF ExistAccessRights = 'Y' THEN
		
					   	accessRightsValue := AccessRights;
		
						IF accessRightsValue < DirittoDaEred THEN
						   BEGIN
								/* aggiornamento a Diritto */
								UPDATE 	SECURITY
								SET 	ACCESSRIGHTS = DirittoDaEred
								WHERE 	THING = IDDocumento
										AND
										PERSONORGROUP = idGruppoSuperiore
										AND
										CHA_TIPO_DIRITTO = 'A'
										AND ACCESSRIGHTS = accessRightsValue;
							EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;
							END;
						END IF;
					ELSE
						BEGIN
							/* inserimento a Diritto */
							INSERT INTO SECURITY
							(
								THING,
								PERSONORGROUP,
								ACCESSRIGHTS,
								ID_GRUPPO_TRASM,
								CHA_TIPO_DIRITTO
							)
							VALUES
							(
								IDDocumento,
								idGruppoSuperiore,
								DirittoDaEred,
								IDGruppoMittente,
								'A'
							);
						EXCEPTION
							WHEN DUP_VAL_ON_INDEX THEN
								 NULL;	
						END;
					END IF;
		
				 END LOOP;
				 CLOSE cursor_IDGruppiSuperioriReg;			
			END;		
		
		END IF;
	 END;


	 /* visibilita' ai ruoli superiori delle UO superiori ------------------------------------*/
	 LOOP
	 	 BEGIN
		 	 SELECT id_parent INTO IDUnitaOrganizzativa
			 FROM DPA_CORR_GLOBALI
			 WHERE system_id = MyUO;
		 END;

		 IF IDUnitaOrganizzativa = 0 THEN
		 	EXIT;
		 ELSE
		 	MyUO := IDUnitaOrganizzativa;
		 END IF;

		 IF PariLivello = 0 THEN
		 	BEGIN
				 ---------------------------------------------------------------------
				 ---------------------------------------------------------------------
				 --			INVIA AI GRUPPI DI LIVELLO SUPERIORE
				 ---------------------------------------------------------------------
				 ---------------------------------------------------------------------
				 IF IDRegistro = NULL THEN
				 	BEGIN
					      /* SENZA REGISTRO (DOC. GRIGIO) */
					 	 OPEN cursor_IDGruppiSuperiori(IDUnitaOrganizzativa,LivelloRuolo);
						 LOOP
						 	 FETCH cursor_IDGruppiSuperiori INTO recordCorrente;
							 EXIT WHEN cursor_IDGruppiSuperiori%NOTFOUND;
							 idGruppoSuperiore := recordCorrente;
			
							 -- Verifica se non vi sia già una trasmissione per il documento:
							-- - se presente, si distinguono 2 casi:
							--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
							--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
							--	2) altrimenti non fa nulla
							-- - se non presente viene fatta in ogni caso la insert con
							--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
							BEGIN
								SELECT ACCESSRIGHTS INTO AccessRights FROM (
									SELECT 	ACCESSRIGHTS
									FROM 	SECURITY
									WHERE 	THING = IDDocumento
											AND
											PERSONORGROUP = idGruppoSuperiore
											AND
											CHA_TIPO_DIRITTO = 'A'
											ORDER BY ACCESSRIGHTS DESC
								 ) WHERE ROWNUM = 1;
							EXCEPTION
								WHEN NO_DATA_FOUND THEN
									 ExistAccessRights := 'N';
							END;
			
							IF ExistAccessRights = 'Y' THEN
			
							   	accessRightsValue := AccessRights;
			
			
								IF accessRightsValue < DirittoDaEred THEN
								   BEGIN
										/* aggiornamento a DirittoDaEred */
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS = DirittoDaEred
										WHERE 	THING = IDDocumento
												AND
												PERSONORGROUP = idGruppoSuperiore
												AND
												CHA_TIPO_DIRITTO = 'A'
												AND ACCESSRIGHTS = accessRightsValue;
									EXCEPTION
										WHEN DUP_VAL_ON_INDEX THEN
											NULL;
									END;
								END IF;
							ELSE
								BEGIN
									/* inserimento a DirittoDaEred */
									INSERT INTO SECURITY
									(
										THING,
										PERSONORGROUP,
										ACCESSRIGHTS,
										ID_GRUPPO_TRASM,
										CHA_TIPO_DIRITTO
									)
									VALUES
									(
										IDDocumento,
										idGruppoSuperiore,
										DirittoDaEred,
										IDGruppoMittente,
										'A'
									);
								EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;	
								END;
							END IF;
						
						 END LOOP;
						 CLOSE cursor_IDGruppiSuperiori;
					END;
				ELSE
					BEGIN
						 /* ESISTE UN REGISTRO (DOC. PROTOCOLLATO) */
						 OPEN cursor_IDGruppiSuperioriReg(IDUnitaOrganizzativa,LivelloRuolo);
						 LOOP
						 	 FETCH cursor_IDGruppiSuperioriReg INTO recordCorrente;
							 EXIT WHEN cursor_IDGruppiSuperioriReg%NOTFOUND;
							 idGruppoSuperiore := recordCorrente;
			
							 -- Verifica se non vi sia già una trasmissione per il documento:
							-- - se presente, si distinguono 2 casi:
							--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
							--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
							--	2) altrimenti non fa nulla
							-- - se non presente viene fatta in ogni caso la insert con
							--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
							BEGIN
								SELECT ACCESSRIGHTS INTO AccessRights FROM (
									SELECT 	ACCESSRIGHTS
									FROM 	SECURITY
									WHERE 	THING = IDDocumento
											AND
											PERSONORGROUP = idGruppoSuperiore
											AND
											CHA_TIPO_DIRITTO = 'A'
											ORDER BY ACCESSRIGHTS DESC
								 ) WHERE ROWNUM = 1;
							EXCEPTION
								WHEN NO_DATA_FOUND THEN
									 ExistAccessRights := 'N';
							END;
			
							IF ExistAccessRights = 'Y' THEN
			
							   	accessRightsValue := AccessRights;
			
			
								IF accessRightsValue < DirittoDaEred THEN
								   BEGIN
										/* aggiornamento a DirittoDaEred */
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS = DirittoDaEred
										WHERE 	THING = IDDocumento
												AND
												PERSONORGROUP = idGruppoSuperiore
												AND
												CHA_TIPO_DIRITTO = 'A'
												AND ACCESSRIGHTS = accessRightsValue;
									EXCEPTION
										WHEN DUP_VAL_ON_INDEX THEN
											NULL;
									END;
								END IF;
							ELSE
								BEGIN
									/* inserimento a DirittoDaEred */
									INSERT INTO SECURITY
									(
										THING,
										PERSONORGROUP,
										ACCESSRIGHTS,
										ID_GRUPPO_TRASM,
										CHA_TIPO_DIRITTO
									)
									VALUES
									(
										IDDocumento,
										idGruppoSuperiore,
										DirittoDaEred,
										IDGruppoMittente,
										'A'
									);
								EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;	
								END;
							END IF;
						
						 END LOOP;
						 CLOSE cursor_IDGruppiSuperioriReg;
					END;
				END IF;
			END;
		ELSE
			BEGIN
			 	 ---------------------------------------------------------------------
				 ---------------------------------------------------------------------
				 --			INVIA AI GRUPPI DI LIVELLO UGUALE / SUPERIORE
				 ---------------------------------------------------------------------
				 ---------------------------------------------------------------------
				 IF IDRegistro = NULL THEN
				 	 BEGIN
					 	 /* SENZA REGISTRO (DOC. GRIGIO) */
						 OPEN cursor_IDGruppiSup_PariLiv(IDUnitaOrganizzativa,LivelloRuolo);
						 LOOP
						 	 FETCH cursor_IDGruppiSup_PariLiv INTO recordCorrente;
							 EXIT WHEN cursor_IDGruppiSup_PariLiv%NOTFOUND;
							 idGruppoSuperiore := recordCorrente;
			
							 -- Verifica se non vi sia già una trasmissione per il documento:
							-- - se presente, si distinguono 2 casi:
							--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
							--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
							--	2) altrimenti non fa nulla
							-- - se non presente viene fatta in ogni caso la insert con
							--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
							BEGIN
								SELECT ACCESSRIGHTS INTO AccessRights FROM (
									SELECT 	ACCESSRIGHTS
									FROM 	SECURITY
									WHERE 	THING = IDDocumento
											AND
											PERSONORGROUP = idGruppoSuperiore
											AND
											CHA_TIPO_DIRITTO = 'A'
											ORDER BY ACCESSRIGHTS DESC
								 ) WHERE ROWNUM = 1;
							EXCEPTION
								WHEN NO_DATA_FOUND THEN
									 ExistAccessRights := 'N';
							END;
			
							IF ExistAccessRights = 'Y' THEN
			
							   	accessRightsValue := AccessRights;
			
			
								IF accessRightsValue < DirittoDaEred THEN
								   BEGIN
										/* aggiornamento a DirittoDaEred */
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS = DirittoDaEred
										WHERE 	THING = IDDocumento
												AND
												PERSONORGROUP = idGruppoSuperiore
												AND
												CHA_TIPO_DIRITTO = 'A'
												AND ACCESSRIGHTS = accessRightsValue;
									EXCEPTION
										WHEN DUP_VAL_ON_INDEX THEN
											NULL;
									END;
								END IF;
							ELSE
								BEGIN
									/* inserimento a DirittoDaEred */
									INSERT INTO SECURITY
									(
										THING,
										PERSONORGROUP,
										ACCESSRIGHTS,
										ID_GRUPPO_TRASM,
										CHA_TIPO_DIRITTO
									)
									VALUES
									(
										IDDocumento,
										idGruppoSuperiore,
										DirittoDaEred,
										IDGruppoMittente,
										'A'
									);
								EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;	
								END;
							END IF;			
			
						 END LOOP;
						 CLOSE cursor_IDGruppiSup_PariLiv;
					END;				
				ELSE			
					BEGIN	
						 /* ESISTE UN REGISTRO (DOC. PROTOCOLLATO) */				
						 OPEN cursor_IDGruppiSup_PariLivReg(IDUnitaOrganizzativa,LivelloRuolo);
						 LOOP
						 	 FETCH cursor_IDGruppiSup_PariLivReg INTO recordCorrente;
							 EXIT WHEN cursor_IDGruppiSup_PariLivReg%NOTFOUND;
							 idGruppoSuperiore := recordCorrente;
			
							 -- Verifica se non vi sia già una trasmissione per il documento:
							-- - se presente, si distinguono 2 casi:
							--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato)
							--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
							--	2) altrimenti non fa nulla
							-- - se non presente viene fatta in ogni caso la insert con
							--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
							BEGIN
								SELECT ACCESSRIGHTS INTO AccessRights FROM (
									SELECT 	ACCESSRIGHTS
									FROM 	SECURITY
									WHERE 	THING = IDDocumento
											AND
											PERSONORGROUP = idGruppoSuperiore
											AND
											CHA_TIPO_DIRITTO = 'A'
											ORDER BY ACCESSRIGHTS DESC
								 ) WHERE ROWNUM = 1;
							EXCEPTION
								WHEN NO_DATA_FOUND THEN
									 ExistAccessRights := 'N';
							END;
			
							IF ExistAccessRights = 'Y' THEN
			
							   	accessRightsValue := AccessRights;
			
			
								IF accessRightsValue < DirittoDaEred THEN
								   BEGIN
										/* aggiornamento a DirittoDaEred */
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS = DirittoDaEred
										WHERE 	THING = IDDocumento
												AND
												PERSONORGROUP = idGruppoSuperiore
												AND
												CHA_TIPO_DIRITTO = 'A'
												AND ACCESSRIGHTS = accessRightsValue;
									EXCEPTION
										WHEN DUP_VAL_ON_INDEX THEN
											NULL;
									END;
								END IF;
							ELSE
								BEGIN
									/* inserimento a DirittoDaEred */
									INSERT INTO SECURITY
									(
										THING,
										PERSONORGROUP,
										ACCESSRIGHTS,
										ID_GRUPPO_TRASM,
										CHA_TIPO_DIRITTO
									)
									VALUES
									(
										IDDocumento,
										idGruppoSuperiore,
										DirittoDaEred,
										IDGruppoMittente,
										'A'
									);
								EXCEPTION
									WHEN DUP_VAL_ON_INDEX THEN
										 NULL;	
								END;
							END IF;			
			
						 END LOOP;
						 CLOSE cursor_IDGruppiSup_PariLivReg;
					END;					
				END IF;
				
			END;
		END IF;

	END LOOP;

	ReturnValue := 0;

END;
/