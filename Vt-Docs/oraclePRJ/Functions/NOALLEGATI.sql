--------------------------------------------------------
--  DDL for Function NOALLEGATI
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."NOALLEGATI" (docId INT)
RETURN int IS risultato int;
BEGIN
select count(system_id) into risultato from profile p where p.ID_DOCUMENTO_PRINCIPALE =docId;
if(risultato>0)
then risultato:=1;
end if;
RETURN risultato;
END noAllegati; 

/
