CREATE OR REPLACE FUNCTION @db_user.TempiMediCompattaTableFunction (ID_AMM number, ID_REGISTRO number, ANNO number)
RETURN TempiMediTableRow PIPELINED IS
--dichiarazione
out_rec TempiMediTableType := TempiMediTableType(NULL,NULL,NULL);
--DICHIARAZIONI VARIABILI

CodClass varchar (255);
DescClass varchar (255);
ContaFasc number;
valore float;
tempoMedio float;
-- variabili ausiliarie per il cursore che recupera le voci di titolario
SYSTEM_ID_VT number;
DESCRIPTION_VT VARCHAR (255);
VAR_CODICE_VT VARCHAR (255);
--variabili ausuliarie per il cursore dei fascicoli
DTA_CREAZIONE DATE;
DTA_CHIUSURA DATE;
INTERVALLO INT;

NUM_LIVELLO1 VARCHAR(255);
VAR_CODICE_LIVELLO1 VARCHAR(255);
DESCRIPTION__LIVELLO1 VARCHAR(255);
TOT_VT number;
CONTATORE number;
tmp float;

--Dichiarazione dei cursori
--Cursore per le voci di titolario
CURSOR c_VociTit (amm number, reg number) is
select project.system_id,project.description,project.var_codice,project.num_livello  from project
where project.cha_tipo_proj = 'T' and project.var_codice is not null and
project.id_amm =amm and (project.id_registro = reg OR project.id_registro is null)
order by project.var_cod_liv1;

-- contiene tutti i fascicoli (TIPO "F")
CURSOR c_Fascicoli (amm number, parentId number) is
select project.dta_creazione, project.dta_chiusura
from project
where project.cha_tipo_proj = 'F' and project.id_amm = amm
and project.id_parent = parentId;
c1 c_Fascicoli%ROWTYPE;

begin
--SETTAGGIO INIZIALE VARIABILI
ContaFasc := 0;
tempoMedio := 0;
valore := 0;
intervallo := 0;
CONTATORE := 0;
TOT_VT := 0;
tmp := 0;

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
Open c_VociTit(ID_AMM,ID_REGISTRO);
LOOP
FETCH c_VociTit into SYSTEM_ID_VT,DESCRIPTION_VT,VAR_CODICE_VT,NUM_LIVELLO1;
EXIT WHEN c_VociTit%NOTFOUND;

if(NUM_LIVELLO1 = 1)
then
VAR_CODICE_LIVELLO1 := VAR_CODICE_VT;
DESCRIPTION__LIVELLO1 := DESCRIPTION_VT;
CONTATORE := 0;
end if;
CONTATORE := CONTATORE +1;
--2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
open c_Fascicoli (ID_AMM , SYSTEM_ID_VT );
LOOP
FETCH c_Fascicoli into c1;
EXIT WHEN c_Fascicoli%NOTFOUND;
--conto le differenze parziali di tutti i fascicoli contenuti nella voce di titolario selezionata
IF ((c1.dta_creazione IS NOT NULL) AND (c1.dta_chiusura IS NOT NULL))
then
contaFasc := contaFasc + 1;
tmp := c1.dta_chiusura - c1.dta_creazione;
tmp := tmp + 1;
tmp := ROUND (tmp);
intervallo := intervallo + tmp;
end if;
----------------- end
END LOOP;
--(FINE 2 ciclo)
close c_Fascicoli;

--converto i valori trovati e calcolo il tempo di lavorazione medio di tutti i fascicoli della voce di titolario prescelta
if ((intervallo <> 0) and (contaFasc <> 0))
then
tempoMedio := round(intervallo / contaFasc,2);
TOT_VT := TOT_VT + tempoMedio;
if(tempoMedio < 0) then
tempoMedio:=0;
TOT_VT := 0;
end if;
end if;
-- INSERISCO I VALORI TROVATI NELLA TABELLA TEMPORANEA
if(NUM_LIVELLO1=1)
then
out_rec.COD_CLASS := VAR_CODICE_LIVELLO1;
out_rec.DESC_CLASS := DESCRIPTION__LIVELLO1;
out_rec.T_MEDIO_LAV := TOT_VT/ CONTATORE;

PIPE ROW(out_rec);
end if;
-- reset delle variabili di conteggio
contaFasc := 0;
intervallo := 0;
tempoMedio := 0;
TOT_VT := 0;

END LOOP;
--(FINE 1 ciclo)
close c_VociTit;
RETURN;

EXCEPTION WHEN OTHERS THEN
RETURN;
END TempiMediCompattaTableFunction;
/