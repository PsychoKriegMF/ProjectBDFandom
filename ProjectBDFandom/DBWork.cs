using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ProjectBDFandom
{
    internal class DBWork
    {
        static private string dbname = "Fandom.db";  // Путь к БД
        static private string path = $"Data Source={dbname};";
        static private List<string> queryes = new List<string>(); // Список для хранения SQL-запросов

        // Метод для заполнения списка запросов из файла SQL
        static private void FillQueryes(string filename = @"SQL\Create.sql")
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"SQL file '{filename}' not found.");
                return;
            }

            // Считываем SQL-запросы из файла
            string sqlFileContent = File.ReadAllText(filename);

            // Регулярное выражение для поиска SQL-запросов
            string pattern = @"(?i)\b(?:create|alter|drop|insert|update|delete|select)\b.*?(?=;|$)";
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Находим все SQL-запросы в содержимом файла и добавляем их в список
            MatchCollection matches = regex.Matches(sqlFileContent);
            foreach (Match match in matches)
            {
                string sqlQuery = match.Value.Trim();
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    queryes.Add(sqlQuery);
                }
            }

            Console.WriteLine("SQL queries loaded successfully.");
        }

        // Метод для выполнения всех SQL-запросов из списка
        static private void ExecuteQueries()
        {
            using (var connection = new SQLiteConnection(path))
            {
                connection.Open();

                foreach (var query in queryes)
                {
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("All SQL queries executed successfully.");
            }
        }

        // Метод для инициализации базы данных
        static public void InitializeDatabase(string filePath = @"SQL\Create.sql")
        {
            // Заполняем список запросов из файла
            FillQueryes(filePath);

            // Выполняем запросы в базе данных
            ExecuteQueries();
        }
        static public bool MakeDB()
        {
            try
            {
                // Заполняем список запросов из файла
                FillQueryes();

                // Выполняем запросы в базе данных
                ExecuteQueries();

                return true; // Возвращаем true при успешном выполнении
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Возвращаем false при возникновении ошибки
            }
        }
    }
}
