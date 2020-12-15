CREATE OR REPLACE PROCEDURE Sp_Dpa_Uo_Reg (
	   	  		  			idUO IN NUMBER, 
							returnValue OUT NUMBER) IS
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

record_corr_ruolo NUMBER;
record_corr_registro NUMBER;

record_trovato NUMBER;
rec NUMBER;
  	 
		CURSOR cursor_ruoli(system_id_uo NUMBER) IS
			    SELECT system_id
				FROM DPA_CORR_GLOBALI
				WHERE
					  cha_tipo_urp = 'R'
					  AND cha_tipo_ie = 'I'
					  AND dta_fine IS NULL
					  AND id_old = 0
					  AND id_uo = system_id_uo;

		CURSOR cursor_registri(system_id_ruolo NUMBER) IS
			    SELECT id_registro
				FROM DPA_L_RUOLO_REG
				WHERE id_ruolo_in_uo = system_id_ruolo;


BEGIN	
	
	BEGIN
		 DELETE FROM DPA_UO_REG WHERE id_uo = idUO;
	EXCEPTION   	   		
		WHEN OTHERS THEN
	   		ReturnValue := 1;
		 	RETURN;
	END;
	
	-- cicla per tutti i ruoli validi della UO passata
	OPEN cursor_ruoli(idUO);
	LOOP
		FETCH cursor_ruoli INTO record_corr_ruolo;
		EXIT WHEN cursor_ruoli%NOTFOUND;
	
	 	BEGIN
	
		 	 -- cicla per tutti i registri associati al ruolo in UO
		 	 OPEN cursor_registri(record_corr_ruolo);
			 LOOP
			 	 FETCH cursor_registri INTO record_corr_registro;
				 EXIT WHEN cursor_registri%NOTFOUND;
		
				 BEGIN
		
					record_trovato := 0;
		
					BEGIN
		
						SELECT
							 system_id INTO rec
						FROM
							 DPA_UO_REG
						WHERE
							 ID_UO = idUO
							 AND ID_REGISTRO = record_corr_registro;
		
					EXCEPTION
						WHEN NO_DATA_FOUND THEN
							 record_trovato := 1;
						WHEN OTHERS THEN
							 ReturnValue := 1;
							 RETURN;
					END;
		
					IF record_trovato = 1 THEN
					   BEGIN
						   INSERT INTO DPA_UO_REG
						   		 (SYSTEM_ID, ID_UO, ID_REGISTRO)
						   VALUES
						   		 (seq.NEXTVAL, idUO, record_corr_registro);
					   EXCEPTION
					   	   WHEN OTHERS THEN
						   		ReturnValue := 1;
							 	RETURN;
					   END;
					END IF;
		
				 END;
		
			 END LOOP;
			 CLOSE cursor_registri;
	
	 	 END;
	
	END LOOP;
	CLOSE cursor_ruoli;

	ReturnValue := 0;
	
END;