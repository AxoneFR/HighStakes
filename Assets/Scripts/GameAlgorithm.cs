using Game;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameAlgorithm : Turn
{
    private int status = -1;
    private int count = -1;
    private Card nowCard = null;

    private int statusSelected = -1;
    private int countSelected = -1;
    private Card nowCardSelectedLow = null;
    private List<Card> nowCardSelected = null;
    private int passCnt = 0;

    private bool Allow8Clear = true;

    // Initialise le jeu avec le texte de statut et le paramètre Allow8Clear.
    public new void Initialise(Text status, bool Allow8Clear)
    {
        statusText = status;
        this.Allow8Clear = Allow8Clear;
    }

    // Vérifie si des cartes sont sélectionnées.
    public bool isSelected()
    {
        return (nowCardSelected?.Count ?? 0) > 0;
    }

    // Définit l'état des cartes sélectionnées.
    private void SetSelected()
    {
        status = statusSelected;
        count = countSelected;
        nowCard = nowCardSelectedLow;
    }

    // Réinitialise les cartes sélectionnées.
    private void ResetSelected()
    {
        statusSelected = -1;
        countSelected = -1;
        nowCardSelectedLow = null;
        nowCardSelected = null;
    }

    // Réinitialise la carte actuellement jouée.
    private void ResetCard()
    {
        status = -1;
        count = -1;
        nowCard = null;
    }

    // Vérifie si un joueur peut annuler son action.
    public bool CanCancel(Player p)
    {
        return !p.isHouse && nowCardSelected?.Count > 0;
    }

    // Vérifie si les cartes sélectionnées peuvent être soumises.
    public bool SelectedCardCanSubmit(Player p)
    {
        if(!p.isHouse && nowCardSelected != null && (nowCard == null || nowCardSelected[0].Rank >= nowCard?.Rank)) //carte sup ou égale
        {
            if (nowCardSelected?.Count == 1 && (count == -1 || count == 1))
            {
                statusSelected = 0;
                countSelected = 1;
                nowCardSelectedLow = nowCardSelected[0];
                return true;
            }
            else if (nowCardSelected?.Count > 1 && (count == -1 || count == nowCardSelected?.Count))
            {
                SortCard(nowCardSelected);
                bool isSameRank = true;
                bool isSameSuit = true;
                bool isContinuous = true;
                Card.Ranks tmpRank = nowCardSelected[0].Rank;
                Card.Suits tmpSuit = nowCardSelected[0].Suit;
                Card.Ranks tmpstartRank = nowCardSelected[0].Rank;
                foreach (var tmp in nowCardSelected)
                {
                    if (tmpRank != tmp.Rank)
                    {
                        isSameRank = false;
                    }
                    if (tmpSuit != tmp.Suit)
                    {
                        isSameSuit = false;
                    }
                    if (tmpstartRank != tmp.Rank)
                    {
                        isContinuous = false;
                    }
                    tmpstartRank++;
                }

                if (isSameRank)
                {
                    statusSelected = 1;
                    countSelected = nowCardSelected.Count;
                    nowCardSelectedLow = nowCardSelected[0];
                    return true;
                }

                else if (isSameSuit && isContinuous && nowCardSelected?.Count > 2)
                {
                    statusSelected = 2;
                    countSelected = nowCardSelected.Count;
                    nowCardSelectedLow = nowCardSelected[0];
                    return true;
                }
            }
        }
        return false;
    }

    // Trie les cartes par rang.
    private void SortCard(List<Card> cards)
    {
        cards.Sort((a, b) =>
        {
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

    // Désactive les cartes pour tous les joueurs sauf le joueur actuel.
    public void MakeBlackCard(Player now)
    {
        foreach (var tmp in this.GetTotalPlayer())
        {
            tmp.CardDisable(tmp != now);
        }
    }

    // Annule la sélection des cartes.
    public void CancelClick()
    {
        if (GetNowTurn() is Player p)
        {
            if (!p.isHouse)
            {
                nowCardSelected = null;
                foreach (var tmp in p.cards)
                {
                    tmp.Disable(false);
                }
                GetNowTurn().SortCard();
            }
        }
    }

    // Soumet les cartes sélectionnées.
    public void SubmitClick()
    {
        if (nowCardSelected?.Count > 0)
        {
            bool is8Clear = false;
            SoundManager.instance.PlaySound("soundCard2");

            var now = GetNowTurn();
            var deck = now.cards;
            foreach (var tmp in nowCardSelected)
            {
                if(Allow8Clear && tmp.Rank == Card.Ranks.Two)
                {
                    is8Clear = true;
                }

                deck.Remove(tmp);
                trash.Add(tmp);
            }
            SortCard(nowCardSelected);
            trash.CardTrash(nowCardSelected);

            now.SortCard();
            print($"{now.cards.Count}");
            if (now.cards.Count == 0)
            {
                this.DelNowPlayer(now);
                AddWinner(now);
            }

            SetSelected();
            ResetSelected();
            passCnt = 0;

            if (is8Clear)
            {
                ChangeStatus("Stack cleared!");
                StartCoroutine(_8Clear());
            }
            else
            {
                NextTurn();
                StartCoroutine(AI());
            }
        }
    }

    // Gère l'effet de suppression de pile (8Clear).
    public IEnumerator _8Clear()
    {
        yield return new WaitForSeconds(1);

        ChangeStatusNowTurn();
        CheckPass(forceWin: true);
        StartCoroutine(AI());
    }

    // Gère le passage du tour pour le joueur actuel.
    public void PassClick()
    {
        if (isSelected())
        {
            CancelClick();
        }
        else
        {
            GetNowTurn().SortCard();
            SoundManager.instance.PlaySound("soundCard3");
            ResetSelected();
            passCnt += 1;
            NextTurn();
            StartCoroutine(AI());
        }
    }

    // Sélectionne ou désélectionne une carte.
    public void SelectCard(Player p, Card wantTrash)
    {
        if (!wantTrash.isDisabled)
        {
            if(!nowCardSelected?.Contains(wantTrash) ?? true)
            {

                if (nowCardSelected == null)
                {
                    nowCardSelected = new List<Card>();
                }
                nowCardSelected.Add(wantTrash);

                var position = wantTrash.gameObject.transform.position;
                iTween.MoveTo(wantTrash.gameObject, new Vector2(position.x, -2.5f), 1f);
            }
            else
            {
                nowCardSelected.Remove(wantTrash);
                if (nowCardSelected.Count == 0)
                {
                    ResetSelected();
                }
                var position = wantTrash.gameObject.transform.position;
                iTween.MoveTo(wantTrash.gameObject, new Vector2(position.x, -3.5f), 1f);
            }
        }
    }

    // Vérifie si un joueur doit passer ou si la partie se termine.
    public void CheckPass(bool forceWin = false)
    {
        //Debug.Log($"{passCnt + 1} {NowPlayerCount()}");
        if (passCnt + 1 >= NowPlayerCount() || forceWin)
        {
            ResetCard();

            foreach (var tmp in trash.cards.ToArray())
            {
                trash.cards.Remove(tmp);
                iTween.MoveTo(tmp.gameObject, new Vector2(5, 0.15f), 1f);
                tmp.Hide();
            }
        }
    }

    // Termine le jeu et détruit les objets associés.
    public void EndGame()
    {
        try
        {
            foreach (var p in GetTotalPlayer())
            {
                DestroyPlayer(p);
            }
            DestroyPlayer(trash);
        }
        catch
        {

        }
        
    }

    // Détruit un joueur et ses cartes.
    public void DestroyPlayer(Player p)
    {
        p.Clear();
        foreach (var tmp in p.cards)
        {
            tmp.Hide();
            iTween.MoveTo(tmp.gameObject, new Vector2(5, 0.15f), 1f);
            Destroy(tmp.gameObject, 1f);
            //tmp.DestroyCard();
        }
        Destroy(p.gameObject);
    }

    // Gère les actions du joueur IA pendant son tour.
    public IEnumerator AI()
    {
        yield return new WaitForSeconds(1);

        try
        {
            var p = GetNowTurn();
            if (p.isHouse)
            {
                ResetSelected();

                if (nowCard != null)
                {
                    if (status == 0) 
                    {
                        List<Card> tmpNeed = new List<Card>();
                        foreach (var tmp in p.cards)
                        {
                            if (tmp.Rank > nowCard.Rank)
                            {
                                statusSelected = status;
                                countSelected = count;
                                nowCardSelectedLow = tmp;
                                nowCardSelected = new List<Card> { tmp };
                                break;
                            }
                        }
                    }
                    else if (status == 1)
                    {
                        int need = count;
                        List<Card> tmpNeed = new List<Card>();
                        Card tmpCard = null;

                        foreach (var tmp in p.cards)
                        {
                            if (tmp.Rank <= nowCard.Rank)
                            {
                                continue;
                            }

                            if (tmpCard == null || tmpCard.Rank == tmp.Rank)
                            {
                                tmpCard = tmp;
                                tmpNeed.Add(tmp);
                            }
                            else
                            {
                                tmpNeed = new List<Card>();
                                tmpCard = null;
                            }

                            if (tmpNeed.Count == need)
                            {
                                statusSelected = status;
                                countSelected = count;
                                nowCardSelectedLow = tmp;
                                nowCardSelected = tmpNeed;
                                break;
                            }
                        }
                    }
                    else if (status == 2)
                    {
                        int need = count;
                        List<Card> tmpNeed = new List<Card>();
                        Card tmpCard = null;

                        List<Card.Suits> suits = new List<Card.Suits> { Card.Suits.Club, Card.Suits.Diamond, Card.Suits.Heart, Card.Suits.Spade };

                        foreach (var suit in suits)
                        {
                            tmpNeed = new List<Card>();
                            tmpCard = null;

                            foreach (var tmp in p.cards)
                            {
                                if (tmp.Rank <= nowCard.Rank || tmp.Suit != suit)
                                {
                                    continue;
                                }

                                if (tmpCard != null && tmpCard.Rank + 1 != tmp.Rank)
                                {
                                    tmpNeed = new List<Card>();
                                }

                                tmpCard = tmp;
                                tmpNeed.Add(tmp);

                                if (tmpNeed.Count == need)
                                {
                                    statusSelected = status;
                                    countSelected = count;
                                    nowCardSelectedLow = tmp;
                                    nowCardSelected = tmpNeed;
                                    break;
                                }
                            }
                        }


                    }
                }
                else
                {
                    statusSelected = 0;
                    countSelected = 1;
                    nowCardSelectedLow = p.cards[0];
                    nowCardSelected = new List<Card> { p.cards[0] };
                }

                if (nowCardSelected?.Count > 0)
                {
                    SubmitClick();
                }
                else
                {
                    PassClick();
                }


            }
        }
        catch
        {

        }
    }
}
