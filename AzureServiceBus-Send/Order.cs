using System.Text.Json;

namespace AzureServiceBus_Send
{
    internal class Order
    {
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total 
        { 
            get
            {
                return Quantity * UnitPrice;
            }   
        }

        public decimal TotalValue 
        { 
            get; set; 
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
