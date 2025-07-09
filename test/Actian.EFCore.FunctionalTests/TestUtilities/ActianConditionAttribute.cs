using System;
using System.Linq;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Ingres.Client;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

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
            isMet = true;
        }

        if (Conditions.HasFlag(ActianCondition.SupportsAttach))
        {
            var defaultConnection = new IngresConnectionStringBuilder(TestEnvironment.DefaultConnection);
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
        =>
            // ReSharper disable once UseStringInterpolation
            string.Format(
                "The test SQL Server does not meet these conditions: '{0}'",
                string.Join(
                    ", ", Enum.GetValues(typeof(ActianCondition))
                        .Cast<Enum>()
                        .Where(f => Conditions.HasFlag(f))
                        .Select(f => Enum.GetName(typeof(ActianCondition), f))));
}
