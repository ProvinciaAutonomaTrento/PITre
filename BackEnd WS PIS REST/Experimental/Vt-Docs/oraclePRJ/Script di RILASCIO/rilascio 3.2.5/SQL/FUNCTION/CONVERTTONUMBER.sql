ALTER FUNCTION CONVERTTONUMBER(@p_string VARCHAR(12))
RETURNS INT
AS
BEGIN
	DECLARE @V_NEW_NUM INT = 0

        SET @V_NEW_NUM = CONVERT(INT, CASE WHEN IsNumeric(CONVERT(VARCHAR(12), @p_string)) = 1 then CONVERT(VARCHAR(12), @p_string) else 0 End) 

    RETURN @V_NEW_NUM;
END
    
