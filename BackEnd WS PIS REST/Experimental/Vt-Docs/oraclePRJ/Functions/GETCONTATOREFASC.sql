--------------------------------------------------------
--  DDL for Function GETCONTATOREFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCONTATOREFASC" (systemId INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);

BEGIN

select
valore_oggetto_db, anno
into valoreContatore, annoContatore
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;


if (tipocontatore <> 'T') then
begin

select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore
and
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
END;
else
risultato :=   nvl(valoreContatore,'');
END IF;










RETURN risultato;
END getContatoreFasc; 

/
