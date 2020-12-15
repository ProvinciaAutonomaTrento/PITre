--------------------------------------------------------
--  Ref Constraints for Table DPA_DOC_ARRIVO_PAR
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_DOC_ARRIVO_PAR" ADD CONSTRAINT "DPA_DOC_ARRIVO_PAR_R01" FOREIGN KEY ("ID_MITT_DEST")
	  REFERENCES "ITCOLL_6GIU12"."DPA_CORR_GLOBALI" ("SYSTEM_ID") ENABLE NOVALIDATE;
