/******************************************************************************
AUTHOR:     Claudio Rinaldi

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedura recupera le relazioni del fascicolo/titolario/template
            passato come parametro.

******************************************************************************/
CREATE PROCEDURE SP_UPD_REL_PROJECT_TEMPLATE
( 
    @ID_FASCICOLO VARCHAR(255),
    @ID_TITOLARIO VARCHAR(255),
    @ID_TEMPLATE  VARCHAR(255)
)
AS
BEGIN
    
    IF(@ID_TEMPLATE = '')
    BEGIN 
        IF(@ID_FASCICOLO <> '')
        BEGIN
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TIPO_FASCICOLO = @ID_FASCICOLO
        END
        ELSE
        BEGIN
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TITOLARIO = @ID_TITOLARIO
        END
    END
    ELSE IF(@ID_FASCICOLO <> '')
    BEGIN
        UPDATE REL_PROJECT_STRUCTURE SET ID_TEMPLATE = @ID_TEMPLATE WHERE ID_TIPO_FASCICOLO = @ID_FASCICOLO
    END
    ELSE IF(@ID_TITOLARIO <> '')
    BEGIN
        UPDATE REL_PROJECT_STRUCTURE SET ID_TEMPLATE = @ID_TEMPLATE WHERE ID_TITOLARIO = @ID_TITOLARIO
    END
END
GO
