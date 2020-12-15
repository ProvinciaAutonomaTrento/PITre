--------------------------------------------------------
--  DDL for Procedure TMP_DEBUGANNUALEDOCTABLEF
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."TMP_DEBUGANNUALEDOCTABLEF" (mese number,p_anno number,id_reg number,id_ammi number,VAR_SED varchar, titolario    NUMBER)
--dichiarazioni variabili

-- modifica APrile 2011 - uso di EXECUTE IMMEDIATE per migliorare la leggibilita del codice
IS istruzioneSQLbase varchar2(2000);
istruzioneSQL varchar2(2000); 

totAnnDoc float;
totAnnProt float;
percAnnProt float;
totAnnProtA float;
percAnnProtA float;
totAnnProtP float;
percAnnProtP float;
totAnnProtI float;
percAnnProtI float;
totAnnDocGrigi float;
percAnnDocGrigi float;
totAnnDocClass float;
percAnnDocClass float;
totAnnDocProf float;
totAnnProtClass float;
percAnnProtClass float;
totAnnProtAClass float;
percAnnProtAClass float;
totAnnProtPClass float;
percAnnProtPClass float;
totAnnProtIClass float;
percAnnProtIClass float;
totAnnProtAnnul float;
percAnnProtAnnul float ;
/******************************************************************/
/**************Dati Riepilogativi del Mese*****************************/
/******************************************************************/
/*Dati Generali*/
totMonDoc float;
totMonProt float;
totMonProtA float;
totMonProtP float;
totMonProtI float;
totMonProtAnnul float;
totMonDocGrigi float;
totMonDocClass float;
/*docs senza docs acq*/
totMonDocProf float;
totMonProtClass float;
totMonProtAClass float;
totMonProtPClass float;
totMonProtIClass float;
/*Percentuali*/
percMonProt float;
percMonProtA float;
percMonProtP float;
percMonProtI float;
percMonProtAnnul float;
percMonDocGrigi float;
percMonDocClass float;
percMonProtClass float;
percMonProtAClass float;
percMonProtPClass float;
percMonProtIClass float;
totMonProtAnnulClass float;
totMonDocGrigiClass float;
/*Dichiarazione delle variabili per i profili (Immagini) *************************************************************************************************/
/*Mensili*/
totMonProf float;
totMonProfProt float;
totMonProfProtA float;
totMonProfProtP float;
totMonProfProtI float;
totMonProfGrigi float;
totMonProfProtAnnul float;
/*Annuali*/
totAnnProf float;
totAnnProfProt float;
totAnnProfProtA float;
totAnnProfProtP float;
totAnnProfProtI float;
totAnnProfGrigi float;
totAnnProfProtAnnull float;
/*Percentuali*/
PercAnnProfProt float;
PercAnnProfProtA float;
PercAnnProfProtP float;
PercAnnProfProtI float;
PercAnnProfGrigi float;
PercAnnProfProtAnnull float;

TotAnnDocGrigiClass float;
percAnnDocGrigiClass float;

TotAnnProtAnnulClass float;
percAnnProtannulClass float;

MESE_VC VARCHAR(255);
var_s VARCHAR(255);
i number;

type tab_system_id is table of  project.system_id%TYPE ;
 report_temp_result tab_system_id; 




begin

--insert into report_temp_result 


--verifica valore parametro var_sede
if (var_sed = '')
then
var_s := null;
else
var_s := var_sed;
end if;

/*Impostiamo i valori di default*/
/*Mensili*/
totMonProf  := 0;
totMonProfProt  := 0;
totMonProfProtA  := 0;
totMonProfProtP  := 0;
totMonProfProtI  := 0;
totMonProfGrigi  := 0;
/*Annuali*/
totAnnProf  := 0;
totAnnProfProt  := 0;
totAnnProfProtA  := 0;
totAnnProfProtP := 0;
totAnnProfProtI := 0;
totAnnProfGrigi := 0;
/*Percentuali*/
PercAnnProfProt := 0;
PercAnnProfProtA := 0;
PercAnnProfProtP := 0;
PercAnnProfProtI := 0;
PercAnnProfGrigi := 0;
totAnnProfProtAnnull := 0;
/**************************************************************************************************************************************************/

