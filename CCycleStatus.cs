using System;
using System.Diagnostics;

namespace CTRX
{
    [Serializable]
    public class CCycleStatus
    {
        public enum STATE_CYCLE { CYCLE_OK, STEP_EXIT, TRIG_ENDED, STEP_ENTRY, TRIG_COMPLETED, CYCLE_ERROR }
        private Stopwatch cycleWatch;
        private Stopwatch stepWatch;
        private DateTime dateStartCycle;
        private DateTime dateEndCycle;
        private DateTime dateStartStep;
        private DateTime dateEndStep;
        private TimeSpan stepDelay;
        private TimeSpan processDelay;
        private STATE_CYCLE statusCycle;
        private int stepSrc;
        private int stepDest;
        private string dateEnterSrce;
        private string dateEnterDest;
        private string dateExitSrce;
       



        /// <summary>
        /// Date de départ de cycle
        /// </summary>
        public DateTime DateStartCycle { get => dateStartCycle; set => dateStartCycle = value; }
        /// <summary>
        /// Date de fin de cycle
        /// </summary>
        public DateTime DateEndCycle { get => dateEndCycle; set => dateEndCycle = value; }
        /// <summary>
        /// Date de sortie d'étape
        /// </summary>
        public DateTime DateEndStep { get => dateEndStep; set => dateEndStep = value; }
        /// <summary>
        /// Date d'entrée d'étape
        /// </summary>
        public DateTime DateStartStep { get => dateStartStep; set => dateStartStep = value; }
        /// <summary>
        /// Chronométre cycle
        /// </summary>
        public Stopwatch CycleWatch { get => cycleWatch; set => cycleWatch = value; }
       
        /// <summary>
        /// Chronometre d'étape
        /// </summary>
        public Stopwatch StepWatch { get => stepWatch; set => stepWatch = value; }
        public STATE_CYCLE StatusCycle { get => statusCycle; set => statusCycle = value; }
        public TimeSpan StepDelay { get => stepDelay; set => stepDelay = value; }
        public TimeSpan ProcessDelay { get => processDelay; set => processDelay = value; }
        public int StepSrc { get => stepSrc; set => stepSrc = value; }
        public int StepDest { get => stepDest; set => stepDest = value; }
        public string DateEnterSrce { get => dateEnterSrce; set => dateEnterSrce = value; }
        public string DateEnterDest { get => dateEnterDest; set => dateEnterDest = value; }
        public string DateExitSrce { get => dateExitSrce; set => dateExitSrce = value; }
       
        /// <summary>
        /// Evénement à l'armement du cycle
        /// </summary>
        public event EventHandler ProcessArmed;
        /// <summary>
        /// Evénement à la fin du cycle
        /// </summary>
        public event EventHandler ProcessCompleted;
        /// <summary>
        /// Evénement au départ du cycle
        /// </summary>
        public event EventHandler ProcessStarted;
        /// <summary>
        /// Evénement sur une erreur de cycle
        /// </summary>
        public event EventHandler ProcessError;
        /// <summary>
        /// Evénement signalant le changement d'étape
        /// </summary>
        public event EventHandler TransitionCompleted;

        /// <summary>
        /// Fonction signalant le changement d'étape
        /// </summary>
        protected virtual void OnTransitionCompleted(EventArgs e)
        {
            StatusCycle = STATE_CYCLE.TRIG_COMPLETED;
            
            // Evénement disponible pour lancer une action sans attendre le SliceTime
            TransitionCompleted?.Invoke(this, e);
            
        }
        /// <summary>
        /// Fonction signalant l'armement du cycle
        /// </summary>
        protected virtual void OnProcessArmed(EventArgs e)
        {
            stepWatch = new Stopwatch();
            stepWatch.Start();

            ProcessArmed?.Invoke(this, e);
        }
        /// <summary>
        /// Fonction signalant le depart du cycle
        /// </summary>
        protected virtual void OnProcessStarted(EventArgs e)
        {
            // Enregistrement date de départ et lancement du chrono
            DateStartCycle = DateTime.Now;
            CycleWatch = new Stopwatch();
            CycleWatch.Start();

            ProcessStarted?.Invoke(this, e);

        }
        /// <summary>
        /// Fonction signalant la fin du cycle
        /// </summary>
        protected virtual void OnProcessCompleted(EventArgs e)
        {
            // Enregistrement date de fin  et arret du chrono
            DateEndCycle = DateTime.Now;
            ProcessDelay = CycleWatch.Elapsed;
            CycleWatch.Stop();

            ProcessCompleted?.Invoke(this, e);

        }
        /// <summary>
        /// Fonction signalant une erreur de cycle
        /// </summary>
        protected virtual void OnProcessError(EventArgs e)
        {
            ProcessError?.Invoke(this, e);
        }

        public string GetDateEnterSrce()
        {
            return dateEnterSrce;
        }
        public string GetDateExitSrce()
        {
            return dateExitSrce;
        }
        public string GetDateEnterDest()
        {
            return dateEnterDest;
        }
    }
}