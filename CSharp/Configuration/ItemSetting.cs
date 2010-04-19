using System;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SecureSwitch.Configuration {

	/// <summary>
	/// Indicates the type of security for a file or directory.
	/// </summary>
	public enum SecurityType {

		/// <summary>
		/// The item should be secure.
		/// </summary>
		Secure,

		/// <summary>
		/// The item should be insecure.
		/// </summary>
		Insecure,

		/// <summary>
		/// The item should be ignored.
		/// </summary>
		Ignore

	}

	/// <summary>
	/// Represents an file or directory entry in the &lt;secureSwitch&gt;
	/// configuration section.
	/// </summary>
	public abstract class ItemSetting : ConfigurationElement {

		#region Constructors

		/// <summary>
		/// Creates an instance of ItemSetting.
		/// </summary>
		protected ItemSetting()
			: base() {
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the file.</param>
		protected ItemSetting(string path)
			: this() {
			Path = path;
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the file.</param>
		/// <param name="secure">The type of security for the file.</param>
		protected ItemSetting(string path, SecurityType secure)
			: this(path) {
			Secure = secure;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the path of this item.
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true, IsKey = true)]
		public virtual string Path {
			get { return (string)this["path"]; }
			set { this["path"] = value; }
		}

		/// <summary>
		/// Gets or sets the type of security for this item.
		/// </summary>
		[ConfigurationProperty("secure", DefaultValue = SecurityType.Secure)]
		public SecurityType Secure {
			get { return (SecurityType)this["secure"]; }
			set { this["secure"] = value; }
		}

		#endregion

	}

	/// <summary>
	/// Represents a collection of ItemSetting objects.
	/// </summary>
	public abstract class ItemSettingCollection : ConfigurationElementCollection {

		/// <summary>
		/// Returns the index of a specified item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns the index of the item.</returns>
		public int IndexOf(ItemSetting item) {
			if (item != null)
				return BaseIndexOf(item);
			else
				return -1;
		}

		/// <summary>
		/// Returns the index of an item with the specified path in the collection.
		/// </summary>
		/// <param name="path">The path of the item to find.</param>
		/// <returns>Returns the index of the item with the path.</returns>
		public int IndexOf(string path) {
			if (path == null)
				throw new ArgumentNullException("path");
			else
				return this.IndexOf((ItemSetting)BaseGet(path.ToLower(CultureInfo.InvariantCulture)));
		}

	}

}
