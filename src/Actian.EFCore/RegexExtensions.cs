// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Text.RegularExpressions;

namespace Actian.EFCore
{
    public static class RegexExtensions
    {
        public static bool Matches(this string input, Regex re, out Match match)
        {
            match = re.Match(input);
            return match.Success;
        }

        public static bool Matches(this string input, string pattern, out Match match)
        {
            return input.Matches(new Regex(pattern), out match);
        }

        public static bool Matches(this string input, string pattern, RegexOptions options, out Match match)
        {
            return input.Matches(new Regex(pattern, options), out match);
        }

        public static bool IsMatch(this string input, Regex re)
        {
            return re.IsMatch(input);
        }

        public static bool IsMatch(this string input, string pattern)
        {
            return input.IsMatch(new Regex(pattern));
        }

        public static bool IsMatch(this string input, string pattern, RegexOptions options)
        {
            return input.IsMatch(new Regex(pattern, options));
        }

        public static string ReplaceRegex(this string input, Regex re, string replacement)
        {
            return re.Replace(input, replacement);
        }

        public static string ReplaceRegex(this string input, Regex re, MatchEvaluator evaluator)
        {
            return re.Replace(input, evaluator);
        }

        public static string ReplaceRegex(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string ReplaceRegex(this string input, string pattern, string replacement, RegexOptions options)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        public static string ReplaceRegex(this string input, string pattern, MatchEvaluator evaluator)
        {
            return Regex.Replace(input, pattern, evaluator);
        }

        public static string ReplaceRegex(this string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
        {
            return Regex.Replace(input, pattern, evaluator, options);
        }
    }
}
