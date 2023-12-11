using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TodoLibrary.DataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration configuration;

        public SqlDataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<T>> LoadData<T, U>(
            string storedProcedure,
            U parameters,
            string connectionStringName)
        {
            string connectionString = configuration.GetConnectionString(connectionStringName);

            using IDbConnection connection = new SqlConnection(connectionString);

            var rows = await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            return rows.ToList();

        }

        public async Task SaveData<T>(
            string storedProcedure,
            T parameters,
            string connectionStringName)
        {
            string connectionString = configuration.GetConnectionString(connectionStringName);

            using IDbConnection connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);

        }


    }
}
