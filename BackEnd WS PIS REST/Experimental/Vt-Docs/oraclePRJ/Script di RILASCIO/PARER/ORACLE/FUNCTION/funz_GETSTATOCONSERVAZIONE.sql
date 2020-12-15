create or replace 
FUNCTION GETSTATOCONSERVAZIONE(IDPROFILE NUMBER)
RETURN VARCHAR 
IS 
  result VARCHAR(3000);
  idDocPr INT;
BEGIN
  BEGIN
    SELECT cha_stato INTO result
    FROM dpa_versamento
    WHERE id_profile = IDPROFILE;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
	  -- gestione allegati
      BEGIN
        SELECT id_documento_principale INTO idDocPr
        FROM profile
        where system_id= IDPROFILE;
        IF idDocPr IS NOT NULL THEN
          SELECT cha_stato INTO result
          FROM dpa_versamento
          WHERE id_profile = idDocPr;
        ELSE
          result := 'N';
        END IF;
        
      EXCEPTION
        WHEN NO_DATA_FOUND THEN
          result := 'N';
      END;
  END;
  RETURN result;
END GETSTATOCONSERVAZIONE;