using System;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using Actian.EFCore.Diagnostics.Internal;
using System.Threading.Tasks;

namespace Actian.EFCore
{
    // TODO: Make sure read of inserted values works
    public class DataAnnotationActianTest : DataAnnotationRelationalTestBase<DataAnnotationActianTest.DataAnnotationActianFixture>
    {
        public DataAnnotationActianTest(DataAnnotationActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
             => facade.UseTransaction(transaction.GetDbTransaction());

        protected override TestHelpers TestHelpers
            => ActianTestHelpers.Instance;

        [ConditionalFact]
        public virtual void Default_for_key_string_column_throws()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<Login1>().Property(l => l.UserName).HasDefaultValue("default");
            modelBuilder.Ignore<Profile1>();

            Assert.Equal(
                CoreStrings.WarningAsErrorTemplate(
                    RelationalEventId.ModelValidationKeyDefaultValueWarning,
                    RelationalResources.LogKeyHasDefaultValue(new TestLogger<ActianLoggingDefinitions>())
                        .GenerateMessage(nameof(Login1.UserName), nameof(Login1)),
                    "RelationalEventId.ModelValidationKeyDefaultValueWarning"),
                Assert.Throws<InvalidOperationException>(() => Validate(modelBuilder)).Message);
        }

        [ConditionalFact]
        public virtual void Default_for_key_which_is_also_an_fk_column_does_not_throw()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<PrincipalA>();
            modelBuilder.Entity<DependantA>(
                b =>
                {
                    b.HasKey(e => new { e.Id, e.PrincipalId });
                    b.Property(e => e.PrincipalId).HasDefaultValue(77);
                });

            Validate(modelBuilder);
        }

        [ConditionalFact]
        public virtual void Default_for_part_of_composite_key_does_not_throw()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<PrincipalB>(
                b =>
                {
                    b.HasKey(e => new { e.Id1, e.Id2 });
                    b.Property(e => e.Id1).HasDefaultValue(77);
                });

