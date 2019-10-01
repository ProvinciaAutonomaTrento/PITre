--------------------------------------------------------
--  Ref Constraints for Table DPA_VIS_TIPO_DOC
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_VIS_TIPO_DOC" ADD FOREIGN KEY ("ID_TIPO_DOC")
	  REFERENCES "ITCOLL_6GIU12"."DPA_TIPO_ATTO" ("SYSTEM_ID") ENABLE;