percMonProt := 0;
percMonProtA := 0;
percMonProtP := 0;
percMonProtI := 0;
percMonProtAnnul := 0;
percMonDocGrigi := 0;
percMonDocClass := 0;
percMonProtClass := 0;
percMonProtAClass := 0;
percMonProtPClass := 0;
percMonProtIClass := 0;
/******************************************************************/
totAnnDoc := 0;
totAnnProt := 0;
totAnnProtA := 0;
totAnnProtP := 0;
totAnnProtI := 0;
totAnnDocGrigi := 0;
totAnnDocClass := 0;
totAnnDocProf := 0;
totAnnProtClass := 0;
totAnnProtAClass := 0;
totAnnProtPClass := 0;
totAnnProtIClass := 0;
totMonProtAnnulClass := 0;
totMonDocGrigiClass := 0;
totAnnProtAnnul := 0;
percAnnProt := 0;
percAnnProtA := 0;
percAnnProtP := 0;
percAnnProtI := 0;
percAnnDocGrigi := 0;
percAnnDocClass := 0;
percAnnProtClass := 0;
percAnnProtAClass := 0;
percAnnProtPClass := 0;
percAnnProtIClass := 0;
percAnnProtAnnul := 0;

totMonProfProt  := 0;
totMonProfProtA  := 0;
totMonProfProtP  := 0;
totMonProfProtI  := 0;
totMonProfGrigi  := 0;
totMonProfProtAnnul := 0;
PercAnnProfProtAnnull := 0;

TotAnnDocGrigiClass := 0;
percAnnDocGrigiClass := 0;

TotAnnProtAnnulClass := 0;
percAnnProtannulClass := 0;


/*cicliamo dall'inizio dell'anno fino al mese di interesse*/

i := 0;

while (i < mese)
loop
/*Incrementiamo il contatore*/
i := i + 1;

/*Query che recupera i dati del singolo mese*/
/*Totale dati del mese*/
/*Non filtriamo sul registro, questa query deve essere ripetuta per tutti i mesi di interesse per ogni registro*/

if((var_s != '') and (var_s is not null)) then 

istruzioneSQLbase := ' 
select count(profile.system_id)  from profile 
    where to_number(to_char(profile.DTA_PROTO,''MM'')) = '||i||' 
    AND profile.NUM_ANNO_PROTO = '||p_anno||' 
    AND profile.cha_da_proto = ''0'' 
    and profile.id_registro = '||id_reg||' 
    and PROFILE.var_sede = '||var_s ;
    else   
istruzioneSQLbase := ' 
select count(profile.system_id)  from profile 
    where to_number(to_char(profile.DTA_PROTO,''MM'')) = '||i||' 
    AND profile.NUM_ANNO_PROTO = '||p_anno||' 
    AND profile.cha_da_proto = ''0'' 
    and profile.id_registro = '||id_reg||' 
    and PROFILE.var_sede is NULL ' ;
end if; 
    
istruzioneSQL := istruzioneSQLbase || ' AND profile.cha_tipo_proto = ''A'' AND profile.dta_annulla is null ';
execute immediate istruzioneSQL into totMonProtA; 

--select count(profile.system_id) into totMonProtP from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i    AND profile.NUM_ANNO_PROTO = p_anno   
--AND profile.cha_tipo_proto = 'P'     AND profile.cha_da_proto = '0'    and profile.id_registro = id_reg   AND profile.dta_annulla is null     AND profile.var_sede = var_s;
istruzioneSQL := istruzioneSQLbase || ' AND profile.cha_tipo_proto = ''P'' AND profile.dta_annulla is null ';
execute immediate istruzioneSQL into totMonProtP; 
 
--select count(profile.system_id) into totMonProtI from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno 
--AND profile.cha_tipo_proto = 'I' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null AND profile.var_sede = var_s;
istruzioneSQL := istruzioneSQLbase || ' AND profile.cha_tipo_proto = ''I'' AND profile.dta_annulla is null ';
execute immediate istruzioneSQL into totMonProtI; 

--select count(profile.system_id) into totMonProtAnnul from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO  = p_anno 
--    AND profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND  profile.dta_annulla is not null AND profile.num_proto is not null AND profile.var_sede = var_s;
istruzioneSQL := istruzioneSQLbase || ' AND profile.dta_annulla is NOT null ';
execute immediate istruzioneSQL into totMonProtAnnul ; 

select count(profile.system_id) into totMonDocGrigi from profile,people,dpa_l_ruolo_reg 
where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo AND dpa_l_ruolo_reg.id_registro = id_reg 
and profile.author = people.system_id and people.id_amm = id_ammi and profile.num_proto is null 
AND profile.cha_tipo_proto = 'G' AND ID_DOCUMENTO_PRINCIPALE  IS NULL 
AND to_number(to_char(profile.creation_date,'MM')) = i 
AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno AND profile.var_sede = var_s;


