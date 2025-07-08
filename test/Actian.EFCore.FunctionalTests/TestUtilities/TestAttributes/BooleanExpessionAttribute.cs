using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BooleanExpessionAttribute : ActianTestAttribute, ITestCondition
    {
        public BooleanExpessionAttribute(string skipReason = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
            SkipReason = string.IsNullOrWhiteSpace(skipReason)
                ? "Boolean Expessions are not currently supported in the Where clause."
                : $"BooleanExpessionAttribute:\n{skipReason.NormalizeText()}";
        }

        public string SkipReason { get; }

        public ValueTask<bool> IsMetAsync() => new ValueTask<bool>(IsMet());

        private bool IsMet()
            => Member.DeclaringType.GetCustomAttributes<BooleanExpessionAttribute>().Any();
    }
}
