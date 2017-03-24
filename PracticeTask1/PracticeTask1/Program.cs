using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = User.GetDefaultUser();
            Console.WriteLine(user);
        }
    }
}
