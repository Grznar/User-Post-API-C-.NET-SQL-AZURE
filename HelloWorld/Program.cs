using System;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
            
            //ASYNC AWAIT
Task firstTask=new Task(() => {
    Thread.Sleep(100);
    Console.WriteLine("Task 1");
});
firstTask.Start();
Task thirdTask=ConsoleAfterDelayAsync("task 3",50);
Task secondTask=ConsoleAfterDelayAsync("task 2",150);


ConsoleAfterDelay("Delay",75);
Console.WriteLine("After the Task was created ");


await firstTask;
await secondTask;
await thirdTask;
    }
    static async Task  ConsoleAfterDelayAsync(string text,int delayTime)
    {
        await Task.Delay(delayTime);
        Console.WriteLine(text);
    }
    static void  ConsoleAfterDelay(string text,int delayTime)
    {
        Task.Delay(delayTime);
        Console.WriteLine(text);
    }
}
//             IConfiguration config = new ConfigurationBuilder()
//             .AddJsonFile("appsettings.json").Build();

//             DataContextDapper dapper = new DataContextDapper(config);


//             // Console.WriteLine(rightNow.ToString());
            
//         //     Computer myComputer = new Computer() 
//         //     {
//         //         ComputerId = 0,
//         //         Motherboard = "Z690",
//         //         HasWifi = true,
//         //         HasLTE = false,
//         //         ReleaseDate = DateTime.Now,
//         //         Price = 943.87m,
//         //         VideoCard = "RTX 2060"
//         //     };

            

            

//         //     string sql = @"INSERT INTO TutorialAppSchema.Computer (
//         //         Motherboard,
//         //         HasWifi,
//         //         HasLTE,
//         //         ReleaseDate,
//         //         Price,
//         //         VideoCard
//         //     ) VALUES ('" + myComputer.Motherboard 
//         //             + "','" + myComputer.HasWifi
//         //             + "','" + myComputer.HasLTE
//         //             + "','" + myComputer.ReleaseDate.ToString("yyyy-MM-dd")
//         //             + "','" + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
//         //             + "','" + myComputer.VideoCard
//         //     + "') ";

//         // File.WriteAllText("log.txt",sql+"\n\n");

//         //     using StreamWriter openFile=new("log.txt",append:true);
            
//         //     openFile.WriteLine(sql);
//         //     openFile.Close();
//             string computersJson = File.ReadAllText("ComputersSnake.json");
//             Mapper mapper = new Mapper(new MapperConfiguration((cfg)=>
//             {
//             cfg.CreateMap<ComputerSnake,Computer>()
//             .ForMember(destination =>destination.ComputerId,options =>
//                     options.MapFrom(source=>source.computer_id))
           
//             .ForMember(destination =>destination.HasLTE,options =>
//                     options.MapFrom(source=>source.has_lte))
            
//             .ForMember(destination =>destination.CPUCores, options =>
//                     options.MapFrom(source=>source.cpu_cores))
            
//             .ForMember(destination =>destination.Motherboard,options =>
//                     options.MapFrom(source=>source.motherboard))
            
//             .ForMember(destination =>destination.HasWifi,options =>
//                     options.MapFrom(source=>source.has_wifi))
//             .ForMember(destination =>destination.Price,options =>
//                     options.MapFrom(source=>source.price))
//                     .ForMember(destination =>destination.ReleaseDate,options =>
//                     options.MapFrom(source=>source.release_date))
//             .ForMember(destination =>destination.VideoCard,options =>
//                     options.MapFrom(source=>source.video_card));
            
//             }
//             ));
//             IEnumerable<Computer>?computersSystem = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson);
// // IEnumerable<ComputerSnake>?computersSystem=System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(computersJson);
// if(computersSystem!=null)
// {
// IEnumerable<Computer>computersResult = mapper.Map<IEnumerable<Computer>>(computersSystem);
// Console.WriteLine("MAPPING:  "+computersResult.Count());

// //foreach(Computer computer in computersSystem)
// //{
// //Console.WriteLine(computer.Motherboard);
// //}
// }
//  IEnumerable<Computer>?computersJsonPropertyMapping = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson);
// if(computersJsonPropertyMapping!=null)
// {
// Console.WriteLine("JSON PROPERT: "+computersJsonPropertyMapping.Count());
// }

