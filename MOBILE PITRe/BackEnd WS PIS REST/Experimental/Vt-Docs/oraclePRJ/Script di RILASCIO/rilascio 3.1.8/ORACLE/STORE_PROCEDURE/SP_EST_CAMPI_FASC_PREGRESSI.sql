create or replace PROCEDURE SP_EST_CAMPI_FASC_PREGRESSI 
( id_oggetto_in IN NUMBER
, id_template_in IN NUMBER
, anno_in IN NUMBER
, id_aoo_rf_in IN NUMBER
, returnvalue OUT NUMBER
) 
/******************************************************************************

  AUTHOR:  Lembo Alessandro

  NAME:     SP_EST_CAMPI_FASC_PREGRESSI

  PURPOSE:  Store procedure per propagare l'aggiunta di campi in una tipologia ai fascicoli pregressi.

******************************************************************************/
IS
--prelevo i fasc pregressi
CURSOR FASC_PREGRESSI
IS
SELECT DISTINCT(id_project) 
FROM DPA_ASS_TEMPLATES_FASC 
WHERE ID_TEMPLATE= id_template_in 
AND id_project IS NOT NULL;

id_fasc NUMBER(15);

BEGIN
OPEN FASC_PREGRESSI;
LOOP FETCH FASC_PREGRESSI INTO id_fasc;
EXIT WHEN FASC_PREGRESSI%NOTFOUND;
      INSERT INTO DPA_ASS_TEMPLATES_FASC
      (
      SYSTEM_ID, 
      ID_OGGETTO,
      ID_TEMPLATE,
      id_project,
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
      SEQ_DPA_ASS_TEMPLATES_FASC.nextval, 
      id_oggetto_in,
      id_template_in,
      id_fasc,
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
      CLOSE FASC_PREGRESSI;
      returnvalue:=0;
EXCEPTION WHEN OTHERS THEN returnvalue:=-1; RETURN;

END SP_EST_CAMPI_FASC_PREGRESSI;