
if exists (select * from dbo.sysobjects where id = object_id(N'[docsadm].[Sp_Fill_Dpa_Uo_Reg]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [docsadm].[Sp_Fill_Dpa_Uo_Reg]
GO


CREATE PROCEDURE Sp_Fill_Dpa_Uo_Reg AS


DECLARE @record_corr_uo int
DECLARE @record_corr_ruolo int
DECLARE @record_corr_registro int
DECLARE @rec int

BEGIN
	DECLARE cursor_uo
					
	CURSOR FOR
		SELECT system_id
		FROM DPA_CORR_GLOBALI
		WHERE
	 	 cha_tipo_urp = 'U'
		 AND cha_tipo_ie = 'I'
		 AND dta_fine IS NULL
		 AND id_old = 0
	
	OPEN cursor_uo
	FETCH NEXT FROM cursor_uo
	INTO @record_corr_uo 


	WHILE @@FETCH_STATUS = 0
	BEGIN

		DECLARE cursor_ruoli
					
		CURSOR FOR
			SELECT system_id
			FROM DPA_CORR_GLOBALI
			WHERE
				  cha_tipo_urp = 'R'
				  AND cha_tipo_ie = 'I'
				  AND dta_fine IS NULL
				  AND id_old = 0
				  AND id_uo = @record_corr_uo
		
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
				WHERE ID_UO = @record_corr_uo
					AND ID_REGISTRO = @record_corr_registro)
				
				IF (@rec = 0)
					BEGIN
						INSERT INTO DPA_UO_REG
					   		 (ID_UO, ID_REGISTRO)
					   	VALUES
					   		 (@record_corr_uo, @record_corr_registro)
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

		-- Posizionamento sul record successivo del cursore cursor_uo
		FETCH NEXT FROM cursor_uo
		INTO 	@record_corr_uo 
	END

	-- Chiusura e deallocazione cursore cursor_uo
	CLOSE cursor_uo
	DEALLOCATE cursor_uo
END