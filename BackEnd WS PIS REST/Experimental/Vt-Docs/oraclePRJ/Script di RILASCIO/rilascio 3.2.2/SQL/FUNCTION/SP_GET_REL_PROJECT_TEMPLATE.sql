/******************************************************************************
AUTHOR:     Claudio Rinaldi

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedura recupera le relazioni del fascicolo/titolario/template
            passato come parametro.

******************************************************************************/
CREATE PROCEDURE SP_GET_REL_PROJECT_TEMPLATE
( 
    @ID_FASCICOLO VARCHAR(255),
    @ID_TITOLARIO VARCHAR(255),
    @ID_TEMPLATE  VARCHAR(255)
)
AS
    DECLARE @CODICE VARCHAR(255)
BEGIN
    
    IF(@ID_FASCICOLO <> '')
    BEGIN
        SELECT SYSTEM_ID,ID_TIPO_FASCICOLO,ID_TITOLARIO,ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TIPO_FASCICOLO = @ID_FASCICOLO
    END
    ELSE IF(@ID_TITOLARIO <> '')
    BEGIN
        SELECT SYSTEM_ID,ID_TIPO_FASCICOLO,ID_TITOLARIO,ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TITOLARIO = @ID_TITOLARIO
    END
    ELSE IF(@ID_TEMPLATE <> '')
    BEGIN 
        SELECT SYSTEM_ID,ID_TIPO_FASCICOLO,ID_TITOLARIO,ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TEMPLATE = @ID_TEMPLATE
    END
END
GO
