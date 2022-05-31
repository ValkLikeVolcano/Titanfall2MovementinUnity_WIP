using UnityEngine;

namespace N1C_Movement
{
	/// <summary>
	///     A basic state machine that handles multiple states (also is hierarchical)
	/// </summary>
	public abstract class StateMachine<T> where T : StateMachine<T>
	{
		/// <summary>
		///     sets the current active state of the state machine
		/// </summary>
		public void SetState(State<T> newState)
		{
			if (State == newState) return;
			// else

			State?.Deactivate();

			State = newState;

			State?.Activate();
		}

		public string GetEditorDescription() =>
			State.GetEditorDescription();

		public State<T> State { get; private set; }
	}

	// todo: clean up states where states should have implemented enter, exit (instead of letting state machine directly call handlers)

	/// <summary>
	///     A basic abstract state class that contains handlers as callbacks for behaviours
	/// </summary>
	public abstract class State<T> where T : StateMachine<T>
	{
		protected State(T stateMachine) => this.stateMachine = stateMachine;

		public virtual void Activate()
		{
			if (_active) return;
			// else

			_active = true;
			ActivateHandler();
		}

		public virtual void Deactivate()
		{
			if (!_active) return;

			// else
			_active = false;
			DeactivateHandler();
		}

		public virtual void Update() { }
		public virtual void BeforeCharacterUpdate(float deltaTime) { }

		public virtual void UpdateVelocity(ref Vector3 velocity, float deltaTime) { }

		public virtual string GetEditorDescription() => "== NOT IMPLEMENTED ==";

		protected readonly T stateMachine;

		bool _active;

		protected virtual void ActivateHandler() { }
		protected virtual void DeactivateHandler() { }
	}

	/// <summary>
	///     A basic root state that handles multiple sub states
	/// </summary>
	public abstract class RootState<T> : State<T> where T : StateMachine<T>
	{
		protected RootState(T stateMachine) : base(stateMachine)
		{
		}

		public override void Activate()
		{
			// parent activates before child
			base.Activate();
			State?.Activate();
		}

		public override void Deactivate()
		{
			// child deactivates before root
			State?.Deactivate();
			base.Deactivate();
		}

		public sealed override void Update()
		{
			UpdateRoot();
			State?.Update();
		}

		public sealed override void BeforeCharacterUpdate(float deltaTime)
		{
			BeforeCharacterUpdateRoot(deltaTime);
			State?.BeforeCharacterUpdate(deltaTime);
		}

		public sealed override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			UpdateVelocityRoot(ref velocity, deltaTime);
			State?.UpdateVelocity(ref velocity, deltaTime);
		}

		public void SetChildState(State<T> state)
		{
			if (State == state) return;
			// else

			State?.Deactivate();

			State = state;

			State?.Activate();
		}

		public sealed override string GetEditorDescription() =>
			$"[Root]{GetRootDescription()}\n\t{State?.GetEditorDescription()}";

		protected virtual string GetRootDescription() => "== ROOT NOT IMPLEMENTED ==";
		
		protected State<T> State { get; private set; }

		protected virtual void UpdateRoot() { }

		protected virtual void BeforeCharacterUpdateRoot(float deltaTime) { }

		protected virtual void UpdateVelocityRoot(ref Vector3 velocity, float deltaTime) { }
	}
}
