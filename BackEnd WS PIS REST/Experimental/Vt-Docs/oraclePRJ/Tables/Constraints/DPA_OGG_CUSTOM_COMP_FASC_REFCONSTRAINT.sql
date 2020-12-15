--------------------------------------------------------
--  Ref Constraints for Table DPA_OGG_CUSTOM_COMP_FASC
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_OGG_CUSTOM_COMP_FASC" ADD FOREIGN KEY ("ID_OGG_CUSTOM")
	  REFERENCES "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM_FASC" ("SYSTEM_ID") ENABLE;
