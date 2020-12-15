

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[REPORT_ANNUALE_DOC_X_UO]
--parametri di input
@anno int,
@id_registro int,
@id_amm int

AS
--tabella temporanea
CREATE TABLE [@db_user].[#REPORT_ANNUALE_X_UO]
(
[UO] [varchar] (250),
[TOT_PROT] [varchar] (250),
[ARRIVO] [varchar] (250),
[PERC_ARRIVO] [varchar] (250),
[PARTENZA] [varchar] (250),
[PERC_PARTENZA] [varchar] (250),
[INTERNI] [varchar] (250),
[PERC_INTERNI] [varchar] (250),
[ANNULL] [varchar] (250),
[PERC_ANNULL] [varchar] (250),
[PROFILI] [varchar] (250),
[PERC_PROFILI] [varchar] (250),
[CLASSIFICATI] [varchar] (250),
[PERC_CLASSIFICATI] [varchar] (250),
) ON [PRIMARY]
-- VARIABILI TEMPORANEE
DECLARE @ProtA FLOAT
DECLARE @ProtP FLOAT
DECLARE @ProtI FLOAT
DECLARE @ProtAndAnn FLOAT
DECLARE @ProtNotImg FLOAT
DECLARE @ProtAndClass FLOAT
DECLARE @PercProtA float
DECLARE @PercProtP float
DECLARE @PercProtI float
DECLARE @PercProtAndAnn float
DECLARE @PercProtNotImg float
DECLARE @PercProtAndClass float
--variabili temporanee totali
DECLARE @TOTPROT FLOAT
DECLARE @TOTPROTA FLOAT
DECLARE @TOTPROTP FLOAT
DECLARE @TOTPROTI FLOAT
DECLARE @TOTPROTANN FLOAT
DECLARE @TOTPROTPROF FLOAT
DECLARE @TOTPROTCLASS FLOAT
DECLARE @PERCTOTPROTA FLOAT
DECLARE @PERCTOTPROTP FLOAT
DECLARE @PERCTOTPROTI FLOAT
DECLARE @PERCTOTPROTANN FLOAT
DECLARE @PERCTOTPROTPROF FLOAT
DECLARE @PERCTOTPROTCLASS FLOAT
-- SET VARIABILI TEMPORANEE TOTALI
SET @TOTPROT = 0
SET @TOTPROTA = 0
SET @TOTPROTP = 0
SET @TOTPROTI = 0
SET @TOTPROTANN = 0
SET @TOTPROTPROF = 0
SET @TOTPROTCLASS = 0
SET @PERCTOTPROTA = 0
SET @PERCTOTPROTP = 0
SET @PERCTOTPROTI = 0
SET @PERCTOTPROTANN = 0
SET @PERCTOTPROTPROF = 0
SET @PERCTOTPROTCLASS = 0


--VARIABILI CURSORE
DECLARE @TOT_PROT_UO FLOAT
DECLARE @ID_UO FLOAT
DECLARE @VAR_UO VARCHAR (250)
-- definizione cursore
DECLARE C_UO CURSOR LOCAL FOR
select count(*) as TotProtUO,
dpa_corr_globali.system_id,dpa_corr_globali.var_desc_corr
from
profile,dpa_corr_globali
where
profile.id_registro = @id_registro
and
dpa_corr_globali.id_amm = @id_amm
and
profile.num_anno_proto = @anno
and
profile.id_uo_prot = dpa_corr_globali.system_id
and profile.cha_da_proto = '0'
group by dpa_corr_globali.var_desc_corr,dpa_corr_globali.system_id
OPEN C_UO
FETCH NEXT FROM C_UO into @TOT_PROT_UO,@ID_UO,@VAR_UO
while(@@fetch_status=0) --( 1 CICLO )
BEGIN -- PER OGNI UO APPARTENENTE AL REGISTRO ED ALL AMMINISTAZIONE DEI PARAM DI INPUT
--setting variabili temporanee
set @protA = 0
set @ProtP = 0
set @ProtI = 0
set @ProtAndAnn = 0
set @ProtNotImg = 0
set @ProtAndClass = 0
set @PercProtA = 0
set @PercProtP = 0
set @PercProtI = 0
set @PercProtAndAnn = 0
set @PercProtNotImg = 0
set @PercProtAndClass = 0
--conta PROTO A per singola uo
select @ProtA = count(profile.system_id)
from profile
where profile.cha_da_proto = '0'
and profile.id_registro = @id_registro
and profile.num_anno_proto = @anno
AND profile.cha_tipo_proto = 'A'
and profile.id_uo_prot = @ID_UO
and profile.dta_annulla is null
--conta PROTO P per singola uo
select @ProtP = count(profile.system_id)
from profile
where profile.cha_da_proto = '0'
and profile.id_registro = @id_registro
and profile.num_anno_proto = @anno
AND profile.cha_tipo_proto = 'P'
and profile.id_uo_prot = @ID_UO
and profile.dta_annulla is null
--conta PROTO I per singola uo
select @ProtI = count(profile.system_id)
from profile
where profile.cha_da_proto = '0'
and profile.id_registro = @id_registro
and profile.num_anno_proto = @anno
AND profile.cha_tipo_proto = 'I'
and profile.id_uo_prot = @ID_UO
and profile.dta_annulla is null
--conta i DOC Protocollati ed annullati per singola uo
select @ProtAndAnn = count(profile.system_id)
from profile
where profile.cha_da_proto = '0'
and profile.id_registro = @id_registro
and profile.num_anno_proto = @anno
AND profile.num_proto is not null
and profile.dta_annulla is not null
and profile.id_uo_prot = @ID_UO
--conta I DOC protocollati SENZA IMG per singola uo
select @ProtNotImg = count(profile.system_id)
from profile
where profile.cha_img = '0'
and profile.cha_da_proto = '0'
and profile.id_registro = @id_registro
and profile.num_anno_proto = @anno
--and profile.dta_annulla is null
and profile.id_uo_prot = @ID_UO
--conta I DOC protocollati e classificati per singola uo
select @ProtAndClass = count(profile.system_id)
from profile
where
docnumber in (select project_components.link from project_components)
and profile.id_registro = @id_registro
AND profile.num_proto is not null
and profile.num_anno_proto = @anno
and profile.id_uo_prot = @ID_UO
--CALCOLO LE PERCENTUALI
IF (@TOT_PROT_UO <> 0)
BEGIN
IF (@ProtA <> 0)
BEGIN --% di protocolli A sul totale dei protocolli
SET @PercProtA = ROUND(((@ProtA / @TOT_PROT_UO) * 100),2)
END
IF (@ProtP <> 0)
BEGIN --% di protocolli P sul totale dei protocolli
SET @PercProtP = ROUND(((@ProtP / @TOT_PROT_UO) * 100),2)
END
IF (@ProtI <> 0)
BEGIN --% di protocolli I sul totale dei protocolli
SET @PercProtI = ROUND(((@ProtI / @TOT_PROT_UO) * 100),2)
END
IF (@ProtAndAnn <> 0)
BEGIN --% di protocolli Annullati sul totale dei protocolli
SET @PercProtAndAnn = ROUND(((@ProtAndAnn / @TOT_PROT_UO) * 100),2)
END
IF (@ProtNotImg <> 0)
BEGIN --% di protocolli senza Immagine sul totale dei protocolli
SET @PercProtNotImg = ROUND(((@ProtNotImg / @TOT_PROT_UO) * 100),2)
END
IF (@ProtAndClass <> 0)
BEGIN --% di protocolli senza Immagine sul totale dei protocolli
SET @PercProtAndClass = ROUND(((@ProtAndClass / @TOT_PROT_UO) * 100),2)
END
END
-- INSERISCO NELLA TAB TEMPORANEA
insert into #REPORT_ANNUALE_X_UO
(UO,TOT_PROT,ARRIVO,PERC_ARRIVO,PARTENZA,PERC_PARTENZA,INTERNI,PERC_INTERNI,ANNULL,PERC_ANNULL,PROFILI,PERC_PROFILI,CLASSIFICATI,PERC_CLASSIFICATI)
values
(@VAR_UO,@TOT_PROT_UO,@ProtA,@PercProtA,@ProtP,@PercProtP,@ProtI,@PercProtI,@ProtAndAnn,@PercProtAndAnn,@ProtNotImg,@PercProtNotImg,@ProtAndClass,@PercProtAndClass)
--aggiorno variabili totali
set @TOTPROT = @TOTPROT + @TOT_PROT_UO
set @TOTPROTA = @TOTPROTA + @ProtA
set @TOTPROTP = @TOTPROTP + @ProtP
set @TOTPROTI = @TOTPROTI + @ProtI
set @TOTPROTANN = @TOTPROTANN + @ProtAndAnn
set @TOTPROTPROF = @TOTPROTPROF + @ProtNotImg
set @TOTPROTCLASS = @TOTPROTCLASS + @ProtAndClass


FETCH next from C_UO into @TOT_PROT_UO,@ID_UO,@VAR_UO

END    --( END 1 CICLO ) --
--verifico e calcolo percentuali totali
IF (@TOTPROT <> 0)
BEGIN
IF (@TOTPROTA <> 0)
BEGIN --% di protocolli A sul totale dei protocolli
SET @PERCTOTPROTA = ROUND(((@TOTPROTA / @TOTPROT) * 100),2)
END
IF (@TOTPROTP <> 0)
BEGIN --% di protocolli P sul totale dei protocolli
SET @PERCTOTPROTP = ROUND(((@TOTPROTP / @TOTPROT) * 100),2)
END
IF (@TOTPROTI <> 0)
BEGIN --% di protocolli I sul totale dei protocolli
SET @PERCTOTPROTI = ROUND(((@TOTPROTI / @TOTPROT) * 100),2)
END
IF (@TOTPROTANN <> 0)
BEGIN --% di protocolli ANNULLATI sul totale dei protocolli
SET @PERCTOTPROTANN = ROUND(((@TOTPROTANN / @TOTPROT) * 100),2)
END
IF (@TOTPROTPROF <> 0)
BEGIN --% di protocolli senza Immagine sul totale dei protocolli
SET @PERCTOTPROTPROF = ROUND(((@TOTPROTPROF / @TOTPROT) * 100),2)
END
IF (@TOTPROTCLASS <> 0)
BEGIN --% di protocolli senza Immagine sul totale dei protocolli
SET @PERCTOTPROTCLASS = ROUND(((@TOTPROTCLASS / @TOTPROT) * 100),2)
END
END
-- inserisco i valori totali come ultima riga di report
insert into #REPORT_ANNUALE_X_UO
(UO,TOT_PROT,ARRIVO,PERC_ARRIVO,PARTENZA,PERC_PARTENZA,INTERNI,PERC_INTERNI,ANNULL,PERC_ANNULL,PROFILI,PERC_PROFILI,CLASSIFICATI,PERC_CLASSIFICATI)
values
('TOTALE',@TOTPROT,@TOTPROTA,@PERCTOTPROTA,@TOTPROTP,@PERCTOTPROTP,@TOTPROTI,@PERCTOTPROTI,@TOTPROTANN,@PERCTOTPROTANN,@TOTPROTPROF,@PERCTOTPROTPROF,@TOTPROTCLASS,@PERCTOTPROTCLASS)

DEALLOCATE C_UO
-- return dei risultati
SELECT * FROM #REPORT_ANNUALE_X_UO

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


