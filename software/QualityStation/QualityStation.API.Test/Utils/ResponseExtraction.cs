using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.API.Test.Utils
{
    public class ResponseExtraction
    {
        public static T GetObjectFromOkResponse<T>(ActionResult<T?> response)
        {
            var objectResult = response.Result.Should().BeOfType<OkObjectResult>().Subject;
            return objectResult.Value.Should().BeAssignableTo<T>().Subject;
        }

        public static List<T> GetListObjectFromOkResponse<T>(ActionResult<List<T>> response)
        {
            var objectResult = response.Result.Should().BeOfType<OkObjectResult>().Subject;
            return objectResult.Value.As<List<T>>();
        }

        public static string GetErrorMessageFromBadRequestResponse<T>(ActionResult<T?> response)
        {
            var objectResult = response.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            return (string)objectResult.Value!;
        }
        public static string GetErrorMessageFromConflictResponse<T>(ActionResult<T?> response)
        {
            var objectResult = response.Result.Should().BeOfType<ConflictObjectResult>().Subject;
            return (string)objectResult.Value!;
        }
        public static string GetErrorMessageFromNotFoundResponse<T>(ActionResult<T?> response)
        {
            var objectResult = response.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            return (string)objectResult.Value!;
        }
    }
}
