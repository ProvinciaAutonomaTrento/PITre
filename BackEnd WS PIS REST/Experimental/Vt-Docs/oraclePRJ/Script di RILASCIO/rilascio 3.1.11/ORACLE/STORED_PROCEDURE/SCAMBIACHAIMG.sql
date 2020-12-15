create or replace
PROCEDURE scambiachaimg (
   p_idprincipale         NUMBER,
   p_idallegato           NUMBER,
   returnvalue      OUT   NUMBER
)
IS
BEGIN
   DECLARE
      cha_principale   NUMBER;
      cha_allegato     NUMBER;
      ext_principale   VARCHAR(2000);
      ext_allegato   VARCHAR(2000);
   BEGIN
      cha_principale := 0;
      cha_allegato := 0;
      returnvalue := 0;

      <<reperimento_cha_principale>>
      BEGIN
         SELECT cha_img, ext
           INTO cha_principale, ext_principale
           FROM PROFILE
          WHERE docnumber = p_idprincipale;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            returnvalue := 1;
            RETURN;
      END reperimento_cha_principale;
      
      dbms_output.PUT_LINE('Cha_principale: '||cha_principale );

      <<reperimento_cha_allegato>>
      BEGIN
         SELECT cha_img, ext
           INTO cha_allegato, ext_allegato
           FROM PROFILE
          WHERE docnumber = p_idallegato;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            returnvalue := 2;
            RETURN;
      END reperimento_cha_allegato;

      dbms_output.PUT_LINE('Cha_allegato: ' || cha_allegato);
      
      IF (cha_principale <> cha_allegato or ext_principale <> ext_allegato)
      THEN
         BEGIN

            <<update_profile_principale>>
            BEGIN
               UPDATE PROFILE
                  SET cha_img = cha_allegato, ext = ext_allegato
                WHERE system_id = p_idprincipale;
            EXCEPTION
               WHEN OTHERS
               THEN
                  returnvalue := 3;
                  RETURN;
            END update_profile_principale;
            dbms_output.PUT_LINE('Aggiornata profile principale');

            <<update_profile_allegato>>
            BEGIN
               UPDATE PROFILE
                  SET cha_img = cha_principale, ext = ext_principale
                WHERE system_id = p_idallegato;
            EXCEPTION
               WHEN OTHERS
               THEN
                  returnvalue := 4;
                  RETURN;
            END update_profile_allegato;
            dbms_output.PUT_LINE('Aggiornata profile allegato');
         END;
      END IF;
   END;
END;