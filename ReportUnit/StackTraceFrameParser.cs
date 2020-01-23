using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportUnit
{
    /// <summary>
    /// Processes stack trace text report
    /// </summary>
    class StackTraceFrameParser
    {
        private static StackTraceFrameParser Null = new StacktracePathManagerNull();
        public static StackTraceFrameParser Instance { get; private set; } = Null;
        public string AssemblyPath { get; set; }

        /// <summary>
        /// Use provided path.
        /// </summary>
        /// <param name="path"></param>
        protected StackTraceFrameParser(string path)
        {
            if (path == null || path.ToLower() == "current" )
            {
                AssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                AssemblyPath = Path.GetDirectoryName(AssemblyPath);
            }
            else
                AssemblyPath = path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processData">When false, nothing is changed.</param>
        public static void Initialize(string arg)
        {
            Instance = string.IsNullOrEmpty(arg) ? Null : new StackTraceFrameParser(arg);
        }

        /// <summary>
        /// Removes path to files in report which have path same as current assembly path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string RemoveFolderPath(string filePath)
        {
            return filePath.Replace(AssemblyPath, "..\\");// ?
                        //$"{filePath[0]}:\\..." + filePath.Substring(AssemblyPath.Length) :
                        //filePath;                
        }
        private string MarkFilePaths(string s)
        {
            Regex r = new Regex(@"in ([a-zA-Z0-9\s\n\r:\\\._<>]*)\\([a-zA-Z0-9_\.\\]*\.(c|C)s:line [0-9]*)");//(@"in ([a-zA-Z0-9\s\n\r\.:\\_<>]*:line [0-9]*)");
            s = r.Replace(s, "in <span class='marked_lines'>$1\r\n\\$2</span>");
            return s;
        }

        /// <summary>
        /// Processes stack frame (containing At [assembly] and In [file])
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual string ParseFrame(string s)
        {
            string content = s;//.Replace s.Substring(s.IndexOf(" in "));
            content = RemoveFolderPath(content);
            content = MarkFilePaths(content);
            return content.Replace(" at ", " <br>At ").Replace(" in ", " <br>In ");
        }
    }

    class StacktracePathManagerNull:StackTraceFrameParser
    {
        public StacktracePathManagerNull():base(null)
        {

        }
        public override string ParseFrame(string s)
        {
            return s;
        }
    }
}
