--------------------------------------------------------
--  DDL for Function GETCONTATOREDOCORDINAMENTO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCONTATOREDOCORDINAMENTO" (docNumber INT, tipoContatore CHAR)
RETURN int IS risultato VARCHAR(255);

BEGIN

select
valore_oggetto_db
into risultato
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';

RETURN to_number(risultato);

END getContatoreDocOrdinamento; 

/
