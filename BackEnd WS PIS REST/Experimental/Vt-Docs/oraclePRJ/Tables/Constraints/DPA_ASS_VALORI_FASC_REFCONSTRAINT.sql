--------------------------------------------------------
--  Ref Constraints for Table DPA_ASS_VALORI_FASC
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASS_VALORI_FASC" ADD FOREIGN KEY ("ID_OGGETTO_CUSTOM")
	  REFERENCES "ITCOLL_6GIU12"."DPA_OGGETTI_CUSTOM_FASC" ("SYSTEM_ID") ENABLE;
