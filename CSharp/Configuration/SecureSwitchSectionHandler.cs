using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using SecureSwitch.Configuration;

namespace SecureSwitch {

	/// <summary>
	/// The exception thrown for any errors while reading the secureSwitch 
	/// section of a configuration file.
	/// </summary>
	public class SecureSwitchSectionException : System.Configuration.ConfigurationException {

		/// <summary>
		/// Intializes an instance of this exception.
		/// </summary>
		public SecureSwitchSectionException() {
		}

		/// <summary>
		/// Initializes an instance of this exception with specified parameters.
		/// </summary>
		/// <param name="message">The message to display to the client when the exception is thrown.</param>
		/// <param name="node">The XmlNode that contains the error.</param>
		public SecureSwitchSectionException(string message, XmlNode node)
			: base(message, node) {
		}

	}


	/// <summary>
	/// SecureSwitchSectionHandler reads any &lt;secureSwitch&gt; section from a configuration file.
	/// </summary>
	public class SecureSwitchSectionHandler : IConfigurationSectionHandler {

		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		public SecureSwitchSectionHandler() {
		}

		/// <summary>
		/// Parses the XML configuration section and returns the settings.
		/// </summary>
		/// <param name="parent">
		///		The configuration settings in a corresponding parent 
		///		configuration section.
		/// </param>
		/// <param name="configContext">
		///		An HttpConfigurationContext when Create is called from the ASP.NET 
		///		configuration system. Otherwise, this parameter is reserved and is 
		///		a null reference (Nothing in Visual Basic).
		/// </param>
		/// <param name="section">
		///		The XmlNode that contains the configuration information from the 
		///		configuration file. Provides direct access to the XML contents of 
		///		the configuration section.
		/// </param>
		/// <returns>
		///		Returns a Settings instance initialized with the 
		///		read configuration settings.
		///	</returns>
		public object Create(object parent, object configContext, XmlNode section) {
			// Create a Settings object for the settings in this section.
			Settings Settings = new Settings();

			// Read the general settings.
			ReadGeneralSettings(section, Settings);

			// Traverse the child nodes
			foreach (XmlNode Node in section.ChildNodes) {
				if (Node.NodeType == System.Xml.XmlNodeType.Comment)
					// Skip comment nodes (thanks to dcbrower on CodeProject for pointing this out).
					continue;
				else if (Node.Name.ToLower() == "directory")
					// This is a directory path node.
					Settings.Directories.Add(ReadDirectoryItem(Node));
				else if (Node.Name.ToLower() == "file")
					// This is a file path node.
					Settings.Files.Add(ReadFileItem(Node));
				else
					// Throw an exception for this unrecognized node.
					throw new SecureSwitchSectionException(string.Format("'{0}' is not an acceptable setting.", Node.Name), Node);
			}

			// Return the settings.
			return Settings;
		}

		/// <summary>
		/// Reads general settings from the SecureSwitch section into the given Settings instance.
		/// </summary>
		/// <param name="section">The XmlNode to read from.</param>
		/// <param name="settings">The Settings instance to set.</param>
		protected void ReadGeneralSettings(XmlNode section, Settings settings) {
			// Get the mode attribute.
			if (section.Attributes["mode"] != null) {
				string ModeValue = section.Attributes["mode"].Value.Trim();
				if (Enum.IsDefined(typeof(Mode), ModeValue))
					settings.Mode = (Mode)Enum.Parse(typeof(Mode), ModeValue);
				else
					throw new SecureSwitchSectionException("Invalid value for the 'mode' attribute.", section);
			}

			// Get the encryptedUri attribute.
			if (section.Attributes["encryptedUri"] != null)
				settings.EncryptedUri = section.Attributes["encryptedUri"].Value;

			// Get the unencryptedUri attribute.
			if (section.Attributes["unencryptedUri"] != null)
				settings.UnencryptedUri = section.Attributes["unencryptedUri"].Value;

			// Validate that if either encryptedUri or unencryptedUri are set, both must be set.
			if (
				(settings.EncryptedUri.Length > 0 && settings.UnencryptedUri.Length == 0) ||
				(settings.UnencryptedUri.Length > 0 && settings.EncryptedUri.Length == 0))
				throw new SecureSwitchSectionException("You must specify both 'encryptedUri' and 'unencryptedUri', or neither.", section);

			// Get the maintainPath attribute.
			if (section.Attributes["maintainPath"] != null) {
				string Value = section.Attributes["maintainPath"].Value.ToLower();
				settings.MaintainPath = (Value == "true" || Value == "yes" || Value == "on");
			}

			// Get the warningBypassMode attribute.
			if (section.Attributes["warningBypassMode"] != null) {
				string WarningBypassModeValue = section.Attributes["warningBypassMode"].Value.Trim();
				if (Enum.IsDefined(typeof(SecurityWarningBypassMode), WarningBypassModeValue))
					settings.WarningBypassMode = (SecurityWarningBypassMode)Enum.Parse(typeof(SecurityWarningBypassMode), WarningBypassModeValue);
				else
					throw new SecureSwitchSectionException("Invalid value for the 'warningBypassMode' attribute.", section);
			}

			// Get the bypassQueryParamName attribute.
			if (section.Attributes["bypassQueryParamName"] != null)
				settings.BypassQueryParamName = section.Attributes["bypassQueryParamName"].Value;
		}

		/// <summary>
		/// Reads the typical attributes for a ItemSetting from the configuration node.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <param name="item">The ItemSetting to set values for.</param>
		protected void ReadChildItem(XmlNode node, ItemSetting item) {
			// Set the item only if the node has a valid path attribute value.
			if (node.Attributes["path"] != null && node.Attributes["path"].Value.Trim().Length > 0) {
				// Get the value of the path attribute.
				item.Path = node.Attributes["path"].Value.Trim().ToLower();

				// Remove leading and trailing "/" characters.
				if (item.Path.Length > 0)
					item.Path = item.Path.Trim('/');

				// Check for a secure attribute.
				if (node.Attributes["secure"] != null) {
					string SecureValue = node.Attributes["secure"].Value.Trim();
					if (Enum.IsDefined(typeof(SecurityType), SecureValue))
						item.Secure = (SecurityType)Enum.Parse(typeof(SecurityType), SecureValue);
					else
						throw new SecureSwitchSectionException("Invalid value for the 'secure' attribute.", node);
				}
			} else
				// Throw an exception for the missing Path attribute.
				throw new SecureSwitchSectionException("'path' attribute not found.", node);
		}

		/// <summary>
		/// Reads a directory item from the configuration node and returns a new instance of DirectorySetting.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <returns>A DirectorySetting initialized with values read from the node.</returns>
		protected DirectorySetting ReadDirectoryItem(XmlNode node) {
			// Create a DirectorySetting instance.
			DirectorySetting Item = new DirectorySetting();

			// Read the typical attributes.
			ReadChildItem(node, Item);

			// Check for a recurse attribute.
			if (node.Attributes["recurse"] != null) {
				string Value = node.Attributes["recurse"].Value.ToLower();
				Item.Recurse = (Value == "true" || Value == "yes" || Value == "on");
			}

			return Item;
		}

		/// <summary>
		/// Reads a file item from the configuration node and returns a new instance of FileSetting.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <returns>A FileSetting initialized with values read from the node.</returns>
		protected FileSetting ReadFileItem(XmlNode node) {
			// Create a FileSetting instance.
			FileSetting Item = new FileSetting();

			// Read the typical attributes.
			ReadChildItem(node, Item);

			return Item;
		}

	}

}
