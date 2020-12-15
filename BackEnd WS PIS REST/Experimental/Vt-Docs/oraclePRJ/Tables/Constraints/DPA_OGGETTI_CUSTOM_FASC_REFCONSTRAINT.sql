--------------------------------------------------------
--  Ref Constraints for Table DPA_OGGETTI_CUSTOM_FASC
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM_FASC" ADD FOREIGN KEY ("ID_TIPO_OGGETTO")
	  REFERENCES "ITCOLL_6GIU12"."DPA_TIPO_OGGETTO_FASC" ("SYSTEM_ID") ENABLE;