            Validate(modelBuilder);
        }

        [ConditionalFact]
        public virtual void Default_for_all_parts_of_composite_key_throws()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<PrincipalB>(
                b =>
                {
                    b.HasKey(e => new { e.Id1, e.Id2 });
                    b.Property(e => e.Id1).HasDefaultValue(77);
                    b.Property(e => e.Id2).HasDefaultValue(78);
                });

            Assert.Equal(
                CoreStrings.WarningAsErrorTemplate(
                    RelationalEventId.ModelValidationKeyDefaultValueWarning,
                    RelationalResources.LogKeyHasDefaultValue(new TestLogger<ActianLoggingDefinitions>())
                        .GenerateMessage(nameof(PrincipalB.Id1), nameof(PrincipalB)),
                    "RelationalEventId.ModelValidationKeyDefaultValueWarning"),
                Assert.Throws<InvalidOperationException>(() => Validate(modelBuilder)).Message);
        }

        public override IModel Non_public_annotations_are_enabled()
        {
            var model = base.Non_public_annotations_are_enabled();

            var property = GetProperty<PrivateMemberAnnotationClass>(model, "PersonFirstName");
            Assert.Equal("dsdsd", property.GetColumnName());
            Assert.Equal("nvarchar(128)", property.GetColumnType());

            return model;
        }

        public override IModel Field_annotations_are_enabled()
        {
            var model = base.Field_annotations_are_enabled();

            var property = GetProperty<FieldAnnotationClass>(model, "_personFirstName");
            Assert.Equal("dsdsd", property.GetColumnName());
            Assert.Equal("nvarchar(128)", property.GetColumnType());

            return model;
        }

        public override IModel Key_and_column_work_together()
        {
            var model = base.Key_and_column_work_together();

            var relational = GetProperty<ColumnKeyAnnotationClass1>(model, "PersonFirstName");
            Assert.Equal("dsdsd", relational.GetColumnName());
            Assert.Equal("nvarchar(128)", relational.GetColumnType());

            return model;
        }

        public override IModel Key_and_MaxLength_64_produce_nvarchar_64()
        {
            var model = base.Key_and_MaxLength_64_produce_nvarchar_64();

            var property = GetProperty<ColumnKeyAnnotationClass2>(model, "PersonFirstName");

            var storeType = property.GetRelationalTypeMapping().StoreType;

            Assert.Equal("nvarchar(64)", storeType);

            return model;
        }

        public override IModel Timestamp_takes_precedence_over_MaxLength()
        {
            var model = base.Timestamp_takes_precedence_over_MaxLength();

            var property = GetProperty<TimestampAndMaxlength>(model, "MaxTimestamp");

            var storeType = property.GetRelationalTypeMapping().StoreType;

            Assert.Equal("long byte", storeType);

            return model;
        }

        public override IModel TableNameAttribute_affects_table_name_in_TPH()
        {
            var model = base.TableNameAttribute_affects_table_name_in_TPH();

            Assert.Equal("A", model.FindEntityType(typeof(TNAttrBase)).GetTableName());

            return model;
        }

        public override IModel DatabaseGeneratedOption_configures_the_property_correctly()
        {
            var model = base.DatabaseGeneratedOption_configures_the_property_correctly();

            var identity = model.FindEntityType(typeof(GeneratedEntity)).FindProperty(nameof(GeneratedEntity.Identity));
            Assert.Equal(ActianValueGenerationStrategy.IdentityByDefaultColumn, identity.GetValueGenerationStrategy());

            return model;
        }

        [ConditionalFact]
        public virtual void ColumnAttribute_configures_the_property_correctly()
        {
            var modelBuilder = CreateModelBuilder();
            modelBuilder.Entity<One>().HasKey(o => o.UniqueNo);

            var model = modelBuilder.FinalizeModel();

            Assert.Equal(
                "Unique_No",
                model.FindEntityType(typeof(One)).FindProperty(nameof(One.UniqueNo)).GetColumnName());
        }

        public override IModel DatabaseGeneratedOption_Identity_does_not_throw_on_noninteger_properties()
        {
            var model = base.DatabaseGeneratedOption_Identity_does_not_throw_on_noninteger_properties();

            var entity = model.FindEntityType(typeof(GeneratedEntityNonInteger));

            var stringProperty = entity.FindProperty(nameof(GeneratedEntityNonInteger.String));
            Assert.Equal(ActianValueGenerationStrategy.None, stringProperty.GetValueGenerationStrategy());

            var dateTimeProperty = entity.FindProperty(nameof(GeneratedEntityNonInteger.DateTime));
            Assert.Equal(ActianValueGenerationStrategy.None, dateTimeProperty.GetValueGenerationStrategy());

            var guidProperty = entity.FindProperty(nameof(GeneratedEntityNonInteger.Guid));
            Assert.Equal(ActianValueGenerationStrategy.None, guidProperty.GetValueGenerationStrategy());

            return model;
        }

        [ActianTodo]
        public override async Task ConcurrencyCheckAttribute_throws_if_value_in_database_changed()
        {
            await base.ConcurrencyCheckAttribute_throws_if_value_in_database_changed();

            AssertSql(
                """
SELECT FIRST 1 "s"."Unique_No", "s"."MaxLengthProperty", "s"."Name", "s"."RowVersion", "s"."AdditionalDetails_Name", "s"."AdditionalDetails_Value", "s"."Details_Name", "s"."Details_Value"
FROM "Sample" AS "s"
WHERE "s"."Unique_No" = 1
""",
                //
                """
SELECT FIRST 1 "s"."Unique_No", "s"."MaxLengthProperty", "s"."Name", "s"."RowVersion", "s"."AdditionalDetails_Name", "s"."AdditionalDetails_Value", "s"."Details_Name", "s"."Details_Value"
FROM "Sample" AS "s"
WHERE "s"."Unique_No" = 1
""",
                //
                """
@p2='1'
@p0='ModifiedData' (Nullable = false)
@p1='00000000-0000-0000-0003-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE "Sample" SET "Name" = @p0, "RowVersion" = @p1
WHERE "Unique_No" = @p2 AND "RowVersion" = @p3;
SELECT @@ROW_COUNT;
""",
                //
                """
@p2='1'
@p0='ChangedData' (Nullable = false)
@p1='00000000-0000-0000-0002-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE "Sample" SET "Name" = @p0, "RowVersion" = @p1
WHERE "Unique_No" = @p2 AND "RowVersion" = @p3;
SELECT @@ROW_COUNT;
""");
        }

        public override async Task DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity()
        {
            await base.DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity();

            AssertSql(
                """
@p0=NULL
@p1='Third' (Nullable = false)
@p2='00000000-0000-0000-0000-000000000003'
@p3='Third Additional Name'
@p4='0' (Nullable = true)
@p5='Third Name'
@p6='0' (Nullable = true)

INSERT INTO "Sample" ("MaxLengthProperty", "Name", "RowVersion", "AdditionalDetails_Name", "AdditionalDetails_Value", "Details_Name", "Details_Value")
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6);
SELECT "Unique_No"
FROM "Sample"
WHERE @@ROW_COUNT = 1 AND "Unique_No" = LAST_IDENTITY();
""");
        }

        [ActianTodo]
        public override async Task MaxLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        {
            await base.MaxLengthAttribute_throws_while_inserting_value_longer_than_max_length();

            AssertSql(
                """
@p0='Short'
@p1='ValidString' (Nullable = false)
@p2='00000000-0000-0000-0000-000000000001'
@p3='Third Additional Name'
@p4='0' (Nullable = true)
@p5='Third Name'
@p6='0' (Nullable = true)

INSERT INTO "Sample" ("MaxLengthProperty", "Name", "RowVersion", "AdditionalDetails_Name", "AdditionalDetails_Value", "Details_Name", "Details_Value")
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6);
SELECT "Unique_No"
FROM "Sample"
WHERE @@ROW_COUNT = 1 AND "Unique_No" = LAST_IDENTITY();
""",
                //
                """
@p0='VeryVeryVeryVeryVeryVeryLongString'
@p1='ValidString' (Nullable = false)
@p2='00000000-0000-0000-0000-000000000002'
@p3='Third Additional Name'
@p4='0' (Nullable = true)
@p5='Third Name'
@p6='0' (Nullable = true)

INSERT INTO "Sample" ("MaxLengthProperty", "Name", "RowVersion", "AdditionalDetails_Name", "AdditionalDetails_Value", "Details_Name", "Details_Value")
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6);
SELECT "Unique_No"
FROM "Sample"
WHERE @@ROW_COUNT = 1 AND "Unique_No" = LAST_IDENTITY();
""");
        }

        [ActianTodo]
        public override async Task StringLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        {
            await base.StringLengthAttribute_throws_while_inserting_value_longer_than_max_length();

            AssertSql(
                """
@p0='ValidString'

INSERT INTO "Two" ("Data")
VALUES (@p0);
SELECT "Id", "Timestamp"
FROM "Two"
WHERE @@ROW_COUNT = 1 AND "Id" = LAST_IDENTITY();
""",
                //
                """
@p0='ValidButLongString'

INSERT INTO "Two" ("Data")
VALUES (@p0);
SELECT "Id", "Timestamp"
FROM "Two"
WHERE @@ROW_COUNT = 1 AND "Id" = LAST_IDENTITY();
""");
        }

        [ActianTodo]
        public override async Task TimestampAttribute_throws_if_value_in_database_changed()
            => await base.TimestampAttribute_throws_if_value_in_database_changed();

        // Not validating SQL because not significantly different from other tests and
        // row version value is not stable.
        private static readonly string _eol = Environment.NewLine;

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class DataAnnotationActianFixture : DataAnnotationRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;
        }
    }
}
