using System;

namespace SharpDetectionNTLMSSP.FunModule
{
    class RDP : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            /*
             * target 192.168.65.133 RDP Auth Fail: 解密操作失败，请参见内部异常。
             * target 192.168.65.133 RDPInfo Error: 解密操作失败，请参见内部异常。
             */
            throw new NotImplementedException();
        }
    }
}
