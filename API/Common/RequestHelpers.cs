using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Common;
public class RequestHelpers {
    public static IConfiguration? Configuration = null;

    public static JsonResult Success(Dictionary<string, object>? data = null, HttpResponse? response = null) {
        var dict = data ?? new Dictionary<string, object>();
        dict["success"] = true;
        if (response != null)
            response.StatusCode = (int)HttpStatusCode.OK;
        return new JsonResult(dict) { StatusCode = (int)HttpStatusCode.OK };
    }

    public static JsonResult Failure(Dictionary<string, object>? data = null, HttpResponse? response = null, int code = (int)HttpStatusCode.InternalServerError) {
        var dict = data ?? new Dictionary<string, object>();
        dict["success"] = false;
        if (response != null)
            response.StatusCode = code;
        return new JsonResult(dict) { StatusCode = code };
    }
    
    public static JsonResult Json(object? data, HttpResponse? response = null, int code = (int)HttpStatusCode.OK)
    {
        if (response != null)
            response.StatusCode = code;
        return new JsonResult(data) { StatusCode = code };
    }    

    public static Dictionary<string, object> ToDict(params object[] keyvalues)
    {
        if (keyvalues.Length % 2 != 0)
            throw new ArgumentException("Parameters must be the pairs, i.e. it must be an even number of parameters");
        var dict = new Dictionary<string, object>();
        for (var i = 0; i < keyvalues.Length; i += 2)
        {
            dict[keyvalues[i] + ""] = keyvalues[i + 1];
        }
        return dict;
    }
}

public enum ErrorCodes {
    BadCredentials,
    RemoteServerError,
    RemoteServiceConfigError
}
