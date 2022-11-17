using System;

namespace Task_2
{
    static class Program
    {
        /// <summary>
        /// Запись пользователя, которому будет отправляться сообщение
        /// </summary>
        public interface IUser
        {
            /// <summary>
            /// Уникальный идентификатор
            /// </summary>
            string Id { get; }

            /// <summary>
            /// Метод доставки сообщения, напаример: 
            /// 0 -не нужна доставка, 1 - Телеграмм, 2 - СМС, 3 - e-mail, 4 - WhatsApp и тд и тп
            /// </summary>
            int DeliveryMethod { get; }

            /// <summary>
            /// Адресс, по которму будут отправляться сообщания. Зависит от метода доставки:
            /// аккаунт Телеграмм, номер телефона, e-mail и тд
            /// </summary>
            string Address { get; }
        }


        /// <summary>
        /// Сообщение для доставки пользователю
        /// </summary>
        public interface IMessage
        {
            /// <summary>
            /// Идентификатор пользователя, которому надо доставить сообщение
            /// </summary>
            int UserId { get; }

            /// <summary>
            /// Текст сообщения
            /// </summary>
            string MessageText { get; }
        }

        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        public interface IUserRepository
        {
            IUser Get(int userId);
        }


        /// <summary>
        /// Интерфейс, который реализуют все воркеры, отправляющие сообщения
        /// Существуют реализации ISender для отправки по 
        /// Телеграмм, СМС, e-mail, WhatsApp и тд и тп
        /// </summary>
        public interface ISender
        {
            /// <summary>
            /// Отправляет сообщение по указанному адресу, возвращает True в случае успешной доставки
            /// </summary>
            /// <param name="message"> текст сообщения </param>
            /// <param name="address"> адрес доставки, в зависимости от реализации может содержать 
            /// имя аккаунта, телефон, e-mail и тд и тп</param>
            /// <returns>Результат доставки, True, если сообщение успешно доставлено</returns>
            bool Send(string message, string address);
        }

        public class Postman
        {
            /// <summary>
            /// Список, куда следует поместить все сообщения, которые не удалось доставить
            /// </summary>
            public IEnumerable<IMessage> FailedMessages;

            /// <summary>
            /// Используется для синхронизации записи сообщений в список <see cref="FailedMessages"/>.
            /// </summary>
            private object _locker = new object();

            /// <summary>
            /// Репозиторий пользователей
            /// </summary>
            private readonly IUserRepository _userRepository;

            /// <summary>
            /// Отправляет сообщения из списка messages пользователям
            /// Сообщение отправляется методом, указанным в записи пользователя
            /// по адресу, указанным в записи пользователя
            /// В случае, если сообщение не удалось доставить, помещает его в  FailedMessages
            /// </summary>
            /// <param name="messages"> коллекция сообщений </param>
            public void Send(IEnumerable<IMessage> messages)
            {
                if (_userRepository is null || FailedMessages is null)
                {
                    Console.WriteLine(
                        $"Instance userRepository or FailedMessages is null"); //Пусть консоль заменяет какой-нибудь логгер.
                    return;
                }

                Parallel.ForEach(messages,
                    new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    }, //Решил ограничиться кол-во доступных процессоров.
                    SingleMessageSend);
            }

            /// <summary>
            /// Метод для отправки сообщения конкретному пользователю.
            /// </summary>
            /// <param name="message">Содержит Id пользователя получателя и отправляемое сообщение.
            /// Параметр является экземпляром класса, реализующего интерфейс <see cref="IMessage"/>.</param>
            private void SingleMessageSend(IMessage message)
            {
                var sendResult = false;
                try
                {
                    var user = _userRepository.Get(message.UserId);
                    sendResult = SenderDetermination(user.DeliveryMethod)
                        .Send(message.MessageText, user.Address);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (sendResult is false)
                    {
                        lock (_locker)
                        {
                            FailedMessages = UpdateFailedMessages(message);
                        }
                    }
                }
            }

            /// <summary>
            /// Метод добавления сообщения в список неотправленных сообщений 
            /// </summary>
            /// <param name="message">Содержит Id пользователя получателя и отправляемое сообщение.
            /// Параметр является экземпляром класса, реализующий интерфейс <see cref="IMessage"/>.</param>
            /// <returns>Обновленный "список" неотправленных сообщений.</returns>
            private IEnumerable<IMessage> UpdateFailedMessages(IMessage message)
            {
                if (FailedMessages.Contains(message) is false)
                    return FailedMessages.Concat(new[] {message});
                return FailedMessages;
            }

            /// <summary>
            /// Метод выбора способа отправки сообщения.
            /// </summary>
            /// <param name="delMethod">Номер способа доставки</param>
            /// <returns>Реализацию интерфейса <see cref="ISender"/>.</returns>
            private ISender SenderDetermination(int delMethod) => delMethod switch
            {
                0 => new TrueSender(),
                //1 => new TelegramSender(),
                //2 => new SMSSender(),
                //3 => new MailSender(),
                //4 => new WhatupSende(),
                _ => new FalseSender(),
            };

            /// <summary>
            /// Класс-заглушка. Сообщение не отправляется и не записывается в список <see cref="Postman.FailedMessages"/>.
            /// </summary>
            private class
                TrueSender : ISender //Классы TrueSender и FalseSender были написаны для избежания возвращения null в методе SenderDetermination
            {
                public bool Send(string message, string address)
                {
                    return true;
                }
            }

            /// <summary>
            /// Класс-заглушка. Сообщение не отправляется, но записывается в список <see cref="Postman.FailedMessages"/>.
            /// </summary>
            private class FalseSender : ISender
            {
                public bool Send(string message, string address)
                {
                    return false;
                }
            }
        }
    }
}
