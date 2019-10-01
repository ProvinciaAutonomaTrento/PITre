--------------------------------------------------------
--  Ref Constraints for Table DPA_ASSOCIAZIONE_TEMPLATES
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASSOCIAZIONE_TEMPLATES" ADD FOREIGN KEY ("ID_OGGETTO")
	  REFERENCES "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM" ("SYSTEM_ID") ENABLE;
