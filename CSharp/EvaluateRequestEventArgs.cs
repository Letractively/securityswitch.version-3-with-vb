using System;
using System.Web;
using SecureSwitch.Configuration;

namespace SecureSwitch {

	/// <summary>
	/// Defines the arguments used for the EvaluateRequest event.
	/// </summary>
	public class EvaluateRequestEventArgs : EventArgs {

		// Fields.
		private HttpApplication _application;
		private bool _cancelEvaluation = false;
		private Settings _settings;


		/// <summary>
		/// Gets the HttpApplication used to evaluate the request.
		/// </summary>
		public HttpApplication Application {
			get { return _application; }
		}

		/// <summary>
		/// Gets or sets a flag indicating whether or not to cancel the evaluation.
		/// </summary>
		public bool CancelEvaluation {
			get { return _cancelEvaluation; }
			set { _cancelEvaluation = value; }
		}

		/// <summary>
		/// Gets the Settings used to evaluate the request.
		/// </summary>
		public Settings Settings {
			get { return _settings; }
		}

		/// <summary>
		/// Creates an instance of EvaluateRequestEventArgs with an instance of Settings.
		/// </summary>
		/// <param name="application">The HttpApplication for the current context.</param>
		/// <param name="settings">An instance of Settings used for the evaluation of the request.</param>
		public EvaluateRequestEventArgs(HttpApplication application, Settings settings)
			: base() {
			_application = application;
			_settings = settings;
		}

	}

}
