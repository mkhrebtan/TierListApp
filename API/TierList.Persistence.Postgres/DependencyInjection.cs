using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore;
using TierList.Domain.Abstraction;
using TierList.Domain.Repos;
using TierList.Persistence.Postgres.Repos;

namespace TierList.Persistence.Postgres;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<TierListDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ITierListRepository, TierListRepository>();
    }
}
