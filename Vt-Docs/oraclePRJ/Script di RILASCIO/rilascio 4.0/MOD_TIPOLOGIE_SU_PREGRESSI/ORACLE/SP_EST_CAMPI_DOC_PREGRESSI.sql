create or replace PROCEDURE SP_EST_CAMPI_DOC_PREGRESSI
( id_oggetto_in IN NUMBER
, id_template_in IN NUMBER
, anno_in IN NUMBER
, id_aoo_rf_in IN NUMBER
, returnvalue OUT NUMBER
) 
/******************************************************************************

  AUTHOR:  Lembo Alessandro

  NAME:     SP_EST_CAMPI_DOC_PREGRESSI

  PURPOSE:  Store procedure per propagare l'aggiunta di campi in una tipologia ai documenti pregressi.

******************************************************************************/
IS
--prelevo i doc pregressi
CURSOR DOC_PREGRESSI
IS
SELECT DISTINCT(DOC_NUMBER) 
FROM DPA_ASSOCIAZIONE_TEMPLATES 
WHERE ID_TEMPLATE= id_template_in 
AND DOC_NUMBER IS NOT NULL;

id_doc NUMBER(15);

BEGIN
OPEN DOC_PREGRESSI;
LOOP FETCH DOC_PREGRESSI INTO id_doc;
EXIT WHEN DOC_PREGRESSI%NOTFOUND;
      INSERT INTO DPA_ASSOCIAZIONE_TEMPLATES
      (
      SYSTEM_ID, 
      ID_OGGETTO,
      ID_TEMPLATE,
      Doc_Number,
      Valore_Oggetto_Db,
      Anno,
      ID_AOO_RF,
      CODICE_DB,
      MANUAL_INSERT,
      VALORE_SC,
      DTA_INS,
      ANNO_ACC
      )
      VALUES
      (
      SEQ_DPA_ASSOCIAZIONE_TEMPLATES.nextval, 
      id_oggetto_in,
      id_template_in,
      id_doc,
      '',
      anno_in,
      id_aoo_rf_in,
      '',
      0,
      NULL,
      SYSDATE,
      ''
      );
      END LOOP;
      CLOSE DOC_PREGRESSI;
      returnvalue:=0;
EXCEPTION WHEN OTHERS THEN returnvalue:=-1; RETURN;

END SP_EST_CAMPI_DOC_PREGRESSI;