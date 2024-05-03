using System.Data;
using System.Data.SqlClient;
using Warehouse.DTOs;
using Warehouse.Models;

namespace Warehouse.Services
{

    public interface IDbService
    {
        Task<List<WarehouseProductDTO>> GetWarehouseProducts();
        Task<string> CreateProductWarehouse();
    }
    
    public class DbService(IConfiguration configuration):IDbService
    {
        private async Task<SqlConnection> GetConnection()
        {
            var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
            return sqlConnection;
        }
        
        public async Task<List<WarehouseProductDTO>> GetWarehouseProducts()
        {
            await using var sqlConnection = await GetConnection();
            var response = new  List<WarehouseProductDTO>();
            var command = new SqlCommand("SELECT * FROM Product_Warehouse", sqlConnection);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                response.Add(new WarehouseProductDTO(
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

        public async Task<string> CreateProductWarehouse()
        {
            
            return "git git";
        }
    }
}