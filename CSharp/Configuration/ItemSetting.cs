using System;
using System.Collections;

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
	/// The ItemSettingComparer class implements the IComparer interface to compare.
	/// </summary>
	public class ItemSettingComparer : IComparer {
		/// <summary>
		/// Initialize an instance of this class.
		/// </summary>
		public ItemSettingComparer() {
		}

		/// <summary>
		/// Compares the two objects as string and ItemSetting or both ItemSetting 
		/// by the Path property.
		/// </summary>
		/// <param name="x">First object to compare.</param>
		/// <param name="y">Second object to compare.</param>
		/// <returns></returns>
		public int Compare(object x, object y) {
			// Check the type of the parameters
			if (!(x is ItemSetting) && !(x is string))
				// Throw an exception for the first argument
				throw new ArgumentException("Parameter must be a ItemSetting or a String.", "x");
			else if (!(y is ItemSetting) && !(y is string))
				// Throw an exception for the second argument
				throw new ArgumentException("Parameter must be a ItemSetting or a String.", "y");

			// Initialize the path variables
			string xPath = string.Empty;
			string yPath = string.Empty;

			// Get the path for x
			if (x is ItemSetting)
				xPath = ((ItemSetting)x).Path;
			else
				xPath = (string)x;

			// Get the path for y
			if (y is ItemSetting)
				yPath = ((ItemSetting)y).Path;
			else
				yPath = (string)y;

			// Compare the paths, ignoring case
			return string.Compare(xPath, yPath, true);
		}
	}


	/// <summary>
	/// The ItemSetting class is the base class that represents entries in the &lt;secureSwitch&gt;
	/// configuration section.
	/// </summary>
	public class ItemSetting {
		// Fields
		private SecurityType secure = SecurityType.Secure;
		private string path = string.Empty;

		/// <summary>
		/// Gets or sets the type of security for this directory or file.
		/// </summary>
		public SecurityType Secure {
			get { return secure; }
			set { secure = value; }
		}

		/// <summary>
		/// Gets or sets the path of this directory or file.
		/// </summary>
		public string Path {
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// Creates an instance of this class.
		/// </summary>
		public ItemSetting() {
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="ignore">A flag to ignore security for the directory or file.</param>
		public ItemSetting(string path, SecurityType secure) {
			// Initialize the path and secure properties
			this.path = Path;
			this.secure = secure;
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		public ItemSetting(string path)
			: this(path, SecurityType.Secure) {
		}

	}

	/// <summary>
	/// The ItemSettingCollection class houses a collection of ItemSetting instances.
	/// </summary>
	public class ItemSettingCollection : CollectionBase {

		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public ItemSettingCollection() {
		}

		/// <summary>
		/// Returns the index of a specified item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns the index of the item.</returns>
		public int IndexOf(ItemSetting item) {
			return List.IndexOf(item);
		}

		/// <summary>
		/// Returns the index of an item with the specified path in the collection.
		/// </summary>
		/// <param name="Path">The path of the item to find.</param>
		/// <returns>Returns the index of the item with the path.</returns>
		public int IndexOf(string path) {
			// Create a comparer for sorting and searching
			ItemSettingComparer Comparer = new ItemSettingComparer();
			InnerList.Sort(Comparer);
			return InnerList.BinarySearch(path, Comparer);
		}

	}

}
