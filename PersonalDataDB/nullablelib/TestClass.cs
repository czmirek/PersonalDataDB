using System.Diagnostics.CodeAnalysis;

namespace nullablelib
{
    public class TestClass
    {
        public static string? DoSomething([DisallowNull] string nonNullString)
        {
            if(nonNullString == "test")
            {
                return null;
            }
            return nonNullString.Substring(0,2);
        }
    }
}
