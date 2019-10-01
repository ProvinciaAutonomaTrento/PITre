

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE  procedure [@db_user].[Report_Tempo_Medio_Lav_Compact]


--PARAMETRI DI INPUT
@ID_AMM int,
@ID_REGISTRO int,
@ANNO int

as

--DICHIARAZIONI VARIABILI
declare @CodClass varchar (255)
declare @DescClass varchar (255)
declare @ContaFasc INT
declare @valore float
declare @tempoMedio float

declare @NUM_LIVELLO1 VARCHAR(255)
declare @VAR_CODICE_LIVELLO1 VARCHAR(255)
declare @DESCRIPTION__LIVELLO1 VARCHAR(255)
declare @TOT_VT int
declare @CONTATORE int

--SETTAGGIO INIZIALE VARIABILI
set @ContaFasc = 0
set @tempoMedio = 0
set @valore = 0
set @CONTATORE = 0
set @TOT_VT = 0



--TABELLA TEMPORANEA ALLOCAZIONE RISULTATI
CREATE TABLE [@db_user].[#TEMP_REPORT_LAVORAZIONE_FASCICOLI]
(
[COD_CLASS] [varchar] (255),
[DESC_CLASS] [varchar] (255),
[T_MEDIO_LAV] [INT]

) ON [PRIMARY]

-- variabili ausiliarie per il cursore che recupera le voci di titolario
DECLARE @SYSTEM_ID_VT INT
DECLARE @DESCRIPTION_VT VARCHAR (255)
DECLARE @VAR_CODICE_VT VARCHAR (255)

--variabili ausuliarie per il cursore dei fascicoli
DECLARE @DTA_CREAZIONE DATETIME
DECLARE @DTA_CHIUSURA DATETIME
DECLARE @INTERVALLO INT
set @intervallo = 0

--1° QUERY- elenco voci di titolario  -- (input : @id_amm)
DECLARE c_VociTit CURSOR LOCAL FOR -- contiene tutte le voci di titolario (TIPO "T")
select system_id,description,var_codice,num_livello from project where var_codice is not null and
id_amm =@ID_AMM and cha_tipo_proj = 'T' and (id_registro = @id_registro OR id_registro is null)
order by var_cod_liv1
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1
while(@@fetch_status=0)
BEGIN
--------2° QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
if(@NUM_LIVELLO1 = 1)
Begin
set @VAR_CODICE_LIVELLO1 = @VAR_CODICE_VT
set @DESCRIPTION__LIVELLO1 = @DESCRIPTION_VT
set @CONTATORE = 0
end
set @CONTATORE = @CONTATORE +1;

DECLARE c_Fascicoli CURSOR LOCAL FOR -- contiene tutti i fascicoli (TIPO "F")
select project.dta_creazione, project.dta_chiusura
from project
where cha_tipo_proj = 'F' and id_amm = @ID_AMM
and id_parent = @SYSTEM_ID_VT
OPEN c_Fascicoli
FETCH next from c_Fascicoli into @DTA_CREAZIONE,@DTA_CHIUSURA
while(@@fetch_status=0)
BEGIN
-----------------conto le differenze parziali di tutti i fascicoli contenuti nella voce di titolario selezionata
IF ((@DTA_CREAZIONE IS NOT NULL) AND (@DTA_CHIUSURA IS NOT NULL))
BEGIN
set @contaFasc = @contaFasc + 1
set @intervallo = @intervallo + DATEDIFF(DAY,@DTA_CREAZIONE,@DTA_CHIUSURA)
end
----------------- end
FETCH next from c_Fascicoli into @DTA_CREAZIONE,@DTA_CHIUSURA
END
--(FINE 2 ciclo)
DEALLOCATE c_Fascicoli
--converto i valori trovati e calcolo il tempo di lavorazione medio di tutti i fascicoli della voce di titolario prescelta
if ((@intervallo <> 0) and (@contaFasc <> 0))
begin
set @tempoMedio = @intervallo / @contaFasc
set @TOT_VT = @TOT_VT + @tempoMedio
if(@tempoMedio < 0)
begin
set @tempoMedio = 0
end
end
-- INSERISCO I VALORI TROVATI NELLA TABELLA TEMPORANEA
if(@NUM_LIVELLO1 = 1)
begin
set @TOT_VT = @TOT_VT / @CONTATORE
INSERT INTO #TEMP_REPORT_LAVORAZIONE_FASCICOLI (COD_CLASS,DESC_CLASS,T_MEDIO_LAV)
VALUES (@VAR_CODICE_VT,@DESCRIPTION_VT,@TOT_VT)
end
-- reset delle variabili di conteggio
set @contaFasc = 0
set @intervallo = 0
set @tempoMedio = 0
set @TOT_VT = 0

FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1
END
-- (FINE 1 ciclo)
DEALLOCATE c_VociTit

-- restituzione informazioni richieste
select * from [@db_user].[#TEMP_REPORT_LAVORAZIONE_FASCICOLI]



GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO