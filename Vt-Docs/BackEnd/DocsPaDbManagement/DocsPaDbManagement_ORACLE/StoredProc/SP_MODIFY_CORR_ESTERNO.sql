CREATE OR REPLACE PROCEDURE SP_MODIFY_CORR_ESTERNO
	(   IDCorrGlobale IN NUMBER,  
	desc_corr IN VARCHAR2,
	nome IN VARCHAR2,
	cognome IN VARCHAR2,
	codice_aoo IN VARCHAR2,
	codice_amm IN VARCHAR2,
	email IN VARCHAR2,
	indirizzo IN VARCHAR2,
	cap IN VARCHAR2,
	provincia IN VARCHAR2,
	nazione IN VARCHAR2,
	citta IN VARCHAR2,
	cod_fiscale IN VARCHAR2,
	telefono IN VARCHAR2,
	telefono2 IN VARCHAR2,
	note IN VARCHAR2,
	fax IN VARCHAR2,
	var_idDocType NUMBER,
	ReturnValue OUT NUMBER
	)
  
  IS 
  
  BEGIN
  
  	   DECLARE
			cod_rubrica VARCHAR2(128);
			id_reg NUMBER;
			idAmm NUMBER;
			new_var_cod_rubrica VARCHAR2(128);
			cha_dettaglio CHAR(1):= '0';
			cha_tipo_urp CHAR(1);
			myProfile NUMBER;
			new_IDCorrGlobale NUMBER;
			IdentityCorrGlobali NUMBER;
			IdentityDettGlobali NUMBER;
			OutValue  NUMBER := 1; 
			RTN NUMBER;
			v_id_docType NUMBER;
			IdentityDpaTCanaleCorr NUMBER;
  		
  	   BEGIN
  
			<<REPERIMENTO_DATI>>
			BEGIN
			
					SELECT		  
						VAR_COD_RUBRICA, 
						CHA_TIPO_URP,  
						ID_REGISTRO,
						ID_AMM 
					INTO 
						cod_rubrica, 
						cha_tipo_urp, 
						id_reg, 
						idAmm
					FROM DPA_CORR_GLOBALI 
					WHERE system_id = IDCorrGlobale;
				
					EXCEPTION 
						  WHEN NO_DATA_FOUND THEN
						  OutValue := 0;
						  return;
		   
	   	    END REPERIMENTO_DATI;
			
			<<DATI_CANALE_UTENTE>>
			BEGIN
				 
					 SELECT ID_DOCUMENTTYPE into v_id_docType
					 FROM DPA_T_CANALE_CORR
					 WHERE ID_CORR_GLOBALE = IDCorrGlobale;
					 
					 EXCEPTION 
							  WHEN NO_DATA_FOUND THEN
							  OutValue := 0; 
							  
			END DATI_CANALE_UTENTE;
			
			IF /* 0 */ OutValue = 1 THEN
					
						
			 		IF /* 1 */ cha_tipo_urp='U' OR cha_tipo_urp='P' THEN
					   cha_dettaglio := '1';
					END IF; /* 1 */ 
					
					--VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli		
					SELECT count(ID_PROFILE)
					INTO myProfile
					FROM DPA_DOC_ARRIVO_PAR 
					WHERE ID_MITT_DEST = IDCorrGlobale;
					
				
					-- 1) non è stato mai utilizzato come corrisp in un protocollo    
					
					IF /* 2 */ (myProfile = 0) THEN
			    
					   	BEGIN 
							
							UPDATE 	DPA_CORR_GLOBALI 
							SET VAR_CODICE_AOO=codice_aoo, 
							VAR_CODICE_AMM = codice_amm, 
							VAR_EMAIL = email,
							VAR_DESC_CORR = desc_corr,
							VAR_NOME = nome,
							VAR_COGNOME= cognome
							WHERE 	SYSTEM_ID = IDCorrGlobale;

						EXCEPTION 
							WHEN OTHERS THEN
		 	  	  		  	   	 OutValue:= 0;
								 RETURN;
								 
						END;
						
						/* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE  
						PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI  
						*/
						
						IF /* 3 */	cha_tipo_urp='U' OR cha_tipo_urp='P' THEN
							    
								<<UPDATE_DPA_DETT_GLOBALI>>
								BEGIN
										UPDATE DPA_DETT_GLOBALI
										SET VAR_INDIRIZZO = indirizzo, 
										VAR_CAP = cap, 
										VAR_PROVINCIA = provincia, 
										VAR_NAZIONE = nazione, 
										VAR_COD_FISCALE = cod_fiscale, 
										VAR_TELEFONO = telefono, 
										VAR_TELEFONO2 = telefono2, 
										VAR_NOTE= note, 
										VAR_CITTA= citta,
										VAR_FAX = fax
										WHERE ID_CORR_GLOBALI =IDCorrGlobale; 
										
								EXCEPTION 
										  WHEN OTHERS THEN
										  OutValue:= 0;
										  RETURN;
										  
								END UPDATE_DPA_DETT_GLOBALI;		
							
						END IF; /* 3 */
						
						--METTI QUI UPDATE SU DPA_T_CANALE_CORR
						IF /* 5 */var_idDocType != v_id_docType THEN
						
						   -- SE ENTRO QUI VUOL DIRE CHE IL TIPO_CANALE DEL CORRISP
						   --ESTERNO è STATO CAMBIATO, QUINDI AGGIORNO LA DPA_T_CANALE_CORR   
						   BEGIN 
						   
						   		 UPDATE DPA_T_CANALE_CORR 
						   		 SET ID_DOCUMENTTYPE = var_idDocType
						   		 WHERE ID_CORR_GLOBALE = IDCorrGlobale;
								 
								 
								 EXCEPTION 
									  WHEN OTHERS THEN
									  	   OutValue:= 0; 
										   RETURN;
										   			 
						   END;
						
						END IF; /* 5 */
						
					ELSE
					
						-- caso 2) Il corrisp è stato utilizzato come corrisp in un protocollo 
						-- NUOVO CODICE RUBRICA 
						new_var_cod_rubrica := cod_rubrica || '_' || TO_CHAR(IDCorrGlobale) ;
								 
						<<STORICIZZAZIONE_CORRISP>>
						BEGIN
						
							UPDATE	DPA_CORR_GLOBALI 
							SET 	DTA_FINE = SYSDATE(),  VAR_COD_RUBRICA =new_var_cod_rubrica, VAR_CODICE = new_var_cod_rubrica
							WHERE 	SYSTEM_ID = IDCorrGlobale;
						
						EXCEPTION 
							WHEN OTHERS THEN
							OutValue:= 0;
							RETURN;
						
						END STORICIZZAZIONE_CORRISP;
					
					
						SELECT seq.NEXTVAL INTO IdentityCorrGlobali FROM dual;
						
						/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO 
						INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
						<<INSERIMENTO_NUOVO_CORRISP>>
						BEGIN 
						
							INSERT INTO DPA_CORR_GLOBALI (
								SYSTEM_ID,
								NUM_LIVELLO, 
								CHA_TIPO_IE,
								ID_REGISTRO, 
								ID_AMM,
								VAR_DESC_CORR, 
								VAR_NOME,
								VAR_COGNOME,
								ID_OLD, 
								DTA_INIZIO, 
								ID_PARENT, 
								VAR_CODICE, 
								CHA_TIPO_CORR,
								CHA_TIPO_URP,
								CHA_PA,
								VAR_CODICE_AOO,
								VAR_COD_RUBRICA, 
								CHA_DETTAGLI, 
								VAR_EMAIL, 
								VAR_CODICE_AMM								
							) 
							
							VALUES (
								IdentityCorrGlobali,
								'0',
								'E',
								id_reg,
								idAmm,
								desc_corr,
								nome,
								cognome,
								IDCorrGlobale, 
								SYSDATE(),
								'0',
								cod_rubrica,
								'S',
								cha_tipo_urp,
								'1',
								codice_aoo,
								cod_rubrica,
								cha_dettaglio,
								email,
								codice_amm			
							);
						
						EXCEPTION WHEN OTHERS THEN
								  OutValue:= 0;
								  RETURN;
						
						END INSERIMENTO_NUOVO_CORRISP;
						
						/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
						RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
						E UNITA' ORGANIZZATIVE */
						
						IF /* 4 */ cha_tipo_urp='U' OR cha_tipo_urp='P' THEN 
						   
							--PRENDO LA SYSTEM_ID APPENA INSERITA 
							SELECT SEQ.CURRVAL INTO new_IDCorrGlobale FROM DUAL;
							
							SELECT seq.NEXTVAL INTO IdentityDettGlobali FROM dual;

							<<INSERIMENTO_DETTAGLIO_CORRISP>>					   
							BEGIN	
												   
								INSERT INTO DPA_DETT_GLOBALI 
								(
									SYSTEM_ID,
									ID_CORR_GLOBALI, 
									VAR_INDIRIZZO, 
									VAR_CAP, 
									VAR_PROVINCIA, 
									VAR_NAZIONE, 
									VAR_COD_FISCALE, 
									VAR_TELEFONO, 
									VAR_TELEFONO2, 
									VAR_NOTE, 
									VAR_CITTA,
									VAR_FAX
								)
								VALUES
								(
									IdentityDettGlobali,
									new_IDCorrGlobale,
									indirizzo,
									cap,
									provincia,
									nazione,
									cod_fiscale,
									telefono,
									telefono2,
									note,
									citta,
									fax			
								);
								
							EXCEPTION 
									  WHEN OTHERS THEN
									  	   OutValue:= 0; 
										   RETURN;
										   
							END INSERIMENTO_DETTAGLIO_CORRISP;
								
						END IF;/* 4 */	
						
						--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA  
						<<INSERIMENTO_DPA_T_CANALE_CORR>>
						BEGIN
						
							SELECT seq.NEXTVAL INTO IdentityDpaTCanaleCorr FROM dual;
							
							INSERT INTO DPA_T_CANALE_CORR
							(
							 	   SYSTEM_ID,
							 	   ID_CORR_GLOBALE,
								   ID_DOCUMENTTYPE,
								   CHA_PREFERITO
							)
							
							VALUES 
							(
							 	  IdentityDpaTCanaleCorr,
								  new_IDCorrGlobale,
								  var_idDocType,
								  '1'
							);
							
							EXCEPTION 
									  WHEN OTHERS THEN
									  	   OutValue:= 0; 
										   RETURN;
						
						END INSERIMENTO_DPA_T_CANALE_CORR;
						
					END IF;/* 2 */	
				
			END IF /* 0 */	;
			
	   		ReturnValue := OutValue;
  	   END;
  
	
	
  END;
/
