using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WaterButt
{
	/// <summary>
	/// A RainwaveSchedule object represents an event on a channel.
	/// Note:
	///		You should not instantiate an object of this class directly, but rather obtain one from RainwaveChannel.schedule_current, RainwaveChannel.schedule_next, or RainwaveChannel.schedule_history.
	/// </summary>
	public class RainwaveSchedule
	{
		public RainwaveSchedule() { }
		public RainwaveSchedule(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

		#region Schedule Variables:

		/// <summary>The RainwaveChannel object the RainwaveSchedule belongs to.</summary>
		public RainwaveChannel Channel = null;

		[DataMember(Name = "sched_actualtime")]
		public long lActualTime = 0;
		/// <summary>The actual time of the RainwaveSchedule object in UTC time.</summary>
		public DateTime dtActualTime;

		[DataMember(Name = "sched_endtime")]
		public long lEndTime = 0;
		/// <summary>The end time of the RainwaveSchedule object in UTC time.</summary>
		public DateTime dtEndTime;

		/// <summary>The ID of the RainwaveSchedule object.</summary>
		[DataMember(Name = "sched_id")]
		public int iID { get; set; }

		/// <summary>The Length of the RainwaveSchedule object...???</summary>
		[DataMember(Name = "sched_length")]
		public int iLength { get; set; }

		/// <summary>The Name of the RainwaveSchedule object.</summary>
		[DataMember(Name = "sched_name")]
		public string sName { get; set; }

		/// <summary>The Notes of the RainwaveSchedule object.</summary>
		[DataMember(Name = "sched_notes")]
		public string sNotes { get; set; }

		[DataMember(Name = "sched_starttime")]
		public long lStartTime = 0;
		/// <summary>The start time of the RainwaveSchedule object in UTC time.</summary>
		public DateTime dtStartTime;

		/// <summary>
		/// The Type of RainwaveSchedule object.
		///		0 = RainwaveElection,
		///		4 = RainwaveOneTimePlay
		/// </summary>
		[DataMember(Name = "sched_type")]
		public int iType { get; set; }

		/// <summary> The sched_used...???</summary>
		[DataMember(Name = "sched_used")]
		public int iUsed { get; set; }

		/// <summary> The ID of the RainwaveListener that created the RainwaveSchedule object.</summary>
		[DataMember(Name = "user_id")]
		public int iUserID { get; set; }


		#endregion
	}

	/// <summary>A RainwaveElection object is a subclass of RainwaveSchedule and represents an election event on a channel.</summary>
	public class RainwaveElection : RainwaveSchedule
	{
		public RainwaveElection() { }
		public RainwaveElection(RainwaveChannel p_rwChannel) : base(p_rwChannel) { }

		#region Schedule Collections:

		/// <summary>A list of RainwaveCandidate objects in the election.</summary>
		[DataMember(Name = "song_data")]
		public List<RainwaveCandidate> Candidates { get; set; }

		#endregion
	}

	/// <summary>A RainwaveOneTimePlay object is a subclass of RainwaveSchedule and represents a song added directly to the timeline by a manager.</summary>
	public class RainwaveOneTimePlay : RainwaveSchedule
	{
		public RainwaveOneTimePlay() { }
		public RainwaveOneTimePlay(RainwaveChannel p_rwChannel) : base(p_rwChannel) { }

		#region OneTimePlay Variables:

		/// <summary>The RainwaveListener who added the song to the timeline.</summary>
		[DataMember(Name = "_id")]
		public RainwaveListener AddedBy = null;

		/// <summary>The RainwaveSong for the event.</summary>
		[DataMember(Name = "_id")]
		public RainwaveSong Song = null;

		#endregion
	}
}
