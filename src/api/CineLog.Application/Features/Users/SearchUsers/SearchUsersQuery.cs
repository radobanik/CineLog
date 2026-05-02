using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Users.SearchUsers
{

    public record SearchUsersQuery(string Query, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<DiscoverUserResponse>>;

}
