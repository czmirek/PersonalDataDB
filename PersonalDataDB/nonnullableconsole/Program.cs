using nullablelib;
using System;

namespace nonnullableconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestClass.DoSomething(null);
            Console.ReadLine();
        }
    }
}
