using MySolution.Application.Validation;
using Newtonsoft.Json;

namespace MySolution.Api.Handler;

public class ErrorHandler
{
    public bool success { get; set; }
    public string errorMessage { get; set; }
    public string errorMessageCode { get; set; }
    public List<ValidationError> errors { get; set; }
    public object data { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}