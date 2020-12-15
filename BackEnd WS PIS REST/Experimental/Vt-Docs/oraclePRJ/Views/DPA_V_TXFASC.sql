--------------------------------------------------------
--  DDL for View DPA_V_TXFASC
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."DPA_V_TXFASC" ("ID_FASC", "DTA_INVIO", "RAGIONE", "ID_CORR_GLOB") AS 
  select    id_project AS ID_FASC,dta_invio AS DTA_INVIO,upper(r.var_desc_ragione) as RAGIONE,s.id_corr_globale as ID_CORR_GLOB from dpa_trasmissione t,dpa_trasm_singola s,dpa_ragione_trasm r where t.cha_tipo_oggetto='F' and s.id_trasmissione=t.system_id and r.system_id=s.id_ragione and dta_invio=dpa_getMaxdtaInvioFAsc(t.id_project) group by dta_invio,id_project,upper(r.var_desc_ragione),s.id_corr_globale order by dtA_invio desc;
