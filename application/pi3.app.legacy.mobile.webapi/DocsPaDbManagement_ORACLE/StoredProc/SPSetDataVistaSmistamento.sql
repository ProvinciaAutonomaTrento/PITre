CREATE OR REPLACE PROCEDURE SPsetDataVistaSmistamento(
p_idPeople IN NUMBER,
p_idOggetto IN NUMBER,
p_idGruppo IN NUMBER,
p_tipoOggetto IN CHAR,
p_idTrasmissione IN NUMBER,
p_resultValue OUT number
) IS 

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

p_cha_tipo_trasm CHAR(1) := NULL;
p_chaTipoDest NUMBER;


BEGIN
	    p_resultValue:=0;
			
		 
	    DECLARE 
		
		CURSOR cursorTrasmSingolaDocumento IS		
		
		SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
 	    FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
 	    WHERE a.system_id = p_idTrasmissione and a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale = 
		(select system_id from dpa_corr_globali where id_gruppo = p_idGruppo)
       		 OR b.id_corr_globale = 
			 (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = p_idPeople))
			 AND a.ID_PROFILE = p_idOggetto and
			 b.ID_RAGIONE = c.SYSTEM_ID;	
		

 BEGIN
 
 
  IF(p_tipoOggetto='D') THEN
  
		 FOR currentTrasmSingola IN cursorTrasmSingolaDocumento
		 LOOP
			 	BEGIN
						
				  	 IF (currentTrasmSingola.cha_tipo_ragione = 'N' OR currentTrasmSingola.cha_tipo_ragione = 'I') then 
					  -- SE è una trasmissione senza workFlow
			 		  	  begin
						  
						  	  -- nella trasmissione utente relativa all'utente che sta vedendo il documento
							  -- setto la data di vista 
							  
					 		  UPDATE DPA_TRASM_UTENTE
							  SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
							  DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END),
					      	  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
							  WHERE 
							  DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
							  AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
							  and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;
								     
							  EXCEPTION
							  WHEN OTHERS THEN  p_resultValue:=1;
							  RETURN;
					     end;
						 
						 begin
						 
						 	  IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') then
				  	 		  -- se è una trasmissione è di tipo SINGOLA a un RUOLO allora devo aggiornare 
							  -- anche le trasmissioni singole relative agli altri utenti del ruolo
							      begin
								  	   -- nelle trasmissioni utente relative agli altri utenti del ruolo 
							  	  	   -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
									   
									  UPDATE DPA_TRASM_UTENTE SET
									  DPA_TRASM_UTENTE.CHA_VISTA = '1',
									  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
									  WHERE 
									  DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
									  AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
									  AND DPA_TRASM_UTENTE.ID_PEOPLE != p_idPeople;
									  
									  EXCEPTION
							  		  WHEN OTHERS THEN  p_resultValue:=1;
							   		  RETURN;
								  end;
								  
							  end if;
						  end;
					   ELSE
					   	   
						   BEGIN
						   	   -- la ragione di trasmissione prevede workflow
						   	   BEGIN
							   
							      UPDATE DPA_TRASM_UTENTE
				  				  SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
				  				  DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  sysdate ELSE DTA_VISTA END),
				  				  DTA_ACCETTATA = (CASE WHEN DTA_ACCETTATA IS NULL THEN  sysdate ELSE DTA_ACCETTATA END),
				  				  CHA_ACCETTATA = '1', CHA_IN_TODOLIST = '0'
	
								  WHERE  
				  				  DPA_TRASM_UTENTE.CHA_ACCETTATA = '0'
				  				  AND id_trasm_singola = currentTrasmSingola.SYSTEM_ID
				  				  and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;
					  
								    EXCEPTION
								  		  WHEN OTHERS THEN  p_resultValue:=1;
								   		  RETURN;
								 
								 END;	  
									
								BEGIN	
										-- se la trasm è con WorkFlow ed è di tipo UNO e il dest è Ruolo allora levo la validità della
									-- trasmissione a tutti gli altri utenti del ruolo
									 IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') THEN
										
										begin
											UPDATE DPA_TRASM_UTENTE SET
											  DPA_TRASM_UTENTE.CHA_VALIDA= '0',
											  DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0' 
											  WHERE 
											 -- DPA_TRASM_UTENTE.DTA_VISTA IS NULL 
											  id_trasm_singola =  currentTrasmSingola.SYSTEM_ID
											  AND DPA_TRASM_UTENTE.ID_PEOPLE !=p_idPeople;
											
											 EXCEPTION
									  		  WHEN OTHERS THEN  p_resultValue:=1;
									   		  RETURN;
											  
					 
										end;
									END IF;
								END;
					
						   END;
					  
					   END IF;
					 
		  		 END;
		  
		 END LOOP;
 END IF;
 
 
 END;    
END SPsetDataVistaSmistamento;
/
