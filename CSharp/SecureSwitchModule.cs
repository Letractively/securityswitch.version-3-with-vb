using System;
using System.Configuration;
using System.Web;
using SecureSwitch.Configuration;

namespace SecureSwitch {

	/// <summary>
	/// Hooks the application's BeginRequest event in order to request the current 
	/// page securely if specified in the configuration file.
	/// </summary>
	public class SecureSwitchModule : IHttpModule {

		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		public SecureSwitchModule() {
		}

		/// <summary>
		/// Disposes of any resources used.
		/// </summary>
		public void Dispose() {
			// No resources were used.
		}

		/// <summary>
		/// Occurs just before the SecureSwitchModule evaluates the current request.
		/// </summary>
		public event BeforeEvaluateRequestEventHandler BeforeEvaluateRequest;

		/// <summary>
		/// Initializes the module by hooking the application's BeginRequest event if indicated by the config settings.
		/// </summary>
		/// <param name="application">The HttpApplication this module is bound to.</param>
		public void Init(HttpApplication context) {
			if (context != null) {
				// Get the settings for the SecureSwitch section.
				Settings Settings = (Settings)ConfigurationSettings.GetConfig("secureSwitch");
				if (Settings != null && Settings.Mode != Mode.Off) {
					// Store the settings in application state for quick access on each request.
					context.Application["Settings"] = Settings;

					// Add a reference to the Application_ProcessRequest handler for the application's
					// AcquireRequestState event.
					// * This ensures that the session ID is available for cookie-less session processing.
					context.AcquireRequestState += new EventHandler(this.Application_ProcessRequest);
				}
			}
		}

		/// <summary>
		/// Raises the BeforeEvaluateRequest event.
		/// </summary>
		/// <param name="e">The EvaluateRequestEventArgs used for the event.</param>
		protected void OnBeforeEvaluateRequest(EvaluateRequestEventArgs e) {
			// Raise the event.
			BeforeEvaluateRequestEventHandler Handler = BeforeEvaluateRequest;
			if (Handler != null)
				Handler(this, e);
		}

		/// <summary>
		/// Process this request by evaluating it appropriately.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">EventArgs passed in.</param>
		private void Application_ProcessRequest(Object source, EventArgs e) {
			// Cast the source as an HttpApplication instance.
			HttpApplication Context = source as HttpApplication;
			if (Context != null) {
				// Retrieve the settings from application state.
				Settings Settings = (Settings)Context.Application["Settings"];

				// Call the BeforeEvaluateRequest event and check if a subscriber indicated to cancel the 
				// evaluation of the current request.
				EvaluateRequestEventArgs Args = new EvaluateRequestEventArgs(Context, Settings);
				OnBeforeEvaluateRequest(Args);

				if (!Args.CancelEvaluation) {
					// Evaluate the response against the settings.
					SecurityType Secure = RequestEvaluator.Evaluate(Context.Request, Settings, false);

					// Take appropriate action.
					if (Secure == SecurityType.Secure)
						SslHelper.RequestSecurePage(Settings);
					else if (Secure == SecurityType.Insecure)
						SslHelper.RequestUnsecurePage(Settings);
				}
			}
		}

	}


	/// <summary>
	/// Represents the method that handles the event raised just before a request is evaluated by 
	/// the SecureSwitchModule.
	/// </summary>
	/// <param name="sender">The SecureSwitchModule that is the source of the event.</param>
	/// <param name="e">An EvaluateRequestEventArgs that contains the event data.</param>
	public delegate void BeforeEvaluateRequestEventHandler(object sender, EvaluateRequestEventArgs e);

}
