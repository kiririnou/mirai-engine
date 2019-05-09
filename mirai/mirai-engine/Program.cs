using System;

namespace _1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            string cs = "99";
            while (cs != "exit")
            {
                Console.WriteLine("Enter case");
                cs = Console.ReadLine();
                switch (cs)
                {
                    case ("1"):
                        {

                            int CArea = rand.Next(1, 50);

                            double diam = Math.Sqrt(CArea / Math.PI) * 2;

                            Console.WriteLine("Area : " + diam * diam + "; Perimeter : " + diam * 2);

                            break;
                        }
                    case("2"):
                        {
                            double ab = Math.Sqrt(16 * 16 + 15 * 15);
                            double bc = Math.Sqrt(0 + 2 * 2);
                            double ac = Math.Sqrt(16 * 16 + 17 * 17);

                            double med = Math.Sqrt(2 * ab * ab + 2 * bc * bc + 2 * ac * ac);

                            double p = (ab + bc + ac) / 2;

                            double high = ab / 2 * Math.Sqrt((p * (p - ab) * (p - bc) * (p - ac)));

                            Console.WriteLine("height : " + high + "; median : " + med);
                                
                            break;
                        }
                    case("5"):
                        {
                            Console.WriteLine("Enter number of month");
                            int Mounth = Convert.ToInt32(Console.ReadLine());

                            int days = DateTime.DaysInMonth(2019,Mounth);

                            Console.WriteLine("Days in - "+ Mounth + " - " + days);
                            break;
                        }
                    case("6"):
                        {
                            int times = Convert.ToInt32(Console.ReadLine());

                            double firstnNum = 28.3;
                            double secondNum = 1000;

                            while(times!=0)
                            {
                                firstnNum += firstnNum;
                                secondNum += secondNum;

                                Console.WriteLine("1 род = " + firstnNum + " куб. м \t = \t" + secondNum + "куб. футів");

                                times--;
                            }
                            break;
                        }
                    case("11"):
                        {
                            int[] array = new int[10];

                            for (int i = 0; i <= array.Length-1;i++)
                            {
                                array[i]=rand.Next(1, 50);

                                Console.WriteLine(i + " - " + array[i]);
                            }

                            Console.WriteLine();

                            int tempnum = array[3];
                            array[3] = array[9];
                            array[9] = tempnum;

                            foreach(int num in array)
                            {
                                Console.WriteLine(num);
                            }

                            break;
                        }

                }

            }

        }
    }
}
