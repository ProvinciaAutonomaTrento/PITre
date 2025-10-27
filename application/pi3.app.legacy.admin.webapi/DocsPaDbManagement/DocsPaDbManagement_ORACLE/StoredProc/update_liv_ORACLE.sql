-- LA PROCEDURA prende in input due parametri:
-- @num_livello di tipo intero : numero di livello che si vuole sistemare
-- @id_amm di tipo intero : sytem_id dell'amministazione di interesse

CREATE OR REPLACE procedure update_liv (v_num_livello in number , v_id_amm in number)
IS
-- dichiarazione variabili
v_id_parent number;
v_system_id number;
v_id_parent_2 number;
v_var_cod_liv1 varchar2(32);
cont2 number;
maxliv number;
-- fine dichiarazione 

-- la query ritorna i system_id associati ai nodi di titolario relativi al numero di livello in input alla procedure
cursor c2 is
SELECT system_id, id_parent from project where num_livello = v_num_livello and id_amm=v_id_amm and cha_tipo_proj= 'T' ORDER BY ID_PARENT, system_id;

BEGIN
SELECT MAX(num_livello) INTO maxliv from project where id_amm=v_id_amm and cha_tipo_proj= 'T';
IF v_num_livello <= maxliv THEN
	open c2;
	cont2 := 0;
	fetch c2 into v_system_id, v_id_parent;
	v_id_parent_2:= v_id_parent;
	
	  LOOP
	   
	   IF v_id_parent_2 <> v_id_parent then
	 	 -- per azzerare il contatore e far ripartire la numerazione dei var_cod_liv 
		 -- quando il record corrente presenta un' id_parent diverso da quello del record precedente
		cont2 := 0;
		v_id_parent_2 := v_id_parent ;
		
		END IF;
		cont2 := cont2 + 1;
		-- viene  ricavato il var cod_liv_1 del padre
		 select var_cod_liv1 into v_var_cod_liv1 from project where system_id = v_id_parent and id_amm=v_id_amm and cha_tipo_proj= 'T' and num_livello = v_num_livello - 1;
		
		 IF cont2 <= 9 THEN
	   	  update project set var_cod_liv1 = v_var_cod_liv1 || '000' || TO_CHAR(cont2) where num_livello = v_num_livello and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
	     END IF;
		 IF cont2 > 9 AND cont2 <= 99 THEN
		  update project set var_cod_liv1 = v_var_cod_liv1 || '00' || TO_CHAR(cont2) where num_livello = v_num_livello and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
		 END IF;
		 IF cont2 > 99 AND cont2 <= 999 THEN
		  update project set var_cod_liv1 = v_var_cod_liv1 || '0' || TO_CHAR(cont2) where num_livello = v_num_livello and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
		 END IF;
		 IF cont2 > 999 THEN
		  update project set var_cod_liv1 = v_var_cod_liv1 || TO_CHAR(cont2) where num_livello = v_num_livello and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
		 END IF;
		FETCH c2 INTO v_system_id, v_id_parent;
	EXIT WHEN c2%NOTFOUND;  -- exit loop if condition is true
	END LOOP;
close c2;
commit;
ELSE
	dbms_output.put_line('L''amministrazione ' || v_id_amm || ' non presenta il livello ' || v_num_livello);
END IF; 

END;
/