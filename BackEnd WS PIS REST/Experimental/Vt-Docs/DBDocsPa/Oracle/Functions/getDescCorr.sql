CREATE OR REPLACE FUNCTION @db_user.getDescCorr (corrId INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select var_desc_corr into risultato from dpa_corr_globali where system_id=corrId;
RETURN risultato;
END getDescCorr;
/