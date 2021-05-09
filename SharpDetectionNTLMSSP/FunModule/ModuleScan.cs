
namespace SharpDetectionNTLMSSP.FunModule
{
    abstract class ModuleScan
    {
        public abstract TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey);
    }
}
