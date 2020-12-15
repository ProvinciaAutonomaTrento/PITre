CREATE OR REPLACE FUNCTION @db_user.CHECKPROFILEDATA 
(numProto number, dataProt date) RETURN NUMBER IS
tmpVar NUMBER;
/******************************************************************************
   NAME:       NuovaFunction
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        10/07/2012   Abbatangeli       1. Created this function.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     NuovaFunction
      Sysdate:         10/07/2012
      Date and Time:   10/07/2012, 12:27:18, and 10/07/2012 12:27:18
      Username:        Abbatangeli (set in TOAD Options, Procedure Editor)
      Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/

 minProto NUMBER;
 maxProto NUMBER;
 
BEGIN
   minProto:=0;
   maxProto:=0;
   tmpVar := 0;
   
   SELECT A.minimo, B.massimo into minProto,maxProto FROM
(select nvl(max(id_vecchio_documento),0) as minimo from profile where TRUNC(creation_date,'DDD') < TRUNC(creation_date,'DDD')) A,
(select nvl(min(id_vecchio_documento),999999999) as massimo from profile where TRUNC(creation_date,'DDD') > TRUNC(creation_date,'DDD')) B;

if (numProto < minProto or numProto > maxProto) then tmpVar :=numProto; else tmpVar :=0; end if;

   RETURN tmpVar;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CHECKPROFILEDATA;
/