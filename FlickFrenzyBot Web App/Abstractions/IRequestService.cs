namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IRequestService
    {
        Task<(string PosterUrl, string Info)> GetResponseAsync(string message);
    }
}
