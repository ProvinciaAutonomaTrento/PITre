--------------------------------------------------------
--  Ref Constraints for Table DPA_DATA_ARRIVO_STO
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_DATA_ARRIVO_STO" ADD FOREIGN KEY ("ID_PEOPLE")
	  REFERENCES "ITCOLL_6GIU12"."PEOPLE" ("SYSTEM_ID") ENABLE;
