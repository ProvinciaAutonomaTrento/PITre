CREATE PROCEDURE [docsadm].[I_SMISTAMENTO_SMISTADOC_R]
	@IDPeopleMittente int,
	@IDCorrGlobaleRuoloMittente int,
	@IDGruppoMittente int,
	@IDAmministrazioneMittente int,
	@IDCorrGlobaleDestinatario int,
	@IDDocumento int,
	@IDTrasmissione int,
	@FlagCompetenza bit, 
	@IDTrasmissioneUtenteMittente int,
	@TrasmissioneConWorkflow bit, 
	@NoteGeneraliDocumento varchar(250),
	@NoteIndividuali varchar(250)		
AS

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
				SET @Identity=scope_identity()

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
					@NoteIndividuali varchar(250),
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
						SET @Identity=scope_identity()
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
							FETCH NEXT FROM cursor_IDUtenti
							INTO 	@IDUtente 			
					
						END
						CLOSE cursor_IDUtenti
						DEALLOCATE cursor_IDUtenti
						IF (@TrasmissioneConWorkflow='1')
							UPDATE 	DPA_TRASM_UTENTE
							SET 	DTA_VISTA=GETDATE(),
									CHA_VISTA='1',
									DTA_ACCETTATA=GETDATE(),
									CHA_ACCETTATA='1',
									VAR_NOTE_ACC='Documento accettato e smistato',
									CHA_IN_TODOLIST = '0'
							--WHERE	SYSTEM_ID=@IDTrasmissioneUtenteMittente
							WHERE	SYSTEM_ID IN ( SELECT TU.SYSTEM_ID FROM DPA_TRASM_UTENTE TU,
	   						 				DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS 
	  										WHERE TU.ID_PEOPLE=@IDPeopleMittente
	  										AND TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND 
											TX.SYSTEM_ID = @IDTrasmissione
	  										AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA )
						ELSE													
							UPDATE 	DPA_TRASM_UTENTE
							SET 	DTA_VISTA = GETDATE(),
									CHA_VISTA = '1',
									CHA_IN_TODOLIST = '0'
							WHERE	SYSTEM_ID = @IDTrasmissioneUtenteMittente;
						IF ((SELECT top 1 A.CHA_TIPO_TRASM 
								      FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
								      WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
								      AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
								      DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= @IDPeopleMittente AND
								      TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA)
								      ORDER BY CHA_TIPO_DEST
						   )='S' AND @TrasmissioneConWorkflow='1')
					   		UPDATE 	DPA_TRASM_UTENTE
							SET 	CHA_VALIDA = '0', cha_in_todolist = '0'
							--WHERE	ID_TRASM_SINGOLA = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente)
							--AND		SYSTEM_ID NOT IN (@IDTrasmissioneUtenteMittente)
							WHERE ID_TRASM_SINGOLA IN 
							      (SELECT DISTINCT SYSTEM_ID FROM DPA_TRASM_SINGOLA WHERE ID_TRASMISSIONE = @IDTrasmissione)
								  AND SYSTEM_ID NOT IN( @IDTrasmissioneUtenteMittente)

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
