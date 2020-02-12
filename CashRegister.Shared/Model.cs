using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashRegister.Shared
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        public List<ReceiptLine> ReceiptLines { get; set; }
    }

    public class ReceiptLine
    {
        public int ReceiptLineId { get; set; }


        public Product Product { get; set; }

        public int ProductId { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        public Receipt Receipt { get; set; }
    }

    public class Receipt
    {
        public int ReceiptId { get; set; }

        public List<ReceiptLine> ReceiptLines { get; set; }

        [Required]
        public long ReceiptTimestamp { get; set; }

        [Required]
        public double TotalPrice { get; set; }
    }
}
