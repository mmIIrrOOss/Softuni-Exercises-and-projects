﻿namespace _04.AddMinion
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Transactions;
    using Models;

    class StartUp
    {
        private const string DBName = "master";
        private const string ServerName = ".";
        private const string Authentication = "Integrated Security=true";
        private const string DefaultCountryName = "Undefined";

        private const string TownInsertionQueryPath = @"..\..\SQL Queries\TownInsertion.sql";
        private const string CountryInsertionQueryPath = @"..\..\SQL Queries\CountryInsertion.sql";
        private const string MinionInsertionQueryPath = @"..\..\SQL Queries\MinionInsertion.sql";
        private const string VillainInsertionQueryPath = @"..\..\SQL Queries\Villain-Insertion.sql";
        private const string MinionVillainInsertionQueryPath = @"..\..\SQL Queries\Minion-Vilion-Insertion.sql";

        static void Main(string[] args)
        {
            string connectionString = $@"
                Server={ServerName}; 
                Database={DBName}; 
                {Authentication};";

            while (true)
            {
                Console.WriteLine("Enter Minion and Villain:");

                Minion minion = GetMinionFromConsole();
                Villain villain = GeVillainFromConsole();

                using (TransactionScope transaction = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        try
                        {
                            AssureTownExists(minion.Town, connection);
                            AssureMinionExists(minion, connection);
                            AssureVillainExists(villain, connection);
                            AddMinionToVillainServants(minion.Name, villain.Name, connection);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    transaction.Complete();
                }

                AskToContinue();
            }
        }
        private static void AskToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("To continue enter - Go");
            Console.WriteLine("To stop enter - Stop");
            string input = Console.ReadLine();

            if (input.Equals("Go", StringComparison.OrdinalIgnoreCase))
            {
                Console.Clear();
                return;
            }
            else if (input.Equals("Stop", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(Environment.ExitCode);
            }

            Console.WriteLine("Wrong command! Try again.");
            AskToContinue();
        }

        private static void AddMinionToVillainServants(string minionName, string villainName, SqlConnection connection)
        {
            string cmdText = File.ReadAllText(MinionVillainInsertionQueryPath);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@minionName", minionName);
                command.Parameters.AddWithValue("@villainName", villainName);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}");
        }

        private static void AssureVillainExists(Villain villain, SqlConnection connection)
        {
            if (IsRecordAvailable(villain.Name, "Name", "Villains", connection))
            {
                return;
            }

            string cmdText = File.ReadAllText(VillainInsertionQueryPath);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@name", villain.Name);
                command.Parameters.AddWithValue("@evilnessFactor", villain.EvillnessFactor);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Villain {villain.Name} was added to the database.");
        }

        private static void AssureMinionExists(Minion minion, SqlConnection connection)
        {
            if (IsRecordAvailable(minion.Name, "Name", "Minions", connection))
            {
                return;
            }

            string cmdText = File.ReadAllText(MinionInsertionQueryPath);
            SqlCommand command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@townName", minion.Town);
            command.Parameters.AddWithValue("@name", minion.Name);
            command.Parameters.AddWithValue("@age", minion.Age);
            command.ExecuteNonQuery();

            Console.WriteLine($"Minion {minion.Name} was added to the database.");
        }

        private static void AssureTownExists(string town, SqlConnection connection)
        {
            if (IsRecordAvailable(town, "Name", "Towns", connection))
            {
                return;
            }

            AssureCountryIsAvailable(DefaultCountryName, connection);

            string cmdText = File.ReadAllText(TownInsertionQueryPath);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@countryName", DefaultCountryName);
                command.Parameters.AddWithValue("@townName", town);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Town {town} was added to the database.");
        }

        private static void AssureCountryIsAvailable(string coutryName, SqlConnection connection)
        {
            if (IsRecordAvailable(DefaultCountryName, "Name", "Countries", connection))
            {
                return;
            }

            string cmdText = File.ReadAllText(CountryInsertionQueryPath);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@countryName", coutryName);
                command.ExecuteNonQuery();
            }
        }

        private static bool IsRecordAvailable(string keyValue, string colName, string tableName, SqlConnection connection)
        {
            object selection = null;
            string cmdText = $"SELECT {colName} FROM {tableName} WHERE {colName} = '{keyValue}';";
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@keyValue", keyValue);
                selection = command.ExecuteScalar();
            }

            return selection != DBNull.Value && selection != null;
        }

        private static Villain GeVillainFromConsole()
        {
            string[] villainData = Console.ReadLine()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .ToArray();

            string name = villainData[0];
            if (villainData.Length == 1)
            {
                return new Villain(name);
            }

            string evilnessFactor = villainData[1];
            return new Villain(name, evilnessFactor);
        }

        private static Minion GetMinionFromConsole()
        {
            string[] minionData = Console.ReadLine()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .ToArray();

            string name = minionData[0];
            int age = int.Parse(minionData[1]);
            string town = minionData[2];

            return new Minion(name, age, town);
        }
    }
}
