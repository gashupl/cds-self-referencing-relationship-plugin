using Microsoft.Xrm.Sdk.Query;

namespace SelfReferencingRelationshipPlugin.Internal
{
    class QueryExpressionBuilder
    {
        const string relationOneName = "one";
        const string relationTwoName = "two";

        public static QueryExpression GetSelfRelationsQuery(GetSelfRelationsQueryParameter param)
        {
            var condition1 = GetEqualCondition(GetRelationAttributeName(param.EntityName, relationOneName), param.RecordOneId); 
            var condition2 = GetEqualCondition(GetRelationAttributeName(param.EntityName, relationTwoName), param.RecordTwoId);

            var filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(condition1);
            filter.Conditions.Add(condition2);

            return new QueryExpression()
            {
                EntityName = param.RelationName,
                Criteria = filter
            };
        }

        private static string GetRelationAttributeName(string entityName, string relationNumberName)
        {
            return $"{entityName}id{relationNumberName}";
        }

        private static ConditionExpression GetEqualCondition(string name, object value)
        {
            var condition = new ConditionExpression
            {
                AttributeName = name,
                Operator = ConditionOperator.Equal
            };
            condition.Values.Add(value);
            return condition; 
        }
    }
}
