--------------------------------------------------------
--  DDL for Function ANNUALEDOCTABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ANNUALEDOCTABLEFUNCTION" 
(mese number,p_anno number,id_reg number,id_ammi number
,SEDE varchar  --se SEDE = 'RITORNA_ELAPSED' ritorna l'elapsed in perc-grigi 
, titolario    NUMBER)
return AnnualeDocTableRow pipelined is
out_rec AnnualeDocTableType := AnnualeDocTableType(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);

   tsstarttime        TIMESTAMP;
   tsendtime          TIMESTAMP;
   elapsed            VARCHAR2 (40); 
   min_elapsed        VARCHAR2 (2);  
   sec_elapsed        VARCHAR2 (2);
   msec_elapsed       VARCHAR2 (3);

istruzioneSQLbase varchar2(2000);
istruzioneSQL varchar2(2000); 
condizione_titolario varchar2(200);
condizione_sede     varchar2(200);
condizione_sede_p   varchar2(200);
mese_successivo varchar2(200);
totAnnDoc float := 0 ;
totAnnProt float := 0 ;
percAnnProt float := 0 ;
totAnnProtA float := 0 ;
percAnnProtA float := 0 ;
totAnnProtP float := 0 ;
percAnnProtP float := 0 ;
totAnnProtI float := 0 ;
percAnnProtI float := 0 ;
totAnnDocGrigi float := 0 ;
percAnnDocGrigi float := 0 ;
totAnnDocClass float := 0 ;
percAnnDocClass float := 0 ;
totAnnDocProf float := 0 ;
totAnnProtClass float := 0 ;
percAnnProtClass float := 0 ;
totAnnProtAClass float := 0 ;
percAnnProtAClass float := 0 ;
totAnnProtPClass float := 0 ;
percAnnProtPClass float := 0 ;
totAnnProtIClass float := 0 ;
percAnnProtIClass float := 0 ;
totAnnProtAnnul float := 0 ;
percAnnProtAnnul float ;
/******************************************************************/
/**************Dati Riepilogativi del Mese*****************************/
/******************************************************************/
/*Dati Generali*/
totMonDoc float := 0 ;
totMonProt float := 0 ;
totMonProtA float := 0 ;
totMonProtP float := 0 ;
totMonProtI float := 0 ;
totMonProtAnnul float := 0 ;
totMonDocGrigi float := 0 ;
totMonDocClass float := 0 ;
/*docs senza docs acq*/
totMonDocProf float := 0 ;
totMonProtClass float := 0 ;
totMonProtAClass float := 0 ;
totMonProtPClass float := 0 ;
totMonProtIClass float := 0 ;
/*Percentuali*/
percMonProt float := 0 ;
percMonProtA float := 0 ;
percMonProtP float := 0 ;
percMonProtI float := 0 ;
percMonProtAnnul float := 0 ;
percMonDocGrigi float := 0 ;
percMonDocClass float := 0 ;
percMonProtClass float := 0 ;
percMonProtAClass float := 0 ;
percMonProtPClass float := 0 ;
percMonProtIClass float := 0 ;
totMonProtAnnulClass float := 0 ;
totMonDocGrigiClass float := 0 ;
/*Dichiarazione delle variabili per i profili (Immagini) *************************************************************************************************/
/*Mensili*/
totMonProf float := 0 ;
totMonProfProt float := 0 ;
totMonProfProtA float := 0 ;
totMonProfProtP float := 0 ;
totMonProfProtI float := 0 ;
totMonProfGrigi float := 0 ;
totMonProfProtAnnul float := 0 ;
/*Annuali*/
totAnnProf float := 0 ;
totAnnProfProt float := 0 ;
totAnnProfProtA float := 0 ;
totAnnProfProtP float := 0 ;
totAnnProfProtI float := 0 ;
totAnnProfGrigi float := 0 ;
totAnnProfProtAnnull float := 0 ;
/*Percentuali*/
PercAnnProfProt float := 0 ;
PercAnnProfProtA float := 0 ;
PercAnnProfProtP float := 0 ;
PercAnnProfProtI float := 0 ;
PercAnnProfGrigi float := 0 ;
PercAnnProfProtAnnull float := 0 ;
TotAnnDocGrigiClass float := 0 ;
percAnnDocGrigiClass float := 0 ;
TotAnnProtAnnulClass float := 0 ;
percAnnProtannulClass float := 0 ;
MESE_VC VARCHAR(255);
i number :=0;
begin
  tsstarttime := CURRENT_TIMESTAMP;
while (i < mese)
loop
i := i + 1;

IF  i = 12 then
    mese_successivo :=       ' and to_date(''01/01/'||(p_anno+1)||''', ''dd/mm/yyyy'') ' ; 
else 
    mese_successivo :=       ' and to_date(''01/'||(i+1)||'/'||p_anno||''', ''dd/mm/yyyy'') ' ;
END IF; 

IF titolario <> 0 THEN 
    condizione_titolario := ' AND p.id_titolario = '||titolario||' ' ;
END IF; 
IF titolario = 0 THEN 
    condizione_titolario := '  '  ;
END IF; 
if  sede is not null and sede <> 'RITORNA_ELAPSED'  
then
-- importante lasciare lo spazio prima di AND !    
    condizione_sede := ' AND profile.var_sede = '''||sede||'''  ';
    condizione_sede_p := '     AND p.var_sede = '''||sede||'''  ';
    
else 
    condizione_sede := ' AND profile.var_sede IS NULL  ';
    condizione_sede_p := '     AND p.var_sede IS NULL  ';
END IF;  
 

-- inizio calcoli
-- primo blocco:    Protocolli        
    istruzioneSQLbase := ' select  sum(undistinct_count)  from mv_prospetti_documenti p  '||
        ' where p.proto_month = to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'') '||
        ' AND p.NUM_ANNO_PROTO = '||p_anno||
        ' AND p.cha_da_proto = ''0''  '||
        ' and p.cha_in_cestino_0fornull = ''0'' '||       
        ' and p.id_registro_0fornull = '||id_reg|| 
        condizione_sede_p  ;
           
    istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''A'' AND p.flag_annullato = 0 and p.flag_NUM_PROTO = 1 ';
    execute immediate istruzioneSQL into totMonProtA;
    totMonProtA := NVL(totMonProtA,0);
    
/*      select count(profile.system_id) into totMonProtP from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno 
        AND profile.cha_tipo_proto = 'P' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/
    istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''P'' AND p.flag_annullato = 0 and p.flag_NUM_PROTO = 1  ';
    execute immediate istruzioneSQL into totMonProtP; 
    totMonProtP := NVL(totMonProtP,0);
     
/*      select count(profile.system_id) into totMonProtI from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO = p_anno 
        AND profile.cha_tipo_proto = 'I' AND profile.cha_da_proto = '0' and profile.id_registro = id_reg AND profile.dta_annulla is null
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/
  istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''I'' AND p.flag_annullato = 0 and p.flag_NUM_PROTO = 1  ';
    execute immediate istruzioneSQL into totMonProtI;
    totMonProtI := NVL(totMonProtI,0) ;  

/*      select count(profile.system_id) into totMonProtAnnul from profile where to_number(to_char(profile.DTA_PROTO,'MM')) = i AND profile.NUM_ANNO_PROTO  = p_anno AND profile.cha_da_proto = '0' and  profile.id_registro = id_reg 
        AND  profile.dta_annulla is not null AND profile.num_proto is not null
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/
    istruzioneSQL := istruzioneSQLbase || 'AND p.flag_annullato = 1 ';
    execute immediate istruzioneSQL into totMonProtAnnul ;
    totMonProtAnnul := NVL(totMonProtAnnul,0); 
  
/*istruzioneSQLbase := ' select count( distinct  profile.system_id) -- into totMonDocGrigi 
from profile,people,dpa_l_ruolo_reg 
where profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo and  profile.author = people.system_id 

        AND dpa_l_ruolo_reg.id_registro = '||id_reg||' 
        and people.id_amm = '||id_ammi||' 
        and profile.num_proto is null 
        AND profile.cha_tipo_proto = ''G''  
        AND ID_DOCUMENTO_PRINCIPALE  IS NULL 
        -- AND to_number(to_char(profile.creation_date,''MM'')) = '||i||' 
        -- AND to_number(to_char(profile.CREATION_DATE,''YYYY'')) = '||p_anno||'
        and PROFILE.creation_date BETWEEN to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'')
                     '||mese_successivo||'
        and nvl(profile.CHA_IN_CESTINO,''0'') <> ''1''  ';
    istruzioneSQL := istruzioneSQLbase || condizione_sede;
*/

istruzioneSQLbase := ' select sum(distinct_count) from MV_PROSPETTI_DOCGRIGI p '||
        ' where p.id_amm = '||id_ammi||
        ' AND p.id_registro = '||id_reg||
        ' and p.creation_month = to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'') ' 
        --' and p.flag_immagine = ''0''   '
        ;
istruzioneSQL := istruzioneSQLbase || condizione_sede_p;
execute immediate istruzioneSQL into totMonDocGrigi  ;
totMonDocGrigi := NVL(totMonDocGrigi,0); 

/*
-- secondo blocco:    doc SENZA immagine 
*/

--mv_profile 
    istruzioneSQLbase := '
    select sum(undistinct_count) from mv_prospetti_documenti p
        where to_number(to_char(p.PROTO_month,''MM'')) = '||i||'
        AND p.NUM_ANNO_PROTO = '||p_anno||'
        AND p.cha_da_proto = ''0''
        and p.cha_in_cestino_0fornull = ''0''
            --AND p.cha_img = ''0'' OBSOLETA
         and p.flag_immagine = ''0''
         and p.id_registro_0fornull = '||id_reg||
        condizione_sede_p;

/*select count(p.system_id) into totMonProfProtA from profile p 
where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno 
and p.cha_tipo_proto = 'A' AND p.cha_da_proto = '0' and p.cha_img = '0' 
and p.id_registro = id_reg AND p.dta_annulla is null
        AND p.var_sede is null and nvl(p.CHA_IN_CESTINO,'0') <> '1';
*/

    istruzioneSQL := istruzioneSQLbase || ' AND p.flag_annullato = 0 and p.cha_tipo_proto = ''A'' ';
    execute immediate istruzioneSQL into totMonProfProtA ;
    totMonProfProtA := NVL(totMonProfProtA,0);

--        select count(p.system_id) into totMonProfProtP from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno
--        and p.cha_tipo_proto = 'P' AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is null
--        AND p.var_sede is null and nvl(p.CHA_IN_CESTINO,'0') <> '1';
    istruzioneSQL := istruzioneSQLbase || ' AND p.flag_annullato = 0 and p.cha_tipo_proto = ''P'' ';
    execute immediate istruzioneSQL into totMonProfProtP ;
    totMonProfProtP := NVL(totMonProfProtP,0) ; 

--        select count(p.system_id) into totMonProfProtI from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i AND p.NUM_ANNO_PROTO = p_anno and p.cha_tipo_proto = 'I' AND p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = id_reg AND p.dta_annulla is null
--        AND p.var_sede is null and nvl(p.CHA_IN_CESTINO,'0') <> '1';
    istruzioneSQL := istruzioneSQLbase || ' AND p.flag_annullato = 0 and p.cha_tipo_proto = ''I'' ';
    execute immediate istruzioneSQL into totMonProfProtI  ;
    totMonProfProtI  := NVL(totMonProfProtI,0);
    
--        select count(p.system_id) into totMonProfProtAnnul from profile p where to_number(to_char(p.DTA_PROTO,'MM')) = i
-- AND p.NUM_ANNO_PROTO = p_anno AND num_proto is not null and p.cha_da_proto = '0' and  p.cha_img = '0'
-- and p.id_registro = id_reg AND p.dta_annulla is not null
--        AND p.var_sede is null and nvl(p.CHA_IN_CESTINO,'0') <> '1';
    istruzioneSQL := istruzioneSQLbase || ' AND p.flag_annullato = 1 AND flag_num_proto = 1';
    execute immediate istruzioneSQL into totMonProfProtAnnul  ;
    totMonProfProtAnnul  := NVL(totMonProfProtAnnul  ,0);

istruzioneSQLbase := ' select sum(distinct_count) from MV_prospetti_DocGrigi p '||
        ' where p.id_amm = '||id_ammi||
        ' AND p.id_registro = '||id_reg||
        ' and p.creation_month = to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'') '|| 
        ' and p.flag_immagine = ''0''   '
        ;
istruzioneSQL := istruzioneSQLbase || condizione_sede_p;
 execute immediate istruzioneSQL into totMonProfGrigi  ;  --grigi senza immagini
    totMonProfGrigi := NVL(totMonProfGrigi,0) ; 
-- terzo blocco :       doc classificati
/*
    -- Protocolli tipo A
        SELECT  COUNT (distinct PROFILE.system_id) into totMonProtAClass
        FROM project_components prc, PROFILE     , project pr
        WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
        and to_number(to_char(profile.DTA_PROTO,'MM')) = i
        AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
        AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy')
                     AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
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
        AND profile.cha_tipo_proto = 'A'  AND profile.dta_annulla is null and num_proto is not null AND PROFILE.dta_proto IS NOT NULL
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/
    istruzioneSQLbase := ' select sum(distinct_COUNT) from  mv_prospetti_documenticlass p '|| 
' where PROTO_month = to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'') '|| 
' AND ( id_registro = '||id_reg||'  OR id_registro IS NULL ) ' 
||condizione_titolario||' 
AND id_amm = '||id_ammi||' 
'||condizione_sede_p||'   
and nvl_CHA_IN_CESTINO = ''0'' ';  

           
 istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''A'' AND p.flag_annullato = 0 '|| ' and flag_num_proto = 1 AND flag_dta_proto = 1 ';
    execute immediate istruzioneSQL into totMonProtAClass;
    totMonProtAClass := NVL(totMonProtAClass,0);  

/*        SELECT  COUNT ((PROFILE.system_id)) into totMonProtPClass
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
                                )) ) 
        AND profile.cha_tipo_proto = 'P'  AND profile.dta_annulla is null and num_proto is not null 
        AND PROFILE.dta_proto IS NOT  NULL
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/
 
istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''P'' AND p.flag_annullato = 0 '||
    ' and flag_num_proto = 1 AND flag_dta_proto = 1   ';
    execute immediate istruzioneSQL into totMonProtPClass; 
    totMonProtPClass := NVL(totMonProtPClass,0);
/*      -- Protocolli tipo I
        SELECT  COUNT ((PROFILE.system_id)) into totMonProtIClass
        FROM project_components prc, PROFILE     , project pr
        WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
        and to_number(to_char(profile.DTA_PROTO,'MM')) = i
        AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
        AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy')    
                    AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
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
        AND profile.cha_tipo_proto = 'I'  AND profile.dta_annulla is null 
        and num_proto is not null
        AND PROFILE.dta_proto IS NOT NULL
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';   
*/         

   
    istruzioneSQL := istruzioneSQLbase || '  AND p.cha_tipo_proto = ''I'' AND p.flag_annullato = 0 '||
    ' and flag_num_proto = 1 AND flag_dta_proto = 1        ';
    
    execute immediate istruzioneSQL into totMonProtIClass; 
    totMonProtIClass := NVL(totMonProtIClass,0);

/*        -- Protocolli annullati
        SELECT  COUNT ((PROFILE.system_id)) into totMonProtAnnulClass
        FROM project_components prc, PROFILE     , project pr
        WHERE prc.LINK = PROFILE.system_id AND prc.project_id = pr.system_id
        and to_number(to_char(profile.DTA_PROTO,'MM')) = i
        AND ( (PROFILE.id_registro = id_reg) OR (PROFILE.id_registro IS NULL))
        AND PROFILE.creation_date BETWEEN to_date('01/01/'||p_anno,'dd/mm/yyyy') 
                            AND to_date('31/12/'||p_anno||' 23:59:59','dd/mm/yyyy HH24:MI:SS')
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
        AND profile.var_sede is null
        AND profile.cha_da_proto = '0' 
        AND profile.dta_annulla is not null 
        and num_proto is not null  and nvl(profile.CHA_IN_CESTINO,'0') <> '1'; 
*/

-- Protocolli annullati

    istruzioneSQL := istruzioneSQLbase || ' AND p.flag_annullato = 1 '
    ||' and flag_num_proto = 1 '    ; 
                    --     AND profile.dta_annulla is not null and num_proto is not null
    execute immediate istruzioneSQL into totMonProtAnnulClass; 
    totMonProtAnnulClass := NVL(totMonProtAnnulClass,0) ;


/*      -- Protocolli tipo Grigi, con id_documento principale = NULL
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
        AND profile.cha_tipo_proto = 'G' and id_documento_principale IS NULL        AND profile.dta_annulla is null 
        AND profile.var_sede is null and nvl(profile.CHA_IN_CESTINO,'0') <> '1';
*/


  /* 
istruzioneSQLbase := 'select count( distinct profile.system_id) -- into totMonDocGrigiClass 
from project_components prc, profile,people,dpa_l_ruolo_reg 
where prc.LINK = PROFILE.system_id  and profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo 
and  profile.author = people.system_id 
        AND dpa_l_ruolo_reg.id_registro = '||id_reg||'
        and people.id_amm = '||id_ammi||' 
        and profile.num_proto is null 
        AND profile.cha_tipo_proto = ''G''  
        AND ID_DOCUMENTO_PRINCIPALE  IS NULL 
        -- AND to_number(to_char(profile.creation_date,''MM'')) = '||i||'  -- AND to_number(to_char(profile.CREATION_DATE,''YYYY'')) = '||p_anno||'
        and PROFILE.creation_date BETWEEN to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'')
                     '||mese_successivo||'
        and nvl(profile.CHA_IN_CESTINO,''0'') <> ''1''  
         AND prc.project_id IN (    SELECT system_id FROM project  WHERE cha_tipo_proj = ''C''  AND id_fascicolo IN (       -----= parentid
                      SELECT system_id FROM project     WHERE cha_tipo_proj = ''F''
                         AND id_parent IN ( -- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                                SELECT     system_id    FROM    project pr
                                     WHERE pr.var_codice IS NOT NULL'||
                                       condizione_titolario||'    
                                       AND pr.id_amm = '||id_ammi||'
                                       AND pr.cha_tipo_proj = ''T''
                                       AND ( pr.id_registro = '||id_reg||' OR pr.id_registro IS NULL)
          -- in questo caso non serve ricostruire la gerarchia tra i nodi
          --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1 
                                ))  
        ) ';
   
*/
istruzioneSQLbase := 'select sum(distinct_count) -- into totMonDocGrigiClass 
from  mv_prospetti_docgrigiclass p 
where P.grigio_month = to_date(''01/'||i||'/'||p_anno||''', ''dd/mm/yyyy'')
and p.nvl_cha_in_cestino <> ''1''    AND p.id_amm = '||id_ammi||
' AND ( p.id_registro = '||id_reg||' OR p.id_registro IS NULL) '
  ||condizione_titolario
  ||condizione_sede_p ; 
  execute immediate istruzioneSQLbase into totMonDocGrigiClass ;     
    totMonDocGrigiClass := NVL(totMonDocGrigiClass,0) ; 
    
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
/*inseriamo i dati mensili in una tabella*/
out_rec.THING := MESE_VC;
out_rec.TOT_DOC := totMonDoc;
out_rec.GRIGI := totMonDocGrigi;


IF SEDE = 'RITORNA_ELAPSED' THEN
-- valorizzo il tempo di FINE esecuzione e calcolo l'elapsed
      tsendtime := CURRENT_TIMESTAMP;      elapsed := tsendtime - tsstarttime ; 
      min_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+4,2) ; --"MI" ; 
      sec_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+7,2) ; --"SS" ; 
      msec_elapsed := SUBSTR(elapsed, INSTR(elapsed,' ')+10,3) ; --"MI" ;

    out_rec.PERC_GRIGI := min_elapsed||'mi '||sec_elapsed||'sec '||msec_elapsed||'msecs';
     tsstarttime := CURRENT_TIMESTAMP;  
ELSE     out_rec.PERC_GRIGI := to_char(percMonDocGrigi);
END IF; 



out_rec.PROT := totMonProt;
out_rec.PERC_PROT := to_char(percMonProt);
out_rec.ANNULL := totMonProtAnnul;
out_rec.PERC_ANNULL := to_char(percMonProtAnnul);
out_rec.ARRIVO := totMonProtA;
out_rec.PERC_ARRIVO := to_char(percMonProtA);
out_rec.PARTENZA := totMonProtP;
out_rec.PERC_PARTENZA := to_char(percMonProtP);
out_rec.INTERNI := totMonProtI;
out_rec.PERC_INTERNI := to_char(percMonProtI);
PIPE ROW(out_rec);
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
out_rec.THING := p_anno;
out_rec.TOT_DOC := totAnnDoc;
out_rec.GRIGI := totAnnDocGrigi;

