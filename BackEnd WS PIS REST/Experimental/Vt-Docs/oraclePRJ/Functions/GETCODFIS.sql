--------------------------------------------------------
--  DDL for Function GETCODFIS
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODFIS" (corrId INT)
RETURN varchar IS risultato varchar(16);
BEGIN
select /*+index (a) */ a.var_cod_fiscale var_cod_fiscale into risultato from dpa_dett_globali a where a.id_corr_globali=corrId;
RETURN risultato;
END getCodFis; 

/
