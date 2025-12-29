using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;

namespace GeminiLocalApp
{
    /// <summary>
    /// This is the operation class for chatting with Gemini.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// The client instance.
        /// </summary>
        private readonly Client client;
        /// <summary>
        /// The cached contents to make a chat.
        /// </summary>
        private List<Content> contents = new() { };

        /// <summary>
        /// Initialize the chat with your Gemini API client.
        /// </summary>
        /// <param name="clientArg">
        /// Your client.
        /// </param>
        public Chat(Client clientArg)
        {
            client = clientArg;
        }

        /// <summary>
        /// Sends message and returns the response.
        /// </summary>
        /// <param name="content">
        /// The user's prompt.
        /// </param>
        /// <returns>
        /// The model's response.
        /// </returns>
        public async Task<Content> SendMessage (Content content)
        {
            contents.Add(content);
            // The response.
            GenerateContentResponse response = await client.Models.GenerateContentAsync(
                "gemini-2.5-flash-lite",
                contents
            );
            if (response.Candidates != null)
            {
                Candidate candidate = response.Candidates[0];
                if (candidate.Content != null)
                {
                    Content content1 = candidate.Content;
                    contents.Add (content1);
                    return content1;
                }
            }
            throw new Exception("Unexpected Error.");
        }

        public async IAsyncEnumerable<Content> SendMessageStream(Content content)
        {
            contents.Add(content);
            await foreach (var chunk in client.Models.GenerateContentStreamAsync(
                "gemini-2.5-flash-lite",
                contents
            ))
            {
                if (chunk.Candidates != null)
                {
                    Candidate candidate = chunk.Candidates[0];
                    if (candidate.Content != null)
                    {
                        contents.Add(candidate.Content);
                        yield return candidate.Content;
                    }
                }
            }

        }

        /// <summary>
        /// Returns the chat history.
        /// </summary>
        public List<Content> History { get { return contents; } }
    }
}
