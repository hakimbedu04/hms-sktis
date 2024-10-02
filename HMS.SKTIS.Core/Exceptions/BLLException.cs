using System.Collections.Generic;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.Core
{
    /// <summary>
    /// Class BLLException
    /// </summary>
    public class BLLException : ExceptionBase
    {



        /// <summary>
        /// Prevents a default instance of the <see cref="BLLException"/> class from being created.
        /// </summary>
        private BLLException() : base("") { }

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum)
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(ExceptionCodes.BLLExceptions code)
            : base(EnumHelper.GetDescription(code))
        {
            Code = code.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum) and a message
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(ExceptionCodes.BLLExceptions code, string message)
            : base(message)
        {
            Code = code.ToString();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BLLException"/> class with multiple error codes
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="errorCodes">The error codes.</param>
        public BLLException(ExceptionCodes.BLLExceptions code, List<string> errorCodes)
            : base("")
        {
            Code = code.ToString();
            ErrorCodes = errorCodes;
        }


        /// <summary>
        /// Initializes a new instance of the BLLException with HResult code
        /// </summary>
        /// <param name="hResult"></param>
        public BLLException(int hResult)
            : base(EnumHelper.GetDescription(convertHResultToBLLExceptions(hResult)))
        {
            Code = convertHResultToBLLExceptions(hResult).ToString();
        }


        private static ExceptionCodes.BLLExceptions convertHResultToBLLExceptions(int hResult)
        {
            var ex = ExceptionCodes.BLLExceptions.UnhandledException;
            switch (hResult)
            {
                case -2146233087: ex = ExceptionCodes.BLLExceptions.KeyExist; break;
                case -2146232032: ex = ExceptionCodes.BLLExceptions.Mandatory; break;
            }
            return ex;
        }
    }
}
