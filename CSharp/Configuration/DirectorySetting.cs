using System;
using System.Configuration;
using System.Globalization;

namespace SecuritySwitch.Configuration {

	/// <summary>
	/// Represents a directory entry in the &lt;securitySwitch&gt;
	/// configuration section.
	/// </summary>
	public class DirectorySetting : ItemSetting {

		#region Constructors

		/// <summary>
		/// Creates an instance of DirectorySetting.
		/// </summary>
		public DirectorySetting()
			: base() {
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory.</param>
		public DirectorySetting(string path)
			: base(path) {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory.</param>
		/// <param name="secure">The type of security for the directory.</param>
		public DirectorySetting(string path, SecurityType secure)
			: base(path, secure) {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="secure">The type of security for the directory.</param>
		/// <param name="recurse">A flag indicating whether or not to recurse this directory 
		/// when evaluating security.</param>
		public DirectorySetting(string path, SecurityType secure, bool recurse)
			: this(path, secure) {
			Recurse = recurse;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the path of this directory setting.
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true, IsKey = true), RegexStringValidator(@"^(?:|/|[\w\-][\w\.\-,]*(?:/[\w\.\-]+)*/?)$")]
		public override string Path {
			get { return base.Path; }
			set { base.Path = CleanPath(value); }
		}

		/// <summary>
		/// Gets or sets a flag indicating whether or not to include all files in any sub-directories 
		/// when evaluating a request.
		/// </summary>
		[ConfigurationProperty("recurse", DefaultValue = false)]
		public bool Recurse {
			get { return (bool)this["recurse"]; }
			set { this["recurse"] = value; }
		}

		#endregion

		/// <summary>
		/// Overriden to "clean-up" any inconsistent, yet allowed, input.
		/// </summary>
		protected override void PostDeserialize() {
			base.PostDeserialize();
			this["path"] = CleanPath(Path);
		}

		/// <summary>
		/// Cleans the specified path as needed.
		/// </summary>
		/// <param name="path">The path to clean.</param>
		/// <returns>A string containing the cleaned path value.</returns>
		protected string CleanPath(string path) {
			// Strip any trailing slash from the path.
			if (path.EndsWith("/"))
				return path.Substring(0, path.Length - 1);
			else
				return path;
		}

	}

	/// <summary>
	/// Represents a collection of DirectorySetting objects.
	/// </summary>
	public class DirectorySettingCollection : ItemSettingCollection {

		/// <summary>
		/// Gets the element name for this collection.
		/// </summary>
		protected override string ElementName {
			get { return "directories"; }
		}

		/// <summary>
		/// Gets a flag indicating an exception should be thrown if a duplicate element 
		/// is added to the collection.
		/// </summary>
		protected override bool ThrowOnDuplicate {
			get { return true; }
		}

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index">The index to retrieve the element from.</param>
		/// <returns>The DirectorySetting located at the specified index.</returns>
		public DirectorySetting this[int index] {
			get { return (DirectorySetting)BaseGet(index); }
		}

		/// <summary>
		/// Gets the element with the specified path.
		/// </summary>
		/// <param name="path">The path of the element to retrieve.</param>
		/// <returns>The DirectorySetting with the specified path.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public new DirectorySetting this[string path] {
			get {
				if (path == null)
					throw new ArgumentNullException("path");
				else
					return (DirectorySetting)BaseGet(path.ToLower(CultureInfo.InvariantCulture));
			}
		}

		#region Collection Methods

		/// <summary>
		/// Adds a DirectorySetting to the collection.
		/// </summary>
		/// <param name="fileSetting">An initialized DirectorySetting instance.</param>
		public void Add(DirectorySetting fileSetting) {
			BaseAdd(fileSetting);
		}

		/// <summary>
		/// Clears all file entries from the collection.
		/// </summary>
		public void Clear() {
			BaseClear();
		}

		/// <summary>
		/// Removes the specified DirectorySetting from the collection, if it exists.
		/// </summary>
		/// <param name="fileSetting">A DirectorySetting to remove.</param>
		public void Remove(DirectorySetting fileSetting) {
			int Index = base.IndexOf(fileSetting);
			if (Index >= 0)
				BaseRemoveAt(Index);
		}

		/// <summary>
		/// Removes a DirectorySetting from the collection with a matching path as specified.
		/// </summary>
		/// <param name="path">The path of a DirectorySetting to remove.</param>
		public void Remove(string path) {
			int Index = base.IndexOf(path);
			if (Index >= 0)
				BaseRemoveAt(Index);
		}

		/// <summary>
		/// Removes the DirectorySetting from the collection at the specified index.
		/// </summary>
		/// <param name="index">The index of the DirectorySetting to remove.</param>
		public void RemoveAt(int index) {
			BaseRemoveAt(index);
		}

		#endregion

		/// <summary>
		/// Creates a new element for this collection.
		/// </summary>
		/// <returns>A new instance of FileSetting.</returns>
		protected override ConfigurationElement CreateNewElement() {
			return new DirectorySetting();
		}

		/// <summary>
		/// Gets the key for the specified element.
		/// </summary>
		/// <param name="element">An element to get a key for.</param>
		/// <returns>A string containing the Path of the DirectorySetting.</returns>
		protected override object GetElementKey(ConfigurationElement element) {
			if (element != null)
				return ((DirectorySetting)element).Path.ToLower(CultureInfo.InvariantCulture);
			else
				return null;
		}

	}

}
