select  METRIC_NAME,
        round(VALUE,2)
from    SYS.V_$SYSMETRIC
where   METRIC_NAME IN ('Database CPU Time Ratio',
                        'Database Wait Time Ratio') AND
        INTSIZE_CSEC = 
        (select max(INTSIZE_CSEC) from SYS.V_$SYSMETRIC)
