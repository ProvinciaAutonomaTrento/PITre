
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE   PROCEDURE [@db_user].[REPORT_ANNUALE_FASCICOLI]
--(INPUT: filtri AOO e anno)
@ID_AMM int,
@ID_REGISTRO int,
@ANNO int,
@MESE INT

as

-- variabili globali
declare @totFasc float
declare @totFascA float
declare @totFascC float
declare @MESE_VC varchar (255)

--variabili mensili
declare @contaMese int
declare @totFascM float
declare @totFascMA float
declare @totFascMC float
declare @totPercFascA float
declare @totPercFascC float

--settaggio variabili
set @totFasc = 0
set @totFascA = 0
set @totFascC = 0
set @contaMese = 1
set @totFascM  = 0
set @totFascMA = 0
set @totFascMC = 0


set @totPercFascA = 0
set @totPercFascC =0


/* Creo una tabella temporanea */
CREATE TABLE [@db_user].[#REPORT_ANNUALE_FASCICOLI]
(
[TOTFASC] [float],
[TOTFASCA] [int],
[TOTFASCC] [int],
[MESE] [varchar] (255),
[TOTFASCM] [int],
[TOTFASCMA] [int],
[TOTFASCMC] [int],
[TOTPERCFASCA] [float],
[TOTPERCFASCC] [float]
)
ON [PRIMARY]
--conta valori globali
-- CONTA FASCICOLI totali nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
set @totFasc = (select COUNT (DISTINCT (SYSTEM_ID)) from project
where cha_tipo_proj = 'F'
and id_amm = @id_amm
and year (dta_creazione) = @anno
and (id_registro = @id_registro or id_registro is null))


-- CONTA FASCICOLI CREATI NELL'ANNO  nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
set @totFascA = (select COUNT (DISTINCT (SYSTEM_ID)) from project
where cha_tipo_proj = 'F'
and  year (dta_creazione) = @anno
and cha_stato = 'A'
and id_amm = @id_amm
and (id_registro = @id_registro or id_registro is null))


-- CONTA FASCICOLI CHIUSI NELL'ANNO nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
set @totFascC = (select COUNT (DISTINCT (SYSTEM_ID)) from project
where cha_tipo_proj = 'F' and cha_stato = 'C'
and  (year(dta_CHIUSURA)=@anno)
and id_amm = @id_amm
and (id_registro = @id_registro or id_registro is null))
--fine conta


--ciclo scansione mensile
while @mese  >= @contaMese
begin
--conto  i fascicoli creati (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
set @totFascMA = (select COUNT (DISTINCT (SYSTEM_ID)) from project
where cha_tipo_proj = 'F' and cha_stato = 'A'
and  month(dta_creazione) = @contaMese and year(dta_creazione)=@anno
and id_amm = @id_amm and (id_registro = @id_registro or id_registro is null))
--conto  i fascicoli chiusi (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
set @totFascMC = (select COUNT (DISTINCT (SYSTEM_ID)) from project
where cha_tipo_proj = 'F' and cha_stato = 'C'
and  month(dta_CHIUSURA) = @contaMese and year(dta_chiusura)=@anno
and id_amm = @id_amm and (id_registro = @id_registro or id_registro is null))
set @totFascM = @totFascMA + @totFascMC
--calcolo percentuali
if(@totFascM <> 0)
begin
if(@totFascMA <> 0)
begin
set @TotPercFascA = ROUND(((@totFascMA / @totFascM) * 100),2)
end
if(@totFascMC <> 0)
begin
set @TotPercFascC = ROUND(((@totFascMC / @totFascM) * 100),2)
end
end
-- parsing valore mese

Select @MESE_VC =
CASE @contaMese
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
end
--
-- inserimento dati nella tabella temporanea
INSERT INTO [@db_user].[#REPORT_ANNUALE_FASCICOLI](TOTFASC,TOTFASCA,TOTFASCC,MESE,TOTFASCM,TOTFASCMA,TOTFASCMC,TOTPERCFASCA,TOTPERCFASCC)
VALUES (@totFasc,@totFascA,@totFascC,@MESE_VC,@totFascM, @totFascMA, @totFascMC, @totPercFascA, @totPercFascC)

--reset dei contatori
set @contaMese = @contaMese + 1
set @totFascM  = 0
set @totFascMA = 0
set @totFascMC = 0
set @totPercFascA = 0
set @totPercFascC =0

end

--fine ciclo

SELECT * FROM #REPORT_ANNUALE_FASCICOLI



GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO