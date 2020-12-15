--------------------------------------------------------
--  DDL for Procedure TMP_DOCUOTABLEFUNCTION
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."TMP_DOCUOTABLEFUNCTION" (anno number,id_reg number,id_amm number)
--RETURN DocUoTableRow PIPELINED
IS
BEGIN
DECLARE
out_doc DocUoTable;
--dichiarazioni  variabili temporanee
ProtA FLOAT;
ProtP FLOAT;
ProtI FLOAT;
ProtAndAnn FLOAT;
ProtNotImg FLOAT;
ProtAndClass FLOAT;
PercProtA float;
PercProtP float;
PercProtI float;
PercProtAndAnn float;
PercProtNotImg float;
PercProtAndClass float;
--dichiarazioni variabili temporanee totali
TOTPROT FLOAT;
TOTPROTA FLOAT;
TOTPROTP FLOAT;
TOTPROTI FLOAT;
TOTPROTANN FLOAT;
TOTPROTPROF FLOAT;
TOTPROTCLASS FLOAT;
PERCTOTPROTA FLOAT;
PERCTOTPROTP FLOAT;
PERCTOTPROTI FLOAT;
PERCTOTPROTANN FLOAT;
PERCTOTPROTPROF FLOAT;
PERCTOTPROTCLASS FLOAT;
--VARIABILI CURSORE
TOT_PROT_UO FLOAT;
ID_UO FLOAT;
VAR_UO VARCHAR (100);

--dicharazione cursore
CURSOR C_UO IS
-- /*+index(PROFILE) index (dpa_corr_globali)*/
select    
count(*) TotProtUO, dpa_corr_globali.system_id,dpa_corr_globali.var_desc_corr
from profile,dpa_corr_globali
where profile.id_registro = id_reg and dpa_corr_globali.id_amm = id_amm
and    profile.num_anno_proto = anno and profile.id_uo_prot = dpa_corr_globali.system_id
group by dpa_corr_globali.var_desc_corr,dpa_corr_globali.system_id;
BEGIN
--setting variabili totali
TOTPROT := 0;
TOTPROTA := 0;
TOTPROTP := 0;
TOTPROTI := 0;
TOTPROTANN := 0;
TOTPROTPROF := 0;
TOTPROTCLASS := 0;
PERCTOTPROTA := 0;
PERCTOTPROTP := 0;
PERCTOTPROTI := 0;
PERCTOTPROTANN := 0;
PERCTOTPROTCLASS := 0;

--inizializzazione dell'output
out_doc := DocUoTable(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);


BEGIN -- CURSORE
--apro il cursore
OPEN C_UO;
LOOP --ciclo del cursore
FETCH C_UO into TOT_PROT_UO,ID_UO,VAR_UO;
EXIT WHEN C_UO%NOTFOUND;
-- BLOCCO MANIPOLAZIONI DATI
--setting variabili temporanee
protA := 0;
ProtP := 0;
ProtI := 0;
ProtAndAnn := 0;
ProtNotImg := 0;
ProtAndClass := 0;
PercProtA := 0;
PercProtP := 0;
PercProtI := 0;
PercProtAndAnn := 0;
PercProtNotImg := 0;
PercProtAndClass := 0;
--conta PROTO A per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtA from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'A'
and profile.id_uo_prot = ID_UO;
--conta PROTO P per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtP from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'P'
and profile.id_uo_prot = ID_UO;
--conta PROTO I per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtI from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'I'
and profile.id_uo_prot = ID_UO;
--conta i DOC Protocollati ed annullati per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtAndAnn from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.num_proto is not null
and profile.dta_annulla is not null and profile.id_uo_prot = ID_UO;
--conta I DOC protocollati SENZA IMG per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtNotImg from profile
where 
Getchaimg(docnumber) ! = '0'
-- profile.cha_img = '0' 
 and profile.cha_da_proto = '0'
