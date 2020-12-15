--------------------------------------------------------
--  Ref Constraints for Table DPA_STATI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_STATI" ADD FOREIGN KEY ("ID_DIAGRAMMA")
	  REFERENCES "ITCOLL_6GIU12"."DPA_DIAGRAMMI_STATO" ("SYSTEM_ID") ENABLE;
