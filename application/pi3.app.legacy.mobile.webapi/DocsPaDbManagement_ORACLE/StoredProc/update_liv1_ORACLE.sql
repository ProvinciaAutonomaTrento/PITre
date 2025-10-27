-- LA PROCEDURA update_liv1 prende in input un parametro:
-- @id_amm di tipo intero : sytem_id dell'amministazione di interesse

CREATE OR REPLACE procedure update_liv1 (v_id_amm in number)
IS
-- dichiarazione variabili
rd_parent number;
v_system_id number;
id_parent_2 number;
var_cod_liv1 varchar2(32);
cont number;
-- fine dichiarazione 

cursor c1 is
    SELECT system_id from project where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T'  ORDER BY system_id;

BEGIN

open c1;
cont := 0;
fetch c1 into v_system_id;
  LOOP
   
   cont := cont + 1;
   IF cont <= 9 THEN
   	  update project set var_cod_liv1 = '000' || TO_CHAR(cont) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
   END IF; 
   IF cont > 9 AND cont <= 99 THEN
	  update project set var_cod_liv1 = '00' || TO_CHAR(cont) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
   END IF;
   IF cont > 99 AND cont <= 999 THEN
         update project set var_cod_liv1 = '0' || TO_CHAR(cont) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
   END IF;
   IF cont > 999 THEN
         update project set var_cod_liv1 = TO_CHAR(cont) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
   END IF;

   FETCH c1 INTO v_system_id;
EXIT WHEN c1%NOTFOUND;  -- exit loop if condition is true
END LOOP;
close c1;
commit;
END;
/