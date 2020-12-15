/******************************************************************************
AUTHOR:     Claudio Rinaldi

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedura recupera le relazioni del fascicolo/titolario/template
            passato come parametro.

******************************************************************************/
CREATE PROCEDURE SP_INS_REL_PROJECT_TEMPLATE
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
        INSERT INTO REL_PROJECT_STRUCTURE (ID_TEMPLATE,ID_TIPO_FASCICOLO) VALUES (@ID_TEMPLATE,@ID_FASCICOLO)
    END
    ELSE IF(@ID_TITOLARIO <> '')
    BEGIN
        INSERT INTO REL_PROJECT_STRUCTURE (ID_TEMPLATE,ID_TITOLARIO) VALUES (@ID_TEMPLATE,@ID_TITOLARIO)
    END
END
GO
