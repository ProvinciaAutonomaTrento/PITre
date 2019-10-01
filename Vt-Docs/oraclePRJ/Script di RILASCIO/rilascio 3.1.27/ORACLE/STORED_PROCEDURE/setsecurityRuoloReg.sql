create or replace
PROCEDURE           setsecurityRuoloReg(
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
            CHA_TIPO_DIRITTO,
			VAR_NOTE_SEC
          )
          VALUES
          (
            idProfile,
            idGruppo ,
            diritto ,
            'A',
			'ACQUISITO PERCHE'' RESPONSABILE DI REGISTRO'
          );
      END;
    END IF;
  END IF;
 
  RETURNVALUE := DIRITTO;
END setsecurityRuoloReg;