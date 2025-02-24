using System;
using System.Threading.Tasks;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.TestUtilities;
using Identity30.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.AspNetIdentity;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

#nullable enable

namespace Actian.EFCore
{
    public class MigrationsInfrastructureActianTest(
        MigrationsInfrastructureActianTest.ActianMigrationsInfrastructureFixture fixture)
        : MigrationsInfrastructureTestBase<MigrationsInfrastructureActianTest.ActianMigrationsInfrastructureFixture>(fixture)
    {
        [ActianTodo]
        public override void Can_apply_all_migrations()
        {
        }

        [ActianTodo]
        public override void Can_apply_one_migration_in_parallel()
        {
            base.Can_apply_one_migration_in_parallel();
        }

        [ActianTodo]
        public override async Task Can_apply_one_migration_in_parallel_async()
        {
            await base.Can_apply_one_migration_in_parallel_async();
        }

        [ActianTodo]
        public override void Can_apply_second_migration_in_parallel()
        {
            base.Can_apply_second_migration_in_parallel();
        }

        [ActianTodo]
        public override async Task Can_apply_second_migration_in_parallel_async()
        {
            await base.Can_apply_second_migration_in_parallel_async();
        }

        [ActianTodo]
        public override async Task Can_apply_all_migrations_async()
        {
            await base.Can_apply_all_migrations_async();
        }

        [ActianTodo]
        public override void Can_apply_one_migration()
        {
            base.Can_apply_one_migration();
        }

        [ActianTodo]
        public override void Can_apply_range_of_migrations()
        {
            base.Can_apply_range_of_migrations();
        }

        [ActianTodo]
        public override async Task Can_generate_one_up_and_down_script()
        {
            await base.Can_generate_one_up_and_down_script();
        }


        [ActianTodo]
        public override void Can_generate_migration_from_initial_database_to_initial()
        {
            base.Can_generate_migration_from_initial_database_to_initial();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override void Can_generate_no_migration_script()
        {
            base.Can_generate_no_migration_script();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override async Task Can_generate_up_and_down_scripts()
        {
            await base.Can_generate_up_and_down_scripts();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
CREATE TABLE "Table1" (
    "Id" integer NOT NULL WITH DEFAULT 0,
    "Foo" integer NOT NULL WITH DEFAULT 0,
    "Description" long nvarchar NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK_Table1" PRIMARY KEY ("Id")
);
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000001_Migration1', N'7.0.0-test');
COMMIT;
ALTER TABLE "Table1" RENAME COLUMN "Foo" TO "Bar";
MODIFY "Table1" TO RECONSTRUCT;
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000002_Migration2', N'7.0.0-test');
COMMIT;
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000003_Migration3', N'7.0.0-test');
COMMIT;
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000004_Migration4', N'7.0.0-test');
COMMIT;
INSERT INTO Table1 (Id, Bar, Description) VALUES (-1, ' ', 'Value With

Empty Lines')
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000005_Migration5', N'7.0.0-test');
COMMIT;
INSERT INTO Table1 (Id, Bar, Description) VALUES (-2, ' ', 'GO
Value With

Empty Lines')
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000006_Migration6', N'7.0.0-test');
COMMIT;
INSERT INTO Table1 (Id, Bar, Description) VALUES (-3, ' ', 'GO
Value With

GO

Empty Lines
GO')
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000007_Migration7', N'7.0.0-test');
COMMIT;

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override async Task Can_generate_up_and_down_scripts_noTransactions()
        {
            await base.Can_generate_up_and_down_scripts_noTransactions();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
CREATE TABLE "Table1" (
    "Id" integer NOT NULL WITH DEFAULT 0,
    "Foo" integer NOT NULL WITH DEFAULT 0,
    "Description" long nvarchar NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK_Table1" PRIMARY KEY ("Id")
);
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000001_Migration1', N'7.0.0-test');
ALTER TABLE "Table1" RENAME COLUMN "Foo" TO "Bar";
MODIFY "Table1" TO RECONSTRUCT;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000002_Migration2', N'7.0.0-test');
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000003_Migration3', N'7.0.0-test');
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000004_Migration4', N'7.0.0-test');
INSERT INTO Table1 (Id, Bar, Description) VALUES (-1, ' ', 'Value With

Empty Lines')
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000005_Migration5', N'7.0.0-test');
INSERT INTO Table1 (Id, Bar, Description) VALUES (-2, ' ', 'GO
Value With

Empty Lines')
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000006_Migration6', N'7.0.0-test');
INSERT INTO Table1 (Id, Bar, Description) VALUES (-3, ' ', 'GO
Value With

GO

Empty Lines
GO')
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000007_Migration7', N'7.0.0-test');

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override async Task Can_generate_up_and_down_script_using_names()
        {
            await base.Can_generate_up_and_down_script_using_names();

            Assert.Equal(
                """
ALTER TABLE "Table1" RENAME COLUMN "Foo" TO "Bar";
MODIFY "Table1" TO RECONSTRUCT;
START TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES (N'00000000000002_Migration2', N'7.0.0-test');
COMMIT;

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override async Task Can_generate_idempotent_up_and_down_scripts()
        {
            await base.Can_generate_idempotent_up_and_down_scripts();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000001_Migration1')
BEGIN
    CREATE TABLE "Table1" (
        "Id" integer NOT NULL WITH DEFAULT 0,
        "Foo" integer NOT NULL WITH DEFAULT 0,
        "Description" long nvarchar NOT NULL WITH DEFAULT '',
        CONSTRAINT "PK_Table1" PRIMARY KEY ("Id")
    );
END;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000001_Migration1')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000001_Migration1', N'7.0.0-test');
END;
COMMIT;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    ALTER TABLE "Table1" RENAME COLUMN "Foo" TO "Bar";
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    MODIFY "Table1" TO RECONSTRUCT;
END;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000002_Migration2', N'7.0.0-test');
END;
COMMIT;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000003_Migration3')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000003_Migration3', N'7.0.0-test');
END;
COMMIT;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000004_Migration4')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000004_Migration4', N'7.0.0-test');
END;
COMMIT;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000005_Migration5')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-1, ' ', 'Value With

    Empty Lines')
END;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000005_Migration5')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000005_Migration5', N'7.0.0-test');
END;
COMMIT;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000006_Migration6')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-2, ' ', 'GO
    Value With

    Empty Lines')
END;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000006_Migration6')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000006_Migration6', N'7.0.0-test');
END;
COMMIT;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000007_Migration7')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-3, ' ', 'GO
    Value With

    GO

    Empty Lines
    GO')
END;
START TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000007_Migration7')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000007_Migration7', N'7.0.0-test');
END;
COMMIT;

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ActianTodo]
        public override async Task Can_generate_idempotent_up_and_down_scripts_noTransactions()
        {
            await base.Can_generate_idempotent_up_and_down_scripts();

            Assert.Equal(
                """
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" nvarchar(150) NOT NULL WITH DEFAULT '',
    "ProductVersion" nvarchar(32) NOT NULL WITH DEFAULT '',
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000001_Migration1')
BEGIN
    CREATE TABLE "Table1" (
        "Id" integer NOT NULL WITH DEFAULT 0,
        "Foo" integer NOT NULL WITH DEFAULT 0,
        "Description" long nvarchar NOT NULL WITH DEFAULT '',
        CONSTRAINT "PK_Table1" PRIMARY KEY ("Id")
    );
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000001_Migration1')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000001_Migration1', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    ALTER TABLE "Table1" RENAME COLUMN "Foo" TO "Bar";
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    MODIFY "Table1" TO RECONSTRUCT;
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000002_Migration2')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000002_Migration2', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000003_Migration3')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000003_Migration3', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000004_Migration4')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000004_Migration4', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000005_Migration5')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-1, ' ', 'Value With

    Empty Lines')
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000005_Migration5')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000005_Migration5', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000006_Migration6')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-2, ' ', 'GO
    Value With

    Empty Lines')
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000006_Migration6')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000006_Migration6', N'7.0.0-test');
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000007_Migration7')
BEGIN
    INSERT INTO Table1 (Id, Bar, Description) VALUES (-3, ' ', 'GO
    Value With

    GO

    Empty Lines
    GO')
END;
IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'00000000000007_Migration7')
BEGIN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N'00000000000007_Migration7', N'7.0.0-test');
END;

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("Actian.EFCore", ActiveProvider);
        }

        [ActianTodo]
        public override void Can_revert_all_migrations()
        {
            base.Can_revert_all_migrations();
        }

        [ActianTodo]
        public override void Can_revert_one_migrations()
        {
            base.Can_revert_one_migrations();
        }

        [ActianTodo]
        [ConditionalFact]
        public async Task Empty_Migration_Creates_Database()
        {
            using var context = new BloggingContext(
                Fixture.TestStore.AddProviderOptions(
                    new DbContextOptionsBuilder().EnableServiceProviderCaching(false)).Options);
            var creator = (ActianDatabaseCreator)context.GetService<IRelationalDatabaseCreator>();
            creator.RetryTimeout = TimeSpan.FromMinutes(10);

            await context.Database.MigrateAsync();

            Assert.True(creator.Exists());
        }

        private class BloggingContext(DbContextOptions options) : DbContext(options)
        {
            // ReSharper disable once UnusedMember.Local
            public DbSet<Blog> ?Blogs { get; set; }

            // ReSharper disable once ClassNeverInstantiated.Local
            public class Blog
            {
                // ReSharper disable UnusedMember.Local
                public int Id { get; set; }

                public string ?Name { get; set; }
                // ReSharper restore UnusedMember.Local
            }
        }

        [DbContext(typeof(BloggingContext))]
        [Migration("00000000000000_Empty")]
        public class EmptyMigration : Migration
        {
            protected override void Up(MigrationBuilder migrationBuilder)
            {
            }
        }

        public override void Can_diff_against_2_2_model()
        {
            using var context = new ModelSnapshot22.BloggingContext();
            DiffSnapshot(new BloggingContextModelSnapshot22(), context);
        }

        public class BloggingContextModelSnapshot22 : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "ModelSnapshot22.Blog", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("Name");

                        b.HasKey("Id");

                        b.ToTable("Blogs");

                        b.HasData(
                            new { Id = 1, Name = "HalfADonkey" });
                    });

                modelBuilder.Entity(
                    "ModelSnapshot22.Post", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<int?>("BlogId");

                        b.Property<string>("Content");

                        b.Property<DateTime>("EditDate");

                        b.Property<string>("Title");

                        b.HasKey("Id");

                        b.HasIndex("BlogId");

                        b.ToTable("Post");
                    });

                modelBuilder.Entity(
                    "ModelSnapshot22.Post", b =>
                    {
                        b.HasOne("ModelSnapshot22.Blog", "Blog")
                            .WithMany("Posts")
                            .HasForeignKey("BlogId");
                    });
#pragma warning restore 612, 618
            }
        }

        [ActianTodo]
        public override void Can_diff_against_2_1_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity21ModelSnapshot(), context);
        }

        public class AspNetIdentity21ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.1.0")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Name")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex") // Don't change to HasDatabaseName
                            .HasFilter("[NormalizedName] IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("RoleId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<int>("AccessFailedCount");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Email")
                            .HasMaxLength(256);

                        b.Property<bool>("EmailConfirmed");

                        b.Property<bool>("LockoutEnabled");

                        b.Property<DateTimeOffset?>("LockoutEnd");

                        b.Property<string>("NormalizedEmail")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedUserName")
                            .HasMaxLength(256);

                        b.Property<string>("PasswordHash");

                        b.Property<string>("PhoneNumber");

                        b.Property<bool>("PhoneNumberConfirmed");

                        b.Property<string>("SecurityStamp");

                        b.Property<bool>("TwoFactorEnabled");

                        b.Property<string>("UserName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("[NormalizedUserName] IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("RoleId");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasMaxLength(128);

                        b.Property<string>("Value");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });
#pragma warning restore 612, 618
            }
        }

