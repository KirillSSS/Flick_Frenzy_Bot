using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlickFrenzyBot_Web_App.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "N/A";
        public string Genre { get; set; } = "N/A";
        public string Rated { get; set; } = "N/A";
        public string Runtime { get; set; } = "N/A";
        public string Director { get; set; } = "N/A";
        public string Actors { get; set; } = "N/A";
        public string Plot { get; set; } = "N/A";
        public string Awards { get; set; } = "N/A";
        public string Released { get; set; } = "N/A";
        public string Poster { get; set; } = "N/A";
        public List<Rating>? Ratings { get; set; }
        public List<Review>? Reviews { get; }

        [NotMapped]
        public string Response { get; set; } = "N/A";

        public string GetShortInfo()
        {
            var output = $"{Title}: \n\n";

            if (Ratings is not null)
                foreach (var rating in Ratings)
                    output += $"    {rating.Source}: {rating.Value}\n\n";

            return output;
        }

        public string GetPlotInfo()
        {
            var output = Genre != "N/A" ? $"Genre: \n{Genre} \n\n" : $"Genre: Sorry, I have no info about it \n\n";
            output += Plot != "N/A" ? $"Plot: \n{Plot}\n\n" : $"Plot: Sorry, I have no info about it \n\n";
            return output;
        }

        public string GetGeneralInfo()
        {
            var output = Released != "N/A" ? $"Released: {Released} \n\n" : $"Released: Sorry, I have no info about it \n\n";
            output += Rated != "N/A" ? $"Rated: {Rated}\n\n" : $"Rated: Sorry, I have no info about it \n\n";
            output += Runtime != "N/A" ? $"Runtime: {Runtime}\n\n" : $"Runtime: Sorry, I have no info about it \n\n";
            return output;
        }

        public string GetFilmmakersInfo()
        {
            var output = Director != "N/A" ? $"Director: \n{Director} \n\n" : $"Director: Sorry, I have no info about it \n\n";
            output += Actors != "N/A" ? $"Actors: \n{Actors}\n\n" : $"Actors: Sorry, I have no info about it \n\n";
            return output;
        }

        public string GetAwardsInfo()
        {
            var output = $"Rating: \n";

            if (Ratings is not null)
                foreach (var rating in Ratings)
                    output += $"{rating.Source}: {rating.Value}\n\n";

            output += Awards != "N/A" ? $"Awards: \n{Awards} \n\n" : $"Awards: Sorry, I have no info about it \n\n";

            return output;
        }

        public string GetCompleteInfo()
        {
            var output = Released != "N/A" ? $"{Title} ({Released}): \n" : $"{Title}: \n";

            output += $"{Rated} - {Runtime}\n\n";
            output += GetPlotInfo();
            output += GetFilmmakersInfo();
            output += GetAwardsInfo();

            return output;
        }
    }
}
