SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER   PROCEDURE @db_user.I_SMISTAMENTO_SMISTADOC_R_2
@IDPeopleMittente int,
@IDCorrGlobaleRuoloMittente int,
@IDGruppoMittente int,
@IDAmministrazioneMittente int,
@IDCorrGlobaleDestinatario int,
@IDDocumento int,
@TipoDiritto nchar(1),
@Rights int,
@IDRagioneTrasm int
AS

DECLARE @ReturnValue int
DECLARE @Identity int
DECLARE @IdentityTrasm int

BEGIN
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
			SET @IdentityTrasm = @Identity

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

							-- Verifica se non vi sia gia' una trasmissione per il documento:
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
								)

							IF (NOT @AccessRights IS NULL)
								BEGIN
									IF (@AccessRights < @Rights)
										-- aggiornamento a @Rights
										UPDATE 	SECURITY
										SET 	ACCESSRIGHTS=@Rights
										WHERE 	THING = @IDDocumento
										AND
										PERSONORGROUP = @IDGroups
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
										@TipoDiritto 
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

RETURN @ReturnValue

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

