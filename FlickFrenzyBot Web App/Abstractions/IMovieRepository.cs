﻿using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Movie? GetByTitle(string title);
    }
}
