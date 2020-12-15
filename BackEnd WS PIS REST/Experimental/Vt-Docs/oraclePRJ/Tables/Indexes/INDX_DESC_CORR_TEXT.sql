--------------------------------------------------------
--  DDL for Index INDX_DESC_CORR_TEXT
--------------------------------------------------------

  CREATE INDEX "ITCOLL_6GIU12"."INDX_DESC_CORR_TEXT" ON "ITCOLL_6GIU12"."DPA_CORR_GLOBALI" ("VAR_DESC_CORR") 
   INDEXTYPE IS "CTXSYS"."CONTEXT"  PARAMETERS ('  replace sync (on commit) stoplist ctxsys.empty_stoplist');
