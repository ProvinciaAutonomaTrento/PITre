// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.Core.Services;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using static Pi3.App.Legacy.WebApi.Models.HttpPerformanceLoggerService;

namespace Pi3.App.Legacy.WebApi.Models
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPerformanceLogger(
            this IServiceCollection services, Action<PerformanceLoggerOptions> options)
        {
            services.Configure<PerformanceLoggerOptions>(options);
            services.AddSingleton<IPerformanceLoggerService, HttpPerformanceLoggerService>();

            return services;
        }
    }

    public class PerformanceLoggerOptions
    {
        public int? MaxEntries { get; set; } = Int32.MaxValue;
    }

    public enum PerformanceLogOrderBy
    {
        Avg,
        Min,
        Max,
        Count,
        Sum,
        LastStartDate,
        LastEndDate,
        LastTotalSeconds,
    }

    public interface IPerformanceLoggerService : IService
    {
        Task Begin(string logName);

        Task End(string logName);

        Task<string> Collect(string? logName = null, PerformanceLogOrderBy? orderBy = null, bool? orderByDescending = false);
    }

    public class HttpPerformanceLoggerService : IPerformanceLoggerService
    {
        private ConcurrentDictionary<string, PerformanceLog> _items
         = new ConcurrentDictionary<string, PerformanceLog>();

        private readonly IOptions<PerformanceLoggerOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpPerformanceLoggerService(IOptions<PerformanceLoggerOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        public async Task Begin(string logName)
        {
            var instance = (this._httpContextAccessor.HttpContext.GetRouteValue("instance") ?? string.Empty).ToString();

            var key = $"{instance.ToLowerInvariant()}_{logName.ToLowerInvariant()}";

            if (!this._items.ContainsKey(key))
                this._items.TryAdd(key, new PerformanceLog(this._options.Value.MaxEntries.Value, instance.ToLowerInvariant(), logName));

            this._items[key].Begin();
        }

        public async Task End(string logName)
        {
            var instance = (this._httpContextAccessor.HttpContext.GetRouteValue("instance") ?? string.Empty).ToString();

            var key = $"{instance.ToLowerInvariant()}_{logName.ToLowerInvariant()}";

            if (this._items.ContainsKey(key))
                this._items[key].End();
        }


        public async Task<string> Collect(string? logName = null, PerformanceLogOrderBy? orderBy = null, bool? orderByDescending = false)
        {
            var instance = (this._httpContextAccessor.HttpContext.GetRouteValue("instance") ?? string.Empty).ToString().ToLowerInvariant();

            var options = new JsonSerializerSettings();
            options.Formatting = Newtonsoft.Json.Formatting.Indented;

            IQueryable<PerformanceLog> queryable = this._items.Values.Where(v => v.Instance.Equals(instance, StringComparison.InvariantCultureIgnoreCase)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(logName))
            {
                var key = $"{instance.ToLowerInvariant()}_{logName.ToLowerInvariant()}";

                queryable = queryable
                    .Where(v => v.LogName.Equals(logName, StringComparison.InvariantCultureIgnoreCase)).AsQueryable();
            }

            if (orderBy.HasValue)
            {
                switch (orderBy)
                {
                    case PerformanceLogOrderBy.Min:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.Min) : queryable.OrderByDescending(v => v.Min);
                        break;
                    case PerformanceLogOrderBy.Max:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.Max) : queryable.OrderByDescending(v => v.Max);
                        break;
                    case PerformanceLogOrderBy.Count:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.Count) : queryable.OrderByDescending(v => v.Count);
                        break;
                    case PerformanceLogOrderBy.Avg:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.Avg) : queryable.OrderByDescending(v => v.Avg);
                        break;
                    case PerformanceLogOrderBy.Sum:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.Sum) : queryable.OrderByDescending(v => v.Sum);
                        break;
                    case PerformanceLogOrderBy.LastStartDate:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.LastStartDate) : queryable.OrderByDescending(v => v.LastStartDate);
                        break;
                    case PerformanceLogOrderBy.LastEndDate:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.LastEndDate) : queryable.OrderByDescending(v => v.LastEndDate);
                        break;
                    case PerformanceLogOrderBy.LastTotalSeconds:
                        queryable = !orderByDescending.GetValueOrDefault() ? queryable.OrderBy(v => v.LastTotalSeconds) : queryable.OrderByDescending(v => v.LastTotalSeconds);
                        break;
                }
            }

            return JsonConvert.SerializeObject(queryable.ToArray(), options);
        }
    }

    public class PerformanceLog
    {
        private Stopwatch _stopwatch = null!;
        private List<PerformanceLogEntry> _entries = null!;
        private int _maxEntries;

        public PerformanceLog(int maxEntries, string instance, string logName)
        {
            this._maxEntries = maxEntries;
            this.Instance = instance;
            this.LogName = logName;

            this._entries = new List<PerformanceLogEntry>();
        }

        public string Instance
        {
            get;
            private set;
        }

        public string LogName
        {
            get;
            private set;
        }

        public PerformanceLogEntry[] Entries
        {
            get
            {
                return this._entries.OrderByDescending(e => e.StartDate).ToArray();
            }
        }

        public double? Avg
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.Average(e => e.TotalSeconds);
                return
                    null;
            }
        }

        public double? Min
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.Min(e => e.TotalSeconds);
                return
                    null;
            }
        }

        public double? Max
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.Max(e => e.TotalSeconds);
                return
                    null;
            }
        }

        public int Count
        {
            get
            {
                return this._entries.Count;
            }
        }

        public double? Sum
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.Sum(e => e.TotalSeconds);
                return
                    null;
            }
        }

        public DateTime? LastStartDate
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.OrderByDescending(e => e.StartDate).First().StartDate;
                return
                    null;
            }
        }

        public DateTime? LastEndDate
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.OrderByDescending(e => e.EndDate).First().EndDate;
                return
                    null;
            }
        }

        public double? LastTotalSeconds
        {
            get
            {
                if (this._entries.Any())
                    return this._entries.OrderByDescending(e => e.EndDate).First().TotalSeconds;
                return
                    null;
            }
        }

        public void Begin()
        {
            this._stopwatch = new Stopwatch();
            this._stopwatch.Start();

            if (this._entries.Count >= this._maxEntries)
                this._entries.RemoveAt(0);

            this._entries.Add(new PerformanceLogEntry()
            {
                StartDate = DateTime.Now
            });
        }

        public void End()
        {
            this._stopwatch.Stop();

            var last = this._entries.Last();
            last.EndDate = DateTime.Now;
            last.TotalSeconds = this._stopwatch.Elapsed.TotalSeconds;
        }
    }

    public class PerformanceLogEntry
    {
        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime? EndDate
        {
            get;
            set;
        }

        public double? TotalSeconds
        {
            get;
            set;
        }
    }
}
