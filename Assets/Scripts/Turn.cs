using Game;
using System.Collections.Generic;
using UnityEngine.UI;

public class Turn : WinManager
{
    private int nowIndex = 0;  // Index du joueur dont c'est le tour
    private readonly List<Player> nowPlayer = new List<Player>();  // Liste des joueurs actuellement dans le jeu
    private readonly List<Player> totalPlayer = new List<Player>();  // Liste de tous les joueurs
    public Player trash;  // Joueur "trash" (peut être un joueur spécial)
    public Text statusText;  // Affichage du statut du tour

    // Initialisation des variables ou paramètres supplémentaires (actuellement vide)
    public new void Initialise()
    {
    }

    // Ajoute un joueur à la liste des joueurs actuels et totaux
    public void AddPlayer(Player a)
    {
        nowPlayer.Add(a);
        totalPlayer.Add(a);
    }

    // Récupère le nombre de cartes de chaque joueur dans une liste
    public int[] GetPlayerCardsCnt()
    {
        var total = new int[4] { 0, 0, 0, 0 };  // Initialise un tableau pour les compteurs de cartes
        var index = 0;
        foreach (var tmp in GetTotalPlayer())
        {
            total[index] = tmp.cards.Count;  // Récupère le nombre de cartes du joueur
            index++;
        }
        return total;  // Retourne le tableau des compteurs
    }

    // Retourne la liste complète de tous les joueurs
    public List<Player> GetTotalPlayer()
    {
        return totalPlayer;
    }

    // Retourne le nombre total de joueurs dans la liste actuelle
    public int NowPlayerCount()
    {
        return nowPlayer.Count;
    }

    // Supprime un joueur de la liste des joueurs actuels
    public void DelNowPlayer(Player a)
    {
        nowPlayer.Remove(a);
    }

    // Récupère le joueur dont c'est actuellement le tour
    public Player GetNowTurn()
    {
        if (nowPlayer.Count > 0)
        {
            try
            {
                return nowPlayer[nowIndex];  // Retourne le joueur correspondant à l'index actuel
            }
            catch
            {
                nowIndex = 0;  // Si un problème survient, réinitialise l'index à 0
                return nowPlayer[0];  // Retourne le premier joueur
            }
        }
        else
        {
            return null;  // Retourne null si la liste des joueurs est vide
        }
    }

    // Passe au joueur suivant pour le tour
    public void NextTurn()
    {
        nowIndex += 1; 
        if (nowPlayer.Count <= nowIndex)  
        {
            nowIndex = 0;  // Réinitialise l'index à 0 pour repartir au début
        }
        ChangeStatusNowTurn();  // Met à jour l'affichage du statut du tour
    }

    // Change le texte du statut du tour (ex : "C'est le tour de Player1")
    public void ChangeStatus(string tt)
    {
        statusText.text = tt;
    }

    // Met à jour l'affichage du statut pour indiquer le joueur dont c'est le tour
    public void ChangeStatusNowTurn()
    {
        if (GetNowTurn() != null)  // Si un joueur est disponible
        {
            ChangeStatus($"{GetNowTurn().name}");  
        }
        else
        {
            ChangeStatus($"Game Done!");  
        }
    }
}
