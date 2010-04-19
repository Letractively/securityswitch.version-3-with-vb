using System;
using System.Configuration;
using System.Globalization;

namespace SecureSwitch.Configuration {

	/// <summary>
	/// Represents an file entry in the &lt;secureSwitch&gt;
	/// configuration section.
	/// </summary>
	public class FileSetting : ItemSetting {

		#region Constructors

		/// <summary>
		/// Creates an instance of FileSetting.
		/// </summary>
		public FileSetting()
			: base() {
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the file.</param>
		public FileSetting(string path)
			: base(path) {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the file.</param>
		/// <param name="secure">The type of security for the file.</param>
		public FileSetting(string path, SecurityType secure)
			: base(path, secure) {
		}

		#endregion

		/// <summary>
		/// Gets or sets the path of this file setting.
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true, IsKey = true), RegexStringValidator(@"^(?:|[\w\-][\w\.\-,]*(?:/[\w\.\-,]+)*)$")]
		public override string Path {
			get { return base.Path; }
			set { base.Path = value; }
		}

	}

	/// <summary>
	/// Represents a collection of FileSetting objects.
	/// </summary>
	public class FileSettingCollection : ItemSettingCollection {

		/// <summary>
		/// Gets the element name for this collection.
		/// </summary>
		protected override string ElementName {
			get { return "files"; }
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
		/// <returns>The FileSetting located at the specified index.</returns>
		public FileSetting this[int index] {
			get { return (FileSetting)BaseGet(index); }
		}

		/// <summary>
		/// Gets the element with the specified path.
		/// </summary>
		/// <param name="path">The path of the element to retrieve.</param>
		/// <returns>The FileSetting with the specified path.</returns>
		public new FileSetting this[string path] {
			get {
				if (path == null)
					throw new ArgumentNullException("path");
				else
					return (FileSetting)BaseGet(path.ToLower(CultureInfo.InvariantCulture));
			}
		}

		#region Collection Methods

		/// <summary>
		/// Adds a FileSetting to the collection.
		/// </summary>
		/// <param name="fileSetting">An initialized FileSetting instance.</param>
		public void Add(FileSetting fileSetting) {
			BaseAdd(fileSetting);
		}

		/// <summary>
		/// Clears all file entries from the collection.
		/// </summary>
		public void Clear() {
			BaseClear();
		}

		/// <summary>
		/// Removes the specified FileSetting from the collection, if it exists.
		/// </summary>
		/// <param name="fileSetting">A FileSetting to remove.</param>
		public void Remove(FileSetting fileSetting) {
			int Index = base.IndexOf(fileSetting);
			if (Index >= 0)
				BaseRemoveAt(Index);
		}

		/// <summary>
		/// Removes a FileSetting from the collection with a matching path as specified.
		/// </summary>
		/// <param name="path">The path of a FileSetting to remove.</param>
		public void Remove(string path) {
			int Index = base.IndexOf(path);
			if (Index >= 0)
				BaseRemoveAt(Index);
		}

		/// <summary>
		/// Removes the FileSetting from the collection at the specified index.
		/// </summary>
		/// <param name="index">The index of the FileSetting to remove.</param>
		public void RemoveAt(int index) {
			BaseRemoveAt(index);
		}

		#endregion

		/// <summary>
		/// Creates a new element for this collection.
		/// </summary>
		/// <returns>A new instance of FileSetting.</returns>
		protected override ConfigurationElement CreateNewElement() {
			return new FileSetting();
		}

		/// <summary>
		/// Gets the key for the specified element.
		/// </summary>
		/// <param name="element">An element to get a key for.</param>
		/// <returns>A string containing the Path of the FileSetting.</returns>
		protected override object GetElementKey(ConfigurationElement element) {
			if (element != null)
				return ((FileSetting)element).Path.ToLower(CultureInfo.InvariantCulture);
			else
				return null;
		}

	}

}
