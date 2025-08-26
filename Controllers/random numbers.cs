using Microsoft.AspNetCore.Mvc;
using System;

namespace randomnumbers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomController : ControllerBase
    {
        private static readonly Random _random = new Random();

        // GET: api/random/number
        [HttpGet("number")]
        public IActionResult GetRandomNumber([FromQuery] int? min, [FromQuery] int? max)
        {
            try
            {
                int minValue = min ?? 0;
                int maxValue = max ?? 100;

                if (minValue > maxValue)
                {
                    return BadRequest(new { error = "Bad Request", message = "min no puede ser mayor que max" });
                }

                int result = _random.Next(minValue, maxValue + 1);

                return Ok(new { 
                    result, 
                    min = minValue, 
                    max = maxValue,
                    operation = "random number"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        // GET: api/random/decimal
        [HttpGet("decimal")]
        public IActionResult GetRandomDecimal()
        {
            try
            {
                double result = _random.NextDouble();
                return Ok(new { 
                    result = Math.Round(result, 6), 
                    range = "0 to 1",
                    operation = "random decimal"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        // GET: api/random/string
        [HttpGet("string")]
        public IActionResult GetRandomString([FromQuery] int? length)
        {
            try
            {
                int stringLength = length ?? 8;

                if (stringLength < 1 || stringLength > 1024)
                {
                    return BadRequest(new { 
                        error = "Bad Request", 
                        message = "Length must be between 1 and 1024 characters" 
                    });
                }

                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                char[] result = new char[stringLength];
                
                for (int i = 0; i < stringLength; i++)
                {
                    result[i] = chars[_random.Next(chars.Length)];
                }

                return Ok(new { 
                    result = new string(result), 
                    length = stringLength,
                    operation = "random string"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        // POST: api/random/custom
        [HttpPost("custom")]
        public IActionResult GetCustomRandom([FromBody] CustomRandomRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Type))
                {
                    return BadRequest(new { error = "Bad Request", message = "Missing required field: type" });
                }

                object result;
                string operation;

                switch (request.Type.ToLower())
                {
                    case "number":
                        int min = request.Min ?? 0;
                        int max = request.Max ?? 100;

                        if (min > max)
                        {
                            return BadRequest(new { error = "Bad Request", message = "min cannot be greater than max" });
                        }

                        result = _random.Next(min, max + 1);
                        operation = "custom random number";
                        break;

                    case "decimal":
                        int decimals = request.Decimals ?? 2;
                        
                        if (decimals < 0 || decimals > 15)
                        {
                            return BadRequest(new { 
                                error = "Bad Request", 
                                message = "decimals must be between 0 and 15" 
                            });
                        }

                        double decimalValue = _random.NextDouble();
                        result = Math.Round(decimalValue, decimals);
                        operation = "custom random decimal";
                        break;

                    case "string":
                        int length = request.Length ?? 8;

                        if (length < 1 || length > 1024)
                        {
                            return BadRequest(new { 
                                error = "Bad Request", 
                                message = "length must be between 1 and 1024" 
                            });
                        }

                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        char[] stringResult = new char[length];
                        
                        for (int i = 0; i < length; i++)
                        {
                            stringResult[i] = chars[_random.Next(chars.Length)];
                        }

                        result = new string(stringResult);
                        operation = "custom random string";
                        break;

                    default:
                        return BadRequest(new { 
                            error = "Bad Request", 
                            message = "Invalid type. Must be 'number', 'decimal', or 'string'" 
                        });
                }

                return Ok(new { result, operation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        // GET: api/random/health
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", service = "Random Numbers API" });
        }
    }

    // Clase para el request del endpoint custom
    public class CustomRandomRequest
    {
        public string Type { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public int? Decimals { get; set; }
        public int? Length { get; set; }
    }
}