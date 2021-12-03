using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRX
{
    public class CHandlerRequest
    {
        private bool req_Maint;
        private bool req_Resume;
        private bool req_StepByStep;
        private bool req_Pause;
        private bool req_Prod;
        private bool req_Stop;
        private bool req_End;
        private bool req_Start;
        private bool req_Init;
        private bool req_RazCpt;
        private bool req_Chgt;
        private bool req_Dechgt;
        private bool req_Config;
        private bool req_Acces;
        private bool req_Histo;
        private bool req_Load;
        private bool req_Save;
        private bool req_Debug;
        private bool req_Quit;
        private bool req_RazCptDbg;

        /// <summary>
        /// Demande de mise en mode maintenance
        /// </summary>
        public bool Req_Maint { get => req_Maint; set => req_Maint = value; }
        /// <summary>
        /// Demande un redémarrage après une pause
        /// </summary>
        public bool Req_Resume { get => req_Resume; set => req_Resume = value; }
        /// <summary>
        /// Activation du mode pas à pas du cycle de production
        /// </summary>
        public bool Req_StepByStep { get => req_StepByStep; set => req_StepByStep = value; }
        /// <summary>
        /// Demande une pause
        /// </summary>
        public bool Req_Pause { get => req_Pause; set => req_Pause = value; }
        /// <summary>
        /// Demande de mise en production
        /// </summary>
        public bool Req_Prod { get => req_Prod; set => req_Prod = value; }
        /// <summary>
        /// Demande un arrêt immédiat des mouvements
        /// </summary>
        public bool Req_Stop { get => req_Stop; set => req_Stop = value; }
        /// <summary>
        /// Demande une fin de production
        /// </summary>
        public bool Req_End { get => req_End; set => req_End = value; }
        /// <summary>
        /// Demande un démarrage de production
        /// </summary>
        public bool Req_Start { get => req_Start; set => req_Start = value; }
        /// <summary>
        /// Demande d'initialisation
        /// </summary>
        public bool Req_Init { get => req_Init; set => req_Init = value; }
        /// <summary>
        /// Demande une réinitialisation des compteurs produits
        /// </summary>
        public bool Req_RazCpt { get => req_RazCpt; set => req_RazCpt = value; }
        /// <summary>
        /// Demande une réinitialisation des compteurs machine
        /// </summary>
        public bool Req_RazCptDbg { get => req_RazCptDbg; set => req_RazCptDbg = value; }
        /// <summary>
        /// Demande la fermeture du programme
        /// </summary>
        public bool Req_Quit { get => req_Quit; set => req_Quit = value; }
        /// <summary>
        /// Demande d'ouvrir la fenêtre des  options de debug
        /// </summary>
        public bool Req_Debug { get => req_Debug; set => req_Debug = value; }
        /// <summary>
        /// Sauvegarde dans un fichier (Suivant le contexte)
        /// </summary>
        public bool Req_Save { get => req_Save; set => req_Save = value; }
        /// <summary>
        /// Chargement depuis un fichier (Suivant le contexte)
        /// </summary>
        public bool Req_Load { get => req_Load; set => req_Load = value; }
        /// <summary>
        /// Demande l'ouverture de la fenêtre de l'historique
        /// </summary>
        public bool Req_Histo { get => req_Histo; set => req_Histo = value; }
        /// <summary>
        /// Demande le changement d'accés
        /// </summary>
        public bool Req_Acces { get => req_Acces; set => req_Acces = value; }
        /// <summary>
        /// Demande l'ouverture du fichier de configuration
        /// </summary>
        public bool Req_Config { get => req_Config; set => req_Config = value; }
        /// <summary>
        /// Demande une procédure de déchargement du produit
        /// </summary>
        public bool Req_Dechgt { get => req_Dechgt; set => req_Dechgt = value; }
        /// <summary>
        /// Demande une procédure de chargement
        /// </summary>
        public bool Req_Chgt { get => req_Chgt; set => req_Chgt = value; }
    }

    public class CHandlerStatus
    {
    }

    public class CHandlerOption
    {
    }
}