namespace HipAndClavicle.ViewModels
{
    public class CheckoutVM
    {
        public Order Order { get; set; }
        public List<ShoppingCartItemViewModel> Items { get; set; }
        public ShoppingCart Cart { get; set; }

        // Customer Information
        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your street address.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter your city.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter your state.")]
        public State State { get; set; }

        [Required(ErrorMessage = "Please enter your ZIP code.")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Please enter your card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Please enter the expiration date.")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "Please enter the CVV.")]
        public string CVV { get; set; }
    }
}
