--------------------------------------------------------
--  Ref Constraints for Table DPA_PASSI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_PASSI" ADD FOREIGN KEY ("ID_STATO")
	  REFERENCES "ITCOLL_6GIU12"."DPA_STATI" ("SYSTEM_ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."DPA_PASSI" ADD FOREIGN KEY ("ID_DIAGRAMMA")
	  REFERENCES "ITCOLL_6GIU12"."DPA_DIAGRAMMI_STATO" ("SYSTEM_ID") ENABLE;
