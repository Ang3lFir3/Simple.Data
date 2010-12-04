﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Simple.Data.Ado
{
    class AdoAdapterManyFinder
    {
        private readonly AdoAdapter _adapter;
        private readonly DbTransaction _transaction;
        private readonly DbConnection _connection;

        public AdoAdapterManyFinder(AdoAdapter adapter) : this(adapter, null)
        {
        }

        public AdoAdapterManyFinder(AdoAdapter adapter, DbTransaction transaction)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            _adapter = adapter;

            if (transaction != null)
            {
                _transaction = transaction;
                _connection = transaction.Connection;
            }
        }

        public IEnumerable<IDictionary<string, object>> FindMany(string tableName, SimpleExpression criteria)
        {
            if (criteria == null) return FindAll(TableName.Parse(tableName));

            var commandBuilder = new FindHelper(_adapter.GetSchema()).GetFindByCommand(TableName.Parse(tableName), criteria);
            return ExecuteQuery(commandBuilder);
        }

        private IEnumerable<IDictionary<string, object>> FindAll(TableName tableName)
        {
            return ExecuteQuery("select * from " + _adapter.GetSchema().FindTable(tableName).ActualName);
        }

        private IEnumerable<IDictionary<string, object>> ExecuteQuery(ICommandBuilder commandBuilder)
        {
            var connection = _connection ?? _adapter.CreateConnection();
            var command = commandBuilder.GetCommand(connection);
            command.Transaction = _transaction;
            return TryExecuteQuery(command);
        }

        private IEnumerable<IDictionary<string, object>> ExecuteQuery(string sql, params object[] values)
        {
            var connection = _connection ?? _adapter.CreateConnection();
            var command = CommandHelper.Create(connection, sql, values);
            command.Transaction = _transaction;
            return TryExecuteQuery(command);
        }

        private static IEnumerable<IDictionary<string, object>> TryExecuteQuery(IDbCommand command)
        {
            try
            {
                return command.ToAsyncEnumerable();
            }
            catch (DbException ex)
            {
                throw new AdoAdapterException(ex.Message, command);
            }
        }
    }
}
