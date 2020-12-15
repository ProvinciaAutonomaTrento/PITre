--------------------------------------------------------
--  DDL for Index INDX_OGG_TEXT
--------------------------------------------------------

  CREATE INDEX "ITCOLL_6GIU12"."INDX_OGG_TEXT" ON "ITCOLL_6GIU12"."PROFILE" ("VAR_PROF_OGGETTO") 
   INDEXTYPE IS "CTXSYS"."CONTEXT"  PARAMETERS ('  replace sync (on commit) stoplist ctxsys.empty_stoplist');
