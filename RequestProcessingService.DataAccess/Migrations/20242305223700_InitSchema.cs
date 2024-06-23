using System;
using FluentMigrator;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(20242305223700, TransactionBehavior.None)]
public sealed class InitSchema : Migration
{
    public override void Up()
    {
         const string sql =
            @"
             create table if not exists report_requests
             (
                 request_id          bigint primary key,
                 is_completed        boolean not null,
                 product_id bigint   not null,
                 check_period_from   timestamp with time zone not null,
                 check_period_to     timestamp with time zone not null,
                 racio               float8,
                 payment_count       integer
             );
            ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = "drop table if exists report_requests;";

        Execute.Sql(sql);
    }
}
