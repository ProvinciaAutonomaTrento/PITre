
Begin 
	Utl_Backup_Plsql_Code    ( 'FUNCTION', 'DOCUOTABLEFUNCTION');
end;
/


create or replace
FUNCTION          DOCUOTABLEFUNCTION (anno number,id_reg number,id_amm number)
RETURN DocUoTableRow PIPELINED
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
VAR_UO VARCHAR (256);

--dicharazione cursore
Cursor C_Uo Is
Select   Totprotuo, System_Id,Var_Desc_Corr
from MV_PROSPETTI_R3 
where num_anno_proto = anno ; 
/*select  count(*) TotProtUO, dpa_corr_globali.system_id,dpa_corr_globali.var_desc_corr
from profile,dpa_corr_globali
where profile.id_registro = id_reg and dpa_corr_globali.id_amm = id_amm
And    Profile.Num_Anno_Proto = Anno And Profile.Id_Uo_Prot = Dpa_Corr_Globali.System_Id
group by dpa_corr_globali.var_desc_corr,dpa_corr_globali.system_id;*/
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
/* campi della MV:  
id_registro_0fornull, creation_year,          proto_month, p.num_anno_proto,
p.ID_UO_PROT ,  p.cha_da_proto,  cha_in_cestino_0fornull, p.var_sede, p.cha_tipo_proto
, flag_immagine, flag_annullato, flag_num_proto, undistinct_count */

--conta PROTO A per singola uo
/*select count(profile.system_id) into ProtA from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'A'
and profile.id_uo_prot = ID_UO; */
select sum(undistinct_count) into ProtA from MV_PROSPETTI_DOCUMENTI    
    where id_registro_0fornull = id_reg
    And Num_Anno_Proto = Anno 
    AND cha_da_proto = '0'
    and id_uo_prot = ID_UO
    AND cha_tipo_proto = 'A'
    and flag_num_proto = '1'
    And Flag_Annullato = '0'
    And Cha_In_Cestino_0fornull = '0';
    
ProtA := NVL(ProtA,0); 

--conta PROTO P per singola uo
/*select count(profile.system_id) into ProtP from profile where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'P' and profile.id_uo_prot = ID_UO; */
select sum(undistinct_count) into ProtP from MV_PROSPETTI_DOCUMENTI   
    where id_registro_0fornull = id_reg
    And Num_Anno_Proto = Anno 
    AND cha_da_proto = '0'
    and id_uo_prot = ID_UO
    AND cha_tipo_proto = 'P'
    and flag_num_proto = '1'
    AND flag_annullato = '0'
    And Cha_In_Cestino_0fornull = '0' ;
ProtP := NVL(ProtP,0); 

--conta PROTO I per singola uo
/*select count(profile.system_id) into ProtI from profile where profile.cha_da_proto = '0' and profile.id_registro = id_reg
and profile.num_anno_proto = anno AND profile.cha_tipo_proto = 'I' and profile.id_uo_prot = ID_UO; */
select sum(undistinct_count) into ProtI from MV_PROSPETTI_DOCUMENTI   
    where id_registro_0fornull = id_reg
    And Num_Anno_Proto = Anno 
    AND cha_da_proto = '0'
    and id_uo_prot = ID_UO
    AND cha_tipo_proto = 'I'
    And Flag_Num_Proto = '1'
    AND flag_annullato = '0' And Cha_In_Cestino_0fornull = '0';
ProtI := NVL(ProtI,0); 

--conta i DOC Protocollati ed annullati per singola uo
/*select count(profile.system_id) into ProtAndAnn from profile
where profile.cha_da_proto = '0' and profile.id_registro = id_reg and profile.num_anno_proto = anno 
AND profile.num_proto is not null and profile.dta_annulla is not null and profile.id_uo_prot = ID_UO; */
select sum(undistinct_count) into ProtAndAnn from MV_PROSPETTI_DOCUMENTI   
    where id_registro_0fornull = id_reg
    And Num_Anno_Proto = Anno 
    AND cha_da_proto = '0'
    and id_uo_prot = ID_UO
    and flag_num_proto = '1'
    And Flag_Annullato = '1'
    And Cha_In_Cestino_0fornull = '0';
ProtAndAnn := NVL(ProtAndAnn,0);
 
--conta I DOC protocollati SENZA IMG per singola uo
/* select count(profile.system_id) into ProtNotImg from profile
where profile.cha_img = '0' and profile.cha_da_proto = '0'
and profile.id_registro = id_reg and profile.num_anno_proto = anno
and profile.cha_tipo_proto <> 'G'and profile.id_uo_prot = ID_UO; */
select sum(undistinct_count) into ProtNotImg from MV_PROSPETTI_DOCUMENTI   
    where id_registro_0fornull = id_reg
    and num_anno_proto = anno
    and id_uo_prot = ID_UO
    And Cha_Tipo_Proto <> 'G'
    and flag_immagine = '0'    And Cha_In_Cestino_0fornull = '0';
ProtNotImg := nvl(ProtNotImg,0); 

--conta I DOC protocollati e classificati per singola uo
/*select count(profile.system_id) into ProtAndClass from profile
    where profile.id_registro = id_reg AND profile.num_proto is not null
    and profile.id_uo_prot = ID_UO and profile.num_anno_proto = anno
    and cha_fascicolato = '1'; */
   
/*campi della     MV_PROSPETTI_DOCUMENTICLASSUO
p.id_registro ,id_uo_prot, p.cha_tipo_proto, num_anno_proto
, nvl_cha_in_cestino , flag_annullato , undistinct_count , distinct_count */  
select sum(distinct_count) into ProtAndClass from MV_PROSPETTI_DOCUMENTICLASSUO
    where id_registro = id_reg  --AND profile.num_proto is not null -- condizione incorporata nella MV
    and id_uo_prot = ID_UO and num_anno_proto = anno     ;
ProtAndClass := nvl(ProtAndClass,0);     


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
PIPE ROW(out_doc);
--aggiorno variabili totali
TOTPROT := TOTPROT + TOT_PROT_UO;
TOTPROTA := TOTPROTA + ProtA;
TOTPROTP := TOTPROTP + ProtP;
TOTPROTI := TOTPROTI + ProtI;
TOTPROTANN := TOTPROTANN + ProtAndAnn;
TOTPROTPROF := TOTPROTPROF + ProtNotImg;
TOTPROTCLASS := TOTPROTCLASS + ProtAndClass;
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
PIPE ROW(out_doc);
END IF;

CLOSE C_UO; --chiudo cursore
RETURN; --return funzione
EXCEPTION WHEN OTHERS THEN
out_doc.UO := SQLERRM;
out_doc.TOT_PROT:= length(VAR_UO);
PIPE ROW(out_doc);

END;
END;
END DocUoTableFunction;
/