CREATE OR REPLACE PROCEDURE SP_GET_REL_PROJECT_TEMPLATE
( 
    p_ID_FASCICOLO IN VARCHAR2,
    p_ID_TITOLARIO IN VARCHAR2,
    p_ID_TEMPLATE  IN VARCHAR2,
    p_res_cursor OUT SYS_REFCURSOR
)
IS
    /******************************************************************************
   NAME:       SP_GET_REL_PROJECT_TEMPLATE
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        01/02/2017   crinaldi       1. Created this procedure.

   NOTES:
   
******************************************************************************/
BEGIN
    
    IF(p_ID_FASCICOLO IS NOT NULL) THEN

        OPEN p_res_cursor FOR
        SELECT SYSTEM_ID, ID_TIPO_FASCICOLO, ID_TITOLARIO, ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TIPO_FASCICOLO = p_ID_FASCICOLO;

    ELSIF(p_ID_TITOLARIO IS NOT NULL) THEN

        OPEN p_res_cursor FOR
        SELECT SYSTEM_ID, ID_TIPO_FASCICOLO, ID_TITOLARIO, ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TITOLARIO = p_ID_TITOLARIO;

    ELSIF(p_ID_TEMPLATE IS NOT NULL) THEN
    
        OPEN p_res_cursor FOR 
        SELECT SYSTEM_ID, ID_TIPO_FASCICOLO, ID_TITOLARIO, ID_TEMPLATE
        FROM REL_PROJECT_STRUCTURE 
        WHERE ID_TEMPLATE = p_ID_TEMPLATE;
        
    END IF;
    
END;
