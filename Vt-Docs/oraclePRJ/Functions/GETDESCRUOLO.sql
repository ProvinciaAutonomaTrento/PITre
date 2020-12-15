--------------------------------------------------------
--  DDL for Function GETDESCRUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDESCRUOLO" (idGruppo INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select var_desc_corr into risultato from dpa_corr_globali where id_Gruppo=idGruppo;
RETURN risultato;
END getDescRuolo;

/
