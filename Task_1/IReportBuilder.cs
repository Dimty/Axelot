namespace Task_1
{
    /// <summary>
    /// Отвечает за построение отчета.
    /// </summary>
    public interface IReportBuilder
    {
        /// <summary>
        /// Создает отчеты.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        char[] Build(int id);

        /// <summary>
        /// Отменяет построение отчета.
        /// </summary>
        /// <param name="id"></param>
        void Stop(int id);
    }
}