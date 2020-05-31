using System;
using System.Collections.Generic;
using zsNBT;

namespace NBTTester
{
    public class Test2
    {
        public string Test2Str = "Test 2";
        public List<int> TestList = new List<int>();

        public Test2()
        {
            TestList.Add(4);
        }

        public List<byte> testByteList = new List<byte>();
        public List<float> testFloatList = new List<float>();

    }
    public class TestClass
    {
        public string Name = "Test";
        public int Val = 128;
        public Test2 t2 = new Test2();
        public string DYN;
        public double TestDouble = 3.5;
        public string[] TestStringArray = new string[] { "Test1", "Test2" };
        public int[] TestIntArray = new int[] { 9, 438, 2 };
        public float TestFloat = 32.34f;
        public double[] TestDoubleArray = new double[] { 4.3, 4.6 };
    }
    class Program
    {
        static void Main(string[] args)
        {
            NBTFolder folder = new NBTFolder("Root");
            
            NBTString str = new NBTString("Test", "TestValue");
            folder.Add(str);

            TestClass vx = new TestClass();
            vx.t2.TestList.Add(231);
            vx.DYN = "VerifyCode:30";
            NBTFolder vx_root = vx.ToNBT<TestClass>();

            Console.WriteLine(folder.ToString());


            Console.WriteLine(vx_root.ToString());

            vx_root.WriteFile("Test.NBT");


            NBTFolder vx_root2 = new NBTFolder();
            vx_root2.ReadFile("Test.NBT");
            vx_root2 = vx_root2[0] as NBTFolder;

            Console.WriteLine(vx_root2.ToString());

            TestClass vx2 = vx_root2.FromNBT<TestClass>();

            Console.WriteLine(vx2.DYN);

            if(vx2.DYN == "VerifyCode:30")
            {
                Console.WriteLine("Data verified!\n" + vx_root2.ToString());
            }
        }
    }
}
