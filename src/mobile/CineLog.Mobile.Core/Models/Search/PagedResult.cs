using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.Search
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        bool HasMore);
}