if((var_s != '') and (var_s is not null)) then 

-- documenti con immagini
istruzioneSQLbase := '
select count(p.system_id) from profile p 
    where to_number(to_char(p.DTA_PROTO,''MM'')) = '||i||' 
    AND p.NUM_ANNO_PROTO = '||p_anno||' 
    AND p.cha_da_proto = ''0'' 
    and p.cha_img = ''0'' 
    and p.id_registro = '||id_reg||' 
    and PROFILE.var_sede = '||var_s ;
   else 
istruzioneSQLbase := '
select count(p.system_id) from profile p 
    where to_number(to_char(p.DTA_PROTO,''MM'')) = '||i||' 
    AND p.NUM_ANNO_PROTO = '||p_anno||' 
    AND p.cha_da_proto = ''0'' 
    and p.cha_img = ''0'' 
    and p.id_registro = '||id_reg||' 
    and PROFILE.var_sede is null ';
end if ; 

--select count(p.system_id) into totMonProfProtA from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i 
--AND p.NUM_ANNO_PROTO = p_anno AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.var_sede = var_s
--AND p.dta_annulla is null and p.cha_tipo_proto = 'A';

istruzioneSQL := istruzioneSQLbase || ' AND p.dta_annulla is null and p.cha_tipo_proto = ''A'' ';
execute immediate istruzioneSQL into totMonProfProtA ; 

--select count(p.system_id) into totMonProfProtP from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i 
--AND p.NUM_ANNO_PROTO = p_anno AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.var_sede = var_s
--AND p.dta_annulla is null and p.cha_tipo_proto = 'P';
istruzioneSQL := istruzioneSQLbase || ' AND p.dta_annulla is null and p.cha_tipo_proto = ''P'' ';
execute immediate istruzioneSQL into totMonProfProtP ; 



--select count(p.system_id) into totMonProfProtI from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i 
--AND p.NUM_ANNO_PROTO = p_anno AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.var_sede = var_s
--AND p.dta_annulla is null and p.cha_tipo_proto = 'I';
istruzioneSQL := istruzioneSQLbase || ' AND p.dta_annulla is null and p.cha_tipo_proto = ''I'' ';
execute immediate istruzioneSQL into totMonProfProtI  ; 

--select count(p.system_id) into totMonProfProtAnnul from profile p 
--where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno and p.cha_da_proto = '0' and  p.cha_img = '0' AND p.var_sede = var_s and p.id_registro = id_reg 
--AND p.dta_annulla is not null AND num_proto is not null; 
istruzioneSQL := istruzioneSQLbase || ' AND p.dta_annulla is not null AND num_proto is not null';
execute immediate istruzioneSQL into totMonProfProtAnnul  ; 

select count(profile.system_id) into totMonProfGrigi from profile,people,dpa_l_ruolo_reg where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo AND dpa_l_ruolo_reg.id_registro = id_reg and profile.author = people.system_id and people.id_amm = id_ammi and profile.cha_img = '0' and to_number(to_char(profile.creation_date,'MM')) = i AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno and profile.cha_tipo_proto = 'G'  AND ID_DOCUMENTO_PRINCIPALE  IS NULL 
AND profile.var_sede = var_s;

/* 
-- blocco per doc classificati

Inizio Modifiche Aprile 2011 per ticket 1399733 - Prospetti riepilogativi : incongruenza nei totali rispetto a report ottenuto con DocClassCompTableFunction 

le incongruenze erano dovute a uso campo deprecato profile.cha_fascicolato; si deve invece adottare logica estrazione usata in 
funzione DocClassCompTableFunction, basato su Join tra project_components, PROFILE e project  
*/

--select count(profile.system_id) into totMonProtAClass from profile 
--where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'A' 
--and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' 
--AND profile.num_proto is not null AND dta_annulla is null AND profile.var_sede = var_s;

