using System;

namespace SecureSwitch.Configuration {

	/// <summary>
	/// The DirectorySetting class represents a directory entry in the &lt;secureSwitch&gt;
	/// configuration section.
	/// </summary>
	public class DirectorySetting : ItemSetting {

		// Fields
		private bool recurse = false;

		/// <summary>
		/// Gets or sets a flag indicating whether or not to include all files in any sub-directories 
		/// when evaluating a request.
		/// </summary>
		public bool Recurse {
			get { return recurse; }
			set { recurse = value; }
		}

		/// <summary>
		/// Creates an instance with default values.
		/// </summary>
		public DirectorySetting()
			: base() {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="ignore">A flag to ignore security for the directory or file.</param>
		public DirectorySetting(string path, SecurityType secure, bool recurse)
			: base(path, secure) {
			this.recurse = recurse;
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		public DirectorySetting(string path)
			: base(path) {
		}

	}

	/// <summary>
	/// The DirectorySettingCollection class houses a collection of DirectorySetting instances.
	/// </summary>
	public class DirectorySettingCollection : ItemSettingCollection {

		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public DirectorySettingCollection()
			: base() {
		}

		/// <summary>
		/// Indexer for the collection.
		/// </summary>
		public DirectorySetting this[int index] {
			get { return (DirectorySetting)List[index]; }
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(DirectorySetting item) {
			return List.Add(item);
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The index to insert the item at.</param>
		/// <param name="item">The item to insert.</param>
		public void Insert(int index, DirectorySetting item) {
			List.Insert(index, item);
		}

		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(DirectorySetting item) {
			List.Remove(item);
		}

	}

}
