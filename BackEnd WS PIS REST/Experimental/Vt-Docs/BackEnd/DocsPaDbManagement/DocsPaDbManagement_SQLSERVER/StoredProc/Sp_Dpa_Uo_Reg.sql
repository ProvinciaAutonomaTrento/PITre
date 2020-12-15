
if exists (select * from dbo.sysobjects where id = object_id(N'[docsadm].[Sp_Dpa_Uo_Reg]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [docsadm].[Sp_Dpa_Uo_Reg]
GO


CREATE PROCEDURE Sp_Dpa_Uo_Reg 
	@idUO int
AS

/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione della tabella DPA_UO_REG.
-- Viene passato come valore di input la system_id della UO.
-- All'inizio elimina tutti i record nella DPA_UO_REG con ID_UO uguale a quella passata,
-- poi cicla su tutti i ruoli della UO in questione per reperire le system_id dei registri associati,
-- quindi inserisce (se non esiste già) la coppia ID_UO e ID_REGISTRO.
--
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- 1: Errore generico
-------------------------------------------------------------------------------------------------------
*/
DECLARE @ReturnValue int
DECLARE @record_corr_ruolo int
DECLARE @record_corr_registro int
DECLARE @rec int


	BEGIN 
		DELETE FROM DPA_UO_REG WHERE id_uo = @idUO		
	END

	DECLARE cursor_ruoli
				
	CURSOR FOR
		SELECT system_id
		FROM DPA_CORR_GLOBALI
		WHERE
			  cha_tipo_urp = 'R'
			  AND cha_tipo_ie = 'I'
			  AND dta_fine IS NULL
			  AND id_old = 0
			  AND id_uo = @idUO
	
	OPEN cursor_ruoli
	FETCH NEXT FROM cursor_ruoli
	INTO @record_corr_ruolo 
	
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		DECLARE cursor_registri
				
		CURSOR FOR
			SELECT id_registro
			FROM DPA_L_RUOLO_REG
			WHERE id_ruolo_in_uo = @record_corr_ruolo
		
		OPEN cursor_registri
		FETCH NEXT FROM cursor_registri
		INTO @record_corr_registro 
	
	
		WHILE @@FETCH_STATUS = 0
		BEGIN
	
			SET @rec = (SELECT count(system_id)
			FROM DPA_UO_REG
			WHERE ID_UO = @idUO
				AND ID_REGISTRO = @record_corr_registro)
			
			IF (@rec = 0)
				BEGIN
					INSERT INTO DPA_UO_REG
				   		 (ID_UO, ID_REGISTRO)
				   	VALUES
				   		 (@idUO, @record_corr_registro)
					
					IF (@@ROWCOUNT = 0)
						BEGIN
							SET @ReturnValue=1
							RETURN
						END
				END
	
			-- Posizionamento sul record successivo del cursore cursor_registri
			FETCH NEXT FROM cursor_registri
			INTO 	@record_corr_registro 
		END
	
		-- Chiusura e deallocazione cursore cursor_registri
		CLOSE cursor_registri
		DEALLOCATE cursor_registri
	
		-- Posizionamento sul record successivo del cursore cursor_ruoli
		FETCH NEXT FROM cursor_ruoli
		INTO 	@record_corr_ruolo 
	END
	
	-- Chiusura e deallocazione cursore cursor_uo
	CLOSE cursor_ruoli
	DEALLOCATE cursor_ruoli
		
RETURN @ReturnValue

GO