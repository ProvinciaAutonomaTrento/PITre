--------------------------------------------------------
--  Ref Constraints for Table DPA_OGG_CUSTOM_COMP
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_OGG_CUSTOM_COMP" ADD FOREIGN KEY ("ID_OGG_CUSTOM")
	  REFERENCES "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM" ("SYSTEM_ID") ENABLE;
