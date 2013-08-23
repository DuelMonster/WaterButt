using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WaterButt
{
	/// <summary>A RainwaveListener object represents a radio listener.</summary>
	public class RainwaveListener
	{
		public RainwaveListener() { }
		public RainwaveListener(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

		#region Listener Raw Data:

		private dynamic _oJSON = null;
		private dynamic oJSON
		{
			get
			{
				if (_oJSON == null)
				{
					dynamic oArgs = new DynamicDictionary();
					oArgs.listener_uid = iID;

					string sJSON = Channel.Client.Call(string.Format("async/{0}/listener_detail", Channel.iID), oArgs);
					if (!sJSON.ToLower().Contains("user does not exist"))
						_oJSON = Channel.Client.JSON.ToDynamic(sJSON).listener_detail;
					else
						throw new KeyNotFoundException("A RainwaveListener for the given listener ID does not exist.");
				}
				return _oJSON;
			}
		}

		#endregion

		#region Listener Collections:

		//[DataMember(Name = "user_topalbums")]
		//public dynamic _raw_TopAlbums = null;
		/// <summary>A list of the ten RainwaveAlbum objects that the listener has given the highest rating to.</summary>
		public List<RainwaveAlbum> TopAlbums = new List<RainwaveAlbum>();

		#endregion

		#region Listener Variables:

		/// <summary>The RainwaveChannel the listener belongs to.</summary>
		public RainwaveChannel Channel { get; set; }

		/// <summary>The URL to the listeners avatar image.</summary>
		[DataMember(Name = "user_avatar")]
		public string sAvatar { get { return oJSON.user_avatar; } }

		/// <summary>The ID of the listener.</summary>
		[DataMember(Name = "user_id")]
		public int iID { get; set; }

		/// <summary>The number of requests made by the listener that lost their election.</summary>
		[DataMember(Name = "radio_losingrequests")]
		public int iLosingRequests { get { return oJSON.radio_losingrequests; } }

		/// <summary>The number of votes the listeners has given to a song that lost an election.</summary>
		[DataMember(Name = "radio_losingvotes")]
		public int iLosingVotes { get { return oJSON.radio_losingvotes; } }

		/// <summary>The name of the listener.</summary>
		[DataMember(Name = "username")]
		public string sName { get; set; }

		/// <summary>The total number of times the listener changed a song rating.</summary>
		[DataMember(Name = "radio_totalmindchange")]
		public int iTotalMindChanges { get { return oJSON.radio_totalmindchange; } }

		/// <summary>The total number of songs the listener has rated.</summary>
		[DataMember(Name = "radio_totalratings")]
		public int iTotalRatings { get { return oJSON.radio_totalratings; } }

		/// <summary>The number of votes the listener has cast in the last two weeks.</summary>
		[DataMember(Name = "radio_2wkvotes")]
		public int iVotes { get; set; }

		/// <summary>The number of requests made by the listener that won their election.</summary>
		[DataMember(Name = "radio_winningrequests")]
		public int iWinningRequests { get { return oJSON.radio_winningrequests; } }

		/// <summary>The number of votes the listener has given to a song that won an election.</summary>
		[DataMember(Name = "radio_winningvotes")]
		public int iWinningVotes { get { return oJSON.radio_winningvotes; } }

		#endregion
	}

	/// <summary>A RainwaveRequestingUser object represents a radio listener the is in the Request queue.</summary>
	public class RainwaveRequestingUser
	{
		public RainwaveRequestingUser() { }
		public RainwaveRequestingUser(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

		#region RequestingUser Variables:

		/// <summary>The RainwaveChannel the listener belongs to.</summary>
		public RainwaveChannel Channel { get; set; }

		[DataMember(Name = "request_expires_at")]
		public DateTime dtExpiresAt { get; set; }

		[DataMember(Name = "request_username")]
		public string sUsername { get; set; }

		[DataMember(Name = "request_tunedin_expiry")]
		public DateTime dtTunedInExpiry { get; set; }

		[DataMember(Name = "request_user_id")]
		public int iUserID { get; set; }

		[DataMember(Name = "request_id")]
		public int iID { get; set; }

		#endregion
	}
}
