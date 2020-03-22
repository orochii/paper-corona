using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackjack : MonoBehaviour {
    public class CardData {
        public int icon;
        public int value;

        public CardData(int i, int v) {
            icon = i;
            value = v;
        }
    }

    [SerializeField] Card cardPrefab;
    [SerializeField] Transform yourHand;
    [SerializeField] Transform enemyHand;
    [SerializeField] Text yourCounter;
    [SerializeField] Text enemyCounter;
    [SerializeField] Vector3 cardOffset;
    [SerializeField] Vector3 cardOffsetRand;
    [SerializeField] GameObject playerOptions;
    [SerializeField] Text resultText;

    List<Card> yourCards = new List<Card>();
    List<Card> enemyCards = new List<Card>();
    int yourValue,yourShownValue;
    int enemyValue, enemyShownValue;
    List<CardData> deck = new List<CardData>();

    private void OnEnable() {
        GameState.Instance.MinigameOpen = true;
        Restart();
    }

    private void OnDisable() {
        GameState.Instance.MinigameOpen = false;
    }

    private void Restart() {
        result = EResult.NONE;
        phase = EPhase.START;
        playerOptions.SetActive(false);
        // Repopulate if necessary
        if (deck.Count < 8) {
            // Populate deck of cards
            deck.Clear();
            for (int i = 0; i < 4; i++) {
                for (int v = 1; v < 14; v++) {
                    // Create card data
                    CardData cd = new CardData(i, v);
                    // Append wherever
                    int rndIdx = UnityEngine.Random.Range(0, deck.Count);
                    deck.Insert(rndIdx, cd);
                }
            }
        }
        // Destroy all card objects
        foreach (Card c in yourCards) Destroy(c.gameObject);
        foreach (Card c in enemyCards) Destroy(c.gameObject);
        yourCards.Clear();
        enemyCards.Clear();
        // Give players their initial cards
        DrawCardEnemy(false);
        DrawCardEnemy();
        DrawCardPlayer();
        DrawCardPlayer();
        // Start game
        StartCoroutine(OnStart());
    }

    public void DrawCardPlayer() {
        (yourValue,yourShownValue) = DrawCard(true, yourHand, yourCards);
        yourCounter.text = "Valor:" + System.Environment.NewLine + yourShownValue;
    }
    public void DrawCardEnemy(bool faceUp=true) {
        (enemyValue,enemyShownValue) = DrawCard(faceUp, enemyHand, enemyCards);
        enemyCounter.text = "Valor:" + System.Environment.NewLine + enemyShownValue;
    }

    private void ShowEnemyCards() {
        foreach (Card card in enemyCards) card.Flip = true;
        enemyCounter.text = "Valor:" + System.Environment.NewLine + enemyValue;
    }

    public void PassTurn() {
        phase = EPhase.ENEMYTURN;
        playerOptions.SetActive(false);
        StartCoroutine(EnemyDecision());
    }

    private (int,int) DrawCard(bool faceUp, Transform handTransform, List<Card> cardAry) {
        // Do nothing if there are no cards available.
        if (deck.Count < 1) return (-1,-1);
        // Draw top card from deck.
        CardData card = deck[0];
        deck.RemoveAt(0);
        // Create card in the board.
        Card newCard = Instantiate<Card>(cardPrefab, handTransform);
        newCard.Set(card.icon, card.value);
        if (faceUp) newCard.Flip = true;
        cardAry.Add(newCard);
        Vector3 cardOffRnd = new Vector3(UnityEngine.Random.value * cardOffsetRand.x, UnityEngine.Random.value * cardOffsetRand.y, UnityEngine.Random.value * cardOffsetRand.z);
        newCard.targetPos = cardOffset * cardAry.Count + cardOffRnd;
        // Recount card values
        int aces = 0;
        int shownAces = 0;
        int totalValue = 0;
        int shownTotalValue = 0;
        foreach (Card c in cardAry) {
            // Backend value
            int v = c.Value;
            if (v > 10) v = 10;
            if (v == 1) aces++;
            totalValue += v;
            // Shown value
            if (c.Flip) {
                if (v == 1) shownAces++;
                shownTotalValue += v;
            }
        }
        while (aces > 0 && totalValue < 13) {
            totalValue += 9;
            aces--;
        }
        while (shownAces > 0 && shownTotalValue < 13) {
            totalValue += 9;
            shownTotalValue--;
        }
        return (totalValue,shownTotalValue);
    }

    public enum EPhase {
        START, PLAYERTURN, ENEMYTURN, END, CLOSE
    }
    public enum EResult {
        NONE, LOSE, WIN, DRAW
    }

    EPhase phase;
    EResult result;

    public IEnumerator OnStart() {
        yield return new WaitForSeconds(1);
        phase = EPhase.PLAYERTURN;
        playerOptions.SetActive(true);
    }

    public IEnumerator EnemyDecision() {
        yield return new WaitForSeconds(0.7f);
        bool stop = false;
        while (!stop) {
            if (enemyValue < 13) {
                DrawCardEnemy();
            } else {
                int prob = UnityEngine.Random.Range(13, 19);
                if (prob < enemyValue) stop = true;
                else DrawCardEnemy();
            }
            yield return new WaitForSeconds(0.05f * enemyValue);
        }
        // Start judge
        phase = EPhase.END;
    }

    private void Update() {
        if (result != EResult.NONE) return;
        if (phase == EPhase.END) {
            Judge();
        }
    }

    private void Judge() {
        // See if player lost
        bool playerLost = yourValue > 21;
        bool enemyLost = enemyValue > 21;
        // Draw
        if (playerLost && enemyLost) {
            result = EResult.DRAW;
        } else {
            if (playerLost) result = EResult.LOSE;
            else if (enemyLost) result = EResult.WIN;
            else if (yourValue == enemyValue) result = EResult.DRAW;
            else {
                if (yourValue < enemyValue) result = EResult.LOSE;
                else result = EResult.WIN;
            }
        }
        StartCoroutine(ShowResult());
    }

    private IEnumerator ShowResult() {
        ShowEnemyCards();
        resultText.text = result.ToString();
        resultText.gameObject.SetActive(true);
        resultText.color = Color.clear;
        float perc = 0;
        while (perc < 1) {
            resultText.color = Color.Lerp(Color.clear, Color.white, perc);
            perc += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        perc = 0;
        while (perc < 1) {
            resultText.color = Color.Lerp(Color.clear, Color.white, 1-perc);
            perc += Time.deltaTime;
            yield return null;
        }
        resultText.gameObject.SetActive(false);
        if (result == EResult.DRAW) {
            Restart();
        } else {
            if (result == EResult.WIN) GameState.CallResult = true;
            yield return new WaitForSeconds(1.2f);
            phase = EPhase.CLOSE;
            gameObject.SetActive(false);
        }
    }
}
