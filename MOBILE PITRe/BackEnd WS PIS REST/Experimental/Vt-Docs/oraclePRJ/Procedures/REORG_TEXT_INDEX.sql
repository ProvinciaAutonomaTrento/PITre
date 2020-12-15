--------------------------------------------------------
--  DDL for Procedure REORG_TEXT_INDEX
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."REORG_TEXT_INDEX" IS
BEGIN
CTX_DDL.SYNC_INDEX('INDX_OGG_TEXT','30M');
CTX_DDL.SYNC_INDEX('INDX_VAR_DESC_CORR_TEXT','30M');

EXCEPTION
WHEN OTHERS THEN
NULL;
RAISE;
END reorg_text_index; 

/
