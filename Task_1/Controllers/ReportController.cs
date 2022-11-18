using Microsoft.AspNetCore.Mvc;

namespace Task_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private static int Id = 0;
        
        private readonly IReportBuilder _reportBuilder;
        public ReportController(IReportBuilder reportBuilder)
        {
            _reportBuilder = reportBuilder;
        }
        [HttpGet(Name = "Build")]
        public int Get()
        {
            var currId = Id++;
            _reportBuilder.Build(currId);
            return currId;
        }
        
        [HttpPost(Name = "Stop")]
        public void Post([FromBody] int value)
        {
            _reportBuilder.Stop(value);
        }

    }
}
