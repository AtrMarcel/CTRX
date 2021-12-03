using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRX
{
    [Serializable]
    public class CPostStatus
    {
        private bool error;
        private bool warning;
        private bool busy;
        private bool ended;
        private uint codeError;
        private bool initOk;
        private bool actived;
        private bool paused;
        private bool free;

        public CPostStatus()
        {
            Error = false;
            Warning = false;
            Busy = false;
            CodeError = 0;
        }

        /// <summary>
        /// Poste en defaut majeur
        /// </summary>
        public bool Error { get => error; set => error = value; }
        /// <summary>
        /// Poste en défaut mineur
        /// </summary>
        public bool Warning { get => warning; set => warning = value; }
        /// <summary>
        /// Poste en cours de fonctionnement
        /// </summary>
        public bool Busy { get => busy; set => busy = value; }
        /// <summary>
        /// Numéro du code erreur
        /// </summary>
        public uint CodeError { get => codeError; set => codeError = value; }
        /// <summary>
        /// Initialisation OK du poste
        /// </summary>
        public bool InitOk { get => initOk; set => initOk = value; }
        /// <summary>
        /// Activation du poste
        /// </summary>
        public bool Actived { get => actived; set => actived = value; }
        /// <summary>
        /// Poste en pause
        /// </summary>
        public bool Paused { get => paused; set => paused = value; }
        /// <summary>
        /// Cycle terminé
        /// </summary>
        public bool Ended { get => ended; set => ended = value; }

        /// <summary>
        /// Poste disponible
        /// </summary>
        public bool Free { get => free; set => free = value; }
    }

    [Serializable]
    public class CPostOption
    {
        private bool autoArmed;
        private bool autoStart;
        private bool stepByStep;
        private uint nivLog;
        private uint nivTrace;
        private uint sliceTime;
        private bool simulation;
        private bool valid;

        public bool AutoArmed { get => autoArmed; set => autoArmed = value; }
        public bool AutoStart { get => autoStart; set => autoStart = value; }
        public bool StepByStep { get => stepByStep; set => stepByStep = value; }
        public uint NivLog { get => nivLog; set => nivLog = value; }
        public uint NivTrace { get => nivTrace; set => nivTrace = value; }
        public uint SliceTime { get => sliceTime; set => sliceTime = value; }
        public bool Simulation { get => simulation; set => simulation = value; }
        public bool Valid { get => valid; set => valid = value; }
    }

    [Serializable]
    public class CPostCommand
    {
        private bool reqStart;
        private bool reqActive;
        private bool reqStop;
        private bool reqEnd;
        private bool reqInit;

        public bool ReqStart { get => reqStart; set => reqStart = value; }
        public bool ReqActive { get => reqActive; set => reqActive = value; }
        public bool ReqStop { get => reqStop; set => reqStop = value; }
        public bool ReqEnd { get => reqEnd; set => reqEnd = value; }
        public bool ReqInit { get => reqInit; set => reqInit = value; }
    }
}