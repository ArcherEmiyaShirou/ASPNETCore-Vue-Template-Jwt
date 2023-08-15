using System.Net;
using System.Text.Json;

namespace Backend.Contract.Entity
{
    public record CustomResponse<T> (int Code,T? Data, string Message) where T : class
    {
        public static CustomResponse<T> Success(T? Data)
        {
            return new CustomResponse<T>(200, Data, "请求成功");
        }

        public static CustomResponse<T> Success()
        {
            return Success(null);
        }

        public static CustomResponse<T> Failure(int code,string message)
        {
            return new CustomResponse<T>(code, null, message);
        }

        public static CustomResponse<T> Forbidden(string message)
        {
            return Failure((int)HttpStatusCode.Forbidden, message);
        }

        public static CustomResponse<T> UnAuthorized(string message)
        {
            return Failure((int)HttpStatusCode.Unauthorized, message);
        }

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
