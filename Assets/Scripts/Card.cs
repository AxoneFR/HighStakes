using UnityEngine;

namespace Game
{
    public class Card : MonoBehaviour
    {
        // Définition des différentes couleurs d'une carte
        public enum Suits
        {
            Diamond, Club, Heart, Spade
        }

        // Définition des différentes valeurs d'une carte
        public enum Ranks
        {
            Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Two
        }

        // Propriétés de la carte
        public Suits Suit { get; set; } // Couleur de la carte
        public Ranks Rank { get; set; } // Valeur de la carte
        public Sprite back; // Image du dos de la carte
        private Sprite face; // Image de la face de la carte
        private SpriteRenderer _spriteRenderer; // Composant pour afficher l'image de la carte
        public bool isFaceUp; // Indique si la carte est face visible
        public Player owner; // Joueur propriétaire de la carte
        public bool isDisabled = false; // Indique si la carte est désactivée

        /// <summary>
        /// Initialise la carte avec ses valeurs, sa position, sa rotation et son image de face.
        /// </summary>
        public void Initialise(Suits s, Ranks r, Vector2 pos, Quaternion rot, Sprite cardFace)
        {
            Suit = s;
            Rank = r;
            transform.position = pos;
            transform.rotation = rot;
            face = cardFace;
            isFaceUp = false;
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            BoxCollider myBC = (BoxCollider)gameObject.AddComponent(typeof(BoxCollider));
            //gameObject.GetComponent<BoxCollider>().enabled = true;
            //myBC.isTrigger = true;
        }

        /// <summary>
        /// Détruit l'objet carte.
        /// </summary>
        public void DestroyCard()
        {
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Active ou désactive visuellement la carte.
        /// </summary>
        public void Disable(bool status)
        {
            isDisabled = status;
            if (status)
            {
                _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Grise la carte
            }
            else
            {
                _spriteRenderer.color = new Color(1, 1, 1, 1f); // Remet la couleur d'origine
            }
        }

        /// <summary>
        /// Retourne la carte (face visible ou dos visible).
        /// </summary>
        public void Flip()
        {
            if (isFaceUp)
            {
                _spriteRenderer.sprite = back;
                isFaceUp = false;
            }
            else
            {
                _spriteRenderer.sprite = face;
                isFaceUp = true;
            }
        }

        /// <summary>
        /// Affiche la face de la carte.
        /// </summary>
        public void Show()
        {
            _spriteRenderer.sprite = face;
            isFaceUp = true;
        }

        /// <summary>
        /// Cache la face de la carte en affichant son dos.
        /// </summary>
        public void Hide()
        {
            _spriteRenderer.sprite = back;
            isFaceUp = false;
        }
    }
}