--select count(profile.system_id) into totMonProtPClass from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'P' and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' AND profile.num_proto is not null AND dta_annulla is null AND profile.var_sede = var_s;
--select count(profile.system_id) into totMonProtIClass from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'I' and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' AND profile.num_proto is not null AND dta_annulla is null AND profile.var_sede = var_s;
--select count(profile.system_id) into totMonProtAnnulClass from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno and profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.cha_fascicolato = '1' AND profile.num_proto is not null AND profile.dta_annulla is not null  AND profile.var_sede = var_s;
--select count(profile.system_id) into totMonDocGrigiClass from profile,people,dpa_l_ruolo_reg where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
--AND dpa_l_ruolo_reg.id_registro = id_reg and profile.author = people.system_id and people.id_amm = id_ammi AND to_number(to_char(profile.creation_date,'MM')) = i 
--AND profile.cha_fascicolato = '1' AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno and profile.cha_tipo_proto = 'G'  AND ID_DOCUMENTO_PRINCIPALE  IS NULL AND profile.var_sede = var_s;


if((var_s != '') and (var_s is not null)) then 

    istruzioneSQLbase := 

'SELECT  COUNT ((PROFILE.system_id)) --into totMonProtAClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id
AND prc.project_id = pr.system_id and to_number(to_char(profile.DTA_PROTO,''MM'')) = '||i||'
AND ( (PROFILE.id_registro = '||id_reg||') OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date(''01/01/'''||p_anno||',''dd/mm/yyyy'') 
    AND to_date(''31/12/'''||p_anno||' 23:59:59'',''dd/mm/yyyy HH24:MI:SS'')
AND profile.var_sede = '||var_s||'
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = ''C''
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = ''F''
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = '||titolario||'    
                               AND pr.id_amm = '||id_ammi||'
                               AND pr.cha_tipo_proj = ''T''
                               AND ( pr.id_registro = '||id_reg||' OR pr.id_registro IS NULL)
                        ))  
 )' ;
   else 
    istruzioneSQLbase := 

'SELECT  COUNT ((PROFILE.system_id)) --into totMonProtAClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id
AND prc.project_id = pr.system_id and to_number(to_char(profile.DTA_PROTO,''MM'')) = '||i||'
AND ( (PROFILE.id_registro = '||id_reg||') OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date(''01/01/'''||p_anno||',''dd/mm/yyyy'') 
    AND to_date(''31/12/'''||p_anno||' 23:59:59'',''dd/mm/yyyy HH24:MI:SS'')
AND profile.var_sede is null 
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = ''C''
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = ''F''
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = '||titolario||'    
                               AND pr.id_amm = '||id_ammi||'
                               AND pr.cha_tipo_proj = ''T''
                               AND ( pr.id_registro = '||id_reg||' OR pr.id_registro IS NULL)
                        ))  
 )' ;
end if ; 

istruzioneSQL := istruzioneSQLbase || 'AND profile.cha_tipo_proto = ''A''  AND profile.dta_annulla is null and num_proto is not null';
execute immediate istruzioneSQL into  totMonProtAClass  ;





istruzioneSQL := istruzioneSQLbase || 'AND profile.cha_tipo_proto = ''P''  AND profile.dta_annulla is null and num_proto is not null';
execute immediate istruzioneSQL into  totMonProtPClass ;

-- Protocolli tipo I
istruzioneSQL := istruzioneSQLbase || 'AND profile.cha_tipo_proto = ''I''  AND profile.dta_annulla is null and num_proto is not null';
execute immediate istruzioneSQL into  totMonProtIClass ;
       

-- Protocolli annullati
istruzioneSQL := istruzioneSQLbase || 'AND profile.cha_da_proto = ''0'' AND profile.dta_annulla is not null and num_proto is not null';
execute immediate istruzioneSQL into  totMonProtAnnulClass ;


-- Protocolli tipo Grigi, con id_documento principale = NULL
istruzioneSQL := istruzioneSQLbase || 'AND profile.cha_tipo_proto = ''G'' and id_documento_principale IS NULL  AND profile.dta_annulla is null';
execute immediate istruzioneSQL into  totMonDocGrigiClass ;


--else

