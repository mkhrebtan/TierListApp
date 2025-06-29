using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Queries.DTOs;
using TierList.Domain.Entities;

namespace TierList.Application.Common.Models;

public record TierListResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ErrorType? ErrorType { get; init; }
    public ITierListDTO? TierListData { get; init; }

    public static TierListResult Success(ITierListDTO? data = null) => new () { IsSuccess = true, TierListData = data ?? null };
    public static TierListResult Failure(string error, ErrorType errorType) => new () { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}
