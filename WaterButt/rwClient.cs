using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WaterButt
{
	/// <summary>
	/// A RainwaveClient object provides a simple interface to the Rainwave API (see http://rainwave.cc/api/ for details about the API).
	/// Parameters:	
	///     iUserID – the User ID to use when communicating with the API.
	///     sAPIKey – the API key to use when communicating with the API.
	/// </summary>
	public class RainwaveClient
	{
		public RainwaveClient(int p_iUserID, string p_sAPIKey)
		{
			iUserID = p_iUserID;
			sAPIKey = p_sAPIKey;
		}

		#region Client Variables:

		public fastJSON.JSON JSON = fastJSON.JSON.Instance;

		/// <summary>The URL upon which all API calls are based.</summary>
		public const string sBaseURL = "http://rainwave.cc/";

		/// <summary>The User ID to use when communicating with the API. Find your User ID at: http://rainwave.cc/auth/. </summary>
		private int iUserID = 0;

		/// <summary>The API key to use when communicating with the API. Find your API key at: http://rainwave.cc/auth/. </summary>
		private string sAPIKey = "";

		/// <summary>The WebRequest object used by the Call method for talking to the Rainwave API.</summary>
		private WebRequest wrAPI = null;

		#endregion

		#region Client Collections:

		private List<RainwaveChannel> _Channels = null;
		/// <summary>A list of RainwaveChannel objects associated with this RainwaveClient object.</summary>
		public List<RainwaveChannel> Channels
		{
			get
			{
				if (_Channels == null)
				{
					string sJSON = "";

					if (iUserID > 0 && !string.IsNullOrEmpty(sAPIKey))
						sJSON = Call("async/1/stations_user");
					else
						sJSON = Call("async/1/stations");

					_Channels = JSON.ToObject<List<RainwaveChannel>>(JSON.ToJSON(JSON.ToDynamic(sJSON).stations));

					// Now that we have the RainwaveChannel objects, we need to attach them to the RainwaveClient object.
					// There isn't currently a way to do it automatically using fastJSON, but I may look into modifying it...
					foreach (RainwaveChannel rwC in _Channels) rwC.Client = this;
				}

				return _Channels;
			}
		}

		#endregion

		#region Client Methods:

		/// <summary>
		/// Make a direct call to the API if you know the necessary path and arguments.
		/// Usage:
		///     RainwaveClient rw = new RainwaveClient(%user_id%, "%api_key%");
		///     rw.call("async/%Channel_id%/album", {{"album_id", %album_id%}});
		/// </summary>
		/// <param name="p_sPath">The URL path of the API method to call.</param>
		/// <param name="p_saArgs">Any arguments required by the API method.</param>
		/// <returns>A JSON formatted string:  "{'playlist_album': {'album_name': 'Wild Arms', ...}}"</returns>
		public string Call(string p_sPath, DynamicDictionary p_Args = null, bool p_bBeautify = false)
		{
			wrAPI = WebRequest.Create(Path.Combine(sBaseURL, p_sPath));

			if (!(p_sPath.ToLower().EndsWith("/get") ||
				  p_sPath.ToLower().EndsWith("/stations")))
			{
				string sArgs = sArgs = "user_id=" + iUserID.ToString() + "&key=" + sAPIKey; ;
				if (p_Args != null)
				{
					if (p_Args.GetVaule("user_id") != null && p_Args.GetVaule("key") != null)
						sArgs = "";
					foreach (string sName in p_Args.GetMemberNames())
						sArgs += (sArgs == "" ? "" : "&") + sName + "=" + p_Args.GetVaule(sName).ToString();
				}
				byte[] oDataStream = Encoding.UTF8.GetBytes(sArgs);
				wrAPI.Method = "POST";
				wrAPI.ContentType = "application/x-www-form-urlencoded";
				wrAPI.ContentLength = oDataStream.Length;
				Stream oRequestStream = wrAPI.GetRequestStream();
				// Send the data.
				oRequestStream.Write(oDataStream, 0, oDataStream.Length);
				oRequestStream.Close();
			}

			string sJSON = (new StreamReader(wrAPI.GetResponse().GetResponseStream())).ReadToEnd();
			if (p_bBeautify)
				return JSON.Beautify(sJSON);
			else
				return sJSON;
		}

		#endregion
	}

	/// <summary>The DynamicDictionary class is derived from DynamicObject.</summary>
	public class DynamicDictionary : DynamicObject
	{
		// The inner dictionary.
		Dictionary<string, object> myDictionary = new Dictionary<string, object>();

		/// <summary>
		/// This property returns the number of elements in the inner dictionary.
		/// </summary>
		public int Count { get { return myDictionary.Count; } }

		/// <summary>
		/// Use this method only when you need to access an entry using the name stored as string.
		/// </summary>
		/// <param name="p_sName">The name of the DynamicDictionary entry.</param>
		/// <returns>The Value of the DynamicDictionary entry.</returns>
		public IEnumerable<string> GetMemberNames()
		{
			return myDictionary.Keys.ToArray();
		}

		/// <summary>
		/// Use this method only when you need to access an entry using the name stored as string.
		/// </summary>
		/// <param name="p_sName">The name of the DynamicDictionary entry.</param>
		/// <returns>The Value of the DynamicDictionary entry.</returns>
		public object GetVaule(string p_sName)
		{
			object oResult = null;
			myDictionary.TryGetValue(p_sName, out oResult);
			return oResult;
		}

		// If you try to get a value of a property not defined in the class, this method 
		// is called.
		public override bool TryGetMember(GetMemberBinder p_Binder, out object p_oResult)
		{
			// Converting the property name to lowercase so that property names become 
			// case-insensitive.
			string sName = p_Binder.Name.ToLower();

			// If the property name is found in a dictionary,set the result parameter to 
			// the property value and return true. Otherwise, return false.
			return myDictionary.TryGetValue(sName, out p_oResult);
		}

		// If you try to set a value of a property that is not defined in the class, this
		// method is called.
		public override bool TrySetMember(SetMemberBinder p_Binder, object p_oValue)
		{
			// Converting the property name to lowercase so that property names become
			// case-insensitive.
			myDictionary[p_Binder.Name.ToLower()] = p_oValue;

			// You can always add a value to a dictionary, so this method always returns
			// true.
			return true;
		}
	}
}
