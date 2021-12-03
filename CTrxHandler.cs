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
    public class CTrxHandler : CTRX.CPost
    {
        #region Declaration

        public enum PAGE { DEMARRAGE, ATTENTE, MAINTENANCE, INITIALISATION, CHARGEMENT,
            PRODUCTION, DECHARGEMENT, DEFAUT, CONFIGURATION, ACCES, HISTORIQUE, DEBUG, SECURITE, RAPPORT}

        private CTrxHandlerCycle handlerCycle;

        public CTrxHandlerCycle HandlerCycle { get => handlerCycle; set => handlerCycle = value; }

        public CTrxHandler()
        {
            Name = "Handler TRX";
            Id = 2000;


            HandlerCycle = new CTrxHandlerCycle(CTrxHandlerCycle.State.IDLE);

            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            HandlerCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            HandlerCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            HandlerCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            HandlerCycle.TransitionCompleted += TransitionCompleted;
        }
        public CTrxHandler(CTrxHandlerCycle cameraCycle)
        {
            Name = "Camera";
            Id = 2000;


            HandlerCycle = new CTrxHandlerCycle(CTrxHandlerCycle.State.IDLE);

            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            HandlerCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            HandlerCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            HandlerCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            HandlerCycle.TransitionCompleted += TransitionCompleted;
        }
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
                MajRapport(": Cycle in running " + HandlerCycle.ToString(), true);
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

            if (HandlerCycle.ListTrig.Count() > 0)
            {
                HandlerCycle.CheckTrigValid();
                foreach (var trig in HandlerCycle.ListTrig)
                {
                    if (IsTrigOk((int)trig) == true)
                    {
                        HandlerCycle.SetTrig((int)trig);

                    }
                }
            }
            else
            {
                if (Command.ReqActive) HandlerCycle.SetActive();

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
                    res = true;
                    break;
                case 2:
                    res = true;
                    break;
                case 3:
                    res = true;
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
            switch (HandlerCycle.GetStateDest())
            {
                case 0:
                    break;
                case 1:

                    //Nb_Advance = CalculNbAdvance();
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
            HandlerCycle = new CTrxHandlerCycle(0);
            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            HandlerCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            HandlerCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            HandlerCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            HandlerCycle.TransitionCompleted += TransitionCompleted;

            return 1;
        }

        /// <summary>
        /// Sauvegarde l'état du cycle
        /// </summary>
        public override int Save()
        {
            SaveCycleJson = JsonConvert.SerializeObject(HandlerCycle, Newtonsoft.Json.Formatting.Indented);

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle saved " + HandlerCycle.ToString(), true);
            }

            return 1;
        }

        /// <summary>
        /// Recharge l'état mémorisé du cycle
        /// </summary>
        public override int Load()
        {

            HandlerCycle = JsonConvert.DeserializeObject<CTrxHandlerCycle>(SaveCycleJson);


            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            HandlerCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            HandlerCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            HandlerCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            HandlerCycle.TransitionCompleted += TransitionCompleted;

            // Redéclenche la temporisation de l'étape
            HandlerCycle.StepWatch.Start();

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle loaded " + HandlerCycle.ToString(), true);
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
            HandlerCycle.CheckTrigValid();

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
                    HandlerCycle.ProcessDelay.Hours,
                    HandlerCycle.ProcessDelay.Minutes,
                    HandlerCycle.ProcessDelay.Seconds,
                    HandlerCycle.ProcessDelay.Milliseconds);
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
                    HandlerCycle.StepDelay.Hours,
                    HandlerCycle.StepDelay.Minutes,
                    HandlerCycle.StepDelay.Seconds,
                    HandlerCycle.StepDelay.Milliseconds);
                string s = ": Cycle in " + HandlerCycle.sGetState() + " - Time in Step n°" + HandlerCycle.GetStateSrce() + ": " + elapsedTime;

                MajRapport(s, true);
            }

        }

        #endregion

        #region Méthodes

        /// <summary>
        /// Gestion d'une demande provenant de l'IHM
        /// </summary>
        public bool GestIhmEvt()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sauvegarde des evénements
        /// </summary>
        public void LogIhmEvt()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Mise à jour du contexte du Handler
        /// </summary>
        public void MajContexte()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sauvegarde du contexte
        /// </summary>
        public void SaveContexte()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gestion de l'ensemble des postes
        /// </summary>
        public void GestPostes()
        {
            throw new System.NotImplementedException();
        }
        #endregion


    }

    public class CTrxHandlerCycle : CTRX.CCycleStatus
    {
        #region Declaration
        public enum State { IDLE, INIT, PROD, MAINT, WARNING, ERROR }
        public enum Trigger { Tr0, Tr1, Tr2, Tr3, Tr4, Tr5 }

        private readonly StateMachine<State, Trigger> handlerCycle;
        private StateMachine<State, Trigger>.Transition trigFired;

        //private IEnumerable<Trigger> listTrig;
        private List<Trigger> listTrig;
        private string sListTrig;

        public State Step => handlerCycle.State;

        [JsonIgnore]
        public StateMachine<State, Trigger> HandlerCycle => handlerCycle;
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

        public CTrxHandlerCycle(State state)
        {
            handlerCycle = new StateMachine<State, Trigger>(state);
            ListTrig = new List<Trigger>();
            InitCycle();
            DateEnterSrce = DateTime.Now.ToString("HH:mm:ss.ffffff   ");
            DateEnterDest = DateEnterSrce;
            DateExitSrce = DateEnterSrce;
        }

        [JsonConstructor]
        private CTrxHandlerCycle(string Step) : this((State)Enum.Parse(typeof(State), Step)) { }
        #endregion

        #region Initialisation

        /// <summary>
        /// Initialisation du cycle
        /// </summary>
        private void InitCycle()
        {
            // Evénement sur changement d'état
            HandlerCycle.OnTransitionCompleted(OnTriggerCompleted);


            // Permet de devalider les exceptions
            HandlerCycle.OnUnhandledTrigger((Step, trigger) => { Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffffff  ") + "Trigger not valid: " + (int)trigger); });


            // Construction du diagramme
            // Etape 0
            HandlerCycle.Configure(State.IDLE)
                .OnActivate(() => OnProcessArmed(EventArgs.Empty))
                .Permit(Trigger.Tr0, State.INIT)
                .OnEntry(() => OnEnterStep())
                .OnExit(() => OnExitStep());

            // Etape 1
            HandlerCycle.Configure(State.INIT)
              .Permit(Trigger.Tr1, State.PROD)
              .OnEntry(() => OnEnterStep())
              .OnExit(() => OnExitStep());

            // Etape 2
            HandlerCycle.Configure(State.PROD)
             .Permit(Trigger.Tr2, State.MAINT)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 3
            HandlerCycle.Configure(State.MAINT)
             .Permit(Trigger.Tr3, State.WARNING)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 4
            HandlerCycle.Configure(State.WARNING)
             .Permit(Trigger.Tr5, State.ERROR)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 4
            HandlerCycle.Configure(State.ERROR)
             .Permit(Trigger.Tr5, State.IDLE)
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
            if (HandlerCycle.State == (State)StepInit) OnProcessArmed(EventArgs.Empty);
            // Evenement sur l'entree de la dernière etape: Event Completed
            if (HandlerCycle.State == (State)StepFin) OnProcessCompleted(EventArgs.Empty);
            DateEnterSrce = DateEnterDest;
            DateEnterDest = DateTime.Now.ToString("HH:mm:ss.ffffff");
            DateStartStep = DateTime.Now;

            if (HandlerCycle.State != (State)StepInit)
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
            if (HandlerCycle.State == (State)StepInit) OnProcessStarted(EventArgs.Empty);
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
            listTrig = HandlerCycle.PermittedTriggers.ToList();

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
            HandlerCycle.Activate();
        }

        /// <summary>
        /// Exécution de la transition 
        /// </summary>
        /// <param name="trig"></param>
        /// <returns></returns>
        public int SetTrig(int trig)
        {
            HandlerCycle.Fire((Trigger)trig);

            return 1;
        }

        /// <summary>
        /// Récupération du Flow Chart detaper
        /// </summary>
        /// <returns></returns>
        public string ToDotGraph()
        {
            string graph = UmlDotGraph.Format(HandlerCycle.GetInfo());
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
            string sState = "" + HandlerCycle.State;
            return sState;
        }

        public override string ToString()
        {
            return $"{nameof(HandlerCycle)}[state={HandlerCycle.State}]";
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
