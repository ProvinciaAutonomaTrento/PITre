CREATE OR REPLACE VIEW @db_user.DPA_V_DOCUMENTI
(ID_PROFILE, NUM_PROTO, DTA_PROTO, COD_FASC, CHA_TIPO_PROTO, 
 DTA_PROTO_IN, VAR_PROTO_IN, VAR_PROF_OGGETTO, VAR_NOTE, AMBITO, 
 SETTORE, OPERAZIONE, OPERATORE, MITTENTE, DATATRASMISSIONE)
AS 
SELECT   /*+index (p)*/  p.system_id AS id_profile, p.num_proto, p.dta_proto,
codfasc (p.system_id) AS cod_fasc, p.cha_tipo_proto, p.dta_proto_in,
p.var_proto_in, p.var_prof_oggetto, p.var_note,
getdesccorr (dca.id_corr_globale) AS ambito,
getdesccorr (dca.id_parent) AS settore,
'protocollazione' AS operazione,
getpeoplename (p.id_people_prot) AS operatore,
corrcat (p.system_id, p.cha_tipo_proto) AS mittente,
getdtainvio (p.system_id) AS datatrasmissione
FROM PROFILE p, dpa_corr_abilitati dca
WHERE p.id_uo_prot = dca.id_corr_globale
AND p.cha_tipo_proto IN ('A', 'P')
AND p.cha_da_proto = '0'
AND (p.system_id > (SELECT MAX (id_ultimo_doc_scaricato)
FROM dpa_dati_scaricati))
AND dca.cha_applicazione = '1'
AND dca.id_corr_globale = p.id_uo_prot
ORDER BY p.num_anno_proto, p.id_registro, p.num_proto
/