/*select count(profile.system_id) into totMonProtA from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'A' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null;
select count(profile.system_id) into totMonProtP from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'P' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null;
select count(profile.system_id) into totMonProtI from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'I' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null;
select count(profile.system_id) into totMonProtAnnul from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO  = p_anno AND profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND  profile.dta_annulla is not null AND profile.num_proto is not null;
select count(profile.system_id) into totMonDocGrigi from profile,people,dpa_l_ruolo_reg where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo AND dpa_l_ruolo_reg.id_registro = id_reg and  profile.author = people.system_id and people.id_amm = id_ammi and profile.num_proto is null AND profile.cha_tipo_proto = 'G'  AND ID_DOCUMENTO_PRINCIPALE  IS NULL AND to_number(to_char(profile.creation_date,'MM')) = i AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno;

select count(p.system_id) into totMonProfProtA from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno and p.cha_tipo_proto = 'A' AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is null;
select count(p.system_id) into totMonProfProtP from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno and p.cha_tipo_proto = 'P' AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is null;
select count(p.system_id) into totMonProfProtI from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno and p.cha_tipo_proto = 'I' AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is null;
select count(p.system_id) into totMonProfProtAnnul from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno AND num_proto is not null and p.cha_da_proto = '0' and  p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is not null;
select count(profile.system_id) into totMonProfGrigi from profile,people,dpa_l_ruolo_reg where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo AND dpa_l_ruolo_reg.id_registro = id_reg and profile.author = people.system_id and people.id_amm = id_ammi and profile.cha_img = '0' and to_number(to_char(profile.creation_date,'MM')) = i AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno and profile.cha_tipo_proto = 'G'  AND ID_DOCUMENTO_PRINCIPALE  IS NULL;


-- blocco per doc classificati

Inizio Modifiche Aprile 2011 per ticket 1399733 - Prospetti riepilogativi : incongruenza nei totali rispetto a report ottenuto con DocClassCompTableFunction 

le incongruenze erano dovute a uso campo deprecato profile.cha_fascicolato; si deve invece adottare logica estrazione usata in 
funzione DocClassCompTableFunction, basato su Join tra project_components, PROFILE e project  

select count(profile.system_id) into totMonProtAClass from profile 
    where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'A' 
    and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' 
    AND profile.num_proto is not null AND dta_annulla is null;

select count(profile.system_id) into totMonProtPClass from profile 
    where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'P' 
    and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' 
    AND profile.num_proto is not null AND dta_annulla is null;

select count(profile.system_id) into totMonProtIClass from profile 
    where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno AND profile.cha_tipo_proto = 'I' 
    and profile.cha_da_proto = '0' and  profile.id_registro = id_reg AND profile.cha_fascicolato = '1' 
    AND profile.num_proto is not null AND dta_annulla is null;

select count(profile.system_id) into totMonProtAnnulClass from profile 
    where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno and profile.cha_da_proto = '0' 
    and profile.id_registro = id_reg AND profile.cha_fascicolato = '1' AND profile.num_proto is not null 
    AND profile.dta_annulla is not null ;

select count(profile.system_id) into totMonDocGrigiClass from profile,dpa_l_ruolo_reg,people 
    where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo AND dpa_l_ruolo_reg.id_registro = id_reg 
    and profile.author = people.system_id and people.id_amm = id_ammi 
    AND to_number(to_char(profile.creation_date,'MM')) = i AND profile.cha_fascicolato = '1' 
    AND to_number(to_char(profile.CREATION_DATE,'YYYY')) = p_anno and profile.cha_tipo_proto = 'G'  
    AND ID_DOCUMENTO_PRINCIPALE  IS NULL;



-- Protocolli tipo A
SELECT  COUNT ((PROFILE.system_id)) into totMonProtAClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
and to_number(to_char(profile.DTA_PROTO,'MM')) = i
AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = 'C'
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = 'F'
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = titolario    
                               AND pr.id_amm = id_ammi
                               AND pr.cha_tipo_proj = 'T'
                               AND ( pr.id_registro = id_reg OR pr.id_registro IS NULL)
  -- in questo caso non serve ricostruire la gerarchia tra i nodi
  --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1 
                        ))  
) 
AND profile.cha_tipo_proto = 'A'  AND profile.dta_annulla is null ;

SELECT  COUNT ((PROFILE.system_id)) into totMonProtPClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
and to_number(to_char(profile.DTA_PROTO,'MM')) = i
AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM report_temp_results ) 
AND profile.cha_tipo_proto = 'P' AND profile.dta_annulla is null ;
        
-- Protocolli tipo I
SELECT  COUNT ((PROFILE.system_id)) into totMonProtIClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
and to_number(to_char(profile.DTA_PROTO,'MM')) = i
AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = 'C'
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = 'F'
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = titolario    
                               AND pr.id_amm = id_ammi
                               AND pr.cha_tipo_proj = 'T'
                               AND ( pr.id_registro = id_reg OR pr.id_registro IS NULL)
  -- in questo caso non serve ricostruire la gerarchia tra i nodi
  --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1 
                        ))  
)
AND profile.cha_tipo_proto = 'I' AND profile.dta_annulla is null ;  
 

-- Protocolli annullati
SELECT  COUNT ((PROFILE.system_id)) into totMonProtAnnulClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
and to_number(to_char(profile.DTA_PROTO,'MM')) = i
AND profile.cha_da_proto = '0' AND profile.dta_annulla is not null 
AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = 'C'
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = 'F'
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = titolario    
                               AND pr.id_amm = id_ammi
                               AND pr.cha_tipo_proj = 'T'
                               AND ( pr.id_registro = id_reg OR pr.id_registro IS NULL)
  -- in questo caso non serve ricostruire la gerarchia tra i nodi
  --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1 
                        ))  
) ; 



-- Protocolli tipo Grigi, con id_documento principale = NULL
SELECT  COUNT ((PROFILE.system_id)) into totMonDocGrigiClass
FROM project_components prc, PROFILE     , project pr
WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
AND to_char(profile.creation_date,'MM') = i 
AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
AND prc.project_id IN ( -- tabella temporanea 
SELECT system_id FROM project
     WHERE cha_tipo_proj = 'C'
       AND id_fascicolo IN (       -----= parentid
              SELECT system_id FROM project
               WHERE cha_tipo_proj = 'F'
                 AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                        SELECT     system_id
                        FROM       project pr
                             WHERE pr.var_codice IS NOT NULL
                               AND pr.id_titolario = titolario    
                               AND pr.id_amm = id_ammi
                               AND pr.cha_tipo_proj = 'T'
                               AND ( pr.id_registro = id_reg OR pr.id_registro IS NULL)
  -- in questo caso non serve ricostruire la gerarchia tra i nodi
  --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1 
                        ))  
) 
AND profile.cha_tipo_proto = 'G' and id_documento_principale IS NULL;

-- fine modifiche Aprile 2011

end if;
*/

