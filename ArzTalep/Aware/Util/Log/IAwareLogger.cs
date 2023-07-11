using System;

namespace Aware.Util.Log
{
    public interface IAwareLogger
    {
        void Error(string key, Exception ex);

        void Error(string key, string message, Exception ex, params object[] param);

        void Info(string key, string message, params object[] param);

        void Warn(string key, string message, bool sendMail, params object[] param);

        void Critical(string key, string message, bool sendMail, params object[] param);
    }
}