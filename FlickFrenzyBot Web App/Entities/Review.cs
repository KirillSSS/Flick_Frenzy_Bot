namespace FlickFrenzyBot_Web_App.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public string Comment { get; set; } = "N/A";
        public string Score { get; set; } = "N/A";
    }
}
