--------------------------------------------------------
--  Ref Constraints for Table PGU_ENTI_UTENTI
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PGU_ENTI_UTENTI" ADD FOREIGN KEY ("IDENTE")
	  REFERENCES "ITCOLL_6GIU12"."PGU_ENTI" ("ID") ON DELETE CASCADE ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."PGU_ENTI_UTENTI" ADD FOREIGN KEY ("IDUTENTE")
	  REFERENCES "ITCOLL_6GIU12"."PGU_UTENTI" ("ID") ON DELETE CASCADE ENABLE;
