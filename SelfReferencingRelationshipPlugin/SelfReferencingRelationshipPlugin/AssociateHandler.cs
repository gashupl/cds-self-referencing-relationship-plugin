using Microsoft.Xrm.Sdk;
using SelfReferencingRelationshipPlugin.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfReferencingRelationshipPlugin
{
    public class AssociateHandler : PluginBase, IPlugin
    {
        public AssociateHandler(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
        }

        protected override bool IsContextValid(IPluginExecutionContext context)
        {
            if (context.MessageName == MessageName.Associate)
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
                    RecordOneId = relatedEntityId.Value,
                    RecordTwoId = target.Id
                });

                var queryResults = orgService.RetrieveMultiple(query).Entities.ToList();
                if (queryResults.Count == 0)
                {
                    var relatedEntities = new EntityReferenceCollection
                            {
                                new EntityReference(target.LogicalName, target.Id)
                            };

                    var mirroredRelationship = new Relationship(relationship.SchemaName);
                    mirroredRelationship.PrimaryEntityRole = EntityRole.Referencing;

                    orgService.Associate(target.LogicalName, relatedEntityId.Value,  mirroredRelationship, relatedEntities);
                }
            }
        }

    }
}
