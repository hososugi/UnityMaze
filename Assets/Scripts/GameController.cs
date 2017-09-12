using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int MazeWidth = 4;
    public int MazeDepth = 4;
    public Transform RoomObject;
    public int visitedCount = 0;
    public Stack<Transform> visitedRooms;
    public int finishedCount = 0;
    public int roomWidth = 4;
    public int roomDepth = 4;
    public Transform startRoom;
    public Transform roomContainer;
    public List<string> AllDirections = new List<string> { "north", "east", "south", "west" };
    
    private int MazeStartX;
    private int MazeStartZ;
    private Transform[,] room2dArray;

	// Use this for initialization
	void Start () {
        MazeStartX = (int) -System.Math.Floor((double) (MazeWidth / 2)) * roomWidth;
        MazeStartZ = (int) System.Math.Floor((double) (MazeDepth / 2)) * roomDepth;
        visitedRooms = new Stack<Transform>();

        room2dArray = new Transform[MazeWidth, MazeDepth];
        roomContainer = GameObject.Find("RoomContainer").transform;

        //BuildMapGrid();
        CreateMaze();
    }

    void BuildMapGrid() {
        for(var z = 0; z < MazeDepth; z++)
        {
            for(var x = 0; x < MazeWidth; x++)
            {
                var room = Instantiate(RoomObject);
                int roomX = MazeStartX + (x * roomWidth);
                int roomZ = MazeStartZ + (z * roomDepth);

                room.name = "RoomObject_" + x + "_" + z;
                room.parent = roomContainer;
                room.transform.position = new Vector3(roomX, 0, roomZ);

                // Set up the available directions.
                var directions = new List<string> { "north", "east", "south", "west" };
                if (x == 0)
                {
                    directions.Remove("west");
                }
                if (x == MazeWidth - 1)
                {
                    directions.Remove("east");
                }
                if (z == 0)
                {
                    //directions.Remove("south");
                }
                if (z == MazeDepth - 1)
                {
                    directions.Remove("north");
                }

                var roomScript = room.GetComponent<Room>();
                roomScript.SetPossibleDirections(directions);
                roomScript.SetLocation(x, z);

                room2dArray[x, z] = room;
                
            }
        }
        Debug.Log("Done creating rooms.");

        CreateMaze();
    }

    public void CreateMaze()
    {
        int startX = Random.Range(0,2) * (MazeWidth-1);
        int startZ = Random.Range(0,2) * (MazeDepth-1);

        Debug.Log("Map start room: " + startX + "," + startZ);
        
        // Set the location of the main camera.
        var camera = GameObject.Find("Main Camera").transform;
        camera.position = new Vector3(startX, camera.position.y, startZ);

        // Loop through all the rooms until every room has been visited and confirmed finished.
        CreateRoom(startX, startZ);

        Debug.Log("Went through all the rooms.");       
    }

    private void CreateRoom(int x, int z, string movedDirection = null)
    {
        int movedDirectionIndex = AllDirections.IndexOf(movedDirection);
        string previousRoomDirection = (movedDirection != null)? AllDirections[(movedDirectionIndex + 2) % 4] : null;

        // Set up the current room.
        var room = Instantiate(RoomObject);
        int roomX = MazeStartX + (x * roomWidth);
        int roomZ = MazeStartZ + (z * roomDepth);

        room.name = "RoomObject_" + x + "_" + z;
        room.parent = roomContainer;
        room.transform.position = new Vector3(roomX, 0, roomZ);

        var roomScript = room.GetComponent<Room>();
        roomScript.SetLocation(x, z);

        room2dArray[x, z] = room;

        Room previousRoomScript;
        int previousRoomX = -1;
        int previousRoomZ = -1;
        
        if(previousRoomDirection != null)
        {
            roomScript.OpenDirection(previousRoomDirection);

            Transform previousRoom;
            
            switch (previousRoomDirection)
            {
                case "north":
                    previousRoom = room2dArray[x, z + 1];
                    break;
                case "east":
                    previousRoom = room2dArray[x + 1, z];
                    break;
                case "south":
                    previousRoom = room2dArray[x, z - 1];
                    break;
                case "west":
                    previousRoom = room2dArray[x - 1, z];
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            previousRoomScript = previousRoom.GetComponent<Room>();
            previousRoomScript.GetLocation(out previousRoomX, out previousRoomZ);
        }

        // Find a possible neighbor and recurse.
        if(x == 0 || (previousRoomX == x - 1))
        {
            roomScript.RemovePossibleDirection("west");
        }
        if(x == MazeWidth - 1 || (previousRoomX == x + 1))
        {
            roomScript.RemovePossibleDirection("east");
        }
        if(z == 0 || (previousRoomZ == z - 1))
        {
            roomScript.RemovePossibleDirection("south");
        }
        if(z == MazeDepth - 1 || (previousRoomZ == z + 1))
        {
            roomScript.RemovePossibleDirection("north");
        }

        List<string> availableDirections = roomScript.AvailableDirections();

        while(availableDirections.Count > 0)
        {
            var directionIndex = Random.Range(0, availableDirections.Count);
            var direction = availableDirections[directionIndex];

            int newX = x;
            int newZ = z;

            switch (direction)
            {
                case "north":
                    newZ += 1;
                    break;
                case "east":
                    newX += 1;
                    break;
                case "south":
                    newZ -= 1;
                    break;
                case "west":
                    newX -= 1;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            if(room2dArray[newX, newZ] == null)
            {
                roomScript.OpenDirection(direction);
                CreateRoom(newX, newZ, direction);
            }

            roomScript.RemovePossibleDirection(direction);
            availableDirections = roomScript.AvailableDirections();
        }

        return;

    }

    private Transform RoomRecursion(Transform currentRoom)
    {
        currentRoom.gameObject.SetActive(true);

        var currentRoomScript = currentRoom.GetComponent<Room>();

        currentRoomScript.visited = true;

        var availableDirections = currentRoomScript.AvailableDirections();

        if(availableDirections.Count > 0)
        {
            bool foundNeighbor = false;
            var directionIndex = Random.Range(0, availableDirections.Count);
            var direction = availableDirections[directionIndex];
            var directionOppositIndex = (directionIndex + 2) % 4;
            var directionOpposite = AllDirections[directionOppositIndex];
            var neighborRoom = GetNeighborRoom(currentRoomScript, direction);
            var neighborRoomScript = neighborRoom.GetComponent<Room>();

            while(foundNeighbor == false && availableDirections.Count > 0)
            {
                if (neighborRoomScript.visited == false)
                {
                    foundNeighbor = true;
                    currentRoomScript.OpenDirection(direction, neighborRoom);
                    neighborRoomScript.OpenDirection(directionOpposite, currentRoom);
                }
                else
                {
                    currentRoomScript.RemovePossibleDirection(direction);
                    availableDirections = currentRoomScript.AvailableDirections();
                }
            }

            // Recurse with the neighbor room.
            if(foundNeighbor)
            {
                RoomRecursion(neighborRoom);
            }
        }

        return currentRoom;
    }

    private Transform GetNeighborRoom(Room currentRoomScript, string direction)
    {
        Transform neighborRoom;
        int roomX, roomZ;

        currentRoomScript.GetLocation(out roomX, out roomZ);

        switch(direction)
        {
            case "north":
                neighborRoom = room2dArray[roomX, roomZ + 1];
                break;
            case "east":
                neighborRoom = room2dArray[roomX + 1, roomZ];
                break;
            case "south":
                neighborRoom = room2dArray[roomX, roomZ - 1];
                break;
            case "west":
                neighborRoom = room2dArray[roomX - 1, roomZ];
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        return neighborRoom;
    }
}
