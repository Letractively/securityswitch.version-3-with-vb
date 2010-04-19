using System;
using System.Collections.Specialized;

namespace SecureSwitch.Configuration {

	/// <summary>
	/// The different modes supported for the &lt;secureSwitch&gt; configuration section.
	/// </summary>
	public enum Mode {
		/// <summary>
		/// Indicates that web page security is on and all requests should be monitored.
		/// </summary>
		On,

		/// <summary>
		/// Only remote requests are to be monitored.
		/// </summary>
		RemoteOnly,

		/// <summary>
		/// Only local requests are to be monitored.
		/// </summary>
		LocalOnly,

		/// <summary>
		/// Web page security is off and no monitoring should occur.
		/// </summary>
		Off
	}

	/// <summary>
	/// The different modes for bypassing security warnings.
	/// </summary>
	public enum SecurityWarningBypassMode {
		/// <summary>
		/// Always bypass security warnings when switching to an unencrypted page.
		/// </summary>
		AlwaysBypass,

		/// <summary>
		/// Only bypass security warnings when switching to an unencrypted page if the proper query parameter is present.
		/// </summary>
		BypassWithQueryParam,

		/// <summary>
		/// Never bypass security warnings when switching to an unencrypted page.
		/// </summary>
		NeverBypass
	}


	/// <summary>
	/// Settings contains the settings of a SecureSwitch configuration section.
	/// </summary>
	public class Settings {

		// Fields
		private string bypassQueryParamName = "BypassSecurityWarning";
		private string encryptedUri = string.Empty;
		private bool maintainPath = true;
		private Mode mode = Mode.On;
		private DirectorySettingCollection directories;
		private FileSettingCollection files;
		private string unencryptedUri = string.Empty;
		private SecurityWarningBypassMode warningBypassMode = SecurityWarningBypassMode.BypassWithQueryParam;

		#region Properties

		/// <summary>
		/// Gets or sets the name of the query parameter that will indicate to the module to bypass
		/// any security warning if WarningBypassMode = BypassWithQueryParam.
		/// </summary>
		public string BypassQueryParamName {
			get { return bypassQueryParamName; }
			set { bypassQueryParamName = value; }
		}

		/// <summary>
		/// Gets or sets the path to a URI for encrypted redirections, if any.
		/// </summary>
		public string EncryptedUri {
			get { return encryptedUri; }
			set {
				if (value != null && value.Length > 0)
					encryptedUri = value;
				else
					encryptedUri = string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets a flag indicating whether or not to maintain the current path when redirecting
		/// to a different host.
		/// </summary>
		public bool MaintainPath {
			get { return maintainPath; }
			set { maintainPath = value; }
		}

		/// <summary>
		/// Gets or sets the mode indicating how the secure web page settings handled.
		/// </summary>
		public Mode Mode {
			get { return mode; }
			set { mode = value; }
		}

		/// <summary>
		/// Gets the collection of directories read from the configuration section.
		/// </summary>
		public DirectorySettingCollection Directories {
			get { return directories; }
		}

		/// <summary>
		/// Gets the collection of files read from the configuration section.
		/// </summary>
		public FileSettingCollection Files {
			get { return files; }
		}

		/// <summary>
		/// Gets or sets the path to a URI for unencrypted redirections, if any.
		/// </summary>
		public string UnencryptedUri {
			get { return unencryptedUri; }
			set {
				if (value != null && value.Length > 0)
					unencryptedUri = value;
				else
					unencryptedUri = string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the bypass mode indicating whether or not to bypass security warnings
		/// when switching to a unencrypted page.
		/// </summary>
		public SecurityWarningBypassMode WarningBypassMode {
			get { return warningBypassMode; }
			set { warningBypassMode = value; }
		}

		#endregion

		/// <summary>
		/// The default constructor creates the needed lists.
		/// </summary>
		public Settings() {
			// Create the collections
			directories = new DirectorySettingCollection();
			files = new FileSettingCollection();
		}

	}

}
