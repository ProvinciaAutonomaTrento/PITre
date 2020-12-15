--------------------------------------------------------
--  Ref Constraints for Table DPA_VIS_TIPO_FASC
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_VIS_TIPO_FASC" ADD FOREIGN KEY ("ID_TIPO_FASC")
	  REFERENCES "ITCOLL_6GIU12"."DPA_TIPO_FASC" ("SYSTEM_ID") ENABLE;
