using System.Data;
using System.Data.SqlClient;
using Warehouse.DTOs;
using Warehouse.Models;

namespace Warehouse.Services
{
    public interface IDbService
    {
        Task<List<ProductWarehouseDTO>> GetWarehouseProducts();
        Task<int?> CreateProductWarehouse(CreateProductWarehouseRequest request);
    }

    public class DbService : IDbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<SqlConnection> GetConnection()
        {
            var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
            if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
            return sqlConnection;
        }

        public async Task<List<ProductWarehouseDTO>> GetWarehouseProducts()
        {
            await using var sqlConnection = await GetConnection();
            var response = new List<ProductWarehouseDTO>();
            var command = new SqlCommand("SELECT * FROM Product_Warehouse", sqlConnection);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                response.Add(new ProductWarehouseDTO(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetDouble(5),
                        reader.GetDateTime(6)
                    )
                );
            }

            return response;
        }

        public async Task<int?> CreateProductWarehouse(CreateProductWarehouseRequest request)
        {
            await using var sqlConnection = await GetConnection();

            var sqlCommand1 = new SqlCommand("SELECT * FROM PRODUCT WHERE IdProduct = @id", sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@id", request.IdProduct);
            await using var reader1 = await sqlCommand1.ExecuteReaderAsync();
            if (!reader1.HasRows)
            {
                return null;
            }
            await reader1.CloseAsync();
            
            var sqlCommand2 = new SqlCommand("SELECT * FROM WAREHOUSE WHERE IdWarehouse = @id", sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@id", request.IdWarehouse);
            await using var reader2 = await sqlCommand2.ExecuteReaderAsync();
            if (!reader2.HasRows)
            {
                return null;
            }
            await reader2.CloseAsync();

            if (request.Amount <= 0)
            {
                return null;
            }

            var sqlCommand3 = new SqlCommand("SELECT IdOrder FROM [ORDER] WHERE IdProduct = @1 AND Amount = @2 AND CreatedAt < @3", sqlConnection);
            sqlCommand3.Parameters.AddWithValue("@1", request.IdProduct);
            sqlCommand3.Parameters.AddWithValue("@2", request.Amount);
            sqlCommand3.Parameters.AddWithValue("@3", request.CreatedAt);
            await using var reader3 = await sqlCommand3.ExecuteReaderAsync();
            if (!reader3.HasRows)
            {
                return null;
            }

            await reader3.ReadAsync();
            var idOrder = reader3.GetInt32(0);
            await reader3.CloseAsync();

            var sqlCommand4 = new SqlCommand("SELECT * FROM PRODUCT_WAREHOUSE WHERE IdOrder = @id", sqlConnection);
            sqlCommand4.Parameters.AddWithValue("@id", idOrder);
            await using var reader4 = await sqlCommand4.ExecuteReaderAsync();
            if (reader4.HasRows)
            {
                return null;
            }
            await reader4.CloseAsync();

            var sqlCommand5 = new SqlCommand("UPDATE [ORDER] SET FulfilledAt = GETDATE() WHERE IdOrder = @1", sqlConnection);
            sqlCommand5.Parameters.AddWithValue("@1", idOrder);
            await sqlCommand5.ExecuteNonQueryAsync();

            var sqlCommand6 = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @1", sqlConnection);
            sqlCommand6.Parameters.AddWithValue("@1", request.IdProduct);
            await using var reader6 = await sqlCommand6.ExecuteReaderAsync();
            await reader6.ReadAsync();
            var price = reader6.GetDecimal(0);
            await reader6.CloseAsync();

            var sqlCommand7 = new SqlCommand("INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@1, @2, @3, @4, @5, GETDATE())", sqlConnection);
            sqlCommand7.Parameters.AddWithValue("@1", request.IdWarehouse);
            sqlCommand7.Parameters.AddWithValue("@2", request.IdProduct);
            sqlCommand7.Parameters.AddWithValue("@3", idOrder);
            sqlCommand7.Parameters.AddWithValue("@4", request.Amount);
            sqlCommand7.Parameters.AddWithValue("@5", request.Amount * price);
            await sqlCommand7.ExecuteNonQueryAsync();

            var sqlCommand8 = new SqlCommand("SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdOrder = @1", sqlConnection);
            sqlCommand8.Parameters.AddWithValue("@1", idOrder);
            await using var reader8 = await sqlCommand8.ExecuteReaderAsync();
            await reader8.ReadAsync();
            var id = reader8.GetInt32(0);
            await reader8.CloseAsync();

            Console.WriteLine(id);
            return id;
        }
    }
}
