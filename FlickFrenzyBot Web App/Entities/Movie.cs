﻿using Microsoft.AspNetCore.Razor.TagHelpers;
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

        public string GetShortInfo()
        {
            var output = $"{Title}: \n\n";

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
