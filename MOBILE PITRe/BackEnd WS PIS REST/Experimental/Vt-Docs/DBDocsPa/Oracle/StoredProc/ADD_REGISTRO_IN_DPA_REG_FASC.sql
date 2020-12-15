CREATE OR REPLACE PROCEDURE @db_user.ADD_REGISTRO_IN_DPA_REG_FASC(p_newIdRegistro number, p_id_amm number, p_result OUT integer) IS
BEGIN
DECLARE
CURSOR currTit IS
SELECT system_id
FROM project
WHERE ID_AMM = p_id_amm
AND CHA_TIPO_PROJ= 'T' AND ID_REGISTRO IS NULL;
BEGIN
-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
p_result:=0;
FOR currentTit IN currTit
LOOP
begin

INSERT INTO DPA_REG_FASC
(
system_id,
id_titolario,
num_rif,
id_registro
)
VALUES
(
seq.nextval,
currentTit.system_id,
1,
p_newIdRegistro
);

EXCEPTION
WHEN OTHERS THEN
p_result := 1;
RETURN;
end;

END LOOP;

end;


END ADD_REGISTRO_IN_DPA_REG_FASC;
/