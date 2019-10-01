begin 
Utl_Backup_Plsql_code ('PROCEDURE','removetransmodelvisibiletorole'); 
end;
/

create or replace
PROCEDURE removetransmodelvisibiletorole(
    -- Id corr globali del ruolo di cui eleminare i modelli
    rolecorrbglobid IN INTEGER,
    returnvalue OUT INTEGER )
IS
  /******************************************************************************
  NAME:       RemoveTransModelVisibileToRole
  AUTHOR:     Samuele Furnari
  PURPOSE:    Store procedure per la cancellazione di modelli di trasmissione
  visibili solo al ruolo con id corr globali pari a quello
  passato per parametro
  ******************************************************************************/
  idmodello      NUMBER;
  countidmodello NUMBER;
  -- Cursore per scorrere sugli id di modelli di trasmissione che hanno
  -- esattamente un solo mittente.
  CURSOR models
  IS
    Select Md.Id_Modello,
      COUNT(*) AS numMitt
    FROM dpa_modelli_mitt_dest md
    WHERE md.cha_tipo_mitt_dest = 'M'
    AND md.id_corr_globali      = rolecorrbglobid
    GROUP BY md.id_modello
    ORDER BY md.id_modello;
BEGIN
  -- Apertura cursore
  OPEN models;
  LOOP
    FETCH models INTO idmodello, countidmodello;
    EXIT
  WHEN models%NOTFOUND;
    IF countidmodello = 1 THEN
      BEGIN
        -- Cancellazione delle righe della DPA_MODELLI_DEST_CON_NOTIFICA
        DELETE dpa_modelli_dest_con_notifica
        WHERE id_modello = idmodello;
        -- Cancellazione delle righe della DPA_MODELLI_MITT_DEST
        DELETE dpa_modelli_mitt_dest
        WHERE id_modello = idmodello;
        -- Cancellazione della tupla da DPA_MODELLI_TRASM
        DELETE dpa_modelli_trasm
        WHERE system_id = idmodello;
      END;
    END IF;
  END LOOP;
  CLOSE models;
  returnvalue := 0;
EXCEPTION
WHEN OTHERS THEN
  RETURN;
END removetransmodelvisibiletorole;
/
