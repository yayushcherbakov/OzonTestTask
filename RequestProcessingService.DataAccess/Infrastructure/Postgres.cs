using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;
using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Models;

namespace RequestProcessingService.DataAccess.Infrastructure;

public static class Postgres
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    /// <summary>
    /// Map DAL models to composite types (enables UNNEST)
    /// </summary>
    public static void MapCompositeTypes()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        var mapper = NpgsqlConnection.GlobalTypeMapper;

        mapper.MapComposite<ReportRequestEntityV1>("report_request_v1", Translator);
        mapper.MapComposite<ReportResultV1>("report_result_v1", Translator);
    }

    /// <summary>
    /// Add migration infrastructure
    /// </summary>
    public static void AddMigrations(IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(s =>
                {
                    var cfg = s.GetRequiredService<IOptions<DataAccessOptions>>();
                    return cfg.Value.PostgresConnectionString;
                })
                .ScanIn(typeof(Postgres).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}