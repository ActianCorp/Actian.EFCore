﻿using System.Data;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Actian.EFCore.Scaffolding.DatabaseModelFactory
{
    public class ActianDatabaseModelFixture : SharedStoreFixtureBase<PoolableDbContext>
    {
        protected override string StoreName { get; } = "DatabaseModelFactory";
        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
        public new ActianTestStore TestStore { get; }
        private readonly ActianTestStore IIDbDbStore;
        public ITestOutputHelper Output { get; private set; }
        public string DatabaseName => TestEnvironment.GetDatabaseName(StoreName);

        public ActianDatabaseModelFixture()
        {
            TestStore = (ActianTestStore)base.TestStore;
            if (TestStore.ConnectionState != ConnectionState.Closed)
            {
                TestStore.CloseConnection();
            }
            IIDbDbStore = ActianTestStore.GetIIDbDb();

            CreateUser("db2");
            CreateUser("db.2");

            IIDbDbStore.ExecuteNonQuery(@$"grant access, db_admin on database {DatabaseName} to public");
        }

        public void SetOutput(ITestOutputHelper output)
        {
            Output = output;
            TestStore.SetOutput(output);
            IIDbDbStore.SetOutput(output);
        }

        private void CreateUser(string user)
        {
            if (!UserExists(user))
            {
                IIDbDbStore.ExecuteNonQuery(@$"create user ""{user}""");
            }
        }

        private bool UserExists(string user)
        {
            return IIDbDbStore.ExecuteScalar<int?>(@$"
                select 1
                  from iiusers
                 where user_name = '{user.Replace("'", "''")}'
            ") == 1;
        }

        protected override bool ShouldLogCategory(string logCategory)
            => logCategory == DbLoggerCategory.Scaffolding.Name;

        protected override void Clean(DbContext context)
        {
            base.Clean(context);
        }

        public override void Dispose()
        {
            base.Dispose();
            IIDbDbStore.Dispose();
        }

    }
}
