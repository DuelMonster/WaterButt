using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WaterButt
{
	/// <summary>
	/// A RainwaveChannel object represents one channel on the Rainwave network.
	/// Note:
	///		You should not instantiate an object of this class directly, but rather obtain one from RainwaveClient.channels.
	/// </summary>
	public class RainwaveChannel
	{
		public RainwaveChannel() { }
		public RainwaveChannel(RainwaveClient p_rwClient) { Client = p_rwClient; }

		#region Channel Collections:

		private List<RainwaveAlbum> _Albums = null;
		/// <summary>A list of RainwaveAlbum objects in the playlist of the channel.</summary>
		public List<RainwaveAlbum> Albums
		{
			get
			{
				if (_Albums == null || bIsStale)
				{
					string sJSON = Client.Call(string.Format("async/{0}/all_albums", iID));
					_Albums = Client.JSON.ToObject<List<RainwaveAlbum>>(Client.JSON.ToJSON(Client.JSON.ToDynamic(sJSON).playlist_all_albums));
					// Now that we have the RainwaveAlbum objects we need to attach them to the RainwaveChannel object.
					foreach (RainwaveAlbum rwAlbum in _Albums) rwAlbum.Channel = this;
				}

				return _Albums;
			}
		}

		private List<RainwaveArtist> _Artists = null;
		/// <summary>A list of RainwaveArtist objects in the playlist of the channel.</summary>
		public List<RainwaveArtist> Artists
		{
			get
			{
				if (_Artists == null || bIsStale)
				{
					string sJSON = Client.Call(string.Format("async/{0}/artist_list", iID));
					_Artists = Client.JSON.ToObject<List<RainwaveArtist>>(Client.JSON.ToJSON(Client.JSON.ToDynamic(sJSON).artist_list));
				}

				return _Artists;
			}
		}

		private List<RainwaveListener> _Listeners = null;
		/// <summary>A list of RainwaveListener objects listening to the channel.</summary>
		public List<RainwaveListener> Listeners
		{
			get
			{
				if (_Listeners == null || bIsStale)
				{
					string sJSON = Client.Call(string.Format("async/{0}/listeners_current", iID));
					_Listeners = Client.JSON.ToObject<List<RainwaveListener>>(Client.JSON.ToJSON(Client.JSON.ToDynamic(sJSON).listeners_current.users));
					// Now that we have the RainwaveListeners objects we need to attach them to the RainwaveChannel object.
					foreach (RainwaveListener rwListener in _Listeners) rwListener.Channel = this;
				}

				return _Listeners;
			}
		}

		private List<RainwaveUserRequest> _Requests = null;
		/// <summary>A list of RainwaveUserRequests objects in the request line of the channel.</summary>
		public List<RainwaveUserRequest> Requests
		{
			get
			{
				if (_Requests == null || bIsStale) DoAsyncRequestsGet();
				return _Requests;
			}
		}

		private List<RainwaveRequestingUser> _RequestingUsers = null;
		/// <summary>A list of RainwaveListener objects listening to the channel.</summary>
		public List<RainwaveRequestingUser> RequestingUsers
		{
			get
			{
				if (_RequestingUsers == null || bIsStale) DoAsyncRequestsGet();
				return _RequestingUsers;
			}
		}

		private List<RainwaveSchedule> _Schedule_Current = null;
		/// <summary>The current RainwaveSchedule for the channel.</summary>
		public List<RainwaveSchedule> Schedule_Current
		{
			get
			{
				if (bIsStale) DoAsyncGet();
				return _Schedule_Current;
			}
		}

		private List<RainwaveSchedule> _Schedule_History = null;
		/// <summary>A list of the past RainwaveSchedule objects for the channel.</summary>
		public List<RainwaveSchedule> Schedule_History
		{
			get
			{
				if (bIsStale) DoAsyncGet();
				return _Schedule_History;
			}
		}

		private List<RainwaveSchedule> _Schedule_Next = null;
		/// <summary>A list of the next RainwaveSchedule objects for the channel.</summary>
		public List<RainwaveSchedule> Schedule_Next
		{
			get
			{
				if (bIsStale) DoAsyncGet();
				return _Schedule_Next;
			}
		}

		#endregion

		#region Channel Variables:

		/// <summary>The RainwaveClient object that the channel belongs to.</summary>
		public RainwaveClient Client { get; set; }

		/// <summary>A description of the channel.</summary>
		[DataMember(Name = "description")]
		public string sDescription { get; set; }

		/// <summary>The number of guests listening to the channel.</summary>
		public int iGuestCount
		{
			get
			{
				string sJSON = Client.Call(string.Format("async/{0}/listeners_current", iID));
				return (int)Client.JSON.ToDynamic(sJSON).listeners_current.guests;
			}
		}

		/// <summary>The ID of the channel.</summary>
		[DataMember(Name = "id")]
		public int iID { get; set; }

		/// <summary>The name of the channel.</summary>
		[DataMember(Name = "name")]
		public string sName { get; set; }

		/// <summary>The URL of the OGG stream for the channel.</summary>
		[DataMember(Name = "oggstream")]
		public string sOggStream { get; set; }

		/// <summary>The URL of the MP3 stream for the channel.</summary>
		[DataMember(Name = "stream")]
		public string sStream { get; set; }

		#endregion

		#region Channel Methods:


		/// <summary>
		/// Marks an Album as favourite or not
		/// </summary>
		/// <param name="p_iAlbumID">The ID of the Album to change.</param>
		/// <param name="p_bFav">Boolean value depicting if the Album should be marked as a favourite or not.</param>
		/// <returns>A JSON formatted string.</returns>
		public string favAlbum(int p_iAlbumID, bool p_bFav)
		{
			dynamic oArgs = new DynamicDictionary();
			oArgs.album_id = p_iAlbumID;
			oArgs.fav = p_bFav;

			return Client.Call(string.Format("async/{0}/fav_album", iID), oArgs);
		}

		/// <summary>
		/// Marks an Song as favourite or not
		/// </summary>
		/// <param name="p_iAlbumID">The ID of the Song to change.</param>
		/// <param name="p_bFav">Boolean value depicting if the Song should be marked as a favourite or not.</param>
		/// <returns>A JSON formatted string.</returns>
		public string favSong(int p_iSongID, bool p_bFav)
		{
			dynamic oArgs = new DynamicDictionary();
			oArgs.album_id = p_iSongID;
			oArgs.fav = p_bFav;

			return Client.Call(string.Format("async/{0}/fav_song", iID), oArgs);
		}

		/// <summary>
		/// Raises an IndexError if there is no album with the given ID in the playlist of the channel.
		/// </summary>
		/// <param name="p_iID">The ID of the desired album.</param>
		/// <returns>A RainwaveAlbum for the given album ID.</returns>
		public RainwaveAlbum getAlbumByID(int p_iID)
		{
			RainwaveAlbum rwAlbum = null;
			if (!AlbumExists(p_iID, out rwAlbum))
				throw new KeyNotFoundException("A RainwaveAlbum for the given album ID does not exist.");
			return rwAlbum;
		}

		/// <summary>
		/// Raises an IndexError if there is no album with the given ID in the playlist of the channel.
		/// </summary>
		/// <param name="p_sName">The name of the desired album.</param>
		/// <returns>A RainwaveAlbum for the given album name.</returns>
		public RainwaveAlbum getAlbumByName(string p_sName)
		{
			RainwaveAlbum rwAlbum = null;
			if (!AlbumExists(p_sName, out rwAlbum))
				throw new KeyNotFoundException("A RainwaveAlbum for the given album name does not exist.");
			return rwAlbum;
		}

		/// <summary>
		/// Raises an IndexError if there is no artist with the given ID in the playlist of the channel.
		/// </summary>
		/// <param name="p_iID">The ID of the desired artist.</param>
		/// <returns>A RainwaveArtist for the given artist ID</returns>
		public RainwaveArtist getArtistByID(int p_iID)
		{
			RainwaveArtist rwArtist = null;
			if (!ArtistExists(p_iID, out rwArtist))
				throw new KeyNotFoundException("A RainwaveArtist for the given artist ID does not exist.");
			return rwArtist;
		}

		/// <summary>
		/// Raises an IndexError if there is no listener with the given ID.
		/// </summary>
		/// <param name="p_iID">The ID of the desired listener.</param>
		/// <returns>A RainwaveListener for the given listener ID.</returns>
		public RainwaveListener getListenerByID(int p_iID)
		{
			RainwaveListener rwListener = null;
			if (!ListenerExists(p_iID, out rwListener))
				throw new KeyNotFoundException("A RainwaveListener for the given listener ID does not exist.");
			return rwListener;
		}

		/// <summary>
		/// Raises an IndexError if there is no listener with the given name currently listening to the channel.
		/// </summary>
		/// <param name="p_sName">The name of the desired listener.</param>
		/// <returns>A RainwaveListener for the given given listener name.</returns>
		public RainwaveListener getListenerByName(string p_sName)
		{
			RainwaveListener rwListener = null;
			if (!ListenerExists(p_sName, out rwListener))
				throw new KeyNotFoundException("A RainwaveListener for the given listener name does not exist.");
			return rwListener;
		}

		////start_sync()
		////// Begin syncing the timeline for the channel.

		////stop_sync()
		////// Stop syncing the timeline for the channel.

		public string vote(int p_iElecEntryID)
		{
			dynamic oArgs = new DynamicDictionary();
			oArgs.elec_entry_id = p_iElecEntryID;

			return Client.Call(string.Format("async/{0}/vote", iID), oArgs);
		}

		#endregion

		#region Channel Private Members:

		private bool bIsStale
		{
			get
			{
				bool _bIsStale = false;

				if (_Schedule_Next == null || _Schedule_Next.Count == 0)
					_bIsStale = true;
				else
					_bIsStale = (DateTime.UtcNow > _Schedule_Next[0].dtStartTime.ToUniversalTime());

				return _bIsStale;
			}
		}

		private void DoAsyncGet()
		{
			_Schedule_Current = new List<RainwaveSchedule>();
			_Schedule_History = new List<RainwaveSchedule>();
			_Schedule_Next = new List<RainwaveSchedule>();

			string sJSON = Client.Call(string.Format("async/{0}/get", iID));
			dynamic oJSON = Client.JSON.ToDynamic(sJSON);

			_Schedule_Current.Add(CreateCurrentSchedule(oJSON.sched_current));

			foreach (dynamic o in oJSON.sched_history)
				_Schedule_History.Add(CreateSchedule(o));

			foreach (dynamic o in oJSON.sched_next)
				_Schedule_Next.Add(CreateSchedule(o));
		}

		private RainwaveSchedule CreateCurrentSchedule(dynamic oSchedule)
		{
			RainwaveSchedule rwSchedule = null;

			if (oSchedule.sched_type == 4)
			{
				RainwaveOneTimePlay rwOTPSchedule = new RainwaveOneTimePlay(this);
				rwSchedule = rwOTPSchedule;
			}
			else
			{
				RainwaveElection rwElecSchedule = new RainwaveElection(this);
				rwElecSchedule.Candidates = Client.JSON.ToObject<List<RainwaveCandidate>>(Client.JSON.ToJSON(oSchedule.song_data));
				rwSchedule = rwElecSchedule;
			}

			rwSchedule.lActualTime = oSchedule.sched_actualtime;
			rwSchedule.lEndTime = oSchedule.sched_endtime;
			rwSchedule.iID = Convert.ToInt32(oSchedule.sched_id);
			rwSchedule.iLength = Convert.ToInt32(oSchedule.sched_length);
			rwSchedule.sName = oSchedule.sched_name;
			rwSchedule.sNotes = oSchedule.sched_notes;
			rwSchedule.lStartTime = oSchedule.sched_starttime;
			rwSchedule.iType = Convert.ToInt32(oSchedule.sched_type);
			rwSchedule.iUsed = Convert.ToInt32(oSchedule.sched_used);
			rwSchedule.iUserID = Convert.ToInt32(oSchedule.user_id);

			return rwSchedule;
		}
		private RainwaveSchedule CreateSchedule(dynamic oSchedule)
		{
			RainwaveSchedule rwSchedule = null;

			if (oSchedule["sched_type"] == 4)
			{
				RainwaveOneTimePlay rwOTPSchedule = new RainwaveOneTimePlay(this);
				rwSchedule = rwOTPSchedule;
			}
			else
			{
				RainwaveElection rwElecSchedule = new RainwaveElection(this);
				rwElecSchedule.Candidates = Client.JSON.ToObject<List<RainwaveCandidate>>(Client.JSON.ToJSON(oSchedule["song_data"]));
				rwSchedule = rwElecSchedule;
			}

			rwSchedule.lActualTime = Convert.ToInt64(oSchedule["sched_actualtime"]);
			rwSchedule.iID = Convert.ToInt32(oSchedule["sched_id"]);
			rwSchedule.iLength = Convert.ToInt32(oSchedule["sched_length"]);
			rwSchedule.sName = Convert.ToString(oSchedule["sched_name"]);
			rwSchedule.sNotes = Convert.ToString(oSchedule["sched_notes"]);
			rwSchedule.lStartTime = Convert.ToInt64(oSchedule["sched_starttime"]);
			rwSchedule.iType = Convert.ToInt32(oSchedule["sched_type"]);
			rwSchedule.iUsed = Convert.ToInt32(oSchedule["sched_used"]);
			rwSchedule.iUserID = Convert.ToInt32(oSchedule["user_id"]);

			return rwSchedule;
		}

		private void DoAsyncRequestsGet()
		{
			string sJSON = Client.Call(string.Format("async/{0}/requests_get", iID));
			dynamic oJSON = Client.JSON.ToDynamic(sJSON);

			_Requests = Client.JSON.ToObject<List<RainwaveUserRequest>>(Client.JSON.ToJSON(oJSON.requests_user));
			_RequestingUsers = Client.JSON.ToObject<List<RainwaveRequestingUser>>(Client.JSON.ToJSON(oJSON.requests_all));

			// Now that we have the RainwaveUserRequest and RainwaveRequestingUser objects, we need to attach them to the RainwaveChannel object.
			//foreach (RainwaveUserRequest rwUR in _Requests) rwUR.Channel = this;
			foreach (RainwaveRequestingUser rwRU in _RequestingUsers) rwRU.Channel = this;
		}

		private bool ListenerExists(int p_iID, out RainwaveListener rwListener)
		{
			rwListener = null;
			foreach (RainwaveListener rwL in Listeners)
				if (rwL.iID == p_iID)
				{
					rwListener = rwL;
					return true;
				}
			return false;
		}
		private bool ListenerExists(string p_sName, out RainwaveListener rwListener)
		{
			rwListener = null;
			foreach (RainwaveListener rwL in Listeners)
				if (rwL.sName == p_sName)
				{
					rwListener = rwL;
					return true;
				}
			return false;
		}

		private bool AlbumExists(int p_iID, out RainwaveAlbum rwAlbum)
		{
			rwAlbum = null;
			foreach (RainwaveAlbum rwA in Albums)
				if (rwA.iID == p_iID)
				{
					rwAlbum = rwA;
					return true;
				}
			return false;
		}
		private bool AlbumExists(string p_sName, out  RainwaveAlbum rwAlbum)
		{
			rwAlbum = null;
			foreach (RainwaveAlbum rwA in Albums)
				if (rwA.sName == p_sName)
				{
					rwAlbum = rwA;
					return true;
				}
			return false;
		}

		private bool ArtistExists(int p_iID, out RainwaveArtist rwArtist)
		{
			rwArtist = null;
			foreach (RainwaveArtist rwA in Artists)
				if (rwA.iID == p_iID)
				{
					rwArtist = rwA;
					return true;
				}
			return false;
		}

		#endregion
	}
}
