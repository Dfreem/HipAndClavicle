namespace HipAndClavicle.ViewModels
{

    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;

        public string Email { get; set; }


        public string? Product { get; set; }
        public string? City { get; set; }
    }
}