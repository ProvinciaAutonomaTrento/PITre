CREATE OR REPLACE PROCEDURE @db_user.SP_INSERT_AREA_SCARTO 
(
  p_idAmm              NUMBER,
  p_idPeople           NUMBER,
  p_idProfile          NUMBER,
  p_idProject          NUMBER,
  p_oggetto            VARCHAR,
  p_tipoDoc            CHAR,    
  p_idGruppo           NUMBER,
  p_idRegistro         NUMBER,
  p_result             OUT   NUMBER
)
IS
 
idRuoloInUo NUMBER:=0; 
id_scarto_1 NUMBER:=0;
id_scarto_2 NUMBER:=0;
res number:=0;


begin

SELECT seq.NEXTVAL into id_scarto_1 from dual;

SELECT seq.NEXTVAL into id_scarto_2 from dual;

SELECT DPA_CORR_GLOBALI.SYSTEM_ID INTO idRuoloInUo FROM DPA_CORR_GLOBALI WHERE DPA_CORR_GLOBALI.ID_GRUPPO = p_idGruppo;

begin
SELECT DISTINCT DPA_AREA_SCARTO.SYSTEM_ID  INTO res  FROM DPA_AREA_SCARTO WHERE
DPA_AREA_SCARTO.ID_PEOPLE=p_idPeople AND 
DPA_AREA_SCARTO.ID_RUOLO_IN_UO = idRuoloInUo AND
DPA_AREA_SCARTO.CHA_STATO='N';
exception when others then res:=0;
end;
IF (res>0) THEN

    INSERT INTO DPA_ITEMS_SCARTO (
    SYSTEM_ID,
    ID_SCARTO,
    ID_PROFILE,
    ID_PROJECT,
    CHA_TIPO_DOC,
    VAR_OGGETTO,
    ID_REGISTRO,
    DATA_INS,
    CHA_STATO
    )
    VALUES
    (
    id_scarto_1,
    res,
    p_idProfile,         
    p_idProject,         
    p_tipoDoc,          
    p_oggetto,
    p_idRegistro,
    sysdate,
    'N'
    );
    p_result:=id_scarto_1;

ELSE

    INSERT INTO DPA_AREA_SCARTO (
    SYSTEM_ID,           
    ID_AMM,                 
    ID_PEOPLE,              
    ID_RUOLO_IN_UO,         
    CHA_STATO,                     
    DATA_APERTURA
    )
    VALUES(
    id_scarto_1,
    p_idAmm,
    p_idPeople,
    idRuoloInUo,
    'N',
    sysdate
    );

    INSERT INTO DPA_ITEMS_SCARTO (
    SYSTEM_ID,
    ID_SCARTO,
    ID_PROFILE,
    ID_PROJECT,
    CHA_TIPO_DOC,
    VAR_OGGETTO,
    ID_REGISTRO,
    DATA_INS,
    CHA_STATO
    )
    VALUES
    (
    id_scarto_2,
    id_scarto_1,
    p_idProfile,         
    p_idProject,         
    p_tipoDoc,          
    p_oggetto,
    p_idRegistro,
    sysdate,
    'N'
    );

    p_result:=id_scarto_2;

END IF;

exception when others then p_result:=-1;

END SP_INSERT_AREA_SCARTO;
/
