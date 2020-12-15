--------------------------------------------------------
--  DDL for Function GETSEDOCTRASMCONRAG
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSEDOCTRASMCONRAG" (idprofile number,vardescragione varchar)  RETURN NUMBER IS
risultato NUMBER;
begin
BEGIN
if(upper(varDescRagione)!='TUTTE')
then

SELECT COUNT (tx.system_id) into risultato
FROM dpa_trasmissione tx, dpa_trasm_singola ts, dpa_ragione_trasm tr
WHERE tx.system_id = ts.id_trasmissione
AND ts.id_ragione = tr.system_id
AND UPPER (tr.var_desc_ragione) = UPPER (varDescRagione)
AND tx.id_profile = idprofile;
else
/* se  stato trasmesso almeno una volta */
SELECT COUNT (tx.system_id) into risultato
FROM dpa_trasmissione tx
WHERE tx.id_profile = idprofile;

end if;

if(risultato>0)
then
risultato:=1;
END if;

EXCEPTION
WHEN others THEN
risultato:=0;

end;
return risultato ;
END GetSeDocTrasmConRag ; 

/
