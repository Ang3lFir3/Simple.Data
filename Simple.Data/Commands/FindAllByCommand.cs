﻿using System;
using System.Dynamic;
using System.Linq;

namespace Simple.Data.Commands
{
    class FindAllByCommand : ICommand
    {
        public bool IsCommandFor(string method)
        {
            return method.StartsWith("FindAllBy") || method.StartsWith("find_all_by_", StringComparison.InvariantCultureIgnoreCase);
        }

        public object Execute(DataStrategy dataStrategy, DynamicTable table, InvokeMemberBinder binder, object[] args)
        {
            var criteria = ExpressionHelper.CriteriaDictionaryToExpression(table.GetQualifiedName(), MethodNameParser.ParseFromBinder(binder, args));
            return new SimpleQuery(dataStrategy, table.GetQualifiedName(), criteria);
        }
    }
}
