USE PCM_DEPOSITO_1
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================
-- Author:		Giovanni Olivari
-- Create date: 10/06/2013
-- Description:	Elimina eventuali autorizzazioni scadute
-- =====================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_DeleteExpiredAuthorization
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	
	DECLARE @idAuthorization INT
	DECLARE @idPeople INT

	DECLARE @sql_string nvarchar(MAX)
	DECLARE @nomeUtenteDeposito varchar(200) 
	
	
	
	-- Lettura parametri di configurazione
	--
	SELECT @nomeUtenteDeposito=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_UTENTE_DEPOSITO'



	-- Chiude e dealloca eventuali cursori rimasti aperti
	--
	execute sp_ARCHIVE_BE_CleanUpCursor 'authorization_cursor'



	-- Seleziona le autorizzazioni che sono scadute
	--
	DECLARE authorization_cursor CURSOR
	FOR SELECT SYSTEM_ID, PEOPLE_ID
		FROM ARCHIVE_AUTHORIZATION
		WHERE ENDDATE < GETDATE()
	     
	OPEN authorization_cursor
	FETCH authorization_cursor INTO @idAuthorization, @idPeople
	WHILE (@@FETCH_STATUS = 0)
		BEGIN
		
			print '@idAuthorization: ' + CAST(@idAuthorization AS NVARCHAR(MAX))
			print '@idPeople: ' + CAST(@idPeople AS NVARCHAR(MAX))


			BEGIN TRANSACTION T1

				
			set @logType = 'INFO'
			set @log = 'Start procedura aggiornamento autorizzazione scaduta - AuthorizationID: ' + CAST(@idAuthorization AS NVARCHAR(MAX))
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



			-- Elimina utente da ARCHIVE_OrgChart
			--
			DELETE FROM ARCHIVE_OrganizationalChart
			WHERE SYSTEM_ID IN
				(
				SELECT SYSTEM_ID
				FROM DPA_CORR_GLOBALI
				WHERE ID_PEOPLE = @idPeople
				)
			
			IF @@ERROR <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''aggiornamento autorizzazione scaduta per la tabella ARCHIVE_OrganizationalChart'
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END
				
				
				
			-- Imposta DPA_CORR_GLOBALI.DTA_FINE = GETDATE dove ID_PEOPLE = @idPeople per disabilitare l'account dell'utente
			--
			UPDATE DPA_CORR_GLOBALI
			SET DTA_FINE = GETDATE()
			WHERE ID_PEOPLE = @idPeople
			
			IF @@ERROR <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''aggiornamento autorizzazione scaduta per la tabella DPA_CORR_GLOBALI'
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END
			
			
			
			-- Elimina la relazione utente-ruolo
			--
			SET @sql_string = CAST(N'
				DELETE FROM PEOPLEGROUPS
				WHERE GROUPS_SYSTEM_ID = ' AS NVARCHAR(MAX)) + CAST(@nomeUtenteDeposito AS NVARCHAR(MAX)) + CAST(N'.fn_ARCHIVE_GetGroupsIDForRuoloConsultatore()
				AND PEOPLE_SYSTEM_ID = ' AS NVARCHAR(MAX)) + CAST(@idPeople AS NVARCHAR(MAX))
								
			PRINT @sql_string;

			EXECUTE sp_executesql @sql_string;
			
			IF @@ERROR <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''aggiornamento autorizzazione scaduta per la tabella PEOPLEGROUPS'
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END
			
			
			
			-- Elimina doc/fasc dall'area di lavoro
			--
			DELETE FROM DPA_AREA_LAVORO
			WHERE ID_PEOPLE = @idPeople
			
			IF @@ERROR <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''aggiornamento autorizzazione scaduta per la tabella DPA_AREA_LAVORO'
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END
			
			
			
			-- Elimina visibilità doc/fasc dalla Security utilizzando la condizione sul campo VAR_NOTE_SEC = @idAutorizzazione
			--
			DELETE FROM SECURITY
			WHERE VAR_NOTE_SEC = @idAuthorization
			
			IF @@ERROR <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''aggiornamento autorizzazione scaduta per la tabella SECURITY'
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END


			COMMIT TRANSACTION T1

				
			set @logType = 'INFO'
			set @log = 'End procedura aggiornamento autorizzazione scaduta - AuthorizationID: ' + CAST(@idAuthorization AS NVARCHAR(MAX))
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			
			FETCH authorization_cursor INTO @idAuthorization, @idPeople
			
		END
			
	CLOSE authorization_cursor
	DEALLOCATE authorization_cursor
	
END
GO
