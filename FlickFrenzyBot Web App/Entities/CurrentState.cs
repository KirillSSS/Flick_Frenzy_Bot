namespace FlickFrenzyBot_Web_App.Entities
{
    public class CurrentState
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int? currentMovieId { get; set; } = null;
        public int? currentMessageId { get; set; } = null;
        public string currentMessageType { get; set; } = "N/A";
    }
}
