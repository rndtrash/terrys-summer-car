namespace TSC
{
	public struct PosRot
	{
		public PosRot( Vector3 position, Vector3 rotation )
		{
			Position = position;
			Rotation = rotation;
		}

		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
	}

	public struct TypePosRot
	{
		public string Type { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
	}

	public struct Cube
	{
		public Vector3 First { get; set; }
		public Vector3 Second { get; set; }
	}
}