// //foreach(Computer computer in computersSystem)
// //{
// //Console.WriteLine(computer.Motherboard);
// //}
// }

    


// //           //  Console.WriteLine(computersJson);
// //             JsonSerializerOptions options =new JsonSerializerOptions()
// //             { 
// //                 PropertyNamingPolicy =JsonNamingPolicy.CamelCase
// //             };
//             // IEnumerable<Computer>?computersNewtsoft = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);
// //  IEnumerable<Computer>?computersSystem = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);

// //             if(computersNewtsoft!=null)
// //             {
// //                 foreach(Computer computer in computersNewtsoft)
// //                 {   
// //                     //Console.WriteLine(computer.Motherboard);
// //                         string sql = @"INSERT INTO TutorialAppSchema.Computer (
// //                 Motherboard,
// //                 HasWifi,
// //                 HasLTE,
// //                 ReleaseDate,
// //                 Price,
// //                 VideoCard
// //             ) VALUES ('" + EscapeSingleQuote(computer.Motherboard)
// //                     + "','" + computer.HasWifi
// //                     + "','" + computer.HasLTE
// //                     + "','" + computer.ReleaseDate?.ToString("yyyy-MM-dd")
// //                     + "','" + computer.Price
// //                     + "','" + EscapeSingleQuote(computer.VideoCard)
// //             + "') ";
// //             dapper.ExecuteSql(sql);
// //                 }
// //             }
// //             JsonSerializerSettings settings = new JsonSerializerSettings()
// //             {
// //              ContractResolver=new CamelCasePropertyNamesContractResolver()
// //             };
// //             string computersCopy=JsonConvert.SerializeObject(computersNewtsoft,settings);
// //              File.WriteAllText("computersCopyNewtsoft.txt",computersCopy);
// //  string computersCopySystem=System.Text.Json.JsonSerializer.Serialize(computersSystem,options);
// //              File.WriteAllText("computersCopySystem.txt",computersCopySystem);

// //         }
// // static string EscapeSingleQuote(string input)
// // {
// // string output= input.Replace("'","''");
// // return output;
// // }






//             // // Console.WriteLine(sql);

//             // // int result = dapper.ExecuteSqlWithRowCount(sql);
//             // bool result = dapper.ExecuteSql(sql);

//             // // Console.WriteLine(result);

//             // string sqlSelect = @"
//             // SELECT 
//             //     Computer.ComputerId,
//             //     Computer.Motherboard,
//             //     Computer.HasWifi,
//             //     Computer.HasLTE,
//             //     Computer.ReleaseDate,
//             //     Computer.Price,
//             //     Computer.VideoCard
//             //  FROM TutorialAppSchema.Computer";

//             // IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);

//             // Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'" 
//             //     + ",'Price','VideoCard'");
//             // foreach(Computer singleComputer in computers)
//             // {
//             //     Console.WriteLine("'" + singleComputer.ComputerId 
//             //         + "','" + singleComputer.Motherboard
//             //         + "','" + singleComputer.HasWifi
//             //         + "','" + singleComputer.HasLTE
//             //         + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
//             //         + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
//             //         + "','" + singleComputer.VideoCard + "'");
//             // }

//             // IEnumerable<Computer>? computersEf = entityFramework.Computer?.ToList<Computer>();

//             // if (computersEf != null)
//             // {
//             //     Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'" 
//             //         + ",'Price','VideoCard'");
//             //     foreach(Computer singleComputer in computersEf)
//             //     {
//             //         Console.WriteLine("'" + singleComputer.ComputerId 
//             //             + "','" + singleComputer.Motherboard
//             //             + "','" + singleComputer.HasWifi
//             //             + "','" + singleComputer.HasLTE
//             //             + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
//             //             + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
//             //             + "','" + singleComputer.VideoCard + "'");
//             //     }
//             // }

//             // // myComputer.HasWifi = false;
//             // // Console.WriteLine(myComputer.Motherboard);
//             // // Console.WriteLine(myComputer.HasWifi);
//             // // Console.WriteLine(myComputer.ReleaseDate);
//             // // Console.WriteLine(myComputer.VideoCard);
        








}




