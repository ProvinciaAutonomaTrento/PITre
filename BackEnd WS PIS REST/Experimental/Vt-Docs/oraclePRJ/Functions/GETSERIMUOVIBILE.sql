--------------------------------------------------------
--  DDL for Function GETSERIMUOVIBILE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSERIMUOVIBILE" 
(p_idProfile number,
p_idTipoAtto number :=0,
p_id_people_connesso number,
p_idPeople number,
p_id_ruolo_creatore number,
p_Cfg_Enable_canc_doc_trasm number)
RETURN int IS risultato int ;


BEGIN
begin
risultato := 0;

SELECT COUNT(NUM_PROTO) into risultato FROM PROFILE WHERE SYSTEM_ID = p_idProfile;

if(risultato = 0) then

IF (p_idPeople=p_id_people_connesso AND p_Cfg_Enable_canc_doc_trasm=0) then
--Controllo se ? stato trasmesso
select  count(trasm.system_id) into risultato
from dpa_trasmissione trasm, dpa_trasm_singola sing, dpa_trasm_utente ut, profile p
where trasm.system_id = sing.id_trasmissione and sing.system_id = ut.id_trasm_singola
and trasm.id_profile=p.SYSTEM_ID
and p.CHA_DA_PROTO <> 0 and p.NUM_PROTO is null
and trasm.id_profile = p_idProfile;

if(risultato>0) then

risultato :=1;
return risultato;

end if;

END if;


IF (p_idPeople <> p_id_people_connesso) then
--controllo se ? stato trasmesso per interop
select count(trasm.system_id) into risultato
from dpa_trasmissione trasm, dpa_trasm_singola sing, dpa_trasm_utente ut, dpa_ragione_trasm rag, profile p
where trasm.system_id = sing.id_trasmissione and sing.system_id = ut.id_trasm_singola and sing.id_ragione = rag.system_id
and trasm.id_profile=p.SYSTEM_ID
and p.CHA_DA_PROTO <> 0 and p.NUM_PROTO is null
and trasm.id_profile = p_idProfile and rag.cha_tipo_ragione ='I'
and upper (var_desc_ragione) = upper('INTEROPERABILITA');

-- si puo rimuovere solo se trasm interop
if(risultato=0) then

risultato :=1;
return risultato;

end if;


-- controllo di prau -- se il creatore ? prau posso rimuovere
SELECT  COUNT(*) into risultato FROM DPA_TIPO_F_RUOLO WHERE id_Ruolo_in_uo= p_id_ruolo_creatore
AND ID_TIPO_FUNZ IN (SELECT SYSTEM_ID FROM DPA_TIPO_FUNZIONE WHERE UPPER(VAR_COD_TIPO)='PRAU');

if (risultato = 0) then

risultato := 1;
return risultato;
end if;

END if;

if(p_idTipoAtto <> 0) then
--Verifica se il documento ? repertoriato
select count(profile.docnumber) into risultato
from profile, dpa_associazione_templates, dpa_oggetti_custom
where
profile.docnumber = dpa_associazione_templates.doc_number
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
profile.system_id = p_idProfile
and
dpa_associazione_templates.id_template = p_idTipoAtto
and
dpa_oggetti_custom.repertorio = 1
and
dpa_associazione_templates.valore_oggetto_db is not null;


if ( risultato > 0) then

risultato := 1;
return risultato;
end if;

end if;

else risultato := 1; end if;



EXCEPTION
WHEN OTHERS THEN
risultato := -1;


end;
return risultato;
end GetSeRimuovibile;

/
