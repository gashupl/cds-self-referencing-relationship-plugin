using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfReferencingRelationshipPlugin.Internal
{
    public abstract class PluginBase : IPlugin
    {
        protected readonly List<string> _relationships;
        public PluginBase(string unsecureConfig, string secureConfig)
        {
            _relationships = JsonHelper.Deserialize<List<string>>(unsecureConfig);
        }

        protected bool HandleRelationship(string relationship)
        {
            if (_relationships.Contains(relationship))
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

        protected Relationship GetRelationship(IPluginExecutionContext context)
        {
            return context != null &&
                   context.InputParameters != null &&
                   context.InputParameters.Contains(ParameterName.Relationship)
                ? context.InputParameters[ParameterName.Relationship] as Relationship
                : null;
        }

        public EntityReference GetTargetEntityReference(IPluginExecutionContext context)
        {
            return context != null && context.InputParameters != null &&
                   context.InputParameters.Contains(ParameterName.Target)
                ? context.InputParameters[ParameterName.Target] as EntityReference
                : null;
        }

        public Guid? GetFirstRelatedEntityId(IPluginExecutionContext context)
        {
            if (context.InputParameters.Contains(ParameterName.RelatedEntities)
                && context.InputParameters[ParameterName.RelatedEntities] is EntityReferenceCollection)
            {
                if (context.InputParameters[ParameterName.RelatedEntities] is EntityReferenceCollection relatedEntities
                    && relatedEntities.Count > 0 && relatedEntities[0] != null)
                {
                    return relatedEntities[0].Id;
                }
            }

            return null;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = serviceFactory.CreateOrganizationService(context.UserId);

            if (context == null)
            {
                throw new ApplicationException("Failed to initialize plugin execution context");
            }

            if (IsContextValid(context))
            {
                Execute(context, orgService); 
            }
            
        }

        protected abstract bool IsContextValid(IPluginExecutionContext context);

        protected abstract void Execute(IPluginExecutionContext context, IOrganizationService orgService);
    }
}
