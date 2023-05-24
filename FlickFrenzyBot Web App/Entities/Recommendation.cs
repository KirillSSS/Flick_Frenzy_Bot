namespace FlickFrenzyBot_Web_App.Entities
{
    public class Recommendation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string? Genres { get; set; }
        public string? Rated { get; set; }
        public string? Directors { get; set; }
        public string? Actors { get; set; }
        public string? IMDbScore { get; set; }
    }
}
