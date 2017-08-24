using System;

namespace SmartQQ.Builder
{
    /// <summary>
    ///     因API错误产生的异常。
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        ///     声明一个API异常。
        /// </summary>
        /// <param name="errorCode"></param>
        public ApiException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     返回的错误码。
        /// </summary>
        public int ErrorCode { get; }

        /// <inheritdoc />
        public override string Message => "API错误，返回码" + ErrorCode;
    }
}
