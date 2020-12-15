--------------------------------------------------------
--  DDL for Function CHECKGESTIONEARCHIVIO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CHECKGESTIONEARCHIVIO" 
(
idgroupParam INT
)
RETURN INT IS retValue INT;

cnt INT := 0;

BEGIN
retValue := 0;

select count(*) into cnt
from dpa_corr_globali corr, dpa_tipo_f_ruolo tipof, dpa_funzioni f
where tipof.id_ruolo_in_uo = corr.system_id and f.id_tipo_funzione=tipof.ID_TIPO_FUNZ
and corr.id_gruppo = idgroupParam
and f.COD_FUNZIONE='GEST_ARCHIVIA';

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;

RETURN retValue;
END checkGestioneArchivio; 

/
