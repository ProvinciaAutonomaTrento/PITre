
CREATE  PROCEDURE [DOCSADM].[SPsetDataVistaSmistamento]
@idPeople INT,
@idOggetto INT,
@idGruppo INT,
@tipoOggetto CHAR(1), 
@idTrasmissione INT,
@resultValue int out 
AS 

/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
     -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
		(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
	 -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
	    relativa all'utente corrente					 

dpa_trasm_singola.cha_tipo_trasm = 'T''
     -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
		(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
     -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
	    relativa all'utente corrente	

*/



--DECLARE @resultValue INT

DECLARE @sysTrasmSingola INT
DECLARE @chaTipoTrasm CHAR(1)
DECLARE @chaTipoRagione CHAR(1)
DECLARE @chaTipoDest CHAR(1)

BEGIN
	    SET @resultValue = 0
			
		 
	    DECLARE cursorTrasmSingolaDocumento CURSOR FOR	
		
		SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
 	    FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
 	    WHERE a.system_id = @idTrasmissione and a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale = 
		(select system_id from dpa_corr_globali where id_gruppo = @idGruppo)
       		 OR b.id_corr_globale = 
			 (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = @idPeople))
			 AND a.ID_PROFILE = @idOggetto and
			 b.ID_RAGIONE = c.SYSTEM_ID	
	 
  IF(@tipoOggetto='D') 
  
	
	OPEN cursorTrasmSingolaDocumento
	FETCH NEXT FROM cursorTrasmSingolaDocumento
	INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest

	BEGIN

	WHILE @@FETCH_STATUS = 0
	
		begin

  	 	IF (@chaTipoRagione = 'N' OR @chaTipoRagione = 'I')
	      	
		  BEGIN
			  
				-- SE è una trasmissione senza workFlow
			  	BEGIN 
				  	  -- nella trasmissione utente relativa all'utente che sta vedendo il documento
					  -- setto la data di vista 
					  
			 		  UPDATE DPA_TRASM_UTENTE
					  SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
					  DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
			      	               DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
					  WHERE 
					  DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
					  AND id_trasm_singola = @sysTrasmSingola
					  and DPA_TRASM_UTENTE.ID_PEOPLE =@idPeople
						     
					 IF (@@ERROR <> 0)
						BEGIN
						 	SET @resultValue=1
						        return @resultValue
						END
	           	       	             END
		 
	 	   		IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R') 
	 		  	   	 -- se è una trasmissione è di tipo SINGOLA a un RUOLO allora devo aggiornare 
		          	      		-- anche le trasmissioni singole relative agli altri utenti del ruolo
		      			
					BEGIN

			  	  	 -- nelle trasmissioni utente relative agli altri utenti del ruolo 
		  	  	   	-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
				   
					  UPDATE DPA_TRASM_UTENTE SET
					  DPA_TRASM_UTENTE.CHA_VISTA = '1',
					  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
					  WHERE 
					  DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
					  AND id_trasm_singola = @sysTrasmSingola
					  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
					  
						  
				  	 IF (@@ERROR <> 0)
						BEGIN
						 	SET @resultValue=1
						        return @resultValue
						END
		      			END
				
	       	      END  
			  		  
	   	ELSE
	   	   	-- la ragione di trasmissione prevede workflow
	   	   	BEGIN
		   
			    UPDATE DPA_TRASM_UTENTE
				  SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
				  DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  GETDATE() ELSE DTA_VISTA END),
				  DTA_ACCETTATA = (CASE WHEN DTA_ACCETTATA IS NULL THEN  GETDATE() ELSE DTA_ACCETTATA END),
				  CHA_ACCETTATA = '1', CHA_IN_TODOLIST = '0'

				WHERE  
				  DPA_TRASM_UTENTE.CHA_ACCETTATA = '0'
				  AND id_trasm_singola = @sysTrasmSingola
				  and DPA_TRASM_UTENTE.ID_PEOPLE = @idPeople
				  
				  IF (@@ERROR <> 0)
					BEGIN
						SET @resultValue=1
						return @resultValue
					END
 			
				-- se la trasm è con WorkFlow ed è di tipo UNO e il dest è Ruolo allora levo la validità della
				-- trasmissione a tutti gli altri utenti del ruolo
				IF (@chaTipoTrasm = 'S' AND @chaTipoDest= 'R') 
					
					begin
						UPDATE DPA_TRASM_UTENTE SET
						  DPA_TRASM_UTENTE.CHA_VALIDA= '0',
						  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
						  WHERE 
						 -- DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
						  id_trasm_singola = @sysTrasmSingola
						  AND DPA_TRASM_UTENTE.ID_PEOPLE != @idPeople
						
						IF (@@ERROR <> 0)
							BEGIN
								SET @resultValue=1
								return @resultValue
							END
 
					end
					
			END

	
             FETCH NEXT FROM cursorTrasmSingolaDocumento
	INTO @sysTrasmSingola, @chaTipoTrasm, @chaTipoRagione, @chaTipoDest

	END
    END
    -- rilascio le risorse allocate

 END   
    CLOSE cursorTrasmSingolaDocumento 
    DEALLOCATE cursorTrasmSingolaDocumento


 RETURN @resultValue
GO