        [ActianTodo]
        public override void Can_diff_against_2_2_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity22ModelSnapshot(), context);
        }

        public class AspNetIdentity22ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "2.2.0-preview1")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Name")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex")
                            .HasFilter("[NormalizedName] IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("RoleId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .ValueGeneratedOnAdd();

                        b.Property<int>("AccessFailedCount");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken();

                        b.Property<string>("Email")
                            .HasMaxLength(256);

                        b.Property<bool>("EmailConfirmed");

                        b.Property<bool>("LockoutEnabled");

                        b.Property<DateTimeOffset?>("LockoutEnd");

                        b.Property<string>("NormalizedEmail")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedUserName")
                            .HasMaxLength(256);

                        b.Property<string>("PasswordHash");

                        b.Property<string>("PhoneNumber");

                        b.Property<bool>("PhoneNumberConfirmed");

                        b.Property<string>("SecurityStamp");

                        b.Property<bool>("TwoFactorEnabled");

                        b.Property<string>("UserName")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("[NormalizedUserName] IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType");

                        b.Property<string>("ClaimValue");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName");

                        b.Property<string>("UserId")
                            .IsRequired();

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("RoleId");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId");

                        b.Property<string>("LoginProvider")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasMaxLength(128);

                        b.Property<string>("Value");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade);

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade);
                    });
