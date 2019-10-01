


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
CREATE procedure [@db_user].[ins_occ]
(@id_reg int, @idamm int, @Prefix_cod_rub varchar (128),
@desc_corr varchar (128),
@cha_dettagli varchar (1), @RESULT INT OUT)

as

BEGIN
declare @sys_dpa_corr_globali int

set @sys_dpa_corr_globali = 0
if (@sys_dpa_corr_globali <> 0)
BEGIN
SET @RESULT = @sys_dpa_corr_globali

END

else
BEGIN

if(@id_reg=0)
set @id_reg=null

--inserisco il nuovo occ_
INSERT INTO
[@db_user].DPA_CORR_GLOBALI (ID_REGISTRO,
ID_AMM,VAR_DESC_CORR,
ID_OLD,DTA_INIZIO,ID_PARENT
,CHA_TIPO_CORR,CHA_DETTAGLI)
VALUES (@id_reg,@idamm,@desc_corr,0,getdate(),0,'O',0)

SET @RESULT = SCOPE_IDENTITY()

UPDATE [@db_user].DPA_CORR_GLOBALI SET
VAR_COD_RUBRICA=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT) ,
VAR_CODICE=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT)
WHERE SYSTEM_ID=@RESULT

END


return @RESULT

END


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO