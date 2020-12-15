--------------------------------------------------------
--  Ref Constraints for Table DPA_AMMINISTRA
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_AMMINISTRA" ADD CONSTRAINT "FK_AMM_ID_DISPOSITIVO_STAMPA" FOREIGN KEY ("ID_DISPOSITIVO_STAMPA")
	  REFERENCES "ITCOLL_6GIU12"."DPA_DISPOSITIVI_STAMPA" ("ID") ON DELETE SET NULL ENABLE NOVALIDATE;
