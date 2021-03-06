﻿/*
* Copyright (c) 2015 Anshoo Arora (Relevant Codes)
* 
* MIT license
* 
* See the accompanying LICENSE file for terms.
*/

namespace ReportUnit
{
    using System;
    using System.IO;
    using System.Linq;

    using Design;
    using Logging;

    class Program
    {
        /// <summary>
        /// ReportUnit usage
        /// </summary>
        private static string reportUnitUsage = "[INFO] Usage 1:  ReportUnit \"path-to-folder\"" +
                                                "\n[INFO] Usage 2:  ReportUnit \"input-folder\" \"output-folder\" [\"Solution path (will be removed from stack frame file paths)\"|current (assembly path will be used)]" +
                                                "\n[INFO] Usage 3:  ReportUnit \"input.xml\" \"output.html\" [\"Solution path (will be removed from stack frame file paths)\"|current (assembly path will be used)]";

        /// <summary>
        /// Logger
        /// </summary>
        private static Logger logger = Logger.GetLogger();

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Accepts 3 types of input arguments
        ///     Type 1: reportunit "path-to-folder"
        ///         args.length = 1 && args[0] is a directory
        ///     Type 2: reportunit "path-to-folder" "output-folder"
        ///         args.length = 2 && both args are directories
        ///     Type 3: reportunit "input.xml" "output.html"
        ///         args.length = 2 && args[0] is xml-input && args[1] is html-output
        /// </param>
        static void Main(string[] args)
        {            
            CopyrightMessage();

            if (args.Length == 0 || args.Length > 3)
            {
                logger.Error("Invalid number of arguments specified.\n" + reportUnitUsage);
                return;
            }

            foreach (string arg in args)
            {
                if (arg.Trim() == "" || arg == "\\\\")
                {
                    logger.Error("Invalid argument(s) specified.\n" + reportUnitUsage);
                    return;
                }
            }

            for (int ix = 0; ix < args.Length; ix++)
            {
                args[ix] = args[ix].Replace('"', '\\');
            }

            if (args.Length == 2 || args.Length == 3)
            {
                if ((Path.GetExtension(args[0]).ToLower().Contains("xml")) && (Path.GetExtension(args[1]).ToLower().Contains("htm")))
                {
                    if (!Directory.GetParent(args[1]).Exists)
                        Directory.CreateDirectory(Directory.GetParent(args[1]).FullName);

                    new ReportBuilder(Theme.Standard).FileReport(args[0], args[1], args.Length > 2?args[2]:null);
                    return;
                }

                if (!Directory.Exists(args[0]))
                {
                    logger.Error("Input directory " + args[0] + " not found.");
                    return;
                }

                if (!Directory.Exists(args[1]))
                    Directory.CreateDirectory(args[1]);

                if (Directory.Exists(args[0]) && Directory.Exists(args[1]))
                {
                    new ReportBuilder(Theme.Standard).FolderReport(args[0], args[1], args.Length > 2 ? args[2] : null);
                }
                else
                {
                    logger.Error("Invalid files specified.\n" + reportUnitUsage);
                }

                return;
            }

            if (Path.GetExtension(args[0]).ToLower().Contains("xml"))
            {
                new ReportBuilder(Theme.Standard).FileReport(args[0], Path.ChangeExtension(args[0], "html"));
                return;
            }

            if (!Directory.Exists(args[0]))
            {
                logger.Error("The path of directory you have specified does not exist.\n" + reportUnitUsage);
                return;
            }

            new ReportBuilder(Theme.Standard).FolderReport(args[0]); 
        }

        private static void CopyrightMessage()
        {
            Console.WriteLine("\n--\nReportUnit v1.1. Report generator for the test-runner family.");
            Console.WriteLine("http://reportunit.relevantcodes.com/");
            Console.WriteLine("Copyright (c) 2015 Anshoo Arora (Relevant Codes)");
            Console.WriteLine("Developers:  Anshoo Arora, Sandra Greenhalgh\n--\n");
        }

    }
}
