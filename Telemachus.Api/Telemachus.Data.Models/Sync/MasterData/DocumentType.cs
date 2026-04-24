using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class DocumentType : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DisplayIndex { get; set; }
    }
}
