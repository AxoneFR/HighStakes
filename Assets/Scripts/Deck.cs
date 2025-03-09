using UnityEngine;

namespace Game
{
    public class Deck : Hand
    {
        /// <summary>
        /// Remplit le deck avec toutes les cartes d'un jeu classique (52 cartes).
        /// </summary>
        public void Populate()
        {
            for (int i = 0; i < 4; i++) // Boucle sur les 4 couleurs
            {
                for (int j = 0; j < 13; j++) // Boucle sur les 13 valeurs de carte
                {
                    GameObject card = Instantiate(cardPrefab); // Création d'une carte
                    Card c = card.GetComponent<Card>() as Card;
                    c.Initialise((Card.Suits)i, (Card.Ranks)j, new Vector2(5, 0.15f), Quaternion.identity, faces[(i * 13) + j]); // Initialisation de la carte
                    Add(c, null); // Ajout de la carte au deck
                }
            }
        }

        /// <summary>
        /// Affiche les cartes en les disposant sur une grille et en retournant une sur deux.
        /// </summary>
        public void DisplayCards()
        {
            float x = -3f;
            float y = 4.5f;
            for (int r = 0; r < 4; r++) // Parcours des rangées
            {
                for (int c = 0; c < 13; c++) // Parcours des colonnes
                {
                    int index = (r * 13) + c;
                    cards[index].transform.position = new Vector2(x + (c * .5f), y + (r * -3)); // Placement de la carte
                    if (c % 2 == 0) cards[index].Flip(); // Retourne une carte sur deux
                }
            }
        }

        /// <summary>
        /// Mélange les cartes du deck de manière aléatoire.
        /// </summary>
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i); // Sélection aléatoire d'un index
                (cards[i], cards[j]) = (cards[j], cards[i]); // Échange de position entre deux cartes
                cards[i].GetComponent<SpriteRenderer>().sortingOrder = i; // Mise à jour de l'ordre d'affichage
            }
        }

        /// <summary>
        /// Distribue la première carte du deck à un joueur.
        /// </summary>
        public void Deal(Player hand)
        {
            if (cards.Count != 0)
            {
                SoundManager.instance.PlaySound("soundCard2"); // Joue un son
                Card top = cards[0]; // Récupère la première carte
                cards.Remove(top); // Retire la carte du deck
                hand.Add(top); // Ajoute la carte à la main du joueur
            }
        }
    }
}
