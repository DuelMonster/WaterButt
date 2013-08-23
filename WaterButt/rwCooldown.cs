
using System.Runtime.Serialization;
namespace WaterButt
{
	public class RainwaveCooldownGroup
	{
		public RainwaveCooldownGroup() { }
		public RainwaveCooldownGroup(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

		#region CooldownGroup Variables:

		/// <summary>The RainwaveChannel the listener belongs to.</summary>
		public RainwaveChannel Channel { get; set; }

		/// <summary>The URL to the listeners avatar image.</summary>
		[DataMember(Name = "genre_name")]
		public string sName { get; set; }

		/// <summary>The ID of the listener.</summary>
		[DataMember(Name = "genre_id")]
		public int iID { get; set; }

		#endregion
	}
}
