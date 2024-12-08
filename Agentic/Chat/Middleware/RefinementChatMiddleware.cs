using Agentic.Profiles;
using Agentic.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agentic.Chat.Middleware
{
    internal class RefinementChatMiddleware : IChatMiddleware
    {
        private readonly int _maxRefinements;
        private readonly string _refinementPrompt;
        private readonly string _refinementInstruction;

        public RefinementChatMiddleware(RefinementDefinition refinement)
        {
            _refinementPrompt = refinement.Prompt ?? string.Empty;
            _refinementInstruction = refinement.Instruction ?? string.Empty;
            _maxRefinements = refinement.Limit;
        }

        public async Task InvokeAsync(ChatMiddlewareContext context, ChatMiddlewareDelegate next)
        {
            int refinements = _maxRefinements;

            await next(context).ConfigureAwait(false);
            var lastResponse = context.Response;

            while (refinements > 0 && lastResponse != null)
            {
                var refinementMessage = string.Join("\n", _refinementPrompt, _refinementInstruction);

                // Adjust the context for the next iteration
                context.AdditionalMessages = new List<ChatMessage>
                {
                    // Include the last response as part of the conversation history
                    // so the model sees what it produced previously.
                    lastResponse,

                    // The user "refinement" prompt, instructing the model how to refine.
                    new ChatMessage(Role.User, refinementMessage)
                };

                // Reset response before calling next for a refined response
                context.Response = null;
                await next(context).ConfigureAwait(false);

                var currentResponse = context.Response;

                // If we got no response or a duplicate response, break out.
                if (currentResponse == null || string.IsNullOrEmpty(currentResponse.Content))
                {
                    break;
                }

                bool isSignificantlySmaller = false;
                if (lastResponse.Content.Length > 0)
                {
                    var sizeRatio = (float)currentResponse.Content.Length / lastResponse.Content.Length;
                    isSignificantlySmaller = sizeRatio < 0.1f;
                }
                if (isSignificantlySmaller) break;

                var isSameContent = TextSimilarity.Check(lastResponse.Content, currentResponse.Content);
                if (isSameContent) break;

                lastResponse = currentResponse;
                refinements--;
            }

            // Set the final response after all refinement attempts
            context.Response = lastResponse;
        }
    }
}
