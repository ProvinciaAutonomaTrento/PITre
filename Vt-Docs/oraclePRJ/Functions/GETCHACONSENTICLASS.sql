--------------------------------------------------------
--  DDL for Function GETCHACONSENTICLASS
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCHACONSENTICLASS" (p_id_parent number, p_cha_tipo_proj varchar2, p_id_fascicolo number)
RETURN VARCHAR2
IS
risultato   VARCHAR2 (16);
BEGIN

BEGIN

BEGIN
risultato := '0';

if (p_cha_tipo_proj = 'F') 
then select cha_consenti_class into risultato from project where system_id = p_id_parent;
end if;
    
if (p_cha_tipo_proj = 'C')
then select cha_consenti_class into risultato from project 
        where system_id in (select id_parent from project where system_id = p_id_fascicolo);
end if;

END;

END;

RETURN risultato;
END getChaConsentiClass; 

/
