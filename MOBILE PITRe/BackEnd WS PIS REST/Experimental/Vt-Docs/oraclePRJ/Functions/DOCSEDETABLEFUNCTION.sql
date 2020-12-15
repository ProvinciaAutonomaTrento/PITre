--------------------------------------------------------
--  DDL for Function DOCSEDETABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DOCSEDETABLEFUNCTION" (anno number,p_id_registro number,id_ammi number)
RETURN DocSedeTableRow PIPELINED IS
--inizializzazione dei tipi (documenti protocollati,classificati,profili)
out_doc_prot DocSedeTable := DocSedeTable(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
out_doc_class DocSedeTable := DocSedeTable(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
out_doc_prof DocSedeTable := DocSedeTable(NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
--dichiarazioni  variabili
totDoc FLOAT;
totGrigi FLOAT;
totProt FLOAT;
totProtA FLOAT;
totProtP FLOAT;
totProtI FLOAT;
totProtClear FLOAT;
totClass FLOAT;
totClassGrigi FLOAT;
totClassProt float;
totClassProtA float;
totClassProtP float;
totClassProtI float;
totClassProtClear float;
totProf FLOAT;
totProfGrigi FLOAT;
totProfProt FLOAT;
totProfProtA FLOAT;
totProfProtP FLOAT;
totProfProtI FLOAT;
totProfProtClear FLOAT;
PercGrigi float;
PercProt float;
PercProtA float;
PercProtP float;
PercProtI float;
PercProtClear float;
PercClass float;
PercClassGrigi float;
PercClassProt float;
PercClassProtA float;
PercClassProtP float;
PercClassProtI float;
PercClassProtClear float;
PercProf FLOAT;
PercProfGrigi FLOAT;
PercProfProt FLOAT;
PercProfProtA FLOAT;
PercProfProtP FLOAT;
PercProfProtI FLOAT;
PercProfProtClear FLOAT;
--variabile del cursore
v_VAR_SEDE VARCHAR (255);
--dicharazione cursore
CURSOR C_VAR_SEDE IS
SELECT DISTINCT (VAR_SEDE) FROM PROFILE WHERE VAR_SEDE IS NOT NULL;
BEGIN -- CURSORE

--end setting
--apro il cursore
OPEN C_VAR_SEDE;
LOOP --ciclo del cursore
FETCH C_VAR_SEDE into v_VAR_SEDE;
EXIT WHEN C_VAR_SEDE%NOTFOUND;

--setting delle variabili
totDoc :=0;
totGrigi :=0;
totProt :=0;
totProtA :=0;
totProtP :=0;
totProtI :=0;
totProtClear :=0;
totClass :=0;
totClassGrigi :=0;
totClassProt :=0;
totClassProtA :=0;
totClassProtP :=0;
totClassProtI :=0;
totClassProtClear :=0;
totProf :=0;
totProfGrigi :=0;
totProfProt :=0;
totProfProtA :=0;
totProfProtP :=0;
totProfProtI :=0;
totProfProtClear :=0;
PercGrigi :=0;
PercProt :=0;
PercProtA :=0;
PercProtP :=0;
PercProtI :=0;
PercProtClear :=0;
PercClass :=0;
PercClassGrigi :=0;
PercClassProt :=0;
PercClassProtA :=0;
PercClassProtP :=0;
PercClassProtI :=0;
PercClassProtClear :=0;
PercProf :=0;
PercProfGrigi :=0;
PercProfProt :=0;
PercProfProtA :=0;
PercProfProtP :=0;
PercProfProtI :=0;
PercProfProtClear :=0;


-- conto i doc grigi della sede passata dal cursore (flitro @var_sede ,@anno, @id_registro)
select sum(undistinct_count) into totGrigi  from MV_PROSPETTI_DOCGRIGI mv 
where mv.id_amm = id_ammi  
 --and TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY')) = anno 
 and mv.CREATION_MONTH between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
 and mv.var_sede = v_VAR_SEDE;   

/*select count(profile.system_id) into totGrigi from profile,people
where profile.author = people.system_id and people.id_amm = id_ammi and num_proto is null AND cha_tipo_proto = 'G' and id_documento_principale is null and
TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY')) = anno and (profile.var_sede = v_VAR_SEDE);      */



--  conto i doc protocollati (Annullati) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro)
select sum(undistinct_count) into totProtClear from MV_PROSPETTI_DOCUMENTI mv
where mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 1 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE;  

/*select count(system_id) into totProtClear from profile where cha_da_proto = '0' and  id_registro = p_id_registro
AND  dta_annulla is not null AND num_proto is not null AND NUM_ANNO_PROTO = anno AND ( profile.var_sede = v_VAR_SEDE); */

-- conto i doc Protocollati (Arrivo) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select sum(undistinct_count) into totProtA from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'A'
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(profile.system_id) into totProtA from profile where profile.cha_da_proto = '0' and profile.id_registro = p_id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'A' AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede  = v_VAR_SEDE);  */



-- conto i doc Protocollati (Partenza) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select sum(undistinct_count) into totProtP from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'P'
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(profile.system_id)into totProtP from profile where profile.cha_da_proto = '0' and profile.id_registro = p_id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'P' AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede  = v_VAR_SEDE); */