/*Calcoliamo i valori annuali a partire dai dati del mese***************************************/
--
totMonProt := totMonProtA + totMonProtP + totMonProtI + totMonProtAnnul;
totMonDoc := totMonProt + totMonDocGrigi;
totAnnDoc := totAnnDoc + totMonDoc;
totAnnProt := totAnnProt + totMonProt;
totAnnProtA := totAnnProtA + totMonProtA;
totAnnProtP := totAnnProtP + totMonProtP;
totAnnProtI := totAnnProtI + totMonProtI;
totAnnDocGrigi := totAnnDocGrigi + totMonDocGrigi;

totMonDocClass := totMonDocGrigiClass + totMonProtAnnulClass + totMonProtAClass + totMonProtPClass + totMonProtIClass;
totAnnDocClass := totAnnDocClass + totMonDocClass;
--totAnnDocClass := totMonDocGrigiClass; 

-- Totale Protocolli = Annullati + tipo A + tipo P + tipo I  
totMonProtClass := totMonProtAnnulClass + totMonProtAClass + totMonProtPClass + totMonProtIClass;
totAnnProtClass := totAnnProtClass + totMonProtClass;
totAnnProtAClass := totAnnProtAClass + totMonProtAClass;
totAnnProtPClass := totAnnProtPClass + totMonProtPClass;
totAnnProtIClass := totAnnProtIClass + totMonProtIClass;
TotAnnProtAnnulClass := TotAnnProtAnnulClass + totMonProtAnnulClass;


totAnnProtAnnul := totAnnProtAnnul + totMonProtAnnul;
--
totMonProf := totMonProfProtAnnul + totMonProfGrigi + totMonProfProtA + totMonProfProtP + totMonProfProtI;
totAnnProf  := totAnnProf + totMonProf;
--
totMonProfProt :=  totMonProfProtAnnul + totMonProfProtA + totMonProfProtP + totMonProfProtI;
totAnnProfProt  := totAnnProfProt + totMonProfProt;
totAnnProfProtA  := totAnnProfProtA + totMonProfProtA;
totAnnProfProtP := totAnnProfProtP + totMonProfProtP;
totAnnProfProtI := totAnnProfProtI + totMonProfProtI;
totAnnProfGrigi := totAnnProfGrigi + totMonProfGrigi;
totAnnProfProtAnnull := totAnnProfProtAnnull + totMonProfProtAnnul;

TotAnnDocGrigiClass := TotAnnDocGrigiClass + totMonDocGrigiClass;

/*****Percentuali************************************************************************/
/*Percentuale dei protoclli annullati classificati*/
if(TotAnnProtAnnulClass <> 0 and totAnnProtClass <> 0)
then
    percAnnProtannulClass := ROUND(((TotAnnProtAnnulClass / totAnnProtClass) * 100),2);
