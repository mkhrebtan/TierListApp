using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

namespace TierList.Application.Queries.GetLists;

internal sealed class GetTierListsQueryHandler : IQueryHandler<GetTierListsQuery, IReadOnlyCollection<TierListBriefDto>>
{
    private readonly ITierListRepository _tierListRepository;

    public GetTierListsQueryHandler(ITierListRepository tierListRepository)
    {
        _tierListRepository = tierListRepository;
    }

    public async Task<Result<IReadOnlyCollection<TierListBriefDto>>> Handle(GetTierListsQuery query)
    {
        if (query.UserId <= 0)
        {
            return Result<IReadOnlyCollection<TierListBriefDto>>.Failure(
                new Error("Validation", "Invalid user ID provided."));
        }

        var listsEntities = await _tierListRepository.GetAllAsync(query.UserId);
        var listsDtos = listsEntities
            .Select(l => new TierListBriefDto
            {
                Id = l.Id,
                Title = l.Title,
                Created = l.Created,
                LastModified = l.LastModified,
            }).ToList().AsReadOnly();

        return Result<IReadOnlyCollection<TierListBriefDto>>.Success(listsDtos);
    }
}
