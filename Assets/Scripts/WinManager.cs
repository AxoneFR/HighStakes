using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    public readonly List<Player> whoWin = new List<Player>();  // Liste des joueurs gagnants

    public void Initialise()
    {

    }

    // Ajoute un joueur à la liste des gagnants
    public void AddWinner(Player p)
    {
        whoWin.Add(p);
    }

    // Vérifie si le jeu est terminé (quand il y a 4 gagnants)
    public bool IsGameDone()
    {
        return whoWin.Count >= 4; 
    }

    // Retourne une liste contenant les noms des joueurs gagnants (modifié pour un format lisible)
    public List<string> IsGameDoneList()
    {
        var aa = new List<string>();  // Liste pour stocker les noms des gagnants
        foreach(var a in whoWin)
        {
            aa.Add(a.name.Replace("Your turn", "You").Replace("'s turn", ""));
        }
        return aa;  
    }
}