CREATE OR REPLACE FUNCTION @db_user.CHECKPROTODATA (numProto number, dataProt date, idRegistro number) RETURN NUMBER IS
tmpVar NUMBER;
/******************************************************************************
   NAME:       NuovaFunction
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        10/07/2012   Abbatangeli       1. Created this function.

   NOTES:
      Sysdate:         10/07/2012
      Username:        Abbatangeli

******************************************************************************/

 minProto NUMBER;
 maxProto NUMBER;

BEGIN
   minProto:=0;
   maxProto:=0;
   tmpVar := 0;

   SELECT A.minimo, B.massimo into minProto,maxProto FROM
(select nvl(max(num_proto),0) as minimo from profile 
        where TRUNC(dta_proto,'DDD') < TRUNC(dataProt,'DDD') and id_registro = idRegistro) A,
(select nvl(min(num_proto),999999999) as massimo from profile 
        where TRUNC(dta_proto,'DDD') > TRUNC(dataProt,'DDD') and id_registro = idRegistro) B;

if (numProto < minProto or numProto > maxProto) then tmpVar :=numProto; else tmpVar :=0; end if;

   RETURN tmpVar;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CHECKPROTODATA;
/

