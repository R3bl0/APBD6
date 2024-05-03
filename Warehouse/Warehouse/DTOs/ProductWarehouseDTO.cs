using System.ComponentModel.DataAnnotations;
using Warehouse.Models;

namespace Warehouse.DTOs
{
    public record WarehouseProductDTO(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount, double Price, DateTime CreatedAt)
    {
        public WarehouseProductDTO(ProductWarehouse productWarehouse) : this(productWarehouse.IdProductWarehouse, productWarehouse.IdWarehouse,
            productWarehouse.IdProduct, productWarehouse.IdOrder, productWarehouse.Amount, productWarehouse.Price, productWarehouse.CreatedAt)
        { }
    }
}