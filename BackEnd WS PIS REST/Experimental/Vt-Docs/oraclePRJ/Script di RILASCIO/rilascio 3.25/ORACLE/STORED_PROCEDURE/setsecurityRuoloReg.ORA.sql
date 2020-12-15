begin 
Utl_Backup_Plsql_code ('PROCEDURE','setsecurityRuoloReg'); 
end;
/

create or replace
PROCEDURE setsecurityRuoloReg(
    idCorrGlobali IN NUMBER,
    idProfile     IN NUMBER,
    diritto       IN NUMBER,
    Idreg         IN NUMBER,
    ReturnValue OUT NUMBER)
IS
  idGruppo dpa_corr_globali.id_gruppo%TYPE;
BEGIN
  SELECT ID_GRUPPO
  INTO idGruppo
  FROM DPA_CORR_GLOBALI
  WHERE SYSTEM_ID = idCorrGlobali;
  IF (idGruppo   IS NOT NULL) THEN
    BEGIN
      SELECT MAX(accessrights)
      INTO ReturnValue
      FROM security
      WHERE thing       = idProfile
      AND personorgroup = idGruppo;
    END;
    --
    IF (ReturnValue < diritto ) THEN
      BEGIN
        UPDATE security
        SET accessrights  = diritto
        WHERE thing       = idProfile
        AND personorgroup = idGruppo;
      END;
    END IF;
    IF (ReturnValue IS NULL) THEN
      BEGIN
        --insert into security values(idProfile,idGruppo,diritto,null,'A');
        INSERT
        INTO security
          (
            thing ,
            personorgroup,
            ACCESSRIGHTS,
            CHA_TIPO_DIRITTO
          )
          VALUES
          (
            idProfile,
            idGruppo ,
            diritto ,
            'A'
          );
      END;
    END IF;
  END IF;
  INSERT
  INTO security
    (
      THING,
      PERSONORGROUP,
      ACCESSRIGHTS,
      ID_GRUPPO_TRASM,
      CHA_TIPO_DIRITTO
    )
  SELECT SYSTEM_ID,
    idGruppo,
    diritto,
    NULL,
    'A'
  FROM PROFILE p
  WHERE ID_REGISTRO=Idreg
  AND num_proto   IS NOT NULL
  AND NOT EXISTS
    (SELECT 'x'
    FROM SECURITY s1
    WHERE s1.THING      =p.system_id
    AND s1.PERSONORGROUP=idGruppo
    AND s1.ACCESSRIGHTS =diritto
    );
  ReturnValue := diritto;
END setsecurityRuoloReg; 
/
