
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[REPORT_ANNUALE_DOC_X_SEDE]
--parametri di input
@anno int,
@id_registro int,
@id_amm int

AS

--tabella temporanea
CREATE TABLE [@db_user].[#REPORT_ANNUALE_X_SEDE]
(
[ANNO] [varchar] (50),
[SEDE] [varchar] (250),
[TOT_DOC] [varchar] (250),
[GRIGI] [varchar] (250),
[PERC_GRIGI] [varchar] (250),
[PROT] [varchar] (250),
[PERC_PROT] [varchar] (250),
[ANNULL] [varchar] (250),
[PERC_ANNULL] [varchar] (250),
[ARRIVO] [varchar] (250),
[PERC_ARRIVO] [varchar] (250),
[PARTENZA] [varchar] (250),
[PERC_PARTENZA] [varchar] (250),
[INTERNI] [varchar] (250),
[PERC_INTERNI] [varchar] (250),

) ON [PRIMARY]

-- variabili locali
DECLARE @totDoc FLOAT
DECLARE @totGrigi FLOAT
DECLARE @totProt FLOAT
DECLARE @totProtA FLOAT
DECLARE @totProtP FLOAT
DECLARE @totProtI FLOAT
DECLARE @totProtClear FLOAT

DECLARE @totClass FLOAT
DECLARE @totClassGrigi FLOAT
DECLARE @totClassProt float
DECLARE @totClassProtA float
DECLARE @totClassProtP float
DECLARE @totClassProtI float
DECLARE @totClassProtClear float

DECLARE @totProf FLOAT
DECLARE @totProfGrigi FLOAT
DECLARE @totProfProt FLOAT
DECLARE @totProfProtA FLOAT
DECLARE @totProfProtP FLOAT
DECLARE @totProfProtI FLOAT
DECLARE @totProfProtClear FLOAT

DECLARE @PercGrigi float
DECLARE @PercProt float
DECLARE @PercProtA float
DECLARE @PercProtP float
DECLARE @PercProtI float
DECLARE @PercProtClear float

DECLARE @PercClass float
DECLARE @PercClassGrigi float
DECLARE @PercClassProt float
DECLARE @PercClassProtA float
DECLARE @PercClassProtP float
DECLARE @PercClassProtI float
DECLARE @PercClassProtClear float

DECLARE @PercProf FLOAT
DECLARE @PercProfGrigi FLOAT
DECLARE @PercProfProt FLOAT
DECLARE @PercProfProtA FLOAT
DECLARE @PercProfProtP FLOAT
DECLARE @PercProfProtI FLOAT
DECLARE @PercProfProtClear FLOAT

-- VARIABILE DEL CURSORE
DECLARE @VAR_SEDE VARCHAR (255)



--CURSORE SEDI DISPONIBILI
DECLARE C_VAR_SEDE CURSOR LOCAL FOR

-- selezione di tutte le sedi disponibili
SELECT DISTINCT (VAR_SEDE) FROM PROFILE WHERE VAR_SEDE IS NOT NULL AND VAR_SEDE <> ''

OPEN C_VAR_SEDE

FETCH next from C_VAR_SEDE into @VAR_SEDE
while(@@fetch_status=0) --( 1 CICLO ) -- PER OGNI SEDE DISPONIBILE
BEGIN
--SETTING DELLE VARIABILI
SET @totDoc = 0
SET @totGrigi = 0
SET @totProt = 0
SET @totProtA = 0
SET @totProtP = 0
SET @totProtI = 0
SET @totProtClear = 0

set @PercGrigi = 0
set @PercProt = 0
set @PercProtA = 0
set @PercProtP = 0
set @PercProtI = 0
set @PercProtClear = 0

set @totClass = 0
set @totClassGrigi = 0
set @totClassProt = 0
set @totClassProtA = 0
set @totClassProtP = 0
set @totClassProtI = 0
set @totClassProtClear = 0

set @PercClass = 0
set @PercClassGrigi = 0
set @PercClassProt = 0
set @PercClassProtA = 0
set @PercClassProtP = 0
set @PercClassProtI = 0
set @PercClassProtClear = 0

set @totProf = 0
set @totProfGrigi = 0
set @totProfProt = 0
set @totProfProtA = 0
set @totProfProtP = 0
set @totProfProtI = 0
set @totProfProtClear = 0

set @PercProf = 0
set @PercProfGrigi = 0
set @PercProfProt = 0
set @PercProfProtA = 0
set @PercProfProtP = 0
set @PercProfProtI = 0
set @PercProfProtClear = 0
--END SETTING


-- conto i doc grigi della sede passata dal cursore (flitro @var_sede ,@anno, @id_registro)
select @totGrigi = count(profile.system_id) from profile,people
where profile.author = people.system_id and people.id_amm = @id_amm and num_proto is null AND cha_tipo_proto = 'G'
AND YEAR(CREATION_DATE) = @anno and (profile.var_sede = @VAR_SEDE);

--  conto i doc protocollati (Annullati) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro)
select @totProtClear = count(system_id) from profile where cha_da_proto = '0' and  id_registro = @id_registro
AND  dta_annulla is not null AND num_proto is not null AND NUM_ANNO_PROTO = @anno AND ( profile.var_sede = @VAR_SEDE);

