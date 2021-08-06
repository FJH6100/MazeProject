using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct GridSquare
{
    public bool up;
    public bool left;
    public bool right;
    public bool down;
    public int xValue;
    public int yValue;
    public bool visited;
}

public class GameManager : MonoBehaviour
{    
    public GameObject square;
    public GameObject vertiWall;
    public GameObject horiWall;
    public GameObject pathfinder;
    Vector3 point;
    private Camera cam;
    public int x = 10;
    private int startX = 0;
    private int startY = 0;
    private int xCoord;
    private int yCoord;
    GridSquare[,] grid;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
		cam.transform.position = new Vector3(x/2 - 1, x/2 - .5f, cam.transform.position.z);
		cam.orthographicSize = x/2 + 1;
        grid = new GridSquare[x, x];
        GenerateMazeData();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Instantiate(square, new Vector2(i, j), Quaternion.identity);
                if (i != x - 1)
                {
                    if (grid[i,j].right == true)
                        Instantiate(vertiWall, new Vector3(i + .5f, j, -1), Quaternion.identity);
                }
                if (j != x - 1)
                {
                    if (grid[i,j].up == true)
                        Instantiate(horiWall, new Vector3(i, j + .5f, -1), Quaternion.identity);
                }
            }
        }
        for (int j = 0; j < x; j++)
        {
            Instantiate(vertiWall, new Vector2(-.5f, j), Quaternion.identity);
            if (j != 0)
                Instantiate(horiWall, new Vector2(j, -.5f), Quaternion.identity);
            Instantiate(vertiWall, new Vector2(x - .5f, j), Quaternion.identity);
            if (j != x - 1)
                Instantiate(horiWall, new Vector2(j, x - .5f), Quaternion.identity);
        }
    }

    void GenerateMazeData()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if (i != x - 1)
                {
                    grid[i, j].right = true;
                }
                if (j != x - 1)
                {
                    grid[i, j].up = true;
                }
                if (i > 0)
                {
                    grid[i,j].left = grid[i - 1,j].right;
                }
                if (j > 0)
                {
                    grid[i,j].down = grid[i,j - 1].up;
                }
                grid[i, j].xValue = i;
                grid[i, j].yValue = j;
            }
        }
        VisitSquare(ref grid);      
    }

    void VisitSquare(ref GridSquare[,] grid)
    {
        Stack<GridSquare> marked = new Stack<GridSquare>();
        int firstNumber = Random.Range(0, x - 1);
        int secondNumber = Random.Range(0, x - 1);
        marked.Push(grid[firstNumber, secondNumber]);
        grid[firstNumber, secondNumber].visited = true;
        while (marked.Count > 0)
        {
            marked.Pop();
            List<GridSquare> neighbors = new List<GridSquare>();
            if (grid[firstNumber, secondNumber].up == true)
            {
                if (!grid[firstNumber, secondNumber + 1].visited)
                    neighbors.Add(grid[firstNumber, secondNumber + 1]);
            }
            if (grid[firstNumber, secondNumber].left == true)
            {
                if (!grid[firstNumber - 1, secondNumber].visited)
                    neighbors.Add(grid[firstNumber - 1, secondNumber]);
            }
            if (grid[firstNumber, secondNumber].right == true)
            {
                if (!grid[firstNumber + 1, secondNumber].visited)
                    neighbors.Add(grid[firstNumber + 1, secondNumber]);
            }
            if (grid[firstNumber, secondNumber].down == true)
            {
                if (!grid[firstNumber, secondNumber - 1].visited)
                    neighbors.Add(grid[firstNumber, secondNumber - 1]);
            }

            if (neighbors.Count > 0)
            {
                marked.Push(grid[firstNumber, secondNumber]);
                GridSquare g = neighbors[Random.Range(0, neighbors.Count)];
                if (g.xValue < firstNumber)
                {
                    grid[g.xValue, g.yValue].right = false;
                    grid[firstNumber, secondNumber].left = false;
                }
                else if (g.xValue > firstNumber)
                {
                    grid[firstNumber, secondNumber].right = false;
                    grid[g.xValue, g.yValue].left = false;
                }
                else if (g.yValue < secondNumber)
                {
                    grid[g.xValue, g.yValue].up = false;
                    grid[firstNumber, secondNumber].down = false;
                }
                else if (g.yValue > secondNumber)
                {
                    grid[firstNumber, secondNumber].up = false;
                    grid[g.xValue, g.yValue].down = false;
                }
                marked.Push(grid[g.xValue, g.yValue]);
                grid[g.xValue, g.yValue].visited = true;
                firstNumber = g.xValue;
                secondNumber = g.yValue;
            }
            else
            {
                if (marked.Count > 0)
                {
                    GridSquare end = marked.Peek();
                    firstNumber = end.xValue;
                    secondNumber = end.yValue;
                }
            }
        }       
    }

    void Pathfind(int xStart, int yStart, int xFinish, int yFinish)
    {
        int currentX = xStart;
        int currentY = yStart;
        Stack<GridSquare> path = new Stack<GridSquare>();
        List<GridSquare> explored = new List<GridSquare>();
        explored.Add(grid[currentX, currentY]);
        while (currentX != xFinish || currentY != yFinish)
        {
            List<GridSquare> neighbors = new List<GridSquare>();
            if (currentY < x - 1)
            {
                if (!grid[currentX, currentY].up && !explored.Contains(grid[currentX, currentY + 1]))
                    neighbors.Add(grid[currentX, currentY + 1]);
            }
            if (currentY > 0)
            {
                if (!grid[currentX, currentY].down && !explored.Contains(grid[currentX, currentY - 1]))
                    neighbors.Add(grid[currentX, currentY - 1]);
            }
            if (currentX < x - 1)
            {
                if (!grid[currentX, currentY].right && !explored.Contains(grid[currentX + 1, currentY]))
                    neighbors.Add(grid[currentX + 1, currentY]);
            }
            if (currentX > 0)
            {
                if (!grid[currentX, currentY].left && !explored.Contains(grid[currentX - 1, currentY]))
                    neighbors.Add(grid[currentX - 1, currentY]);
            }
            if (neighbors.Count != 0)
            {
				if (!path.Contains(grid[currentX,currentY]));
					path.Push(grid[currentX,currentY]);
                int minValue = int.MaxValue;
                foreach (GridSquare n in neighbors)
                {
                    int f = (Mathf.Abs(n.xValue - xStart) + Mathf.Abs(n.yValue - yStart)) + (Mathf.Abs(n.xValue - xFinish) + Mathf.Abs(n.yValue - yFinish));
                    if (f < minValue)
                    {
                        minValue = f;
                        currentX = n.xValue;
                        currentY = n.yValue;
                    }   
                }
                explored.Add(grid[currentX, currentY]);
                path.Push(grid[currentX, currentY]);
            }
            else
            {
                GridSquare previous = path.Pop();
                currentX = previous.xValue;
                currentY = previous.yValue;
            }
        }
        StartCoroutine(DrawPath(path));
    }

    IEnumerator DrawPath(Stack<GridSquare> s)
    {
        GridSquare[] nArray = s.ToArray();
        for (int i = s.Count - 1; i > -1; i--)
        {
			Instantiate(pathfinder, new Vector3(nArray[i].xValue, nArray[i].yValue, -1), Quaternion.identity);
            //pathfinder.transform.position = new Vector3(nArray[i].xValue, nArray[i].yValue, -1);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        point = cam.ScreenToWorldPoint(Input.mousePosition);
        xCoord = Mathf.RoundToInt(point.x);
        yCoord = Mathf.RoundToInt(point.y);
        if (Input.GetMouseButtonDown(0))
        {
            if (xCoord > -1 && xCoord < x && yCoord > -1 && yCoord < x)
            {
                Pathfind(startX, startY, xCoord, yCoord);
                startX = xCoord;
                startY = yCoord;
            }
        }
    }
}
