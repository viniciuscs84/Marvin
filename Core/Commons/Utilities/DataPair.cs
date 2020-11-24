using System.Collections.Generic;
using System.Linq;

namespace Marvin.Commons.Utilities
{
    /// <summary>
    /// DataPair class Utility. Represents a Key / Value string pair
    /// </summary>
    public class DataPair
    {
        /// <summary>
        /// Get o set the pair key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Get or set the pair value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// DataPair Collection
    /// </summary>
    public class DataPairCollection : List<DataPair>
    {
        /// <summary>
        /// Get DataPair object by the key
        /// </summary>
        /// <param name="key">The key of DataPair</param>
        /// <returns>DataPair object</returns>
        public DataPair GetDataPair(string key)
        {
            return this.FirstOrDefault(p => p.Key == key);
        }

        /// <summary>
        /// Get DataPair value by the key
        /// </summary>
        /// <param name="key">The key of DataPair</param>
        /// <returns>DataPair value</returns>
        public string GetValue(string key)
        {
            DataPair pairData = GetDataPair(key);
            return pairData != null ? pairData.Value : null;
        }
    }
}
