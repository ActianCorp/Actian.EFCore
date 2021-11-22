﻿using Actian.EFCore.Scaffolding.Internal;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Sprache;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore.Tests.Scaffolding
{
    public class ActianNameFilterParser_TableNameWithSchema
    {
        public ActianNameFilterParser_TableNameWithSchema(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("schema.table", "schema", false, "table", false)]
        [InlineData("\"schema\".\"table\"", "schema", true, "table", true)]
        [InlineData("\"my schema\".table", "my schema", true, "table", false)]
        [InlineData("\"my \"\"schema\"\"\".table", "my \"schema\"", true, "table", false)]
        [InlineData("my schema.table", "my schema", false, "table", false)]
        public void Can_parse(string str, string expectedSchemaName, bool expectedSchemaDelimited, string expectedTableName, bool expectedTableDelimited)
        {
            var actual = ActianNameFilterParser.TableNameWithSchema.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().BeEquivalentTo((
                schema: (name: expectedSchemaName, delimited: expectedSchemaDelimited),
                name: (name: expectedTableName, delimited: expectedTableDelimited)
            ));
        }

        [Theory]
        [InlineData("()")]
        [InlineData("table")]
        public void Can_not_parse(string str)
        {
            var actual = ActianNameFilterParser.TableNameWithSchema.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
