using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGrupo3S.Helpers
{
    public class CompleteString
    {
        public string Complete(int num, params object[] longstring) {

            string parts = "";

            for (int i = 0; i<longstring.Length; i++) {

                if (longstring[i] == null)
                {
                    longstring[i] = "null";
                }

                parts = parts + longstring[i]+",";

            }

            Console.WriteLine(string.Concat(longstring));
            return (parts);
        }
    }
}