select  a.sid,
        b.username,
        a.wait_class,
        a.total_waits,
        round((a.time_waited / 100),2) time_waited_secs
from    sys.v_$session_wait_class a,
        sys.v_$session b
where   b.sid = a.sid and
        b.username ='COMUNI_PROD' and
        a.wait_class != 'Idle'
order by 5 desc;
