using System.Text;
using Exception = System.Exception;
using Random = System.Random;

namespace Task_1
{
    /// <summary>
    /// Отвечает за "построение" отчетов.
    /// </summary>
    public class ReportBuilder : IReportBuilder
    {
        /// <summary>
        /// Список всех билдеров, содержащие активные задачи.
        /// </summary>
        ///
        //TODO: заменить список билдеров на что-нибудь вменяемое.
        private static List<ReportBuilder> _listRepBuilder = new();

        /// <summary>
        /// Для синхронизации записи и удаления.
        /// </summary>
        private static object objSync = new object();

        /// <summary>
        /// Контекст репортера.
        /// </summary>
        private readonly IReporter _reporter;

        /// <summary>
        /// Id выполняемой задачи.
        /// </summary>
        private int id;

        /// <summary>
        /// Токен отмены.
        /// </summary>
        public CancellationTokenSource _cts = new CancellationTokenSource();

        public ReportBuilder(IReporter reporter)
        {
            _reporter = reporter;

            lock (objSync)
            {
                _listRepBuilder.Add(this);
            }
        }

        /// <summary>
        /// Запускает таск генерации отчета.
        /// </summary>
        /// <param name="id">Id отчета\таска генерирующий этот отчет</param>
        /// <returns></returns>
        public char[] Build(int id)
        {
            this.id = id;
            char[] chars = new char[0];

            lock (objSync)
            {
                GenerateReport(id).Start();
            }

            return chars;
        }

        /// <summary>
        /// Отменяет генерацию отчета.
        /// </summary>
        /// <param name="id">Id отчета\таска генерирующий этот отчет</param>
        public void Stop(int id)
        {
            lock (objSync)
            {
                foreach (var item in _listRepBuilder)
                {
                    if(item.id == id) item._cts.Cancel();
                }
            }
        }

        /// <summary>
        /// Имитирует построение отчета.
        /// </summary>
        /// <param name="id">Id отчета\таска генерирующий этот отчет</param>
        /// <returns></returns>
        //TODO: переделать метод "GenerateReport".
        private Task GenerateReport(int id)
        {
            Task t = new Task(() =>
            {
                try
                {
                    WaitingCycle(id);
                }
                catch (Exception e)
                {
                    _reporter.ReportError(id);
                }
                finally
                {
                    lock (objSync)
                    {
                        _listRepBuilder.RemoveAt(this.id);
                    }
                }
            });
            return t;
        }

        /// <summary>
        /// Имитирует время потраченное на генерацию отчета 
        /// </summary>
        /// <param name="id">Id отчета\таска генерирующий этот отчет</param>
        /// <returns></returns>
        /// <exception cref="Exception">Report failed</exception>
        private void WaitingCycle(int id)
        {
            var awaitFrom = 5;
            var awaitTo = 46;
            var timeOut = new Random().Next(awaitFrom, awaitTo);
            var buildTime = new Random().Next(awaitFrom, awaitTo);
            var isError = new Random().Next() % 100 > 80 ? true : false;

            //TODO: переписать цикл ожидания

            for (int i = 0; i < timeOut; i++)
            {
                if (_cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Task " + id + " was cancelled.");
                    return;
                }

                if (isError && i == 2) throw new Exception("Report failed");

                if (buildTime == i)
                {
                    var result = Encoding.UTF8.GetBytes("Report built in " + i + " s.");
                    _reporter.ReportSuccess(result, id);
                    return;
                }

                Thread.Sleep(1000);
            }

            _reporter.ReportTimeout(id);
        }
    }
}