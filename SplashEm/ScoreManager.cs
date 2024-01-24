using System;

public class ScoreManager
{
    public int score;

    // Property za dostop do trenutnega števila točk
    public int Score => score;

    // Dogodek, ki se sproži ob vsakem novem dosežku
    public event Action<int> OnScoreUpdated;

    // Konstruktor
    public ScoreManager()
    {
        // Lahko dodate dodatno inicializacijo, če je potrebno
        score = 0;
    }

    // Metoda za posodobitev števca točk
    public void UpdateScore(int points)
    {
        score += points;

        // Sprožite dogodek ob posodobitvi rezultata
        OnScoreUpdated?.Invoke(score);
    }

    // Metoda za ponastavitev števca točk
    public void ResetScore()
    {
        score = 0;
        OnScoreUpdated?.Invoke(score);
    }
}
