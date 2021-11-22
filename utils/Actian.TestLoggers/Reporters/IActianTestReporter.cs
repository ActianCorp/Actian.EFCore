﻿using System.Collections.Generic;
using System.Linq;

namespace Actian.TestLoggers
{
    public interface IActianTestReporter
    {
        void CreateReport(IEnumerable<ActianTestProject> testProjects);
    }

    public static class ActianTestReporterExtensions
    {
        public static void CreateReport(this IActianTestReporter reporter, params ActianTestProject[] testProjects)
        {
            reporter.CreateReport(testProjects.AsEnumerable());
        }

        public static void CreateReports(this IEnumerable<IActianTestReporter> reporters, IEnumerable<ActianTestProject> testProjects)
        {
            foreach (var reporter in reporters)
            {
                reporter.CreateReport(testProjects);
            }
        }

        public static void CreateReports(this IEnumerable<IActianTestReporter> reporters, params ActianTestProject[] testProjects)
        {
            reporters.CreateReports(testProjects.AsEnumerable());
        }
    }
}
