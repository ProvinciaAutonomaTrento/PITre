--------------------------------------------------------
--  Constraints for Table DPA_CORR_GLOBALI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_CORR_GLOBALI" ADD CONSTRAINT "COD_RUBRICA" UNIQUE ("VAR_COD_RUBRICA", "ID_REGISTRO") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."DPA_CORR_GLOBALI" ADD PRIMARY KEY ("SYSTEM_ID") ENABLE;
