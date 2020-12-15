
ALTER FUNCTION [DOCSADM].[getContatoreDocOrdinamento](@docNumber INT, @tipoContatore CHAR(2000))

RETURNS INT AS
BEGIN



   DECLARE @risultato VARCHAR(255)



   select   @risultato = valore_oggetto_db
   from
   [DOCSADM].dpa_associazione_templates, [DOCSADM].dpa_oggetti_custom, [DOCSADM].dpa_tipo_oggetto
   where
   [DOCSADM].dpa_associazione_templates.doc_number = STR(@docNumber)
   and
   [DOCSADM].dpa_associazione_templates.id_oggetto = [DOCSADM].dpa_oggetti_custom.system_id
   and
   [DOCSADM].dpa_oggetti_custom.CHA_TIPO_TAR = @tipoContatore
   and
   [DOCSADM].dpa_oggetti_custom.id_tipo_oggetto = [DOCSADM].dpa_tipo_oggetto.system_id
   and
   [DOCSADM].dpa_tipo_oggetto.descrizione = 'Contatore'
   and
   [DOCSADM].dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA = '1'

   RETURN CAST(@risultato as INT)

END