IF SEDE = 'RITORNA_ELAPSED' THEN 
-- valorizzo il tempo di FINE esecuzione e calcolo l'elapsed
      tsendtime := CURRENT_TIMESTAMP;      elapsed := tsendtime - tsstarttime ; 
      min_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+4,2) ; --"MI" ;
      sec_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+7,2) ; --"SS" ; 
      msec_elapsed := SUBSTR(elapsed, INSTR(elapsed,' ')+10,3) ; --"MI" ;

        out_rec.PERC_GRIGI := min_elapsed||'mi '||sec_elapsed||'sec '||msec_elapsed||'msecs';  
ELSE    out_rec.PERC_GRIGI := to_char(percAnnDocGrigi);

END IF; 



out_rec.PROT := totAnnProt;
out_rec.PERC_PROT := to_char(percAnnProt);
out_rec.ANNULL := totAnnProtAnnul;
out_rec.PERC_ANNULL := percAnnProtAnnul;
out_rec.ARRIVO := totAnnProtA;
out_rec.PERC_ARRIVO := to_char(percAnnProtA);
out_rec.PARTENZA := totAnnProtP;
out_rec.PERC_PARTENZA := to_char(percAnnProtP);
out_rec.INTERNI := totAnnProtI;
out_rec.PERC_INTERNI := to_char(percAnnProtI);
PIPE ROW(out_rec);
/*Inseriamo nella tabella i valori reltivi alla classificazione*/
out_rec.THING := 'Classificati';
out_rec.TOT_DOC := totAnnDocClass;
out_rec.GRIGI := TotAnnDocGrigiClass;

