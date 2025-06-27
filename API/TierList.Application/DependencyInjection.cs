using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Services;

namespace TierList.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITierListService, TierListService>();
    }
}
