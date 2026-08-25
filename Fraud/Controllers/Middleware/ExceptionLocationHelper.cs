using Fraud.Core.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fraud.Controllers.Middleware
{ 
    public static class ExceptionLocationHelper
    {
        public static string GetSourceLocation(Exception ex)
        {
            var trace = new StackTrace(ex, true);
            var frame = trace.GetFrame(0);
            if (frame == null) return "Unknown location";

            var fileName = frame.GetFileName();
            var line = frame.GetFileLineNumber();
            var method = frame.GetMethod();
            var typeName = method?.DeclaringType?.Name ?? "Unknown";
            var methodName = method?.Name ?? "Unknown";

            return fileName != null
                ? $"{Path.GetFileName(fileName)}:{line} ({typeName}.{methodName})"
                : $"{typeName}.{methodName}";
        }

        public static string Simplify(Exception ex)
        {
            return ex switch
            {
                DbUpdateException dbEx
                    when dbEx.InnerException is SqlException sqlEx
                    => $"DB error [{sqlEx.Number}]: {sqlEx.Message}",

                SqlException sqlEx
                    => $"DB error [{sqlEx.Number}]: {sqlEx.Message}",

                ValidationException
                    => "Validation Error",

                NotFoundException
                    => ex.Message,

                _ => ex.Message
            };
        }
    }
}