#pragma warning restore 612, 618
            }
        }

        [ActianTodo]
        public override void Can_diff_against_3_0_ASP_NET_Identity_model()
        {
            using var context = new ApplicationDbContext();
            DiffSnapshot(new AspNetIdentity30ModelSnapshot(), context);
        }

        public class AspNetIdentity30ModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
#pragma warning disable 612, 618
                modelBuilder
                    .HasAnnotation("ProductVersion", "3.0.0")
                    .HasAnnotation("Relational:MaxIdentifierLength", 128)
                    .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id")
                            .HasColumnType("nvarchar(450)");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken()
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("Name")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedName")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedName")
                            .IsUnique()
                            .HasName("RoleNameIndex")
                            .HasFilter("[NormalizedName] IS NOT NULL");

                        b.ToTable("AspNetRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasColumnType("int")
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("ClaimValue")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("RoleId")
                            .IsRequired()
                            .HasColumnType("nvarchar(450)");

                        b.HasKey("Id");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetRoleClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUser", b =>
                    {
                        b.Property<string>("Id")
                            .HasColumnType("nvarchar(450)");

                        b.Property<int>("AccessFailedCount")
                            .HasColumnType("int");

                        b.Property<string>("ConcurrencyStamp")
                            .IsConcurrencyToken()
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("Email")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.Property<bool>("EmailConfirmed")
                            .HasColumnType("bit");

                        b.Property<bool>("LockoutEnabled")
                            .HasColumnType("bit");

                        b.Property<DateTimeOffset?>("LockoutEnd")
                            .HasColumnType("datetimeoffset");

                        b.Property<string>("NormalizedEmail")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.Property<string>("NormalizedUserName")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.Property<string>("PasswordHash")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("PhoneNumber")
                            .HasColumnType("nvarchar(max)");

                        b.Property<bool>("PhoneNumberConfirmed")
                            .HasColumnType("bit");

                        b.Property<string>("SecurityStamp")
                            .HasColumnType("nvarchar(max)");

                        b.Property<bool>("TwoFactorEnabled")
                            .HasColumnType("bit");

                        b.Property<string>("UserName")
                            .HasColumnType("nvarchar(256)")
                            .HasMaxLength(256);

                        b.HasKey("Id");

                        b.HasIndex("NormalizedEmail")
                            .HasName("EmailIndex");

                        b.HasIndex("NormalizedUserName")
                            .IsUnique()
                            .HasName("UserNameIndex")
                            .HasFilter("[NormalizedUserName] IS NOT NULL");

                        b.ToTable("AspNetUsers");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.Property<int>("Id")
                            .ValueGeneratedOnAdd()
                            .HasColumnType("int")
                            .HasAnnotation("Actian:ValueGenerationStrategy", ActianValueGenerationStrategy.IdentityColumn);

                        b.Property<string>("ClaimType")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("ClaimValue")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("UserId")
                            .IsRequired()
                            .HasColumnType("nvarchar(450)");

                        b.HasKey("Id");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserClaims");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .HasColumnType("nvarchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderKey")
                            .HasColumnType("nvarchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("ProviderDisplayName")
                            .HasColumnType("nvarchar(max)");

                        b.Property<string>("UserId")
                            .IsRequired()
                            .HasColumnType("nvarchar(450)");

                        b.HasKey("LoginProvider", "ProviderKey");

                        b.HasIndex("UserId");

                        b.ToTable("AspNetUserLogins");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.Property<string>("UserId")
                            .HasColumnType("nvarchar(450)");

                        b.Property<string>("RoleId")
                            .HasColumnType("nvarchar(450)");

                        b.HasKey("UserId", "RoleId");

                        b.HasIndex("RoleId");

                        b.ToTable("AspNetUserRoles");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.Property<string>("UserId")
                            .HasColumnType("nvarchar(450)");

                        b.Property<string>("LoginProvider")
                            .HasColumnType("nvarchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("Name")
                            .HasColumnType("nvarchar(128)")
                            .HasMaxLength(128);

                        b.Property<string>("Value")
                            .HasColumnType("nvarchar(max)");

                        b.HasKey("UserId", "LoginProvider", "Name");

                        b.ToTable("AspNetUserTokens");
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                            .WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();

                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });

                modelBuilder.Entity(
                    "Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                    {
                        b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired();
                    });
#pragma warning restore 612, 618
            }
        }

        protected override Task ExecuteSqlAsync(string value)
        {
            ((ActianTestStore)Fixture.TestStore).ExecuteScript(value);
            return Task.CompletedTask;
        }

        public class ActianMigrationsInfrastructureFixture : MigrationsInfrastructureFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();
