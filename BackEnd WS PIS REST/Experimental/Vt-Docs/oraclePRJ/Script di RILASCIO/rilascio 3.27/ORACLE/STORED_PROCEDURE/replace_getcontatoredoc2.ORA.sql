
-- PALUMBO: modifica per problema ricerca con griglia

begin
Utl_Backup_Plsql_code ('PROCEDURE','getContatoreDoc2');
end;
/

CREATE OR REPLACE FUNCTION getContatoreDoc2 (docNumber INT, tipoContatore CHAR, oggettoCustomId INT)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;
BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';


begin

select
valore_oggetto_db, anno, repertorio
into valoreContatore, annoContatore, repertorio
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
dpa_oggetti_custom.system_id= oggettoCustomId;
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
--and
--dpa_oggetti_custom.cha_tipo_tar = 'T';
exception when others then null;

end;

IF(repertorio = 1) THEN
BEGIN
risultato := '#CONTATORE_DI_REPERTORIO#';
RETURN risultato;
END;
END IF;

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
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.system_id= oggettoCustomId
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
--and
--dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
and
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
exception when others then null;

END;
END IF;

if(codiceRegRf is  null)
then
risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
else  
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
end if; 

RETURN risultato;
End Getcontatoredoc2;
/


