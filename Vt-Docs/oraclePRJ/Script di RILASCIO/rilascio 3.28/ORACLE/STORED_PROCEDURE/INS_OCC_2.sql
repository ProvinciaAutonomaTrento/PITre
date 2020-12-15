
create or replace PROCEDURE INS_OCC_2
 (p_ID_REG in integer,
 p_IDAMM in integer,
 p_Prefix_cod_rub in VARCHAR,
 p_DESC_CORR in VARCHAR,
 p_CHA_DETTAGLI in VARCHAR,
 p_ID_Corr_Globali in VARCHAR,
 p_EMAIL in VARCHAR,
 p_RESULT OUT integer)
 as
 BEGIN
 declare sysid integer;
         id_reg NUMBER;
          idamm NUMBER;
          myprofile NUMBER:=0;
          Countprofilato Number := 0;
          Countprofdoc Number := 0;
          countProfFasc number     := 0;
          new_var_cod_rubrica1 VARCHAR2 (128);
          cod_rubrica VARCHAR2 (128);
 
 BEGIN
 -- verifica preesistenza dell'occ corrente
 -- per sviluppi futuri .....
 /*
 
 select system_id into p_RESULT
 from ( select system_id  from DPA_CORR_GLOBALI
 where UPPER (var_desc_corr) = UPPER (p_DESC_CORR) and  CHA_TIPO_CORR = 'O' AND ID_AMM=P_IDAMM)
 where rownum =1;
 
 
 EXCEPTION WHEN NO_DATA_FOUND THEN
 */
 --inserisco il nuovo occ_
 begin
 select seq.nextval into sysid from dual;
 
 --controlli preliminari
 --verifico se il corrisp  stato utilizzato come dest/mitt di protocolli
 IF(not(p_ID_Corr_Globali = 0))
   then
   BEGIN
     SELECT COUNT (id_profile)
       INTO myprofile
     FROM dpa_doc_arrivo_par
     Where Id_Mitt_Dest = p_ID_Corr_Globali;
 -- verifico se il corrispondente  stato usato o meno nei campi profilati
     Select Count(System_Id) 
       Into Countprofdoc 
     From Dpa_Associazione_Templates 
     where valore_oggetto_db = to_char(p_ID_Corr_Globali);
           
     Select Count(System_Id) 
       Into Countproffasc 
     from dpa_ass_templates_fasc
     where valore_oggetto_db = to_char(p_ID_Corr_Globali);
      
     Countprofilato := Countprofdoc + Countproffasc;
     END;
     END IF;     
 --FINE controlli preliminari
 
   IF(myprofile = 0 AND Countprofilato = 0)
   THEN
   BEGIN
   
     if(p_id_reg=0) then
       
       INSERT INTO
       DPA_CORR_GLOBALI (system_id,ID_REGISTRO,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
       VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
       VALUES (sysid,null,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
       returning system_id into p_RESULT;
       else
       INSERT INTO
       DPA_CORR_GLOBALI (system_id,id_registro,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
       VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
       VALUES (sysid,p_ID_REG,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
       returning system_id into p_RESULT;
       end if;
 
   END;
   
   ELSE
     BEGIN
        Select Var_Cod_Rubrica, id_registro, id_amm                     
        INTO new_var_cod_rubrica1, id_reg, idamm
        FROM dpa_corr_globali
        WHERE system_id = p_ID_Corr_Globali;
        
        -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
       new_var_cod_rubrica1 := new_var_cod_rubrica1 || '_' || TO_CHAR (p_ID_Corr_Globali);
     
         UPDATE DPA_CORR_GLOBALI
           SET DTA_FINE = SYSDATE(),
                         var_cod_rubrica = new_var_cod_rubrica1,
                         var_codice      = new_var_cod_rubrica1,
                         Id_Parent = Null
          WHERE SYSTEM_ID = p_ID_Corr_Globali;
         
         INSERT INTO
         DPA_CORR_GLOBALI (system_id,id_registro,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
         VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
         VALUES (sysid,id_reg,idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,p_ID_Corr_Globali,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
         returning system_id into p_RESULT;
      END;
     END IF;
 
 END;
 END;
 
 end; 
/