-- conto i doc Protocollati (Arrivo) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select @totProtA = count(system_id) from profile where profile.cha_da_proto = '0' and profile.id_registro = @id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'A' AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede  = @VAR_SEDE);

-- conto i doc Protocollati (Partenza) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select @totProtP = count(system_id) from profile where profile.cha_da_proto = '0' and profile.id_registro = @id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'P' AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede  = @VAR_SEDE);

-- conto i doc Protocollati (Interni) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select @totProtI = count(system_id) from profile where profile.cha_da_proto = '0' and profile.id_registro = @id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'I' AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede  = @VAR_SEDE);

set @totProt = @totProtA + @totProtP + @totProtI + @totProtClear
set @totDoc = @totGrigi + @totProt;

----CALCOLO PERCENTUALI  --
if ((@totDoc <> 0) and (@totGrigi <> 0 )) -- % doc grigi
begin
set @PercGrigi = ROUND(((@totGrigi / @totDoc) * 100),2)
end
if ((@totDoc <> 0) and (@totProt <> 0 )) -- % doc protocollati
begin
set @PercProt = ROUND(((@totProt / @totDoc) * 100),2)
end
if ((@totProtClear <> 0) and (@totProt <> 0 )) -- % doc protocollati Annullati
begin
set @PercProtClear = ROUND(((@totProtClear / @totProt) * 100),2)
end
if ((@totProtA <> 0) and (@totProt <> 0 )) -- % doc protocollati Arrivo
begin
set @PercProtA = ROUND(((@totProtA / @totProt) * 100),2)
end
if ((@totProtP <> 0) and (@totProt <> 0 )) -- % doc protocollati Partenza
begin
set @PercProtP = ROUND(((@totProtP / @totProt) * 100),2)
end
if ((@totProtI <> 0) and (@totProt <> 0 )) -- % doc protocollati Interni
begin
set @PercProtI = ROUND(((@totProtI / @totProt) * 100),2)
end
--Inserisco nella tabella temporanea i risultati di protocollo prodotti
insert into #REPORT_ANNUALE_X_SEDE
(ANNO, SEDE, TOT_DOC,GRIGI, PERC_GRIGI,PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
('Creati', @VAR_SEDE, @totDoc, @totGrigi, @PercGrigi, @totProt, @PercProt,@totProtClear, @PercProtClear, @totProtA, @PercProtA, @totProtP, @PercProtP, @totProtI, @PercProtI )

-- conto il totale dei documenti classificati (flitro @var_sede ,@anno,@id_registro,tipo)
-- conto i doc grigi classificati
select @totClassGrigi = count(profile.system_id) from profile,people
where profile.author = people.system_id and people.id_amm = @id_amm
AND profile.cha_fascicolato = '1' and profile.num_proto is null AND profile.cha_tipo_proto = 'G'
AND YEAR(CREATION_DATE) = @anno AND (profile.var_sede = @VAR_SEDE)

--conto i doc classificati e protocollati A
select @totClassProtA = count(system_id) from profile where profile.cha_da_proto = '0'
and  profile.id_registro = @id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'A' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede = @VAR_SEDE)

--conto i doc classificati e protocollati P
select @totClassProtP = count(profile.system_id) from profile where profile.cha_da_proto = '0'
and  profile.id_registro = @id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'P' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede = @VAR_SEDE)

--conto i doc classificati e protocollati I
select @totClassProtI = count(profile.system_id) from profile where profile.cha_da_proto = '0'
and  profile.id_registro = @id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'I' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede = @VAR_SEDE)

--conto i doc classificati e annullati
select @totClassProtClear = count (profile.system_id) from profile where profile.cha_da_proto = '0'
and  profile.id_registro = @id_registro AND profile.cha_fascicolato = '1'
AND  profile.dta_annulla is not null AND profile.num_proto is not null
AND profile.NUM_ANNO_PROTO = @anno AND (profile.var_sede = @VAR_SEDE)

set @totClassProt = @totClassProtA + @totClassProtP + @totClassProtI + @totClassProtClear
Set @totClass = @totClassGrigi + @totClassProt


