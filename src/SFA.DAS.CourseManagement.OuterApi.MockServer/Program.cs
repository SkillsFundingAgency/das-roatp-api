using System;

namespace SFA.DAS.CourseManagement.OuterApi.MockServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Outer API Server on http://localhost:5021");
            OuterApiMockServer.Run();
            Console.WriteLine(("Press any key to stop the server"));
            Console.ReadKey();
        }
    }
}
