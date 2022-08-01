using System;

namespace TopLearn.Core.Generator
{
    public class Generator
    {
        public static string GenerateUniqCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
