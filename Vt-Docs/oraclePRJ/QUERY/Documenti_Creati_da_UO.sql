
-- This query will display the current date

select docname, pe.FULL_NAME as creatore, g.VAR_DESC_CORR as ruolo, creation_date as data_creazione, cha_tipo_proto as tipo_protocollo, 
num_proto as numero_protocollo, num_anno_proto as anno, var_segnatura as segnatura, var_prof_oggetto as oggetto, corrcat(p.DOCNUMBER, p.CHA_TIPO_PROTO) as mitt_dest,
classcat(p.docnumber), (select decode(nvl( c.cartaceo,'0'),'0','NO','SI') from versions c where c.DOCNUMBER = p.DOCNUMBER and getmaxver(c.docnumber) = c.version_id) as nativo_digitale
 from profile p, people pe, dpa_corr_globali g
 where p.id_ruolo_creatore IN (
      (select system_id from dpa_corr_globali where id_uo in 
      (SELECT dpa_corr_globali.system_id
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.dta_fine IS NULL
      CONNECT BY PRIOR dpa_corr_globali.system_id = dpa_corr_globali.id_parent
      START WITH dpa_corr_globali.system_id =
      (SELECT system_id FROM dpa_corr_globali WHERE var_codice = 'S059')
      )))
and p.AUTHOR = pe.SYSTEM_ID
and p.ID_RUOLO_CREATORE = g.SYSTEM_ID


