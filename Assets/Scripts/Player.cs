using UnityEngine;
using Game;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class Player : Hand
    {
        public float xPos, yPos;  // Position du joueur sur l'axe X et Y
        public bool isHouse;  // Indique si le joueur est une "maison" (un NPC)
        public bool isTrash;  // Indique si le joueur est le joueur "trash"
        public string name;  // Nom du joueur
        private int trashOrder = 1;  // Ordre des cartes pour le joueur trash

        // Initialise les propriétés du joueur et de la main
        public void Initialise(float x, float y, bool houseOrPlayer, bool isTrash, string name)
        {
            base.Initialise();  // Appel du constructeur de la classe parente Hand
            xPos = x;
            yPos = y;
            isHouse = houseOrPlayer;
            this.isTrash = isTrash;
            this.name = name;
        }

        // Joue une carte si elle est valide, retourne la carte à jouer
        public Card PlayCard(Card c)
        {
            foreach (var tmp in this.cards)
            {
                // Si la carte est valide, retourne la carte
                if (c == null || tmp.Rank > c.Rank)
                {
                    return tmp;
                }
            }
            return null;  // Retourne null si aucune carte n'est valide
        }

        // Déplace les cartes vers le "trash" avec un certain effet visuel
        public void CardTrash(List<Card> cards)
        {
            int size = cards.Count;
            for (int i = 0; i < size; i++)
            {
                float cardDistance = 1.2f;
                cards[i].GetComponent<SpriteRenderer>().sortingOrder = trashOrder++;

                // Déplace les cartes à leur position "trash" avec un effet visuel
                iTween.MoveTo(cards[i].gameObject, new Vector2(cardDistance * (i - (size / 2)) + 0, 0.15f), 1f);
                cards[i].Show();  // Affiche la carte
            }
        }

        // Désactive ou active les cartes du joueur
        public void CardDisable(bool status)
        {
            foreach (var tmp in this.cards)
            {
                tmp.Disable(status);  // Change l'état de chaque carte
            }
        }

        // Trie les cartes et ajuste leur position en fonction du type de joueur (trash, maison, etc.)
        public void SortCard()
        {
            if (!isTrash)
            {
                Sort();  // Trie les cartes du joueur s'il n'est pas "trash"
            }

            int size = cards.Count;
            for (int i = 0; i < size; i++)
            {
                float cardDistance = 1f;
                if (isHouse && !isTrash)
                {
                    cardDistance = 0.3f;  // Distance réduite pour les maisons
                }

                float goLeft = 1.5f;
                if (isHouse && !isTrash)
                {
                    goLeft = 0;  // Ajuste la position de la maison
                }

                // Modifie l'ordre de tri des cartes
                cards[i].GetComponent<SpriteRenderer>().sortingOrder = i + 1;

                // Déplace les cartes selon les règles du jeu
                if (!isTrash)
                {
                    if (size / 2 <= i - 1)
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(cardDistance * (i - (size / 2)) + xPos - goLeft, yPos), 1f);
                    }
                    else
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(-1 * cardDistance * ((size / 2) - i) + xPos - goLeft, yPos), 1f);
                    }
                }
                else
                {
                    iTween.MoveTo(cards[i].gameObject, new Vector2(0, 0), 1f);  // Déplace les cartes "trash"
                }

                // Affiche ou cache les cartes en fonction du type de joueur
                if (isHouse && !isTrash)
                {
                    cards[i].Hide();  // Cache la carte si c'est une maison
                }
                else
                {
                    cards[i].Show();  // Affiche la carte sinon
                }
            }
        }

        // Ajoute une carte à la main du joueur et trie les cartes si nécessaire
        public void Add(Card c)
        {
            base.Add(c, this);  // Ajoute la carte à la main

            if (!isTrash)
            {
                SortCard();  // Trie les cartes si le joueur n'est pas "trash"
            }
        }
    }
}
