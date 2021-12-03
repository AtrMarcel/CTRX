using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using System.Timers;

namespace CTRX
{
    [Serializable]
    public class CPickupHead : CPost
    {
        #region Declaration
        [NonSerialized]
        private CPickupHeadCycle pickupCycle;
        

        public CPickupHead()
        {
            Name = "PickupHead";
            Id = 2000;


            PickupCycle = new CPickupHeadCycle(CPickupHeadCycle.State.Step0);

            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            PickupCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            PickupCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            PickupCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            PickupCycle.TransitionCompleted += TransitionCompleted;
        }

        public CPickupHead(CPickupHeadCycle pickupCycle)
        {
            Name = "PickupHead";
            Id = 2000;



            PickupCycle = pickupCycle;

            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            PickupCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            PickupCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            PickupCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            PickupCycle.TransitionCompleted += TransitionCompleted;
        }

       
        public CPickupHeadCycle PickupCycle { get => pickupCycle; set => pickupCycle = value; }

        #endregion

        #region Cycle PLC
        /// <summary>
        /// Démarrage de la tache automate
        /// </summary>
        public override void Run(Object source, ElapsedEventArgs e)
        {

            if (MajEntrees() == 0)
            {
                if (MajCycle() == 0)
                {

                }
                else
                {
                    Status.CodeError = Id + 2;
                }
            }
            else
            {
                Status.CodeError = Id + 1;
            }
            // Trace 
            if (Option.NivTrace > 5)
            {
                MajRapport(": Cycle in running " + PickupCycle.ToString(), true);
            }
        }
        /// <summary>
        /// Lecture des variables d'entrée
        /// </summary>
        public override int MajEntrees()
        {
            return 0;
        }

        /// <summary>
        /// Mise à jour des étapes du cycle
        /// </summary>
        public override int MajCycle()
        {
            int res = 0;

            if (PickupCycle.ListTrig.Count() > 0)
            {
                PickupCycle.CheckTrigValid();
                foreach (var trig in PickupCycle.ListTrig)
                {
                    if (IsTrigOk((int)trig) == true)
                    {
                        PickupCycle.SetTrig((int)trig);

                    }
                }
            }
            else
            {
                if (Command.ReqActive) PickupCycle.SetActive();

            }

            return res;
        }

        public bool IsTrigOk(int trig)
        {
            bool res;

            // Ecrire les équations des triggers
            switch (trig)
            {
                case 0:
                    res = Option.AutoStart | Command.ReqStart;
                    break;
                case 1:
                    res = PickupCycle.StepWatch.ElapsedMilliseconds > 200;
                    break;
                case 2:
                    res = PickupCycle.StepWatch.ElapsedMilliseconds > 200;
                    break;
                case 3:
                    res = PickupCycle.StepWatch.ElapsedMilliseconds > 200;
                    break;
                case 4:
                    res = true;
                    break;
                case 5:
                    res = (Option.AutoArmed | Command.ReqActive) & !Status.Error & !Status.Warning;
                    break;
                default:
                    res = false;
                    break;

            }
            return res;
        }

        /// <summary>
        /// Programmation des variables de sortie
        /// </summary>
        public override int MajSorties()
        {
            int res = 0;
            switch (PickupCycle.GetStateDest())
            {
                case 0:
                    break;
                case 1:
                   
                    break;
                case 2:
                    break;
                case 3:
                   
                    break;
                case 4:
                    break;

            }

            return res;
        }


        #endregion

        #region Commandes

        /// <summary>
        /// Réinitialise le cycle
        /// </summary>
        public override int Reset()
        {
            // Tick Cycle
            TickCycle.AutoReset = false;
            TickCycle.Stop();

            // Post.Status
            Status.Actived = false;
            Status.Busy = false;
            Status.Ended = false;


            // PostCycle
            PickupCycle = new CPickupHeadCycle(0);
            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            PickupCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            PickupCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            PickupCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            PickupCycle.TransitionCompleted += TransitionCompleted;

            return 1;
        }

        /// <summary>
        /// Sauvegarde l'état du cycle
        /// </summary>
        public override int Save()
        {
            SaveCycleJson = JsonConvert.SerializeObject(PickupCycle, Newtonsoft.Json.Formatting.Indented);

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle saved " + PickupCycle.ToString(), true);
            }

            return 1;
        }

        /// <summary>
        /// Recharge l'état mémorisé du cycle
        /// </summary>
        public override int Load()
        {

            PickupCycle = JsonConvert.DeserializeObject<CPickupHeadCycle>(SaveCycleJson);


            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            PickupCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            PickupCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            PickupCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            PickupCycle.TransitionCompleted += TransitionCompleted;

            // Redéclenche la temporisation de l'étape
            PickupCycle.StepWatch.Start();

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle loaded " + PickupCycle.ToString(), true);
            }


