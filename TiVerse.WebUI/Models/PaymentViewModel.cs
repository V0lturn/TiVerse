using LiqPay.SDK.Dto;

namespace TiVerse.WebUI.Models
{
    public class PaymentViewModel
    {
        public LiqPayRequest PaymentParams { get; set; }
        public string FormCode { get; set; }
    }
}
