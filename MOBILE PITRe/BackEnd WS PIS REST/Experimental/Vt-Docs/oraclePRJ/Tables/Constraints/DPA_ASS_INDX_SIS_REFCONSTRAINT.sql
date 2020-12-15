--------------------------------------------------------
--  Ref Constraints for Table DPA_ASS_INDX_SIS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASS_INDX_SIS" ADD FOREIGN KEY ("ID_INDICE_SIS")
	  REFERENCES "ITCOLL_6GIU12"."DPA_INDX_SIS" ("SYSTEM_ID") ENABLE;
