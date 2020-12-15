/******************************************************************************
AUTHOR:     Claudio Rinaldi

NAME:       SP_INSERT_PROJECT_TEMPLATE

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedure inserisce un nodo della struttura template e torna l'id

******************************************************************************/
CREATE PROCEDURE SP_INSERT_STRUCTURE_NODE
( 
    @NAME VARCHAR(255),
    @PARENT_ID INTEGER,
    @TEMPLATE_ID INTEGER,
    @ID INTEGER OUTPUT
)
AS
BEGIN
    INSERT INTO PROJECT_STRUCTURE(ID_PARENT,NAME,ID_TEMPLATE)
    VALUES (@PARENT_ID,@NAME,@TEMPLATE_ID)
    
    SET @ID = SCOPE_IDENTITY() 
END
GO