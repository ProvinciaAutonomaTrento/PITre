CREATE OR REPLACE PROCEDURE SP_INSERT_PROJECT_TEMPLATE
( 
    NAME IN VARCHAR,
    ID OUT INTEGER,
    returnvalue OUT INTEGER
)
IS
/******************************************************************************
   NAME:       SP_INS_REL_PROJECT_TEMPLATE
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        01/02/2017   crinaldi       1. Created this procedure.

   NOTES:
   
******************************************************************************/
    key_value INTEGER := 0;
BEGIN

    key_value := SEQ_PROJECT_TEMPLATE.NEXTVAL;

    INSERT INTO PROJECT_TEMPLATE (SYSTEM_ID, NAME) 
    VALUES (key_value, NAME) ;
    
    ID := key_value;
    returnvalue := 0;
    
    EXCEPTION
    WHEN OTHERS THEN
        returnvalue := -1;
END;