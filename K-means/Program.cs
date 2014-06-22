using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Threading;
using System.Drawing;

namespace K_means
{
    class Program
    {
        static void Main(string[] args)
        {
            //int ColorValue = Color.FromName("blue").ToArgb() & 0xFFFFFF;
            //string ColorHex = string.Format("#{0:x6}", ColorValue);
            //var list= Helper.GetNumbers();
            var list = new List<double>() { 1.0, 1.1, 1.2, 5.5, 5.7, 23.4, 23.6 };
            Compute(list, 3);
        }

        static List<Crowd> Compute(List<double> list, int count)
        {
            var crowds = new List<Crowd>(count);
            list.Sort();
            var step = (int)(list.Count() / (count * 2));
            for (int i = 0; i < count; i++)
            {
                crowds.Add(new Crowd());
                crowds[i].Center = list[step + step * 2 * i];
            }

            while (crowds.Sum(crowd => crowd.Change) > 0.01)
            {
                //Empty List and refresh Center
                crowds.ForEach(crowd => { if (crowd.List.Count() != 0) { crowd.RefreshCenter(); crowd.List.Clear(); } });

                foreach (var num in list)
                {
                    int index = 0; double minDistance = double.MaxValue;
                    for (int i = 0; i < count; i++)
                    {
                        var distance = Math.Abs(crowds[i].Center - num);
                        if (distance < minDistance)
                        {
                            index = i; minDistance = distance;
                        }
                    }
                    crowds[index].List.Add(num);
                }
            }
            crowds.ForEach(crowd => Console.WriteLine(crowd.List.Max()));
            return crowds;
        }
    }

    class Crowd
    {
        public List<double> List { get; set; }

        public double Average { get { return List.Average(); } }

        public double Center { get; set; }

        public double Change { get; private set; }

        public Crowd()
        {
            Change = double.MaxValue;
            List = new List<double>();
        }

        public void RefreshCenter()
        {
            Change = Math.Abs(Average - Center);
            Center = Average;
        }
    }

    class Helper
    {
        public static List<double> GetNumbers()
        {
            string file=System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data.xlsx");
            string connStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", file);

            using (var conn = new OleDbConnection(connStr))
            {
                conn.Open();
                var sql = "SELECT * FROM [Sheet1$]";
                var cmd = new OleDbCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                List<double> result = new List<double>();
                while (reader.Read())
                {
                    result.Add((double)reader[0]);
                }
                return result;
            }
        }
    }
}
