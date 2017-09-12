using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour {
    public List<string> possibleDirections;
    public Dictionary<string, Transform> directions = new Dictionary<string, Transform>();
    public List<int> location = new List<int>();
    public bool visited;
    public double startOpacity = .1;
    public double visitedOpacity = .5;
    public double finishedOpacity = 1;
    
	void Awake()  {
        this.visited = false;
        possibleDirections = new List<string> { "north", "east", "south", "west" };
        //this.directions = new Dictionary<string, Transform>();
    }

    public void visit() {
        this.visited = true;
    }

    public void finish() {
    }

    public void SetPossibleDirections(List<string> possibleDirections) {
        this.possibleDirections = possibleDirections;
    }

    public void RemovePossibleDirection(string direction)
    {
        this.possibleDirections.Remove(direction);
    }

    public void SetLocation(int x, int z)
    {
        location.Add(x);
        location.Add(z);
    }

    public void GetLocation(out int x, out int z)
    {
        x = location[0];
        z = location[1];
    }

    public void Visit() {
        this.gameObject.SetActive(true);
        this.visited = true;
    }

    public List<string> AvailableDirections()
    {
        List<string> returnDirections = new List<string>();

        foreach(string direction in this.possibleDirections) {
            if(!this.directions.ContainsKey(direction)) {
                returnDirections.Add(direction);
            }
        }

        return returnDirections;
    }

    public void OpenDirection(string direction)
    {
        if (this.possibleDirections.Contains(direction))
        {
            Debug.Log("OpenDirection '" + direction + "' for '" + this.gameObject.name + "'");

            var beams = this.transform.Find("Beams_" + direction);

            if (beams)
            {
                beams.gameObject.SetActive(false);
            }
        }

        return;
    }

    public bool OpenDirection(string direction, Transform neighborRoom)
    {
        bool added = false;
        var neighborRoomName = neighborRoom.name;

        if (this.possibleDirections.Contains(direction))
        {
            this.directions.Add(direction, neighborRoom);
            var beams = this.transform.Find("Beams_" + direction);

            if (beams)
            {
                beams.gameObject.SetActive(false);
            }

            added = true;
        }

        return added;
    }
}
