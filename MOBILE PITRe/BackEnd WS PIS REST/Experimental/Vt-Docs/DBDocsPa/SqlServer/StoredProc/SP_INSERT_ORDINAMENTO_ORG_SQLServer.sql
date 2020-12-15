BEGIN TRANSACTION
ALTER TABLE DPA_CORR_GLOBALI ADD
	ID_PESO_ORG int NULL
GO
COMMIT


CREATE PROCEDURE SP_INSERT_ORDINAMENTO_ORG AS

DECLARE @idParent int
DECLARE @idUO int
DECLARE @idRuolo int
DECLARE @contatoreUO int
DECLARE @contatoreRUOLO int

BEGIN	
	DECLARE cursor_IDParent CURSOR FOR
		select distinct id_parent 
		from dpa_corr_globali 
		where cha_tipo_urp = 'U' and 
		cha_tipo_ie = 'I' and 
		dta_fine is null and 
		id_parent not in (0)
		order by id_parent
	BEGIN
		OPEN cursor_IDParent
	
		FETCH NEXT FROM cursor_IDParent
		INTO @idParent
	
		WHILE @@FETCH_STATUS = 0
	
			BEGIN
				DECLARE cursor_UO CURSOR FOR
					select system_id 
					from dpa_corr_globali 
					where id_parent = @idParent and
					cha_tipo_urp = 'U' and 
					cha_tipo_ie = 'I' and 
					dta_fine is null

				BEGIN
					OPEN cursor_UO
					
					SET @contatoreUO = 0
		
					FETCH NEXT FROM cursor_UO
					INTO @idUO
				
					WHILE @@FETCH_STATUS = 0
				
						BEGIN
							SET @contatoreUO = @contatoreUO + 1				
							UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = @contatoreUO WHERE SYSTEM_ID = @idUO	
							
							DECLARE cursor_RUOLI CURSOR FOR
								select system_id 
								from dpa_corr_globali 
								where id_uo = @idUO and
								cha_tipo_urp = 'R' and 
								cha_tipo_ie = 'I' and 
								dta_fine is null 
							
							BEGIN
								OPEN cursor_RUOLI
						
								SET @contatoreRUOLO = 0
					
								FETCH NEXT FROM cursor_RUOLI
								INTO @idRuolo
							
								WHILE @@FETCH_STATUS = 0
							
									BEGIN
										SET @contatoreRUOLO = @contatoreRUOLO + 1	
										UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = @contatoreRUOLO WHERE SYSTEM_ID = @idRuolo
			
										FETCH NEXT FROM cursor_RUOLI
										INTO @idRuolo
									END
								
									CLOSE cursor_RUOLI
									DEALLOCATE cursor_RUOLI
			
								FETCH NEXT FROM cursor_UO
								INTO @idUO
							END
						END
		
						CLOSE cursor_UO
						DEALLOCATE cursor_UO
		
					FETCH NEXT FROM cursor_IDParent
					INTO @idParent
				END
			END
	
			CLOSE cursor_IDParent
			DEALLOCATE cursor_IDParent
	END
END

GO

EXEC SP_INSERT_ORDINAMENTO_ORG

GO