----CALCOLO PERCENTUALI  --
if ((@totDoc <> 0) and (@totClass <> 0 )) -- % doc classificati
begin
set @PercClass = ROUND(((@totClass / @totDoc) * 100),2)
end
if ((@totClassGrigi <> 0) and (@totClass <> 0 )) -- % doc grigi e classificati
begin
set @PercClassGrigi = ROUND(((@totClassGrigi / @totClass) * 100),2)
end
if ((@totClassProt <> 0) and (@totClass <> 0 )) -- % doc protocollati e classificati
begin
set @PercClassProt = ROUND(((@totClassProt / @totClass) * 100),2)
end
if ((@totClassProtClear <> 0) and (@totClassProt <> 0 )) -- % doc protocollati classificati ed annullati
begin
set @PercClassProtClear  = ROUND(((@totClassProtClear / @totClassProt) * 100),2)
end
if ((@totClassProtA <> 0) and (@totClassProt <> 0 )) -- % doc protocollati Arrivo classificiati
begin
set @PercClassProtA = ROUND(((@totClassProtA / @totClassProt) * 100),2)
end
if ((@totClassProtP <> 0) and (@totClassProt <> 0 )) -- % doc protocollati Partenza e classificati
begin
set @PercClassProtP = ROUND(((@totClassProtP / @totClassProt) * 100),2)
end
if ((@totClassProtI <> 0) and (@totClassProt <> 0 )) -- % doc protocollati Partenza e classificati
begin
set @PercClassProtI = ROUND(((@totClassProtI / @totClassProt) * 100),2)
end
--Inserisco nella tabella temporanea i risultati di classifica prodotti
insert into #REPORT_ANNUALE_X_SEDE
(ANNO, SEDE, TOT_DOC,GRIGI, PERC_GRIGI,PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
('Classificati', @VAR_SEDE, @TotClass, @totClassGrigi, @PercClassGrigi, @totClassProt, @PercClassProt,@totClassProtClear, @PercClassProtClear, @totClassProtA, @PercClassProtA, @totClassProtP, @PercClassProtP, @totClassProtI, @PercClassProtI )

select @totProfGrigi = count(profile.system_id) from profile,people
where profile.author = people.system_id and people.id_amm = @id_amm
and profile.cha_img = '0' and YEAR(CREATION_DATE) = @anno and profile.cha_tipo_proto = 'G' AND profile.var_sede = @VAR_SEDE

-- conto i doc protocollati  A senza doc acquisiti
select @totProfProtA = count(p.system_id) from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = @id_registro AND p.NUM_ANNO_PROTO = @anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'A' AND p.var_sede = @VAR_SEDE

-- conto i doc protocollati P senza doc acquisiti
select @totProfProtP = count(p.system_id) from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = @id_registro AND p.NUM_ANNO_PROTO = @anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'P' AND p.var_sede = @VAR_SEDE

-- conto i doc protocollati  I senza doc acquisiti
select @totProfProtI = count(p.system_id) from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = @id_registro AND p.NUM_ANNO_PROTO = @anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'I' AND p.var_sede = @VAR_SEDE

-- conto i doc protocollati  Annullati senza doc acquisiti
select @totProfProtClear = count(profile.system_id) from profile where profile.cha_da_proto = '0' and  profile.id_registro = @id_registro
AND profile.dta_annulla is not null AND profile.num_proto is not null and profile.cha_img = '0' and profile.NUM_ANNO_PROTO = @anno AND profile.var_sede = @VAR_SEDE

set @totProfProt = @totProfProtA + @totProfProtP + @totProfProtI + @totProfProtClear
set @totProf = @totProfGrigi + @totProfProt

-- calcolo percentuali --
if ((@totDoc <> 0) and (@totProf <> 0 )) -- %  profili
begin
set @PercProf = ROUND(((@totProf / @totDoc) * 100),2)
end
if ((@totProfGrigi <> 0) and (@totProf <> 0 )) -- % profili doc grigi
begin
set @PercProfGrigi = ROUND(((@totProfGrigi / @totProf) * 100),2)
end
if ((@totProfProt <> 0) and (@totProf <> 0 )) -- % profili protocollati
begin
set @PercProfProt = ROUND(((@totProfProt / @totProf) * 100),2)
end
if ((@totProfProtClear <> 0) and (@totProfProt <> 0 )) -- % profili protocollati ed annullati
begin
set @PercProfProtClear  = ROUND(((@totProfProtClear / @totProfProt) * 100),2)
end
if ((@totProfProtA <> 0) and (@totProfProt <> 0 )) -- % profili protocollati A
begin
set @PercProfProtA = ROUND(((@totProfProtA / @totProfProt) * 100),2)
end
if ((@totProfProtP <> 0) and (@totProfProt <> 0 )) -- % profili protocollati P
begin
set @PercProfProtP = ROUND(((@totProfProtP / @totProfProt) * 100),2)
end
if ((@totProfProtI <> 0) and (@totProfProt <> 0 )) -- % profili protocollati I
begin
set @PercProfProtI = ROUND(((@totProfProtI / @totProfProt) * 100),2)
end
--Inserisco nella tabella temporanea i risultati di profilazione prodotti
insert into #REPORT_ANNUALE_X_SEDE
(ANNO, SEDE, TOT_DOC,GRIGI, PERC_GRIGI,PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
('Senza Img.', @VAR_SEDE, @totProf, @totProfGrigi, @PercProfGrigi, @totProfProt, @PercProfProt,@totProfProtClear, @PercProfProtClear, @totProfProtA, @PercProfProtA, @totProfProtP, @PercProfProtP, @totProfProtI, @PercProfProtI )
-- RESET DELLE VARIABILI

FETCH next from C_VAR_SEDE into @VAR_SEDE

END    --( END 1 CICLO ) --


deallocate C_VAR_SEDE
-- return dei risultati
SELECT * FROM #REPORT_ANNUALE_X_SEDE




GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO