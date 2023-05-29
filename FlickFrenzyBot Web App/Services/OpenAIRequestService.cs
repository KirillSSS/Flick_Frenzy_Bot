﻿using OpenAI_API.Completions;
using OpenAI_API;

namespace FlickFrenzyBot_Web_App.Services
{
    public class OpenAIRequestService
    {
        private const string apiKey = "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";

        public static async Task<string> GetCorrectTitleAsync(string prompt)
        {
            var openAI = new OpenAIAPI(apiKey);

            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = $"I want the full title \"{prompt}\". Give me the full name of this movie, series, cartoon or show. The result should contain only the title and no additional words, phrases or sentences.";
            completionRequest.Model = OpenAI_API.Models.Model.DavinciText;

            var completions = await openAI.Completions.CreateCompletionAsync(completionRequest);
            var output = "";

            foreach (var completion in completions.Completions)
                output += completion.Text;

            return output;
        }
    }
}
