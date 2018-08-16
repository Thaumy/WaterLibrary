using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib;
using StdLib.FrameHandler;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)//Jlinf获取信息测试
        {
            LibInformation LI = new LibInformation("");
            JlinfObject jo = LI.getJsonLibInformation("https://thaumy.github.io/StdLib1x/xplore/st110_Jlinf.json");

            Console.WriteLine(jo.projectVer);
            Console.WriteLine(jo.projectMoniker);
            Console.WriteLine(jo.editionType);
            Console.WriteLine(jo.stepping);
            Console.WriteLine(jo.targetFramework);
            Console.WriteLine(jo.targetFrameworkMoniker);
            Console.WriteLine(jo.compat);
            Console.WriteLine(jo.platform);
            Console.WriteLine(jo.architecture);
            Console.WriteLine(jo.developmentCode);
            Console.WriteLine(jo.summary);
            Console.WriteLine(jo.isNewVer);
            Console.WriteLine(jo.newVerURL);


            LibInformation local_LI = new LibInformation("");
            JlinfObject local_jo = LI.getJsonLibInformation(@"C:\Users\Thaumy\Desktop\StdLib1.10\测试文件\st110_Jlinf.json",2048,true);

            Console.WriteLine(local_jo.projectVer);
            Console.WriteLine(local_jo.projectMoniker);
            Console.WriteLine(local_jo.editionType);
            Console.WriteLine(local_jo.stepping);
            Console.WriteLine(local_jo.targetFramework);
            Console.WriteLine(local_jo.targetFrameworkMoniker);
            Console.WriteLine(local_jo.compat);
            Console.WriteLine(local_jo.platform);
            Console.WriteLine(local_jo.architecture);
            Console.WriteLine(local_jo.developmentCode);
            Console.WriteLine(local_jo.summary);
            Console.WriteLine(local_jo.isNewVer);
            Console.WriteLine(local_jo.newVerURL);

            Console.ReadKey();
        }
    }
}
