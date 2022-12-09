namespace ProjectDeception.Exceptions
{
	/// <summary>
	/// Thrown when the requesting player isn't in a room
	/// </summary>
	[Serializable]
	public class NoRoomJoinedException : Exception
	{
		public NoRoomJoinedException() { }
		public NoRoomJoinedException(string message) : base(message) { }
		public NoRoomJoinedException(string message, Exception inner) : base(message, inner) { }
		protected NoRoomJoinedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
