using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Application.Common
{
    public interface IUserDefaultsService
    {
        Task EnsureDefaultsAsync(Guid userId, CancellationToken ct = default);
    }
}
