﻿using System;

namespace SFA.DAS.CourseManagement.OuterApi.MockServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OuterApiMockServer.Run();
            Console.WriteLine(("Press any key to stop the server..."));
            Console.ReadKey();
        }
    }
}
