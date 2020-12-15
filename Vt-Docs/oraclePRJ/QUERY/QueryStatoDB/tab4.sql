select  WAIT_CLASS,
        TOTAL_WAITS,
        round(100 * (TOTAL_WAITS / SUM_WAITS),2) PCT_WAITS,
        ROUND((TIME_WAITED / 100),2) TIME_WAITED_SECS,
        round(100 * (TIME_WAITED / SUM_TIME),2) PCT_TIME
from
(select WAIT_CLASS,
        TOTAL_WAITS,
        TIME_WAITED
from    V$SYSTEM_WAIT_CLASS
where   WAIT_CLASS != 'Idle'),
(select  sum(TOTAL_WAITS) SUM_WAITS,
        sum(TIME_WAITED) SUM_TIME
from    V$SYSTEM_WAIT_CLASS
where   WAIT_CLASS != 'Idle')
order by 5 desc;
