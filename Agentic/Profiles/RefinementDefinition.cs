namespace Agentic.Profiles
{
    public class RefinementDefinition
    {
        public string Prompt { get; set; } = "Review the previous answer and refine it further, ensure it can be performed or answered by an AI agent using only text and text-based tools.";
        public string Instruction { get; set; } = "If the answer is good already and needs no further refinement, or potential refinements are very minor, answer with a completely empty answer, nothing else. Do not mention you are refining or anything of that kind, only to rewrite the previous answer.";
        public int Limit { get; set; } = 3;
    }
}
