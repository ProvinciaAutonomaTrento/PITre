CREATE or replace FUNCTION getdatainserimentoLF(
    sysid      NUMBER,
    iddocprinc NUMBER)
  RETURN DATE
IS
  risultato DATE;
BEGIN
  IF iddocprinc = NULL THEN
    SELECT data_inserimento
    INTO risultato
    FROM DPA_ELEMENTO_IN_LIBRO_FIRMA
    WHERE DOC_NUMBER=sysid;
  ELSE 
    BEGIN
      SELECT data_inserimento
      INTO risultato
      FROM DPA_ELEMENTO_IN_LIBRO_FIRMA
      WHERE DOC_NUMBER=iddocprinc;
    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      SELECT data_inserimento
      INTO risultato
      FROM DPA_ELEMENTO_IN_LIBRO_FIRMA
      WHERE DOC_NUMBER=sysid;
    END;
  END IF;
RETURN risultato;
END getdatainserimentoLF;