-- conto i doc Protocollati (Interni) della sede passata dal cursore (flitro @var_sede ,@anno,@id_registro,tipo)
select sum(undistinct_count) into totProtP from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'I'
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(profile.system_id) into totProtI from profile where profile.cha_da_proto = '0' and profile.id_registro = p_id_registro AND dta_annulla is null
AND profile.cha_tipo_proto = 'I' AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede  = v_VAR_SEDE); */


totProt := totProtA + totProtP +totProtI + totProtClear;

totDoc := totGrigi + totProt;

----CALCOLO PERCENTUALI  --
if ((totDoc <> 0) and (totGrigi <> 0 )) then -- % doc grigi
PercGrigi := ROUND(((totGrigi / totDoc) * 100),2);
end if;
if ((totDoc <> 0) and (totProt <> 0 )) then -- % doc protocollati
PercProt:= ROUND(((totProt / totDoc) * 100),2);
end if;
if ((totProtClear <> 0) and (totProt <> 0 )) then  -- % doc protocollati Annullati
PercProtClear := ROUND(((totProtClear / totProt) * 100),2);
end if;
if ((totProtA <> 0) and (totProt <> 0 )) then -- % doc protocollati Arrivo
PercProtA := ROUND(((totProtA / totProt) * 100),2);
end if;
if ((totProtP <> 0) and (totProt <> 0 )) then -- % doc protocollati Partenza
PercProtP := ROUND(((totProtP / totProt) * 100),2);
end if;
if ((totProtI <> 0) and (totProt <> 0 )) then -- % doc protocollati Interni
PercProtI := ROUND(((totProtI / totProt) * 100),2);
end if;
--assegno i risultati di protocollo prodotti all'oggetto out_doc_prot
out_doc_prot.ANNO := 'Creati';
out_doc_prot.SEDE := v_VAR_SEDE;
out_doc_prot.TOT_DOC := totDoc;
out_doc_prot.GRIGI := totGrigi;
out_doc_prot.PERC_GRIGI := PercGrigi;
out_doc_prot.PROT := totProt;
out_doc_prot.PERC_PROT := PercProt;
out_doc_prot.ANNULL := totProtClear;
out_doc_prot.PERC_ANNULL := PercProtClear;
out_doc_prot.ARRIVO := totProtA;
out_doc_prot.PERC_ARRIVO := PercProtA;
out_doc_prot.PARTENZA := totProtP;
out_doc_prot.PERC_PARTENZA := PercProtP;
out_doc_prot.INTERNI := totProtI;
out_doc_prot.PERC_INTERNI := PercProtI;
--end assegnazione
--inserimento
PIPE ROW(out_doc_prot);
--end inserimento

