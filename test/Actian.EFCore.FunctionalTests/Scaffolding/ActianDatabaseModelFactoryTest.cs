using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Actian.EFCore.Diagnostics.Internal;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.TestUtilities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Actian.EFCore.Scaffolding
{
#nullable enable

    [Collection("Test Collection")]
    public class ActianDatabaseModelFactoryTest : IClassFixture<ActianDatabaseModelFactoryTest.ActianDatabaseModelFixture>
    {
        protected ActianDatabaseModelFixture Fixture { get; }

        public ActianDatabaseModelFactoryTest(ActianDatabaseModelFixture fixture)
        {
            Fixture = fixture;
            Fixture.OperationReporter.Clear();
        }

        #region Sequences

        [ConditionalFact]
        public void Create_sequences_with_facets_maxvalue()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE SEQUENCE DefaultFacetsSequence;

CREATE SEQUENCE CustomFacetsSequence
    AS int
    START WITH -1
    INCREMENT BY 2
    MAXVALUE 100
    CYCLE;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var defaultSequence = dbModel.Sequences.First(ds => ds.Name == "defaultfacetssequence");
                    Assert.Equal("dbo", defaultSequence.Schema);
                    Assert.Equal("defaultfacetssequence", defaultSequence.Name);
                    Assert.Equal("bigint", defaultSequence.StoreType);
                    Assert.False(defaultSequence.IsCyclic);
                    Assert.Equal(1, defaultSequence.IncrementBy);
                    Assert.Null(defaultSequence.StartValue);
                    Assert.Null(defaultSequence.MinValue);
                    Assert.Null(defaultSequence.MaxValue);

                    var customSequence = dbModel.Sequences.First(ds => ds.Name == "customfacetssequence");
                    Assert.Equal("dbo", customSequence.Schema);
                    Assert.Equal("customfacetssequence", customSequence.Name);
                    Assert.Equal("integer", customSequence.StoreType);
                    Assert.True(customSequence.IsCyclic);
                    Assert.Equal(2, customSequence.IncrementBy);
                    Assert.Equal(-1, customSequence.StartValue);
                    Assert.Null(defaultSequence.MinValue);
                    Assert.Equal(100, customSequence.MaxValue);
                },
                @"
DROP SEQUENCE DefaultFacetsSequence;

DROP SEQUENCE CustomFacetsSequence");

        [ConditionalFact]
        public void Create_sequences_with_facets_minvalue()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE SEQUENCE DefaultFacetsSequence;

CREATE SEQUENCE CustomFacetsSequence
    AS int
    START WITH -2
    INCREMENT BY 2
    MINVALUE -3
    CYCLE;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var defaultSequence = dbModel.Sequences.First(ds => ds.Name == "defaultfacetssequence");
                    Assert.Equal("dbo", defaultSequence.Schema);
                    Assert.Equal("defaultfacetssequence", defaultSequence.Name);
                    Assert.Equal("bigint", defaultSequence.StoreType);
                    Assert.False(defaultSequence.IsCyclic);
                    Assert.Equal(1, defaultSequence.IncrementBy);
                    Assert.Null(defaultSequence.StartValue);
                    Assert.Null(defaultSequence.MinValue);
                    Assert.Null(defaultSequence.MaxValue);

                    var customSequence = dbModel.Sequences.First(ds => ds.Name == "customfacetssequence");
                    Assert.Equal("dbo", customSequence.Schema);
                    Assert.Equal("customfacetssequence", customSequence.Name);
                    Assert.Equal("integer", customSequence.StoreType);
                    Assert.True(customSequence.IsCyclic);
                    Assert.Equal(2, customSequence.IncrementBy);
                    Assert.Equal(-2, customSequence.StartValue);
                    Assert.Equal(-3, customSequence.MinValue);
                    Assert.Null(defaultSequence.MaxValue);
                },
                @"
DROP SEQUENCE DefaultFacetsSequence;

DROP SEQUENCE CustomFacetsSequence");

        [ConditionalFact]
        //AS data type
        //  Specifies the data type of the sequence as one of the following:
        //    INTEGER
        //    BIGINT
        //    DECIMAL(with an optional precision, but 0 scale)
        //  Default: INTEGER
        public void Sequence_min_max_start_values_are_null_if_default()
            => Test(
                @"

CREATE SEQUENCE IntSequence AS int;

CREATE SEQUENCE BigIntSequence AS bigint;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.All(
                        dbModel.Sequences,
                        s =>
                        {
                            Assert.Null(s.StartValue);
                            Assert.Null(s.MinValue);
                            Assert.Null(s.MaxValue);
                        });
                },
                @"
DROP SEQUENCE IntSequence;

DROP SEQUENCE BigIntSequence;");

        [ConditionalFact]
        public void Sequence_min_max_start_values_are_not_null_if_decimal()
            => Test(
                @"
CREATE SEQUENCE DecimalSequence AS decimal;

CREATE SEQUENCE NumericSequence AS numeric;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.All(
                        dbModel.Sequences,
                        s =>
                        {
                            Assert.NotNull(s.StartValue);
                            Assert.NotNull(s.MinValue);
                            Assert.NotNull(s.MaxValue);
                        });
                },
                @"
DROP SEQUENCE DecimalSequence;

DROP SEQUENCE NumericSequence;");

        //Ingres.Client.IngresException : CREATE SEQUENCE: Invalid data type for sequence definition.
        public void Sequence_high_min_max_start_values_are_not_null_if_decimal()
            => Test(
                @"
CREATE SEQUENCE HighDecimalSequence
 AS numeric(38, 0)
 START WITH -99999999999999999999999999999999999999
 INCREMENT BY 1
 MINVALUE -99999999999999999999999999999999999999
 MAXVALUE 99999999999999999999999999999999999999
 CACHE;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.All(
                        dbModel.Sequences,
                        s =>
                        {
                            Assert.NotNull(s.StartValue);
                            Assert.Equal(long.MinValue, s.StartValue);
                            Assert.NotNull(s.MinValue);
                            Assert.Equal(long.MinValue, s.MinValue);
                            Assert.NotNull(s.MaxValue);
                            Assert.Equal(long.MaxValue, s.MaxValue);
                        });
                },
                @"
DROP SEQUENCE [HighDecimalSequence];");

        //AS data type
        //  Specifies the data type of the sequence as one of the following:
        //    INTEGER
        //    BIGINT
        //    DECIMAL(with an optional precision, but 0 scale)
        //  Default: INTEGER
        public void Sequence_using_type_alias()
        {
            Fixture.TestStore.ExecuteNonQuery(
                @"
CREATE TYPE [TestTypeAlias] FROM int;");

            Test(
                @"
CREATE SEQUENCE [TypeAliasSequence] AS [TestTypeAlias];",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var sequence = Assert.Single(dbModel.Sequences);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", sequence.Schema);
                    Assert.Equal("TypeAliasSequence", sequence.Name);
                    Assert.Equal("int", sequence.StoreType);
                    Assert.False(sequence.IsCyclic);
                    Assert.Equal(1, sequence.IncrementBy);
                    Assert.Null(sequence.StartValue);
                    Assert.Null(sequence.MinValue);
                    Assert.Null(sequence.MaxValue);
                },
                @"
DROP SEQUENCE [TypeAliasSequence];
DROP TYPE [TestTypeAlias];");
        }

        [ConditionalFact]
        public void Sequence_using_type_with_facets()
            => Test(
                @"
CREATE SEQUENCE TypeFacetSequence AS decimal(10, 0);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var sequence = Assert.Single(dbModel.Sequences);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", sequence.Schema);
                    Assert.Equal("typefacetsequence", sequence.Name);
                    Assert.Equal("decimal(10)", sequence.StoreType);
                    Assert.False(sequence.IsCyclic);
                    Assert.Equal(1, sequence.IncrementBy);
                },
                @"
DROP SEQUENCE TypeFacetSequence;");

        [ConditionalFact]
        public void Filter_sequences_based_on_schema()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE SEQUENCE Sequence;

SET SESSION AUTHORIZATION db2;
CREATE SEQUENCE db2.Sequence",
                Enumerable.Empty<string>(),
                new[] { "db2" },
                (dbModel, scaffoldingFactory) =>
                {
                    var sequence = Assert.Single(dbModel.Sequences);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("db2", sequence.Schema);
                    Assert.Equal("sequence", sequence.Name);
                    Assert.Equal("bigint", sequence.StoreType);
                    Assert.False(sequence.IsCyclic);
                    Assert.Equal(1, sequence.IncrementBy);
                },
                @"
SET SESSION AUTHORIZATION dbo;
DROP SEQUENCE Sequence;

SET SESSION AUTHORIZATION db2;
DROP SEQUENCE db2.Sequence;");

        #endregion

        #region Model

        [ConditionalFact]
        public void Set_default_schema()
            => Test(
                "SET SESSION AUTHORIZATION dbo;SELECT 1",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var defaultSchema = Fixture.TestStore.ExecuteScalar<string>("SELECT DBMSINFO('SESSION_SCHEMA')");
                    Assert.Equal(defaultSchema, dbModel.DefaultSchema);
                },
                null);

        [ConditionalFact]
        public void Create_tables()
            => Test(
                @"
SET SESSION AUTHORIZATION db2;
CREATE TABLE Everest ( id int );

CREATE TABLE Denali ( id int );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Collection(
                        dbModel.Tables.OrderBy(t => t.Name),
                        d =>
                        {
                            Assert.Equal("db2", d.Schema);
                            Assert.Equal("denali", d.Name);
                        },
                        e =>
                        {
                            Assert.Equal("db2", e.Schema);
                            Assert.Equal("everest", e.Name);
                        });
                },
                @"
DROP TABLE Everest;

