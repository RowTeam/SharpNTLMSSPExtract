using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static SharpDetectionNTLMSSP.FormatUtils;

namespace SharpDetectionNTLMSSP
{
    public class ParsingResponse
    {
        private static TriageNTLMSSPKey ParseTargetInfo(byte[] records, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            int pos = 0;
            while (pos + 4 < records.Length)
            {
                int recordType = FormatUtils.ReadInt2(records, pos);
                int recordLength = FormatUtils.ReadInt2(records, pos + 2);
                pos += 4;

                switch (recordType)
                {
                    case 1:
                        _TriageNTLMSSPKey.NbtComputerName = Encoding.Unicode.GetString(records, pos, recordLength);                        
                        break;
                    case 2:
                        _TriageNTLMSSPKey.NbtDomainName = Encoding.Unicode.GetString(records, pos, recordLength);                       
                        break;
                    case 3:
                        _TriageNTLMSSPKey.DnsComputerName = Encoding.Unicode.GetString(records, pos, recordLength);                        
                        break;
                    case 4:
                        _TriageNTLMSSPKey.DnsDomainName = Encoding.Unicode.GetString(records, pos, recordLength);                        
                        break;
                    case 7:
                        _TriageNTLMSSPKey.TimeStamp = DateTime.FromFileTime(BitConverter.ToInt64(records, pos));                        
                        break;
                }
                pos += recordLength;
            }
            return _TriageNTLMSSPKey;
        }

        public static TriageNTLMSSPKey ParsingSocketStremResponse(byte[] responseBuffer, TriageNTLMSSPKey _TriageNTLMSSPKey, ref byte[] otherResponseBuffer)
        {
            try {
                var responseBuffer_String = BitConverter.ToString(responseBuffer).Replace("-", "");
                var NTLMSSP_Bytes_Index = responseBuffer_String.IndexOf("4E544C4D53535000") / 2;

                var len = responseBuffer.Length - NTLMSSP_Bytes_Index;
                var challengeResult = new Byte[len];
                Array.Copy(responseBuffer, NTLMSSP_Bytes_Index, challengeResult, 0, len);

                NTLM_CHALLENGE_MESSAGE typeMessage = ChallengeFromBytes(challengeResult);

                _TriageNTLMSSPKey.OsBuildNumber = typeMessage.Build;
                _TriageNTLMSSPKey.OsMajor = typeMessage.Major;
                _TriageNTLMSSPKey.OsMinor = typeMessage.Minor;

                var TargetInfo = challengeResult.Skip(typeMessage.TargetInfoBufferOffset).ToArray().Take(typeMessage.TargetInfoLen).ToArray();
                _TriageNTLMSSPKey = ParseTargetInfo(TargetInfo, _TriageNTLMSSPKey);

                var otherOffset = typeMessage.TargetInfoBufferOffset + typeMessage.TargetInfoLen;
                len = len - otherOffset;
                var otherByteResult = new Byte[len];
                Array.Copy(challengeResult, otherOffset, otherByteResult, 0, len);
                otherResponseBuffer = otherByteResult;
            }
            catch {}
            return _TriageNTLMSSPKey;
        }
    }
}
