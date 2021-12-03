using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace CTRX
{
    [Serializable]
    public class CTaper : CPost
    {
        private CTaperCycle taperCycle;

        public CTaper()
        {
            this.TaperCycle = new CTaperCycle(0); ;
        }

        public CTaper(CTaperCycle taperCycle)
        {
            this.TaperCycle = taperCycle;
        }

        public CTaperCycle TaperCycle { get => taperCycle; set => taperCycle = value; }

        public override int MajEntrees() { return 1; }

        public override int MajCycle() { return 1; }

        public override int MajSorties() { return 1; }

        public override void Run(Object source, ElapsedEventArgs e) { }


        public override int Reset() { return 1; }

        public override int Load() { return 1; }

        public override int Save() { return 1; }
    }

    [Serializable]
    public class CTaperCycle : CCycleStatus
    {
        public enum State { Step0, Step1, Step2, Step3, Step4, Step5 }
        public enum Trigger { Tr0, Tr1, Tr2, Tr3, Tr4, Tr5, Tr6 }

        private readonly StateMachine<State, Trigger> taperCycle;
        private StateMachine<State, Trigger>.Transition trigFired;
        private IEnumerable<Trigger> listTrig;
        private string sListTrig;

        public IEnumerable<Trigger> ListTrig { get => listTrig; set => listTrig = value; }
        public StateMachine<State, Trigger>.Transition TrigFired { get => trigFired; set => trigFired = value; }

        public StateMachine<State, Trigger> _TaperCycle => taperCycle;

        public string SListTrig { get => sListTrig; set => sListTrig = value; }

        public CTaperCycle(State state)
        {
            taperCycle = new StateMachine<State, Trigger>(state);
            InitStateMachine();
        }

        #region Initialisation

        private void InitStateMachine()
        {
            // Evénement sur changement d'état
          
            _TaperCycle.OnTransitionCompleted(OnTransitionCompleted);


            // Permet de devalider les exceptions
            _TaperCycle.OnUnhandledTrigger((Step, trigger) => { Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffffff  ") + "Trigger not valid"); });


            // Construction du diagramme
            _TaperCycle.Configure(State.Step0)
                .OnActivate(() => OnProcessArmed(EventArgs.Empty))
                .Permit(Trigger.Tr0, State.Step1)
                .Permit(Trigger.Tr1, State.Step2)
                .OnEntry(() => OnEnterStep())
                .OnExit(() => OnExitStep());

            _TaperCycle.Configure(State.Step1)
              .Permit(Trigger.Tr2, State.Step3)
              .OnEntry(() => OnEnterStep())
              .OnExit(() => OnExitStep())
              .InternalTransition(Trigger.Tr0, () => OnTrigInterne());

            _TaperCycle.Configure(State.Step2)
             .Permit(Trigger.Tr3, State.Step4)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            _TaperCycle.Configure(State.Step3)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            _TaperCycle.Configure(State.Step4)
             .Permit(Trigger.Tr5, State.Step5)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            _TaperCycle.Configure(State.Step5)
             .Permit(Trigger.Tr6, State.Step0)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());
        }

        #endregion
        private void CheckTrigValid()
        {
            listTrig = _TaperCycle.PermittedTriggers;
          
            SListTrig = "(";
            for (int i = 0; i < listTrig.Count(); i++) SListTrig += listTrig.ElementAt(i) + ",";
            
            SListTrig = SListTrig.Substring(0, SListTrig.Length - 1);
            SListTrig += ")";
        }
        #region Evénement

        private void OnTransitionCompleted(StateMachine<State, Trigger>.Transition transition)
        {
            try
            {
                TrigFired = transition;
                CheckTrigValid();

                OnTransitionCompleted(EventArgs.Empty);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

            }
        }
        private void OnExitStep()
        {
            var StepInit = 0;

            // Evénement sur la sortie de l'étape initiale: Event Started
            if (_TaperCycle.State == (State)StepInit) OnProcessStarted(EventArgs.Empty);
            DateEndStep = DateTime.Now;
        }
        private void OnEnterStep()
        {
            var StepFin = Enum.GetNames(typeof(State)).Length - 1;
            var StepInit = 0;

            // Evenement sur l'entree de l'étape initiale: Event Armed
            if (_TaperCycle.State == (State)StepInit) OnProcessArmed(EventArgs.Empty);
            // Evenement sur l'entree de la dernière etape: Event Completed
            if (_TaperCycle.State == (State)StepFin) OnProcessCompleted(EventArgs.Empty);
            DateStartStep = DateTime.Now;
        }

        private void OnTrigInterne()
        {
            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffffff  ") + "Trigger interne");
        }
       
        #endregion

        #region Méthodes

        #endregion
    }
}