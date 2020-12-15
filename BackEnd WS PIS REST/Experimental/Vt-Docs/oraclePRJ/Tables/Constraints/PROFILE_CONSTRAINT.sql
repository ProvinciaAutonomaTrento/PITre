--------------------------------------------------------
--  Constraints for Table PROFILE
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PROFILE" ADD CONSTRAINT "INDX_PROFILE_PK" PRIMARY KEY ("SYSTEM_ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."PROFILE" MODIFY ("CHA_FIRMATO" NOT NULL ENABLE);
 
  ALTER TABLE "ITCOLL_6GIU12"."PROFILE" ADD CHECK ("SYSTEM_ID" IS NOT NULL) ENABLE;
