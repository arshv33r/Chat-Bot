using System.Linq;
using DobicChatBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DobicChatBot.Controllers
{
    [ApiController]
    [Route("api")]
    public class ChatbotController : ControllerBase
    {
        private readonly ExcelService _excel;

        public ChatbotController(ExcelService excel)
        {
            _excel = excel;
        }

        [HttpGet("apps")]
        public IActionResult GetApps()
        {
            var apps = _excel.GetApplications()
                             .Select(e => new { e.IssueID, e.Label });
            return Ok(apps);
        }

        [HttpGet("issues")]
        public IActionResult GetIssues([FromQuery] string appId)
        {
            var issues = _excel.GetIssuesForApp(appId)
                               .Select(e => new { e.IssueID, e.Label });
            return Ok(issues);
        }

        [HttpGet("solution")]
        public IActionResult GetSolution([FromQuery] string issueId)
        {
            var solution = _excel.GetSolution(issueId);
            return Ok(new { Solution = solution });
        }
    }
}