namespace FlickFrenzyBot_Web_App.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; } = "N/A";
        public string Name { get; set; } = "N/A";
        public string Surname { get; set; } = "N/A";
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public Recommendation? Recommendation { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
