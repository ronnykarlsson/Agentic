namespace Agentic.Clients.OpenAI
{
    public class OpenAIEmbeddingRequest
    {
        public OpenAIEmbeddingRequest()
        {
        }

        public OpenAIEmbeddingRequest(string model, string input)
        {
            Model = model;
            Input = input;
        }

        public string Model { get; set; }
        public string Input { get; set; }
    }
}
