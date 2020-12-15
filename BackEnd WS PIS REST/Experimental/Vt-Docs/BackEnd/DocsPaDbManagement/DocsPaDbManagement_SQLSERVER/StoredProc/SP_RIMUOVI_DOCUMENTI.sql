CREATE    PROCEDURE SP_RIMUOVI_DOCUMENTI
	@idProfile INT, 
	@ReturnValue INT OUT
	
  AS 

BEGIN 
	SET @ReturnValue = 1
	
	DELETE FROM DPA_TRASM_UTENTE WHERE id_trasm_singola in
		SELECT system_id FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in 
			(select	t.system_id  from dpa_trasmissione t where	t.id_profile =@idProfile))
							
	DELETE FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in 
		(select	t.system_id  from dpa_trasmissione t where	t.id_profile =@idProfile)
					 	
	DELETE FROM DPA_TRASMISSIONE WHERE id_profile=@idProfile
			   			  
	DELETE FROM project_components where LINK =@idProfile
	DELETE FROM VERSIONS WHERE DOCNUMBER = @idProfile
	DELETE FROM COMPONENTS WHERE DOCNUMBER = @idProfile
	DELETE FROM DPA_AREA_LAVORO WHERE ID_PROFILE = @idProfile
	DELETE FROM DPA_PROF_PAROLE WHERE ID_PROFILE = @idProfile
	DELETE FROM PROFILE WHERE DOCNUMBER = @idProfile
	DELETE FROM SECURITY WHERE THING = @idProfile
	delete from dpa_todolist where id_profile = @idProfile
		 
	DELETE FROM DPA_ASSOCIAZIONE_TEMPLATES WHERE DOC_NUMBER = @idProfile
	RETURN @ReturnValue
END
GO
