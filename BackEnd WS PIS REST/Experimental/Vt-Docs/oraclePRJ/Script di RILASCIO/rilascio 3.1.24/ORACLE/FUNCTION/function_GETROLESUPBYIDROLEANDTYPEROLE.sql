create or replace
FUNCTION             GETROLESUPBYIDROLEANDTYPEROLE
(
  idGruppo number,
  idTipoRuolo number
)
RETURN number IS 
risultato number;
idUO number;

BEGIN
  
	SELECT id_uo
	into idUO
	FROM dpa_corr_globali
	WHERE ID_GRUPPO = idGruppo;
	
	SELECT ID_GRUPPO
	INTO risultato
      FROM DPA_CORR_GLOBALI
      WHERE ID_TIPO_RUOLO = idTipoRuolo and dta_fine is null
      AND ID_UO IN 
      (
		  SELECT system_id
		  FROM dpa_corr_globali 
		  WHERE dpa_corr_globali.dta_fine IS NULL
		  CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id
		  START WITH dpa_corr_globali.system_id = idUO
      ) 
	  and rownum = 1;
	  
	RETURN risultato;
  
END GETROLESUPBYIDROLEANDTYPEROLE;