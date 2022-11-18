using System.Text;

namespace Task_1
{
    /// <summary>
    /// Записывает отчеты в файл.
    /// </summary>
    public class Reporter : IReporter
    {
        /// <summary>
        /// Создает отчет с успешным завершением.
        /// </summary>
        /// <param name="data">Данные для записи.</param>
        /// <param name="id">Id отчета.</param>
        public void ReportSuccess(byte[] data, int id)
        {
            WriteToFile("Report_", data, id);
        }

        /// <summary>
        /// Создает отчет с ошибкой во время генерации.
        /// </summary>
        /// <param name="id">Id отчета.</param>
        public void ReportError(int id)
        {
            var data = Encoding.UTF8.GetBytes("Report error");
            WriteToFile("Error_", data, id);
        }

        /// <summary>
        /// Создает отчет с просроченным временем для генерации отчета.
        /// </summary>
        /// <param name="id">Id отчета.</param>
        public void ReportTimeout(int id)
        {
            var data = Encoding.UTF8.GetBytes("Report error");
            WriteToFile("Timeout_", data, id);
        }

        /// <summary>
        /// Создает файлы и записывает туда результат генерации отчета.
        /// </summary>
        /// <param name="to">Результат генерации.</param>
        /// <param name="data">Данные для записи.</param>
        /// <param name="id">Id отчета.</param>
        private static void WriteToFile(string to, byte[] data, int id)
        {
            using var fs = new FileStream(to + id + ".txt", FileMode.OpenOrCreate);
            using (var sw = new StreamWriter(fs))
            {
                var str = Encoding.UTF8.GetString(data, 0, data.Length);
                sw.WriteLine(str);
            }
        }
    }
}