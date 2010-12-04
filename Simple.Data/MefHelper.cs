﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Data
{
    public interface IMefHelper
    {
        T Compose<T>(string contractName);
    }

    class MefHelper : IMefHelper
    {
        public T Compose<T>(string contractName)
        {
            using (var container = CreateContainer())
            {
                var export = container.GetExport<T>(contractName);
                if (export == null) throw new ArgumentException("Unrecognised file.");
                return export.Value;
            }
        }

        private CompositionContainer CreateContainer()
        {
            var path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            path = Path.GetDirectoryName(path);
            if (path == null) throw new ArgumentException("Unrecognised file.");

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var folderCatalog = new DirectoryCatalog(path, "Simple.Data.*.dll");
            return new CompositionContainer(new AggregateCatalog(assemblyCatalog, folderCatalog));
        }
    }
}
