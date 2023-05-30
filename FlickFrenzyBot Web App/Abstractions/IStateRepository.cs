﻿using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IStateRepository : IRepository<CurrentState>
    {
        CurrentState? GetByUserId(int id);
    }
}