DROP TABLE Denali;");

        [ConditionalFact]
        public void Expose_join_table_when_interloper_reference()
            => Test(
                @"
CREATE TABLE BBlogs (Id int GENERATED ALWAYS AS IDENTITY CONSTRAINT PK_BBlogs PRIMARY KEY);
CREATE TABLE PPosts (Id int GENERATED ALWAYS AS IDENTITY CONSTRAINT PK_PPosts PRIMARY KEY);

CREATE TABLE BBlogPPosts (
    BBlogId int NOT NULL CONSTRAINT FK_BBlogPPosts_BBlogs REFERENCES BBlogs (Id) ON DELETE CASCADE,
    PPostId int NOT NULL CONSTRAINT FK_BBlogPPosts_PPosts REFERENCES PPosts (Id) ON DELETE CASCADE,
    CONSTRAINT PK_BBlogPPosts  PRIMARY KEY (BBlogId, PPostId));

CREATE TABLE LinkToBBlogPPosts (
    LinkId1 int NOT NULL,
    LinkId2 int NOT NULL,
    CONSTRAINT PK_LinkToBBlogPPosts PRIMARY KEY (LinkId1, LinkId2),
    CONSTRAINT FK_LinkToBBlogPPosts_BlogPosts FOREIGN KEY (LinkId1, LinkId2) REFERENCES BBlogPPosts (BBlogId, PPostId));
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Collection(
                        dbModel.Tables.OrderBy(t => t.Name),
                        t =>
                        {
                            Assert.Equal("dbo", t.Schema);
                            Assert.Equal("bblogpposts", t.Name);
                            Assert.Collection(t.Columns.OrderBy(c => c.Name),
                                c => Assert.Equal("bblogid", c.Name),
                                c => Assert.Equal("ppostid", c.Name));
                            Assert.Collection(t.ForeignKeys,
                                c =>
                                {
                                    Assert.Equal("bblogs", c.PrincipalTable.Name);
                                    Assert.Equal("bblogpposts", c.Table.Name);
                                    Assert.Collection(c.Columns, c => Assert.Equal("bblogid", c.Name));
                                },
                                c =>
                                {
                                    Assert.Equal("pposts", c.PrincipalTable.Name);
                                    Assert.Equal("bblogpposts", c.Table.Name);
                                    Assert.Collection(c.Columns, c => Assert.Equal("ppostid", c.Name));
                                });
                        },
                        t =>
                        {
                            Assert.Equal("dbo", t.Schema);
                            Assert.Equal("bblogs", t.Name);
                            Assert.Collection(t.Columns, c => Assert.Equal("id", c.Name));
                        },
                        t =>
                        {
                            Assert.Equal("dbo", t.Schema);
                            Assert.Equal("linktobblogpposts", t.Name);
                            Assert.Collection(t.Columns.OrderBy(c => c.Name),
                                c => Assert.Equal("linkid1", c.Name),
                                c => Assert.Equal("linkid2", c.Name));
                            Assert.Collection(t.ForeignKeys,
                                c =>
                                {
                                    Assert.Equal("bblogpposts", c.PrincipalTable.Name);
                                    Assert.Equal("linktobblogpposts", c.Table.Name);
                                    Assert.Collection(
                                        c.Columns,
                                        c => Assert.Equal("linkid1", c.Name),
                                        c => Assert.Equal("linkid2", c.Name));
                                });
                        },
                        t =>
                        {
                            Assert.Equal("dbo", t.Schema);
                            Assert.Equal("pposts", t.Name);
                            Assert.Collection(t.Columns, c => Assert.Equal("id", c.Name));
                        });

                    var model = scaffoldingFactory.Create(dbModel, new ModelReverseEngineerOptions());

                    Assert.Collection(
                        model.GetEntityTypes(),
                        e =>
                        {
                            Assert.Equal("Bblog", e.Name);
                            Assert.Collection(e.GetProperties(), p => Assert.Equal("Id", p.Name));
                            Assert.Empty(e.GetForeignKeys());
                            Assert.Empty(e.GetSkipNavigations());
                            Assert.Collection(e.GetNavigations(), p => Assert.Equal("Bblogpposts", p.Name));
                        },
                        e =>
                        {
                            Assert.Equal("Bblogppost", e.Name);
                            Assert.Collection(e.GetProperties(),
                                p => Assert.Equal("Bblogid", p.Name),
                                p => Assert.Equal("Ppostid", p.Name));
                            Assert.Collection(e.GetForeignKeys(),
                                k =>
                                {
                                    Assert.Equal("Bblog", k.PrincipalEntityType.Name);
                                    Assert.Equal("Bblogppost", k.DeclaringEntityType.Name);
                                    Assert.Collection(k.Properties, p => Assert.Equal("Bblogid", p.Name));
                                },
                                k =>
                                {
                                    Assert.Equal("Ppost", k.PrincipalEntityType.Name);
                                    Assert.Equal("Bblogppost", k.DeclaringEntityType.Name);
                                    Assert.Collection(k.Properties, p => Assert.Equal("Ppostid", p.Name));
                                });
                            Assert.Empty(e.GetSkipNavigations());
                            Assert.Collection(e.GetNavigations(),
                                p => Assert.Equal("Bblog", p.Name),
                                p => Assert.Equal("Linktobblogppost", p.Name),
                                p => Assert.Equal("Ppost", p.Name));
                        },
                        e =>
                        {
                            Assert.Equal("Linktobblogppost", e.Name);
                            Assert.Collection(e.GetProperties(),
                                p => Assert.Equal("Linkid1", p.Name),
                                p => Assert.Equal("Linkid2", p.Name));
                            Assert.Collection(e.GetForeignKeys(),
                                k =>
                                {
                                    Assert.Equal("Bblogppost", k.PrincipalEntityType.Name);
                                    Assert.Equal("Linktobblogppost", k.DeclaringEntityType.Name);
                                    Assert.Collection(k.Properties,
                                        p => Assert.Equal("Linkid1", p.Name),
                                        p => Assert.Equal("Linkid2", p.Name));
                                    Assert.Collection(k.PrincipalKey.Properties,
                                        p => Assert.Equal("Bblogid", p.Name),
                                        p => Assert.Equal("Ppostid", p.Name));
                                });
                            Assert.Empty(e.GetSkipNavigations());
                            Assert.Collection(e.GetNavigations(), p => Assert.Equal("Bblogppost", p.Name));
                        },
                        e =>
                        {
                            Assert.Equal("Ppost", e.Name);
                            Assert.Collection(e.GetProperties(), p => Assert.Equal("Id", p.Name));
                            Assert.Empty(e.GetForeignKeys());
                            Assert.Empty(e.GetSkipNavigations());
                            Assert.Collection(e.GetNavigations(), p => Assert.Equal("Bblogpposts", p.Name));
                        });
                },
                @"
DROP TABLE LinkToBBlogPPosts;
DROP TABLE BBlogPPosts;
DROP TABLE PPosts;
DROP TABLE BBlogs;");

        [ConditionalFact]
        public void Default_database_collation_is_not_scaffolded()
            => Test(
                @"",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) => Assert.Null(dbModel.Collation),
                @"");

        #endregion

        #region FilteringSchemaTable

        [ConditionalFact]
        public void Filter_schemas()
            => Test(
                @"
SET SESSION AUTHORIZATION db2;
CREATE TABLE db2.K2 ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION INITIAL_USER;
CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B));",
                Enumerable.Empty<string>(),
                new[] { "db2" },
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables[0];
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

SET SESSION AUTHORIZATION db2;
DROP TABLE db2.K2;");

        [ConditionalFact]
        public void Filter_tables()
            => Test(
                @"
CREATE TABLE K2 ( Id int, A varchar not null, UNIQUE (A ) );

CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B), FOREIGN KEY (B) REFERENCES K2 (A) );",
                new[] { "K2" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

DROP TABLE K2;");

        //E_US09E7 line 3, Syntax error on '"', p3 = '"'.  The correct syntax is:
        //    [result_variable =] EXECUTE PROCEDURE | CALLPROC proc_name
        //        [(parameter = value, ...)]
    
        public void Filter_tables_with_quote_in_name()
            => Test(
                @"
CREATE TABLE ""K2'"" ( Id int, A varchar not null, UNIQUE (A ) );

CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B), FOREIGN KEY (B) REFERENCES ""K2'"" (A) );",
                new[] { "K2'" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k2'", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

DROP TABLE ""K2'"";");

        [ConditionalFact]
        public void Filter_tables_with_qualified_name()
            => Test(
                @"
CREATE TABLE ""K.2"" ( Id int, A varchar not null, UNIQUE (A ) );

CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B) );",
                new[] { "\"K.2\"" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables[0];
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k.2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

DROP TABLE ""K.2"";");

        [ConditionalFact]
        public void Filter_tables_with_schema_qualified_name1()
            => Test(
                @"
CREATE TABLE K2 ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION db2;
CREATE TABLE db2.K2 ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION INITIAL_USER;
CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B) );",
                new[] { "dbo.k2" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

DROP TABLE K2;

SET SESSION AUTHORIZATION db2;
DROP TABLE db2.K2;");

        [ConditionalFact]
        public void Filter_tables_with_schema_qualified_name2()
            => Test(
                @"
CREATE TABLE ""K.2"" ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION ""db.2"";
CREATE TABLE ""db.2"".""K.2"" ( Id int, A varchar not null, UNIQUE (A ) );

CREATE TABLE ""db.2"".Kilimanjaro ( Id int, B varchar not null, UNIQUE (B) );",
                new[] { "\"db.2\".\"K.2\"" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k.2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE ""db.2"".Kilimanjaro;

SET SESSION AUTHORIZATION INITIAL_USER;
DROP TABLE ""K.2"";

SET SESSION AUTHORIZATION ""db.2"";
DROP TABLE ""db.2"".""K.2"";");

        [ConditionalFact]
        public void Filter_tables_with_schema_qualified_name3()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE ""K.2"" ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION db2;
CREATE TABLE db2.""K.2"" ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION dbo;
CREATE TABLE Kilimanjaro ( Id int, B varchar not null, UNIQUE (B) );",
                new[] { "dbo.\"K.2\"" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k.2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE Kilimanjaro;

DROP TABLE ""K.2"";

SET SESSION AUTHORIZATION db2;
DROP TABLE db2.""K.2"";");

        [ConditionalFact]
        public void Filter_tables_with_schema_qualified_name4()
            => Test(
                @"
CREATE TABLE K2 ( Id int, A varchar not null, UNIQUE (A ) );

SET SESSION AUTHORIZATION ""db.2"";
CREATE TABLE ""db.2"".K2 ( Id int, A varchar not null, UNIQUE (A ) );

CREATE TABLE ""db.2"".Kilimanjaro ( Id int, B varchar not null, UNIQUE (B) );",
                new[] { "\"db.2\".K2" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("k2", table.Name);
                    Assert.Equal(2, table.Columns.Count);
                    Assert.Equal(1, table.UniqueConstraints.Count);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE ""db.2"".Kilimanjaro;

SET SESSION AUTHORIZATION INITIAL_USER;
DROP TABLE K2;

SET SESSION AUTHORIZATION ""db.2"";
DROP TABLE ""db.2"".K2;");

        [ConditionalFact]
        public void Complex_filtering_validation()
            =>Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE SEQUENCE dbo.sequence;
SET SESSION AUTHORIZATION db2;
CREATE SEQUENCE db2.sequence;

SET SESSION AUTHORIZATION ""db.2"";
CREATE TABLE ""db.2"".QuotedTableName ( Id int PRIMARY KEY );
CREATE TABLE ""db.2"".""Table.With.Dot"" ( Id int PRIMARY KEY );
CREATE TABLE ""db.2"".SimpleTableName ( Id int PRIMARY KEY );
CREATE TABLE ""db.2"".JustTableName ( Id int PRIMARY KEY );

SET SESSION AUTHORIZATION INITIAL_USER;
CREATE TABLE QuotedTableName ( Id int PRIMARY KEY );
CREATE TABLE ""Table.With.Dot"" ( Id int PRIMARY KEY );
CREATE TABLE SimpleTableName ( Id int PRIMARY KEY );
CREATE TABLE JustTableName ( Id int PRIMARY KEY );

SET SESSION AUTHORIZATION db2;
CREATE TABLE db2.QuotedTableName ( Id int PRIMARY KEY );
CREATE TABLE db2.""Table.With.Dot"" ( Id int PRIMARY KEY );
CREATE TABLE db2.SimpleTableName ( Id int PRIMARY KEY );
CREATE TABLE db2.JustTableName ( Id int PRIMARY KEY );

CREATE TABLE db2.PrincipalTable (
    Id int PRIMARY KEY,
    UC1 nvarchar(450) not null,
    UC2 int not null,
    Index1 tinyint,
    Index2 bigint,
    CONSTRAINT UX UNIQUE (UC1, UC2)
);

CREATE INDEX IX_COMPOSITE ON db2.PrincipalTable ( Index2, Index1 );

CREATE TABLE db2.DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId1 nvarchar(450),
    ForeignKeyId2 int,
    FOREIGN KEY (ForeignKeyId1, ForeignKeyId2) REFERENCES db2.PrincipalTable(UC1, UC2) ON DELETE CASCADE
);",
                new[] { "\"db.2\".QuotedTableName", "\"db.2\".SimpleTableName", "dbo.\"Table.With.Dot\"", "dbo.SimpleTableName", "JustTableName" },
                new[] { "db2" },
                (dbModel, scaffoldingFactory) =>
                {
                    var sequence = Assert.Single(dbModel.Sequences);
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("db2", sequence.Schema);

                    Assert.Single(dbModel.Tables, t => t is { Schema: "db.2", Name: "quotedtablename" });
                    Assert.DoesNotContain(dbModel.Tables, t => t is { Schema: "db.2", Name: "table.with.dot" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "db.2", Name: "simpletablename" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "db.2", Name: "justtablename" });

                    Assert.DoesNotContain(dbModel.Tables, t => t is { Schema: "dbo", Name: "quotedtablename" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "dbo", Name: "table.with.dot" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "dbo", Name: "simpletablename" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "dbo", Name: "justtablename" });

                    Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "quotedtablename" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "table.with.dot" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "simpletablename" });
                    Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "justtablename" });

                    var principalTable = Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "principaltable" });
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.NotNull(principalTable.PrimaryKey);
                    Assert.Single(principalTable.UniqueConstraints);
                    Assert.Single(principalTable.Indexes);

                    var dependentTable = Assert.Single(dbModel.Tables, t => t is { Schema: "db2", Name: "dependenttable" });
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Single(dependentTable.ForeignKeys);
                },
                @"
