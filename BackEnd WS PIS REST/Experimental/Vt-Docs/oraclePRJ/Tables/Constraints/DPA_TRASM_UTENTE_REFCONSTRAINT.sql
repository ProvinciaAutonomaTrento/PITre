--------------------------------------------------------
--  Ref Constraints for Table DPA_TRASM_UTENTE
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_TRASM_UTENTE" ADD CONSTRAINT "FK_DPA_TRASM_UTENTE_R01" FOREIGN KEY ("ID_TRASM_SINGOLA")
	  REFERENCES "ITCOLL_6GIU12"."DPA_TRASM_SINGOLA" ("SYSTEM_ID") ENABLE NOVALIDATE;
