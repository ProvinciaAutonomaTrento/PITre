select  end_time,
        value
from    sys.v_$sysmetric_history
where   metric_name = 'Database CPU Time Ratio'
order by 1 desc;
