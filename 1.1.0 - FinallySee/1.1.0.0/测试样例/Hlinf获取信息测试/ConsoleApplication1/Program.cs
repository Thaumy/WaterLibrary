using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib.FrameHandler;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            LibInformation LI = new LibInformation("https://thaumy.github.io/StdLib1x/xplore/st110_HIinf.html");

            //Hlinf获取信息测试
            Console.WriteLine(LI.HlinfURL);
            Console.WriteLine(LI.isNewVer);
            Console.WriteLine(LI.newVerDownloadURL);
            Console.WriteLine(LI.thisVerDownloadURL);
            Console.ReadKey();
        }
    }
}
