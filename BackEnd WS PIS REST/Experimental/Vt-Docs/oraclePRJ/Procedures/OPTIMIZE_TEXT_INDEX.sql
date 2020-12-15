--------------------------------------------------------
--  DDL for Procedure OPTIMIZE_TEXT_INDEX
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."OPTIMIZE_TEXT_INDEX" IS
BEGIN
CTX_DDL.OPTIMIZE_INDEX('INDX_OGG_TEXT','FULL');
CTX_DDL.OPTIMIZE_INDEX('INDX_VAR_DESC_CORR_TEXT','FULL');
EXCEPTION
WHEN OTHERS THEN
RAISE;

END optimize_text_index; 

/
