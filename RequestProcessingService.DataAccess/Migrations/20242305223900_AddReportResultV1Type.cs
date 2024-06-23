using FluentMigrator;

namespace RequestProcessingService.DataAccess.Migrations;

[Migration(20242305223900, TransactionBehavior.None)]
public class AddReportResultV1Type: Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'report_result_v1') THEN
            CREATE TYPE report_result_v1 as
            (
                request_id            bigint
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
        DROP TYPE IF EXISTS report_result_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}