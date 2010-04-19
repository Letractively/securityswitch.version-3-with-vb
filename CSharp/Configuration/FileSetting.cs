using System;

namespace SecureSwitch.Configuration {

	/// <summary>
	/// The FileSetting class represents an file entry in the &lt;secureSwitch&gt;
	/// configuration section.
	/// </summary>
	public class FileSetting : ItemSetting {

		/// <summary>
		/// Creates an instance with default values.
		/// </summary>
		public FileSetting()
			: base() {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="ignore">A flag to ignore security for the directory or file.</param>
		public FileSetting(string path, SecurityType secure)
			: base(path, secure) {
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		public FileSetting(string path)
			: base(path) {
		}

	}

	/// <summary>
	/// The FileSettingCollection class houses a collection of FileSetting instances.
	/// </summary>
	public class FileSettingCollection : ItemSettingCollection {

		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public FileSettingCollection()
			: base() {
		}

		/// <summary>
		/// Indexer for the collection.
		/// </summary>
		public FileSetting this[int index] {
			get { return (FileSetting)List[index]; }
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(FileSetting item) {
			return List.Add(item);
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The index to insert the item at.</param>
		/// <param name="item">The item to insert.</param>
		public void Insert(int index, FileSetting item) {
			List.Insert(index, item);
		}

		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(FileSetting item) {
			List.Remove(item);
		}

	}

}
