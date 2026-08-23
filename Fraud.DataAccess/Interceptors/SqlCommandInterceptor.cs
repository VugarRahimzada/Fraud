using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Fraud.DataAccess.Interceptors
{
    // Hər icra olunan SQL sorğusunu HttpContext.Items-ə yazır ki,
    // xəta baş verəndə middleware "son işlənən sorğu"nu oxuya bilsin.
    public class SqlCommandInterceptor : DbCommandInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlCommandInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            Store(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            Store(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            Store(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        private void Store(DbCommand command)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
                httpContext.Items["LastSqlQuery"] = command.CommandText;
        }
    }
}