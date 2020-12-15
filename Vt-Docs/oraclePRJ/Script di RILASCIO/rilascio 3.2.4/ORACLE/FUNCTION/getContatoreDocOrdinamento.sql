create or replace
FUNCTION            getContatoreDocOrdinamento (docNumber INT, tipoContatore CHAR)
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
DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO = DPA_OGGETTI_CUSTOM.SYSTEM_ID
AND 
dpa_oggetti_custom.CHA_TIPO_TAR = tipoContatore
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';

RETURN to_number(risultato);

END getContatoreDocOrdinamento; 