namespace Task_1
{
    /// <summary>
    /// Отвечает за запись отчета.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Запись при успешном выполнении генерации отчета
        /// </summary>
        /// <param name="data">Данные для записи.</param>
        /// <param name="id">Id отчета.</param>
        void ReportSuccess(byte[] data, int id);

        /// <summary>
        /// Запись при ошибке во время выполнении генерации отчета
        /// </summary>
        /// <param name="id">Id отчета.</param>
        void ReportError(int id);

        /// <summary>
        /// Запись при истекшем времени выполнении генерации отчета
        /// </summary>
        /// <param name="id">Id отчета.</param>
        void ReportTimeout(int id);
    }
}