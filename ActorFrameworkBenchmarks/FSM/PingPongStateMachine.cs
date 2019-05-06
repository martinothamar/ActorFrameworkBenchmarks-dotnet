using Stateless;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks.FSM
{
    public class PingPongStateMachine
    {
        enum Trigger
        {
            Activate,
            Ping,
            Deactivate
        }

        enum State
        {
            Inactive,
            Active,
            HandlingPing
        }

        private readonly string _id;
        private readonly StateMachine<State, Trigger> _machine;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _pingTrigger;

        private State _state;

        public PingPongStateMachine(string id)
        {
            _id = id;
            _state = State.Inactive;

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);
            _pingTrigger = _machine.SetTriggerParameters<string>(Trigger.Ping);

            _machine.Configure(State.Inactive)
                .OnEntryFromAsync(Trigger.Deactivate, () => OnDeactivateAsync())
                .Permit(Trigger.Activate, State.Active);

            _machine.Configure(State.Active)
                .OnEntryAsync(()  => OnActivateAsync())
                .Permit(Trigger.Ping, State.HandlingPing);
        }

        public Task OnActivateAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnDeactivateAsync()
        {
            return Task.CompletedTask;
        }

        public Task Ping(string ping)
        {
            return Task.FromResult("pong");
        }
    }
}
