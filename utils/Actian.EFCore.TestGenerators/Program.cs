﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Actian.EFCore.TestGenerators
{
    static class Program
    {
        static Program()
        {
            Paths.Init();
        }

        static void Main()
        {
            foreach (var generate in GetGenerators())
            {
                generate();
            }
        }

        static IEnumerable<Action> GetGenerators()
        {
            return typeof(Program).Assembly.GetTypes()
                .OrderBy(type => type.Name)
                .Select(type => type.GetMethod("Generate", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null))
                .Where(method => method is not null)
                .Select(method => (Action)(() =>
                {
                    method.Invoke(null, null);
                }));
        }
    }
}
