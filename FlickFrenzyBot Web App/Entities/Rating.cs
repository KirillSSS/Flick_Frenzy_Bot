namespace FlickFrenzyBot_Web_App.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public string Source { get; set; } = "N/A";
        public string Value { get; set; } = "N/A";
    }
}
