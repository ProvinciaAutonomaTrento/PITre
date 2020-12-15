if exists (select * from dbo.sysobjects where id = object_id(N'[docsadm].[I_SMISTAMENTO_SMISTADOC_R_2]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [docsadm].[I_SMISTAMENTO_SMISTADOC_R_2]
GO


CREATE PROCEDURE docsadm.I_SMISTAMENTO_SMISTADOC_R_2
	@IDPeopleMittente int,
	@IDCorrGlobaleRuoloMittente int,
	@IDGruppoMittente int,
	@IDAmministrazioneMittente int,
	@IDCorrGlobaleDestinatario int,
	@IDDocumento int,
	@FlagCompetenza bit -- Competenza=1, CC=0
AS

/*
SP per la gestione delle trasmissioni a ruolo nella protocolazione semplificata .
Valori di ritorno gestiti:
- 0: Operazione andata a buon fine
- 1: Non è presente la ragione trasmissione per l'amministrazione corrente (COMPETENZA,CONOSCENZA)
- 2: Non è stato inserito il record in tabella DPA_TRASMISSIONI
- 3: Non è stato inserito il record in tabella DPA_TRASM_SINGOLE
- 4: Non è stato inserito il record in tabella DPA_TRASM_UTENTE

*/

-- Reprimento ID da tabella "DPA_RAGIONE_TRASM"
DECLARE @IDRagioneTrasm int
DECLARE @DescrizioneRagione varchar(32)
DECLARE @Rights int

IF (@FlagCompetenza=1)
	BEGIN
		SET @DescrizioneRagione='COMPETENZA'
		SET @Rights = 63
	END
ELSE
	BEGIN
		SET @DescrizioneRagione='CONOSCENZA'
		SET @Rights = 45
	END

SET @IDRagioneTrasm=(SELECT 	SYSTEM_ID 
		     FROM 	DPA_RAGIONE_TRASM
		     WHERE 	ID_AMM=@IDAmministrazioneMittente AND
				VAR_DESC_RAGIONE=@DescrizioneRagione)

DECLARE @ReturnValue int


IF (NOT @IDRagioneTrasm IS NULL)

	BEGIN
		DECLARE @Identity int
		DECLARE @IdentityTrasm int

		-- Inserimento in tabella DPA_TRASMISSIONE
		
		INSERT INTO DPA_TRASMISSIONE
		(
			ID_RUOLO_IN_UO,
			ID_PEOPLE,
			CHA_TIPO_OGGETTO,
			ID_PROFILE,
			ID_PROJECT,
			DTA_INVIO,
			VAR_NOTE_GENERALI			
		)
		VALUES
		(
			@IDCorrGlobaleRuoloMittente,
			@IDPeopleMittente,
			'D',	
			@IDDocumento,
			NULL,
			GETDATE(),
			NULL
		)
		
		IF (@@ROWCOUNT = 0)
			BEGIN
				SET @ReturnValue=2
			END
		ELSE
			BEGIN				
				-- Reperimento identity appena immessa
				SET @Identity=scope_identity()
				SET @IdentityTrasm = @Identity
				
				-- Inserimento in tabella DPA_TRASM_SINGOLA
				INSERT INTO DPA_TRASM_SINGOLA
				(
					ID_RAGIONE,
					ID_TRASMISSIONE,
					CHA_TIPO_DEST,
					ID_CORR_GLOBALE,
					VAR_NOTE_SING,
					CHA_TIPO_TRASM,
					DTA_SCADENZA,
					ID_TRASM_UTENTE
				)
				VALUES
				(
					@IDRagioneTrasm,
					@Identity,
					'R',
					@IDCorrGlobaleDestinatario,
					NULL,
					'S',
					NULL,
					NULL
				)		

				IF (@@ROWCOUNT = 0)
					BEGIN
						SET @ReturnValue=3
					END
				ELSE		
					BEGIN
						-- Reperimento identity appena immessa
						SET @Identity=scope_identity()

						-- Prende gli utenti del ruolo
						DECLARE cursor_IDUtenti
					
						CURSOR FOR
					
							SELECT 	P.SYSTEM_ID
							FROM 	GROUPS G, 
									PEOPLEGROUPS PG, 
									PEOPLE P,
									DPA_CORR_GLOBALI CG
							WHERE 	PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND 
									PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND 
									G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = @IDCorrGlobaleDestinatario) AND
									P.DISABLED NOT IN ('Y') AND
									P.SYSTEM_ID=CG.ID_PEOPLE	
									AND CG.CHA_TIPO_URP != 'L'	
									AND CG.DTA_FINE IS NULL
									AND PG.DTA_FINE IS NULL
					
						OPEN cursor_IDUtenti
					
						DECLARE @IDUtente int
					
						FETCH NEXT FROM cursor_IDUtenti
						INTO 	@IDUtente 
					
					
						WHILE @@FETCH_STATUS = 0
						BEGIN
							-- Inserimento in tabella DPA_TRASM_UTENTE
							INSERT INTO DPA_TRASM_UTENTE
							(
								ID_TRASM_SINGOLA,
								ID_PEOPLE,
								DTA_VISTA,
								DTA_ACCETTATA,
								DTA_RIFIUTATA,
								DTA_RISPOSTA,
								CHA_VISTA,
								CHA_ACCETTATA,
								CHA_RIFIUTATA,
								VAR_NOTE_ACC,
								VAR_NOTE_RIF,
								CHA_VALIDA,
								ID_TRASM_RISP_SING
							)
							VALUES
							(
								@Identity,
								@IDUtente,
								NULL,
								NULL,
								NULL,
								NULL,
								'0',
								'0',
								'0',
								NULL,
								NULL,
								'1',
								NULL			
							)
							-- Verifica se non vi sia già una trasmissione per il documento:
							-- - se presente, si distinguono 2 casi:
							--	1) se ACCESSRIGHT < @Rights
							--	   viene fatto un'aggiornamento impostandone il valore a @Rights
							--	2) altrimenti non fa nulla
							-- - se non presente viene fatta in ogni caso la insert con 
							--   valore di ACCESSRIGHT = @Rights
							DECLARE @IDGroups int 

							SET @IDGroups = (
									SELECT A.ID_GRUPPO 
			 						FROM DPA_CORR_GLOBALI A
			 						WHERE A.SYSTEM_ID = @IDCorrGlobaleDestinatario
									)

							DECLARE @AccessRights int
						
							SET @AccessRights=
								(		
									SELECT 	MAX(ACCESSRIGHTS)
									FROM 		SECURITY 
									WHERE 	THING = @IDDocumento 
											AND
											PERSONORGROUP = @IDGroups 
											AND
											CHA_TIPO_DIRITTO='T'
								)
							
							IF (NOT @AccessRights IS NULL)
								BEGIN
									IF (@AccessRights < @Rights)
										-- aggiornamento a @Rights
										UPDATE 	SECURITY
										SET 		ACCESSRIGHTS=@Rights
										WHERE 	THING = @IDDocumento 
												AND
												PERSONORGROUP = @IDGroups 
												AND
												CHA_TIPO_DIRITTO='T'	
												AND
												ACCESSRIGHTS=@AccessRights
								END
					
							ELSE			
								BEGIN
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
										@IDDocumento,
										@IDGroups,
										@Rights,
										@IDGruppoMittente,
										'T'							
									)
								END	
					
							
					
					
							-- Posizionamento sul record successivo del cursore temporaneo
							FETCH NEXT FROM cursor_IDUtenti
							INTO 	@IDUtente 			
					
						END
					
						-- Chiusura e deallocazione cursore	
						CLOSE cursor_IDUtenti
						DEALLOCATE cursor_IDUtenti
						
						--AGGIORNAMENTO DELLA DATA DI INVIO DELLA TRASMISSIONE
						UPDATE DPA_TRASMISSIONE SET DTA_INVIO = GETDATE() WHERE SYSTEM_ID = @IdentityTrasm
						
						-- Esecuzione a buon fine
						SET @ReturnValue=0
					END
			END
	END
ELSE
	
	BEGIN
		SET @ReturnValue=1
	END

RETURN @ReturnValue


GO
