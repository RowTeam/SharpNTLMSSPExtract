using SharpNTLMSSPExtract.Domain;

namespace SharpNTLMSSPExtract.Commands
{
    public interface ICommand
    {
        void Execute(ArgumentParserContent arguments);
    }
}
