// See https://aka.ms/new-console-template for more information


using System;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=;Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
        NpgsqlCommand product = new NpgsqlCommand("SELECT * FROM product", conn);
        NpgsqlCommand customer = new NpgsqlCommand("SELECT * FROM customer", conn);

        NpgsqlDataReader reader = product.ExecuteReader();
        DataTable product_table = new DataTable();
        product_table.Load(reader);

        NpgsqlDataReader read = customer.ExecuteReader();
        DataTable customer_table = new DataTable();
        customer_table.Load(read);

        prob_7(product_table);

        prob_20(customer_table);
        conn.Close();
    }

    static void prob_7(DataTable data)
    {
        Console.WriteLine("Problem 7");
        Console.WriteLine("-------------");

        string filterCondition = "prod_quantity > 12 AND prod_quantity < 30";

        DataRow[] filteredRows = data.Select(filterCondition);

        DataTable filteredDataTable = data.Clone();

        foreach (DataRow row in filteredRows)
        {
            filteredDataTable.ImportRow(row);
        }
        print_results(filteredDataTable);
    }

    static void prob_20(DataTable data)
    {
        Dictionary<int, int> rep_balances = new Dictionary<int, int>();

        Console.WriteLine("\n\nProblem 20");
        Console.WriteLine("-------------\n");

        var results = data.AsEnumerable().GroupBy(r =>
        {
            int rep_id;
            int.TryParse(r.Field<string>("rep_id"), out rep_id);
            return rep_id;
        })
        .Select(g => new
        {
            rep_id = g.Key,
            Total_balance = g.Sum(r => r.Field<decimal>("cust_balance"))
        });

        var filteredResults = results.Where(r => r.Total_balance > 12000 && r.rep_id > 0)
            .OrderBy(r => r.rep_id);

        foreach(var result in filteredResults)
        {
            Console.WriteLine("Rep ID: {0}, Total Balance: {1}", result.rep_id, result.Total_balance);
        }

    }
    static void print_results(DataTable data)
    {
        Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        foreach (DataColumn col in data.Columns)
        {
            Console.Write(col.ColumnName);
            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col.ColumnName, maxLabelSize);
            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
        }

        Console.WriteLine();

        foreach (DataRow dataRow in data.Rows)
        {
            for (int j = 0; j < dataRow.ItemArray.Length; j++)
            {
                Console.Write(dataRow.ItemArray[j]);
                for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
            }
            Console.WriteLine();
            }
        }
    }