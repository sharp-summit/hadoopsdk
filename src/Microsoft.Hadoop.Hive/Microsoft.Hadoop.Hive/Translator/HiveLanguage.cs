// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQToolkit.Data.Common;
using IQToolkit.Data;
using System.Reflection;
using System.Linq.Expressions;
using IQToolkit;

namespace Microsoft.Hadoop.Hive
{
    class HiveLanguage : QueryLanguage
    {
        DbTypeSystem typeSystem = new DbTypeSystem();

        public HiveLanguage()
        {
        }

        public override QueryTypeSystem TypeSystem
        {
            get { return this.typeSystem; }
        }

        public override string Quote(string name)
        {
            return name;
        }

        private static readonly char[] splitChars = new char[] { '.' };

        public override bool AllowsMultipleCommands
        {
            get { return false; }
        }

        public override bool AllowSubqueryInSelectWithoutFrom
        {
            get { return true; }
        }

        public override bool AllowDistinctInAggregates
        {
            get { return true; }
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            throw new NotSupportedException();
        }

        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new HiveLinguist(this, translator);
        }

        class HiveLinguist : QueryLinguist
        {
            public HiveLinguist(HiveLanguage language, QueryTranslator translator)
                : base(language, translator)
            {
            }

            public override Expression Translate(Expression expression)
            {
                // fix up any order-by's
                expression = OrderByRewriter.Rewrite(this.Language, expression);

                expression = base.Translate(expression);

                expression = RenameTopClauseColumns(expression);

                // convert skip/take info into RowNumber pattern
                expression = SkipToRowNumberRewriter.Rewrite(this.Language, expression);

                // fix up any order-by's we may have changed
                expression = OrderByRewriter.Rewrite(this.Language, expression);

                expression = RedundantColumnRemover.Remove(expression);

                expression = UnusedColumnRemover.Remove(expression);

                return expression;
            }

            public override string Format(Expression expression)
            {
                return HiveFormatter.Format(expression, this.Language);
            }

            public override Expression Parameterize(Expression expression)
            {
                // TODO - need to figure out if this needs to be parameterized
                return expression;
            }

            public override IEnumerable<string> ExtractFileReferences(Expression expression)
            {
                return AddFileGatherer.Gather(expression);
            }

            public Expression RenameTopClauseColumns(Expression expression)
            {
                var select = ((ProjectionExpression)expression).Select;

                var outputType = TypeHelper.GetElementType(expression.Type);
                MappingEntity entity = this.Translator.Mapper.Mapping.GetEntity(outputType);
                var newProjector = this.Translator.Mapper.GetEntityExpression(select, entity);
                ProjectedColumns pc = ColumnProjector.ProjectColumns(this.Language, newProjector, null, select.Alias, select.Alias);
                
                var newColumns = new List<ColumnDeclaration>();
                for (int i =0; i < pc.Columns.Count; i++)
                {
                    var col = select.Columns.FirstOrDefault(item => item.Name.Contains(pc.Columns[i].Name));
                    if (col == null)
                    {
                        col = select.Columns[i];
                    }
                    newColumns.Add(new ColumnDeclaration(pc.Columns[i].Name, col.Expression, select.Columns[i].QueryType));
                }

                return new ProjectionExpression(
                    new SelectExpression(select.Alias, newColumns, select.From, select.Where, select.OrderBy, select.GroupBy,
                                         select.ClusterBy, select.IsDistinct, select.Map, select.Skip, select.Take, select.IsReverse),
                    newProjector
                    );
            }
        }

    }

}
