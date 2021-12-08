using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using System.Timers;
using System.Threading;

namespace CTRX
{
    [Serializable]
    public class CDetaper : CPost
    {
        #region Declaration
        [NonSerialized]
        private CDetaperCycle detaperCycle;
        private uint nb_Advance;
        private uint cpt_Advance;
        private uint cpt_GblAdvance;
        private uint tps_SigPulse;
        private uint tps_SigPeriode;
        private uint tps_WaitCover;

        public CDetaper()
        {
            Name = "Detaper";
            Id = 0x2000;
            Nb_Advance = 3;
            Cpt_Advance = 0;
            Cpt_GblAdvance = 0;
            Tps_SigPulse = 50;
            Tps_SigPeriode = 100;
            Tps_WaitCover = 3000;

            DetaperCycle = new CDetaperCycle(CDetaperCycle.State.Step0);
           
            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            DetaperCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            DetaperCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            DetaperCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            DetaperCycle.TransitionCompleted += TransitionCompleted;
        }

        public CDetaper(CDetaperCycle detaperCycle)
        {
            Name = "Detaper";
            Id = 0x2000;
            Nb_Advance = 3;
            Cpt_Advance = 0;
            Cpt_GblAdvance = 0;
            Tps_SigPulse = 50;
            Tps_SigPeriode = 100;
            Tps_WaitCover = 3000;
            

            DetaperCycle = detaperCycle;

            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            DetaperCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            DetaperCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            DetaperCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            DetaperCycle.TransitionCompleted += TransitionCompleted;
        }

        /// <summary>
        /// Nombre d'avance à effectuer
        /// </summary>
        public uint Nb_Advance { get => nb_Advance; set => nb_Advance = value; }
        /// <summary>
        /// Compteur d'avance en cours
        /// </summary>
        public uint Cpt_Advance { get => cpt_Advance; set => cpt_Advance = value; }
        /// <summary>
        /// Compteur global d'avance
        /// </summary>
        public uint Cpt_GblAdvance { get => cpt_GblAdvance; set => cpt_GblAdvance = value; }
        /// <summary>
        /// Durée de l'impulsion de commande d'avance
        /// </summary>
        public uint Tps_SigPulse { get => tps_SigPulse; set => tps_SigPulse = value; }
        /// <summary>
        /// Période minimale entre deux commande d'avance
        /// </summary>
        public uint Tps_SigPeriode { get => tps_SigPeriode; set => tps_SigPeriode = value; }
        /// <summary>
        /// Temps d'attente entre la commande de fermeture couvercle et  la commande d'avance
        /// </summary>
        public uint Tps_WaitCover { get => tps_WaitCover; set => tps_WaitCover = value; }
        public CDetaperCycle DetaperCycle { get => detaperCycle; set => detaperCycle = value; }

        #endregion

