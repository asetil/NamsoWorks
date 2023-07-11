using System;

namespace Aware.Util.Log
{
    public interface ILogger
    {
        void Error(string message, Exception ex);
        void Error(string format, Exception ex, params object[] param);
        void Info(string message, params object[] param);
        void Warn(string message, string title, bool sendMail = false);
        void Critical(string title, string message, bool sendMail, params object[] param);
    }
}