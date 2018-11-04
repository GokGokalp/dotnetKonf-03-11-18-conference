using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BadWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BadWebApp.Controllers
{
    [Route("api/calculate")]
    public class CalculationController : Controller
    {
        PrimeNumberFinderHelper _primeNumberFinderHelper;
        SerializerHelper _serializerHelper;

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(string nums)
        {
            List<CalculationResponse> response = new List<CalculationResponse>();
            int[] numbers = GetNumbersFromArgs(nums);

            _serializerHelper = new SerializerHelper();
            _primeNumberFinderHelper = new PrimeNumberFinderHelper();

            for (int number = 0; number < numbers.Length; number++)
            {
                CalculationResponse calculationResponse = new CalculationResponse();

                long nthPrimeNumber = await FindNthPrimeNumber(numbers[number]);

                calculationResponse.Content = $"{numbers[number]}nth prime number is {nthPrimeNumber}";

                double squareRootOfNthPrimeNumber = FindSquareRootOfNthPrimeNumber(nthPrimeNumber);

                calculationResponse.Content += $"\nSquare root of {numbers[number]}nth prime number is {squareRootOfNthPrimeNumber}.";

                response.Add(calculationResponse);
            }

            string result = await _serializerHelper.SerializeIntoJson(response);
            
            return Ok(result);
        }

        public struct CalculationResponse
        {
            public string Content { get; set; }
        }

        int[] GetNumbersFromArgs(string nums)
        {
            int[] _nums =  nums.Split(',').Select(x =>
            {
                (bool, int) input = CheckIsItANumber(x);

                if (input.Item1)
                {
                    return input.Item2;
                }

                return default(int);
            }).ToArray();

            return _nums;
        }

        //Some child methods for profiling callchain.
        double FindSquareRootOfNthPrimeNumber(double nthPrimeNumber)
        {
            double squareRoot = 0;

            squareRoot = Math.Sqrt(nthPrimeNumber);

            Task.Delay(10000);

            return squareRoot;
        }

        //Some child methods for profiling callchain.
        async Task<long> FindNthPrimeNumber(int number)
        {
            //CPU intensive sample for profiling with perf on linux.
            long nthPrimeNumber = await _primeNumberFinderHelper.FindNthPrimeNumber(number);

            return nthPrimeNumber;
        }

        //Some child methods for profiling callchain.
        (bool, int) CheckIsItANumber(string num)
        {
            int? result = null;

            //try
            //{
                result = int.Parse(num);
            //}
            //catch (System.Exception ex)
            //{
                // DotNETRuntime:Exception* tracepoint sample.
             //   Console.WriteLine("Exception occured while parsing.");
            //}

            return (result != null, result.GetValueOrDefault()); // It can be causing wrong business logic.
        }
    }
}
