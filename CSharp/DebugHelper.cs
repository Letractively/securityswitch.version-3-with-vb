using System;
using System.Web;

namespace SecuritySwitch {
	
	/// <summary>
	/// A helper class for debugging.
	/// </summary>
	internal static class DebugHelper {

		// The category to use when tracing.
		const string TraceCategory = "SecuritySwitchModule";

		// Holds a reference to the current trace context.
		private static TraceContext _trace;
		private static TraceContext Trace {
			get {
				if (_trace == null) {
					_trace = HttpContext.Current.Trace;
				}
				return _trace;
			}
		}


		internal static void Output(string message) {
			Trace.Write(TraceCategory, message);
		}

		internal static void Output(string formattedMessage, params object[] args) {
			Trace.Write(TraceCategory, string.Format(formattedMessage, args));
		}

		internal static void OutputIf(bool condition, string message) {
			if (condition) {
				Trace.Write(TraceCategory, message);
			}
		}

		internal static void OutputIf(bool condition, string formattedMessage, params object[] args) {
			if (condition) {
				Trace.Write(TraceCategory, string.Format(formattedMessage, args));
			}
		}

	}

}