            return 1;
        }



        #endregion

        #region Evénements

        public void ProcessArmed(object sender, EventArgs e)
        {
            // Status
            Status.Actived = true;
            Status.Busy = false;
            Status.Ended = false;

            // Command
            Command.ReqActive = false;
            PickupCycle.CheckTrigValid();

            // Trace 
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle armed", true);
            }
        }
        public void ProcessStarted(object sender, EventArgs e)
        {
            // Status
            Status.Busy = true;

            // Command
            Command.ReqStart = false;

            // Trace 
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle started", true);
            }

        }
        public void ProcessCompleted(object sender, EventArgs e)
        {
            if (Status.Busy)
            {
                Status.Actived = false;
                Status.Ended = true;
                Status.Busy = false;
            }

            // Trace 
            if (Option.NivTrace > 0)
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}  ms",
                    PickupCycle.ProcessDelay.Hours,
                    PickupCycle.ProcessDelay.Minutes,
                    PickupCycle.ProcessDelay.Seconds,
                    PickupCycle.ProcessDelay.Milliseconds);
                MajRapport(": Cycle completed - Time process: " + elapsedTime, true);
            }


        }
        public void TransitionCompleted(object sender, EventArgs e)
        {


            // Mise à jour des sorties
            if (MajSorties() == 0)
            {
                if (Option.StepByStep)
                {
                    Command.ReqStart = false;
                    TickCycle.AutoReset = false;
                    TickCycle.Stop();
                }
            }
            else
            {
                Status.CodeError = Id + 3;
            }

            // Trace 
            if (Option.NivTrace > 1)
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000} ms",
                    PickupCycle.StepDelay.Hours,
                    PickupCycle.StepDelay.Minutes,
                    PickupCycle.StepDelay.Seconds,
                    PickupCycle.StepDelay.Milliseconds);
                string s = ": Cycle in " + PickupCycle.sGetState() + " - Time in Step n°" + PickupCycle.GetStateSrce() + ": " + elapsedTime;

                MajRapport(s, true);
            }

        }
        #endregion

        #region Méthodes
        
        #endregion
    }

    [Serializable]
    public class CPickupHeadCycle : CCycleStatus
    {
        #region Declaration
        public enum State { Step0, Step1, Step2, Step3, Step4, Step5, Step6, Step7}
        public enum Trigger { Tr0, Tr1, Tr2, Tr3, Tr4, Tr5, Tr6, Tr7, Tr8, Tr9, Tr10 }

        private readonly StateMachine<State, Trigger> headCycle;
        private StateMachine<State, Trigger>.Transition trigFired;

        //private IEnumerable<Trigger> listTrig;
        private List<Trigger> listTrig;
        private string sListTrig;

        public State Step => headCycle.State;

        [JsonIgnore]
        public StateMachine<State, Trigger> HeadCycle => headCycle;
        /// <summary>
        /// Transisition franchie
        /// </summary>
        public StateMachine<State, Trigger>.Transition TrigFired { get => trigFired; set => trigFired = value; }


        /// <summary>
        /// Liste des transitions franchissables
        /// </summary>

        //public IEnumerable<Trigger> ListTrig { get => listTrig; set => listTrig = value; }

        public List<Trigger> ListTrig { get => listTrig; set => listTrig = value; }
        /// <summary>
        /// Liste des transitions franchissable (Formart Texte)
        /// </summary>
        public string SListTrig { get => sListTrig; set => sListTrig = value; }

        public CPickupHeadCycle(State state)
        {
            headCycle = new StateMachine<State, Trigger>(state);
            ListTrig = new List<Trigger>();
            InitCycle();
            DateEnterSrce = DateTime.Now.ToString("HH:mm:ss.ffffff   ");
            DateEnterDest = DateEnterSrce;
            DateExitSrce = DateEnterSrce;
        }

        [JsonConstructor]
        private CPickupHeadCycle(string Step) : this((State)Enum.Parse(typeof(State), Step)) { }
        #endregion

        #region Initialisation

        /// <summary>
        /// Initialisation du cycle
        /// </summary>
        private void InitCycle()
        {
            // Evénement sur changement d'état
            HeadCycle.OnTransitionCompleted(OnTriggerCompleted);


            // Permet de devalider les exceptions
            HeadCycle.OnUnhandledTrigger((Step, trigger) => { Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffffff  ") + "Trigger not valid: " + (int)trigger); });


            // Construction du diagramme
            // Etape 0
            HeadCycle.Configure(State.Step0)
                .OnActivate(() => OnProcessArmed(EventArgs.Empty))
                .Permit(Trigger.Tr0, State.Step1)
                .OnEntry(() => OnEnterStep())
                .OnExit(() => OnExitStep());

            // Etape 1
            HeadCycle.Configure(State.Step1)
              .Permit(Trigger.Tr1, State.Step2)
              .Permit(Trigger.Tr2, State.Step7)
              .OnEntry(() => OnEnterStep())
              .OnExit(() => OnExitStep());

            // Etape 2
            HeadCycle.Configure(State.Step2)
             .Permit(Trigger.Tr3, State.Step3)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 3
            HeadCycle.Configure(State.Step3)
             .Permit(Trigger.Tr4, State.Step4)
             .Permit(Trigger.Tr5, State.Step7)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 4
            HeadCycle.Configure(State.Step4)
             .Permit(Trigger.Tr6, State.Step5)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 5
            HeadCycle.Configure(State.Step5)
             .Permit(Trigger.Tr7, State.Step6)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 6
            HeadCycle.Configure(State.Step6)
             .Permit(Trigger.Tr8, State.Step2)
             .Permit(Trigger.Tr9, State.Step7)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 7
            HeadCycle.Configure(State.Step7)
             .Permit(Trigger.Tr10, State.Step0)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

        }

        #endregion

        #region Evénements
        /// <summary>
        /// Evénement sur une transition finie
        /// </summary>
        private void OnTriggerCompleted(StateMachine<State, Trigger>.Transition transition)
        {
            try
            {
                TrigFired = transition;
                StepSrc = (int)transition.Source;
                StepDest = (int)transition.Destination;
                CheckTrigValid();

                OnTransitionCompleted(EventArgs.Empty);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

            }
        }

        /// <summary>
        /// Evénement sur l'entrée de l'étape
        /// </summary>
        private void OnEnterStep()
        {
            var StepFin = Enum.GetNames(typeof(State)).Length - 1;
            var StepInit = 0;

            // Evenement sur l'entree de l'étape initiale: Event Armed
            if (HeadCycle.State == (State)StepInit) OnProcessArmed(EventArgs.Empty);
            // Evenement sur l'entree de la dernière etape: Event Completed
            if (HeadCycle.State == (State)StepFin) OnProcessCompleted(EventArgs.Empty);
            DateEnterSrce = DateEnterDest;
            DateEnterDest = DateTime.Now.ToString("HH:mm:ss.ffffff");
            DateStartStep = DateTime.Now;

            if (HeadCycle.State != (State)StepInit)
            {
                StepWatch = new Stopwatch();
                StepWatch.Start();
            }


        }

        /// <summary>
        /// Evénement sur une sortie d'étape
        /// </summary>
        private void OnExitStep()
        {
            var StepInit = 0;

            // Evénement sur la sortie de l'étape initiale: Event Started
            if (HeadCycle.State == (State)StepInit) OnProcessStarted(EventArgs.Empty);
            DateExitSrce = DateTime.Now.ToString("HH:mm:ss.ffffff");
            DateEndStep = DateTime.Now;

            StepDelay = StepWatch.Elapsed;
            StepWatch.Stop();

        }

        #endregion

        #region Méthodes
        /// <summary>
        /// Controle des transitions franchissables
        /// </summary>
        public void CheckTrigValid()
        {
            listTrig = HeadCycle.PermittedTriggers.ToList();

            SListTrig = "(";
            for (int i = 0; i < listTrig.Count(); i++) SListTrig += listTrig.ElementAt(i) + ",";

            SListTrig = SListTrig.Substring(0, SListTrig.Length - 1);
            SListTrig += ")";
        }

        /// <summary>
        /// Activation du cycle detaper
        /// </summary>
        public void SetActive()
        {
            HeadCycle.Activate();
        }

        /// <summary>
        /// Exécution de la transition 
        /// </summary>
        /// <param name="trig"></param>
        /// <returns></returns>
        public int SetTrig(int trig)
        {
            HeadCycle.Fire((Trigger)trig);

            return 1;
        }

        /// <summary>
        /// Récupération du Flow Chart detaper
        /// </summary>
        /// <returns></returns>
        public string ToDotGraph()
        {
            string graph = UmlDotGraph.Format(HeadCycle.GetInfo());
            graph = graph.Replace("entry / Function", "entry/onEntry()");
            graph = graph.Replace("exit / Function", "exit/onExit()");
            //Export DOT Graph
            return graph;
        }

        /// <summary>
        /// Récupération de l'étape en cours
        /// </summary>
        /// <returns></returns>
        public string sGetState()
        {
            string sState = "" + HeadCycle.State;
            return sState;
        }

        public override string ToString()
        {
            return $"{nameof(HeadCycle)}[state={HeadCycle.State}]";
        }


        /// <summary>
        /// Récupération de l'étape source
        /// </summary>
        /// <returns></returns>
        public int GetStateSrce()
        {
            int srce = -1;
            try
            {
                srce = StepSrc;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

            }
            return srce;
            ;
        }

        /// <summary>
        /// Récupération de l'étape de destination
        /// </summary>
        /// <returns></returns>
        public int GetStateDest()
        {
            int dest = -1;
            try
            {
                dest = StepDest;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

            }
            return dest;

        }
        #endregion
    }
}