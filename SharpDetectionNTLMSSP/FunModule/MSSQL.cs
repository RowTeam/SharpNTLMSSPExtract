namespace SharpDetectionNTLMSSP.FunModule
{
    class MSSQL : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            socketMessage.SendMessage(NTLMSSPBuffer.mssql_buffer_v1);
            var response = socketMessage.ReceiveMessage();
            socketMessage.SendMessage(NTLMSSPBuffer.mssql_buffer_v2);
            response = socketMessage.ReceiveMessage();

            if (response.Length == 0) return null;
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(response, _TriageNTLMSSPKey, ref response);

            return _TriageNTLMSSPKey;
        }
    }
}
