using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace envolti.lib.data.sqlserver
{
    public class CustomSqlExecutionStrategy : SqlServerRetryingExecutionStrategy
    {
        public CustomSqlExecutionStrategy( ExecutionStrategyDependencies dependencies )
            : base( dependencies, maxRetryCount: 5 ) { }
    }
}
