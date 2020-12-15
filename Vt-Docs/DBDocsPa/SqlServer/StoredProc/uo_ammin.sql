SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE @db_user.uo_ammin  @id_amm int as
-- dichiarazione variabili
DECLARE @num_livello int
DECLARE @system_id varchar(32)
DECLARE @var_desc_amm varchar(32)
DECLARE @var_codice_amm varchar(32)
DECLARE @identity int
-- fine dichiarazione
BEGIN
DECLARE curr2 CURSOR FOR
SELECT DISTINCT num_livello
FROM @db_user.DPA_CORR_GLOBALI
WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = @id_amm
ORDER BY NUM_LIVELLO DESC
OPEN curr2

FETCH NEXT FROM curr2 INTO @num_livello
-- finch?i sono record da prelevare
WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE @db_user.DPA_CORR_GLOBALI
SET NUM_LIVELLO = @num_livello+1
WHERE NUM_LIVELLO = @num_livello
AND cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = @id_amm

FETCH NEXT FROM curr2 INTO @num_livello
END

CLOSE curr2
DEALLOCATE curr2
SELECT  @system_id=SYSTEM_ID,
@var_desc_amm=VAR_DESC_AMM,
@var_codice_amm=VAR_CODICE_AMM
FROM @db_user.DPA_AMMINISTRA WHERE SYSTEM_ID = @id_amm

SELECT SYSTEM_ID
FROM @db_user.DPA_CORR_GLOBALI WHERE VAR_COD_RUBRICA = @var_codice_amm
IF @@ROWCOUNT = 0

BEGIN
SELECT SYSTEM_ID
FROM @db_user.DPA_CORR_GLOBALI WHERE VAR_CODICE = @var_codice_amm
IF @@ROWCOUNT = 0
BEGIN
--INSERISCO IL CORRISPONDENTE
INSERT INTO @db_user.DPA_CORR_GLOBALI
(

ID_AMM,
VAR_COD_RUBRICA,
VAR_DESC_CORR,
ID_PARENT,
NUM_LIVELLO,
VAR_CODICE,
CHA_TIPO_IE,
CHA_TIPO_CORR,
CHA_TIPO_URP
)
VALUES (
@id_amm,
@var_codice_amm,
@var_desc_amm,
0,
0,
@var_codice_amm,
'I',
'S',
'U'
)

SET @identity = SCOPE_IDENTITY()

UPDATE @db_user.DPA_CORR_GLOBALI
SET ID_PARENT = @identity
WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = @id_amm
AND NUM_LIVELLO = 1
END
ELSE
print 'ATTENZIONE: VAR_CODICE GI?RESENTE - LA PROCEDURA TERMINA'

END
ELSE
print 'ATTENZIONE: CODICE RUBRICA GI?RESENTE - LA PROCEDURA TERMINA'

END



GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO