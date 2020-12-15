--------------------------------------------------------
--  Ref Constraints for Table DPA_OGGETTI_CUSTOM
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM" ADD FOREIGN KEY ("ID_TIPO_OGGETTO")
	  REFERENCES "ITCOLL_6GIU12"."DPA_TIPO_OGGETTO" ("SYSTEM_ID") ENABLE;