end if;
/*Percentuale annuale dei documenti grigi classificati*/
if(totAnnDocClass <> 0 and TotAnnDocGrigiClass <> 0)
then
    percAnnDocGrigiClass := ROUND(((TotAnnDocGrigiClass / totAnnDocClass) * 100),2);
end if;




/*Percentuale dei profili annullati*/
if((TotAnnProfProt <> 0) AND (totAnnProfProtAnnull <> 0))
then
    PercAnnProfProtAnnull := ROUND(((totAnnProfProtAnnull / TotAnnProfProt) * 100),2);
end if;

if(totAnnProt <> 0)
then
/*Percentuale di documenti protocollati*/
    percAnnProt := ROUND(((totAnnProt / totAnnDoc) * 100),2);
if(totAnnProtA <> 0)
then
/*Percentuale di protocolli in arrivo*/
    percAnnProtA := ROUND(((totAnnProtA / totAnnProt) * 100),2);
end if;

if(totAnnProtP <> 0)
then
/*Percentuale di protocolli in partenza*/
    percAnnProtP := ROUND(((totAnnProtP / totAnnProt) * 100),2);
end if;

if(totAnnProtI <> 0)
then
/*Percentuale di protocolli interni*/
    percAnnProtI := ROUND(((totAnnProtI / totAnnProt) * 100),2);
end if;

if(totAnnProtAnnul <> 0)
then
/*Percentuale di protocolli annullati*/
    percAnnProtAnnul := ROUND(((totAnnProtAnnul / totAnnProt) * 100),2);
end if;
end if;

if(totAnnDoc <> 0)
then
if(totAnnDocGrigi <> 0)
then
/*Percentuale di doc grigi*/
percAnnDocGrigi := ROUND(((totAnnDocGrigi / totAnnDoc ) * 100),2);
end if;
if(totAnnDocClass <> 0)
then
/*Percentuale di doc classificati*/
percAnnDocClass := ROUND(((totAnnDocClass / totAnnDoc) * 100),2);
end if;
end if;

if(totAnnDocClass <> 0)
then
if(totAnnProtClass <> 0)
then
/*Percentuale di doc classificati e protocollati*/
percAnnProtClass := ROUND(((totAnnProtClass / totAnnDocClass)*100),2);
if(totAnnProtAClass <> 0)
then
/*Percentuale di doc classificati e protocollati in arrivo*/
percAnnProtAClass := ROUND(((totAnnProtAClass / totAnnProtClass) * 100),2);
end if;
if(totAnnProtPClass <> 0)
then
/*Percentuale di doc classificati e protocollati in partenza*/
percAnnProtPClass := ROUND(((totAnnProtPClass / totAnnProtClass) * 100),2) ;
end if;
if(totAnnProtIClass <> 0)
then
/*Percentuale di doc classificati e protocollati interni*/
percAnnProtIClass := ROUND(((totAnnProtIClass / totAnnProtClass) * 100),2);
end if;
end if;
end if;

/*Calcoliamo le percentuali mensili**************************************************************************************/
if(totMonDoc <> 0)
then
if(totMonProt <> 0)
then
/*Percentuale mensile di protocolli*/
percMonProt := ROUND(((totMonProt / totMonDoc) * 100),2);
if(totMonProtA <> 0)
then
/*Percentuale mensile di protocolli ARRIVO*/
percMonProtA := ROUND(((totMonProtA / totMonProt) * 100),2);
end if;
if(totMonProtP <> 0)
then
/*Percentuale mensile di protocolli PARTENZA*/
percMonProtP := ROUND(((totMonProtP / totMonProt) * 100),2);
end if;
if(totMonProtI <> 0)
then
/*Percentuale mensile di protocolli INTERNI*/
percMonProtI := ROUND(((totMonProtI / totMonProt) * 100),2);
end if;
if(totMonProtAnnul <> 0)
then
/*Percentuale mensile di protocolli Annullati*/
percMonProtAnnul := ROUND(((totMonProtAnnul / totMonProt) * 100),2);
end if;
end if;

