--------------------------------------------------------
--  Ref Constraints for Table DPA_ASSOCIAZIONE_VALORI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASSOCIAZIONE_VALORI" ADD FOREIGN KEY ("ID_OGGETTO_CUSTOM")
	  REFERENCES "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM" ("SYSTEM_ID") ENABLE;
