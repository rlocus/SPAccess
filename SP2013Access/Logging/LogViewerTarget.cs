using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using NLog.Common;
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

            if (this.TargetLogViewer != null)
            {
                this.TargetLogViewer.LogEntries.Add(logEvent);
            }
        }
    }
}
