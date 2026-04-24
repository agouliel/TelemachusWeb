using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces;
using Telemachus.Business.Models.Events;
using Telemachus.Data.Models.Authentication;
using Telemachus.Documents;
using Telemachus.Helpers;
using static DocumentCreator.DocumentOperator;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class StatementController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IStatementService _statementService;

        public StatementController(IStatementService statementService, IWebHostEnvironment env, UserManager<User> userManager)
        {
            _statementService = statementService;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("document/{id:int}/download/text")]
        public async Task<IActionResult> GetText(int id, [FromQuery(Name = "exclude[]")] List<string> excludedFields)
        {
            var userId = ClaimHelper.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var document = await _statementService.GetDocumentFactsViewModel(userId, id);
            document.Facts = document.Facts.Where(_ => _.Excluded == false).ToList();
            document.VesselName = "M/T " + user.UserName.ToUpper();
            document.Operator = user.Operator != "GRACE" ? Operators.Ionia : Operators.Grace;
            var result = new StringBuilder();
            result.AppendLine($"Vessel: {document.VesselName}");
            if (!string.IsNullOrEmpty(document.FormattedDate))
            {
                result.AppendLine($"Date: {document.FormattedDate}");
            }
            result.AppendLine(string.Empty);
            foreach (var e in document.Facts)
            {
                var text = $"{e.Timestamp.Value.ToString("dd/MM/yy")} {e.Timestamp.Value.ToString("HH:mm")}: {(e.EventTypeId != 16 ? e.Name : e.CustomEventName).ToUpper()}";
                if (!string.IsNullOrEmpty(e.Remarks))
                {
                    text += $", Remarks: {e.Remarks}";
                }
                result.AppendLine(text);
            }
            result.AppendLine(string.Empty);
            result.AppendLine("Remarks (delays, stoppages etc.): ");
            result.AppendLine(document.Remarks);
            return Ok(result.ToString());
        }
        [HttpGet("document/{id:int}/download")]
        public async Task<IActionResult> DownloadDocument(int id, [FromQuery(Name = "exclude[]")] List<string> excludedFields)
        {
            var userId = ClaimHelper.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var document = await _statementService.GetDocumentFactsViewModel(userId, id);
            foreach (var field in excludedFields)
            {
                if (field == "date")
                {
                    document.Date = null;
                }
                if (field == "operationGrade")
                {
                    document.OperationGrade = null;
                }
                if (field == "voyage")
                {
                    document.Voyage = null;
                }
                if (field == "portId")
                {
                    document.PortId = null;
                }
                if (field == "terminal")
                {
                    document.Terminal = null;
                }
                if (field == "remarks")
                {
                    document.Remarks = null;
                }
            }
            document.Facts = document.Facts.Where(_ => _.Excluded == false).ToList();
            document.VesselName = "M/T " + user.UserName.ToUpper();
            document.Operator = user.Operator != "GRACE" ? Operators.Ionia : Operators.Grace;
            var contentPath = Path.Combine(_env.ContentRootPath, "assets", "images");
            var wordDocument = new EventDocument(contentPath, document);
            wordDocument.Close();
            var bytes = wordDocument.ToArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordDocument.FileName);
        }
        [HttpGet("document/{id:int?}")]
        public async Task<IActionResult> GetDocument(int? id, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var userId = ClaimHelper.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var document = await _statementService.GetDocumentFactsViewModel(userId, id, dateFrom, dateTo);
            document.VesselName = "M/T " + user.UserName.ToUpper();
            document.Operator = user.Operator != "GRACE" ? Operators.Ionia : Operators.Grace;
            return Ok(document);
        }
        [HttpGet()]
        public async Task<IActionResult> GetStatements()
        {
            var userId = ClaimHelper.GetUserId(User);
            var statements = await _statementService.GetStatements(userId);
            return Ok(statements);
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateStatement(int id, [FromBody] DocumentViewModelDTO document)
        {
            var userId = ClaimHelper.GetUserId(User);
            var statement = await _statementService.UpdateStatement(userId, document, id);
            return await GetDocument(statement.Id, statement.FromDate, statement.ToDate);
        }
        [HttpPatch("{id:int}/{complete:int?}")]
        public async Task<IActionResult> PatchStatement(int id, int? complete = 1)
        {
            var userId = ClaimHelper.GetUserId(User);
            await _statementService.PatchStatement(userId, id, complete == 0 ? false : true);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStatement(int id)
        {
            var userId = ClaimHelper.GetUserId(User);
            await _statementService.DeleteStatement(userId, id);
            return NoContent();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateStatement([FromBody] DocumentViewModelDTO document)
        {
            var userId = ClaimHelper.GetUserId(User);
            var statement = await _statementService.CreateStatement(userId, document);
            return await GetDocument(statement.Id, statement.FromDate, statement.ToDate);
        }

    }
}
