using System;
using System.Management;
using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32
{
    /// <summary>
    /// Helper to detect whether provided process id is running under administrative privilege 
    /// From: https://stackoverflow.com/questions/53459123/check-if-another-process-has-admin-privileges/53460693#53460693
    /// </summary>
    public class ProcessHelper
    {
        #region CONSTANTES

        private const int TopkenDuplicate = 0x0002;
        private const int TokenQuery = 0x0008;

        private const int SecurityImpersonation = 2;
        private const int TokenImpersonation = 2;

        #endregion

        #region API32

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DuplicateTokenEx(IntPtr hTok, UInt32 DesiredAccess, IntPtr SecAttPtr, int ImpLvl, int TokType, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateWellKnownSid(WELL_KNOWN_SID_TYPE WellKnownSidType, IntPtr DomainSid, IntPtr pSid, ref uint cbSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CheckTokenMembership(IntPtr TokenHandle, IntPtr SidToCheck, out bool IsMember);

        private enum ProcessAccessFlags : uint
        {
            QueryInformation = 0x0400,
            QueryLimitedInformation = 0x1000
        }

        private enum WELL_KNOWN_SID_TYPE
        {
            WinBuiltinAdministratorsSid = 26
        }

        #endregion

        public static bool IsRunningAsAdministrator(int processId)
        {
            // try to access process with limited information access rights
            var processHandle = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
            if (processHandle == IntPtr.Zero)
            {
                processHandle = OpenProcess(ProcessAccessFlags.QueryInformation, false, processId); // < Vista
            }
            var processHandleWatcher = new CloseHandleWatcher(processHandle);

            var principalTokenHandle = IntPtr.Zero;
            var haveToken = OpenProcessToken(processHandle, TopkenDuplicate, out principalTokenHandle);
            var principalTokenHandleWatcher = new CloseHandleWatcher(principalTokenHandle);

            var impersonatedTokenHandle = IntPtr.Zero;
            if (haveToken)
            {
                haveToken = DuplicateTokenEx(principalTokenHandle, TokenQuery, IntPtr.Zero, SecurityImpersonation, TokenImpersonation, out impersonatedTokenHandle);
                var impersonatedTokenHandleWatcher = new CloseHandleWatcher(impersonatedTokenHandle);

                // inquire the size of the Sid data to be created
                var sidUseLocalComputerDomain = IntPtr.Zero;
                uint numberOfBytesToBeAllocated = 0;
                CreateWellKnownSid(WELL_KNOWN_SID_TYPE.WinBuiltinAdministratorsSid, sidUseLocalComputerDomain, IntPtr.Zero, ref numberOfBytesToBeAllocated);

                // create the built in admin Sid pointer
                var comMemoryWatcher = new MarshalledComMemoryWatcher(Convert.ToInt32(numberOfBytesToBeAllocated));
                var sidPointer = comMemoryWatcher.MemoryBlock;
                var succeed = sidPointer != IntPtr.Zero && CreateWellKnownSid(WELL_KNOWN_SID_TYPE.WinBuiltinAdministratorsSid, IntPtr.Zero, sidPointer, ref numberOfBytesToBeAllocated);

                // check whether the impersonated orocess token is memebr of the built in admin group
                bool isMember = false;
                return succeed &&
                    CheckTokenMembership(impersonatedTokenHandle, sidPointer, out isMember) &&
                    isMember;
            }
            return false;
        }

        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            var searcher = new ManagementObjectSearcher(query);
            var processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
    }
}
