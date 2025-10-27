CREATE OR REPLACE PROCEDURE SP_DELETE_CORR_ESTERNO (IDCorrGlobale IN NUMBER, liste IN NUMBER, ReturnValue OUT NUMBER)  IS
	
	/*
	-------------------------------------------------------------------------------------------------------
	SP per la Cancellazione corrispondente
	
	 Valori di ritorno gestiti:
	
	 0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
	 1: DISABILITAZIONE EFFETTUATA - il corrispondente è presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato

	 2: CORRISPONDENTE NON RIMOSSO - il corrispondente è presente nella lista di distribuzione e non posso rimuoverlo
	
	 3: ERRORE: la DELETE sulla dpa_corr_globali NON è andata a buon fine
	 4: ERRORE: la DELETE sulla dpa_dett_globali NON è andata a buon fine
	 5: ERRORE: l' UPDATE sulla dpa_corr_globali NON è andata a buon fine
 	 6: ERRORE: la DELETE sulla dpa_liste_distr NON è andata a buon fine
	-------------------------------------------------------------------------------------------------------
	
	*/

	countDoc number; -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
	cha_tipo_urp VARCHAR2(1);
	var_inLista VARCHAR2(1); -- valore 'N' (il corr non è presente in nessuna lista di sistribuzione), 'Y' altrimenti
	
	countLista number;

	BEGIN
	
		select cha_tipo_urp INTO cha_tipo_urp from dpa_corr_globali where system_id = IDCorrGlobale;
		
		var_inLista := 'N'; -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

		SELECT count(SYSTEM_ID) into countLista FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;
	
		IF (countLista > 0) THEN -- se il corrispondente è contenuto nelle liste di distribuzione
			BEGIN
				IF (liste = 1) THEN
				--- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente è in una lista di distibuzione, in caso affermativo non posso rimuoverlo
					BEGIN
					-- CASO 1.1 - Il corrispondente è predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).	
						ReturnValue := 2;
						RETURN; 			
					END;
				ELSE
				-- CASO 2 - Le liste di distribuzione NON SONO abilitate
				
					BEGIN
						var_inLista := 'Y';	
					END;
					
				END IF;
	
			END;
		END IF;
		
		
		-- Se la procedura va avanti, cio' significa che:
		-- Le liste di distribuzione non sono abilitate (liste = 0), oppure sono abilitate (liste=1) ma il corrispondente che si tenta di rimuovere non è contenuto in una lista

		SELECT count(ID_PROFILE) INTO countDoc  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST = IDCorrGlobale;
	 
		IF (countDoc = 0) THEN
			-- CASO 3 -  il corrispondente non è stato mai utilizzato come mitt/dest di protocolli
			   		BEGIN	
					
							-- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
							DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = IDCorrGlobale;
				
							EXCEPTION 
									  WHEN OTHERS THEN
					 	  	  		  ReturnValue:=3;-- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).
			
					END;
					IF (ReturnValue=3) THEN
					   RETURN; --ESCO DALLA PROCEDURA 
					 
					ELSE
					 	 BEGIN
						 	  
							  ReturnValue:=0;
							  
							  DELETE FROM DPA_T_CANALE_CORR WHERE  ID_CORR_GLOBALE = IDCorrGlobale;
							  
							  -- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poichè in fase di creazione di un ruolo
							  -- non viene fatta la insert in tale tabella
							  IF(cha_tipo_urp != 'R') THEN
									
									BEGIN
				
							 			 -- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
							 			 DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = IDCorrGlobale;
						    
							 			 EXCEPTION
								 		 WHEN OTHERS THEN
						 	 	  	  		  	   ReturnValue:=4;-- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
									 END; 		  
										 
									 IF (ReturnValue=4) THEN
										 
										 	RETURN; -- ESCO DALLA PROCEDURA
										 
										 ELSE
										   
										   ReturnValue:=0; -- CANCELLAZIONE ANDATA A BUON FINE
										 
									END IF;		
										
								END IF;
																	
								IF (ReturnValue=0 AND liste = 0 AND var_inLista = 'Y')	THEN
									
									   BEGIN
										   --se:
										   -- 1) sono andate bene le DELETE precedenti
										   -- 2) sono disabilitate le liste di distribuzione
										   -- 3) il corrispondente è nella DPA_LISTE_DISTR 
	
										   -- rimuovo il corrispondente dalla DPA_LISTE_DISTR 
										   -- rimuovo il corrispondente dalla DPA_LISTE_DISTR 
										   DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;
									
										    EXCEPTION
									 		 WHEN OTHERS THEN
							 	 	  	  		  	  ReturnValue:=6;-- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).
										  		  								   
										END;
										
								 END IF;
									
					     	END;	  
					END IF;
					
					
		ELSE
		
			-- CASO 4 -  il corrispondente è  stato utilizzato come mitt/dest di protocolli
			BEGIN	
				-- 4.1) disabilitazione del corrispondente
				UPDATE DPA_CORR_GLOBALI SET DTA_FINE = SYSDATE() WHERE SYSTEM_ID = IDCorrGlobale;
				
				    
				EXCEPTION 
					WHEN OTHERS THEN
						 	ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
							
			END;	
				
			IF(ReturnValue=5) THEN
				
					RETURN;
				
			ELSE
				
					ReturnValue:=1;	-- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
				
			END IF;
						
			
			
		END IF;
			
END;
/
