
using UnityEngine;

namespace N1C_Movement
{
	public abstract class StateMachine<T> where T : StateMachine<T>
	{
		public void SetState(State<T> newState)
		{
			if (State == newState) return;
			// else

			State?.ExitHandler();

			State = newState;

			State?.EnteredHandler();
		}

		public State<T> State { get; private set; }
	}

	public abstract class State<T> where T : StateMachine<T>
	{
		protected State(T stateMachine) => this.stateMachine = stateMachine;

		public virtual void EnteredHandler() { }
		public virtual void Update() { }

		public virtual void ExitHandler() { }
		public virtual void BeforeCharacterUpdate(float deltaTime) { }

		public virtual void UpdateVelocity(ref Vector3 velocity, float deltaTime) { }

		protected readonly T stateMachine;
	}

	public abstract class RootState<T> : State<T> where T : StateMachine<T>
	{
		protected RootState(T stateMachine) : base(stateMachine)
		{
		}

		public override void EnteredHandler()
		{
			_active = true;

			State?.EnteredHandler();
		}

		public override void Update()
		{
			State?.Update();
		}

		public override void ExitHandler()
		{
			_active = false;

			State?.ExitHandler();
		}

		public override void BeforeCharacterUpdate(float deltaTime) { }

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			State?.UpdateVelocity(ref velocity, deltaTime);
		}

		public void SetState(State<T> state)
		{
			if (State == state) return;
			// else

			State?.ExitHandler();

			State = state;

			if (_active)
				State?.EnteredHandler();
		}

		protected State<T> State { get; private set; }

		bool _active;
	}
}
