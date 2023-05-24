using System.ComponentModel.DataAnnotations.Schema;

namespace FlickFrenzyBot_Web_App.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Rated { get; set; }
        public string Runtime { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Awards { get; set; }
        public string Released { get; set; }
        public string Poster { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Review> Reviews { get; }

        [NotMapped]
        public string Response { get; set; }
    }
}
