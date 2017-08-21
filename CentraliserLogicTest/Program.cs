using CentraliserLogicTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentraliserLogicTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CentraliseService _centraliseService = new CentraliseService();
            var input = "";
            do
            {
                _centraliseService.Calculate();
                input = Console.ReadLine();

                if (input == "c")
                {
                    //CENTRALISE!!!!!
                    _centraliseService.Centralise();
                }

            } while (input != "s");

            

            
        }
    }
}
