begin 
Utl_Backup_Plsql_code ('FUNCTION','getstatodelega'); 
end;
/

create or replace
FUNCTION getstatodelega(
      id_delega NUMBER)
    RETURN VARCHAR
  IS
    stato_delega VARCHAR2 (100);
    /******************************************************************************
    NAME:       getstatodelega
    PURPOSE:    ritorna lo stato della delega in base ai valori di data_scadenza e data_decorrenza:
    se     data_scadenza < sysdate, lo stato è 'SCADUTA'
    se sysdate < data_decorrenza < data_scadenza  , lo stato è 'IMPOSTATA'
    se data_decorrenza < sysdate < data_scadenza  , lo stato è 'ATTIVA'
    REVISIONS:
    Ver        Date        Author           Description
    ---------  ----------  ---------------  ------------------------------------
    1.0        21/09/2010  Paolo De Luca        1. Created this function.
    NOTES:
    ******************************************************************************/
  BEGIN
    SELECT
      CASE
        WHEN NVL (data_scadenza, SYSDATE + 1) <= SYSDATE
        THEN 'SCADUTA'
        ELSE
          CASE
            WHEN data_decorrenza IS NULL
            THEN 'decorrenza nulla!'
            WHEN data_decorrenza > SYSDATE
            THEN 'IMPOSTATA'
            WHEN data_decorrenza <= SYSDATE
            THEN 'ATTIVA'
            ELSE 'CASO NON PREVISTO'
          END
      END
    INTO stato_delega
    FROM dpa_deleghe
    WHERE system_id = id_delega;
    RETURN stato_delega;
  EXCEPTION
  WHEN NO_DATA_FOUND THEN
    NULL;
  WHEN OTHERS THEN
    stato_delega := '-1';
    RETURN stato_delega;
    -- Consider logging the error and then re-raise
    RAISE;
END getstatodelega;
/
