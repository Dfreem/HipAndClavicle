namespace HipAndClavicle.ViewModels
{

    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }

        public string Email { get; set; }
    }
}