--------------------------------------------------------
--  DDL for Function OGG_SEARCH
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."OGG_SEARCH" ( txt varchar,sysid number ) RETURN NUMBER IS
tmpVar NUMBER;
/******************************************************************************
   NAME:       ogg_search
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        04/01/2012   LucianiLu       1. Created this function.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     ogg_search
      Sysdate:         04/01/2012
      Date and Time:   04/01/2012, 15:39:05, and 04/01/2012 15:39:05
      Username:        LucianiLu (set in TOAD Options, Procedure Editor)
      Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/
BEGIN
   tmpVar := 0;
   
   select count(*)  into tmpvar from profile A where system_id=sysid and  catsearch (A.VAR_PROF_OGGETTO, txt||'*', '')>0;
   
   RETURN tmpVar;
   
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END ogg_search;

/
