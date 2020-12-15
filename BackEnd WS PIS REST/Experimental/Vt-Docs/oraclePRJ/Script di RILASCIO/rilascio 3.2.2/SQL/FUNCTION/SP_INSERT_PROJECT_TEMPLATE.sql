/******************************************************************************
AUTHOR:     Claudio Rinaldi

NAME:       SP_INSERT_PROJECT_TEMPLATE

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedure inserisce un template di fascicolo e torna l'id

******************************************************************************/
CREATE PROCEDURE SP_INSERT_PROJECT_TEMPLATE
( 
    @NAME VARCHAR(255),
    @ID INTEGER OUTPUT
)
AS
BEGIN
    INSERT INTO PROJECT_TEMPLATE (NAME) VALUES (@NAME)
    
    SET @ID = SCOPE_IDENTITY() 
END
GO