if(totMonDocGrigi <> 0)
then
/*Percentuale mensile di Doc Grigi*/
percMonDocGrigi := ROUND(((totMonDocGrigi / totMonDoc) * 100),2);
end if;
if(totMonDocClass <> 0)
then
/*Percentuale mensile di Doc Class*/
percMonDocClass := ROUND(((totMonDocClass / totMonDoc) * 100),2);
end if;
if(totMonProtClass <> 0)
then
/*Percentuale mensile di protocolli Class*/
percMonProtClass := ROUND(((totMonProtClass / totMonDoc) * 100),2);
end if;
if(totMonProtAClass <> 0)
then
/*Percentuale mensile di protocolli Arrivo Class*/
percMonProtAClass := ROUND(((totMonProtAClass / totMonDoc) * 100),2);
end if;
if(totMonProtPClass <> 0)
then
/*Percentuale mensile di protocolli Partenza Class*/
percMonProtPClass := ROUND(((totMonProtPClass / totMonDoc) * 100),2);
end if;
if(totMonProtIClass <> 0)
then
/*Percentuale mensile di protocolli Interni Class*/
percMonProtIClass := ROUND(((totMonProtIClass / totMonDoc) * 100),2);
end if;

end if;
/*******************************************************************************************************************/
/*Calcoliamo le percentuali  dei profili ( Immagini)  */
if(totAnnProf<>0)
then
if(totAnnProfGrigi<>0)
then
/*Percentuale  annuale di profili grigi*/
PercAnnProfGrigi := ROUND(((totAnnProfGrigi / totAnnProf) * 100),2);
end if;

if(totAnnProfProt<>0)
then
/*Percentuale  annuale di profili protocollati*/
PercAnnProfProt := ROUND(((totAnnProfProt / totAnnProf) * 100),2);
end if;

if(totAnnProfProtA<>0)
then
/*Percentuale  annuale di profili protocollati ARRIVO*/
PercAnnProfProtA := ROUND(((totAnnProfProtA / totAnnProfProt) * 100),2);
end if;

if(totAnnProfProtP<>0)
then
/*Percentuale  annuale di profili protocollati PARTENZA*/
PercAnnProfProtP := ROUND(((totAnnProfProtP / totAnnProfProt) * 100),2);
end if;

if(totAnnProfProtI<>0)
then
/*Percentuale  annuale di profili protocollati PARTENZA*/
PercAnnProfProtI := ROUND(((totAnnProfProtI / totAnnProfProt) * 100),2);
end if;
end if;
/*******************************************************************************************************************/

MESE_VC :=
CASE i
WHEN 1 THEN 'Gennaio'
WHEN 2 THEN 'Febbraio'
WHEN 3 THEN 'Marzo'
WHEN 4 THEN 'Aprile'
WHEN 5 THEN 'Maggio'
WHEN 6 THEN 'Giugno'
WHEN 7 THEN 'Luglio'
WHEN 8 THEN 'Agosto'
WHEN 9 THEN 'Settembre'
WHEN 10 THEN 'Ottobre'
WHEN 11 THEN 'Novembre'
WHEN 12 THEN 'Dicembre'
end;




/*RESET DELLE VARIABILI*/
totMonDoc := 0;
totMonProt := 0;
totMonProtA := 0;
totMonProtP := 0;
totMonProtI := 0;
totMonDocGrigi := 0;
/*RESET DELLE PERCENTUALI MENSILI*/
percMonProt := 0;
percMonProtA := 0;
percMonProtP := 0;
percMonProtI := 0;
percMonProtAnnul := 0;
percMonDocGrigi := 0;
percMonDocClass := 0;
percMonProtClass := 0;
percMonProtAClass := 0;
percMonProtPClass := 0;
percMonProtIClass := 0;
/**********************************/
end loop;

/*Inseriamo nella tabella i valori reltivi all'anno*/
/*Aggiungiamo al totale dei documenti annuale il totale dei documenti grigi dell'anno */
/*totAnnDoc := totAnnDoc + totAnnDocGrigi;*/



/*RESET DELLE PERCENTUALI ANNUALI*/
percAnnProt := 0;
percAnnProtA := 0;
percAnnProtP := 0;
percAnnProtI := 0;
percAnnProtAnnul := 0;
percAnnDocGrigi := 0;
percAnnDocClass := 0;
percAnnProtClass := 0;
percAnnProtAClass := 0;
percAnnProtPClass := 0;
percAnnProtIClass := 0;
PercAnnProfGrigi := 0;
PercAnnProfProt := 0;
PercAnnProfProtA := 0;
PercAnnProfProtP := 0;
PercAnnProfProtI := 0;


RETURN;

--EXCEPTION WHEN OTHERS THEN
--RETURN;
END  tmp_debugAnnualeDocTableF  ; 

/
