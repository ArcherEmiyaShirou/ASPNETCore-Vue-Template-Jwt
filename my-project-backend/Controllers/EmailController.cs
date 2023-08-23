using Backend.Common.Utills;
using Backend.Contract.Entity;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace my_project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSerivce emailService;

        public EmailController(IEmailSerivce emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailTestAsync(string email)
        {
            var data = new Dictionary<string, string>
            {
                {"type",Const.EMAIL_TYPE_REGISTRATION },
                {"code","114514" }
            };
            await emailService.SendEmailAsync(data);

            return Ok(CustomResponse<object>.Success());
        }
    }
}
