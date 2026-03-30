using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Helper
{
    public class DataAccessor
    {
        private readonly string connectionString;

        public DataAccessor(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public (IEnumerable<T1>, IEnumerable<T2>) QueryMultiple<T1, T2>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute multiple queries and get the multiple result sets
                using (var multi = connection.QueryMultiple(sql, parameters))
                {
                    // Read the first result set as T1
                    var firstResultSet = multi.Read<T1>().ToList();
                    // Read the second result set as T2
                    var secondResultSet = multi.Read<T2>().ToList();
                    return (firstResultSet, secondResultSet);
                }
            }
        }
        public (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>) QueryMultiple<T1, T2, T3, T4>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute multiple queries and get the multiple result sets
                using (var multi = connection.QueryMultiple(sql, parameters))
                {
                    // Read the first result set as T1
                    var firstResultSet = multi.Read<T1>().ToList();
                    // Read the second result set as T2
                    var secondResultSet = multi.Read<T2>().ToList();
                    var thirdResultSet = multi.Read<T3>().ToList();
                    var fourthResultSet = multi.Read<T4>().ToList();
                    return (firstResultSet, secondResultSet, thirdResultSet, fourthResultSet);
                }
            }
        }
        public (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>) QueryMultiple<T1, T2, T3, T4, T5>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute multiple queries and get the multiple result sets
                using (var multi = connection.QueryMultiple(sql, parameters))
                {
                    // Read the first result set as T1
                    var firstResultSet = multi.Read<T1>().ToList();
                    // Read the second result set as T2
                    var secondResultSet = multi.Read<T2>().ToList();
                    var thirdResultSet = multi.Read<T3>().ToList();
                    var fourthResultSet = multi.Read<T4>().ToList();
                    var fiveResultSet = multi.Read<T5>().ToList();
                    return (firstResultSet, secondResultSet, thirdResultSet, fourthResultSet, fiveResultSet);
                }
            }
        }
        public (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>) QueryMultiple<T1, T2, T3, T4, T5, T6>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute multiple queries and get the multiple result sets
                using (var multi = connection.QueryMultiple(sql, parameters))
                {
                    // Read the first result set as T1
                    var firstResultSet = multi.Read<T1>().ToList();
                    // Read the second result set as T2
                    var secondResultSet = multi.Read<T2>().ToList();
                    var thirdResultSet = multi.Read<T3>().ToList();
                    var fourthResultSet = multi.Read<T4>().ToList();
                    var fiveResultSet = multi.Read<T5>().ToList();
                    var sixResultSet = multi.Read<T6>().ToList();
                    return (firstResultSet, secondResultSet, thirdResultSet, fourthResultSet, fiveResultSet, sixResultSet);
                }
            }
        }
        public (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) QueryThreeSet<T1, T2, T3>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute multiple queries and get the multiple result sets
                using (var multi = connection.QueryMultiple(sql, parameters))
                {
                    // Read the first result set as T1
                    var firstResultSet = multi.Read<T1>().ToList();
                    // Read the second result set as T2
                    var secondResultSet = multi.Read<T2>().ToList();
                    // Read the third result set as T3
                    var thirdResultSet = multi.Read<T3>().ToList();
                    return (firstResultSet, secondResultSet, thirdResultSet);
                }
            }
        }
        public void ExecuteNonQuery(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql, parameters);
            }
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>(sql, parameters, commandTimeout: 180);
            }
        }

        public T QuerySingle<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.QuerySingle<T>(sql, parameters);
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sql, parameters);
            }
        }
    }
}