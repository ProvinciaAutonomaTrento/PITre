begin 
Utl_Backup_Plsql_code ('FUNCTION','isvalidmodellotrasmissione'); 
end;
/

create or replace
FUNCTION isvalidmodellotrasmissione(
      -- Id del template da analizzare
      templateid NUMBER )
    RETURN NUMBER
  IS
    -- 0 se il modello valido, -1 se verificato un errore, un numero maggiore
    -- di 0 se ci sono ruoli inibiti
    retval NUMBER;
    /******************************************************************************
    AUTHOR:    Samuele Furnari
    NAME:       IsValidModelloTrasmissione
    PURPOSE:
    Funzione per la verifica di validita di un modello di trasmissione.
    Per essere valido, il modello di cui viene passato l'id non deve
    contenere destinatari inibiti alla ricezione di trasmissioni
    ******************************************************************************/
  BEGIN
    retval := 0;
    -- Conteggio dei destinatari inibiti alla ricezione di trasmissioni
    SELECT COUNT ('x')
    INTO retval
    FROM dpa_modelli_mitt_dest,
      dpa_corr_globali
    WHERE id_modello                             = templateid
    AND dpa_modelli_mitt_dest.cha_tipo_urp       = 'R'
    AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D'
    AND dpa_modelli_mitt_dest.id_corr_globali    = dpa_corr_globali.system_id
    AND ( dpa_corr_globali.cha_disabled_trasm    = '1'
    OR dpa_corr_globali.dta_fine                IS NOT NULL );
    RETURN retval;
  EXCEPTION
  WHEN OTHERS THEN
    RETURN -1;
  END isvalidmodellotrasmissione;
/
