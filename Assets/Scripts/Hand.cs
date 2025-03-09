using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Hand : MonoBehaviour
    {
        public GameObject cardPrefab;  // Référence au prefab de carte
        public Sprite[] faces;  // Tableau de sprites représentant les faces des cartes
        public List<Card> cards;  // Liste des cartes détenues par le joueur

        // Initialise la main de cartes, vide la liste des cartes.
        public void Initialise()
        {
            cards = new List<Card>();
        }

        // Trie les cartes par rang (du plus bas au plus haut).
        public void Sort()
        {
            cards.Sort((a, b) =>
            {
                // Compare les rangs des cartes pour déterminer leur ordre
                if ((int)a.Rank > (int)b.Rank)
                {
                    return 1;
                }
                else if ((int)a.Rank < (int)b.Rank)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
        }

        // Ajoute une carte à la main du joueur, et lui assigne un propriétaire.
        public virtual void Add(Card c, Player owner)
        {
            c.owner = owner;  // Assigne le propriétaire à la carte
            cards.Add(c);  // Ajoute la carte à la liste
        }

        // Vide la main en détruisant toutes les cartes et en réinitialisant la liste.
        public void Clear()
        {
            foreach (Card c in cards)
            {
                c.DestroyCard();  // Détruit chaque carte
            }
            cards.Clear();  // Vide la liste des cartes
        }

        // retourner toutes les cartes face visible.
        //public void FlipFaceUp()
        //{
        //    foreach (Card c in cards)
        //    {
        //        if (!c.isFaceUp) c.Flip();  // Retourne la carte si elle est face cachée
        //    }
        //}
    }
}