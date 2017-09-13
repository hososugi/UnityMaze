using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int MazeWidth = 1;
    public int MazeDepth = 1;
    public Transform RoomObject;
    public Transform startRoom;
    public Transform roomContainer;
    public Transform player;
    public List<string> AllDirections = new List<string> { "north", "east", "south", "west" };

    private int roomWidth = 4;
    private int roomDepth = 4;
    private int MazeStartX;
    private int MazeStartZ;
    private Transform[,] room2dArray;

	// Use this for initialization
	void Start () {
        MazeStartX = (int) -System.Math.Floor((double) (MazeWidth / 2)) * roomWidth;
        MazeStartZ = (int) System.Math.Floor((double) (MazeDepth / 2)) * roomDepth;

        room2dArray = new Transform[MazeWidth, MazeDepth];
        roomContainer = GameObject.Find("RoomContainer").transform;
        
        CreateMaze();
    }

    public void CreateMaze()
    {
        int startX = Random.Range(0,2) * (MazeWidth-1);
        int startZ = Random.Range(0,2) * (MazeDepth-1);

        Debug.Log("Map start room: " + startX + "," + startZ);

        // Loop through all the rooms until every room has been visited and confirmed finished.
        Transform startRoom = CreateRoom(startX, startZ);

        // Set the location of the main camera.
        var camera = GameObject.Find("Main Camera").transform;
        camera.position = new Vector3(startRoom.transform.position.x, camera.position.y, startRoom.transform.position.z);

        var player = GameObject.Find("Player").transform;
        player.position = new Vector3(startRoom.transform.position.x, player.position.y, startRoom.transform.position.z);


        Debug.Log("Went through all the rooms.");       
    }

    private Transform CreateRoom(int x, int z, string movedDirection = null)
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

        return room;

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
