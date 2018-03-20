using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGrupo3S.ValidationHelper;

namespace WebGrupo3S.Helpers
{
    public class WriteLogMessages
    {
        public static void WriteFile(object userName, string input)
        {
            Logs miLogs = new Logs("logs", "c:\\");
            string user = userName == null ? "Unknown User" : userName.ToString();
            miLogs.Add(user+"@"+input);
        }
    }
}