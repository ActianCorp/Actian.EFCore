using System;
using System.Data.SqlTypes;
using System.IO;
using Actian.EFCore;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Migrations.Operations;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore
{
    public class ActianMigrationSqlGeneratorTest : MigrationsSqlGeneratorTestBase
    {
        [ConditionalFact]
        public void CreateIndexOperation_unique_online()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Schema = "dbo",
                    Columns = new[] { "FirstName", "LastName" },
                    IsUnique = true,
                    [ActianAnnotationNames.CreatedOnline] = true
                });

            AssertSql(
                """
set session authorization "dbo";
GO

CREATE INDEX "IX_People_Name" ON "dbo"."People" ("FirstName", "LastName") WHERE "FirstName" IS NOT NULL AND "LastName" IS NOT NULL WITH (ONLINE = ON);
""");
        }

        [ConditionalFact]
        public void CreateIndexOperation_unique_sortintempdb()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Schema = "dbo",
                    Columns = new[] { "FirstName", "LastName" },
                    IsUnique = true,
                    [ActianAnnotationNames.SortInTempDb] = true
                });

            AssertSql(
                """
set session authorization "dbo";
GO

CREATE INDEX "IX_People_Name" ON "dbo"."People" ("FirstName", "LastName") WHERE "FirstName" IS NOT NULL AND "LastName" IS NOT NULL WITH (SORT_IN_TEMPDB = ON);
""");
        }

        [ConditionalTheory]
        [InlineData(DataCompressionType.None, "NONE")]
        [InlineData(DataCompressionType.Row, "ROW")]
        [InlineData(DataCompressionType.Page, "PAGE")]
        public void CreateIndexOperation_unique_datacompression(DataCompressionType dataCompression, string dataCompressionSql)
        {
            if (dataCompressionSql is null)
            {
                throw new ArgumentNullException(nameof(dataCompressionSql));
            }

            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Schema = "dbo",
                    Columns = new[] { "FirstName", "LastName" },
                    IsUnique = true,
                    [ActianAnnotationNames.DataCompression] = dataCompression
                });

            AssertSql(
                $"""
set session authorization "dbo";
GO

CREATE INDEX "IX_People_Name" ON "dbo"."People" ("FirstName", "LastName") WHERE "FirstName" IS NOT NULL AND "LastName" IS NOT NULL WITH (DATA_COMPRESSION = {dataCompression.ToString().ToUpper()});

""");
        }

        [ConditionalFact]
        public virtual void AddColumnOperation_identity_legacy()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "Id",
                    ClrType = typeof(int),
                    ColumnType = "int",
                    DefaultValue = 0,
                    IsNullable = false,
                    [ActianAnnotationNames.ValueGenerationStrategy] =
                        ActianValueGenerationStrategy.IdentityColumn
                });

            AssertSql(
                """
ALTER TABLE "People" ADD "Id" int NOT NULL WITH DEFAULT 0;
GO

MODIFY "People" TO RECONSTRUCT;

""");
        }

        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            AssertSql(
                """
ALTER TABLE "People" ADD "Alias" long nvarchar NOT NULL WITH DEFAULT '';
GO

MODIFY "People" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_unicode_no_model()
        {
            base.AddColumnOperation_with_unicode_no_model();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Name" long varchar WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_fixed_length_no_model()
        {
            base.AddColumnOperation_with_fixed_length_no_model();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Name" char(100) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;

""");
        }

        public override void AddColumnOperation_with_maxLength_no_model()
        {
            base.AddColumnOperation_with_maxLength_no_model();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Name" nvarchar(30) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_maxLength_overridden()
        {
            base.AddColumnOperation_with_maxLength_overridden();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Name" nvarchar(32) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_precision_and_scale_overridden()
        {
            base.AddColumnOperation_with_precision_and_scale_overridden();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Pi" decimal(15,10) NOT NULL WITH DEFAULT 0;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_precision_and_scale_no_model()
        {
            base.AddColumnOperation_with_precision_and_scale_no_model();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Pi" decimal(20,7) NOT NULL WITH DEFAULT 0;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        public override void AddColumnOperation_with_unicode_overridden()
        {
            base.AddColumnOperation_with_unicode_overridden();

            AssertSql(
                """
ALTER TABLE "Person" ADD "Name" long nvarchar WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
        }

        [ConditionalFact]
        public virtual void AddColumnOperation_with_rowversion_overridden()
        {
            Generate(
                modelBuilder => modelBuilder.Entity<Person>().Property < byte[] > ("RowVersion"),
                new AddColumnOperation
                {
                    Table = "Person",
                    Name = "RowVersion",
                    ClrType = typeof(byte[]),
                    IsRowVersion = true,
                    IsNullable = true
                });

            AssertSql(
                """
ALTER TABLE "Person" ADD "RowVersion" long byte WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
    }

        [ConditionalFact]
        public virtual void AddColumnOperation_with_rowversion_no_model()
    {
        Generate(
            new AddColumnOperation
            {
                Table = "Person",
                Name = "RowVersion",
                ClrType = typeof(byte[]),
                    IsRowVersion = true,
                    IsNullable = true
                });

            AssertSql(
                """
ALTER TABLE "Person" ADD "RowVersion" long byte WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
""");
}

public override void AlterColumnOperation_without_column_type()
{
    base.AlterColumnOperation_without_column_type();

    AssertSql(
        """
ALTER TABLE "People" ALTER COLUMN "LuckyNumber" integer NOT NULL WITH DEFAULT 0;
GO

MODIFY "People" TO RECONSTRUCT;

""");
}

public override void AddForeignKeyOperation_without_principal_columns()
{
    base.AddForeignKeyOperation_without_principal_columns();

    AssertSql(
        """
ALTER TABLE "People" ADD FOREIGN KEY ("SpouseId") REFERENCES "People";
""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_with_identity_legacy()
{
    Generate(
        new AlterColumnOperation
        {
            Table = "People",
            Name = "Id",
            ClrType = typeof(int),
            [ActianAnnotationNames.ValueGenerationStrategy] =
                ActianValueGenerationStrategy.IdentityColumn
        });

    AssertSql(
        """
ALTER TABLE "People" ALTER COLUMN "Id" integer NOT NULL WITH DEFAULT 0;
GO

MODIFY "People" TO RECONSTRUCT;
""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_with_index_no_oldColumn()
{
    Generate(
        modelBuilder => modelBuilder
            .HasAnnotation(CoreAnnotationNames.ProductVersion, "1.0.0-rtm")
            .Entity<Person>(
                x =>
                {
                    x.Property<string>("Name").HasMaxLength(30);
                    x.HasIndex("Name");
                }),
        new AlterColumnOperation
        {
            Table = "Person",
            Name = "Name",
            ClrType = typeof(string),
            MaxLength = 30,
            IsNullable = true,
            OldColumn = new AddColumnOperation()
        });

    AssertSql(
        """
ALTER TABLE "Person" ALTER COLUMN "Name" nvarchar(30) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;

""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_with_added_index()
{
    Generate(
        modelBuilder => modelBuilder
            .HasAnnotation(CoreAnnotationNames.ProductVersion, "1.1.0")
            .Entity<Person>(
                x =>
                {
                    x.Property<string>("Name").HasMaxLength(30);
                    x.HasIndex("Name");
                }),
        new AlterColumnOperation
        {
            Table = "Person",
            Name = "Name",
            ClrType = typeof(string),
            MaxLength = 30,
            IsNullable = true,
            OldColumn = new AddColumnOperation { ClrType = typeof(string), IsNullable = true }
        },
        new CreateIndexOperation
        {
            Name = "IX_Person_Name",
            Table = "Person",
            Columns = new[] { "Name" }
        });

    AssertSql(
        """
ALTER TABLE "Person" ALTER COLUMN "Name" nvarchar(30) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
GO

CREATE INDEX "IX_Person_Name" ON "Person" ("Name");

""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_with_added_index_no_oldType()
{
    Generate(
        modelBuilder => modelBuilder
            .HasAnnotation(CoreAnnotationNames.ProductVersion, "2.1.0")
            .Entity<Person>(
                x =>
                {
                    x.Property<string>("Name");
                    x.HasIndex("Name");
                }),
        new AlterColumnOperation
        {
            Table = "Person",
            Name = "Name",
            ClrType = typeof(string),
            IsNullable = true,
            OldColumn = new AddColumnOperation { ClrType = typeof(string), IsNullable = true }
        },
        new CreateIndexOperation
        {
            Name = "IX_Person_Name",
            Table = "Person",
            Columns = new[] { "Name" }
        });

    AssertSql(
        """
ALTER TABLE "Person" ALTER COLUMN "Name" nvarchar(220) WITH NULL;
GO

MODIFY "Person" TO RECONSTRUCT;
GO

CREATE INDEX "IX_Person_Name" ON "Person" ("Name");

""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_identity_legacy()
{
    Generate(
        modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersion, "1.1.0"),
        new AlterColumnOperation
        {
            Table = "Person",
            Name = "Id",
            ClrType = typeof(long),
            [ActianAnnotationNames.ValueGenerationStrategy] = ActianValueGenerationStrategy.IdentityColumn,
            OldColumn = new AddColumnOperation
            {
                ClrType = typeof(int),
                [ActianAnnotationNames.ValueGenerationStrategy] = ActianValueGenerationStrategy.IdentityColumn
            }
        });

    AssertSql(
        """
ALTER TABLE "Person" ALTER COLUMN "Id" bigint NOT NULL WITH DEFAULT 0;
GO

MODIFY "Person" TO RECONSTRUCT;

""");
}

[ConditionalFact]
        public virtual void AlterColumnOperation_add_identity_legacy()
{
    var ex = Assert.Throws<InvalidOperationException>(
        () => Generate(
            modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersion, "1.1.0"),
            new AlterColumnOperation
            {
                Table = "Person",
                Name = "Id",
                ClrType = typeof(int),
                [ActianAnnotationNames.ValueGenerationStrategy] = ActianValueGenerationStrategy.IdentityColumn,
                OldColumn = new AddColumnOperation { ClrType = typeof(int) }
            }));

    Assert.Equal(ActianStrings.AlterIdentityColumn, ex.Message);
}

[ConditionalFact]
        public virtual void AlterColumnOperation_remove_identity_legacy()
{
    var ex = Assert.Throws<InvalidOperationException>(
        () => Generate(
            modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersion, "1.1.0"),
            new AlterColumnOperation
            {
                Table = "Person",
                Name = "Id",
                ClrType = typeof(int),
                OldColumn = new AddColumnOperation
                {
                    ClrType = typeof(int),
                    [ActianAnnotationNames.ValueGenerationStrategy] = ActianValueGenerationStrategy.IdentityColumn
                }
            }));

    Assert.Equal(ActianStrings.AlterIdentityColumn, ex.Message);
}

[ActianTodo]
[ConditionalFact]
        public virtual void CreateDatabaseOperation()
{
    Generate(
        new ActianCreateDatabaseOperation { Name = "Northwind" });

    AssertSql(
        """
CREATE DATABASE "Northwind";
GO

IF SERVERPROPERTY('EngineEdition') <> 5
BEGIN
    ALTER DATABASE "Northwind" SET READ_COMMITTED_SNAPSHOT ON;
END;
""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void CreateDatabaseOperation_with_collation()
{
    Generate(
        new ActianCreateDatabaseOperation { Name = "Northwind" }); //, Collation = "German_PhoneBook_CI_AS" });

    AssertSql(
        """
CREATE DATABASE "Northwind"
COLLATE German_PhoneBook_CI_AS;
GO

IF SERVERPROPERTY('EngineEdition') <> 5
BEGIN
    ALTER DATABASE "Northwind" SET READ_COMMITTED_SNAPSHOT ON;
END;
""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void AlterDatabaseOperation_collation()
{
    Generate(
        new AlterDatabaseOperation { Collation = "German_PhoneBook_CI_AS" });

    Assert.Contains(
        "COLLATE German_PhoneBook_CI_AS",
        Sql);
}

[ActianTodo]
[ConditionalFact]
        public virtual void AlterDatabaseOperation_collation_to_default()
{
    Generate(
        new AlterDatabaseOperation { Collation = null, OldDatabase = { Collation = "SQL_Latin1_General_CP1_CI_AS" } });

    AssertSql(
        """
BEGIN
DECLARE @db_name nvarchar(max) = DB_NAME();
DECLARE @defaultCollation nvarchar(max) = CAST(SERVERPROPERTY('Collation') AS nvarchar(max));
EXEC(N'ALTER DATABASE "' + @db_name + '" COLLATE ' + @defaultCollation + N';');
END

""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void AlterDatabaseOperation_memory_optimized()
{
    Generate(
        new AlterDatabaseOperation { [ActianAnnotationNames.MemoryOptimized] = true });

    Assert.Contains(
        "CONTAINS MEMORY_OPTIMIZED_DATA;",
        Sql);
}

[ActianTodo]
[ConditionalFact]
        public virtual void DropDatabaseOperation()
{
    Generate(
        new ActianDropDatabaseOperation { Name = "Northwind" });

    AssertSql(
        """
IF SERVERPROPERTY('EngineEdition') <> 5
BEGIN
    ALTER DATABASE "Northwind" SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END;
GO

DROP DATABASE "Northwind";
""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void DropIndexOperations_throws_when_no_table()
{
    var migrationBuilder = new MigrationBuilder("Actian");

    migrationBuilder.DropIndex(
        name: "IX_Name");

    var ex = Assert.Throws<InvalidOperationException>(
        () => Generate(migrationBuilder.Operations.ToArray()));

    Assert.Equal(ActianStrings.IndexTableRequired, ex.Message);
}

[ActianTodo]
[ConditionalFact]
        public virtual void MoveSequenceOperation_legacy()
{
    Generate(
        new RenameSequenceOperation
        {
            Name = "EntityFrameworkHiLoSequence",
            Schema = "dbo",
            NewSchema = "my"
        });

    AssertSql(
        """
ALTER SCHEMA "my" TRANSFER "EntityFrameworkHiLoSequence";
""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void MoveTableOperation_legacy()
{
    Generate(
        new RenameTableOperation
        {
            Name = "People",
            Schema = "dbo",
            NewSchema = "hr"
        });

    AssertSql(
        """
ALTER SCHEMA "hr" TRANSFER "People";
""");
}

[ActianTodo]
[ConditionalFact]
        public virtual void RenameIndexOperations_throws_when_no_table()
{
    var migrationBuilder = new MigrationBuilder("Actian");

    migrationBuilder.RenameIndex(
        name: "IX_OldIndex",
        newName: "IX_NewIndex");

    var ex = Assert.Throws<InvalidOperationException>(
        () => Generate(migrationBuilder.Operations.ToArray()));

    Assert.Equal(ActianStrings.IndexTableRequired, ex.Message);
}

[ActianTodo]
[ConditionalFact]
        public virtual void RenameSequenceOperation_legacy()
{
    Generate(
        new RenameSequenceOperation
        {
            Name = "EntityFrameworkHiLoSequence",
            Schema = "dbo",
            NewName = "MySequence"
        });

    AssertSql(
        """
EXEC sp_rename N'"EntityFrameworkHiLoSequence"', N'MySequence';
""");
}

[ConditionalFact]
        public override void RenameTableOperation_legacy()
{
    base.RenameTableOperation_legacy();

    AssertSql(
        """
set session authorization "dbo";
GO

ALTER TABLE "dbo"."People" RENAME TO "Person";
GO

MODIFY "dbo"."Person" TO RECONSTRUCT;
""");
}

public override void RenameTableOperation()
{
    base.RenameTableOperation();

    AssertSql(
        """
set session authorization "dbo";
GO

ALTER TABLE "dbo"."People" RENAME TO "Person";
GO

MODIFY "dbo"."Person" TO RECONSTRUCT;

""");
}

[ConditionalFact]
        public virtual void SqlOperation_handles_backslash()
{
    Generate(
        new SqlOperation { Sql = @"-- Multiline \" + EOL + "comment" });

    AssertSql(
        """
-- Multiline \
comment
""");
}

[ConditionalFact]
        public virtual void SqlOperation_ignores_sequential_gos()
{
    Generate(
        new SqlOperation { Sql = "-- Ready set" + EOL + "GO" + EOL + "GO" });

    AssertSql(
        """
-- Ready set
GO
GO
""");
}

[ConditionalFact]
        public virtual void SqlOperation_handles_go()
{
    Generate(
        new SqlOperation { Sql = "-- I" + EOL + "go" + EOL + "-- Too" });

    AssertSql(
        """
-- I
go
-- Too
""");
}

[ConditionalFact]
        public virtual void SqlOperation_handles_go_with_count()
{
    Generate(
        new SqlOperation { Sql = "-- I" + EOL + "GO 2" });

    AssertSql(
        """
-- I
GO 2
""");
}

[ConditionalFact]
        public virtual void SqlOperation_ignores_non_go()
{
    Generate(
        new SqlOperation { Sql = "-- I GO 2" });

    AssertSql(
        """
-- I GO 2
""");
}

public override void SqlOperation()
{
    base.SqlOperation();

    AssertSql(
        """
-- I <3 DDL
""");
}

[ActianTodo]
public override void InsertDataOperation_all_args_spatial()
{
    base.InsertDataOperation_all_args_spatial();

    AssertSql(
        """
IF EXISTS (SELECT * FROM "sys"."identity_columns" WHERE "name" IN (N'Id', N'Full Name', N'Geometry') AND "object_id" = OBJECT_ID(N'"People"'))
    SET IDENTITY_INSERT "People" ON;
INSERT INTO "People" ("Id", "Full Name", "Geometry")
VALUES (0, NULL, NULL),
(1, 'Daenerys Targaryen', NULL),
(2, 'John Snow', NULL),
(3, 'Arya Stark', NULL),
(4, 'Harry Strickland', NULL),
(5, 'The Imp', NULL),
(6, 'The Kingslayer', NULL),
(7, 'Aemon Targaryen', geography::Parse('GEOMETRYCOLLECTION (LINESTRING (1.1 2.2 NULL, 2.2 2.2 NULL, 2.2 1.1 NULL, 7.1 7.2 NULL), LINESTRING (7.1 7.2 NULL, 20.2 20.2 NULL, 20.2 1.1 NULL, 70.1 70.2 NULL), MULTIPOINT ((1.1 2.2 NULL), (2.2 2.2 NULL), (2.2 1.1 NULL)), POLYGON ((1.1 2.2 NULL, 2.2 2.2 NULL, 2.2 1.1 NULL, 1.1 2.2 NULL)), POLYGON ((10.1 20.2 NULL, 20.2 20.2 NULL, 20.2 10.1 NULL, 10.1 20.2 NULL)), POINT (1.1 2.2 3.3), MULTILINESTRING ((1.1 2.2 NULL, 2.2 2.2 NULL, 2.2 1.1 NULL, 7.1 7.2 NULL), (7.1 7.2 NULL, 20.2 20.2 NULL, 20.2 1.1 NULL, 70.1 70.2 NULL)), MULTIPOLYGON (((10.1 20.2 NULL, 20.2 20.2 NULL, 20.2 10.1 NULL, 10.1 20.2 NULL)), ((1.1 2.2 NULL, 2.2 2.2 NULL, 2.2 1.1 NULL, 1.1 2.2 NULL))))'));
IF EXISTS (SELECT * FROM "sys"."identity_columns" WHERE "name" IN (N'Id', N'Full Name', N'Geometry') AND "object_id" = OBJECT_ID(N'"People"'))
    SET IDENTITY_INSERT "People" OFF;
""");
}

// The test data we're using is geographic but is represented in NTS as a GeometryCollection
protected override string GetGeometryCollectionStoreType()
    => "geography";

public override void InsertDataOperation_required_args()
{
    base.InsertDataOperation_required_args();

    AssertSql(
        """
INSERT INTO "dbo"."People" ("First Name")
VALUES (N'John');
SELECT @@ROW_COUNT;

""");
}

public override void InsertDataOperation_required_args_composite()
{
    base.InsertDataOperation_required_args_composite();

    AssertSql(
        """
INSERT INTO "dbo"."People" ("First Name", "Last Name")
VALUES (N'John', N'Snow');
SELECT @@ROW_COUNT;

""");
}

public override void InsertDataOperation_required_args_multiple_rows()
{
    base.InsertDataOperation_required_args_multiple_rows();

    AssertSql(
        """
INSERT INTO "dbo"."People" ("First Name")
VALUES (N'John');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Daenerys');
SELECT @@ROW_COUNT;

""");
}

[ConditionalFact]
        public virtual void InsertDataOperation_max_batch_size_is_respected()
{
    // The SQL Server max batch size is 42 by default
    var values = new object[50, 1];
    for (var i = 0; i < 50; i++)
    {
        values[i, 0] = "Foo" + i;
    }

    Generate(
        CreateGotModel,
        new InsertDataOperation
        {
            Table = "People",
            Columns = new[] { "First Name" },
            Values = values
        });

    AssertSql(
        """
INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo0');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo1');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo2');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo3');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo4');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo5');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo6');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo7');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo8');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo9');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo10');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo11');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo12');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo13');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo14');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo15');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo16');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo17');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo18');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo19');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo20');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo21');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo22');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo23');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo24');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo25');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo26');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo27');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo28');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo29');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo30');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo31');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo32');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo33');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo34');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo35');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo36');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo37');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo38');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo39');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo40');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo41');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo42');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo43');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo44');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo45');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo46');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo47');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo48');
SELECT @@ROW_COUNT;

INSERT INTO "dbo"."People" ("First Name")
VALUES (N'Foo49');
SELECT @@ROW_COUNT;


""");
}

public override void InsertDataOperation_throws_for_unsupported_column_types()
    => base.InsertDataOperation_throws_for_unsupported_column_types();

public override void DeleteDataOperation_all_args()
{
    base.DeleteDataOperation_all_args();

    AssertSql(
        """
DELETE FROM "People"
WHERE "First Name" = N'Hodor';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'John';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Arya';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Harry';
SELECT @@ROW_COUNT;

""");
}

public override void DeleteDataOperation_all_args_composite()
{
    base.DeleteDataOperation_all_args_composite();

    AssertSql(
        """
DELETE FROM "People"
WHERE "First Name" = N'Hodor' AND "Last Name" IS NULL;
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Daenerys' AND "Last Name" = N'Targaryen';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'John' AND "Last Name" = N'Snow';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Arya' AND "Last Name" = N'Stark';
SELECT @@ROW_COUNT;

DELETE FROM "People"
WHERE "First Name" = N'Harry' AND "Last Name" = N'Strickland';
SELECT @@ROW_COUNT;

""");
}

public override void DeleteDataOperation_required_args()
{
    base.DeleteDataOperation_required_args();

    AssertSql(
        """
DELETE FROM "People"
WHERE "Last Name" = N'Snow';
SELECT @@ROW_COUNT;

""");
}

public override void DeleteDataOperation_required_args_composite()
{
    base.DeleteDataOperation_required_args_composite();

    AssertSql(
        """
DELETE FROM "People"
WHERE "First Name" = N'John' AND "Last Name" = N'Snow';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_all_args()
{
    base.UpdateDataOperation_all_args();

    AssertSql(
        """
UPDATE "People" SET "Birthplace" = N'Winterfell', "House Allegiance" = N'Stark', "Culture" = N'Northmen'
WHERE "First Name" = N'Hodor';
SELECT @@ROW_COUNT;

UPDATE "People" SET "Birthplace" = N'Dragonstone', "House Allegiance" = N'Targaryen', "Culture" = N'Valyrian'
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_all_args_composite()
{
    base.UpdateDataOperation_all_args_composite();

    AssertSql(
        """
UPDATE "People" SET "House Allegiance" = N'Stark'
WHERE "First Name" = N'Hodor' AND "Last Name" IS NULL;
SELECT @@ROW_COUNT;

UPDATE "People" SET "House Allegiance" = N'Targaryen'
WHERE "First Name" = N'Daenerys' AND "Last Name" = N'Targaryen';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_all_args_composite_multi()
{
    base.UpdateDataOperation_all_args_composite_multi();

    AssertSql(
        """
UPDATE "People" SET "Birthplace" = N'Winterfell', "House Allegiance" = N'Stark', "Culture" = N'Northmen'
WHERE "First Name" = N'Hodor' AND "Last Name" IS NULL;
SELECT @@ROW_COUNT;

UPDATE "People" SET "Birthplace" = N'Dragonstone', "House Allegiance" = N'Targaryen', "Culture" = N'Valyrian'
WHERE "First Name" = N'Daenerys' AND "Last Name" = N'Targaryen';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_all_args_multi()
{
    base.UpdateDataOperation_all_args_multi();

    AssertSql(
        """
UPDATE "People" SET "Birthplace" = N'Dragonstone', "House Allegiance" = N'Targaryen', "Culture" = N'Valyrian'
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_required_args()
{
    base.UpdateDataOperation_required_args();

    AssertSql(
        """
UPDATE "People" SET "House Allegiance" = N'Targaryen'
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_required_args_composite()
{
    base.UpdateDataOperation_required_args_composite();

    AssertSql(
        """
UPDATE "People" SET "House Allegiance" = N'Targaryen'
WHERE "First Name" = N'Daenerys' AND "Last Name" = N'Targaryen';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_required_args_composite_multi()
{
    base.UpdateDataOperation_required_args_composite_multi();

    AssertSql(
        """
UPDATE "People" SET "Birthplace" = N'Dragonstone', "House Allegiance" = N'Targaryen', "Culture" = N'Valyrian'
WHERE "First Name" = N'Daenerys' AND "Last Name" = N'Targaryen';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_required_args_multi()
{
    base.UpdateDataOperation_required_args_multi();

    AssertSql(
        """
UPDATE "People" SET "Birthplace" = N'Dragonstone', "House Allegiance" = N'Targaryen', "Culture" = N'Valyrian'
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

""");
}

public override void UpdateDataOperation_required_args_multiple_rows()
{
    base.UpdateDataOperation_required_args_multiple_rows();

    AssertSql(
        """
UPDATE "People" SET "House Allegiance" = N'Stark'
WHERE "First Name" = N'Hodor';
SELECT @@ROW_COUNT;

UPDATE "People" SET "House Allegiance" = N'Targaryen'
WHERE "First Name" = N'Daenerys';
SELECT @@ROW_COUNT;

""");
}

public override void DefaultValue_with_line_breaks(bool isUnicode)
{
    base.DefaultValue_with_line_breaks(isUnicode);

    var storeType = isUnicode ? "long nvarchar" : "long varchar";
    var unicodePrefixForType = isUnicode ? "N" : string.Empty;
    var expectedSql = @$"CREATE TABLE ""dbo"".""TestLineBreaks"" (
    ""TestDefaultValue"" { storeType} NOT NULL WITH DEFAULT {unicodePrefixForType}'
Various Line
Breaks
'
);
    ";
            AssertSql(expectedSql);
}

public override void DefaultValue_with_line_breaks_2(bool isUnicode)
{
    base.DefaultValue_with_line_breaks_2(isUnicode);

    var storeType = isUnicode ? "long nvarchar" : "long varchar";
    var unicodePrefix = isUnicode ? "N" : string.Empty;
    var unicodePrefixForType = isUnicode ? "n" : string.Empty;
    var expectedSql = @$"CREATE TABLE ""dbo"".""TestLineBreaks"" (
    ""TestDefaultValue"" { storeType} NOT NULL WITH DEFAULT { unicodePrefix}'0
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
49
50
51
52
53
54
55
56
57
58
59
60
61
62
63
64
65
66
67
68
69
70
71
72
73
74
75
76
77
78
79
80
81
82
83
84
85
86
87
88
89
90
91
92
93
94
95
96
97
98
99
100
101
102
103
104
105
106
107
108
109
110
111
112
113
114
115
116
117
118
119
120
121
122
123
124
125
126
127
128
129
130
131
132
133
134
135
136
137
138
139
140
141
142
143
144
145
146
147
148
149
150
151
152
153
154
155
156
157
158
159
160
161
162
163
164
165
166
167
168
169
170
171
172
173
174
175
176
177
178
179
180
181
182
183
184
185
186
187
188
189
190
191
192
193
194
195
196
197
198
199
200
201
202
203
204
205
206
207
208
209
210
211
212
213
214
215
216
217
218
219
220
221
222
223
224
225
226
227
228
229
230
231
232
233
234
235
236
237
238
239
240
241
242
243
244
245
246
247
248
249
250
251
252
253
254
255
256
257
258
259
260
261
262
263
264
265
266
267
268
269
270
271
272
273
274
275
276
277
278
279
280
281
282
283
284
285
286
287
288
289
290
291
292
293
294
295
296
297
298
299
'
);
";
            AssertSql(expectedSql);
}

public override void Sequence_restart_operation(long? startsAt)
{
    base.Sequence_restart_operation(startsAt);

    var expectedSql = startsAt.HasValue
        ? @$"set session authorization ""dbo"";
GO

ALTER SEQUENCE ""dbo"".""TestRestartSequenceOperation"" RESTART WITH 3;"
                : @"set session authorization ""dbo"";
GO

ALTER SEQUENCE ""dbo"".""TestRestartSequenceOperation"" RESTART WITH NULL;";
    AssertSql(expectedSql);
}

[ConditionalFact]
public virtual void CreateIndex_generates_exec_when_legacy_filter_and_idempotent()
{
    Generate(
        modelBuilder =>
        {
            modelBuilder
                .HasAnnotation(CoreAnnotationNames.ProductVersion, "1.1.0")
                .Entity("Table1").Property<int?>("Column1");
        },
        migrationBuilder => migrationBuilder.CreateIndex(
            name: "IX_Table1_Column1",
            table: "Table1",
            column: "Column1",
            unique: true),
        MigrationsSqlGenerationOptions.Idempotent);

    AssertSql(
        """
CREATE INDEX "IX_Table1_Column1" ON "Table1" ("Column1") WHERE "Column1" IS NOT NULL;
""");
}

[ConditionalFact]
        public virtual void AlterColumn_make_required_with_idempotent()
{
    Generate(
        new AlterColumnOperation
        {
            Table = "Person",
            Name = "Name",
            ClrType = typeof(string),
            IsNullable = false,
            DefaultValue = "",
            OldColumn = new AddColumnOperation
            {
                Table = "Person",
                Name = "Name",
                ClrType = typeof(string),
                IsNullable = true
            }
        },
        MigrationsSqlGenerationOptions.Idempotent);

    AssertSql(
        """
ALTER TABLE "Person" ALTER COLUMN "Name" long nvarchar NOT NULL WITH DEFAULT N'';
GO

MODIFY "Person" TO RECONSTRUCT;

""");
}

private static void CreateGotModel(ModelBuilder b)
    => b.HasDefaultSchema("dbo").Entity(
        "Person", pb =>
        {
            pb.ToTable("People");
            pb.Property<string>("FirstName").HasColumnName("First Name");
            pb.Property<string>("LastName").HasColumnName("Last Name");
            pb.Property<string>("Birthplace").HasColumnName("Birthplace");
            pb.Property<string>("Allegiance").HasColumnName("House Allegiance");
            pb.Property<string>("Culture").HasColumnName("Culture");
            pb.HasKey("FirstName", "LastName");
        });

public ActianMigrationSqlGeneratorTest()
            : base(
                ActianTestHelpers.Instance,
                new ServiceCollection(),
                ActianTestHelpers.Instance.AddProviderOptions(
                    ((IRelationalDbContextOptionsBuilderInfrastructure)
                        new ActianDbContextOptionsBuilder(new DbContextOptionsBuilder()))
                        .OptionsBuilder).Options)
        {
}
    }
}
