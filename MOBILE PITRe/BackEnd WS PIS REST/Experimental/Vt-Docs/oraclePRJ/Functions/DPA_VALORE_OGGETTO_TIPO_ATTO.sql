--------------------------------------------------------
--  DDL for Function DPA_VALORE_OGGETTO_TIPO_ATTO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DPA_VALORE_OGGETTO_TIPO_ATTO" (id_atto number, doc_num number)
RETURN VARCHAR2 IS
/******************************************************************************
NAME:       dpa_valore_oggetto_tipo_atto
PURPOSE:

REVISIONS:
Ver        Date        Author           Description
---------  ----------  ---------------  ------------------------------------
1.0        12/10/2007          1. Created this function.

NOTES:

Automatically available Auto Replace Keywords:
Object Name:     dpa_valore_oggetto_tipo_atto
Sysdate:         12/10/2007
Date and Time:   12/10/2007, 10:01:03 AM, and 12/10/2007 10:01:03 AM
Username:         (set in TOAD Options, Procedure Editor)
Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/
BEGIN
declare
tmpVar varchar2(255) := ' ';
begin

if (id_atto = 84 )
then
begin
select a.valore_oggetto_db into tmpVar from dpa_associazione_templates a
where a.id_template=id_atto and a.doc_number=doc_num and a.id_oggetto=181;
RETURN tmpVar;
end;
else
return tmpVar ;
end if;

EXCEPTION
WHEN OTHERS THEN
return  tmpVar;

if tmpVar is null
then
tmpVar := ' ';
end if;


end;

END dpa_valore_oggetto_tipo_atto; 

/
