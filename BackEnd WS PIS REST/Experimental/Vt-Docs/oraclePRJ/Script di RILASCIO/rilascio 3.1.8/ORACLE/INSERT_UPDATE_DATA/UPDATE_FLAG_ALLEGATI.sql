-- Aggiornamento flag allegati

-- per PEC

UPDATE versions set cha_allegati_esterno = 'P'
where docnumber in 
(select b.system_id from dpa_notifica a, profile b 
where a.docnumber = b.id_documento_principale and 
b.var_prof_oggetto LIKE 'Ricevuta di ritorno delle Mail%');

-- per IS

UPDATE versions set cha_allegati_esterno = 'I'
where docnumber in 
(select b.system_id from dpa_notifica a, profile b 
where a.docnumber = b.id_documento_principale and 
(upper(b.var_prof_oggetto) = upper('Ricevuta di avvenuta consegna') OR 
upper(b.var_prof_oggetto) = upper('Ricevuta di mancata consegna')));