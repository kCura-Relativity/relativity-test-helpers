﻿using DbContextHelper;
using Relativity.API;
using Relativity.Test.Helpers.ServiceFactory;
using System;
using System.Data.SqlClient;

namespace Relativity.Test.Helpers
{
    public class TestHelper : IHelper
    {
        private readonly string _username;
        private readonly string _password;

        public TestHelper()
        {
            var username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
            var password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
            ForUser(username, password);
        }

        private TestHelper(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public static IHelper ForUser(string username, string password)
        {
            return new TestHelper(username, password);
        }

        public static IHelper System()
        {
            var username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
            var password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
            return ForUser(username, password);
        }

        public IDBContext GetDBContext(int caseID)
        {
            //You can create a new DBcontext using kCura.Data.RowDataGeteway until Relativity versions lower than 9.6.85.9
            //kCura.Data.RowDataGateway.Context context = new kCura.Data.RowDataGateway.Context(SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS, string.Format("EDDS{0}", caseID == -1 ? "" : caseID.ToString()), SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME, SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD);
            //return new DBContext(context);

            //You can create a new DBcontext using DBContextHelper for Relativity versions equal to or greater than 9.6.85.9
            var context = new DbContext(SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS, string.Format("EDDS{0}", caseID == -1 ? "" : caseID.ToString()), SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME, SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD);
            return context;
        }

        public Guid GetGuid(int workspaceID, int artifactID)
        {
            var sql = "select ArtifactGuid from eddsdbo.ArtifactGuid where artifactId = @artifactId";
            var context = GetDBContext(workspaceID);
            var result = context.ExecuteSqlStatementAsScalar<Guid>(sql, new SqlParameter("artifactId", artifactID));
            return result;
        }

        public ISecretStore GetSecretStore()
        {
            throw new NotImplementedException();
        }

        public IInstanceSettingsBundle GetInstanceSettingBundle()
        {
            throw new NotImplementedException();
        }

        public ILogFactory GetLoggerFactory()
        {
            throw new NotImplementedException();
        }

        public string GetSchemalessResourceDataBasePrepend(IDBContext context)
        {
            throw new NotImplementedException();
        }

        public IServicesMgr GetServicesManager()
        {
            return new ServicesManager(_username, _password);
        }

        public IUrlHelper GetUrlHelper()
        {
            return new URLHelper();
        }

        public string ResourceDBPrepend()
        {
            throw new NotImplementedException();
        }

        public string ResourceDBPrepend(IDBContext context)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IStringSanitizer GetStringSanitizer(int workspaceID)
        {
            throw new NotImplementedException();
        }
    }


}
