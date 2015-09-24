using NLog;
using NLog.Targets;
using SP2013Access.Controls;

namespace SP2013Access.Logging
{
    public sealed class LogViewerTarget : TargetWithLayout
    {
        public LogViewer TargetLogViewer { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            base.Write(logEvent);
            TargetLogViewer?.LogEntries.Add(logEvent);
        }
    }
}