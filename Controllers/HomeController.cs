using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTC1002.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;


namespace PTC1002.Controllers
{
    public class HomeController : Controller
    {
        private const int V = 0;
        private readonly ILogger<HomeController> _logger;
        private static string pathString = @"C:\\Users\\neodev\\source\\repos\\PTC1002\\wwwroot\\data\\Data.csv";
        // Instantiate random number generator.  
        private readonly Random _random = new Random();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
           
        }
        public IActionResult Index()
        {
            var num = 0; var floatNum = 0; var alphanum = 0;
            //Calculate Percentage
            DataCount dataCount = new DataCount();
            string csvData = System.IO.File.ReadAllText(pathString);

            foreach (var row in csvData.Split("\r\n"))
            {
                //if row is numeric, dataCount will be +1
                if (CheckNumberString(row) == "num")
                {
                    num = dataCount.Numeric++;
                }
                else if (CheckNumberString(row) == "floatnum")
                {
                    floatNum = dataCount.FloatNum++;
                }
                else if (CheckNumberString(row) == "alphanum")
                {
                    alphanum = dataCount.AlphaNumeric++;
                }
                else
                {
                    return null;
                }
            }
            var total = num + floatNum + alphanum;
            ViewData["Numeric"] = (num * 100) / total;
            ViewData["AlphaNum"] = (alphanum * 100) / total;
            ViewData["Float"] = (floatNum * 100) / total;
           
            return View();
        }     
       
        // Generates a random number within a range.      
        public int RandomNumber()
        {
            return _random.Next(10000);
        }
        //Generates random float number
        public float RamdomFloat()
        {
            return (float)_random.NextDouble()*5;
        }
        // Generates a random string with a given size.    
        public string RadomAlpahNumeric(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz 0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s=>s[_random.Next(s.Length)]).ToArray());
        }

        //GenerateRamdomNumber
        public void GenerateNum()
        {
            using (var file = new StreamWriter(pathString, true))
            {
                DataTypes dataType = new DataTypes();
                var num = _random.Next(0, 3);
              
                if (num == 1)
                {

                    dataType.Numeric = RandomNumber();
                    file.WriteLine(dataType.Numeric + ',');

                }
                else if (num == 2)
                {
                    dataType.AlphaNumeric = RadomAlpahNumeric(5);

                    file.WriteLine(dataType.AlphaNumeric + ',');
                }
                else
                {
                    dataType.Float = RamdomFloat();
                    file.WriteLine(dataType.Float + ',');
                }

                file.Close();
            }
        }
       
        //Get File size
        [HttpGet]
        public JsonResult GetfileSize()
        {
            var size=0.0;          
            FileInfo fi = new FileInfo(pathString);
           
            if (fi.Exists) 
            {
                size = fi.Length / 1024;
            }
            var value = Convert.ToString(size);
            return Json(value);
        }
        
        //DataType Count
        [HttpGet]
        public JsonResult GetCount()
        {
            //calling GenNum Fun
            GenerateNum();

            string csvData = System.IO.File.ReadAllText(pathString);
            DataCount dataCount = new DataCount();
            //dataCount.Clear();
            foreach (var row in csvData.Split("\r\n"))
            {
                //if row is numeric, dataCount will be +1
                if (CheckNumberString(row) == "num")
                {
                    dataCount.Numeric++;
                }
                else if (CheckNumberString(row) == "floatnum")
                {
                    dataCount.FloatNum++;
                }
                else if(CheckNumberString(row) == "alphanum") 
                {
                    dataCount.AlphaNumeric++;
                }
                else
                {
                    return null;
                }
            }
            
            return Json(dataCount);
        }
              
        //Check Number DataType
        public string CheckNumberString(string num)
        {   //check numeric
            var isNumber = num.All(Char.IsNumber);

            if (num.Contains('.'))
            {
                return "floatnum";
            }
            else if (isNumber)
            {
                return "num";
            }
            else
            {
                return "alphanum";
            }
            
        }

        public IActionResult Report()
        {
            var total=0; var num=0; var alphanum = 0; var floatNum = 0;
            string csvData = System.IO.File.ReadAllText(pathString);
            List<DataTypes> dataTypes = new List<DataTypes>();

            //Execute a loop over the rows.
            foreach (string row in csvData.Split("\r\n").Take(20))
            {
                DataTypes dt = new DataTypes();
                if (!string.IsNullOrEmpty(row))
                {
                    if (CheckNumberString(row) == "num")
                    {
                        dt.Numeric = Int32.Parse(row);

                    }
                    else if (CheckNumberString(row) == "floatnum")
                    {
                        dt.Float = float.Parse(row);

                    }
                    else if (CheckNumberString(row) == "alphanum")
                    {
                        dt.AlphaNumeric = row;
                        alphanum++;
                    }
                    else
                    {
                        return null;
                    }
                    dataTypes.Add(dt);

                }
            }
                //Calculate Percentage
                DataCount dataCount = new DataCount();
                
                foreach (var row in csvData.Split("\r\n"))
                {
                    //if row is numeric, dataCount will be +1
                    if (CheckNumberString(row) == "num")
                    {
                        num = dataCount.Numeric++;
                    }
                    else if (CheckNumberString(row) == "floatnum")
                    {
                        floatNum = dataCount.FloatNum++;
                    }
                    else if (CheckNumberString(row) == "alphanum")
                    {
                        alphanum = dataCount.AlphaNumeric++;
                    }
                    else
                    {
                        return null;
                    }
                }
                total = num + floatNum + alphanum;
            ViewData["Numeric"] = (num * 100) / total;
            ViewData["AlphaNum"] = (alphanum * 100) / total;
            ViewData["Float"] = (floatNum * 100) / total;


            return View(dataTypes);
        }

        [ResponseCache(Duration = V, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
