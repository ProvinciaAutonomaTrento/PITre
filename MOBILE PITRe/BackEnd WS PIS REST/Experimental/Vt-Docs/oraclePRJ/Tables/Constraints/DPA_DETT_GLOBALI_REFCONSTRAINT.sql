--------------------------------------------------------
--  Ref Constraints for Table DPA_DETT_GLOBALI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_DETT_GLOBALI" ADD FOREIGN KEY ("ID_QUALIFICA_CORR")
	  REFERENCES "ITCOLL_6GIU12"."DPA_QUALIFICA_CORRISPONDENTE" ("SYSTEM_ID") ENABLE;
