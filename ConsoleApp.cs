namespace ConsoleApp
{
    using System;
    using System.Data;
    using System.Configuration;
    using Newtonsoft.Json;
    using Microsoft.AnalysisServices.AdomdClient;

    internal class GetDataAzure
    {
        static string azure_path = ConfigurationManager.AppSettings["azure_link"];
        static string azure_database = ConfigurationManager.AppSettings["azure_database"];
        static string azure_login = ConfigurationManager.AppSettings["azure_login"];
        static string azure_password = ConfigurationManager.AppSettings["azure_password"];

        static void Main(string[] args)
        {
            OlapDaxDataset();
        }

        // Execute script DAX
        public static void OlapDaxDataset()
        {
            // To set any script of DAX
            var query = $@"EVALUATE 'Calendar'";
            var dataset = ExecuteDaxQuery(query);

            // Output columns into JSON
            var columns = JsonConvert.SerializeObject(dataset.Columns);
            Console.WriteLine(columns);

            // Output rows into JSON
            foreach (DataRow row in dataset.Rows)
            {
                var rows = JsonConvert.SerializeObject(row.ItemArray);
                Console.WriteLine(rows);
            }
        }

        // Connect to azure analysis services
        public static DataTable ExecuteDaxQuery(string query)
        {
            var connection_string = $@"Provider=MSOLAP.8;Data Source={azure_path};Initial Catalog={azure_database};User ID={azure_login};Password={azure_password};" +
                "Persist Security Info=True;Impersonation Level=Impersonate";

            var tabular_results = new DataTable();

            using (var connection = new AdomdConnection(connection_string))
            {
                var current_data_adapter = new AdomdDataAdapter(query, connection);
                current_data_adapter.Fill(tabular_results);
            }
            return tabular_results;
        }

    }

}
