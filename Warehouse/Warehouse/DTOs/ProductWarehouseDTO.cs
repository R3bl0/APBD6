using System.ComponentModel.DataAnnotations;
using Warehouse.Models;

namespace Warehouse.DTOs
{
    public record WarehouseProductDTO(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount, double Price, DateTime CreatedAt)
    {
        public WarehouseProductDTO(WarehouseProduct warehouseProduct) : this(warehouseProduct.IdProductWarehouse, warehouseProduct.IdWarehouse,
            warehouseProduct.IdProduct, warehouseProduct.IdOrder, warehouseProduct.Amount, warehouseProduct.Price, warehouseProduct.CreatedAt)
        { }
    }
}