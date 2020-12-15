--------------------------------------------------------
--  DDL for Procedure COMPUTEATIPICITAINSROLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."COMPUTEATIPICITAINSROLE" 
(
  -- Id dell'amministrazione
  IdAmm IN NUMBER  
  -- Id della uo in cui ? stato inserito il ruolo
, idUo IN NUMBER  
, returnValue OUT NUMBER  
) AS 
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     COMPUTEATIPICITAINSROLE

    PURPOSE:  Store per il calcolo dell'atipicit? di documenti e fascicoli eseguita
              nel momento in cui viene inserito un ruolo

  ******************************************************************************/

  BEGIN
    -- Calcolo dell'atipicit? sui documenti
    vis_doc_anomala_custom(idamm, 'select distinct(s.thing) 
                                    from security s 
                                    where exists (Select ''x'' From profile p Where p.system_id = s.thing)
                                    and exists (
                                      select ''x''
                                      from dpa_corr_globali cg1
                                      where cg1.id_gruppo=s.personorgroup  and cg1.dta_fine is null and id_amm = ' || idAmm || ' AND 
                                      exists 
                                      (
                                        select ''x''
                                        from dpa_corr_globali cg
                                        where cg1.id_uo =  cg.SYSTEM_ID And
                                        cg.CHA_TIPO_URP = ''U'' AND 
                                        cg.dta_fine is null and
                                        cg.ID_AMM = ' || idAmm || '
                                         start with cg.SYSTEM_ID = ' || idUo || '
                                         connect by prior
                                        cg.SYSTEM_ID = cg.ID_PARENT 
                                        ))');
    
   
    -- Calcolo dell'atipicit? sui fascicoli
    vis_fasc_anomala_custom(idAmm, 'select distinct(s.thing) 
                                    from security s 
                                    Where Exists (  Select ''x'' 
                                                    From project p 
                                                    Where p.system_id = s.thing And 
                                                    p.cha_tipo_fascicolo = ''P'')
                                    And Exists (Select ''x'' 
                                      from dpa_corr_globali cg1
                                      where id_amm = ' || idAmm || ' AND cg1.dta_fine is null and personorgroup = cg1.id_gruppo
                                      And Exists 
                                       (
                                        select ''x'' 
                                        from dpa_corr_globali cg where 
                                         cg.CHA_TIPO_URP = ''U'' AND 
                                        cg.ID_AMM = ' || idAmm || ' And cg.dta_fine is null and cg1.id_uo = cg.SYSTEM_ID
                                        start with cg.SYSTEM_ID = ' || idUo ||
                                        ' connect by prior
                                        cg.SYSTEM_ID = cg.ID_PARENT ))');
    
  END;
  
  returnvalue := 0;
  
  END COMPUTEATIPICITAINSROLE;

/
