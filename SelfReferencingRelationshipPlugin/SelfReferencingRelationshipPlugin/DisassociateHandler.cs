using Microsoft.Xrm.Sdk;
using SelfReferencingRelationshipPlugin.Internal;
using System.Linq;

namespace SelfReferencingRelationshipPlugin
{
    public class DisassociateHandler : PluginBase
    {
        public DisassociateHandler(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
        }

        protected override bool IsContextValid(IPluginExecutionContext context)
        {
            if (context.MessageName == MessageName.Disassociate)
            {
                var relationship = GetRelationship(context);
                if (HandleRelationship(relationship?.SchemaName))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Execute(IPluginExecutionContext context, IOrganizationService orgService)
        {
            var target = GetTargetEntityReference(context);
            var relatedEntityId = GetFirstRelatedEntityId(context);
            var relationship = GetRelationship(context);

            if (target?.Id != null && relatedEntityId != null && relationship != null)
            {
                var query = QueryExpressionBuilder.GetSelfRelationsQuery(new GetSelfRelationsQueryParameter()
                {
                    RelationName = relationship.SchemaName,
                    EntityName = target.LogicalName,
                    RecordOneId = target.Id,
                    RecordTwoId = relatedEntityId.Value
                });

                var queryResults = orgService.RetrieveMultiple(query).Entities.ToList();
                if (queryResults.Count != 0)
                {
                    var relatedEntities = new EntityReferenceCollection
                            {
                                new EntityReference(target.LogicalName, target.Id)
                            };

                    var mirroredRelationship = new Relationship(relationship.SchemaName);
                    mirroredRelationship.PrimaryEntityRole = EntityRole.Referencing;

                    orgService.Disassociate(target.LogicalName, relatedEntityId.Value, mirroredRelationship, relatedEntities);
                }
            }

        }

    }
}
