using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20242305223800, TransactionBehavior.None)]
public class AddReportRequestsV1Type: Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'report_request_v1') THEN
            CREATE TYPE report_request_v1 as
            (
                request_id            bigint
                , is_completed        boolean
                , product_id          bigint
                , check_period_from   timestamp with time zone
                , check_period_to     timestamp with time zone
                , racio               float8
                , payment_count       integer
            );
        END IF;
    END
$$;";

        Execute.Sql(sql);
    }
    
    public override void Down()
    {
        const string sql = @"
DO $$
    BEGIN
        DROP TYPE IF EXISTS report_request_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}
