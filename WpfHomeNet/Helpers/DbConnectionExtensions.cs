using System.Data;
using System.Threading.Tasks;

namespace WpfHomeNet.Helpers
{
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Асинхронно открывает соединение IDbConnection.
        /// </summary>
        /// <param name="connection">Соединение для открытия</param>
        /// <returns>Задача, представляющая операцию открытия</returns>
        public static async Task OpenAsync(this IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                return;

            // Выполняем синхронный Open() в фоновом потоке
            await Task.Run(() => connection.Open());
        }
    }
}
