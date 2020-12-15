/* Deve essere verificato che il corrispondente:
	
	 1)  	Non è mai stato inserito come mittente/destinatario di protocolli (ovvero non deve essere presente
	    	nella tabella DPA_DOC_ARRIVO_PAR)
	
	2)	Non è mai stato inserito in una lista di distribuzione */

	-- paremetri:

	--   @IDCorrGlobale: system_id della DPA_CORR_GLOBALI, relativa al corrispondente che si sta tentando di rimuovere
	--   @liste: indica se le liste di distribuzione sono abilitate o meno
	
	
	CREATE PROCEDURE SP_DELETE_CORR_ESTERNO @IDCorrGlobale INT, @liste INT  AS
	
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

	DECLARE @countDoc int	-- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
	DECLARE @ReturnValue int
	DECLARE @cha_tipo_urp varchar(1)
	DECLARE @var_inLista varchar(1) -- valore 'N' (il corr non è presente in nessuna lista di sistribuzione), 'Y' altrimenti
	BEGIN
	
		SET @cha_tipo_urp = (select cha_tipo_urp from dpa_corr_globali where system_id = @IDCorrGlobale)
		
		SET @var_inLista = 'N' -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

		SELECT SYSTEM_ID  FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale
	
		IF @@ROWCOUNT > 0
			BEGIN
				IF (@liste = 1) 
				--- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente è in una lista di distibuzione, in caso affermativo non posso rimuoverlo
					BEGIN
					-- CASO 1.1 - Il corrispondente è predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).	
						
						RETURN 2
					END
				ELSE
				-- CASO 2 - Le liste di distribuzione NON SONO abilitate
				
					BEGIN
						SET @var_inLista = 'Y'	
					END
	
			END

		
		
		-- Se la procedura va avanti, cio' significa che:
		-- Le liste di distribuzione non sono abilitate (@liste = 0), oppure sono abilitate (@liste=1) ma il corrispondente che si tenta di rimuovere non è contenuto in una lista

		SELECT ID_PROFILE  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST =  @IDCorrGlobale
	
		IF (@@ROWCOUNT = 0)
			-- CASO 3 -  il corrispondente non è stato mai utilizzato come mitt/dest di protocolli
			BEGIN	
					
				-- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
				DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = @IDCorrGlobale
				
				IF @@ROWCOUNT = 0
					-- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).
					BEGIN
						SET @ReturnValue=3 
					END
				ELSE
					BEGIN
						SET @ReturnValue=0 

						DELETE FROM DPA_T_CANALE_CORR WHERE ID_CORR_GLOBALE = @IDCorrGlobale

						-- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poichè in fase di creazione di un ruolo
						-- non viene fatta la insert in tale tabella
						IF( @cha_tipo_urp != 'R') 
							
							BEGIN

							-- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
							DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = @IDCorrGlobale
						
							IF @@ROWCOUNT = 0
								-- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
								BEGIN
									SET @ReturnValue=4
								END
							ELSE
								-- CAS0 3.1.2.2 - la rimozione da DPA_DETT_GLOBALI VAa buon fine  (VALORE RITORNATO = 0).
								BEGIN
									SET @ReturnValue=0  -- operazione andata a buon fine
								END
							END
						
						IF (@ReturnValue=0 AND @liste = 0 AND @var_inLista = 'Y')
							BEGIN
								--se:
									-- 1) sono andate bene le DELETE precedenti
									-- 2) sono disabilitate le liste di distribuzione
									-- 3) il corrispondente è nella DPA_LISTE_DISTR 

								-- rimuovo il corrispondente dalla DPA_LISTE_DISTR 
								DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale
								
								IF @@ROWCOUNT = 0
								-- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).
								BEGIN
									SET @ReturnValue=6
								END
								
							END
					END

			END	
			
		ELSE
			-- CASO 4 -  il corrispondente è  stato utilizzato come mitt/dest di protocolli
			BEGIN	
				-- 4.1) disabilitazione del corrispondente
				UPDATE DPA_CORR_GLOBALI SET DTA_FINE = GETDATE() WHERE SYSTEM_ID = @IDCorrGlobale
				
				IF @@ROWCOUNT = 0
					-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
					BEGIN
						SET @ReturnValue=5
					END
	
				ELSE
					-- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
					BEGIN
						SET @ReturnValue=1 
					END
			END
			
	END
	
	RETURN @ReturnValue
GO