-- conto i doc grigi classificati
select sum(undistinct_count) into totClassGrigi   from MV_PROSPETTI_DOCGRIGICLASS mv
where mv.id_amm = id_ammi  
 --and TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY')) = anno 
 and mv.grigio_month between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
 and mv.var_sede = v_VAR_SEDE;  

/*select count(profile.system_id) into totClassGrigi from profile,people
where profile.author = people.system_id and people.id_amm = id_ammi
AND profile.cha_fascicolato = '1' and profile.num_proto is null AND profile.cha_tipo_proto = 'G'  
and id_documento_principale is null
AND TO_NUMBER(TO_CHAR(PROFILE.CREATION_DATE,'YYYY')) = anno AND (profile.var_sede = v_VAR_SEDE); */


--conto i doc classificati e protocollati A 
select sum(not_distinct_count) into totClassProtA from MV_PROSPETTI_DOCUMENTICLASS mv
where flag_num_proto = 1 
and  mv.id_registro = p_id_registro 
AND mv.cha_tipo_proto = 'A' AND  mv.flag_annullato = 0 
and mv.proto_month between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
 and mv.var_sede = v_VAR_SEDE;

/*select count(profile.system_id) into totClassProtA from profile where profile.cha_da_proto = '0'
and  profile.id_registro = p_id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'A' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede = v_VAR_SEDE);  */


--conto i doc classificati e protocollati P
select sum(not_distinct_count) into totClassProtP from MV_PROSPETTI_DOCUMENTICLASS mv
where flag_num_proto = 1 
and  mv.id_registro = p_id_registro 
AND mv.cha_tipo_proto = 'P' AND  mv.flag_annullato = 0 
and mv.proto_month between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE;

/*select count(profile.system_id) into totClassProtP from profile where profile.cha_da_proto = '0'
and  profile.id_registro = p_id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'P' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede = v_VAR_SEDE); */


--conto i doc classificati e protocollati I
select sum(not_distinct_count) into totClassProtI from MV_PROSPETTI_DOCUMENTICLASS mv
where flag_num_proto = 1 and  mv.id_registro = p_id_registro 
AND mv.cha_tipo_proto = 'I' AND  mv.flag_annullato = 0 
and mv.proto_month between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE;

/*select count(profile.system_id) into totClassProtI from profile where profile.cha_da_proto = '0'
and  profile.id_registro = p_id_registro AND profile.cha_fascicolato = '1'
AND profile.num_proto is not null AND profile.cha_tipo_proto = 'I' AND  profile.dta_annulla is null
AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede = v_VAR_SEDE); */

--conto i doc classificati e annullati
select sum(not_distinct_count) into totClassProtI from MV_PROSPETTI_DOCUMENTICLASS mv
where flag_num_proto = 1 and  mv.id_registro = p_id_registro 
AND  mv.flag_annullato = 1 
and mv.proto_month between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE;

/*select count (profile.system_id) into totClassProtClear from profile where profile.cha_da_proto = '0'
and  profile.id_registro = p_id_registro AND profile.cha_fascicolato = '1'
AND  profile.dta_annulla is not null AND profile.num_proto is not null
AND profile.NUM_ANNO_PROTO = anno AND (profile.var_sede = v_VAR_SEDE); */

totClassProt := totClassProtA + totClassProtP +totClassProtI + totClassProtClear;

totClass := totClassGrigi + totClassProt;