        #region Cycle PLC
        /// <summary>
        /// Démarrage de la tache automate
        /// </summary>
        public override void Run(Object source, ElapsedEventArgs e)
        {
            if (!Option.MajSortieByEvt)
            {
                //Thread.Sleep((int)Option.SliceTime / 10);
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
            }
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

            if (DetaperCycle.ListTrig.Count() > 0)
            {
                DetaperCycle.CheckTrigValid();
                foreach (var trig in DetaperCycle.ListTrig)
                {
                    if (IsTrigOk((int)trig) == true)
                    {
                        DetaperCycle.SetTrig((int)trig);

                    }
                }
            }
            else
            {
                if (Command.ReqActive) DetaperCycle.SetActive();

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
                    res = DetaperCycle.StepWatch.ElapsedMilliseconds > tps_WaitCover;
                    break;
                case 2:
                    res = DetaperCycle.StepWatch.ElapsedMilliseconds > tps_SigPulse;
                    break;
                case 3:
                    res = (DetaperCycle.StepWatch.ElapsedMilliseconds > tps_SigPeriode) && (cpt_Advance < nb_Advance);
                    break;
                case 4:
                    res = cpt_Advance >= nb_Advance;
                    break;
                case 5:
                    res = (Option.AutoArmed | Command.ReqActive ) & !Status.Error & !Status.Warning;
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
            switch(DetaperCycle.GetStateDest())
            {
                case 0:
                    break;
                case 1:
                    Cpt_Advance = 0;
                    //Nb_Advance = CalculNbAdvance();
                    break;
                case 2:
                    break;
                case 3:
                    if (Status.NbPasInStep == 0)
                    {
                        Cpt_Advance++;
                        Cpt_GblAdvance++;
                    }
                    break;
                case 4:
                    break;

            }
           
            // Trace 
            if (Option.NivTrace > 5)
            {
                string txt;

                txt = DateTime.Now.ToString("HH:mm:ss.ffffff  ");
                txt += "Detaper : Cycle in running Step n°" + DetaperCycle.GetStateDest() + " - Nb.Pas: " + Status.NbPasInStep;

                Debug.WriteLine(txt);
            }
            Status.NbPasInStep++;
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
            DetaperCycle = new CDetaperCycle(0);
            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            DetaperCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            DetaperCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            DetaperCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            DetaperCycle.TransitionCompleted += TransitionCompleted;

            return 1;
        }

        /// <summary>
        /// Sauvegarde l'état du cycle
        /// </summary>
        public override int Save()
        {
            SaveCycleJson = JsonConvert.SerializeObject(DetaperCycle, Newtonsoft.Json.Formatting.Indented);

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle saved " + DetaperCycle.ToString(), true);
            }

            return 1;
        }

