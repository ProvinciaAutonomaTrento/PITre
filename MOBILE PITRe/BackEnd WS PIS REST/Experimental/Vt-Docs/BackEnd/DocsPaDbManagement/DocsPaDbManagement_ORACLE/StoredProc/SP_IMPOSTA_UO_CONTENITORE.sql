CREATE OR REPLACE PROCEDURE SP_IMPOSTA_UO_CONTENITORE (v_id_amm in number)
IS

-- dichiarazione variabili
v_num_livello number;
v_system_id varchar2(32);
v_var_desc_amm varchar2(32);
v_var_codice_amm varchar2(32);
v_identity number;
counter number;
counter2 number;
avanti number;

-- fine dichiarazione 

	BEGIN  -- INIZIO 4° BEGIN 
		 
		 SELECT count(*) INTO avanti
		 FROM DPA_CORR_GLOBALI 
		 WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' 
		 AND ID_AMM = v_id_amm and  NUM_LIVELLO = 0 and id_parent = 0;
		
		IF (avanti > 1) THEN
		 BEGIN -- INIZIO 3°BEGIN    			
 	
			  SELECT  VAR_DESC_AMM , 
					 VAR_CODICE_AMM into v_var_desc_amm, v_var_codice_amm 
			  FROM DPA_AMMINISTRA 
			  WHERE SYSTEM_ID = v_id_amm;
		
			  SELECT count(*) INTO counter 
			  FROM DPA_CORR_GLOBALI 
			  WHERE VAR_COD_RUBRICA = v_var_codice_amm OR VAR_CODICE = v_var_codice_amm;
			  
			  IF (counter = 0) THEN
			
			  	 BEGIN -- 1° BEGIN  
				   	 
		 	  	   	  UPDATE DPA_CORR_GLOBALI 
			  	   	  SET NUM_LIVELLO = NUM_LIVELLO+1 
			  	   	  WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = v_id_amm;
					 
					  
				     --INSERISCO IL CORRISPONDENTE DI LIVELLO 0   
					 INSERT INTO DPA_CORR_GLOBALI
					 (
						SYSTEM_ID,
						ID_AMM,
						VAR_COD_RUBRICA,
						VAR_DESC_CORR,
						ID_PARENT, 
						NUM_LIVELLO, 
						VAR_CODICE, 
						CHA_TIPO_IE, 
						CHA_TIPO_CORR, 
						CHA_TIPO_URP
						) 
						VALUES (	
						seq.nextval,
						v_id_amm,
						v_var_codice_amm,
						v_var_desc_amm,
						0,
						0,
						v_var_codice_amm,
						'I',
						'S',
						'U'
					   );
							
					   UPDATE DPA_CORR_GLOBALI 
					   set ID_PARENT = SEQ.CURRVAL
					   WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = v_id_amm
					   AND NUM_LIVELLO = 1;
					    
					   commit;
																
				END;-- FINE 1° BEGIN 
			
			ELSE 
				dbms_output.put_line('ATTENZIONE: VAR_CODICE O CODICE RUBRICE PRESENTI - LA PROCEDURA TERMINA QUI');				
			END IF;
					
		END;-- FINE 3° BEGIN 
	 END IF;
	
 END;-- FINE 4° BEGIN
/