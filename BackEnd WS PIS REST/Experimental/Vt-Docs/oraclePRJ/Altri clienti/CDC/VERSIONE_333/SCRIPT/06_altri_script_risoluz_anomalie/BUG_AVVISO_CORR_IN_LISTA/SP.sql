
CREATE OR REPLACE FUNCTION getcodRuolo (idGruppo INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select var_COD_RUBRICA into risultato from dpa_corr_globali where id_Gruppo=idGruppo;
RETURN risultato;
END getCODRuolo;
/