and profile.id_registro = id_reg and profile.num_anno_proto = anno
and profile.cha_tipo_proto <> 'G'and profile.id_uo_prot = ID_UO;
--conta I DOC protocollati e classificati per singola uo
select /*+index(PROFILE) */  count(profile.system_id) into ProtAndClass from profile
where profile.id_registro = id_reg AND profile.num_proto is not null
and profile.id_uo_prot = ID_UO and profile.num_anno_proto = anno
and cha_fascicolato = '1';
--CALCOLO LE PERCENTUALI
IF (TOT_PROT_UO <> 0)
THEN
IF (ProtA <> 0)
THEN  --% di protocolli A sul totale dei protocolli
PercProtA := ROUND(((ProtA / TOT_PROT_UO) * 100),2);
END IF;
IF (ProtP <> 0)
THEN --% di protocolli P sul totale dei protocolli
PercProtP := ROUND(((ProtP / TOT_PROT_UO) * 100),2);
END IF;
IF (ProtI <> 0)
THEN --% di protocolli I sul totale dei protocolli
PercProtI := ROUND(((ProtI / TOT_PROT_UO) * 100),2);
END IF;
IF (ProtAndAnn <> 0)
THEN --% di protocolli Annullati sul totale dei protocolli
PercProtAndAnn := ROUND(((ProtAndAnn / TOT_PROT_UO) * 100),2);
END IF;
IF (ProtNotImg <> 0)
THEN --% di protocolli senza Immagine sul totale dei protocolli
PercProtNotImg := ROUND(((ProtNotImg / TOT_PROT_UO) * 100),2);
END IF;
IF (ProtAndClass <> 0)
THEN --% di protocolli senza Immagine sul totale dei protocolli
PercProtAndClass := ROUND(((ProtAndClass / TOT_PROT_UO) * 100),2);
END IF;
END IF;
--assegno i risultati di protocollo prodotti all'oggetto out_doc_prot
out_doc.UO := VAR_UO;
out_doc.TOT_PROT:= TOT_PROT_UO;
out_doc.ARRIVO := ProtA;
out_doc.PERC_ARRIVO := PercProtA;
out_doc.PARTENZA := ProtP;
out_doc.PERC_PARTENZA := PercProtP;
out_doc.INTERNI := ProtI;
out_doc.PERC_INTERNI := PercProtI;
out_doc.ANNULL :=ProtAndAnn;
out_doc.PERC_ANNULL := PercProtAndAnn;
out_doc.PROFILI := ProtNotImg;
out_doc.PERC_PROFILI := PercProtNotImg;
out_doc.CLASSIFICATI := ProtAndClass;
out_doc.PERC_CLASSIFICATI := PercProtAndClass;
--inserimento
--PIPE ROW(out_doc);
--aggiorno variabili totali
TOTPROT := TOTPROT + TOT_PROT_UO;
TOTPROTA := TOTPROTA + ProtA;
TOTPROTP := TOTPROTP + ProtP;
TOTPROTI := TOTPROTI + ProtI;
TOTPROTANN := TOTPROTANN + ProtAndAnn;
TOTPROTPROF := TOTPROTPROF + ProtNotImg;
TOTPROTCLASS := TOTPROTCLASS + ProtAndClass;


dbms_output.put_line ('ProtA: '||ProtA); 
END LOOP;  -- ciclo del cursore

--verifico e calcolo percentuali totali
IF (TOTPROT <> 0)
THEN
IF (TOTPROTA <> 0)
THEN --% di protocolli A sul totale dei protocolli
PERCTOTPROTA := ROUND(((TOTPROTA / TOTPROT) * 100),2);
END IF;
IF (TOTPROTP <> 0)
THEN --% di protocolli P sul totale dei protocolli
PERCTOTPROTP := ROUND(((TOTPROTP / TOTPROT) * 100),2);
END IF;
IF (TOTPROTI <> 0)
THEN --% di protocolli I sul totale dei protocolli
PERCTOTPROTI := ROUND(((TOTPROTI / TOTPROT) * 100),2);
END IF;
IF (TOTPROTANN <> 0)
THEN --% di protocolli ANNULLATI sul totale dei protocolli
PERCTOTPROTANN := ROUND(((TOTPROTANN / TOTPROT) * 100),2);
END IF;
IF (TOTPROTPROF <> 0)
THEN --% di protocolli senza Immagine sul totale dei protocolli
PERCTOTPROTPROF := ROUND(((TOTPROTPROF / TOTPROT) * 100),2);
END IF;
IF (TOTPROTCLASS <> 0)
THEN --% di protocolli senza Immagine sul totale dei protocolli
PERCTOTPROTCLASS := ROUND(((TOTPROTCLASS / TOTPROT) * 100),2);
END IF;
--assegno i risultati di protocollo prodotti all'oggetto out_doc_prot
out_doc.UO := 'TOTALE';
out_doc.TOT_PROT:= TOTPROT;
out_doc.ARRIVO := TOTPROTA;
out_doc.PERC_ARRIVO := PERCTOTPROTA;
out_doc.PARTENZA := TOTPROTP;
out_doc.PERC_PARTENZA := PERCTOTPROTP;
out_doc.INTERNI := TOTPROTI;
out_doc.PERC_INTERNI := PERCTOTPROTI;
out_doc.ANNULL :=TOTPROTANN;
out_doc.PERC_ANNULL := PERCTOTPROTANN;
out_doc.PROFILI := TOTPROTPROF;
out_doc.PERC_PROFILI := PERCTOTPROTPROF;
out_doc.CLASSIFICATI := TOTPROTCLASS;
out_doc.PERC_CLASSIFICATI := PERCTOTPROTCLASS;
--inserimento
--PIPE ROW(out_doc);
END IF;

CLOSE C_UO; --chiudo cursore
RETURN; --return funzione
EXCEPTION WHEN OTHERS THEN
RETURN;
END;
END;
END ; 

/
