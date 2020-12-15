CREATE OR REPLACE PROCEDURE @db_user.createProjectSP(p_idpeople number, p_description varchar, p_projectId OUT number)
is

BEGIN

DECLARE
projId  number;


BEGIN

p_projectId:=0;

SELECT SEQ.NEXTVAL INTO projId FROM DUAL;

p_projectId:= projId;

<<INSERIMENTO_IN_PROJECT>>
BEGIN
INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED
)
VALUES
(
p_projectId,
p_description,
'Y'
);
EXCEPTION
WHEN OTHERS THEN  p_projectId:=0;
RETURN;

END INSERIMENTO_IN_PROJECT;

<<INSERIMENTO_SECURITY>>
BEGIN
INSERT INTO SECURITY
(
THING,
PERSONORGROUP,
ACCESSRIGHTS,
ID_GRUPPO_TRASM,
CHA_TIPO_DIRITTO
) VALUES (
p_projectId, p_idpeople, 0, NULL, NULL
)	;
EXCEPTION
WHEN OTHERS THEN p_projectId:=0;
return;
END INSERIMENTO_SECURITY;

END;

END;
/


CREATE OR REPLACE PROCEDURE @db_user.CREATE_NEW_NODO_TITOLARIO(p_idAmm number, p_livelloNodo number,
p_description varchar2, p_codiceNodo varchar2, p_idRegistroNodo number, p_idParent number,
p_varCodLiv1 varchar2, p_mesiConservazione number, p_chaRW char, p_idTipoFascicolo number, p_bloccaFascicolo varchar2, p_sysIdTitolario number, p_noteNodo varchar, p_idTitolario OUT number) IS
BEGIN
DECLARE CURSOR currReg IS
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = p_idAmm and cha_rf = '0';

secProj NUMBER;
secFasc NUMBER;
secRoot NUMBER;
varChiaveTit varchar2(64);
varChiaveFasc varchar2(64);
varChiaveRoot varchar2(64);
BEGIN
p_idTitolario:=0;

SELECT SEQ.NEXTVAL INTO secProj FROM DUAL;
p_idTitolario:= secProj;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveTit:= p_idamm ||'_'|| p_codiceNodo || '_' || p_idParent || '_0' ;
else
varChiaveTit:= p_codiceNodo || '_' || p_idParent || '_'  || p_idRegistroNodo;
end if;

BEGIN

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE
)
VALUES
(
secProj,
p_description,
'Y',
'T',
p_codiceNodo,
p_idAmm,
p_idRegistroNodo,
p_livelloNodo,
NULL,
p_idParent,
p_varCodLiv1,
sysdate ,
NULL,
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveTit,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo
);


EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;

END;

BEGIN

SELECT SEQ.NEXTVAL INTO secFasc FROM DUAL;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_0' ;
else
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_'  || p_idRegistroNodo;
end if;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE
)
VALUES
(
secFasc,
p_description,
'Y',
'F',
p_codiceNodo,
P_idAmm,
p_idRegistroNodo,
NULL,
'G',
p_idTitolario,
NULL,
sysdate ,
'A',
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveFasc,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo
);

EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;


BEGIN


if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_0' ;
else
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_'  || p_idRegistroNodo;
end if;

SELECT SEQ.NEXTVAL INTO secRoot FROM DUAL;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE
)
VALUES
(
secRoot,
'Root Folder',
'Y',
'C',
NULL,
p_idAmm,
NULL,
NULL,
NULL,
secFasc,
NULL,
sysDate,
NULL,
secFasc,
p_chaRW,
p_mesiConservazione,
varChiaveRoot,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
IF(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') THEN
FOR currentReg IN currReg
LOOP
BEGIN
INSERT INTO DPA_REG_FASC
(
system_id,
id_Titolario,
num_rif,
id_registro
)
VALUES
(
seq.nextval,
p_idTitolario,
1,
currentReg.system_id
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END LOOP;

-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
NULL	-- SE IL NODO ? COMUNE A TUTTI p_idRegistro = NULL
);

ELSE -- il nodo creato ? associato a uno solo registro

BEGIN
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
p_idRegistroNodo	-- REGISTRO SU CUI ? CRETO IL NODO
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END IF;
end;

END CREATE_NEW_NODO_TITOLARIO;
/