SET SESSION AUTHORIZATION dbo;
DROP SEQUENCE dbo.sequence;
SET SESSION AUTHORIZATION db2;
DROP SEQUENCE db2.sequence;

SET SESSION AUTHORIZATION ""db.2"";
DROP TABLE ""db.2"".QuotedTableName;
DROP TABLE ""db.2"".""Table.With.Dot"";
DROP TABLE ""db.2"".SimpleTableName;
DROP TABLE ""db.2"".JustTableName;

SET SESSION AUTHORIZATION INITIAL_USER;
DROP TABLE QuotedTableName;
DROP TABLE ""Table.With.Dot"";
DROP TABLE SimpleTableName;
DROP TABLE JustTableName;

SET SESSION AUTHORIZATION db2;
DROP TABLE db2.QuotedTableName;
DROP TABLE db2.""Table.With.Dot"";
DROP TABLE db2.SimpleTableName;
DROP TABLE db2.JustTableName;
DROP TABLE db2.DependentTable;
DROP TABLE db2.PrincipalTable;");

        #endregion

        #region Table

        [TestUtilities.ActianCondition(ActianCondition.SupportsMemoryOptimized)]
        public void Set_memory_optimized_table_annotation()
            => Test(
                @"
IF SERVERPROPERTY('IsXTPSupported') = 1 AND SERVERPROPERTY('EngineEdition') <> 5
BEGIN
IF NOT EXISTS (
    SELECT 1 FROM sys.filegroups FG JOIN sys.database_files F ON FG.data_space_id = F.data_space_id WHERE FG.type = N'FX' AND F.type = 2)
    BEGIN
    DECLARE @db_name nvarchar(max) = DB_NAME();
    DECLARE @fg_name nvarchar(max);
    SELECT TOP(1) @fg_name = name FROM sys.filegroups WHERE type = N'FX';

    IF @fg_name IS NULL
        BEGIN
        SET @fg_name = @db_name + N'_MODFG';
        EXEC(N'ALTER DATABASE CURRENT ADD FILEGROUP ' + @fg_name + ' CONTAINS MEMORY_OPTIMIZED_DATA;');
        END

    DECLARE @path nvarchar(max);
    SELECT TOP(1) @path = physical_name FROM sys.database_files WHERE charindex('\', physical_name) > 0 ORDER BY file_id;
    IF (@path IS NULL)
        SET @path = '\' + @db_name;

    DECLARE @filename nvarchar(max) = right(@path, charindex('\', reverse(@path)) - 1);
    SET @filename = REPLACE(left(@filename, len(@filename) - charindex('.', reverse(@filename))), '''', '''''') + N'_MOD';
    DECLARE @new_path nvarchar(max) = REPLACE(CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(max)), '''', '''''') + @filename;

    EXEC(N'
        ALTER DATABASE CURRENT
        ADD FILE (NAME=''' + @filename + ''', filename=''' + @new_path + ''')
        TO FILEGROUP ' + @fg_name + ';')
    END
END

IF SERVERPROPERTY('IsXTPSupported') = 1
EXEC(N'ALTER DATABASE CURRENT SET MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT ON;');

CREATE TABLE Blogs (
    Id int NOT NULL IDENTITY,
    CONSTRAINT PK_Blogs PRIMARY KEY NONCLUSTERED (Id)
) WITH (MEMORY_OPTIMIZED = ON);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.Single(dbModel.Tables, t => t.Name == "blogs");

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.True((bool)table[ActianAnnotationNames.MemoryOptimized]!);
                },
                "DROP TABLE Blogs");

        [ConditionalFact]
        public void Create_columns()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE Blogs (
    Id int,
    Name nvarchar(100) NOT NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables[0];

                    Assert.Equal(2, table.Columns.Count);
                    Assert.All(
                        table.Columns, c =>
                        {
                            Assert.Equal("dbo", c.Table.Schema);
                            Assert.Equal("blogs", c.Table.Name);
                        });

                    Assert.Single(table.Columns, c => c.Name == "id");
                    Assert.Single(table.Columns, c => c.Name == "name");
                },
                "DROP TABLE Blogs");

        [ConditionalFact]
        public void Create_view_columns()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE VIEW BlogsView
 AS
SELECT
 CAST(100 AS int) AS Id,
 CAST(N'' AS nvarchar(100)) AS Name;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = Assert.IsType<DatabaseView>(dbModel.Tables[0]);

                    Assert.Equal(2, table.Columns.Count);
                    Assert.Null(table.PrimaryKey);
                    Assert.All(
                        table.Columns, c =>
                        {
                            Assert.Equal("dbo", c.Table.Schema);
                            Assert.Equal("blogsview", c.Table.Name);
                        });

                    Assert.Single(table.Columns, c => c.Name == "id");
                    Assert.Single(table.Columns, c => c.Name == "name");
                },
                "DROP VIEW BlogsView;");

        [ConditionalFact]
        public void Create_primary_key()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE PrimaryKeyTable (
    Id int PRIMARY KEY
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables[0].PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("primarykeytable", pk.Table.Name);
                    Assert.StartsWith("$prima_", pk.Name);
                    Assert.Null(pk[ActianAnnotationNames.Clustered]);
                    Assert.Equal(
                        new List<string> { "id" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE PrimaryKeyTable;");

        [ConditionalFact]
        public void Create_unique_constraints()
            => Test(
                @"
CREATE TABLE UniqueConstraint (
    Id int,
    Name int UNIQUE NOT NULL,
    IndexProperty int
);

CREATE INDEX IX_INDEX on UniqueConstraint ( IndexProperty );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var uniqueConstraint = Assert.Single(dbModel.Tables[0].UniqueConstraints);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", uniqueConstraint.Table.Schema);
                    Assert.Equal("uniqueconstraint", uniqueConstraint.Table.Name);
                    Assert.StartsWith("$uniqu_", uniqueConstraint.Name);
                    Assert.Null(uniqueConstraint[ActianAnnotationNames.Clustered]);
                    Assert.Equal(
                        new List<string> { "name" }, uniqueConstraint.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE UniqueConstraint;");

        [ConditionalFact]
        public void Create_indexes()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE IndexTable (
    Id int,
    Name int,
    IndexProperty int
);

CREATE INDEX IX_NAME on IndexTable ( Name );
CREATE INDEX IX_INDEX on IndexTable ( IndexProperty );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables[0];

                    Assert.Equal(2, table.Indexes.Count);
                    Assert.All(
                        table.Indexes, c =>
                        {
                            Assert.Equal("dbo", c.Table!.Schema);
                            Assert.Equal("indextable", c.Table.Name);
                        });

                    Assert.Single(table.Indexes, c => c.Name == "ix_name");
                    Assert.Single(table.Indexes, c => c.Name == "ix_index");
                },
                "DROP TABLE IndexTable;");

        [ConditionalFact]
        public void Create_multiple_indexes_on_same_column()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE IndexTable (
    Id int,
    IndexProperty int
);

CREATE INDEX IX_One on IndexTable ( IndexProperty );
CREATE INDEX IX_Two on IndexTable ( IndexProperty );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables[0];

                    Assert.Equal(2, table.Indexes.Count);
                    Assert.All(
                        table.Indexes, c =>
                        {
                            Assert.Equal("dbo", c.Table!.Schema);
                            Assert.Equal("indextable", c.Table.Name);
                        });

                    Assert.Collection(
                        table.Indexes.OrderBy(i => i.Name),
                        index =>
                        {
                            Assert.Equal("ix_one", index.Name);
                        },
                        index =>
                        {
                            Assert.Equal("ix_two", index.Name);
                        });
                },
                "DROP TABLE IndexTable;");

        [ConditionalFact]
        public void Create_foreign_keys()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE FirstDependent (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);

CREATE TABLE SecondDependent (
    Id int PRIMARY KEY,
    FOREIGN KEY (Id) REFERENCES PrincipalTable(Id) ON DELETE NO ACTION
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var firstFk = Assert.Single(dbModel.Tables.Single(t => t.Name == "firstdependent").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", firstFk.Table.Schema);
                    Assert.Equal("firstdependent", firstFk.Table.Name);
                    Assert.Equal("dbo", firstFk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", firstFk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid" }, firstFk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, firstFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, firstFk.OnDelete);

                    var secondFk = Assert.Single(dbModel.Tables.Single(t => t.Name == "seconddependent").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", secondFk.Table.Schema);
                    Assert.Equal("seconddependent", secondFk.Table.Name);
                    Assert.Equal("dbo", secondFk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", secondFk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "id" }, secondFk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, secondFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.NoAction, secondFk.OnDelete);
                },
                @"
DROP TABLE SecondDependent;
DROP TABLE FirstDependent;
DROP TABLE PrincipalTable;");

        //Triggers are not currently supported
        public void Triggers()
            => Test(
                new[]
                {
                @"
CREATE TABLE SomeTable (
    Id int IDENTITY PRIMARY KEY,
    Foo int,
    Bar int,
    Baz int
);",
                @"
CREATE TRIGGER Trigger1
    ON SomeTable
    AFTER INSERT AS
BEGIN
    UPDATE SomeTable SET Bar=Foo WHERE Id IN (SELECT INSERTED.Id FROM INSERTED);
END;",
                @"
CREATE TRIGGER Trigger2
    ON SomeTable
    AFTER INSERT AS
BEGIN
    UPDATE SomeTable SET Baz=Foo WHERE Id IN (SELECT INSERTED.Id FROM INSERTED);
END;"
                },
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var table = dbModel.Tables.Single();
                    var triggers = table.Triggers;

                    Assert.Collection(
                        triggers.OrderBy(t => t.Name),
                        t => Assert.Equal("Trigger1", t.Name),
                        t => Assert.Equal("Trigger2", t.Name));
                },
                "DROP TABLE SomeTable;");

        #endregion

        #region ColumnFacets

        public void Column_with_type_alias_assigns_underlying_store_type()
        {
            Fixture.TestStore.ExecuteNonQuery(
                @"
CREATE TYPE dbo.TestTypeAlias FROM nvarchar(max);
CREATE TYPE db2.TestTypeAlias FROM int;");

            Test(
                @"
CREATE TABLE TypeAlias (
    Id int,
    typeAliasColumn dbo.TestTypeAlias NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var column = Assert.Single(dbModel.Tables.Single().Columns, c => c.Name == "typeAliasColumn");

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("nvarchar(max)", column.StoreType);
                },
                @"
DROP TABLE TypeAlias;
DROP TYPE dbo.TestTypeAlias;
DROP TYPE db2.TestTypeAlias;");
        }

        public void Column_with_sysname_assigns_underlying_store_type_and_nullability()
            => Test(
                @"
CREATE TABLE TypeAlias (
    Id int,
    typeAliasColumn sysname
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var column = Assert.Single(dbModel.Tables.Single().Columns, c => c.Name == "typeAliasColumn");

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("nvarchar(128)", column.StoreType);
                    Assert.False(column.IsNullable);
                },
                @"
DROP TABLE TypeAlias;");

        [ConditionalFact]
        public void Decimal_numeric_types_have_precision_scale()
            => Test(
                @"
CREATE TABLE NumericColumns (
    Id int,
    decimalColumn decimal NOT NULL,
    decimal105Column decimal(10, 5) NOT NULL,
    decimalDefaultColumn decimal(18, 2) NOT NULL,
    numericColumn numeric NOT NULL,
    numeric152Column numeric(15, 2) NOT NULL,
    numericDefaultColumn numeric(18, 2) NOT NULL,
    numericDefaultPrecisionColumn numeric(38, 5) NOT NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("decimal", columns.Single(c => c.Name == "decimalcolumn").StoreType);
                    Assert.Equal("decimal(10,5)", columns.Single(c => c.Name == "decimal105column").StoreType);
                    Assert.Equal("decimal(18,2)", columns.Single(c => c.Name == "decimaldefaultcolumn").StoreType);
                    Assert.Equal("decimal", columns.Single(c => c.Name == "numericcolumn").StoreType);
                    Assert.Equal("decimal(15,2)", columns.Single(c => c.Name == "numeric152column").StoreType);
                    Assert.Equal("decimal(18,2)", columns.Single(c => c.Name == "numericdefaultcolumn").StoreType);
                    Assert.Equal("decimal(38,5)", columns.Single(c => c.Name == "numericdefaultprecisioncolumn").StoreType);
                },
                "DROP TABLE NumericColumns;");

        public void Max_length_of_negative_one_translate_to_max_in_store_type()
            => Test(
                @"
CREATE TABLE MaxColumns (
    Id int,
    varcharMaxColumn varchar(max) NULL,
    nvarcharMaxColumn nvarchar(max) NULL,
    varbinaryMaxColumn varbinary(max) NULL,
    binaryVaryingMaxColumn binary varying(max) NULL,
    charVaryingMaxColumn char varying(max) NULL,
    characterVaryingMaxColumn character varying(max) NULL,
    nationalCharVaryingMaxColumn national char varying(max) NULL,
    nationalCharacterVaryingMaxColumn national char varying(max) NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("varchar(max)", columns.Single(c => c.Name == "varcharMaxColumn").StoreType);
                    Assert.Equal("nvarchar(max)", columns.Single(c => c.Name == "nvarcharMaxColumn").StoreType);
                    Assert.Equal("varbinary(max)", columns.Single(c => c.Name == "varbinaryMaxColumn").StoreType);
                    Assert.Equal("varbinary(max)", columns.Single(c => c.Name == "binaryVaryingMaxColumn").StoreType);
                    Assert.Equal("varchar(max)", columns.Single(c => c.Name == "charVaryingMaxColumn").StoreType);
                    Assert.Equal("nvarchar(max)", columns.Single(c => c.Name == "nationalCharVaryingMaxColumn").StoreType);
                    Assert.Equal("nvarchar(max)", columns.Single(c => c.Name == "nationalCharacterVaryingMaxColumn").StoreType);
                },
                "DROP TABLE MaxColumns;");

        [ConditionalFact]
        public void Specific_max_length_are_add_to_store_type()
            => Test(
                @"
CREATE TABLE LengthColumns (
    Id int,
    char10Column char(10) NULL,
    varchar66Column varchar(66) NULL,
    nchar99Column nchar(99) NULL,
    nvarchar100Column nvarchar(100) NULL,
    binary111Column binary(111) NULL,
    character155Column character(155) NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("char(10)", columns.Single(c => c.Name == "char10column").StoreType);
                    Assert.Equal("varchar(66)", columns.Single(c => c.Name == "varchar66column").StoreType);
                    Assert.Equal("nchar(99)", columns.Single(c => c.Name == "nchar99column").StoreType);
                    Assert.Equal("nvarchar(100)", columns.Single(c => c.Name == "nvarchar100column").StoreType);
                    Assert.Equal("byte(111)", columns.Single(c => c.Name == "binary111column").StoreType);
                    Assert.Equal("char(155)", columns.Single(c => c.Name == "character155column").StoreType);
                },
                "DROP TABLE LengthColumns;");

        [ConditionalFact]
        public void Default_max_length_are_added_to_binary_varbinary()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthBinaryColumns (
    Id int,
    binaryColumn byte(8000),
    varbinaryColumn varbyte(8000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("byte(8000)", columns.Single(c => c.Name == "binarycolumn").StoreType);
                    Assert.Equal("byte varying(8000)", columns.Single(c => c.Name == "varbinarycolumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthBinaryColumns;");

        [ConditionalFact]
        public void Default_max_length_are_added_to_char_1()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthCharColumns (
    Id int,
    charColumn char(8000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("char(8000)", columns.Single(c => c.Name == "charcolumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthCharColumns;");

        [ConditionalFact]
        public void Default_max_length_are_added_to_char_2()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthCharColumns (
    Id int,
    characterColumn character(8000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("char(8000)", columns.Single(c => c.Name == "charactercolumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthCharColumns;");

        public void Default_max_length_are_added_to_varchar()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthVarcharColumns (
    Id int,
    charVaryingColumn char varying(8000),
    characterVaryingColumn character varying(8000),
    varcharColumn varchar(8000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("varchar(8000)", columns.Single(c => c.Name == "charVaryingColumn").StoreType);
                    Assert.Equal("varchar(8000)", columns.Single(c => c.Name == "characterVaryingColumn").StoreType);
                    Assert.Equal("varchar(8000)", columns.Single(c => c.Name == "varcharColumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthVarcharColumns;");

        public void Default_max_length_are_added_to_nchar_1()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthNcharColumns (
    Id int,
    nationalCharColumn national char(4000),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("nchar(4000)", columns.Single(c => c.Name == "nationalCharColumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthNcharColumns;");

        public void Default_max_length_are_added_to_nchar_2()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthNcharColumns (
    Id int,
    nationalCharacterColumn national character(4000),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("nchar(4000)", columns.Single(c => c.Name == "nationalCharacterColumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthNcharColumns;");

        [ConditionalFact]
        public void Default_max_length_are_added_to_nchar_3()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthNcharColumns (
    Id int,
    ncharColumn nchar(4000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("nchar(4000)", columns.Single(c => c.Name == "ncharcolumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthNcharColumns;");

        public void Default_max_length_are_added_to_nvarchar()
            => Test(
                @"
CREATE TABLE DefaultRequiredLengthNvarcharColumns (
    Id int,
    nationalCharVaryingColumn national char varying(4000),
    nationalCharacterVaryingColumn national character varying(4000),
    nvarcharColumn nvarchar(4000)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("nvarchar(4000)", columns.Single(c => c.Name == "nationalCharVaryingColumn").StoreType);
                    Assert.Equal("nvarchar(4000)", columns.Single(c => c.Name == "nationalCharacterVaryingColumn").StoreType);
                    Assert.Equal("nvarchar(4000)", columns.Single(c => c.Name == "nvarcharColumn").StoreType);
                },
                "DROP TABLE DefaultRequiredLengthNvarcharColumns;");

        [ConditionalFact]
        public void Datetime_types_have_precision_if_non_null_scale()
            => Test(
                @"
CREATE TABLE LengthColumns (
    Id int,
    time4Column time(4) NULL,
    timestamp4Column timestamp(4) NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;

                    Assert.Equal("time(4) without time zone", columns.Single(c => c.Name == "time4column").StoreType);
                    Assert.Equal("timestamp(4) without time zone", columns.Single(c => c.Name == "timestamp4column").StoreType);
                },
                "DROP TABLE LengthColumns;");

        [ConditionalFact]
        public void Types_with_required_length_uses_length_of_one()
            => Test(
                @"
CREATE TABLE OneLengthColumns (
    Id int,
    binaryColumn binary NULL,
    characterColumn character NULL,
    charColumn char NULL,
    ncharColumn nchar NULL,
    nvarcharColumn nvarchar NULL,
    varcharColumn varchar NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal("byte(1)", columns.Single(c => c.Name == "binarycolumn").StoreType);
                    Assert.Equal("char(1)", columns.Single(c => c.Name == "charactercolumn").StoreType);
                    Assert.Equal("char(1)", columns.Single(c => c.Name == "charcolumn").StoreType);
                    Assert.Equal("nchar(1)", columns.Single(c => c.Name == "ncharcolumn").StoreType);
                    Assert.Equal("nvarchar(1)", columns.Single(c => c.Name == "nvarcharcolumn").StoreType);
                    Assert.Equal("varchar(1)", columns.Single(c => c.Name == "varcharcolumn").StoreType);
                },
                "DROP TABLE OneLengthColumns;");

        [ConditionalFact]
        public void Store_types_without_any_facets()
            => Test(
                @"
CREATE TABLE NoFacetTypes (
    Id int,
    bigintColumn bigint NOT NULL,
    byteColumn byte NOT NULL,
    dateColumn date NOT NULL,
    timeColumn time NULL,
    time4Column time(4) NULL,
    time5Column time(5) NULL,
    time9Column time(9) NULL,
    floatColumn float NOT NULL,
    intColumn int NOT NULL,
    smallintColumn smallint NOT NULL,
    textColumn text NULL,
    timestampColumn timestamp NULL,
    timestamp4Column timestamp(4) NULL,
    timestamp5Column timestamp(5) NULL,
    timestamp9Column timestamp(9) NULL,
    tinyintColumn tinyint NOT NULL,
    uniqueidentifierColumn uuid NULL
)",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single(t => t.Name == "nofacettypes").Columns;

                    Assert.Equal("bigint", columns.Single(c => c.Name == "bigintcolumn").StoreType);
                    Assert.Equal("byte(1)", columns.Single(c => c.Name == "bytecolumn").StoreType);
                    Assert.Equal("ansidate", columns.Single(c => c.Name == "datecolumn").StoreType);
                    Assert.Equal("time without time zone", columns.Single(c => c.Name == "timecolumn").StoreType);
                    Assert.Equal("time(4) without time zone", columns.Single(c => c.Name == "time4column").StoreType);
                    Assert.Equal("time(5) without time zone", columns.Single(c => c.Name == "time5column").StoreType);
                    Assert.Equal("time(9) without time zone", columns.Single(c => c.Name == "time9column").StoreType);
                    Assert.Equal("float", columns.Single(c => c.Name == "floatcolumn").StoreType);
                    Assert.Equal("integer", columns.Single(c => c.Name == "intcolumn").StoreType);
                    Assert.Equal("smallint", columns.Single(c => c.Name == "smallintcolumn").StoreType);
                    Assert.Equal("text", columns.Single(c => c.Name == "textcolumn").StoreType);
                    Assert.Equal("timestamp without time zone", columns.Single(c => c.Name == "timestampcolumn").StoreType);
                    Assert.Equal("timestamp(4) without time zone", columns.Single(c => c.Name == "timestamp4column").StoreType);
                    Assert.Equal("timestamp(5) without time zone", columns.Single(c => c.Name == "timestamp5column").StoreType);
                    Assert.Equal("timestamp(9) without time zone", columns.Single(c => c.Name == "timestamp9column").StoreType);
                    Assert.Equal("tinyint", columns.Single(c => c.Name == "tinyintcolumn").StoreType);
                    Assert.Equal("uuid", columns.Single(c => c.Name == "uniqueidentifiercolumn").StoreType);
                },
                @"
DROP TABLE NoFacetTypes;");

        public void Default_and_computed_values_are_stored()
            => Test(
                @"
CREATE TABLE DefaultComputedValues (
    Id int,
    FixedDefaultValue datetime2 NOT NULL DEFAULT ('October 20, 2015 11am'),
    ComputedValue AS GETDATE(),
    A int NOT NULL,
    B int NOT NULL,
    SumOfAAndB AS A + B,
    SumOfAAndBPersisted AS A + B PERSISTED,
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var fixedDefaultValue = columns.Single(c => c.Name == "FixedDefaultValue");
                    Assert.Equal("('October 20, 2015 11am')", fixedDefaultValue.DefaultValueSql);
                    Assert.Null(fixedDefaultValue.ComputedColumnSql);

                    var computedValue = columns.Single(c => c.Name == "ComputedValue");
                    Assert.Null(computedValue.DefaultValueSql);
                    Assert.Equal("(getdate())", computedValue.ComputedColumnSql);

                    var sumOfAAndB = columns.Single(c => c.Name == "SumOfAAndB");
                    Assert.Null(sumOfAAndB.DefaultValueSql);
                    Assert.Equal("([A]+[B])", sumOfAAndB.ComputedColumnSql);
                    Assert.False(sumOfAAndB.IsStored);

                    var sumOfAAndBPersisted = columns.Single(c => c.Name == "SumOfAAndBPersisted");
                    Assert.Null(sumOfAAndBPersisted.DefaultValueSql);
                    Assert.Equal("([A]+[B])", sumOfAAndBPersisted.ComputedColumnSql);
                    Assert.True(sumOfAAndBPersisted.IsStored);
                },
                "DROP TABLE DefaultComputedValues;");

        public void Non_literal_bool_default_values_are_passed_through()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A bit DEFAULT (CHOOSE(1, 0, 1, 2)),
    B bit DEFAULT ((CONVERT([bit],(CHOOSE(1, 0, 1, 2))))),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("(choose((1),(0),(1),(2)))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("(CONVERT([bit],choose((1),(0),(1),(2))))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_int_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A int DEFAULT -1,
    B int DEFAULT 0
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("-1", column.DefaultValueSql);
                    Assert.Equal(-1, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal(0, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_short_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A smallint DEFAULT -1,
    B smallint DEFAULT 0
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("-1", column.DefaultValueSql);
                    Assert.Equal((short)-1, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal((short)0, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_long_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A bigint DEFAULT -1,
    B bigint DEFAULT 0
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("-1", column.DefaultValueSql);
                    Assert.Equal((long)-1, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal((long)0, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_byte_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A Boolean DEFAULT 1,
    B Boolean DEFAULT 0
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("1", column.DefaultValueSql);
                    Assert.Equal(Convert.ToBoolean(1), column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal(Convert.ToBoolean(0), column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        // Choose() and Convert() are not supported
        public void Non_literal_int_default_values_are_passed_through()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A int DEFAULT (CHOOSE(1, 0, 1, 2)),
    B int DEFAULT ((CONVERT([int],(CHOOSE(1, 0, 1, 2))))),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("(choose((1),(0),(1),(2)))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("(CONVERT([int],choose((1),(0),(1),(2))))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_double_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A float DEFAULT -1.1111,
    B float DEFAULT 0.0,
    C float DEFAULT 1.1000000000000001e+000,
    D float DEFAULT 1.1234
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("-1.1111", column.DefaultValueSql);
                    Assert.Equal(-1.1111, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal((double)0, column.DefaultValue);

                    column = columns.Single(c => c.Name == "c");
                    Assert.Equal("1.1000000000000001e+000", column.DefaultValueSql);
                    Assert.Equal(1.1000000000000001e+000, column.DefaultValue);

                    column = columns.Single(c => c.Name == "d");
                    Assert.Equal("1.1234", column.DefaultValueSql);
                    Assert.Equal(1.1234, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        //real - not supported in Ingres
        public void Simple_float_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A real DEFAULT -1.1111,
    B real DEFAULT (0.0),
    C real DEFAULT (1.1000000000000001e+000),
    D real DEFAULT ((CONVERT ( ""real"", ( (1.1234) ) ))),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("((-1.1111))", column.DefaultValueSql);
                    Assert.Equal((float)-1.1111, column.DefaultValue);

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("((0.0))", column.DefaultValueSql);
                    Assert.Equal((float)0, column.DefaultValue);

                    column = columns.Single(c => c.Name == "C");
                    Assert.Equal("((1.1000000000000001e+000))", column.DefaultValueSql);
                    Assert.Equal((float)1.1000000000000001e+000, column.DefaultValue);

                    column = columns.Single(c => c.Name == "D");
                    Assert.Equal("(CONVERT([real],(1.1234)))", column.DefaultValueSql);
                    Assert.Equal((float)1.1234, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_decimal_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A decimal(10,5) DEFAULT -1.1111,
    B decimal(10,5) DEFAULT 0.0,
    C decimal(10,5) DEFAULT 0,
    D decimal(10,5) DEFAULT 1.1234
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("-1.1111", column.DefaultValueSql);
                    Assert.Equal((decimal)-1.1111, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal((decimal)0, column.DefaultValue);

                    column = columns.Single(c => c.Name == "c");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal((decimal)0, column.DefaultValue);

                    column = columns.Single(c => c.Name == "d");
                    Assert.Equal("1.1234", column.DefaultValueSql);
                    Assert.Equal((decimal)1.1234, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_bool_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A Boolean DEFAULT 0,
    B Boolean DEFAULT 1,
    C Boolean DEFAULT FaLse,
    D Boolean DEFAULT tRuE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("0", column.DefaultValueSql);
                    Assert.Equal(false, column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("1", column.DefaultValueSql);
                    Assert.Equal(true, column.DefaultValue);

                    column = columns.Single(c => c.Name == "c");
                    Assert.Equal("FALSE", column.DefaultValueSql);
                    Assert.Equal(false, column.DefaultValue);

                    column = columns.Single(c => c.Name == "d");
                    Assert.Equal("TRUE", column.DefaultValueSql);
                    Assert.Equal(true, column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_DateTime_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A timestamp DEFAULT '1973-09-03T12:00:01.0020000',
    B time DEFAULT '12:12:12'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("'1973-09-03T12:00:01.0020000'", column.DefaultValueSql);
                    Assert.Equal(new DateTime(1973, 9, 3, 12, 0, 1, 2, DateTimeKind.Unspecified), column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("'12:12:12'", column.DefaultValueSql);
                    Assert.Equal(12, ((TimeOnly)column.DefaultValue!).Hour);
                    Assert.Equal(12, ((TimeOnly)column.DefaultValue!).Minute);
                    Assert.Equal(12, ((TimeOnly)column.DefaultValue!).Second);
                },
                "DROP TABLE MyTable;");

        // functions calls are not supported as part of the Create Table query
        public void Non_literal_or_non_parsable_DateTime_default_values_are_passed_through()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A datetime2 DEFAULT (CONVERT([datetime2],(getdate()))),
    B datetime DEFAULT getdate(),
    C datetime2 DEFAULT ((CONVERT([datetime2],('12-01-16 12:32')))),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("(CONVERT([datetime2],getdate()))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("(getdate())", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));

                    column = columns.Single(c => c.Name == "C");
                    Assert.Equal("(CONVERT([datetime2],'12-01-16 12:32'))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_DateOnly_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A date DEFAULT '1968-10-23',
    B date DEFAULT '1973-09-03T01:02:03'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("'1968-10-23'", column.DefaultValueSql);
                    Assert.Equal(new DateTime(1968, 10, 23), column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("'1973-09-03T01:02:03'", column.DefaultValueSql);
                    Assert.Equal(new DateTime(1973, 9, 3, 1, 2, 3), column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_TimeOnly_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A time DEFAULT '12:00:01.0020000'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("'12:00:01.0020000'", column.DefaultValueSql);
                    Assert.Equal(new TimeOnly(12, 0, 1, 2), column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        public void Simple_DateTimeOffset_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A datetimeoffset DEFAULT ('1973-09-03T12:00:01.0000000+10:00'),
    B datetimeoffset DEFAULT (CONVERT([datetimeoffset],('1973-09-03T01:02:03'))),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("('1973-09-03T12:00:01.0000000+10:00')", column.DefaultValueSql);
                    Assert.Equal(
                        new DateTimeOffset(new DateTime(1973, 9, 3, 12, 0, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 0, 0, 0)),
                        column.DefaultValue);

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("(CONVERT([datetimeoffset],'1973-09-03T01:02:03'))", column.DefaultValueSql);
                    Assert.Equal(
                        new DateTime(1973, 9, 3, 1, 2, 3, 0, DateTimeKind.Unspecified),
                        ((DateTimeOffset)column.DefaultValue!).DateTime);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_Guid_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A uuid DEFAULT '0e984725-c51c-4bf4-9960-e1c80e27aba0'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("'0e984725-c51c-4bf4-9960-e1c80e27aba0'", column.DefaultValueSql);
                    Assert.Equal(new Guid("0e984725-c51c-4bf4-9960-e1c80e27aba0"), column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        //Convert() and newsequentialid() are not supported
        public void Non_literal_Guid_default_values_are_passed_through()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A uniqueidentifier DEFAULT (CONVERT([uniqueidentifier],(newid()))),
    B uniqueidentifier DEFAULT NEWSEQUENTIALID(),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "A");
                    Assert.Equal("(CONVERT([uniqueidentifier],newid()))", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));

                    column = columns.Single(c => c.Name == "B");
                    Assert.Equal("(newsequentialid())", column.DefaultValueSql);
                    Assert.Null(column.FindAnnotation(RelationalAnnotationNames.DefaultValue));
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void Simple_string_literals_are_parsed_for_HasDefaultValue()
            => Test(
                @"
CREATE TABLE MyTable (
    Id int,
    A nvarchar(100) DEFAULT 'Hot',
    B varchar(100) DEFAULT 'Buttered',
    C character(100) DEFAULT '',
    D text DEFAULT '',
    E nvarchar(100) DEFAULT ' Toast! ' 
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    var column = columns.Single(c => c.Name == "a");
                    Assert.Equal("'Hot'", column.DefaultValueSql);
                    Assert.Equal("Hot", column.DefaultValue);

                    column = columns.Single(c => c.Name == "b");
                    Assert.Equal("'Buttered'", column.DefaultValueSql);
                    Assert.Equal("Buttered", column.DefaultValue);

                    column = columns.Single(c => c.Name == "c");
                    Assert.Equal("' '", column.DefaultValueSql);
                    Assert.Equal(" ", column.DefaultValue);

                    column = columns.Single(c => c.Name == "d");
                    Assert.Equal("''", column.DefaultValueSql);
                    Assert.Null(column.DefaultValue);

                    column = columns.Single(c => c.Name == "e");
                    Assert.Equal("' Toast! '", column.DefaultValueSql);
                    Assert.Equal(" Toast! ", column.DefaultValue);
                },
                "DROP TABLE MyTable;");

        [ConditionalFact]
        public void ValueGenerated_is_set_for_identity_and_computed_column()
            => Test(
                @"
CREATE TABLE ValueGeneratedProperties (
    Id int,
    NoValueGenerationColumn nvarchar(100),
    FixedDefaultValue timestamp NOT NULL DEFAULT '2015-09-20 11:00:00'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Null(columns.Single(c => c.Name == "novaluegenerationcolumn").ValueGenerated);
                    Assert.Null(columns.Single(c => c.Name == "fixeddefaultvalue").ValueGenerated);
                },
                "DROP TABLE ValueGeneratedProperties;");

        public void ConcurrencyToken_is_set_for_rowVersion()
            => Test(
                @"
CREATE TABLE RowVersionTable (
    Id int,
    rowversionColumn rowversion
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.True((bool)columns.Single(c => c.Name == "rowversionColumn")[ScaffoldingAnnotationNames.ConcurrencyToken]!);
                },
                "DROP TABLE RowVersionTable;");

        [ConditionalFact]
        public void Column_nullability_is_set()
        {
            Test(
                @"
CREATE TABLE NullableColumns (
    Id int,
    NullableInt int NULL,
    NonNullString nvarchar(10) NOT NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables[0].Columns;
                    Assert.True(columns.Single(c => c.Name == "nullableint").IsNullable);
                    Assert.False(columns.Single(c => c.Name == "nonnullstring").IsNullable);
                },
                "DROP TABLE NullableColumns;");
        }

        //ToDo
        // iicolumns Catalog
        //Column Name     Data Type   Description
        //table_name      char (256)  The name of the table.
        //column_name     char (256)  The name of the column
        //column_collid   smallint    The column's collation ID. Valid values are:
        //                            -1 The default
        //                            1 for unicode
        //                            2 for unicode_case_insensitive
        //                            3 for sql_character
        [ActianTodo]
        [ConditionalFact]
        public void Column_collation_is_set()
            => Test(
                @"
CREATE TABLE ColumnsWithCollation (
    Id int,
    DefaultCollation nvarchar(16000),
    NonDefaultCollation nvarchar(16000) COLLATE UNICODE_CASE_INSENSITIVE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Null(columns.Single(c => c.Name == "DefaultCollation").Collation);
                    DatabaseColumn? defaultCollationColumn = null;
                    foreach (var column in columns)
                    {
                        if (column.Name.ToLower() == "defaultcollation")
                        {
                            defaultCollationColumn = column;
                            break;
                        }
                    }

                    Assert.Null(defaultCollationColumn!.Collation);
                    Assert.Equal("UNICODE_CASE_INSENSITIVE", columns.Single(c => c.Name == "NonDefaultCollation").Collation);
                },
                "DROP TABLE ColumnsWithCollation;");

        public void Column_sparseness_is_set()
            => Test(
                @"
CREATE TABLE ColumnsWithSparseness (
    Id int,
    Sparse nvarchar(max) SPARSE NULL,
    NonSparse nvarchar(max) NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.True((bool)columns.Single(c => c.Name == "Sparse")[ActianAnnotationNames.Sparse]!);
                    Assert.Null(columns.Single(c => c.Name == "NonSparse")[ActianAnnotationNames.Sparse]);
                },
                "DROP TABLE ColumnsWithSparseness;");

        [TestUtilities.ActianCondition(ActianCondition.SupportsHiddenColumns)]
        public void Hidden_period_columns_are_not_created()
            => Test(
                @"
CREATE TABLE HiddenColumnsTable
(
     Id int NOT NULL PRIMARY KEY CLUSTERED,
     Name varchar(50) NOT NULL,
     SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
     SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
     PERIOD FOR SYSTEM_TIME(SysStartTime, SysEndTime)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = HiddenColumnsTableHistory));
CREATE INDEX IX_HiddenColumnsTable_1 ON HiddenColumnsTable ( Name, SysStartTime);
CREATE INDEX IX_HiddenColumnsTable_2 ON HiddenColumnsTable ( SysStartTime);
CREATE INDEX IX_HiddenColumnsTable_3 ON HiddenColumnsTable ( Name );
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal(2, columns.Count);
                    Assert.DoesNotContain(columns, c => c.Name == "SysStartTime");
                    Assert.DoesNotContain(columns, c => c.Name == "SysEndTime");
                    Assert.Equal("IX_HiddenColumnsTable_3", dbModel.Tables.Single().Indexes.Single().Name);
                },
                @"
ALTER TABLE HiddenColumnsTable SET (SYSTEM_VERSIONING = OFF);
DROP TABLE HiddenColumnsTableHistory;
DROP TABLE HiddenColumnsTable;
");

        [TestUtilities.ActianCondition(ActianCondition.SupportsHiddenColumns)]
        public void Period_columns_are_not_created()
            => Test(
                @"
CREATE TABLE dbo.HiddenColumnsTable
(
     Id int NOT NULL PRIMARY KEY CLUSTERED,
     Name varchar(50) NOT NULL,
     SysStartTime datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME(SysStartTime, SysEndTime)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.HiddenColumnsTableHistory));
CREATE INDEX IX_HiddenColumnsTable_1 ON dbo.HiddenColumnsTable ( Name, SysStartTime);
CREATE INDEX IX_HiddenColumnsTable_2 ON dbo.HiddenColumnsTable ( SysStartTime);
CREATE INDEX IX_HiddenColumnsTable_3 ON dbo.HiddenColumnsTable ( Name );
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    Assert.Equal(2, columns.Count);
                    Assert.DoesNotContain(columns, c => c.Name == "SysStartTime");
                    Assert.DoesNotContain(columns, c => c.Name == "SysEndTime");
                    Assert.Equal("IX_HiddenColumnsTable_3", dbModel.Tables.Single().Indexes.Single().Name);
                },
                @"
ALTER TABLE dbo.HiddenColumnsTable SET (SYSTEM_VERSIONING = OFF);
DROP TABLE dbo.HiddenColumnsTableHistory;
DROP TABLE dbo.HiddenColumnsTable;
");

        #endregion

        #region PrimaryKeyFacets

        [ConditionalFact]
        public void Create_composite_primary_key()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE CompositePrimaryKeyTable (
    Id1 int NOT NULL,
    Id2 int NOT NULL,
    PRIMARY KEY (Id2, Id1)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("compositeprimarykeytable", pk.Table.Name);
                    Assert.StartsWith("$compo_", pk.Name);
                    Assert.Equal(
                        new List<string> { "id2", "id1" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE CompositePrimaryKeyTable;");

        public void Set_clustered_false_for_non_clustered_primary_key()
            => Test(
                @"
CREATE TABLE NonClusteredPrimaryKeyTable (
    Id1 int PRIMARY KEY NONCLUSTERED,
    Id2 int,
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("NonClusteredPrimaryKeyTable", pk.Table.Name);
                    Assert.StartsWith("PK__NonClust", pk.Name);
                    Assert.False((bool)pk[ActianAnnotationNames.Clustered]!);
                    Assert.Equal(
                        new List<string> { "Id1" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE NonClusteredPrimaryKeyTable;");

        public void Set_clustered_false_for_primary_key_if_different_clustered_index()
            => Test(
                @"
CREATE TABLE NonClusteredPrimaryKeyTableWithClusteredIndex (
    Id1 int PRIMARY KEY NONCLUSTERED,
    Id2 int,
);

CREATE CLUSTERED INDEX ClusteredIndex ON NonClusteredPrimaryKeyTableWithClusteredIndex( Id2 );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("NonClusteredPrimaryKeyTableWithClusteredIndex", pk.Table.Name);
                    Assert.StartsWith("PK__NonClust", pk.Name);
                    Assert.False((bool)pk[ActianAnnotationNames.Clustered]!);
                    Assert.Equal(
                        new List<string> { "Id1" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE NonClusteredPrimaryKeyTableWithClusteredIndex;");

        public void Set_clustered_false_for_primary_key_if_different_clustered_constraint()
            => Test(
                @"
CREATE TABLE NonClusteredPrimaryKeyTableWithClusteredConstraint (
    Id1 int PRIMARY KEY,
    Id2 int,
    CONSTRAINT UK_Clustered UNIQUE CLUSTERED ( Id2 ),
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("NonClusteredPrimaryKeyTableWithClusteredConstraint", pk.Table.Name);
                    Assert.StartsWith("PK__NonClust", pk.Name);
                    Assert.False((bool)pk[ActianAnnotationNames.Clustered]!);
                    Assert.Equal(
                        new List<string> { "Id1" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE NonClusteredPrimaryKeyTableWithClusteredConstraint;");

        [ConditionalFact]
        public void Set_primary_key_name_from_index()
            => Test(
                @"
CREATE TABLE PrimaryKeyName (
    Id1 int,
    Id2 int not null,
    CONSTRAINT MyPK PRIMARY KEY ( Id2 )
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("dbo", pk!.Table!.Schema);
                    Assert.Equal("primarykeyname", pk.Table.Name);
                    Assert.StartsWith("mypk", pk.Name);
                    Assert.Null(pk[ActianAnnotationNames.Clustered]);
                    Assert.Equal(
                        new List<string> { "id2" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE PrimaryKeyName;");

        #endregion

        #region UniqueConstraintFacets

        [ConditionalFact]
        public void Create_composite_unique_constraint()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE CompositeUniqueConstraintTable (
    Id1 int NOT NULL,
    Id2 int NOT NULL,
    CONSTRAINT UX UNIQUE (Id2, Id1)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var uniqueConstraint = Assert.Single(dbModel.Tables[0].UniqueConstraints);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", uniqueConstraint.Table.Schema);
                    Assert.Equal("compositeuniqueconstrainttable", uniqueConstraint.Table.Name);
                    Assert.Equal("ux", uniqueConstraint.Name);
                    Assert.Equal(
                        new List<string> { "id2", "id1" }, uniqueConstraint.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE CompositeUniqueConstraintTable;");

        public void Set_clustered_true_for_clustered_unique_constraint()
            => Test(
                @"
CREATE TABLE ClusteredUniqueConstraintTable (
    Id1 int,
    Id2 int UNIQUE CLUSTERED,
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var uniqueConstraint = Assert.Single(dbModel.Tables.Single().UniqueConstraints);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", uniqueConstraint.Table.Schema);
                    Assert.Equal("ClusteredUniqueConstraintTable", uniqueConstraint.Table.Name);
                    Assert.StartsWith("UQ__Clustere", uniqueConstraint.Name);
                    Assert.True((bool)uniqueConstraint[ActianAnnotationNames.Clustered]!);
                    Assert.Equal(
                        new List<string> { "Id2" }, uniqueConstraint.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE ClusteredUniqueConstraintTable;");

        [ConditionalFact]
        public void Set_unique_constraint_name_from_index()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE UniqueConstraintName (
    Id1 int,
    Id2 int not null,
    CONSTRAINT MyUC UNIQUE ( Id2 )
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var uniqueConstraint = Assert.Single(dbModel.Tables.Single().UniqueConstraints);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", uniqueConstraint.Table.Schema);
                    Assert.Equal("uniqueconstraintname", uniqueConstraint.Table.Name);
                    Assert.Equal("myuc", uniqueConstraint.Name);
                    Assert.Equal(
                        new List<string> { "id2" }, uniqueConstraint.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE UniqueConstraintName;");

        #endregion

        #region IndexFacets

        [ConditionalFact]
        public void Create_composite_index()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE CompositeIndexTable (
    Id1 int,
    Id2 int
);

CREATE INDEX IX_COMPOSITE ON CompositeIndexTable ( Id2, Id1 );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables[0].Indexes);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", index.Table!.Schema);
                    Assert.Equal("compositeindextable", index.Table.Name);
                    Assert.Equal("ix_composite", index.Name);
                    Assert.Equal(
                        new List<string> { "id2", "id1" }, index.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE CompositeIndexTable;");

        public void Set_clustered_true_for_clustered_index()
            => Test(
                @"
CREATE TABLE ClusteredIndexTable (
    Id1 int,
    Id2 int,
);

CREATE CLUSTERED INDEX IX_CLUSTERED ON ClusteredIndexTable ( Id2 );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", index.Table!.Schema);
                    Assert.Equal("ClusteredIndexTable", index.Table.Name);
                    Assert.Equal("IX_CLUSTERED", index.Name);
                    Assert.True((bool)index[ActianAnnotationNames.Clustered]!);
                    Assert.Equal(
                        new List<string> { "Id2" }, index.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE ClusteredIndexTable;");

        [ConditionalFact]
        public void Set_unique_true_for_unique_index()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE UniqueIndexTable (
    Id1 int,
    Id2 int not null
);

CREATE UNIQUE INDEX IX_UNIQUE ON UniqueIndexTable ( Id2 );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", index.Table!.Schema);
                    Assert.Equal("uniqueindextable", index.Table.Name);
                    Assert.Equal("ix_unique", index.Name);
                    Assert.True(index.IsUnique);
                    Assert.Null(index.Filter);
                    Assert.Equal(
                        new List<string> { "id2" }, index.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE UniqueIndexTable;");

        //Ingres.Client.IngresException : line 1, Syntax error on 'WHERE'. 
        public void Set_filter_for_filtered_index()
            => Test(
                @"
CREATE TABLE FilteredIndexTable (
    Id1 int,
    Id2 int NULL
);

CREATE UNIQUE INDEX IX_UNIQUE ON FilteredIndexTable ( Id2 ) WHERE Id2 > 10;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", index.Table!.Schema);
                    Assert.Equal("FilteredIndexTable", index.Table.Name);
                    Assert.Equal("IX_UNIQUE", index.Name);
                    Assert.Equal("([Id2]>(10))", index.Filter);
                    Assert.Equal(
                        new List<string> { "Id2" }, index.Columns.Select(ic => ic.Name).ToList());
                },
                "DROP TABLE FilteredIndexTable;");

        public void Ignore_hypothetical_index()
            => Test(
                @"
CREATE TABLE HypotheticalIndexTable (
    Id1 int,
    Id2 int NULL,
);

CREATE INDEX ixHypo ON HypotheticalIndexTable ( Id1 ) WITH STATISTICS_ONLY = -1;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Empty(dbModel.Tables.Single().Indexes);
                },
                "DROP TABLE HypotheticalIndexTable;");

        public void Ignore_columnstore_index()
            => Test(
                @"
CREATE TABLE ColumnStoreIndexTable (
    Id1 int,
    Id2 int NULL,
);

CREATE NONCLUSTERED COLUMNSTORE INDEX ixColumnStore ON ColumnStoreIndexTable ( Id1, Id2 )",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Empty(dbModel.Tables.Single().Indexes);
                },
                "DROP TABLE ColumnStoreIndexTable;");

        //Ingres.Client.IngresException : line 1, Syntax error on 'INCLUDE'. 
        public void Set_include_for_index()
            => Test(
                @"
CREATE TABLE IncludeIndexTable (
    Id int,
    IndexProperty int,
    IncludeProperty int
);

CREATE INDEX IX_INCLUDE ON IncludeIndexTable(IndexProperty) INCLUDE (IncludeProperty);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);
                    Assert.Equal(new[] { "IndexProperty" }, index.Columns.Select(ic => ic.Name).ToList());
                    Assert.Null(index[ActianAnnotationNames.Include]);
                },
                "DROP TABLE IncludeIndexTable;");

        public void Index_fill_factor()
            => Test(
                @"
CREATE TABLE IndexFillFactor
(
    Id INT IDENTITY,
    Name NVARCHAR(100)
);

CREATE NONCLUSTERED INDEX [IX_Name] ON [IndexFillFactor]
(
     [Name] ASC
)
WITH (FILLFACTOR = 80) ON [PRIMARY]",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);
                    Assert.Equal(new[] { "Name" }, index.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(80, index[ActianAnnotationNames.FillFactor]);
                },
                "DROP TABLE IndexFillFactor;");

        #endregion

        #region ForeignKeyFacets

        [ConditionalFact]
        public void Create_composite_foreign_key()
            => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE PrincipalTable (
    Id1 int NOT NULL,
    Id2 int NOT NULL,
    PRIMARY KEY (Id1, Id2)
);

CREATE TABLE DependentTable (
    Id int NOT NULL PRIMARY KEY,
    ForeignKeyId1 int,
    ForeignKeyId2 int,
    FOREIGN KEY (ForeignKeyId1, ForeignKeyId2) REFERENCES PrincipalTable(Id1, Id2) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "dependenttable").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", fk.Table.Schema);
                    Assert.Equal("dependenttable", fk.Table.Name);
                    Assert.Equal("dbo", fk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", fk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid1", "foreignkeyid2" }, fk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id1", "id2" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");

        [ConditionalFact]
        public void Create_multiple_foreign_key_in_same_table()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int NOT NULL PRIMARY KEY
);

CREATE TABLE AnotherPrincipalTable (
    Id int NOT NULL PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int NOT NULL PRIMARY KEY,
    ForeignKeyId1 int,
    ForeignKeyId2 int,
    FOREIGN KEY (ForeignKeyId1) REFERENCES PrincipalTable(Id) ON DELETE CASCADE,
    FOREIGN KEY (ForeignKeyId2) REFERENCES AnotherPrincipalTable(Id) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var foreignKeys = dbModel.Tables.Single(t => t.Name == "dependenttable").ForeignKeys;

                    Assert.Equal(2, foreignKeys.Count);

                    var principalFk = Assert.Single(foreignKeys, f => f.PrincipalTable.Name == "principaltable");

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", principalFk.Table.Schema);
                    Assert.Equal("dependenttable", principalFk.Table.Name);
                    Assert.Equal("dbo", principalFk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", principalFk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid1" }, principalFk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, principalFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, principalFk.OnDelete);

                    var anotherPrincipalFk = Assert.Single(foreignKeys, f => f.PrincipalTable.Name == "anotherprincipaltable");

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", anotherPrincipalFk.Table.Schema);
                    Assert.Equal("dependenttable", anotherPrincipalFk.Table.Name);
                    Assert.Equal("dbo", anotherPrincipalFk.PrincipalTable.Schema);
                    Assert.Equal("anotherprincipaltable", anotherPrincipalFk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid2" }, anotherPrincipalFk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, anotherPrincipalFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, anotherPrincipalFk.OnDelete);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE AnotherPrincipalTable;
DROP TABLE PrincipalTable;");

        public void Create_foreign_key_referencing_unique_constraint()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id1 int,
    Id2 int UNIQUE,
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id2) ON DELETE CASCADE,
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", fk.Table.Schema);
                    Assert.Equal("DependentTable", fk.Table.Name);
                    Assert.Equal("dbo", fk.PrincipalTable.Schema);
                    Assert.Equal("PrincipalTable", fk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "ForeignKeyId" }, fk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "Id2" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");

        [ConditionalFact]
        public void Set_name_for_foreign_key()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    CONSTRAINT MYFK FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "dependenttable").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", fk.Table.Schema);
                    Assert.Equal("dependenttable", fk.Table.Name);
                    Assert.Equal("dbo", fk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", fk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid" }, fk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                    Assert.Equal("myfk", fk.Name);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");

        [ConditionalFact]
        public void Set_referential_action_for_foreign_key()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int not null PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int not null PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE SET NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "dependenttable").ForeignKeys);

                    // ReSharper disable once PossibleNullReferenceException
                    Assert.Equal("dbo", fk.Table.Schema);
                    Assert.Equal("dependenttable", fk.Table.Name);
                    Assert.Equal("dbo", fk.PrincipalTable.Schema);
                    Assert.Equal("principaltable", fk.PrincipalTable.Name);
                    Assert.Equal(
                        new List<string> { "foreignkeyid" }, fk.Columns.Select(ic => ic.Name).ToList());
                    Assert.Equal(
                        new List<string> { "id" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                    Assert.Equal(ReferentialAction.SetNull, fk.OnDelete);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");

        #endregion

        #region Warnings

        [ConditionalFact]
        public void Warn_missing_schema()
            => Test(
                @"
CREATE TABLE Blank (
    Id int
);",
                Enumerable.Empty<string>(),
                new[] { "myschema" },
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Empty(dbModel.Tables);

                    var message = Fixture.OperationReporter.Messages.Single(m => m.Level == LogLevel.Warning).Message;

                    Assert.Equal(
                        ActianResources.LogMissingSchema(new TestLogger<ActianLoggingDefinitions>()).GenerateMessage("myschema"),
                        message);
                },
                "DROP TABLE Blank;");

        [ConditionalFact]
        public void Warn_missing_table()
            => Test(
                @"
CREATE TABLE Blank (
    Id int
);",
                new[] { "mytable" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    Assert.Empty(dbModel.Tables);

                    var message = Fixture.OperationReporter.Messages.Single(m => m.Level == LogLevel.Warning).Message;

                    Assert.Equal(
                        ActianResources.LogMissingTable(new TestLogger<ActianLoggingDefinitions>()).GenerateMessage("mytable"),
                        message);
                },
                "DROP TABLE Blank;");

        [ConditionalFact]
        public void Warn_missing_principal_table_for_foreign_key()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    CONSTRAINT MYFK FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);",
                new[] { "dependenttable" },
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var message = Fixture.OperationReporter.Messages.Single(m => m.Level == LogLevel.Warning).Message;

                    Assert.Equal(
                        ActianResources.LogPrincipalTableNotInSelectionSet(new TestLogger<ActianLoggingDefinitions>())
                            .GenerateMessage(
                                "myfk", "dbo.dependenttable", "dbo.principaltable"), message);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");

        [ConditionalFact]
        public void Skip_reflexive_foreign_key()
            => Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY,
    CONSTRAINT MYFK FOREIGN KEY (Id) REFERENCES PrincipalTable(Id)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var level = Fixture.OperationReporter.Messages
                        .Single(
                            m => m.Message
                                == ActianResources.LogReflexiveConstraintIgnored(new TestLogger<ActianLoggingDefinitions>())
                                    .GenerateMessage("myfk", "dbo.principaltable")).Level;

                    Assert.Equal(LogLevel.Debug, level);

                    var table = Assert.Single(dbModel.Tables);
                    Assert.Empty(table.ForeignKeys);
                },
                @"
DROP TABLE PrincipalTable;");

        [ConditionalFact]
        public void Skip_duplicate_foreign_key()
        => Test(
                @"
SET SESSION AUTHORIZATION dbo;
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY,
    Value1 uuid not null,
    Value2 uuid not null,
	CONSTRAINT UNIQUE_Value1 UNIQUE (Value1),
	CONSTRAINT UNIQUE_Value2 UNIQUE (Value2)
);

CREATE TABLE OtherPrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    ValueKey uuid,
    CONSTRAINT MYFK1 FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id),
    CONSTRAINT MYFK2 FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id),
    CONSTRAINT MYFK3 FOREIGN KEY (ForeignKeyId) REFERENCES OtherPrincipalTable(Id),
    CONSTRAINT MYFK4 FOREIGN KEY (ValueKey) REFERENCES PrincipalTable(Value1),
    CONSTRAINT MYFK5 FOREIGN KEY (ValueKey) REFERENCES PrincipalTable(Value2)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var level = Fixture.OperationReporter.Messages
                        .Single(
                            m => m.Message
                                == ActianResources.LogDuplicateForeignKeyConstraintIgnored(new TestLogger<ActianLoggingDefinitions>())
                                    .GenerateMessage("myfk2", "dbo.dependenttable", "myfk1")).Level;

                    Assert.Equal(LogLevel.Warning, level);

                    var table = dbModel.Tables.Single(t => t.Name == "dependenttable");
                    Assert.Equal(4, table.ForeignKeys.Count);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;
DROP TABLE OtherPrincipalTable;");

        public void No_warning_missing_view_definition()
            => Test(
                @"CREATE TABLE TestViewDefinition (
Id int PRIMARY KEY
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                (dbModel, scaffoldingFactory) =>
                {
                    var message = Fixture.OperationReporter.Messages
                        .SingleOrDefault(
                            m => m.Message
                                == ActianResources.LogMissingViewDefinitionRights(new TestLogger<ActianLoggingDefinitions>())
                                    .GenerateMessage()).Message;

                    Assert.Null(message);
                },
                @"
DROP TABLE TestViewDefinition;");

        #endregion

        private void Test(
            string? createSql,
            IEnumerable<string> tables,
            IEnumerable<string> schemas,
            Action<DatabaseModel, IScaffoldingModelFactory> asserter,
            string? cleanupSql)
        {
            Test(
                string.IsNullOrEmpty(createSql) ? Array.Empty<string>() : new[] { createSql },
                tables,
                schemas,
                asserter,
                cleanupSql);
        }
        private void Test(
            string[] createSqls,
            IEnumerable<string> tables,
            IEnumerable<string> schemas,
            Action<DatabaseModel, IScaffoldingModelFactory> asserter,
            string? cleanupSql)
        {
            foreach (var createSql in createSqls)
            {
                Fixture.TestStore.ExecuteNonQuery(createSql);
            }

            try
            {
                var serviceProvider = ActianTestHelpers.Instance.CreateDesignServiceProvider(reporter: Fixture.OperationReporter)
                    .CreateScope().ServiceProvider;
                var databaseModelFactory = serviceProvider.GetRequiredService<IDatabaseModelFactory>();
                var databaseModel = databaseModelFactory.Create(
                    Fixture.TestStore.ConnectionString,
                    new DatabaseModelFactoryOptions(tables, schemas));
                Assert.NotNull(databaseModel);
                asserter(databaseModel, serviceProvider.GetRequiredService<IScaffoldingModelFactory>());
            }
            finally
            {
                if (!string.IsNullOrEmpty(cleanupSql))
                {
                    Fixture.TestStore.ExecuteNonQuery(cleanupSql);
                }
            }
        }

        [Collection("Test Collection")]
        public class ActianDatabaseModelFixture : SharedStoreFixtureBase<PoolableDbContext>
        {
            protected override string StoreName //=> ActianTestStore.GetIIDbDb().Name;
                => nameof(ActianDatabaseModelFactoryTest);

            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            public new ActianTestStore TestStore
                => (ActianTestStore)base.TestStore;

            public TestOperationReporter OperationReporter { get; } = new();

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();
                //await TestStore.ExecuteNonQueryAsync("CREATE SCHEMA db2");
                //await TestStore.ExecuteNonQueryAsync("CREATE SCHEMA [db.2]");
            }

            protected override bool ShouldLogCategory(string logCategory)
                => logCategory == DbLoggerCategory.Scaffolding.Name;
        }
    }
}
