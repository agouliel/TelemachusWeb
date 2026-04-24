using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Models.Events;
using Telemachus.Data.Models;

namespace Telemachus.Business.Interfaces
{
    public interface IStatementService
    {
        Task<StatementOfFact> CreateStatement(string userId, DocumentViewModelDTO document);
        Task DeleteStatement(string userId, int id);
        public Task<DocumentViewModel> GetDocumentFactsViewModel(string userId, int? statementId, DateTime? from = null, DateTime? to = null);
        Task<List<StatementOfFact>> GetStatements(string userId);
        Task PatchStatement(string userId, int id, bool complete);
        Task<StatementOfFact> UpdateStatement(string userId, DocumentViewModelDTO document, int id);
    }
}
