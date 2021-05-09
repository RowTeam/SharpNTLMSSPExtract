namespace SharpDetectionNTLMSSP.FunModule
{
    class MSSQL : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var response = new byte[1024];
            socketMessage.SendMessage(NTLMSSPBuffer.mssql_buffer_v1);
            response = socketMessage.ReceiveMessage();
            socketMessage.SendMessage(NTLMSSPBuffer.mssql_buffer_v2);
            response = socketMessage.ReceiveMessage();

            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(response, _TriageNTLMSSPKey, ref response);

            return _TriageNTLMSSPKey;
        }
    }
}