        /// <summary>
        /// Recharge l'état mémorisé du cycle
        /// </summary>
        public override int Load()
        {
           
            DetaperCycle = JsonConvert.DeserializeObject<CDetaperCycle>(SaveCycleJson);


            //DetaperCycle.ProcessCompleted -= ProcessCompleted;
            DetaperCycle.ProcessCompleted += ProcessCompleted;
            //DetaperCycle.ProcessStarted -= ProcessStarted;
            DetaperCycle.ProcessStarted += ProcessStarted;
            //DetaperCycle.ProcessArmed -= ProcessArmed;
            DetaperCycle.ProcessArmed += ProcessArmed;
            //DetaperCycle.TransitionCompleted -= TransitionCompleted;
            DetaperCycle.TransitionCompleted += TransitionCompleted;

            // Redéclenche la temporisation de l'étape
            DetaperCycle.StepWatch.Start();

            // Trace
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle loaded " + DetaperCycle.ToString(), true);
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
            DetaperCycle.CheckTrigValid();

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
            if (Status.Busy) {
                Status.Actived = false;
                Status.Ended = true;
                Status.Busy = false;
            }

            if (Command.ReqEnd | !Option.AutoArmed)
            {
                Command.ReqEnd = false;
                // Timer
                TickCycle.AutoReset = false;
                //TickCycle.Enabled = false;
                TickCycle.Stop();
            }
            // Trace 
            if (Option.NivTrace > 0)
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}  ms", 
                    DetaperCycle.ProcessDelay.Hours, 
                    DetaperCycle.ProcessDelay.Minutes, 
                    DetaperCycle.ProcessDelay.Seconds, 
                    DetaperCycle.ProcessDelay.Milliseconds);
                MajRapport(": Cycle completed - Time process: " + elapsedTime, true);
            }

            
        }
        public void TransitionCompleted(object sender, EventArgs e)
        {
            Status.NbPasInStep = 0;
            // Mise à jour des sorties
            if (Option.MajSortieByEvt)
            {
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
                
            }
                // Trace 
                if (Option.NivTrace > 1)
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000} ms", 
                    DetaperCycle.StepDelay.Hours, 
                    DetaperCycle.StepDelay.Minutes, 
                    DetaperCycle.StepDelay.Seconds, 
                    DetaperCycle.StepDelay.Milliseconds);
                string s = ": Cycle in " + DetaperCycle.sGetState() + " - Time in Step n°" + DetaperCycle.GetStateSrce() + ": " + elapsedTime;
                
                MajRapport(s, true);
            }

        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Calcul le nombre d'avance a effectué en fonction de l'état du Handler
        /// </summary>
        public uint CalculNbAdvance()
        {
            return Nb_Advance;
        }
        #endregion
    }

    [Serializable]
    public class CDetaperCycle : CCycleStatus
    {
        #region Declaration
        public enum State { Step0, Step1, Step2, Step3, Step4 }
        public enum Trigger { Tr0, Tr1, Tr2, Tr3, Tr4, Tr5 }
        
        private readonly StateMachine<State, Trigger> detaperCycle;
        private StateMachine<State, Trigger>.Transition trigFired;
       
        //private IEnumerable<Trigger> listTrig;
        private List<Trigger> listTrig;
        private string sListTrig;

        public State Step => detaperCycle.State;

        [JsonIgnore]
        public StateMachine<State, Trigger> DetaperCycle => detaperCycle;
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

        public CDetaperCycle(State state)
        {
            detaperCycle = new StateMachine<State, Trigger>(state);
            ListTrig = new List<Trigger>();
            InitCycle();
            DateEnterSrce = DateTime.Now.ToString("HH:mm:ss.ffffff   ");
            DateEnterDest = DateEnterSrce;
            DateExitSrce = DateEnterSrce;
        }

        [JsonConstructor]
        private CDetaperCycle(string Step) : this((State)Enum.Parse(typeof(State), Step)) { }
        #endregion

        #region Initialisation

        /// <summary>
        /// Initialisation du cycle
        /// </summary>
        private void InitCycle()
        {
            // Evénement sur changement d'état
            DetaperCycle.OnTransitionCompleted(OnTriggerCompleted);


            // Permet de devalider les exceptions
            DetaperCycle.OnUnhandledTrigger((Step, trigger) => { Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.ffffff  ") + "Trigger not valid: " + (int) trigger); });


            // Construction du diagramme
            // Etape 0
            DetaperCycle.Configure(State.Step0)
                .OnActivate(() => OnProcessArmed(EventArgs.Empty))
                .Permit(Trigger.Tr0, State.Step1)
                .OnEntry(() => OnEnterStep())
                .OnExit(() => OnExitStep());

            // Etape 1
            DetaperCycle.Configure(State.Step1)
              .Permit(Trigger.Tr1, State.Step2)
              .OnEntry(() => OnEnterStep())
              .OnExit(() => OnExitStep());

            // Etape 2
            DetaperCycle.Configure(State.Step2)
             .Permit(Trigger.Tr2, State.Step3)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 3
            DetaperCycle.Configure(State.Step3)
             .Permit(Trigger.Tr3, State.Step2)
             .Permit(Trigger.Tr4, State.Step4)
             .OnEntry(() => OnEnterStep())
             .OnExit(() => OnExitStep());

            // Etape 4
            DetaperCycle.Configure(State.Step4)
             .Permit(Trigger.Tr5, State.Step0)
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
            if (DetaperCycle.State == (State)StepInit) OnProcessArmed(EventArgs.Empty);
            // Evenement sur l'entree de la dernière etape: Event Completed
            if (DetaperCycle.State == (State)StepFin) OnProcessCompleted(EventArgs.Empty);
            DateEnterSrce = DateEnterDest;
            DateEnterDest = DateTime.Now.ToString("HH:mm:ss.ffffff");
            DateStartStep = DateTime.Now;
           
            if (DetaperCycle.State != (State)StepInit)
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
            if (DetaperCycle.State == (State)StepInit) OnProcessStarted(EventArgs.Empty);
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
            listTrig = DetaperCycle.PermittedTriggers.ToList();
            
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
            DetaperCycle.Activate();
        }

        /// <summary>
        /// Exécution de la transition 
        /// </summary>
        /// <param name="trig"></param>
        /// <returns></returns>
        public int SetTrig(int trig)
        {
            DetaperCycle.Fire((Trigger)trig);

            return 1;
        }

        /// <summary>
        /// Récupération du Flow Chart detaper
        /// </summary>
        /// <returns></returns>
        public string ToDotGraph()
        {
            string graph = UmlDotGraph.Format(DetaperCycle.GetInfo());
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
            string sState = "" + DetaperCycle.State;
            return sState;
        }

        public override string ToString()
        {
            return $"{nameof(DetaperCycle)}[state={DetaperCycle.State}]";
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