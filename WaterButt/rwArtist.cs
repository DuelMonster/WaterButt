using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WaterButt
{
	/// <summary>
	/// A RainwaveArtist object represents one artist.
	/// Note:
	///		You should not instantiate an object of this class directly, but rather obtain one from RainwaveChannel.Artists or RainwaveSong.Artists.
	/// </summary>
	public class RainwaveArtist
	{
		public RainwaveArtist() { }
		public RainwaveArtist(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

		#region Artist Raw Data:

		private string sJSON = "";
		private dynamic _oJSON = null;
		private dynamic oJSON
		{
			get
			{
				if (_oJSON == null)
				{
					dynamic oArgs = new DynamicDictionary();
					oArgs.artist_id = iID;

					sJSON = Channel.Client.Call(string.Format("async/{0}/artist_detail", Channel.iID), oArgs);
					if (!sJSON.ToLower().Contains("artist id does not exist"))
						_oJSON = Channel.Client.JSON.ToDynamic(sJSON).artist_detail;
					else
						throw new KeyNotFoundException("A RainwaveArtist for the given artist ID does not exist.");
				}
				return _oJSON;
			}
		}

		#endregion

		#region Artist Variables:

		/// <summary>The RainwaveChannel object the artist belongs to.</summary>
		public RainwaveChannel Channel { get; set; }

		/// <summary>The ID of the artist.</summary>
		[DataMember(Name = "artist_id")]
		public int iID { get; set; }

		/// <summary>The name of the artist.</summary>
		[DataMember(Name = "artist_name")]
		public string sName { get; set; }

		/// <summary>The number of songs attributed to the artist.</summary>
		[DataMember(Name = "artist_numsongs")]
		public int iNumSongs { get; set; }

		#endregion

		#region Artist Collections:

		private List<RainwaveSong> _Songs = null;
		/// <summary>A list of RainwaveSong objects attributed to the artist.</summary>
		public List<RainwaveSong> Songs
		{
			get
			{
				if (_Songs == null)
				{
					List<RainwaveSong> tmpSongs = Channel.Client.JSON.ToObject<List<RainwaveSong>>(Channel.Client.JSON.ToJSON(oJSON.songs));

					// Remove all songs that don't belong to the current RainwaqveChannel.
					foreach (RainwaveSong rwSong in tmpSongs)
						if (rwSong.iChannelID == Channel.iID)
						{
							dynamic oArgs = new DynamicDictionary();
							oArgs.album_id = rwSong.iAlbumID;

							string sJSON_tmp = Channel.Client.Call(string.Format("async/{0}/album", Channel.iID), oArgs);
							if (!sJSON_tmp.ToLower().Contains("invalid album id"))
								_Songs = Channel.Client.JSON.ToObject<List<RainwaveSong>>(Channel.Client.JSON.ToJSON(Channel.Client.JSON.ToDynamic(sJSON_tmp).playlist_album.song_data));
							else
								throw new KeyNotFoundException("A RainwaveAlbum for the given album ID does not exist.");

						}
				}
				return _Songs;
			}
		}

		#endregion
	}
}
