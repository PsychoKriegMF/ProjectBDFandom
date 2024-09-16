using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace ProjectBDFandom
{
    internal class FSWork
    {

        private static void ExecuteSqlFromFile(string filePath, SQLiteConnection connection)
        {
            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"SQL file '{filePath}' not found.");
                return;
            }

            // Считываем SQL-запросы из файла
            string sqlFileContent = File.ReadAllText(filePath);

            // Регулярное выражение для поиска SQL-запросов
            string pattern = @"(?i)\b(?:create|alter|drop|insert|update|delete|select)\b.*?(?=;|$)";
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Находим все SQL-запросы в содержимом файла
            MatchCollection matches = regex.Matches(sqlFileContent);

            // Выполняем каждый SQL-запрос
            foreach (Match match in matches)
            {
                // Получаем текст запроса и удаляем лишние пробелы
                string sqlQuery = match.Value.Trim();

                // Проверяем, что запрос не пустой
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var command = new SQLiteCommand(sqlQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Метод для инициализации базы данных
        public static void InitializeDatabase(string filePath)
        {
            string connectionString = $"Data Source={dbname};Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выполняем запросы из файла
                ExecuteSqlFromFile(filePath, connection);

                Console.WriteLine("Database initialized successfully.");
            }
        }

        private static string dbname = "Fandom.db";  // Путь к БД
    
    static public string Path(string location = "myDocs")
        {
            switch (location)
            {
                case "myDocs": 
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "Desctop":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "Current":
                    return Environment.CurrentDirectory;
                default:
                    return string.Empty; break;
            }
        }
       


        static public bool IsFileExist(string path)
        {
            bool result = false;
            if (File.Exists(path))
            {
                result = true;
            }
            return result;
        }
        static public byte[] GetImage()
        {
            byte[] result = null;
            string filename = string.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPG files (*.JPG)|*.jpg|All files(*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            else return result;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                result = new byte[fs.Length];
                fs.Read(result, 0, result.Length);
            }
            return result;
        }
    }
}
