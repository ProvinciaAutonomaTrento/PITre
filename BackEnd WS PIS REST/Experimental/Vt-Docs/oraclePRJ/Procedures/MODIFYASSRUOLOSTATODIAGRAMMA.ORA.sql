CREATE OR REPLACE
PROCEDURE @db_user.MODIFYASSRUOLOSTATODIAGRAMMA
(
    idGruppo    IN NUMBER,
    idStato     IN NUMBER,
    idDiagramma IN NUMBER,
    chaNotVis   IN VARCHAR2,
    result OUT NUMBER)
AS
  counterRecord NUMBER(1,0);
BEGIN
  SELECT COUNT(*)
  INTO counterRecord
  FROM DPA_ASS_RUOLO_STATI_DIAGRAMMA
  WHERE id_diagramma = idDiagramma
  AND id_stato       = idStato
  AND id_gruppo      = idGruppo;
  -- E' cambiata la visibilità del ruolo sullo stato da non visibile a visibile, quindi elimino
  -- il record associato
  IF chaNotVis = 0 AND counterRecord = 1 THEN
    DELETE
    FROM DPA_ASS_RUOLO_STATI_DIAGRAMMA
    WHERE id_diagramma = idDiagramma
    AND id_stato       = idStato
    AND id_gruppo      = idGruppo;
    result            := 0;
  END IF;
  -- Esiste già l'associazione tra ruolo e stato del diagramma. In realtà questa situazione non dovrebbe verificarsi.
  -- Inserita per prevenire eventuali errori da codice
  IF chaNotVis = 1 AND counterRecord = 1 THEN
    UPDATE DPA_ASS_RUOLO_STATI_DIAGRAMMA
    SET cha_not_vis    = '1'
    WHERE id_diagramma = idDiagramma
    AND id_stato       = idStato
    AND id_gruppo      = idGruppo;
    result            := 1;
  END IF;
  -- Non esiste l'associazione ruolo stato del diagramma. Aggiungo il record
  IF chaNotVis = 1 AND counterRecord = 0 THEN
    INSERT
    INTO DPA_ASS_RUOLO_STATI_DIAGRAMMA VALUES
      (
        seq.nextval,
        idGruppo,
        idDiagramma,
        idStato,
        '1'
      );
    result := 2;
  END IF;
END;
/