SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[Sp_Dpa_Uo_Reg]
@idUO int
AS


DECLARE @ReturnValue int
DECLARE @record_corr_ruolo int
DECLARE @record_corr_registro int
DECLARE @rec int


BEGIN
DELETE FROM DPA_UO_REG WHERE id_uo = @idUO
END

DECLARE cursor_ruoli

CURSOR FOR
SELECT system_id
FROM DPA_CORR_GLOBALI
WHERE
cha_tipo_urp = 'R'
AND cha_tipo_ie = 'I'
AND dta_fine IS NULL
AND id_old = 0
AND id_uo = @idUO

OPEN cursor_ruoli
FETCH NEXT FROM cursor_ruoli
INTO @record_corr_ruolo


WHILE @@FETCH_STATUS = 0
BEGIN

DECLARE cursor_registri

CURSOR FOR
SELECT id_registro
FROM DPA_L_RUOLO_REG A, DPA_EL_REGISTRI B
WHERE  A.ID_REGISTRO = B.SYSTEM_ID AND A.id_ruolo_in_uo = @record_corr_ruolo
AND B.CHA_RF = 0;


OPEN cursor_registri
FETCH NEXT FROM cursor_registri
INTO @record_corr_registro


WHILE @@FETCH_STATUS = 0
BEGIN

SET @rec = (SELECT count(system_id)
FROM DPA_UO_REG
WHERE ID_UO = @idUO
AND ID_REGISTRO = @record_corr_registro)

IF (@rec = 0)
BEGIN
INSERT INTO DPA_UO_REG
(ID_UO, ID_REGISTRO)
VALUES
(@idUO, @record_corr_registro)

IF (@@ROWCOUNT = 0)
BEGIN
SET @ReturnValue=1
RETURN
END
END

-- Posizionamento sul record successivo del cursore cursor_registri
FETCH NEXT FROM cursor_registri
INTO 	@record_corr_registro
END

-- Chiusura e deallocazione cursore cursor_registri
CLOSE cursor_registri
DEALLOCATE cursor_registri

-- Posizionamento sul record successivo del cursore cursor_ruoli
FETCH NEXT FROM cursor_ruoli
INTO 	@record_corr_ruolo
END

-- Chiusura e deallocazione cursore cursor_uo
CLOSE cursor_ruoli
DEALLOCATE cursor_ruoli

RETURN @ReturnValue

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO