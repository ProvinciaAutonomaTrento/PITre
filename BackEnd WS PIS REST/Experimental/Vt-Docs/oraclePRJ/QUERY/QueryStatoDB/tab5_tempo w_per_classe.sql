select  to_char(a.end_time,'DD-MON-YYYY HH:MI:SS') end_time,
        b.wait_class,
        round((a.time_waited / 100),2) time_waited 
from    sys.v_$waitclassmetric_history a,
        sys.v_$system_wait_class b
where   a.wait_class# = b.wait_class# and
        b.wait_class != 'Idle'
order by 1 desc,3 desc;
