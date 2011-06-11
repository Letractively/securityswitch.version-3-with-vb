using System;
using System.Web;
using System.Web.Configuration;
using SecuritySwitch.Configuration;

namespace SecuritySwitch {

	/// <summary>
	/// Hooks the application's BeginRequest event in order to request the current 
	/// page securely if specified in the configuration file.
	/// </summary>
	public class SecuritySwitchModule : IHttpModule {

		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		public SecuritySwitchModule() {
			DebugHelper.Output("Module created.");
		}

		/// <summary>
		/// Disposes of any resources used.
		/// </summary>
		public void Dispose() {
			// No resources were used.
			DebugHelper.Output("Module disposed.");
		}

		/// <summary>
		/// Occurs just before the SecuritySwitchModule evaluates the current request.
		/// </summary>
		public event BeforeEvaluateRequestEventHandler BeforeEvaluateRequest;

		/// <summary>
		/// Initializes the module by hooking the application's BeginRequest event if indicated by the config settings.
		/// </summary>
		/// <param name="context">The HttpApplication this module is bound to.</param>
		public void Init(HttpApplication context) {
			if (context != null) {
				// Get the settings for the SecuritySwitch section.
				DebugHelper.Output("Reading settings from configuration.");
				Settings Settings = WebConfigurationManager.GetSection("securitySwitch") as Settings;
				DebugHelper.Output(Settings != null ? "Settings read successfully." : "Settings read failed!");

				if (Settings != null && Settings.Mode != Mode.Off) {
					// Store the settings in application state for quick access on each request.
					context.Application["Settings"] = Settings;
					DebugHelper.Output("Settings cached to application context.");

					// Add a reference to the Application_ProcessRequest handler for the application's
					// AcquireRequestState event.
					// * This ensures that the session ID is available for cookie-less session processing.
					context.AcquireRequestState += new EventHandler(this.Application_ProcessRequest);
					
					//// Hook into the BeginRequest event in order to test for cookie-less sessions.
					//context.BeginRequest += new EventHandler(Application_BeginRequest);
				}
			}
		}

		//private void Application_BeginRequest(object sender, EventArgs e) {
		//    HttpApplication Context = sender as HttpApplication;
		//    if (Context != null) {
		//        if (IsCookielessSessionUsed(Context)) {
		//            // This ensures that the session ID is available for cookie-less session processing.
		//            Context.AcquireRequestState += new EventHandler(Application_ProcessRequest);
		//        } else {
		//            // Process the request here.
		//            Application_ProcessRequest(sender, e);
		//        }
		//    }
		//}

		///// <summary>
		///// Determines if cookie-less sessions are being used.
		///// </summary>
		///// <param name="context">The HttpApplication this module is bound to.</param>
		///// <returns></returns>
		//protected bool IsCookielessSessionUsed(HttpApplication context) {
		//    string VirtualPath = context.Request.Path;
		//    string ModifiedPath = context.Response.ApplyAppPathModifier(VirtualPath);
		//    return !ModifiedPath.Equals(VirtualPath);
		//}
		
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
			DebugHelper.OutputIf(Context == null, "No context to process!");
			if (Context != null) {
				// Retrieve the settings from application state.
				Settings Settings = (Settings)Context.Application["Settings"];
				DebugHelper.Output(Settings != null ? "Settings retrieved from application cache." : "Settings not found in application cache!");

				// Call the BeforeEvaluateRequest event and check if a subscriber indicated to cancel the 
				// evaluation of the current request.
				EvaluateRequestEventArgs Args = new EvaluateRequestEventArgs(Context, Settings);
				DebugHelper.Output("BeforeEvaluateRequest event about to fire.");
				OnBeforeEvaluateRequest(Args);
				DebugHelper.Output("BeforeEvaluateRequest event fired and returned.");

				DebugHelper.OutputIf(Args.CancelEvaluation, "Evaluation was canceled by a user event handler.");
				if (!Args.CancelEvaluation) {
					// Evaluate the response against the settings.
					SecurityType Secure = RequestEvaluator.Evaluate(Context.Request, Settings, false);

					// Take appropriate action.
					DebugHelper.OutputIf(Secure == SecurityType.Ignore, "Ignoring request.");
					if (Secure == SecurityType.Secure) {
						SslHelper.RequestSecurePage(Settings);
					} else if (Secure == SecurityType.Insecure) {
						SslHelper.RequestUnsecurePage(Settings);
					}
				}
			}
		}

	}


	/// <summary>
	/// Represents the method that handles the event raised just before a request is evaluated by 
	/// the SecuritySwitchModule.
	/// </summary>
	/// <param name="sender">The SecuritySwitchModule that is the source of the event.</param>
	/// <param name="e">An EvaluateRequestEventArgs that contains the event data.</param>
	public delegate void BeforeEvaluateRequestEventHandler(object sender, EvaluateRequestEventArgs e);

}
