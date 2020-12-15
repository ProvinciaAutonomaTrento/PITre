--------------------------------------------------------
--  DDL for Procedure UPDATE_CODLIV1
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UPDATE_CODLIV1" (v_id_amm in number)
IS
v_id_parent number;
v_system_id number;
v_id_parent_2 number;
v_num_livello  number;
v_var_cod_liv1 varchar2(32);
cont2 number;

maxliv number;
cursor c2 (v_num_livello NUMBER) is
SELECT system_id, id_parent from project where num_livello = v_num_livello
and id_amm=v_id_amm and cha_tipo_proj= 'T'  ORDER BY ID_PARENT, system_id;

BEGIN


UPDATE PROJECT SET VAR_COD_LIV1 = '0000'
WHERE ID_AMM=v_id_amm
AND cha_tipo_proj= 'T'
AND num_livello = 0;

FOR CNT IN 1 .. 7 LOOP

BEGIN


OPEN c2(CNT);
cont2 := 0;
FETCH c2 INTO v_system_id, v_id_parent;
v_id_parent_2:= v_id_parent;

LOOP

IF v_id_parent_2 <> v_id_parent then
cont2 := 0;
v_id_parent_2 := v_id_parent ;

END IF;


cont2 := cont2 + 1;

IF (CNT = 1) THEN

BEGIN

IF cont2 <= 9 THEN
update project set var_cod_liv1 = '000' || TO_CHAR(cont2) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 9 AND cont2 <= 99 THEN
update project set var_cod_liv1 = '00' || TO_CHAR(cont2) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 99 AND cont2 <= 999 THEN
update project set var_cod_liv1 = '0' || TO_CHAR(cont2) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 999 THEN
update project set var_cod_liv1 = TO_CHAR(cont2) where num_livello = 1 and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;

END;

ELSE
BEGIN

select var_cod_liv1 into v_var_cod_liv1
from project
where system_id = v_id_parent
and id_amm=v_id_amm
and cha_tipo_proj= 'T' and num_livello = CNT - 1;

IF cont2 <= 9 THEN
update project set var_cod_liv1 = v_var_cod_liv1 || '000' || TO_CHAR(cont2) where num_livello = CNT and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 9 AND cont2 <= 99 THEN
update project set var_cod_liv1 = v_var_cod_liv1 || '00' || TO_CHAR(cont2) where num_livello = CNT and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 99 AND cont2 <= 999 THEN
update project set var_cod_liv1 = v_var_cod_liv1 || '0' || TO_CHAR(cont2) where num_livello = CNT and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;
IF cont2 > 999 THEN
update project set var_cod_liv1 = v_var_cod_liv1 || TO_CHAR(cont2) where num_livello = CNT and id_amm=v_id_amm and cha_tipo_proj= 'T' and system_id=v_system_id;
END IF;

END;

END IF;


FETCH c2 INTO v_system_id, v_id_parent;
EXIT WHEN c2%NOTFOUND;
END LOOP;
close c2;
commit;
END;


END LOOP;

END; 

/
