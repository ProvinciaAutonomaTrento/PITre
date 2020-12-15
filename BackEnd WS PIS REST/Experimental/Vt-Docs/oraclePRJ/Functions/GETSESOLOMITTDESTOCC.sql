--------------------------------------------------------
--  DDL for Function GETSESOLOMITTDESTOCC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSESOLOMITTDESTOCC" (docId INT)
RETURN int IS risultato int;
BEGIN
SELECT count(dap.SYSTEM_ID) into risultato
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id and c.cha_tipo_ie in ('I','E')
;
if(risultato>0)
then risultato :=0;
else risultato:=1;
end if;
RETURN risultato;
END getSeSoloMittDestOcc; 

/
