CREATE    PROCEDURE SP_MODIFY_CORR_ESTERNO
	@IDCorrGlobale INT,  
	@desc_corr  VARCHAR(128),
	@nome  VARCHAR(50),
	@cognome  VARCHAR(50),
	@codice_aoo VARCHAR(16),
	@codice_amm VARCHAR(32),
	@email  VARCHAR(128),
	@indirizzo VARCHAR(128),
	@cap VARCHAR(5),
	@provincia VARCHAR(2),
	@nazione VARCHAR(32),
	@citta  VARCHAR(64),
	@cod_fiscale VARCHAR(16),
	@telefono VARCHAR(16),
	@telefono2 VARCHAR(16),
	@note VARCHAR(250),
	@fax VARCHAR(16),
	@localita VARCHAR(128),
	@luogoNascita VARCHAR(128),
	@dataNascita DATE,
        @var_idDocType INT
	
  AS 

/*
 1)	update del corrispondente vecchio:
	- settando var_cod_rubrica = var_cod_rubrica_system_id;
	- settando dta_fine = GETDATE();

2)	insert del nuovo corrispondente (DPA_CORR_GLOBALI):
	- codice rubrica = codice rubrica del corrispondente che è stato storicizzato al punto 1)
	- id_old = system_id del corrispondente storicizzato al punto 1)
	
	2.1) insert del dettaglio del  nuovo corrispondente (DPA_DETT_GLOBALI) solo per UTENTI e UO
		
*/


DECLARE @ReturnValue INT
DECLARE @cod_rubrica VARCHAR(128)
DECLARE @id_reg INT
DECLARE @idAmm INT
DECLARE @new_var_cod_rubrica VARCHAR(128)
DECLARE @cha_dettaglio VARCHAR(1)
DECLARE @cha_tipo_urp VARCHAR(1)
DECLARE @var_new_IDCorrGlobale INT
DECLARE @v_id_docType INT

