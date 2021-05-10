using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpDetectionNTLMSSP
{
    class FormatUtils
    {
        public static int ReadInt2(byte[] src, int srcIndex)
        {
            return unchecked(src[srcIndex] & 0xFF)
                             + ((src[srcIndex + 1] & 0xFF) << 8);
        }

        public static NTLM_CHALLENGE_MESSAGE ChallengeFromBytes(byte[] arr)
        {
            NTLM_CHALLENGE_MESSAGE str = new NTLM_CHALLENGE_MESSAGE();
            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            str = (NTLM_CHALLENGE_MESSAGE)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);
            return str;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct NTLM_CHALLENGE_MESSAGE
        {
            public Int64 Signature;
            public Int32 MessageType;
            //public long TargetNameFields;
            public Int16 TargetNameLen;
            public Int16 TargetNameMaxLen; 
            public Int32 TargetNameBufferOffset;

            public Int32 NegotiateFlags;
            public Int64 ServerChallenge;
            public Int64 Reserved;
            //public long TargetInfoFields;
            public Int16 TargetInfoLen;
            public Int16 TargetInfoMaxLen;
            public Int32 TargetInfoBufferOffset;

            //public long Version;
            public Byte Major;
            public Byte Minor;
            public Int16 Build;
            public Int32 NTLM_Current_Revision;
        }
    }
}
