using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.data.sqlserver
{
    public class CustomSqlExecutionStrategy : SqlServerRetryingExecutionStrategy
    {
        public CustomSqlExecutionStrategy( ExecutionStrategyDependencies dependencies )
            : base( dependencies, maxRetryCount: 5 ) { }
    }
}
