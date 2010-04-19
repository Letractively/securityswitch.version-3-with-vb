using System;
using System.Globalization;
using System.Web;
using System.Web.Configuration;
using SecureSwitch.Configuration;

namespace SecureSwitch {

	/// <summary>
	/// Represents an evaluator for requests that 
	/// </summary>
	public sealed class RequestEvaluator {

		/// <summary>
		/// Evaluates a given request against specified settings for the type of security action required
		/// to fulfill the request properly.
		/// </summary>
		/// <param name="request">The request to evaluate.</param>
		/// <param name="settings">The settings to evaluate against.</param>
		/// <param name="forceEvaluation">
		/// A flag indicating whether or not to force evaluation, despite the mode set.
		/// </param>
		/// <returns>A SecurityType value for the appropriate action.</returns>
		public static SecurityType Evaluate(HttpRequest request, Settings settings, bool forceEvaluation) {
			// Initialize the result to Ignore.
			SecurityType Result = SecurityType.Ignore;

			// Determine if this request should be ignored based on the settings' Mode.
			bool MustEvaluateRequest = (forceEvaluation || RequestMatchesMode(request, settings.Mode));
			DebugHelper.Output(MustEvaluateRequest ? "Evaluating request..." : "Evaluation of request skipped.");
			if (MustEvaluateRequest) {
				// Make sure the request shouldn't be ignored as a HTTP handler.
				if (settings.IgnoreHandlers == IgnoreHandlers.BuiltIn && !IsBuiltInHandlerRequest(request) ||
					settings.IgnoreHandlers == IgnoreHandlers.WithStandardExtensions && !IsStandardHandlerRequest(request) ||
					settings.IgnoreHandlers == IgnoreHandlers.None) {
					// Get the relative file path of the current request from the application root.
					string RelativeFilePath = HttpUtility.UrlDecode(request.Url.AbsolutePath).Remove(0, request.ApplicationPath.Length).ToLower(CultureInfo.CurrentCulture);
					if (RelativeFilePath.StartsWith("/"))
						// Remove any leading "/".
						RelativeFilePath = RelativeFilePath.Substring(1);

					// Get the relative directory of the current request by removing the last segment of the RelativeFilePath.
					string RelativeDirectory = string.Empty;
					int i = RelativeFilePath.LastIndexOf('/');
					if (i >= 0)
						RelativeDirectory = RelativeFilePath.Substring(0, i);

					// Determine if there is a matching file path for the current request.
					i = settings.Files.IndexOf(RelativeFilePath);
					if (i >= 0) {
						Result = settings.Files[i].Secure;
						DebugHelper.Output("Request matches file: {0} - {1}.", settings.Files[i].Secure, settings.Files[i].Path);
					} else {
						// Try to find a matching directory path.
						int j = -1;
						i = 0;
						while (i < settings.Directories.Count) {
							// Try to match the beginning of the directory if recursion is allowed (partial match).
							if ((settings.Directories[i].Recurse && RelativeDirectory.StartsWith(settings.Directories[i].Path, StringComparison.CurrentCultureIgnoreCase) ||
								RelativeDirectory.Equals(settings.Directories[i].Path, StringComparison.CurrentCultureIgnoreCase)) &&
								(j == -1 || settings.Directories[i].Path.Length > settings.Directories[j].Path.Length))
								// First or longer partial match found (deepest recursion is the best match).
								j = i;

							i++;
						}

						if (j > -1) {
							// Indicate a match for a partially matched directory allowing recursion.
							Result = settings.Directories[j].Secure;
							DebugHelper.Output("Request matches directory: {0} - {1}.", settings.Directories[j].Secure, settings.Directories[j].Path);
						} else {
							// No match indicates an insecure result.
							Result = SecurityType.Insecure;
							DebugHelper.Output("Request does not match anything.");
						}
					}
				}
			}

			return Result;
		}

		/// <summary>
		/// Evaluates a given request against configured settings for the type of security action required
		/// to fulfill the request properly.
		/// </summary>
		/// <param name="request">The request to evaluate.</param>
		/// <returns>A SecurityType value for the appropriate action.</returns>
		public static SecurityType Evaluate(HttpRequest request) {
			// Get the settings for the SecureSwitch section.
			Settings Settings = WebConfigurationManager.GetSection("SecureSwitch") as Settings;

			return Evaluate(request, Settings, false);
		}

		/// <summary>
		/// Determines if the specified request is for one of the built-in HTTP handlers.
		/// </summary>
		/// <param name="request">The HttpRequest to test.</param>
		/// <returns>True if the request is for a built-in HTTP handler; false otherwise.</returns>
		private static bool IsBuiltInHandlerRequest(HttpRequest request) {
			// Get the file name of the request.
			string FileName = request.Url.Segments[request.Url.Segments.Length - 1];
			return (
				string.Compare(FileName, "trace.axd", true, CultureInfo.InvariantCulture) == 0 ||
				string.Compare(FileName, "webresource.axd", true, CultureInfo.InvariantCulture) == 0
			);
		}

		/// <summary>
		/// Determines if the specified request is for a standard HTTP handler (.axd).
		/// </summary>
		/// <param name="request">The HttpRequest to test.</param>
		/// <returns>True if the request is for a standard HTTP handler (.axd or .ashx); false otherwise.</returns>
		private static bool IsStandardHandlerRequest(HttpRequest request) {
			string Path = request.Url.AbsolutePath;
			return (
				Path.EndsWith(".axd", true, CultureInfo.InvariantCulture) || 
				Path.EndsWith(".ashx", true, CultureInfo.InvariantCulture) ||
				Path.EndsWith(".asmx/js", true, CultureInfo.InvariantCulture) || 
				Path.EndsWith(".asmx/jsdebug", true, CultureInfo.InvariantCulture)
			);
		}

		/// <summary>
		/// Tests the given request to see if it matches the specified mode.
		/// </summary>
		/// <param name="request">A HttpRequest to test.</param>
		/// <param name="mode">The Mode used in the test.</param>
		/// <returns>
		///		Returns true if the request matches the mode as follows:
		///		<list type="disc">
		///			<item>If mode is On.</item>
		///			<item>If mode is set to RemoteOnly and the request is from a computer other than the server.</item>
		///			<item>If mode is set to LocalOnly and the request is from the server.</item>
		///		</list>
		///	</returns>
		private static bool RequestMatchesMode(HttpRequest request, Mode mode) {
			switch (mode) {
				case Mode.On:
					return true;

				case Mode.RemoteOnly:
					return (request.ServerVariables["REMOTE_ADDR"] != request.ServerVariables["LOCAL_ADDR"]);

				case Mode.LocalOnly:
					return (request.ServerVariables["REMOTE_ADDR"] == request.ServerVariables["LOCAL_ADDR"]);

				default:
					return false;
			}
		}

	}

}
