using System;
using System.Diagnostics;
using System.IO;
using System.Timers;


namespace CTRX
{
    [Serializable]
    public abstract class CPost : CInfo
    {

        private CPostStatus status;
        private CPostOption option;
        private CPostCommand command;
        [NonSerialized]
        private System.Timers.Timer tickCycle;
        private string saveCycleJson;

        protected CPost()
        {
            TickCycle = new Timer
            {
                AutoReset = true,
                Enabled = false
            };

            Status = new CPostStatus
            {
                CodeError = 0,
              
            };
            Option = new CPostOption
            {
                Valid = false,
                AutoArmed = false,
                AutoStart = false,
                NivLog = 0,
                NivTrace = 0,
                MajSortieByEvt = false,
                SliceTime = 100
            };
            Command = new CPostCommand
            {
                ReqActive = false,
                ReqStart = false,
                ReqStop = false,
                ReqEnd = false
            };
            SaveCycleJson = "";
        }

        /// <summary>
        /// Tick du cycle PLC
        /// </summary>
      
        public Timer TickCycle { get => tickCycle; set => tickCycle = value; }
        public CPostStatus Status { get => status; set => status = value; }
        public CPostOption Option { get => option; set => option = value; }
        public CPostCommand Command { get => command; set => command = value; }
        public string SaveCycleJson { get => saveCycleJson; set => saveCycleJson = value; }

        /// <summary>
        /// Lance le cycle grafcet
        /// </summary>
        public abstract void Run(Object source, ElapsedEventArgs e);

        /// <summary>
        /// Lecture des variables d'entrées du poste
        /// </summary>
        public abstract int MajEntrees();
        /// <summary>
        /// Mise à jour du cycle
        /// </summary>
        public abstract int MajCycle();
        /// <summary>
        /// Programmation des sorties
        /// </summary>
        public abstract int MajSorties();
        /// <summary>
        /// Sauvegarde dans un fichier en fonction du niveau de détail
        /// </summary>
        public int MajRapport(String text, bool heure)
        {
            string txt = "";
            if (Option.NivTrace > 0)
            {
                if (heure) txt = DateTime.Now.ToString("HH:mm:ss.ffffff  ");
                txt += Name;
                txt += text;
                Debug.WriteLine(txt);
            }


             if (Option.NivLog > 0)
            {
                string path = @".\" + Name + ".txt";

                txt = "";
                if (heure) txt = DateTime.Now.ToString("HH:mm:ss.ffffff  ");
                txt += Name;
                txt += text;

                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(txt);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(txt);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Commande de démarrage du cycle
        /// </summary>
        public int Init()
        {
            int res = (int) Id;

            if (Option.Valid & !Status.InitOk)
            {
                Status.InitOk = true;
                Status.Error = false;
                Status.Warning = false;

                // Trace 
                if (Option.NivTrace > 0)
                {
                    MajRapport(": Init cycle", true);
                }
            }
            else
            {
                // Trace 
                if (Option.NivTrace > 0)
                {
                    MajRapport(": Not valid", true);
                }

            }

            return res;
        }

        /// <summary>
        /// Commande de démarrage du cycle
        /// </summary>
        public int Start()
        {
            int res = 0;

            if (Status.Actived & !Status.Paused)
            {

                if (!Status.Busy) Command.ReqStart = true;

                if (Option.StepByStep)
                {
                    // Timer
                    TickCycle.AutoReset = true;
                    TickCycle.Start();
                }
                
            }
            else
            {
                // Trace 
                if (Option.NivTrace > 0)
                {
                    if (!Status.Actived) MajRapport(": Not actived", true);
                }
            }


            return res;
        }

        /// <summary>
        /// Commande d'arrêt immédiat du cycle
        /// </summary>
        public int Stop()
        {
            // Post.Status
            Status.Error = true;
            Status.InitOk = false;
            Status.Paused = false;

            // PostCycle
            // Reset du cycle
            Reset();

            // Timer
            TickCycle.AutoReset = false;
            //TickCycle.Enabled = false;
            TickCycle.Stop();

            // Trace 
            if (Option.NivTrace > 0)
            {
                MajRapport(": Cycle stopped", true);
            }
            return 1;
        }

        /// <summary>
        /// Commande d'arrêt du cycle
        /// </summary>
        public int End()
        {
            Command.ReqEnd = true;
            Option.AutoArmed = false;
            Option.AutoStart = false;

       
            if (Status.Ended)
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
                MajRapport(": Request end cycle", true);
            }
            return 1;
        }


        /// <summary>
        /// Commande de mise en pause du cycle
        /// </summary>
        public int Pause()
        {
            if (Status.Actived)
            {
                // Post.Status
                Status.Paused = true;

                // Timer
                TickCycle.AutoReset = false;
                TickCycle.Stop();

                // Trace 
                if (Option.NivTrace > 0)
                {
                    MajRapport(": Cycle paused", true);
                }
            }

            return 1;
        }

        /// <summary>
        /// Commande de reprise du cycle
        /// </summary>
        public int Resume()
        {
            if (Status.Actived)
            {
                // Post.Status
                Status.Paused = false;

                // Timer
                TickCycle.AutoReset = true;
                TickCycle.Start();

                // Trace 
                if (Option.NivTrace > 0)
                {
                    MajRapport(": Cycle resumed", true);
                }
            }
            return 1;
        }

        /// <summary>
        /// Active le grafcet
        /// </summary>
        /// <summary>
        /// Active le cycle
        /// </summary>
        public int Active()
        {
            if (Status.InitOk & !Status.Actived)
            {
                // Active le cycle: PostCycle.SetActive()
                Command.ReqActive = true;
                //DetaperCycle.SetActive();

                // Lance le timer: Post.Run() 
                TickCycle.Interval = Option.SliceTime;
                TickCycle.AutoReset = true;
                TickCycle.Elapsed -= Run;
                TickCycle.Elapsed += Run;
                TickCycle.Start();
            }
            else
            {
                // Trace 
                if (Option.NivTrace > 0)
                {
                    if (!Status.InitOk) MajRapport(": Not iniatialized", true);
                }
            }


            return 1;
        }


        /// <summary>
        /// Réinitialise le grafcet
        /// </summary>
        public abstract int Reset();


        /// <summary>
        /// Sauvegarde le contexte grafcet
        /// </summary>
        public abstract int Save();


        /// <summary>
        /// Charge le contexte grafcet
        /// </summary>
        public abstract int Load();

       
    }
}