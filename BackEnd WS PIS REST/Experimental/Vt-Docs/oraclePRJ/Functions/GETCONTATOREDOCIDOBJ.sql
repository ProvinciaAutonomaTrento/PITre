--------------------------------------------------------
--  DDL for Function GETCONTATOREDOCIDOBJ
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCONTATOREDOCIDOBJ" (docNumber INT, tipoContatore CHAR,IdObj int)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);

BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';

select
valore_oggetto_db, anno
into valoreContatore, annoContatore
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore' and dpa_oggetti_custom.system_id=IdObj
and
dpa_oggetti_custom.cha_tipo_tar = tipoContatore;

IF(tipoContatore<>'T') THEN
BEGIN
select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore' and dpa_oggetti_custom.system_id=IdObj
and
dpa_oggetti_custom.cha_tipo_tar = tipoContatore
and
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
END;
END IF;

if(codiceRegRf is not  null)
then
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
else
risultato :=   nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
end if;

RETURN risultato;
END getContatoreDocIdObj; 

/