BEGIN

	SELECT
		@cod_rubrica = VAR_COD_RUBRICA, 
		@cha_tipo_urp = CHA_TIPO_URP,  
		@id_reg = ID_REGISTRO,
		@idAmm = ID_AMM
	 FROM DPA_CORR_GLOBALI 
	 WHERE system_id = @IDCorrGlobale
	
	IF @@ROWCOUNT > 0  --1
		BEGIN
			
			SELECT @v_id_docType =  ID_DOCUMENTTYPE
			FROM DPA_T_CANALE_CORR
			WHERE ID_CORR_GLOBALE = @IDCorrGlobale
			
			IF @@ROWCOUNT > 0
				 BEGIN
				-- calcolo il nuovo codice rubrica
				--SET @new_var_cod_rubrica = @var_cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale) 
				SET @new_var_cod_rubrica = @cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale )
	 			
				SET @cha_dettaglio = '0' -- default
	
				IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
					SET @cha_dettaglio = '1'
				
				BEGIN
					
			
					--VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli		
					SELECT ID_PROFILE  
					FROM DPA_DOC_ARRIVO_PAR 
					WHERE ID_MITT_DEST =  @IDCorrGlobale
					
					-- 1) non è stato mai utilizzato come corrisp in un protocollo
					IF (@@ROWCOUNT = 0) 
						BEGIN
							--non devo storicizzare, aggiorno solamente i dati
							UPDATE 	DPA_CORR_GLOBALI 
								SET 	VAR_CODICE_AOO= @codice_aoo, 
									VAR_CODICE_AMM = @codice_amm, 
									VAR_EMAIL = @email,
									VAR_DESC_CORR = @desc_corr,
									VAR_NOME = @nome,
									VAR_COGNOME= @cognome
								WHERE 	SYSTEM_ID = @IDCorrGlobale
							
							IF(@@ROWCOUNT > 0) -- SE UPDATE è andata a buon fine
								BEGIN
									begin
									--per utenti e Uo aggiorno il dettaglio
									IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
									
										BEGIN
		
											UPDATE DPA_DETT_GLOBALI
											SET 	VAR_INDIRIZZO = @indirizzo, 
												VAR_CAP = @cap, 
												VAR_PROVINCIA = @provincia, 
												VAR_NAZIONE = @nazione, 
												VAR_COD_FISCALE = @cod_fiscale, 
												VAR_TELEFONO = @telefono, 
												VAR_TELEFONO2 = @telefono2, 
												VAR_NOTE= @note, 
												VAR_CITTA= @citta,
												VAR_FAX = @fax,
												VAR_LOCALITA=@localita,
												VAR_LUOGO_NASCITA=@luogoNascita,
												DTA_NASCITA= @dataNascita
											WHERE ID_CORR_GLOBALI = @IDCorrGlobale
										
											IF(@@ROWCOUNT > 0)
		 										SET @ReturnValue = 1
											ELSE
												SET @ReturnValue = 0

											
										END
								
									ELSE
										SET @ReturnValue = 1 -- CASO RUOLI
									end

									IF (@ReturnValue = 1)
										
										IF (@var_idDocType != @v_id_docType)
											
											BEGIN
											
									   			 
												UPDATE DPA_T_CANALE_CORR 
									   			 SET  ID_DOCUMENTTYPE =  @var_idDocType 
									   			 WHERE ID_CORR_GLOBALE = @IDCorrGlobale
												
												IF(@@ROWCOUNT > 0)
			 										SET @ReturnValue = 1
												ELSE
													SET @ReturnValue = 0
											END
										

								END
								
							ELSE
								SET @ReturnValue = 0 	
							
						END
					ELSE
			
					-- caso 2) Il corrisp è stato utilizzato come destinatario
					BEGIN
						--  INIZIO STORICIZZAZIONE DEL CORRISPONDENTE 
						
						UPDATE	DPA_CORR_GLOBALI 
						SET 	DTA_FINE = GETDATE(),  VAR_COD_RUBRICA =@new_var_cod_rubrica,  VAR_CODICE =@new_var_cod_rubrica
						WHERE 	SYSTEM_ID = @IDCorrGlobale

						-- se la storicizzazione è andata a buon fine, 
						--posso inserire il nuovo corrispondente
						IF @@ROWCOUNT > 0 
					
							BEGIN

								INSERT INTO DPA_CORR_GLOBALI (
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
									'0',
									'E',
									@id_reg,
									@idAmm,
									@desc_corr,
									@nome,
									@cognome,
									@IDCorrGlobale, 
									GETDATE(),
									'0',
									@cod_rubrica,
									'S',
									@cha_tipo_urp,
									'1',
									@codice_aoo,
									@cod_rubrica,
									@cha_dettaglio,
									@email,
									@codice_amm			
								)

								--prendo la systemId appena inserita
								SET @var_new_IDCorrGlobale = @@identity
					
								IF @@ROWCOUNT > 0 -- se l'inserimento del nuovo corrisp è andato a buon fine
						
									IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' ) -- CASO UTENTE/UO: inserisco il dettaglio
										BEGIN
			
											INSERT INTO DPA_DETT_GLOBALI 
											(
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
												VAR_FAX,
												VAR_LOCALITA,
												VAR_LUOGO_NASCITA,
												DTA_NASCITA
											)
			
											VALUES
			
											(
												@var_new_IDCorrGlobale,
												@indirizzo,
												@cap,
												@provincia,
												@nazione,
												@cod_fiscale,
												@telefono,
												@telefono2,
												@note,
												@citta,
												@fax,
												@localita,
												@luogoNascita,
												@dataNascita
														
											)

											IF @@ROWCOUNT > 0 
												-- se la insert su dpa_dett_globali è andata a buon fine
												SET @ReturnValue = 1 -- valore ritornato 1
											ELSE
												-- se la insert su dpa_dett_globali non è andata a buon fine
												SET @ReturnValue = 0 -- valore ritornato 0
										END
									
									ELSE  -- CASO RUOLO: non inserisco il dettaglio
										-- vuol dire che il corrispondente è un RUOLO (quindi non deve essere fatta la insert sulla dpa_dett_globali)
							 			--valore ritornato 1 perchè significa che l'operazione di inserimento del nuovo ruolo è andato a buon fine
										SET @ReturnValue = 1		
									
									IF (@ReturnValue = 1)
										
										BEGIN

											INSERT INTO DPA_T_CANALE_CORR
											(  
											 	   ID_CORR_GLOBALE,
												   ID_DOCUMENTTYPE,
												   CHA_PREFERITO
											)
											
											VALUES 
											(
												  @var_new_IDCorrGlobale,
												  @var_idDocType,
												  '1'
											)
											
											IF @@ROWCOUNT > 0 
												-- se la insert su DPA_T_CANALE_CORR è andata a buon fine
												SET @ReturnValue = 1 -- valore ritornato 1
											ELSE
												-- se la insert su DPA_T_CANALE_CORRnon è andata a buon fine
												SET @ReturnValue = 0 -- valore ritornato 0
									END
								ELSE
						
									SET @ReturnValue = 0 -- inserimento non andato a buon fine: ritorno 0 ed esco
					
							-- FINE STORICIZZAZIONE

							END	
					
					END


		
				END
			
			END
		ELSE
			SET @ReturnValue = 0 
	
	END


		
	ELSE
		SET @ReturnValue = 0 -- la storicizzazione del corrispondente è andata male:  ritorno 0 ed esco -- END 1
				
END

RETURN @ReturnValue
GO
