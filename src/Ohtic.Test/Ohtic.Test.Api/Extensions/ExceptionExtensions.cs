using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Ohtic.Test.Api.Extensions
{
    internal static class ExceptionExtensions
    {
        internal static bool IsSystemNotFound(this Exception ex)
        {
            return
                ex is KeyNotFoundException ||
                ex is IndexOutOfRangeException ||
                ex is ArgumentOutOfRangeException ||
                ex is ArgumentException;
        }

        internal static bool IsMySqlConstraintViolation(this Exception ex)
            => ex.IsMySqlExceptionOfErrorCode(3819);

        internal static bool IsMySqlDataTooLong(this Exception ex)
            => ex.IsMySqlExceptionOfErrorCode(MySqlErrorCode.DataTooLong);
            
        internal static bool IsMySqlDuplicateKeyEntry(this Exception ex)
            => ex.IsMySqlExceptionOfErrorCode(MySqlErrorCode.DuplicateKeyEntry);

        internal static bool IsMySqlNoReferencedRow(this Exception ex)
            => ex.IsMySqlExceptionOfErrorCode(MySqlErrorCode.NoReferencedRow2);

        private static bool IsMySqlExceptionOfErrorCode(this Exception ex, MySqlErrorCode errorCode)
        {
            return
                ex.IsMySqlException(out var mySqlException) &&
                mySqlException!.ErrorCode == errorCode;
        }

        private static bool IsMySqlExceptionOfErrorCode(this Exception ex, int errorCode)
        {
            return
                ex.IsMySqlException(out var mySqlException) &&
                (int)mySqlException!.ErrorCode == errorCode;
        }

        private static bool IsMySqlException(this Exception ex, out MySqlException? mySqlException)
        {
            if (ex is DbUpdateException dbUpdateException &&
            dbUpdateException.InnerException is MySqlException innerException)
            {
                mySqlException = innerException;
                return true;
            }

            mySqlException = null;
            return false;
        }
    }
}
