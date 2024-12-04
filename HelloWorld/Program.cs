﻿using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Dapper;
using HelloWorld.Data;
using Microsoft.Data.SqlClient;

namespace HelloWorld
{
    public class Computer
    {
        // private string _motherboard;
        public string Motherboard { get; set; }
        public int CPUCores { get; set; }
        public bool HasWifi { get; set; }
        public bool HasLTE { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public string VideoCard { get; set; }

        public Computer()
        {
            if (VideoCard == null)
            {
                VideoCard = "";
            }
            if (Motherboard == null)
            {
                Motherboard = "";
            }
        }
    }

internal class Program
{
    static void Main(string[] args)
    {
        Computer myComputer = new Computer() 
            {
                Motherboard = "Z690",
                HasWifi = true,
                HasLTE = false,
                ReleaseDate = DateTime.Now,
                Price = 943.87m,
                VideoCard = "RTX 2060"
            };
            // myComputer.HasWifi = false;
            // Console.WriteLine(myComputer.Motherboard);
            // Console.WriteLine(myComputer.HasWifi);
            // Console.WriteLine(myComputer.ReleaseDate);
            // Console.WriteLine(myComputer.VideoCard);

    DataContextDapper dapper = new DataContextDapper();


DateTime rightNow = dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
Console.WriteLine(rightNow.ToString());


string sql=@"INSERT INTO TutorialAppSchema.Computer (
                Motherboard,
                HasWifi,
                HasLTE,
                ReleaseDate,
                Price,
                VideoCard
) VALUES('"+myComputer.Motherboard
    + "', '"+ myComputer.HasWifi
    + "', '"+ myComputer.HasLTE
    + "', '"+ myComputer.ReleaseDate
    + "', '"+ myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
    + "', '"+ myComputer.VideoCard
   + "')";

   Console.WriteLine(sql);
     // int result =  dapper.ExecuteSqlWithRowCount(sql);
bool result =  dapper.ExecuteSql(sql);

      string sqlSelect = @"
                SELECT 
                Computer.Motherboard,
                Computer.HasWifi,
                Computer.HasLTE,
                Computer.ReleaseDate,
                Computer.Price,
                Computer.VideoCard
      FROM TutorialAppSchema.Computer";
    Console.WriteLine(result);

IEnumerable<Computer> computers= dapper.LoadData<Computer>(sqlSelect);
Console.WriteLine("'Motherboard', 'HasWifi', 'HasLTE', 'Price,'VideoCard'");
foreach(Computer singleComputer in computers)
{
    Console.WriteLine("'"+myComputer.Motherboard
     + "', '"+ myComputer.HasWifi
    + "', '"+ myComputer.HasLTE
    + "', '"+ myComputer.ReleaseDate
    + "', '"+ myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
    + "', '"+ myComputer.VideoCard
   + "'");
}

    }
}
}