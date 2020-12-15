--------------------------------------------------------
--  Constraints for Table PROJECT
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PROJECT" ADD CONSTRAINT "INDX_PROJECT_PK" PRIMARY KEY ("SYSTEM_ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."PROJECT" MODIFY ("VAR_CHIAVE_FASC" NOT NULL ENABLE);
 
  ALTER TABLE "ITCOLL_6GIU12"."PROJECT" MODIFY ("CHA_CONSENTI_CLASS" NOT NULL ENABLE);
