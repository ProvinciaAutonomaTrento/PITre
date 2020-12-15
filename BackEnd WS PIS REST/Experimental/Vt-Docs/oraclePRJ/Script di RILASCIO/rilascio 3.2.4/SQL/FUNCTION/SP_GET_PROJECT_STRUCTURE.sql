/******************************************************************************
AUTHOR:     Claudio Rinaldi

PURPOSE:    TITOLARIO - STRUTTURA SOTTOFASCICOLI
            La procedura recupera la struttura dei sottosfascicoli associati
            con un nodo titolario e con una tipologia dei fascicoli.
            Se esiste una struttura associata con una tipologia vince rispetto
            ad una struttura associata al nodo titolario.

******************************************************************************/
CREATE PROCEDURE SP_GET_PROJECT_STRUCTURE
( 
    @ID_FASCICOLO VARCHAR(255),
    @ID_TITOLARIO VARCHAR(255),
    @ID_TEMPLATE  VARCHAR(255),
	@ID_OBJECT VARCHAR(255) OUT
)
AS
    DECLARE @CODICE VARCHAR(255)
BEGIN
    
    IF(@ID_TEMPLATE = '')
    BEGIN
        SET @CODICE = (SELECT ID_TEMPLATE FROM REL_PROJECT_STRUCTURE WHERE ID_TIPO_FASCICOLO = @ID_FASCICOLO)
        IF(@CODICE IS NULL OR LEN(@CODICE) = 0)
             SET @CODICE = (SELECT ID_TEMPLATE FROM REL_PROJECT_STRUCTURE WHERE ID_TITOLARIO = @ID_TITOLARIO)
		
		SET @ID_OBJECT = @CODICE
    END
    ELSE SET @CODICE = @ID_TEMPLATE

    ;WITH NAME_TREE AS (
       SELECT SYSTEM_ID, ID_PARENT, NAME
       FROM PROJECT_STRUCTURE
       WHERE ID_TEMPLATE = @CODICE AND ID_PARENT IS NULL
       UNION ALL
       SELECT C.SYSTEM_ID, C.ID_PARENT, C.NAME
       FROM PROJECT_STRUCTURE C
         JOIN NAME_TREE P ON C.ID_PARENT = p.system_id  -- this is the recursion
    ) 
    SELECT * FROM NAME_TREE
END
GO
