namespace N1C_Movement
{
	public class CharacterWallState : RootState<CharacterStateMachine>
	{
		public CharacterWallState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		protected override string GetRootDescription() => "Wall State";
	}
}
