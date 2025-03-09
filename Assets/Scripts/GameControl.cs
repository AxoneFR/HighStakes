using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        public Canvas canvas_1, canvas_2, canvas_3, canvas_4;
        public GameObject text;

        public Text player1_cnt, player2_cnt, player3_cnt, status;

        public Text win1, win2, win3, win4;
        public Lean.Gui.LeanButton pass, submit;

        public GameObject deckPrefab, playerPrefab;

        public bool _8clear = true;
        private GameAlgorithm turn;
        
        private Deck deck;
        private GameObject Canvas;
        private Transform CanvasTranform;
        
        private bool isLoading = true;
        private bool isStart = false;

        // Permet de changer l'état de la variable _8clear (8clear activé/désactivé).
        public void _8clearT()
        {
            _8clear = !_8clear;
        }

        // Gère le clic sur le bouton de soumission.
        public void SubmitClick()
        {
            if (submit.IsInteractable())
            {
                turn?.SubmitClick();
            }
        }

        // Gère le clic sur le bouton de passage.
        public void PassClick()
        {
            if (pass.IsInteractable())
            {
                turn?.PassClick();
            }
        }

        // Gère la fin du jeu.
        public void RealEnd()
        {
            isLoading = false;
            isStart = false;
            turn?.EndGame();
            ReadyStart();
        }

        // Gère la fin du jeu avec un classement spécifique.
        public void RealEndNormal(List<string> rankList)
        {
            isLoading = false;
            isStart = false;
            turn?.EndGame();
            GameResult(rankList);
        }

        // Démarre le jeu, initialise les paramètres et distribue les cartes.
        public void RealStart()
        {
            isStart = true;
            canvas_1.gameObject.SetActive(true);
            canvas_2.gameObject.SetActive(false);
            canvas_4.gameObject.SetActive(false);

            turn = gameObject.AddComponent<GameAlgorithm>();
            turn.Initialise(status, Allow8Clear: !_8clear);
            isLoading = true;
            InitPlayers();
            deck = deckPrefab.GetComponent<Deck>();
            deck.Initialise();
            deck.Populate();
            deck.Shuffle();
            StartCoroutine(InitialDeal());
        }

        // Prépare le jeu pour un nouveau départ.
        public void ReadyStart()
        {
            CloseMenu();
            canvas_1.gameObject.SetActive(false);
            canvas_2.gameObject.SetActive(true);
            canvas_4.gameObject.SetActive(false);
        }

        // Affiche les résultats du jeu avec un classement.
        public void GameResult(List<string> rankList)
        {
            CloseMenu();

            win1.text = $"1. {rankList[0]} (PRESIDENT)";
            win2.text = $"2. {rankList[1]} (VICE-PRESIDENT)";
            win3.text = $"3. {rankList[2]} (SERVANT) ";
            win4.text = $"4. {rankList[3]} (UNDER-SERVANT)";

            canvas_1.gameObject.SetActive(false);
            canvas_2.gameObject.SetActive(false);
            canvas_4.gameObject.SetActive(true);
        }

        // Quitte le jeu.
        public void RealKill()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Ouvre le menu.
        public void OpenMenu()
        {
            if (!isLoading)
            {
                canvas_3.gameObject.SetActive(true);
            }

        }

        // Ferme le menu.
        public void CloseMenu()
        {
            canvas_3.gameObject.SetActive(false);
        }

        // Fonction d'initialisation d'Awake (pas utilisée ici).
        void Awake()
        {

        }

        // Fonction de démarrage du jeu.
        void Start()
        {
            ReadyStart();
        }

        // Fonction de mise à jour du jeu.
        void Update()
        {
            if (!isStart)
            {
                return;
            }

            // Vérifie si le jeu est terminé.
            if (turn?.IsGameDone() ?? false)
            {
                var rankList = turn?.IsGameDoneList();
                RealEndNormal(rankList);
            }

            // Met à jour les boutons en fonction de l'état du joueur actuel.
            if (turn?.GetNowTurn() is Player now)
            {
                if (turn?.isSelected() ?? false)
                {
                    pass.GetComponentInChildren<Text>().text = "CANCEL";
                }
                else
                {
                    pass.GetComponentInChildren<Text>().text = "PASS";
                }

                submit.enabled = turn?.SelectedCardCanSubmit(now) ?? false;
                pass.enabled = !now.isHouse;

                turn?.MakeBlackCard(now);
                turn?.CheckPass();

                var remainCnt = turn?.GetPlayerCardsCnt();
                player1_cnt.text = $"Princess ({remainCnt[1]})";
                player2_cnt.text = $"Soldier ({remainCnt[2]})";
                player3_cnt.text = $"Fisherman ({remainCnt[3]})";
            }

            // Affiche un message de chargement si le jeu est en cours de chargement.
            if (isLoading)
            {
                pass.enabled = false; //désactive le bouton pass
                submit.enabled = false; //désactive le bouton submit
                status.text = $"Loading...";
            }

            // Gère le clic de la souris pour sélectionner une carte.
            if (Input.GetMouseButtonDown(0) && !isLoading)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit2);
                if (hit2.transform?.gameObject?.GetComponent<Card>() is Card clickedCard)
                {
                    if (!clickedCard.owner.isHouse && turn.GetNowTurn() == clickedCard.owner)
                    {
                        turn?.SelectCard(clickedCard.owner, clickedCard);
                    }
                }
            }
        }

        // Initialise les joueurs.
        private void InitPlayers()
        {
            MakePlayer(x: 0, y: -3.5f, isHouse: false, playerName: "player", buttonName: "Your turn");
            MakePlayer(x: -7f, y: 6.8f, isHouse: true, playerName: "com1", buttonName: "Princess's turn");
            MakePlayer(x: 0, y: 6.8f, isHouse: true, playerName: "com2", buttonName: "Solider's turn");
            MakePlayer(x: 7f, y: 6.8f, isHouse: true, playerName: "com3", buttonName: "Fisherman's turn");
            MakePlayer(x: 0, y: 0, isHouse: true, playerName: "trash", buttonName: "Trash's turn", isTrash: true);
        }

        // Crée un joueur dans le jeu.
        private void MakePlayer(float x, float y, bool isHouse, string playerName, string buttonName, bool isTrash = false)
        {
            float locationY = 1.5f;
            if (isHouse)
                locationY = -1.5f;

            GameObject playerClone = Instantiate(playerPrefab);
            Player player = playerClone.GetComponent<Player>();
            player.Initialise(x, y, isHouse, isTrash, buttonName);

            if (!isTrash)
            {
                turn.AddPlayer(player);
            }
            else
            {
                turn.trash = player;
            }
        }

        // Distribue les cartes initiales aux joueurs.
        private IEnumerator InitialDeal()
        {
            yield return new WaitForSeconds(1);
            int n = 13;
            while (n > 0)
            {
                foreach (var tmp in turn.GetTotalPlayer())
                {
                    deck.Deal(tmp);
                }
                yield return new WaitForSecondsRealtime(0.2F);
                n--;
            }
            isLoading = false;
        }
        
    }
}
