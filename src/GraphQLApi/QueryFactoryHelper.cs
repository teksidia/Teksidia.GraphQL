using System;
using System.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace GraphQLApi
{
    public class QueryFactoryHelper
    {
        private readonly string _connectionString;

        public QueryFactoryHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public QueryFactory GetQueryFactory()
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                var compiler = new SqlServerCompiler();
                return new QueryFactory(connection, compiler);
            }
            catch (Exception e)
            {
                throw new Exception("Check that appsettings.config contains valid connection string", e);
            }
        }
    }
}
