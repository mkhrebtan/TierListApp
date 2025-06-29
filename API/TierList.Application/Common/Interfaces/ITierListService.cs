using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Application.Commands;
using TierList.Application.Common.Models;
using TierList.Application.Queries;
using TierList.Application.Queries.DTOs;

namespace TierList.Application.Common.Interfaces;

public interface ITierListService
{
    TierListResult CreateTierList(CreateTierListCommand request);

    TierListResult DeleteTierList(DeleteTierListCommand request);

    IReadOnlyCollection<TierListBriefDTO> GetTierLists(GetTierListsQuery request);

    TierListResult UpdateTierList(UpdateTierListCommand request);

    TierListResult GetTierListData(GetTierListDataQuery request);
}