----CALCOLO PERCENTUALI  --
if ((totDoc <> 0) and (totClass <> 0 )) then -- % doc classificati
PercClass:= ROUND(((totClass / totDoc) * 100),2);
end if;
if ((totClassGrigi <> 0) and (totClass <> 0 )) then -- % doc grigi e classificati
PercClassGrigi := ROUND(((totClassGrigi / totClass) * 100),2);
end if;
if ((totClassProt <> 0) and (totClass <> 0 )) then -- % doc protocollati e classificati
PercClassProt := ROUND(((totClassProt /totClass) * 100),2);
end if;
if ((totClassProtClear <> 0) and (totClassProt <> 0 )) then-- % doc protocollati classificati ed annullati
PercClassProtClear := ROUND(((totClassProtClear / totClassProt) * 100),2);
end if;
if ((totClassProtA <> 0) and (totClassProt <> 0 )) then -- % doc protocollati Arrivo classificiati
PercClassProtA := ROUND(((totClassProtA / totClassProt) * 100),2);
end if;
if ((totClassProtP <> 0) and (totClassProt <> 0 )) then-- % doc protocollati Partenza e classificati
PercClassProtP := ROUND(((totClassProtP / totClassProt) * 100),2);
end if;
if ((totClassProtI <> 0) and (totClassProt <> 0 )) then -- % doc protocollati Partenza e classificati
PercClassProtI := ROUND(((totClassProtI / totClassProt) * 100),2);
end if;
--assegno i risultati di protocollo prodotti all'oggetto out_doc_class
out_doc_class.ANNO := 'Classificati';
out_doc_class.SEDE := v_VAR_SEDE;
out_doc_class.TOT_DOC := TotClass;
out_doc_class.GRIGI := totClassGrigi;
out_doc_class.PERC_GRIGI := PercClassGrigi;
out_doc_class.PROT := totClassProt;
out_doc_class.PERC_PROT := PercClassProt;
out_doc_class.ANNULL := totClassProtClear;
out_doc_class.PERC_ANNULL := PercClassProtClear;
out_doc_class.ARRIVO := totClassProtA;
out_doc_class.PERC_ARRIVO := PercClassProtA;
out_doc_class.PARTENZA := totClassProtP;
out_doc_class.PERC_PARTENZA := PercClassProtP;
out_doc_class.INTERNI := totClassProtI;
out_doc_class.PERC_INTERNI := PercClassProtI;
--end assegnazione
--inserimento
PIPE ROW(out_doc_class);
--end inserimento


select sum(undistinct_count) into totProfGrigi  from MV_PROSPETTI_DOCGRIGI mv 
where mv.id_amm = id_ammi and flag_immagine = 0   
 --and TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY')) = anno 
 and mv.CREATION_MONTH between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                        and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
 and mv.var_sede = v_VAR_SEDE;   

/*select count(profile.system_id) into totProfGrigi from profile,people
where profile.author = people.system_id and people.id_amm = id_ammi
and profile.cha_img = '0' and TO_NUMBER(TO_CHAR(profile.CREATION_DATE,'YYYY')) = anno and profile.cha_tipo_proto = 'G'  and id_documento_principale is null AND (profile.var_sede = v_VAR_SEDE);
*/


-- conto i doc protocollati  A senza doc acquisiti, come prima ma con flag_immagine = 0 
select sum(undistinct_count) into totProfProtA from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'A' and mv.flag_immagine = 0 
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(p.system_id) into totProfProtA from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = p_id_registro AND p.NUM_ANNO_PROTO = anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'A' AND (p.var_sede = v_VAR_SEDE); */


-- conto i doc protocollati P senza doc acquisiti, con flag_immagine
select sum(undistinct_count) into totProfProtP from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'P' and mv.flag_immagine = 0 
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(p.system_id) into totProfProtP from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = p_id_registro AND p.NUM_ANNO_PROTO = anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'P' AND (p.var_sede = v_VAR_SEDE); */

-- conto i doc protocollati I senza doc acquisiti, con flag_immagine
select sum(undistinct_count) into totProfProtI from MV_PROSPETTI_DOCUMENTI mv
where mv.CHA_TIPO_PROTO = 'I' and mv.flag_immagine = 0 
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 0 and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(p.system_id) into totProfProtI from profile p
where p.cha_da_proto = '0' and p.cha_img = '0' and p.id_registro = p_id_registro AND p.NUM_ANNO_PROTO = anno AND p.dta_annulla is null
and p.cha_tipo_proto = 'I' AND (p.var_sede = v_VAR_SEDE); */

