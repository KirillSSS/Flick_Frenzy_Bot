namespace FlickFrenzyBot_Web_App.Entities
{
    public class Recommendation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Genres { get; set; } = "N/A";
        public string Rated { get; set; } = "N/A";
        public string Directors { get; set; } = "N/A";
        public string Actors { get; set; } = "N/A";
        public string IMDbScore { get; set; } = "N/A";
    }
}
