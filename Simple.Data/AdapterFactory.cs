﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data.Extensions;

namespace Simple.Data
{
    class AdapterFactory : IAdapterFactory
    {
        private readonly IMefHelper _mefHelper;

        internal AdapterFactory() : this(new MefHelper())
        {
            
        }

        internal AdapterFactory(IMefHelper mefHelper)
        {
            _mefHelper = mefHelper;
        }

        public Adapter Create(object settings)
        {
            return Create(ObjectEx.ObjectToDictionary(settings));
        }

        public Adapter Create(string adapterName, object settings)
        {
            return Create(adapterName, ObjectEx.ObjectToDictionary(settings));
        }

        public Adapter Create(IEnumerable<KeyValuePair<string,object>> settings)
        {
            if (settings.Any( kvp => kvp.Key.Equals("ConnectionString",StringComparison.OrdinalIgnoreCase)))
            {
                return Create("Ado", settings);
            }

            throw new ArgumentException("Cannot infer adapter type from settings.");
        }

        public virtual Adapter Create(string adapterName, IEnumerable<KeyValuePair<string, object>> settings)
        {
            return DoCreate(adapterName, settings);
        }

        protected Adapter DoCreate(string adapterName, IEnumerable<KeyValuePair<string, object>> settings)
        {
            var adapter = _mefHelper.Compose<Adapter>(adapterName);
            if (adapter != null)
            {
                adapter.Setup(settings);
            }
            return adapter;
        }
    }
}
