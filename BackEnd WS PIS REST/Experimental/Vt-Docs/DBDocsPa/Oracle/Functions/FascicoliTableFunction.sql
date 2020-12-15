CREATE OR REPLACE FUNCTION @db_user.FascicoliTableFunction (p_ID_AMM number, p_ID_REGISTRO number, ANNO number, mese number)
RETURN FascicoliTableRow PIPELINED IS
--dichiarazione
out_rec FascicoliTableType := FascicoliTableType(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);

-- variabili globali
totFasc float;
totFascA float;
totFascC float;
MESE_VC varchar(255);

--variabili mensili
contaMese number;
totFascM float;
totFascMA float;
totFascMC float;
totPercFascA float;
totPercFascC float;
begin
--settaggio variabili
totFasc := 0;
totFascA := 0;
totFascC := 0;
contaMese := 1;
totFascM  := 0;
totFascMA := 0;
totFascMC := 0;
totPercFascA := 0;
totPercFascC :=0;



--conta valori globali
-- CONTA FASCICOLI totali nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
select COUNT(project.SYSTEM_ID) into totFasc from project
where project.cha_tipo_proj = 'F'
and project.id_amm = p_ID_AMM
and to_number(to_char(project.dta_creazione,'YYYY')) = anno
and (project.id_registro = p_ID_REGISTRO or project.id_registro is null);


-- CONTA FASCICOLI CREATI NELL'ANNO  nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
select COUNT(project.SYSTEM_ID) into totFascA from project
where project.cha_tipo_proj = 'F'
and to_number(to_char(project.dta_creazione,'YYYY')) = anno
and project.cha_stato = 'A'
and project.id_amm = p_ID_AMM
and (project.id_registro = p_ID_REGISTRO or project.id_registro is null);


-- CONTA FASCICOLI CHIUSI NELL'ANNO nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
select COUNT (project.SYSTEM_ID) into totFascC from project
where project.cha_tipo_proj = 'F' and project.cha_stato = 'C'
and  to_number(to_char(project.dta_chiusura,'YYYY')) = anno
and project.id_amm = p_ID_AMM
and (project.id_registro = p_ID_REGISTRO or project.id_registro is null);
--fine conta


--ciclo scansione mensile
while (mese >= contaMese)
LOOP
--conto  i fascicoli creati (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
select COUNT (project.SYSTEM_ID) into totFascMA from project
where project.cha_tipo_proj = 'F' and project.cha_stato = 'A'
and  to_number(to_char(project.dta_creazione,'MM')) = contaMese and to_number(to_char(project.dta_creazione,'YYYY')) = anno
and project.id_amm = p_ID_AMM and (project.id_registro = p_ID_REGISTRO or project.id_registro is null);

--conto  i fascicoli chiusi (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
select COUNT(project.SYSTEM_ID) into totFascMC from project
where project.cha_tipo_proj = 'F' and project.cha_stato = 'C'
and  to_number(to_char(project.dta_chiusura,'MM')) = contaMese and to_number(to_char(project.dta_chiusura,'YYYY')) = anno
and project.id_amm = p_ID_AMM and (project.id_registro = p_ID_REGISTRO or project.id_registro is null);

totFascM := totFascMA + totFascMC;
--calcolo percentuali
if(totFascM <> 0)
then
TotPercFascA := ROUND(((totFascMA / totFascM) * 100),2);
TotPercFascC := ROUND(((totFascMC / totFascM) * 100),2);
end if;
-- parsing valore mese

MESE_VC :=
CASE contaMese
WHEN 1 THEN 'Gennaio'
WHEN 2 THEN 'Febbraio'
WHEN 3 THEN 'Marzo'
WHEN 4 THEN 'Aprile'
WHEN 5 THEN 'Maggio'
WHEN 6 THEN 'Giugno'
WHEN 7 THEN 'Luglio'
WHEN 8 THEN 'Agosto'
WHEN 9 THEN 'Settembre'
WHEN 10 THEN 'Ottobre'
WHEN 11 THEN 'Novembre'
WHEN 12 THEN 'Dicembre'
end;
--
-- inserimento dati nella tabella temporanea
out_rec.TOTFASC := totFasc;
out_rec.TOTFASCA := totFascA;
out_rec.TOTFASCC := totFascC;
out_rec.MESE := MESE_VC;
out_rec.TOTFASCM := totFascM;
out_rec.TOTFASCMA := totFascMA;
out_rec.TOTFASCMC := totFascMC;
out_rec.TOTPERCFASCA := totPercFascA;
out_rec.TOTPERCFASCC := totPercFascC;

PIPE ROW(out_rec);
--INSERT INTO [docsadm].[#REPORT_ANNUALE_FASCICOLI](TOTFASC,TOTFASCA,TOTFASCC,MESE,TOTFASCM,TOTFASCMA,TOTFASCMC,TOTPERCFASCA,TOTPERCFASCC)
--	VALUES (totFasc, totFascA, totFascC, MESE_VC, totFascM, totFascMA, totFascMC, totPercFascA, totPercFascC)

--reset dei contatori
contaMese := contaMese + 1;
totFascM  := 0;
totFascMA := 0;
totFascMC := 0;
totPercFascA := 0;
totPercFascC := 0;

end loop;
--fine ciclo
RETURN;

EXCEPTION WHEN OTHERS THEN
RETURN;
END FascicoliTableFunction;
/