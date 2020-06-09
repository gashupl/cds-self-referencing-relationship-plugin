using System;

namespace SelfReferencingRelationshipPlugin.Internal
{
    public class GetSelfRelationsQueryParameter
    {
        public string RelationName { get; set; }
        public string EntityName { get; set; }
        public Guid RecordOneId { get; set; }
        public Guid RecordTwoId { get; set; }

    }
}
