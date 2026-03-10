//Tekniskt sett en DTO, denna klass skickar skickar success/failure information.

internal class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";

    public static ServiceResult Success(string message = "")
    {
        return new ServiceResult { IsSuccess = true, Message = message };
    }

    public static ServiceResult Failure(string message)
    {
        return new ServiceResult { IsSuccess = false, Message = message };
    }
}
