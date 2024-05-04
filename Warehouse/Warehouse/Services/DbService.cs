using System.Data;
using System.Data.SqlClient;
using Warehouse.DTOs;
using Warehouse.Models;

namespace Warehouse.Services
{

    public interface IDbService
    {
        Task<List<ProductWarehouseDTO>> GetWarehouseProducts();
        Task<string> CreateProductWarehouse(CreateProductWarehouseRequest request);
    }
    
    public class DbService(IConfiguration configuration):IDbService
    {
        private async Task<SqlConnection> GetConnection()
        {
            var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
            return sqlConnection;
        }
        
        public async Task<List<ProductWarehouseDTO>> GetWarehouseProducts()
        {
            await using var sqlConnection = await GetConnection();
            var response = new  List<ProductWarehouseDTO>();
            var command = new SqlCommand("SELECT * FROM Product_Warehouse", sqlConnection);
            var reader = await command.ExecuteReaderAsync();
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

        public async Task<string> CreateProductWarehouse(CreateProductWarehouseRequest request)
        {
            await using var sqlConnection = await GetConnection();
            
            var sqlCommand = new SqlCommand("SELECT * FROM PRODUCT WHERE IdProduct = @id", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", request.IdProduct);
            var reader = await sqlCommand.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }
            
            sqlCommand = new SqlCommand("SELECT * FROM WAREHOUSE WHERE IdWarehouse = @id", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", request.IdWarehouse);
            await reader.CloseAsync();
            reader = await sqlCommand.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }

            if (request.Amount<=0)
            {
                return null;
            }
            
            
            
            Console.WriteLine("git");
            return "git git";
        }
    }
}