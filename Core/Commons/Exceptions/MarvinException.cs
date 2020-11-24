using System;

namespace Marvin.Commons.Exceptions
{
    /// <summary>
    /// Represent errors that occurr during a Marvin Application execution.
    /// </summary>
    public class EssentialsException : Exception
    {
        /// <summary>
        /// Get error code
        /// </summary>
        public string Code
        {
            /* Use Data dictionary to store the property value */
            protected set
            {
                Data.Remove("Code");
                if (value != null)
                    Data.Add("Code", value);
            }
            get
            {
                if (Data.Contains("Code"))
                    return Data["Code"].ToString();
                return null;
            }
        }

        /// <summary>
        /// Get friendly message for GUI
        /// </summary>
        public string FriendlyMessage
        {
            /* Use Data dictionary to store the property value */
            protected set
            {
                Data.Remove("FriendlyMessage");
                if (value != null)
                    Data.Add("FriendlyMessage", value);
            }
            get
            {
                if (Data.Contains("FriendlyMessage"))
                    return Data["FriendlyMessage"].ToString();
                return null;
            }
        }

        /// <summary>
        /// Get moment that the error occur
        /// </summary>
        public DateTime Date
        {
            /* Use Data dictionary to store the property value */
            protected set
            {
                Data.Remove("Date");
                if (value != DateTime.MinValue)
                    Data.Add("Date", value);
            }
            get
            {
                if (!Data.Contains("Date"))
                    Data.Add("Date", DateTime.Now);
                return Convert.ToDateTime(Data["Date"]);
            }
        }

        /// <summary>
        /// Get manipulated model object
        /// </summary>
        public object ModelObject
        {
            /* Use Data dictionary to store the property value */
            protected set
            {
                Data.Remove("ModelObject");
                if (value != null)
                    Data.Add("ModelObject", value);
            }
            get
            {
                if (Data.Contains("ModelObject"))
                    return Data["ModelObject"];
                return null;
            }
        }

        /// <summary>
        /// Create a new Instance.
        /// </summary>
        /// <param name="message">General error message</param>
        /// <param name="code">Error code</param>
        /// <param name="friendlyMessage">Friendly error message for GUI</param>
        /// <param name="model">Manipulated model object</param>
        /// <param name="innerException">Inner exception</param>
        public EssentialsException(string message, string code = null, string friendlyMessage = null, object model = null, Exception innerException = null)
            : base(message, innerException)
        {
            Code = code;
            /* If friendly message isn't passed, use general message */
            FriendlyMessage = string.IsNullOrEmpty(friendlyMessage) ? message + "!" : friendlyMessage;
            Date = DateTime.Now;
            ModelObject = model;
        }
    }
}
