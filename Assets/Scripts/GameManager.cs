using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum States
{
    CanMove,
    CantMove
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BoxCollider2D collider;
    public GameObject token1, token2;
    public int Size = 3;
    public int[,] Matrix;
    [SerializeField] private States state = States.CanMove;
    public Camera camera;
    void Start()
    {
        Instance = this;
        Matrix = new int[Size, Size];
        Calculs.CalculateDistances(collider, Size);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Matrix[i, j] = 0; // 0: desocupat, 1: fitxa jugador 1, -1: fitxa IA;
            }
        }
    }
    private void Update()
    {
        if (state == States.CanMove)
        {
            Vector3 m = Input.mousePosition;
            m.z = 10f;
            Vector3 mousepos = camera.ScreenToWorldPoint(m);
            if (Input.GetMouseButtonDown(0))
            {
                if (Calculs.CheckIfValidClick((Vector2)mousepos, Matrix))
                {
                    state = States.CantMove;
                    if(Calculs.EvaluateWin(Matrix)==2)
                        StartCoroutine(WaitingABit());
                }
            }
        }
    }
    private IEnumerator WaitingABit()
    {
        yield return new WaitForSeconds(1f);
        RandomAI();
    }
    public void RandomAI()
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        int x = 0;
        int y = 0;

        int bestValue = int.MinValue;
        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                if (Matrix[i, j] == 0)
                {
                    // La IA hace un movimiento
                    Matrix[i, j] = -1;

                    // Aumentamos la profundidad a 5
                    int value = MinimaxAlgorithm(Matrix, 5, true, alpha, beta); 
                    if (value > bestValue)
                    {
                        bestValue = value;
                        x = i;
                        y = j;
                    }
                    // Deshacemos el movimiento
                    Matrix[i, j] = 0; 

                    // Poda alfa-beta
                    alpha = Mathf.Max(alpha, bestValue);
                    if (beta <= alpha)
                        break; // Poda beta
                }
            }
        }

        DoMove(x, y, -1);
        state = States.CanMove;

    }

    public int MinimaxAlgorithm(int[,] matrix, int maxDepth, bool isPlayerMinimizer, int alpha, int beta)
    {
        int result = Calculs.EvaluateWin(matrix);
        if (result == 1) return -1; // El jugador ha ganado, valor negativo para la IA
        else if (result == -1) return 1; // La IA ha ganado, valor positivo para la IA
        else if (result == 0) return 0; // Empate
        else if (maxDepth == 0) return 0; // No más profundidad, retornamos 0 como empate

        if (isPlayerMinimizer) // Es el turno del jugador (quien minimiza)
        {
            int bestValue = int.MaxValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        matrix[i, j] = 1; // El jugador pone su ficha
                        bestValue = Mathf.Min(bestValue, MinimaxAlgorithm(matrix, maxDepth - 1, false, alpha, beta));
                        matrix[i, j] = 0; // Deshacemos el movimiento

                        beta = Mathf.Min(beta, bestValue);
                        if (beta <= alpha)
                            break; // Poda beta
                    }
                }
            }
            return bestValue;
        }
        else // Es el turno de la IA (quien maximiza)
        {
            int bestValue = int.MinValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        matrix[i, j] = -1; // La IA pone su ficha
                        bestValue = Mathf.Max(bestValue, MinimaxAlgorithm(matrix, maxDepth - 1, true, alpha, beta));
                        matrix[i, j] = 0; // Deshacemos el movimiento

                        alpha = Mathf.Max(alpha, bestValue);
                        if (beta <= alpha)
                            break; // Poda alpha
                    }
                }
            }
            return bestValue;
        }
    }


    /// <summary>
    /// Sirve para mover una ficha en la matriz y en el tablero
    /// </summary>
    /// <param name="x">posicion x del tablero</param>
    /// <param name="y">posicion y del tablero</param>
    /// <param name="team">que jugador es quien estaba poniendo ficha jugador = 1 y la ia = -1</param>
    public void DoMove(int x, int y, int team)
    {
        Matrix[x, y] = team;
        if (team == 1)
            Instantiate(token1, Calculs.CalculatePoint(x, y), Quaternion.identity);
        else
            Instantiate(token2, Calculs.CalculatePoint(x, y), Quaternion.identity);
        int result = Calculs.EvaluateWin(Matrix);
        switch (result)
        {
            case 0:
                Debug.Log("Draw");
                break;
            case 1:
                Debug.Log("You Win");
                break;
            case -1:
                Debug.Log("You Lose");
                break;
            case 2:
                if(state == States.CantMove)
                    state = States.CanMove;
                break;
        }
    }
}
