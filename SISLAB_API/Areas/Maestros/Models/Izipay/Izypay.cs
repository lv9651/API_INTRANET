namespace YourNamespace.Models
{
    public class PaymentRequest
    {
        public string PaymentType { get; set; } // "CONTADO" o "CREDITO"
        public string Amount { get; set; }
        public string transactionId { get; set; } // Monto total del pago

    
        public string OrderNumber { get; set; } // Número de pedido
        public decimal? CreditAvailable { get; set; } // Crédito disponible (si aplica)
        public string FirstName { get; set; } // Nombre
        public string LastName { get; set; } // Apellido
        public string Email { get; set; } // Correo electrónico
        public string PhoneNumber { get; set; } // Teléfono
        public string Street { get; set; } // Calle
        public string DocumentNumber { get; set; } // Número de documento
    }
}