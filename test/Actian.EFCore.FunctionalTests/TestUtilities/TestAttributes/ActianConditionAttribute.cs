using System;
using System.Threading.Tasks;
using Ingres.Client;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ActianConditionAttribute : Attribute, ITestCondition
    {
        public ActianCondition Conditions { get; set; }

        public ActianConditionAttribute(ActianCondition conditions)
        {
            Conditions = conditions;
        }

        public ValueTask<bool> IsMetAsync()
        {
            var isMet = true;

            if (Conditions.HasFlag(ActianCondition.SupportsHiddenColumns))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsMemoryOptimized))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.IsSqlAzure))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.IsNotSqlAzure))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsAttach))
            {
                var defaultConnection = new IngresConnectionStringBuilder(TestEnvironment.DefaultConnection);
                isMet &= defaultConnection.Server.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);
            }

            if (Conditions.HasFlag(ActianCondition.IsNotCI))
            {
                isMet &= !TestEnvironment.IsCI;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsFullTextSearch))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsOnlineIndexes))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsTemporalTablesCascadeDelete))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsUtf8))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsFunctions2019))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsFunctions2017))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.SupportsJsonPathExpressions))
            {
                isMet = false;
            }

            if (Conditions.HasFlag(ActianCondition.BooleanExpession))
            {
                isMet = false;
            }

            return ValueTask.FromResult(isMet);
        }

        public string SkipReason
        {
            get
            {
                var (_, reason) = IsMetInternal();
                return reason;
            }
        }

        private (bool isMet, string reason) IsMetInternal()
        {
            if (Conditions.HasFlag(ActianCondition.Todo))
            {
                return (false, "Todo");
            }
            return (true, "");
        }
    }
}
