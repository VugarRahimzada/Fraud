using Fraud.Core.Entities;
using Fraud.Core.FraudDetection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Abstractions
{
    public interface IFraudDetectionEngine
    {
        /// <summary>
        /// transaction.FromCard və transaction.ToCard naviqasiyaları çağırıcı tərəfindən
        /// (in-memory, tracked instansiyalarla) doldurulmuş olmalıdır.
        /// </summary>
        Task<FraudEvaluationResult> EvaluateAsync(
            Transaction transaction,
            CancellationToken ct = default);
    }
}
