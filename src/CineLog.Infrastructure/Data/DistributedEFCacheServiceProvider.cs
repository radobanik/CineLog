using System.Text.Json;
using System.Text.Json.Serialization;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CineLog.Infrastructure.Data;

/// <summary>
/// Backs EFCoreSecondLevelCacheInterceptor with IDistributedCache (Redis).
/// </summary>
public class DistributedEFCacheServiceProvider : IEFCacheServiceProvider
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedEFCacheServiceProvider> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = false
    };

    public DistributedEFCacheServiceProvider(
        IDistributedCache cache,
        ILogger<DistributedEFCacheServiceProvider> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public void ClearAllCachedEntries()
    {
        // IDistributedCache has no "clear all" — no-op;
        // production would use Redis FLUSHDB on a dedicated DB.
        _logger.LogDebug("ClearAllCachedEntries called — no-op on distributed cache.");
    }

    public EFCachedData? GetValue(EFCacheKey cacheKey, EFCachePolicy cachePolicy)
    {
        try
        {
            var bytes = _cache.Get(cacheKey.KeyHash);
            if (bytes is null) return null;
            return JsonSerializer.Deserialize<EFCachedDataDto>(bytes, JsonOptions)?.ToEFCachedData();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get EF cache value for key {Key}", cacheKey.KeyHash);
            return null;
        }
    }

    public void InsertValue(EFCacheKey cacheKey, EFCachedData? value, EFCachePolicy cachePolicy)
    {
        if (value is null) return;
        try
        {
            var dto = EFCachedDataDto.From(value);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(dto, JsonOptions);

            var entryOptions = new DistributedCacheEntryOptions();
            entryOptions.AbsoluteExpirationRelativeToNow =
                cachePolicy.CacheTimeout ?? TimeSpan.FromMinutes(10);

            _cache.Set(cacheKey.KeyHash, bytes, entryOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to insert EF cache value for key {Key}", cacheKey.KeyHash);
        }
    }

    public void InvalidateCacheDependencies(EFCacheKey cacheKey)
    {
        try
        {
            _cache.Remove(cacheKey.KeyHash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate EF cache for key {Key}", cacheKey.KeyHash);
        }
    }

    // ── Internal DTOs ─────────────────────────────────────────────────────────

    private record EFCachedDataDto(
        EFTableRowsDto? TableRows,
        int NonQuery,
        string? ScalarJson,
        string? ScalarType,
        bool IsNull)
    {
        public static EFCachedDataDto From(EFCachedData d) => new(
            d.TableRows is null ? null : EFTableRowsDto.From(d.TableRows),
            d.NonQuery,
            d.Scalar is null ? null : JsonSerializer.Serialize(d.Scalar, JsonOptions),
            d.Scalar?.GetType().AssemblyQualifiedName,
            d.IsNull);

        public EFCachedData ToEFCachedData()
        {
            object? scalar = null;
            if (ScalarJson is not null && ScalarType is not null)
            {
                var type = Type.GetType(ScalarType);
                scalar = type is null
                    ? ScalarJson
                    : JsonSerializer.Deserialize(ScalarJson, type, JsonOptions);
            }

            return new EFCachedData
            {
                TableRows = TableRows?.ToEFTableRows(),
                NonQuery = NonQuery,
                Scalar = scalar,
                IsNull = IsNull
            };
        }
    }

    private record EFTableRowsDto(
        string? TableName,
        int? FieldCount,
        int? VisibleFieldCount,
        Dictionary<int, EFTableColumnInfo> ColumnsInfo,
        List<EFTableRowDto> Rows)
    {
        public static EFTableRowsDto From(EFTableRows r) => new(
            r.TableName,
            r.FieldCount,
            r.VisibleFieldCount,
            r.ColumnsInfo is null
                ? new Dictionary<int, EFTableColumnInfo>()
                : r.ColumnsInfo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            r.Rows?.Select(row => new EFTableRowDto(
                row.Values?.Cast<object?>().ToList() ?? [])).ToList() ?? []);

        public EFTableRows ToEFTableRows()
        {
            var fc = FieldCount.GetValueOrDefault();
            var vfc = VisibleFieldCount.GetValueOrDefault();
            return new EFTableRows
            {
                TableName = TableName,
                FieldCount = fc,
                VisibleFieldCount = vfc,
                ColumnsInfo = ColumnsInfo ?? new Dictionary<int, EFTableColumnInfo>(),
                Rows = Rows.Select(r => new EFTableRow(r.Values.Cast<object>().ToList())).ToList()
            };
        }
    }

    private record EFTableRowDto(List<object?> Values);
}
