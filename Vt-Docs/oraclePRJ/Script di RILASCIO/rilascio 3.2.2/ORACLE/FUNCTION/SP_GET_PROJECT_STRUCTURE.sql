CREATE OR REPLACE PROCEDURE SVILUPPO_PITRE.SP_GET_PROJECT_STRUCTURE
(
    p_ID_FASCICOLO IN VARCHAR2,
    p_ID_TITOLARIO IN VARCHAR2,
    p_ID_TEMPLATE  IN VARCHAR2,
    p_res_cursor OUT SYS_REFCURSOR
)
IS
/******************************************************************************
   NAME:       SP_GET_PROJECT_STRUCTURE
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        01/02/2017   crinaldi       1. Created this procedure.

   NOTES:
   
******************************************************************************/
CODICE VARCHAR2(255) := '';
BEGIN
   
    IF(p_ID_TEMPLATE IS NULL) THEN
        BEGIN
            SELECT ID_TEMPLATE INTO CODICE FROM REL_PROJECT_STRUCTURE WHERE ID_TIPO_FASCICOLO = p_ID_FASCICOLO;
            EXCEPTION
            WHEN NO_DATA_FOUND THEN
                SELECT ID_TEMPLATE INTO CODICE FROM REL_PROJECT_STRUCTURE WHERE ID_TITOLARIO = p_ID_TITOLARIO;
        END;
    ELSE 
        CODICE := p_ID_TEMPLATE;
    END IF;

    OPEN p_res_cursor FOR
        SELECT SYSTEM_ID, ID_PARENT, NAME
        FROM PROJECT_STRUCTURE
        WHERE ID_TEMPLATE = CODICE
        START WITH ID_PARENT IS NULL
        CONNECT BY PRIOR system_id = id_parent;
       
END;
/
