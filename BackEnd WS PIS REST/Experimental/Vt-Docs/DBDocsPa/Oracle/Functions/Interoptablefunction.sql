
CREATE OR REPLACE FUNCTION @db_user.Interoptablefunction (anno NUMBER,id_registro NUMBER)
RETURN InteropTableRow PIPELINED IS
--inizializzazione
out_rec_amm InteropTableType := InteropTableType(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
out_rec_anno InteropTableType := InteropTableType(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
-- variabili del cursore
VAR_CODICE_AMM VARCHAR (255);
VAR_CODICE_AOO VARCHAR (255);
DOC_SPEDITI NUMBER;
MESE NUMBER;
--Dichiarazione del cursore
CURSOR c_data (a NUMBER,reg NUMBER) IS
SELECT DISTINCT(COUNT(*)),TO_NUMBER(TO_CHAR(dta_spedizione,'MM')),var_codice_amm,var_codice_aoo
FROM PROFILE p,DPA_STATO_INVIO si
WHERE cha_tipo_proto = 'P'
AND id_registro = reg
AND p.system_id = si.id_profile
AND TO_NUMBER(TO_CHAR(dta_spedizione,'YYYY')) = a
AND var_codice_amm IS NOT NULL
AND  UPPER(var_codice_aoo) <> UPPER (Getregdescr(reg))
GROUP BY ROLLUP (var_codice_amm, var_codice_aoo,TO_NUMBER(TO_CHAR(dta_spedizione,'MM')));

BEGIN
OPEN c_data(anno,id_registro);
--set iniziale variabili
out_rec_amm.gennaio:=0;
out_rec_amm.febbraio:=0;
out_rec_amm.marzo:=0;
out_rec_amm.aprile:=0;
out_rec_amm.maggio:=0;
out_rec_amm.giugno:=0;
out_rec_amm.luglio:=0;
out_rec_amm.agosto:=0;
out_rec_amm.settembre:=0;
out_rec_amm.ottobre:=0;
out_rec_amm.novembre:=0;
out_rec_amm.dicembre:=0;
LOOP

FETCH c_data INTO DOC_SPEDITI,MESE,VAR_CODICE_AMM,VAR_CODICE_AOO;
EXIT WHEN c_data%NOTFOUND;
IF ((DOC_SPEDITI<> 0 ) AND (MESE<> 0) AND (VAR_CODICE_AMM IS NOT NULL) AND (VAR_CODICE_AOO IS NOT NULL)) THEN

out_rec_amm.var_cod_amm:=VAR_CODICE_AMM;
out_rec_amm.var_cod_aoo:=VAR_CODICE_AOO;
IF (MESE = 1) THEN
out_rec_amm.GENNAIO:= DOC_SPEDITI;
END IF;
IF (MESE = 2) THEN
out_rec_amm.FEBBRAIO:= DOC_SPEDITI;
END IF;
IF (MESE = 3) THEN
out_rec_amm.MARZO:= DOC_SPEDITI;
END IF;
IF (MESE = 4) THEN
out_rec_amm.APRILE:= DOC_SPEDITI;
END IF;
IF (MESE = 5) THEN
out_rec_amm.MAGGIO:= DOC_SPEDITI;
END IF;
IF (MESE = 6) THEN
out_rec_amm.GIUGNO:= DOC_SPEDITI;
END IF;
IF (MESE = 7) THEN
out_rec_amm.LUGLIO:= DOC_SPEDITI;
END IF;
IF (MESE = 8) THEN
out_rec_amm.AGOSTO:= DOC_SPEDITI;
END IF;
IF (MESE = 9) THEN
out_rec_amm.SETTEMBRE:= DOC_SPEDITI;
END IF;
IF (MESE = 10) THEN
out_rec_amm.OTTOBRE:= DOC_SPEDITI;
END IF;
IF (MESE = 11) THEN
out_rec_amm.NOVEMBRE:= DOC_SPEDITI;
END IF;
IF (MESE = 12) THEN
out_rec_amm.DICEMBRE:= DOC_SPEDITI;
END IF;
END IF;
-- TOTALE PARZIALE
IF ((DOC_SPEDITI<> 0 ) AND (MESE IS NULL) AND (VAR_CODICE_AMM IS NOT NULL) AND (VAR_CODICE_AOO IS NOT NULL)) THEN
out_rec_amm.tot_m_sped:=DOC_SPEDITI;
out_rec_amm.tot_sped:=0;
PIPE ROW(out_rec_amm);
out_rec_amm.gennaio:=0;
out_rec_amm.febbraio:=0;
out_rec_amm.marzo:=0;
out_rec_amm.aprile:=0;
out_rec_amm.maggio:=0;
out_rec_amm.giugno:=0;
out_rec_amm.luglio:=0;
out_rec_amm.agosto:=0;
out_rec_amm.settembre:=0;
out_rec_amm.ottobre:=0;
out_rec_amm.novembre:=0;
out_rec_amm.dicembre:=0;
END IF;

-- TOTALE
IF ((DOC_SPEDITI<> 0 ) AND (MESE IS NULL) AND (VAR_CODICE_AMM IS NULL) AND (VAR_CODICE_AOO IS NULL)) THEN
-- INSERIMENTO DEL TOTALE FINALE
out_rec_anno.var_cod_amm := '0';
out_rec_anno.var_cod_aoo:='0';
out_rec_anno.gennaio:='0';
out_rec_anno.febbraio:='0';
out_rec_anno.marzo:='0';
out_rec_anno.aprile:='0';
out_rec_anno.maggio:='0';
out_rec_anno.giugno:='0';
out_rec_anno.luglio:='0';
out_rec_anno.agosto:='0';
out_rec_anno.settembre:='0';
out_rec_anno.ottobre:='0';
out_rec_anno.novembre:='0';
out_rec_anno.dicembre:='0';
out_rec_anno.tot_m_sped:='0';
out_rec_anno.tot_sped:=DOC_SPEDITI;
PIPE ROW(out_rec_anno);
END IF;

END LOOP;--fine del cursore
CLOSE c_data;
RETURN;

EXCEPTION WHEN OTHERS THEN
BEGIN
RETURN;
END;
END Interoptablefunction;
/