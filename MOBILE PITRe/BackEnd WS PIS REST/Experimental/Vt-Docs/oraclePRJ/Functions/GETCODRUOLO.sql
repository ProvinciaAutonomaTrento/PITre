--------------------------------------------------------
--  DDL for Function GETCODRUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUOLO" (idGruppo INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select var_COD_RUBRICA into risultato from dpa_corr_globali where id_Gruppo=idGruppo;
RETURN risultato;
END getCODRuolo; 

/