-- conto i doc protocollati  Annullati senza doc acquisiti, con flag_immagine
select sum(undistinct_count) into totProfProtI from MV_PROSPETTI_DOCUMENTI mv
where mv.flag_immagine = 0 
and mv.id_registro_0fornull = p_id_registro
and cha_da_proto = '0' and flag_annullato = 1 
and flag_num_proto = 1 
and mv.proto_month    between to_date('01/01/'||anno||' 00:00:00','mm/dd/yyyy hh24:mi:ss') 
                       and    to_date('31/12/'||anno||' 23:59:59','mm/dd/yyyy hh24:mi:ss') 
and mv.var_sede = v_VAR_SEDE; 

/*select count(profile.system_id) into totProfProtClear from profile
 where profile.cha_da_proto = '0' and  profile.id_registro = p_id_registro
AND profile.dta_annulla is not null AND profile.num_proto is not null and profile.cha_img = '0' and profile.NUM_ANNO_PROTO = anno AND (profile.var_sede = v_VAR_SEDE);
*/

totProfProt := totProfProtA + totProfProtP + totProfProtI + totProfProtClear;
totProf := totProfGrigi + totProfProt;

-- calcolo percentuali --
if ((totDoc <> 0) and (totProf <> 0 )) then -- %  profili
PercProf := ROUND(((totProf / totDoc) * 100),2);
end if;
if ((totProfGrigi <> 0) and (totProf <> 0 )) then-- % profili doc grigi
PercProfGrigi := ROUND(((totProfGrigi / totProf) * 100),2);
end if;
if ((totProfProt <> 0) and (totProf <> 0 )) then -- % profili protocollati
PercProfProt := ROUND(((totProfProt / totProf) * 100),2);
end if;
if ((totProfProtClear <> 0) and (totProfProt <> 0 )) then -- % profili protocollati ed annullati
PercProfProtClear  := ROUND(((totProfProtClear / totProfProt) * 100),2);
end if;
if ((totProfProtA <> 0) and (totProfProt <> 0 )) then-- % profili protocollati A
PercProfProtA := ROUND(((totProfProtA / totProfProt) * 100),2);
end if;
if ((totProfProtP <> 0) and (totProfProt <> 0 )) then-- % profili protocollati P
PercProfProtP := ROUND(((totProfProtP / totProfProt) * 100),2);
end if;
if ((totProfProtI <> 0) and (totProfProt <> 0 )) then -- % profili protocollati I
PercProfProtI := ROUND(((totProfProtI / totProfProt) * 100),2);
end if;
--assegno i risultati di protocollo prodotti all'oggetto out_doc_class
out_doc_prof.ANNO := 'Senza Img.';
out_doc_prof.SEDE := v_VAR_SEDE;
out_doc_prof.TOT_DOC := totProf;
out_doc_prof.GRIGI := totProfGrigi;
out_doc_prof.PERC_GRIGI := PercProfGrigi;
out_doc_prof.PROT := totProfProt;
out_doc_prof.PERC_PROT := PercProfProt;
out_doc_prof.ANNULL := totProfProtClear;
out_doc_prof.PERC_ANNULL := PercProfProtClear;
out_doc_prof.ARRIVO := totProfProtA;
out_doc_prof.PERC_ARRIVO := PercProfProtA;
out_doc_prof.PARTENZA := totProfProtP;
out_doc_prof.PERC_PARTENZA := PercProfProtP;
out_doc_prof.INTERNI := totProfProtI;
out_doc_prof.PERC_INTERNI := PercProfProtI;
--end assegnazione
--inserimento
PIPE ROW(out_doc_prof);
--end inserimento
-- RESET DELLE VARIABILI
--end blocco manipolazione dati

END LOOP;  -- ciclo del cursore

CLOSE C_VAR_SEDE; --chiudo cursore
RETURN; --return funzione

EXCEPTION WHEN OTHERS THEN
RETURN;
END DocSedeTableFunction; -- end funzione 

/
