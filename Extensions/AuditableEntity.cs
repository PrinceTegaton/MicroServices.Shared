using System;

namespace MicroServices.Shared.Extensions
{
    public class AuditableEntity
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public long? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}