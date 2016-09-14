using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace AsyncClient
{
    public sealed class TextBoxListener : TraceListener
    {
        private readonly TextBoxBase m_Output;

        public TextBoxListener(TextBoxBase output)
        {
            Name = "Trace";
            m_Output = output;
        }
        
        public override void Write(string message)
        {

            Action append = delegate
            {
                m_Output.AppendText($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] ");
                m_Output.AppendText(message);
            };
            if (m_Output.Dispatcher.CheckAccess())
            {
                m_Output.Dispatcher.BeginInvoke(append);
            }
            else
            {
                append();
            }

        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }
    }
}
