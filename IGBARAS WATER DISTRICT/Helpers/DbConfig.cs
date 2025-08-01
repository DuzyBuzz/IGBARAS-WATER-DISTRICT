using System;
using System.IO;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class DbConfig
    {
        public static string DbPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "Datafile.mdb"); 

        public static string ConnectionString =>
            $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={DbPath};Persist Security Info=False;";

    }


}