//                await ((ActianTestStore)TestStore).ExecuteNonQueryAsync(
//                    @"USE master
//IF EXISTS(select * from sys.databases where name='TransactionSuppressed')
//DROP DATABASE TransactionSuppressed");
            }

            public override MigrationsContext CreateContext()
            {
                var options = AddOptions(TestStore.AddProviderOptions(new DbContextOptionsBuilder()))
                    .UseActian(TestStore.ConnectionString, b => b.ApplyConfiguration())
                    .UseInternalServiceProvider(ServiceProvider)
                    .Options;
                return new MigrationsContext(options);
            }
        }
    }
}

//namespace ModelSnapshot22
//{
//    public class Blog
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public ICollection<Post> Posts { get; set; }
//    }

//    public class Post
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Content { get; set; }
//        public DateTime EditDate { get; set; }

//        public Blog Blog { get; set; }
//    }

//    public class BloggingContext : DbContext
//    {
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//            => optionsBuilder.UseActian(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");

//        public DbSet<Blog> Blogs { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//            => modelBuilder.Entity<Blog>().HasData(
//                new Blog { Id = 1, Name = "HalfADonkey" });
//    }
//}

namespace Identity30.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseActian(TestEnvironment.GetConnectionString("MigrationsTest"));
            //=> optionsBuilder.UseActian(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>(
                b =>
                {
                    b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                    b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
                    b.ToTable("AspNetUsers");
                });

            builder.Entity<IdentityUserClaim<string>>(
                b =>
                {
                    b.ToTable("AspNetUserClaims");
                });

            builder.Entity<IdentityUserLogin<string>>(
                b =>
                {
                    b.ToTable("AspNetUserLogins");
                });

            builder.Entity<IdentityUserToken<string>>(
                b =>
                {
                    b.ToTable("AspNetUserTokens");
                });

            builder.Entity<IdentityRole>(
                b =>
                {
                    b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
                    b.ToTable("AspNetRoles");
                });

            builder.Entity<IdentityRoleClaim<string>>(
                b =>
                {
                    b.ToTable("AspNetRoleClaims");
                });

            builder.Entity<IdentityUserRole<string>>(
                b =>
                {
                    b.ToTable("AspNetUserRoles");
                });
        }
    }
}