IF SEDE = 'RITORNA_ELAPSED' THEN 
-- valorizzo il tempo di FINE esecuzione e calcolo l'elapsed
      tsendtime := CURRENT_TIMESTAMP;      elapsed := tsendtime - tsstarttime ; 
      min_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+4,2) ; --"MI" ; 
      sec_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+7,2) ; --"SS" ; 
      msec_elapsed := SUBSTR(elapsed, INSTR(elapsed,' ')+10,3) ; --"MI" ;

        out_rec.PERC_GRIGI := min_elapsed||'mi '||sec_elapsed||'sec '||msec_elapsed||'msecs';  
ELSE    out_rec.PERC_GRIGI := percAnnDocGrigiClass;

END IF; 




out_rec.PROT := totAnnProtClass;
out_rec.PERC_PROT := to_char(percAnnProtClass);
out_rec.ANNULL := to_char(TotAnnProtAnnulClass);
out_rec.PERC_ANNULL := to_char(percAnnProtannulClass);
out_rec.ARRIVO := totAnnProtAClass;
out_rec.PERC_ARRIVO := to_char(percAnnProtAClass);
out_rec.PARTENZA := totAnnProtPClass;
out_rec.PERC_PARTENZA := to_char(percAnnProtPClass);
out_rec.INTERNI := totAnnProtIClass;
out_rec.PERC_INTERNI := to_char(percAnnProtIClass);
PIPE ROW(out_rec);
/*Inseriamo nella tabella i valori reltivi alle Immagini - Doc. Fisici Acquisiti -*/
out_rec.THING := 'Senza Img.';
out_rec.TOT_DOC := totAnnProf;
out_rec.GRIGI := totAnnProfGrigi;


