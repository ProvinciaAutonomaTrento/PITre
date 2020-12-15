--------------------------------------------------------
--  DDL for Procedure DELETEREGISTROREPERTORIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DELETEREGISTROREPERTORIO" (
  -- Id del contatore
  countId Number
) As
Begin
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     DeleteRegistroRepertorio
    
    PURPOSE:  Store per l'eliminazione di un registro di repertorio nell'anagrafica            

  ******************************************************************************/
  Delete From dpa_registri_repertorio Where CounterId = countId;
    
End DeleteRegistroRepertorio;

/
