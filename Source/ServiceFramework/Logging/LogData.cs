using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ServiceFramework.Logging
{
    /// <summary>
    /// A class holding log data before being written.
    /// </summary>
    public sealed class LogData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        public LogData()
        {
            Logger = "Logger";
            FormatProvider = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        /// <value>
        /// The logger name.
        /// </value>
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the trace level.
        /// </summary>
        /// <value>
        /// The trace level.
        /// </value>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message parameters. Used with String.Format.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Gets or sets the message formatter <see langword="delegate"/>.
        /// </summary>
        /// <value>
        /// The message formatter <see langword="delegate"/>.
        /// </value>
        public Func<string> MessageFormatter { get; set; }

        /// <summary>
        /// Gets or sets the format provider.
        /// </summary>
        /// <value>
        /// The format provider.
        /// </value>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the log properties.
        /// </summary>
        /// <value>
        /// The log properties.
        /// </value>
        public IDictionary<string, object> Properties { get; set; }


        /// <summary>
        /// Formats the log message.
        /// </summary>
        /// <returns>The formatted log message.</returns>
        public string FormatMessage()
        {
            try
            {
                if (MessageFormatter != null)
                    return MessageFormatter();

                if (Parameters != null && Parameters.Length > 0)
                    return string.Format(FormatProvider, Message, Parameters);

            }
            catch (Exception)
            {
                // don't throw error 
            }

            return Message ?? string.Empty;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var message = new StringBuilder();
            message
                .Append(DateTime.Now.ToString("HH:mm:ss.fff"))
                .Append(" [")
                .Append(LogLevel.ToString()[0])
                .Append("] ");

            if (!string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(MemberName))
            {
                message
                    .Append("[")
                    .Append(FilePath)
                    .Append(" ")
                    .Append(MemberName)
                    .Append("()")
                    .Append(" Ln: ")
                    .Append(LineNumber)
                    .Append("] ");
            }

            try
            {
                if (MessageFormatter != null)
                    message.Append(MessageFormatter());
                else if (Parameters != null && Parameters.Length > 0)
                    message.AppendFormat(FormatProvider, Message, Parameters);
                else
                    message.Append(Message);

                if (Exception != null)
                    message.Append(" ").Append(Exception);

            }
            catch (Exception)
            {
                // don't throw error 
            }

            return message.ToString();
        }


        /// <summary>
        /// Reset all properties back to default.
        /// </summary>
        internal void Reset()
        {
            Logger = "Logger";
            LogLevel = LogLevel.Trace;
            Message = null;
            Parameters = null;
            MessageFormatter = null;
            FormatProvider = CultureInfo.InvariantCulture;
            Exception = null;
            MemberName = null;
            FilePath = null;
            LineNumber = 0;
            Properties?.Clear();
        }
    }
}