IF SEDE = 'RITORNA_ELAPSED' THEN 
-- valorizzo il tempo di FINE esecuzione e calcolo l'elapsed
      tsendtime := CURRENT_TIMESTAMP;      elapsed := tsendtime - tsstarttime ; 
      min_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+4,2) ; --"MI" ; 
      sec_elapsed  := SUBSTR(elapsed, INSTR(elapsed,' ')+7,2) ; --"SS" ; 
      msec_elapsed := SUBSTR(elapsed, INSTR(elapsed,' ')+10,3) ; --"MI" ;

        out_rec.PERC_GRIGI := min_elapsed||'mi '||sec_elapsed||'sec '||msec_elapsed||'msecs';  
ELSE    out_rec.PERC_GRIGI := PercAnnProfGrigi;

END IF; 




out_rec.PROT := totAnnProfProt;
out_rec.PERC_PROT := to_char(PercAnnProfProt);
out_rec.ANNULL := to_char(totAnnProfProtAnnull);
out_rec.PERC_ANNULL := to_char(PercAnnProfProtAnnull);
out_rec.ARRIVO := totAnnProfProtA;
out_rec.PERC_ARRIVO := to_char(PercAnnProfProtA);
out_rec.PARTENZA := totAnnProfProtP;
out_rec.PERC_PARTENZA := to_char(PercAnnProfProtP);
out_rec.INTERNI := totAnnProfProtI;
out_rec.PERC_INTERNI := to_char(PercAnnProfProtI);
PIPE ROW(out_rec);
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
EXCEPTION WHEN OTHERS THEN
out_rec.PROT := totAnnProtClass;
out_rec.PERC_PROT := 'errore SQL';
out_rec.ANNULL := to_char(TotAnnProtAnnulClass);
out_rec.PERC_ANNULL := to_char(percAnnProtannulClass);
out_rec.ARRIVO := totAnnProtAClass;
out_rec.PERC_ARRIVO := to_char(percAnnProtAClass);
out_rec.PARTENZA := totAnnProtPClass;
out_rec.PERC_PARTENZA := to_char(percAnnProtPClass);
out_rec.INTERNI := totAnnProtIClass;
out_rec.PERC_INTERNI := to_char(percAnnProtIClass);
PIPE ROW(out_rec); 

RETURN;

END  AnnualeDocTableFunction;

/
